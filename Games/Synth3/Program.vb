' Inspired by: "Code-It-Yourself! Sound Synthesizer #3 - Oscillators & Envelopes" -- @javidx9
' https://youtu.be/

Option Explicit On
Option Strict On
Option Infer On

Imports System.Data
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.JavaScript
Imports System.Threading
Imports Synth3.Synth

Friend Module Synth

  '//////////////////////////////////////////////////////////////////////////////
  ' Utilities

  ' Converts frequency (Hz) to angular velocity
  Public Function W(hertz As Double) As Double
    Return hertz * 2.0 * Math.PI
  End Function

  ' A basic note
  Public Class Note

    Public Id As Integer
    Public [On] As Double
    Public [Off] As Double
    Public Active As Boolean
    Public Channel As Integer

    Public Sub New(id As Integer, [on] As Double, [off] As Double, active As Boolean, channel As Integer)
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

  Public Function Osc(time As Double, hertz As Double, Optional type As Integer = OSC_SINE, Optional lfoHertz As Double = 0.0, Optional lfoAmplitude As Double = 0.0, Optional custom As Double = 50.0) As Double

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
        Dim n = 1.0#
        Do While n < custom
          output += (Math.Sin(n * freq)) / n
          n += 1.0
        Loop
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
      'Case SCALE_DEFAULT
      '  Return 256 * Math.Pow(1.0594630943592953, nNoteID)
      Case Else
        Return 256 * Math.Pow(1.0594630943592953, noteId)
    End Select
  End Function

  '///////////////////////////////////////////////////////////////////////////////
  ' Envelopes

  Public MustInherit Class Envelope

    Public MustOverride Function Amplitude(time As Double, ByVal dTimeOn As Double, ByVal dTimeOff As Double) As Double

  End Class

  Public Class EnvelopeADSR
    Inherits Envelope

    Public AttackTime As Double
    Public DecayTime As Double
    Public SustainAmplitude As Double
    Public ReleaseTime As Double
    Public StartAmplitude As Double

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
      If (result <= 0.000) Then result = 0.0

      Return result

    End Function

  End Class

  Public Function Env(time As Double, envAdsr As EnvelopeADSR, timeOn As Double, timeOff As Double) As Double
    Return envAdsr.Amplitude(time, timeOn, timeOff)
  End Function

  Public MustInherit Class InstrumentBase
    Public Volume As Double
    Public Env As New EnvelopeADSR
    Public MustOverride Function Sound(time As Double, n As Note, ByRef noteFinished As Boolean) As Double
  End Class

  Public Class InstrumentBell
    Inherits InstrumentBase

    Public Sub New()
      Env.AttackTime = 0.01
      Env.DecayTime = 1.0
      Env.SustainAmplitude = 0.0
      Env.ReleaseTime = 1.0
      Volume = 1.0
    End Sub

    Public Overrides Function Sound(time As Double, n As Note, ByRef noteFinished As Boolean) As Double

      Dim amplitude = Synth.Env(time, Env, n.On, n.Off)

      If amplitude <= 0.0 Then noteFinished = True

      Dim result = 1.0 * Synth.Osc(n.On - time, Scale(n.Id + 12), Synth.OSC_SINE, 5.0, 0.001) +
                   0.5 * Synth.Osc(n.On - time, Scale(n.Id + 24)) +
                   0.25 * Synth.Osc(n.On - time, Scale(n.Id + 36))

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
      Volume = 1.0
    End Sub

    Public Overrides Function Sound(time As Double, n As Note, ByRef noteFinished As Boolean) As Double

      Dim amplitude = Synth.Env(time, Env, n.On, n.Off)

      If amplitude <= 0.0 Then noteFinished = True

      Dim result = 1.0 * Synth.Osc(n.On - time, Synth.Scale(n.Id), Synth.OSC_SQUARE, 5.0, 0.001) +
                   0.5 * Synth.Osc(n.On - time, Synth.Scale(n.Id + 12)) +
                   0.25 * Synth.Osc(n.On - time, Synth.Scale(n.Id + 24))

      Return amplitude * result * Volume

    End Function

  End Class

  Public Class InstrumentHarmonica
    Inherits InstrumentBase

    Public Sub New()
      Env.AttackTime = 0.05
      Env.DecayTime = 1.0
      Env.SustainAmplitude = 0.95
      Env.ReleaseTime = 0.1
      Volume = 1.0
    End Sub
    Public Overrides Function Sound(time As Double, n As Note, ByRef noteFinished As Boolean) As Double

      Dim amplitude = Synth.Env(time, Env, n.On, n.Off)

      If amplitude <= 0.0 Then noteFinished = True

      Dim result = 1.0 * Osc(n.On - time, Scale(n.Id), Synth.OSC_SQUARE, 5.0, 0.001) +
                   0.5 * Osc(n.On - time, Scale(n.Id + 12), Synth.OSC_SQUARE) +
                   0.05 * Osc(n.On - time, Scale(n.Id + 24), Synth.OSC_NOISE)

      Return amplitude * result * Volume

    End Function

  End Class

End Module

Module Program

#Region "Win32"

  Private Declare Function GetAsyncKeyState Lib "user32.dll" (virtualKeyCode As Integer) As Short

#End Region

  Private ReadOnly m_vecNotes As New List(Of Synth.Note)
  Private ReadOnly m_instBell As New Synth.InstrumentBell()
  Private ReadOnly m_instHarm As New Synth.InstrumentHarmonica()

  Private ReadOnly m_syncObject As New Object

  Private Function MakeNoise(channel As Integer, time As Double) As Double

    Dim mixedOutput = 0.0#

    'mixedOutput = Math.Sin(W(440.0) * time + 0.05 * 440.0 * Math.Sin(W(1.0) * time))

    SyncLock m_syncObject

      For index = m_vecNotes.Count - 1 To 0 Step -1
        Dim entry = m_vecNotes(index)
        'For Each entry In m_vecNotes
        Dim noteFinished = False
        Dim sound = 0.0#
        If entry?.Channel = 2 Then sound = m_instBell.Sound(time, entry, noteFinished)
        If entry?.Channel = 1 Then sound = m_instHarm.Sound(time, entry, noteFinished) * 0.5
        mixedOutput += sound
        If noteFinished AndAlso entry?.Off > entry?.On Then entry.Active = False
      Next

      'Monitor.PulseAll(m_syncObject)

    End SyncLock

    Return mixedOutput * 0.2

  End Function

  Sub Main()

    ' Shameless self-promotion
    Console.WriteLine("gotBASIC.com and OneLoneCoder.com - Synthesizer Part 3")
    Console.WriteLine("Multiple Oscillators with Polyphony")
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
    'Dim sound As New olcNoiseMaker(Of Short)(devices(0), 44100, 1, 8, 512) ' settings in the original C++ version; doesn't seem to work well enough here.
    Dim sound As New olcNoiseMaker(Of Short)(devices(0), 44100, 1, 8, 256)

    ' Link noise function with sound machine
    sound.SetUserFunction(AddressOf MakeNoise)

    Console.CursorVisible = False

    While True

      Dim abort = (GetAsyncKeyState(27) And &H8000) <> 0
      If abort Then
        sound.StopAudio()
        Exit While
      End If

      For k = 0 To 14

        Dim str = "ZSXCFVGBNJMK" & ChrW(&HBC) & ChrW(&HBE) & ChrW(&HBF)
        Dim keyState = GetAsyncKeyState(AscW(str(k)))
        Dim timeNow = sound.GetTime

        Dim i = k

        Dim notefound As Note = Nothing '        = vecNotes.Find(Function(item) item.Id = i)

        SyncLock m_syncObject

          For index = m_vecNotes.Count - 1 To 0 Step -1
            If m_vecNotes(index)?.Id = i Then notefound = m_vecNotes(index) : Exit For
          Next
          'For Each entry In m_vecNotes
          '  If entry.Id = i Then notefound = entry : Exit For
          'Next

          If notefound Is Nothing Then
            ' Note not found in list
            If (keyState And &H8000) <> 0 Then
              ' Key has been pressed so create a new note
              SyncLock sound.BufferLock
                Dim n = New Synth.Note(i, timeNow, 0, True, 1)
                m_vecNotes?.Add(n)
              End SyncLock
            Else
              ' Note not in list, but key has been relesed...
              ' ... nothing to do
            End If
          Else
            ' Note exists in list
            If (keyState And &H8000) <> 0 Then
              ' Key is still held, so nothing to do
              If notefound?.Off > notefound?.On Then
                'Key has been pressed again during release phase
                If notefound IsNot Nothing Then notefound.On = timeNow
                If notefound IsNot Nothing Then notefound.Active = True
              End If
            Else
              ' Key has been released, so switch off
              If notefound?.Off < notefound?.On Then
                If notefound IsNot Nothing Then notefound.Off = timeNow
              End If
            End If

          End If

          SyncLock sound.BufferLock
            For index = m_vecNotes.Count - 1 To 0 Step -1
              If Not m_vecNotes(index)?.Active Then
                m_vecNotes.RemoveAt(index)
                'Monitor.PulseAll(m_syncObject)
                Exit For
              End If
            Next
          End SyncLock

        End SyncLock

      Next

      Dim row = Console.CursorTop : Console.WriteLine("Notes: " & m_vecNotes?.Count & "    ") : Console.CursorTop = row

    End While

  End Sub

End Module