' Inspired by "Programming Balls #2 Circle V Edges Collisions C++" -- @javidx9
' https://youtu.be/ebq7L2Wtbl4

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine

Module Program

  Sub Main() 'args As String())
    Dim game As New Balls
    If game.ConstructConsole(320, 240, 4, 4) <> 0 Then
      game.Start()
    Else
      Console.WriteLine("Could not construct console")
    End If
  End Sub

End Module

Class Balls
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private ReadOnly m_balls As New List(Of Ball)
  Private ReadOnly m_lines As New List(Of LineSegment)
  'Private ReadOnly m_modelCircle As New List(Of (Single, Single))
  Private m_selectedBall As Ball = Nothing
  Private m_spriteBalls() As Sprite
  Private m_selectedLine As LineSegment = Nothing
  Private m_selectedLineStart As Boolean = False

  Public Overrides Function OnUserCreate() As Boolean

    Dim ballRadius = 2.0F
    For i = 0 To 99
      AddBall(CSng(Rand / RAND_MAX) * ScreenWidth(), CSng(Rand / RAND_MAX) * ScreenHeight(), ballRadius)
    Next

    AddBall(28.0F, 33.0F, ballRadius * 3)
    AddBall(28.0F, 35.0F, ballRadius * 2)

    Dim lineRadius = 4.0F
    m_lines.Add(New LineSegment With {.Sx = 12.0F, .Sy = 4.0F, .Ex = 64.0F, .Ey = 4.0F, .Radius = lineRadius})
    m_lines.Add(New LineSegment With {.Sx = 76.0F, .Sy = 4.0F, .Ex = 132.0F, .Ey = 4.0F, .Radius = lineRadius})
    m_lines.Add(New LineSegment With {.Sx = 12.0F, .Sy = 68.0F, .Ex = 64.0F, .Ey = 68.0F, .Radius = lineRadius})
    m_lines.Add(New LineSegment With {.Sx = 76.0F, .Sy = 68.0F, .Ex = 132.0F, .Ey = 68.0F, .Radius = lineRadius})
    m_lines.Add(New LineSegment With {.Sx = 4.0F, .Sy = 12.0F, .Ex = 4.0F, .Ey = 60.0F, .Radius = lineRadius})
    m_lines.Add(New LineSegment With {.Sx = 140.0F, .Sy = 12.0F, .Ex = 140.0F, .Ey = 60.0F, .Radius = lineRadius})

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Dim doCirclesOverlap = Function(x1 As Single, y1 As Single, r1 As Single, x2 As Single, y2 As Single, r2 As Single) As Boolean
                             Return Math.Abs((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)) <= (r1 + r2) * (r1 + r2)
                           End Function

    Dim isPointInCircle = Function(x1 As Single, y1 As Single, r1 As Single, px As Single, py As Single) As Boolean
                            Return Math.Abs((x1 - px) * (x1 - px) + (y1 - py) * (y1 - py)) < (r1 * r1)
                          End Function

    If m_mouse(0).Pressed Then
      ' Check for selected ball
      m_selectedBall = Nothing
      For Each ball In m_balls
        If isPointInCircle(ball.Px, ball.Py, ball.Radius, m_mousePosX, m_mousePosY) Then
          m_selectedBall = ball
          Exit For
        End If
      Next
      ' Check for selected line segment end
      m_selectedLine = Nothing
      For Each line In m_lines
        If isPointInCircle(line.Sx, line.Sy, line.Radius, m_mousePosX, m_mousePosY) Then
          m_selectedLine = line
          m_selectedLineStart = True
          Exit For
        End If
        If isPointInCircle(line.Ex, line.Ey, line.Radius, m_mousePosX, m_mousePosY) Then
          m_selectedLine = line
          m_selectedLineStart = False
          Exit For
        End If
      Next
    End If

    If m_mouse(0).Held Then
      If m_selectedLine IsNot Nothing Then
        If m_selectedLineStart Then
          m_selectedLine.Sx = GetMouseX()
          m_selectedLine.Sy = GetMouseY()
        Else
          m_selectedLine.Ex = GetMouseX()
          m_selectedLine.Ey = GetMouseY()
        End If
      End If
    End If

    If m_mouse(0).Released Then
      If m_selectedBall IsNot Nothing Then
        ' Apply velocity
        m_selectedBall.Vx = 5.0F * ((m_selectedBall.Px) - m_mousePosX)
        m_selectedBall.Vy = 5.0F * ((m_selectedBall.Py) - m_mousePosY)
      End If
      m_selectedBall = Nothing
      m_selectedLine = Nothing
    End If

    If m_mouse(1).Held Then
      For Each ball In m_balls
        ball.Vx += (m_mousePosX - ball.Px) * 0.01F
        ball.Vy += (m_mousePosY - ball.Py) * 0.01F
      Next
    End If

    Dim collidingPairs = New List(Of (Ball, Ball))
    Dim fakeBalls As New List(Of Ball)

    ' Threshold indicating stability of object
    Dim stable = 0.05F

    ' Multiple simulation updates with small time steps permit more accurate physics
    ' and realistic results at the expense of CPU time of course
    Dim simulationUpdates = 4

    ' Multiple collision trees require more steps to resolve. Normally we would
    ' continue simulation until the object has no simulation time left for this
    ' epoch, however this is risky as the system may never find stability, so we
    ' can clamp it here
    Dim maxSimulationSteps = 15

    ' Break up the frame elapsed time into smaller deltas for each simulation update
    Dim simElapsedTime = elapsedTime / simulationUpdates

    ' Main simulation loop
    For i = 0 To simulationUpdates - 1

      ' Set all balls time to maximum for this epoch
      For Each ball In m_balls
        ball.SimTimeRemaining = simElapsedTime
      Next

      ' Erode simulation time on a per object basis, depending upon what happens
      ' to it during its journey through this epoch
      For j = 0 To maxSimulationSteps - 1

        ' Update Ball Positions
        For Each ball In m_balls

          If ball.SimTimeRemaining > 0.0F Then

            ball.Ox = ball.Px
            ball.Oy = ball.Py

            ball.Ax = -ball.Vx * 0.8F
            ball.Ay = -ball.Vy * 0.8F + 100.0F

            ball.Vx += ball.Ax * ball.SimTimeRemaining
            ball.Vy += ball.Ay * ball.SimTimeRemaining

            ball.Px += ball.Vx * ball.SimTimeRemaining
            ball.Py += ball.Vy * ball.SimTimeRemaining

            ' Crudely wrap balls to screen - note this cause issues when collisions occur on screen boundaries
            If ball.Px < 0 Then ball.Px += ScreenWidth()
            If ball.Px >= ScreenWidth() Then ball.Px -= ScreenWidth()
            If ball.Py < 0 Then ball.Py += ScreenHeight()
            If ball.Py >= ScreenHeight() Then ball.Py -= ScreenHeight()

            ' Stop ball when velocity is negligible
            If Math.Abs(ball.Vx * ball.Vx + ball.Vy * ball.Vy) < stable Then
              ball.Vx = 0
              ball.Vy = 0
            End If

          End If

        Next

        ' Work out static collisions with walls and displace balls so no overlaps
        For Each ball In m_balls

          Dim fDeltaTime = ball.SimTimeRemaining

          ' Against Edges
          For Each edge In m_lines

            ' Check that line formed by velocity vector, intersects with line segment
            Dim fLineX1 = edge.Ex - edge.Sx
            Dim fLineY1 = edge.Ey - edge.Sy

            Dim fLineX2 = ball.Px - edge.Sx
            Dim fLineY2 = ball.Py - edge.Sy

            Dim fEdgeLength = fLineX1 * fLineX1 + fLineY1 * fLineY1

            ' This is nifty - It uses the DP of the line segment vs the line to the object, to work out
            ' how much of the segment is in the "shadow" of the object vector. The min and max clamp
            ' this to lie between 0 and the line segment length, which is then normalised. We can
            ' use this to calculate the closest point on the line segment
            Dim t = Math.Max(0, Math.Min(fEdgeLength, (fLineX1 * fLineX2 + fLineY1 * fLineY2))) / fEdgeLength

            ' Which we do here
            Dim fClosestPointX = edge.Sx + t * fLineX1
            Dim fClosestPointY = edge.Sy + t * fLineY1

            ' And once we know the closest point, we can check if the ball has collided with the segment in the
            ' same way we check if two balls have collided
            Dim fDistance = CSng(Math.Sqrt((ball.Px - fClosestPointX) * (ball.Px - fClosestPointX) + (ball.Py - fClosestPointY) * (ball.Py - fClosestPointY)))

            If fDistance <= (ball.Radius + edge.Radius) Then

              ' Collision has occurred - treat collision point as a ball that cannot move. To make this
              ' compatible with the dynamic resolution code below, we add a fake ball with an infinite mass
              ' so it behaves like a solid object when the momentum calculations are performed
              Dim fakeball = New Ball()
              fakeball.radius = edge.Radius
              fakeball.mass = ball.Mass * 0.8F
              fakeball.px = fClosestPointX
              fakeball.py = fClosestPointY
              fakeball.vx = -ball.Vx ' We will use these later to allow the lines to impart energy into ball
              fakeball.vy = -ball.Vy ' if the lines are moving, i.e. like pinball flippers

              ' Store Fake Ball
              fakeBalls.Add(fakeball)

              ' Add collision to vector of collisions for dynamic resolution
              collidingPairs.Add((ball, fakeball))

              ' Calculate displacement required
              Dim fOverlap = 1.0F * (fDistance - ball.Radius - fakeball.Radius)

              ' Displace Current Ball away from collision
              ball.Px -= fOverlap * (ball.Px - fakeball.px) / fDistance
              ball.Py -= fOverlap * (ball.Py - fakeball.Py) / fDistance

            End If

          Next

          ' Against other balls
          For Each target In m_balls

            If ball.Id <> target.Id Then ' Do not check against self

              If doCirclesOverlap(ball.Px, ball.Py, ball.Radius, target.Px, target.Py, target.Radius) Then

                ' Collision has occurred
                collidingPairs.Add((ball, target))

                ' Distance between ball centers
                Dim fDistance = CSng(Math.Sqrt((ball.Px - target.Px) ^ 2 + (ball.Py - target.Py) ^ 2))

                ' Calculate displacement required
                Dim fOverlap = 0.5F * (fDistance - ball.Radius - target.Radius)

                ' Displace Current Ball away from collision
                ball.Px -= fOverlap * (ball.Px - target.Px) / fDistance
                ball.Py -= fOverlap * (ball.Py - target.Py) / fDistance

                ' Displace Target Ball away from collision - Note, this should affect the timing of the target ball
                ' and it does, but this is absorbed by the target ball calculating its own time delta later on
                target.Px += fOverlap * (ball.Px - target.Px) / fDistance
                target.Py += fOverlap * (ball.Py - target.Py) / fDistance

              End If

            End If

          Next

          ' Time displacement - we knew the velocity of the ball, so we can estimate the distance it should have covered
          ' however due to collisions it could not do the full distance, so we look at the actual distance to the collision
          ' point and calculate how much time that journey would have taken using the speed of the object. Therefore
          ' we can now work out how much time remains in that timestep.
          Dim fIntendedSpeed = CSng(Math.Sqrt(ball.Vx * ball.Vx + ball.Vy * ball.Vy))
          Dim fIntendedDistance = fIntendedSpeed * ball.SimTimeRemaining
          Dim fActualDistance = CSng(Math.Sqrt((ball.Px - ball.Ox) * (ball.Px - ball.Ox) + (ball.Py - ball.Oy) * (ball.Py - ball.Oy)))
          Dim fActualTime = fActualDistance / fIntendedSpeed

          ' After static resolution, there may be some time still left for this epoch, so allow simulation to continue
          ball.SimTimeRemaining = ball.SimTimeRemaining - fActualTime

        Next

        ' Now work out dynamic collisions
        Dim fEfficiency = 1.0F
        For Each c In collidingPairs

          Dim b1 = c.Item1
          Dim b2 = c.Item2

          ' Distance between balls
          Dim fDistance = CSng(Math.Sqrt((b1.Px - b2.Px) ^ 2 + (b1.Py - b2.Py) ^ 2))

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
          Dim m1 = fEfficiency * (dpNorm1 * (b1.Mass - b2.Mass) + 2.0F * b2.Mass * dpNorm2) / (b1.Mass + b2.Mass)
          Dim m2 = fEfficiency * (dpNorm2 * (b2.Mass - b1.Mass) + 2.0F * b1.Mass * dpNorm1) / (b1.Mass + b2.Mass)

          ' Update ball velocities
          b1.Vx = tx * dpTan1 + nx * m1
          b1.Vy = ty * dpTan1 + ny * m1
          b2.Vx = tx * dpTan2 + nx * m2
          b2.Vy = ty * dpTan2 + ny * m2

        Next

        ' Remove collisions
        collidingPairs.Clear()

        ' Remove fake balls
        fakeBalls.Clear()

      Next

    Next

    '' Update Ball Positions
    'For Each ball In m_balls

    '  ' Add Drag to emulate rolling friction
    '  ball.Ax = -ball.Vx * 0.8F
    '  ball.Ay = -ball.Vy * 0.8F

    '  ' Update ball physics
    '  ball.Vx += ball.Ax * elapsedTime
    '  ball.Vy += ball.Ay * elapsedTime
    '  ball.Px += ball.Vx * elapsedTime
    '  ball.Py += ball.Vy * elapsedTime

    '  ' Wrap the balls around the screen
    '  If ball.Px < 0 Then ball.Px += ScreenWidth()
    '  If ball.Px >= ScreenWidth() Then ball.Px -= ScreenWidth()
    '  If ball.Py < 0 Then ball.Py += ScreenHeight()
    '  If ball.Py >= ScreenHeight() Then ball.Py -= ScreenHeight()

    '  ' Clamp velocity near zero
    '  If Math.Abs(ball.Vx * ball.Vx + ball.Vy * ball.Vy) < 0.01F Then
    '    ball.Vx = 0
    '    ball.Vy = 0
    '  End If

    'Next

    '' Static collisions, i.e. overlap
    'For Each ball In m_balls
    '  For Each target In m_balls
    '    If ball.Id <> target.Id Then
    '      If doCirclesOverlap(ball.Px, ball.Py, ball.Radius, target.Px, target.Py, target.Radius) Then
    '        ' Collision has occured
    '        collidingPairs.Add((ball, target))
    '        ' Distance between ball centers
    '        Dim distance = CSng(Math.Sqrt((ball.Px - target.Px) * (ball.Px - target.Px) + (ball.Py - target.Py) * (ball.Py - target.Py)))
    '        ' Calculate displacement required
    '        Dim overlap = 0.5F * (distance - ball.Radius - target.Radius)
    '        ' Display Current Ball away from collision
    '        ball.Px -= overlap * (ball.Px - target.Px) / distance
    '        ball.Py -= overlap * (ball.Py - target.Py) / distance
    '        ' Display Target Ball away from collision
    '        target.Px += overlap * (ball.Px - target.Px) / distance
    '        target.Py += overlap * (ball.Py - target.Py) / distance
    '      End If
    '    End If
    '  Next
    'Next

    '' Now work out dynamic collisions
    'For Each c In collidingPairs

    '  Dim b1 = c.Item1
    '  Dim b2 = c.Item2

    '  ' Distance between balls
    '  Dim fDistance = CSng(Math.Sqrt((b1.Px - b2.Px) * (b1.Px - b2.Px) + (b1.Py - b2.Py) * (b1.Py - b2.Py)))

    '  ' Normal
    '  Dim nx = (b2.Px - b1.Px) / fDistance
    '  Dim ny = (b2.Py - b1.Py) / fDistance

    '  ' Tangent
    '  Dim tx = -ny
    '  Dim ty = nx

    '  ' Dot Product Tangent
    '  Dim dpTan1 = b1.Vx * tx + b1.Vy * ty
    '  Dim dpTan2 = b2.Vx * tx + b2.Vy * ty

    '  ' Dot Product Normal
    '  Dim dpNorm1 = b1.Vx * nx + b1.Vy * ny
    '  Dim dpNorm2 = b2.Vx * nx + b2.Vy * ny

    '  ' Conservation of momentum in 1D
    '  Dim m1 = (dpNorm1 * (b1.Mass - b2.Mass) + 2.0F * b2.Mass * dpNorm2) / (b1.Mass + b2.Mass)
    '  Dim m2 = (dpNorm2 * (b2.Mass - b1.Mass) + 2.0F * b1.Mass * dpNorm1) / (b1.Mass + b2.Mass)

    '  ' Update ball velocities
    '  b1.Vx = tx * dpTan1 + nx * m1
    '  b1.Vy = ty * dpTan1 + ny * m1
    '  b2.Vx = tx * dpTan2 + nx * m2
    '  b2.Vy = ty * dpTan2 + ny * m2

    '  ' Wikipedia Version - Maths is smarter but same
    '  ' Dim kx = (b1.vx - b2.vx)
    '  ' Dim ky = (b1.vy - b2.vy)
    '  ' Dim p = 2.0F * (nx * kx + ny * ky) / (b1.mass + b2.mass)
    '  ' b1.vx = b1.vx - p * b2.mass * nx
    '  ' b1.vy = b1.vy - p * b2.mass * ny
    '  ' b2.vx = b2.vx + p * b1.mass * nx
    '  ' b2.vy = b2.vy + p * b1.mass * ny

    'Next

    ' Clear Screen
    Cls()

    ' Draw Lines
    For Each line In m_lines

      FillCircle(line.Sx, line.Sy, line.Radius, PIXEL_HALF, FG_WHITE)
      FillCircle(line.Ex, line.Ey, line.Radius, PIXEL_HALF, FG_WHITE)

      Dim nx = -(line.Ey - line.Sy)
      Dim ny = (line.Ex - line.Sx)
      Dim d = CSng(Math.Sqrt(nx * nx + ny * ny))
      nx /= d
      ny /= d

      DrawLine(line.Sx + nx * line.Radius, line.Sy + ny * line.Radius, line.Ex + nx * line.Radius, line.Ey + ny * line.Radius)
      DrawLine(line.Sx - nx * line.Radius, line.Sy - ny * line.Radius, line.Ex - nx * line.Radius, line.Ey - ny * line.Radius)

    Next

    ' Draw Balls
    For Each ball In m_balls
      FillCircle(ball.Px, ball.Py, ball.Radius, PIXEL_SOLID, FG_RED)

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

  Private Sub AddBall(x As Single, y As Single, Optional r As Single = 5.0F, Optional s As Integer = 0)

    Dim b = New Ball
    b.Px = x : b.Py = y
    b.Vx = 0 : b.Vy = 0
    b.Ax = 0 : b.Ay = 0
    b.Ox = 0 : b.Oy = 0
    b.Radius = r
    b.Mass = r * 10.0F
    b.Friction = 0.0F
    b.Score = s
    b.SimTimeRemaining = 0.0F
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
  Public Property Ox As Single
  Public Property Oy As Single
  Public Property Radius As Single
  Public Property Mass As Single
  Public Property Friction As Single
  Public Property Score As Integer
  Public Property Id As Integer
  Public Property SimTimeRemaining As Single
End Class

Class LineSegment
  Public Sx As Single
  Public Sy As Single
  Public Ex As Single
  Public Ey As Single
  Public Radius As Single
End Class