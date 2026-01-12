Imports System.IO
Imports System.IO.Ports
Imports System.Threading.Thread
Imports System.Windows

Friend Module Program

  Private WithEvents Device As SerialPort

  Private Const POLY = &H1021
  Private ReadOnly m_crct(256) As Integer

  Private Class Sector
    Public Data As Byte() = New Byte(399) {}
    Public Crc As Byte() = New Byte(1) {}
  End Class

  Private Data As New List(Of Sector)

  Private Enum TransferState
    Idle
    Ready
    WaitingStart
    Receive
    WaitReceiveResults
    WaitReadyForNext
    Abort
  End Enum

  Sub Main() 'args As String())

    Dim totalSectors = &H3200
    Dim dataSize = 400

    Dim speed = 19200
    Dim port = "COM4"
    'Dim settings = "N,8,1"
    Dim cts = True

    ' open file...
    Dim path = "c:\users\dualb\desktop\hawk.img"

    '' query serial ports that are available...
    'For Each portName In SerialPort.GetPortNames
    '  Console.WriteLine(portName)
    'Next

    Dim data As List(Of Sector) = Nothing
    If IsHawkDump(path) Then
      Console.WriteLine("Loading HawkDump...")
      data = LoadHawkDump(path)
    End If

    If data IsNot Nothing Then

      Dim OK = CByte(&HFF)
      Dim FAILED = CByte(&H0)
      Dim retries = 10

      Device = New SerialPort()

      Try

        ' initialize com port
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

        ' open com port
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

        ' ping with 0xFF
        Device.Write({OK}, 0, 1)

        ' wait for 0xFF
        Do
          If Console.KeyAvailable Then
            Dim k = Console.ReadKey(True)
            If k.Key = ConsoleKey.Escape Then Return
          End If
          If Device.BytesToRead > 0 Then
            Dim b(0) As Byte : Dim r = Device.Read(b, 0, 1)
            If b(0) = OK Then Console.Write("*") : Exit Do Else Console.Write("?")
          Else
            Console.Write(".") : Sleep(1000)
          End If
        Loop

        Dim sector = 0
        Do
          ' check for escape key...
          If Console.KeyAvailable Then
            Dim k = Console.ReadKey(True)
            If k.Key = ConsoleKey.Escape Then Return
          End If
          ' send 402 chunk
          Device.Write(data(sector).Data, 0, data(sector).Data.Length)
          Device.Write(data(sector).Crc, 0, data(sector).Crc.Length)
          ' wait for FF (good) Or 00 (bad)
          Do
            If Device.BytesToRead > 0 Then
              Dim b(0) As Byte : Dim r = Device.Read(b, 0, 1)
              ' resend if 00, send next if FF
              If b(0) = OK Then
                ' load next chunk if under 0x3200
                sector += 1
                If sector > &H3200 Then
                  Console.WriteLine("#")
                  Return
                Else
                  Console.WriteLine("+")
                  Console.Write($"({sector + 1} of {data.Count})")
                  Exit Do
                End If
              ElseIf b(0) = FAILED Then
                Console.Write("-")
                Exit Do
              Else
                Console.Write("?")
              End If
            Else
              Console.Write(".")
            End If
          Loop

          ' wait for FF
          Do
            If Console.KeyAvailable Then
              Dim k = Console.ReadKey(True)
              If k.Key = ConsoleKey.Escape Then Return
            End If
            If Device.BytesToRead > 0 Then
              Dim b(0) As Byte : Dim r = Device.Read(b, 0, 1)
              If b(0) = OK Then Console.Write("*") : Exit Do Else Console.Write("?")
            Else
              Console.Write(".")
            End If
          Loop

        Loop

      Finally
        ' close com port
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

    End If

  End Sub

  Private Function IsHawkDump(path As String) As Boolean
    If Not System.IO.File.Exists(path) Then Return False
    Using dump = System.IO.File.OpenRead(path)
      Dim header = New Byte(7) {}
      dump.ReadExactly(header, 0, 8)
      Dim marker = Text.Encoding.ASCII.GetString(header)
      Return marker = "HawkDump"
    End Using
  End Function

  Private Function IsFinchDump(path As String) As Boolean
    Using dump = System.IO.File.OpenRead(path)
      Dim header = New Byte(8) {}
      dump.ReadExactly(header, 0, 8)
      Dim marker = Text.Encoding.ASCII.GetString(header)
      Return marker = "FINCHDUMP"
    End Using
  End Function

  Private Function LoadHawkDump(path As String) As List(Of Sector)

    Const ignoreCrc = False
    Const blockLength = 416

    Using dump = System.IO.File.OpenRead(path)

      Console.WriteLine("File format: HawkDump")

      ' 416 bytes per sector block
      ' "HawkDump\r\n" 10b
      ' address 2b
      ' data 400b  
      ' crc 2b
      ' \r\n 2b

      Dim t = dump.Length
      If t Mod blockLength <> 0 Then
        Console.WriteLine($"File size mismatch: {t / blockLength:F8} blocks!") : Return Nothing
      End If

      Dim data = New List(Of Sector)
      Dim s = 0

      While (s + 1) * blockLength <= dump.Length
        Dim header = New Byte(7) {}
        dump.ReadExactly(header, 0, 8)
        For i = 0 To 8 - 1
          header(i) = CByte((header(i) And &H7F))
        Next
        Dim marker = Text.Encoding.ASCII.GetString(header)
        If marker <> "HawkDump" Then Console.WriteLine($"Missing ""HawkDump"" header at sector 0x{s:X4}!") : Return Nothing
        dump.Seek(2 + 2, SeekOrigin.Current) ' CRLF sectorAddress
        Dim ns = New Sector
        dump.ReadExactly(ns.Data, 0, 400) ' sector data
        Dim dataCRC = CalcCrc(ns.Data)
        ns.Crc(0) = CByte(((dataCRC And &HFF00) >> 8)) ' high byte
        ns.Crc(1) = CByte((dataCRC And &HFF)) ' low byte
        Dim crcH = dump.ReadByte() ' crc high byte
        Dim crcL = dump.ReadByte() ' crc low byte
        If Not (crcL = &HFF AndAlso crcH = &HFF) AndAlso
           Not ignoreCrc AndAlso
           (crcL <> ns.Crc(1) OrElse crcH <> ns.Crc(0)) Then
          Console.WriteLine($"CRC mismatch at sector 0x{s:X4}.")
          Console.WriteLine($"Sector CRC: 0x{ByteToHex(ns.Crc(0)) + ByteToHex(ns.Crc(1))}.")
          Console.WriteLine($"File CRC: 0x{ByteToHex(CByte(crcH)) + ByteToHex(CByte(crcL))}.")
        End If
        data.Add(ns)
        s += 1
        dump.Seek(2, SeekOrigin.Current) ' CRLF
      End While
      Console.WriteLine($"Loaded {s} sectors.")
      Return data
    End Using
  End Function

  Private Sub PrepareBuffers(path As String)

    Dim ignoreCrc = False

    Using fileInput = IO.File.OpenRead(path)

      Dim header = New Byte(7) {}
      fileInput.ReadExactly(header, 0, 8)
      fileInput.Seek(0, SeekOrigin.Begin)
      'For i = 0 To 8 - 1
      '  header(i) = CByte((header(i) And &H7F))
      'Next
      Dim marker = Text.Encoding.ASCII.GetString(header)
      'marker = marker.ToUpperInvariant()

      If marker = "HAWKDUMP" Then

#Region "HAWKDUMP"

        Console.WriteLine("File format: HawkDump")
        'fileInput.Seek(0, SeekOrigin.Begin)
        ' 416 bytes per sector block
        ' "HawkDump\r\n" 10b
        ' address 2b
        ' data 400b  
        ' crc 2b
        ' \r\n 2b
        Const blockLength = 416

        Dim t = fileInput.Length
        If t Mod blockLength <> 0 Then Console.WriteLine($"File size mismatch: {t / blockLength:F8} blocks?") : Return

        'fileInput.Seek(0, SeekOrigin.Begin)

        Data = New List(Of Sector)
        Dim s = 0

        While ((s + 1) * blockLength) <= fileInput.Length
          fileInput.ReadExactly(header, 0, 8)
          For i = 0 To 8 - 1
            header(i) = CByte((header(i) And &H7F))
          Next
          marker = Text.Encoding.ASCII.GetString(header)
          marker = marker.ToUpperInvariant()
          If marker <> "HAWKDUMP" Then
            Console.WriteLine("Missing ""HawkDump"" header at sector 0x" & s.ToString("X4"))
            Return
          End If
          fileInput.Seek(2 + 2, SeekOrigin.Current) ' \r\n sectorAddress

          Dim ns As New Sector
          '    ns.address = s;
          fileInput.ReadExactly(ns.Data, 0, 400) ' sector data
          Dim dataCRC = CalcCrc(ns.Data)
          ns.Crc(0) = CByte(((dataCRC And &HFF00) >> 8)) ' high byte
          ns.Crc(1) = CByte((dataCRC And &HFF)) ' low byte

          Dim crcH = fileInput.ReadByte() ' crc high byte
          Dim crcL = fileInput.ReadByte() ' crc low byte

          If (crcL = &HFF) AndAlso (crcH = &HFF) Then

          Else
            If (Not ignoreCrc) AndAlso ((crcL <> ns.Crc(1)) OrElse (crcH <> ns.Crc(0))) Then
              Console.WriteLine("CRC mismatch at sector 0x" & s.ToString("X4"))
              Console.WriteLine("Sector CRC: 0x" & ByteToHex(ns.Crc(0)) + ByteToHex(ns.Crc(1)))
              Console.WriteLine("File CRC: 0x" & ByteToHex(CByte(crcH)) + ByteToHex(CByte(crcL)))
            End If
            ' no crc
          End If
          Data.Add(ns)
          s += 1
          fileInput.Seek(2, SeekOrigin.Current) ' \r\n
        End While
        'currentState = state.Ready
        'sectorNumber = 0
        Console.WriteLine("Loaded " & s & " sectors")
#End Region

      ElseIf marker = "FINCHDUM" Then

#Region "FINCHDUMP"

        Console.WriteLine("File format: FinchDump")
        fileInput.Seek(0, SeekOrigin.Begin)
        ' 419 per sector block
        ' "FinchDump\r\n" 11b
        ' address 4b
        ' data 400b  
        ' crc 2b
        ' \r\n 2b
        Const blockLength As Integer = 419

        Dim t = fileInput.Length
        If (t Mod blockLength) <> 0 Then
          ' finch2.bin total 30 469 061
          Console.WriteLine("File size mismatch: " & (CSng(t) / CSng(blockLength)).ToString("F8") & " blocks?")
          '   return;
        End If
        fileInput.Seek(0, SeekOrigin.Begin)

        Data = New List(Of Sector)
        Dim s As Integer = 0

        While ((s + 1) * blockLength) <= fileInput.Length
          fileInput.ReadExactly(header, 0, 8)
          For i = 0 To 8 - 1
            header(i) = CByte((header(i) And &H7F))
          Next
          marker = Text.Encoding.ASCII.GetString(header)
          marker = marker.ToUpperInvariant()
          If marker <> "FINCHDUM" Then
            Console.WriteLine("Missing ""FinchDump"" header at sector 0x" & s.ToString("X4"))
            Return
          End If
          fileInput.Seek(3 + 4, SeekOrigin.Current) ' p\r\n sectorAddress

          Dim ns As New Sector
          '    ns.address = s;
          fileInput.ReadExactly(ns.Data, 0, 400) ' sector data
          Dim dataCRC = CalcCrc(ns.Data)
          ns.Crc(0) = CByte(((dataCRC And &HFF00) >> 8)) ' high byte
          ns.Crc(1) = CByte((dataCRC And &HFF)) ' low byte

          Dim crcH = fileInput.ReadByte() ' crc high byte
          Dim crcL = fileInput.ReadByte() ' crc low byte

          If (crcL = &HFF) AndAlso (crcH = &HFF) Then

          Else
            If (Not ignoreCrc) AndAlso ((crcL <> ns.Crc(1)) OrElse (crcH <> ns.Crc(0))) Then
              Console.WriteLine("CRC mismatch at sector 0x" & s.ToString("X4"))
              Console.WriteLine("Sector CRC: 0x" & ByteToHex(ns.Crc(0)) + ByteToHex(ns.Crc(1)))
              Console.WriteLine("File CRC: 0x" & ByteToHex(CByte(crcH)) + ByteToHex(CByte(crcL)))
            End If
            ' no crc
          End If
          Data.Add(ns)
          s += 1
          fileInput.Seek(2, SeekOrigin.Current) ' \r\n
        End While
        'currentState = state.Ready
        'sectorNumber = 0
        Console.WriteLine("Loaded " & s & " sectors")
#End Region

      Else

#Region "RAW512"

        ' HACKfix.img total 6 651 904, 512 bytes per blocks, 12 992 blocks
        ' CPU5fix.img total 6 651 904
        ' CRC 16 XMODEM CCITT
        Console.WriteLine("File format: RAW, 512 bytes blocks")
        ' 400 + padding = 512 per sector block
        Const blockLength = 512

        Dim t = fileInput.Length
        If t Mod blockLength <> 0 Then Console.WriteLine($"File size mismatch: {t / blockLength:F8} blocks?") : Return
        fileInput.Seek(0, SeekOrigin.Begin)

        Data = New List(Of Sector)
        Dim s = 0
        While (s + 1) * blockLength <= fileInput.Length
          Dim ns = New Sector
          fileInput.ReadExactly(ns.Data, 0, 400)
          Dim dataCRC = CalcCrc(ns.Data)
          ns.Crc(0) = CByte(((dataCRC And &HFF00) >> 8)) ' high byte
          ns.Crc(1) = CByte((dataCRC And &HFF)) ' low byte
          Dim crcH = fileInput.ReadByte()
          Dim crcL = fileInput.ReadByte()
          If Not (crcL = &HFF AndAlso crcH = &HFF) AndAlso
             Not ignoreCrc AndAlso
             (crcL <> ns.Crc(1) OrElse crcH <> ns.Crc(0)) Then
            Console.WriteLine($"CRC mismatch at sector 0x{s:X4}.")
            Console.WriteLine($"Sector CRC: 0x{ByteToHex(ns.Crc(0)) + ByteToHex(ns.Crc(1))}.")
            Console.WriteLine($"File CRC: 0x{ByteToHex(CByte(crcH)) + ByteToHex(CByte(crcL))}.")
          End If
          Data.Add(ns)
          s += 1
          fileInput.Seek(blockLength * s, SeekOrigin.Begin)
        End While
        Console.WriteLine($"Loaded {s} sectors.")

      End If

#End Region

    End Using

  End Sub

  Private Function ByteToHex(b As Byte) As String
    Return Convert.ToString(b, 16).PadLeft(2, "0"c).ToUpperInvariant()
  End Function

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