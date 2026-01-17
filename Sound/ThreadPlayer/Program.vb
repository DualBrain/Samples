Imports System.Collections.Concurrent
Imports System.Runtime.InteropServices
Imports System.Threading

Module AudioPlayer

  ' Constants
  Private Const SAMPLE_RATE As Integer = 44100
  Private Const BITS_PER_SAMPLE As Integer = 16
  Private Const CHANNELS As Integer = 1
  Private Const BLOCK_ALIGN As Integer = (BITS_PER_SAMPLE \ 8) * CHANNELS
  Private Const FORMAT_PCM As Short = 1
  Private Const BUFFER_MS As Integer = 200 '50
  Private Const NUM_BUFFERS As Integer = 6 '4

  ' WinMM structures
  <StructLayout(LayoutKind.Sequential)>
  Public Structure WAVEFORMATEX
    Public wFormatTag As Short
    Public nChannels As Short
    Public nSamplesPerSec As Integer
    Public nAvgBytesPerSec As Integer
    Public nBlockAlign As Short
    Public wBitsPerSample As Short
    Public cbSize As Short
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Public Structure WAVEHDR
    Public lpData As IntPtr
    Public dwBufferLength As Integer
    Public dwBytesRecorded As Integer
    Public dwUser As IntPtr
    Public dwFlags As Integer
    Public dwLoops As Integer
    Public lpNext As IntPtr
    Public reserved As IntPtr
  End Structure

  ' WinMM declarations
  <DllImport("winmm.dll")>
  Private Function waveOutOpen(ByRef hWaveOut As IntPtr, ByVal uDeviceID As Integer,
                               ByRef lpFormat As WAVEFORMATEX, ByVal dwCallback As IntPtr,
                               ByVal dwInstance As IntPtr, ByVal dwFlags As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutPrepareHeader(hWaveOut As IntPtr,
                                        ByRef lpWaveOutHdr As WAVEHDR,
                                        ByVal uSize As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutWrite(hWaveOut As IntPtr,
                                ByRef lpWaveOutHdr As WAVEHDR,
                                ByVal uSize As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutUnprepareHeader(hWaveOut As IntPtr,
                                          ByRef lpWaveOutHdr As WAVEHDR,
                                          ByVal uSize As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutClose(hWaveOut As IntPtr) As Integer
  End Function

  ' Playback state
  Private waveOutHandle As IntPtr
  Private bufferSize As Integer = (SAMPLE_RATE * BUFFER_MS * BLOCK_ALIGN) \ 1000
  Private buffers(NUM_BUFFERS - 1) As WAVEHDR
  Private bufferPtrs(NUM_BUFFERS - 1) As IntPtr
  Private activeTones As New List(Of ActiveTone)
  Private toneQueue As New ConcurrentQueue(Of ToneRequest)
  Private running As Boolean = True

  ' Tone classes
  Private Class ToneRequest
    Public Frequency As Integer
    Public Duration As Integer
  End Class

  'Private Class ActiveTone
  '  Public Frequency As Integer
  '  Public SamplesRemaining As Integer
  '  Public SampleIndex As Integer
  'End Class

  Private Class ActiveTone
    Public Frequency As Integer
    Public SamplesRemaining As Integer
    Public Phase As Double ' in radians
  End Class


  ' Entry point
  Sub Main()
    InitWaveOut()

    Dim playbackThread As New Thread(AddressOf PlaybackLoop)
    playbackThread.IsBackground = True
    playbackThread.Start()

    Console.WriteLine("Async SOUND player ready.")
    Console.WriteLine("Type: SOUND duration, frequency (e.g. SOUND 1000, 440)")
    Console.WriteLine("Type EXIT to quit.")

    While True
      Dim line = Console.ReadLine()
      If line?.Trim().ToUpper() = "EXIT" Then Exit While
      If line?.Trim().ToUpper().StartsWith("SOUND") Then
        Dim parts = line.Substring(5).Split(","c)
        If parts.Length = 2 AndAlso Integer.TryParse(parts(0).Trim(), Nothing) AndAlso Integer.TryParse(parts(1).Trim(), Nothing) Then
          toneQueue.Enqueue(New ToneRequest With {
            .Duration = Integer.Parse(parts(0)),
            .Frequency = Integer.Parse(parts(1))
          })
        Else
          Console.WriteLine("Invalid SOUND syntax. Use: SOUND duration, freq")
        End If
      End If
    End While

    running = False
    playbackThread.Join()
    waveOutClose(waveOutHandle)
  End Sub

  ' Init audio output
  Private Sub InitWaveOut()
    Dim fmt As New WAVEFORMATEX With {
      .wFormatTag = FORMAT_PCM,
      .nChannels = CHANNELS,
      .nSamplesPerSec = SAMPLE_RATE,
      .wBitsPerSample = BITS_PER_SAMPLE,
      .nBlockAlign = BLOCK_ALIGN,
      .nAvgBytesPerSec = SAMPLE_RATE * BLOCK_ALIGN,
      .cbSize = 0
    }

    Dim result = waveOutOpen(waveOutHandle, -1, fmt, IntPtr.Zero, IntPtr.Zero, 0)
    If result <> 0 Then
      Throw New Exception("waveOutOpen failed: " & result)
    End If
  End Sub

  ' Playback loop with buffer recycling
  Private Sub PlaybackLoop()
    ' Setup wave buffers
    For i = 0 To NUM_BUFFERS - 1
      bufferPtrs(i) = Marshal.AllocHGlobal(bufferSize)
      buffers(i) = New WAVEHDR With {
        .lpData = bufferPtrs(i),
        .dwBufferLength = bufferSize
      }
      waveOutPrepareHeader(waveOutHandle, buffers(i), Marshal.SizeOf(buffers(i)))
      FillBuffer(bufferPtrs(i), bufferSize)
      waveOutWrite(waveOutHandle, buffers(i), Marshal.SizeOf(buffers(i)))
    Next

    ' Loop to refill completed buffers
    While running
      ' Accept queued tones
      Dim tone As ToneRequest
      While toneQueue.TryDequeue(tone)
        'activeTones.Add(New ActiveTone With {
        '  .Frequency = tone.Frequency,
        '  .SamplesRemaining = (tone.Duration * SAMPLE_RATE) \ 1000,
        '  .SampleIndex = 0
        '})
        activeTones.Add(New ActiveTone With {
          .Frequency = tone.Frequency,
          .SamplesRemaining = (tone.Duration * SAMPLE_RATE) \ 1000,
          .Phase = 0
        })
      End While

      For i = 0 To NUM_BUFFERS - 1
        If (buffers(i).dwFlags And &H1) <> 0 Then ' WHDR_DONE
          FillBuffer(bufferPtrs(i), bufferSize)
          waveOutWrite(waveOutHandle, buffers(i), Marshal.SizeOf(buffers(i)))
        End If
      Next

      Thread.Sleep(10)
    End While

    ' Clean up buffers
    For i = 0 To NUM_BUFFERS - 1
      waveOutUnprepareHeader(waveOutHandle, buffers(i), Marshal.SizeOf(buffers(i)))
      Marshal.FreeHGlobal(bufferPtrs(i))
    Next
  End Sub

  ' Fill buffer with mixed tones or silence
  'Private Sub FillBuffer(ptr As IntPtr, size As Integer)
  '  Dim sampleCount = size \ 2
  '  Dim mixed(sampleCount - 1) As Double

  '  ' Mix all active tones
  '  For Each tone In activeTones.ToList()
  '    For i = 0 To sampleCount - 1
  '      If tone.SamplesRemaining <= 0 Then Exit For

  '      'Dim t = 2 * Math.PI * tone.Frequency * tone.SampleIndex / SAMPLE_RATE
  '      'mixed(i) += Math.Sin(t)
  '      'tone.SampleIndex += 1

  '      Dim phaseStep = 2 * Math.PI * tone.Frequency / SAMPLE_RATE
  '      mixed(i) += Math.Sin(tone.Phase)
  '      tone.Phase += phaseStep
  '      If tone.Phase > 2 * Math.PI Then tone.Phase -= 2 * Math.PI

  '      tone.SamplesRemaining -= 1
  '    Next
  '  Next

  '  ' Remove finished tones
  '  activeTones.RemoveAll(Function(t) t.SamplesRemaining <= 0)

  '  ' Normalize and convert to PCM
  '  Dim output(sampleCount * 2 - 1) As Byte
  '  Dim scale = Math.Max(1, activeTones.Count)
  '  For i = 0 To sampleCount - 1
  '    Dim sample = Math.Max(-1.0, Math.Min(1.0, mixed(i)))
  '    Dim val As Short = CShort(sample * Short.MaxValue / scale)
  '    BitConverter.GetBytes(val).CopyTo(output, i * 2)
  '  Next

  '  Marshal.Copy(output, 0, ptr, size)
  'End Sub

  Private Sub FillBuffer(ptr As IntPtr, size As Integer)
    Dim sampleCount = size \ 2
    Dim mixed(sampleCount - 1) As Double

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

    Dim output(sampleCount * 2 - 1) As Byte
    Dim scale = Math.Max(1, activeTones.Count)
    For i = 0 To sampleCount - 1
      Dim sample = Math.Max(-1.0, Math.Min(1.0, mixed(i)))
      Dim val As Short = CShort(sample * Short.MaxValue / scale)
      BitConverter.GetBytes(val).CopyTo(output, i * 2)
    Next

    Marshal.Copy(output, 0, ptr, size)

  End Sub


End Module
