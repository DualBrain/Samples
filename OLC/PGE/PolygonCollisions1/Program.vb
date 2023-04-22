' Inspired by "Convex Polygon Collisions #1" -- @javidx9
' https://youtu.be/7Ik2vowGcU0

Option Explicit On
Option Strict On
Option Infer On

Imports Olc.Presets

Friend Module Program

  Sub Main()
    Dim game As New PolygonCollisions
    If game.Construct(256, 240, 4, 4) Then
      game.Start()
    End If
  End Sub

End Module

Friend Class PolygonCollisions
  Inherits Olc.PixelGameEngine

  Private Class Vec2d
    Public X As Single
    Public Y As Single
    Public Sub New()
    End Sub
    Public Sub New(x As Single, y As Single)
      Me.X = x
      Me.Y = y
    End Sub
  End Class

  Private Class Polygon
    Public P As New List(Of Vec2d)    ' Transformed Points
    Public Pos As New Vec2d           ' Position of shape
    Public Angle As Single            ' Direction of shape
    Public O As New List(Of Vec2d)    ' "Model" of shape
    Public Overlap As Boolean = False ' Flag to indicate if overlap has occurred
  End Class

  Private ReadOnly m_shapes As New List(Of Polygon)

  Private m_mode As Integer = 0

  Friend Sub New()
    AppName = "Polygon Collisions"
  End Sub

  Protected Overrides Function OnUserCreate() As Boolean

    ' Create Pentagon
    Dim s1 As New Polygon()
    Dim fTheta = MathF.PI * 2.0F / 5.0F
    s1.Pos = New Vec2d With {.X = 100, .Y = 100}
    s1.Angle = 0.0F
    For i = 0 To 4
      s1.O.Add(New Vec2d With {.X = 30.0F * MathF.Cos(fTheta * i), .Y = 30.0F * MathF.Sin(fTheta * i)})
      s1.P.Add(New Vec2d With {.X = 30.0F * MathF.Cos(fTheta * i), .Y = 30.0F * MathF.Sin(fTheta * i)})
    Next

    ' Create Triangle
    Dim s2 As New Polygon()
    fTheta = MathF.PI * 2.0F / 3.0F
    s2.Pos = New Vec2d With {.X = 200, .Y = 150}
    s2.Angle = 0.0F
    For i = 0 To 2
      s2.O.Add(New Vec2d With {.X = 20.0F * MathF.Cos(fTheta * i), .Y = 20.0F * MathF.Sin(fTheta * i)})
      s2.P.Add(New Vec2d With {.X = 20.0F * MathF.Cos(fTheta * i), .Y = 20.0F * MathF.Sin(fTheta * i)})
    Next

    ' Create Quad
    Dim s3 As New Polygon With {.Pos = New Vec2d With {.X = 50, .Y = 200},
                                .Angle = 0.0F}
    s3.O.Add(New Vec2d With {.X = -30, .Y = -30})
    s3.O.Add(New Vec2d With {.X = -30, .Y = 30})
    s3.O.Add(New Vec2d With {.X = 30, .Y = 30})
    s3.O.Add(New Vec2d With {.X = 30, .Y = -30})
    's3.P.Capacity = 4
    For index = 0 To s3.O.Count - 1
      s3.P.Add(New Vec2d)
    Next

    m_shapes.Add(s1)
    m_shapes.Add(s2)
    m_shapes.Add(s3)

    Return True

  End Function

  Private Shared Function ShapeOverlap_SAT(r1 As Polygon, r2 As Polygon) As Boolean

    Dim poly1 = r1
    Dim poly2 = r2

    For shape = 0 To 1

      If shape = 1 Then
        poly1 = r2
        poly2 = r1
      End If

      For a = 0 To poly1.P.Count - 1

        Dim b = (a + 1) Mod poly1.P.Count
        Dim axisProj = New Vec2d(-(poly1.P(b).Y - poly1.P(a).Y), poly1.P(b).X - poly1.P(a).X)
        Dim d = MathF.Sqrt(axisProj.X * axisProj.X + axisProj.Y * axisProj.Y)
        axisProj = New Vec2d(axisProj.X / d, axisProj.Y / d)

        ' Work out min and max 1D points for r1
        Dim min_r1 = Single.PositiveInfinity, max_r1 = Single.NegativeInfinity
        For p = 0 To poly1.P.Count - 1
          Dim q = (poly1.P(p).X * axisProj.X + poly1.P(p).Y * axisProj.Y)
          min_r1 = Math.Min(min_r1, q)
          max_r1 = Math.Max(max_r1, q)
        Next

        ' Work out min and max 1D points for r2
        Dim min_r2 = Single.PositiveInfinity, max_r2 = Single.NegativeInfinity
        For p = 0 To poly2.P.Count - 1
          Dim q = (poly2.P(p).X * axisProj.X + poly2.P(p).Y * axisProj.Y)
          min_r2 = Math.Min(min_r2, q)
          max_r2 = Math.Max(max_r2, q)
        Next

        If Not (max_r2 >= min_r1 AndAlso max_r1 >= min_r2) Then
          Return False
        End If

      Next

    Next

    Return True

  End Function

  Private Shared Function ShapeOverlap_SAT_STATIC(r1 As Polygon, r2 As Polygon) As Boolean

    Dim poly1 = r1
    Dim poly2 = r2

    Dim overlap = Single.PositiveInfinity

    For shape = 0 To 1

      If shape = 1 Then
        poly1 = r2
        poly2 = r1
      End If

      For a = 0 To poly1.P.Count - 1

        Dim b = (a + 1) Mod poly1.P.Count
        Dim axisProj = New Vec2d(-(poly1.P(b).Y - poly1.P(a).Y), poly1.P(b).X - poly1.P(a).X)

        ' Optional normalisation of projection axis enhances stability slightly
        ' Dim d = MathF.Sqrt(axisProj.x * axisProj.x + axisProj.y * axisProj.y)
        ' axisProj = New Vec2d(axisProj.x / d, axisProj.y / d)

        ' Work out min and max 1D points for r1
        Dim min_r1 = Single.PositiveInfinity
        Dim max_r1 = Single.NegativeInfinity
        For p = 0 To poly1.P.Count - 1
          Dim q = poly1.P(p).X * axisProj.X + poly1.P(p).Y * axisProj.Y
          min_r1 = Math.Min(min_r1, q)
          max_r1 = Math.Max(max_r1, q)
        Next

        ' Work out min and max 1D points for r2
        Dim min_r2 = Single.PositiveInfinity
        Dim max_r2 = Single.NegativeInfinity
        For p = 0 To poly2.P.Count - 1
          Dim q = poly2.P(p).X * axisProj.X + poly2.P(p).Y * axisProj.Y
          min_r2 = Math.Min(min_r2, q)
          max_r2 = Math.Max(max_r2, q)
        Next

        ' Calculate actual overlap along projected axis, and store the minimum
        overlap = Math.Min(Math.Min(max_r1, max_r2) - Math.Max(min_r1, min_r2), overlap)

        If Not (max_r2 >= min_r1 AndAlso max_r1 >= min_r2) Then
          Return False
        End If

      Next

    Next

    ' If we got here, the objects have collided, we will displace r1
    ' by overlap along the vector between the two object centers
    Dim d = New Vec2d(r2.Pos.X - r1.Pos.X, r2.Pos.Y - r1.Pos.Y)
    Dim s = MathF.Sqrt(d.X * d.X + d.Y * d.Y)
    r1.Pos.X -= overlap * d.X / s
    r1.Pos.Y -= overlap * d.Y / s

    Return True

  End Function

  Private Shared Function ShapeOverlap_DIAGS(r1 As Polygon, r2 As Polygon) As Boolean

    Dim poly1 = r1
    Dim poly2 = r2

    For shape = 0 To 1

      If shape = 1 Then
        poly1 = r2
        poly2 = r1
      End If

      ' Check diagonals of polygon...
      For p = 0 To poly1.P.Count() - 1

        Dim line_r1s As Vec2d = poly1.Pos
        Dim line_r1e As Vec2d = poly1.P(p)

        ' ...against edges of the other
        For q = 0 To poly2.P.Count() - 1

          Dim line_r2s As Vec2d = poly2.P(q)
          Dim line_r2e As Vec2d = poly2.P((q + 1) Mod poly2.P.Count())

          ' Standard "off the shelf" line segment intersection
          Dim h = (line_r2e.X - line_r2s.X) * (line_r1s.Y - line_r1e.Y) - (line_r1s.X - line_r1e.X) * (line_r2e.Y - line_r2s.Y)
          Dim t1 = ((line_r2s.Y - line_r2e.Y) * (line_r1s.X - line_r2s.X) + (line_r2e.X - line_r2s.X) * (line_r1s.Y - line_r2s.Y)) / h
          Dim t2 = ((line_r1s.Y - line_r1e.Y) * (line_r1s.X - line_r2s.X) + (line_r1e.X - line_r1s.X) * (line_r1s.Y - line_r2s.Y)) / h

          If t1 >= 0.0F AndAlso t1 < 1.0F AndAlso t2 >= 0.0F AndAlso t2 < 1.0F Then
            Return True
          End If

        Next

      Next

    Next

    Return False

  End Function

  ' Use edge/diagonal intersections.
  Private Shared Function ShapeOverlap_DIAGS_STATIC(r1 As Polygon, r2 As Polygon) As Boolean

    Dim poly1 = r1
    Dim poly2 = r2

    For shape = 0 To 1

      If shape = 1 Then
        poly1 = r2
        poly2 = r1
      End If

      ' Check diagonals of this polygon...
      For p = 0 To poly1.P.Count - 1

        Dim line_r1s = poly1.Pos
        Dim line_r1e = poly1.P(p)

        Dim displacement = New Vec2d(0, 0)

        ' ...against edges of this polygon
        For q = 0 To poly2.P.Count - 1

          Dim line_r2s = poly2.P(q)
          Dim line_r2e = poly2.P((q + 1) Mod poly2.P.Count)

          ' Standard "off the shelf" line segment intersection
          Dim h = (line_r2e.X - line_r2s.X) * (line_r1s.Y - line_r1e.Y) - (line_r1s.X - line_r1e.X) * (line_r2e.Y - line_r2s.Y)
          Dim t1 = ((line_r2s.Y - line_r2e.Y) * (line_r1s.X - line_r2s.X) + (line_r2e.X - line_r2s.X) * (line_r1s.Y - line_r2s.Y)) / h
          Dim t2 = ((line_r1s.Y - line_r1e.Y) * (line_r1s.X - line_r2s.X) + (line_r1e.X - line_r1s.X) * (line_r1s.Y - line_r2s.Y)) / h

          If t1 >= 0.0F AndAlso t1 < 1.0F AndAlso t2 >= 0.0F AndAlso t2 < 1.0F Then
            displacement.X += (1.0F - t1) * (line_r1e.X - line_r1s.X)
            displacement.Y += (1.0F - t1) * (line_r1e.Y - line_r1s.Y)
          End If

        Next

        r1.Pos.X += displacement.X * (If(shape = 0, -1, +1))
        r1.Pos.Y += displacement.Y * (If(shape = 0, -1, +1))

      Next

    Next

    ' Cant overlap if static collision is resolved
    Return False

  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    If GetKey(Key.F1).Released Then m_mode = 0
    If GetKey(Key.F2).Released Then m_mode = 1
    If GetKey(Key.F3).Released Then m_mode = 2
    If GetKey(Key.F4).Released Then m_mode = 3

    ' Shape 1
    If GetKey(Key.LEFT).Held Then m_shapes(0).Angle -= 2.0F * elapsedTime
    If GetKey(Key.RIGHT).Held Then m_shapes(0).Angle += 2.0F * elapsedTime

    If GetKey(Key.UP).Held Then
      m_shapes(0).Pos.X += MathF.Cos(m_shapes(0).Angle) * 60.0F * elapsedTime
      m_shapes(0).Pos.Y += MathF.Sin(m_shapes(0).Angle) * 60.0F * elapsedTime
    End If

    If GetKey(Key.DOWN).Held Then
      m_shapes(0).Pos.X -= MathF.Cos(m_shapes(0).Angle) * 60.0F * elapsedTime
      m_shapes(0).Pos.Y -= MathF.Sin(m_shapes(0).Angle) * 60.0F * elapsedTime
    End If

    ' Shape 2
    If GetKey(Key.A).Held Then m_shapes(1).Angle -= 2.0F * elapsedTime
    If GetKey(Key.D).Held Then m_shapes(1).Angle += 2.0F * elapsedTime

    If GetKey(Key.W).Held Then
      m_shapes(1).Pos.X += MathF.Cos(m_shapes(1).Angle) * 60.0F * elapsedTime
      m_shapes(1).Pos.Y += MathF.Sin(m_shapes(1).Angle) * 60.0F * elapsedTime
    End If

    If GetKey(Key.S).Held Then
      m_shapes(1).Pos.X -= MathF.Cos(m_shapes(1).Angle) * 60.0F * elapsedTime
      m_shapes(1).Pos.Y -= MathF.Sin(m_shapes(1).Angle) * 60.0F * elapsedTime
    End If

    ' Update Shapes and reset flags
    For Each r In m_shapes
      For i = 0 To r.O.Count - 1
        r.P(i) = New Vec2d((r.O(i).X * MathF.Cos(r.Angle)) - (r.O(i).Y * MathF.Sin(r.Angle)) + r.Pos.X,
                                 (r.O(i).X * MathF.Sin(r.Angle)) + (r.O(i).Y * MathF.Cos(r.Angle)) + r.Pos.Y)
      Next
      r.Overlap = False
    Next

    ' Check for overlap
    For m = 0 To m_shapes.Count - 2
      For n = m + 1 To m_shapes.Count - 1
        Select Case m_mode
          Case 0 : m_shapes(m).Overlap = m_shapes(m).Overlap Or ShapeOverlap_SAT(m_shapes(m), m_shapes(n))
          Case 1 : m_shapes(m).Overlap = m_shapes(m).Overlap Or ShapeOverlap_SAT_STATIC(m_shapes(m), m_shapes(n))
          Case 2 : m_shapes(m).Overlap = m_shapes(m).Overlap Or ShapeOverlap_DIAGS(m_shapes(m), m_shapes(n))
          Case 3 : m_shapes(m).Overlap = m_shapes(m).Overlap Or ShapeOverlap_DIAGS_STATIC(m_shapes(m), m_shapes(n))
        End Select
      Next
    Next

    ' === Render Display ===
    Clear(Blue)

    ' Draw Shapes
    For Each r In m_shapes
      ' Draw Boundary
      For i = 0 To r.P.Count - 1
        DrawLine(r.P(i).X, r.P(i).Y, r.P((i + 1) Mod r.P.Count).X, r.P((i + 1) Mod r.P.Count).Y, If(r.Overlap, Red, White))
      Next
      ' Draw Direction
      DrawLine(r.P(0).X, r.P(0).Y, r.Pos.X, r.Pos.Y, If(r.Overlap, Red, White))
    Next

    ' Draw HUD
    DrawString(8, 10, "F1: SAT", If(m_mode = 0, Red, Yellow))
    DrawString(8, 20, "F2: SAT/STATIC", If(m_mode = 1, Red, Yellow))
    DrawString(8, 30, "F3: DIAG", If(m_mode = 2, Red, Yellow))
    DrawString(8, 40, "F4: DIAG/STATIC", If(m_mode = 3, Red, Yellow))

    Return True

  End Function

End Class