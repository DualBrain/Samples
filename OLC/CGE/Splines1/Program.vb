' Inspired by: "Programming & Using Splines - Part#1" -- @javidx9
' https://youtu.be/9_aJGUTePYo

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour

Module Program

  Sub Main() 'args As String())
    Dim game As New Splines1
    game.ConstructConsole(160, 100, 8, 8)
    game.Start()
  End Sub

End Module

Class Splines1
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_path As New Spline
  Private m_selectedPoint As Integer
  Private m_marker As Double

  Public Overrides Function OnUserCreate() As Boolean

    m_path.Points.Add(New Point2D(10, 41))
    m_path.Points.Add(New Point2D(20, 41))
    m_path.Points.Add(New Point2D(30, 41))
    m_path.Points.Add(New Point2D(40, 41))
    m_path.Points.Add(New Point2D(50, 41))
    m_path.Points.Add(New Point2D(60, 41))
    m_path.Points.Add(New Point2D(70, 41))
    m_path.Points.Add(New Point2D(80, 41))
    m_path.Points.Add(New Point2D(90, 41))
    m_path.Points.Add(New Point2D(100, 41))

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
    If m_keys(VK_LEFT).Held Then m_path.Points(m_selectedPoint).X -= 30 * elapsedTime
    If m_keys(VK_RIGHT).Held Then m_path.Points(m_selectedPoint).X += 30 * elapsedTime
    If m_keys(VK_UP).Held Then m_path.Points(m_selectedPoint).Y -= 30 * elapsedTime
    If m_keys(VK_DOWN).Held Then m_path.Points(m_selectedPoint).Y += 30 * elapsedTime
    If m_keys(AscW("A")).Held Then m_marker -= 5.0 * elapsedTime
    If m_keys(AscW("S")).Held Then m_marker += 5.0 * elapsedTime

    If m_marker > m_path.Points.Count - 1 Then m_marker -= m_path.Points.Count
    If m_marker < 0 Then m_marker += m_path.Points.Count

    ' Draw Spline
    For t = 0.0 To m_path.Points.Count Step 0.005
      Dim pos = m_path.GetSplinePoint(t, True)
      Draw(pos.X, pos.Y)
    Next

    ' Draw Control Points
    For i = 0 To m_path.Points.Count - 1
      Fill(m_path.Points(i).X - 1, m_path.Points(i).Y - 1, m_path.Points(i).X + 2, m_path.Points(i).Y + 2, PIXEL_SOLID, FG_RED)
      DrawString(m_path.Points(i).X, m_path.Points(i).Y, CStr(i))
    Next

    ' Highlight control point
    Fill(m_path.Points(m_selectedPoint).X - 1, m_path.Points(m_selectedPoint).Y - 1, m_path.Points(m_selectedPoint).X + 2, m_path.Points(m_selectedPoint).Y + 2, PIXEL_SOLID, FG_YELLOW)
    DrawString(m_path.Points(m_selectedPoint).X, m_path.Points(m_selectedPoint).Y, CStr(m_selectedPoint))

    ' Draw agent to demonstrate gradient
    Dim p1 = m_path.GetSplinePoint(m_marker, True)
    Dim g1 = m_path.GetSplineGradient(m_marker, True)
    Dim r = Math.Atan2(-g1.Y, g1.X)
    DrawLine(5.0F * Math.Sin(r) + p1.X, 5.0F * Math.Cos(r) + p1.Y, -5.0F * Math.Sin(r) + p1.X, -5.0F * Math.Cos(r) + p1.Y, PIXEL_SOLID, FG_BLUE)

    Return True

  End Function

End Class

Class Point2D
  Public Property X As Double
  Public Property Y As Double
  Public Sub New()
  End Sub
  Public Sub New(x As Double, y As Double)
    Me.X = x
    Me.Y = y
  End Sub
End Class

Class Spline

  Public Property Points As New List(Of Point2D)

  Public Function GetSplinePoint(t As Double, Optional bLooped As Boolean = False) As Point2D

    Dim p0, p1, p2, p3 As Integer

    If Not bLooped Then
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

  Public Function GetSplineGradient(t As Double, Optional bLooped As Boolean = False) As Point2D

    Dim p0, p1, p2, p3 As Integer

    If Not bLooped Then
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

End Class