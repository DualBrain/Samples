Module FrameBuffer

  Public Width As Integer
  Public Height As Integer
  Public Pixels() As Integer

  Public Sub Init(w As Integer, h As Integer)
    Width = w
    Height = h
    ReDim Pixels(w * h - 1)
  End Sub

  Public Sub Clear(color As Integer)
    For i = 0 To Pixels.Length - 1
      Pixels(i) = color
    Next
  End Sub

  Public Sub SetPixel(x As Integer, y As Integer, color As Integer)
    If x < 0 OrElse y < 0 OrElse x >= Width OrElse y >= Height Then Return
    Pixels(y * Width + x) = color
  End Sub

  Public Function GetPixel(x As Integer, y As Integer) As Integer
    If x < 0 OrElse y < 0 OrElse x >= Width OrElse y >= Height Then Return &HFF000000
    Return Pixels(y * Width + x)
  End Function

End Module
