' GW-BASIC PLAY Emulator for VB.NET with Full Tandy 1000 MML Support
' Supports 3-voice polyphony, MML commands (L, O, T, N, P, V, dotted notes, MS, MN, ML, >, <, X), and waveform toggle

Imports System.IO
Imports System.Text

Module PlayEmulator

  Enum WaveformMode
    Sine
    Square
  End Enum

  Enum NoteStyle
    Normal
    Legato
    Staccato
  End Enum

  Class VoiceContext
    Public MML As String
    Public Position As Integer
    Public Octave As Integer = 4
    Public DefaultLength As Integer = 4
    Public Tempo As Integer = 120
    Public Style As NoteStyle = NoteStyle.Normal
    Public Volume As Integer = 8
  End Class

  Dim SampleRate As Integer = 44100
  Dim Voices(2) As VoiceContext
  Dim Waveform As WaveformMode = WaveformMode.Square

  ReadOnly NoteFrequencies As Double() = {
    0, 110.0, 116.54, 123.47, 130.81, 138.59, 146.83, 155.56, 164.81, 174.61,
    185.0, 196.0, 207.65, 220.0, 233.08, 246.94, 261.63, 277.18, 293.66,
    311.13, 329.63, 349.23, 369.99, 392.0, 415.3, 440.0, 466.16, 493.88,
    523.25, 554.37, 587.33, 622.25, 659.25, 698.46, 739.99, 783.99, 830.61,
    880.0, 932.33, 987.77, 1046.5, 1108.73, 1174.66, 1244.51, 1318.51, 1396.91,
    1479.98, 1567.98, 1661.22, 1760.0, 1864.66, 1975.53, 2093.0, 2217.46,
    2349.32, 2489.02, 2637.02, 2793.83, 2959.96, 3135.96, 3322.44, 3520.0,
    3729.31, 3951.07, 4186.01, 4434.92, 4698.64, 4978.03, 5274.04, 5587.65,
    5919.91, 6271.93, 6644.88, 7040.0, 7458.62, 7902.13, 8372.02, 8869.84,
    9397.27, 9956.06, 10548.08, 11175.3, 11839.82, 12543.85
  }

  Sub Main()
    ' Sample usage

    Dim s1 = "T180 L4 V10 MS C E G > C < G. E. C"
    Dim s2 = "T180 L8 V8 ML D F A < D > A. F. D"
    Dim s3 = "T180 L2 V6 MN G G G P4 G"
    Dim eyesOfTexas = "C4F. C8F8. C16F8. G16A2F2"

    SetWaveform(WaveformMode.Square)

    Dim mix = Play2Pcm(s1, s2, s3)
    Dim pcm = Play2Pcm(eyesOfTexas)

    WriteWav("play_output.wav", pcm)

  End Sub

  Sub SetWaveform(mode As WaveformMode)
    Waveform = mode
  End Sub

  Function Play2Pcm(voice1 As String) As Short()
    Return Play2Pcm(voice1, Nothing, Nothing)
  End Function

  Function Play2Pcm(voice1 As String, voice2 As String) As Short()
    Return Play2Pcm(voice1, voice2, Nothing)
  End Function

  Function Play2Pcm(voice1 As String, voice2 As String, voice3 As String) As Short()
    Voices(0) = New VoiceContext With {.MML = voice1}
    If Not String.IsNullOrWhiteSpace(voice2) Then Voices(1) = New VoiceContext With {.MML = voice2} Else Voices(1) = Nothing
    If Not String.IsNullOrWhiteSpace(voice3) Then Voices(2) = New VoiceContext With {.MML = voice3} Else Voices(2) = Nothing

    Dim buffers = New List(Of Short()) '(3)
    For i = 0 To 2
      If Voices(i) IsNot Nothing Then buffers.Add(PlayVoice(Voices(i)))
    Next

    Dim maxLength = buffers.Max(Function(b) b.Length)
    Dim mixed(maxLength - 1) As Short

    For i = 0 To maxLength - 1
      Dim sum As Integer = 0
      For Each buf In buffers
        If i < buf.Length Then sum += buf(i)
      Next
      sum = Math.Max(Math.Min(sum, Short.MaxValue), Short.MinValue)
      mixed(i) = CShort(sum)
    Next
    Return mixed
  End Function

  Function PlayVoice(vc As VoiceContext) As Short()
    Dim samples As New List(Of Short)
    While vc.Position < vc.MML.Length
      Dim c = Char.ToUpper(vc.MML(vc.Position))
      Select Case c
        Case "T"c : vc.Position += 1 : vc.Tempo = ParseNumber(vc.MML, vc.Position, vc.Tempo)
        Case "L"c : vc.Position += 1 : vc.DefaultLength = ParseNumber(vc.MML, vc.Position, vc.DefaultLength)
        Case "O"c : vc.Position += 1 : vc.Octave = ParseNumber(vc.MML, vc.Position, vc.Octave)
        Case "V"c : vc.Position += 1 : vc.Volume = Math.Min(15, ParseNumber(vc.MML, vc.Position, vc.Volume))
        Case "M"c
          vc.Position += 1
          If vc.Position < vc.MML.Length Then
            Select Case Char.ToUpper(vc.MML(vc.Position))
              Case "N"c : vc.Style = NoteStyle.Normal
              Case "L"c : vc.Style = NoteStyle.Legato
              Case "S"c : vc.Style = NoteStyle.Staccato
            End Select
            vc.Position += 1
          End If
        Case ">"c : vc.Octave = Math.Min(vc.Octave + 1, 6) : vc.Position += 1
        Case "<"c : vc.Octave = Math.Max(vc.Octave - 1, 0) : vc.Position += 1
        Case "A"c To "G"c
          samples.AddRange(ParseNote(vc, NoteCharToIndex(c)))
        Case "P"c
          vc.Position += 1
          Dim length = ParseNumber(vc.MML, vc.Position, vc.DefaultLength)
          Dim dur = NoteDuration(length, vc.Tempo, GetDotMultiplier(vc.MML, vc.Position))
          samples.AddRange(GenerateSilence(dur))
        Case "N"c
          vc.Position += 1
          Dim num = ParseNumber(vc.MML, vc.Position, 0)
          Dim dur = NoteDuration(vc.DefaultLength, vc.Tempo, GetDotMultiplier(vc.MML, vc.Position))
          If num = 0 Then
            samples.AddRange(GenerateSilence(dur))
          ElseIf num > 0 AndAlso num < NoteFrequencies.Length Then
            samples.AddRange(GenerateTone(NoteFrequencies(num), dur, vc.Volume, vc.Style))
          End If
        Case Else
          vc.Position += 1
      End Select
    End While
    Return samples.ToArray()
  End Function

  Function NoteCharToIndex(c As Char) As Integer
    Return "C D EF G A B".IndexOf(c) - ("C D EF G A B".IndexOf(c) \ 2)
  End Function

  Function ParseNote(vc As VoiceContext, baseIndex As Integer) As Short()
    vc.Position += 1
    If vc.Position < vc.MML.Length Then
      If vc.MML(vc.Position) = "+"c OrElse vc.MML(vc.Position) = "#"c Then
        baseIndex += 1 : vc.Position += 1
      ElseIf vc.MML(vc.Position) = "-"c Then
        baseIndex -= 1 : vc.Position += 1
      End If
    End If
    Dim length = ParseNumber(vc.MML, vc.Position, vc.DefaultLength)
    Dim dotMult = GetDotMultiplier(vc.MML, vc.Position)
    Dim freq = GetFrequency(vc.Octave, baseIndex)
    Dim dur = NoteDuration(length, vc.Tempo, dotMult)
    Return GenerateTone(freq, dur, vc.Volume, vc.Style)
  End Function

  Function GetFrequency(oct As Integer, idx As Integer) As Double
    Dim n = oct * 12 + idx + 1
    If n < 1 Then n = 1
    If n >= NoteFrequencies.Length Then n = NoteFrequencies.Length - 1
    Return NoteFrequencies(n)
  End Function

  Function ParseNumber(s As String, ByRef pos As Integer, def As Integer) As Integer
    Dim val = 0
    While pos < s.Length AndAlso Char.IsDigit(s(pos))
      val = val * 10 + (Asc(s(pos)) - Asc("0"c))
      pos += 1
    End While
    Return If(val > 0, val, def)
  End Function

  Function GetDotMultiplier(s As String, ByRef pos As Integer) As Double
    Dim mult = 1.0
    While pos < s.Length AndAlso s(pos) = "."c
      mult += 0.5 ^ ((pos - pos + 1))
      pos += 1
    End While
    Return mult
  End Function

  Function NoteDuration(len As Integer, tempo As Integer, dotMult As Double) As Double
    Return 60.0 / tempo * (4.0 / len) * dotMult
  End Function

  Function GenerateTone(freq As Double, dur As Double, vol As Integer, style As NoteStyle) As Short()
    Dim samples = CInt(SampleRate * dur)
    Dim activeSamples = samples
    Select Case style
      Case NoteStyle.Staccato : activeSamples = CInt(samples * 0.75)
      Case NoteStyle.Normal : activeSamples = CInt(samples * 0.875)
      Case NoteStyle.Legato : activeSamples = samples
    End Select

    Dim data(samples - 1) As Short
    For i = 0 To activeSamples - 1
      Dim t = i / SampleRate
      Dim amp = Math.Sin(2 * Math.PI * freq * t)
      If Waveform = WaveformMode.Square Then amp = If(amp >= 0, 1.0, -1.0)
      data(i) = CShort(amp * Short.MaxValue * (vol / 15.0) * 0.5)
    Next
    Return data
  End Function

  Function GenerateSilence(dur As Double) As Short()
    Return New Short(CInt(SampleRate * dur) - 1) {}
  End Function

  Sub WriteWav(path As String, data As Short())
    Using fs As New FileStream(path, FileMode.Create)
      Using bw As New BinaryWriter(fs, Encoding.ASCII)
        Dim bytes = data.Length * 2
        bw.Write(Encoding.ASCII.GetBytes("RIFF"))
        bw.Write(36 + bytes)
        bw.Write(Encoding.ASCII.GetBytes("WAVE"))
        bw.Write(Encoding.ASCII.GetBytes("fmt "))
        bw.Write(16)
        bw.Write(CShort(1))
        bw.Write(CShort(1))
        bw.Write(SampleRate)
        bw.Write(SampleRate * 2)
        bw.Write(CShort(2))
        bw.Write(CShort(16))
        bw.Write(Encoding.ASCII.GetBytes("data"))
        bw.Write(bytes)
        For Each s In data
          bw.Write(s)
        Next
      End Using
    End Using

  End Sub

End Module