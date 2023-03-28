' Inspired by "Programming Panning & Zooming" -- @javidx9
' https://youtu.be/ZQ8qtAizis4

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine

Module Program

  Sub Main() 'args As String())
    Dim game As New PanAndZoom
    game.ConstructConsole(160, 100, 8, 8)
    game.Start()
  End Sub

End Module

Class PanAndZoom
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_offsetX As Single = 0.0F
  Private m_offsetY As Single = 0.0F
  Private m_scaleX As Single = 1.0F
  Private m_scaleY As Single = 1.0F

  Private m_startPanX As Single = 0.0F
  Private m_startPanY As Single = 0.0F

  Private m_selectedCellX As Single = 0.0F
  Private m_selectedCellY As Single = 0.0F

  Public Sub New()
    m_sAppName = "Pan And Zoom"
  End Sub

  Public Overrides Function OnUserCreate() As Boolean

    m_offsetX = -ScreenWidth() / 2.0F
    m_offsetY = -ScreenHeight() / 2.0F
    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Just grab a copy of mouse coordinates for convenience
    Dim mouseX = CSng(GetMouseX())
    Dim mouseY = CSng(GetMouseY())

    ' For panning, we need to capture the screen location when the user starts to pan...
    If GetMouse(2).Pressed Then
      m_startPanX = mouseX
      m_startPanY = mouseY
    End If

    ' ...as the mouse moves, the screen location changes. Convert this screen
    ' coordinate change into world coordinates to implement the pan. Simples.
    If GetMouse(2).Held Then
      m_offsetX -= (mouseX - m_startPanX) / m_scaleX
      m_offsetY -= (mouseY - m_startPanY) / m_scaleY
      m_startPanX = mouseX
      m_startPanY = mouseY
    End If

    ' For zoom, we need to extract the location of the cursor before and after the
    ' scale is changed. Here we get the cursor and translate into world space...
    Dim mouseWorldX_BeforeZoom, mouseWorldY_BeforeZoom As Single
    ScreenToWorld(GetMouseX(), GetMouseY(), mouseWorldX_BeforeZoom, mouseWorldY_BeforeZoom)

    ' ...change the scale as required...
    If GetKey(AscW("Q"c)).Held Then
      m_scaleX *= 1.001F
      m_scaleY *= 1.001F
    End If

    If GetKey(AscW("A"c)).Held Then
      m_scaleX *= 0.999F
      m_scaleY *= 0.999F
    End If

    ' ...now get the location of the cursor in world space again - It will have changed
    ' because the scale has changed, but we can offset our world now to fix the zoom
    ' location in screen space, because we know how much it changed laterally between
    ' the two spatial scales. Neat huh? ;-)
    Dim mouseWorldX_AfterZoom, mouseWorldY_AfterZoom As Single
    ScreenToWorld(GetMouseX(), GetMouseY(), mouseWorldX_AfterZoom, mouseWorldY_AfterZoom)
    m_offsetX += (mouseWorldX_BeforeZoom - mouseWorldX_AfterZoom)
    m_offsetY += (mouseWorldY_BeforeZoom - mouseWorldY_AfterZoom)

    ' Clear Screen
    Fill(0, 0, ScreenWidth(), ScreenHeight(), PIXEL_SOLID, FG_BLACK)

    ' Clip
    Dim worldLeft, worldTop, worldRight, worldBottom As Single
    ScreenToWorld(0, 0, worldLeft, worldTop)
    ScreenToWorld(ScreenWidth(), ScreenHeight(), worldRight, worldBottom)

    Dim func = Function(x As Single) As Single
                 Return CSng(Math.Sin(x))
               End Function

    ' Draw Main Axes a 10x10 Unit Grid
    ' Draw 10 horizontal lines
    Dim linesDrawn = 0
    For y = 0.0F To 10.0F
      If y >= worldTop AndAlso y <= worldBottom Then

        Dim sx = 0.0F, sy = y
        Dim ex = 10.0F, ey = y
        Dim pixel_sx, pixel_sy, pixel_ex, pixel_ey As Integer

        WorldToScreen(sx, sy, pixel_sx, pixel_sy)
        WorldToScreen(ex, ey, pixel_ex, pixel_ey)

        DrawLine(pixel_sx, pixel_sy, pixel_ex, pixel_ey, PIXEL_SOLID, FG_WHITE)
        linesDrawn += 1

      End If
    Next

    ' Draw 10 vertical lines
    For x = 0.0F To 10.0F
      If x >= worldLeft AndAlso x <= worldRight Then

        Dim sx = x, sy = 0.0F
        Dim ex = x, ey = 10.0F
        Dim pixel_sx, pixel_sy, pixel_ex, pixel_ey As Integer

        WorldToScreen(sx, sy, pixel_sx, pixel_sy)
        WorldToScreen(ex, ey, pixel_ex, pixel_ey)

        DrawLine(pixel_sx, pixel_sy, pixel_ex, pixel_ey, PIXEL_SOLID, FG_WHITE)
        linesDrawn += 1

      End If
    Next

    ' Draw selected cell
    ' We can easily determine where the mouse is in world space. In fact we already
    ' have this frame so just reuse the values
    If GetMouse(1).Released Then
      m_selectedCellX = CInt(mouseWorldX_AfterZoom)
      m_selectedCellY = CInt(mouseWorldY_AfterZoom)
    End If

    ' Draw selected cell by filling with red circle. Convert cell coords
    ' into screen space, also scale the radius
    Dim cx, cy, cr As Integer
    WorldToScreen(m_selectedCellX + 0.5F, m_selectedCellY + 0.5F, cx, cy)
    cr = CInt(Fix(0.3F * m_scaleX))
    FillCircle(cx, cy, cr, PIXEL_SOLID, FG_RED)
    DrawString(2, 2, "Lines Drawn: " + linesDrawn.ToString())

    ' Draw Chart
    Dim worldPerScreenWidthPixel = (worldRight - worldLeft) / ScreenWidth()
    Dim worldPerScreenHeightPixel = (worldBottom - worldTop) / ScreenHeight()
    Dim px, py, opx, opy As Integer
    WorldToScreen(worldLeft - worldPerScreenWidthPixel, -CSng(Math.Sin((worldLeft - worldPerScreenWidthPixel) - 5.0F)) + 5.0F, opx, opy)
    For x = worldLeft To worldRight Step worldPerScreenWidthPixel
      Dim y = -CSng(Math.Sin(x - 5.0F)) + 5.0F
      WorldToScreen(x, y, px, py)
      DrawLine(opx, opy, px, py, PIXEL_SOLID, FG_GREEN)
      opx = px
      opy = py
    Next x

    Return True

  End Function

  Private Sub WorldToScreen(worldX As Single, worldY As Single, ByRef screenX As Integer, ByRef screenY As Integer)
    screenX = CInt((worldX - m_offsetX) * m_scaleX)
    screenY = CInt((worldY - m_offsetY) * m_scaleY)
  End Sub

  Private Sub ScreenToWorld(screenX As Integer, screenY As Integer, ByRef worldX As Single, ByRef worldY As Single)
    worldX = screenX / m_scaleX + m_offsetX
    worldY = screenY / m_scaleY + m_offsetY
  End Sub

End Class