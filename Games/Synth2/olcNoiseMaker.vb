' Code-It-Yourself! Sound Synthesizer #1 - Basic Noises
' https://youtu.be/tgamhuQnOkM

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
  Friend Declare Function WaveOutOpen Lib "winmm.dll" Alias "waveOutOpen" (ByRef waveOut As IntPtr, deviceId As Integer, format As WaveFormat, callback As WaveOutCallback, instance As IntPtr, flags As Integer) As Integer
  Friend Declare Function WaveOutPrepareHeader Lib "winmm.dll" Alias "waveOutPrepareHeader" (waveOut As IntPtr, ByRef waveOutHdr As WaveHeader, size As Integer) As Integer
  Friend Declare Function WaveOutWrite Lib "winmm.dll" Alias "waveOutWrite" (waveOut As IntPtr, ByRef waveOutHdr As WaveHeader, size As Integer) As Integer
  Friend Declare Function WaveOutUnprepareHeader Lib "winmm.dll" Alias "waveOutUnprepareHeader" (waveOut As IntPtr, ByRef waveOutHdr As WaveHeader, size As Integer) As Integer
  Friend Declare Function WaveOutClose Lib "winmm.dll" Alias "waveOutClose" (waveOut As IntPtr) As Integer
  Friend Declare Function WaveOutReset Lib "winmm.dll" Alias "waveOutReset" (waveOut As IntPtr) As Integer

End Module

Public Class olcNoiseMaker(Of T)

  Private m_delegate As WaveOutCallback

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

    Dim result As Integer

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

      Dim deviceID = d

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
      If WaveOutOpen(m_waveOut, deviceID, waveFormat, m_delegate, IntPtr.Zero, CALLBACK_FUNCTION) <> S_OK Then
        Return Destroy()
      End If

    End If

    ' Allocate Wave|Block Memory and Link headers to block memory
    m_waveHeaders = New WaveHeader(m_blockCount - 1) {}
    ReDim m_buffers(m_blockCount - 1)
    For i = 0 To m_waveHeaders.Length - 1
      m_waveHeaders(i) = New WaveHeader With {.BufferLength = m_blockSamples * 2, ' number of bytes
                                              .Data = Marshal.AllocHGlobal(m_blockSamples * 2)} ' number of bytes
      m_buffers(i) = m_waveHeaders(i).Data
      result = WaveOutPrepareHeader(m_waveOut, m_waveHeaders(i), Marshal.SizeOf(GetType(WaveHeader)))
    Next

    ReDim m_buffer(m_blockSamples - 1)
    m_ready = True

    Dim thread = New Thread(AddressOf MainThread)
    thread.Start()

    Return True

  End Function

  Public Sub [Stop]()
    m_ready = False
    'm_playbackThread.Join()
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
      Dim result = WaveOutGetDevCaps(n, caps, sz)
      If result = S_OK Then
        devices.Add(caps.Name)
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
  Protected Overridable Function UserProcess(ByVal dTime As Double) As Double
    Return 0.0
  End Function

  Private m_userFunction As Func(Of Double, Double)

  Private m_sampleRate As Integer
  Private m_channels As Integer
  Private m_blockCount As Integer
  Private m_blockSamples As Integer
  Private m_bufferIndex As Integer

  Private m_waveHeaders() As WaveHeader
  Private m_buffers() As IntPtr

  Private m_waveOut As IntPtr = IntPtr.Zero

  Private ReadOnly m_playbackThread As New Object()

  Private m_ready As Boolean = False
  Private m_blockFree As Integer

  Private m_globalTime As Double

  ' Handler for soundcard request for more data
  Private Sub WaveOutCallbackFunc(waveOut As IntPtr,
                                  msg As Integer,
                                  instance As IntPtr,
                                  param1 As IntPtr,
                                  param2 As IntPtr)

    If msg = WOM_DONE Then
      SyncLock m_playbackThread
        m_blockFree += 1
        m_bufferIndex = (m_bufferIndex + 1) Mod m_blockCount
        Monitor.PulseAll(m_playbackThread)
      End SyncLock
    End If

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

      FillBuffer(currentBufferIndex)
      apiResult = WaveOutWrite(m_waveOut, m_waveHeaders(currentBufferIndex), sz)
      currentBufferIndex = (currentBufferIndex + 1) Mod m_blockCount

    End While

    ' cleanup
    apiResult = WaveOutReset(m_waveOut)
    For i = 0 To m_waveHeaders.Length - 1
      apiResult = WaveOutUnprepareHeader(m_waveOut, m_waveHeaders(i), sz)
      Marshal.FreeHGlobal(m_waveHeaders(i).Data)
    Next
    apiResult = WaveOutClose(m_waveOut)

  End Sub

  Private ReadOnly m_sizeOfT As Integer = Marshal.SizeOf(GetType(T))
  Private ReadOnly m_maxSample As Short = CShort(Math.Pow(2, (m_sizeOfT * 8) - 1) - 1)
  Private m_timeStep As Double = 1.0# / 44100 ' default to 44100 - will reset appropriately in Create.

  Private m_buffer() As Short
  Private m_fillBufferI As Integer

  Private Sub FillBuffer(bufferIndex As Integer)

    ' Copy the sine wave to the buffer
    'Dim buffer(m_blockSamples - 1) As Short
    For m_fillBufferI = 0 To m_buffer.Length - 1
      If m_userFunction Is Nothing Then
        m_buffer(m_fillBufferI) = CShort(Clip(UserProcess(m_globalTime), 1.0) * m_maxSample)
      Else
        m_buffer(m_fillBufferI) = CShort(Clip(m_userFunction(m_globalTime), 1.0) * m_maxSample)
      End If
      m_globalTime += m_timeStep
    Next
    Marshal.Copy(m_buffer, 0, m_buffers(bufferIndex), m_buffer.Length)

  End Sub

End Class
