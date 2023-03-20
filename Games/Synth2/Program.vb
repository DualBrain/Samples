' Inspired by: "Code-It-Yourself! Sound Synthesizer #2 - Oscillators & Envelopes" -- @javidx9
' https://youtu.be/OSCzKOqtgcA

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

#Region "Helper methods that are copied out of ConsoleGameEngine"

  Private ReadOnly m_random As New Random
  Private Const PI As Single = Math.PI '3.14159
  Private Const RAND_MAX As Integer = 2147483647

  Private ReadOnly Property Rnd As Double
    Get
      Return m_random.NextDouble
    End Get
  End Property

  ' Provide for something *similar* to C++.
  Private ReadOnly Property Rand As Integer
    Get
      Return CInt(Fix(m_random.NextDouble * RAND_MAX))
    End Get
  End Property

#End Region

#Region "Introduced in Part 2"

  Private Function W(hertz As Double) As Double
    Return hertz * 2.0 * PI
  End Function

  Private Const OSC_SINE = 0
  Private Const OSC_SQUARE = 1
  Private Const OSC_TRIANGLE = 2
  Private Const OSC_SAW_ANA = 3
  Private Const OSC_SAW_DIG = 4
  Private Const OSC_NOISE = 5

  Private m_oscN As Double
  Private m_oscOutput As Double

  Private Function Osc(hertz As Double, time As Double, Optional type As Integer = OSC_SINE) As Double

    Select Case type

      Case OSC_SINE ' Sine wave bewteen -1 and +1
        Return Math.Sin(W(hertz) * time)

      Case OSC_SQUARE ' Square wave between -1 and +1
        Return If(Math.Sin(W(hertz) * time) > 0, 1.0, -1.0)

      Case OSC_TRIANGLE ' Triangle wave between -1 and +1
        Return Math.Asin(Math.Sin(W(hertz) * time)) * (2.0 / PI)

      Case OSC_SAW_ANA ' Saw wave (analogue / warm / slow)
        m_oscOutput = 0.0#
        For m_oscN = 1.0 To 39.0
          m_oscOutput += (Math.Sin(m_oscN * W(hertz) * time)) / m_oscN
        Next
        Return m_oscOutput * (2.0 / PI)

      Case OSC_SAW_DIG ' Saw Wave (optimised / harsh / fast)
        Return (2.0 / PI) * (hertz * PI * (time Mod (1.0 / hertz)) - (PI / 2.0))

      Case OSC_NOISE ' Pseudorandom noise
        Return 2.0 * (Rand / RAND_MAX - 1.0)

      Case Else
        Return 0.0

    End Select

  End Function

  ' Amplitude (Attack, Decay, Sustain, Release) Envelope
  Private Class EnvelopeADSR

    Public AttackTime As Double
    Public DecayTime As Double
    Public SustainAmplitude As Double
    Public ReleaseTime As Double
    Public StartAmplitude As Double
    Public TriggerOffTime As Double
    Public TriggerOnTime As Double
    Public bNoteOn As Boolean

    Public Sub New()
      AttackTime = 0.1
      DecayTime = 0.01
      StartAmplitude = 1.0
      SustainAmplitude = 0.8
      ReleaseTime = 0.2
      bNoteOn = False
      TriggerOffTime = 0.0
      TriggerOnTime = 0.0
    End Sub

    ' Call when key is pressed
    Public Sub NoteOn(timeOn As Double)
      TriggerOnTime = timeOn
      bNoteOn = True
    End Sub

    ' Call when key is released
    Public Sub NoteOff(timeOff As Double)
      TriggerOffTime = timeOff
      bNoteOn = False
    End Sub

    Private m_amplitude As Double
    Private m_lifeTime As Double

    ' Get the correct amplitude at the requested point in time
    Public Function GetAmplitude(time As Double) As Double

      m_amplitude = 0.0#
      m_lifeTime = time - TriggerOnTime

      If bNoteOn Then

        If m_lifeTime <= AttackTime Then
          ' In attack Phase - approach max amplitude
          m_amplitude = (m_lifeTime / AttackTime) * StartAmplitude
        End If

        If m_lifeTime > AttackTime AndAlso m_lifeTime <= (AttackTime + DecayTime) Then
          ' In decay phase - reduce to sustained amplitude
          m_amplitude = ((m_lifeTime - AttackTime) / DecayTime) * (SustainAmplitude - StartAmplitude) + StartAmplitude
        End If

        If m_lifeTime > (AttackTime + DecayTime) Then
          ' In sustain phase - dont change until note released
          m_amplitude = SustainAmplitude
        End If

      Else
        ' Note has been released, so in release phase
        m_amplitude = ((time - TriggerOffTime) / ReleaseTime) * (0.0 - SustainAmplitude) + SustainAmplitude
      End If

      ' Amplitude should not be negative
      If m_amplitude <= 0.0001 Then m_amplitude = 0.0

      Return m_amplitude

    End Function

  End Class

  Private ReadOnly m_envelope As New EnvelopeADSR

#End Region

  ' Global synthesizer variables
  Private m_frequencyOutput As Double = 0.0                                   ' dominant output frequency of instrument, i.e. the note
  Private ReadOnly m_octaveBaseFrequency As Double = 110.0                    ' A2 frequency Of octave represented by keyboard
  Private ReadOnly m_12thRootOf2 As Double = Math.Pow(2.0, 1.0 / 12.0)   ' assuming western 12 notes per ocatve

  Private m_makeNoiseOutput As Double

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
    'Dim output = 0.1 * Math.Sin(m_frequencyOutput * 2 * 3.14159 * time)
    'Return If(output > 0, 0.2, -0.2)

    ' A cord?
    'Dim output = 1.0 * Math.Sin(m_frequencyOutput * 2 * 3.14159 * time) + Math.Sin((m_frequencyOutput + 20) * 2.0 * 3.14159 * time)
    'Return output * 0.4

    ' -----------------------
    ' Introduced in Part 2...
    ' -----------------------

    ' Now use Osc...
    'Dim output = Osc(m_frequencyOutput, time, 1)
    'Return output * 0.4 ' Master Volume

    ' Mix together a little sine And square waves
    m_makeNoiseOutput = m_envelope.GetAmplitude(time) * (1.0 * Osc(m_frequencyOutput * 0.5, time, OSC_SINE) + 1.0 * Osc(m_frequencyOutput, time, OSC_SAW_ANA))
    Return m_makeNoiseOutput * 0.4 ' Master Volume

  End Function

  Sub Main() 'args As String())

    ' Shameless self-promotion
    Console.WriteLine("gotBASIC.com and OneLoneCoder.com - Synthesizer Part 2")
    Console.WriteLine("Multiple Oscillators with Single Amplitude Envelope, No Polyphony")
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
    Dim abort = False
    Dim keys = "ZSXCFVGBNJMK" & ChrW(&HBC) & ChrW(&HBE) & ChrW(&HBF)
    Dim k = 0
    Dim row = 0

    While True

      keyPressed = False

      abort = (GetAsyncKeyState(27) And &H8000) <> 0
      If abort Then
        sound.StopAudio()
        Exit While
      End If

      For k = 0 To 14
        If (GetAsyncKeyState(AscW(keys(k))) And &H8000) <> 0 Then
          If currentKey <> k Then
            m_frequencyOutput = m_octaveBaseFrequency * Math.Pow(m_12thRootOf2, k)
            m_envelope.NoteOn(sound.GetTime) ' <--- introduced in Part 2
            'row = Console.CursorTop : Console.WriteLine($"Note On : {sound.GetTime}s {m_frequencyOutput}Hz") : Console.CursorTop = row
            currentKey = k
          End If
          keyPressed = True
        End If
      Next

      If Not keyPressed Then

        If currentKey <> -1 Then
          'row = Console.CursorTop : Console.WriteLine($"Note Off: {sound.GetTime}s                        ") : Console.CursorTop = row
          m_envelope.NoteOff(sound.GetTime) ' <--- introduced in Part 2
          currentKey = -1
        End If
        m_frequencyOutput = 0.0

        While Console.KeyAvailable : Console.ReadKey(True) : End While ' Flush 'Console' key buffer.

      End If

    End While

  End Sub

End Module