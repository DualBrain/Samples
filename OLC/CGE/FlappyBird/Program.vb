' Inspired by: "Code-It-Yourself! Flappy Bird (Quick and Simple C++)" -- @javidx9
' https://youtu.be/b6A4XHkTjs8

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour

Module Program

  Sub Main() 'args As String())
    Dim game As New FlappyBird
    game.ConstructConsole(80, 48, 16, 16)
    game.Start()
  End Sub

End Module

Class FlappyBird
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_birdPosition As Double = 0.0
  Private m_birdVelocity As Double = 0.0
  Private m_birdAcceleration As Single = 0.0
  Private ReadOnly m_gravity As Single = 100.0
  Private m_levelPosition As Double = 0.0

  Private m_sectionWidth As Single
  Private m_listSection As New List(Of Integer)

  Private m_hasCollided As Boolean = False
  Private m_resetGame As Boolean = False

  Private m_attemptCount As Integer = 0
  Private m_flapCount As Integer = 0
  Private m_maxFlapCount As Integer = 0

  Public Overrides Function OnUserCreate() As Boolean

    m_listSection = New List(Of Integer) From {0, 0, 0, 0}
    m_resetGame = True
    m_sectionWidth = CSng(ScreenWidth() / (m_listSection.Count - 1))

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    If m_resetGame Then
      m_hasCollided = False
      m_resetGame = False
      m_listSection = New List(Of Integer)({0, 0, 0, 0})
      m_birdAcceleration = 0.0
      m_birdVelocity = 0.0
      m_birdPosition = ScreenHeight() / 2.0
      m_flapCount = 0
      m_attemptCount += 1
    End If

    ' Game
    If m_hasCollided Then
      ' Do nothing until user releases space
      If m_keys(VK_SPACE).Released Then m_resetGame = True
    Else

      If m_keys(VK_SPACE).Pressed AndAlso m_birdVelocity >= m_gravity / 10.0 Then
        m_birdAcceleration = 0.0
        m_birdVelocity = -m_gravity / 4.0
        m_flapCount += 1
        If m_flapCount > m_maxFlapCount Then m_maxFlapCount = m_flapCount
      Else
        m_birdAcceleration += m_gravity * elapsedTime
      End If

      If m_birdAcceleration >= m_gravity Then m_birdAcceleration = m_gravity

      m_birdVelocity += m_birdAcceleration * elapsedTime
      m_birdPosition += m_birdVelocity * elapsedTime
      m_levelPosition += 14.0 * elapsedTime

      If m_levelPosition > m_sectionWidth Then
        m_levelPosition -= m_sectionWidth
        m_listSection.RemoveAt(0)
        Dim i = CInt(Rnd() * (ScreenHeight() - 20))
        If i <= 10 Then i = 0
        m_listSection.Add(i)
      End If

      ' Display
      Cls()

      ' Draw Sections
      Dim section = 0
      For Each s In m_listSection
        If s <> 0 Then
          Fill(section * m_sectionWidth + 10 - m_levelPosition, ScreenHeight() - s, section * m_sectionWidth + 15 - m_levelPosition, ScreenHeight(), PIXEL_SOLID, FG_GREEN)
          Fill(section * m_sectionWidth + 10 - m_levelPosition, 0, section * m_sectionWidth + 15 - m_levelPosition, ScreenHeight() - s - 15, PIXEL_SOLID, FG_GREEN)
        End If
        section += 1
      Next

      Dim birdX = CInt(ScreenWidth() / 3.0F)

      ' Collision Detection
      m_hasCollided = m_birdPosition < 2 OrElse m_birdPosition > ScreenHeight() - 2 OrElse
                      m_bufScreen(CInt(m_birdPosition + 0) * ScreenWidth() + birdX).CharUnion.UnicodeChar <> " "c OrElse
                      m_bufScreen(CInt(m_birdPosition + 1) * ScreenWidth() + birdX).CharUnion.UnicodeChar <> " "c OrElse
                      m_bufScreen(CInt(m_birdPosition + 0) * ScreenWidth() + birdX + 6).CharUnion.UnicodeChar <> " "c OrElse
                      m_bufScreen(CInt(m_birdPosition + 1) * ScreenWidth() + birdX + 6).CharUnion.UnicodeChar <> " "c

      ' Draw Bird
      If m_birdVelocity > 0 Then
        DrawString(birdX, m_birdPosition + 0, "\\")
        DrawString(birdX, m_birdPosition + 1, "<\\\\=Q")
      Else
        DrawString(birdX, m_birdPosition + 0, "<///=Q")
        DrawString(birdX, m_birdPosition + 1, "///")
      End If

      DrawString(1, 1, $"Attempt: {m_attemptCount} Score: {m_flapCount} High Score: {m_maxFlapCount}")

    End If

    Return True

  End Function

End Class