Option Explicit On
Option Infer On
Option Strict On

Imports System.IO
Imports System.Text
Imports NAudio.Wave

Namespace Global.PlayString

  Friend NotInheritable Class ParsePlayMacro

    Private ReadOnly m_data As New List(Of Byte())

    ' C, C#, D, D#, E,F, F#, G, G#, A, A#, B
    Private Shared ReadOnly FREQ As Double() = {16.35, 17.32, 18.35, 19.45, 20.6, 21.83, 23.12, 24.5, 25.96, 27.5, 29.14, 30.87, ' Octave 0
                                                32.7, 34.65, 36.71, 38.89, 41.2, 43.65, 46.25, 49, 51.91, 55, 58.27, 61.74, ' Octave 1
                                                65.41, 69.3, 73.42, 77.78, 82.41, 87.31, 92.5, 98, 103.83, 110, 116.54, 123.47, ' Octave 2
                                                130.81, 138.59, 146.83, 155.56, 164.81, 174.61, 185, 196, 207.65, 220, 233.08, 246.94, ' Octave 3
                                                261.63, 277.18, 293.66, 311.13, 329.63, 349.23, 369.99, 392, 415.3, 440, 466.16, 493.88, ' Octave 4
                                                523.25, 554.37, 587.33, 622.25, 659.25, 698.46, 739.99, 783.99, 830.61, 880, 932.33, 987.77, ' Octave 5
                                                1046.5, 1108.73, 1174.66, 1244.51, 1318.51, 1396.91, 1479.98, 1567.98, 1661.22, 1760, 1864.66, 1975.53, ' Octave 6
                                                2093, 2217.46, 2349.32, 2489.02, 2637.02, 2793.83, 2959.96, 3135.96, 3322.44, 3520, 3729.31, 3951.07, ' Octave 7
                                                4186.01, 4434.92, 4698.63, 4978.03, 5274.04, 5587.65, 5919.91, 6271.93, 6644.88, 7040, 7458.62, 7902.13} ' Octave 8

    Private Enum NoteDurations
      Normal
      Legato
      Staccato
    End Enum

    Private m_noteLength As Integer = 4
    Private m_octave As Integer = 4
    Private m_noteDuration As NoteDurations = NoteDurations.Normal
    Private m_tempo As Integer = 120
    Private m_musicBackground As Boolean = False

    Private Enum NoteDurationType
      Normal
      Legato
      Staccato
    End Enum

    Private ReadOnly m_commands As New List(Of Command)

    Friend Sub New(macro As String) ' PARSE

      macro = macro.ToUpper()

      Dim i = 0

      Dim octave = 4
      Dim adjustOctiveOfNextNote = 0

      While i < macro.Length

        Dim c = macro(i)

        Select Case c

          Case " "c, ChrW(13), ChrW(10), ";"c

          Case "<"c
            adjustOctiveOfNextNote -= 1
            If octave + adjustOctiveOfNextNote < 0 Then
              'TODO: error
            End If

          Case ">"c
            adjustOctiveOfNextNote += 1
            If octave + adjustOctiveOfNextNote > 6 Then
              'TODO: error
            End If

          Case "L"c
            i += 1
            Dim val = ""
            While i < macro.Length AndAlso Char.IsDigit(macro(i))
              val += macro(i)
              i += 1
            End While
            Dim length = Integer.Parse(val)
            If length < 1 OrElse length > 64 Then
              'TODO: error
            End If
            m_commands.Add(New PlayNoteLength(length))
            i -= 1

          Case "T"c
            i += 1
            Dim val = ""
            While i < macro.Length AndAlso Char.IsDigit(macro(i))
              val += macro(i)
              i += 1
            End While
            Dim duration = Integer.Parse(val)
            m_commands.Add(New PlayTempo(duration))
            i -= 1

          Case "M"c

            i += 1
            Select Case macro(i)
              Case "L"c
                m_commands.Add(New PlayLegato)
              Case "N"c
                m_commands.Add(New PlayNormal)
              Case "S"c
                m_commands.Add(New PlayStaccato)
              Case "B"c
                m_commands.Add(New PlayMusicBackground)
              Case "F"c
                m_commands.Add(New PlayMusicForeground)
              Case Else
                Throw New Exception($"Invalid duration {macro(i)}!")
            End Select

          Case "A"c, "B"c, "C"c, "D"c, "E"c, "F"c, "G"c
            'TODO: Need to handle "dotted" beyond single and double.
            Dim flat = False
            Dim sharp = False
            Dim dotted = False
            Dim doubledotted = False
            Dim n As New PlayNote
            i += 1
            If i < macro.Length AndAlso (macro(i) = "+"c OrElse macro(i) = "#"c) Then
              sharp = True
              i += 1
            End If
            If i < macro.Length AndAlso macro(i) = "-"c Then
              flat = True
              i += 1
            End If
            If i < macro.Length AndAlso macro(i) = "."c Then
              dotted = True
              i += 1
              If i < macro.Length AndAlso macro(i) = "."c Then
                doubledotted = True
                i += 1
              End If
            End If

            If i < macro.Length AndAlso Char.IsDigit(macro(i)) Then
              Dim val = ""
              While i < macro.Length AndAlso Char.IsDigit(macro(i))
                val += macro(i)
                i += 1
              End While
              Dim duration = Integer.Parse(val) ' A16 = L16A
              If i < macro.Length AndAlso (macro(i) = "+"c OrElse macro(i) = "#"c) Then
                sharp = True
                i += 1
              End If
              If i < macro.Length AndAlso macro(i) = "-"c Then
                flat = True
                i += 1
              End If
              If i < macro.Length AndAlso macro(i) = "."c Then
                dotted = True
                i += 1
                If macro(i) = "."c Then
                  doubledotted = True
                  i += 1
                End If
              End If
              n.Note = c
              n.Length = duration

            Else n.Note = c

            End If

            n.Flat = flat
            n.Sharp = sharp
            n.Dotted = dotted
            n.DoubleDotted = doubledotted
            If adjustOctiveOfNextNote <> 0 Then
              m_commands.Add(New PlayOctave(octave + adjustOctiveOfNextNote))
            End If
            m_commands.Add(n)
            If adjustOctiveOfNextNote <> 0 Then
              m_commands.Add(New PlayOctave(octave))
              adjustOctiveOfNextNote = 0
            End If
            i -= 1

          Case "O"c
            i += 1
            Dim val = ""
            While i < macro.Length AndAlso Char.IsDigit(macro(i))
              val += macro(i)
              i += 1
            End While
            octave = Integer.Parse(val)
            If octave < 0 OrElse octave > 6 Then
              'TODO: error
            End If
            m_commands.Add(New PlayOctave(octave))
            i -= 1

          Case "P"c
            'TODO: Can be "dotted"; see A-G.
            i += 1
            Dim val = ""
            While i < macro.Length AndAlso Char.IsDigit(macro(i))
              val += macro(i)
              i += 1
            End While
            Dim pause = Integer.Parse(val)
            If pause < 1 OrElse pause > 64 Then
              'TODO: error
            End If
            m_commands.Add(New PlayPause(pause))
            i -= 1

          Case "N"c ' Play note n where n 0 to 84

            Dim n As New PlayNote

            i += 1
            Dim val = ""
            While i < macro.Length AndAlso Char.IsDigit(macro(i))
              val += macro(i)
              i += 1
            End While
            Dim note = Integer.Parse(val)
            If note = 0 Then
              m_commands.Add(New PlayPause(1))
            ElseIf note < 0 OrElse note > 84 Then
              'TODO: error
            Else
              Dim notes = {"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "H"}

              Dim tempOctave = (note - 1) \ 12
              Dim tempNote = note Mod 12 : If tempNote = 0 Then tempNote = 12
              n.Note = notes(tempNote - 1)(0)
              If notes(tempNote - 1).Contains("#") Then
                n.Sharp = True
              End If

              If adjustOctiveOfNextNote <> 0 Then
                m_commands.Add(New PlayOctave(tempOctave))
              End If
              m_commands.Add(n)
              If adjustOctiveOfNextNote <> 0 Then
                m_commands.Add(New PlayOctave(octave))
              End If

            End If
            i -= 1

          Case Else
            Throw New Exception($"Command '{c}' not implemented!")
        End Select

        i += 1

      End While
    End Sub

    Friend Sub Generate() ' PROCESS
      For Each c As Command In m_commands
        If TypeOf c Is PlayTempo Then
          m_tempo = CType(c, PlayTempo).Tempo
        ElseIf TypeOf c Is PlayNoteLength Then
          m_noteLength = CType(c, PlayNoteLength).Length
        ElseIf TypeOf c Is PlayMusicBackground Then
          m_musicBackground = True
        ElseIf TypeOf c Is PlayMusicForeground Then
          m_musicBackground = False
        ElseIf TypeOf c Is PlayNormal Then
          m_noteDuration = NoteDurations.Normal
        ElseIf TypeOf c Is PlayLegato Then
          m_noteDuration = NoteDurations.Legato
        ElseIf TypeOf c Is PlayStaccato Then
          m_noteDuration = NoteDurations.Staccato
        ElseIf TypeOf c Is PlayNote Then
          Dim n = TryCast(c, PlayNote)
          Dim noteno = n.GetNumber()
          Dim herz = FREQ((12 * (m_octave + 2)) + noteno)
          Dim thisNoteLength = m_noteLength
          If n.Length > 0 Then
            thisNoteLength = n.Length
          End If
          Dim totalSeconds = (1 / (m_tempo / 60)) * (4 / thisNoteLength)
          Dim noteSeconds = totalSeconds
          Dim pause = 0#
          If m_noteDuration = NoteDurations.Normal Then
            noteSeconds *= (7 / 8)
            pause = totalSeconds - noteSeconds
          ElseIf m_noteDuration = NoteDurations.Staccato Then
            noteSeconds *= (3 / 4)
            pause = totalSeconds - noteSeconds
          End If
          If n.DoubleDotted Then
            noteSeconds += (noteSeconds / 2) + (noteSeconds / 4)
          ElseIf n.Dotted Then
            noteSeconds += (noteSeconds / 2)
          End If
          Tone(herz, noteSeconds)
          If pause > 0 Then
            Tone(0, pause)
          End If
        ElseIf TypeOf c Is PlayPause Then
          Dim pp = TryCast(c, PlayPause)
          Dim totalSeconds = (1 / (m_tempo / 60)) * (4 / pp.Pause)
          Tone(0, totalSeconds)
        ElseIf TypeOf c Is PlayOctave Then
          m_octave = CType(c, PlayOctave).Octave
        Else
          Throw New Exception("Not implemented " & c.[GetType]().ToString())
        End If
      Next
    End Sub

    Private Sub Tone(hz As Double, seconds As Double)
      Dim sampleRate = 44100.0#
      Dim duration = CInt(Fix((sampleRate * seconds)))
      Dim d = New Byte(duration - 1) {}
      Dim amplitude = 127.0#
      For n = 0 To duration - 1
        Dim value = CInt(Fix((amplitude * Math.Sin((2 * Math.PI * n * hz) / sampleRate))))
        If value > 4 Then
          value = 127
        ElseIf value < -4 Then
          value = -127
        Else
          value = 0
        End If
        d(n) = CByte((128 - value))
      Next
      m_data.Add(d)
    End Sub

    Private Function GetTotalLength() As UInteger
      Dim _length = 0UI
      For Each d In m_data
        _length += CUInt(d.Length)
      Next
      Return _length
    End Function

    Friend Async Function PlayAsync() As Task

      Dim subChunk2Size = GetTotalLength()
      Dim audioFormat = 1US ' PCM
      Dim numChannels = 1US ' Mono
      Dim bitsPerSample = 8US ' 8 bit
      Dim sampleRate = 44100UI
      Dim byteRate = CUInt((sampleRate * numChannels * (bitsPerSample / 8)))
      Dim subChunk1Size = 16UI
      Dim chunkSize = CUInt(36 + subChunk2Size)
      Dim blockAlign = CUShort((numChannels * (bitsPerSample / 8)))

      Using sw As New MemoryStream()
        MemoryStreamWriter(sw, Encoding.ASCII.GetBytes("RIFF"))
        MemoryStreamWriter(sw, BitConverter.GetBytes(chunkSize))
        MemoryStreamWriter(sw, Encoding.ASCII.GetBytes("WAVE"))
        MemoryStreamWriter(sw, Encoding.ASCII.GetBytes("fmt "))
        MemoryStreamWriter(sw, BitConverter.GetBytes(subChunk1Size))
        MemoryStreamWriter(sw, BitConverter.GetBytes(audioFormat))
        MemoryStreamWriter(sw, BitConverter.GetBytes(numChannels))
        MemoryStreamWriter(sw, BitConverter.GetBytes(sampleRate))
        MemoryStreamWriter(sw, BitConverter.GetBytes(byteRate))
        MemoryStreamWriter(sw, BitConverter.GetBytes(blockAlign))
        MemoryStreamWriter(sw, BitConverter.GetBytes(bitsPerSample))
        MemoryStreamWriter(sw, Encoding.ASCII.GetBytes("data"))
        For Each d As Byte() In m_data
          MemoryStreamWriter(sw, d)
        Next
        sw.Seek(0, SeekOrigin.Begin)
        Using player As New WaveOutEvent
          Using reader = New WaveFileReader(sw)
            player.Init(reader)
            player.Play()
            Do Until player.PlaybackState = PlaybackState.Stopped
              Await Task.Delay(1)
            Loop
          End Using
        End Using
      End Using

    End Function

    Private Shared Sub MemoryStreamWriter(sw As MemoryStream, bytes As Byte())
      For Each b In bytes
        sw.WriteByte(b)
      Next
    End Sub

    Friend Sub PlayOrSaveWav(Optional filespec As String = Nothing)

      Dim subChunk2Size = GetTotalLength()
      Dim audioFormat = 1US ' PCM
      Dim numChannels = 1US ' Mono
      Dim bitsPerSample = 8US ' 8 bit
      Dim sampleRate = 44100UI
      Dim byteRate = CUInt((sampleRate * numChannels * (bitsPerSample / 8)))
      Dim subChunk1Size = 16UI
      Dim chunkSize = CUInt(36 + subChunk2Size)
      Dim blockAlign = CUShort((numChannels * (bitsPerSample / 8)))

      If filespec Is Nothing Then

        Using sw As New MemoryStream()
          MemoryStreamWriter(sw, Encoding.ASCII.GetBytes("RIFF"))
          MemoryStreamWriter(sw, BitConverter.GetBytes(chunkSize))
          MemoryStreamWriter(sw, Encoding.ASCII.GetBytes("WAVE"))
          MemoryStreamWriter(sw, Encoding.ASCII.GetBytes("fmt "))
          MemoryStreamWriter(sw, BitConverter.GetBytes(subChunk1Size))
          MemoryStreamWriter(sw, BitConverter.GetBytes(audioFormat))
          MemoryStreamWriter(sw, BitConverter.GetBytes(numChannels))
          MemoryStreamWriter(sw, BitConverter.GetBytes(sampleRate))
          MemoryStreamWriter(sw, BitConverter.GetBytes(byteRate))
          MemoryStreamWriter(sw, BitConverter.GetBytes(blockAlign))
          MemoryStreamWriter(sw, BitConverter.GetBytes(bitsPerSample))
          MemoryStreamWriter(sw, Encoding.ASCII.GetBytes("data"))
          For Each d As Byte() In m_data
            MemoryStreamWriter(sw, d)
          Next
          sw.Seek(0, SeekOrigin.Begin)
          Using player As New WaveOutEvent
            Using reader = New WaveFileReader(sw)
              player.Init(reader)
              player.Play()
              Do Until player.PlaybackState = PlaybackState.Stopped
                Threading.Thread.Sleep(1)
              Loop
            End Using
          End Using
        End Using

      Else

        Using fs As New FileStream(filespec, FileMode.Create)
          Using bw As New BinaryWriter(fs)
            bw.Write(Encoding.ASCII.GetBytes("RIFF"))
            bw.Write(chunkSize)
            bw.Write(Encoding.ASCII.GetBytes("WAVE"))
            bw.Write(Encoding.ASCII.GetBytes("fmt "))
            bw.Write(subChunk1Size)
            bw.Write(audioFormat)
            bw.Write(numChannels)
            bw.Write(sampleRate)
            bw.Write(byteRate)
            bw.Write(blockAlign)
            bw.Write(bitsPerSample)
            bw.Write(Encoding.ASCII.GetBytes("data"))
            For Each d As Byte() In m_data
              bw.Write(d)
            Next
            bw.Close()
          End Using
        End Using

      End If

    End Sub

#Region "Command Classes - Used with Parsing / Execution"

    Private MustInherit Class Command

    End Class

    Private NotInheritable Class PlayLegato
      Inherits Command
    End Class

    Private NotInheritable Class PlayMusicBackground
      Inherits Command

    End Class

    Private NotInheritable Class PlayMusicForeground
      Inherits Command

    End Class

    Private NotInheritable Class PlayNormal
      Inherits Command

    End Class

    Private NotInheritable Class PlayNoteLength
      Inherits Command

      Public ReadOnly Property Length As Integer

      Public Sub New(length As Integer)
        Me.Length = length
      End Sub

    End Class

    Private NotInheritable Class PlayOctave
      Inherits Command

      Public ReadOnly Property Octave As Integer

      Public Sub New(octave As Integer)
        Me.Octave = octave
      End Sub

    End Class

    Private NotInheritable Class PlayNote
      Inherits Command

      Public Property Note As Char
      Public Property Length As Integer
      Public Property Dotted As Boolean
      Public Property DoubleDotted As Boolean
      Public Property Sharp As Boolean
      Public Property Flat As Boolean

      'Public Sub New(note As Char)
      '  Me.Note = note
      '  Length = -1 ' Note length is set by the Ln command 
      'End Sub

      'Public Sub New(note As Char, length As Integer)
      '  Me.Note = note
      '  Me.Length = length
      'End Sub

      Public Sub New()
      End Sub

      Public Function GetNumber() As Integer
        ' C, C#, D, D#, E,F, F#, G, G#, A, A#, H
        Dim number As Integer = 0
        Select Case Note
          Case "C"c : number = 0
          Case "D"c : number = 2
          Case "E"c : number = 4
          Case "F"c : number = 5
          Case "G"c : number = 7
          Case "A"c : number = 9
          Case "B"c : number = 11
        End Select
        If Sharp Then number += 1
        If Flat Then number -= 1
        Return number
      End Function

    End Class

    Private NotInheritable Class PlayPause
      Inherits Command

      Public ReadOnly Property Pause As Integer

      Public Sub New(pause As Integer)
        Me.Pause = pause
      End Sub

    End Class

    Private NotInheritable Class PlayStaccato
      Inherits Command

    End Class

    Private NotInheritable Class PlayTempo
      Inherits Command

      Public ReadOnly Property Tempo As Integer

      Public Sub New(tempo As Integer)
        Me.Tempo = tempo
      End Sub

    End Class

#End Region

  End Class

End Namespace