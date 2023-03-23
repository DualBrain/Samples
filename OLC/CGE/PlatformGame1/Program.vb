' Inspired by "Code-It-Yourself! Simple Tile Based Platform Game #1
' https://youtu.be/oJvJZNyW_rw

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine

Module Program

  Sub Main() 'args As String())
    Dim game As New Empty
    game.ConstructConsole(256, 240, 4, 4)
    game.Start()
  End Sub

End Module

Class Empty
  Inherits ConsoleGameEngine.ConsoleGameEngine

  ' Level storage
  Private m_level() As Integer
  Private m_levelWidth As Integer
  Private m_levelHeight As Integer

  ' Player Properties
  Private m_playerPosX As Single = 1.0F
  Private m_playerPosY As Single = 1.0F
  Private m_playerVelX As Single = 0.0F
  Private m_playerVelY As Single = 0.0F
  Private m_playerOnGround As Boolean = False

  ' Camera properties
  Private m_cameraPosX As Single = 0.0F
  Private m_cameraPosY As Single = 0.0F

  ' Sprite Resources
  Private m_spriteTiles As Sprite = Nothing
  Private m_spriteMan As Sprite = Nothing

  ' Sprite selection flags
  Private m_dirModX As Integer = 0
  Private m_dirModY As Integer = 0

  Public Overrides Function OnUserCreate() As Boolean

    m_levelWidth = 64
    m_levelHeight = 16

    Dim level = "................................................................"
    level &= "................................................................"
    level &= ".......ooooo...................................................."
    level &= "........ooo....................................................."
    level &= ".......................########................................."
    level &= ".....BB?BBBB?BB.......###..............#.#......................"
    level &= "....................###................#.#......................"
    level &= "...................####........................................."
    level &= "GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG.##############.....########"
    level &= "...................................#.#...............###........"
    level &= "........................############.#............###..........."
    level &= "........................#............#.........###.............."
    level &= "........................#.############......###................."
    level &= "........................#................###...................."
    level &= "........................#################......................."
    level &= "................................................................"

    ' Convert the string to an array so that it's easier to work with.
    ReDim m_level((m_levelWidth * m_levelHeight) - 1)
    For c = 0 To level.Length - 1
      m_level(c) = AscW(level(c))
    Next

    m_spriteTiles = New Sprite("JarioSprites/leveljario.spr")
    m_spriteMan = New Sprite("JarioSprites/minijario.spr")

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Utility Lambdas
    Dim getTile = Function(x As Single, y As Single) As Integer
                    Dim index = CInt(Fix(y)) * m_levelWidth + CInt(Fix(x))
                    If index >= 0 AndAlso index < (m_levelWidth * m_levelHeight) Then 'x >= 0 AndAlso x < nLevelWidth AndAlso y >= 0 AndAlso y < nLevelHeight Then
                      Return m_level(index)
                    Else
                      Return AscW("#"c) 'AscW(" "c)
                    End If
                  End Function

    Dim setTile = Sub(x As Single, y As Single, c As Integer)
                    Dim index = CInt(Fix(y)) * m_levelWidth + CInt(Fix(x))
                    If index >= 0 AndAlso index < (m_levelWidth * m_levelHeight) Then 'x >= 0 AndAlso x < nLevelWidth AndAlso y >= 0 AndAlso y < nLevelHeight Then
                      m_level(index) = c
                    End If
                  End Sub

    'm_playerVelX = 0.0F
    'm_playerVelY = 0.0F

    ' Handle Input
    If IsFocused() Then

      If GetKey(VK_UP).Held Then
        m_playerVelY = -6.0F
      End If

      If GetKey(VK_DOWN).Held Then
        m_playerVelY = 6.0F
      End If

      If GetKey(VK_LEFT).Held Then
        m_playerVelX += (If(m_playerOnGround, -25.0F, -15.0F)) * elapsedTime
        m_dirModY = 1
      End If

      If GetKey(VK_RIGHT).Held Then
        m_playerVelX += (If(m_playerOnGround, 25.0F, 15.0F)) * elapsedTime
        m_dirModY = 0
      End If

      If GetKey(VK_SPACE).Pressed Then
        If m_playerVelY = 0 Then
          m_playerVelY = -12.0F
          m_dirModX = 1
        End If
      End If

    End If

    ' Gravity
    m_playerVelY += 20.0F * elapsedTime

    ' Drag
    If m_playerOnGround Then
      m_playerVelX += -3.0F * m_playerVelX * elapsedTime
      If Math.Abs(m_playerVelX) < 0.01F Then m_playerVelX = 0.0F
    End If

    ' Clamp velocities
    If m_playerVelX > 10.0F Then m_playerVelX = 10.0F
    If m_playerVelX < -10.0F Then m_playerVelX = -10.0F
    If m_playerVelY > 100.0F Then m_playerVelY = 100.0F
    If m_playerVelY < -100.0F Then m_playerVelY = -100.0F

    ' Calculate potential new position
    Dim newPlayerPosX = m_playerPosX + m_playerVelX * elapsedTime
    Dim newPlayerPosY = m_playerPosY + m_playerVelY * elapsedTime

    If newPlayerPosX < 0 Then newPlayerPosX = 0
    If newPlayerPosX > m_levelWidth - 1 Then newPlayerPosX = m_levelWidth - 1

    Dim sky = AscW("."c)

    ' Check for pickups!
    If getTile(newPlayerPosX + 0, newPlayerPosY + 0) = AscW("o"c) Then
      setTile(newPlayerPosX + 0, newPlayerPosY + 0, sky)
    End If

    If getTile(newPlayerPosX + 0, newPlayerPosY + 1) = AscW("o"c) Then
      setTile(newPlayerPosX + 0, newPlayerPosY + 1, sky)
    End If

    If getTile(newPlayerPosX + 1, newPlayerPosY + 0) = AscW("o"c) Then
      setTile(newPlayerPosX + 1, newPlayerPosY + 0, sky)
    End If

    If getTile(newPlayerPosX + 1, newPlayerPosY + 1) = AscW("o"c) Then
      setTile(newPlayerPosX + 1, newPlayerPosY + 1, sky)
    End If

    ' Check for Collision
    If m_playerVelX <= 0 Then ' Moving Left
      If getTile(newPlayerPosX + 0.0F, m_playerPosY + 0.0F) <> sky OrElse getTile(newPlayerPosX + 0.0F, m_playerPosY + 0.9F) <> sky Then
        newPlayerPosX = CInt(Fix(newPlayerPosX)) + 1
        m_playerVelX = 0
      End If
    Else ' Moving Right
      If getTile(newPlayerPosX + 1.0F, m_playerPosY + 0.0F) <> sky OrElse getTile(newPlayerPosX + 1.0F, m_playerPosY + 0.9F) <> sky Then
        newPlayerPosX = CInt(Fix(newPlayerPosX))
        m_playerVelX = 0
      End If
    End If

    m_playerOnGround = False
    If m_playerVelY <= 0 Then ' Moving Up
      If getTile(newPlayerPosX + 0.0F, newPlayerPosY) <> sky OrElse getTile(newPlayerPosX + 0.9F, newPlayerPosY) <> sky Then
        newPlayerPosY = CInt(Fix(newPlayerPosY)) + 1
        m_playerVelY = 0
      End If
    Else ' Moving Down
      If getTile(newPlayerPosX + 0.0F, newPlayerPosY + 1.0F) <> sky OrElse getTile(newPlayerPosX + 0.9F, newPlayerPosY + 1.0F) <> sky Then
        newPlayerPosY = CInt(Fix(newPlayerPosY))
        m_playerVelY = 0
        m_playerOnGround = True ' Player has a solid surface underfoot
        m_dirModX = 0
      End If
    End If

    ' Apply new position
    m_playerPosX = newPlayerPosX
    m_playerPosY = newPlayerPosY

    ' Link camera to player position
    m_cameraPosX = m_playerPosX
    m_cameraPosY = m_playerPosY

    ' Draw Level
    Dim tileWidth = 16
    Dim tileHeight = 16
    'Dim visibleTilesX = CInt(Fix(ScreenWidth() / tileWidth))
    'Dim visibleTilesY = CInt(Fix(ScreenHeight() / tileHeight))
    Dim visibleTilesX = ScreenWidth() \ tileWidth
    Dim visibleTilesY = ScreenHeight() \ tileHeight

    ' Calculate Top-Leftmost visible tile
    Dim offsetX = m_cameraPosX - visibleTilesX / 2.0F
    Dim offsetY = m_cameraPosY - visibleTilesY / 2.0F

    ' Clamp camera to game boundaries
    If offsetX < 0 Then offsetX = 0
    If offsetY < 0 Then offsetY = 0
    If offsetX > m_levelWidth - visibleTilesX Then offsetX = m_levelWidth - visibleTilesX
    If offsetY > m_levelHeight - visibleTilesY Then offsetY = m_levelHeight - visibleTilesY

    ' Get offsets for smooth movement
    Dim tileOffsetX = (offsetX - CInt(Fix(offsetX))) * tileWidth
    Dim tileOffsetY = (offsetY - CInt(Fix(offsetY))) * tileHeight

    ' Draw visible tile map
    For x = -1 To visibleTilesX + 1 'Step 1
      For y = -1 To visibleTilesY + 1 'Step 1
        Dim titleId = getTile(x + offsetX, y + offsetY)
        Select Case ChrW(titleId)
          Case "."c ' Sky
            Fill(x * tileWidth - tileOffsetX, y * tileHeight - tileOffsetY, (x + 1) * tileWidth - tileOffsetX, (y + 1) * tileHeight - tileOffsetY, PIXEL_SOLID, FG_CYAN)
          Case "#"c ' Solid Block
            'DrawPartialSprite(x * nTileWidth - fTileOffsetX, y * nTileHeight - fTileOffsetY, spriteTiles, 2 * nTileWidth, 0 * nTileHeight, nTileWidth, nTileHeight)
            DrawPartialSprite(x * tileWidth - tileOffsetX, y * tileHeight - tileOffsetY, m_spriteTiles, 2 * tileWidth, 0 * tileHeight, tileWidth, tileHeight)
          Case "G"c ' Ground Block
            DrawPartialSprite(x * tileWidth - tileOffsetX, y * tileHeight - tileOffsetY, m_spriteTiles, 0 * tileWidth, 0 * tileHeight, tileWidth, tileHeight)
          Case "B"c ' Brick Block
            DrawPartialSprite(x * tileWidth - tileOffsetX, y * tileHeight - tileOffsetY, m_spriteTiles, 0 * tileWidth, 1 * tileHeight, tileWidth, tileHeight)
          Case "?"c ' Question Block
            DrawPartialSprite(x * tileWidth - tileOffsetX, y * tileHeight - tileOffsetY, m_spriteTiles, 1 * tileWidth, 1 * tileHeight, tileWidth, tileHeight)
          Case "o"c ' Coin
            Fill(x * tileWidth - tileOffsetX, y * tileHeight - tileOffsetY, (x + 1) * tileWidth - tileOffsetX, (y + 1) * tileHeight - tileOffsetY, PIXEL_SOLID, FG_CYAN)
            DrawPartialSprite(x * tileWidth - tileOffsetX, y * tileHeight - tileOffsetY, m_spriteTiles, 3 * tileWidth, 0 * tileHeight, tileWidth, tileHeight)
          Case Else
            Fill(x * tileWidth - tileOffsetX, y * tileHeight - tileOffsetY, (x + 1) * tileWidth - tileOffsetX, (y + 1) * tileHeight - tileOffsetY, PIXEL_SOLID, FG_BLACK)
        End Select
      Next
    Next

    ' Draw Player
    'Fill((m_playerPosX - offsetX) * tileWidth, (m_playerPosY - offsetY) * tileWidth, (m_playerPosX - offsetX + 1.0F) * tileWidth, (m_playerPosY - offsetY + 1.0F) * tileWidth, PIXEL_SOLID, FG_GREEN)
    DrawPartialSprite((m_playerPosX - offsetX) * tileWidth, (m_playerPosY - offsetY) * tileWidth, m_spriteMan, m_dirModX * tileWidth, m_dirModY * tileHeight, tileWidth, tileHeight)

    Return True

  End Function

End Class