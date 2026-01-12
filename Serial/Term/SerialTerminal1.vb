Imports System.IO.Ports
Imports System.Text

Public Class SerialTerminal1

  Private WithEvents m_serialPort As SerialPort

  Public Sub New(portName As String, baudRate As Integer, parity As Parity, dataBits As Integer, stopBits As StopBits)
    m_serialPort = New SerialPort(portName, baudRate, parity, dataBits, stopBits)
    AddHandler m_serialPort.DataReceived, AddressOf SerialDataReceived
  End Sub

  Public Sub Open()
    If Not m_serialPort.IsOpen Then
      m_serialPort.Open()
    End If
  End Sub

  Public Sub Close()
    If m_serialPort.IsOpen Then
      m_serialPort.Close()
    End If
  End Sub

  Public Sub SendData(data As String)
    If m_serialPort.IsOpen Then
      m_serialPort.Write(data)
    End If
  End Sub

  Private Sub SerialDataReceived(sender As Object, e As SerialDataReceivedEventArgs)
    Dim receivedData As String = m_serialPort.ReadExisting()
    Console.WriteLine("Received: " & receivedData)
  End Sub

End Class