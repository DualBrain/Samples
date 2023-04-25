Option Explicit On
Option Strict On
Option Infer On

Public Class Bus

  Public cpu As New olc6502
  Public ram(64 * 1024 - 1) As Byte

  Public Sub New()
    cpu.ConnectBus(Me)
    For i = 0 To ram.Length - 1
      ram(i) = &H0
    Next
  End Sub

  Public Sub Write(addr As UShort, data As Byte)
    If addr >= &H0 AndAlso addr <= &HFFFF Then
      ram(addr) = data
    End If
  End Sub

  Public Function Read(addr As UShort, Optional bReadOnly As Boolean = False) As Byte
    If addr >= &H0 AndAlso addr <= &HFFFF Then
      Return ram(addr)
    End If
    Return &H0
  End Function

End Class