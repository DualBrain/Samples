Imports System.IO.Ports
Imports System.Threading

Module Program

  Sub Main() 'args As String())

    'Example1()
    'Example2()
    Example3()

  End Sub

  Sub Example1()
    Dim terminal As New SerialTerminal1("COM5", 1200, Parity.None, 8, StopBits.One)
    terminal.Open()
    Console.WriteLine("Serial Port Open. Type to send data:")
    While True
      Dim input As String = Console.ReadLine()
      If input.ToLower() = "exit" Then Exit While
      terminal.SendData(input & vbCrLf)
    End While
    terminal.Close()
    Console.WriteLine("Serial Port Closed.")
  End Sub

  Sub Example2()

    Dim echo = True
    Dim terminal As New SerialTerminal2("COM5", 1200, Parity.None, 8, StopBits.One, echo)
    terminal.Open()

    Dim receiveThread As New Thread(AddressOf terminal.ProcessReceivedData)
    receiveThread.IsBackground = True
    receiveThread.Start()

    Do
      'Dim input As String = Console.ReadLine()
      'If input.ToLower() = "exit" Then Exit While
      'terminal.SendData(input & vbCrLf)
      If Console.KeyAvailable Then
        Dim keyInfo = Console.ReadKey(True)
        If keyInfo.Key = ConsoleKey.Escape Then Exit Do
        terminal.SendData(keyInfo.KeyChar)
      End If
    Loop

    terminal.Close()

  End Sub

  Sub Example3()

    Dim echo = True
    Dim terminal As New SerialTerminal3("COM5", 1200, Parity.None, 8, StopBits.One, echo)
    terminal.Open()

    Dim receiveThread As New Thread(AddressOf terminal.ProcessReceivedData)
    receiveThread.IsBackground = True
    receiveThread.Start()

    Do
      If Console.KeyAvailable Then
        Dim keyInfo = Console.ReadKey(True)
        If keyInfo.Key = ConsoleKey.Escape Then Exit Do
        terminal.SendData(keyInfo.KeyChar)
      End If
    Loop

    terminal.Close()

  End Sub

End Module
