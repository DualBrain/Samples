Option Explicit On
Option Strict On
Option Infer On
Imports Olc

Friend Module Program

  Sub Main() 'args As String())
    Dim game As New SpriteTest
    If game.Construct(800, 600, 1, 1) Then
      game.Start()
    End If
  End Sub

End Module

Friend Class SpriteTest
  Inherits Olc.PixelGameEngine

  Friend Sub New()
    AppName = "Sprite Demo"
  End Sub

  Private ReadOnly m_spriteCount As Integer = 256 '1024
  Private m_sprite1 As Sprite
  Private m_sprite2 As Sprite
  Private ReadOnly m_locations As New List(Of Position)
  Private ReadOnly m_directions As New List(Of Position)
  Private m_background As Sprite
  Private ReadOnly m_rnd As New Random()

  Private Class Position
    Public X As Integer
    Public y As Integer
  End Class

  Protected Overrides Function OnUserCreate() As Boolean
    ' Load the sprites into memory...
    m_sprite1 = New Sprite("sprite1.png")
    m_sprite2 = New Sprite("sprite2.png")
    ' Initialize the location for all the active sprites...
    For entry = 0 To m_spriteCount - 1
      m_locations.Add(New Position With {.X = m_rnd.Next(0, 799), .y = m_rnd.Next(0, 599)})
      m_directions.Add(New Position With {.X = m_rnd.Next(0, 2), .y = m_rnd.Next(0, 2)})
    Next
    ' Create the background using the smaller sprite...
    m_background = New Sprite(800, 600)
    SetDrawTarget(m_background)
    For x = 0 To 800 Step 32
      For y = 0 To 600 Step 32
        DrawSprite(x, y, m_sprite1, 1)
      Next
    Next
    SetDrawTarget(Nothing)
    Return True
  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean
    ' Draw the background...
    DrawSprite(0, 0, m_background, 1)
    ' Update positions and determine direction for next pass.
    For entry = 0 To m_spriteCount - 1
      m_locations(entry).X += (m_directions(entry).X - 1)
      m_locations(entry).y += (m_directions(entry).y - 1)
      m_directions(entry) = New Position With {.X = m_rnd.Next(0, 3), .y = m_rnd.Next(0, 3)}
    Next
    ' Draw all the sprites...
    SetPixelMode(Pixel.Mode.Mask)
    For Each entry In m_locations
      DrawSprite(entry.X, entry.y, m_sprite2, 1)
    Next
    SetPixelMode(Pixel.Mode.Normal)
    Return True
  End Function

End Class
