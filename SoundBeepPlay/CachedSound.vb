Option Explicit On
Option Infer On
Option Strict On

Imports NAudio.Wave

Class CachedSound

  Public Property AudioData As Single()

  Public Property WaveFormat As WaveFormat

  Public Sub New(audioFileName As String)
    Using audioFileReader = New AudioFileReader(audioFileName)
      ' TODO: could add resampling in here if required
      WaveFormat = audioFileReader.WaveFormat
      Dim wholeFile = New List(Of Single)(CInt(Fix((audioFileReader.Length / 4))))
      Dim readBuffer = New Single(audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels - 1) {}
      Do
        Dim samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)
        If samplesRead > 0 Then
          wholeFile.AddRange(readBuffer.Take(samplesRead))
        Else
          Exit Do
        End If
      Loop
      AudioData = wholeFile.ToArray()
    End Using
  End Sub

End Class