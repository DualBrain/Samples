Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading

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
  Public Reserved As IntPtr
End Structure

Public Enum Colour As Short
  FG_BLACK = &H0
  FG_DARK_BLUE = &H1
  FG_DARK_GREEN = &H2
  FG_DARK_CYAN = &H3
  FG_DARK_RED = &H4
  FG_DARK_MAGENTA = &H5
  FG_DARK_YELLOW = &H6
  FG_GREY = &H7 ' Thanks MS :-/
  FG_DARK_GREY = &H8
  FG_BLUE = &H9
  FG_GREEN = &HA
  FG_CYAN = &HB
  FG_RED = &HC
  FG_MAGENTA = &HD
  FG_YELLOW = &HE
  FG_WHITE = &HF
  BG_BLACK = &H0
  BG_DARK_BLUE = &H10
  BG_DARK_GREEN = &H20
  BG_DARK_CYAN = &H30
  BG_DARK_RED = &H40
  BG_DARK_MAGENTA = &H50
  BG_DARK_YELLOW = &H60
  BG_GREY = &H70
  BG_DARK_GREY = &H80
  BG_BLUE = &H90
  BG_GREEN = &HA0
  BG_CYAN = &HB0
  BG_RED = &HC0
  BG_MAGENTA = &HD0
  BG_YELLOW = &HE0
  BG_WHITE = &HF0
End Enum

Public Enum PIXEL_TYPE As Short
  PIXEL_SOLID = &H2588
  PIXEL_THREEQUARTERS = &H2593
  PIXEL_HALF = &H2592
  PIXEL_QUARTER = &H2591
End Enum

Public Class Sprite

  Public Sub New()
  End Sub

  Public Sub New(w As Integer, h As Integer)
    Create(w, h)
  End Sub

  Public Sub New(file As String)
    If Not Load(file) Then Create(8, 8)
  End Sub

  Public Property Width As Integer
  Public Property Height As Integer

  Private m_glyphs() As Char
  Private m_colours() As Colour

  Private Sub Create(w As Integer, h As Integer)
    Width = w
    Height = h
    ReDim m_glyphs(w * h)
    ReDim m_colours(w * h)
    For i = 0 To (w * h) - 1
      m_glyphs(i) = " "c
      m_colours(i) = Colour.BG_BLACK
    Next
  End Sub

  Public Sub SetGlyph(x As Integer, y As Integer, c As Char)
    If x < 0 OrElse x >= Width OrElse y < 0 OrElse y >= Height Then
      Return
    Else
      m_glyphs(y * Width + x) = c
    End If
  End Sub

  Public Sub SetColour(x As Integer, y As Integer, c As Colour)
    If x < 0 OrElse x >= Width OrElse y < 0 OrElse y >= Height Then
      Return
    Else
      m_colours(y * Width + x) = c
    End If
  End Sub

  Public Function GetGlyph(x As Integer, y As Integer) As Char
    If x < 0 OrElse x >= Width OrElse y < 0 OrElse y >= Height Then
      Return " "c
    Else
      Return m_glyphs(y * Width + x)
    End If
  End Function

  Public Function GetColour(x As Integer, y As Integer) As Colour
    If x < 0 OrElse x >= Width OrElse y < 0 OrElse y >= Height Then
      Return Colour.BG_BLACK
    Else
      Return m_colours(y * Width + x)
    End If
  End Function

  Public Function SampleColour(x As Single, y As Single) As Colour
    Dim sx = CInt(x * Width)
    Dim sy = CInt(y * Height - 1.0)
    If sx < 0 OrElse sx >= Width OrElse sy < 0 OrElse sy >= Height Then
      Return Colour.BG_BLACK
    Else
      Return m_colours(sy * Width + sx)
    End If
  End Function

  Public Function Save(file As String) As Boolean

    Using f = IO.File.Create(file)

      If f Is Nothing Then Return False

      Using bw As New System.IO.BinaryWriter(f)
        bw.Write(Width)
        bw.Write(Height)
        For i As Integer = 0 To (Width * Height) - 1
          bw.Write(m_colours(i))
        Next
        For i As Integer = 0 To (Width * Height) - 1
          bw.Write(m_glyphs(i))
        Next
      End Using

      f.Close()

    End Using

    Return True

  End Function

  Public Function Load(file As String) As Boolean

    ReDim m_glyphs(0)
    ReDim m_colours(0)
    Width = 0
    Height = 0

    Using f = IO.File.OpenRead(file)

      If f Is Nothing Then Return False

      Using br As New IO.BinaryReader(f)

        Width = br.ReadInt32()
        Height = br.ReadInt32()

        Create(Width, Height)

        For i = 0 To (Width * Height) - 1
          m_colours(i) = br.ReadInt16()
        Next
        For i As Integer = 0 To (Width * Height) - 1
          m_glyphs(i) = br.ReadChar ' br.ReadInt16()
        Next

      End Using

      f.Close()

    End Using

    Return True

  End Function

End Class

Public MustInherit Class ConsoleGameEngine

#Region "Win32"

  ' Structure for a KEY_EVENT_RECORD
  <StructLayout(LayoutKind.Sequential)>
  Private Structure KEY_EVENT_RECORD
    Public bKeyDown As Short
    Public wRepeatCount As Short
    Public wVirtualKeyCode As Short
    Public wVirtualScanCode As Short
    Public uChar As Char
    Public dwControlKeyState As Integer
  End Structure

  ' Structure for a MOUSE_EVENT_RECORD
  <StructLayout(LayoutKind.Sequential)>
  Private Structure MOUSE_EVENT_RECORD
    Public dwMousePosition As COORD
    Public dwButtonState As Integer
    Public dwControlKeyState As Integer
    Public dwEventFlags As Integer
  End Structure

  ' Structure for a WINDOW_BUFFER_SIZE_RECORD
  <StructLayout(LayoutKind.Sequential)>
  Private Structure WINDOW_BUFFER_SIZE_RECORD
    Public dwSize As COORD
  End Structure

  ' Structure for a MENU_EVENT_RECORD
  <StructLayout(LayoutKind.Sequential)>
  Private Structure MENU_EVENT_RECORD
    Public dwCommandId As Integer
  End Structure

  ' Structure for a FOCUS_EVENT_RECORD
  <StructLayout(LayoutKind.Sequential)>
  Private Structure FOCUS_EVENT_RECORD
    Public bSetFocus As Short
  End Structure

  <StructLayout(LayoutKind.Explicit)>
  Private Structure INPUT_RECORD
    <FieldOffset(0)> Public EventType As UShort
    <FieldOffset(4)> Public KeyEvent As KEY_EVENT_RECORD
    <FieldOffset(4)> Public MouseEvent As MOUSE_EVENT_RECORD
    <FieldOffset(4)> Public WindowBufferSizeEvent As WINDOW_BUFFER_SIZE_RECORD
    <FieldOffset(4)> Public MenuEvent As MENU_EVENT_RECORD
    <FieldOffset(4)> Public FocusEvent As FOCUS_EVENT_RECORD
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Private Structure CONSOLE_SCREEN_BUFFER_INFO
    Public dwSize As COORD
    Public dwCursorPosition As COORD
    Public wAttributes As Short
    Public srWindow As SMALL_RECT
    Public dwMaximumWindowSize As COORD
  End Structure

  <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
  Private Structure CONSOLE_FONT_INFOEX
    Public cbSize As Integer
    Public nFont As Integer
    Public dwFontSize As COORD
    Public FontFamily As Integer
    Public FontWeight As Integer
    <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=LF_FACESIZE)>
    Public FaceName As String
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Private Structure SMALL_RECT
    Public Left As Short
    Public Top As Short
    Public Right As Short
    Public Bottom As Short
    Public Sub New(left As Short, top As Short, right As Short, bottom As Short)
      Me.Left = left
      Me.Top = top
      Me.Right = right
      Me.Bottom = bottom
    End Sub
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Private Structure COORD
    Public X As Short
    Public Y As Short
    Public Sub New(x As Short, y As Short)
      Me.X = x
      Me.Y = y
    End Sub
  End Structure

  Private Structure CHAR_INFO
    Public UnicodeChar As Char
    Public Attributes As Short
  End Structure

  <StructLayout(LayoutKind.Explicit, CharSet:=CharSet.Unicode)>
  Public Structure CharUnion
    <FieldOffset(0)> Public UnicodeChar As Char
    <FieldOffset(0)> Public AsciiChar As Byte
  End Structure

  <StructLayout(LayoutKind.Explicit, CharSet:=CharSet.Unicode)>
  Public Structure CharInfo
    <FieldOffset(0)> Public CharUnion As CharUnion
    <FieldOffset(2)> Public Attributes As Short
  End Structure

  Private Declare Auto Function GetStdHandle Lib "kernel32.dll" (ByVal nStdHandle As Integer) As IntPtr
  Private Declare Function SetConsoleWindowInfo Lib "kernel32" (hConsoleOutput As IntPtr, bAbsolute As Boolean, ByRef lpConsoleWindow As SMALL_RECT) As Boolean
  Private Declare Function SetConsoleScreenBufferSize Lib "kernel32" (hConsoleOutput As IntPtr, dwSize As COORD) As Boolean
  Private Declare Function SetConsoleActiveScreenBuffer Lib "kernel32" (hConsoleOutput As IntPtr) As Boolean
  'Private Declare Unicode Function SetCurrentConsoleFontEx Lib "kernel32" (hConsoleOutput As IntPtr, bMaximumWindow As Boolean, ByRef lpConsoleCurrentFontEx As CONSOLE_FONT_INFOEX) As Boolean
  <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Unicode)>
  Private Shared Function SetCurrentConsoleFontEx(hConsoleOutput As IntPtr, bMaximumWindow As Boolean, lpConsoleCurrentFontEx As CONSOLE_FONT_INFOEX) As Boolean
  End Function
  Private Declare Unicode Function GetConsoleScreenBufferInfo Lib "kernel32.dll" (ByVal hConsoleOutput As IntPtr, ByRef lpConsoleScreenBufferInfo As CONSOLE_SCREEN_BUFFER_INFO) As Boolean
  Private Declare Auto Function SetConsoleMode Lib "kernel32.dll" (ByVal hConsoleHandle As IntPtr, ByVal dwMode As Integer) As Boolean
  Private Delegate Function ConsoleCtrlHandlerRoutine(ByVal dwCtrlType As Integer) As Boolean
  Private Declare Auto Function SetConsoleCtrlHandler Lib "kernel32.dll" (ByVal handlerRoutine As ConsoleCtrlHandlerRoutine, ByVal add As Boolean) As Boolean
  Private Declare Function GetAsyncKeyState Lib "user32.dll" (ByVal virtualKeyCode As Integer) As Short
  Private Declare Function GetNumberOfConsoleInputEvents Lib "kernel32" (ByVal hConsoleInput As IntPtr, ByRef lpcNumberOfEvents As Integer) As Boolean
  Private Declare Function ReadConsoleInput Lib "kernel32" Alias "ReadConsoleInputW" (ByVal hConsoleInput As IntPtr, ByRef lpBuffer As INPUT_RECORD, ByVal nLength As Integer, ByRef lpNumberOfEventsRead As Integer) As Boolean
  Private Declare Function waveOutOpen Lib "winmm.dll" (ByRef hwo As IntPtr, ByVal uDeviceID As Integer, ByRef pwfx As WAVEFORMATEX, ByVal dwCallback As IntPtr, ByVal dwCallbackInstance As IntPtr, ByVal fdwOpen As Integer) As Integer
  Private Declare Function waveOutPrepareHeader Lib "winmm.dll" (ByVal hwo As IntPtr, ByRef pwh As WAVEHDR, ByVal cbwh As Integer) As Integer
  Private Declare Function waveOutUnprepareHeader Lib "winmm.dll" (ByVal hwo As IntPtr, ByRef pwh As WAVEHDR, ByVal cbwh As Integer) As Integer
  Private Declare Function waveOutWrite Lib "winmm.dll" (ByVal hwo As IntPtr, ByRef pwh As WAVEHDR, ByVal cbwh As Integer) As Integer
  Private Declare Function GetLastError Lib "kernel32.dll" () As Integer

  <DllImport("kernel32.dll", SetLastError:=True)>
  Private Shared Function SetConsoleTitle(ByVal lpConsoleTitle As String) As Boolean
  End Function

  Private Declare Function WriteConsoleOutputCharacter Lib "kernel32.dll" Alias "WriteConsoleOutputCharacterA" (hConsoleOutput As IntPtr, lpCharacter As String, nLength As UInteger, dwWriteCoord As COORD, ByRef lpNumberOfCharsWritten As UInteger) As Boolean
  'Private Declare Function WriteConsoleOutput Lib "kernel32.dll" Alias "WriteConsoleOutputA" (ByVal hConsoleOutput As IntPtr, ByVal lpBuffer As CharInfo(), ByVal dwBufferSize As COORD, ByVal dwBufferCoord As COORD, ByRef lpWriteRegion As SMALL_RECT) As Boolean
  Private Declare Unicode Function WriteConsoleOutput Lib "kernel32.dll" Alias "WriteConsoleOutputW" (ByVal hConsoleOutput As IntPtr, ByVal lpBuffer As CharInfo(), ByVal dwBufferSize As COORD, ByVal dwBufferCoord As COORD, ByRef lpWriteRegion As SMALL_RECT) As Boolean

  Private Const WHDR_PREPARED As Integer = &H2
  Private Const WOM_DONE As Integer = &H3BD
  Private Const STD_INPUT_HANDLE As Integer = -10
  Private Const STD_OUTPUT_HANDLE As Integer = -11
  Private Const STD_ERROR_HANDLE As Integer = -12
  Private Const INVALID_HANDLE_VALUE As Integer = -1
  Private Const FF_DONTCARE As Integer = &H0
  Private Const FW_NORMAL As Short = &H400
  Private Const LF_FACESIZE As Integer = 32
  Private Const ENABLE_EXTENDED_FLAGS As Integer = &H80
  Private Const ENABLE_WINDOW_INPUT As Integer = &H20
  Private Const ENABLE_MOUSE_INPUT As Integer = &H10
  Private Const CTRL_CLOSE_EVENT As Integer = 2
  Private Const FOCUS_EVENT As Integer = &H10
  Private Const MOUSE_EVENT As Integer = &H2
  Private Const MOUSE_MOVED As Integer = &H1
  Private Const CALLBACK_FUNCTION As Integer = &H3
  Private Const S_OK As Integer = &H0

#End Region

#Region "VK"

  Public Const VK_SPACE As Integer = &H20
  Public Const VK_LEFT As Integer = &H25
  Public Const VK_RIGHT As Integer = &H27
  Public Const VK_UP As Integer = &H26
  Public Const VK_DOWN As Integer = &H28

#End Region

  Public Property Rand As New Random

  Private m_nScreenWidth As Integer
  Private m_nScreenHeight As Integer
  Private ReadOnly m_hConsole As IntPtr
  Private ReadOnly m_hConsoleIn As IntPtr
  Private ReadOnly m_keyNewState(255) As Short
  Private ReadOnly m_keyOldState(255) As Short
  Private ReadOnly m_mouseOldState(4) As Boolean
  Private ReadOnly m_mouseNewState(4) As Boolean

  Public Structure sKeyState
    Public bPressed As Boolean
    Public bReleased As Boolean
    Public bHeld As Boolean
  End Structure

  Public ReadOnly m_keys(255) As sKeyState
  Public ReadOnly m_mouse(4) As sKeyState

  Private ReadOnly m_hOriginalConsole As IntPtr
  Private m_mousePosX As Integer
  Private m_mousePosY As Integer
  Private m_bEnableSound As Boolean
  Private ReadOnly m_sAppName As String
  Private m_rectWindow As SMALL_RECT
  Private m_bufScreen As CharInfo()
  Private m_bConsoleInFocus = True

  Shared m_bAtomActive = False
  Shared ReadOnly m_cvGameFinished = New System.Threading.AutoResetEvent(False)
  Shared ReadOnly m_muxGame = New System.Threading.Mutex()

  Public Sub New()

    m_nScreenWidth = 80
    m_nScreenHeight = 30

    m_hConsole = GetStdHandle(STD_OUTPUT_HANDLE)
    m_hConsoleIn = GetStdHandle(STD_INPUT_HANDLE)

    Array.Clear(m_keyNewState, 0, 256)
    Array.Clear(m_keyOldState, 0, 256)
    Array.Clear(m_keys, 0, 256)
    m_mousePosX = 0
    m_mousePosY = 0

    m_bEnableSound = False

    m_sAppName = "Default"
  End Sub

  Public Function GetKey(nKeyID As Integer) As sKeyState
    Return m_keys(nKeyID)
  End Function

  Public Function GetMouseX() As Integer
    Return m_mousePosX
  End Function

  Public Function GetMouseY() As Integer
    Return m_mousePosY
  End Function

  Public Function GetMouse(nMouseButtonID As Integer) As sKeyState
    Return m_mouse(nMouseButtonID)
  End Function

  Public Function IsFocused() As Boolean
    Return m_bConsoleInFocus
  End Function

  Public Sub EnableSound()
    m_bEnableSound = True
  End Sub

  Public Function ConstructConsole(width As Integer, height As Integer, fontw As Integer, fonth As Integer) As Integer

    If m_hConsole = INVALID_HANDLE_VALUE Then
      Return GenError("Bad Handle")
    End If

    m_nScreenWidth = width
    m_nScreenHeight = height

    ' Update 13/09/2017 - It seems that the console behaves differently on some systems
    ' and I'm unsure why this is. It could be to do with windows default settings, or
    ' screen resolutions, or system languages. Unfortunately, MSDN does not offer much
    ' by way of useful information, and so the resulting sequence is the reult of experiment
    ' that seems to work in multiple cases.
    '
    ' The problem seems to be that the SetConsoleXXX functions are somewhat circular and 
    ' fail depending on the state of the current console properties, i.e. you can't set
    ' the buffer size until you set the screen size, but you can't change the screen size
    ' until the buffer size is correct. This coupled with a precise ordering of calls
    ' makes this procedure seem a little mystical :-P. Thanks to wowLinh for helping - Jx9

    ' Change console visual size to a minimum so ScreenBuffer can shrink
    ' below the actual visual size
    m_rectWindow = New SMALL_RECT With {
        .Left = 0,
        .Top = 0,
        .Right = 1,
        .Bottom = 1
    }
    SetConsoleWindowInfo(m_hConsole, True, m_rectWindow)

    ' Set the size of the screen buffer
    Dim coord As COORD
    coord.X = m_nScreenWidth
    coord.Y = m_nScreenHeight
    If Not SetConsoleScreenBufferSize(m_hConsole, coord) Then
      Return GenError("SetConsoleScreenBufferSize")
    End If

    ' Assign screen buffer to the console
    If Not SetConsoleActiveScreenBuffer(m_hConsole) Then
      Return GenError("SetConsoleActiveScreenBuffer")
    End If

    ' Set the font size now that the screen buffer has been assigned to the console
    Dim cfi As New CONSOLE_FONT_INFOEX
    'cfi.cbSize = Len(cfi)
    cfi.cbSize = Marshal.SizeOf(cfi)
    cfi.nFont = 0
    cfi.dwFontSize.X = fontw
    cfi.dwFontSize.Y = fonth
    cfi.FontFamily = FF_DONTCARE
    cfi.FontWeight = FW_NORMAL

    ' Dim version As DWORD = GetVersion()
    ' Dim major As DWORD = CByte(LOBYTE(LOWORD(version)))
    ' Dim minor As DWORD = CByte(HIBYTE(LOWORD(version)))

    ' If (major > 6) OrElse ((major = 6) AndAlso (minor >= 2) AndAlso (minor < 4)) Then
    '     wcscpy_s(cfi.FaceName, L"Raster") ' Windows 8 :(
    ' Else
    '     wcscpy_s(cfi.FaceName, L"Lucida Console") ' Everything else :P
    ' End If

    ' wcscpy_s(cfi.FaceName, L"Liberation Mono")

    'wcscpy_s(cfi.FaceName, "Consolas")

    Dim faceName(32) As Char 'Allocate space for a 32-character buffer
    'Dim name = "Consolas"
    'name.CopyTo(faceName) 'Copy the string into the buffer
    'cfi.FaceName = faceName 'Assign the buffer to cfi.FaceName
    cfi.FaceName = "Consolas" '& ChrW(0)
    If Not SetCurrentConsoleFontEx(m_hConsole, False, cfi) Then
      Return GenError("SetCurrentConsoleFontEx")
    End If

    ' Get screen buffer info and check the maximum allowed window size. Return
    ' error if exceeded, so user knows their dimensions/fontsize are too large
    Dim csbi As CONSOLE_SCREEN_BUFFER_INFO
    If Not GetConsoleScreenBufferInfo(m_hConsole, csbi) Then
      Return GenError("GetConsoleScreenBufferInfo")
    End If
    If m_nScreenHeight > csbi.dwMaximumWindowSize.Y Then
      Return GenError("Screen Height / Font Height Too Big")
    End If
    If m_nScreenWidth > csbi.dwMaximumWindowSize.X Then
      Return GenError("Screen Width / Font Width Too Big")
    End If

    ' Set Physical Console Window Size
    m_rectWindow = New SMALL_RECT(0, 0, CShort(m_nScreenWidth - 1), CShort(m_nScreenHeight - 1))
    If Not SetConsoleWindowInfo(m_hConsole, True, m_rectWindow) Then
      Return GenError("SetConsoleWindowInfo")
    End If

    ' Set flags to allow mouse input		
    If Not SetConsoleMode(m_hConsoleIn, ENABLE_EXTENDED_FLAGS Or ENABLE_WINDOW_INPUT Or ENABLE_MOUSE_INPUT) Then
      Return GenError("SetConsoleMode")
    End If

    ' Allocate memory for screen buffer
    m_bufScreen = New CharInfo(m_nScreenWidth * m_nScreenHeight - 1) {}
    Array.Clear(m_bufScreen, 0, m_nScreenWidth * m_nScreenHeight)

    SetConsoleCtrlHandler(AddressOf CloseHandler, True)
    Return 1

  End Function

  Private Function CloseHandler(evt As UInteger) As Boolean

    ' Note that std::unique_lock has been replaced by Threading.Monitor.Enter, 
    ' which is a similar construct in .NET. Also, DWORD has been replaced by
    ' UInteger, since it is an unsigned 32-bit integer in both languages.

    ' Note this gets called in a seperate OS thread, so it must
    ' only exit when the game has finished cleaning up, Or else
    ' the process will be killed before OnUserDestroy() has finished
    If evt = CTRL_CLOSE_EVENT Then
      m_bAtomActive = False
      ' Wait for thread to be exited
      'Dim ul As New Threading.Monitor.Enter(m_muxGame)
      'm_cvGameFinished.Wait(ul)
    End If
    Return True
  End Function

  Private Shared Function GenError(msg As String) As Integer
    Dim lastError = GetLastError
    Dim ex As Exception = New System.ComponentModel.Win32Exception()
    Dim errorMessage As String = ex.Message
    Console.SetOut(New StreamWriter(Console.OpenStandardOutput()))
    Console.WriteLine($"ERROR: {msg}{Environment.NewLine}" & vbTab & $"{errorMessage}")
    Return 0
  End Function

  'Public Overridable Sub Draw(x As Integer, y As Integer, Optional c As Char = "█"c, Optional col As Short = &HF)
  '  If x >= 0 AndAlso x < m_nScreenWidth AndAlso y >= 0 AndAlso y < m_nScreenHeight Then
  '    m_bufScreen(y * m_nScreenWidth + x).CharUnion.UnicodeChar = c
  '    m_bufScreen(y * m_nScreenWidth + x).Attributes = col
  '  End If
  'End Sub

  Public Overridable Sub Draw(x As Integer, y As Integer, Optional c As Short = &H2588S, Optional col As Short = &HFS)
    'Public Overridable Sub Draw(x As Integer, y As Integer, c As Short, Optional col As Short = &HFS)
    If x >= 0 AndAlso x < m_nScreenWidth AndAlso y >= 0 AndAlso y < m_nScreenHeight Then
      m_bufScreen(y * m_nScreenWidth + x).CharUnion.UnicodeChar = ChrW(c)
      m_bufScreen(y * m_nScreenWidth + x).Attributes = col
    End If
  End Sub

  Public Sub Fill(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer, Optional c As Short = &H2588S, Optional col As Short = &HFS)
    Clip(x1, y1)
    Clip(x2, y2)
    For x = x1 To x2 - 1
      For y = y1 To y2 - 1
        Draw(x, y, c, col)
      Next
    Next
  End Sub

  Public Sub DrawString(x As Integer, y As Integer, c As String, Optional col As Short = &HFS)
    For i = 0 To c.Length - 1
      m_bufScreen(y * m_nScreenWidth + x + i).CharUnion.UnicodeChar = c(i)
      m_bufScreen(y * m_nScreenWidth + x + i).Attributes = col
    Next i
  End Sub

  Public Sub DrawStringAlpha(x As Integer, y As Integer, c As String, Optional col As Short = &HFS)
    For i As Integer = 0 To c.Length - 1
      If c(i) <> " "c Then
        m_bufScreen(y * m_nScreenWidth + x + i).CharUnion.UnicodeChar = c(i)
        m_bufScreen(y * m_nScreenWidth + x + i).Attributes = col
      End If
    Next i
  End Sub

  Public Sub Clip(ByRef x As Integer, ByRef y As Integer)
    If x < 0 Then x = 0
    If x >= m_nScreenWidth Then x = m_nScreenWidth
    If y < 0 Then y = 0
    If y >= m_nScreenHeight Then y = m_nScreenHeight
  End Sub

  Private Sub DrawLine(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, Optional ByVal c As Short = &H2588, Optional ByVal col As Short = &HF)
    Dim x, y, dx, dy, dx1, dy1, px, py, xe, ye, i As Integer
    dx = x2 - x1
    dy = y2 - y1
    dx1 = Math.Abs(dx)
    dy1 = Math.Abs(dy)
    px = 2 * dy1 - dx1
    py = 2 * dx1 - dy1
    If dy1 <= dx1 Then
      If dx >= 0 Then
        x = x1
        y = y1
        xe = x2
      Else
        x = x2
        y = y2
        xe = x1
      End If
      Draw(x, y, c, col)
      For i = 0 To (xe - x) - 1
        x += 1
        If px < 0 Then
          px += 2 * dy1
        Else
          If (dx < 0 AndAlso dy < 0) OrElse (dx > 0 AndAlso dy > 0) Then
            y += 1
          Else
            y -= 1
          End If
          px += 2 * (dy1 - dx1)
        End If
        Draw(x, y, c, col)
      Next
    Else
      If dy >= 0 Then
        x = x1
        y = y1
        ye = y2
      Else
        x = x2
        y = y2
        ye = y1
      End If
      Draw(x, y, c, col)
      For i = 0 To (ye - y) - 1
        y += 1
        If py <= 0 Then
          py += 2 * dx1
        Else
          If (dx < 0 AndAlso dy < 0) OrElse (dx > 0 AndAlso dy > 0) Then
            x += 1
          Else
            x -= 1
          End If
          py += 2 * (dx1 - dy1)
        End If
        Draw(x, y, c, col)
      Next
    End If
  End Sub

  Private Sub DrawTriangle(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, ByVal x3 As Integer, ByVal y3 As Integer, Optional ByVal c As Short = &H2588, Optional ByVal col As Short = &HF)
    DrawLine(x1, y1, x2, y2, c, col)
    DrawLine(x2, y2, x3, y3, c, col)
    DrawLine(x3, y3, x1, y1, c, col)
  End Sub

  Private Shared Sub SWAP(ByRef x As Integer, ByRef y As Integer)
    Dim t = x
    x = y
    y = t
  End Sub

  Public Sub FillTriangle(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer, x3 As Integer, y3 As Integer, Optional c As Short = &H2588, Optional col As Short = &HF)

    Dim drawline As Action(Of Integer, Integer, Integer) = Sub(sx As Integer, ex As Integer, ny As Integer)
                                                             For i As Integer = sx To ex
                                                               Draw(i, ny, c, col)
                                                             Next
                                                           End Sub

    Dim t1x, t2x, y, minx, maxx, t1xp, t2xp As Integer
    Dim changed1 As Boolean = False
    Dim changed2 As Boolean = False
    Dim signx1, signx2, dx1, dy1, dx2, dy2 As Integer
    Dim e1, e2 As Integer
    ' Sort vertices
    If y1 > y2 Then SWAP(y1, y2) : SWAP(x1, x2)
    If y1 > y3 Then SWAP(y1, y3) : SWAP(x1, x3)
    If y2 > y3 Then SWAP(y2, y3) : SWAP(x2, x3)

    t1x = x1 : t2x = x1 : y = y1 ' Starting points
    dx1 = Math.Abs(x2 - x1) : If dx1 < 0 Then dx1 = -dx1 : signx1 = -1 Else signx1 = 1
    dy1 = Math.Abs(y2 - y1)
    dx2 = Math.Abs(x3 - x1) : If dx2 < 0 Then dx2 = -dx2 : signx2 = -1 Else signx2 = 1
    dy2 = Math.Abs(y3 - y1)

    If dy1 > dx1 Then ' swap values
      SWAP(dx1, dy1)
      changed1 = True
    End If
    If dy2 > dx2 Then ' swap values
      SWAP(dy2, dx2)
      changed2 = True
    End If

    e2 = CInt(dx2 \ 2)
    ' Flat top, just process the second half
    'If y1 = y2 Then GoTo [next]
    e1 = CInt(dx1 \ 2)

    For i As Integer = 0 To dx1 - 1
      t1xp = 0 : t2xp = 0
      If t1x < t2x Then minx = t1x : maxx = t2x Else minx = t2x : maxx = t1x
      ' process first line until y value is about to change
      While i < dx1
        i += 1
        e1 += dy1
        While e1 >= dx1
          e1 -= dx1
          If changed1 Then t1xp = signx1 Else GoTo next1 't1x += signx1;
        End While
        If changed1 Then Exit While Else t1x += signx1
      End While
      ' Move line
next1:
      ' process second line until y value is about to change
      While True
        e2 += dy2
        While e2 >= dx2
          e2 -= dx2
          If changed2 Then t2xp = signx2 Else GoTo next2 't2x += signx2;
        End While
        If changed2 Then Exit While Else t2x += signx2
      End While
next2:
      If minx > t1x Then minx = t1x : If minx > t2x Then minx = t2x
      If maxx < t1x Then maxx = t1x : If maxx < t2x Then maxx = t2x
      drawline(minx, maxx, y) ' Draw line from min to max points found on current y level
      If Not changed1 Then t1x += signx1 ' Move endpoint 1 to next position
      t1x += t1xp
      If Not changed2 Then t2x += signx2 ' Move endpoint 2 to next position
      t2x += t2xp
    Next
  End Sub

  Sub DrawCircle(ByVal xc As Integer, ByVal yc As Integer, ByVal r As Integer, Optional ByVal c As Short = &H2588, Optional ByVal col As Short = &HF)
    Dim x As Integer = 0
    Dim y As Integer = r
    Dim p As Integer = 3 - 2 * r
    If r = 0 Then Return
    While y >= x ' only formulate 1/8 of circle
      Draw(xc - x, yc - y, c, col) 'upper left left
      Draw(xc - y, yc - x, c, col) 'upper upper left
      Draw(xc + y, yc - x, c, col) 'upper upper right
      Draw(xc + x, yc - y, c, col) 'upper right right
      Draw(xc - x, yc + y, c, col) 'lower left left
      Draw(xc - y, yc + x, c, col) 'lower lower left
      Draw(xc + y, yc + x, c, col) 'lower lower right
      Draw(xc + x, yc + y, c, col) 'lower right right
      If p < 0 Then p += 4 * x + 6 : x += 1 Else p += 4 * (x - y) + 10 : x += 1 : y -= 1
    End While
  End Sub

  Sub FillCircle(ByVal xc As Integer, ByVal yc As Integer, ByVal r As Integer, Optional ByVal c As Short = &H2588, Optional ByVal col As Short = &HF)
    Dim x As Integer = 0
    Dim y As Integer = r
    Dim p As Integer = 3 - 2 * r
    If r = 0 Then Return
    Dim drawline = Sub(ByVal sx As Integer, ByVal ex As Integer, ByVal ny As Integer)
                     For i As Integer = sx To ex
                       Draw(i, ny, c, col)
                     Next
                   End Sub

    While y >= x
      ' Modified to draw scan-lines instead of edges
      drawline(xc - x, xc + x, yc - y)
      drawline(xc - y, xc + y, yc - x)
      drawline(xc - x, xc + x, yc + y)
      drawline(xc - y, xc + y, yc + x)
      If p < 0 Then p += 4 * x + 6 : x += 1 Else p += 4 * (x - y) + 10 : x += 1 : y -= 1
    End While
  End Sub

  Sub DrawSprite(x As Integer, y As Integer, sprite As Sprite)
    If sprite Is Nothing Then
      Return
    End If
    For i As Integer = 0 To sprite.Width - 1
      For j As Integer = 0 To sprite.Height - 1
        If sprite.GetGlyph(i, j) <> " "c Then
          Draw(x + i, y + j, AscW(sprite.GetGlyph(i, j)), sprite.GetColour(i, j))
        End If
      Next
    Next
  End Sub

  Sub DrawPartialSprite(x As Integer, y As Integer, sprite As Sprite, ox As Integer, oy As Integer, w As Integer, h As Integer)
    If sprite Is Nothing Then
      Return
    End If
    For i As Integer = 0 To w - 1
      For j As Integer = 0 To h - 1
        If sprite.GetGlyph(i + ox, j + oy) <> " "c Then
          Draw(x + i, y + j, AscW(sprite.GetGlyph(i + ox, j + oy)), sprite.GetColour(i + ox, j + oy))
        End If
      Next
    Next
  End Sub

  Sub DrawWireFrameModel(vecModelCoordinates As List(Of (x As Single, y As Single)), x As Single, y As Single, Optional r As Single = 0.0F, Optional s As Single = 1.0F, Optional col As Short = Colour.FG_WHITE, Optional c As Short = PIXEL_TYPE.PIXEL_SOLID)
    ' Tuple.Item1 = x coordinate
    ' Tuple.Item2 = y coordinate
    ' Create translated model vector of coordinate pairs
    Dim vecTransformedCoordinates As New List(Of (x As Single, y As Single))
    Dim verts As Integer = vecModelCoordinates.Count()
    vecTransformedCoordinates.Capacity = verts

    ' Rotate
    For i As Integer = 0 To verts - 1
      Dim coordX As Single = vecModelCoordinates(i).x
      Dim coordY As Single = vecModelCoordinates(i).y
      vecTransformedCoordinates.Add((coordX * Math.Cos(r) - coordY * Math.Sin(r), coordX * Math.Sin(r) + coordY * Math.Cos(r)))
    Next

    ' Scale
    For i As Integer = 0 To verts - 1
      vecTransformedCoordinates(i) = (vecTransformedCoordinates(i).x * s, vecTransformedCoordinates(i).y * s)
    Next

    ' Translate
    For i As Integer = 0 To verts - 1
      vecTransformedCoordinates(i) = (vecTransformedCoordinates(i).x + x, vecTransformedCoordinates(i).y + y)
    Next

    ' Draw Closed Polygon
    For i As Integer = 0 To verts - 1
      Dim j As Integer = (i + 1) Mod verts
      DrawLine(CInt(vecTransformedCoordinates(i).x), CInt(vecTransformedCoordinates(i).y), CInt(vecTransformedCoordinates(j).x), CInt(vecTransformedCoordinates(j).y), c, col)
    Next

  End Sub

  Public Sub Start()
    ' Start the thread
    m_bAtomActive = True
    Dim t As New Threading.Thread(AddressOf GameThread)
    ' Wait for thread to be exited
    t.Start()
    't.Join()
  End Sub

  Public Function ScreenWidth() As Integer
    Return m_nScreenWidth
  End Function

  Public Function ScreenHeight() As Integer
    Return m_nScreenHeight
  End Function

  Private Sub GameThread()
    ' Create user resources as part of this thread
    If Not OnUserCreate() Then
      m_bAtomActive = False
    End If
    ' Check if sound system should be enabled
    If m_bEnableSound Then
      If Not CreateAudio() Then
        m_bAtomActive = False ' Failed to create audio system			
        m_bEnableSound = False
      End If
    End If

    Dim tp1 As DateTime = DateTime.Now
    Dim tp2 As DateTime = DateTime.Now

    While m_bAtomActive
      ' Run as fast as possible
      While m_bAtomActive
        ' Handle Timing
        tp2 = DateTime.Now
        Dim elapsedTime As TimeSpan = tp2 - tp1
        tp1 = tp2
        Dim fElapsedTime As Single = elapsedTime.TotalSeconds

        ' Handle Keyboard Input
        For i As Integer = 0 To 255
          m_keyNewState(i) = GetAsyncKeyState(i)

          m_keys(i).bPressed = False
          m_keys(i).bReleased = False

          If m_keyNewState(i) <> m_keyOldState(i) Then
            If (m_keyNewState(i) And &H8000) <> 0 Then
              m_keys(i).bPressed = Not m_keys(i).bHeld
              m_keys(i).bHeld = True
            Else
              m_keys(i).bReleased = True
              m_keys(i).bHeld = False
            End If
          End If

          m_keyOldState(i) = m_keyNewState(i)
        Next

        ' Handle Mouse Input - Check for window events
        Dim inBuf(31) As INPUT_RECORD
        Dim events As Integer = 0
        GetNumberOfConsoleInputEvents(m_hConsoleIn, events)
        If events > 0 Then
          ReadConsoleInput(m_hConsoleIn, inBuf(0), events, events)
        End If

        ' Handle events - we only care about mouse clicks and movement
        ' for now
        For i As Integer = 0 To events - 1
          Select Case inBuf(i).EventType
            Case FOCUS_EVENT
              m_bConsoleInFocus = inBuf(i).FocusEvent.bSetFocus
            Case MOUSE_EVENT
              Select Case inBuf(i).MouseEvent.dwEventFlags
                Case MOUSE_MOVED
                  m_mousePosX = inBuf(i).MouseEvent.dwMousePosition.X
                  m_mousePosY = inBuf(i).MouseEvent.dwMousePosition.Y
                Case 0
                  For m As Integer = 0 To 4
                    m_mouseNewState(m) = (inBuf(i).MouseEvent.dwButtonState And (1 << m)) > 0
                  Next
                Case Else
                  ' Do nothing
              End Select
            Case Else
              ' Do nothing
          End Select
        Next

        For m As Integer = 0 To 4
          m_mouse(m).bPressed = False
          m_mouse(m).bReleased = False

          If m_mouseNewState(m) <> m_mouseOldState(m) Then
            If m_mouseNewState(m) Then
              m_mouse(m).bPressed = True
              m_mouse(m).bHeld = True
            Else
              m_mouse(m).bReleased = True
              m_mouse(m).bHeld = False
            End If
          End If

          m_mouseOldState(m) = m_mouseNewState(m)
        Next

        ' Handle Frame Update
        If Not OnUserUpdate(fElapsedTime) Then
          m_bAtomActive = False
        End If

        ' Update Title & Present Screen Buffer
        Dim s As String = String.Format("OneLoneCoder.com - Console Game Engine - {0} - FPS: {1:0.00}", m_sAppName, 1.0F / fElapsedTime)
        SetConsoleTitle(s)
        WriteConsoleOutput(m_hConsole, m_bufScreen, New COORD(m_nScreenWidth, m_nScreenHeight), New COORD(0, 0), m_rectWindow)

        If m_bEnableSound Then
          ' Close and Clean up audio system
        End If

        'Allow the user to free resources if they have overridden the destroy function
        If OnUserDestroy() Then
          'Delete resources and exit if the user has permitted destroy
          Erase m_bufScreen
          SetConsoleActiveScreenBuffer(m_hOriginalConsole)
          'm_cvGameFinished.notify_one()
        Else
          'User denied destroy for some reason, so continue running
          m_bAtomActive = True
        End If

      End While
    End While
  End Sub

  MustOverride Function OnUserCreate() As Boolean
  MustOverride Function OnUserUpdate(elapsedTime As Single) As Boolean

  Overridable Function OnUserDestroy() As Boolean
    Return False
  End Function

  Private Class AudioSample

    Sub New()
    End Sub

    Public Sub New(ByVal sWavFile As String)
      ' Load Wav file and convert to float format
      Dim f As System.IO.FileStream '= Nothing
      Try
        f = New System.IO.FileStream(sWavFile, System.IO.FileMode.Open, System.IO.FileAccess.Read)
      Catch ex As Exception
        Return
      End Try

      Dim dump(3) As Byte
      f.Read(dump, 0, 4) ' Read "RIFF"
      If System.Text.Encoding.ASCII.GetString(dump, 0, 4) <> "RIFF" Then Return
      f.Read(dump, 0, 4) ' Not Interested
      f.Read(dump, 0, 4) ' Read "WAVE"
      If System.Text.Encoding.ASCII.GetString(dump, 0, 4) <> "WAVE" Then Return

      ' Read Wave description chunk
      f.Read(dump, 0, 4) ' Read "fmt "
      f.Read(dump, 0, 4) ' Not Interested
      wavHeader = New WAVEFORMATEX() With {.wFormatTag = 0}
      Dim length = Marshal.SizeOf(wavHeader) - 2 ' -2 because structure has 2 bytes to indicate its own size which are not in the wav file
      Dim headerBytes(length - 1) As Byte
      f.Read(headerBytes, 0, length)
      Dim headerPtr = Marshal.AllocHGlobal(length)
      Marshal.Copy(headerBytes, 0, headerPtr, length)
      Marshal.PtrToStructure(headerPtr, wavHeader)
      Marshal.FreeHGlobal(headerPtr)

      ' Just check if wave format is compatible with olcCGE
      If wavHeader.wBitsPerSample <> 16 OrElse wavHeader.nSamplesPerSec <> 44100 Then
        f.Close()
        Return
      End If

      ' Search for audio data chunk
      Dim nChunksize As Long = 0
      f.Read(dump, 0, 4) ' Read chunk header
      f.Read(BitConverter.GetBytes(nChunksize), 0, 4) ' Read chunk size
      While System.Text.Encoding.ASCII.GetString(dump, 0, 4) <> "data"
        ' Not audio data, so just skip it
        f.Seek(nChunksize, System.IO.SeekOrigin.Current)
        f.Read(dump, 0, 4)
        f.Read(BitConverter.GetBytes(nChunksize), 0, 4)
      End While

      ' Finally got to data, so read it all in and convert to float samples
      nSamples = nChunksize / (wavHeader.nChannels * (wavHeader.wBitsPerSample >> 3))
      nChannels = wavHeader.nChannels

      ' Create floating point buffer to hold audio sample
      fSample = New Single(nSamples * nChannels - 1) {}

      ' Read in audio data and normalise
      For i As Long = 0 To nSamples - 1
        For c As Integer = 0 To nChannels - 1
          Dim s As Short = 0
          f.Read(BitConverter.GetBytes(s), 0, 2)
          fSample(i * nChannels + c) = s / Short.MaxValue
        Next
      Next

      ' All done, flag sound as valid
      f.Close()
      bSampleValid = True
    End Sub

    Public wavHeader As WAVEFORMATEX
    Public fSample As Single()
    Public nSamples As Long
    Public nChannels As Integer
    Public bSampleValid As Boolean

  End Class

  Private ReadOnly vecAudioSamples As New List(Of AudioSample)

  Structure sCurrentlyPlayingSample
    Public nAudioSampleID As Integer '= 0
    Public nSamplePosition As Long '= 0
    Public bFinished As Boolean '= False
    Public bLoop As Boolean '= False
  End Structure

  Private ReadOnly listActiveSamples As New List(Of sCurrentlyPlayingSample)

  Function LoadAudioSample(sWavFile As String) As UInteger
    If Not m_bEnableSound Then
      Return UInteger.MaxValue
    End If
    Dim a As New AudioSample(sWavFile)
    If a.bSampleValid Then
      vecAudioSamples.Add(a)
      Return CUInt(vecAudioSamples.Count)
    Else
      Return UInteger.MaxValue
    End If
  End Function

  ' Add sample 'id' to the mixers sounds to play list
  Sub PlaySample(id As Integer, Optional bLoop As Boolean = False)
    Dim a As sCurrentlyPlayingSample
    a.nAudioSampleID = id
    a.nSamplePosition = 0
    a.bFinished = False
    a.bLoop = bLoop
    listActiveSamples.Add(a)
  End Sub

  Shared Sub StopSample(id As Integer)

  End Sub

  Public Const WAVE_MAPPER As Integer = -1

  ' The audio system uses by default a specific wave format
  Public Function CreateAudio(Optional ByVal nSampleRate As UInteger = 44100,
                             Optional ByVal nChannels As UInteger = 1,
                             Optional ByVal nBlocks As UInteger = 8,
                             Optional ByVal nBlockSamples As UInteger = 512) As Boolean

    ' Initialise Sound Engine
    m_bAudioThreadActive = False
    m_nSampleRate = nSampleRate
    m_nChannels = nChannels
    m_nBlockCount = nBlocks
    m_nBlockSamples = nBlockSamples
    m_nBlockFree = m_nBlockCount
    m_nBlockCurrent = 0
    m_pBlockMemory = Nothing
    m_pWaveHeaders = Nothing

    ' Device is available
    Dim waveFormat As New WAVEFORMATEX With {
        .wFormatTag = WAVE_FORMAT_PCM,
        .nSamplesPerSec = m_nSampleRate,
        .wBitsPerSample = CShort(8 * Marshal.SizeOf(Of Short)),
        .nChannels = m_nChannels,
        .nBlockAlign = CShort(waveFormat.wBitsPerSample / 8 * waveFormat.nChannels),
        .nAvgBytesPerSec = waveFormat.nSamplesPerSec * waveFormat.nBlockAlign,
        .cbSize = 0
    }

    ' Open Device if valid
    'If waveOutOpen(m_hwDevice, WAVE_MAPPER, waveFormat, AddressOf waveOutProcWrap, Me, CALLBACK_FUNCTION) <> S_OK Then
    '  Return DestroyAudio()
    'End If

    ' Allocate Wave|Block Memory
    m_pBlockMemory = New Short(m_nBlockCount * m_nBlockSamples - 1) {}
    If m_pBlockMemory Is Nothing Then
      Return DestroyAudio()
    End If
    Array.Clear(m_pBlockMemory, 0, m_nBlockCount * m_nBlockSamples)

    m_pWaveHeaders = New WAVEHDR(m_nBlockCount - 1) {}
    If m_pWaveHeaders Is Nothing Then
      Return DestroyAudio()
    End If
    Array.Clear(m_pWaveHeaders, 0, m_nBlockCount)

    ' Link headers to block memory
    For n As Integer = 0 To m_nBlockCount - 1
      m_pWaveHeaders(n).dwBufferLength = m_nBlockSamples * Marshal.SizeOf(Of Short)()
      'm_pWaveHeaders(n).lpData = Marshal.StringToHGlobalAnsi(CType(m_pBlockMemory + (n * m_nBlockSamples), String))
    Next

    m_bAudioThreadActive = True
    m_AudioThread = New System.Threading.Thread(AddressOf AudioThread)
    m_AudioThread.Start()

    ' Start the ball rolling with the sound delivery thread
    Dim lm As New Object()
    SyncLock lm
      Monitor.Pulse(m_muxBlockNotZero)
    End SyncLock

    Return True

  End Function

  Private Const WAVE_FORMAT_PCM As UShort = &H1

  ' Stop and clean up audio system
  Public Function DestroyAudio() As Boolean
    m_bAudioThreadActive = False
    Return False
  End Function

  ' Handler for soundcard request for more data
  Private Sub WaveOutProc(ByVal hWaveOut As IntPtr, ByVal uMsg As UInteger, ByVal dwParam1 As UInteger, ByVal dwParam2 As UInteger)
    If uMsg <> WOM_DONE Then Return
    m_nBlockFree += 1
    'Dim lm As New Threading.Mutex(m_muxBlockNotZero)
    'm_cvBlockNotZero.NotifyOne()
  End Sub

  ' Static wrapper for sound card handler
  Private Shared Sub WaveOutProcWrap(ByVal hWaveOut As IntPtr, ByVal uMsg As UInteger, ByVal dwInstance As UInteger, ByVal dwParam1 As UInteger, ByVal dwParam2 As UInteger)
    CType(dwInstance, ConsoleGameEngine).WaveOutProc(hWaveOut, uMsg, dwParam1, dwParam2)
  End Sub

  Public Shared Narrowing Operator CType(v As UInteger) As ConsoleGameEngine
    Throw New NotImplementedException()
  End Operator

  ' Audio thread. This loop responds to requests from the soundcard to fill 'blocks'
  ' with audio data. If no requests are available it goes dormant until the sound
  ' card is ready for more data. The block is fille by the "user" in some manner
  ' and then issued to the soundcard.
  Private Sub AudioThread()

    m_fGlobalTime = 0.0F
    Dim fTimeStep As Single = 1.0F / CSng(m_nSampleRate)

    ' Goofy hack to get maximum integer for a type at run-time
    Dim nMaxSample As Short = CShort(Math.Pow(2, (2 * 8) - 1)) - 1
    Dim fMaxSample As Single = CSng(nMaxSample)
    Dim nPreviousSample As Short = 0

    While m_bAudioThreadActive
      ' Wait for block to become available
      If m_nBlockFree = 0 Then
        Dim lm As New Threading.Mutex()
        lm.WaitOne()

        While m_nBlockFree = 0 ' sometimes, Windows signals incorrectly
          m_cvBlockNotZero.WaitOne()
        End While
      End If

      ' Block is here, so use it
      m_nBlockFree -= 1

      ' Prepare block for processing
      If m_pWaveHeaders(m_nBlockCurrent).dwFlags And WHDR_PREPARED Then
        WaveOutUnprepareHeader(m_hwDevice, m_pWaveHeaders(m_nBlockCurrent), CUInt(Marshal.SizeOf(m_pWaveHeaders(m_nBlockCurrent))))
      End If

      Dim nNewSample As Short = 0
      Dim nCurrentBlock As Integer = m_nBlockCurrent * m_nBlockSamples

      Dim clip = Function(fSample As Single, fMax As Single) As Single
                   If fSample >= 0.0F Then
                     Return Math.Min(fSample, fMax)
                   Else
                     Return Math.Max(fSample, -fMax)
                   End If
                 End Function

      For n As UInteger = 0 To m_nBlockSamples - 1 Step m_nChannels
        ' User Process
        For c As UInteger = 0 To m_nChannels - 1
          nNewSample = CShort(clip(GetMixerOutput(c, m_fGlobalTime, fTimeStep), 1.0F) * fMaxSample)
          m_pBlockMemory(nCurrentBlock + n + c) = nNewSample
          nPreviousSample = nNewSample
        Next

        m_fGlobalTime += fTimeStep
      Next

      ' Send block to sound device
      Dim junk = waveOutPrepareHeader(m_hwDevice, m_pWaveHeaders(m_nBlockCurrent), CUInt(Marshal.SizeOf(m_pWaveHeaders(m_nBlockCurrent))))
      junk = waveOutWrite(m_hwDevice, m_pWaveHeaders(m_nBlockCurrent), CUInt(Marshal.SizeOf(m_pWaveHeaders(m_nBlockCurrent))))
      m_nBlockCurrent += 1
      m_nBlockCurrent = m_nBlockCurrent Mod m_nBlockCount
    End While

  End Sub

  ' Overridden by user if they want to generate sound in real-time
  Public Overridable Function OnUserSoundSample(ByVal nChannel As Integer, ByVal fGlobalTime As Single, ByVal fTimeStep As Single) As Single
    Return 0.0F
  End Function

  ' Overriden by user if they want to manipulate the sound before it is played
  Public Overridable Function OnUserSoundFilter(ByVal nChannel As Integer, ByVal fGlobalTime As Single, ByVal fSample As Single) As Single
    Return fSample
  End Function

  ' The Sound Mixer - If the user wants to play many sounds simultaneously, and
  ' perhaps the same sound overlapping itself, then you need a mixer, which
  ' takes input from all sound sources for that audio frame. This mixer maintains
  ' a list of sound locations for all concurrently playing audio samples. Instead
  ' of duplicating audio data, we simply store the fact that a sound sample is in
  ' use and an offset into its sample data. As time progresses we update this offset
  ' until it is beyond the length of the sound sample it is attached to. At this
  ' point we remove the playing sound from the list.
  '
  ' Additionally, the users application may want to generate sound instead of just
  ' playing audio clips (think a synthesizer for example) in which case we also
  ' provide an "onUser..." event to allow the user to return a sound for that point
  ' in time.
  '
  ' Finally, before the sound is issued to the operating system for performing, the
  ' user gets one final chance to "filter" the sound, perhaps changing the volume
  ' or adding funky effects.
  Function GetMixerOutput(ByVal nChannel As Integer, ByVal fGlobalTime As Single, ByVal fTimeStep As Single) As Single
    ' Accumulate sample for this channel
    Dim fMixerSample As Single = 0.0F

    For Each s As sCurrentlyPlayingSample In listActiveSamples
      ' Calculate sample position
      s.nSamplePosition += CLng(vecAudioSamples(s.nAudioSampleID - 1).wavHeader.nSamplesPerSec * fTimeStep)

      ' If sample position is valid add to the mix
      If s.nSamplePosition < vecAudioSamples(s.nAudioSampleID - 1).nSamples Then
        fMixerSample += vecAudioSamples(s.nAudioSampleID - 1).fSample((s.nSamplePosition * vecAudioSamples(s.nAudioSampleID - 1).nChannels) + nChannel)
      Else
        s.bFinished = True ' Else sound has completed
      End If
    Next

    ' If sounds have completed then remove them
    listActiveSamples.RemoveAll(Function(s As sCurrentlyPlayingSample) s.bFinished)

    ' The users application might be generating sound, so grab that if it exists
    fMixerSample += OnUserSoundSample(nChannel, fGlobalTime, fTimeStep)

    ' Return the sample via an optional user override to filter the sound
    Return OnUserSoundFilter(nChannel, fGlobalTime, fMixerSample)
  End Function

  Dim m_nSampleRate As UInteger
  Dim m_nChannels As UInteger
  Dim m_nBlockCount As UInteger
  Dim m_nBlockSamples As UInteger
  Dim m_nBlockCurrent As UInteger

  Dim m_pBlockMemory As Short() = Nothing
  Dim m_pWaveHeaders As WAVEHDR() = Nothing
  ReadOnly m_hwDevice As IntPtr = IntPtr.Zero

  Dim m_AudioThread As Thread
  Dim m_bAudioThreadActive As Boolean = False
  Dim m_nBlockFree As Integer = 0
  ReadOnly m_cvBlockNotZero As New Threading.ManualResetEvent(False)
  ReadOnly m_muxBlockNotZero As New Threading.Mutex()
  Dim m_fGlobalTime As Single

End Class