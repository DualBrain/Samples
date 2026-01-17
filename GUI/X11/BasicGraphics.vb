Module BasicGraphics

  Private CurrentColor As Integer = &HFFFFFFFF

  Public Sub SCREEN(w As Integer, h As Integer)
    FrameBuffer.Init(w, h)
  End Sub

  Public Sub COLOR(rgb As Integer)
    CurrentColor = rgb Or &HFF000000
  End Sub

  Public Sub CLS(Optional rgb As Integer = &H0)
    FrameBuffer.Clear(rgb Or &HFF000000)
  End Sub

  Public Sub PSET(x As Integer, y As Integer)
    FrameBuffer.SetPixel(x, y, CurrentColor)
  End Sub

  Public Sub LINE(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer)
    Dim dx = Math.Abs(x2 - x1)
    Dim dy = Math.Abs(y2 - y1)
    Dim sx = If(x1 < x2, 1, -1)
    Dim sy = If(y1 < y2, 1, -1)
    Dim err = dx - dy

    While True
      PSET(x1, y1)
      If x1 = x2 AndAlso y1 = y2 Then Exit While
      Dim e2 = err * 2
      If e2 > -dy Then err -= dy : x1 += sx
      If e2 < dx Then err += dx : y1 += sy
    End While
  End Sub

  Public Sub CIRCLE(cx As Integer, cy As Integer, r As Integer)
    Dim x = r
    Dim y = 0
    Dim err = 0

    While x >= y
      PSET(cx + x, cy + y)
      PSET(cx + y, cy + x)
      PSET(cx - y, cy + x)
      PSET(cx - x, cy + y)
      PSET(cx - x, cy - y)
      PSET(cx - y, cy - x)
      PSET(cx + y, cy - x)
      PSET(cx + x, cy - y)

      y += 1
      err += 1 + 2 * y
      If 2 * (err - x) + 1 > 0 Then
        x -= 1
        err += 1 - 2 * x
      End If
    End While
  End Sub

End Module
