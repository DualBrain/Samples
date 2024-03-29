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

  Private Declare Function GetAsyncKeyState Lib "user32.dll" (virtualKeyCode As Integer) As Short

  Private Const BUFFER_COUNT As Integer = 8
  Private Const BUFFER_SIZE As Integer = 2048 '256

  Private m_waveOut As IntPtr
  Private ReadOnly m_waveHeaders(BUFFER_COUNT - 1) As WaveHeader
  Private ReadOnly m_buffers(BUFFER_COUNT - 1) As IntPtr
  'Private ReadOnly m_sineWave(BUFFER_SIZE - 1) As Short
  Private ReadOnly m_playbackThread As New Object()
  Private m_bufferIndex As Integer
  Private m_stopPlayback As Boolean = False

  Public Function W(hertz As Double) As Double
    Return hertz * 2.0 * Math.PI
  End Function

  Private m_frequencyOutput As Double = 0.0                                   ' dominant output frequency of instrument, i.e. the note
  Private ReadOnly m_octaveBaseFrequency As Double = 110.0                    ' frequency Of octave represented by keyboard
  Private ReadOnly m_12thRootOf2 As Double = Math.Pow(2.0, 1.0 / 12.0)   ' assuming western 12 notes per ocatve

  Sub Main()

    ' Initialize the sine wave buffer
    'For i = 0 To m_sineWave.Length - 1
    '  m_sineWave(i) = CShort(Short.MaxValue * Math.Sin(8 * Math.PI * i / m_sineWave.Length))
    'Next

    ' Open the wave output device
    Dim format = New WaveFormat With {.FormatTag = &H1,
                                      .Channels = 1,
                                      .SamplesPerSec = 44100,
                                      .BitsPerSample = 16,
                                      .BlockAlign = 2,
                                      .AvgBytesPerSec = 88200,
                                      .Size = 0}
    'Dim result = WaveOutOpen(m_waveOut, WAVE_MAPPER, format, New WaveOutCallback(AddressOf WaveOutCallbackFunc), IntPtr.Zero, CALLBACK_FUNCTION)
    Dim result = WaveOutOpen(m_waveOut, 0, format, New WaveOutCallback(AddressOf WaveOutCallbackFunc), IntPtr.Zero, CALLBACK_FUNCTION)
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

    Dim currentKey = -1
    Dim keyPressed = False

    Dim keys = "ZSXCFVGBNJMK" & ChrW(&HBC) & ChrW(&HBE) & ChrW(&HBF)
    Dim abort = False
    Dim row = 0
    Dim k = 0

    Console.WriteLine("Press ESC to stop playback.")
    Do

      keyPressed = False

      abort = (GetAsyncKeyState(27) And &H8000) <> 0
      If abort Then Exit Do

      For k = 0 To 14
        If (GetAsyncKeyState(AscW(keys(k))) And &H8000) <> 0 Then
          If currentKey <> k Then
            m_frequencyOutput = m_octaveBaseFrequency * Math.Pow(m_12thRootOf2, k)
            'row = Console.CursorTop : Console.WriteLine($"Note On : {m_globalTime}s {m_frequencyOutput}Hz") : Console.CursorTop = row
            currentKey = k
          End If
          keyPressed = True
        End If
      Next

      If Not keyPressed Then

        If currentKey <> -1 Then
          'row = Console.CursorTop : Console.WriteLine($"Note Off: {m_globalTime}s                        ") : Console.CursorTop = row
          currentKey = -1
        End If
        m_frequencyOutput = 0.0

        ' Flush the keyboard buffer.
        While Console.KeyAvailable
          Console.ReadKey(True)
        End While

      End If

    Loop

    ' Wait for a key press to stop playback
    'Console.WriteLine("Press any key to stop playback.")
    'Console.ReadKey(True)

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

    Dim sz = Marshal.SizeOf(GetType(WaveHeader))

    ' Play the sine wave continuously
    While Not m_stopPlayback

      SyncLock m_playbackThread
        While m_bufferIndex = currentBufferIndex
          Monitor.Wait(m_playbackThread)
        End While
      End SyncLock

      If Not m_stopPlayback Then
        ' Refill the used buffer and play it
        FillBuffer(currentBufferIndex)
        'Marshal.Copy(m_sineWave, 0, bufferToRefill.Data, BUFFER_SIZE) ' handled in FillBuffer.
        result = WaveOutWrite(m_waveOut, m_waveHeaders(currentBufferIndex), sz)
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

  'Private Sub FillBuffer(bufferIndex As Integer)
  '  Dim bufferPtr = m_buffers(bufferIndex)
  '  ' Copy the sine wave to the buffer
  '  Dim buffer(BUFFER_SIZE - 1) As Short
  '  For i = 0 To buffer.Length - 1
  '    buffer(i) = m_sineWave(i)
  '  Next
  '  Marshal.Copy(buffer, 0, bufferPtr, buffer.Length)
  'End Sub

  Private ReadOnly m_timeStep As Double = 1.0# / 44100 ' default to 44100 - will reset appropriately in Create.
  Private m_globalTime As Double = 0.0#
  Private ReadOnly m_buffer(BUFFER_SIZE - 1) As Short

  Private Sub FillBuffer(bufferIndex As Integer)

    ' Copy the sine wave to the buffer
    'Dim buffer(BUFFER_SIZE - 1) As Short
    For i = 0 To m_buffer.Length - 1
      m_buffer(i) = CShort(Clip(UserProcess(m_globalTime), 1.0) * Short.MaxValue)
      m_globalTime += m_timeStep
    Next
    Marshal.Copy(m_buffer, 0, m_buffers(bufferIndex), m_buffer.Length)

  End Sub

  Private Function Clip(sample As Double, max As Double) As Double
    Return If(sample >= 0.0, Math.Min(sample, max), Math.Max(sample, -max))
  End Function

  Private Function UserProcess(time As Double) As Double
    'Return 0.0
    'Return 0.5 + Math.Sin(W(440.0) * time + 0.05 * 440.0 * Math.Sin(W(1.0) * time))
    'Return 0.5 + Math.Sin(W(m_frequencyOutput) * time + 0.05 * m_frequencyOutput * Math.Sin(W(1.0) * time))
    Dim output = 0.1 * Math.Sin(m_frequencyOutput * 2 * 3.14159 * time)
    Return If(output > 0, 0.2, -0.2)
  End Function

  Private Sub WaveOutCallbackFunc(handle As IntPtr, msg As Integer, instance As IntPtr, param1 As IntPtr, param2 As IntPtr)
    If msg = WOM_DONE Then
      SyncLock m_playbackThread
        m_bufferIndex = (m_bufferIndex + 1) Mod BUFFER_COUNT
        Monitor.PulseAll(m_playbackThread)
      End SyncLock
    End If
  End Sub

End Module