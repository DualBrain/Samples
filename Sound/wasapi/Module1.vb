' Compact VB.NET tone player using waveOut
Imports System
Imports System.Runtime.InteropServices
Imports System.Threading

Module WaveOutTonePlayer

  Const SAMPLE_RATE = 44100, BITS = 16, CH = 1
  Const BLOCK_ALIGN = CH * BITS \ 8
  Const BUFFER_MS = 200
  Const BUFFER_SIZE = SAMPLE_RATE * BLOCK_ALIGN * BUFFER_MS \ 1000
  Const WHDR_DONE = &H1

  <StructLayout(LayoutKind.Sequential)>
  Structure WAVEFORMATEX
    Public wFormatTag As Short, nChannels As Short
    Public nSamplesPerSec As Integer, nAvgBytesPerSec As Integer
    Public nBlockAlign As Short, wBitsPerSample As Short, cbSize As Short
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Class WAVEHDR
    Public lpData As IntPtr
    Public dwBufferLength As Integer
    Public dwFlags As Integer, dwLoops As Integer
    Public dwUser, lpNext, reserved As IntPtr
  End Class

  Declare Function waveOutOpen Lib "winmm.dll" Alias "waveOutOpen" (ByRef h As IntPtr, uDeviceID As Integer, ByRef f As WAVEFORMATEX, cb As IntPtr, ui2 As IntPtr, uFlags As Integer) As Integer
  Declare Function waveOutPrepareHeader Lib "winmm.dll" (h As IntPtr, ByRef hdr As WAVEHDR, size As Integer) As Integer
  Declare Function waveOutWrite Lib "winmm.dll" (h As IntPtr, ByRef hdr As WAVEHDR, size As Integer) As Integer
  Declare Function waveOutUnprepareHeader Lib "winmm.dll" (h As IntPtr, ByRef hdr As WAVEHDR, size As Integer) As Integer
  Declare Function waveOutClose Lib "winmm.dll" (h As IntPtr) As Integer
  Declare Function waveOutReset Lib "winmm.dll" (h As IntPtr) As Integer

  Sub Main()
    Dim h As IntPtr
    Dim fmt As New WAVEFORMATEX With {.wFormatTag = 1, .nChannels = CH, .nSamplesPerSec = SAMPLE_RATE, .wBitsPerSample = BITS, .nBlockAlign = BLOCK_ALIGN, .nAvgBytesPerSec = SAMPLE_RATE * BLOCK_ALIGN}
    If waveOutOpen(h, -1, fmt, IntPtr.Zero, IntPtr.Zero, 0) <> 0 Then Return
    Dim buf(BUFFER_SIZE - 1) As Byte
    Dim hdr As New WAVEHDR()
    Dim gh = GCHandle.Alloc(buf, GCHandleType.Pinned)
    hdr.lpData = gh.AddrOfPinnedObject() : hdr.dwBufferLength = BUFFER_SIZE

    waveOutPrepareHeader(h, hdr, Marshal.SizeOf(hdr))

    Dim freq = 440, phase = 0.0, step1 = 2 * Math.PI * freq / SAMPLE_RATE

    Console.WriteLine("Playing tone. Press any key to stop.")
    While Not Console.KeyAvailable
      For i = 0 To BUFFER_SIZE \ 2 - 1
        Dim s = Math.Sin(phase)
        Dim v = CShort(s * Short.MaxValue)
        buf(i * 2) = CByte(v And &HFF)
        buf(i * 2 + 1) = CByte((v >> 8) And &HFF)
        phase += step1
        If phase > 2 * Math.PI Then phase -= 2 * Math.PI
      Next

      waveOutWrite(h, hdr, Marshal.SizeOf(hdr))
      While (hdr.dwFlags And WHDR_DONE) = 0
        Thread.Sleep(5)
      End While
    End While

    waveOutReset(h)
    waveOutUnprepareHeader(h, hdr, Marshal.SizeOf(hdr))
    gh.Free()
    waveOutClose(h)
  End Sub
End Module
