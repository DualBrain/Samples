' Inspired by: "Code-It-Yourself! Sound Synthesizer #4 - Waveout API, Sequencing & Ducktales" -- @javidx9
' https://youtu.be/roRH3PdTajs

Option Explicit On
Option Strict On
Option Infer On

Imports System.Runtime.InteropServices
Imports System.Threading

Friend Module Win32

  Friend Const S_OK As Integer = 0
  Friend Const WAVE_FORMAT_PCM As Integer = 1
  Friend Const WHDR_PREPARED As Integer = &H1

  Friend Const WAVE_MAPPER As Integer = -1
  Friend Const CALLBACK_FUNCTION As Integer = &H30000
  Friend Const WOM_DONE As Integer = &H3BD


  <StructLayout(LayoutKind.Sequential)>
  Friend Structure WaveFormat
    Public FormatTag As Short
    Public Channels As Short
    Public SamplesPerSec As Integer
    Public AvgBytesPerSec As Integer
    Public BlockAlign As Short
    Public BitsPerSample As Short
    Public Size As Short
  End Structure

  <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
  Friend Structure WaveOutCaps
    Public Mid As Short
    Public Pid As Short
    Public DriverVersion As Integer
    <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
    Public Name As String
    Public Formats As Integer
    Public Channels As Short
    Public Support As Integer
    Sub Init()
      Name = Space(32)
    End Sub
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Friend Structure WaveHeader
    Public Data As IntPtr
    Public BufferLength As Integer
    Public BytesRecorded As Integer
    Public User As IntPtr
    Public Flags As Integer
    Public Loops As Integer
    Public [Next] As IntPtr
    Public Reserved As IntPtr
  End Structure

  Friend Delegate Sub WaveOutCallback(waveOut As IntPtr, msg As Integer, instance As IntPtr, param1 As IntPtr, param2 As IntPtr)

  'Friend Declare Sub WaveOutProc Lib "winmm.dll" Alias "waveOutProc" (waveOut As IntPtr, uMsg As Integer, dwInstance As Integer, dwParam1 As Integer, dwParam2 As Integer)
  Friend Declare Function WaveOutGetNumDevs Lib "winmm.dll" Alias "waveOutGetNumDevs" () As Integer
  Friend Declare Auto Function WaveOutGetDevCaps Lib "winmm.dll" Alias "waveOutGetDevCaps" (deviceId As Short, ByRef caps As WaveOutCaps, size As Integer) As Integer
  'Friend Declare Function WaveOutOpen Lib "winmm.dll" Alias "waveOutOpen" (ByRef waveOut As IntPtr, deviceId As Integer, format As WaveFormat, callback As WaveOutCallback, instance As IntPtr, flags As Integer) As Integer
  'Friend Declare Function WaveOutPrepareHeader Lib "winmm.dll" Alias "waveOutPrepareHeader" (waveOut As IntPtr, ByRef waveOutHdr As WaveHeader, size As Integer) As Integer
  'Friend Declare Function WaveOutWrite Lib "winmm.dll" Alias "waveOutWrite" (waveOut As IntPtr, ByRef waveOutHdr As WaveHeader, size As Integer) As Integer
  'Friend Declare Function WaveOutUnprepareHeader Lib "winmm.dll" Alias "waveOutUnprepareHeader" (waveOut As IntPtr, ByRef waveOutHdr As WaveHeader, size As Integer) As Integer
  Friend Declare Function WaveOutOpen Lib "winmm.dll" Alias "waveOutOpen" (ByRef waveOut As IntPtr, deviceId As Integer, format As WaveFormat, callback As IntPtr, instance As IntPtr, flags As Integer) As Integer
  Friend Declare Function WaveOutPrepareHeader Lib "winmm.dll" Alias "waveOutPrepareHeader" (waveOut As IntPtr, waveOutHdr As IntPtr, size As Integer) As Integer
  Friend Declare Function WaveOutWrite Lib "winmm.dll" Alias "waveOutWrite" (waveOut As IntPtr, waveOutHdr As IntPtr, size As Integer) As Integer
  Friend Declare Function WaveOutUnprepareHeader Lib "winmm.dll" Alias "waveOutUnprepareHeader" (waveOut As IntPtr, waveOutHdr As IntPtr, size As Integer) As Integer
  Friend Declare Function WaveOutClose Lib "winmm.dll" Alias "waveOutClose" (waveOut As IntPtr) As Integer
  Friend Declare Function WaveOutReset Lib "winmm.dll" Alias "waveOutReset" (waveOut As IntPtr) As Integer

End Module

Public Class olcNoiseMaker(Of T)

  Public ReadOnly BufferLock As New Object()

  Private m_delegate As WaveOutCallback
  Private m_delegateHandle As GCHandle

  Public Sub New(outputDevice As String,
                 Optional sampleRate As Integer = 44100,
                 Optional channels As Integer = 1,
                 Optional blocks As Integer = 8,
                 Optional blockSamples As Integer = 512)
    Create(outputDevice, sampleRate, channels, blocks, blockSamples)
  End Sub

  Public Function Create(outputDevice As String,
                         Optional sampleRate As Integer = 44100,
                         Optional channels As Integer = 1,
                         Optional blocks As Integer = 8,
                         Optional blockSamples As Integer = 512) As Boolean

    m_ready = False
    m_sampleRate = sampleRate
    m_timeStep = 1.0# / m_sampleRate
    m_channels = channels
    m_blockCount = blocks
    m_blockSamples = blockSamples
    m_blockFree = m_blockCount
    m_bufferIndex = 0

    ' Validate device
    Dim devices = Enumerate()
    Dim d = devices.FindIndex(Function(x) x = outputDevice)

    If d <> -1 Then

      ' Device is available

      Dim deviceID = -1 'WAVE_MAPPER 'd

      Dim waveFormat As New WaveFormat With {
        .FormatTag = WAVE_FORMAT_PCM,
        .SamplesPerSec = m_sampleRate,
        .BitsPerSample = CShort(Marshal.SizeOf(GetType(T)) * 8),
        .Channels = CShort(m_channels),
        .BlockAlign = (waveFormat.BitsPerSample \ 8S) * waveFormat.Channels,
        .AvgBytesPerSec = waveFormat.SamplesPerSec * waveFormat.BlockAlign,
        .Size = 0}

      ' Open Device if valid
      m_delegate = New WaveOutCallback(AddressOf WaveOutCallbackFunc)
      m_delegateHandle = GCHandle.Alloc(m_delegate, GCHandleType.Normal)
      Dim ptr = Marshal.GetFunctionPointerForDelegate(m_delegate)
      Dim apiResult = WaveOutOpen(m_waveOut, deviceID, waveFormat, ptr, IntPtr.Zero, CALLBACK_FUNCTION)
      'Dim apiResult = WaveOutOpen(m_waveOut, deviceID, waveFormat, m_delegate, IntPtr.Zero, CALLBACK_FUNCTION)
      If apiResult <> S_OK Then
        Debug.WriteLine($"WaveOutOpen = {apiResult}: {Now}")
        Return Destroy()
      End If

    End If

    ' Allocate Wave|Block Memory and Link headers to block memory
    ReDim m_wavePinnedHeaders(m_blockCount - 1) 'As GCHandle
    m_waveHeaders = New WaveHeader(m_blockCount - 1) {}
    ReDim m_buffers(m_blockCount - 1)
    For i = 0 To m_waveHeaders.Length - 1
      m_waveHeaders(i) = New WaveHeader With {.BufferLength = m_blockSamples * 2, ' number of bytes
                                              .Data = Marshal.AllocHGlobal(m_blockSamples * 2)} ' number of bytes
      m_wavePinnedHeaders(i) = GCHandle.Alloc(m_waveHeaders(i), GCHandleType.Pinned)
      m_buffers(i) = m_waveHeaders(i).Data
      'result = WaveOutPrepareHeader(m_waveOut, m_waveHeaders(i), Marshal.SizeOf(GetType(WaveHeader)))
      Dim apiResult = WaveOutPrepareHeader(m_waveOut, m_wavePinnedHeaders(i).AddrOfPinnedObject, Marshal.SizeOf(GetType(WaveHeader)))
      If apiResult <> 0 Then Debug.WriteLine($"WaveOutPrepareHeader = {apiResult}: {Now}")
    Next

    m_ready = True

    ReDim m_buffer(m_blockSamples - 1)

    Dim thread = New Thread(AddressOf MainThread)
    thread.Start()

    Return True

  End Function

  Public Sub [Stop]()
    m_ready = False
  End Sub

  Public Function GetTime() As Double
    Return m_globalTime
  End Function

  Public Shared Function Enumerate() As List(Of String)
    Dim deviceCount = WaveOutGetNumDevs()
    Dim devices = New List(Of String)()
    Dim caps = New WaveOutCaps : caps.Init()
    For n = 0S To CShort(deviceCount - 1S)
      Dim sz = Marshal.SizeOf(GetType(WaveOutCaps))
      Dim apiResult = WaveOutGetDevCaps(n, caps, sz)
      If apiResult = S_OK Then
        devices.Add(caps.Name)
      Else
        Debug.WriteLine($"WaveOutGetDevCaps = {apiResult}: {Now}")
      End If
    Next
    Return devices
  End Function

  Public Sub SetUserFunction(func As Func(Of Double, Double))
    m_userFunction = func
  End Sub

  Public Function Clip(sample As Double, max As Double) As Double
    Return If(sample >= 0.0, Math.Min(sample, max), Math.Max(sample, -max))
  End Function

  Public Function Destroy() As Boolean
    Return False
  End Function

  Public Sub StopAudio()
    m_ready = False
    'm_playbackThread.Join()
  End Sub

  ' Override to process current sample
  Protected Overridable Function UserProcess(time As Double) As Double
    Return 0.0
  End Function

  Private m_userFunction As Func(Of Double, Double)

  Private m_sampleRate As Integer
  Private m_channels As Integer
  Private m_blockCount As Integer
  Private m_blockSamples As Integer
  Private m_bufferIndex As Integer

  Private m_wavePinnedHeaders() As GCHandle
  Private m_waveHeaders() As WaveHeader
  Private m_buffers() As IntPtr

  Private m_waveOut As IntPtr = IntPtr.Zero

  Private ReadOnly m_playbackThread As New Object()

  Private m_ready As Boolean = False
  Private m_blockFree As Integer

  Private m_globalTime As Double

  ' Handler for soundcard request for more data
  Public Sub WaveOutCallbackFunc(waveOut As IntPtr,
                                  msg As Integer,
                                  instance As IntPtr,
                                  param1 As IntPtr,
                                  param2 As IntPtr)
    If msg <> WOM_DONE Then Return

    SyncLock m_playbackThread
      m_blockFree += 1
      m_bufferIndex = (m_bufferIndex + 1) Mod m_blockCount
      Monitor.PulseAll(m_playbackThread)
    End SyncLock

  End Sub

  ' Main thread. This loop responds to requests from the soundcard to fill 'blocks'
  ' with audio data. If no requests are available it goes dormant until the sound
  ' card is ready for more data. The block is filled by the "user" in some manner
  ' and then issued to the soundcard.
  Private Sub MainThread()

    Dim apiResult As Integer

    m_globalTime = 0.0#

    Dim currentBufferIndex = m_bufferIndex

    Dim sz = Marshal.SizeOf(GetType(WaveHeader))

    While m_ready

      ' Wait for block to become available
      If m_blockFree = 0 Then
        SyncLock m_playbackThread
          While m_bufferIndex = currentBufferIndex
            Monitor.Wait(m_playbackThread)
          End While
        End SyncLock
      End If

      ' Block is here, so use it
      m_blockFree -= 1

      If (m_waveHeaders(currentBufferIndex).Flags And WHDR_PREPARED) <> 0 Then
        apiResult = WaveOutUnprepareHeader(m_waveOut, m_wavePinnedHeaders(currentBufferIndex).AddrOfPinnedObject, sz)
        If apiResult <> 0 Then Debug.WriteLine($"WaveOutUnprepareHeader = {apiResult}: {Now}") : Exit While
      End If

      FillBuffer(currentBufferIndex)

      ' Send block to sound device
      apiResult = WaveOutPrepareHeader(m_waveOut, m_wavePinnedHeaders(currentBufferIndex).AddrOfPinnedObject, sz)
      If apiResult <> 0 Then Debug.WriteLine($"WaveOutPrepareHeader = {apiResult}: {Now}") : Exit While
      'apiResult = WaveOutWrite(m_waveOut, m_waveHeaders(currentBufferIndex), sz)
      apiResult = WaveOutWrite(m_waveOut, m_wavePinnedHeaders(currentBufferIndex).AddrOfPinnedObject, sz)
      If apiResult <> 0 Then Debug.WriteLine($"WaveOutWrite = {apiResult}") : Exit While
      currentBufferIndex = (currentBufferIndex + 1) Mod m_blockCount

    End While

    ' cleanup
    m_delegateHandle.Free()
    apiResult = WaveOutReset(m_waveOut)
    If apiResult <> 0 Then Debug.WriteLine($"WaveOutReset = {apiResult}: {Now}")
    For i = 0 To m_waveHeaders.Length - 1
      'apiResult = WaveOutUnprepareHeader(m_waveOut, m_waveHeaders(i), Marshal.SizeOf(GetType(WaveHeader)))
      apiResult = WaveOutUnprepareHeader(m_waveOut, m_wavePinnedHeaders(i).AddrOfPinnedObject, sz)
      If apiResult <> 0 Then Debug.WriteLine($"WaveOutUnprepareHeader = {apiResult}: {Now}")
      Marshal.FreeHGlobal(m_waveHeaders(i).Data)
      m_wavePinnedHeaders(i).Free()
    Next
    apiResult = WaveOutClose(m_waveOut)
    If apiResult <> 0 Then Debug.WriteLine($"WaveOutClose = {apiResult}")

  End Sub

  Private ReadOnly m_sizeOfT As Integer = Marshal.SizeOf(GetType(T))
  Private ReadOnly m_maxSample As Short = CShort(Math.Pow(2, (m_sizeOfT * 8) - 1) - 1)
  Private m_timeStep As Double = 1.0# / 44100 ' default to 44100 - will reset appropriately in Create.

  Private m_buffer() As Short
  Private m_iBuffer As Integer

  Private Sub FillBuffer(bufferIndex As Integer)

    ' Copy the sine wave to the buffer
    'Dim buffer(m_blockSamples - 1) As Short
    For m_iBuffer = 0 To m_buffer.Length - 1
      If m_userFunction Is Nothing Then
        m_buffer(m_iBuffer) = CShort(Fix(Clip(UserProcess(m_globalTime), 1.0) * m_maxSample))
      Else
        m_buffer(m_iBuffer) = CShort(Fix(Clip(m_userFunction(m_globalTime), 1.0) * m_maxSample))
      End If
      m_globalTime += m_timeStep
    Next
    Marshal.Copy(m_buffer, 0, m_buffers(bufferIndex), m_buffer.Length)

  End Sub

End Class
