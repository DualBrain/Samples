' Inspired by: "Code-It-Yourself! Sound Synthesizer #4 - Waveout API, Sequencing & Ducktales" -- @javidx9
' https://youtu.be/roRH3PdTajs

Option Explicit On
Option Strict On
Option Infer On

Imports System.Runtime.InteropServices

Friend Module Synth

  '//////////////////////////////////////////////////////////////////////////////
  ' Utilities

  ' Converts frequency (Hz) to angular velocity
  Public Function W(hertz As Double) As Double
    Return hertz * 2.0 * Math.PI
  End Function

  ' A basic note
  Public Class Note

    Public Property Id As Integer
    Public Property [On] As Double
    Public Property [Off] As Double
    Public Property Active As Boolean
    Public Property Channel As InstrumentBase

    Public Sub New()
    End Sub

    Public Sub New(id As Integer, [on] As Double, [off] As Double, active As Boolean, channel As InstrumentBase)
      Me.Id = id
      Me.On = [on]
      Me.Off = [off]
      Me.Active = active
      Me.Channel = channel
    End Sub

  End Class

#Region "Helper methods that are copied out of ConsoleGameEngine"

  Private ReadOnly m_random As New Random
  Friend Const PI As Single = Math.PI '3.14159
  Friend Const RAND_MAX As Integer = 2147483647

  Friend ReadOnly Property Rnd As Double
    Get
      Return m_random.NextDouble
    End Get
  End Property

  ' Provide for something *similar* to C++.
  Friend ReadOnly Property Rand As Integer
    Get
      Return CInt(Fix(m_random.NextDouble * RAND_MAX))
    End Get
  End Property

#End Region

  '//////////////////////////////////////////////////////////////////////////////
  ' Multi-Function Oscillator

  Public Const OSC_SINE As Integer = 0
  Public Const OSC_SQUARE As Integer = 1
  Public Const OSC_TRIANGLE As Integer = 2
  Public Const OSC_SAW_ANA As Integer = 3
  Public Const OSC_SAW_DIG As Integer = 4
  Public Const OSC_NOISE As Integer = 5

  Public Function Osc(time As Double, hertz As Double, Optional type As Integer = OSC_SINE, Optional lfoHertz As Double = 0.0, Optional lfoAmplitude As Double = 0.0, Optional custom As Integer = 50) As Double

    Dim freq = W(hertz) * time + lfoAmplitude * hertz * (Math.Sin(W(lfoHertz) * time)) ' osc(dTime, dLFOHertz, OSC_SINE)

    Select Case type
      Case OSC_SINE ' Sine wave bewteen -1 and +1
        Return Math.Sin(freq)

      Case OSC_SQUARE ' Square wave between -1 and +1
        Return If(Math.Sin(freq) > 0, 1.0, -1.0)

      Case OSC_TRIANGLE ' Triangle wave between -1 and +1
        Return Math.Asin(Math.Sin(freq)) * (2.0 / PI)

      Case OSC_SAW_ANA ' Saw wave (analogue / warm / slow)
        Dim output = 0.0#
        For n = 1.0# To custom - 1 Step 1.0
          output += (Math.Sin(n * freq)) / n
        Next
        Return output * (2.0 / PI)

      Case OSC_SAW_DIG
        Return (2.0 / PI) * (hertz * PI * (time Mod (1.0 / hertz)) - (PI / 2.0))

      Case OSC_NOISE
        Return 2.0 * (Rand / RAND_MAX) - 1.0

      Case Else
        Return 0.0

    End Select

  End Function

  '//////////////////////////////////////////////////////////////////////////////
  ' Scale to Frequency conversion

  Public Const SCALE_DEFAULT As Integer = 0

  Public Function Scale(noteId As Integer, Optional scaleId As Integer = SCALE_DEFAULT) As Double
    Select Case scaleId
      Case Else
        Return 8 * Math.Pow(1.0594630943592953, noteId)
    End Select
  End Function

  '///////////////////////////////////////////////////////////////////////////////
  ' Envelopes

  Public MustInherit Class Envelope

    Public MustOverride Function Amplitude(time As Double, ByVal dTimeOn As Double, ByVal dTimeOff As Double) As Double

  End Class

  Public Class EnvelopeADSR
    Inherits Envelope

    Public Property AttackTime As Double
    Public Property DecayTime As Double
    Public Property SustainAmplitude As Double
    Public Property ReleaseTime As Double
    Public Property StartAmplitude As Double

    Public Sub New()
      AttackTime = 0.1
      DecayTime = 0.1
      SustainAmplitude = 1.0
      ReleaseTime = 0.2
      StartAmplitude = 1.0
    End Sub

    Public Overrides Function Amplitude(time As Double, timeOn As Double, timeOff As Double) As Double

      Dim result = 0.0#
      Dim releaseAmplitude = 0.0#

      If (timeOn > timeOff) Then ' Note is on

        Dim lifeTime = time - timeOn

        If (lifeTime <= AttackTime) Then
          result = (lifeTime / AttackTime) * StartAmplitude
        End If

        If (lifeTime > AttackTime AndAlso lifeTime <= (AttackTime + DecayTime)) Then
          result = ((lifeTime - AttackTime) / DecayTime) * (SustainAmplitude - StartAmplitude) + StartAmplitude
        End If

        If (lifeTime > (AttackTime + DecayTime)) Then
          result = SustainAmplitude
        End If

      Else ' Note is off

        Dim lifeTime = timeOff - timeOn

        If (lifeTime <= AttackTime) Then
          releaseAmplitude = (lifeTime / AttackTime) * StartAmplitude
        End If

        If (lifeTime > AttackTime AndAlso lifeTime <= (AttackTime + DecayTime)) Then
          releaseAmplitude = ((lifeTime - AttackTime) / DecayTime) * (SustainAmplitude - StartAmplitude) + StartAmplitude
        End If

        If (lifeTime > (AttackTime + DecayTime)) Then
          releaseAmplitude = SustainAmplitude
        End If

        result = ((time - timeOff) / ReleaseTime) * (0.0 - releaseAmplitude) + releaseAmplitude

      End If

      ' Amplitude should not be negative
      If (result <= 0.000) OrElse Double.IsNaN(result) Then result = 0.0

      Return result

    End Function

  End Class

  Public Function Env(time As Double, envAdsr As EnvelopeADSR, timeOn As Double, timeOff As Double) As Double
    Return envAdsr.Amplitude(time, timeOn, timeOff)
  End Function

  Public MustInherit Class InstrumentBase
    Public Property Volume As Double
    Public Property Env As New EnvelopeADSR
    Public Property MaxLifeTime As Double
    Public Property Name As String
    Public MustOverride Function Sound(time As Double, n As Note, ByRef noteFinished As Boolean) As Double
  End Class

  Public Class InstrumentBell
    Inherits InstrumentBase

    Public Sub New()
      Env.AttackTime = 0.01
      Env.DecayTime = 1.0
      Env.SustainAmplitude = 0.0
      Env.ReleaseTime = 1.0
      MaxLifeTime = 3.0
      Name = "Bell"
      Volume = 1.0
    End Sub

    Public Overrides Function Sound(time As Double, n As Note, ByRef noteFinished As Boolean) As Double

      Dim amplitude = Synth.Env(time, Env, n.On, n.Off)

      If amplitude <= 0.0 Then noteFinished = True

      Dim result = 1.0 * Osc(n.On - time, Scale(n.Id + 12), OSC_SINE, 5.0, 0.001) +
                   0.5 * Osc(n.On - time, Scale(n.Id + 24)) +
                   0.25 * Osc(n.On - time, Scale(n.Id + 36))

      Return amplitude * result * Volume

    End Function
  End Class

  Public Class InstrumentBell8
    Inherits InstrumentBase

    Public Sub New()
      Env.AttackTime = 0.01
      Env.DecayTime = 0.5
      Env.SustainAmplitude = 0.8
      Env.ReleaseTime = 1.0
      MaxLifeTime = 3.0
      Name = "8-Bit Bell"
      Volume = 1.0
    End Sub

    Public Overrides Function Sound(time As Double, n As Note, ByRef noteFinished As Boolean) As Double

      Dim amplitude = Synth.Env(time, Env, n.On, n.Off)

      If amplitude <= 0.0 Then noteFinished = True

      Dim result = 1.0 * Osc(n.On - time, Scale(n.Id), OSC_SQUARE, 5.0, 0.001) +
                   0.5 * Osc(n.On - time, Scale(n.Id + 12)) +
                   0.25 * Osc(n.On - time, Scale(n.Id + 24))

      Return amplitude * result * Volume

    End Function

  End Class

  Public Class InstrumentHarmonica
    Inherits InstrumentBase

    Public Sub New()
      Env.AttackTime = 0.00
      Env.DecayTime = 1.0
      Env.SustainAmplitude = 0.95
      Env.ReleaseTime = 0.1
      MaxLifeTime = -1.0
      Name = "Harmonica"
      Volume = 0.3
    End Sub

    Public Overrides Function Sound(time As Double, n As Note, ByRef noteFinished As Boolean) As Double

      Dim amplitude = Synth.Env(time, Env, n.On, n.Off)

      If amplitude <= 0.0 Then noteFinished = True

      Dim result = 1.0 * Osc(n.On - time, Scale(n.Id - 12), OSC_SAW_ANA, 5.0, 0.001, 100) +
                   1.0 * Osc(time - n.On, Scale(n.Id), OSC_SQUARE, 5.0, 0.001) +
                   0.5 * Osc(time - n.On, Scale(n.Id + 12), OSC_SQUARE) +
                   0.05 * Osc(time - n.On, Scale(n.Id + 24), OSC_NOISE)

      Return amplitude * result * Volume

    End Function

  End Class

  Public Class InstrumentDrumKick
    Inherits InstrumentBase

    Public Sub New()
      Env.AttackTime = 0.01
      Env.DecayTime = 0.15
      Env.SustainAmplitude = 0.0
      Env.ReleaseTime = 0.0
      MaxLifeTime = 1.5
      Name = "Drum Kick"
      Volume = 1.0
    End Sub

    Public Overrides Function Sound(time As Double, n As Note, ByRef noteFinished As Boolean) As Double

      Dim amplitude = Synth.Env(time, Env, n.On, n.Off)

      If MaxLifeTime > 0.0 AndAlso time - n.On >= MaxLifeTime Then
        noteFinished = True
      End If

      Dim result = 0.99 * Osc(time - n.On, Scale(n.Id - 36), OSC_SINE, 1.0, 1.0) +
                   0.01 * Osc(time - n.On, 0, OSC_NOISE)

      Return amplitude * result * Volume

    End Function

  End Class

  Public Class InstrumentDrumSnare
    Inherits InstrumentBase

    Public Sub New()
      Env.AttackTime = 0.0
      Env.DecayTime = 0.2
      Env.SustainAmplitude = 0.0
      Env.ReleaseTime = 0.0
      MaxLifeTime = 1.0
      Name = "Drum Snare"
      Volume = 1.0
    End Sub

    Public Overrides Function Sound(time As Double, n As Note, ByRef noteFinished As Boolean) As Double

      Dim amplitude = Synth.Env(time, Env, n.On, n.Off)

      If MaxLifeTime > 0.0 AndAlso time - n.On >= MaxLifeTime Then
        noteFinished = True
      End If

      Dim result = 0.5 * Osc(time - n.On, Scale(n.Id - 24), OSC_SINE, 0.5, 1.0) +
                   0.5 * Osc(time - n.On, 0, OSC_NOISE)

      Return amplitude * result * Volume

    End Function

  End Class

  Public Class InstrumentDrumHiHat
    Inherits InstrumentBase

    Public Sub New()
      Env.AttackTime = 0.01
      Env.DecayTime = 0.05
      Env.SustainAmplitude = 0.0
      Env.ReleaseTime = 0.0
      MaxLifeTime = 1.0
      Name = "Drum HiHat"
      Volume = 0.5
    End Sub

    Public Overrides Function Sound(time As Double, n As Note, ByRef noteFinished As Boolean) As Double

      Dim amplitude = Synth.Env(time, Env, n.On, n.Off)

      If MaxLifeTime > 0.0 AndAlso time - n.On >= MaxLifeTime Then
        noteFinished = True
      End If

      Dim result = 0.1 * Synth.Osc(time - n.On, Synth.Scale(n.Id - 12), Synth.OSC_SQUARE, 1.5, 1) +
                   0.9 * Synth.Osc(time - n.On, 0, Synth.OSC_NOISE)

      Return amplitude * result * Volume

    End Function

  End Class

  Public Class Sequencer

    Public Class Channel
      Public Property Instrument As InstrumentBase
      Public Property Beat As String
    End Class

    Public Property Beats As Integer
    Public Property SubBeats As Integer
    Public Property Tempo As Single
    Public Property BeatTime As Single
    Public Property Accumulate As Double
    Public Property CurrentBeat As Integer
    Public Property TotalBeats As Integer
    Public Property Channels As New List(Of Channel)
    Public Property Notes As New List(Of Note)

    Public Sub New(Optional tempo As Single = 120.0F, Optional beats As Integer = 4, Optional subBeats As Integer = 4)
      Me.Beats = beats
      Me.SubBeats = subBeats
      Me.Tempo = tempo
      BeatTime = (60.0F / Me.Tempo) / Me.SubBeats
      CurrentBeat = 0
      TotalBeats = Me.SubBeats * Me.Beats
      Accumulate = 0
    End Sub

    Public Function Update(elapsedTime As Double) As Integer

      Notes.Clear()

      Accumulate += elapsedTime

      While Accumulate >= BeatTime

        Accumulate -= BeatTime
        CurrentBeat += 1

        If CurrentBeat >= TotalBeats Then
          CurrentBeat = 0
        End If

        For c = 0 To Channels.Count - 1
          If Channels(c).Beat.Chars(CurrentBeat) = "X"c Then
            Notes.Add(New Note() With {.Id = 64, .Active = True, .Channel = Channels(c).Instrument})
          End If
        Next

      End While

      Return Notes.Count

    End Function

    Public Sub AddInstrument(inst As InstrumentBase)
      Channels.Add(New Channel With {.Instrument = inst})
    End Sub

  End Class

End Module

Module Program

#Region "Win32"

  <StructLayout(LayoutKind.Sequential)>
  Private Structure SMALL_RECT
    Public Left As Short
    Public Top As Short
    Public Right As Short
    Public Bottom As Short
    Public Sub New(left As Short, top As Short, right As Short, bottom As Short)
      Me.Left = left
      Me.Top = top
      Me.Right = right
      Me.Bottom = bottom
    End Sub
  End Structure

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
  Private Declare Function WriteConsoleOutputCharacter Lib "kernel32.dll" Alias "WriteConsoleOutputCharacterA" (hConsoleOutput As IntPtr, lpCharacter As String, nLength As UInteger, dwWriteCoord As Coord, ByRef lpNumberOfCharsWritten As Integer) As Boolean
  Private Declare Function GetAsyncKeyState Lib "user32.dll" (virtualKeyCode As Integer) As Short
  Private Declare Function CloseHandle Lib "kernel32.dll" (hObject As Long) As Long
  Private Declare Function SetConsoleWindowInfo Lib "kernel32" (hConsoleOutput As IntPtr, bAbsolute As Boolean, ByRef lpConsoleWindow As SMALL_RECT) As Boolean
  Private Declare Function SetConsoleScreenBufferSize Lib "kernel32" (hConsoleOutput As IntPtr, dwSize As Coord) As Boolean

  Private Const GENERIC_READ As Integer = &H80000000
  Private Const GENERIC_WRITE As Integer = &H40000000
  Private Const CONSOLE_TEXTMODE_BUFFER As Integer = 1

#End Region

  Private ReadOnly m_vecNotes As New List(Of Note)
  'Private ReadOnly m_instBell As New InstrumentBell()
  Private ReadOnly m_instHarm As New InstrumentHarmonica()
  Private ReadOnly m_instDrumKick As New InstrumentDrumKick()
  Private ReadOnly m_instDrumSnare As New InstrumentDrumSnare()
  Private ReadOnly m_instDrumHiHat As New InstrumentDrumHiHat()

  Private m_mixedOutput As Double
  Private m_makeNoiseIndex As Integer
  Private m_noteFinished As Boolean

  Private Function MakeNoise(time As Double) As Double

    m_mixedOutput = 0.0#

    ' Iterate through all active notes, and mix together
    For m_makeNoiseIndex = m_vecNotes.Count - 1 To 0 Step -1
      Try
        Dim entry = m_vecNotes(m_makeNoiseIndex)
        If entry Is Nothing Then Continue For
        'For Each entry In m_vecNotes
        m_noteFinished = False
        ' Get sample for this note by using the correct instrument and envelope
        If entry.Channel IsNot Nothing Then
          ' Mix into output
          m_mixedOutput += entry.Channel.Sound(time, entry, m_noteFinished)
        End If
        If m_noteFinished Then entry.Active = False ' Flag note to be removed
      Catch
        Continue For ' Hack to handle removal of entry...
      End Try
    Next

    Return m_mixedOutput * 0.2

  End Function

  Sub Main()

    ' Shameless self-promotion
    Console.WriteLine("gotBASIC.com and OneLoneCoder.com - Synthesizer Part 4")
    Console.WriteLine("Multiple FM Oscillators, Sequencing, Polyphony")
    Console.WriteLine()

    ' Get all sound hardware
    Dim devices = olcNoiseMaker(Of Short).Enumerate()

    ' Create sound machine!!
    Dim sound As New olcNoiseMaker(Of Short)(devices(0), 44100, 1, 8, 256)

    ' Link noise function with sound machine
    sound.SetUserFunction(AddressOf MakeNoise)

    ' Setup the screen...
    Dim screen((80 * 30) - 1) As Char
    Dim hConsole As IntPtr = CreateConsoleScreenBuffer(GENERIC_READ Or GENERIC_WRITE, 0, IntPtr.Zero, CONSOLE_TEXTMODE_BUFFER, IntPtr.Zero)
    Dim rect = New SMALL_RECT With {.Left = 0, .Top = 0, .Right = 1, .Bottom = 1}
    SetConsoleWindowInfo(hConsole, True, rect)
    Dim coord As Coord
    coord.X = 80
    coord.Y = 30
    If Not SetConsoleScreenBufferSize(hConsole, coord) Then
      Stop
    End If
    SetConsoleActiveScreenBuffer(hConsole)
    rect = New SMALL_RECT(0, 0, 80 - 1, 30 - 1)
    If Not SetConsoleWindowInfo(hConsole, True, rect) Then
      Stop
    End If
    Dim dwBytesWritten = 0

    ' Lambda function to draw into the character array conveniently
    Dim draw = Sub(x As Integer, y As Integer, s As String)
                 For ii = 0 To s.Length - 1
                   screen(y * 80 + x + ii) = s(ii)
                 Next
               End Sub

    Dim clock_old_time = Stopwatch.GetTimestamp ' DateAndTime.Now
    Dim clock_real_time = Stopwatch.GetTimestamp 'DateAndTime.Now
    Dim elapsedTime = 0.0#
    Dim dWallTime = 0.0#

    ' Establish Sequencer
    Dim seq As New Sequencer(90.0)
    seq.AddInstrument(m_instDrumKick)
    seq.AddInstrument(m_instDrumSnare)
    seq.AddInstrument(m_instDrumHiHat)

    seq.Channels(0).Beat = "X...X...X..X.X.."
    seq.Channels(1).Beat = "..X...X...X...X."
    seq.Channels(2).Beat = "X.X.X.X.X.X.X.XX"

    Dim keys = "ZSXCFVGBNJMK" & ChrW(&HBC) & ChrW(&HBE) & ChrW(&HBF)

    Dim time_last_loop As Long
    Dim timeNow As Double
    Dim noteCount As Integer
    Dim k As Integer
    Dim keyState As Integer
    Dim kk As Integer
    Dim noteIndex As Integer
    Dim foundIndex As Integer
    Dim a As Integer
    Dim i As Integer
    Dim beats As Integer
    Dim subbeats As Integer
    Dim n As Integer
    Dim v As Sequencer.Channel
    Dim abort As Boolean

    Do

      ' --- SOUND STUFF ---

      ' Update Timings ======================================================================================
      clock_real_time = Stopwatch.GetTimestamp()
      time_last_loop = clock_real_time - clock_old_time
      clock_old_time = clock_real_time
      elapsedTime = time_last_loop / Stopwatch.Frequency
      dWallTime += elapsedTime

      timeNow = sound.GetTime()

      ' Sequencer (generates notes, note offs applied by note lifespan) ======================================
      noteCount = seq.Update(elapsedTime)
      For a = 0 To noteCount - 1
        seq.Notes(a).On = timeNow
        m_vecNotes.Add(seq.Notes(a))
      Next

      ' Keyboard (generates and removes notes depending on key state) ========================================

      abort = (GetAsyncKeyState(27) And &H8000) <> 0
      If abort Then
        sound.StopAudio()
        Exit Do
      End If

      For k = 0 To 14

        keyState = GetAsyncKeyState(AscW(keys.Chars(k)))

        ' Check if note already exists in currently playing notes
        kk = k + 65

        ' See if the current note is already in the list of playing notes...
        foundIndex = -1
        For noteIndex = m_vecNotes.Count - 1 To 0 Step -1
          If m_vecNotes(noteIndex).Id = kk Then 'AndAlso m_vecNotes(noteIndex) Is m_instHarm Then
            foundIndex = noteIndex : Exit For
          End If
        Next

        If foundIndex = -1 Then
          ' Note not found in vector
          If (keyState And &H8000) <> 0 Then
            ' Key has been pressed so create a new note, add note to vector
            m_vecNotes.Add(New Note(kk, timeNow, 0, True, m_instHarm))
          End If
        Else
          ' Note exists in vector
          If (keyState And &H8000) <> 0 Then
            ' Key is still held, so do nothing
            If m_vecNotes(foundIndex).Off > m_vecNotes(foundIndex).On Then
              ' Key has been pressed again during release phase
              m_vecNotes(foundIndex).On = timeNow
              m_vecNotes(foundIndex).Active = True
            End If
          Else
            ' Key has been released, so switch off
            If m_vecNotes(foundIndex).Off < m_vecNotes(foundIndex).On Then
              m_vecNotes(foundIndex).Off = timeNow
            End If
          End If
        End If

      Next

      For index = m_vecNotes.Count - 1 To 0 Step -1
        If Not m_vecNotes(index).Active Then
          m_vecNotes(index) = Nothing
          m_vecNotes.RemoveAt(index)
          Exit For
        End If
      Next

      ' --- VISUAL STUFF ---

      ' Clear Background
      For i = 0 To 80 * 30 - 1
        screen(i) = " "c
      Next

      ' Draw Sequencer
      draw(2, 2, "SEQUENCER:")
      For beats = 0 To seq.Beats - 1
        draw(beats * seq.SubBeats + 20, 2, "O"c)
        For subbeats = 1 To seq.SubBeats - 1
          draw(beats * seq.SubBeats + subbeats + 20, 2, "."c)
        Next
      Next

      ' Draw Sequences
      n = 0
      For Each v In seq.Channels
        draw(2, 3 + n, v.Instrument.Name)
        draw(20, 3 + n, v.Beat)
        n += 1
      Next

      ' Draw Beat Cursor
      draw(20 + seq.CurrentBeat, 1, "|"c)

      ' Draw Keyboard
      draw(2, 8, "|   |   |   |   |   | |   |   |   |   | |   | |   |   |   |  ")
      draw(2, 9, "|   | S |   |   | F | | G |   |   | J | | K | | L |   |   |  ")
      draw(2, 10, "|   |___|   |   |___| |___|   |   |___| |___| |___|   |   |__")
      draw(2, 11, "|     |     |     |     |     |     |     |     |     |     |")
      draw(2, 12, "|  Z  |  X  |  C  |  V  |  B  |  N  |  M  |  ,  |  .  |  /  |")
      draw(2, 13, "|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|")

      ' Draw Stats - puts a significant amount of pressure on the GC...
      'Dim stats = $"Notes: {m_vecNotes.Count} Wall Time: {dWallTime:N4} CPU Time: {dTimeNow:N4} Latency: {(dWallTime - dTimeNow):N4}"
      'draw(2, 15, stats)

      ' Update Display - puts a lot of pressure on the GC...
      'WriteConsoleOutputCharacter(hConsole, screen, 80 * 30, New Coord(0, 0), dwBytesWritten)

      Console.Title = m_vecNotes.Count.ToString

    Loop

  End Sub

End Module