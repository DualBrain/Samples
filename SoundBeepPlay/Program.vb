Imports System

Imports Audio

Imports NAudio.Wave

Module Program

  'Private WithEvents WaveOut As IWavePlayer
  'Private ReadOnly m_mixer As New SampleProviders.MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))

  Private WithEvents Player As New Player3()

  Private streams As New Dictionary(Of Integer, IO.Stream)

  Private m_stack As New Stack(Of IO.Stream)

  Sub Main(args As String())

    Do
      If Console.KeyAvailable Then
        Dim k = Console.ReadKey(True)
        Select Case k.Key
          Case ConsoleKey.B
            m_stack.Push(New SoundGenerator(44100, 800, 4.55).Stream)
            'If streams.Count = 0 Then
            PlayNext()
            'End If
          Case ConsoleKey.Escape
            Exit Do
          Case Else
        End Select
      End If
    Loop

  End Sub

  Private Sub PlayNext()

    If m_stack.Count > 0 Then

      Dim s = m_stack.Pop

      Dim id = 0
      Do
        If streams.ContainsKey(id) Then
          id += 1
        Else
          Exit Do
        End If
      Loop

      streams.Add(id, s)
      Player.Play(id, streams(id))

    End If

  End Sub

  Private Sub Player_AudioFinished(sender As Object, e As AudioEventArgs2) Handles Player.AudioFinished
    Console.WriteLine("Finished")
    If e.LogIndex IsNot Nothing Then
      Dim id = CInt(e.LogIndex)
      If streams.ContainsKey(id) Then
        streams(id).Close()
        streams(id) = Nothing
        streams.Remove(id)
      End If
      PlayNext()
    End If
  End Sub

  Private Sub Player_Clock(sender As Object, e As ClockEventArgs2) Handles Player.Clock
    Console.WriteLine(e.Clock)
  End Sub

  Private Sub Player_AudioClosed(sender As Object, e As AudioEventArgs2) Handles Player.AudioClosed
    Console.WriteLine("Closed")
  End Sub

  Private Sub Player_AudioStarted(sender As Object, e As AudioEventArgs2) Handles Player.AudioStarted
    Console.WriteLine("Started")
  End Sub

End Module