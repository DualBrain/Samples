' Inspired by "Programming Racing Lines" -- @javidx9
' https://youtu.be/FlieT66N9OM

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine

Public Module Program

  Sub Main() 'args As String())
    Dim game As New RacingLines
    game.ConstructConsole(256, 240, 4, 4)
    game.Start()
  End Sub

End Module

Friend Class RacingLines
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private ReadOnly path As New Spline
  Private ReadOnly trackLeft As New Spline
  Private ReadOnly trackRight As New Spline
  Private ReadOnly racingLine As New Spline

  Private ReadOnly nNodes As Integer = 20 ' Number of nodes in spline

  Private ReadOnly fDisplacement(19) As Single ' Displacement along spline node normal

  Private nIterations As Integer = 1
  Private fMarker As Single = 1.0F
  Private nSelectedNode As Integer = -1

  Private vecModelCar As New List(Of (Single, Single))

  Public Sub New()
    m_sAppName = "Racing Line"
  End Sub

  Public Overrides Function OnUserCreate() As Boolean

    For i = 0 To nNodes - 1
      ' Could use allocation functions for these now, but just size via append
      trackLeft.Points.Add(New Point2D(0.0F, 0.0F))
      trackRight.Points.Add(New Point2D(0.0F, 0.0F))
      racingLine.Points.Add(New Point2D(0.0F, 0.0F))
    Next

    ' A hand crafted track
    path.Points = New List(Of Point2D) From {New Point2D(81.8F, 196.0F), New Point2D(108.0F, 210.0F), New Point2D(152.0F, 216.0F),
                                             New Point2D(182.0F, 185.6F), New Point2D(190.0F, 159.0F), New Point2D(198.0F, 122.0F), New Point2D(226.0F, 93.0F),
                                             New Point2D(224.0F, 41.0F), New Point2D(204.0F, 15.0F), New Point2D(158.0F, 24.0F), New Point2D(146.0F, 52.0F),
                                             New Point2D(157.0F, 93.0F), New Point2D(124.0F, 129.0F), New Point2D(83.0F, 104.0F), New Point2D(77.0F, 62.0F),
                                             New Point2D(40.0F, 57.0F), New Point2D(21.0F, 83.0F), New Point2D(33.0F, 145.0F), New Point2D(30.0F, 198.0F),
                                             New Point2D(48.0F, 210.0F)}

    vecModelCar = New List(Of (Single, Single)) From {(2, 0), (0, -1), (0, 1)}

    path.UpdateSplineProperties()

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Clear Screen
    Fill(0, 0, ScreenWidth(), ScreenHeight(), PIXEL_SOLID, FG_DARK_GREEN)

    ' Handle iteration count
    If (m_keys(AscW("A"c)).Held) Then
      nIterations += 1
    End If
    If (m_keys(AscW("S"c)).Held) Then
      nIterations -= 1
    End If
    If (nIterations < 0) Then
      nIterations = 0
    End If

    ' Check if node is selected with mouse
    If (GetMouse(0).Pressed) Then
      For i = 0 To path.Points.Count() - 1
        Dim d = CSng(Math.Sqrt(Math.Pow(path.Points(i).X - GetMouseX(), 2) + Math.Pow(path.Points(i).Y - GetMouseY(), 2)))
        If (d < 5.0F) Then
          nSelectedNode = i
          Exit For
        End If
      Next
    End If

    If (GetMouse(0).Released) Then
      nSelectedNode = -1
    End If

    ' Move selected node
    If (GetMouse(0).Held AndAlso nSelectedNode >= 0) Then
      path.Points(nSelectedNode).X = GetMouseX()
      path.Points(nSelectedNode).Y = GetMouseY()
      path.UpdateSplineProperties()
    End If

    ' Move car around racing line
    fMarker += 2.0F * elapsedTime
    If fMarker >= racingLine.TotalSplineLength Then
      fMarker -= racingLine.TotalSplineLength
    End If

    ' Calculate track boundary points
    Dim fTrackWidth = 10.0F
    For i = 0 To path.Points.Count - 1
      Dim p1 = path.GetSplinePoint(i)
      Dim g1 = path.GetSplineGradient(i)
      Dim glen = CSng(Math.Sqrt(g1.X * g1.X + g1.Y * g1.Y))
      trackLeft.Points(i).X = p1.X + fTrackWidth * (-g1.Y / glen)
      trackLeft.Points(i).Y = p1.Y + fTrackWidth * (g1.X / glen)
      trackRight.Points(i).X = p1.X - fTrackWidth * (-g1.Y / glen)
      trackRight.Points(i).Y = p1.Y - fTrackWidth * (g1.X / glen)
    Next

    ' Draw Track
    Dim fRes = 0.2F
    For t = 0.0F To path.Points.Count Step fRes
      Dim pl1 = trackLeft.GetSplinePoint(t)
      Dim pr1 = trackRight.GetSplinePoint(t)
      Dim pl2 = trackLeft.GetSplinePoint(t + fRes)
      Dim pr2 = trackRight.GetSplinePoint(t + fRes)
      FillTriangle(pl1.X, pl1.Y, pr1.X, pr1.Y, pr2.X, pr2.Y, PIXEL_SOLID, FG_GREY)
      FillTriangle(pl1.X, pl1.Y, pl2.X, pl2.Y, pr2.X, pr2.Y, PIXEL_SOLID, FG_GREY)
    Next

    ' Reset racing line
    For i = 0 To racingLine.Points.Count - 1
      racingLine.Points(i).X = path.Points(i).X
      racingLine.Points(i).Y = path.Points(i).Y
      racingLine.Points(i).Length = path.Points(i).Length
      fDisplacement(i) = 0
    Next
    racingLine.UpdateSplineProperties()

    For n = 0 To nIterations - 1
      For i = 0 To racingLine.Points.Count - 1

        ' Get locations of neighbour nodes
        Dim pointRight = racingLine.Points((i + 1) Mod racingLine.Points.Count)
        Dim pointLeft = racingLine.Points((i + racingLine.Points.Count - 1) Mod racingLine.Points.Count)
        Dim pointMiddle = racingLine.Points(i)

        ' Create vectors to neighbours
        Dim vectorLeft As New Point2D(pointLeft.X - pointMiddle.X, pointLeft.Y - pointMiddle.Y)
        Dim vectorRight As New Point2D(pointRight.X - pointMiddle.X, pointRight.Y - pointMiddle.Y)

        ' Normalise neighbours
        Dim lengthLeft = CSng(Math.Sqrt(vectorLeft.X * vectorLeft.X + vectorLeft.Y * vectorLeft.Y))
        Dim leftn As New Point2D(vectorLeft.X / lengthLeft, vectorLeft.Y / lengthLeft)
        Dim lengthRight = CSng(Math.Sqrt(vectorRight.X * vectorRight.X + vectorRight.Y * vectorRight.Y))
        Dim rightn As New Point2D(vectorRight.X / lengthRight, vectorRight.Y / lengthRight)

        ' Add together to create bisector vector
        Dim vectorSum As New Point2D(rightn.X + leftn.X, rightn.Y + leftn.Y)
        Dim len = CSng(Math.Sqrt(vectorSum.X * vectorSum.X + vectorSum.Y * vectorSum.Y))
        vectorSum.X /= len : vectorSum.Y /= len

        ' Get point gradient and normalise
        Dim g = path.GetSplineGradient(i)
        Dim glen = CSng(Math.Sqrt(g.X * g.X + g.Y * g.Y))
        g.X /= glen : g.Y /= glen

        ' Project required correction onto point tangent to give displacement
        Dim dp = -g.Y * vectorSum.X + g.X * vectorSum.Y

        ' Shortest path
        fDisplacement(i) += dp * 0.3F

        ' Curvature
        'fDisplacement((i + 1) Mod racingLine.points.Count) += dp * -0.2F
        'fDisplacement((i - 1 + racingLine.points.Count) Mod racingLine.points.Count) += dp * -0.2F

      Next

      'Clamp displaced points to track width
      For i = 0 To racingLine.Points.Count - 1

        If fDisplacement(i) >= fTrackWidth Then fDisplacement(i) = fTrackWidth
        If fDisplacement(i) <= -fTrackWidth Then fDisplacement(i) = -fTrackWidth

        Dim g = path.GetSplineGradient(i)
        Dim glen = CSng(Math.Sqrt(g.X * g.X + g.Y * g.Y))
        g.X /= glen : g.Y /= glen

        racingLine.Points(i).X = path.Points(i).X + -g.Y * fDisplacement(i)
        racingLine.Points(i).Y = path.Points(i).Y + g.X * fDisplacement(i)

      Next

    Next

    path.DrawSelf(Me, 0, 0)
    'trackLeft.DrawSelf(Me, 0, 0)
    'trackRight.DrawSelf(Me, 0, 0)

    racingLine.UpdateSplineProperties()
    racingLine.DrawSelf(Me, 0, 0, PIXEL_SOLID, FG_BLUE)

    For Each i In path.Points
      Fill(i.X - 1, i.Y - 1, i.X + 2, i.Y + 2, PIXEL_SOLID, FG_RED)
    Next

    Dim car_p = racingLine.GetSplinePoint(fMarker)
    Dim car_g = racingLine.GetSplineGradient(fMarker)
    DrawWireFrameModel(vecModelCar, car_p.X, car_p.Y, CSng(Math.Atan2(car_g.Y, car_g.X)), 4.0F, FG_BLACK)

    Return True

  End Function

End Class

Friend Class Point2D
  Friend Property X As Single
  Friend Property Y As Single
  Friend Property Length As Single
  Friend Sub New()
  End Sub
  Friend Sub New(x As Single, y As Single)
    Me.X = x
    Me.Y = y
  End Sub
End Class

Friend Class Spline

  Friend Property Points As New List(Of Point2D)
  Friend Property TotalSplineLength As Single
  Friend Property IsLooped As Boolean = True

  Friend Function GetSplinePoint(t As Single) As Point2D

    Dim p0, p1, p2, p3 As Integer

    If Not IsLooped Then
      p1 = CInt(Fix(t)) + 1
      p2 = p1 + 1
      p3 = p2 + 1
      p0 = p1 - 1
    Else
      p1 = CInt(Fix(t)) Mod Points.Count
      p2 = (p1 + 1) Mod Points.Count
      p3 = (p2 + 1) Mod Points.Count
      p0 = If(p1 >= 1, p1 - 1, Points.Count - 1)
    End If

    t -= CInt(Fix(t))

    Dim tt = t * t
    Dim ttt = tt * t

    Dim q1 = -ttt + 2.0F * tt - t
    Dim q2 = 3.0F * ttt - 5.0F * tt + 2.0F
    Dim q3 = -3.0F * ttt + 4.0F * tt + t
    Dim q4 = ttt - tt

    Dim tx = 0.5F * (Points(p0).X * q1 + Points(p1).X * q2 + Points(p2).X * q3 + Points(p3).X * q4)
    Dim ty = 0.5F * (Points(p0).Y * q1 + Points(p1).Y * q2 + Points(p2).Y * q3 + Points(p3).Y * q4)

    Return New Point2D(tx, ty)

  End Function

  Friend Function GetSplineGradient(t As Single) As Point2D

    Dim p0, p1, p2, p3 As Integer

    If Not IsLooped Then
      p1 = CInt(Fix(t)) + 1
      p2 = p1 + 1
      p3 = p2 + 1
      p0 = p1 - 1
    Else
      p1 = CInt(Fix(t)) Mod Points.Count
      p2 = (p1 + 1) Mod Points.Count
      p3 = (p2 + 1) Mod Points.Count
      p0 = If(p1 >= 1, p1 - 1, Points.Count - 1)
    End If

    t -= CInt(Fix(t))

    Dim tt = t * t
    Dim ttt = tt * t

    Dim q1 = -3.0F * tt + 4.0F * t - 1.0F
    Dim q2 = 9.0F * tt - 10.0F * t
    Dim q3 = -9.0F * tt + 8.0F * t + 1.0F
    Dim q4 = 3.0F * tt - 2.0F * t

    Dim tx = 0.5F * (Points(p0).X * q1 + Points(p1).X * q2 + Points(p2).X * q3 + Points(p3).X * q4)
    Dim ty = 0.5F * (Points(p0).Y * q1 + Points(p1).Y * q2 + Points(p2).Y * q3 + Points(p3).Y * q4)

    Return New Point2D(tx, ty)

  End Function

  Friend Function CalculateSegmentLength(node As Integer) As Single

    Dim length = 0.0F
    Dim stepSize = 0.005F

    Dim oldPoint = GetSplinePoint(node)

    Dim max = 1.0F - stepSize
    For t = 0 To max Step stepSize
      Dim newPoint = GetSplinePoint(node + t)
      length += CSng(Math.Sqrt((newPoint.X - oldPoint.X) * (newPoint.X - oldPoint.X) + (newPoint.Y - oldPoint.Y) * (newPoint.Y - oldPoint.Y)))
      oldPoint = newPoint
    Next

    Return length

  End Function

  Friend Function GetNormalisedOffset(p As Single) As Single
    ' Which node is the base?
    Dim i = 0
    While p > Points(i).Length
      p -= Points(i).Length
      i += 1
    End While
    ' The fractional is the offset 
    Return i + (p / Points(i).Length)
  End Function

  Friend Sub UpdateSplineProperties()

    ' Use to cache local spline lengths and overall spline length
    TotalSplineLength = 0.0F

    If IsLooped Then
      ' Each node has a succeeding length
      For i = 0 To Points.Count - 1
        Points(i).Length = CalculateSegmentLength(i)
        TotalSplineLength += Points(i).Length
      Next
    Else
      For i = 1 To Points.Count - 3
        Points(i).Length = CalculateSegmentLength(i)
        TotalSplineLength += Points(i).Length
      Next
    End If

  End Sub

  Friend Sub DrawSelf(gfx As ConsoleGameEngine.ConsoleGameEngine, ox As Single, oy As Single, Optional c As Integer = &H2588, Optional col As Integer = &HF)
    If IsLooped Then
      For t = 0 To Points.Count Step 0.005F
        Dim pos = GetSplinePoint(t)
        gfx.Draw(pos.X, pos.Y, c, col)
      Next
    Else ' Not Looped
      For t = 0 To Points.Count - 3 Step 0.005F
        Dim pos = GetSplinePoint(t)
        gfx.Draw(pos.X, pos.Y, c, col)
      Next
    End If
  End Sub

End Class