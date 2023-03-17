' Part 1
' "Code-It-Yourself! First Person Shooter (Quick and Simple C++)" -- javidx9
' https://youtu.be/xW8skO7MFYw
' Part 2
' "Upgraded! First Person Shooter at Command Prompt (C++)" -- javidx9
' https://youtu.be/HEb2akswCcw

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine

Module Program

  Sub Main() 'args As String())
    Dim game As New CommandLineFps
    game.ConstructConsole(320, 240, 4, 4)
    game.Start()
  End Sub

End Module

Class CommandLineFps
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private ReadOnly m_mapWidth As Integer = 32 'World Dimensions
  Private ReadOnly m_mapHeight As Integer = 32

  Private m_playerX As Double = 14.7 'Player Start Position
  Private m_playerY As Double = 8 '5.09F
  Private m_playerA As Double = -3.14159 / 2.0 'Player Start Rotation
  Private ReadOnly m_fov As Double = 3.14159 / 4.0 'Field of View
  Private ReadOnly m_depth As Double = 16.0 'Maximum rendering distance
  Private ReadOnly m_speed As Double = 5.0 'Walking Speed

  Private m_map As String

  Private m_spriteWall As Sprite
  Private m_spriteLamp As Sprite
  Private m_spriteFireBall As Sprite

  Private m_depthBuffer() As Double = Nothing

  Private Class ScreenObject
    Public X As Single
    Public Y As Single
    Public Vx As Single
    Public Vy As Single
    Public Remove As Boolean
    Public Sprite As Sprite
  End Class

  Private m_listObjects As New List(Of ScreenObject)()

  Public Overrides Function OnUserCreate() As Boolean

    m_map &= "#########.......#########......."
    m_map &= "#...............#..............."
    m_map &= "#.......#########.......########"
    m_map &= "#..............##..............#"
    m_map &= "#......##......##......##......#"
    m_map &= "#......##..............##......#"
    m_map &= "#..............##..............#"
    m_map &= "###............####............#"
    m_map &= "##.............###.............#"
    m_map &= "#............####............###"
    m_map &= "#..............................#"
    m_map &= "#..............##..............#"
    m_map &= "#..............##..............#"
    m_map &= "#...........#####...........####"
    m_map &= "#..............................#"
    m_map &= "###..####....########....#######"
    m_map &= "####.####.......######.........."
    m_map &= "#...............#..............."
    m_map &= "#.......#########.......##..####"
    m_map &= "#..............##..............#"
    m_map &= "#......##......##.......#......#"
    m_map &= "#......##......##......##......#"
    m_map &= "#..............##..............#"
    m_map &= "###............####............#"
    m_map &= "##.............###.............#"
    m_map &= "#............####............###"
    m_map &= "#..............................#"
    m_map &= "#..............................#"
    m_map &= "#..............##..............#"
    m_map &= "#...........##..............####"
    m_map &= "#..............##..............#"
    m_map &= "################################"

    m_spriteWall = New Sprite("FPSSprites/fps_wall1.spr")
    m_spriteLamp = New Sprite("FPSSprites/fps_lamp1.spr")
    m_spriteFireBall = New Sprite("FPSSprites/fps_fireball1.spr")

    ReDim m_depthBuffer(ScreenWidth() - 1)

    m_listObjects = New List(Of ScreenObject) From {
      New ScreenObject With {.X = 8.5F, .Y = 8.5F, .Vx = 0.0F, .Vy = 0.0F, .Remove = False, .Sprite = m_spriteLamp},
      New ScreenObject With {.X = 7.5F, .Y = 7.5F, .Vx = 0.0F, .Vy = 0.0F, .Remove = False, .Sprite = m_spriteLamp},
      New ScreenObject With {.X = 10.5F, .Y = 3.5F, .Vx = 0.0F, .Vy = 0.0F, .Remove = False, .Sprite = m_spriteLamp}}

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Handle CCW Rotation
    If m_keys(AscW("A"c)).bHeld Then
      m_playerA -= m_speed * 0.5F * elapsedTime
    End If

    ' Handle CW Rotation
    If m_keys(AscW("D"c)).bHeld Then
      m_playerA += m_speed * 0.5F * elapsedTime
    End If

    ' Handle Forwards movement & collision
    If m_keys(AscW("W"c)).bHeld Then
      m_playerX += Math.Sin(m_playerA) * m_speed * elapsedTime
      m_playerY += Math.Cos(m_playerA) * m_speed * elapsedTime
      If m_map.Chars(CInt(Fix(m_playerX)) * m_mapWidth + CInt(Fix(m_playerY))) = "#"c Then
        m_playerX -= Math.Sin(m_playerA) * m_speed * elapsedTime
        m_playerY -= Math.Cos(m_playerA) * m_speed * elapsedTime
      End If
    End If

    ' Handle backwards movement & collision
    If m_keys(AscW("S"c)).bHeld Then
      m_playerX -= Math.Sin(m_playerA) * m_speed * elapsedTime
      m_playerY -= Math.Cos(m_playerA) * m_speed * elapsedTime
      If m_map.Chars(CInt(Fix(m_playerX)) * m_mapWidth + CInt(Fix(m_playerY))) = "#"c Then
        m_playerX += Math.Sin(m_playerA) * m_speed * elapsedTime
        m_playerY += Math.Cos(m_playerA) * m_speed * elapsedTime
      End If
    End If

    ' Handle Strafe Right movement & collision
    If m_keys(AscW("E"c)).bHeld Then
      m_playerX += Math.Cos(m_playerA) * m_speed * elapsedTime
      m_playerY -= Math.Sin(m_playerA) * m_speed * elapsedTime
      If m_map.Chars(CInt(Fix(m_playerX)) * m_mapWidth + CInt(Fix(m_playerY))) = "#" Then
        m_playerX -= Math.Cos(m_playerA) * m_speed * elapsedTime
        m_playerY += Math.Sin(m_playerA) * m_speed * elapsedTime
      End If
    End If

    ' Handle Strafe Left movement & collision
    If m_keys(AscW("Q"c)).bHeld Then
      m_playerX -= Math.Cos(m_playerA) * m_speed * elapsedTime
      m_playerY += Math.Sin(m_playerA) * m_speed * elapsedTime
      If m_map.Chars(CInt(Fix(m_playerX)) * m_mapWidth + CInt(Fix(m_playerY))) = "#" Then
        m_playerX += Math.Cos(m_playerA) * m_speed * elapsedTime
        m_playerY -= Math.Sin(m_playerA) * m_speed * elapsedTime
      End If
    End If

    ' Fire Bullets
    If m_keys(VK_SPACE).bReleased Then
      Dim o As New ScreenObject()
      o.X = CSng(m_playerX)
      o.Y = CSng(m_playerY)
      Dim fNoise = ((Rand / RAND_MAX) - 0.5) * 0.1
      o.Vx = CSng(Math.Sin(m_playerA + fNoise) * 8.0)
      o.Vy = CSng(Math.Cos(m_playerA + fNoise) * 8.0)
      o.Sprite = m_spriteFireBall
      o.Remove = False
      m_listObjects.Add(o)
    End If

    For x = 0 To ScreenWidth() - 1

      ' For each column, calculate the projected ray angle into world space
      Dim rayAngle = (m_playerA - m_fov / 2.0F) + (x / ScreenWidth()) * m_fov

      ' Find distance to wall
      Dim stepSize = 0.01 ' Increment size for ray casting, decrease to increase resolution
      Dim distanceToWall = 0.0 ' distance to wall

      Dim hitWall As Boolean = False ' Set when ray hits wall block
      Dim boundary As Boolean = False ' Set when ray hits boundary between two wall blocks

      Dim eyeX = Math.Sin(rayAngle) ' Unit vector for ray in player space
      Dim eyeY = Math.Cos(rayAngle)

      Dim sampleX = 0.0

      Dim lit As Boolean = False

      ' Incrementally cast ray from player, along ray angle, testing for intersection with a block
      While Not hitWall AndAlso distanceToWall < m_depth
        distanceToWall += stepSize
        Dim testX As Integer = CInt(Fix(m_playerX + eyeX * distanceToWall))
        Dim testY As Integer = CInt(Fix(m_playerY + eyeY * distanceToWall))

        ' Test if ray is out of bounds
        If testX < 0 OrElse testX >= m_mapWidth OrElse testY < 0 OrElse testY >= m_mapHeight Then
          hitWall = True ' Just set distance to maximum depth
          distanceToWall = m_depth
        Else
          ' Ray is inbounds so test to see if the ray cell is a wall block
          If m_map.Chars(testX * m_mapWidth + testY) = "#"c Then
            ' Ray has hit wall
            hitWall = True

            ' Determine where ray has hit wall. Break Block boundary
            ' into 4 line segments
            Dim blockMidX = testX + 0.5
            Dim blockMidY = testY + 0.5

            Dim testPointX = m_playerX + eyeX * distanceToWall
            Dim testPointY = m_playerY + eyeY * distanceToWall

            Dim testAngle = Math.Atan2((testPointY - blockMidY), (testPointX - blockMidX))

            If testAngle >= -3.14159 * 0.25 AndAlso testAngle < 3.14159 * 0.25 Then sampleX = testPointY - testY
            If testAngle >= 3.14159 * 0.25 AndAlso testAngle < 3.14159 * 0.75 Then sampleX = testPointX - testX
            If testAngle < -3.14159 * 0.25 AndAlso testAngle >= -3.14159 * 0.75 Then sampleX = testPointX - testX
            If testAngle >= 3.14159 * 0.75 OrElse testAngle < -3.14159 * 0.75 Then sampleX = testPointY - testY

          End If
        End If
      End While

      ' Calculate distance to ceiling and floor
      Dim ceiling As Integer = ScreenHeight() \ 2 - CInt(Fix(ScreenHeight() / distanceToWall))
      Dim floor As Integer = ScreenHeight() - ceiling

      ' Update Depth Buffer
      m_depthBuffer(x) = distanceToWall

      For y = 0 To ScreenHeight() - 1
        ' Each Row
        If y <= ceiling Then
          Draw(x, y, AscW(" "c))
        ElseIf y > ceiling AndAlso y <= floor Then
          ' Draw Wall
          If distanceToWall < m_depth Then
            Dim sampleY = (y - ceiling) / (floor - ceiling)
            Draw(x, y, m_spriteWall.SampleGlyph(sampleX, sampleY), m_spriteWall.SampleColour(sampleX, sampleY))
          Else
            Draw(x, y, PIXEL_SOLID, 0)
          End If
        Else ' Floor
          Draw(x, y, PIXEL_SOLID, FG_DARK_GREEN)
        End If
      Next

    Next

    ' Update & Draw Objects
    For Each obj In m_listObjects

      ' Update Object Physics
      obj.X += obj.Vx * elapsedTime
      obj.Y += obj.Vy * elapsedTime

      ' Check if object is inside wall - set flag for removal
      If m_map.Chars(CInt(Fix(obj.X)) * m_mapWidth + CInt(Fix(obj.Y))) = "#"c Then
        obj.Remove = True
      End If

      ' Can object be seen?
      Dim vecX = obj.X - m_playerX
      Dim vecY = obj.Y - m_playerY
      Dim distanceFromPlayer = Math.Sqrt(vecX * vecX + vecY * vecY)

      Dim eyeX = Math.Sin(m_playerA)
      Dim eyeY = Math.Cos(m_playerA)

      ' Calculate angle between object and players feet, and players looking direction
      ' to determine if the object is in the players field of view
      Dim ojectAngle = Math.Atan2(eyeY, eyeX) - Math.Atan2(vecY, vecX)
      If ojectAngle < -3.14159 Then
        ojectAngle += 2.0 * 3.14159
      End If
      If ojectAngle > 3.14159 Then
        ojectAngle -= 2.0 * 3.14159
      End If

      Dim inPlayerFov As Boolean = Math.Abs(ojectAngle) < m_fov / 2.0

      If inPlayerFov AndAlso distanceFromPlayer >= 0.5 AndAlso distanceFromPlayer < m_depth AndAlso Not obj.Remove Then
        Dim objectCeiling = (ScreenHeight() / 2.0) - ScreenHeight() / distanceFromPlayer
        Dim objectFloor = ScreenHeight() - objectCeiling
        Dim objectHeight = objectFloor - objectCeiling
        Dim objectAspectRatio = obj.Sprite.Height / obj.Sprite.Width
        Dim objectWidth = objectHeight / objectAspectRatio
        Dim middleOfObject = (0.5F * (ojectAngle / (m_fov / 2.0)) + 0.5) * ScreenWidth()

        ' Draw Lamp
        For lx = 0 To objectWidth - 1
          For ly = 0 To objectHeight - 1
            Dim sampleX = lx / objectWidth
            Dim sampleY = ly / objectHeight
            Dim c = obj.Sprite.SampleGlyph(sampleX, sampleY)
            Dim objectColumn = CInt(Fix(middleOfObject + lx - (objectWidth / 2.0)))
            If objectColumn >= 0 AndAlso objectColumn < ScreenWidth() Then
              If c <> AscW(" "c) AndAlso m_depthBuffer(objectColumn) >= distanceFromPlayer Then
                Draw(objectColumn, objectCeiling + ly, c, obj.Sprite.SampleColour(sampleX, sampleY))
                m_depthBuffer(objectColumn) = distanceFromPlayer
              End If
            End If
          Next
        Next
      End If
    Next

    ' Remove dead objects from object list
    m_listObjects.RemoveAll(Function(o) o.Remove)

    ' Display Map & Player
    For nx = 0 To m_mapWidth - 1
      For ny = 0 To m_mapWidth - 1
        Draw(nx + 1, ny + 1, AscW(m_map.Chars(ny * m_mapWidth + nx)))
      Next
    Next
    Draw(1 + CInt(Fix(m_playerY)), 1 + CInt(Fix(m_playerX)), AscW("P"c))

    Return True

  End Function

End Class