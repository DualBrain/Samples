' Inspired by "Code-It-Yourself! Tetris - Programming from Scratch (Quick and Simple C++)"
'
' https://youtu.be/8OK8_tHeCIA

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

  Private Declare Function CreateConsoleScreenBuffer Lib "kernel32.dll" (ByVal dwDesiredAccess As Integer, ByVal dwShareMode As UInteger, ByVal lpSecurityAttributes As IntPtr, ByVal dwFlags As UInteger, ByVal lpScreenBufferData As IntPtr) As IntPtr
  Private Declare Function SetConsoleActiveScreenBuffer Lib "kernel32.dll" (ByVal hConsoleOutput As IntPtr) As Boolean
  Private Declare Function WriteConsoleOutputCharacter Lib "kernel32.dll" Alias "WriteConsoleOutputCharacterA" (ByVal hConsoleOutput As IntPtr, ByVal lpCharacter As String, ByVal nLength As UInteger, ByVal dwWriteCoord As Coord, ByRef lpNumberOfCharsWritten As UInteger) As Boolean
  Private Declare Function GetAsyncKeyState Lib "user32.dll" (ByVal virtualKeyCode As Integer) As Short
  Private Declare Function CloseHandle Lib "kernel32.dll" (ByVal hObject As Long) As Long

  Private Const GENERIC_READ As Integer = &H80000000
  Private Const GENERIC_WRITE As Integer = &H40000000
  Private Const CONSOLE_TEXTMODE_BUFFER As Integer = 1

#End Region

  Private ReadOnly m_tetromino() As String = {"..X...X...X...X.",
                                              "..X..XX...X.....",
                                              ".....XX..XX.....",
                                              "..X..XX..X......",
                                              ".X...XX...X.....",
                                              ".X...X...XX.....",
                                              "..X...X..XX....."}

  Private ReadOnly m_fieldWidth As Integer = 12
  Private ReadOnly m_fieldHeight As Integer = 18
  Private ReadOnly m_field(m_fieldWidth * m_fieldHeight) As Byte

  Private ReadOnly m_screenWidth As Integer = Console.WindowWidth
  Private ReadOnly m_screenHeight As Integer = Console.WindowHeight

  Sub Main() 'args As String())

    For x = 0 To m_fieldWidth - 1
      For y = 0 To m_fieldHeight - 1
        m_field(y * m_fieldWidth + x) = If(x = 0 OrElse x = m_fieldWidth - 1 OrElse y = m_fieldHeight - 1, 9, 0)
      Next
    Next

    Dim screen(m_screenWidth * m_screenHeight) As Char
    For i = 0 To (m_screenWidth * m_screenHeight) - 1
      screen(i) = " "
    Next

    Dim hConsole As IntPtr = CreateConsoleScreenBuffer(GENERIC_READ Or GENERIC_WRITE, 0, IntPtr.Zero, CONSOLE_TEXTMODE_BUFFER, IntPtr.Zero)
    SetConsoleActiveScreenBuffer(hConsole)
    Dim dwBytesWritten As UInteger = 0

    Dim gameOver = False
    Dim currentPiece = 0
    Dim currentRotation = 0
    Dim currentX = m_fieldWidth \ 2
    Dim currentY = 0

    Dim key(3) As Boolean
    Dim rotateHold = True

    Dim speed = 20
    Dim speedCounter = 0

    Dim rnd = New Random()

    Dim vLines As New List(Of Integer)
    Dim score = 0
    Dim pieceCount = 0

    While Not gameOver

      ' Game Timing
      Threading.Thread.Sleep(50)
      speedCounter += 1
      Dim forceDown = speedCounter = speed

      ' Input
      For k = 0 To 3                                           ' R   L   D Z
        key(k) = (&H8000 And GetAsyncKeyState(AscW(Mid$(ChrW(&H27) + ChrW(&H25) + ChrW(&H28) + "Z", k + 1, 1)))) <> 0
      Next

      ' Game Logic

      ' Handle player movement
      currentX += If((key(0) AndAlso DoesPieceFit(currentPiece, currentRotation, currentX + 1, currentY)), 1, 0)
      currentX -= If((key(1) AndAlso DoesPieceFit(currentPiece, currentRotation, currentX - 1, currentY)), 1, 0)
      currentY += If((key(2) AndAlso DoesPieceFit(currentPiece, currentRotation, currentX, currentY + 1)), 1, 0)

      ' Rotate, but latch to stop wild spinning
      If key(3) Then
        currentRotation += If(rotateHold AndAlso DoesPieceFit(currentPiece, currentRotation + 1, currentX, currentY), 1, 0)
        rotateHold = False
      Else
        rotateHold = True
      End If

      If forceDown Then

        If DoesPieceFit(currentPiece, currentRotation, currentX, currentY + 1) Then
          currentY += 1 ' It can, so do it!
        Else

          ' Lock the current piece into the field
          For px = 0 To 3
            For py = 0 To 3
              If m_tetromino(currentPiece)(Rotate(px, py, currentRotation)) <> "." Then
                m_field((currentY + py) * m_fieldWidth + (currentX + px)) = currentPiece + 1
              End If
            Next
          Next

          pieceCount += 1
          If pieceCount Mod 10 = 0 Then
            If speed >= 10 Then speed -= 1
          End If

          ' Check have we got any lines
          For py = 0 To 3
            If currentY + py < m_fieldHeight - 1 Then
              Dim line = True
              For px = 1 To m_fieldWidth - 2
                line = line And m_field((currentY + py) * m_fieldWidth + px) <> 0
              Next
              If line Then
                For px = 1 To m_fieldWidth - 2
                  m_field((currentY + py) * m_fieldWidth + px) = 8
                Next
                vLines.Add(currentY + py)
              End If
            End If
          Next

          score += 25
          If vLines.Any Then score += vLines.Count * 100

          ' Choose next piece
          currentX = m_fieldWidth \ 2
          currentY = 0
          currentRotation = 0
          currentPiece = rnd.Next(0, 6)

          ' If piece does not fit
          gameOver = Not DoesPieceFit(currentPiece, currentRotation, currentX, currentY)

        End If

        speedCounter = 0

      End If

      ' Render Output

      ' Draw Field
      For x = 0 To m_fieldWidth - 1
        For y = 0 To m_fieldHeight - 1
          screen((y + 2) * m_screenWidth + (x + 2)) = " ABCDEFG=#"(m_field(y * m_fieldWidth + x))
        Next
      Next

      ' Draw Current Piece
      For px = 0 To 3
        For py = 0 To 3
          If m_tetromino(currentPiece)(Rotate(px, py, currentRotation)) = "X" Then
            screen((currentY + py + 2) * m_screenWidth + (currentX + px + 2)) = Chr(currentPiece + 65)
          End If
        Next
      Next

      ' Draw Score
      'TODO: Need to "print" to the array.
      Dim txt = $"SCORE: {score}"
      For index = 0 To txt.Length - 1
        screen(2 * m_screenWidth + m_fieldWidth + 6 + index) = txt(index)
      Next

      ' Animate Line Completion
      If vLines.Any Then

        ' Display Frame (cheekily to draw lines)
        WriteConsoleOutputCharacter(hConsole, screen, m_screenWidth * m_screenHeight, New Coord(0, 0), dwBytesWritten)
        Threading.Thread.Sleep(400)
        For Each v In vLines
          For px = 1 To m_fieldWidth - 2
            For py = v To 1 Step -1
              m_field(py * m_fieldWidth + px) = m_field((py - 1) * m_fieldWidth + px)
            Next
            m_field(px) = 0
          Next
        Next
        vLines.Clear()

      End If

      ' Display Frame
      WriteConsoleOutputCharacter(hConsole, screen, m_screenWidth * m_screenHeight, New Coord(0, 0), dwBytesWritten)

    End While

    CloseHandle(hConsole)
    Console.WriteLine($"Game Over!! Score:{score}")
    Console.ReadLine()

  End Sub

  Function Rotate(px As Integer, py As Integer, r As Integer) As Integer
    Select Case r Mod 4
      Case 0 : Return py * 4 + px         ' 0 degrees
      Case 1 : Return 12 + py - (px * 4)  ' 90 degrees
      Case 2 : Return 15 - (py * 4) - px  ' 180 degrees
      Case 3 : Return 3 - py + (px * 4)   ' 270 degrees
    End Select
    Return 0
  End Function

  Function DoesPieceFit(tetromino As Integer, rotation As Integer, posX As Integer, posY As Integer) As Boolean

    ' All Field cells >0 are occupied
    For px = 0 To 3
      For py = 0 To 3

        ' Get index into piece
        Dim pi = Rotate(px, py, rotation)

        ' Get index into field
        Dim fi = (posY + py) * m_fieldWidth + (posX + px)

        ' Check that test Is in bounds. Note out of bounds does
        ' Not necessarily mean a fail, as the long vertical piece
        ' can have cells that lie outside the boundary, so we'll
        ' just ignore them
        If (posX + px >= 0 AndAlso posX + px < m_fieldWidth) Then
          If (posY + py >= 0 AndAlso posY + py < m_fieldHeight) Then
            ' In Bounds so do collision check
            If (m_tetromino(tetromino)(pi) <> "." AndAlso m_field(fi) <> 0) Then
              Return False ' fail On first hit
            End If
          End If
        End If

      Next
    Next

    Return True

  End Function

End Module