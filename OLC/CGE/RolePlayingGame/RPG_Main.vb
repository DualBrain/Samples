Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine

Class RPG_Main
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_pCurrentMap As Map = Nothing

  Private m_pPlayer As Dynamic = Nothing

  Private ReadOnly m_dynamics As New List(Of Dynamic)

  Private ReadOnly m_script As New ScriptProcessor

  ' Camera properties
  Private m_cameraPosX As Single = 0.0F
  Private m_cameraPosY As Single = 0.0F

  ' Sprite Resources
  Private m_sprFont As Sprite = Nothing

  Public Overrides Function OnUserCreate() As Boolean

    m_sAppName = "Top Down Role Playing Game"

    Command.g_engine = Me
    RPG_Assets.Get().LoadSprites()

    m_sprFont = RPG_Assets.Get.GetSprite("font")

    m_pCurrentMap = New Map_Village1()

    m_pPlayer = New Dynamic_Creature("player", RPG_Assets.Get.GetSprite("player")) With {.Px = 5, .Py = 5}

    m_dynamics.Add(m_pPlayer)
    m_dynamics.Add(New Dynamic_Creature("skelly1", RPG_Assets.Get.GetSprite("skelly")) With {.Px = 12, .Py = 12})
    m_dynamics.Add(New Dynamic_Creature("skelly2", RPG_Assets.Get.GetSprite("skelly")) With {.Px = 5, .Py = 8})

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Update script
    m_script.ProcessCommands(elapsedTime)

    If m_script.UserControlEnabled Then

      m_pPlayer.Vx = 0.0F
      m_pPlayer.Vy = 0.0F

      ' Handle Input
      If IsFocused() Then

        If GetKey(VK_UP).Held Then m_pPlayer.Vy = -4.0F
        If GetKey(VK_DOWN).Held Then m_pPlayer.Vy = 4.0F
        If GetKey(VK_LEFT).Held Then m_pPlayer.Vx = -4.0F
        If GetKey(VK_RIGHT).Held Then m_pPlayer.Vx = 4.0F

        If GetKey(AscW("Z"c)).Released Then
          'NOTE: C++ supports "macros" and in the video there was an X macro created that "save typing" of the `m_script.AddCommand(New Command_` portion of the below entries.
          m_script.AddCommand(New Command_MoveTo(m_pPlayer, 10, 10, 3.0F))
          m_script.AddCommand(New Command_MoveTo(m_pPlayer, 15, 10, 3.0F))
          m_script.AddCommand(New Command_MoveTo(m_dynamics(1), 15, 12, 2.0F))
          m_script.AddCommand(New Command_ShowDialog(New List(Of String) From {"Grrrrr!"}))
          m_script.AddCommand(New Command_ShowDialog(New List(Of String) From {"I think OOP", "is really useful!"}))
          m_script.AddCommand(New Command_MoveTo(m_pPlayer, 15, 15, 3.0F))
          m_script.AddCommand(New Command_MoveTo(m_pPlayer, 10, 10, 3.0F))
        End If

      End If

    Else
      If m_showDialog Then
        If GetKey(VK_SPACE).Released Then
          m_showDialog = False
          m_script.CompleteCommand()
        End If
      End If
    End If

    For Each obj In m_dynamics

      ' Calculate potential new position
      Dim newObjectPosX = obj.Px + obj.Vx * elapsedTime
      Dim newObjectPosY = obj.Py + obj.Vy * elapsedTime

      If newObjectPosY < 0 Then newObjectPosX = 0
      If newObjectPosX > m_pCurrentMap.nWidth - 1 Then newObjectPosX = m_pCurrentMap.nWidth - 1

      ' Check for Collision
      If obj.Vx <= 0 Then ' Moving Left
        If m_pCurrentMap.GetSolid(newObjectPosX + 0.0F, obj.Py + 0.0F) OrElse m_pCurrentMap.GetSolid(newObjectPosX + 0.0F, obj.Py + 0.9F) Then
          newObjectPosX = CInt(Fix(newObjectPosX)) + 1
          obj.Vx = 0
        End If
      Else ' Moving Right
        If m_pCurrentMap.GetSolid(newObjectPosX + 1.0F, obj.Py + 0.0F) OrElse m_pCurrentMap.GetSolid(newObjectPosX + 1.0F, obj.Py + 0.9F) Then
          newObjectPosX = CInt(Fix(newObjectPosX))
          obj.Vx = 0
        End If
      End If

      If obj.Vy <= 0 Then ' Moving Up
        If m_pCurrentMap.GetSolid(newObjectPosX + 0.0F, newObjectPosY) OrElse m_pCurrentMap.GetSolid(newObjectPosX + 0.9F, newObjectPosY) Then
          newObjectPosY = CInt(Fix(newObjectPosY)) + 1
          obj.Vy = 0
        End If
      Else ' Moving Down
        If m_pCurrentMap.GetSolid(newObjectPosX + 0.0F, newObjectPosY + 1.0F) OrElse m_pCurrentMap.GetSolid(newObjectPosX + 0.9F, newObjectPosY + 1.0F) Then
          newObjectPosY = CInt(Fix(newObjectPosY))
          obj.Vy = 0
        End If
      End If

      ' Apply new position
      obj.Px = newObjectPosX
      obj.Py = newObjectPosY

      obj.Update(elapsedTime)

    Next

    ' Link camera to player position
    m_cameraPosX = m_pPlayer.Px
    m_cameraPosY = m_pPlayer.Py

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

    ' Draw Object
    For Each obj In m_dynamics
      obj.DrawSelf(Me, offsetX, offsetY)
    Next
    m_pPlayer.DrawSelf(Me, offsetX, offsetY)

    If m_showDialog Then
      DisplayDialog(m_dialogToShow, 20, 20)
    End If

    'DrawBigText("Hello Everybody!", 10, 10)
    'DrawBigText("Code-It-Yourself RPG Part #2", 10, 20)

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

  Private m_dialogToShow As List(Of String)
  Private m_showDialog As Boolean = False
  Private m_dialogX As Single = 0.0F
  Private m_dialogY As Single = 0.0F

  Friend Sub ShowDialog(lines As List(Of String))
    m_dialogToShow = lines
    m_showDialog = True
  End Sub

  Private Sub DisplayDialog(lines As List(Of String), x As Integer, y As Integer)

    Dim maxLineLength = 0
    Dim count = lines.Count

    For Each line In lines
      If line.Length > maxLineLength Then maxLineLength = line.Length
    Next

    ' Draw Box
    Fill(x - 1, y - 1, x + maxLineLength * 8 + 1, y + count * 8 + 1, PIXEL_SOLID, FG_DARK_BLUE)
    DrawLine(x - 2, y - 2, x - 2, y + count * 8 + 1)
    DrawLine(x + maxLineLength * 8 + 1, y - 2, x + maxLineLength * 8 + 1, y + count * 8 + 1)
    DrawLine(x - 2, y - 2, x + maxLineLength * 8 + 1, y - 2)
    DrawLine(x - 2, y + count * 8 + 1, x + maxLineLength * 8 + 1, y + count * 8 + 1)

    For l = 0 To lines.Count - 1
      DrawBigText(lines(l), x, y + l * 8)
    Next

  End Sub

End Class