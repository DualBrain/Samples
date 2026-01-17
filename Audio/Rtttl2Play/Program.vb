Imports System.IO

Module Module1

  ' For files:
  ' - https://picaxe.com/rtttl-ringtones-for-tune-command/
  ' - https://web.archive.org/web/20240106064604/https://blamba.de/rtttl/

  ' A Nokring/RTTTL format ringtone MUST contain very specific
  ' elements in order to be recognized by ringtone programs.
  ' 
  ' A Nokring/RTTTL ringtone looks like this...
  '
  ' `summer:d=4,o=5,b=140:8g6,8a6,a6,p,8d6,8f6,g6,p`
  '
  ' ...and has the following three parts separated by two colons:
  ' 
  ' part 1: the name (here: `summer`), usually a single suite of
  '         characters with no spaces
  ' part 2: the settings (here: `d=4,o=5,b=140`), where `d=` is
  '         the default duration of a note. In this case, the `4`
  '         means that each note with no duration mentioned (see
  '         below) is by default considered a quarter note.
  '         `8` would mean an eight note, and so on. Accordingly,
  '         `o=` is the default octave. There are four octaves in
  '         the Nokring/RTTTL format. And `b=` is the tempo, in
  '         `beats per minute`.
  ' part 3: the notes. Each note is separated by a comma and includes,
  '         in sequence: a duration marker, a standard music note,
  '         either a, b, c, d, e, f or g, and an octave marker. If
  '         no duration or octave marker are present, the default applies.
  '
  ' It's important to note that the three parts must be present AND
  ' separated by a colon. The name part may be blank but the separating
  ' colon must be there. In other words, if there are not two colons
  ' in a ringtone string, it is not a standard or proper Nokring/RTTTL
  ' ringtone.

  ' https://en.wikipedia.org/wiki/Ring_Tone_Text_Transfer_Language

  Friend ReadOnly m_rtttlPitches As New List(Of String) From {
      "p", "c", "c#", "d", "d#", "e", "f",
      "f#", "g", "g#", "a", "a#", "b"}

  ' ------------

  ' Picaxe TUNE file

  ' https://picaxe.com/basic-commands/digital-inputoutput/tune/
  ' also see manual (pdf) on page 258 for more details
  ' on how the actual tune data is encoded
  ' additional...
  ' https://picaxeforum.co.uk/threads/about-the-ringtones-and-command-sound-tune.10861/
  ' https://www.picaxeforum.co.uk/threads/playing-a-note-the-hard-way.5881/
  ' https://www.picaxeforum.co.uk/threads/playing-a-tune-on-the-bigger-picaxes.9578/
  ' https://www.picaxeforum.co.uk/threads/dtmf-dialer-dual-08m.8140/
  ' https://community.robotshop.com/forum/t/example-music-for-the-picaxe/12937
  '
  ' more ringtone sites
  ' http://www.ringtonerfest.com/
  ' http://www.free-ringtones.eu.com/
  ' http://www.tones4free.com/

  ' Map semitone offset to GW-BASIC note names
  Friend ReadOnly m_picaxeNotes = {
    "C", "C#", "D", "D#", "E",
    "F", "F#", "G", "G#", "A",
    "A#", "B"}

  Sub Main(args As String())

    If Debugger.IsAttached Then
      args = {"c:\rtttl\adamsfamily.txt", "c:\rtttl\adamsfamily.bas"}
    End If

    If args.Length <> 2 Then Console.WriteLine("Usage: R2P i o") : Return

    Dim inputPath = args(0)
    Dim outputPath = args(1)

    Dim melody = ParseRtttl(File.ReadAllText(inputPath))
    Dim programLines = GeneratePlay(melody)

    Using writer As New StreamWriter(outputPath)
      For i = 0 To programLines.Count - 1
        writer.WriteLine($"{i + 1} {programLines(i)}")
      Next
    End Using

  End Sub

  Function ParseNotes(text As String) As List(Of Note)

    Dim notes = text.Trim.ToLower.Split(","c)
    Dim parsedNotes = New List(Of Note)

    For Each rawNote In notes
      Dim note = rawNote.Trim()
      Dim newNote As New Note()

      ' Duration
      If Char.IsDigit(note(0)) Then
        If note.Length > 1 AndAlso Char.IsDigit(note(1)) Then
          newNote.Duration = Integer.Parse(note.Substring(0, 2))
          note = note.Substring(2)
        Else
          newNote.Duration = Integer.Parse(note(0).ToString)
          note = note.Substring(1)
        End If
      End If

      ' Pitch
      If note.Length > 1 AndAlso
         note(1) = "#"c Then
        newNote.Pitch = m_rtttlPitches.IndexOf(note.Substring(0, 2))
        note = note.Substring(2)
      Else
        newNote.Pitch = m_rtttlPitches.IndexOf(note.Substring(0, 1))
        note = note.Substring(1)
      End If

      ' Octave
      If note.Length >= 1 AndAlso
         Char.IsDigit(note(0)) Then
        newNote.Octave = Integer.Parse(note(0).ToString)
        note = note.Substring(1)
      End If

      ' Dot
      If note.Length >= 1 Then
        newNote.Dot = True
      End If

      parsedNotes.Add(newNote)

    Next

    Return parsedNotes

  End Function

  Function ParseRtttl(text As String) As Melody
    text = text.Trim
    Dim melody = New Melody()
    Dim parts = text.Split(":"c)
    Dim defaults = parts(1).Split(","c)

    melody.Name = parts(0)

    For Each df In defaults
      Dim d = df.Trim()
      If d.StartsWith("d=") Then
        melody.Duration = Integer.Parse(d.Substring(2))
      ElseIf d.StartsWith("o=") Then
        melody.Octave = Integer.Parse(d.Substring(2))
      ElseIf d.StartsWith("b=") Then
        melody.BPM = Integer.Parse(d.Substring(2))
      End If
    Next

    melody.Notes = ParseNotes(parts(2))

    Console.WriteLine(melody)

    Return melody

  End Function

  Function ParsePicaxe(text As String) As Melody

    'TODO: Parse the text to determine the hex bytes.

    ' Example hex array from the Addams tune (just the bytes)
    Dim hexBytes As Byte() = {
        &H4C, &H41, &H46, &H4A, &H46, &H41, &H40, &H48, &H46, &H45, &H48, &H45,
        &H41, &H6A, &H46, &H41, &H46, &H4A, &H46, &H41, &H40, &H48, &H46, &H45,
        &H41, &H43, &H45, &H6, &H41, &H43, &H45, &H6, &H41, &H43, &H47, &H8,
        &H43, &H45, &H47, &H48, &H43, &H45, &H47, &H48, &H41, &H43, &H45, &H46
    }

    Dim playBuilder As New Text.StringBuilder()

    ' Start PLAY string with tempo, octave, and default length
    playBuilder.Append("T150 O4 L4 ")

    For Each b As Byte In hexBytes
      ' Ignore control bytes (less than $40 or special codes like $06, $08, $6A)
      If b < &H40 OrElse b = &H6A OrElse b = &H6 OrElse b = &H8 Then
        Continue For
      End If

      ' Calculate note offset from $40 (assumed C4)
      Dim offset As Integer = b - &H40
      Dim octave As Integer = 4 + (offset \ 12)
      Dim noteIndex As Integer = offset Mod 12
      If noteIndex < 0 Then noteIndex += 12 ' handle negatives just in case

      ' Append note in GW-BASIC format (e.g. O4 C#, O5 D)
      ' If octave changes, include octave command
      If playBuilder.Length > 0 Then
        ' Check last octave to avoid redundant O commands (simple version: always write octave)
        playBuilder.Append("O" & octave & " ")
      End If

      playBuilder.Append(m_picaxeNotes(noteIndex) & " ")
    Next

    ' Output the final PLAY string
    Console.WriteLine("GW-BASIC PLAY command:")
    Console.WriteLine(playBuilder.ToString.Trim)

    ' Wait for user
    Console.WriteLine(vbCrLf & "Press any key to exit...")
    Console.ReadKey()

    Return Nothing

  End Function

  Function GeneratePlay(melody As Melody) As List(Of String)

    Dim program = New List(Of String)
    Dim config = $"L{melody.Duration} O{melody.Octave - 1} T{melody.BPM} "
    Dim cmd = ""
    Dim prevDuration = melody.Duration
    Dim prevOctave = melody.Octave
    Dim unprocessedLeft = False

    For Each note In melody.Notes

      unprocessedLeft = True

      If Not note.Duration.HasValue Then note.Duration = melody.Duration
      If Not note.Octave.HasValue Then note.Octave = melody.Octave

      If note.Pitch = 0 Then
        cmd += $"p{note.Duration} "
      Else
        If note.Duration.Value <> prevDuration Then
          cmd += $"L{note.Duration} "
          prevDuration = note.Duration.Value
        End If
        If note.Octave.Value <> prevOctave Then
          cmd += $"O{note.Octave - 1} "
          prevOctave = note.Octave.Value
        End If
        If note.Dot Then
          cmd += $"{m_rtttlPitches(note.Pitch)}. "
        Else
          cmd += $"{m_rtttlPitches(note.Pitch)} "
        End If
      End If

      If (config.Length + cmd.Length + 8) > 60 Then
        program.Add($"PLAY ""{config}{cmd}""")
        config = $"L{prevDuration} O{prevOctave - 1} T{melody.BPM} "
        cmd = ""
        unprocessedLeft = False
      End If

    Next

    If unprocessedLeft Then
      program.Add($"PLAY ""{config}{cmd}""")
    End If

    Return program

  End Function

End Module