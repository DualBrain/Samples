' Inspired by: "Code-It-Yourself! Worms Part #3" -- @javid
' https://youtu.be/NKK5tIRZqyQ

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine

Module Program

  Sub Main() 'args As String())
    Dim game As New Worms
    game.ConstructConsole(256, 160, 6, 6)
    game.Start()
  End Sub

End Module

Class Worms
  Inherits ConsoleGameEngine.ConsoleGameEngine

  ' Terrain size
  Private ReadOnly m_mapWidth As Integer = 1024
  Private ReadOnly m_mapHeight As Integer = 512
  Private m_map As Integer() = Nothing

  ' Camera coordinates
  Private m_cameraPosX As Single = 0.0F
  Private m_cameraPosY As Single = 0.0F
  Private m_targetCameraPosX As Single = 0.0F
  Private m_targetCameraPosY As Single = 0.0F

  ' list of things that exist in game world
  Private ReadOnly m_objects As New List(Of PhysicsObject)

  Private m_objectUnderControl As PhysicsObject = Nothing ' Pointer to object currently under control
  Private m_cameraTrackingObject As PhysicsObject = Nothing ' Pointer to object that camera should track

  ' Flags that govern/are set by game state machine
  Private m_zoomOut As Boolean = False                ' Render whole map
  Private m_gameIsStable As Boolean = False           ' All physics objects are stable
  Private m_enablePlayerControl As Boolean = False    ' The player is in control, keyboard input enabled
  Private m_enableComputerControl As Boolean = False  ' The AI is in control
  Private m_energizing As Boolean = False             ' Weapon is charging
  Private m_fireWeapon As Boolean = False             ' Weapon should be discharged
  Private m_showCountDown As Boolean = False          ' Display turn time counter on screen
  Private m_playerHasFired As Boolean = False         ' Weapon has been discharged

  Private m_energyLevel As Single = 0.0F              ' Energy accumulated through charging (player only)
  Private m_turnTime As Single = 0.0F                 ' Time left to take turn

  ' Vector to store teams
  Private ReadOnly m_teams As New List(Of TeamObject)

  ' Current team being controlled
  Private m_currentTeam As Integer = 0

  ' AI control flags
  Private m_aiJump As Boolean = False                 ' AI has pressed "JUMP" key
  Private m_aiAimLeft As Boolean = False              ' AI has pressed "AIM_LEFT" key
  Private m_aiAimRight As Boolean = False             ' AI has pressed "AIM_RIGHT" key
  Private m_aiEnergize As Boolean = False             ' AI has pressed "FIRE" key

  Private m_aiTargetAngle As Single = 0.0F            ' Angle AI should aim for
  Private m_aiTargetEnergy As Single = 0.0F           ' Energy level AI should aim for
  Private m_aiSafePosition As Single = 0.0F           ' X-Coordinate considered safe for AI to move to
  Private m_aiTargetWorm As WormObject = Nothing      ' Pointer to worm AI has selected as target
  Private m_aiTargetX As Single = 0.0F                ' Coordinates of target missle location
  Private m_aiTargetY As Single = 0.0F

  Private Enum GameState
    Reset
    GenerateTerrain
    GeneratingTerrain
    AllocateUnits
    AllocatingUnits
    StartPlay
    CameraMode
    GameOver1
    GameOver2
  End Enum

  Private m_gameState As GameState
  Private m_nextState As GameState

  Private Enum AiState
    AssessEnvironment
    Move
    ChooseTarget
    PositionForTarget
    Aim
    Fire
  End Enum

  Private m_aiState As AiState
  Private m_aiNextState As AiState

  Public Overrides Function OnUserCreate() As Boolean

    ' Create Map
    m_map = New Integer(m_mapWidth * m_mapHeight - 1) {}
    'Array.Clear(m_map, 0, m_mapWidth * m_mapHeight)

    ' Set initial states for state machines
    m_gameState = GameState.Reset
    m_nextState = GameState.Reset
    m_aiState = AiState.AssessEnvironment
    m_aiNextState = AiState.AssessEnvironment

    m_gameIsStable = False

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Tab key toggles between whole map view and up close view
    If m_keys(VK_TAB).Released Then m_zoomOut = Not m_zoomOut

    ' Mouse Edge Map Scroll
    Dim mapScrollSpeed = 400.0F
    If m_mousePosX < 5 Then m_cameraPosX -= mapScrollSpeed * elapsedTime
    If m_mousePosX > ScreenWidth() - 5 Then m_cameraPosX += mapScrollSpeed * elapsedTime
    If m_mousePosY < 5 Then m_cameraPosY -= mapScrollSpeed * elapsedTime
    If m_mousePosY > ScreenHeight() - 5 Then m_cameraPosY += mapScrollSpeed * elapsedTime

    ' Control Supervisor
    Select Case m_gameState

      Case GameState.Reset
        m_enablePlayerControl = False
        m_gameIsStable = False
        m_playerHasFired = False
        m_showCountDown = False
        m_nextState = GameState.GenerateTerrain

      Case GameState.GenerateTerrain
        m_zoomOut = True
        CreateMap()
        m_gameIsStable = False
        m_showCountDown = False
        m_nextState = GameState.GeneratingTerrain

      Case GameState.GeneratingTerrain
        m_showCountDown = False
        If m_gameIsStable Then m_nextState = GameState.AllocateUnits

      Case GameState.AllocateUnits
        ' Deploy teams
        Dim teams = 4
        Dim wormsPerTeam = 4
        ' Calculate spacing of worms and teams
        Dim spacePerTeam = CSng(m_mapWidth / teams)
        Dim spacePerWorm = spacePerTeam / (wormsPerTeam * 2.0F)
        ' Create teams
        For t = 0 To teams - 1
          m_teams.Add(New TeamObject)
          Dim teamMiddle = (spacePerTeam / 2.0F) + (t * spacePerTeam)
          For w = 0 To wormsPerTeam - 1
            Dim wormX = teamMiddle - ((spacePerWorm * wormsPerTeam) / 2.0F) + w * spacePerWorm
            Dim wormY = 0.0F
            ' Add worms to teams
            Dim worm = New WormObject(wormX, wormY) With {.TeamId = t}
            m_objects.Add(worm)
            m_teams(t).Members.Add(worm)
            m_teams(t).TeamSize = wormsPerTeam
          Next
          m_teams(t).CurrentMember = 0
        Next
        ' Select players first worm for control and camera tracking
        m_objectUnderControl = m_teams(0).Members(m_teams(0).CurrentMember)
        m_cameraTrackingObject = m_objectUnderControl
        m_showCountDown = False
        m_nextState = GameState.AllocatingUnits

      Case GameState.AllocatingUnits
        If m_gameIsStable Then
          m_enablePlayerControl = True
          m_enableComputerControl = False
          m_turnTime = 15.0F
          m_zoomOut = False
          m_nextState = GameState.StartPlay
        End If

      Case GameState.StartPlay
        m_showCountDown = True
        ' If player has discharged weapon, or turn time is up, move on to next state
        If m_playerHasFired OrElse m_turnTime <= 0.0F Then
          m_nextState = GameState.CameraMode
        End If

      Case GameState.CameraMode
        m_enableComputerControl = False
        m_enablePlayerControl = False
        m_playerHasFired = False
        m_showCountDown = False
        m_energyLevel = 0.0F
        If m_gameIsStable Then ' Once settled, choose next worm
          ' Get Next Team, if there is no next team, game is over
          Dim oldTeam = m_currentTeam
          Do
            m_currentTeam += 1
            m_currentTeam = m_currentTeam Mod m_teams.Count
          Loop While Not m_teams(m_currentTeam).IsTeamAlive()
          ' Lock controls if AI team is currently playing
          If m_currentTeam = 0 Then ' Player Team
            ' for all AI
            m_enablePlayerControl = False
            m_enableComputerControl = True
            ' for Human player.
            'm_enablePlayerControl = True
            'm_enableComputerControl = False
          Else ' AI Team
            m_enablePlayerControl = False
            m_enableComputerControl = True
          End If
          ' Set control and camer
          m_objectUnderControl = m_teams(m_currentTeam).GetNextMember()
          m_cameraTrackingObject = m_objectUnderControl
          m_turnTime = 15.0F
          m_zoomOut = False
          m_nextState = GameState.StartPlay
          ' If no different team could be found...
          If m_currentTeam = oldTeam Then
            ' ...Game is over, Current Team has won!
            m_nextState = GameState.GameOver1
          End If
        End If

      Case GameState.GameOver1 ' Zoom out and launch a load of missiles!
        m_enableComputerControl = False
        m_enablePlayerControl = True
        m_zoomOut = True
        m_showCountDown = False
        For i = 0 To 99
          Dim bombX = Rand Mod m_mapWidth
          Dim bombY = Rand Mod (m_mapHeight \ 2)
          m_objects.Add(New MissileObject(bombX, bombY, 0.0F, 0.5F))
        Next
        m_nextState = GameState.GameOver2

      Case GameState.GameOver2 ' Stay here and wait for chaos to settle
        m_enableComputerControl = False
        m_enablePlayerControl = False
        ' No exit from this state!

    End Select

    ' AI State Machine
    If m_enableComputerControl Then
      Select Case m_aiState
        Case AiState.AssessEnvironment
          Dim action = Rand Mod 3
          Select Case action
            Case 0 ' Play Defensive - move away from team
              ' Find nearest ally, walk away from them
              Dim fNearestAllyDistance = Single.PositiveInfinity
              Dim fDirection = 0.0F
              Dim origin = TryCast(m_objectUnderControl, WormObject)
              For Each w In m_teams(m_currentTeam).Members
                If w IsNot m_objectUnderControl Then
                  If Math.Abs(w.Px - origin.Px) < fNearestAllyDistance Then
                    fNearestAllyDistance = Math.Abs(w.Px - origin.Px)
                    fDirection = If((w.Px - origin.Px) < 0.0F, 1.0F, -1.0F)
                  End If
                End If
              Next
              If fNearestAllyDistance < 50.0F Then
                m_aiSafePosition = origin.Px + fDirection * 80.0F
              Else
                m_aiSafePosition = origin.Px
              End If
            Case 1 ' Play Ballsy - move towards middle
              Dim origin = TryCast(m_objectUnderControl, WormObject)
              Dim direction = If((CSng(m_mapWidth) / 2.0F - origin.Px) < 0.0F, -1.0F, 1.0F)
              m_aiSafePosition = origin.Px + direction * 200.0F
            Case 2 ' Play Dumb - don't move
              Dim origin = TryCast(m_objectUnderControl, WormObject)
              m_aiSafePosition = origin.Px
          End Select
          ' Clamp so don't walk off map
          If m_aiSafePosition <= 20.0F Then m_aiSafePosition = 20.0F
          If m_aiSafePosition >= m_mapWidth - 20.0F Then m_aiSafePosition = m_mapWidth - 20.0F
          m_aiNextState = AiState.Move

        Case AiState.Move
          Dim origin = TryCast(m_objectUnderControl, WormObject)
          If m_turnTime >= 8.0F AndAlso origin.Px <> m_aiSafePosition Then
            ' Walk towards target until it is in range
            If m_aiSafePosition < origin.Px AndAlso m_gameIsStable Then
              origin.ShootAngle = -3.14159F * 0.6F
              m_aiJump = True
              m_aiNextState = AiState.Move
            End If
            If m_aiSafePosition > origin.Px AndAlso m_gameIsStable Then
              origin.ShootAngle = -3.14159F * 0.4F
              m_aiJump = True
              m_aiNextState = AiState.Move
            End If
          Else
            m_aiNextState = AiState.ChooseTarget
          End If

        Case AiState.ChooseTarget ' Worm finished moving, choose target
          m_aiJump = False
          ' Select Team that is not itself
          Dim origin = TryCast(m_objectUnderControl, WormObject)
          Dim currentTeamId = origin.TeamId
          Dim targetTeamId = 0
          Do
            targetTeamId = Rand() Mod m_teams.Count
          Loop While targetTeamId = currentTeamId OrElse Not m_teams(targetTeamId).IsTeamAlive()
          ' Aggressive strategy is to aim for opponent unit with most health
          Dim mostHealthyWorm = m_teams(targetTeamId).Members(0)
          For Each w In m_teams(targetTeamId).Members
            If w.Health > mostHealthyWorm.Health Then
              mostHealthyWorm = w
            End If
          Next
          m_aiTargetWorm = mostHealthyWorm
          m_aiTargetX = mostHealthyWorm.px
          m_aiTargetY = mostHealthyWorm.py
          m_aiNextState = AiState.PositionForTarget

        Case AiState.PositionForTarget ' Calculate trajectory for target, if the worm needs to move, do so
          Dim origin = TryCast(m_objectUnderControl, WormObject)
          Dim dy = -(m_aiTargetY - origin.Py)
          Dim dx = -(m_aiTargetX - origin.Px)
          Dim speed = 30.0F
          Dim gravity = 2.0F
          m_aiJump = False
          Dim a = speed * speed * speed * speed - gravity * (gravity * dx * dx + 2.0F * dy * speed * speed)
          If a < 0 Then
            ' Target is out of range
            If m_turnTime >= 5.0F Then
              ' Walk towards target until it is in range
              If m_aiTargetWorm.Px < origin.Px AndAlso m_gameIsStable Then
                origin.ShootAngle = -3.14159F * 0.6F
                m_aiJump = True
                m_aiNextState = AiState.PositionForTarget
              End If
              If m_aiTargetWorm.Px > origin.Px AndAlso m_gameIsStable Then
                origin.ShootAngle = -3.14159F * 0.4F
                m_aiJump = True
                m_aiNextState = AiState.PositionForTarget
              End If
            Else
              ' Worm is stuck, so just fire in direction of enemy!
              ' Its dangerous to self, but may clear a blockage
              m_aiTargetAngle = origin.ShootAngle
              m_aiTargetEnergy = 0.75F
              m_aiNextState = AiState.Aim
            End If
          Else
            ' Worm is close enough, calculate trajectory
            Dim b1 = CSng(speed * speed + Math.Sqrt(a))
            Dim b2 = CSng(speed * speed - Math.Sqrt(a))
            Dim theta1 = CSng(Math.Atan(b1 / (gravity * dx))) ' Max Height
            Dim theta2 = CSng(Math.Atan(b2 / (gravity * dx))) ' Min Height
            ' We'll use max as its a greater chance of avoiding obstacles
            m_aiTargetAngle = theta1 - If(dx > 0, 3.14159F, 0.0F)
            Dim fireX = CSng(Math.Cos(m_aiTargetAngle))
            Dim fireY = CSng(Math.Sin(m_aiTargetAngle))
            ' AI is clamped to 3/4 power
            m_aiTargetEnergy = 0.75F
            m_aiNextState = AiState.Aim
          End If

        Case AiState.Aim
          Dim worm = TryCast(m_objectUnderControl, WormObject)
          m_aiAimLeft = False
          m_aiAimRight = False
          m_aiJump = False
          If worm.ShootAngle < m_aiTargetAngle Then
            m_aiAimRight = True
          Else
            m_aiAimLeft = True
          End If
          ' Once cursors are aligned, fire - some noise could be
          ' included here to give the AI a varying accuracy, and the
          ' magnitude of the noise could be linked to game difficulty
          If Math.Abs(worm.ShootAngle - m_aiTargetAngle) <= 0.001F Then
            m_aiAimLeft = False
            m_aiAimRight = False
            m_energyLevel = 0.0F
            m_aiNextState = AiState.Fire
          Else
            m_aiNextState = AiState.Aim
          End If

        Case AiState.Fire
          m_aiEnergize = True
          m_fireWeapon = False
          m_energizing = True
          If m_energyLevel >= m_aiTargetEnergy Then
            m_fireWeapon = True
            m_aiEnergize = False
            m_energizing = False
            m_enableComputerControl = False
            m_aiNextState = AiState.AssessEnvironment
          End If

      End Select
    End If

    ' Decrease Turn Time
    m_turnTime -= elapsedTime

    If m_objectUnderControl IsNot Nothing Then

      m_objectUnderControl.Ax = 0.0F

      If m_objectUnderControl.Stable Then

        If (m_enablePlayerControl AndAlso m_keys(AscW("Z"c)).Pressed) OrElse (m_enableComputerControl AndAlso m_aiJump) Then
          Dim a = CType(m_objectUnderControl, WormObject).ShootAngle
          m_objectUnderControl.Vx = 4.0F * CSng(Math.Cos(a))
          m_objectUnderControl.Vy = 8.0F * CSng(Math.Sin(a))
          m_objectUnderControl.Stable = False
          m_aiJump = False
        End If
        If (m_enablePlayerControl AndAlso m_keys(AscW("S"c)).Held) OrElse (m_enableComputerControl AndAlso m_aiAimRight) Then
          Dim worm = CType(m_objectUnderControl, WormObject)
          worm.ShootAngle += 1.0F * elapsedTime
          If worm.ShootAngle > 3.14159F Then worm.ShootAngle -= 3.14159F * 2.0F
        End If
        If (m_enablePlayerControl AndAlso m_keys(AscW("A"c)).Held) OrElse (m_enableComputerControl AndAlso m_aiAimLeft) Then
          Dim worm = CType(m_objectUnderControl, WormObject)
          worm.ShootAngle -= 1.0F * elapsedTime
          If worm.ShootAngle < -3.14159F Then worm.ShootAngle += 3.14159F * 2.0F
        End If
        If m_enablePlayerControl AndAlso m_keys(VK_SPACE).Pressed Then
          m_fireWeapon = False
          m_energizing = True
          m_energyLevel = 0.0F
        End If
        If (m_enablePlayerControl AndAlso m_keys(VK_SPACE).Held) OrElse (m_enableComputerControl AndAlso m_aiEnergize) Then
          If m_energizing Then
            m_energyLevel += 0.75F * elapsedTime
            If m_energyLevel > 1.0F Then m_energyLevel = 1.0F : m_fireWeapon = True
          End If
        End If
        If m_enablePlayerControl AndAlso m_keys(VK_SPACE).Released Then
          If m_energizing Then m_fireWeapon = True
          m_energizing = False
        End If

      End If

      If m_cameraTrackingObject IsNot Nothing Then
        m_targetCameraPosX = CSng(m_cameraTrackingObject.Px - ScreenWidth() / 2)
        m_targetCameraPosY = CSng(m_cameraTrackingObject.Py - ScreenHeight() / 2)
        m_cameraPosX += (m_targetCameraPosX - m_cameraPosX) * 15.0F * elapsedTime
        m_cameraPosY += (m_targetCameraPosY - m_cameraPosY) * 15.0F * elapsedTime
      End If

      If m_fireWeapon Then
        Dim worm = TryCast(m_objectUnderControl, WormObject)
        ' Get Weapon Origin
        Dim ox = worm.Px
        Dim oy = worm.Py
        ' Get Weapon Direction
        Dim dx = CSng(Math.Cos(worm.ShootAngle))
        Dim dy = CSng(Math.Sin(worm.ShootAngle))
        ' Create Weapon Object
        Dim m = New MissileObject(ox, oy, dx * 40.0F * m_energyLevel, dy * 40.0F * m_energyLevel)
        m_cameraTrackingObject = m
        m_objects.Add(m)
        ' Reset variables
        m_fireWeapon = False
        m_energyLevel = 0.0F
        m_energizing = False
        m_playerHasFired = True
        If Rand Mod 100 >= 50 Then m_zoomOut = True
      End If

    End If

    ' Clamp map boundaries
    If m_cameraPosX < 0 Then m_cameraPosX = 0
    If m_cameraPosX >= m_mapWidth - ScreenWidth() Then m_cameraPosX = m_mapWidth - ScreenWidth()
    If m_cameraPosY < 0 Then m_cameraPosY = 0
    If m_cameraPosY >= m_mapHeight - ScreenHeight() Then m_cameraPosY = m_mapHeight - ScreenHeight()

    ' Do 10 physics iterations per frame
    For z = 0 To 9

      ' Update physics of all physical objects
      Dim boomList As New List(Of (X As Single, Y As Single, Radius As Single))
      For Each p In m_objects

        ' Apply Gravity
        p.Ay += 2.0F

        ' Update Velocity
        p.Vx += p.Ax * elapsedTime
        p.Vy += p.Ay * elapsedTime

        ' Update Position
        Dim potentialX = p.Px + p.Vx * elapsedTime
        Dim potentialY = p.Py + p.Vy * elapsedTime

        ' Reset Acceleration
        p.Ax = 0.0F
        p.Ay = 0.0F

        p.Stable = False

        ' Collision Check With Map
        Dim angle = CSng(Math.Atan2(p.Vy, p.Vx))
        Dim responseX = 0.0F
        Dim responseY = 0.0F
        Dim collision = False

        For r = angle - 3.14159F / 2.0F To angle + 3.14159F / 2.0F Step 3.14159F / 8.0F

          ' Iterate through semicircle of objects radius rotated to direction of travel
          Dim testPosX = CSng((p.Radius) * Math.Cos(r) + potentialX)
          Dim testPosY = CSng((p.Radius) * Math.Sin(r) + potentialY)

          If testPosX >= m_mapWidth Then testPosX = m_mapWidth - 1
          If testPosY >= m_mapHeight Then testPosY = m_mapHeight - 1
          If testPosX < 0 Then testPosX = 0
          If testPosY < 0 Then testPosY = 0

          ' Test if any points on semicircle intersect with terrain
          If m_map(CInt(testPosY) * m_mapWidth + CInt(testPosX)) > 0 Then
            ' Accumulate collision points to give an escape response vector
            ' Effectively, normal to the areas of contact
            responseX += potentialX - testPosX
            responseY += potentialY - testPosY
            collision = True
          End If
        Next

        ' Calculate magnitudes of response and velocity vectors
        Dim magVelocity = CSng(Math.Sqrt(p.Vx * p.Vx + p.Vy * p.Vy))
        Dim magResponse = CSng(Math.Sqrt(responseX * responseX + responseY * responseY))

        If p.Px < 0 OrElse p.Px > m_mapWidth OrElse p.Py < 0 OrElse p.Py > m_mapHeight Then
          p.Dead = True : If TypeOf p Is WormObject Then CType(p, WormObject).Health = 0.0
        End If

        ' Find angle of collision
        If collision Then

          p.Stable = True

          ' Calculate reflection vector of objects velocity vector, using response vector as normal
          Dim dot = p.Vx * (responseX / magResponse) + p.Vy * (responseY / magResponse)

          ' Use friction coefficient to dampen response (approximating energy loss)
          p.Vx = p.Friction * (-2.0F * dot * (responseX / magResponse) + p.Vx)
          p.Vy = p.Friction * (-2.0F * dot * (responseY / magResponse) + p.Vy)

          'Some objects will "die" after several bounces
          If p.BounceBeforeDeath > 0 Then

            p.BounceBeforeDeath -= 1
            p.Dead = (p.BounceBeforeDeath = 0)
            If p.Dead Then
              If TypeOf p Is WormObject Then CType(p, WormObject).Health = 0.0
              ' Action upon object death
              ' = 0 Nothing
              ' > 0 Explosion 
              Dim response = p.BounceDeathAction()
              'If response > 0 Then Boom(p.px, p.py, response)
              If response > 0 Then boomList.Add((p.Px, p.Py, response))
            End If

          End If
        Else
          ' No collision so update objects position
          p.Px = potentialX
          p.Py = potentialY
        End If

        ' Turn off movement when tiny
        If magVelocity < 0.1F Then p.Stable = True

      Next

      ' Had to move this outside of the loop since Boom adds new objects.
      If boomList.Any Then
        For Each b In boomList
          Boom(b.X, b.Y, b.Radius)
        Next
        boomList.Clear()
        m_cameraTrackingObject = Nothing 'm_objectUnderControl
      End If

      ' Remove dead objects from the list, so they are not processed further. As the object
      ' is a unique pointer, it will go out of scope too, deleting the object automatically. Nice :-)
      m_objects.RemoveAll(Function(o) o.Dead)

    Next

    ' Draw Landscape
    If Not m_zoomOut Then

      For x = 0 To ScreenWidth() - 1
        For y = 0 To ScreenHeight() - 1
          Select Case m_map((y + CInt(Fix(m_cameraPosY))) * m_mapWidth + (x + CInt(Fix(m_cameraPosX))))
            Case -1 : Draw(x, y, PIXEL_SOLID, FG_DARK_BLUE)
            Case -2 : Draw(x, y, PIXEL_QUARTER, FG_BLUE Or BG_DARK_BLUE)
            Case -3 : Draw(x, y, PIXEL_HALF, FG_BLUE Or BG_DARK_BLUE)
            Case -4 : Draw(x, y, PIXEL_THREEQUARTERS, FG_BLUE Or BG_DARK_BLUE)
            Case -5 : Draw(x, y, PIXEL_SOLID, FG_BLUE)
            Case -6 : Draw(x, y, PIXEL_QUARTER, FG_CYAN Or BG_BLUE)
            Case -7 : Draw(x, y, PIXEL_HALF, FG_CYAN Or BG_BLUE)
            Case -8 : Draw(x, y, PIXEL_THREEQUARTERS, FG_CYAN Or BG_BLUE)
            Case 0 : Draw(x, y, PIXEL_SOLID, FG_CYAN)
            Case 1 : Draw(x, y, PIXEL_SOLID, FG_DARK_GREEN)
          End Select
        Next
      Next

      ' Draw objects - they draw themselves
      For Each p In m_objects

        p.Draw(Me, m_cameraPosX, m_cameraPosY)

        If p Is m_objectUnderControl Then

          Dim worm = CType(p, WormObject)

          ' Draw Crosshair
          Dim cx = worm.Px + 8.0F * CSng(Math.Cos(worm.ShootAngle)) - m_cameraPosX
          Dim cy = worm.Py + 8.0F * CSng(Math.Sin(worm.ShootAngle)) - m_cameraPosY

          Draw(cx, cy, PIXEL_SOLID, FG_BLACK)
          Draw(cx + 1, cy, PIXEL_SOLID, FG_BLACK)
          Draw(cx - 1, cy, PIXEL_SOLID, FG_BLACK)
          Draw(cx, cy + 1, PIXEL_SOLID, FG_BLACK)
          Draw(cx, cy - 1, PIXEL_SOLID, FG_BLACK)

          'DrawString(cx - 3, cy - 3, m_energyLevel.ToString, fg_white)
          For i = 0 To (11.0F * m_energyLevel) - 1
            Draw(worm.Px - 5 + i - m_cameraPosX, worm.Py - 12 - m_cameraPosY, PIXEL_SOLID, FG_GREEN)
            Draw(worm.Px - 5 + i - m_cameraPosX, worm.Py - 11 - m_cameraPosY, PIXEL_SOLID, FG_RED)
          Next

        End If
      Next

    Else

      For x = 0 To ScreenWidth() - 1
        For y = 0 To ScreenHeight() - 1

          Dim fx = CInt(Fix((x / ScreenWidth()) * m_mapWidth))
          Dim fy = CInt(Fix((y / ScreenHeight()) * m_mapHeight))

          Select Case m_map(fy * m_mapWidth + fx)
            Case -1 : Draw(x, y, PIXEL_SOLID, FG_DARK_BLUE)
            Case -2 : Draw(x, y, PIXEL_QUARTER, FG_BLUE Or BG_DARK_BLUE)
            Case -3 : Draw(x, y, PIXEL_HALF, FG_BLUE Or BG_DARK_BLUE)
            Case -4 : Draw(x, y, PIXEL_THREEQUARTERS, FG_BLUE Or BG_DARK_BLUE)
            Case -5 : Draw(x, y, PIXEL_SOLID, FG_BLUE)
            Case -6 : Draw(x, y, PIXEL_QUARTER, FG_CYAN Or BG_BLUE)
            Case -7 : Draw(x, y, PIXEL_HALF, FG_CYAN Or BG_BLUE)
            Case -8 : Draw(x, y, PIXEL_THREEQUARTERS, FG_CYAN Or BG_BLUE)
            Case 0 : Draw(x, y, PIXEL_SOLID, FG_CYAN)
            Case 1 : Draw(x, y, PIXEL_SOLID, FG_DARK_GREEN)
          End Select
        Next
      Next

      For Each p In m_objects
        p.Draw(Me, p.Px - (p.Px / m_mapWidth) * ScreenWidth(), p.Py - (p.Py / m_mapHeight) * ScreenHeight(), True)
      Next

    End If

    ' Check for game state stability
    m_gameIsStable = True
    For Each p In m_objects
      If Not p.Stable Then m_gameIsStable = False : Exit For
    Next

    ' This little marker is handy for debugging
    'If m_gameIsStable Then
    '  Fill(2, 2, 6, 6, PIXEL_SOLID, FG_RED)
    'End If

    ' Draw Team Health Bars
    For t = 0 To m_teams.Count - 1
      Dim totalHealth = 0.0F
      Dim maxHealth = m_teams(t).TeamSize
      For Each w In m_teams(t).Members ' Accumulate team health
        totalHealth += w.Health
      Next
      Dim cols = {FG_RED, FG_BLUE, FG_MAGENTA, FG_GREEN}
      Fill(4, 4 + t * 4, (totalHealth / maxHealth) * (ScreenWidth() - 8) + 4, 4 + t * 4 + 3, PIXEL_SOLID, cols(t))
    Next

    ' Mystery Code !! Displays Seven-Segment Display for countdown timer
    ' Display Turn Time - Check out the discord #challenges for explanation!
    ' Thanks to all those awesome discord chatters who took part! I've kept
    ' this minimised and obfuscated to maintain the spirit of the challenge!
    If m_showCountDown AndAlso m_turnTime >= 0.0F Then
      'Dim d = "w$]m.k{%\x7Fo"
      Dim d As String = "w$]m.k{%" & ChrW(&H7F) & "o"
      Dim tx = 4
      Dim ty = m_teams.Count * 4 + 8
      Dim s = m_turnTime.ToString
      For r = 0 To 12
        For c = 0 To (If(m_turnTime < 10.0F, 0, 1))
          Dim a = AscW(s.Chars(c)) - 48
          If Not (r Mod 6) <> 0 Then
            DrawStringAlpha(tx, ty, If((AscW(d(a)) And (1 << (r \ 2))) <> 0, " ##### ", " "), FG_BLACK) : tx += 8
          Else
            DrawStringAlpha(tx, ty, If((AscW(d(a)) And (1 << (If(r < 6, 1, 4)))) <> 0, "# ", " "), FG_BLACK) : tx += 6
            DrawStringAlpha(tx, ty, If((AscW(d(a)) And (1 << (If(r < 6, 2, 5)))) <> 0, "# ", " "), FG_BLACK) : tx += 2
          End If
        Next
        ty += 1 : tx = 4
      Next
    End If

    ' Update State Machine
    m_gameState = m_nextState
    m_aiState = m_aiNextState

    Return True

  End Function

  ' Explosion Function
  Private Sub Boom(worldX As Single, worldY As Single, radius As Single)

    Dim circleBresenham = Sub(xc As Integer, yc As Integer, r As Integer)

                            ' Taken from wikipedia
                            Dim x = 0
                            Dim y = r
                            Dim p = 3 - 2 * r

                            If r = 0 Then Return

                            Dim drawLine = Sub(sx As Integer, ex As Integer, ny As Integer)
                                             For i = sx To ex - 1
                                               If ny >= 0 AndAlso ny < m_mapHeight AndAlso i >= 0 AndAlso i < m_mapWidth Then
                                                 m_map(ny * m_mapWidth + i) = 0
                                               End If
                                             Next
                                           End Sub

                            While y >= x
                              ' Modified to draw scan-lines instead of edges
                              drawLine(xc - x, xc + x, yc - y)
                              drawLine(xc - y, xc + y, yc - x)
                              drawLine(xc - x, xc + x, yc + y)
                              drawLine(xc - y, xc + y, yc + x)
                              If p < 0 Then
                                p += 4 * x + 6
                              Else
                                p += 4 * (x - y) + 10
                                y -= 1
                              End If
                              x += 1
                            End While
                          End Sub

    ' Erase Terrain to form crater
    circleBresenham(CInt(worldX), CInt(worldY), CInt(radius))

    ' Shockwave other entities in range
    For Each p In m_objects

      ' Work out distance between explosion origin and object
      Dim dx = p.Px - worldX
      Dim dy = p.Py - worldY
      Dim dist = CSng(Math.Sqrt(dx * dx + dy * dy))

      If dist < 0.0001F Then
        dist = 0.0001F
      End If

      ' If within blast radius
      If dist < radius Then
        ' Set velocity proportional and away from boom origin
        p.Vx = (dx / dist) * radius
        p.Vy = (dy / dist) * radius
        p.Damage(((radius - dist) / radius) * 0.8F) ' Corrected
        p.Stable = False
      End If

    Next

    ' Launch debris proportional to blast size
    For i = 0 To CInt(radius) - 1
      m_objects.Add(New DebrisObject(worldX, worldY))
    Next

  End Sub

  Private Sub CreateMap()

    ' Used 1D Perlin Noise
    Dim surface = New Single(m_mapWidth - 1) {}
    Dim noiseSeed = New Single(m_mapWidth - 1) {}

    ' Populate with noise
    For i = 0 To m_mapWidth - 1
      noiseSeed(i) = CSng(Rand / RAND_MAX)
    Next

    ' Clamp noise to half way up screen
    noiseSeed(0) = 0.5F

    ' Generate 1D map
    PerlinNoise1D(m_mapWidth, noiseSeed, 8, 2.0F, surface)

    ' Fill 2D map based on adjacent 1D map
    For x = 0 To m_mapWidth - 1
      For y = 0 To m_mapHeight - 1
        If y >= surface(x) * m_mapHeight Then
          m_map(y * m_mapWidth + x) = 1
        Else
          ' Shade the sky according to altitude - we only do top 1/3 of map
          ' as the Boom() function will just paint in 0 (cyan)
          If y < m_mapHeight / 3.0F Then
            m_map(y * m_mapWidth + x) = CInt(Fix((-8.0F * (CSng(y) / (m_mapHeight / 3.0F))) - 1.0F))
          Else
            m_map(y * m_mapWidth + x) = 0
          End If
        End If
      Next
    Next

    ' Clean up!
    'surface = Nothing
    'noiseSeed = Nothing

  End Sub

  Private Shared Sub PerlinNoise1D(count As Integer, seed As Single(), octaves As Integer, bias As Single, output As Single())

    ' Used 1D Perlin Noise

    For x = 0 To count - 1

      Dim noise = 0.0F
      Dim scaleAcc = 0.0F
      Dim scale = 1.0F

      For o = 0 To octaves - 1

        Dim pitch = count >> o
        Dim sample1 = CInt(Fix((x / pitch))) * pitch
        Dim sample2 = (sample1 + pitch) Mod count

        Dim blend = (x - sample1) / pitch
        Dim sample = CSng((1.0F - blend) * seed(sample1) + blend * seed(sample2))
        scaleAcc += scale
        noise += sample * scale
        scale /= bias

      Next

      ' Scale to seed range
      output(x) = noise / scaleAcc

    Next

  End Sub

End Class

Public MustInherit Class PhysicsObject

  Public Px As Single = 0.0F ' Position
  Public Py As Single = 0.0F
  Public Vx As Single = 0.0F ' Velocity
  Public Vy As Single = 0.0F
  Public Ax As Single = 0.0F ' Acceleration
  Public Ay As Single = 0.0F
  Public Radius As Single = 4.0F ' Bounding circle for collision
  Public Stable As Boolean = False ' Has object stopped moving
  Public Friction As Single = 0.8F ' Actually, a dampening factor is a more accurate name

  Public BounceBeforeDeath As Integer = -1 ' How many time object can bounce before death
  ' -1 = infinite
  Public property Dead As Boolean = False ' Flag to indicate object should be removed

  Public Sub New(Optional x As Single = 0.0F, Optional y As Single = 0.0F)
    Px = x
    Py = y
  End Sub

  Public MustOverride Sub Draw(engine As ConsoleGameEngine.ConsoleGameEngine, offsetX As Single, offsetY As Single, Optional pixel As Boolean = False)
  Public MustOverride Function BounceDeathAction() As Integer
  Public MustOverride Function Damage(d As Single) As Boolean

End Class

'' Does nothing, shows a marker that helps with physics debug and test
'Public Class DummyObject
'  Inherits PhysicsObject

'  Private Shared ReadOnly m_vecModel As List(Of (Single, Single)) = DefineDummy()

'  Public Sub New(Optional x As Single = 0.0F, Optional y As Single = 0.0F)
'    MyBase.New(x, y)
'  End Sub

'  Public Overrides Sub Draw(engine As ConsoleGameEngine.ConsoleGameEngine, offsetX As Single, offsetY As Single)
'    engine.DrawWireFrameModel(m_vecModel, Px - offsetX, Py - offsetY, CSng(Math.Atan2(Vy, Vx)), Radius, FG_WHITE)
'  End Sub

'  Public Overrides Function BounceDeathAction() As Integer
'    Return 0 ' Nothing, just fade
'  End Function

'  Shared Function DefineDummy() As List(Of (Single, Single))
'    ' Defines a circle with a line fom center to edge
'    Dim vecModel As New List(Of (Single, Single)) From {(0.0F, 0.0F)}
'    For i = 0 To 9
'      vecModel.Add((CSng(Math.Cos(i / 9.0F * 2.0F * 3.14159F)), CSng(Math.Sin(i / 9.0F * 2.0F * 3.14159F))))
'    Next
'    Return vecModel
'  End Function

'End Class

' a small rock that bounces
Public Class DebrisObject
  Inherits PhysicsObject

  Private Shared ReadOnly m_vecModel As List(Of (Single, Single)) = DefineDebris()

  Public Sub New(x As Single, y As Single)
    MyBase.New(x, y)
    ' Set velocity to random direction and size for "boom" effect
    Dim rand As New Random()
    Vx = 10.0F * CSng(Math.Cos(rand.NextDouble() * 2.0 * Math.PI))
    Vy = 10.0F * CSng(Math.Sin(rand.NextDouble() * 2.0 * Math.PI))
    Radius = 1.0F
    Friction = 0.8F
    Dead = False
    Stable = False
    BounceBeforeDeath = 2 ' After 2 bounces, dispose
  End Sub

  Public Overrides Sub Draw(engine As ConsoleGameEngine.ConsoleGameEngine, offsetX As Single, offsetY As Single, Optional pixel As Boolean = False)
    engine.DrawWireFrameModel(m_vecModel, Px - offsetX, Py - offsetY, CSng(Math.Atan2(Vy, Vx)), If(pixel, 0.5F, Radius), FG_DARK_GREEN)
  End Sub

  Public Overrides Function BounceDeathAction() As Integer
    Return 0 ' Nothing, just fade
  End Function

  Public Overrides Function Damage(d As Single) As Boolean
    Return True ' Cannot be damaged
  End Function

  Private Shared Function DefineDebris() As List(Of (Single, Single))
    ' A small unit rectangle
    Return New List(Of (Single, Single)) From {(0.0F, 0.0F),
                                               (1.0F, 0.0F),
                                               (1.0F, 1.0F),
                                               (0.0F, 1.0F)}
  End Function

End Class

' A projectile weapon
Public Class MissileObject
  Inherits PhysicsObject

  Private Shared ReadOnly vecModel As List(Of (Single, Single)) = DefineMissile()

  Public Sub New(Optional x As Single = 0.0F, Optional y As Single = 0.0F, Optional _vx As Single = 0.0F, Optional _vy As Single = 0.0F)
    MyBase.New(x, y)
    Radius = 2.5F
    Friction = 0.5F
    Vx = _vx
    Vy = _vy
    Dead = False
    BounceBeforeDeath = 1
    Stable = False
  End Sub

  Public Overrides Sub Draw(engine As ConsoleGameEngine.ConsoleGameEngine, offsetX As Single, offsetY As Single, Optional pixel As Boolean = False)
    engine.DrawWireFrameModel(vecModel, Px - offsetX, Py - offsetY, CSng(Math.Atan2(Vy, Vx)), If(pixel, 0.5F, Radius), FG_BLACK)
  End Sub

  Public Overrides Function BounceDeathAction() As Integer
    Return 20 ' Explode Big
  End Function

  Public Overrides Function Damage(d As Single) As Boolean
    Return True
  End Function

  Private Shared Function DefineMissile() As List(Of (Single, Single))

    ' Defines a rocket like shape
    Dim vecModel As New List(Of (Single, Single)) From {(0.0F, 0.0F),
                                                        (1.0F, 1.0F),
                                                        (2.0F, 1.0F),
                                                        (2.5F, 0.0F),
                                                        (2.0F, -1.0F),
                                                        (1.0F, -1.0F),
                                                        (0.0F, 0.0F),
                                                        (-1.0F, -1.0F),
                                                        (-2.5F, -1.0F),
                                                        (-2.0F, 0.0F),
                                                        (-2.5F, 1.0F),
                                                        (-1.0F, 1.0F)}

    ' Scale points to make shape unit sized
    For Each v In vecModel
      v.Item1 /= 2.5F
      v.Item2 /= 2.5F
    Next

    Return vecModel

  End Function

End Class

Public Class WormObject
  Inherits PhysicsObject ' A unit, or worm

  Private Shared m_wormSprite As Sprite = Nothing

  Public ShootAngle As Single = 0.0F
  Public Property Health As Single = 1.0F
  Public Property TeamId As Integer = 0 ' ID of which team this worm belongs to
  Public Property IsPlayable As Boolean = True

  Public Sub New(Optional x As Single = 0.0F, Optional y As Single = 0.0F)
    MyBase.New(x, y)

    Radius = 3.5F
    Friction = 0.2F
    Dead = False
    BounceBeforeDeath = -1
    Stable = False

    ' load sprite data from sprite file
    'If m_sprite Is Nothing Then
    m_wormSprite = New Sprite("worms1.spr")
    'End If

  End Sub

  Public Overrides Sub Draw(engine As ConsoleGameEngine.ConsoleGameEngine, offsetX As Single, offsetY As Single, Optional pixel As Boolean = False)

    If IsPlayable Then ' Draw Worm Sprite with health bar, in team colors

      engine.DrawPartialSprite(Px - offsetX - Radius, Py - offsetY - Radius, m_wormSprite, TeamId * 8, 0, 8, 8)

      ' Drw health bar for worm
      For i = 0 To (11 * Health) - 1
        engine.Draw(Px - 5 + i - offsetX, Py + 5 - offsetY, PIXEL_SOLID, FG_BLUE)
        engine.Draw(Px - 5 + i - offsetX, Py + 6 - offsetY, PIXEL_SOLID, FG_BLUE)
      Next

    Else ' Draw tombstone sprite for team colour

      engine.DrawPartialSprite(Px - offsetX - Radius, Py - offsetY - Radius, m_wormSprite, TeamId * 8, 8, 8, 8)

    End If

  End Sub

  Public Overrides Function BounceDeathAction() As Integer
    Return 0 ' Nothing
  End Function

  Public Overrides Function Damage(d As Single) As Boolean
    Health -= d
    If Health <= 0 Then
      ' Worm has died, no longer playable
      Health = 0.0F
      IsPlayable = False
    End If
    Return Health > 0
  End Function

End Class

' Defines a group of worms
Public Class TeamObject

  Public Property Members As New List(Of WormObject)
  Public Property CurrentMember As Integer = 0 ' Index into vector for current worms turn
  Public Property TeamSize As Integer = 0 ' Total number of worms in team

  Public Function IsTeamAlive() As Boolean
    ' Iterate through all team members, if any of them have > 0 health, return True.
    Dim allDead = False
    For Each w In Members
      allDead = allDead OrElse (w.Health > 0.0F)
    Next
    Return allDead
  End Function

  Public Function GetNextMember() As WormObject
    ' Return a reference to the next team member that is valid for control
    Do
      CurrentMember += 1
      If CurrentMember >= TeamSize Then CurrentMember = 0
    Loop While Members(CurrentMember).Health <= 0
    Return Members(CurrentMember)
  End Function

End Class