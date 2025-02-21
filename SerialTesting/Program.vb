Imports System.IO.Ports
Imports System.Threading.Thread

Module Program

  Private WithEvents Device As SerialPort

  Const POLY = &H1021
  Private ReadOnly m_crct(256) As Integer

  'Private Enum TransferState
  '  WaitingStart
  '  Receive
  'End Enum

  Sub Main() 'args As String())

    Dim totalSectors = &H3200
    Dim dataSize = 400

    Dim speed = 19200
    Dim port = "COM5"
    'Dim settings = "N,8,1"
    Dim cts = True

    '' query serial ports that are available...
    'For Each portName In SerialPort.GetPortNames
    '  Console.WriteLine(portName)
    'Next

    Dim OK = CByte(&HFF)
    Dim FAILED = CByte(&H0)
    Dim retries = 10
    Dim current = 0

    Device = New SerialPort()

    Try

      Try

        Console.WriteLine("Initializing...")

        Device.PortName = port
        Device.BaudRate = speed
        Device.Parity = Parity.None
        Device.DataBits = 8
        Device.StopBits = StopBits.One
        Device.Handshake = If(cts, Handshake.RequestToSend, Handshake.None)

        ' Set the read/write timeouts
        Device.ReadTimeout = 1000
        Device.WriteTimeout = 1000
      Catch ex As Exception
        Console.WriteLine($"Error initializing port...{vbCrLf}{ex.Message}")
        Return
      End Try

      Try

        Console.WriteLine("Opening...")

        Device.Open()
        Dim count = 25
        While Not Device.IsOpen : Sleep(100) : count -= 1 : End While
        If count > 0 Then
          Console.WriteLine($"Session Started: {Now:yyyy-MM-dd}")
          Console.WriteLine($"{Device.PortName} {Device.BaudRate}, N81, Handshake: {If(cts, "CTS/RTS", "NONE")}")
          Console.WriteLine($"ReadTimeout: {Device.ReadTimeout}ms, WriteTimeout: {Device.WriteTimeout}ms, retries: {retries}")
        Else
          Console.WriteLine("Failed to initalize.")
          Return
        End If
      Catch ex As Exception
        Console.WriteLine($"Error opening port...{vbCrLf}{ex.Message}")
        Return
      End Try

      Dim receiveBuffer(513) As Byte
      Dim sector = 0
      'Dim state = TransferState.WaitingStart
      Dim waiting = True
      Dim offset = 0
      Dim crc = -1

      Do

        If Console.KeyAvailable Then
          Dim k = Console.ReadKey(True)
          Select Case k.Key
            Case ConsoleKey.Escape
              Exit Do
            Case Else
          End Select
        End If

        If Device.BytesToRead > 0 Then

          Dim b(0) As Byte : Dim r = Device.Read(b, 0, 1)

          If r = 1 Then

            'Select Case state
            'Case TransferState.WaitingStart
            If waiting Then

              Console.Write(".")

              If b(0) = OK Then
                Device.Write({OK}, 0, 1)
                'state = TransferState.Receive
                waiting = False
              Else
                Console.Write("?"c)
              End If

            Else 'Case TransferState.Receive
              If offset = 0 Then Console.WriteLine($"Sector: {sector}")
              Console.Write("+")
              receiveBuffer(offset) = b(0)
              If offset < dataSize - 1 Then
                offset += 1
              ElseIf offset = dataSize - 1 Then
                offset += 1
                crc = CalcCrc(receiveBuffer, 400)
              ElseIf offset < dataSize + 2 Then
                offset += 1
                If offset = dataSize + 2 Then

                  Console.WriteLine("#")

                  If Device.BytesToRead > 0 Then
                    Console.WriteLine($"UNEXPECTED: {Device.BytesToRead} more in the buffer...")
                    Return
                  End If

                  Dim b1 = (crc And &HFF00) >> 8 ' high byte
                  Dim b2 = (crc And &HFF) ' low byte

                  If receiveBuffer(dataSize) = b1 AndAlso
                       receiveBuffer(dataSize + 1) = b2 Then
                    ' good
                    sector += 1 : offset = 0 : crc = -1
                    Device.Write({OK}, 0, 1) ': Sleep(100)
                  Else
                    ' bad
                    offset = 0 : crc = -1
                    Device.Write({FAILED}, 0, 1) ': Sleep(100)
                  End If
                  Device.Write({OK}, 0, 1) ': Sleep(100)
                  'state = TransferState.Receive
                  waiting = False

                  If sector = totalSectors Then
                    Console.WriteLine("Completed.")
                    Return
                  End If

                End If
              Else
                Stop
              End If
              'Case TransferState.WaitReceiveResults
              '  Console.WriteLine("Sending OK...")
              '  buffer(0) = OK
              '  Device.Write(buffer, 0, 1)
              '  state = TransferState.WaitReadyForNext

              'Case TransferState.WaitReadyForNext
              '  Console.WriteLine("WaitReadyForNext")

              '  If buffer(0) = OK Then
              '    Device.Write(buffer, 0, 1)
              '    state = TransferState.Receive
              '  Else
              '    Console.Write("?"c)
              '  End If

            End If

            'Case Else
            '  Stop

            '  End Select

          End If

          End If

      Loop

    Finally
      If Device?.IsOpen Then
        Console.WriteLine("Closing...")
        Device.DiscardOutBuffer()
        Device.DiscardInBuffer()
        Device.Close()
        Console.WriteLine("Closed.")
      Else
        Console.WriteLine("Port already closed.")
      End If
    End Try

  End Sub

  Private Sub CrcInit()
    For i = 0 To 256 - 1
      Dim v = i << 8
      For x = 0 To 8 - 1
        v = ((v << 1) Xor (If(((v And &H8000) <> 0), POLY, 0)))
      Next
      m_crct(i) = v And &HFFFF
    Next
  End Sub

  Private Function CrcAdd(crc As Integer, val As Byte) As Integer
    Return ((crc << 8) Xor m_crct(((crc >> 8) Xor val) And &HFF)) And &HFFFF
  End Function

  Private Function CalcCrc(d As Byte(), Optional length As Integer = -1) As Integer
    CrcInit()
    Dim c = 0
    If length = -1 Then length = d.Length
    For i = 0 To length - 1
      c = CrcAdd(c, d(i))
    Next
    Return c
  End Function

  Private Sub Device_ErrorReceived(sender As Object, e As SerialErrorReceivedEventArgs) Handles Device.ErrorReceived
    Console.WriteLine($"ERROR: {e.EventType}")
  End Sub

End Module