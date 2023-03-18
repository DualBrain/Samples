' Inspired by: "Code-It-Yourself! Asteroids (Simple C++ and Maths!)" -- javidx9
' https://youtu.be/QgDR8LrRZhk

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports System.Xml

Module Program

  Sub Main() 'args As String())
    Dim game As New Asteroids
    game.ConstructConsole(160, 100, 10, 10)
    game.Start()
  End Sub

End Module

Class SpaceObject
  Public Property X As Single
  Public Property Y As Single
  Public Property Dx As Single
  Public Property Dy As Single
  Public Property Size As Integer
  Public Property Angle As Single
  Public Sub New()
  End Sub
  Public Sub New(x As Single, y As Single, dx As Single, dy As Single, Optional size As Integer = 1, Optional angle As Single = 0.0)
    Me.X = x
    Me.Y = y
    Me.Dx = dx
    Me.Dy = dy
    Me.Size = size
    Me.Angle = angle
  End Sub
End Class

Class Asteroids
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_asteroids As New List(Of SpaceObject)
  Private m_bullets As New List(Of SpaceObject)
  Private m_player As SpaceObject
  Private m_score As Integer

  Private m_modelShip As List(Of (Single, Single))
  Private m_modelAstroid As List(Of (Single, Single))

  Private m_dead As Boolean = False

  Public Overrides Function OnUserCreate() As Boolean

    m_modelShip = New List(Of (Single, Single)) From {(0.0F, -5.0F),
                                                      (-2.5F, +2.5F),
                                                      (+2.5F, +2.5F)} ' A simple Isoceles Triangle

    m_modelAstroid = New List(Of (Single, Single))
    Dim verts = 20
    For i = 0 To verts - 1
      Dim radius = CSng(((Rand / RAND_MAX) * 0.4) + 0.8)
      Dim a = (i / verts) * 6.28318
      m_modelAstroid.Add((CSng(radius * Math.Sin(a)), CSng(radius * Math.Cos(a))))
    Next

    ResetGame()

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    If m_dead Then
      ResetGame()
    End If

    Cls()

    ' Steer
    If m_keys(VK_LEFT).bHeld Then m_player.Angle -= 5.0F * elapsedTime
    If m_keys(VK_RIGHT).bHeld Then m_player.Angle += 5.0F * elapsedTime

    ' Thrust
    If m_keys(VK_UP).bHeld Then
      ' ACCELERATION changes VELOCITY (with respect to time)
      m_player.Dx += CSng(Math.Sin(m_player.Angle) * 20 * elapsedTime)
      m_player.Dy += -CSng(Math.Cos(m_player.Angle) * 20 * elapsedTime)
    End If

    ' VELOCITY changes POSITION (with respect to time)
    m_player.X += m_player.Dx * elapsedTime
    m_player.Y += m_player.Dy * elapsedTime

    ' Keep ship in gamespace
    WrapCoordinates(m_player.X, m_player.Y, m_player.X, m_player.Y)

    ' Check ship collision with asteroids
    For Each a In m_asteroids
      If IsPointInsideCircle(a.X, a.Y, a.Size, m_player.X, m_player.Y) Then
        m_dead = True ' Uh oh...
      End If
    Next

    ' Fire Bullet in direction of player
    If m_keys(VK_SPACE).Released Then
      m_bullets.Add(New SpaceObject(m_player.X, m_player.Y, 50.0F * CSng(Math.Sin(m_player.Angle)), -50.0F * CSng(Math.Cos(m_player.Angle))))
    End If

    ' Update and draw asteroids
    For Each a In m_asteroids
      a.X += a.Dx * elapsedTime
      a.Y += a.Dy * elapsedTime
      a.Angle += 0.5F * elapsedTime
      WrapCoordinates(a.X, a.Y, a.X, a.Y)
      DrawWireFrameModel(m_modelAstroid, a.X, a.Y, a.Angle, a.Size, FG_YELLOW)
    Next

    Dim newAstroids = New List(Of SpaceObject)

    ' Update and draw bullets
    For Each b In m_bullets
      b.X += b.Dx * elapsedTime
      b.Y += b.Dy * elapsedTime
      WrapCoordinates(b.X, b.Y, b.X, b.Y)
      Draw(b.X, b.Y)

      For Each a In m_asteroids
        If IsPointInsideCircle(a.X, a.Y, a.Size, b.X, b.Y) Then
          ' Astroid hit
          b.X = -100 ' set to off screen coord so it will be removed (below, outside of iterator)
          If a.Size > 4 Then
            ' Create two child astroids
            Dim angle1 = (Rand / RAND_MAX) * 6.283185
            Dim angle2 = (Rand / RAND_MAX) * 6.283185
            newAstroids.Add(New SpaceObject(a.X, a.Y, 10 * CSng(Math.Sin(angle1)), 10 * CSng(Math.Cos(angle1)), a.Size >> 1, 0.0))
            newAstroids.Add(New SpaceObject(a.X, a.Y, 10 * CSng(Math.Sin(angle2)), 10 * CSng(Math.Cos(angle2)), a.Size >> 1, 0.0))
          End If
          a.X = -100 ' set to off screen coord so it will be removed (below, outside of iterator)
          m_score += 100
        End If
      Next

    Next

    ' Append new astroids to existing vector
    For Each a In newAstroids
      m_asteroids.Add(a)
    Next

    ' Remove bullets that have gone off screen
    If m_bullets.Any Then
      m_bullets.RemoveAll(Function(o) o.X < 1 Or o.Y < 1 Or o.X >= ScreenWidth() - 1 Or o.Y >= ScreenHeight() - 1)
    End If

    If m_asteroids.Any Then
      m_asteroids.RemoveAll(Function(o) o.X < 0)
    End If

    If Not m_asteroids.Any Then
      m_score += 1000

      ' add them 90 degress left and right to the player, their coordinates
      ' be wrapped by the next astroid update
      m_asteroids.Add(New SpaceObject(30 * CSng(Math.Sin(m_player.Angle - 3.14159 / 2)),
                                           30 * CSng(Math.Cos(m_player.Angle - 3.14159 / 2)),
                                           10 * CSng(Math.Sin(m_player.Angle)),
                                           10 * CSng(Math.Cos(m_player.Angle)),
                                           16, 0))
      m_asteroids.Add(New SpaceObject(30 * CSng(Math.Sin(m_player.Angle + 3.14159 / 2)),
                                           30 * CSng(Math.Cos(m_player.Angle + 3.14159 / 2)),
                                           10 * CSng(Math.Sin(-m_player.Angle)),
                                           10 * CSng(Math.Cos(-m_player.Angle)),
                                           16, 0))

    End If

#Region "Simple Rotation Code - Moved to ConsoleGameEngine"
    '' Draw Ship?
    'Dim mx() = {0, -2.5, 2.5} ' Ship Model Vertices
    'Dim my() = {-5.5, 2.5, 2.5}

    'Dim sx(2), sy(2) As Double

    '' Rotate
    'For i = 0 To 2
    '  sx(i) = mx(i) * Math.Cos(m_player.Angle) - my(i) * Math.Sin(m_player.Angle)
    '  sy(i) = mx(i) * Math.Sin(m_player.Angle) + my(i) * Math.Cos(m_player.Angle)
    'Next

    '' Translate
    'For i = 0 To 2
    '  sx(i) = sx(i) + m_player.X
    '  sy(i) = sy(i) + m_player.Y
    'Next

    '' Draw Closed Polygon
    'For i = 0 To 3
    '  Dim j = i + 1
    '  DrawLine(sx(i Mod 3), sy(i Mod 3), sx(j Mod 3), sy(j Mod 3))
    'Next
#End Region
    DrawWireFrameModel(m_modelShip, m_player.X, m_player.Y, m_player.Angle)

    ' Draw Score
    DrawString(2, 2, $"SCORE: {m_score}")

    Return True

  End Function

  Private Sub ResetGame()

    m_asteroids.Clear()
    m_bullets.Clear()

    m_asteroids.Add(New SpaceObject With {.X = 20, .Y = 20, .Dx = 8, .Dy = -6, .Size = 16, .Angle = 0.0})
    m_asteroids.Add(New SpaceObject With {.X = 100, .Y = 20, .Dx = -5, .Dy = 3, .Size = 16, .Angle = 0.0})

    m_player = New SpaceObject()
    m_player.X = ScreenWidth() \ 2
    m_player.Y = ScreenHeight() \ 2
    m_player.Dx = 0
    m_player.Dy = 0
    m_player.Angle = 0.0

    m_dead = False
    m_score = 0

  End Sub

  Private Function IsPointInsideCircle(cx As Double, cy As Double, radius As Double, x As Double, y As Double) As Boolean
    Return Math.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy)) < radius
  End Function

  Private Sub WrapCoordinates(ix As Single, iy As Single, ByRef ox As Single, ByRef oy As Single)
    ox = ix
    oy = iy
    If ix < 0.0 Then ox = ix + ScreenWidth()
    If ix >= ScreenWidth() Then ox = ix - ScreenWidth()
    If iy < 0.0 Then oy = iy + ScreenHeight()
    If iy >= ScreenHeight() Then oy = iy - ScreenHeight()
  End Sub

  Public Overrides Sub Draw(x As Integer, y As Integer, Optional c As Integer = 9608, Optional col As Integer = 15)
    Dim fx, fy As Single
    WrapCoordinates(x, y, fx, fy)
    x = CInt(fx) : y = CInt(fy)
    MyBase.Draw(x, y, c, col)
  End Sub

  ' The following exists in ConsoleGameEngine.
  'Private Sub DrawWireFrameModel(vecModelCoordinates As List(Of (X As Double, Y As Double)), x As Single, y As Single, Optional ByVal r As Double = 0.0F, Optional s As Double = 1.0F, Optional col As Integer = FG_WHITE)

  '  ' Create translated model vector of coordinate pairs
  '  Dim transformedCoordinates As New List(Of (X As Double, Y As Double))
  '  Dim verts = vecModelCoordinates.Count
  '  For Each entry In vecModelCoordinates
  '    transformedCoordinates.Add((entry.X, entry.Y))
  '  Next

  '  ' Rotate
  '  For i = 0 To verts - 1
  '    transformedCoordinates(i) = (vecModelCoordinates(i).X * Math.Cos(r) - vecModelCoordinates(i).Y * Math.Sin(r),
  '                                       vecModelCoordinates(i).X * Math.Sin(r) + vecModelCoordinates(i).Y * Math.Cos(r))
  '  Next

  '  ' Scale
  '  For i = 0 To verts - 1
  '    transformedCoordinates(i) = (transformedCoordinates(i).X * s,
  '                                       transformedCoordinates(i).Y * s)
  '  Next

  '  ' Translate
  '  For i = 0 To verts - 1
  '    transformedCoordinates(i) = (transformedCoordinates(i).X + x,
  '                                       transformedCoordinates(i).Y + y)
  '  Next

  '  ' Draw Closed Polygon
  '  For i = 0 To verts
  '    Dim j = (i + 1) Mod verts
  '    DrawLine(transformedCoordinates(i).X, transformedCoordinates(i).Y,
  '             transformedCoordinates(j).X, transformedCoordinates(j).Y, PIXEL_SOLID, col)
  '  Next

  'End Sub

End Class