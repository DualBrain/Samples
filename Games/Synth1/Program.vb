' Inspired by: "Code-It-Yourself! Sound Synthesizer #1" -- @javidx9
' https://youtu.be/tgamhuQnOkM

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

  ' Global synthesizer variables
  Private m_frequencyOutput As Double = 0.0                                   ' dominant output frequency of instrument, i.e. the note
  Private ReadOnly m_octaveBaseFrequency As Double = 110.0                    ' frequency Of octave represented by keyboard
  Private ReadOnly m_12thRootOf2 As Double = Math.Pow(2.0, 1.0 / 12.0)   ' assuming western 12 notes per ocatve

  ' Function used by olcNoiseMaker to generate sound waves
  ' Returns amplitude (-1.0 to +1.0) as a function of time
  Function MakeNoise(time As Double) As Double

    'Return 0.5 * Math.Sin(440.0 * 2 * 3.14159 * time)
    'Return 0.5 * Math.Sin(880.0 * 2 * 3.14159 * time)
    'Return 0.5 * Math.Sin(220.0 * 2 * 3.14159 * time)

    ' Note, the following raw sin wave is create pops/clicks when start/stop and changing frequency.
    ' I suspect I know the problem here; but will hold off going to deep into this as if I listen
    ' very closely to the videos, I think I'm still hearing these (albiet maybe less).
    'Dim output = Math.Sin(dFrequencyOutput * 2.0 * 3.14159 * time)
    'Return output * 0.5 ' Master Volume
    ' However, the following square wave seems to produce it a lot less (or pretty much not at all).
    ' Besides, I kind of like the square wave sound a bit more. ;-)

    ' Turn sin into square wave.
    Dim output = 0.1 * Math.Sin(m_frequencyOutput * 2 * 3.14159 * time)
    Return If(output > 0, 0.2, -0.2)

  End Function

  Sub Main() 'args As String())

    ' Shameless self-promotion
    Console.WriteLine("gotBASIC.com and OneLoneCoder.com - Synthesizer Part 1")
    Console.WriteLine("Single Sine Wave Oscillator, No Polyphony")
    Console.WriteLine()

    ' Get all sound hardware
    Dim devices = olcNoiseMaker(Of Short).Enumerate()

    ' Display findings
    For Each d In devices
      Console.WriteLine($"Found Output Device: {d}")
    Next
    Console.WriteLine($"Using Device: {devices(0)}")

    ' Display a keyboard
    Console.WriteLine()
    Console.WriteLine("|   |   |   |   |   | |   |   |   |   | |   | |   |   |   |")
    Console.WriteLine("|   | S |   |   | F | | G |   |   | J | | K | | L |   |   |")
    Console.WriteLine("|   |___|   |   |___| |___|   |   |___| |___| |___|   |   |__")
    Console.WriteLine("|     |     |     |     |     |     |     |     |     |     |")
    Console.WriteLine("|  Z  |  X  |  C  |  V  |  B  |  N  |  M  |  ,  |  .  |  /  |")
    Console.WriteLine("|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|")
    Console.WriteLine()

    ' Create sound machine!!
    Dim sound As New olcNoiseMaker(Of Short)(devices(0), 44100, 1, 8, 512) ' settings in the original C++ version; doesn't seem to work well enough here.
    'Dim sound As New olcNoiseMaker(Of Short)(devices(0), 44100, 1, 8, 2048) ' 2048 seems to be the minimum for 44100.
    'Dim sound As New olcNoiseMaker(Of Short)(devices(0), 22050, 1, 8, 512) ' 512 seems to be the minimum for 22050.
    'Dim sound As New olcNoiseMaker(Of Short)(devices(0), 11025, 1, 8, 128) ' 128 seems to work for 11025

    ' Link noise function with sound machine
    sound.SetUserFunction(AddressOf MakeNoise)

    ' Sit in loop, capturing keyboard state changes and modify synthesizer output accordingly
    Dim currentKey = -1
    Dim keyPressed = False

    While True

      keyPressed = False

      Dim abort = (GetAsyncKeyState(27) And &H8000) <> 0
      If abort Then
        sound.StopAudio()
        Exit While
      End If

      For k = 0 To 14
        Dim str = "ZSXCFVGBNJMK" & ChrW(&HBC) & ChrW(&HBE) & ChrW(&HBF)
        If (GetAsyncKeyState(AscW(str(k))) And &H8000) <> 0 Then
          If currentKey <> k Then
            m_frequencyOutput = m_octaveBaseFrequency * Math.Pow(m_12thRootOf2, k)
            Console.WriteLine($"Note On : {sound.GetTime}s {m_frequencyOutput}Hz")
            currentKey = k
          End If
          keyPressed = True
        End If
      Next

      If Not keyPressed Then
        If currentKey <> -1 Then
          Console.WriteLine("Note Off: {0}s                        ", sound.GetTime())
          currentKey = -1
        End If
        m_frequencyOutput = 0.0
      End If

    End While

    'Return 0

  End Sub

End Module