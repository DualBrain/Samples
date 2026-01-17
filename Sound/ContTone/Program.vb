Imports System
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading

Module WaveOutEnumAndPlay

  ' Constants
  Private Const SAMPLE_RATE As Integer = 44100
  Private Const BITS_PER_SAMPLE As Integer = 16
  Private Const CHANNELS As Integer = 1
  Private Const BLOCK_ALIGN As Integer = (BITS_PER_SAMPLE \ 8) * CHANNELS
  Private Const FORMAT_PCM As Short = 1
  Private Const BUFFER_SECONDS As Integer = 1
  Private Const BUFFER_SIZE As Integer = SAMPLE_RATE * BLOCK_ALIGN * BUFFER_SECONDS
  Private Const NUM_BUFFERS As Integer = 3
  Private Const WHDR_DONE As Integer = &H1

  <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)>
  Private Structure WAVEOUTCAPS
    Public wMid As Short
    Public wPid As Short
    Public vDriverVersion As Integer
    <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
    Public szPname As String
    Public dwFormats As Integer
    Public wChannels As Short
    Public wReserved1 As Short
    Public dwSupport As Integer
  End Structure

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

  <StructLayout(LayoutKind.Sequential)>
  Private Class WAVEHDR
    Public lpData As IntPtr
    Public dwBufferLength As Integer
    Public dwBytesRecorded As Integer
    Public dwUser As IntPtr
    Public dwFlags As Integer
    Public dwLoops As Integer
    Public lpNext As IntPtr
    Public reserved As IntPtr
  End Class

  <DllImport("winmm.dll")>
  Private Function waveOutReset(hWaveOut As IntPtr) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutGetNumDevs() As Integer
  End Function

  <DllImport("winmm.dll", CharSet:=CharSet.Ansi)>
  Private Function waveOutGetDevCaps(uDeviceID As Integer, ByRef pwoc As WAVEOUTCAPS, cbwoc As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutOpen(ByRef hWaveOut As IntPtr, uDeviceID As Integer,
                              ByRef lpFormat As WAVEFORMATEX, dwCallback As IntPtr,
                              dwInstance As IntPtr, dwFlags As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutPrepareHeader(hWaveOut As IntPtr, lpWaveOutHdr As WAVEHDR, uSize As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutWrite(hWaveOut As IntPtr, lpWaveOutHdr As WAVEHDR, uSize As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutUnprepareHeader(hWaveOut As IntPtr, lpWaveOutHdr As WAVEHDR, uSize As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutClose(hWaveOutHandle As IntPtr) As Integer
  End Function

  Sub Main()
    ' Enumerate waveOut devices
    Dim numDevices = waveOutGetNumDevs()
    Console.WriteLine($"Number of waveOut devices: {numDevices}")
    For i = 0 To numDevices - 1
      Dim caps As New WAVEOUTCAPS()
      Dim res = waveOutGetDevCaps(i, caps, Marshal.SizeOf(GetType(WAVEOUTCAPS)))
      If res = 0 Then
        Console.WriteLine($"Device {i}: {caps.szPname}")
      Else
        Console.WriteLine($"Device {i}: Failed to get device caps (error {res})")
      End If
    Next

    Dim waveOutHandle As IntPtr = IntPtr.Zero

    Try
      Dim format As New WAVEFORMATEX With {
        .wFormatTag = FORMAT_PCM,
        .nChannels = CHANNELS,
        .nSamplesPerSec = SAMPLE_RATE,
        .wBitsPerSample = BITS_PER_SAMPLE,
        .nBlockAlign = BLOCK_ALIGN,
        .nAvgBytesPerSec = SAMPLE_RATE * BLOCK_ALIGN,
        .cbSize = 0
      }

      Console.WriteLine("Opening waveOut device 0 explicitly.")
      Dim result = waveOutOpen(waveOutHandle, 0, format, IntPtr.Zero, IntPtr.Zero, 0)
      Console.WriteLine($"waveOutOpen returned {result}, handle: {waveOutHandle}")
      If result <> 0 OrElse waveOutHandle = IntPtr.Zero Then
        Console.WriteLine("Failed to open wave out device 0")
        Return
      End If

      Dim buffers(NUM_BUFFERS - 1)() As Byte
      Dim bufferHandles(NUM_BUFFERS - 1) As GCHandle
      Dim headers(NUM_BUFFERS - 1) As WAVEHDR

      For i = 0 To NUM_BUFFERS - 1
        buffers(i) = New Byte(BUFFER_SIZE - 1) {}
        FillSineWave(buffers(i), 440, SAMPLE_RATE)

        bufferHandles(i) = GCHandle.Alloc(buffers(i), GCHandleType.Pinned)
        headers(i) = New WAVEHDR With {
          .lpData = bufferHandles(i).AddrOfPinnedObject(),
          .dwBufferLength = BUFFER_SIZE,
          .dwFlags = 0,
          .dwLoops = 0
        }

        Dim prepResult = waveOutPrepareHeader(waveOutHandle, headers(i), Marshal.SizeOf(GetType(WAVEHDR)))
        If prepResult <> 0 Then
          Console.WriteLine($"waveOutPrepareHeader failed on buffer {i}: {prepResult}")
          Cleanup(waveOutHandle, headers, bufferHandles, i)
          Return
        End If
      Next

      ' Write all buffers once
      For i = 0 To NUM_BUFFERS - 1
        Console.WriteLine($"Writing buffer {i}, dwFlags=0x{headers(i).dwFlags:X8}")
        Dim writeResult = waveOutWrite(waveOutHandle, headers(i), Marshal.SizeOf(GetType(WAVEHDR)))
        Console.WriteLine($"waveOutWrite returned {writeResult} on buffer {i}")
        If writeResult <> 0 Then
          Console.WriteLine($"waveOutWrite failed on buffer {i}: {writeResult}")
          Cleanup(waveOutHandle, headers, bufferHandles, NUM_BUFFERS)
          Return
        End If
      Next

      Console.WriteLine("Playing continuous 440Hz tone. Press any key to stop.")

      Dim currentBuffer As Integer = 0

      'While Not Console.KeyAvailable
      '  While (headers(currentBuffer).dwFlags And WHDR_DONE) = 0
      '    Thread.Sleep(10)
      '    If Console.KeyAvailable Then Exit While
      '  End While

      '  If Console.KeyAvailable Then Exit While

      '  headers(currentBuffer).dwFlags = 0
      '  Console.WriteLine($"Rewriting buffer {currentBuffer}, dwFlags=0x{headers(currentBuffer).dwFlags:X8}")
      '  Dim writeResult = waveOutWrite(waveOutHandle, headers(currentBuffer), Marshal.SizeOf(GetType(WAVEHDR)))
      '  Console.WriteLine($"waveOutWrite returned {writeResult} on buffer {currentBuffer} in loop")
      '  If writeResult <> 0 Then
      '    Console.WriteLine($"waveOutWrite failed in loop on buffer {currentBuffer}: {writeResult}")
      '    Exit While
      '  End If

      '  currentBuffer = (currentBuffer + 1) Mod NUM_BUFFERS
      'End While

      While Not Console.KeyAvailable
        While (headers(currentBuffer).dwFlags And WHDR_DONE) = 0
          Thread.Sleep(10)
          If Console.KeyAvailable Then Exit While
        End While

        If Console.KeyAvailable Then Exit While

        headers(currentBuffer).dwFlags = 0
        Dim writeResult = waveOutWrite(waveOutHandle, headers(currentBuffer), Marshal.SizeOf(GetType(WAVEHDR)))
        If writeResult = 34 Then
          Console.WriteLine("Error 34 detected: resetting waveOut device")
          waveOutReset(waveOutHandle)
          headers(currentBuffer).dwFlags = WHDR_DONE ' Force done so we can requeue
        ElseIf writeResult <> 0 Then
          Console.WriteLine($"waveOutWrite failed in loop on buffer {currentBuffer}: {writeResult}")
          Exit While
        End If

        currentBuffer = (currentBuffer + 1) Mod NUM_BUFFERS
      End While

      Console.ReadKey(True)

      Cleanup(waveOutHandle, headers, bufferHandles, NUM_BUFFERS)

      Console.WriteLine("Playback stopped. Exiting.")

    Catch ex As Exception
      Console.WriteLine("Exception: " & ex.Message)
    End Try
  End Sub

  Private Sub Cleanup(hWaveOut As IntPtr, headers() As WAVEHDR, handles1() As GCHandle, count As Integer)
    For i = 0 To count - 1
      Try
        waveOutUnprepareHeader(hWaveOut, headers(i), Marshal.SizeOf(GetType(WAVEHDR)))
      Catch ex As Exception
        ' Ignore cleanup errors
      End Try
      If handles1(i).IsAllocated Then handles1(i).Free()
    Next
    waveOutClose(hWaveOut)
  End Sub

  Private Sub FillSineWave(buffer() As Byte, frequency As Double, sampleRate As Integer)
    Dim samples = buffer.Length \ 2 ' 16-bit samples
    Dim phaseStep = 2 * Math.PI * frequency / sampleRate
    Dim phase As Double = 0

    For i = 0 To samples - 1
      Dim sampleValue = Math.Sin(phase)
      phase += phaseStep
      If phase > 2 * Math.PI Then phase -= 2 * Math.PI

      Dim sampleInt16 As Short = CShort(sampleValue * Short.MaxValue)
      buffer(i * 2) = CByte(sampleInt16 And &HFF)
      buffer(i * 2 + 1) = CByte((sampleInt16 >> 8) And &HFF)
    Next
  End Sub

End Module
