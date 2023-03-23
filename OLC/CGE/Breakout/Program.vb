Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour

Module Program

  Sub Main() 'args As String())
    Dim game As New Breakout
    game.ConstructConsole(256, 240, 4, 4)
    game.Start()
  End Sub

End Module

Class Breakout
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_level() As Char
  Private m_width As Integer = 16
  Private m_height As Integer = 15
  Private m_block As Integer = 8

  Private m_bat As Single = 64.0F

  Private m_ballX As Single = 64.0F
  Private m_ballY As Single = 64.0F
  Private m_ballDx As Single
  Private m_ballDy As Single

  Public Overrides Function OnUserCreate() As Boolean

    Dim level = "################"
    level += "#..............#"
    level += "#...11111111...#"
    level += "#...11111111...#"
    level += "#...11....11...#"
    level += "#..............#"
    level += "#..............#"
    level += "#..............#"
    level += "#..............#"
    level += "#..............#"
    level += "#..............#"
    level += "#..............#"
    level += "#..............#"
    level += "#..............#"
    level += "#..............#"

    ReDim m_level((m_width * m_height) - 1)
    For index = 0 To level.Length - 1
      m_level(index) = level(index)
    Next

    Dim ang = CSng(Rand / RAND_MAX) * 3.14159F * 2.0F
    ang = 0.6F
    m_ballDx = CSng(Math.Cos(ang))
    m_ballDy = CSng(Math.Sin(ang))

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Dim batWidth = 10
    Dim speed = 40.0F

    Cls()

    If GetKey(VK_LEFT).Held Then m_bat -= 60.0F * elapsedTime
    If GetKey(VK_RIGHT).Held Then m_bat += 60.0F * elapsedTime

    If m_bat - batWidth < m_block Then m_bat = m_block + batWidth
    If m_bat + batWidth > (m_width - 1) * m_block Then m_bat = (m_width - 1) * m_block - batWidth

    Dim oldX = m_ballX
    Dim oldY = m_ballY
    m_ballX += m_ballDx * elapsedTime * speed
    m_ballY += m_ballDy * elapsedTime * speed

    Dim cellOldX = CInt(Fix(oldX / m_block))
    Dim cellOldY = CInt(Fix(oldY / m_block))

    Dim cellNewX = CInt(Fix(m_ballX / m_block))
    Dim cellNewY = CInt(Fix(m_ballY / m_block))

    Dim newCell = m_level(cellNewY * m_width + cellNewX)
    Dim oldCell = m_level(cellOldY * m_width + cellOldX)

    If newCell <> "."c Then

      If newCell = "1"c Then m_level(cellNewY * m_width + cellNewX) = "."c

      If cellNewX <> cellOldX Then m_ballDx *= -1
      If cellNewY <> cellOldY Then m_ballDy *= -1

    End If

    If m_ballY > m_height * m_block - 2 Then
      If m_ballX > m_bat - batWidth AndAlso m_ballX < m_bat + batWidth Then
        m_ballDy *= -1
      Else
        ' Dead
        m_ballX = (m_width / 2.0F) * m_block
        m_ballY = (m_height / 2.0F) * m_block
        Dim ang = CSng(Rand / RAND_MAX) * 3.14159F * 2.0F
        m_ballDx = CSng(Math.Cos(ang))
        m_ballDy = CSng(Math.Sin(ang))
      End If
    End If

    'If m_ballDx > 

    ' Draw Level
    For y = 0 To m_height - 1
      For x = 0 To m_width - 1
        Select Case m_level(y * m_width + x)
          Case "#"c
            Fill(x * m_block, y * m_block, (x + 1) * m_block, (y + 1) * m_block, PIXEL_SOLID, FG_WHITE)
          Case "1"c
            Fill(x * m_block, y * m_block, (x + 1) * m_block, (y + 1) * m_block, PIXEL_SOLID, FG_GREEN)
          Case "."c
            Fill(x * m_block, y * m_block, (x + 1) * m_block, (y + 1) * m_block, PIXEL_SOLID, FG_BLACK)
          Case Else
            Stop
        End Select
      Next
    Next

    FillCircle(m_ballX, m_ballY, 2.0F, PIXEL_SOLID, FG_YELLOW)

    DrawLine(m_bat - batWidth, m_height * m_block - 2, m_bat + batWidth, m_height * m_block - 2, PIXEL_SOLID, FG_WHITE)

    Return True

  End Function

End Class
