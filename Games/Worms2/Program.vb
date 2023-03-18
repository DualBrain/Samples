' Inspired by: "Code-It-Yourself! Worms Part #2" -- @javid
' https://youtu.be/pV2qYJjCdxM

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
  Private m_map As Byte() = Nothing

  ' Camera coordinates
  Private m_cameraPosX As Single = 0.0F
  Private m_cameraPosY As Single = 0.0F
  Private m_targetCameraPosX As Single = 0.0F
  Private m_targetCameraPosY As Single = 0.0F

  Private Enum GameState
    Reset
    GenerateTerrain
    GeneratingTerrain
    AllocateUnits
    AllocatingUnits
    StartPlay
    CameraMode
  End Enum

  Private m_gameState As GameState
  Private m_nextState As GameState

  Private m_gameIsStable As Boolean = False
  Private m_playerHasControl As Boolean = False
  Private m_playerActionComplete As Boolean = False

  ' list of things that exist in game world
  Private ReadOnly m_objects As New List(Of PhysicsObject)()

  Private m_objectUnderControl As PhysicsObject = Nothing
  Private m_cameraTrackingObject As PhysicsObject = Nothing

  Private m_energizing As Boolean = False
  Private m_energyLevel As Single = 0.0F
  Private m_fireWeapon As Boolean = False

  Public Overrides Function OnUserCreate() As Boolean

    ' Create Map
    m_map = New Byte(m_mapWidth * m_mapHeight - 1) {}
    'Array.Clear(m_map, 0, m_mapWidth * m_mapHeight)
    'CreateMap()

    m_gameState = GameState.Reset
    m_nextState = GameState.Reset

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Press 'M' key to regenerate map
    If m_keys(AscW("M"c)).Released Then CreateMap()

    ' Left click to cause small explosion
    If m_mouse(0).Released Then
      Boom(m_mousePosX + m_cameraPosX, m_mousePosY + m_cameraPosY, 10.0F)
    End If

    ' Right click to drop missile
    If m_mouse(1).Released Then
      m_objects.Add(New MissileObject(m_mousePosX + m_cameraPosX, m_mousePosY + m_cameraPosY))
    End If

    ' Middle click to spawn worm/unit
    If m_mouse(2).Released Then
      Dim worm = New WormObject(m_mousePosX + m_cameraPosX, m_mousePosY + m_cameraPosY)
      m_objects.Add(worm)
      m_objectUnderControl = worm
      m_cameraTrackingObject = worm
    End If

    ' Mouse Edge Map Scroll
    Dim mapScrollSpeed = 400.0F
    If m_mousePosX < 5 Then m_cameraPosX -= mapScrollSpeed * elapsedTime
    If m_mousePosX > ScreenWidth() - 5 Then m_cameraPosX += mapScrollSpeed * elapsedTime
    If m_mousePosY < 5 Then m_cameraPosY -= mapScrollSpeed * elapsedTime
    If m_mousePosY > ScreenHeight() - 5 Then m_cameraPosY += mapScrollSpeed * elapsedTime

    ' Control Supervisor
    Select Case m_gameState
      Case GameState.Reset : m_playerHasControl = False : m_nextState = GameState.GenerateTerrain
      Case GameState.GenerateTerrain
        m_playerHasControl = False
        CreateMap()
        m_nextState = GameState.GeneratingTerrain
      Case GameState.GeneratingTerrain : m_playerHasControl = False : m_nextState = GameState.AllocateUnits
      Case GameState.AllocateUnits
        m_playerHasControl = False
        Dim worm = New WormObject(32.0F, 1.0F)
        m_objects.Add(worm)
        m_objectUnderControl = worm
        m_cameraTrackingObject = worm
        m_nextState = GameState.AllocatingUnits
      Case GameState.AllocatingUnits
        m_playerHasControl = False
        If m_gameIsStable Then
          m_playerActionComplete = False
          m_nextState = GameState.StartPlay
        End If
      Case GameState.StartPlay
        m_playerHasControl = True
        If m_playerActionComplete Then m_nextState = GameState.CameraMode
      Case GameState.CameraMode
        m_playerHasControl = False
        m_playerActionComplete = False
        If m_gameIsStable Then
          m_cameraTrackingObject = m_objectUnderControl
          m_nextState = GameState.StartPlay
        End If
    End Select

    ' Handle User Input
    If m_playerHasControl Then
      If m_objectUnderControl IsNot Nothing Then

        If m_objectUnderControl.Stable Then
          If m_keys(AscW("Z"c)).Pressed Then
            Dim a = CType(m_objectUnderControl, WormObject).m_shootAngle
            m_objectUnderControl.Vx = 4.0F * CSng(Math.Cos(a))
            m_objectUnderControl.Vy = 8.0F * CSng(Math.Sin(a))
            m_objectUnderControl.Stable = False
          End If
          If m_keys(AscW("A"c)).Held Then
            Dim worm = CType(m_objectUnderControl, WormObject)
            worm.m_shootAngle -= 1.0F * elapsedTime
            If worm.m_shootAngle < -3.14159F Then worm.m_shootAngle += 3.14159F * 2.0F
          End If
          If m_keys(AscW("S"c)).Held Then
            Dim worm = CType(m_objectUnderControl, WormObject)
            worm.m_shootAngle += 1.0F * elapsedTime
            If worm.m_shootAngle > 3.14159F Then worm.m_shootAngle -= 3.14159F * 2.0F
          End If
          If m_keys(VK_SPACE).Pressed Then
            m_energizing = True
            m_fireWeapon = False
            m_energyLevel = 0.0F
          End If
          If m_keys(VK_SPACE).Held Then
            If m_energizing Then
              m_energyLevel += 0.75F * elapsedTime
              If m_energyLevel > 1.0F Then m_energyLevel = 1.0F : m_fireWeapon = True
            End If
          End If
          If m_keys(VK_SPACE).Released Then
            If m_energizing Then m_fireWeapon = True
            m_energizing = False
          End If
        End If

        If m_fireWeapon Then
          Dim worm = TryCast(m_objectUnderControl, WormObject)
          ' Get Weapon Origin
          Dim ox = worm.Px
          Dim oy = worm.Py
          ' Get Weapon Direction
          Dim dx = CSng(Math.Cos(worm.m_shootAngle))
          Dim dy = CSng(Math.Sin(worm.m_shootAngle))
          ' Create Weapon Object
          Dim m = New MissileObject(ox, oy, dx * 40.0F * m_energyLevel, dy * 40.0F * m_energyLevel)
          m_objects.Add(m)
          m_cameraTrackingObject = m
          ' Reset variables
          m_fireWeapon = False
          m_energyLevel = 0.0F
          m_energizing = False
          m_playerActionComplete = True
        End If

      End If
    End If

    If m_cameraTrackingObject IsNot Nothing Then
      'm_cameraPosX = m_cameraTrackingObject.Px - CSng(ScreenWidth() / 2)
      'm_cameraPosY = m_cameraTrackingObject.Py - CSng(ScreenHeight() / 2)
      m_targetCameraPosX = CSng(m_cameraTrackingObject.Px - ScreenWidth() / 2)
      m_targetCameraPosY = CSng(m_cameraTrackingObject.Py - ScreenHeight() / 2)
      m_cameraPosX += (m_targetCameraPosX - m_cameraPosX) * 5.0F * elapsedTime
      m_cameraPosY += (m_targetCameraPosY - m_cameraPosY) * 5.0F * elapsedTime
    End If

    ' Clamp map boundaries
    If m_cameraPosX < 0 Then m_cameraPosX = 0
    If m_cameraPosX >= m_mapWidth - ScreenWidth() Then m_cameraPosX = m_mapWidth - ScreenWidth()
    If m_cameraPosY < 0 Then m_cameraPosY = 0
    If m_cameraPosY >= m_mapHeight - ScreenHeight() Then m_cameraPosY = m_mapHeight - ScreenHeight()

    ' Do 10 physics iterations per frame - this allows smaller physics steps
    ' giving rise to more accurate and controllable calculations
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

        ' Iterate through semicircle of objects radius rotated to direction of travel
        For r = angle - 3.14159F / 2.0F To angle + 3.14159F / 2.0F Step 3.14159F / 8.0F
          ' Calculate test point on circumference of circle
          Dim testPosX = CSng((p.Radius) * Math.Cos(r) + potentialX)
          Dim testPosY = CSng((p.Radius) * Math.Sin(r) + potentialY)

          ' Constrain to test within map boundary
          If testPosX >= m_mapWidth Then testPosX = m_mapWidth - 1
          If testPosY >= m_mapHeight Then testPosY = m_mapHeight - 1
          If testPosX < 0 Then testPosX = 0
          If testPosY < 0 Then testPosY = 0

          ' Test if any points on semicircle intersect with terrain
          If m_map(CInt(testPosY) * m_mapWidth + CInt(testPosX)) <> 0 Then
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

        ' Collision occurred
        If collision Then

          ' Force object to be stable, this stops the object penetrating the terrain
          p.Stable = True

          ' Calculate reflection vector of objects velocity vector, using response vector as normal
          Dim dot = p.Vx * (responseX / magResponse) + p.Vy * (responseY / magResponse)

          ' Use friction coefficient to dampen response (approximating energy loss)
          p.Vx = p.Friction * (-2.0F * dot * (responseX / magResponse) + p.Vx)
          p.Vy = p.Friction * (-2.0F * dot * (responseY / magResponse) + p.Vy)

          'Some objects will "die" after several bounces
          If p.BounceBeforeDeath > 0 Then

            p.BounceBeforeDeath -= 1
            p.Dead = p.BounceBeforeDeath = 0

            ' If object died, work out what to do next
            If p.Dead Then
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
    For x = 0 To ScreenWidth() - 1
      For y = 0 To ScreenHeight() - 1
        ' Offset screen coordinates into world coordinates
        Select Case m_map((y + CInt(m_cameraPosY)) * m_mapWidth + (x + CInt(m_cameraPosX)))
          Case 0
            Draw(x, y, PIXEL_SOLID, FG_CYAN) ' Sky
          Case 1
            Draw(x, y, PIXEL_SOLID, FG_DARK_GREEN) ' Land
        End Select
      Next
    Next

    ' Draw Objects
    For Each p In m_objects
      p.Draw(Me, m_cameraPosX, m_cameraPosY)
      If p Is m_objectUnderControl Then

        Dim worm = CType(p, WormObject)
        Dim cx = worm.Px + 8.0F * CSng(Math.Cos(worm.m_shootAngle)) - m_cameraPosX
        Dim cy = worm.Py + 8.0F * CSng(Math.Sin(worm.m_shootAngle)) - m_cameraPosY

        Draw(cx, cy, PIXEL_SOLID, FG_BLACK)
        Draw(cx + 1, cy, PIXEL_SOLID, FG_BLACK)
        Draw(cx - 1, cy, PIXEL_SOLID, FG_BLACK)
        Draw(cx, cy + 1, PIXEL_SOLID, FG_BLACK)
        Draw(cx, cy - 1, PIXEL_SOLID, FG_BLACK)

        For i = 0 To (11.0F * m_energyLevel) - 1
          Draw(worm.Px - 5 + i - m_cameraPosX, worm.Py - 12 - m_cameraPosY, PIXEL_SOLID, FG_GREY)
          Draw(worm.Px - 5 + i - m_cameraPosX, worm.Py - 11 - m_cameraPosY, PIXEL_SOLID, FG_RED)
        Next

      End If
    Next

    m_gameIsStable = True
    For Each p In m_objects
      If Not p.Stable Then m_gameIsStable = False : Exit For
    Next

    If m_gameIsStable Then
      Fill(2, 2, 6, 6, PIXEL_SOLID, FG_RED)
    End If

    m_gameState = m_nextState

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
          m_map(y * m_mapWidth + x) = 0
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
      Dim scale = 1.0F
      Dim scaleAcc = 0.0F

      For o = 0 To octaves - 1

        Dim pitch = count >> o
        Dim sample1 = CInt(Fix((x / pitch))) * pitch
        Dim sample2 = (sample1 + pitch) Mod count

        Dim blend = (x - sample1) / pitch
        Dim sample = CSng((1.0F - blend) * seed(sample1) + blend * seed(sample2))
        noise += sample * scale
        scaleAcc += scale
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
  Public Dead As Boolean = False ' Flag to indicate object should be removed

  Public Sub New(Optional x As Single = 0.0F, Optional y As Single = 0.0F)
    Px = x
    Py = y
  End Sub

  Public MustOverride Sub Draw(engine As ConsoleGameEngine.ConsoleGameEngine, offsetX As Single, offsetY As Single)
  Public MustOverride Function BounceDeathAction() As Integer

End Class

' Does nothing, shows a marker that helps with physics debug and test
Public Class DummyObject
  Inherits PhysicsObject

  Private Shared ReadOnly m_vecModel As List(Of (Single, Single)) = DefineDummy()

  Public Sub New(Optional x As Single = 0.0F, Optional y As Single = 0.0F)
    MyBase.New(x, y)
  End Sub

  Public Overrides Sub Draw(engine As ConsoleGameEngine.ConsoleGameEngine, offsetX As Single, offsetY As Single)
    engine.DrawWireFrameModel(m_vecModel, Px - offsetX, Py - offsetY, CSng(Math.Atan2(Vy, Vx)), Radius, FG_WHITE)
  End Sub

  Public Overrides Function BounceDeathAction() As Integer
    Return 0 ' Nothing, just fade
  End Function

  Shared Function DefineDummy() As List(Of (Single, Single))
    ' Defines a circle with a line fom center to edge
    Dim vecModel As New List(Of (Single, Single)) From {(0.0F, 0.0F)}
    For i = 0 To 9
      vecModel.Add((CSng(Math.Cos(i / 9.0F * 2.0F * 3.14159F)), CSng(Math.Sin(i / 9.0F * 2.0F * 3.14159F))))
    Next
    Return vecModel
  End Function

End Class

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
    BounceBeforeDeath = 5 ' After 5 bounces, dispose
  End Sub

  Public Overrides Sub Draw(engine As ConsoleGameEngine.ConsoleGameEngine, offsetX As Single, offsetY As Single)
    engine.DrawWireFrameModel(m_vecModel, Px - offsetX, Py - offsetY, CSng(Math.Atan2(Vy, Vx)), Radius, FG_DARK_GREEN)
  End Sub

  Public Overrides Function BounceDeathAction() As Integer
    Return 0 ' Nothing, just fade
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
  End Sub

  Public Overrides Sub Draw(engine As ConsoleGameEngine.ConsoleGameEngine, offsetX As Single, offsetY As Single)
    engine.DrawWireFrameModel(vecModel, Px - offsetX, Py - offsetY, CSng(Math.Atan2(Vy, Vx)), Radius, FG_YELLOW)
  End Sub

  Public Overrides Function BounceDeathAction() As Integer
    Return 20 ' Explode Big
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

  Private Shared m_sprite As Sprite = Nothing

  Public m_shootAngle As Single = 0.0F

  Public Sub New(Optional x As Single = 0.0F, Optional y As Single = 0.0F)
    MyBase.New(x, y)

    Radius = 3.5F
    Friction = 0.2F
    Dead = False
    BounceBeforeDeath = -1

    ' load sprite data from sprite file
    'If m_sprite Is Nothing Then
    m_sprite = New Sprite("worms1.spr")
    'End If

  End Sub

  Public Overrides Sub Draw(engine As ConsoleGameEngine.ConsoleGameEngine, offsetX As Single, offsetY As Single)
    engine.DrawPartialSprite(Px - offsetX - Radius, Py - offsetY - Radius, m_sprite, 0, 0, 8, 8)
  End Sub

  Public Overrides Function BounceDeathAction() As Integer
    Return 0 ' Nothing
  End Function

End Class
