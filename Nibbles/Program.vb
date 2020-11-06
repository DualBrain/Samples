Option Explicit On
Option Strict On
Option Infer On

' Requires a reference to QB.Compatibility
' Can be found at https://github.com/DualBrain/QB.Compatibility

Imports System.Text
Imports PlayString
Imports QB.Console
Imports QB.Core

Module Nibbles

  Private WithEvents Timer1 As New Timers.Timer With {.Enabled = True, .Interval = 10}

  '
  '                         Q B a s i c   N i b b l e s
  '
  '                   Copyright (C) Microsoft Corporation 1990
  '
  ' Nibbles is a game for one or two players.  Navigate your snakes
  ' around the game board trying to eat up numbers while avoiding
  ' running into walls or other snakes.  The more numbers you eat up,
  ' the more points you gain and the longer your snake becomes.
  '
  ' To run this game, press Shift+F5.
  '
  ' To exit QBasic, press Alt, F, X.
  '
  ' To get help on a BASIC keyword, move the cursor to the keyword and press
  ' F1 or click the right mouse button.
  '

  'Set default data type to integer for faster game play
  'DEFINT A-Z

  'User-defined TYPEs
  Structure SnakeBody
    Public row As Integer
    Public col As Integer
  End Structure

  'This type defines the player's snake
  Structure SnakeType
    Public head As Integer
    Public length As Integer
    Public row As Integer
    Public col As Integer
    Public direction As Integer
    Public lives As Integer
    Public score As Integer
    Public scolor As Integer
    Public alive As Boolean 'Integer
  End Structure

  'This type is used to represent the playing screen in memory
  'It is used to simulate graphics in text mode, and has some interesting,
  'and slightly advanced methods to increasing the speed of operation.
  'Instead of the normal 80x25 text graphics using "█", we will be
  'using "▄" and "▀" and "█" to mimic an 80x50
  'pixel screen.
  'Check out sub-programs SET and POINTISTHERE to see how this is implemented
  'feel free to copy these (as well as arenaType and the DIM ARENA stmt and the
  'initialization code in the DrawScreen subprogram) and use them in your own
  'programs
  Structure ArenaType
    Public RealRow As Integer 'Maps the 80x50 point into the real 80x25
    Public Acolor As Integer  'Stores the current color of the point
    Public Sister As Integer  'Each char has 2 points in it. Sister is
  End Structure               '-1 if sister point is above, +1 if below

  'Sub Declarations
  'Declare Sub SpacePause (text$)
  'Declare Sub PrintScore (NumPlayers%, score1%, score2%, lives1%, lives2%)
  'Declare Sub Intro ()
  'Declare Sub GetInputs (NumPlayers, speed, diff$, monitor$)
  'Declare Sub DrawScreen ()
  'Declare Sub PlayNibbles (NumPlayers, speed, diff$)
  'Declare Sub Set (row, col, acolor)
  'Declare Sub Center (row, text$)
  'Declare Sub DoIntro ()
  'Declare Sub Initialize ()
  'Declare Sub SparklePause ()
  'Declare Sub Level (WhatToDO, sammy() As snaketype)
  'Declare Sub InitColors ()
  'Declare Sub EraseSnake (snake() As ANY, snakeBod() As ANY, snakeNum%)
  'Declare Function StillWantsToPlay ()
  'Declare Function PointIsThere (row, col, backColor)

  'Constants
  'Const True = -1
  'Const False = Not True
  Const MAXSNAKELENGTH = 1000
  Const STARTOVER = 1             ' Parameters to 'Level' SUB
  Const SAMELEVEL = 2
  Const NEXTLEVEL = 3

  'Global Variables
  Private ReadOnly arena(50, 80) As ArenaType
  Private curLevel%
  Private ReadOnly colorTable%(10)

  Private NumPlayers%
  Private speed%
  Private diff$
  Private monitor$
  REM Private KeyFlags%
  Private kbd$
  Private sisterRow%
  'Private Number%
  REM Private startCol1%
  REM Private startCol2%
  REM Private startRow1%
  REM Private startRow2%
  'Private startTime#
  'Private stopTime#
  Private tail%
  Private playerDied As Boolean
  Private numberRow%
  Private numberCol%
  Private curSpeed%
  REM Private gameOver As Boolean
  Private nonum As Boolean


  Sub Main()

#Region "Prepare Console Window"

    Console.OutputEncoding = Encoding.UTF8

    Dim OrgBufferHeight%, OrgBufferWidth%, OrgWindowHeight%, OrgWindowWidth%

    OrgBufferHeight = Console.BufferHeight
    OrgBufferWidth = Console.BufferWidth
    OrgWindowHeight = Console.WindowHeight
    OrgWindowWidth = Console.WindowWidth

    Resize(80, 26)
    'Console.DisableMinimize()
    'Console.DisableMaximize()
    'Console.DisableResize()
    'Console.DisableQuickEditMode()

    Console.CursorVisible = False

#End Region

    Randomize(Timer)

    Call ClearKeyLocks() ' TODO: Need to address in some other way...

    Intro()

    GetInputs(NumPlayers%, speed, diff$, monitor$)
    'NumPlayers = 1 : speed = 100000000 : diff$ = "N" : monitor$ = "C"
    Call SetColors()
    DrawScreen()

    Do
      PlayNibbles(NumPlayers, speed, diff$)
    Loop While StillWantsToPlay()

    Call RestoreKeyLocks()
    COLOR(15, 0)
    CLS()

  End Sub

  Private Sub SetColors()

    'snake1     snake2   Walls  Background  Dialogs-Fore  Back
    'mono: DATA(15, 7, 7, 0, 15, 0)
    'normal: DATA(14, 13, 12, 1, 15, 4)

    Dim values As Integer()

    If monitor$ = "M" Then
      'RESTORE mono
      values = {15, 7, 7, 0, 15, 0}
    Else
      'RESTORE normal
      values = {14, 13, 12, 1, 15, 4}
    End If

    For A = 1 To 6
      'READ(colorTable(A))
      colorTable(A) = values(A - 1)
    Next A

  End Sub

  Private Sub RestoreKeyLocks()
    'DEF SEG = 0                     ' Restore CapLock, NumLock and ScrollLock states
    'TODO: POKE(1047, KeyFlags)
    'DEF SEG
  End Sub

  Private Sub ClearKeyLocks()

    If Console.CapsLock Then

    End If
    If Console.NumberLock Then

    End If
    'DEF SEG = 0                     ' Turn off CapLock, NumLock and ScrollLock
    REM KeyFlags = PEEK(1047)
    'TODO: POKE(1047, &H0)
    'DEF SEG
  End Sub

  'Center:
  '  Centers text on given row
  Sub Center(ByRef row%, ByRef text$)
    LOCATE(row, 41 - Len(text$) \ 2)
    PRINT(text$, True)
  End Sub

  'DrawScreen:
  '  Draws playing field
  Sub DrawScreen()

    'initialize screen
    'VIEWPRINT
    COLOR(colorTable(1), colorTable(4))
    CLS()

    'Print title & message
    Center(1, "Nibbles!")
    Center(11, "Initializing Playing Field...")

    'Initialize arena array
    For row = 1 To 50
      For col = 1 To 80
        arena(row, col).RealRow = (row + 1) \ 2
        arena(row, col).Sister = (row Mod 2) * 2 - 1
      Next col
    Next row

  End Sub

  'EraseSnake:
  '  Erases snake to facilitate moving through playing field
  Sub EraseSnake(ByRef snake() As SnakeType, ByRef snakeBod(,) As SnakeBody, ByRef snakeNum%)

    For c = 0 To 9
      For b = snake(snakeNum).length - c To 0 Step -10
        tail = (snake(snakeNum).head + MAXSNAKELENGTH - b) Mod MAXSNAKELENGTH
        [Set](snakeBod(tail, snakeNum).row, snakeBod(tail, snakeNum).col, colorTable(4))
      Next b
    Next c

  End Sub

  'GetInputs:
  '  Gets player inputs
  Sub GetInputs(ByRef NumPlayers%, ByRef speed%, ByRef diff$, ByRef monitor$)

    COLOR(7, 0)
    CLS()

    Dim num$ = ""
    Do
      LOCATE(5, 47) : PRINT(Space(34), True)
      LOCATE(5, 20)
      INPUT("How many players (1 or 2)", num$)
    Loop Until Val(num$) = 1 Or Val(num$) = 2
    NumPlayers = CInt(Fix(Val(num$)))

    LOCATE(8, 21) : PRINT("Skill level (1 to 100)")
    LOCATE(9, 22) : PRINT("1   = Novice")
    LOCATE(10, 22) : PRINT("90  = Expert")
    LOCATE(11, 22) : PRINT("100 = Twiddle Fingers")
    LOCATE(12, 15) : PRINT("(Computer speed may affect your skill level)")
    Dim gamespeed$ = ""
    Do
      LOCATE(8, 44) : PRINT(Space(35), True)
      LOCATE(8, 43)
      INPUT(gamespeed$)
    Loop Until Val(gamespeed$) >= 1 And Val(gamespeed$) <= 100
    speed = CInt(Fix(Val(gamespeed$)))

    speed = (100 - speed) * 2 + 1

    Do
      LOCATE(15, 56) : PRINT(Space(25), True)
      LOCATE(15, 15)
      INPUT("Increase game speed during play (Y or N)", diff$)
      diff$ = UCase(diff$)
    Loop Until diff$ = "Y" Or diff$ = "N"

    Do
      LOCATE(17, 46) : PRINT(Space(34), True)
      LOCATE(17, 17)
      INPUT("Monochrome or color monitor (M or C)", monitor$)
      monitor$ = UCase(monitor$)
    Loop Until monitor$ = "M" Or monitor$ = "C"

    'Dim startTime# = Timer                          ' Calculate speed of system
    'For i# = 1 To 1000 : Next i#                 ' and do some compensation
    'Dim stopTime# = Timer
    'speed = speed * 0.5 / (stopTime# - startTime#)
    'TODO: Need to calculate speed... old MS-DOS way is WAY OFF.
    'speed = 100000000

  End Sub

  'InitColors:
  'Initializes playing field colors
  Sub InitColors()

    For row = 1 To 50
      For col = 1 To 80
        arena(row, col).Acolor = colorTable(4)
      Next col
    Next row

    CLS()

    'Set (turn on) pixels for screen border
    For col = 1 To 80
      [Set](3, col, colorTable(3))
      [Set](50, col, colorTable(3))
    Next col

    For row = 4 To 49
      [Set](row, 1, colorTable(3))
      [Set](row, 80, colorTable(3))
    Next row

  End Sub

  'Intro:
  '  Displays game introduction
  Sub Intro()

    SCREEN(0)
    WIDTH(80, 25)
    COLOR(15, 0)
    CLS()

    Center(4, "Q B a s i c   N i b b l e s")
    COLOR(7)
    Center(6, "Copyright (C) Microsoft Corporation 1990")
    Center(8, "Nibbles is a game for one or two players.  Navigate your snakes")
    Center(9, "around the game board trying to eat up numbers while avoiding")
    Center(10, "running into walls or other snakes.  The more numbers you eat up,")
    Center(11, "the more points you gain and the longer your snake becomes.")
    Center(13, " Game Controls ")
    Center(15, "  General             Player 1               Player 2    ")
    Center(16, "                        (Up)                   (Up)      ")
    Center(17, "P - Pause                " + QBChr(24) + "                      W       ")
    Center(18, "                     (Left) " + QBChr(27) + "   " + QBChr(26) + " (Right)   (Left) A   D (Right)  ")
    Center(19, "                         " + QBChr(25) + "                      S       ")
    Center(20, "                       (Down)                 (Down)     ")
    Center(24, "Press any key to continue")

    PLAY("MBT160O1L8CDEDCDL4ECC")

    SparklePause()

  End Sub

  'Level:
  'Sets game level
  Sub Level(ByRef WhatToDO%, ByRef sammy() As SnakeType) REM Static

    Select Case (WhatToDO)

      Case STARTOVER
        curLevel = 1
      Case NEXTLEVEL
        curLevel += 1
    End Select

    sammy(1).head = 1                       'Initialize Snakes
    sammy(1).length = 2
    sammy(1).alive = True
    sammy(2).head = 1
    sammy(2).length = 2
    sammy(2).alive = True

    InitColors()

    Select Case curLevel
      Case 1
        sammy(1).row = 25 : sammy(2).row = 25
        sammy(1).col = 50 : sammy(2).col = 30
        sammy(1).direction = 4 : sammy(2).direction = 3


      Case 2
        For i = 20 To 60
          [Set](25, i, colorTable(3))
        Next i
        sammy(1).row = 7 : sammy(2).row = 43
        sammy(1).col = 60 : sammy(2).col = 20
        sammy(1).direction = 3 : sammy(2).direction = 4

      Case 3
        For i = 10 To 40
          [Set](i, 20, colorTable(3))
          [Set](i, 60, colorTable(3))
        Next i
        sammy(1).row = 25 : sammy(2).row = 25
        sammy(1).col = 50 : sammy(2).col = 30
        sammy(1).direction = 1 : sammy(2).direction = 2

      Case 4
        For i = 4 To 30
          [Set](i, 20, colorTable(3))
          [Set](53 - i, 60, colorTable(3))
        Next i
        For i = 2 To 40
          [Set](38, i, colorTable(3))
          [Set](15, 81 - i, colorTable(3))
        Next i
        sammy(1).row = 7 : sammy(2).row = 43
        sammy(1).col = 60 : sammy(2).col = 20
        sammy(1).direction = 3 : sammy(2).direction = 4

      Case 5
        For i = 13 To 39
          [Set](i, 21, colorTable(3))
          [Set](i, 59, colorTable(3))
        Next i
        For i = 23 To 57
          [Set](11, i, colorTable(3))
          [Set](41, i, colorTable(3))
        Next i
        sammy(1).row = 25 : sammy(2).row = 25
        sammy(1).col = 50 : sammy(2).col = 30
        sammy(1).direction = 1 : sammy(2).direction = 2

      Case 6
        For i = 4 To 49
          If i > 30 Or i < 23 Then
            [Set](i, 10, colorTable(3))
            [Set](i, 20, colorTable(3))
            [Set](i, 30, colorTable(3))
            [Set](i, 40, colorTable(3))
            [Set](i, 50, colorTable(3))
            [Set](i, 60, colorTable(3))
            [Set](i, 70, colorTable(3))
          End If
        Next i
        sammy(1).row = 7 : sammy(2).row = 43
        sammy(1).col = 65 : sammy(2).col = 15
        sammy(1).direction = 2 : sammy(2).direction = 1

      Case 7
        For i = 4 To 49 Step 2
          [Set](i, 40, colorTable(3))
        Next i
        sammy(1).row = 7 : sammy(2).row = 43
        sammy(1).col = 65 : sammy(2).col = 15
        sammy(1).direction = 2 : sammy(2).direction = 1

      Case 8
        For i = 4 To 40
          [Set](i, 10, colorTable(3))
          [Set](53 - i, 20, colorTable(3))
          [Set](i, 30, colorTable(3))
          [Set](53 - i, 40, colorTable(3))
          [Set](i, 50, colorTable(3))
          [Set](53 - i, 60, colorTable(3))
          [Set](i, 70, colorTable(3))
        Next i
        sammy(1).row = 7 : sammy(2).row = 43
        sammy(1).col = 65 : sammy(2).col = 15
        sammy(1).direction = 2 : sammy(2).direction = 1

      Case 9
        For i = 6 To 47
          [Set](i, i, colorTable(3))
          [Set](i, i + 28, colorTable(3))
        Next i
        sammy(1).row = 40 : sammy(2).row = 15
        sammy(1).col = 75 : sammy(2).col = 5
        sammy(1).direction = 1 : sammy(2).direction = 2

      Case Else
        For i = 4 To 49 Step 2
          [Set](i, 10, colorTable(3))
          [Set](i + 1, 20, colorTable(3))
          [Set](i, 30, colorTable(3))
          [Set](i + 1, 40, colorTable(3))
          [Set](i, 50, colorTable(3))
          [Set](i + 1, 60, colorTable(3))
          [Set](i, 70, colorTable(3))
        Next i
        sammy(1).row = 7 : sammy(2).row = 43
        sammy(1).col = 65 : sammy(2).col = 15
        sammy(1).direction = 2 : sammy(2).direction = 1

    End Select
  End Sub

  'PlayNibbles:
  '  Main routine that controls game play
  Sub PlayNibbles(ByRef NumPlayers%, ByRef speed%, ByRef diff$)

    'Initialize Snakes
    Dim sammyBody(MAXSNAKELENGTH - 1, 2) As SnakeBody
    Dim sammy(2) As SnakeType
    sammy(1).lives = 5
    sammy(1).score = 0
    sammy(1).scolor = colorTable(1)
    sammy(2).lives = 5
    sammy(2).score = 0
    sammy(2).scolor = colorTable(2)

    Level(STARTOVER, sammy)
    REM startRow1 = sammy(1).row : startCol1 = sammy(1).col
    REM startRow2 = sammy(2).row : startCol2 = sammy(2).col

    curSpeed = speed

    'play Nibbles until finished

    SpacePause("     Level" + Str(curLevel) + ",  Push Space")
    REM gameOver = False
    Do

      If NumPlayers = 1 Then
        sammy(2).row = 0
      End If

      Dim number = 1          'Current number that snakes are trying to run into
      nonum = True        'nonum = TRUE if a number is not on the screen

      playerDied = False
      PrintScore(NumPlayers, sammy(1).score, sammy(2).score, sammy(1).lives, sammy(2).lives)
      PLAY("T160O1>L20CDEDCDL10ECC")

      Do
        'Print number if no number exists
        If nonum = True Then
          Do
            numberRow = CInt(Fix(Int(Rnd(1) * 47 + 3)))
            numberCol = CInt(Fix(Int(Rnd(1) * 78 + 2)))
            sisterRow = numberRow + arena(numberRow, numberCol).Sister
          Loop Until Not PointIsThere(numberRow, numberCol, colorTable(4)) And Not PointIsThere(sisterRow, numberCol, colorTable(4))
          numberRow = arena(numberRow, numberCol).RealRow
          nonum = False
          COLOR(colorTable(1), colorTable(4))
          LOCATE(numberRow, numberCol)
          PRINT(Right(Str(number), 1), True)
          'Dim count = 0
        End If

        'Delay game
        'For A = 1 To curSpeed : Next A
        'Console.SetCursorPosition(0, 0)
        'Console.Write(curSpeed)
        Threading.Thread.Sleep(curSpeed)

        'Get keyboard input & Change direction accordingly
        kbd$ = INKEY$()

        Select Case kbd$
          Case "w", "W" : If sammy(2).direction <> 2 Then sammy(2).direction = 1
          Case "s", "S" : If sammy(2).direction <> 1 Then sammy(2).direction = 2
          Case "a", "A" : If sammy(2).direction <> 4 Then sammy(2).direction = 3
          Case "d", "D" : If sammy(2).direction <> 3 Then sammy(2).direction = 4
          Case Chr(0) + "H" : If sammy(1).direction <> 2 Then sammy(1).direction = 1
          Case Chr(0) + "P" : If sammy(1).direction <> 1 Then sammy(1).direction = 2
          Case Chr(0) + "K" : If sammy(1).direction <> 4 Then sammy(1).direction = 3
          Case Chr(0) + "M" : If sammy(1).direction <> 3 Then sammy(1).direction = 4
          Case "p", "P" : SpacePause(" Game Paused ... Push Space  ")
          Case Else
        End Select

        For A = 1 To NumPlayers
          'Move Snake
          Select Case sammy(A).direction
            Case 1 : sammy(A).row = sammy(A).row - 1
            Case 2 : sammy(A).row = sammy(A).row + 1
            Case 3 : sammy(A).col = sammy(A).col - 1
            Case 4 : sammy(A).col = sammy(A).col + 1
          End Select

          'If snake hits number, respond accordingly
          If numberRow = Int((sammy(A).row + 1) \ 2) And numberCol = sammy(A).col Then
            PLAY("MBO0L16>CCCE") : Beep()
            If sammy(A).length < (MAXSNAKELENGTH - 30) Then
              sammy(A).length = sammy(A).length + number * 4
            End If
            sammy(A).score = sammy(A).score + number
            PrintScore(NumPlayers, sammy(1).score, sammy(2).score, sammy(1).lives, sammy(2).lives)
            number += 1
            If number = 10 Then
              EraseSnake(sammy, sammyBody, 1)
              EraseSnake(sammy, sammyBody, 2)
              LOCATE(numberRow, numberCol) : PRINT(" ")
              Level(NEXTLEVEL, sammy)
              PrintScore(NumPlayers, sammy(1).score, sammy(2).score, sammy(1).lives, sammy(2).lives)
              SpacePause("     Level" + Str(curLevel) + ",  Push Space")
              If NumPlayers = 1 Then sammy(2).row = 0
              number = 1
              'If diff$ = "P" Then speed -= 10 : curSpeed = speed
              If diff$ = "Y" Then speed -= 10 : curSpeed = speed
            End If
            nonum = True
            'If curSpeed < 1 Then curSpeed = 1
            If curSpeed < 50 Then curSpeed = 50
          End If
        Next

        For a = 1 To NumPlayers
          'If player runs into any point, or the head of the other snake, it dies.
          If PointIsThere(sammy(a).row, sammy(a).col, colorTable(4)) Or (sammy(1).row = sammy(2).row And sammy(1).col = sammy(2).col) Then
            PLAY("MBO0L32EFGEFDC")
            COLOR(Console.ForegroundColor, colorTable(4))
            LOCATE(numberRow, numberCol)
            PRINT(" ")

            playerDied = True
            sammy(a).alive = False
            sammy(a).lives = sammy(a).lives - 1

            'Otherwise, move the snake, and erase the tail
          Else
            sammy(a).head = (sammy(a).head + 1) Mod MAXSNAKELENGTH
            sammyBody(sammy(a).head, a).row = sammy(a).row
            sammyBody(sammy(a).head, a).col = sammy(a).col
            tail = (sammy(a).head + MAXSNAKELENGTH - sammy(a).length) Mod MAXSNAKELENGTH
            [Set](sammyBody(tail, a).row, sammyBody(tail, a).col, colorTable(4))
            sammyBody(tail, a).row = 0
            [Set](sammy(a).row, sammy(a).col, sammy(a).scolor)
          End If
        Next

      Loop Until playerDied

      curSpeed = speed                ' reset speed to initial value

      For A = 1 To NumPlayers
        EraseSnake(sammy, sammyBody, A)

        'If dead, then erase snake in really cool way
        If sammy(A).alive = False Then
          'Update score
          sammy(A).score = sammy(A).score - 10
          PrintScore(NumPlayers, sammy(1).score, sammy(2).score, sammy(1).lives, sammy(2).lives)

          If A = 1 Then
            SpacePause(" Sammy Dies! Push Space! --->")
          Else
            SpacePause(" <---- Jake Dies! Push Space ")
          End If
        End If
      Next

      Level(SAMELEVEL, sammy)
      PrintScore(NumPlayers, sammy(1).score, sammy(2).score, sammy(1).lives, sammy(2).lives)

      'Play next round, until either of snake's lives have run out.

    Loop Until sammy(1).lives = 0 OrElse sammy(2).lives = 0

  End Sub

  'PointIsThere:
  '  Checks the global  arena array to see if the boolean flag is set
  Function PointIsThere(ByRef row%, ByRef col%, ByRef acolor%) As Boolean
    If row <> 0 Then
      If arena(row, col).Acolor <> acolor Then
        Return True
      Else
        Return False
      End If
    Else
      Return False
    End If
  End Function

  'PrintScore:
  '  Prints players scores and number of lives remaining
  Sub PrintScore(ByRef NumPlayers%, ByRef score1%, ByRef score2%, ByRef lives1%, ByRef lives2%)

    COLOR(15, colorTable(4))

    If NumPlayers = 2 Then
      LOCATE(1, 1)
      PRINT($"{score2:N}  Lives: {lives2}  <--JAKE")
    End If

    LOCATE(1, 49)
    PRINT($"SAMMY-->  Lives:  {lives1}     {score1:N}")

  End Sub

  'Set:
  '  Sets row and column on playing field to given color to facilitate moving
  '  of snakes around the field.
  Sub [Set](row%, col%, acolor%)

    If row <> 0 Then

      arena(row, col).Acolor = acolor                  'assign color to arena
      Dim realRow% = arena(row, col).RealRow           'Get real row of pixel
      'Dim topFlag% = CInt(arena(row, col).sister + 1 / 2)    'Deduce whether pixel is on top, or bottom
      'Dim topFlag = If(arena(row, col).sister = 1, True, False)   'Deduce whether pixel is on top, or bottom
      sisterRow = row + arena(row, col).Sister         'Get arena row of sister
      Dim sisterColor = arena(sisterRow, col).Acolor   'Determine sister's color

      LOCATE(realRow, col)

      If acolor = sisterColor Then          'If both points are same
        COLOR(acolor, acolor)               'Print "█"
        PRINT("█", True)
      Else
        If arena(row, col).Sister = 1 Then 'topFlag Then                'Since you cannot have
          If acolor > 7 Then                'bright backgrounds
            COLOR(acolor, sisterColor)      'determine best combo
            PRINT("▀", True)                'to use.
          Else
            COLOR(sisterColor, acolor)
            PRINT("▄", True)
          End If
        Else
          If acolor > 7 Then
            COLOR(acolor, sisterColor)
            PRINT("▄", True)
          Else
            COLOR(sisterColor, acolor)
            PRINT("▀", True)
          End If
        End If
      End If
    End If
  End Sub

  ' Pauses game play and waits for space bar to be pressed before continuing.
  Private Sub SpacePause(text$)

    ' Adjust the size of text to fit the box.
    text = Left(text$ + Space(28), 28)

    ' Draw the box / message.
    COLOR(colorTable(5), colorTable(6))
    Center(11, "████████████████████████████████")
    Center(12, "█ " + text + " █")
    Center(13, "████████████████████████████████")
    While INKEY$() <> "" : End While
    While INKEY$() <> " " : End While
    COLOR(15, colorTable(4))

    ' Restore the screen background.
    For i = 21 To 26
      For j = 24 To 56
        [Set](i, j, arena(i, j).Acolor)
      Next
    Next

  End Sub

  ' Creates flashing border for intro screen.
  Private Sub SparklePause()

    COLOR(4, 0)
    Dim Astring = "*    *    *    *    *    *    *    *    *    *    *    *    *    *    *    *    *    "
    While INKEY$() <> "" : End While ' Clear keyboard buffer.

    While INKEY$() = ""

      For a = 1 To 5

        ' Print horizontal sparkles.
        LOCATE(1, 1) : PRINT(Mid(Astring, a, 80), True)
        LOCATE(22, 1) : PRINT(Mid(Astring, 6 - a, 80), True)

        ' Print Vertical sparkles.
        For b = 2 To 21
          Dim c = (a + b) Mod 5
          If c = 1 Then
            LOCATE(b, 80) : PRINT("*", True)
            LOCATE(23 - b, 1) : PRINT("*", True)
          Else
            LOCATE(b, 80) : PRINT(" ", True)
            LOCATE(23 - b, 1) : PRINT(" ", True)
          End If
        Next

        Threading.Thread.Sleep(100)

      Next

    End While

  End Sub

  '  Determines if users want to play game again.
  Private Function StillWantsToPlay() As Boolean

    COLOR(colorTable(5), colorTable(6))
    Center(10, "█████████████████████████████████")
    Center(11, "█       G A M E   O V E R       █")
    Center(12, "█                               █")
    Center(13, "█      Play Again?   (Y/N)      █")
    Center(14, "█████████████████████████████████")

    While INKEY$() <> "" : End While
    Do
      kbd$ = UCase(INKEY$)
    Loop Until kbd$ = "Y" Or kbd$ = "N"

    COLOR(15, colorTable(4))
    Center(10, "                                 ")
    Center(11, "                                 ")
    Center(12, "                                 ")
    Center(13, "                                 ")
    Center(14, "                                 ")

    If kbd$ = "Y" Then
      Return True
    Else
      COLOR(7, 0)
      CLS()
      Return False
    End If

  End Function

  Public Sub Resize(ByRef cols%, ByRef rows%)
    'Try
    Console.SetWindowSize(cols%, rows%) ' Set the windows size...
    'Catch ex As Exception
    '  Console.WriteLine("1 - " & ex.ToString)
    '  Threading.Thread.Sleep(5000)
    'End Try
    'Try
    Console.SetBufferSize(cols%, rows%) ' Then set the buffer size to the now window size...
    'Catch ex As Exception
    '  Console.WriteLine("2 - " & ex.ToString)
    '  Threading.Thread.Sleep(5000)
    'End Try
    'Try
    Console.SetWindowSize(cols%, rows%) ' Then set the window size again so that the scroll bar area is removed.
    'Catch ex As Exception
    '  Console.WriteLine("3 - " & ex.ToString)
    '  Threading.Thread.Sleep(5000)
    'End Try
  End Sub


  Sub PLAY(macro As String)

    If macro.ToLower.StartsWith("mb") Then
      m_stack.Push(macro)
    Else
      Try
        Dim str2wav As New ParsePlayMacro(macro)
        str2wav.Generate()
        str2wav.PlayOrSaveWav(Nothing)
      Catch ex As Exception
        Console.WriteLine(ex.Message)
      End Try
    End If

  End Sub

  Private m_stack As New Stack(Of String)
  Private m_playing As Boolean

  Private Async Sub Timer1_Elapsed(sender As Object, e As EventArgs) Handles Timer1.Elapsed

    If m_playing Then Return
    If m_stack.Count = 0 Then Return

    m_playing = True
    Try

      Dim macro = m_stack.Pop

      Dim str2wav As New ParsePlayMacro(macro)
      str2wav.Generate()
      Await str2wav.PlayAsync()

    Finally
      m_playing = False
    End Try

  End Sub

End Module