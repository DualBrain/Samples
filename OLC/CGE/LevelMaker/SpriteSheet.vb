Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine

Friend Class SpriteSheet

  Private m_spritesheet As Sprite
  Private m_sprites() As Sprite
  Private m_spriteCount As Integer
  Private m_tileWidth, m_tileHeight As Integer

  Friend Sub Load(file As String, tileWidth As Integer, Optional tileHeight As Integer = -1)

    m_tileHeight = tileHeight
    m_tileWidth = tileWidth

    m_spritesheet = New Sprite(file)

    Dim tileCountX = m_spritesheet.Width \ tileWidth
    Dim tileCountY = m_spritesheet.Height \ tileHeight
    m_spriteCount = tileCountX * tileCountY

    m_sprites = New Sprite(m_spriteCount - 1) {}

    For i = 0 To m_spriteCount - 1
      m_sprites(i) = New Sprite(tileWidth, tileHeight)
      Dim baseX = (i Mod tileCountX)
      Dim baseY = ((i - baseX) \ tileCountX)
      baseX *= tileWidth
      baseY *= tileHeight

      For y = 0 To tileHeight - 1
        For x = 0 To tileWidth - 1
          m_sprites(i).SetColour(x, y, m_spritesheet.GetColour(baseX + x, baseY + y))
          m_sprites(i).SetGlyph(x, y, m_spritesheet.GetGlyph(baseX + x, baseY + y))
        Next
      Next
    Next

    m_spritesheet = Nothing

  End Sub

  Public ReadOnly Property Item(index As Integer) As Sprite
    Get
      Return m_sprites(index)
    End Get
  End Property

  Friend Function GetTileWidth() As Integer
    Return m_tileWidth
  End Function

  Friend Function GetTileHeight() As Integer
    Return m_tileHeight
  End Function

  Friend Function GetTileCount() As Integer
    Return m_spriteCount
  End Function

End Class