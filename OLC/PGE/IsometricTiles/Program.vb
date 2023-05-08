' Inspired by "Coding Quickie: Isometric Tiles" -- @javidx9
' https://youtu.be/ukkbNKTgf5U

Option Explicit On
Option Strict On
Option Infer On

Imports System.Drawing
Imports Olc

Friend Module Program

  Sub Main()
    Dim demo As New IsometricDemo
    If demo.Construct(512, 480, 2, 2) Then
      demo.Start()
    End If
  End Sub

End Module

Friend Class IsometricDemo
  Inherits PixelGameEngine

  Friend Sub New()
    AppName = "Coding Quickie: Isometric Tiles"
  End Sub

  Private vWorldSize As New Vi2d(14, 10)
  Private vTileSize As New Vi2d(40, 20)
  Private vOrigin As New Vi2d(5, 1)
  Private sprIsom As Sprite = Nothing
  Private pWorld As Integer() = Nothing

  Protected Overrides Function OnUserCreate() As Boolean
    sprIsom = New Sprite("isometric_demo.png")
    pWorld = Enumerable.Repeat(0, vWorldSize.x * vWorldSize.y).ToArray()
    Return True
  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Clear(Presets.White)
    Dim vMouse As New Vi2d(GetMouseX(), GetMouseY())
    Dim vCell As New Vi2d(vMouse.x \ vTileSize.x, vMouse.y \ vTileSize.y)
    Dim vOffset As New Vi2d(vMouse.x Mod vTileSize.x, vMouse.y Mod vTileSize.y)
    Dim col As Pixel = sprIsom.GetPixel(3 * vTileSize.x + vOffset.x, vOffset.y)

    Dim vSelected As New Vi2d((vCell.y - vOrigin.y) + (vCell.x - vOrigin.x),
                              (vCell.y - vOrigin.y) - (vCell.x - vOrigin.x))

    If col = Presets.Red Then vSelected += New Olc.Vi2d(-1, 0)
    If col = Presets.Blue Then vSelected += New Olc.Vi2d(0, -1)
    If col = Presets.Green Then vSelected += New Olc.Vi2d(0, 1)
    If col = Presets.Yellow Then vSelected += New Olc.Vi2d(1, 0)

    If GetMouse(0).Pressed Then
      If vSelected.x >= 0 AndAlso vSelected.x < vWorldSize.x AndAlso
         vSelected.y >= 0 AndAlso vSelected.y < vWorldSize.y Then
        pWorld(vSelected.y * vWorldSize.x + vSelected.x) += 1
        pWorld(vSelected.y * vWorldSize.x + vSelected.x) = pWorld(vSelected.y * vWorldSize.x + vSelected.x) Mod 6
      End If
    End If

    Dim ToScreen = Function(x As Integer, y As Integer) As Vi2d
                     Return New Vi2d(CInt(Fix(vOrigin.x * vTileSize.x + (x - y) * (vTileSize.x / 2))),
                                     CInt(Fix(vOrigin.y * vTileSize.y + (x + y) * (vTileSize.y / 2))))
                   End Function

    SetPixelMode(Pixel.Mode.Mask)

    ' (0,0) is at top, defined by vOrigin, so draw from top to bottom
    ' to ensure tiles closest to camera are drawn last
    For y = 0 To vWorldSize.y - 1
      For x = 0 To vWorldSize.x - 1

        ' Convert cell coordinate to world space
        Dim vWorld = ToScreen(x, y)

        Select Case pWorld(y * vWorldSize.x + x)
          Case 0
            ' Invisble Tile
            DrawPartialSprite(vWorld.x, vWorld.y, sprIsom, 1 * vTileSize.x, 0, vTileSize.x, vTileSize.y)
          Case 1
            ' Visible Tile
            DrawPartialSprite(vWorld.x, vWorld.y, sprIsom, 2 * vTileSize.x, 0, vTileSize.x, vTileSize.y)
          Case 2
            ' Tree
            DrawPartialSprite(vWorld.x, vWorld.y - vTileSize.y, sprIsom, 0 * vTileSize.x, 1 * vTileSize.y, vTileSize.x, vTileSize.y * 2)
          Case 3
            ' Spooky Tree
            DrawPartialSprite(vWorld.x, vWorld.y - vTileSize.y, sprIsom, 1 * vTileSize.x, 1 * vTileSize.y, vTileSize.x, vTileSize.y * 2)
          Case 4
            ' Beach
            DrawPartialSprite(vWorld.x, vWorld.y - vTileSize.y, sprIsom, 2 * vTileSize.x, 1 * vTileSize.y, vTileSize.x, vTileSize.y * 2)
          Case 5
            ' Water
            DrawPartialSprite(vWorld.x, vWorld.y - vTileSize.y, sprIsom, 3 * vTileSize.x, 1 * vTileSize.y, vTileSize.x, vTileSize.y * 2)
        End Select
      Next
    Next

    ' Draw Selected Cell - Has varying alpha components
    SetPixelMode(Pixel.Mode.Alpha)

    ' Convert selected cell coordinate to world space
    Dim vSelectedWorld = ToScreen(vSelected.x, vSelected.y)

    ' Draw "highlight" tile
    DrawPartialSprite(vSelectedWorld.X, vSelectedWorld.Y, sprIsom, 0 * vTileSize.x, 0, vTileSize.x, vTileSize.y)

    ' Go back to normal drawing with no expected transparency
    SetPixelMode(Pixel.Mode.Normal)

    ' Draw Hovered Cell Boundary
    'DrawRect(vCell.x * vTileSize.x, vCell.y * vTileSize.y, vTileSize.x, vTileSize.y, Presets.Red)

    ' Draw Debug Info
    DrawString(4, 4, "Mouse   : " & vMouse.x.ToString() & ", " & vMouse.y.ToString(), Presets.Black)
    DrawString(4, 14, "Cell    : " & vCell.x.ToString() & ", " & vCell.y.ToString(), Presets.Black)
    DrawString(4, 24, "Selected: " & vSelected.x.ToString() & ", " & vSelected.y.ToString(), Presets.Black)

    Return True

  End Function

End Class