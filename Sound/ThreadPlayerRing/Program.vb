Imports System
Imports System.Collections.Concurrent
Imports System.Runtime.InteropServices
Imports System.Threading

Module AudioPlayer

  ' Audio constants
  Private Const SAMPLE_RATE As Integer = 44100
  Private Const BITS_PER_SAMPLE As Integer = 16
  Private Const CHANNELS As Integer = 1
  Private Const BLOCK_ALIGN As Integer = (BITS_PER_SAMPLE \ 8) * CHANNELS
  Private Const FORMAT_PCM As Short = 1

  ' Buffer constants
  Private Const BUFFER_SECONDS As Integer = 2
  Private Const SEGMENTS As Integer = 8
  Private Const SEGMENT_SIZE As Integer = (SAMPLE_RATE * BUFFER_SECONDS * BLOCK_ALIGN) \ SEGMENTS

  ' Windows API constants
  Private Const CALLBACK_FUNCTION As Integer = &H30000
  Private Const WHDR_DONE As Integer = &H1

  ' Delegate for waveOut callback
  Private Delegate Sub WaveOutProcDelegate(hwo As IntPtr, uMsg As UInteger, dwInstance As IntPtr, dwParam1 As IntPtr, dwParam2 As IntPtr)

  ' WAVEFORMATEX structure
  <StructLayout(LayoutKind.Sequential)>
  Private Structure WAVEFORMATEX
    Public wFormatTag As Short
    Public nChannels As Short
    Public nSamplesPerSec As Integer
    Public nAvgBytesPerSec As Integer
    Public nBlockAlign As Short
    Public wBitsPerSample As Short
    Public cbSize As Short
  End Structure

  ' WAVEHDR structure
  <StructLayout(LayoutKind.Sequential)>
  Private Structure WAVEHDR
    Public lpData As IntPtr
    Public dwBufferLength As Integer
    Public dwBytesRecorded As Integer
    Public dwUser As IntPtr
    Public dwFlags As Integer
    Public dwLoops As Integer
    Public lpNext As IntPtr
    Public reserved As IntPtr
  End Structure

  ' Import WinMM functions
  <DllImport("winmm.dll")>
  Private Function waveOutOpen(ByRef hWaveOut As IntPtr, uDeviceID As Integer,
                              ByRef lpFormat As WAVEFORMATEX, dwCallback As IntPtr,
                              dwInstance As IntPtr, dwFlags As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutPrepareHeader(hWaveOut As IntPtr, ByRef lpWaveOutHdr As WAVEHDR, uSize As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutWrite(hWaveOut As IntPtr, ByRef lpWaveOutHdr As WAVEHDR, uSize As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutUnprepareHeader(hWaveOut As IntPtr, ByRef lpWaveOutHdr As WAVEHDR, uSize As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutClose(hWaveOut As IntPtr) As Integer
  End Function

  ' Playback state
  Private waveOutHandle As IntPtr = IntPtr.Zero
  Private buffers(SEGMENTS - 1) As WAVEHDR
  Private buffersPinned(SEGMENTS - 1)() As Byte
  Private bufferGCHandles(SEGMENTS - 1) As GCHandle

  Private bufferDoneFlags(SEGMENTS - 1) As Boolean
  Private bufferDoneEvent As New AutoResetEvent(False)

  Private activeTones As New List(Of ActiveTone)
  Private toneQueue As New ConcurrentQueue(Of ToneRequest)

  Private running As Boolean = True

  Private callbackDelegate As WaveOutProcDelegate

  ' Tone request and active tone classes
  Private Class ToneRequest
    Public Frequency As Integer
    Public Duration As Integer
  End Class

  Private Class ActiveTone
    Public Frequency As Integer
    Public SamplesRemaining As Integer
    Public Phase As Double
  End Class

  Sub Main()
    Try
      InitWaveOut()

      ' Start playback thread
      Dim playbackThread As New Thread(AddressOf PlaybackLoop)
      playbackThread.IsBackground = True
      playbackThread.Start()

      Console.WriteLine("Continuous buffer polyphonic SOUND player ready.")
      Console.WriteLine("Enter commands: SOUND duration_ms, frequency_hz")
      Console.WriteLine("Type EXIT to quit.")

      While True
        Dim line = Console.ReadLine()
        If line Is Nothing Then Continue While
        Dim cmd = line.Trim().ToUpperInvariant()
        If cmd = "EXIT" Then Exit While

        If cmd.StartsWith("SOUND") Then
          Dim args = line.Substring(5).Split(","c)
          If args.Length = 2 AndAlso
             Integer.TryParse(args(0).Trim(), Nothing) AndAlso
             Integer.TryParse(args(1).Trim(), Nothing) Then

            Dim dur = Integer.Parse(args(0).Trim())
            Dim freq = Integer.Parse(args(1).Trim())

            toneQueue.Enqueue(New ToneRequest With {
              .Duration = dur,
              .Frequency = freq
            })
          Else
            Console.WriteLine("Invalid SOUND syntax. Usage: SOUND duration_ms, frequency_hz")
          End If
        ElseIf Not String.IsNullOrWhiteSpace(line) Then
          Console.WriteLine("Unknown command.")
        End If
      End While

      running = False
      bufferDoneEvent.Set() ' wake playback thread if waiting
      playbackThread.Join()

    Finally
      Cleanup()
    End Try
  End Sub

  Private Sub InitWaveOut()
    Dim format As New WAVEFORMATEX With {
      .wFormatTag = FORMAT_PCM,
      .nChannels = CHANNELS,
      .nSamplesPerSec = SAMPLE_RATE,
      .wBitsPerSample = BITS_PER_SAMPLE,
      .nBlockAlign = BLOCK_ALIGN,
      .nAvgBytesPerSec = SAMPLE_RATE * BLOCK_ALIGN,
      .cbSize = 0
    }

    ' Setup callback delegate and pointer
    callbackDelegate = AddressOf WaveOutCallback
    Dim callbackPtr = Marshal.GetFunctionPointerForDelegate(callbackDelegate)

    Dim result = waveOutOpen(waveOutHandle, -1, format, callbackPtr, IntPtr.Zero, CALLBACK_FUNCTION)
    Console.WriteLine($"waveOutOpen result: {result}, handle: {waveOutHandle}")
    If result <> 0 OrElse waveOutHandle = IntPtr.Zero Then
      Throw New Exception($"waveOutOpen failed: {result} handle: {waveOutHandle}")
    End If

    ' Allocate and prepare buffers
    For i = 0 To SEGMENTS - 1
      buffersPinned(i) = New Byte(SEGMENT_SIZE - 1) {}
      bufferGCHandles(i) = GCHandle.Alloc(buffersPinned(i), GCHandleType.Pinned)

      buffers(i) = New WAVEHDR With {
        .lpData = bufferGCHandles(i).AddrOfPinnedObject(),
        .dwBufferLength = SEGMENT_SIZE,
        .dwFlags = 0,
        .dwLoops = 0,
        .dwUser = IntPtr.Zero,
        .lpNext = IntPtr.Zero,
        .reserved = IntPtr.Zero
      }

      Dim prepResult = waveOutPrepareHeader(waveOutHandle, buffers(i), Marshal.SizeOf(buffers(i)))
      If prepResult <> 0 Then
        Console.WriteLine($"waveOutPrepareHeader failed on buffer {i}: {prepResult}")
        Throw New Exception($"waveOutPrepareHeader failed on buffer {i}: {prepResult}")
      End If

      bufferDoneFlags(i) = True ' mark all buffers done initially so playback thread fills & writes

    Next
  End Sub

  Private Sub PlaybackLoop()
    ' Start by writing all buffers once to queue them
    For i = 0 To SEGMENTS - 1
      FillBuffer(buffersPinned(i))
      bufferDoneFlags(i) = False
      Dim writeResult = waveOutWrite(waveOutHandle, buffers(i), Marshal.SizeOf(buffers(i)))
      If writeResult <> 0 Then Console.WriteLine($"waveOutWrite failed on init buffer {i}: {writeResult}")
    Next

    While running
      ' Enqueue new tones
      Dim tone As ToneRequest
      While toneQueue.TryDequeue(tone)
        SyncLock activeTones
          activeTones.Add(New ActiveTone With {
            .Frequency = tone.Frequency,
            .SamplesRemaining = (tone.Duration * SAMPLE_RATE) \ 1000,
            .Phase = 0
          })
        End SyncLock
      End While

      ' Wait for any buffer done signal from callback (or timeout)
      bufferDoneEvent.WaitOne(50)

      ' Refill and requeue done buffers
      For i = 0 To SEGMENTS - 1
        If bufferDoneFlags(i) Then
          FillBuffer(buffersPinned(i))
          buffers(i).dwFlags = 0
          bufferDoneFlags(i) = False
          Dim writeResult = waveOutWrite(waveOutHandle, buffers(i), Marshal.SizeOf(buffers(i)))
          If writeResult <> 0 Then Console.WriteLine($"waveOutWrite failed on buffer {i}: {writeResult}")
        End If
      Next
    End While
  End Sub

  ' This callback is called by waveOut when buffer finishes playing
  Private Sub WaveOutCallback(hwo As IntPtr, uMsg As UInteger, dwInstance As IntPtr, dwParam1 As IntPtr, dwParam2 As IntPtr)
    Const WOM_DONE As UInteger = &H3BD
    If uMsg = WOM_DONE Then
      Dim hdrPtr = dwParam1
      For i = 0 To SEGMENTS - 1
        ' Check if this is the buffer that finished
        If buffers(i).lpData = Marshal.ReadIntPtr(hdrPtr, 0) OrElse
           buffers(i).lpData = hdrPtr Then
          bufferDoneFlags(i) = True
          bufferDoneEvent.Set()
          Exit For
        End If
      Next
    End If
  End Sub

  Private Sub FillBuffer(buffer() As Byte)
    Dim sampleCount = buffer.Length \ 2
    Dim mixed(sampleCount - 1) As Double

    SyncLock activeTones
      For Each tone In activeTones.ToList()
        Dim phaseStep = 2 * Math.PI * tone.Frequency / SAMPLE_RATE
        For i = 0 To sampleCount - 1
          If tone.SamplesRemaining <= 0 Then Exit For
          mixed(i) += Math.Sin(tone.Phase)
          tone.Phase += phaseStep
          If tone.Phase > 2 * Math.PI Then tone.Phase -= 2 * Math.PI
          tone.SamplesRemaining -= 1
        Next
      Next

      activeTones.RemoveAll(Function(t) t.SamplesRemaining <= 0)
    End SyncLock

    Dim scale = Math.Max(1, activeTones.Count)
    For i = 0 To sampleCount - 1
      Dim sample = Math.Max(-1.0, Math.Min(1.0, mixed(i)))
      Dim val As Short = CShort(sample * Short.MaxValue / scale)
      Dim bytes = BitConverter.GetBytes(val)
      buffer(i * 2) = bytes(0)
      buffer(i * 2 + 1) = bytes(1)
    Next
  End Sub

  Private Sub Cleanup()
    Try
      waveOutClose(waveOutHandle)
    Catch
    End Try

    For i = 0 To SEGMENTS - 1
      Try
        If bufferGCHandles(i).IsAllocated Then
          waveOutUnprepareHeader(waveOutHandle, buffers(i), Marshal.SizeOf(buffers(i)))
          bufferGCHandles(i).Free()
        End If
      Catch
      End Try
    Next
  End Sub

End Module
