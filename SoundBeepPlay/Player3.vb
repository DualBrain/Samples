Imports NAudio.Wave

Public Class Player3

  Private ReadOnly m_latency As Integer = 100
  Private ReadOnly m_bufferCount As Integer = 2

  Public Class Machine
    Public Property Path As String
    'Public Property Intro As Single = 0
    Private m_eom As Single
    Public Property Eom As Single
      Get
        Return Me.m_eom
      End Get
      Set(value As Single)
        Me.m_eom = value
      End Set
    End Property
    Public Property EomFired As Boolean = False
    Public Property Clock As String = ""
    Public Property Total As String = ""
    Public Property VuLeft As Single = 0
    Public Property VuRight As Single = 0
    Public Property WaveStream As WaveStream
    Public Property SampleProvider As ISampleProvider
    Private ReadOnly m_created As Date = Now
    Public ReadOnly Property Created
      Get
        Return Me.m_created
      End Get
    End Property
    Public Property LogIndex As Integer?
    'Public Property Cut As String
    'Public Property Title As String
    'Public Property Artist As String
    'Public Property Type As String
  End Class

  'Private m_lock As New Object
  Private ReadOnly m_machines As New List(Of Machine)

  Public ReadOnly Property Machines As List(Of Machine)
    Get
      Debug.WriteLine(String.Format("{0} {1} Machines [property] ... ", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
      Return Me.m_machines
    End Get
  End Property

  Private WithEvents WaveOut As IWavePlayer
  Private ReadOnly m_mixer As New NAudio.Wave.SampleProviders.MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1))

  Private WithEvents TimerInternal As New System.Timers.Timer(50) 'm_latency)

  Public Event AudioStarted As EventHandler(Of AudioEventArgs2)
  Public Event AudioEom As EventHandler(Of AudioEventArgs2)
  Public Event AudioFinished As EventHandler(Of AudioEventArgs2)
  Public Event AudioClosed As EventHandler(Of AudioEventArgs2)
  Public Event Clock As EventHandler(Of ClockEventArgs2)
  Public Event Vu As EventHandler(Of VuEventArgs2)

  'Public Event AudioDebugger As EventHandler(Of AudioDebuggerEventArgs2)

  'Private m_suppressTimer As New List(Of Boolean)

  Private ReadOnly m_syncContext As Threading.SynchronizationContext

  Public Sub New()
    Me.m_syncContext = Threading.SynchronizationContext.Current
    Me.TimerInternal.Start()
  End Sub

  Public Sub Play(id As Integer, data As IO.Stream)

    Dim logIndex = id

    Dim machine As New Machine
    machine.WaveStream = New WaveFileReader(data)
    machine.LogIndex = logIndex
    machine.Path = Nothing
    'machine.Intro = intro
    machine.Eom = 0
    machine.EomFired = False

    Dim totalTime As TimeSpan = machine.WaveStream.TotalTime
    machine.Total = String.Format("{0}:{1:00}.{2}", totalTime.Minutes, totalTime.Seconds, totalTime.Milliseconds \ 100)

    Debug.WriteLine(String.Format("{0} {1} Play()... Pcm16BitToSampleProvider(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
    machine.SampleProvider = New SampleProviders.Pcm16BitToSampleProvider(machine.WaveStream)

    Dim sampleProvider = New SampleProviders.MeteringSampleProvider(machine.SampleProvider)
    AddHandler sampleProvider.StreamVolume, AddressOf Me.OnPostVolumeMeter
    machine.SampleProvider = sampleProvider

    Debug.WriteLine(String.Format("{0} {1} Play()... Machines Add(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
    'Debug.WriteLine(String.Format("{0} {1} Play()... SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    'SyncLock m_lock
    Me.m_machines.Add(machine)
    'End SyncLock
    'Debug.WriteLine(String.Format("{0} {1} Play()... End SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))

    Debug.WriteLine(String.Format("{0} {1} Play()... AddMixerInput(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
    Me.m_mixer.AddMixerInput(machine.SampleProvider)

    If Me.WaveOut Is Nothing Then
      Dim device As New WaveOutEvent With {
        .DesiredLatency = Me.m_latency,
        .NumberOfBuffers = Me.m_bufferCount
      }
      Me.WaveOut = device
      Debug.WriteLine(String.Format("{0} {1} Play()... AddHandler PlaybackStopped().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
      AddHandler Me.WaveOut.PlaybackStopped, AddressOf Me.WaveOut_PlaybackStopped
      Debug.WriteLine(String.Format("{0} {1} Play()... WaveOut.Init(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
      Me.WaveOut.Init(New SampleProviders.SampleToWaveProvider(Me.m_mixer))
      Debug.WriteLine(String.Format("{0} {1} Play()... WaveOut.Play(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
      Me.WaveOut.Play()
    Else
      If Not Me.WaveOut.PlaybackState = PlaybackState.Playing Then
        Debug.WriteLine(String.Format("{0} {1} Play()... WaveOut.Play(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
        Me.WaveOut.Play()
      End If
    End If

    RaiseEvent AudioStarted(Me, New AudioEventArgs2(logIndex, Nothing))

  End Sub

  Public Sub Play(path As String,
                  ByVal eom As Single,
                  ByVal logIndex As Integer?)

    'Debug.WriteLine(String.Format("{0} {1} Play()... SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    'SyncLock m_timerLock
    '  m_suppressTimer.Add(True)
    'End SyncLock
    'Debug.WriteLine(String.Format("{0} {1} Play()... End SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))

    Debug.WriteLine(String.Format("{0} {1} Play()... ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))

    'Try

    'If m_waveOut Is Nothing Then
    '  Dim device As New WaveOutEvent
    '  device.DesiredLatency = m_latency
    '  device.NumberOfBuffers = m_bufferCount
    '  m_waveOut = device
    '  RaiseEvent AudioDebugger(Me, New AudioDebuggerEventArgs2(String.Format("Play()... WaveOut.Init(). ({0})", path)))
    '  m_waveOut.Init(New SampleProviders.SampleToWaveProvider(m_mixer))
    'End If

    Dim machine As New Machine

    If String.Compare(IO.Path.GetExtension(path), ".WAV", True) = 0 Then
      Debug.WriteLine(String.Format("{0} {1} Play()... WaveFileReader(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
      machine.WaveStream = New WaveFileReader(path)
    ElseIf String.Compare(IO.Path.GetExtension(path), ".MP3", True) = 0 Then
      Debug.WriteLine(String.Format("{0} {1} Play()... Mp3FileReader(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
      machine.WaveStream = New Mp3FileReader(path)
      If machine.WaveStream.WaveFormat.BitsPerSample <> 16 OrElse
         machine.WaveStream.WaveFormat.SampleRate <> 44100 OrElse
         machine.WaveStream.WaveFormat.Channels <> 2 Then
        Debug.WriteLine(String.Format("{0} {1} Play()... WaveFormatConversionStream(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
        Dim format As New WaveFormat(44100, 16, 2)
        machine.WaveStream = New WaveFormatConversionStream(format, machine.WaveStream)
      End If
      'ElseIf String.Compare(IO.Path.GetExtension(path), ".WMA", True) = 0 Then
      '  Debug.WriteLine(String.Format("{0} {1} Play()... WmaFileReader(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
      '  machine.WaveStream = New NAudio.WindowsMediaFormat.WMAFileReader(path)
    ElseIf String.Compare(IO.Path.GetExtension(path), ".MOD", True) = 0 Then
      Debug.WriteLine(String.Format("{0} {1} Play()... MOD Player(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
      Stop
      'Dim myMod As SongModule = ModuleLoader.Instance.LoadModule("SongModule.Mod|S3M|XM")
      'Dim player As New ModulePlayer(myMod)
      'Dim drv As New SharpMod.SoundRenderer.NAudioWaveChannelDriver(NAudioWaveChannelDriver.Output.WaveOut)
      'player.RegisterRenderer(drv)
      'player.Start()
    Else
      Throw New InvalidOperationException("Unsupported extension")
    End If

    machine.LogIndex = logIndex
    'machine.Title = title
    'machine.Cut = cut
    'machine.Artist = artist
    'machine.Type = type
    machine.Path = path
    'machine.Intro = intro
    machine.Eom = eom
    machine.EomFired = False

    Dim totalTime As TimeSpan = machine.WaveStream.TotalTime
    machine.Total = String.Format("{0}:{1:00}.{2}", totalTime.Minutes, totalTime.Seconds, totalTime.Milliseconds \ 100)

    Debug.WriteLine(String.Format("{0} {1} Play()... Pcm16BitToSampleProvider(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
    machine.SampleProvider = New SampleProviders.Pcm16BitToSampleProvider(machine.WaveStream)

    Dim sampleProvider = New SampleProviders.MeteringSampleProvider(machine.SampleProvider)
    AddHandler sampleProvider.StreamVolume, AddressOf Me.OnPostVolumeMeter
    machine.SampleProvider = sampleProvider

    Debug.WriteLine(String.Format("{0} {1} Play()... Machines Add(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
    'Debug.WriteLine(String.Format("{0} {1} Play()... SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    'SyncLock m_lock
    Me.m_machines.Add(machine)
    'End SyncLock
    'Debug.WriteLine(String.Format("{0} {1} Play()... End SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))

    Debug.WriteLine(String.Format("{0} {1} Play()... AddMixerInput(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
    Me.m_mixer.AddMixerInput(machine.SampleProvider)

    If Me.WaveOut Is Nothing Then
      Dim device As New WaveOutEvent With {
        .DesiredLatency = Me.m_latency,
        .NumberOfBuffers = Me.m_bufferCount
      }
      Me.WaveOut = device
      Debug.WriteLine(String.Format("{0} {1} Play()... AddHandler PlaybackStopped().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
      AddHandler Me.WaveOut.PlaybackStopped, AddressOf Me.WaveOut_PlaybackStopped
      Debug.WriteLine(String.Format("{0} {1} Play()... WaveOut.Init(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
      Me.WaveOut.Init(New SampleProviders.SampleToWaveProvider(Me.m_mixer))
      Debug.WriteLine(String.Format("{0} {1} Play()... WaveOut.Play(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
      Me.WaveOut.Play()
    Else
      If Not Me.WaveOut.PlaybackState = PlaybackState.Playing Then
        Debug.WriteLine(String.Format("{0} {1} Play()... WaveOut.Play(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
        Me.WaveOut.Play()
      End If
    End If

    'Debug.WriteLine(String.Format("{0} Play()... RaiseEvent AudioStarted({1}).", Now, logIndex))
    RaiseEvent AudioStarted(Me, New AudioEventArgs2(logIndex, path))

    'Finally
    '  Debug.WriteLine(String.Format("{0} {1} Play()... SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    '  SyncLock m_timerLock
    '    m_suppressTimer.RemoveAt(0)
    '  End SyncLock
    '  Debug.WriteLine(String.Format("{0} {1} Play()... End SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    'End Try

  End Sub

  Public Sub [Stop](logIndex As Integer)

    'Debug.WriteLine(String.Format("{0} {1} Stop({2}))... SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
    'SyncLock m_timerLock
    '  m_suppressTimer.Add(True)
    'End SyncLock
    'Debug.WriteLine(String.Format("{0} {1} Stop({2}))... End SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId, logIndex))

    'Try

    Dim closedList As New List(Of AudioEventArgs2)

    'Debug.WriteLine(String.Format("{0} {1} Stop({2}))... SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
    'SyncLock m_lock

    Dim machines = From p In Me.m_machines Where p.LogIndex = logIndex
    Dim path = machines(0).Path

    If machines.Count = 1 Then

      Debug.WriteLine(String.Format("{0} {1} Stop()... RemoveMixerInput(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
      Me.m_mixer.RemoveMixerInput(machines(0).SampleProvider)
      machines(0).SampleProvider = Nothing

      machines(0).WaveStream.Close()
      Debug.WriteLine(String.Format("{0} {1} Stop()... WaveStream.Dispose(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
      machines(0).WaveStream.Dispose()
      machines(0).WaveStream = Nothing

      Debug.WriteLine(String.Format("{0} {1} Stop()... Machines Remove(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
      Me.m_machines.Remove(machines(0))

      'Debug.WriteLine(String.Format("{0} Stop()... RaiseEvent AudioClosed({1}).", Now, logIndex))
      'RaiseEvent AudioClosed(Me, New AudioEventArgs2(logIndex, path))
      closedList.Add(New AudioEventArgs2(logIndex, path))

    Else

      Throw New ArgumentException("Path provided does not appear to be currently playing.")

    End If

    If Me.WaveOut.PlaybackState = PlaybackState.Playing AndAlso
       Me.m_machines.Count = 0 Then
      Debug.WriteLine(String.Format("{0} {1} Stop()... WaveOut.Stop(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
      Me.WaveOut.Stop()
    End If

    'End SyncLock
    'Debug.WriteLine(String.Format("{0} {1} Stop({2}))... End SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId, logIndex))

    For Each entry In closedList
      RaiseEvent AudioClosed(Me, entry)
    Next

    'Finally
    '  Debug.WriteLine(String.Format("{0} {1} Stop({2}))... SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
    '  SyncLock m_timerLock
    '    m_suppressTimer.RemoveAt(0)
    '  End SyncLock
    '  Debug.WriteLine(String.Format("{0} {1} Stop({2}))... End SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId, logIndex))
    'End Try

  End Sub

  Public Sub Close()

    'Debug.WriteLine(String.Format("{0} {1} Close())... SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    'SyncLock m_timerLock
    '  m_suppressTimer.Add(True)
    'End SyncLock
    'Debug.WriteLine(String.Format("{0} {1} Close())... End SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))

    'Try

    If Me.WaveOut IsNot Nothing Then

      If Me.WaveOut.PlaybackState = PlaybackState.Playing Then
        Debug.WriteLine(String.Format("{0} {1} Close()... WaveOut.Stop().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
        Me.WaveOut.Stop()
      End If

      Debug.WriteLine(String.Format("{0} {1} Closed()... RemoveHandler PlaybackStopped().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
      RemoveHandler Me.WaveOut.PlaybackStopped, AddressOf Me.WaveOut_PlaybackStopped

      Debug.WriteLine(String.Format("{0} {1} Close()... WaveOut.Dispose().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
      Me.WaveOut.Dispose()
      Me.WaveOut = Nothing

    End If

    'Dim finishedList As New List(Of PassOn)
    Dim closedList As New List(Of AudioEventArgs2)
    'Dim eomList As New List(Of PassOn)

    'Debug.WriteLine(String.Format("{0} {1} Close())... SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    'SyncLock m_lock

    For Each machine In Me.m_machines

      Debug.WriteLine(String.Format("{0} {1} Close()... RemoveMixerInput(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, machine.LogIndex))
      Me.m_mixer.RemoveMixerInput(machine.SampleProvider)
      machine.SampleProvider = Nothing

      If machine.WaveStream IsNot Nothing Then
        machine.WaveStream.Close()
        Debug.WriteLine(String.Format("{0} {1} Close()... WaveStream.Dispose(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, machine.LogIndex))
        machine.WaveStream.Dispose()
        machine.WaveStream = Nothing
      End If
      'RaiseEvent AudioClosed(Me, New AudioEventArgs2(machine.LogIndex, machine.Path))
      closedList.Add(New AudioEventArgs2(machine.LogIndex, machine.Path))
    Next

    Debug.WriteLine(String.Format("{0} {1} Close()... Machines.Clear().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
    Me.m_machines.Clear()

    'End SyncLock
    'Debug.WriteLine(String.Format("{0} {1} Close())... End SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))

    For Each entry In closedList
      'Debug.WriteLine(String.Format("{0} Close()... RaiseEvent AudioClosed({1}).", Now, entry.LogIndex))
      RaiseEvent AudioClosed(Me, entry)
    Next

    'Finally
    '  Debug.WriteLine(String.Format("{0} {1} Close())... SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    '  SyncLock m_timerLock
    '    m_suppressTimer.RemoveAt(0)
    '  End SyncLock
    '  Debug.WriteLine(String.Format("{0} {1} Close())... End SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    'End Try

  End Sub

  'Private m_timerLock As New Object

  Private m_suppressTimer As Boolean

  Private Sub HandleTimer(state As Object)

    If Me.m_suppressTimer Then 'm_suppressTimer.Count > 0 Then
      Return
    End If

    Me.m_suppressTimer = True 'm_suppressTimer.Add(True)

    Try

      'If m_suppressTimer.Count > 0 Then
      '  Return
      'End If

      'Debug.WriteLine(String.Format("{0} {1} DoStuff()... SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
      'SyncLock m_timerLock
      '  m_suppressTimer.Add(True)
      'End SyncLock
      'Debug.WriteLine(String.Format("{0} {1} DoStuff()... End SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))

      'Try

      If Me.WaveOut IsNot Nothing Then

        If Me.WaveOut.PlaybackState = PlaybackState.Playing Then

          Dim finishedList As New List(Of AudioEventArgs2)
          Dim closedList As New List(Of AudioEventArgs2)
          'Dim eomList As New List(Of PassOn)
          'Dim exList As New List(Of PassOn)

          Dim eomList As New List(Of AudioEventArgs2)
          Dim clockList As New List(Of ClockEventArgs2)
          Dim vuList As New List(Of VuEventArgs2)

          'Debug.WriteLine(String.Format("{0} {1} DoStuff()... SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
          'SyncLock m_lock

          For index = Me.m_machines.Count - 1 To 0 Step -1

            Dim machine = Me.m_machines(index)

            Dim totalTime As TimeSpan = machine.WaveStream.TotalTime
            Dim timeLeft As TimeSpan = totalTime.Subtract(machine.WaveStream.CurrentTime)

            Dim played As Integer = (totalTime - timeLeft).TotalSeconds + 5
            Dim watchDog As Integer = DateDiff(DateInterval.Second, machine.Created, Now)

            'If played < watchDog Then

            If machine.WaveStream.Position >= machine.WaveStream.Length OrElse
               timeLeft = TimeSpan.Zero OrElse
               played < watchDog Then

              Debug.WriteLine(String.Format("{0} {1} Timer(length)... RemoveMixerInput(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, machine.LogIndex))
              Me.m_mixer.RemoveMixerInput(machine.SampleProvider)
              machine.SampleProvider = Nothing
              'RaiseEvent AudioFinished(Me, New AudioEventArgs2(machine.LogIndex, machine.Path))
              finishedList.Add(New AudioEventArgs2(machine.LogIndex, machine.Path))

              If machine.WaveStream IsNot Nothing Then
                machine.WaveStream.Close()
                Debug.WriteLine(String.Format("{0} {1} Timer(length)... WaveStream.Dispose(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, machine.LogIndex))
                machine.WaveStream.Dispose()
                machine.WaveStream = Nothing
                'RaiseEvent AudioClosed(Me, New AudioEventArgs2(machine.LogIndex, machine.Path))
                closedList.Add(New AudioEventArgs2(machine.LogIndex, machine.Path))
              End If

              Debug.WriteLine(String.Format("{0} {1} Timer(length)... EomFired Check. ({2} = {3})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, machine.LogIndex, machine.EomFired))
              If Not machine.EomFired Then
                'eomList.Add(New PassOn With {.Path = machine.Path,
                '                             .EomFired = machine.EomFired,
                '                             .LogIndex = machine.LogIndex})
                'RaiseEvent AudioEom(Me, New AudioEventArgs2(machine.LogIndex, machine.Path))
                eomList.Add(New AudioEventArgs2(machine.LogIndex, machine.Path))
              End If

              Debug.WriteLine(String.Format("{0} {1} Timer(length)... Machine.RemoveAt({2}). ({3})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, index, machine.LogIndex))
              Me.m_machines.RemoveAt(index)

            Else

              If Not machine.EomFired AndAlso
                 machine.Eom > 0 Then
                If timeLeft.TotalSeconds <= machine.Eom Then
                  machine.EomFired = True
                  'Debug.WriteLine(String.Format("{0} Timer()... RaiseEvent AudioEom({1}).", Now, machine.LogIndex))
                  'RaiseEvent AudioEom(Me, New AudioEventArgs2(machine.LogIndex, machine.Path))
                  eomList.Add(New AudioEventArgs2(machine.LogIndex, machine.Path))
                End If
              End If

              Dim total As String = String.Format("{0}:{1:00}.{2}", totalTime.Minutes, totalTime.Seconds, totalTime.Milliseconds \ 100)
              Dim clock As String = String.Format("{0}:{1:00}.{2}", timeLeft.Minutes, timeLeft.Seconds, timeLeft.Milliseconds \ 100)
              If clock <> machine.Clock Then
                machine.Clock = clock
                'RaiseEvent Clock(Me, New ClockEventArgs2(machine.LogIndex, machine.Path, clock, total))
                clockList.Add(New ClockEventArgs2(machine.LogIndex, machine.Path, clock, total))
              End If

              'RaiseEvent Vu(Me, New VuEventArgs2(machine.LogIndex, machine.Path, machine.VuLeft, machine.VuRight))
              vuList.Add(New VuEventArgs2(machine.LogIndex, machine.Path, machine.VuLeft, machine.VuRight))

              ' The following is a watchdog method to prevent audio that is "started" but not actually playing from
              ' tanking the system.  It will essentially eject the current audio and reset the system.

              'Dim played As Integer = (totalTime - timeLeft).TotalSeconds + 1
              'Dim watchDog As Integer = DateDiff(DateInterval.Second, machine.Created, Now)

              'If played < watchDog Then

              '  RaiseEvent AudioLog(Me, New AudioLogEventArgs2(machine.Path, "Remove from mixer."))

              '  m_mixer.RemoveMixerInput(machine.SampleProvider)
              '  machine.SampleProvider = Nothing

              '  If machine.WaveStream IsNot Nothing Then

              '    RaiseEvent AudioLog(Me, New AudioLogEventArgs2(machine.Path, "Dispose stream."))

              '    machine.WaveStream.Close()
              '    machine.WaveStream.Dispose()
              '    machine.WaveStream = Nothing
              '  End If

              '  exList.Add(New Removal With {.Path = machine.Path,
              '                               .EomFired = machine.EomFired})

              '  removeList.Add(New Removal With {.Path = machine.Path,
              '                                   .EomFired = machine.EomFired})

              '  m_machines.RemoveAt(index)

              'End If

            End If

          Next

          'Dim handlerRemoved As Boolean = False

          If Me.m_machines.Count = 0 Then

            Debug.WriteLine(String.Format("{0} {1} Timer()... RemoveHandler PlaybackStopped().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
            RemoveHandler Me.WaveOut.PlaybackStopped, AddressOf Me.WaveOut_PlaybackStopped
            If Me.WaveOut IsNot Nothing AndAlso
               Me.WaveOut.PlaybackState = PlaybackState.Playing Then
              Debug.WriteLine(String.Format("{0} {1} Timer()... WaveOut.Stop().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
              Me.WaveOut.Stop()
            End If
            Debug.WriteLine(String.Format("{0} {1} Timer()... WaveOut.Dispose().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
            Me.WaveOut.Dispose()
            Me.WaveOut = Nothing

          End If

          'If m_waveOut IsNot Nothing AndAlso
          '   m_waveOut.PlaybackState = PlaybackState.Playing AndAlso
          '   m_machines.Count = 0 Then

          '  handlerRemoved = True
          '  RemoveHandler m_waveOut.PlaybackStopped, AddressOf m_waveOut_PlaybackStopped

          '  Debug.WriteLine(String.Format("{0} {1} Timer()... WaveOut.Stop().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
          '  m_waveOut.Stop()

          'End If

          'End SyncLock
          'Debug.WriteLine(String.Format("{0} {1} DoStuff()... End SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))

          'If m_machines.Count = 0 Then
          '  Debug.WriteLine(String.Format("{0} {1} Timer()... WaveOut.Dispose().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
          '  m_waveOut.Dispose()
          '  m_waveOut = Nothing
          'End If

          'If handlerRemoved Then
          '  AddHandler m_waveOut.PlaybackStopped, AddressOf m_waveOut_PlaybackStopped
          '  handlerRemoved = False
          'End If

          For Each entry In clockList
            RaiseEvent Clock(Me, entry)
          Next

          For Each entry In vuList
            RaiseEvent Vu(Me, entry)
          Next

          For Each entry In eomList
            'Debug.WriteLine(String.Format("{0} Timer()... RaiseEvent AudioEom({1}).", Now, entry.LogIndex))
            'RaiseEvent AudioEom(Me, New AudioEventArgs2(entry.LogIndex, entry.Path))
            RaiseEvent AudioEom(Me, entry)
          Next

          For Each entry In finishedList
            'Debug.WriteLine(String.Format("{0} Timer()... RaiseEvent AudioFinished({1}).", Now, entry.LogIndex))
            RaiseEvent AudioFinished(Me, entry)
          Next

          For Each entry In closedList
            'Debug.WriteLine(String.Format("{0} Timer()... RaiseEvent AudioClosed({1}).", Now, entry.LogIndex))
            RaiseEvent AudioClosed(Me, entry)
          Next

          'ElseIf m_waveOut.PlaybackState = PlaybackState.Stopped Then

          '  Dim finishedList As New List(Of AudioEventArgs2)
          '  Dim closedList As New List(Of AudioEventArgs2)
          '  Dim eomList As New List(Of AudioEventArgs2)

          '  'Debug.WriteLine(String.Format("{0} {1} DoStuff()... SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
          '  'SyncLock m_lock

          '  If m_machines.Count > 0 Then

          '    Debug.WriteLine(String.Format("{0} {1} Timer(stopped)... RemoveAllMixerInputs().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
          '    m_mixer.RemoveAllMixerInputs()

          '    For index As Integer = m_machines.Count - 1 To 0 Step -1

          '      Debug.WriteLine(String.Format("{0} {1} Timer(stopped)... EomFired Check. ({2} = {3})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, m_machines(index).LogIndex, m_machines(index).EomFired))
          '      If Not m_machines(index).EomFired Then
          '        eomList.Add(New AudioEventArgs2(m_machines(index).LogIndex, m_machines(index).Path))
          '      End If

          '      m_machines(index).SampleProvider = Nothing
          '      'RaiseEvent AudioFinished(Me, New AudioEventArgs2(m_machines(index).LogIndex, m_machines(index).Path))
          '      finishedList.Add(New AudioEventArgs2(m_machines(index).LogIndex, m_machines(index).Path))

          '      If m_machines(index).WaveStream IsNot Nothing Then
          '        m_machines(index).WaveStream.Close()
          '        Debug.WriteLine(String.Format("{0} {1} Timer(stopped)... WaveStream.Dispose(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, m_machines(index).LogIndex))
          '        m_machines(index).WaveStream.Dispose()
          '        m_machines(index).WaveStream = Nothing
          '        'RaiseEvent AudioClosed(Me, New AudioEventArgs2(m_machines(index).LogIndex, m_machines(index).Path))
          '        closedList.Add(New AudioEventArgs2(m_machines(index).LogIndex, m_machines(index).Path))
          '      End If

          '    Next

          '    m_machines.Clear()

          '  End If

          '  'End SyncLock
          '  'Debug.WriteLine(String.Format("{0} {1} DoStuff()... End SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))

          '  For Each entry In finishedList
          '    'Debug.WriteLine(String.Format("{0} Timer(stopped)... RaiseEvent AudioFinished({1}).", Now, entry.LogIndex))
          '    RaiseEvent AudioFinished(Me, entry)
          '  Next

          '  For Each entry In closedList
          '    'Debug.WriteLine(String.Format("{0} Timer(stopped)... RaiseEvent AudioClosed({1}).", Now, entry.LogIndex))
          '    RaiseEvent AudioClosed(Me, entry)
          '  Next

          '  For Each entry In eomList
          '    'Debug.WriteLine(String.Format("{0} Timer(stopped)... RaiseEvent AudioEom({1}).", Now, entry.LogIndex))
          '    RaiseEvent AudioEom(Me, entry)
          '  Next

        End If

      End If

      'Finally
      '  Debug.WriteLine(String.Format("{0} {1} DoStuff()... SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
      '  SyncLock m_timerLock
      '    m_suppressTimer.RemoveAt(0)
      '  End SyncLock
      '  Debug.WriteLine(String.Format("{0} {1} DoStuff()... End SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
      'End Try

    Finally
      Me.m_suppressTimer = False 'm_suppressTimer.RemoveAt(0)
    End Try

  End Sub

  Private Sub Timer_Elapsed(sender As Object, e As System.Timers.ElapsedEventArgs) Handles TimerInternal.Elapsed

    'Debug.WriteLine(String.Format("{0} {1} Timer()... ", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))

    If Me.m_syncContext IsNot Nothing Then
      Try
        Me.m_syncContext?.Post(AddressOf Me.HandleTimer, Nothing)
      Catch
        '?????
      End Try
    Else
      Me.HandleTimer(Nothing)
    End If

  End Sub

  'Private Class PassOn
  '  Public Property Path As String
  '  Public Property EomFired As Boolean
  '  Public Property LogIndex As Integer?
  'End Class

  Private Sub WaveOut_PlaybackStopped(sender As Object, e As StoppedEventArgs) 'Handles m_waveOut.PlaybackStopped

    'Debug.WriteLine(String.Format("{0} {1} PlaybackStopped())... SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    'SyncLock m_timerLock
    '  m_suppressTimer.Add(True)
    'End SyncLock
    'Debug.WriteLine(String.Format("{0} {1} PlaybackStopped())... End SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))

    Debug.WriteLine(String.Format("{0} {1} PlaybackStopped()...", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))

    'Try

    If e.Exception Is Nothing Then

      Dim finishedList As New List(Of AudioEventArgs2)
      Dim closedList As New List(Of AudioEventArgs2)
      Dim eomList As New List(Of AudioEventArgs2)

      'Debug.WriteLine(String.Format("{0} {1} PlaybackStopped())... SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
      'SyncLock m_lock

      If Me.m_machines.Count > 0 Then

        ' Playback stopped, reached end of stream.

        For index As Integer = Me.m_machines.Count - 1 To 0 Step -1

          Debug.WriteLine(String.Format("{0} {1} PlaybackStopped()... EomFired Check. ({2} = {3})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, Me.m_machines(index).LogIndex, Me.m_machines(index).EomFired))
          If Not Me.m_machines(index).EomFired Then
            eomList.Add(New AudioEventArgs2(Me.m_machines(index).LogIndex, Me.m_machines(index).Path))
          End If

          Debug.WriteLine(String.Format("{0} {1} PlaybackStopped()... RemoveMixerInput(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, Me.m_machines(index).LogIndex))
          Me.m_mixer.RemoveMixerInput(Me.m_machines(index).SampleProvider)
          Me.m_machines(index).SampleProvider = Nothing
          'RaiseEvent AudioFinished(Me, New AudioEventArgs2(m_machines(index).LogIndex, m_machines(index).Path))
          finishedList.Add(New AudioEventArgs2(Me.m_machines(index).LogIndex, Me.m_machines(index).Path))

          If Me.m_machines(index).WaveStream IsNot Nothing Then
            Me.m_machines(index).WaveStream.Close()
            Debug.WriteLine(String.Format("{0} {1} PlaybackStopped()... WaveStream.Dispose(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, Me.m_machines(index).LogIndex))
            Me.m_machines(index).WaveStream.Dispose()
            Me.m_machines(index).WaveStream = Nothing
            'RaiseEvent AudioClosed(Me, New AudioEventArgs2(m_machines(index).LogIndex, m_machines(index).Path))
            closedList.Add(New AudioEventArgs2(Me.m_machines(index).LogIndex, Me.m_machines(index).Path))
          End If

          Debug.WriteLine(String.Format("{0} {1} PlaybackStopped()... Machines Remove(). ({2})", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, Me.m_machines(index).LogIndex))
          Me.m_machines.Remove(Me.m_machines(index))

        Next

        'Note: Somehow waveOut can become nothing at this point; I don't think it's part of my code.

        If Me.WaveOut IsNot Nothing AndAlso
           Me.WaveOut.PlaybackState = PlaybackState.Playing AndAlso
           Me.m_machines.Count = 0 Then
          Debug.WriteLine(String.Format("{0} {1} PlaybackStopped()... WaveOut.Stop().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
          Me.WaveOut.Stop()
        End If

        ' Since everything has stopped, let's reset the WaveOut object.
        If Me.WaveOut IsNot Nothing Then
          Debug.WriteLine(String.Format("{0} {1} PlaybackStopped()... RemoveHandler PlaybackStopped().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
          RemoveHandler Me.WaveOut.PlaybackStopped, AddressOf Me.WaveOut_PlaybackStopped
          Debug.WriteLine(String.Format("{0} {1} PlaybackStopped()... WaveOut.Dispose().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
          Me.WaveOut.Dispose()
          Me.WaveOut = Nothing
        End If

      Else

        ' Do nothing... nothing was playing and no exception so all appears to be "normal".
        If Me.WaveOut IsNot Nothing Then
          Debug.WriteLine(String.Format("{0} {1} PlaybackStopped()... RemoveHandler PlaybackStopped().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
          RemoveHandler Me.WaveOut.PlaybackStopped, AddressOf Me.WaveOut_PlaybackStopped
          Debug.WriteLine(String.Format("{0} {1} PlaybackStopped()... WaveOut.Dispose().", Now, System.Threading.Thread.CurrentThread.ManagedThreadId))
          Me.WaveOut.Dispose()
          Me.WaveOut = Nothing
        End If

      End If

      'End SyncLock
      'Debug.WriteLine(String.Format("{0} {1} PlaybackStopped())... End SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))

      For Each entry In eomList
        'Debug.WriteLine(String.Format("{0} PlaybackStopped()... RaiseEvent AudioEom({1}).", Now, eom.LogIndex))
        RaiseEvent AudioEom(Me, entry)
      Next

      For Each entry In finishedList
        'Debug.WriteLine(String.Format("{0} PlaybackStopped()... RaiseEvent AudioFinished({1}).", Now, finished.LogIndex))
        RaiseEvent AudioFinished(Me, entry)
      Next

      For Each entry In finishedList
        'Debug.WriteLine(String.Format("{0} PlaybackStopped()... RaiseEvent AudioClosed({1}).", Now, closed.LogIndex))
        RaiseEvent AudioClosed(Me, entry)
      Next

    Else

      Debug.WriteLine(String.Format("{0} {1} PlaybackStopped()... Exception: {2}", Now, System.Threading.Thread.CurrentThread.ManagedThreadId, e.Exception.Message))
      Stop

    End If

    'Finally
    '  Debug.WriteLine(String.Format("{0} {1} PlaybackStopped())... SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    '  SyncLock m_timerLock
    '    m_suppressTimer.RemoveAt(0)
    '  End SyncLock
    '  Debug.WriteLine(String.Format("{0} {1} PlaybackStopped())... End SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    'End Try

  End Sub

#Region "OnPostVolumeMeter()"

  Private Class VuState
    Public Property Sender As Object
    Public Property Left As Single
    Public Property Right As Single
  End Class

  Private Sub HandleVU(state As Object)

    'SyncLock m_timerLock
    'If m_suppressTimer.Count > 0 Then
    '  Return
    'End If
    'End SyncLock

    'Debug.WriteLine(String.Format("{0} {1} HandleVU()... SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    'SyncLock m_timerLock
    '  m_suppressTimer.Add(True)
    'End SyncLock
    'Debug.WriteLine(String.Format("{0} {1} HandleVU()... End SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))

    'Try

    ' Modified so that the VU values are
    ' stored as member variables and the Vu event
    ' has been moved into the timer.
    ' This was done to get around the issue
    ' where mouse clicking on the window title bar or
    ' opening a new window was causing audio stutter.

    'Debug.WriteLine(String.Format("{0} {1} HandleVU())... SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    'SyncLock m_lock

    Dim vu = TryCast(state, VuState)

    If vu IsNot Nothing Then

      Dim matches = From p In Me.m_machines Where vu.Sender Is p.SampleProvider

      If matches.Count = 1 Then
        matches(0).VuLeft = vu.Left
        matches(0).VuRight = vu.Right
      Else
        ' Ignore?
        'Stop
      End If

    End If

    'End SyncLock
    'Debug.WriteLine(String.Format("{0} {1} HandleVU())... End SyncLock m_lock", Now, Threading.Thread.CurrentThread.ManagedThreadId))

    'Finally
    '  Debug.WriteLine(String.Format("{0} {1} HandleVU())... SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    '  SyncLock m_timerLock
    '    m_suppressTimer.RemoveAt(0)
    '  End SyncLock
    '  Debug.WriteLine(String.Format("{0} {1} HandleVU())... End SyncLock m_timerLock", Now, Threading.Thread.CurrentThread.ManagedThreadId))
    'End Try

  End Sub

  Private Sub OnPostVolumeMeter(sender As Object, e As SampleProviders.StreamVolumeEventArgs)

    'NOTE: Different Thread 

    Dim l As Single
    Dim r As Single

    If e.MaxSampleValues.Length > 1 Then
      l = e.MaxSampleValues(0)
      r = e.MaxSampleValues(1)
    Else
      l = e.MaxSampleValues(0)
      r = e.MaxSampleValues(0)
    End If

    Dim vu = New VuState() With {.Sender = sender, .Left = l, .Right = r}

    If Me.m_syncContext IsNot Nothing Then
      Me.m_syncContext.Post(AddressOf Me.HandleVU, vu)
    Else
      Me.HandleVU(vu)
    End If

  End Sub

#End Region

End Class