Public Class Melody

  Public Property Name As String = ""
  Public Property Duration As Integer = 4
  Public Property Octave As Integer = 5
  Public Property BPM As Integer = 120
  Public Property Notes As New List(Of Note)

  Public Overrides Function ToString() As String
    Return $"RTTTL[{Name}: <d:{Duration} o:{Octave} b:{BPM}> <{String.Join(", ", Notes)}>]"
  End Function

End Class