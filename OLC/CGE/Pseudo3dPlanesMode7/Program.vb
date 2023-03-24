' Inspired by "Programming Pseudo 3D Planes aka MODE7 (C++)" -- @javidx9
' https://youtu.be/ybLZyY655iY

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine

Module Program

  Sub Main() 'args As String())
    Dim game As New FakeMode7
    If game.ConstructConsole(320, 240, 4, 4) <> 0 Then
      game.Start()
    Else
      Stop
    End If
  End Sub

End Module

Class FakeMode7
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private fWorldX As Single = 1000.0F
  Private fWorldY As Single = 1000.0F
  Private fWorldA As Single = 0.1F
  Private fNear As Single = 0.005F
  Private fFar As Single = 0.03F
  Private fFoVHalf As Single = 3.14159F / 4.0F

  Private sprGround As Sprite
  Private sprSky As Sprite

  'Private nMapSize As Integer = 1024

  Public Sub New()
    m_sAppName = "Pseudo 3D Planes"
  End Sub

  Public Overrides Function OnUserCreate() As Boolean

    '' Create a large sprite and fill it with horizontal and vertical lines

    'sprGround = New Sprite(nMapSize, nMapSize)

    'For x = 0 To nMapSize Step 32
    '  For y = 0 To nMapSize - 1
    '    sprGround.SetColour(x, y, FG_MAGENTA)
    '    sprGround.SetGlyph(x, y, PIXEL_SOLID)

    '    sprGround.SetColour(x + 1, y, FG_MAGENTA)
    '    sprGround.SetGlyph(x + 1, y, PIXEL_SOLID)

    '    sprGround.SetColour(x - 1, y, FG_MAGENTA)
    '    sprGround.SetGlyph(x - 1, y, PIXEL_SOLID)

    '    sprGround.SetColour(y, x, FG_BLUE)
    '    sprGround.SetGlyph(y, x, PIXEL_SOLID)

    '    sprGround.SetColour(y, x + 1, FG_BLUE)
    '    sprGround.SetGlyph(y, x + 1, PIXEL_SOLID)

    '    sprGround.SetColour(y, x - 1, FG_BLUE)
    '    sprGround.SetGlyph(y, x - 1, PIXEL_SOLID)
    '  Next
    'Next

    ' Simply load very large sprites from file
    sprGround = New Sprite("mariokart.spr")
    sprSky = New Sprite("sky1.spr")

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Control rendering parameters dynamically
    If GetKey(AscW("Q"c)).Held Then fNear += 0.1F * elapsedTime
    If GetKey(AscW("A"c)).Held Then fNear -= 0.1F * elapsedTime

    If GetKey(AscW("W"c)).Held Then fFar += 0.1F * elapsedTime
    If GetKey(AscW("S"c)).Held Then fFar -= 0.1F * elapsedTime

    If GetKey(AscW("Z"c)).Held Then fFoVHalf += 0.1F * elapsedTime
    If GetKey(AscW("X"c)).Held Then fFoVHalf -= 0.1F * elapsedTime

    ' Create Frustum corner points
    Dim fFarX1 = fWorldX + (CSng(Math.Cos(fWorldA - fFoVHalf)) * fFar)
    Dim fFarY1 = fWorldY + (CSng(Math.Sin(fWorldA - fFoVHalf)) * fFar)

    Dim fNearX1 = fWorldX + (CSng(Math.Cos(fWorldA - fFoVHalf)) * fNear)
    Dim fNearY1 = fWorldY + (CSng(Math.Sin(fWorldA - fFoVHalf)) * fNear)

    Dim fFarX2 = fWorldX + (CSng(Math.Cos(fWorldA + fFoVHalf)) * fFar)
    Dim fFarY2 = fWorldY + (CSng(Math.Sin(fWorldA + fFoVHalf)) * fFar)

    Dim fNearX2 = fWorldX + (CSng(Math.Cos(fWorldA + fFoVHalf)) * fNear)
    Dim fNearY2 = fWorldY + (CSng(Math.Sin(fWorldA + fFoVHalf)) * fNear)

    ' Starting with furthest away line and work towards the camera point
    For y = 0 To ScreenHeight() \ 2 - 1
      ' Take a sample point for depth linearly related to rows down screen
      Dim fSampleDepth = y / (ScreenHeight() / 2.0F)

      ' Use sample point in non-linear (1/x) way to enable perspective
      ' and grab start and end points for lines across the screen
      Dim fStartX = (fFarX1 - fNearX1) / (fSampleDepth) + fNearX1
      Dim fStartY = (fFarY1 - fNearY1) / (fSampleDepth) + fNearY1
      Dim fEndX = (fFarX2 - fNearX2) / (fSampleDepth) + fNearX2
      Dim fEndY = (fFarY2 - fNearY2) / (fSampleDepth) + fNearY2

      ' Linearly interpolate lines across the screen
      For x = 0 To ScreenWidth() - 1
        Dim fSampleWidth = CSng(x / ScreenWidth())
        Dim fSampleX = ((fEndX - fStartX) * fSampleWidth) + fStartX
        Dim fSampleY = ((fEndY - fStartY) * fSampleWidth) + fStartY

        ' Wrap sample coordinates to give "infinite" periodicity on maps
        fSampleX = fSampleX Mod 1.0F
        fSampleY = fSampleY Mod 1.0F

        ' Sample symbol and colour from map sprite, and draw the
        ' pixel to the screen
        Dim sym = sprGround.SampleGlyph(fSampleX, fSampleY)
        Dim col = sprGround.SampleColour(fSampleX, fSampleY)
        Draw(x, y + (ScreenHeight() \ 2), sym, col)

        ' Sample symbol and colour from sky sprite, we can use same
        ' coordinates, but we need to draw the "inverted" y-location
        sym = sprSky.SampleGlyph(fSampleX, fSampleY)
        col = sprSky.SampleColour(fSampleX, fSampleY)
        Draw(x, (ScreenHeight() \ 2) - y, sym, col)
      Next

    Next

    'Draw a blanking line to fill the gap between sky And ground
    DrawLine(0, ScreenHeight() \ 2, ScreenWidth(), ScreenHeight() \ 2, PIXEL_SOLID, FG_CYAN)

    ' Handle user navigation with arrow keys
    If GetKey(VK_LEFT).Held Then
      fWorldA -= 1.0F * elapsedTime
    End If

    If GetKey(VK_RIGHT).Held Then
      fWorldA += 1.0F * elapsedTime
    End If

    If GetKey(VK_UP).Held Then
      fWorldX += CSng(Math.Cos(fWorldA)) * 0.2F * elapsedTime
      fWorldY += CSng(Math.Sin(fWorldA)) * 0.2F * elapsedTime
    End If

    If GetKey(VK_DOWN).Held Then
      fWorldX -= CSng(Math.Cos(fWorldA)) * 0.2F * elapsedTime
      fWorldY -= CSng(Math.Sin(fWorldA)) * 0.2F * elapsedTime
    End If

    Return True

  End Function

End Class