Option Explicit On
Option Strict On
Option Infer On

Imports Olc.PixelGameEngine

' Container class for Advanced 2D Drawing functions
Public Class Gfx2D
  Inherits PgeX

  ' A representation of an affine transform, used to rotate, scale, offset & shear space
  Public Class Transform2D

    Private ReadOnly m_matrix(4, 3, 3) As Single
    Private m_targetMatrix As Integer
    Private m_sourceMatrix As Integer
    Private m_dirty As Boolean

    Public Sub New()
      Reset()
    End Sub

    ' Set this transformation to unity
    Public Sub Reset()

      m_targetMatrix = 0
      m_sourceMatrix = 1
      m_dirty = True

      ' Columns Then Rows

      ' Matrices 0 & 1 are used as swaps in Transform accumulation
      m_matrix(0, 0, 0) = 1.0F : m_matrix(0, 1, 0) = 0.0F : m_matrix(0, 2, 0) = 0.0F
      m_matrix(0, 0, 1) = 0.0F : m_matrix(0, 1, 1) = 1.0F : m_matrix(0, 2, 1) = 0.0F
      m_matrix(0, 0, 2) = 0.0F : m_matrix(0, 1, 2) = 0.0F : m_matrix(0, 2, 2) = 1.0F

      m_matrix(1, 0, 0) = 1.0F : m_matrix(1, 1, 0) = 0.0F : m_matrix(1, 2, 0) = 0.0F
      m_matrix(1, 0, 1) = 0.0F : m_matrix(1, 1, 1) = 1.0F : m_matrix(1, 2, 1) = 0.0F
      m_matrix(1, 0, 2) = 0.0F : m_matrix(1, 1, 2) = 0.0F : m_matrix(1, 2, 2) = 1.0F

      ' Matrix 2 is a cache matrix to hold the immediate transform operation
      ' Matrix 3 is a cache matrix to hold the inverted transform

    End Sub

    ' Append a rotation of fTheta radians to this transform
    Public Sub Rotate(theta As Single)
      ' Construct Rotation Matrix
      m_matrix(2, 0, 0) = MathF.Cos(theta) : m_matrix(2, 1, 0) = MathF.Sin(theta) : m_matrix(2, 2, 0) = 0.0F
      m_matrix(2, 0, 1) = -MathF.Sin(theta) : m_matrix(2, 1, 1) = MathF.Cos(theta) : m_matrix(2, 2, 1) = 0.0F
      m_matrix(2, 0, 2) = 0.0F : m_matrix(2, 1, 2) = 0.0F : m_matrix(2, 2, 2) = 1.0F
      Multiply()
    End Sub

    ' Append a translation (ox, oy) to this transform
    Public Sub Translate(ox As Single, oy As Single)
      ' Construct Translate Matrix
      m_matrix(2, 0, 0) = 1.0F : m_matrix(2, 1, 0) = 0.0F : m_matrix(2, 2, 0) = ox
      m_matrix(2, 0, 1) = 0.0F : m_matrix(2, 1, 1) = 1.0F : m_matrix(2, 2, 1) = oy
      m_matrix(2, 0, 2) = 0.0F : m_matrix(2, 1, 2) = 0.0F : m_matrix(2, 2, 2) = 1.0F
      Multiply()
    End Sub

    ' Append a scaling operation (sx, sy) to this transform
    Public Sub Scale(sx As Single, sy As Single)
      ' Construct Scale Matrix
      m_matrix(2, 0, 0) = sx : m_matrix(2, 1, 0) = 0.0F : m_matrix(2, 2, 0) = 0.0F
      m_matrix(2, 0, 1) = 0.0F : m_matrix(2, 1, 1) = sy : m_matrix(2, 2, 1) = 0.0F
      m_matrix(2, 0, 2) = 0.0F : m_matrix(2, 1, 2) = 0.0F : m_matrix(2, 2, 2) = 1.0F
      Multiply()
    End Sub

    ' Append a shear operation (sx, sy) to this transform
    Public Sub Shear(sx As Single, sy As Single)
      ' Construct Shear Matrix
      m_matrix(2, 0, 0) = 1.0F : m_matrix(2, 1, 0) = sx : m_matrix(2, 2, 0) = 0.0F
      m_matrix(2, 0, 1) = sy : m_matrix(2, 1, 1) = 1.0F : m_matrix(2, 2, 1) = 0.0F
      m_matrix(2, 0, 2) = 0.0F : m_matrix(2, 1, 2) = 0.0F : m_matrix(2, 2, 2) = 1.0F
      Multiply()
    End Sub

    Public Sub Perspective(ox As Single, oy As Single)
      ' Construct Translate Matrix
      m_matrix(2, 0, 0) = 1.0F : m_matrix(2, 1, 0) = 0.0F : m_matrix(2, 2, 0) = 0.0F
      m_matrix(2, 0, 1) = 0.0F : m_matrix(2, 1, 1) = 1.0F : m_matrix(2, 2, 1) = 0.0F
      m_matrix(2, 0, 2) = ox : m_matrix(2, 1, 2) = oy : m_matrix(2, 2, 2) = 1.0F
      Multiply()
    End Sub

    ' Calculate the Forward Transformation of the coordinate (in_x, in_y) -> (out_x, out_y)
    Public Sub Forward(in_x As Single, in_y As Single, ByRef out_x As Single, ByRef out_y As Single)
      out_x = in_x * m_matrix(m_sourceMatrix, 0, 0) + in_y * m_matrix(m_sourceMatrix, 1, 0) + m_matrix(m_sourceMatrix, 2, 0)
      out_y = in_x * m_matrix(m_sourceMatrix, 0, 1) + in_y * m_matrix(m_sourceMatrix, 1, 1) + m_matrix(m_sourceMatrix, 2, 1)
      Dim out_z = in_x * m_matrix(m_sourceMatrix, 0, 2) + in_y * m_matrix(m_sourceMatrix, 1, 2) + m_matrix(m_sourceMatrix, 2, 2)
      If out_z <> 0 Then
        out_x /= out_z
        out_y /= out_z
      End If
    End Sub

    ' Calculate the Inverse Transformation of the coordinate (in_x, in_y) -> (out_x, out_y)
    Public Sub Backward(in_x As Single, in_y As Single, ByRef out_x As Single, ByRef out_y As Single)
      out_x = in_x * m_matrix(3, 0, 0) + in_y * m_matrix(3, 1, 0) + m_matrix(3, 2, 0)
      out_y = in_x * m_matrix(3, 0, 1) + in_y * m_matrix(3, 1, 1) + m_matrix(3, 2, 1)
      Dim out_z = in_x * m_matrix(3, 0, 2) + in_y * m_matrix(3, 1, 2) + m_matrix(3, 2, 2)
      If out_z <> 0 Then
        out_x /= out_z
        out_y /= out_z
      End If
    End Sub

    ' Regenerate the Inverse Transformation
    Public Sub Invert()

      If m_dirty Then ' Obviously costly so only do if needed

        Dim det = m_matrix(m_sourceMatrix, 0, 0) * (m_matrix(m_sourceMatrix, 1, 1) * m_matrix(m_sourceMatrix, 2, 2) - m_matrix(m_sourceMatrix, 1, 2) * m_matrix(m_sourceMatrix, 2, 1)) -
                  m_matrix(m_sourceMatrix, 1, 0) * (m_matrix(m_sourceMatrix, 0, 1) * m_matrix(m_sourceMatrix, 2, 2) - m_matrix(m_sourceMatrix, 2, 1) * m_matrix(m_sourceMatrix, 0, 2)) +
                  m_matrix(m_sourceMatrix, 2, 0) * (m_matrix(m_sourceMatrix, 0, 1) * m_matrix(m_sourceMatrix, 1, 2) - m_matrix(m_sourceMatrix, 1, 1) * m_matrix(m_sourceMatrix, 0, 2))

        Dim idet = 1.0F / det
        m_matrix(3, 0, 0) = (m_matrix(m_sourceMatrix, 1, 1) * m_matrix(m_sourceMatrix, 2, 2) - m_matrix(m_sourceMatrix, 1, 2) * m_matrix(m_sourceMatrix, 2, 1)) * idet
        m_matrix(3, 1, 0) = (m_matrix(m_sourceMatrix, 2, 0) * m_matrix(m_sourceMatrix, 1, 2) - m_matrix(m_sourceMatrix, 1, 0) * m_matrix(m_sourceMatrix, 2, 2)) * idet
        m_matrix(3, 2, 0) = (m_matrix(m_sourceMatrix, 1, 0) * m_matrix(m_sourceMatrix, 2, 1) - m_matrix(m_sourceMatrix, 2, 0) * m_matrix(m_sourceMatrix, 1, 1)) * idet
        m_matrix(3, 0, 1) = (m_matrix(m_sourceMatrix, 2, 1) * m_matrix(m_sourceMatrix, 0, 2) - m_matrix(m_sourceMatrix, 0, 1) * m_matrix(m_sourceMatrix, 2, 2)) * idet
        m_matrix(3, 1, 1) = (m_matrix(m_sourceMatrix, 0, 0) * m_matrix(m_sourceMatrix, 2, 2) - m_matrix(m_sourceMatrix, 2, 0) * m_matrix(m_sourceMatrix, 0, 2)) * idet
        m_matrix(3, 2, 1) = (m_matrix(m_sourceMatrix, 0, 1) * m_matrix(m_sourceMatrix, 2, 0) - m_matrix(m_sourceMatrix, 0, 0) * m_matrix(m_sourceMatrix, 2, 1)) * idet
        m_matrix(3, 0, 2) = (m_matrix(m_sourceMatrix, 0, 1) * m_matrix(m_sourceMatrix, 1, 2) - m_matrix(m_sourceMatrix, 0, 2) * m_matrix(m_sourceMatrix, 1, 1)) * idet
        m_matrix(3, 1, 2) = (m_matrix(m_sourceMatrix, 0, 2) * m_matrix(m_sourceMatrix, 1, 0) - m_matrix(m_sourceMatrix, 0, 0) * m_matrix(m_sourceMatrix, 1, 2)) * idet
        m_matrix(3, 2, 2) = (m_matrix(m_sourceMatrix, 0, 0) * m_matrix(m_sourceMatrix, 1, 1) - m_matrix(m_sourceMatrix, 0, 1) * m_matrix(m_sourceMatrix, 1, 0)) * idet

        m_dirty = False

      End If

    End Sub

    Private Sub Multiply()

      For c = 0 To 2
        For r = 0 To 2
          m_matrix(m_targetMatrix, c, r) = m_matrix(2, 0, r) * m_matrix(m_sourceMatrix, c, 0) +
                                        m_matrix(2, 1, r) * m_matrix(m_sourceMatrix, c, 1) +
                                        m_matrix(2, 2, r) * m_matrix(m_sourceMatrix, c, 2)
        Next
      Next

      Swap(m_targetMatrix, m_sourceMatrix)
      m_dirty = True ' Any transform multiply dirties the inversion

    End Sub

  End Class

  ' Draws a sprite with the transform applied
  Public Shared Sub DrawSprite(sprite As Sprite, transform As Transform2D)

    If sprite Is Nothing Then Return

    Dim ex, ey, sx, sy, px, py As Single

    transform.Forward(0.0F, 0.0F, sx, sy)
    px = sx : py = sy
    sx = MathF.Min(sx, px) : sy = MathF.Min(sy, py)
    ex = MathF.Max(ex, px) : ey = MathF.Max(ey, py)

    transform.Forward(sprite.Width, sprite.Height, px, py)
    sx = MathF.Min(sx, px) : sy = MathF.Min(sy, py)
    ex = MathF.Max(ex, px) : ey = MathF.Max(ey, py)

    transform.Forward(0.0F, sprite.Height, px, py)
    sx = MathF.Min(sx, px) : sy = MathF.Min(sy, py)
    ex = MathF.Max(ex, px) : ey = MathF.Max(ey, py)

    transform.Forward(sprite.Width, 0.0F, px, py)
    sx = MathF.Min(sx, px) : sy = MathF.Min(sy, py)
    ex = MathF.Max(ex, px) : ey = MathF.Max(ey, py)

    transform.Invert()

    If ex < sx Then Swap(ex, sx)
    If ey < sy Then Swap(ey, sy)

    For i = sx To ex
      For j = sy To ey
        Dim ox, oy As Single
        transform.Backward(i, j, ox, oy)
        Singleton.Pge.Draw(CInt(Fix(i)), CInt(Fix(j)), sprite.GetPixel(CInt(Fix(ox + 0.5F)), CInt(Fix(oy + 0.5F))))
      Next
    Next

  End Sub

End Class