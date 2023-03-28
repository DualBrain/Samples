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

  Private ReadOnly m_path As New Spline
  Private ReadOnly m_trackLeft As New Spline
  Private ReadOnly m_trackRight As New Spline
  Private ReadOnly m_racingLine As New Spline

  Private ReadOnly m_nodes As Integer = 20 ' Number of nodes in spline

  Private ReadOnly m_displacement(19) As Single ' Displacement along spline node normal

  Private m_iterations As Integer = 1
  Private m_marker As Single = 1.0F
  Private m_selectedNode As Integer = -1

  Private m_modelCar As New List(Of (Single, Single))

  Public Sub New()
    m_sAppName = "Racing Line"
  End Sub

  Public Overrides Function OnUserCreate() As Boolean

    For i = 0 To m_nodes - 1
      ' Could use allocation functions for these now, but just size via append
      m_trackLeft.Points.Add(New Point2D(0.0F, 0.0F))
      m_trackRight.Points.Add(New Point2D(0.0F, 0.0F))
      m_racingLine.Points.Add(New Point2D(0.0F, 0.0F))
    Next

    ' A hand crafted track
    m_path.Points = New List(Of Point2D) From {New Point2D(81.8F, 196.0F), New Point2D(108.0F, 210.0F), New Point2D(152.0F, 216.0F),
                                               New Point2D(182.0F, 185.6F), New Point2D(190.0F, 159.0F), New Point2D(198.0F, 122.0F), New Point2D(226.0F, 93.0F),
                                               New Point2D(224.0F, 41.0F), New Point2D(204.0F, 15.0F), New Point2D(158.0F, 24.0F), New Point2D(146.0F, 52.0F),
                                               New Point2D(157.0F, 93.0F), New Point2D(124.0F, 129.0F), New Point2D(83.0F, 104.0F), New Point2D(77.0F, 62.0F),
                                               New Point2D(40.0F, 57.0F), New Point2D(21.0F, 83.0F), New Point2D(33.0F, 145.0F), New Point2D(30.0F, 198.0F),
                                               New Point2D(48.0F, 210.0F)}

    m_modelCar = New List(Of (Single, Single)) From {(2, 0), (0, -1), (0, 1)}

    m_path.UpdateSplineProperties()

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Clear Screen
    Fill(0, 0, ScreenWidth(), ScreenHeight(), PIXEL_SOLID, FG_DARK_GREEN)

    ' Handle iteration count
    If (m_keys(AscW("A"c)).Held) Then
      m_iterations += 1
    End If
    If (m_keys(AscW("S"c)).Held) Then
      m_iterations -= 1
    End If
    If (m_iterations < 0) Then
      m_iterations = 0
    End If

    ' Check if node is selected with mouse
    If (GetMouse(0).Pressed) Then
      For i = 0 To m_path.Points.Count() - 1
        Dim d = CSng(Math.Sqrt(Math.Pow(m_path.Points(i).X - GetMouseX(), 2) + Math.Pow(m_path.Points(i).Y - GetMouseY(), 2)))
        If (d < 5.0F) Then
          m_selectedNode = i
          Exit For
        End If
      Next
    End If

    If (GetMouse(0).Released) Then
      m_selectedNode = -1
    End If

    ' Move selected node
    If (GetMouse(0).Held AndAlso m_selectedNode >= 0) Then
      m_path.Points(m_selectedNode).X = GetMouseX()
      m_path.Points(m_selectedNode).Y = GetMouseY()
      m_path.UpdateSplineProperties()
    End If

    ' Move car around racing line
    m_marker += 2.0F * elapsedTime
    If m_marker >= m_racingLine.TotalSplineLength Then
      m_marker -= m_racingLine.TotalSplineLength
    End If

    ' Calculate track boundary points
    Dim trackWidth = 10.0F
    For i = 0 To m_path.Points.Count - 1
      Dim p1 = m_path.GetSplinePoint(i)
      Dim g1 = m_path.GetSplineGradient(i)
      Dim glen = CSng(Math.Sqrt(g1.X * g1.X + g1.Y * g1.Y))
      m_trackLeft.Points(i).X = p1.X + trackWidth * (-g1.Y / glen)
      m_trackLeft.Points(i).Y = p1.Y + trackWidth * (g1.X / glen)
      m_trackRight.Points(i).X = p1.X - trackWidth * (-g1.Y / glen)
      m_trackRight.Points(i).Y = p1.Y - trackWidth * (g1.X / glen)
    Next

    ' Draw Track
    Dim res = 0.2F
    For t = 0.0F To m_path.Points.Count Step res
      Dim pl1 = m_trackLeft.GetSplinePoint(t)
      Dim pr1 = m_trackRight.GetSplinePoint(t)
      Dim pl2 = m_trackLeft.GetSplinePoint(t + res)
      Dim pr2 = m_trackRight.GetSplinePoint(t + res)
      FillTriangle(pl1.X, pl1.Y, pr1.X, pr1.Y, pr2.X, pr2.Y, PIXEL_SOLID, FG_GREY)
      FillTriangle(pl1.X, pl1.Y, pl2.X, pl2.Y, pr2.X, pr2.Y, PIXEL_SOLID, FG_GREY)
    Next

    ' Reset racing line
    For i = 0 To m_racingLine.Points.Count - 1
      m_racingLine.Points(i).X = m_path.Points(i).X
      m_racingLine.Points(i).Y = m_path.Points(i).Y
      m_racingLine.Points(i).Length = m_path.Points(i).Length
      m_displacement(i) = 0
    Next
    m_racingLine.UpdateSplineProperties()

    For n = 0 To m_iterations - 1
      For i = 0 To m_racingLine.Points.Count - 1

        ' Get locations of neighbour nodes
        Dim pointRight = m_racingLine.Points((i + 1) Mod m_racingLine.Points.Count)
        Dim pointLeft = m_racingLine.Points((i + m_racingLine.Points.Count - 1) Mod m_racingLine.Points.Count)
        Dim pointMiddle = m_racingLine.Points(i)

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
        Dim g = m_path.GetSplineGradient(i)
        Dim glen = CSng(Math.Sqrt(g.X * g.X + g.Y * g.Y))
        g.X /= glen : g.Y /= glen

        ' Project required correction onto point tangent to give displacement
        Dim dp = -g.Y * vectorSum.X + g.X * vectorSum.Y

        ' Shortest path
        m_displacement(i) += dp * 0.3F

        ' Curvature
        'm_displacement((i + 1) Mod m_racingLine.Points.Count) += dp * -0.2F
        'm_displacement((i - 1 + m_racingLine.Points.Count) Mod m_racingLine.Points.Count) += dp * -0.2F

      Next

      'Clamp displaced points to track width
      For i = 0 To m_racingLine.Points.Count - 1

        If m_displacement(i) >= trackWidth Then m_displacement(i) = trackWidth
        If m_displacement(i) <= -trackWidth Then m_displacement(i) = -trackWidth

        Dim g = m_path.GetSplineGradient(i)
        Dim glen = CSng(Math.Sqrt(g.X * g.X + g.Y * g.Y))
        g.X /= glen : g.Y /= glen

        m_racingLine.Points(i).X = m_path.Points(i).X + -g.Y * m_displacement(i)
        m_racingLine.Points(i).Y = m_path.Points(i).Y + g.X * m_displacement(i)

      Next

    Next

    m_path.DrawSelf(Me, 0, 0)
    'm_trackLeft.DrawSelf(Me, 0, 0)
    'm_trackRight.DrawSelf(Me, 0, 0)

    m_racingLine.UpdateSplineProperties()
    m_racingLine.DrawSelf(Me, 0, 0, PIXEL_SOLID, FG_BLUE)

    For Each i In m_path.Points
      Fill(i.X - 1, i.Y - 1, i.X + 2, i.Y + 2, PIXEL_SOLID, FG_RED)
    Next

    Dim car_p = m_racingLine.GetSplinePoint(m_marker)
    Dim car_g = m_racingLine.GetSplineGradient(m_marker)
    DrawWireFrameModel(m_modelCar, car_p.X, car_p.Y, CSng(Math.Atan2(car_g.Y, car_g.X)), 4.0F, FG_BLACK)

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