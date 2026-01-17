Imports System
Imports System.Runtime.InteropServices
Imports System.Threading

Module MinimalWaveOutTest

  ' Audio format constants
  Private Const SAMPLE_RATE As Integer = 44100
  Private Const BITS_PER_SAMPLE As Integer = 16
  Private Const CHANNELS As Integer = 1
  Private Const BLOCK_ALIGN As Integer = (BITS_PER_SAMPLE \ 8) * CHANNELS
  Private Const FORMAT_PCM As Short = 1

  Private Const BUFFER_SECONDS As Integer = 2
  Private Const BUFFER_SIZE As Integer = SAMPLE_RATE * BLOCK_ALIGN * BUFFER_SECONDS

  Private Const WHDR_DONE As Integer = &H1

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
  Private Function waveOutClose(hWaveOut As IntPtr) As Integer
  End Function

  Sub Main()
    Dim waveOutHandle As IntPtr = IntPtr.Zero

    Try
      ' Setup format
      Dim format As New WAVEFORMATEX With {
        .wFormatTag = FORMAT_PCM,
        .nChannels = CHANNELS,
        .nSamplesPerSec = SAMPLE_RATE,
        .wBitsPerSample = BITS_PER_SAMPLE,
        .nBlockAlign = BLOCK_ALIGN,
        .nAvgBytesPerSec = SAMPLE_RATE * BLOCK_ALIGN,
        .cbSize = 0
      }

      ' Open default device
      Dim result = waveOutOpen(waveOutHandle, -1, format, IntPtr.Zero, IntPtr.Zero, 0)
      Console.WriteLine($"waveOutOpen returned {result}, handle: {waveOutHandle}")
      If result <> 0 OrElse waveOutHandle = IntPtr.Zero Then
        Console.WriteLine("Failed to open wave out device")
        Return
      End If

      ' Allocate buffer
      Dim buffer(BUFFER_SIZE - 1) As Byte
      Dim bufferHandle As GCHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned)

      ' Fill buffer with 440Hz sine wave
      FillSineWave(buffer, 440, SAMPLE_RATE)

      ' Prepare header
      Dim header As New WAVEHDR With {
        .lpData = bufferHandle.AddrOfPinnedObject(),
        .dwBufferLength = BUFFER_SIZE,
        .dwFlags = 0,
        .dwLoops = 0
      }

      Dim prepResult = waveOutPrepareHeader(waveOutHandle, header, Marshal.SizeOf(GetType(WAVEHDR)))
      Console.WriteLine($"waveOutPrepareHeader returned {prepResult}")
      If prepResult <> 0 Then
        Console.WriteLine("Failed to prepare header")
        bufferHandle.Free()
        waveOutClose(waveOutHandle)
        Return
      End If

      ' Write buffer to device
      Dim writeResult = waveOutWrite(waveOutHandle, header, Marshal.SizeOf(GetType(WAVEHDR)))
      Console.WriteLine($"waveOutWrite returned {writeResult}")
      If writeResult <> 0 Then
        Console.WriteLine("Failed to write buffer")
        waveOutUnprepareHeader(waveOutHandle, header, Marshal.SizeOf(GetType(WAVEHDR)))
        bufferHandle.Free()
        waveOutClose(waveOutHandle)
        Return
      End If

      ' Wait for playback to complete
      Console.WriteLine("Playing 440Hz tone for 2 seconds...")
      While (header.dwFlags And WHDR_DONE) = 0
        Thread.Sleep(20)
      End While

      Console.WriteLine("Playback finished.")

      ' Cleanup
      waveOutUnprepareHeader(waveOutHandle, header, Marshal.SizeOf(GetType(WAVEHDR)))
      bufferHandle.Free()
      waveOutClose(waveOutHandle)

    Catch ex As Exception
      Console.WriteLine("Exception: " & ex.Message)
    End Try
  End Sub

  Private Sub FillSineWave(buffer() As Byte, frequency As Double, sampleRate As Integer)
    Dim samples = buffer.Length \ 2 ' 16-bit samples
    Dim phaseStep = 2 * Math.PI * frequency / sampleRate
    Dim phase As Double = 0

    For i = 0 To samples - 1
      Dim sampleValue = Math.Sin(phase)
      phase += phaseStep
      If phase > 2 * Math.PI Then phase -= 2 * Math.PI

      ' Scale to Int16
      Dim sampleInt16 As Short = CShort(sampleValue * Short.MaxValue)
      buffer(i * 2) = CByte(sampleInt16 And &HFF)
      buffer(i * 2 + 1) = CByte((sampleInt16 >> 8) And &HFF)
    Next
  End Sub

End Module
