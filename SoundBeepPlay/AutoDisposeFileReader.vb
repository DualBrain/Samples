Option Explicit On
Option Infer On
Option Strict On

Imports NAudio.Wave

Class AutoDisposeFileReader
  Implements ISampleProvider

  Private ReadOnly m_reader As AudioFileReader
  Private m_isDisposed As Boolean

  Public Sub New(reader As AudioFileReader)
    m_reader = reader
    WaveFormat = reader.WaveFormat
  End Sub

  Public Function Read(buffer As Single(), offset As Integer, count As Integer) As Integer Implements ISampleProvider.Read
    If m_isDisposed Then
      Return 0
    End If
    Dim r = m_reader.Read(buffer, offset, count)
    If r = 0 Then
      m_reader.Dispose()
      m_isDisposed = True
    End If
    Return r
  End Function

  Public Property WaveFormat As WaveFormat Implements ISampleProvider.WaveFormat

End Class