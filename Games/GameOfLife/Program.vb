' Inspired by: "What are Cellular Automata? -- @javidx9"
' https://youtu.be/E7CxMHsYzSs

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour

Module Program
  Sub Main() 'args As String())
    Dim game As New GameOfLife
    game.ConstructConsole(160, 100, 8, 8)
    game.Start()
  End Sub

End Module

Class GameOfLife
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_output() As Integer
  Private m_state() As Integer

  Public Overrides Function OnUserCreate() As Boolean

    ReDim m_output(ScreenWidth() * ScreenHeight())
    ReDim m_state(ScreenWidth() * ScreenHeight())

    Dim setem = Sub(x As Integer, y As Integer, s As String)
                  Dim p As Integer = 0
                  For Each c As Char In s
                    m_state(y * ScreenWidth() + x + p) = If(c = "#"c, 1, 0)
                    p += 1
                  Next
                End Sub

    '' R-Pentomino
    'setem(80, 50, "  ## ")
    'setem(80, 51, " ##  ")
    'setem(80, 52, "  #  ")

    '' Gosper Glider Gun
    'setem(60, 45, "........................#............")
    'setem(60, 46, "......................#.#............")
    'setem(60, 47, "............##......##............##.")
    'setem(60, 48, "...........#...#....##............##.")
    'setem(60, 49, "##........#.....#...##...............")
    'setem(60, 50, "##........#...#.##....#.#............")
    'setem(60, 51, "..........#.....#.......#............")
    'setem(60, 52, "...........#...#.....................")
    'setem(60, 53, "............##.......................")

    '' Infinite Growth
    'setem(20, 50, "########.#####...###......#######.#####")

    ' Random
    For i = 0 To (ScreenWidth() * ScreenHeight()) - 1
      m_state(i) = If(Rand.Next(0, 2) = 0, 0, 1)
    Next

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Threading.Thread.Sleep(50)

    'If m_keys(VK_SPACE).bHeld Then Return True

    Dim cell As Func(Of Integer, Integer, Integer) = Function(x As Integer, y As Integer)
                                                       Return m_output(y * ScreenWidth() + x)
                                                     End Function

    ' Store output state
    For i = 0 To (ScreenWidth() * ScreenHeight()) - 1
      m_output(i) = m_state(i)
    Next

    For x = 1 To ScreenWidth() - 2
      For y = 1 To ScreenHeight() - 2

        Dim neighbors = cell(x - 1, y - 1) + cell(x + 0, y - 1) + cell(x + 1, y - 1) +
                        cell(x - 1, y + 0) + 0 + cell(x + 1, y + 0) +
                        cell(x - 1, y + 1) + cell(x + 0, y + 1) + cell(x + 1, y + 1)

        If cell(x, y) = 1 Then
          m_state(y * ScreenWidth() + x) = If(neighbors = 2 OrElse neighbors = 3, 1, 0)
        Else
          m_state(y * ScreenWidth() + x) = If(neighbors = 3, 1, 0)
        End If

        If cell(x, y) = 1 Then
          Draw(x, y, PIXEL_SOLID, FG_WHITE)
        Else
          Draw(x, y, PIXEL_SOLID, FG_BLACK)
        End If

      Next
    Next


    Return True

  End Function

End Class