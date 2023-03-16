' Inspired by: "A Request! Then Some Terrible Code." -- @javidx9
' https://youtu.be/oJGx8cqaJLc

'NOTE: Currently not working; not sure if it's a bad API declaration and/or unicode'isms.

Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Math

Module Program

  Private Const ENABLE_VIRTUAL_TERMINAL_PROCESSING As UInt32 = &H4
  Private Const ENABLE_EXTENDED_FLAGS As UInt32 = &H80
  Private Const STD_OUTPUT_HANDLE As UInt32 = &HFFFFFFF5UI
  Private Const STD_INPUT_HANDLE As UInt32 = &HFFFFFFF4UI

  Private Declare Function GetAsyncKeyState Lib "user32.dll" (virtualKeyCode As Integer) As Short
  Private Declare Auto Function GetStdHandle Lib "kernel32.dll" (nStdHandle As UInt32) As IntPtr

  <DllImport("kernel32.dll", SetLastError:=True)>
  Private Function GetConsoleMode(ByVal hConsoleHandle As IntPtr, ByRef lpMode As UInt32) As Boolean
  End Function

  <DllImport("kernel32.dll", SetLastError:=True)>
  Private Function SetConsoleMode(ByVal hConsoleHandle As IntPtr, ByVal dwMode As UInt32) As Boolean
  End Function

  Sub Main() 'args As String())

    Dim d As UInt32 = 0
    GetConsoleMode(GetStdHandle(STD_OUTPUT_HANDLE), d)
    SetConsoleMode(GetStdHandle(STD_OUTPUT_HANDLE), d Or &H4) ' ??? Mystery Console Mode Feature!
    GetConsoleMode(GetStdHandle(STD_INPUT_HANDLE), d)
    SetConsoleMode(GetStdHandle(STD_INPUT_HANDLE), d Or &H200)

    Dim tp1 As DateTime = Now()
    Dim tp2 As DateTime = tp1
    Dim s(80 * 30 - 1) As Char
    Dim n(9) As Char

    Dim pfxy = Sub(b As String, x As Integer, y As Integer)
                 Console.SetCursorPosition(x, y)
                 Console.Write(b)
               End Sub
    Dim sbxy = Sub(x As Integer, y As Integer, cc As Char) s(y * 80 + x) = cc

    Dim t As Single = 0.0F, f As Single = 0.0F, tt As Single = 0.0F ' Timing Variables
    Dim w As Single = 20.0F, m As Single = 40.0F, tw As Single = 20.0F, tm As Single = 40.0F ' World Shape Variables
    Dim e As Single = 1.0F, c As Single = 40.0F ' Difficulty, Player Position
    Dim r As Integer = 0, h As Integer = 1000 ' Player Score

    While h > 0

      ' Timing
      Thread.Sleep(10)
      tp2 = Now()
      Dim elapsedTime As TimeSpan = tp2 - tp1 : tp1 = tp2
      t += (f = elapsedTime.TotalSeconds) : tt += f

      ' Get and Handle Input
      For i As Integer = 0 To 1
        If GetAsyncKeyState(AscW((ChrW(&H25) & ChrW(&H27))(i))) <> 0 Then
          c += If(i = 1, 1, -1) * 40.0F * f
        End If
      Next

      ' Update World
      If t >= 1.5F Then
        t -= 1.5F : tw = Rnd() * 10 + 10 : tm = Rnd() * (76 - (CInt(tw) >> 1)) + 4
      End If
      e += f * 0.001F : w += (tw - w) * e * f : m += (tm - m) * e * f
      Dim p = Sin(tt * e) * m / 2.0F + 40.0F

      ' Fill Row
      For x = 0 To 78
        If x + 1 < p - w / 2 OrElse x > p + w / 2 Then
          s(r * 80 + x) = "."c
        ElseIf x = CInt(p - w / 2) OrElse x = CInt(p + w / 2) Then
          s(r * 80 + x) = "#"c
        Else
          s(r * 80 + x) = " "c
        End If
      Next

      ' Scrolling Effect
      s(r * 80 + 79) = ControlChars.NullChar
      r = (r + 1) Mod 28

      ' Draw To Screen
      For y = 1 To 27
        pfxy(ChrW((AscW(s(y + 28 + r)) Mod 28) * 80).ToString, 0, y)
      Next
      pfxy("O-V-O", c - 2, 2)
      pfxy(" O-O ", c - 2, 3)
      pfxy("  V  ", c - 2, 4)

      ' Collision Checking & Health Update
      h -= If(s(((4 + r) Mod 28) * 80 + CInt(c) - 2) = ".", 1, 0)

      ' Draw HUD
      pfxy("===============================================================================", 0, 28)
      pfxy("Cave Diver - www.onelonecoder.com - Left Arrow / Right Arrow - Survive!", 2, 29)
      Dim dist As Integer = CInt(tt * 100.0F)
      pfxy("Distance Fallen:            ", 2, 30)
      pfxy(dist.ToString(), 19, 30)

      Dim nn As String = CInt(tt * 100.0F).ToString("D10")
      pfxy("Health:             ", 40, 30)
      pfxy(nn, 48, 30)

    End While

    pfxy("Dead...\n", 70, 30)
    Console.ReadLine()

    'Return 0

  End Sub
End Module