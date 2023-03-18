' Inspired by: "Programming & Using Splines - Part#2" -- @javidx9
' https://youtu.be/DzjtU4WLYNs

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour

Module Program

  Sub Main() 'args As String())
    Dim game As New Splines2
    game.ConstructConsole(160, 100, 8, 8)
    game.Start()
  End Sub

End Module

Class Splines2
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_path As New Spline
  Private m_selectedPoint As Integer
  Private m_marker As Double

  Private m_modelCar As List(Of (Single, Single))

  Public Overrides Function OnUserCreate() As Boolean

    For i = 0 To 9
      m_path.Points.Add(New Point2D(30 * Math.Sin(i / 10 * 3.14159 * 2) + ScreenWidth() / 2,
                                       30 * Math.Cos(i / 10 * 3.14159 * 2) + ScreenHeight() / 2))
    Next

    m_modelCar = New List(Of (Single, Single)) From {(1, 1), (1, 3), (3, 0), (0, -3), (-3, 0), (-1, 3), (-1, 1)}

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Clear Screen
    Cls()

    ' Handle input
    If m_keys(AscW("X")).Released Then
      m_selectedPoint += 1
      If m_selectedPoint > m_path.Points.Count - 1 Then
        m_selectedPoint = 0
      End If
    End If
    If m_keys(AscW("Z")).Released Then
      m_selectedPoint -= 1
      If m_selectedPoint < 0 Then
        m_selectedPoint = m_path.Points.Count - 1
      End If
    End If
    If m_keys(VK_LEFT).bHeld Then m_path.Points(m_selectedPoint).X -= 30 * elapsedTime
    If m_keys(VK_RIGHT).bHeld Then m_path.Points(m_selectedPoint).X += 30 * elapsedTime
    If m_keys(VK_UP).bHeld Then m_path.Points(m_selectedPoint).Y -= 30 * elapsedTime
    If m_keys(VK_DOWN).bHeld Then m_path.Points(m_selectedPoint).Y += 30 * elapsedTime
    If m_keys(AscW("A")).bHeld Then m_marker -= 20.0 * elapsedTime
    If m_keys(AscW("S")).bHeld Then m_marker += 20.0 * elapsedTime

    If m_marker > m_path.TotalSplineLength Then m_marker -= m_path.TotalSplineLength
    If m_marker < 0 Then m_marker += m_path.TotalSplineLength

    ' Draw Spline
    For t = 0.0 To m_path.Points.Count Step 0.005
      Dim pos = m_path.GetSplinePoint(t, True)
      Draw(pos.X, pos.Y)
    Next

    m_path.TotalSplineLength = 0.0

    ' Draw Control Points
    For i = 0 To m_path.Points.Count - 1
      m_path.Points(i).Length = m_path.CalculateSegmentLength(i, True)
      m_path.TotalSplineLength += m_path.Points(i).Length
      Fill(m_path.Points(i).X - 1, m_path.Points(i).Y - 1, m_path.Points(i).X + 2, m_path.Points(i).Y + 2, PIXEL_SOLID, FG_RED)
      DrawString(m_path.Points(i).X, m_path.Points(i).Y, CStr(i))
      DrawString(m_path.Points(i).X + 3, m_path.Points(i).Y, CStr(m_path.Points(i).Length))
    Next

    ' Highlight control point
    Fill(m_path.Points(m_selectedPoint).X - 1, m_path.Points(m_selectedPoint).Y - 1, m_path.Points(m_selectedPoint).X + 2, m_path.Points(m_selectedPoint).Y + 2, PIXEL_SOLID, FG_YELLOW)
    DrawString(m_path.Points(m_selectedPoint).X, m_path.Points(m_selectedPoint).Y, CStr(m_selectedPoint))

    ' Draw agent to demonstrate gradient
    Dim offset = m_path.GetNormalisedOffset(m_marker)
    Dim p1 = m_path.GetSplinePoint(offset, True)
    Dim g1 = m_path.GetSplineGradient(offset, True)
    Dim r = Math.Atan2(-g1.Y, g1.X)
    DrawLine(5.0F * Math.Sin(r) + p1.X, 5.0F * Math.Cos(r) + p1.Y, -5.0F * Math.Sin(r) + p1.X, -5.0F * Math.Cos(r) + p1.Y, PIXEL_SOLID, FG_BLUE)

    DrawWireFrameModel(m_modelCar, CSng(p1.X), CSng(p1.Y), CSng(-r + (3.14159F / 2.0F)), 5.0F, FG_CYAN)

    DrawString(2, 2, CStr(offset))
    DrawString(2, 4, CStr(m_marker))

    Return True

  End Function

End Class

Class Point2D
  Public Property X As Double
  Public Property Y As Double
  Public Property Length As Double
  Public Sub New()
  End Sub
  Public Sub New(x As Double, y As Double)
    Me.X = x
    Me.Y = y
  End Sub
End Class

Class Spline

  Public Property Points As New List(Of Point2D)
  Public Property TotalSplineLength As Double

  Public Function GetSplinePoint(t As Double, Optional looped As Boolean = False) As Point2D

    Dim p0, p1, p2, p3 As Integer

    If Not looped Then
      p1 = CInt(Fix(t)) + 1
      p2 = p1 + 1
      p3 = p2 + 1
      p0 = p1 - 1
    Else
      p1 = CInt(Fix(t))
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

  Public Function GetSplineGradient(t As Double, Optional looped As Boolean = False) As Point2D

    Dim p0, p1, p2, p3 As Integer

    If Not looped Then
      p1 = CInt(Fix(t)) + 1
      p2 = p1 + 1
      p3 = p2 + 1
      p0 = p1 - 1
    Else
      p1 = CInt(Fix(t))
      p2 = (p1 + 1) Mod Points.Count
      p3 = (p2 + 1) Mod Points.Count
      p0 = If(p1 >= 1, p1 - 1, Points.Count - 1)
    End If

    t -= CInt(Fix(t))

    Dim tt = t * t
    Dim ttt = tt * t

    Dim q1 = -3.0F * tt + 4.0F * t - 1
    Dim q2 = 9.0F * tt - 10.0F * t
    Dim q3 = -9.0F * tt + 8.0F * t + 1.0F
    Dim q4 = 3.0F * tt - 2.0F * t

    Dim tx = 0.5F * (Points(p0).X * q1 + Points(p1).X * q2 + Points(p2).X * q3 + Points(p3).X * q4)
    Dim ty = 0.5F * (Points(p0).Y * q1 + Points(p1).Y * q2 + Points(p2).Y * q3 + Points(p3).Y * q4)

    Return New Point2D(tx, ty)

  End Function

  Public Function CalculateSegmentLength(node As Integer, Optional looped As Boolean = False) As Double

    Dim length = 0.0
    Dim stepSize = 0.005
    Dim oldPoint, newPoint As Point2D

    oldPoint = GetSplinePoint(node, looped)

    For t = 0 To 1 - stepSize Step stepSize
      newPoint = GetSplinePoint(node + t, looped)
      length += Math.Sqrt((newPoint.X - oldPoint.X) * (newPoint.X - oldPoint.X) + (newPoint.Y - oldPoint.Y) * (newPoint.Y - oldPoint.Y))
      oldPoint = newPoint
    Next

    Return length

  End Function

  Public Function GetNormalisedOffset(p As Double) As Double
    ' Which node is the base?
    Dim i = 0
    While p > Points(i).Length
      p -= Points(i).Length
      i += 1
    End While
    ' The fractional is the offset 
    Return i + (p / Points(i).Length)
  End Function

End Class