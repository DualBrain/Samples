Option Explicit On
Option Strict On
Option Infer On

Imports System.Runtime.InteropServices
Imports System.Threading

Module Program

#Region "Win32 - Audio"

  Private Const WAVE_MAPPER As Integer = -1
  Private Const CALLBACK_FUNCTION As Integer = &H30000
  Private Const WOM_DONE As Integer = &H3BD

  <StructLayout(LayoutKind.Sequential)>
  Public Structure WaveFormat
    Public FormatTag As Short
    Public Channels As Short
    Public SamplesPerSec As Integer
    Public AvgBytesPerSec As Integer
    Public BlockAlign As Short
    Public BitsPerSample As Short
    Public Size As Short
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Public Structure WaveHeader
    Public Data As IntPtr
    Public BufferLength As Integer
    Public BytesRecorded As Integer
    Public User As IntPtr
    Public Flags As Integer
    Public Loops As Integer
    Public [Next] As IntPtr
    Public Reserved As IntPtr
  End Structure

  Private Declare Function WaveOutOpen Lib "winmm.dll" Alias "waveOutOpen" (ByRef waveOut As IntPtr, deviceId As Integer, format As WaveFormat, callback As WaveOutCallback, instance As IntPtr, flags As Integer) As Integer
  Private Declare Function WaveOutPrepareHeader Lib "winmm.dll" Alias "waveOutPrepareHeader" (waveOut As IntPtr, ByRef waveOutHdr As WaveHeader, size As Integer) As Integer
  Private Declare Function WaveOutWrite Lib "winmm.dll" Alias "waveOutWrite" (waveOut As IntPtr, ByRef waveOutHdr As WaveHeader, size As Integer) As Integer
  Private Declare Function WaveOutUnprepareHeader Lib "winmm.dll" Alias "waveOutUnprepareHeader" (waveOut As IntPtr, ByRef waveOutHdr As WaveHeader, size As Integer) As Integer
  Private Declare Function WaveOutClose Lib "winmm.dll" Alias "waveOutClose" (waveOut As IntPtr) As Integer
  Private Declare Function WaveOutReset Lib "winmm.dll" Alias "waveOutReset" (waveOut As IntPtr) As Integer

  Private Delegate Sub WaveOutCallback(waveOut As IntPtr, msg As Integer, instance As IntPtr, param1 As IntPtr, param2 As IntPtr)

#End Region

  Private Const BUFFER_COUNT As Integer = 8
  Private Const BUFFER_SIZE As Integer = 256

  Private m_waveOut As IntPtr
  Private ReadOnly m_waveHeaders(BUFFER_COUNT - 1) As WaveHeader
  Private ReadOnly m_buffers(BUFFER_COUNT - 1) As IntPtr
  Private ReadOnly m_sineWave(BUFFER_SIZE - 1) As Short
  Private ReadOnly m_playbackThread As New Object()
  Private m_bufferIndex As Integer
  Private m_stopPlayback As Boolean = False

  Sub Main()

    ' Initialize the sine wave buffer
    For i = 0 To m_sineWave.Length - 1
      m_sineWave(i) = CShort(Short.MaxValue * Math.Sin(2 * Math.PI * i / m_sineWave.Length))
    Next

    ' Open the wave output device
    Dim format = New WaveFormat With {.FormatTag = &H1,
                                      .Channels = 1,
                                      .SamplesPerSec = 44100,
                                      .BitsPerSample = 16,
                                      .BlockAlign = 2,
                                      .AvgBytesPerSec = 88200,
                                      .Size = 0}
    Dim result = WaveOutOpen(m_waveOut, WAVE_MAPPER, format, New WaveOutCallback(AddressOf WaveOutCallbackFunc), IntPtr.Zero, CALLBACK_FUNCTION)
    If result <> 0 Then
      Console.WriteLine($"Failed to open wave output device. result={result}")
      Return
    End If

    ' Allocate and prepare the wave headers and buffers
    For i = 0 To m_waveHeaders.Length - 1
      m_waveHeaders(i) = New WaveHeader With {.BufferLength = BUFFER_SIZE * 2,
                                              .Data = Marshal.AllocHGlobal(BUFFER_SIZE * 2)}
      m_buffers(i) = m_waveHeaders(i).Data
      result = WaveOutPrepareHeader(m_waveOut, m_waveHeaders(i), Marshal.SizeOf(GetType(WaveHeader)))
    Next

    ' Start the playback thread
    Dim thread = New Thread(AddressOf PlaybackThreadProc)
    thread.Start()

    ' Wait for a key press to stop playback
    Console.WriteLine("Press any key to stop playback.")
    Console.ReadKey(True)

    Console.Write("Stopping playback...")
    m_stopPlayback = True
    While m_stopPlayback : End While : thread = Nothing
    Console.WriteLine("done.")

    ' The following "cleanup" has been moved to the playback thread and completed in the event of a m_stopPlayback = True.
    'Console.Write("WaveOutReset...")
    'result = WaveOutReset(m_waveOut) : Console.WriteLine("done.")
    'Console.Write("WaveOutUnprepareHeaders / FreeHGlobal...")
    'For i = 0 To m_waveHeaders.Length - 1
    '  result = WaveOutUnprepareHeader(m_waveOut, m_waveHeaders(i), Marshal.SizeOf(GetType(WaveHeader)))
    '  Marshal.FreeHGlobal(m_waveHeaders(i).Data)
    'Next
    'Console.WriteLine("done.")
    'Console.Write("WaveOutClose...")
    'result = WaveOutClose(m_waveOut) : Console.WriteLine("done.")

    Console.WriteLine("Press any key to exit.")
    Console.ReadKey(True)

  End Sub

  Private Sub PlaybackThreadProc()

    Dim result As Integer ' Used by the Win32 API calls.

    Dim currentBufferIndex = m_bufferIndex

    ' Fill the initial buffers with sine waves
    For i = 0 To BUFFER_COUNT - 1
      FillBuffer(currentBufferIndex)
      result = WaveOutWrite(m_waveOut, m_waveHeaders(currentBufferIndex), Marshal.SizeOf(GetType(WaveHeader)))
      currentBufferIndex = (currentBufferIndex + 1) Mod BUFFER_COUNT
    Next

    m_stopPlayback = False ' Not absolutely necessary to set to False here; but doing so for completeness.

    ' Play the sine wave continuously
    While Not m_stopPlayback

      SyncLock m_playbackThread
        While m_bufferIndex = currentBufferIndex
          Monitor.Wait(m_playbackThread)
        End While
      End SyncLock

      If Not m_stopPlayback Then
        ' Refill the used buffer and play it
        Dim bufferToRefill = m_waveHeaders(currentBufferIndex)
        FillBuffer(currentBufferIndex)
        Marshal.Copy(m_sineWave, 0, bufferToRefill.Data, BUFFER_SIZE)
        result = WaveOutWrite(m_waveOut, bufferToRefill, Marshal.SizeOf(GetType(WaveHeader)))
        currentBufferIndex = (currentBufferIndex + 1) Mod BUFFER_COUNT
      End If

    End While

    ' Moved this from Main to here as it seems to work better.
    ' Where it was, if I try to check for completeness of this
    ' running thread... the application appears to exit prematurely
    ' without any sort of error(s). By placing it here, can now
    ' check that the m_stopPlayback is False which clearly indicates
    ' that the process has completed from True (to exit) to False (below).
    result = WaveOutReset(m_waveOut)
    For i = 0 To m_waveHeaders.Length - 1
      result = WaveOutUnprepareHeader(m_waveOut, m_waveHeaders(i), Marshal.SizeOf(GetType(WaveHeader)))
      Marshal.FreeHGlobal(m_waveHeaders(i).Data)
    Next
    result = WaveOutClose(m_waveOut)

    m_stopPlayback = False

  End Sub

  Private Sub FillBuffer(bufferIndex As Integer)
    Dim bufferPtr = m_buffers(bufferIndex)
    ' Copy the sine wave to the buffer
    Dim buffer(BUFFER_SIZE - 1) As Short
    For i = 0 To buffer.Length - 1
      buffer(i) = m_sineWave(i)
    Next
    Marshal.Copy(buffer, 0, bufferPtr, buffer.Length)
  End Sub

  Private Sub WaveOutCallbackFunc(handle As IntPtr, msg As Integer, instance As IntPtr, param1 As IntPtr, param2 As IntPtr)
    If msg = WOM_DONE Then
      SyncLock m_playbackThread
        m_bufferIndex = (m_bufferIndex + 1) Mod BUFFER_COUNT
        Monitor.PulseAll(m_playbackThread)
      End SyncLock
    End If
  End Sub

End Module