Imports System.Runtime.InteropServices

' Define the WaveFormatEx structure used to describe the format of the wave data
<StructLayout(LayoutKind.Sequential)>
Public Structure WaveFormatEx
  Public wFormatTag As UShort '2
  Public nChannels As UShort '2
  Public nSamplesPerSec As UInteger '4
  Public nAvgBytesPerSec As UInteger '4
  Public nBlockAlign As UShort '2
  Public wBitsPerSample As UShort '2
  Public cbSize As UShort '2
End Structure

Module Module1

  Private Const MM_WOM_OPEN As Integer = &H3BB
  Private Const MM_WOM_CLOSE As Integer = &H3BC
  Private Const MM_WOM_DONE As Integer = &H3BD

  Private Delegate Sub WaveOutProc(ByVal hwo As IntPtr, ByVal uMsg As Integer, ByVal dwInstance As Integer, ByVal dwParam1 As Integer, ByVal dwParam2 As Integer)

  <Flags>
  Private Enum WaveOutOpenFlags As Integer
    CALLBACK_NULL = &H0
    CALLBACK_FUNCTION = &H30000
    CALLBACK_EVENT = &H50000
    CALLBACK_WINDOW = &H10000
    WAVE_FORMAT_QUERY = &H1
    WAVE_ALLOWSYNC = &H20
    WAVE_MAPPED = &H4
    WAVE_FORMAT_DIRECT = &H2
    WAVE_FORMAT_DIRECT_QUERY = &H8
    WAVE_FORMAT_QUERYSET = &H10
  End Enum

  <Flags>
  Private Enum WaveOutWriteFlags As Integer
    WAVEHDR_DONE = &H1
    WAVEHDR_PREPARED = &H2
    WAVEHDR_BEGINLOOP = &H4
    WAVEHDR_ENDLOOP = &H8
    WAVEHDR_INQUEUE = &H10
  End Enum

  <StructLayout(LayoutKind.Sequential)>
  Private Structure WaveHdr
    Public lpData As IntPtr
    Public dwBufferLength As Integer
    Public dwBytesRecorded As Integer
    Public dwUser As Integer
    Public dwFlags As WaveOutWriteFlags
    Public dwLoops As Integer
    Public lpNext As IntPtr
    Public reserved As Integer
  End Structure

  <DllImport("winmm.dll")>
  Private Function waveOutOpen(ByRef phwo As IntPtr, uDeviceID As Integer, lpFormat As WaveFormatEx, dwCallback As WaveOutProc, dwInstance As Integer, dwFlags As WaveOutOpenFlags) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutClose(ByVal hwo As IntPtr) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutPrepareHeader(ByVal hwo As IntPtr, ByRef pwh As WaveHdr, ByVal cbwh As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutUnprepareHeader(ByVal hwo As IntPtr, ByRef pwh As WaveHdr, ByVal cbwh As Integer) As Integer
  End Function

  <DllImport("winmm.dll")>
  Private Function waveOutWrite(ByVal hwo As IntPtr, ByRef pwh As WaveHdr, ByVal cbwh As Integer) As Integer
  End Function

  Private waveHeader As WaveHdr
  Private waveOutProcHandle As GCHandle
  Private waveOutHandle As IntPtr

  Private Const WAVE_MAPPER As Integer = -1

  Sub Main()

    ' Load wave file
    Dim waveData As Byte() = System.IO.File.ReadAllBytes("d:\test.wav")

    ' Initialize the WaveFormatEx structure with the format of the wave file
    Dim waveFormat As New WaveFormatEx
    waveFormat.wFormatTag = &H1
    waveFormat.nChannels = BitConverter.ToUInt16(waveData, &H16)
    waveFormat.nSamplesPerSec = BitConverter.ToUInt32(waveData, &H18)
    waveFormat.nAvgBytesPerSec = BitConverter.ToUInt32(waveData, &H1C)
    waveFormat.nBlockAlign = BitConverter.ToUInt16(waveData, &H20)
    waveFormat.wBitsPerSample = BitConverter.ToUInt16(waveData, &H22)
    waveFormat.cbSize = 0

    ' Open the wave output device
    Dim result As Integer = waveOutOpen(waveOutHandle, WAVE_MAPPER, waveFormat, AddressOf WaveOutCallback, 0, WaveOutOpenFlags.CALLBACK_FUNCTION)
    If result <> 0 Then
      Console.WriteLine("Error opening wave output device.")
      Return
    End If

    ' Prepare wave header
    waveHeader.lpData = Marshal.AllocHGlobal(waveData.Length)
    Marshal.Copy(waveData, 0, waveHeader.lpData, waveData.Length)
    waveHeader.dwBufferLength = waveData.Length
    waveHeader.dwFlags = WaveOutWriteFlags.WAVEHDR_BEGINLOOP Or WaveOutWriteFlags.WAVEHDR_ENDLOOP
    waveHeader.dwLoops = 0

    ' Prepare wave header for playback
    result = waveOutPrepareHeader(waveOutHandle, waveHeader, Marshal.SizeOf(waveHeader))
    If result <> 0 Then
      Console.WriteLine("Error preparing wave header.")
      waveOutClose(waveOutHandle)
      Return
    End If

    ' Write wave header to the output device
    result = waveOutWrite(waveOutHandle, waveHeader, Marshal.SizeOf(waveHeader))
    If result <> 0 Then
      Console.WriteLine("Error writing wave header.")
      waveOutUnprepareHeader(waveOutHandle, waveHeader, Marshal.SizeOf(waveHeader))
      waveOutClose(waveOutHandle)
      Return
    End If

    ' Wait for wave playback to complete
    Do While waveHeader.dwFlags <> WaveOutWriteFlags.WAVEHDR_DONE
      Threading.Thread.Sleep(10)
    Loop

    ' Cleanup
    waveOutUnprepareHeader(waveOutHandle, waveHeader, Marshal.SizeOf(waveHeader))
    waveOutClose(waveOutHandle)
    Marshal.FreeHGlobal(waveHeader.lpData)

  End Sub

  Private Sub WaveOutCallback(ByVal hwo As IntPtr, ByVal uMsg As Integer, ByVal dwInstance As Integer, ByVal dwParam1 As Integer, ByVal dwParam2 As Integer)
    If uMsg = MM_WOM_OPEN Then
      Console.WriteLine("Wave output device opened.")
    ElseIf uMsg = MM_WOM_CLOSE Then
      Console.WriteLine("Wave output device closed.")
    ElseIf uMsg = MM_WOM_DONE Then
      waveHeader.dwFlags = WaveOutWriteFlags.WAVEHDR_DONE
    End If
  End Sub

End Module

'Module Program

'  Private Const WAVE_MAPPER As Integer = -1

'  ' Declare waveOutOpen function from winmm.dll
'  Private Declare Function waveOutOpen Lib "winmm.dll" (ByRef phwo As IntPtr, ByVal uDeviceID As Integer, ByVal lpFormat As WaveFormatEx,
'       dwCallback As WaveOutProc, ByVal dwInstance As IntPtr, ByVal dwFlags As Integer) As Integer

'  ' Declare waveOutClose function from winmm.dll
'  Private Declare Function waveOutClose Lib "winmm.dll" (ByVal hwo As IntPtr) As Integer

'  ' Declare waveOutPrepareHeader function from winmm.dll
'  Private Declare Function waveOutPrepareHeader Lib "winmm.dll" (ByVal hwo As IntPtr, ByRef pwh As WaveHdr, ByVal cbwh As Integer) As Integer

'  ' Declare waveOutUnprepareHeader function from winmm.dll
'  Private Declare Function waveOutUnprepareHeader Lib "winmm.dll" (ByVal hwo As IntPtr, ByRef pwh As WaveHdr, ByVal cbwh As Integer) As Integer

'  ' Declare waveOutWrite function from winmm.dll
'  Private Declare Function waveOutWrite Lib "winmm.dll" (ByVal hwo As IntPtr, ByRef pwh As WaveHdr, ByVal cbwh As Integer) As Integer

'  ' Declare waveOutProc callback function
'  Private Delegate Sub WaveOutProc(ByVal hwo As IntPtr, ByVal uMsg As Integer, ByVal dwInstance As IntPtr,
'                                   ByRef dwParam1 As IntPtr, ByRef dwParam2 As IntPtr)

'  ' Constants for waveOutOpen
'  Private Const CALLBACK_FUNCTION As Integer = &H3

'  ' Constants for MMRESULT
'  Private Const MMSYSERR_NOERROR As Integer = 0

'  ' Constants for dwFlags in WaveHdr
'  Private Const WHDR_DONE As Integer = &H1

'  ' Constants for waveform-audio output messages
'  Private Const MM_WOM_DONE As Integer = &H3BD

'  ' Private variables
'  Private _waveOut As IntPtr
'  Private _waveHdr As WaveHdr
'  Private _callback As WaveOutProc

'  Sub Main()

'    ' Initialize WaveHdr structure
'    _waveHdr = New WaveHdr()
'    _waveHdr.dwFlags = 0
'    _waveHdr.dwBufferLength = 0
'    _waveHdr.dwBytesRecorded = 0
'    _waveHdr.dwUser = IntPtr.Zero
'    _waveHdr.lpData = IntPtr.Zero

'    ' Initialize callback function
'    _callback = AddressOf WaveOutCallback

'    ' Open wave file
'    Dim waveFile As New WaveFile("d:\test.wav")

'    Dim result As Integer

'    ' Open waveform-audio output device
'    result = waveOutOpen(_waveOut, WAVE_MAPPER, waveFile.Format, AddressOf WaveOutCallback, IntPtr.Zero, CALLBACK_FUNCTION)

'    If result <> MMSYSERR_NOERROR Then
'      Throw New Exception("Error opening waveform-audio output device")
'    End If

'    ' Prepare header
'    _waveHdr.lpData = Marshal.AllocHGlobal(waveFile.Data.Length)
'    Marshal.Copy(waveFile.Data, 0, _waveHdr.lpData, waveFile.Data.Length)
'    _waveHdr.dwBufferLength = waveFile.Data.Length

'    result = waveOutPrepareHeader(_waveOut, _waveHdr, Marshal.SizeOf(_waveHdr))

'    If result <> MMSYSERR_NOERROR Then
'      Throw New Exception("Error preparing header")
'    End If

'    ' Write data to output
'    result = waveOutWrite(_waveOut, _waveHdr, Marshal.SizeOf(_waveHdr))

'    If result <> MMSYSERR_NOERROR Then
'      Throw New Exception("Error writing data to output")
'    End If

'    ' Wait for playback to finish
'    Do While (_waveHdr.dwFlags And WHDR_DONE) = 0
'      System.Threading.Thread.Sleep(10)
'    Loop

'    ' Unprepare header
'    result = waveOutUnprepareHeader(_waveOut, _waveHdr, Marshal.SizeOf(_waveHdr))

'    If result <> MMSYSERR_NOERROR Then
'      Throw New Exception("Error unpreparing header")
'    End If

'    ' Close waveform-audio output device
'    result = waveOutClose(_waveOut)

'    If result <> MMSYSERR_NOERROR Then
'      Throw New Exception("Error closing waveform-audio output device")
'    End If

'    ' Cleanup
'    Marshal.FreeHGlobal(_waveHdr.lpData)
'    waveFile.Dispose()

'  End Sub

'  ' Callback function for waveform-audio output messages
'  Private Sub WaveOutCallback(ByVal hwo As IntPtr, ByVal uMsg As Integer, ByVal dwInstance As IntPtr,
'                              ByRef dwParam1 As IntPtr, ByRef dwParam2 As IntPtr)
'    If uMsg = MM_WOM_DONE Then
'      _waveHdr.dwFlags = _waveHdr.dwFlags Or WHDR_DONE
'    End If
'  End Sub

'End Module

'Module Program1

'  ' Define constants used in the code
'  Const MM_WOM_OPEN = &H3BB
'  Const MM_WOM_CLOSE = &H3BC
'  Const MM_WOM_DONE = &H3BD

'  ' Define the WaveOutCaps structure used to retrieve information about the default device
'  <StructLayout(LayoutKind.Sequential)>
'  Public Structure WaveOutCaps
'    Public wMid As UShort
'    Public wPid As UShort
'    Public vDriverVersion As UShort
'    <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
'    Public szPname As String
'    Public dwFormats As UInteger
'    Public wChannels As UShort
'    Public dwSupport As UInteger
'  End Structure

'  ' Declare the winmm.dll functions we will be using
'  Declare Function waveOutOpen Lib "winmm.dll" (ByRef hWaveOut As IntPtr, ByVal uDeviceID As UInteger, ByRef lpFormat As WaveFormatEx, ByVal dwCallback As IntPtr, ByVal dwInstance As IntPtr, ByVal dwFlags As UInteger) As UInteger
'  Declare Function waveOutClose Lib "winmm.dll" (ByVal hWaveOut As IntPtr) As UInteger
'  Declare Function waveOutPrepareHeader Lib "winmm.dll" (ByVal hWaveOut As IntPtr, ByRef lpWaveOutHdr As WaveHdr, ByVal uSize As UInteger) As UInteger
'  Declare Function waveOutWrite Lib "winmm.dll" (ByVal hWaveOut As IntPtr, ByRef lpWaveOutHdr As WaveHdr, ByVal uSize As UInteger) As UInteger
'  Declare Function waveOutGetNumDevs Lib "winmm.dll" () As UInteger
'  Declare Function waveOutGetDevCaps Lib "winmm.dll" Alias "waveOutGetDevCapsA" (ByVal uDeviceID As UInteger, ByRef lpCaps As WaveOutCaps, ByVal uSize As UInteger) As UInteger
'  Declare Function waveOutUnprepareHeader Lib "winmm.dll" (ByVal hwo As IntPtr, ByRef pwh As WaveHdr, ByVal cbwh As Integer) As Integer

'  ' Define the WaveHdr structure used to describe the wave data buffer
'  <StructLayout(LayoutKind.Sequential)>
'  Public Structure WaveHdr
'    Public lpData As IntPtr
'    Public dwBufferLength As UInteger
'    Public dwBytesRecorded As UInteger
'    Public dwUser As IntPtr
'    Public dwFlags As UInteger
'    Public dwLoops As UInteger
'    Public lpNext As IntPtr
'    Public Reserved As IntPtr
'  End Structure

'  Sub Main1() 'args As String())

'    ' Load the wave file into memory as a byte array
'    Dim waveData() As Byte = IO.File.ReadAllBytes("D:\test.wav")

'    ' Initialize the WaveFormatEx structure with the format of the wave file
'    Dim waveFormat As New WaveFormatEx
'    waveFormat.wFormatTag = &H1
'    waveFormat.nChannels = BitConverter.ToUInt16(waveData, &H16)
'    waveFormat.nSamplesPerSec = BitConverter.ToUInt32(waveData, &H18)
'    waveFormat.nAvgBytesPerSec = BitConverter.ToUInt32(waveData, &H1C)
'    waveFormat.nBlockAlign = BitConverter.ToUInt16(waveData, &H20)
'    waveFormat.wBitsPerSample = BitConverter.ToUInt16(waveData, &H22)
'    'waveFormat.cbSize = 0

'    ' Retrieve information about the default output device
'    Dim deviceCaps As New WaveOutCaps
'    waveOutGetDevCaps(0, deviceCaps, Marshal.SizeOf(deviceCaps))

'    ' Open the default output device for playback
'    Dim hWaveOut As IntPtr = IntPtr.Zero
'    waveOutOpen(hWaveOut, 0, waveFormat, IntPtr.Zero, IntPtr.Zero, 0)

'    ' Initialize the WaveHdr structure with the wave data buffer
'    Dim waveHeader As New WaveHdr
'    waveHeader.lpData = Marshal.AllocHGlobal(waveData.Length)
'    Marshal.Copy(waveData, 0, waveHeader.lpData, waveData.Length)
'    waveHeader.dwBufferLength = CUInt(waveData.Length)

'    ' Prepare the wave data buffer for playback
'    waveOutPrepareHeader(hWaveOut, waveHeader, Marshal.SizeOf(waveHeader))

'    ' Write the wave data buffer to the output device
'    waveOutWrite(hWaveOut, waveHeader, Marshal.SizeOf(waveHeader))

'    ' Wait for the playback to finish before cleaning up
'    Do While waveHeader.dwFlags <> MM_WOM_DONE
'      Threading.Thread.Sleep(100)
'    Loop

'    ' Clean up the resources used by the playback
'    waveOutUnprepareHeader(hWaveOut, waveHeader, Marshal.SizeOf(waveHeader))
'    Marshal.FreeHGlobal(waveHeader.lpData)
'    waveOutClose(hWaveOut)

'    'Console.WriteLine("Hello World!")
'  End Sub

'End Module

'Public Class WaveOutExample

'  Private Const WAVE_MAPPER As Integer = -1
'  Private Const MMSYSERR_NOERROR As Integer = 0
'  Private Const CALLBACK_FUNCTION As Integer = &H30000
'  Private Const WAVE_FORMAT_PCM As Integer = 1
'  'Public Const CALLBACK_FUNCTION As Integer = &H3
'  Public Const S_OK As Integer = 0

'  <StructLayout(LayoutKind.Sequential)>
'  Public Structure WAVEFORMATEX
'    Public wFormatTag As Short
'    Public nChannels As Short
'    Public nSamplesPerSec As Integer
'    Public nAvgBytesPerSec As Integer
'    Public nBlockAlign As Short
'    Public wBitsPerSample As Short
'    Public cbSize As Short
'  End Structure

'  Public Enum WaveFormats As Short
'    WAVE_FORMAT_PCM = 1
'  End Enum

'  <StructLayout(LayoutKind.Sequential)>
'  Public Structure WAVEHDR
'    Public lpData As IntPtr
'    Public dwBufferLength As Integer
'    Public dwBytesRecorded As Integer
'    Public dwUser As Integer
'    Public dwFlags As Integer
'    Public dwLoops As Integer
'    Public lpNext As IntPtr
'    Public reserved As Integer
'  End Structure

'  Private waveOut As IntPtr = IntPtr.Zero
'  Private waveHeader As WAVEHDR
'  Private waveFormat As WAVEFORMATEX

'  Private Delegate Sub WaveOutProc(ByVal hdrvr As IntPtr, ByVal uMsg As Integer, ByVal dwUser As Integer, ByVal wavhdr As IntPtr, ByVal dwParam2 As Integer)

'  Public Enum MMRESULT As UInteger
'    MMSYSERR_BASE = 0
'    MMSYSERR_NOERROR = 0
'    MMSYSERR_ERROR = MMSYSERR_BASE + 1
'    MMSYSERR_BADDEVICEID = MMSYSERR_BASE + 2
'    MMSYSERR_NOTENABLED = MMSYSERR_BASE + 3
'    MMSYSERR_ALLOCATED = MMSYSERR_BASE + 4
'    MMSYSERR_INVALHANDLE = MMSYSERR_BASE + 5
'    MMSYSERR_NODRIVER = MMSYSERR_BASE + 6
'    MMSYSERR_NOMEM = MMSYSERR_BASE + 7
'    MMSYSERR_NOTSUPPORTED = MMSYSERR_BASE + 8
'    MMSYSERR_BADERRNUM = MMSYSERR_BASE + 9
'    MMSYSERR_INVALFLAG = MMSYSERR_BASE + 10
'    MMSYSERR_INVALPARAM = MMSYSERR_BASE + 11
'    MMSYSERR_HANDLEBUSY = MMSYSERR_BASE + 12
'    MMSYSERR_INVALIDALIAS = MMSYSERR_BASE + 13
'    MMSYSERR_BADDB = MMSYSERR_BASE + 14
'    MMSYSERR_KEYNOTFOUND = MMSYSERR_BASE + 15
'    MMSYSERR_READERROR = MMSYSERR_BASE + 16
'    MMSYSERR_WRITEERROR = MMSYSERR_BASE + 17
'    MMSYSERR_DELETEERROR = MMSYSERR_BASE + 18
'    MMSYSERR_VALNOTFOUND = MMSYSERR_BASE + 19
'    MMSYSERR_NODRIVERCB = MMSYSERR_BASE + 20
'    MMSYSERR_MOREDATA = MMSYSERR_BASE + 21
'  End Enum

'  Private Declare Function WaveOutOpen Lib "winmm.dll" (ByRef hwo As IntPtr, ByVal uDeviceID As Integer, ByRef pwfx As WAVEFORMATEX, ByVal dwCallback As WaveOutProc, ByVal dwInstance As Integer, ByVal dwFlags As Integer) As MMRESULT
'  Private Declare Function WaveOutClose Lib "winmm.dll" (ByVal hwo As IntPtr) As MMRESULT
'  Private Declare Function WaveOutReset Lib "winmm.dll" (ByVal hwo As IntPtr) As MMRESULT
'  Private Declare Function WaveOutPrepareHeader Lib "winmm.dll" (ByVal hwo As IntPtr, ByRef pwh As WAVEHDR, ByVal cbwh As Integer) As MMRESULT
'  Private Declare Function WaveOutWrite Lib "winmm.dll" (ByVal hwo As IntPtr, ByRef pwh As WAVEHDR, ByVal cbwh As Integer) As MMRESULT

'  Private Sub WaveOutCallback(ByVal hdrvr As IntPtr, ByVal uMsg As Integer, ByVal dwUser As Integer, ByVal wavhdr As IntPtr, ByVal dwParam2 As Integer)
'    ' Handle callback message here
'  End Sub

'  Public Function OpenWaveOut() As Boolean
'    waveFormat = New WAVEFORMATEX()
'    waveFormat.wFormatTag = WAVE_FORMAT_PCM
'    waveFormat.nChannels = 2
'    waveFormat.nSamplesPerSec = 44100
'    waveFormat.nAvgBytesPerSec = 88200
'    waveFormat.nBlockAlign = 4
'    waveFormat.wBitsPerSample = 16
'    waveFormat.cbSize = 0

'    Dim callbackProc As WaveOutProc = AddressOf WaveOutCallback
'    Dim result As Integer = WaveOutOpen(waveOut, WAVE_MAPPER, waveFormat, callbackProc, 0, CALLBACK_FUNCTION)

'    If result = MMSYSERR_NOERROR Then
'      Return True
'    Else
'      Return False
'    End If
'  End Function

'  Public Sub CloseWaveOut()
'    WaveOutReset(waveOut)
'    WaveOutClose(waveOut)
'  End Sub

'  Public Sub PlayWaveOut()
'    WaveOutPrepareHeader(waveOut, waveHeader, Marshal.SizeOf(waveHeader))
'    WaveOutWrite(waveOut, waveHeader, Marshal.SizeOf(waveHeader))
'  End Sub

'  Public Sub StopWaveOut()
'    WaveOutReset(waveOut)
'  End Sub

'End Class

'Public Class WaveFile
'  Implements IDisposable

'  Private _format As WaveFormatEx
'  Private _data As Byte()

'  Public Sub New(ByVal filename As String)
'    Using stream As New FileStream(filename, FileMode.Open, FileAccess.Read)
'      Using reader As New BinaryReader(stream)
'        ' Read RIFF header
'        If reader.ReadChars(4) <> "RIFF" Then
'          Throw New InvalidDataException("Not a valid RIFF file")
'        End If

'        Dim fileSize As Integer = reader.ReadInt32() - 8

'        If reader.ReadChars(4) <> "WAVE" Then
'          Throw New InvalidDataException("Not a valid WAVE file")
'        End If

'        ' Read format chunk
'        If reader.ReadChars(4) <> "fmt " Then
'          Throw New InvalidDataException("Missing format chunk")
'        End If

'        Dim formatChunkSize As Integer = reader.ReadInt32()

'        If formatChunkSize <> Marshal.SizeOf(_format) Then
'          Throw New InvalidDataException("Invalid format chunk size")
'        End If

'        _format = New WaveFormatEx()
'        _format.wFormatTag = reader.ReadInt16()
'        _format.nChannels = reader.ReadInt16()
'        _format.nSamplesPerSec = reader.ReadInt32()
'        _format.nAvgBytesPerSec = reader.ReadInt32()
'        _format.nBlockAlign = reader.ReadInt16()
'        _format.wBitsPerSample = reader.ReadInt16()

'        ' Read data chunk
'        If reader.ReadChars(4) <> "data" Then
'          Throw New InvalidDataException("Missing data chunk")
'        End If

'        Dim dataSize As Integer = reader.ReadInt32()

'        If dataSize > fileSize Then
'          Throw New InvalidDataException("Invalid data chunk size")
'        End If

'        _data = reader.ReadBytes(dataSize)
'      End Using
'    End Using
'  End Sub

'  Public ReadOnly Property Format As WaveFormatEx
'    Get
'      Return _format
'    End Get
'  End Property

'  Public ReadOnly Property Data As Byte()
'    Get
'      Return _data
'    End Get
'  End Property

'  Public Sub Dispose() Implements IDisposable.Dispose
'    _format = Nothing
'    _data = Nothing
'  End Sub
'End Class

'' Define the WaveFormatEx structure used to describe the format of the wave data
'<StructLayout(LayoutKind.Sequential)>
'Public Structure WaveFormatEx
'  Public wFormatTag As UShort '2
'  Public nChannels As UShort '2
'  Public nSamplesPerSec As UInteger '4
'  Public nAvgBytesPerSec As UInteger '4
'  Public nBlockAlign As UShort '2
'  Public wBitsPerSample As UShort '2
'  'Public cbSize As UShort '2
'End Structure