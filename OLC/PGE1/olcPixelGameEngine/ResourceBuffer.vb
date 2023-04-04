Option Explicit On
Option Strict On
Option Infer On

Public Class ResourceBuffer

  Friend vMemory As Byte()

  Public ReadOnly Property Data As Byte()
    Get
      Return vMemory
    End Get
  End Property

  Sub New(ifs As IO.FileStream, offset As Integer, size As Integer)
    vMemory = New Byte(size - 1) {}
    ifs.Seek(offset, IO.SeekOrigin.Begin)
    ifs.Read(vMemory, 0, vMemory.Length)
  End Sub

End Class
