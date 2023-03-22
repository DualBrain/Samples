' Inspired by "Programming Balls #1 Circle Vs Circle Collisions C++" -- @javidx9
' https://youtu.be/LPzyNOHY3A4

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour

Module Program

  Sub Main() 'args As String())
    Dim game As New Balls
    If game.ConstructConsole(160, 120, 8, 8) <> 0 Then
      game.Start()
    Else
      Console.WriteLine("Could not construct console")
    End If
  End Sub

End Module

Class Balls
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private ReadOnly m_modelCircle As New List(Of (Single, Single))
  Private ReadOnly m_balls As New List(Of Ball)
  Private m_selectedBall As Ball = Nothing

  Public Overrides Function OnUserCreate() As Boolean

    m_modelCircle.Add((0.0F, 0.0F))
    Dim points = 20
    For i = 0 To points - 1
      m_modelCircle.Add((CSng(Math.Cos(i / (points - 1) * 2.0F * 3.14159F)), CSng(Math.Sin(i / (points - 1) * 2.0F * 3.14159F))))
    Next

    'Dim defaultRad = 8.0F
    'AddBall(ScreenWidth() * 0.25F, ScreenHeight() * 0.5F, defaultRad)
    'AddBall(ScreenWidth() * 0.75F, ScreenHeight() * 0.5F, defaultRad)

    ' Add 10 Random Balls
    For i = 0 To 9
      AddBall(Rand Mod ScreenWidth(), Rand Mod ScreenHeight(), Rand Mod 16 + 2)
    Next

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Dim doCirclesOverlap = Function(x1 As Single, y1 As Single, r1 As Single, x2 As Single, y2 As Single, r2 As Single) As Boolean
                             'Return Math.Sqrt((x1 - x2) * (x1 - x2) + (y2 - y2) * (y1 - y2)) < (r1 + r2)
                             Return Math.Abs((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)) <= (r1 + r2) * (r1 + r2)
                           End Function

    Dim isPointInCircle = Function(x1 As Single, y1 As Single, r1 As Single, px As Single, py As Single) As Boolean
                            Return Math.Abs((x1 - px) * (x1 - px) + (y1 - py) * (y1 - py)) < (r1 * r1)
                          End Function

    If m_mouse(0).Pressed OrElse m_mouse(1).Pressed Then
      m_selectedBall = Nothing
      For Each ball In m_balls
        If isPointInCircle(ball.Px, ball.Py, ball.Radius, m_mousePosX, m_mousePosY) Then
          m_selectedBall = ball
          Exit For
        End If
      Next
    End If

    If m_mouse(0).Held Then
      If m_selectedBall IsNot Nothing Then
        m_selectedBall.Px = m_mousePosX
        m_selectedBall.Py = m_mousePosY
      End If
    End If

    If m_mouse(0).Released Then
      m_selectedBall = Nothing
    End If

    If m_mouse(1).Released Then
      If m_selectedBall IsNot Nothing Then
        ' Apply velocity
        m_selectedBall.Vx = 5.0F * ((m_selectedBall.Px) - m_mousePosX)
        m_selectedBall.Vy = 5.0F * ((m_selectedBall.Py) - m_mousePosY)
      End If
      m_selectedBall = Nothing
    End If

    Dim collidingPairs = New List(Of (Ball, Ball))

    ' Update Ball Positions
    For Each ball In m_balls

      ' Add Drag to emulate rolling friction
      ball.Ax = -ball.Vx * 0.8F
      ball.Ay = -ball.Vy * 0.8F

      ' Update ball physics
      ball.Vx += ball.Ax * elapsedTime
      ball.Vy += ball.Ay * elapsedTime
      ball.Px += ball.Vx * elapsedTime
      ball.Py += ball.Vy * elapsedTime

      ' Wrap the balls around the screen
      If ball.Px < 0 Then ball.Px += ScreenWidth()
      If ball.Px >= ScreenWidth() Then ball.Px -= ScreenWidth()
      If ball.Py < 0 Then ball.Py += ScreenHeight()
      If ball.Py >= ScreenHeight() Then ball.Py -= ScreenHeight()

      ' Clamp velocity near zero
      If Math.Abs(ball.Vx * ball.Vx + ball.Vy * ball.Vy) < 0.01F Then
        ball.Vx = 0
        ball.Vy = 0
      End If

    Next

    ' Static collisions, i.e. overlap
    For Each ball In m_balls
      For Each target In m_balls
        If ball.Id <> target.Id Then
          If doCirclesOverlap(ball.Px, ball.Py, ball.Radius, target.Px, target.Py, target.Radius) Then
            ' Collision has occured
            collidingPairs.Add((ball, target))
            ' Distance between ball centers
            Dim distance = CSng(Math.Sqrt((ball.Px - target.Px) * (ball.Px - target.Px) + (ball.Py - target.Py) * (ball.Py - target.Py)))
            ' Calculate displacement required
            Dim overlap = 0.5F * (distance - ball.Radius - target.Radius)
            ' Display Current Ball away from collision
            ball.Px -= overlap * (ball.Px - target.Px) / distance
            ball.Py -= overlap * (ball.Py - target.Py) / distance
            ' Display Target Ball away from collision
            target.Px += overlap * (ball.Px - target.Px) / distance
            target.Py += overlap * (ball.Py - target.Py) / distance
          End If
        End If
      Next
    Next

    ' Now work out dynamic collisions
    For Each c In collidingPairs

      Dim b1 = c.Item1
      Dim b2 = c.Item2

      ' Distance between balls
      Dim fDistance = CSng(Math.Sqrt((b1.Px - b2.Px) * (b1.Px - b2.Px) + (b1.Py - b2.Py) * (b1.Py - b2.Py)))

      ' Normal
      Dim nx = (b2.Px - b1.Px) / fDistance
      Dim ny = (b2.Py - b1.Py) / fDistance

      ' Tangent
      Dim tx = -ny
      Dim ty = nx

      ' Dot Product Tangent
      Dim dpTan1 = b1.Vx * tx + b1.Vy * ty
      Dim dpTan2 = b2.Vx * tx + b2.Vy * ty

      ' Dot Product Normal
      Dim dpNorm1 = b1.Vx * nx + b1.Vy * ny
      Dim dpNorm2 = b2.Vx * nx + b2.Vy * ny

      ' Conservation of momentum in 1D
      Dim m1 = (dpNorm1 * (b1.Mass - b2.Mass) + 2.0F * b2.Mass * dpNorm2) / (b1.Mass + b2.Mass)
      Dim m2 = (dpNorm2 * (b2.Mass - b1.Mass) + 2.0F * b1.Mass * dpNorm1) / (b1.Mass + b2.Mass)

      ' Update ball velocities
      b1.Vx = tx * dpTan1 + nx * m1
      b1.Vy = ty * dpTan1 + ny * m1
      b2.Vx = tx * dpTan2 + nx * m2
      b2.Vy = ty * dpTan2 + ny * m2

      ' Wikipedia Version - Maths is smarter but same
      ' Dim kx = (b1.vx - b2.vx)
      ' Dim ky = (b1.vy - b2.vy)
      ' Dim p = 2.0F * (nx * kx + ny * ky) / (b1.mass + b2.mass)
      ' b1.vx = b1.vx - p * b2.mass * nx
      ' b1.vy = b1.vy - p * b2.mass * ny
      ' b2.vx = b2.vx + p * b1.mass * nx
      ' b2.vy = b2.vy + p * b1.mass * ny

    Next

    ' Clear Screen
    Cls()

    ' Draw Balls
    For Each ball In m_balls
      DrawWireFrameModel(m_modelCircle, ball.Px, ball.Py, CSng(Math.Atan2(ball.Vy, ball.Vx)), ball.Radius, FG_WHITE)
    Next

    ' Draw static collissions
    For Each c In collidingPairs
      DrawLine(c.Item1.Px, c.Item1.Py, c.Item2.Px, c.Item2.Py, PIXEL_SOLID, FG_RED)
    Next

    ' Draw Cue
    If m_selectedBall IsNot Nothing Then
      DrawLine(m_selectedBall.Px, m_selectedBall.Py, m_mousePosX, m_mousePosY, PIXEL_SOLID, FG_BLUE)
    End If

    Return True

  End Function

  Private Sub AddBall(x As Single, y As Single, Optional r As Single = 5.0F)

    Dim b = New Ball
    b.Px = x : b.Py = y
    b.Vx = 0 : b.Vy = 0
    b.Ax = 0 : b.Ay = 0
    b.Radius = r
    b.Mass = r * 10.0F
    b.Id = m_balls.Count
    m_balls.Add(b)

  End Sub

End Class

Class Ball
  Public Property Px As Single
  Public Property Py As Single
  Public Property Vx As Single
  Public Property Vy As Single
  Public Property Ax As Single
  Public Property Ay As Single
  Public Property Radius As Single
  Public Property Mass As Single
  Public Property Id As Integer
End Class