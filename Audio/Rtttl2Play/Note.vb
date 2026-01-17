Public Class Note

  Public Property Duration As Integer?
  Public Property Dot As Boolean = False
  Public Property Pitch As Integer = m_rtttlPitches.IndexOf("p"c)
  Public Property Octave As Integer?

  Public Overrides Function ToString() As String
    Dim s = ""
    If Duration.HasValue Then s += Duration.Value.ToString()
    s += m_rtttlPitches(Pitch)
    If Octave.HasValue Then s += Octave.Value.ToString()
    If Dot Then s += "."
    Return s
  End Function

End Class