' Inspired by "Code-It-Yourself! Role Playing Game Part #1" -- @javidx9
' https://youtu.be/xXXt3htgDok

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine

Module Program

  Sub Main() 'args As String())
    Dim game As New Rpg
    game.ConstructConsole(256, 240, 4, 4)
    game.Start()
  End Sub

End Module

Class Rpg
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_pCurrentMap As Map = Nothing

  ' Player Properties
  Private m_playerPosX As Single = 1.0F
  Private m_playerPosY As Single = 1.0F
  Private m_playerVelX As Single = 0.0F
  Private m_playerVelY As Single = 0.0F

  ' Camera properties
  Private m_cameraPosX As Single = 0.0F
  Private m_cameraPosY As Single = 0.0F

  ' Sprite Resources
  Private m_spriteMan As Sprite = Nothing
  Private m_sprFont As Sprite = Nothing

  ' Sprite selection flags
  Private m_dirModX As Integer = 0
  Private m_dirModY As Integer = 0

  Public Overrides Function OnUserCreate() As Boolean

    RPG_Assets.Get().LoadSprites()

    m_sprFont = RPG_Assets.Get.GetSprite("font")

    m_pCurrentMap = New Map_Village1()

    m_spriteMan = New Sprite("JarioSprites/minijario.spr")

    m_playerPosY = 5.0F
    m_playerPosX = 5.0F

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Handle Input
    If IsFocused() Then

      If GetKey(VK_UP).Held Then
        m_playerVelY = -4.0F
      End If

      If GetKey(VK_DOWN).Held Then
        m_playerVelY = 4.0F
      End If

      If GetKey(VK_LEFT).Held Then
        m_playerVelX = -4.0F
        m_dirModY = 1
      End If

      If GetKey(VK_RIGHT).Held Then
        m_playerVelX = 4.0F
        m_dirModY = 0
      End If

    End If

    ' Gravity
    'm_playerVelX -= 1.0F * elapsedTime

    ' Drag
    'If m_playerOnGround Then
    m_playerVelX += -10.0F * m_playerVelX * elapsedTime
    If Math.Abs(m_playerVelX) < 0.01F Then m_playerVelX = 0.0F
    m_playerVelY += -10.0F * m_playerVelY * elapsedTime
    If Math.Abs(m_playerVelY) < 0.01F Then m_playerVelY = 0.0F
    'End If

    ' Clamp velocities
    If m_playerVelX > 10.0F Then m_playerVelX = 10.0F
    If m_playerVelX < -10.0F Then m_playerVelX = -10.0F
    If m_playerVelY > 100.0F Then m_playerVelY = 100.0F
    If m_playerVelY < -100.0F Then m_playerVelY = -100.0F

    ' Calculate potential new position
    Dim newPlayerPosX = m_playerPosX + m_playerVelX * elapsedTime
    Dim newPlayerPosY = m_playerPosY + m_playerVelY * elapsedTime

    If newPlayerPosX < 0 Then newPlayerPosX = 0
    If newPlayerPosX > m_pCurrentMap.nWidth - 1 Then newPlayerPosX = m_pCurrentMap.nWidth - 1

    ' Check for Collision
    If m_playerVelX <= 0 Then ' Moving Left
      If m_pCurrentMap.GetSolid(newPlayerPosX + 0.0F, m_playerPosY + 0.0F) OrElse m_pCurrentMap.GetSolid(newPlayerPosX + 0.0F, m_playerPosY + 0.9F) Then
        newPlayerPosX = CInt(Fix(newPlayerPosX)) + 1
        m_playerVelX = 0
      End If
    Else ' Moving Right
      If m_pCurrentMap.GetSolid(newPlayerPosX + 1.0F, m_playerPosY + 0.0F) OrElse m_pCurrentMap.GetSolid(newPlayerPosX + 1.0F, m_playerPosY + 0.9F) Then
        newPlayerPosX = CInt(Fix(newPlayerPosX))
        m_playerVelX = 0
      End If
    End If

    If m_playerVelY <= 0 Then ' Moving Up
      If m_pCurrentMap.GetSolid(newPlayerPosX + 0.0F, newPlayerPosY) OrElse m_pCurrentMap.GetSolid(newPlayerPosX + 0.9F, newPlayerPosY) Then
        newPlayerPosY = CInt(Fix(newPlayerPosY)) + 1
        m_playerVelY = 0
      End If
    Else ' Moving Down
      If m_pCurrentMap.GetSolid(newPlayerPosX + 0.0F, newPlayerPosY + 1.0F) OrElse m_pCurrentMap.GetSolid(newPlayerPosX + 0.9F, newPlayerPosY + 1.0F) Then
        newPlayerPosY = CInt(Fix(newPlayerPosY))
        m_playerVelY = 0
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
    Dim visibleTilesX = ScreenWidth() \ tileWidth
    Dim visibleTilesY = ScreenHeight() \ tileHeight

    ' Calculate Top-Leftmost visible tile
    Dim offsetX = m_cameraPosX - visibleTilesX / 2.0F
    Dim offsetY = m_cameraPosY - visibleTilesY / 2.0F

    ' Clamp camera to game boundaries
    If offsetX < 0 Then offsetX = 0
    If offsetY < 0 Then offsetY = 0
    If offsetX > m_pCurrentMap.nWidth - visibleTilesX Then offsetX = m_pCurrentMap.nWidth - visibleTilesX
    If offsetY > m_pCurrentMap.nHeight - visibleTilesY Then offsetY = m_pCurrentMap.nHeight - visibleTilesY

    ' Get offsets for smooth movement
    Dim tileOffsetX = (offsetX - CInt(Fix(offsetX))) * tileWidth
    Dim tileOffsetY = (offsetY - CInt(Fix(offsetY))) * tileHeight

    ' Draw visible tile map
    For x = -1 To visibleTilesX + 1
      For y = -1 To visibleTilesY + 1

        Dim idx = m_pCurrentMap.GetIndex(x + offsetX, y + offsetY)
        Dim sx = idx Mod 10
        Dim sy = CInt(Fix(idx / 10))
        DrawPartialSprite(x * tileWidth - tileOffsetX, y * tileHeight - tileOffsetY, m_pCurrentMap.pSprite, sx * tileWidth, sy * tileHeight, tileWidth, tileHeight)

      Next
    Next

    ' Draw Player
    DrawPartialSprite((m_playerPosX - offsetX) * tileWidth, (m_playerPosY - offsetY) * tileWidth, m_spriteMan, m_dirModX * tileWidth, m_dirModY * tileHeight, tileWidth, tileHeight)

    DrawBigText("Hello Everybody!", 30, 30)

    Return True

  End Function

  Private Sub DrawBigText(text As String, x As Integer, y As Integer)

    Dim i = 0
    For Each c In text
      Dim sx = ((AscW(c) - 32) Mod 16) * 8 ' Offset by 32 since the sprite sheet starts as ASCII 32 (space).
      Dim sy = ((AscW(c) - 32) \ 16) * 8
      DrawPartialSprite(x + (i * 8), y, m_sprFont, sx, sy, 8, 8)
      i += 1
    Next

  End Sub

End Class