Imports System.IO.Ports
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Public Class SerialTerminal3

  Private WithEvents m_serialPort As SerialPort
  Private m_receiveBuffer As New StringBuilder()
  Private m_echoEnabled As Boolean
  Private m_ansiBuffer As New StringBuilder()
  Private ReadOnly m_ansiRegex As New Regex("\x1B\[(\d*;?\d*)?([A-Za-z])", RegexOptions.Compiled)

  Public Sub New(portName As String, baudRate As Integer, parity As Parity, dataBits As Integer, stopBits As StopBits, echo As Boolean)
    m_serialPort = New SerialPort(portName, baudRate, parity, dataBits, stopBits)
    m_echoEnabled = echo
    m_serialPort.Encoding = Encoding.ASCII
    AddHandler m_serialPort.DataReceived, AddressOf SerialDataReceived
  End Sub

  Public Sub Open()
    If Not m_serialPort.IsOpen Then
      m_serialPort.Open()
      Console.Clear()
      Console.WriteLine("ANSI Terminal Ready (Connected to " & m_serialPort.PortName & ")")
    End If
  End Sub

  Public Sub Close()
    If m_serialPort.IsOpen Then
      m_serialPort.Close()
      Console.WriteLine("Connection Closed.")
    End If
  End Sub

  Public Sub SendData(data As Char)
    If m_serialPort.IsOpen Then
      m_serialPort.Write(data)
      If m_echoEnabled Then
        Console.Write(data)
        Select Case data
          Case vbCr : Console.Write(vbLf)
          Case Else
        End Select
      End If
    End If
  End Sub

  Private Sub SerialDataReceived(sender As Object, e As SerialDataReceivedEventArgs)
    Dim receivedData As String = m_serialPort.ReadExisting()
    SyncLock m_receiveBuffer
      m_receiveBuffer.Append(receivedData)
    End SyncLock
  End Sub

  Public Sub ProcessReceivedData()
    Do
      Thread.Sleep(50)
      SyncLock m_receiveBuffer
        If m_receiveBuffer.Length > 0 Then
          Dim buffer = m_receiveBuffer.ToString()
          If buffer.Contains(vbCr) Then buffer = buffer.Replace(vbCr, vbCrLf)
          'Console.Write(buffer)
          m_receiveBuffer.Clear()
          ProcessAnsiSequences(buffer)
        End If
      End SyncLock
    Loop
  End Sub

  Private Sub ProcessAnsiSequences(data As String)

    m_ansiBuffer.Append(data)
    Dim match As Match = m_ansiRegex.Match(m_ansiBuffer.ToString())

    While match.Success
      ApplyAnsiCode(match.Groups(1).Value, match.Groups(2).Value)
      m_ansiBuffer.Remove(match.Index, match.Length)
      match = m_ansiRegex.Match(m_ansiBuffer.ToString())
    End While

    ' Print remaining non-ANSI text
    Console.Write(m_ansiBuffer.ToString())
    m_ansiBuffer.Clear()

  End Sub

  Private Sub ApplyAnsiCode(parameters As String, command As String)

    Dim args As Integer() = parameters.Split(";"c) _
                            .Where(Function(p) Not String.IsNullOrEmpty(p)) _
                            .Select(Function(p) Integer.Parse(p)) _
                            .ToArray()

    Select Case command
      Case "H" ' Cursor Position
        Dim row As Integer = If(args.Length > 0, args(0), 1)
        Dim col As Integer = If(args.Length > 1, args(1), 1)
        Console.SetCursorPosition(Math.Max(0, col - 1), Math.Max(0, row - 1))
      Case "J" ' Clear Screen
        Console.Clear()
      Case "K" ' Clear Line
        Console.Write(vbCr & New String(" ", Console.WindowWidth) & vbCr)
      Case "m" ' Text Attributes (e.g., color, bold)
        If args.Length = 0 Then Console.ResetColor() Else ApplyTextAttributes(args)
      Case "A" ' Cursor Up
        Console.CursorTop = Math.Max(0, Console.CursorTop - If(args.Length > 0, args(0), 1))
      Case "B" ' Cursor Down
        Console.CursorTop = Math.Min(Console.WindowHeight - 1, Console.CursorTop + If(args.Length > 0, args(0), 1))
      Case "C" ' Cursor Forward
        Console.CursorLeft = Math.Min(Console.WindowWidth - 1, Console.CursorLeft + If(args.Length > 0, args(0), 1))
      Case "D" ' Cursor Back
        Console.CursorLeft = Math.Max(0, Console.CursorLeft - If(args.Length > 0, args(0), 1))
      Case "s" ' Save Cursor Position
        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop)
      Case "u" ' Restore Cursor Position
        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop)
      Case "S" ' Scroll Up
        Console.WriteLine(New String(vbLf, If(args.Length > 0, args(0), 1)))
      Case "T" ' Scroll Down
        Console.WriteLine(New String(vbCr, If(args.Length > 0, args(0), 1)))
      Case "?25h" ' Show Cursor
        Console.CursorVisible = True
      Case "?25l" ' Hide Cursor
        Console.CursorVisible = False
    End Select
  End Sub

  Private Sub ApplyTextAttributes(args As Integer())
    For Each code In args
      Select Case code
        Case 0 : Console.ResetColor()
        Case 1 : Console.ForegroundColor = ConsoleColor.White ' Bold
        Case 4 : Console.ForegroundColor = ConsoleColor.Gray ' Underline (approximation)
        Case 30 To 37 : Console.ForegroundColor = CType(code - 30, ConsoleColor)
        Case 40 To 47 : Console.BackgroundColor = CType(code - 40, ConsoleColor)
      End Select
    Next
  End Sub

End Class
