Imports System.IO.Ports
Imports System.Text
Imports System.Threading

Public Class SerialTerminal2

  Private WithEvents m_serialPort As SerialPort
  Private m_receiveBuffer As New StringBuilder()
  Private m_echoEnabled As Boolean

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
          Case vbCr
            Console.Write(vbLf)
          Case vbLf
            Stop
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
    While True
      Thread.Sleep(50)
      SyncLock m_receiveBuffer
        If m_receiveBuffer.Length > 0 Then
          Dim buffer = m_receiveBuffer.ToString()
          If buffer.Contains(vbCr) Then buffer = buffer.Replace(vbCr, vbCrLf)
          Console.Write(buffer)
          m_receiveBuffer.Clear()
        End If
      End SyncLock
    End While
  End Sub

End Class
