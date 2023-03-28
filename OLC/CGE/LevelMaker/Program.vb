Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine
Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Module Program

  Sub Main() 'args As String())
    Dim game As New LevelMaker
    game.ConstructConsole(400, 200, 4, 4)
    game.Start()
  End Sub

End Module

Friend Module Constants

  Friend Const TILE_WIDTH As Integer = 16
  Friend Const FONT_SPRITESHEET As String = "javidx9_nesfont8x8.spr"
  Friend Const TILE_SPRITESHEET As String = "loztheme.spr"
  Friend Const MAP_WIDTH As Integer = 10
  Friend Const MAP_HEIGHT As Integer = 10
  Friend Const DEFAULT_TILE As Integer = 14

  Friend Enum Tool
    TILES
    META
    EXPORT_IMPORT
    LAST
  End Enum

  Friend Enum Popup
    NONE
    NEW_MAP_SIZE
  End Enum

  Friend Class Popup_t

    Friend popup As Popup
    Friend menuActive As Boolean
    Friend newMapSize As NewMapSize_t

    Friend Structure NewMapSize_t
      Friend width As String
      Friend height As String
      Friend field As Integer ' 0 - width, 1 - height
    End Structure

    Friend Sub New()
      popup = Popup.NONE
      menuActive = False
      newMapSize = New NewMapSize_t()
    End Sub

  End Class

  ' embedded fill icon meta, should have a better way to store this...
  Friend fillSpriteData As String = "8 8 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 15 9608 0 0 0 0 0 0 0 0 12 9608 12 9608 12 9608 12 9608 15 9608 0 0 0 0 0 0 12 9608 15 9608 12 9608 12 9608 12 9608 15 9608 0 0 0 0 12 9608 0 9608 15 9608 12 9608 12 9608 12 9608 15 9608 0 0 12 9608 0 9608 0 9608 15 9608 12 9608 15 9608 0 0 0 0 12 9608 0 0 0 9608 0 9608 15 9608 0 9608 0 0 0 0 0 0 0 0 0 0 0 9608 0 9608 0 0 0 0"

End Module

Class LevelMaker
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private mapMoveX, mapMoveY As Single
  Private font As New SpriteSheet
  Private tiles As New SpriteSheet
  Private fillIcon As New Sprite
  Private tool As Tool = Tool.TILES
  Private level As New Level
  Private page As Integer = 0
  Private selectedSprite As Integer = 0
  Private pageCount As Integer = 1
  Private uiBase As Integer = 300
  Private tilesPerRow, tilesPerColumn, tilesPerPage, uiWidth As Integer
  Private worldOffsetX, worldOffsetY As Single
  Private grid As Boolean = False
  Private floodMode As Boolean = False
  Private pickedFirst, pickedSecond As Boolean
  Private startRec As (Integer, Integer)
  Private endRex As (Integer, Integer)
  Private file As String = ""
  Private spriteSheetFile As String = TILE_SPRITESHEET
  Private popup As New Popup_t()

  Public Sub New()
    m_sAppName = "Level Maker"
  End Sub

  Private Sub DrawStringFont(x As Integer, y As Integer, characters As String)
    ' will use ascii
    For Each c In characters
      Dim tileIndex = AscW(c) - AscW(" "c)
      If tileIndex >= font.GetTileCount() Then
        Continue For
      End If
      DrawSprite(x, y, font.Item(tileIndex))
      x += font.GetTileWidth()
    Next c
  End Sub

  Public Overrides Function OnUserCreate() As Boolean

    font.Load(FONT_SPRITESHEET, 8, 8)

    Dim values = fillSpriteData.Split(" "c)
    Dim index = 0
    Dim fillWidth, fillHeight As Integer
    fillWidth = Integer.Parse(values(index)) : index += 1
    fillHeight = Integer.Parse(values(index)) : index += 1
    fillIcon = New Sprite(fillWidth, fillHeight)
    For sy = 0 To fillIcon.Height - 1
      For sx = 0 To fillIcon.Width - 1
        Dim color = CType(Integer.Parse(values(index)), Colour) : index += 1
        Dim glyph = Integer.Parse(values(index)) : index += 1
        fillIcon.SetColour(sx, sy, color)
        fillIcon.SetGlyph(sx, sy, glyph)
      Next
    Next

    level.Create(MAP_WIDTH, MAP_HEIGHT)
    For i = 0 To level.GetWidth() * level.GetHeight() - 1
      level.Item(i) = New Tile()
      level.Item(i).SetLevel(level)
      level.Item(i).SetSpriteId(DEFAULT_TILE)
    Next

    ' search for the default sprite sheet
    Dim spriteFile As New FileStream(TILE_SPRITESHEET, FileMode.Open, FileAccess.Read)
    If Not spriteFile.CanRead Then
      ImportSpriteSheet()
    Else
      level.LoadSpriteSheet(TILE_SPRITESHEET, TILE_WIDTH)
      tiles = level.GetSpriteSheet()
    End If
    spriteFile.Close()

    uiWidth = 400 - uiBase
    tilesPerRow = CInt(Fix(uiWidth / tiles.GetTileWidth()))
    tilesPerColumn = CInt(Fix((200 - 23) / tiles.GetTileHeight()))
    tilesPerPage = tilesPerColumn * tilesPerRow
    pageCount = CInt(Fix((tiles.GetTileCount() / tilesPerPage) + 1))

    Return True

  End Function

  Private moved As Boolean = False

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Dim fTileX = m_mousePosX / 16.0F
    Dim fTileY = m_mousePosY / 16.0F
    ' simple
    Dim tileX = CInt(Math.Round(fTileX - worldOffsetX / 16))
    Dim tileY = CInt(Math.Round(fTileY - worldOffsetY / 16))

    Fill(0, 0, 300, 200, AscW(" "c), BG_BLACK Or FG_BLACK)

    Dim topTileX = -CInt(Fix(Math.Floor(worldOffsetX / 16.0F))) - 1
    Dim topTileY = -CInt(Fix(Math.Floor(worldOffsetY / 16.0F))) - 1

    ' draw map
    For y = topTileY To topTileY + CInt(Fix(Math.Ceiling(200.0 / TILE_WIDTH))) - 1
      For x = topTileX To topTileX + CInt(Fix(Math.Ceiling(300.0 / TILE_WIDTH))) - 1

        If x < 0 OrElse x >= level.GetWidth() OrElse y < 0 OrElse y >= level.GetHeight() Then
          Continue For
        End If

        Dim i = x + y * level.GetWidth()
        If i < 0 OrElse i >= level.GetWidth() * level.GetHeight() Then
          Continue For
        End If

        Dim screenX = CInt(Fix(x * TILE_WIDTH + worldOffsetX))
        Dim screenY = CInt(Fix(y * TILE_WIDTH + worldOffsetY))
        If screenX < -16 OrElse screenX >= 300 OrElse screenY < -16 OrElse screenY >= 200 Then
          Continue For
        End If

        If screenX < 0 Then
          screenX = 0
        End If

        If screenY < 0 Then
          screenY = 0
        End If

        DrawSprite(screenX, screenY, level(i).GetSprite())

        If grid Then
          DrawLine(screenX, screenY, screenX + TILE_WIDTH, screenY, PIXEL_SOLID, BG_BLACK Or FG_BLACK)
          DrawLine(screenX, screenY, screenX, screenY + TILE_WIDTH, PIXEL_SOLID, BG_BLACK Or FG_BLACK)
          DrawLine(screenX + TILE_WIDTH, screenY, screenX + TILE_WIDTH, screenY + TILE_WIDTH, PIXEL_SOLID, BG_BLACK Or FG_BLACK)
          DrawLine(screenX, screenY + TILE_WIDTH, screenX + TILE_WIDTH, screenY + TILE_WIDTH, PIXEL_SOLID, BG_BLACK Or FG_BLACK)
        End If

        If tool = Tool.META Then
          Dim offset = 0
          If level(i).IsSolid() Then
            Fill(screenX + offset, screenY + offset, screenX + 3 + offset, screenY + 3 + offset, PIXEL_SOLID, BG_BLACK Or FG_RED)
            offset += 3
          End If
        End If
      Next
    Next

    If popup.menuActive Then

      Select Case popup.popup
        Case Constants.Popup.NEW_MAP_SIZE

#Region "New Map Size Popup"

          ' New Map Size Popup
          Fill(110, 70, 290, 150, " "c, FG_DARK_GREY Or BG_DARK_GREY)
          DrawStringFont(150, 75, "New Map Size")
          DrawStringFont(125, 90, "Width")
          DrawStringFont(210, 90, "Height")
          Fill(170, 130, 240, 140, " "c, FG_GREY Or BG_GREY)
          DrawStringFont(180, 131, "Create")
          Select Case popup.newMapSize.field
            Case 0
              Fill(125, 100, 200, 108, " "c, FG_GREY Or BG_GREY)
              Fill(210, 100, 280, 108, " "c, FG_BLACK Or BG_BLACK)
              If IsFocused() Then
                If GetKey("0").Pressed OrElse GetKey(VK_NUMPAD0).Pressed Then
                  popup.newMapSize.width &= "0"c
                End If
                If GetKey("1").Pressed OrElse GetKey(VK_NUMPAD1).Pressed Then
                  popup.newMapSize.width &= "1"c
                End If
                If GetKey("2").Pressed OrElse GetKey(VK_NUMPAD2).Pressed Then
                  popup.newMapSize.width &= "2"c
                End If
                If GetKey("3").Pressed OrElse GetKey(VK_NUMPAD3).Pressed Then
                  popup.newMapSize.width &= "3"c
                End If
                If GetKey("4").Pressed OrElse GetKey(VK_NUMPAD4).Pressed Then
                  popup.newMapSize.width &= "4"c
                End If
                If GetKey("5").Pressed OrElse GetKey(VK_NUMPAD5).Pressed Then
                  popup.newMapSize.width &= "5"c
                End If
                If GetKey("6").Pressed OrElse GetKey(VK_NUMPAD6).Pressed Then
                  popup.newMapSize.width &= "6"c
                End If
                If GetKey("7").Pressed OrElse GetKey(VK_NUMPAD7).Pressed Then
                  popup.newMapSize.width &= "7"c
                End If
                If GetKey("8").Pressed OrElse GetKey(VK_NUMPAD8).Pressed Then
                  popup.newMapSize.width &= "8"c
                End If
                If GetKey("9").Pressed OrElse GetKey(VK_NUMPAD9).Pressed Then
                  popup.newMapSize.width &= "9"c
                End If
                If GetKey(VK_BACK).Pressed Then
                  If popup.newMapSize.width.Length <> 0 Then
                    popup.newMapSize.width = popup.newMapSize.width.Substring(0, popup.newMapSize.width.Length - 1)
                  End If
                End If
              End If
            Case 1
              Fill(125, 100, 200, 108, " "c, FG_BLACK Or BG_BLACK)
              Fill(210, 100, 280, 108, " "c, FG_GREY Or BG_GREY)
              If IsFocused() Then
                If GetKey(AscW("0"c)).Pressed OrElse GetKey(VK_NUMPAD0).Pressed Then
                  popup.newMapSize.height &= "0"
                End If
                If GetKey(AscW("1"c)).Pressed OrElse GetKey(VK_NUMPAD1).Pressed Then
                  popup.newMapSize.height &= "1"
                End If
                If GetKey(AscW("2"c)).Pressed OrElse GetKey(VK_NUMPAD2).Pressed Then
                  popup.newMapSize.height &= "2"
                End If
                If GetKey(AscW("3"c)).Pressed OrElse GetKey(VK_NUMPAD3).Pressed Then
                  popup.newMapSize.height &= "3"
                End If
                If GetKey(AscW("4"c)).Pressed OrElse GetKey(VK_NUMPAD4).Pressed Then
                  popup.newMapSize.height &= "4"
                End If
                If GetKey(AscW("5"c)).Pressed OrElse GetKey(VK_NUMPAD5).Pressed Then
                  popup.newMapSize.height &= "5"
                End If
                If GetKey(AscW("6"c)).Pressed OrElse GetKey(VK_NUMPAD6).Pressed Then
                  popup.newMapSize.height &= "6"
                End If
                If GetKey(AscW("7"c)).Pressed OrElse GetKey(VK_NUMPAD7).Pressed Then
                  popup.newMapSize.height &= "7"
                End If
                If GetKey(AscW("8"c)).Pressed OrElse GetKey(VK_NUMPAD8).Pressed Then
                  popup.newMapSize.height &= "8"
                End If
                If GetKey(AscW("9"c)).Pressed OrElse GetKey(VK_NUMPAD9).Pressed Then
                  popup.newMapSize.height &= "9"
                End If
                If GetKey(VK_BACK).Pressed Then
                  If popup.newMapSize.height.Length <> 0 Then
                    popup.newMapSize.height = popup.newMapSize.height.Substring(0, popup.newMapSize.height.Length - 1)
                  End If
                End If
              End If
            Case Else
              popup.newMapSize.field = 0
          End Select
          DrawStringFont(125, 100, popup.newMapSize.width)
          DrawStringFont(210, 100, popup.newMapSize.height)
          If m_mouse(0).Pressed Then
            If m_mousePosX > 125 AndAlso m_mousePosX < 200 AndAlso m_mousePosY > 100 AndAlso m_mousePosY < 108 Then
              popup.newMapSize.field = 0
            End If
            If m_mousePosX > 210 AndAlso m_mousePosX < 280 AndAlso m_mousePosY > 100 AndAlso m_mousePosY < 108 Then
              popup.newMapSize.field = 1
            End If
            If m_mousePosX > 170 AndAlso m_mousePosX < 240 AndAlso m_mousePosY > 130 AndAlso m_mousePosY < 140 Then
              ' Create the map
              popup.popup = Constants.Popup.NONE
              popup.menuActive = False
              Dim width = Integer.Parse(popup.newMapSize.width)
              Dim height = Integer.Parse(popup.newMapSize.height)
              If width <> 0 AndAlso height <> 0 Then
                popup.newMapSize.width = ""
                popup.newMapSize.height = ""
                level.Create(width, height)
                For i = 0 To level.GetWidth() * level.GetHeight() - 1
                  level(i).SetLevel(level)
                  level(i).SetSpriteId(DEFAULT_TILE)
                Next
                level.LoadSpriteSheet(spriteSheetFile, TILE_WIDTH)
                tiles = level.GetSpriteSheet()
                file = ""
              End If
            End If
          End If

#End Region

        Case Else
      End Select

    ElseIf IsFocused() Then

      ' fill the menu
      Fill(uiBase, 0, 400, 200, " "c, BG_GREY Or FG_BLACK)

      If Not GetKey(VK_CONTROL).Held AndAlso ((pickedFirst AndAlso pickedSecond) OrElse (Not pickedFirst AndAlso Not pickedSecond)) Then
        If tool = Tool.TILES Then
          TilesTool(tileX, tileY)
        End If
        If tool = Tool.META Then
          metaTool(tileX, tileY)
        End If
        If tool = Tool.EXPORT_IMPORT Then
          exportAndImportTool()
        End If
      End If

      If tileX >= 0 AndAlso tileY >= 0 AndAlso tileX < level.GetWidth() AndAlso tileY < level.GetHeight() AndAlso m_mousePosX <= 300 Then
        ' draw coords
        Dim str As New StringBuilder("<")
        str.Append(tileX)
        str.Append(", ")
        str.Append(tileY)
        str.Append(">")
        DrawStringFont(0, 0, str.ToString())

        ' draw hovered tile rect
        If pickedFirst Then
          Dim firstRectTileX As Integer = startRec.Item1
          Dim firstRectTileY As Integer = startRec.Item2
          Dim secondRectTileX As Integer = tileX
          Dim secondRectTileY As Integer = tileY
          If pickedSecond Then
            secondRectTileX = endRex.Item1
            secondRectTileY = endRex.Item2
          End If

          Dim rectWidth As Integer = Math.Abs(firstRectTileX - secondRectTileX)
          Dim rectHeight As Integer = Math.Abs(firstRectTileY - secondRectTileY)
          rectHeight += 1
          rectWidth += 1

          DrawLine(firstRectTileX * 16 + worldOffsetX, firstRectTileY * 16 + worldOffsetY, firstRectTileX * 16 + 16 * rectWidth + worldOffsetX, firstRectTileY * 16 + worldOffsetY, PIXEL_SOLID, BG_BLACK Or FG_GREY)
          DrawLine(firstRectTileX * 16 + worldOffsetX, firstRectTileY * 16 + worldOffsetY, firstRectTileX * 16 + worldOffsetX, firstRectTileY * 16 + 16 * rectHeight + worldOffsetY, PIXEL_SOLID, BG_BLACK Or FG_GREY)
          DrawLine(firstRectTileX * 16 + 16 * rectWidth + worldOffsetX, firstRectTileY * 16 + worldOffsetY, firstRectTileX * 16 + 16 * rectWidth + worldOffsetX, firstRectTileY * 16 + 16 * rectHeight + worldOffsetY, PIXEL_SOLID, BG_BLACK Or FG_GREY)
          DrawLine(firstRectTileX * 16 + worldOffsetX, firstRectTileY * 16 + 16 * rectHeight + worldOffsetY, firstRectTileX * 16 + 16 * rectWidth + worldOffsetX, firstRectTileY * 16 + 16 * rectHeight + worldOffsetY, PIXEL_SOLID, BG_BLACK Or FG_GREY)
        Else
          DrawLine(tileX * 16 + worldOffsetX, tileY * 16 + worldOffsetY, tileX * 16 + 16 + worldOffsetX, tileY * 16 + worldOffsetY, PIXEL_SOLID, BG_BLACK Or FG_GREY)
          DrawLine(tileX * 16 + worldOffsetX, tileY * 16 + worldOffsetY, tileX * 16 + worldOffsetX, tileY * 16 + 16 + worldOffsetY, PIXEL_SOLID, BG_BLACK Or FG_GREY)
          DrawLine(tileX * 16 + 16 + worldOffsetX, tileY * 16 + worldOffsetY, tileX * 16 + 16 + worldOffsetX, tileY * 16 + 16 + worldOffsetY, PIXEL_SOLID, BG_BLACK Or FG_GREY)
          DrawLine(tileX * 16 + worldOffsetX, tileY * 16 + 16 + worldOffsetY, tileX * 16 + 16 + worldOffsetX, tileY * 16 + 16 + worldOffsetY, PIXEL_SOLID, BG_BLACK Or Colour.FG_GREY)
        End If

        If GetKey(VK_CONTROL).Held AndAlso GetMouse(0).Pressed Then
          If pickedFirst Then
            If pickedSecond Then
              pickedSecond = False
              startRec = (tileX, tileY)
            Else
              pickedSecond = True
              endRex = (tileX, tileY)
            End If
          Else
            pickedFirst = True
            startRec = (tileX, tileY)
          End If
        End If

        If pickedFirst AndAlso Not pickedSecond AndAlso Not GetKey(VK_CONTROL).Held Then
          pickedFirst = False
        End If

      ElseIf pickedFirst AndAlso pickedSecond Then

        Dim firstRectTileX = startRec.Item1
        Dim firstRectTileY = startRec.Item2
        Dim secondRectTileX = endRex.Item1
        Dim secondRectTileY = endRex.Item2
        Dim rectWidth = Math.Abs(firstRectTileX - secondRectTileX)
        Dim rectHeight = Math.Abs(firstRectTileY - secondRectTileY)
        rectHeight += 1
        rectWidth += 1
        DrawLine(firstRectTileX * 16 + worldOffsetX, firstRectTileY * 16 + worldOffsetY, firstRectTileX * 16 + 16 * rectWidth + worldOffsetX, firstRectTileY * 16 + worldOffsetY, PIXEL_SOLID, BG_BLACK Or FG_GREY)
        DrawLine(firstRectTileX * 16 + worldOffsetX, firstRectTileY * 16 + worldOffsetY, firstRectTileX * 16 + worldOffsetX, firstRectTileY * 16 + 16 * rectHeight + worldOffsetY, PIXEL_SOLID, BG_BLACK Or FG_GREY)
        DrawLine(firstRectTileX * 16 + 16 * rectWidth + worldOffsetX, firstRectTileY * 16 + worldOffsetY, firstRectTileX * 16 + 16 * rectWidth + worldOffsetX, firstRectTileY * 16 + 16 * rectHeight + worldOffsetY, PIXEL_SOLID, BG_BLACK Or FG_GREY)
        DrawLine(firstRectTileX * 16 + worldOffsetX, firstRectTileY * 16 + 16 * rectHeight + worldOffsetY, firstRectTileX * 16 + 16 * rectWidth + worldOffsetX, firstRectTileY * 16 + 16 * rectHeight + worldOffsetY, PIXEL_SOLID, BG_BLACK Or FG_GREY)

      End If

      Dim iconOffset As Integer = 0
      If floodMode OrElse m_keys(VK_SHIFT).Held Then
        DrawSprite(2, 190, fillIcon)
        iconOffset += 10
      End If

#Region "Controls"

      ' world movement
#If SMOOTH_WORLD_MOVEMENT Then
      If GetKey(CChar("W")).bHeld Then
        moved = True
        worldOffsetY += 32 * fElapsedTime * If(GetKey(VK_SHIFT).bHeld, 2, 1)
      End If
      If GetKey(CChar("S")).bHeld Then
        moved = True
        worldOffsetY -= 32 * fElapsedTime * If(GetKey(VK_SHIFT).bHeld, 2, 1)
      End If
      If GetKey(CChar("A")).bHeld Then
        moved = True
        worldOffsetX += 32 * fElapsedTime * If(GetKey(VK_SHIFT).bHeld, 2, 1)
      End If
      If GetKey(CChar("D")).bHeld Then
        moved = True
        worldOffsetX -= 32 * fElapsedTime * If(GetKey(VK_SHIFT).bHeld, 2, 1)
      End If
#Else
      If GetKey(CChar("W")).Pressed OrElse (GetKey(VK_SHIFT).Held AndAlso GetKey(CChar("W")).Held) Then
        moved = True
        worldOffsetY += 16
      End If
      If (Not GetKey(0).Held AndAlso GetKey(CChar("S")).Pressed) OrElse (GetKey(VK_SHIFT).Held AndAlso GetKey(CChar("S")).Held) Then
        moved = True
        worldOffsetY -= 16
      End If
      If GetKey(CChar("A")).Pressed OrElse (GetKey(VK_SHIFT).Held AndAlso GetKey(CChar("A")).Held) Then
        moved = True
        worldOffsetX += 16
      End If
      If GetKey(CChar("D")).Pressed OrElse (GetKey(VK_SHIFT).Held AndAlso GetKey(CChar("D")).Held) Then
        moved = True
        worldOffsetX -= 16
      End If
#End If

      If (GetKey(VK_ESCAPE).Pressed) Then
        pickedFirst = False
        pickedSecond = False
      End If

      If (m_keys(VK_CONTROL).Held And m_keys(AscW("S"c)).Pressed) Then
        If (file.Length = 0) Then
          SaveLevel()
        Else
          level.Save(file)
        End If
      End If

      If (m_keys(VK_CONTROL).Held And m_keys(AscW("L"c)).Pressed) Then
        LoadLevel()
      End If

      If (m_keys(AscW("T"c)).Pressed) Then
        tool = CType(CType(tool, Integer) + 1, Tool)
        If (tool = Tool.LAST) Then
          tool = Tool.TILES
        End If
      End If

      If (m_keys(AscW("G"c)).Pressed) Then
        grid = Not grid
      End If

      If (m_keys(AscW("F"c)).Pressed) Then
        floodMode = Not floodMode
      End If

      If (m_keys(VK_LEFT).Pressed) Then
        If (pageCount <> 0) Then
          page -= 1
          If (page < 0) Then
            page = 0
          End If
        End If
      End If

      If (m_keys(VK_RIGHT).Pressed) Then
        If (pageCount <> 0) Then
          page += 1
          If (page >= pageCount) Then
            page = pageCount - 1
          End If
        End If
      End If

#End Region

    End If

    Return True

  End Function

#Region "Meta Tool"

  Private Enum MetaTools
    SOLID_BRUSH
  End Enum

  Dim selectedMetaTool As MetaTools

  Sub metaTool(tileX As Integer, tileY As Integer)

    Dim title As String = "TILE META"
    DrawStringFont(uiBase + 5, 5, title)

    Dim solidBrushText As String = ""
    If selectedMetaTool = MetaTools.SOLID_BRUSH Then
      solidBrushText &= " * "
    End If
    solidBrushText &= "SOLID"
    DrawStringFont(uiBase + 10, 18, solidBrushText)
    Fill(uiBase + 7, 19, uiBase + 7 + 5, 19 + 5, PIXEL_SOLID, BG_BLACK Or FG_RED)

    ' are we in the world editor
    If tileX >= 0 AndAlso tileY >= 0 AndAlso tileX < level.GetWidth() AndAlso tileY < level.GetHeight() AndAlso Not popup.menuActive Then
      Select Case selectedMetaTool
        Case MetaTools.SOLID_BRUSH
          ' change the tile
          If m_mouse(0).Held Then
            If floodMode OrElse m_keys(VK_SHIFT).Held Then
              FloorFillSolid(tileX, tileY, True)
            Else
              level(tileX + tileY * level.GetWidth()).SetSolid(True)
            End If
          ElseIf m_mouse(1).Held Then
            If floodMode OrElse m_keys(VK_SHIFT).Held Then
              FloorFillSolid(tileX, tileY, False)
            Else
              level(tileX + tileY * level.GetWidth()).SetSolid(False)
            End If
          End If
      End Select
    End If
  End Sub

#End Region

#Region "Import Export"

  Sub exportAndImportTool()
    Dim title As String = "NEW"
    DrawStringFont(uiBase + 6, 5, title)
    DrawStringFont(uiBase + 1, 25, "IMPORT:")
    DrawStringFont(uiBase + 6, 35, "LEVEL")
    DrawStringFont(uiBase + 6, 45, "SPRITESHEET")
    DrawStringFont(uiBase + 1, 75, "EXPORT:")
    DrawStringFont(uiBase + 6, 85, "LEVEL")
    DrawStringFont(uiBase + 6, 95, "SPRITE")
    If popup.menuActive Then Return
    If m_mouse(0).Pressed Then
      ' new
      If m_mousePosX > uiBase + 6 AndAlso m_mousePosX < 400 AndAlso m_mousePosY > 5 AndAlso m_mousePosY < 5 + 8 Then
        popup.menuActive = True
        popup.popup = Constants.Popup.NEW_MAP_SIZE
      End If
      ' import level
      If m_mousePosX > uiBase + 6 AndAlso m_mousePosX < 400 AndAlso m_mousePosY > 25 AndAlso m_mousePosY < 25 + 8 Then
        LoadLevel()
      End If
      ' import spritesheet
      If m_mousePosX > uiBase + 6 AndAlso m_mousePosX < 400 AndAlso m_mousePosY > 35 AndAlso m_mousePosY < 35 + 8 Then
        ImportSpriteSheet()
      End If
      ' export level
      If m_mousePosX > uiBase + 6 AndAlso m_mousePosX < 400 AndAlso m_mousePosY > 75 AndAlso m_mousePosY < 75 + 8 Then
        SaveLevel()
      End If
      ' export level as sprite
      If m_mousePosX > uiBase + 6 AndAlso m_mousePosX < 400 AndAlso m_mousePosY > 85 AndAlso m_mousePosY < 85 + 8 Then
        ExportAsSprite()
      End If
    End If
  End Sub

#Region "Win32"

  '<DllImport("comdlg32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
  'Private Shared Function GetOpenFileName(<[In], Out> ofn As OpenFileName) As Boolean
  'End Function

  '<DllImport("comdlg32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
  'Private Shared Function GetSaveFileName(<[In], Out> ofn As OpenFileName) As Boolean
  'End Function

  Private Declare Function GetOpenFileName Lib "comdlg32.dll" Alias "GetOpenFileNameA" (ByRef lpofn As OPENFILENAME) As Boolean
  Private Declare Function GetSaveFileName Lib "comdlg32.dll" Alias "GetSaveFileNameA" (ByRef lpofn As OPENFILENAME) As Boolean

  '<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
  'Public Structure OpenFileName
  '  Public lStructSize As Integer '= 0
  '  Public hwndOwner As IntPtr '= IntPtr.Zero
  '  Public hInstance As IntPtr '= IntPtr.Zero
  '  Public lpstrFilter As String '= Nothing
  '  Public lpstrCustomFilter As String '= Nothing
  '  Public nMaxCustFilter As Integer '= 0
  '  Public nFilterIndex As Integer ' = 0
  '  Public lpstrFile As String ' = Nothing
  '  Public nMaxFile As Integer '= 0
  '  Public lpstrFileTitle As String '= Nothing
  '  Public nMaxFileTitle As Integer '= 0
  '  Public lpstrInitialDir As String '= Nothing
  '  Public lpstrTitle As String '= Nothing
  '  Public flags As OpenFileNameFlags '= 0
  '  Public nFileOffset As Short '= 0
  '  Public nFileExtension As Short '= 0
  '  Public lpstrDefExt As String '= Nothing
  '  Public lCustData As IntPtr '= IntPtr.Zero
  '  Public lpfnHook As IntPtr '= IntPtr.Zero
  '  Public lpTemplateName As String '= Nothing
  '  Public pvReserved As IntPtr '= IntPtr.Zero
  '  Public dwReserved As Integer '= 0
  '  Public flagsEx As Integer '= 0
  'End Structure

  <StructLayout(LayoutKind.Sequential)> ', CharSet:=CharSet.Auto)>
  Private Structure OPENFILENAME
    Public lStructSize As Integer
    Public hwndOwner As IntPtr
    Public hInstance As IntPtr
    Public lpstrFilter As String
    Public lpstrCustomFilter As String
    Public nMaxCustFilter As Integer
    Public nFilterIndex As Integer
    Public lpstrFile As String
    Public nMaxFile As Integer
    Public lpstrFileTitle As String
    Public nMaxFileTitle As Integer
    Public lpstrInitialDir As String
    Public lpstrTitle As String
    Public flags As Integer
    Public nFileOffset As Short
    Public nFileExtension As Short
    Public lpstrDefExt As String
    Public lCustData As IntPtr
    Public lpfnHook As IntPtr
    Public lpTemplateName As String
  End Structure

  <Flags>
  Public Enum OpenFileNameFlags As Integer
    OFN_FILEMUSTEXIST = &H1000
  End Enum

#End Region

  Private Sub ImportSpriteSheet()

    Dim ofn As New OPENFILENAME
    ofn.lStructSize = Marshal.SizeOf(GetType(OPENFILENAME))
    ofn.lpstrFilter = "olcSprite (*.spr)" & Chr(0) & "*.spr" & Chr(0) & "Any File" & Chr(0) & "*.*" & Chr(0)
    ofn.lpstrFile = New String(Chr(0), MAX_PATH)
    ofn.nMaxFile = ofn.lpstrFile.Length - 1
    ofn.lpstrTitle = "Import Sprite Sheet"

    'ofn.hwndOwner = Nothing
    ofn.flags = OpenFileNameFlags.OFN_FILEMUSTEXIST
    If GetOpenFileName(ofn) Then
      spriteSheetFile = ofn.lpstrFile
      level.LoadSpriteSheet(spriteSheetFile, 16)
      tiles = level.GetSpriteSheet()
    End If

    'Dim openFileDialog1 As New OpenFileDialog()
    'openFileDialog1.Filter = "olcSprite (*.spr)|*.spr|All Files (*.*)|*.*"
    'openFileDialog1.FilterIndex = 0
    'openFileDialog1.Multiselect = False

    'If openFileDialog1.ShowDialog() = DialogResult.OK Then
    '  Dim selectedFilePath As String = openFileDialog1.FileName
    '  ' Process the selected file path here

    '  spriteSheetFile = selectedFilePath
    '  level.LoadSpriteSheet(selectedFilePath, 16)
    '  tiles = level.GetSpriteSheet()

    'End If

  End Sub

  Private Sub LoadLevel()
    'Dim filename(MAX_PATH) As Char
    Dim ofn As New OPENFILENAME
    'Array.Clear(filename, 0, filename.Length)
    ofn.lStructSize = Marshal.SizeOf(GetType(OPENFILENAME))
    'ofn.hwndOwner = Nothing
    ofn.lpstrFile = New String(Chr(0), MAX_PATH) 'filename
    ofn.nMaxFile = ofn.lpstrFile.Length - 1 'MAX_PATH
    ofn.flags = OpenFileNameFlags.OFN_FILEMUSTEXIST
    ofn.lpstrFilter = "Level File (*.lvl)" & Chr(0) & "*.lvl" & Chr(0) & "Any File" & Chr(0) & "*.*" & Chr(0)
    ofn.lpstrTitle = "Load Level"
    If GetOpenFileName(ofn) Then
      level.Load(ofn.lpstrFile)
    End If

    'Dim openFileDialog1 As New OpenFileDialog()
    'openFileDialog1.Filter = "Level File (*.lvl)|*.lvl|All Files (*.*)|*.*"
    'openFileDialog1.FilterIndex = 0
    'openFileDialog1.Multiselect = False

    'If openFileDialog1.ShowDialog() = DialogResult.OK Then
    '  Dim selectedFilePath As String = openFileDialog1.FileName
    '   Process the selected file path here

    '  level.Load(selectedFilePath)

    'End If

  End Sub

  Private Sub SaveLevel()
    'Dim filename(MAX_PATH) As Char
    Dim ofn As New OPENFILENAME
    'Array.Clear(filename, 0, filename.Length)
    ofn.lStructSize = Marshal.SizeOf(GetType(OpenFileName))
    'ofn.hwndOwner = Nothing
    ofn.lpstrFile = New String(Chr(0), MAX_PATH) 'filename
    ofn.nMaxFile = ofn.lpstrFile.Length - 1 'MAX_PATH
    ofn.lpstrFilter = "Level File (.lvl)" & vbNullChar & ".lvl" & vbNullChar & "Any File" & vbNullChar & "*.*" & vbNullChar
    ofn.lpstrTitle = "Save Level"
    If GetSaveFileName(ofn) Then
      Dim f As String = ofn.lpstrFile 'filename
      If Not f.EndsWith(".lvl") Then
        f &= ".lvl"
      End If
      level.Save(f)
      file = f
    End If
  End Sub

  Private Sub ExportAsSprite()
    'Dim filename(MAX_PATH) As Char
    Dim ofn As New OpenFileName()
    'Array.Clear(filename, 0, filename.Length)
    ofn.lStructSize = Marshal.SizeOf(GetType(OpenFileName))
    'ofn.hwndOwner = Nothing
    ofn.lpstrFile = New String(Chr(0), MAX_PATH) 'filename
    ofn.nMaxFile = ofn.lpstrFile.Length - 1 'MAX_PATH
    ofn.lpstrFilter = "olcSprite (*.spr)" & vbNullChar & "*.spr" & vbNullChar & "Any File" & vbNullChar & "*.*" & vbNullChar
    ofn.lpstrTitle = "Export Level As Sprite"
    ofn.lpstrDefExt = "spr"
    If GetSaveFileName(ofn) Then
      Dim f As String = ofn.lpstrFile 'filename
      If Not f.EndsWith(".spr") Then
        f &= ".spr"
      End If
      Dim exportedSprite As New Sprite(level.GetWidth() * TILE_WIDTH, level.GetHeight() * TILE_WIDTH)
      For y = 0 To level.GetHeight() - 1
        For x = 0 To level.GetWidth() - 1
          Dim sprite = level(x + y * level.GetWidth()).GetSprite()
          For sy = 0 To sprite.Height - 1
            For sx = 0 To sprite.Width - 1
              exportedSprite.SetColour(x * TILE_WIDTH + sx, y * TILE_WIDTH + sy, sprite.GetColour(sx, sy))
              exportedSprite.SetGlyph(x * TILE_WIDTH + sx, y * TILE_WIDTH + sy, sprite.GetGlyph(sx, sy))
            Next
          Next
        Next
      Next
      exportedSprite.Save(f)
    End If
  End Sub

#End Region

#Region "Tiles Tool"

  Private Sub TilesTool(tileX As Integer, tileY As Integer)

    ' draw page
    Dim pageText As New StringBuilder("TILES:")
    If pageCount = 0 Then
      pageText.Append(tiles.GetTileCount())
    Else
      pageText.Append((page + 1))
      pageText.Append("/"c)
      pageText.Append(pageCount)
    End If
    DrawStringFont(uiBase + 5, 5, pageText.ToString())

    ' draw sprites in menu
    Dim drawn = 0
    Dim toDraw = Math.Min(tilesPerPage, tiles.GetTileCount() - tilesPerPage * page)
    For row = 0 To tilesPerColumn - 1
      If drawn >= toDraw Then Exit For
      Dim y = 23 + row * tiles.GetTileHeight()
      For col = 0 To tilesPerRow - 1
        If drawn >= toDraw Then Exit For
        Dim x = uiBase + col * tiles.GetTileWidth()
        DrawSprite(x, y, tiles.Item((col + row * tilesPerRow) + tilesPerPage * page))
        drawn += 1
      Next
    Next

    ' draw selected sprite thing
    If selectedSprite >= tilesPerPage * page AndAlso selectedSprite < tilesPerPage * page + tilesPerPage Then
      Dim col = selectedSprite Mod tilesPerRow
      Dim row = CInt(Fix((selectedSprite - col) / tilesPerRow))
      row -= page * tilesPerColumn
      Dim y = 23 + row * tiles.GetTileHeight()
      Dim x = uiBase + col * tiles.GetTileWidth()
      DrawLine(x, y, x + 16, y, PIXEL_SOLID, BG_RED Or FG_RED)
      DrawLine(x, y, x, y + 16, PIXEL_SOLID, BG_RED Or FG_RED)
      DrawLine(x + 16, y, x + 16, y + 16, PIXEL_SOLID, BG_RED Or FG_RED)
      DrawLine(x, y + 16, x + 16, y + 16, PIXEL_SOLID, BG_RED Or FG_RED)
    End If

    ' are we in the selection menu
    If GetMouseX() >= uiBase AndAlso GetMouseX() < uiBase + tilesPerRow * tiles.GetTileWidth() AndAlso GetMouseY() > 28 AndAlso Not popup.menuActive Then
      Dim menuX = GetMouseX() - uiBase
      Dim menuY = GetMouseY() - 28
      Dim col = menuX \ tiles.GetTileWidth
      Dim row = menuY \ tiles.GetTileHeight()
      Dim index = (col + row * tilesPerRow) + page * tilesPerPage
      Dim y = 23 + row * tiles.GetTileHeight()
      Dim x = uiBase + col * tiles.GetTileWidth()
      DrawLine(x, y, x + 16, y, PIXEL_SOLID, BG_RED Or FG_DARK_RED)
      DrawLine(x, y, x, y + 16, PIXEL_SOLID, BG_RED Or FG_DARK_RED)
      DrawLine(x + 16, y, x + 16, y + 16, PIXEL_SOLID, BG_RED Or FG_DARK_RED)
      DrawLine(x, y + 16, x + 16, y + 16, PIXEL_SOLID, BG_RED Or Colour.FG_DARK_RED)
      If m_mouse(0).Pressed Then
        selectedSprite = index
      End If
    End If

    ' are we in the world editor
    If tileX >= 0 AndAlso tileY >= 0 AndAlso tileX < level.GetWidth() AndAlso tileY < level.GetHeight() AndAlso Not popup.menuActive AndAlso GetMouseX() <= 300 Then
      ' do we have a selection rect, if so are we in it?
      If (Not pickedFirst AndAlso Not pickedSecond) OrElse (pickedFirst AndAlso pickedSecond AndAlso tileX >= startRec.Item1 AndAlso tileX < startRec.Item1 + endRex.Item1 - 1 AndAlso tileY >= startRec.Item2 AndAlso tileY < startRec.Item2 + endRex.Item2) Then
        ' change the tile
        If m_mouse(0).Held Then
          If floodMode OrElse m_keys(VK_SHIFT).Held Then
            FloodFillTile(tileX, tileY)
          Else
            level(tileX + tileY * level.GetWidth()).SetSpriteId(selectedSprite)
          End If
        ElseIf m_mouse(1).Pressed Then
          selectedSprite = level(tileX + tileY * level.GetWidth()).GetSpriteId()
        End If
      End If
      If GetKey(VK_BACK).Pressed AndAlso pickedFirst AndAlso pickedSecond Then
        For y = startRec.Item2 To endRex.Item2 + startRec.Item2 - 1
          For x = startRec.Item1 To endRex.Item1 + startRec.Item1 - 2
            Dim index = x + y * level.GetWidth()
            level(index).SetSpriteId(selectedSprite)
          Next
        Next
      End If
    End If

  End Sub

#End Region

#Region "Utils"

  Function EndsWith(value As String, ending As String) As Boolean
    If ending.Length > value.Length Then Return False
    Return Enumerable.SequenceEqual(ending.Reverse(), value.Reverse().Take(ending.Length))
  End Function

  Dim fillTileOfType As Integer = DEFAULT_TILE
  Dim solidStart As Boolean = False

  Sub FloorFillSolid(x As Integer, y As Integer, fill As Boolean)
    fillTileOfType = level(x + y * level.GetWidth()).GetSpriteId()
    solidStart = fill
    Dim q As New Queue(Of (Integer, Integer))
    q.Enqueue((x, y))
    While q.Count <> 0
      Dim xy = q.Dequeue()
      level(xy.Item1 + xy.Item2 * level.GetWidth()).SetSolid(fill)
      If ShouldFillSolid(xy.Item1 + 1, xy.Item2) Then q.Enqueue((xy.Item1 + 1, xy.Item2))
      If ShouldFillSolid(xy.Item1 - 1, xy.Item2) Then q.Enqueue((xy.Item1 - 1, xy.Item2))
      If ShouldFillSolid(xy.Item1, xy.Item2 + 1) Then q.Enqueue((xy.Item1, xy.Item2 + 1))
      If ShouldFillSolid(xy.Item1, xy.Item2 - 1) Then q.Enqueue((xy.Item1, xy.Item2 - 1))
    End While
  End Sub

  Function ShouldFillSolid(x As Integer, y As Integer) As Boolean
    If x < 0 OrElse y < 0 OrElse x >= level.GetWidth() OrElse y >= level.GetHeight() Then Return False
    Return level(x + y * level.GetWidth()).GetSpriteId() = fillTileOfType AndAlso level(x + y * level.GetWidth()).IsSolid() <> solidStart
  End Function

  Private Sub FloodFillTile(x As Integer, y As Integer)

    fillTileOfType = level(x + y * level.GetWidth()).GetSpriteId()
    If fillTileOfType = selectedSprite Then
      Return
    End If

    Dim q As New Queue(Of (Integer, Integer))()
    q.Enqueue((x, y))

    While q.Count > 0
      Dim xy = q.Dequeue()
      level(xy.Item1 + xy.Item2 * level.GetWidth()).SetSpriteId(selectedSprite)
      If ShouldFillTile(xy.Item1 + 1, xy.Item2) Then
        q.Enqueue((xy.Item1 + 1, xy.Item2))
      End If
      If ShouldFillTile(xy.Item1 - 1, xy.Item2) Then
        q.Enqueue((xy.Item1 - 1, xy.Item2))
      End If
      If ShouldFillTile(xy.Item1, xy.Item2 + 1) Then
        q.Enqueue((xy.Item1, xy.Item2 + 1))
      End If
      If ShouldFillTile(xy.Item1, xy.Item2 - 1) Then
        q.Enqueue((xy.Item1, xy.Item2 - 1))
      End If
    End While

  End Sub

  Private Function ShouldFillTile(x As Integer, y As Integer) As Boolean
    If x < 0 OrElse y < 0 OrElse x >= level.GetWidth() OrElse y >= level.GetHeight() Then
      Return False
    End If
    Return level(x + y * level.GetWidth()).GetSpriteId() = fillTileOfType
  End Function

#End Region

End Class
