' Inspired by: "Code-It-Yourself! Snake! - Programming from Scratch (Quick and Simple C++)" -- @javidx9
' https://youtu.be/e8lYLYlrGLg

Option Explicit On
Option Strict On
Option Infer On

Imports System.Runtime.InteropServices

Module Program

#Region "Win32"

  <StructLayout(LayoutKind.Sequential)>
  Public Structure Coord
    Public X As Short
    Public Y As Short
    Public Sub New(x As Short, y As Short)
      Me.X = x
      Me.Y = y
    End Sub
  End Structure

  Private Declare Function CreateConsoleScreenBuffer Lib "kernel32.dll" (dwDesiredAccess As Integer, dwShareMode As UInteger, lpSecurityAttributes As IntPtr, dwFlags As UInteger, lpScreenBufferData As IntPtr) As IntPtr
  Private Declare Function SetConsoleActiveScreenBuffer Lib "kernel32.dll" (hConsoleOutput As IntPtr) As Boolean
  Private Declare Function WriteConsoleOutputCharacter Lib "kernel32.dll" Alias "WriteConsoleOutputCharacterA" (hConsoleOutput As IntPtr, lpCharacter As String, nLength As Integer, dwWriteCoord As Coord, ByRef lpNumberOfCharsWritten As Integer) As Boolean
  Private Declare Function GetAsyncKeyState Lib "user32.dll" (virtualKeyCode As Integer) As Short
  Private Declare Function CloseHandle Lib "kernel32.dll" (hObject As Long) As Long

  Private Const GENERIC_READ As Integer = &H80000000
  Private Const GENERIC_WRITE As Integer = &H40000000
  Private Const CONSOLE_TEXTMODE_BUFFER As Integer = 1

#End Region

  Private ReadOnly m_screenWidth As Integer = 120
  Private ReadOnly m_screenHeight As Integer = 30

  Private Class SnakeSegment
    Public Property X As Integer
    Public Property Y As Integer
    Public Sub New(x As Integer, y As Integer)
      Me.X = x
      Me.Y = y
    End Sub
  End Class

  Private ReadOnly m_random As New Random
  Private Const RAND_MAX As Integer = 2147483647

  Private Function Rand() As Integer
    Return CInt(Fix(m_random.NextDouble * RAND_MAX))
  End Function

  Sub Main() 'args As String())

    Dim screen((m_screenWidth * m_screenHeight) - 1) As Char
    For i = 0 To (m_screenWidth * m_screenHeight) - 1
      screen(i) = " "c
    Next
    Dim hConsole = CreateConsoleScreenBuffer(GENERIC_READ Or GENERIC_WRITE, 0, IntPtr.Zero, CONSOLE_TEXTMODE_BUFFER, IntPtr.Zero)
    SetConsoleActiveScreenBuffer(hConsole)
    Dim dwBytesWritten = 0

    While True

      ' Reset to known state
      Dim snake As New List(Of SnakeSegment) From {New SnakeSegment(60, 15),
                                                    New SnakeSegment(61, 15),
                                                    New SnakeSegment(62, 15),
                                                    New SnakeSegment(63, 15),
                                                    New SnakeSegment(64, 15),
                                                    New SnakeSegment(65, 15),
                                                    New SnakeSegment(66, 15),
                                                    New SnakeSegment(67, 15),
                                                    New SnakeSegment(68, 15),
                                                    New SnakeSegment(69, 15)}
      Dim foodX = 30
      Dim foodY = 15
      Dim score = 0
      Dim snakeDirection = 3
      Dim dead = False
      Dim keyLeftOld = False
      Dim keyRightOld = False

      While Not dead

        ' Frame Timing, compensate for aspect ratio of command line
        Dim t1 = DateTime.Now
        While (DateTime.Now - t1) < If(snakeDirection Mod 2 = 1, TimeSpan.FromMilliseconds(120), TimeSpan.FromMilliseconds(200))

          ' Get Input, 
          Dim keyRight = (&H8000 And GetAsyncKeyState(&H27)) <> 0
          Dim keyLeft = (&H8000 And GetAsyncKeyState(&H25)) <> 0

          If keyRight AndAlso Not keyRightOld Then
            snakeDirection += 1
            If snakeDirection = 4 Then snakeDirection = 0
          End If

          If keyLeft AndAlso Not keyLeftOld Then
            snakeDirection -= 1
            If snakeDirection = -1 Then snakeDirection = 3
          End If

          keyRightOld = keyRight
          keyLeftOld = keyLeft

        End While

        ' ==== Logic

        ' Update Snake Position, place a new head at the front of the list in
        ' the right direction
        Select Case snakeDirection
          Case 0 : snake.Insert(0, New SnakeSegment(snake(0).X, snake(0).Y - 1)) ' UP
          Case 1 : snake.Insert(0, New SnakeSegment(snake(0).X + 1, snake(0).Y)) ' RIGHT
          Case 2 : snake.Insert(0, New SnakeSegment(snake(0).X, snake(0).Y + 1)) ' DOWN
          Case 3 : snake.Insert(0, New SnakeSegment(snake(0).X - 1, snake(0).Y)) ' LEFT
        End Select

        ' Collision Detect Snake V Food
        If snake.First.X = foodX AndAlso snake.First.Y = foodY Then
          score += 1
          While screen(foodY * m_screenWidth + foodX) <> " "
            foodX = Rand() Mod m_screenWidth
            foodY = (Rand() Mod (m_screenHeight - 3)) + 3
          End While
          For i = 0 To 4
            snake.Add(New SnakeSegment(snake.Last.X, snake.Last.Y))
          Next
        End If

        ' Collision Detect Snake V World
        If snake.First.X < 0 OrElse snake.First.X > m_screenWidth Then
          dead = True
        End If
        If snake.First.Y < 3 OrElse snake.First.Y > m_screenHeight Then
          dead = True
        End If

        ' Collision Detect Snake V Snake
        For Each i In snake
          If i IsNot snake.First AndAlso i.X = snake.First.X AndAlso i.Y = snake.First.Y Then
            dead = True
            Exit For
          End If
        Next

        ' Chop off Snakes tail :-/
        snake.RemoveAt(snake.Count - 1)

        ' ==== Presentation

        ' Clear Screen
        For i = 0 To m_screenWidth * m_screenHeight - 1
          screen(i) = " "c
        Next

        ' Draw Stats & Border
        For i = 0 To m_screenWidth - 1
          screen(i) = "="c
          screen(2 * m_screenWidth + i) = "="c
        Next

        Dim txt = $"gotBASIC.com and OneLoneCoder.com - S N A K E ! !                SCORE: {score}"
        For index = 0 To txt.Length - 1
          screen(1 * m_screenWidth + 6 + index) = txt(index)
        Next

        ' Draw Snake Body
        For Each s In snake
          Dim si = s.Y * m_screenWidth + s.X
          If si < m_screenWidth * m_screenHeight Then
            screen(si) = If(dead, "+"c, "O"c)
          End If
        Next

        ' Draw Snake Head
        Dim hi = snake.First.Y * m_screenWidth + snake.First.X
        If hi < m_screenWidth * m_screenHeight Then
          screen(hi) = If(dead, "X"c, "@"c)
        End If

        ' Draw Food
        screen(foodY * m_screenWidth + foodX) = "%"c

        If dead Then
          txt = $"    PRESS 'SPACE' TO PLAY AGAIN    "
          For index = 0 To txt.Length - 1
            screen(15 * m_screenWidth + 40 + index) = txt(index)
          Next
        End If

        ' Display Frame
        WriteConsoleOutputCharacter(hConsole, screen, m_screenWidth * m_screenHeight, New Coord(0, 0), dwBytesWritten)

      End While

      ' Wait for space
      While ((&H8000 And GetAsyncKeyState(&H20)) = 0) : End While

    End While

  End Sub

End Module