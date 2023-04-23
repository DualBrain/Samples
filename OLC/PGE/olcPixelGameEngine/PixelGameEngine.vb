Option Explicit On
Option Strict On
Option Infer On

Imports System.Data.Common
Imports System.Formats.Tar
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.OSPlatform
Imports System.Runtime.InteropServices.RuntimeInformation
Imports System.Threading
Imports System.Xml

Delegate Function WndProc(hWnd As IntPtr, msg As UInteger, wParam As IntPtr, lParam As IntPtr) As IntPtr

Public Module Singleton

  Public AtomActive As Boolean
  Public MapKeys As New Dictionary(Of Integer, Integer)
  Public Pge As PixelGameEngine

End Module

Public MustInherit Class PixelGameEngine

#Region "Win32"

  Private Const VK_F1 As Integer = &H70
  Private Const VK_F2 As Integer = &H71
  Private Const VK_F3 As Integer = &H72
  Private Const VK_F4 As Integer = &H73
  Private Const VK_F5 As Integer = &H74
  Private Const VK_F6 As Integer = &H75
  Private Const VK_F7 As Integer = &H76
  Private Const VK_F8 As Integer = &H77
  Private Const VK_F9 As Integer = &H78
  Private Const VK_F10 As Integer = &H79
  Private Const VK_F11 As Integer = &H7A
  Private Const VK_F12 As Integer = &H7B

  Private Const VK_DOWN As Integer = &H28
  Private Const VK_LEFT As Integer = &H25
  Private Const VK_RIGHT As Integer = &H27
  Private Const VK_UP As Integer = &H26
  Private Const VK_RETURN As Integer = &HD
  Private Const VK_BACK As Integer = &H8
  Private Const VK_ESCAPE As Integer = &H1B
  Private Const VK_PAUSE As Integer = &H13
  Private Const VK_SCROLL As Integer = &H91
  Private Const VK_TAB As Integer = &H9
  Private Const VK_DELETE As Integer = &H2E
  Private Const VK_HOME As Integer = &H24
  Private Const VK_END As Integer = &H23
  Private Const VK_PRIOR As Integer = &H21
  Private Const VK_NEXT As Integer = &H22
  Private Const VK_INSERT As Integer = &H2D
  Private Const VK_SHIFT As Integer = &H10
  Private Const VK_CONTROL As Integer = &H11
  Private Const VK_SPACE As Integer = &H20

  Private Const VK_NUMPAD0 As Integer = &H60
  Private Const VK_NUMPAD1 As Integer = &H61
  Private Const VK_NUMPAD2 As Integer = &H62
  Private Const VK_NUMPAD3 As Integer = &H63
  Private Const VK_NUMPAD4 As Integer = &H64
  Private Const VK_NUMPAD5 As Integer = &H65
  Private Const VK_NUMPAD6 As Integer = &H66
  Private Const VK_NUMPAD7 As Integer = &H67
  Private Const VK_NUMPAD8 As Integer = &H68
  Private Const VK_NUMPAD9 As Integer = &H69

  Private Const VK_MULTIPLY As Integer = &H6A
  Private Const VK_ADD As Integer = &H6B
  Private Const VK_DIVIDE As Integer = &H6F
  Private Const VK_SUBTRACT As Integer = &H6D
  Private Const VK_DECIMAL As Integer = &H6E

  Private Const CS_USEDEFAULT As UInteger = &H80000000UI
  Private Const CS_DBLCLKS As UInteger = 8
  Private Const CS_VREDRAW As UInteger = 1
  Private Const CS_HREDRAW As UInteger = 2
  Private Const CS_OWNDC As Integer = &H20

  Private Const TME_LEAVE As Integer = &H2

  Private Const COLOR_WINDOW As UInteger = 5
  Private Const COLOR_BACKGROUND As UInteger = 1

  Private Const IDI_APPLICATION As Integer = &H7F00

  Private Const IDC_CROSS As UInteger = 32515
  Private Const IDC_ARROW As Integer = 32512

  Private Const WM_DESTROY As UInteger = 2
  Private Const WM_PAINT As UInteger = &HF
  Private Const WM_LBUTTONUP As UInteger = &H202
  Private Const WM_LBUTTONDBLCLK As UInteger = &H203
  Private Const WM_MOUSEMOVE As UInteger = &H200
  Private Const WM_CLOSE As UInteger = &H10
  Private Const WM_MBUTTONUP As Integer = &H208
  Private Const WM_MBUTTONDOWN As Integer = &H207
  Private Const WM_RBUTTONUP As Integer = &H205
  Private Const WM_RBUTTONDOWN As Integer = &H204
  Private Const WM_LBUTTONDOWN As Integer = &H201
  Private Const WM_KEYUP As Integer = &H101
  Private Const WM_KEYDOWN As Integer = &H100
  Private Const WM_KILLFOCUS As Integer = &H8
  Private Const WM_SETFOCUS As Integer = &H7
  Private Const WM_MOUSELEAVE As Integer = &H2A3
  Private Const WM_MOUSEWHEEL As Integer = &H20A
  Private Const WM_SIZE As Integer = &H5
  Private Const WM_CREATE As Integer = &H1

  Private Const WS_OVERLAPPEDWINDOW As UInteger = &HCF0000
  Private Const WS_VISIBLE As UInteger = &H10000000
  Private Const WS_EX_APPWINDOW As UInteger = &H40000UI
  Private Const WS_EX_WINDOWEDGE As UInteger = &H100
  Private Const WS_CAPTION As UInteger = &HC00000
  Private Const WS_SYSMENU As UInteger = &H80000
  Private Const WS_THICKFRAME As UInteger = &H40000
  Private Const WS_POPUP As UInteger = &H80000000UI

  Private Const MONITOR_DEFAULTTONEAREST As Integer = &H2

  <StructLayout(LayoutKind.Sequential)>
  Public Class CREATESTRUCT
    Public lpCreateParams As IntPtr
    Public hInstance As IntPtr
    Public hMenu As IntPtr
    Public hwndParent As IntPtr
    Public cy As Integer
    Public cx As Integer
    Public y As Integer
    Public x As Integer
    Public style As Integer
    Public lpszName As String
    Public lpszClass As String
    Public dwExStyle As UInteger
  End Class

  <StructLayout(LayoutKind.Sequential)>
  Private Structure Point
    Public X As Integer
    Public Y As Integer
    Public Sub New(x As Integer, y As Integer)
      Me.X = x
      Me.Y = y
    End Sub
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Private Structure RECT
    Public Left As Integer
    Public Top As Integer
    Public Right As Integer
    Public Bottom As Integer
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Private Structure MONITORINFO
    Public cbSize As Integer
    Public rcMonitor As RECT
    Public rcWork As RECT
    Public dwFlags As UInt32
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Private Structure MSG
    Public hWnd As IntPtr
    Public message As UInteger
    Public wParam As IntPtr
    Public lParam As IntPtr
    Public time As Integer
    Public pt As Point
  End Structure

  '<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
  'Private Structure WNDCLASSEX
  '  <MarshalAs(UnmanagedType.U4)> Public Size As Integer
  '  <MarshalAs(UnmanagedType.U4)> Public Style As Integer
  '  Public WndProc As IntPtr
  '  Public ClsExtra As Integer
  '  Public WndExtra As Integer
  '  Public hInstance As IntPtr
  '  Public hIcon As IntPtr
  '  Public hCursor As IntPtr
  '  Public hBackground As IntPtr
  '  Public MenuName As String
  '  Public ClassName As String
  '  Public hIconSm As IntPtr
  'End Structure

  <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
  Public Structure WNDCLASS
    <MarshalAs(UnmanagedType.U4)> Public Style As Integer
    Public WndProc As IntPtr
    Public ClsExtra As Integer
    Public WndExtra As Integer
    Public hInstance As IntPtr
    Public hIcon As IntPtr
    Public hCursor As IntPtr
    Public hBackground As IntPtr
    Public MenuName As String
    Public ClassName As String
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Private Structure TRACKMOUSEEVENTSTRUCT
    <MarshalAs(UnmanagedType.U4)> Public cbSize As Integer
    <MarshalAs(UnmanagedType.U4)> Public dwFlags As Integer
    Public hWnd As IntPtr
    <MarshalAs(UnmanagedType.U4)> Public dwHoverTime As Integer
  End Structure

  Private ReadOnly m_delegWndProc As WndProc = AddressOf olc_WindowEvent

  Private Declare Function UpdateWindow Lib "user32.dll" (hWnd As IntPtr) As Boolean
  Private Declare Function ShowWindow Lib "user32.dll" (hWnd As IntPtr, cmdShow As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
  Private Declare Function DestroyWindow Lib "user32.dll" (hWnd As IntPtr) As Boolean
  Private Declare Function GetConsoleWindow Lib "kernel32.dll" () As IntPtr
  'Private Declare Function RegisterClassEx Lib "user32.dll" Alias "RegisterClassExA" (<[In]> ByRef wndClass As WNDCLASSEX) As UShort
  'Private Declare Function CreateWindowEx Lib "user32.dll" Alias "CreateWindowExA" (exStyle As Integer,
  '                                                                                  atom As UShort, 'string lpClassName,
  '                                                                                  windowName As String,
  '                                                                                  style As UInteger,
  '                                                                                  x As Integer,
  '                                                                                  y As Integer,
  '                                                                                  width As Integer,
  '                                                                                  height As Integer,
  '                                                                                  hWndParent As IntPtr,
  '                                                                                  hMenu As IntPtr,
  '                                                                                  hInstance As IntPtr,
  '                                                                                  lpParam As IntPtr) As IntPtr
  Private Declare Function CreateWindowEx Lib "user32.dll" Alias "CreateWindowExW" (exStyle As UInteger,
                                                                                    atom As UShort, 'className As String,
                                                                                    windowName As String,
                                                                                    style As UInteger,
                                                                                    x As Integer,
                                                                                    y As Integer,
                                                                                    width As Integer,
                                                                                    height As Integer,
                                                                                    wndParent As IntPtr,
                                                                                    menu As IntPtr,
                                                                                    hInstance As IntPtr,
                                                                                    lpParam As IntPtr) As IntPtr
  Private Declare Function GetLastError Lib "kernel32.dll" () As UInteger
  Private Declare Function DefWindowProc Lib "user32.dll" Alias "DefWindowProcA" (hWnd As IntPtr, msg As UInteger, wParam As IntPtr, lParam As IntPtr) As IntPtr
  Private Declare Sub PostQuitMessage Lib "user32.dll" (exitCode As Integer)
  Private Declare Function LoadCursor Lib "user32.dll" Alias "LoadCursorA" (hInstance As IntPtr, cursorName As Integer) As IntPtr
  Private Declare Function GetMessage Lib "user32.dll" Alias "GetMessageA" (ByRef lpMsg As MSG, hWnd As IntPtr, wMsgFilterMin As UInteger, wMsgFilterMax As UInteger) As Integer
  Private Declare Function TranslateMessage Lib "user32.dll" (ByRef lpMsg As MSG) As Boolean
  Private Declare Function DispatchMessage Lib "user32.dll" Alias "DispatchMessageA" (ByRef lpMsg As MSG) As Integer

  Private Declare Function LoadIcon Lib "user32.dll" Alias "LoadIconA" (hInstance As IntPtr, lpIconName As Integer) As IntPtr
  Private Declare Function RegisterClass Lib "user32.dll" Alias "RegisterClassA" (ByRef lpWndClass As WNDCLASS) As UShort
  Private Declare Function GetModuleHandle Lib "kernel32.dll" Alias "GetModuleHandleA" (lpModuleName As IntPtr) As IntPtr
  Private Declare Function MonitorFromWindow Lib "user32.dll" (hwnd As IntPtr, dwFlags As UInteger) As IntPtr
  Private Declare Function GetMonitorInfo Lib "user32.dll" Alias "GetMonitorInfoA" (hMonitor As IntPtr, ByRef lpmi As MONITORINFO) As Boolean
  Private Declare Function AdjustWindowRectEx Lib "user32.dll" (ByRef lpRect As RECT, dwStyle As UInteger, bMenu As Boolean, dwExStyle As UInteger) As Boolean
  Private Declare Function CreateWindow Lib "user32.dll" Alias "CreateWindowA" (lpClassName As String, lpWindowName As String, dwStyle As Integer, x As Integer, y As Integer, nWidth As Integer, nHeight As Integer, hWndParent As IntPtr, hMenu As IntPtr, hInstance As IntPtr, lpParam As IntPtr) As IntPtr
  Private Declare Function TrackMouseEvent Lib "user32.dll" (ByRef tme As TRACKMOUSEEVENTSTRUCT) As Boolean

  Private Declare Function FreeConsole Lib "kernel32.dll" () As Boolean
  Private Declare Function FindWindow Lib "user32.dll" (lpClassName As String, lpWindowName As String) As IntPtr

  'Private Declare Function FindWindow Lib "user32.dll" Alias "FindWindowA" (lpClassName As String, lpWindowName As String) As IntPtr
  'Private Declare Function ShowWindow Lib "user32.dll" (hWnd As IntPtr, nCmdShow As Integer) As Boolean
  'Private Const SW_HIDE As Integer = 0

  <StructLayout(LayoutKind.Sequential)>
  Private Structure PIXELFORMATDESCRIPTOR
    Public nSize As UShort
    Public nVersion As UShort
    Public dwFlags As UInteger
    Public iPixelType As Byte
    Public cColorBits As Byte
    Public cRedBits As Byte
    Public cRedShift As Byte
    Public cGreenBits As Byte
    Public cGreenShift As Byte
    Public cBlueBits As Byte
    Public cBlueShift As Byte
    Public cAlphaBits As Byte
    Public cAlphaShift As Byte
    Public cAccumBits As Byte
    Public cAccumRedBits As Byte
    Public cAccumGreenBits As Byte
    Public cAccumBlueBits As Byte
    Public cAccumAlphaBits As Byte
    Public cDepthBits As Byte
    Public cStencilBits As Byte
    Public cAuxBuffers As Byte
    Public iLayerType As Byte
    Public bReserved As Byte
    Public dwLayerMask As UInteger
    Public dwVisibleMask As UInteger
    Public dwDamageMask As UInteger
  End Structure

  Private Const PFD_DRAW_TO_WINDOW As Integer = &H4
  Private Const PFD_SUPPORT_OPENGL As Integer = &H20
  Private Const PFD_DOUBLEBUFFER As Integer = &H1
  Private Const PFD_TYPE_RGBA As Byte = 0
  Private Const PFD_MAIN_PLANE As Byte = 0

  Private Const GL_TEXTURE_2D As UInteger = &HDE1
  Private Const GL_TEXTURE_MAG_FILTER As UInteger = &H2800
  Private Const GL_TEXTURE_MIN_FILTER As UInteger = &H2801
  Private Const GL_NEAREST As UInteger = &H2600
  Private Const GL_TEXTURE_ENV As UInteger = &H2300
  Private Const GL_TEXTURE_ENV_MODE As UInteger = &H2200
  Private Const GL_DECAL As UInteger = &H2101
  Private Const GL_RGBA As UInteger = &H1908
  Private Const GL_UNSIGNED_BYTE As UInteger = &H1401
  Private Const GL_INT As UInteger = &H1404
  Private Const GL_QUADS As UInteger = &H7
  Private Const GL_COLOR_BUFFER_BIT As UInteger = &H4000

  Private Declare Function GetDC Lib "user32" (hWnd As IntPtr) As IntPtr
  Private Declare Function ChoosePixelFormat Lib "gdi32" (hdc As IntPtr, ByRef pfd As PIXELFORMATDESCRIPTOR) As Integer
  Private Declare Function SetPixelFormat Lib "gdi32" (hdc As IntPtr, iPixelFormat As Integer, ByRef pfd As PIXELFORMATDESCRIPTOR) As Integer
  Private Declare Function wglCreateContext Lib "opengl32" (hdc As IntPtr) As IntPtr
  Private Declare Function wglMakeCurrent Lib "opengl32" (hdc As IntPtr, hglrc As IntPtr) As Integer
  Private Declare Sub glViewport Lib "opengl32" (x As Integer, y As Integer, width As Integer, height As Integer)
  Private Declare Function wglGetProcAddress Lib "opengl32" (lpProcName As String) As IntPtr
  Private Declare Sub glEnable Lib "opengl32.dll" (cap As UInteger)
  Private Declare Sub glGenTextures Lib "opengl32.dll" (n As Integer, ByRef textures As UInteger)
  Private Declare Sub glBindTexture Lib "opengl32.dll" (target As UInteger, texture As UInteger)
  Private Declare Sub glTexParameteri Lib "opengl32.dll" (target As UInteger, pname As UInteger, param As Integer)
  Private Declare Sub glTexEnvf Lib "opengl32.dll" (target As UInteger, pname As UInteger, param As Single)
  Private Declare Sub glTexImage2D Lib "opengl32.dll" (target As UInteger, level As Integer, internalformat As Integer, width As Integer, height As Integer, border As Integer, format As UInteger, type As UInteger, data As IntPtr)
  Private Declare Sub glTexSubImage2D Lib "opengl32.dll" (target As UInteger, level As Integer, xoffset As Integer, yoffset As Integer, width As Integer, height As Integer, format As UInteger, type As UInteger, pixels As IntPtr)
  Private Declare Sub glBegin Lib "opengl32.dll" (mode As UInteger)
  Private Declare Sub glTexCoord2f Lib "opengl32.dll" (s As Single, t As Single)
  Private Declare Sub glEnd Lib "opengl32.dll" ()
  'Private Declare Function SwapBuffers Lib "user32.dll" (hdc As IntPtr) As Boolean
  'Declare Function SwapBuffers Lib "opengl32.dll" (hdc As IntPtr) As Boolean
  Declare Function SwapBuffers Lib "gdi32.dll" (hdc As IntPtr) As Boolean
  Private Declare Sub glVertex3f Lib "opengl32.dll" (x As Single, y As Single, z As Single)
  Private Declare Function SetWindowText Lib "user32.dll" Alias "SetWindowTextA" (hwnd As IntPtr, lpString As String) As Boolean
  Private Declare Function wglDeleteContext Lib "opengl32.dll" (hglrc As IntPtr) As Boolean
  Private Declare Function PostMessage Lib "user32.dll" Alias "PostMessageA" (hwnd As IntPtr, wMsg As UInteger, wParam As IntPtr, lParam As IntPtr) As Boolean
  Private Declare Sub glClear Lib "opengl32.dll" (mask As UInteger)

  Private Delegate Function wglSwapInterval_t(interval As Integer) As Integer
  Private wglSwapInterval As wglSwapInterval_t

#End Region

#Region "Linux"


  Private Delegate Function glSwapInterval_t(display As IntPtr, drawable As IntPtr, interval As Integer) As Integer
  Private glSwapIntervalEXT As glSwapInterval_t

  Private Const XK_F1 As Integer = &HFFBE
  Private Const XK_F2 As Integer = &HFFBF
  Private Const XK_F3 As Integer = &HFFC0
  Private Const XK_F4 As Integer = &HFFC1
  Private Const XK_F5 As Integer = &HFFC2
  Private Const XK_F6 As Integer = &HFFC3
  Private Const XK_F7 As Integer = &HFFC4
  Private Const XK_F8 As Integer = &HFFC5
  Private Const XK_F9 As Integer = &HFFC6
  Private Const XK_F10 As Integer = &HFFC7
  Private Const XK_F11 As Integer = &HFFC8
  Private Const XK_F12 As Integer = &HFFC9

  Private Const XK_Down As Integer = &HFF54
  Private Const XK_Left As Integer = &HFF51
  Private Const XK_Right As Integer = &HFF53
  Private Const XK_Up As Integer = &HFF52
  Private Const XK_KP_Enter As Integer = &HFF8D
  Private Const XK_Return As Integer = &HFF0D

  Private Const XK_BackSpace As Integer = &HFF08
  Private Const XK_Escape As Integer = &HFF1B
  Private Const XK_Linefeed As Integer = &HFF0A
  Private Const XK_Pause As Integer = &HFF13
  Private Const XK_Scroll_Lock As Integer = &HFF14
  Private Const XK_Tab As Integer = &HFF09
  Private Const XK_Delete As Integer = &HFF9F
  Private Const XK_Home As Integer = &HFF50
  Private Const XK_End As Integer = &HFF57
  Private Const XK_Page_Up As Integer = &HFF55
  Private Const XK_Page_Down As Integer = &HFF56
  Private Const XK_Insert As Integer = &HFF63
  Private Const XK_Shift_L As Integer = &HFFE1
  Private Const XK_Shift_R As Integer = &HFFE2
  Private Const XK_Control_L As Integer = &HFFE3
  Private Const XK_Control_R As Integer = &HFFE4
  Private Const XK_space As Integer = &H20

  Private Const XK_0 As Integer = &H30
  Private Const XK_1 As Integer = &H31
  Private Const XK_2 As Integer = &H32
  Private Const XK_3 As Integer = &H33
  Private Const XK_4 As Integer = &H34
  Private Const XK_5 As Integer = &H35
  Private Const XK_6 As Integer = &H36
  Private Const XK_7 As Integer = &H37
  Private Const XK_8 As Integer = &H38
  Private Const XK_9 As Integer = &H39

  Private Const XK_KP_0 As Integer = &HFFB0
  Private Const XK_KP_1 As Integer = &HFFB1
  Private Const XK_KP_2 As Integer = &HFFB2
  Private Const XK_KP_3 As Integer = &HFFB3
  Private Const XK_KP_4 As Integer = &HFFB4
  Private Const XK_KP_5 As Integer = &HFFB5
  Private Const XK_KP_6 As Integer = &HFFB6
  Private Const XK_KP_7 As Integer = &HFFB7
  Private Const XK_KP_8 As Integer = &HFFB8
  Private Const XK_KP_9 As Integer = &HFFB9

  Private Const XK_KP_Multiply As Integer = &H1008FFAA
  Private Const XK_KP_Add As Integer = &H1008FFAB
  Private Const XK_KP_Divide As Integer = &H1008FFAF
  Private Const XK_KP_Subtract As Integer = &H1008FFAD
  Private Const XK_KP_Decimal As Integer = &H1008FFAE

  Private Const GLX_RGBA As Integer = 4
  Private Const GLX_DEPTH_SIZE As Integer = 12
  Private Const GLX_DOUBLEBUFFER As Integer = 5

  Private Const AllocNone As Integer = 0
  Private Const None As Integer = 0
  Private Const ExposureMask As Integer = &H8000
  Private Const KeyPressMask As Integer = &H1
  Private Const KeyReleaseMask As Integer = &H2
  Private Const ButtonPressMask As Integer = &H4
  Private Const ButtonReleaseMask As Integer = &H8
  Private Const PointerMotionMask As Integer = &H200
  Private Const FocusChangeMask As Integer = &H20000
  Private Const StructureNotifyMask As Integer = &H20000
  Private Const InputOutput As Integer = 1

  Private Const WEColormap As UInteger = &H2

  Private Const CWEventMask As UInteger = &H80
  Private Const CWColormap As UInteger = &H4

  Private Const XInternalAtom As UInteger = &H400000
  Private Const XMapWindowConst As Integer = 18

  Private Const ClientMessage As Integer = 3

  Private Const GL_TRUE As Integer = 1

  Private olc_Display As IntPtr
  Private olc_WindowRoot As IntPtr
  Private olc_Window As IntPtr
  Private olc_VisualInfo As VisualInfo
  Private olc_ColourMap As IntPtr
  Private olc_SetWindowAttribs As XSetWindowAttributesStruct

  <StructLayout(LayoutKind.Sequential)>
  Public Structure VisualInfo
    Public visual As IntPtr
    Public visualid As Integer
    Public screen As Integer
    Public depth As Integer
    Public klass As Integer
    Public red_mask As ULong
    Public green_mask As ULong
    Public blue_mask As ULong
    Public colormap_size As Integer
    Public bits_per_rgb As Integer
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Private Structure XEvent
    Public type As UInteger
    Public xclient As XClientMessageEvent
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Private Structure XClientMessageEvent
    Public type As UInteger
    Public serial As UInteger
    Public send_event As Boolean
    Public display As IntPtr
    Public window As IntPtr
    Public message_type As IntPtr
    Public format As Integer
    Public data As XClientMessageEventDataUnion
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Private Structure XClientMessageEventDataUnion
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=20)> Public l() As Byte
  End Structure

  <StructLayout(LayoutKind.Explicit)>
  Public Structure ClientMessageDataUnion
    <FieldOffset(0)>
    Public b As Byte

    <FieldOffset(0)>
    Public s As Short

    <FieldOffset(0)>
    Public l As Integer

    <FieldOffset(0)>
    Public ll As Long
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Private Structure XWindowAttributes
    Dim x As Integer
    Dim y As Integer
    Dim width As Integer
    Dim height As Integer
    Dim border_width As Integer
    Dim depth As Integer
    Dim visual As IntPtr
    Dim root As IntPtr
    Dim [Class] As Integer
    Dim bit_gravity As Integer
    Dim win_gravity As Integer
    Dim backing_store As Integer
    Dim backing_planes As Long
    Dim backing_pixel As Long
    Dim save_under As Integer
    Dim colormap As IntPtr
    Dim map_installed As Integer
    Dim map_state As Integer
    Dim all_event_masks As Long
    Dim your_event_mask As Long
    Dim do_not_propagate_mask As Long
    Dim override_redirect As Integer
    Dim screen As IntPtr
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Public Structure XSetWindowAttributesStruct
    Public background_pixmap As IntPtr
    Public background_pixel As ULong
    Public border_pixmap As IntPtr
    Public border_pixel As ULong
    Public bit_gravity As Integer
    Public win_gravity As Integer
    Public backing_store As Integer
    Public backing_planes As ULong
    Public backing_pixel As ULong
    Public save_under As Boolean
    Public event_mask As ULong
    Public do_not_propagate_mask As ULong
    Public override_redirect As Boolean
    Public colormap As IntPtr
    Public cursor As IntPtr
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Structure GLXContext
    Public ptr As IntPtr
  End Structure

  <StructLayout(LayoutKind.Sequential)>
  Structure GLXFBConfig
    Public ptr As IntPtr
  End Structure

  Private Declare Function XInitThreads Lib "libX11.so" () As Integer
  Private Declare Function XOpenDisplay Lib "libX11.so" (display_name As String) As IntPtr
  Private Declare Function DefaultRootWindow Lib "libX11.so" (display As IntPtr) As IntPtr
  Private Declare Function glXChooseVisual Lib "libGL.so" (display As IntPtr, screen As Integer, attribList As Integer()) As VisualInfo
  Private Declare Sub glXSwapBuffers Lib "libGL.so" (display As IntPtr, drawable As IntPtr)
  Private Declare Function XCreateColormap Lib "libX11.so" (display As IntPtr, w As IntPtr, visual As IntPtr, alloc As Integer) As IntPtr
  Private Declare Function XCreateWindow Lib "libX11.so" (display As IntPtr, parent As IntPtr, x As Integer, y As Integer, width As Integer, height As Integer, border_width As Integer, depth As Integer, [class] As Integer, visual As IntPtr, valuemask As UInteger, ByRef attributes As XSetWindowAttributesStruct) As IntPtr
  Private Declare Function XSetWMProtocols Lib "libX11.so" (display As IntPtr, window As IntPtr, protocols() As IntPtr, count As Integer) As Integer
  Private Declare Function XStoreName Lib "libX11.so" (display As IntPtr, w As IntPtr, title As String) As Integer
  Private Declare Function XSendEvent Lib "libX11.so" (display As IntPtr, w As IntPtr, propagate As Boolean, event_mask As UInteger, ByRef event_send As XEvent) As Integer
  Private Declare Function SubstructureRedirectMask Lib "libX11.so" Alias "SubstructureRedirectMask" () As UInteger
  Private Declare Function SubstructureNotifyMask Lib "libX11.so" Alias "SubstructureNotifyMask" () As UInteger
  Private Declare Function XFlush Lib "libX11.so" (display As IntPtr) As Integer
  Private Declare Function XGetWindowAttributes Lib "libX11.so" (display As IntPtr, window As IntPtr, ByRef window_attributes As XWindowAttributes) As Integer
  Private Declare Function XSetWindowAttributes Lib "libX11.so" (display As IntPtr, w As IntPtr, ByRef attributes As XSetWindowAttributesStruct) As Integer
  Private Declare Function XInternalAtom1 Lib "libX11" (display As IntPtr, name As String, onlyIfExists As Boolean) As ULong
  Private Declare Function XMapWindow1 Lib "libX11" (display As IntPtr, w As IntPtr) As Integer
  Private Declare Function XSendEvent Lib "libX11" (display As IntPtr, w As IntPtr, propagate As Boolean, event_mask As Long, ByRef event_send As XClientMessageEvent) As Integer
  Private Declare Function XInternAtom Lib "libX11" (display As IntPtr, atom_name As String, only_if_exists As Boolean) As IntPtr
  Private Declare Auto Function XMapWindow Lib "libX11" (display As IntPtr, w As IntPtr) As Integer
  Private Declare Function glXCreateContext Lib "libGL.so.1" (display As IntPtr, visual As VisualInfo, share_context As IntPtr, direct As Boolean) As GLXContext
  Private Declare Function glXMakeCurrent Lib "libGL.so.1" (display As IntPtr, drawable As IntPtr, context As GLXContext) As Boolean
  Private Declare Function glXGetProcAddress Lib "libGL.so.1" (procname As String) As IntPtr
  Private Declare Function glXDestroyContext Lib "libGL.so.1" (display As IntPtr, ctx As IntPtr) As Integer
  Private Declare Function XDestroyWindow Lib "libX11.so.6" (display As IntPtr, w As IntPtr) As Integer
  Private Declare Function XCloseDisplay Lib "libX11.so.6" (display As IntPtr) As Integer
  'Private Delegate Sub glSwapInterval_t(ByVal display As IntPtr, ByVal drawable As IntPtr, ByVal interval As Integer)
  'Private Declare Sub glSwapIntervalEXT Lib "libGL.so.1" (ByVal display As IntPtr, ByVal drawable As IntPtr, ByVal interval As Integer)

#End Region

  Protected Delegate Function PixelModeDelegate(x As Integer, y As Integer, p1 As Pixel, p2 As Pixel) As Pixel
  Private funcPixelMode As PixelModeDelegate

  Protected Property AppName As String

  Private olc_hWnd As IntPtr
  Private glBuffer As UInteger

  Private nPixelMode As Pixel.Mode
  Private fBlendFactor As Single = 1.0F

  Private nScreenWidth As Integer
  Private nScreenHeight As Integer
  Private nPixelWidth As Integer
  Private nPixelHeight As Integer
  Private bFullScreen As Boolean
  Private bEnableVSYNC As Boolean
  Private fPixelX As Single
  Private fPixelY As Single
  Private pDefaultDrawTarget As Sprite
  Private pDrawTarget As Sprite

  Private Shared m_shutdown As Boolean
  Private nWindowWidth As Integer
  Private nWindowHeight As Integer
  Private bHasInputFocus As Boolean
  Private bHasMouseFocus As Boolean
  Private fFrameTimer As Single = 1.0F
  Private nFrameCount As Integer

  Private pKeyNewState(255) As Boolean
  Private pKeyOldState(255) As Boolean
  Private pKeyboardState(255) As HwButton

  Private pMouseNewState(4) As Boolean
  Private pMouseOldState(4) As Boolean
  Private pMouseState(4) As HwButton
  Private nMousePosXcache As Integer
  Private nMousePosYcache As Integer
  Private nMousePosX As Integer
  Private nMousePosY As Integer
  Private nMouseWheelDelta As Integer
  Private nMouseWheelDeltaCache As Integer
  Private nViewX As Integer
  Private nViewY As Integer
  Private nViewW As Integer
  Private nViewH As Integer

  Private glRenderContext As IntPtr
  Private glDeviceContext As IntPtr

  Private fSubPixelOffsetX As Single
  Private fSubPixelOffsetY As Single

  Protected Structure HwButton
    Public Pressed As Boolean ' Set once during the frame the event occurs
    Public Released As Boolean ' Set once during the frame the event occurs
    Public Held As Boolean ' Set true for all frames between pressed and released events
  End Structure

  Public Enum RCode
    OK
    FAIL
    NO_FILE
  End Enum

  Public Enum Key
    NONE
    A
    B
    C
    D
    E
    F
    G
    H
    I
    J
    K
    L
    M
    N
    O
    P
    Q
    R
    S
    T
    U
    V
    W
    X
    Y
    Z
    K0
    K1
    K2
    K3
    K4
    K5
    K6
    K7
    K8
    K9
    F1
    F2
    F3
    F4
    F5
    F6
    F7
    F8
    F9
    F10
    F11
    F12
    UP
    DOWN
    LEFT
    RIGHT
    SPACE
    TAB
    SHIFT
    CTRL
    INS
    DEL
    HOME
    [END]
    PGUP
    PGDN
    BACK
    ESCAPE
    [RETURN]
    ENTER
    PAUSE
    SCROLL
    NP0
    NP1
    NP2
    NP3
    NP4
    NP5
    NP6
    NP7
    NP8
    NP9
    NP_MUL
    NP_DIV
    NP_ADD
    NP_SUB
    NP_DECIMAL
    PERIOD
    EQUALS
    COMMA
    MINUS
    OEM_1
    OEM_2
    OEM_3
    OEM_4
    OEM_5
    OEM_6
    OEM_7
    OEM_8
    CAPS_LOCK
    ENUM_END
  End Enum

  Private m_fontSprite As Sprite

  Protected Friend Sub New()
    AppName = "Undefined"
    Singleton.Pge = Me
  End Sub

  Public Function Construct(screenW As Integer, screenH As Integer, pixelW As Integer, pixelH As Integer, Optional fullScreen As Boolean = False, Optional vsync As Boolean = False) As Boolean 'RCode

    nScreenWidth = screenW
    nScreenHeight = screenH
    nPixelWidth = pixelW
    nPixelHeight = pixelH
    bFullScreen = fullScreen
    bEnableVSYNC = vsync

    fPixelX = 2.0F / nScreenWidth
    fPixelY = 2.0F / nScreenHeight

    If nPixelWidth = 0 OrElse nPixelHeight = 0 OrElse nScreenWidth = 0 OrElse nScreenHeight = 0 Then
      Return False 'RCode.FAIL
    End If

    ' Load the default font sheet
    olc_ConstructFontSheet()

    ' Create a sprite that represents the primary drawing target
    pDefaultDrawTarget = New Sprite(nScreenWidth, nScreenHeight)
    SetDrawTarget(Nothing)

    Return True 'RCode.OK

  End Function

  Protected Sub SetScreenSize(w As Integer, h As Integer)

    pDefaultDrawTarget = Nothing '.Dispose()
    nScreenWidth = w
    nScreenHeight = h
    pDefaultDrawTarget = New Sprite(nScreenWidth, nScreenHeight)
    SetDrawTarget(Nothing)
    glClear(GL_COLOR_BUFFER_BIT)

    If IsOSPlatform(Windows) Then
      SwapBuffers(glDeviceContext)
    End If

    If IsOSPlatform(Linux) Then
      glXSwapBuffers(olc_Display, olc_Window)
    End If

    glClear(GL_COLOR_BUFFER_BIT)
    olc_UpdateViewport()

  End Sub

  Public Function Start() As RCode

    ' Construct the window
    If IsOSPlatform(Windows) Then
      If olc_WindowCreate_Windows() = IntPtr.Zero Then
        Return RCode.FAIL
      End If
    ElseIf IsOSPlatform(Linux) Then
      If olc_WindowCreate_Linux() = IntPtr.Zero Then
        Return RCode.FAIL
      End If
    Else
      Return RCode.FAIL
    End If

    If IsOSPlatform(Windows) Then

      'FreeConsole() ' doesn't always work??????
      Dim ptr = GetConsoleWindow
      ShowWindow(ptr, 0)

    End If

    ' Start the thread
    Singleton.AtomActive = True
    Dim t As New Thread(AddressOf EngineThread)
    t.Start()

    If IsOSPlatform(Windows) Then
      ' Handle Windows Message Loop
      Dim m = New MSG
      m.message = 0 ' Set the message parameter to zero to retrieve any message.
      m.hWnd = IntPtr.Zero ' Set the window handle parameter to zero to retrieve messages for any window.
      m.wParam = IntPtr.Zero ' Set the wParam parameter to zero.
      m.lParam = IntPtr.Zero ' Set the lParam parameter to zero.
      m.time = 0 ' Set the time parameter to zero.
      m.pt = New Point(0, 0) ' Set the cursor position to (0,0).
      While GetMessage(m, IntPtr.Zero, 0, 0) > 0
        TranslateMessage(m)
        DispatchMessage(m)
      End While
    End If

    ' Wait for thread to be exited

    Return RCode.OK

  End Function

  Public Sub SetDrawTarget(target As Sprite)
    If target IsNot Nothing Then
      pDrawTarget = target
    Else
      pDrawTarget = pDefaultDrawTarget
    End If
  End Sub

  Friend ReadOnly Property GetDrawTarget() As Sprite
    Get
      Return pDrawTarget
    End Get
  End Property

  Private Protected ReadOnly Property GetDrawTargetWidth() As Integer
    Get
      If pDrawTarget IsNot Nothing Then
        Return pDrawTarget.Width
      Else
        Return 0
      End If
    End Get
  End Property

  Private Protected ReadOnly Property GetDrawTargetHeight() As Integer
    Get
      If pDrawTarget IsNot Nothing Then
        Return pDrawTarget.Height
      Else
        Return 0
      End If
    End Get
  End Property

  Protected ReadOnly Property IsFocused() As Boolean
    Get
      Return bHasInputFocus
    End Get
  End Property

  Protected ReadOnly Property GetKey(k As Key) As HwButton
    Get
      Return pKeyboardState(k)
    End Get
  End Property

  Protected ReadOnly Property GetMouse(b As Integer) As HwButton
    Get
      Return pMouseState(b)
    End Get
  End Property

  Protected ReadOnly Property GetMouseX() As Integer
    Get
      Return nMousePosX
    End Get
  End Property

  Protected ReadOnly Property GetMouseY() As Integer
    Get
      Return nMousePosY
    End Get
  End Property

  Protected ReadOnly Property GetMouseWheel() As Integer
    Get
      Return nMouseWheelDelta
    End Get
  End Property

  Public ReadOnly Property ScreenWidth As Integer
    Get
      Return nScreenWidth
    End Get
  End Property

  Public ReadOnly Property ScreenHeight As Integer
    Get
      Return nScreenHeight
    End Get
  End Property

  Protected Function Draw(pos As Vi2d) As Boolean
    Return Draw(pos.x, pos.y, Presets.White)
  End Function

  Protected Function Draw(pos As Vi2d, p As Pixel) As Boolean
    Return Draw(pos.x, pos.y, p)
  End Function

  Protected Overridable Function Draw(x As Integer, y As Integer) As Boolean
    Return Draw(x, y, Presets.White)
  End Function

  Public Function Draw(x As Integer, y As Integer, p As Pixel) As Boolean

    If pDrawTarget Is Nothing Then
      Return False
    End If

    If nPixelMode = Pixel.Mode.Normal Then
      Return pDrawTarget.SetPixel(x, y, p)
    End If

    If nPixelMode = Pixel.Mode.Mask Then
      If p.A = 255 Then
        Return pDrawTarget.SetPixel(x, y, p)
      End If
    End If

    If nPixelMode = Pixel.Mode.Alpha Then
      Dim d = pDrawTarget.GetPixel(x, y)
      Dim a = (p.A / 255.0F) * fBlendFactor
      Dim c = 1.0F - a
      Dim r = a * p.R + c * d.R
      Dim g = a * p.G + c * d.G
      Dim b = a * p.B + c * d.B
      Return pDrawTarget.SetPixel(x, y, New Pixel(CByte(r), CByte(g), CByte(b)))
    End If

    If nPixelMode = Pixel.Mode.Custom Then
      Return pDrawTarget.SetPixel(x, y, funcPixelMode(x, y, p, pDrawTarget.GetPixel(x, y)))
    End If

    Return False

  End Function

  Protected Sub SetSubPixelOffset(ox As Single, oy As Single)
    fSubPixelOffsetX = ox * fPixelX
    fSubPixelOffsetY = oy * fPixelY
  End Sub

  Protected Sub DrawLine(pos1 As Vi2d, pos2 As Vi2d)
    DrawLine(pos1.x, pos1.y, pos2.x, pos2.y, Presets.White, &HFFFFFFFFUI)
  End Sub

  Protected Sub DrawLine(pos1 As Vi2d, pos2 As Vi2d, p As Pixel, Optional pattern As UInteger = &HFFFFFFFFUI)
    DrawLine(pos1.x, pos1.y, pos2.x, pos2.y, p, pattern)
  End Sub

  Protected Sub DrawLine(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer)
    DrawLine(x1, y1, x2, y2, Presets.White, &HFFFFFFFFUI)
  End Sub

  Protected Sub DrawLine(x1 As Single, y1 As Single, x2 As Single, y2 As Single, p As Pixel)
    DrawLine(CInt(Fix(x1)), CInt(Fix(y1)), CInt(Fix(x2)), CInt(Fix(y2)), p, &HFFFFFFFFUI)
  End Sub

  Public Sub DrawLine(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer, p As Pixel, Optional pattern As UInteger = &HFFFFFFFFUI)

    Dim dx = x2 - x1
    Dim dy = y2 - y1

    Dim rol = New Func(Of Boolean)(Function()
                                     pattern = (pattern << 1) Or (pattern >> 31)
                                     Return CInt(pattern And 1) <> 0
                                   End Function)

    ' straight line idea by gurkanctn
    If dx = 0 Then ' Line is vertical
      If y2 < y1 Then Swap(y1, y2)
      For y = y1 To y2
        If rol() Then Draw(x1, y, p)
      Next
      Return
    End If

    If dy = 0 Then ' Line is horizontal
      If x2 < x1 Then Swap(x1, x2)
      For x = x1 To x2
        If rol() Then Draw(x, y1, p)
      Next
      Return
    End If

    ' Line is Funk-aye
    Dim dx1 = MathF.Abs(dx) : Dim dy1 = MathF.Abs(dy)
    Dim px = 2 * dy1 - dx1 : Dim py = 2 * dx1 - dy1

    If dy1 <= dx1 Then

      Dim x, y As Integer
      Dim xe As Integer

      If dx >= 0 Then
        x = x1 : y = y1 : xe = x2
      Else
        x = x2 : y = y2 : xe = x1
      End If
      If rol() Then Draw(x, y, p)

      For i = 0 To xe - x
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
        If rol() Then Draw(x, y, p)
      Next

    Else

      Dim x, y As Integer
      Dim ye As Integer

      If dy >= 0 Then
        x = x1 : y = y1 : ye = y2
      Else
        x = x2 : y = y2 : ye = y1
      End If
      If rol() Then Draw(x, y, p)

      For i = 0 To ye - y
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
        If rol() Then Draw(x, y, p)
      Next

    End If

  End Sub

  Protected Sub DrawCircle(pos As Vi2d, radius As Integer)
    DrawCircle(pos.x, pos.y, radius, Presets.White, &HFF)
  End Sub

  Protected Sub DrawCircle(pos As Vi2d, radius As Integer, p As Pixel, Optional mask As Byte = &HFF)
    DrawCircle(pos.x, pos.y, radius, p, mask)
  End Sub

  Protected Sub DrawCircle(x As Integer, y As Integer, radius As Integer)
    DrawCircle(x, y, radius, Presets.White, &HFF)
  End Sub

  Public Sub DrawCircle(x As Integer, y As Integer, radius As Integer, p As Pixel, Optional mask As Byte = &HFF)

    Dim x0 = 0
    Dim y0 = radius
    Dim d = 3 - 2 * radius
    If radius = 0 Then Return

    While y0 >= x0 ' only formulate 1/8 of circle
      If (mask And &H1) <> 0 Then Draw(x + x0, y - y0, p)
      If (mask And &H2) <> 0 Then Draw(x + y0, y - x0, p)
      If (mask And &H4) <> 0 Then Draw(x + y0, y + x0, p)
      If (mask And &H8) <> 0 Then Draw(x + x0, y + y0, p)
      If (mask And &H10) <> 0 Then Draw(x - x0, y + y0, p)
      If (mask And &H20) <> 0 Then Draw(x - y0, y + x0, p)
      If (mask And &H40) <> 0 Then Draw(x - y0, y - x0, p)
      If (mask And &H80) <> 0 Then Draw(x - x0, y - y0, p)
      If d < 0 Then
        d += 4 * x0 + 6 : x0 += 1
      Else
        d += 4 * (x0 - y0) + 10 : x0 += 1 : y0 -= 1
      End If
    End While

  End Sub

  Protected Sub FillCircle(pos As Vi2d, radius As Integer)
    FillCircle(pos.x, pos.y, radius, Presets.White)
  End Sub

  Public Sub FillCircle(x As Integer, y As Integer, radius As Integer, p As Pixel)

    Dim x0 = 0
    Dim y0 = radius
    Dim d = 3 - 2 * radius
    If radius = 0 Then Return

    Dim drawLine = Sub(sx As Integer, ex As Integer, ny As Integer)
                     For i = sx To ex
                       Draw(i, ny, p)
                     Next
                   End Sub

    While y0 >= x0
      drawLine(x - x0, x + x0, y - y0)
      drawLine(x - y0, x + y0, y - x0)
      drawLine(x - x0, x + x0, y + y0)
      drawLine(x - y0, x + y0, y + x0)
      If d < 0 Then
        d += 4 * x0 + 6 : x0 += 1
      Else
        d += 4 * (x0 - y0) + 10 : x0 += 1 : y0 -= 1
      End If
    End While

  End Sub

  Protected Sub DrawRect(pos As Vi2d, size As Vi2d)
    DrawRect(pos.x, pos.y, size.x, size.y, Presets.White)
  End Sub

  Protected Sub DrawRect(pos As Vi2d, size As Vi2d, p As Pixel)
    DrawRect(pos.x, pos.y, size.x, size.y, p)
  End Sub

  Protected Sub DrawRect(x As Integer, y As Integer, w As Integer, h As Integer)
    DrawRect(x, y, w, h, Presets.White)
  End Sub

  Public Sub DrawRect(x As Integer, y As Integer, w As Integer, h As Integer, p As Pixel)
    DrawLine(x, y, x + w, y, p)
    DrawLine(x + w, y, x + w, y + h, p)
    DrawLine(x + w, y + h, x, y + h, p)
    DrawLine(x, y + h, x, y, p)
  End Sub

  Protected Sub Clear()
    Clear(Presets.Black)
  End Sub

  Protected Sub Clear(p As Pixel)
    Dim pixels = GetDrawTargetWidth() * GetDrawTargetHeight()
    Dim m() = GetDrawTarget().GetData()
    For i = 0 To pixels - 1
      m(i) = p
    Next
#If OLC_DBG_OVERDRAW Then
    Sprite.nOverdrawCount += pixels
#End If
  End Sub

  Protected Sub FillRect(pos As Vi2d, size As Vi2d)
    FillRect(pos.x, pos.y, size.x, size.y, Presets.White)
  End Sub

  Protected Sub FillRect(pos As Vi2d, size As Vi2d, p As Pixel)
    FillRect(pos.x, pos.y, size.x, size.y, p)
  End Sub

  Protected Sub FillRect(x As Integer, y As Integer, w As Integer, h As Integer)
    FillRect(x, y, w, h, Presets.White)
  End Sub

  Protected Sub FillRect(x As Integer, y As Integer, w As Integer, h As Integer, p As Pixel)

    Dim x2 = x + w
    Dim y2 = y + h

    If x < 0 Then x = 0
    If x >= GetDrawTargetWidth() Then x = GetDrawTargetWidth()
    If y < 0 Then y = 0
    If y >= GetDrawTargetHeight() Then y = GetDrawTargetHeight()

    If x2 < 0 Then x2 = 0
    If x2 >= GetDrawTargetWidth() Then x2 = GetDrawTargetWidth()
    If y2 < 0 Then y2 = 0
    If y2 >= GetDrawTargetHeight() Then y2 = GetDrawTargetHeight()

    For i = x To x2 - 1
      For j = y To y2 - 1
        Draw(i, j, p)
      Next
    Next

  End Sub

  Protected Sub DrawTriangle(pos1 As Vi2d, pos2 As Vi2d, pos3 As Vi2d)
    DrawTriangle(pos1.x, pos1.y, pos2.x, pos2.y, pos3.x, pos3.y, Presets.White)
  End Sub

  Protected Sub DrawTriangle(pos1 As Vi2d, pos2 As Vi2d, pos3 As Vi2d, p As Pixel)
    DrawTriangle(pos1.x, pos1.y, pos2.x, pos2.y, pos3.x, pos3.y, p)
  End Sub

  Protected Sub DrawTriangle(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer, x3 As Integer, y3 As Integer)
    DrawTriangle(x1, y1, x2, y2, x3, y3, Presets.White)
  End Sub

  Public Sub DrawTriangle(x1 As Single, y1 As Single, x2 As Single, y2 As Single, x3 As Single, y3 As Single, p As Pixel)
    DrawTriangle(CInt(Fix(x1)), CInt(Fix(y1)), CInt(Fix(x2)), CInt(Fix(y2)), CInt(Fix(x3)), CInt(Fix(y3)), p)
  End Sub

  Public Sub DrawTriangle(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer, x3 As Integer, y3 As Integer, p As Pixel)
    DrawLine(x1, y1, x2, y2, p)
    DrawLine(x2, y2, x3, y3, p)
    DrawLine(x3, y3, x1, y1, p)
  End Sub

  Protected Sub FillTriangle(pos1 As Vi2d, pos2 As Vi2d, pos3 As Vi2d)
    FillTriangle(pos1.x, pos1.y, pos2.x, pos2.y, pos3.x, pos3.y, Presets.White)
  End Sub

  Protected Sub FillTriangle(pos1 As Vi2d, pos2 As Vi2d, pos3 As Vi2d, p As Pixel)
    FillTriangle(pos1.x, pos1.y, pos2.x, pos2.y, pos3.x, pos3.y, p)
  End Sub

  Protected Sub FillTriangle(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer, x3 As Integer, y3 As Integer)
    FillTriangle(x1, y1, x2, y2, x3, y3, Presets.White)
  End Sub

  Public Sub FillTriangle(x1 As Single, y1 As Single, x2 As Single, y2 As Single, x3 As Single, y3 As Single, p As Pixel)
    FillTriangle(CInt(Fix(x1)), CInt(Fix(y1)), CInt(Fix(x2)), CInt(Fix(y2)), CInt(Fix(x3)), CInt(Fix(y3)), p)
  End Sub

  Public Sub FillTriangle(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer, x3 As Integer, y3 As Integer, p As Pixel)

    Dim drawline = Sub(sx As Integer, ex As Integer, ny As Integer)
                     For i = sx To ex
                       Draw(i, ny, p)
                     Next
                   End Sub

    Dim t1x, t2x, y, minx, maxx, t1xp, t2xp As Integer
    Dim changed1, changed2 As Boolean
    Dim signx1, signx2, dx1, dy1, dx2, dy2 As Integer
    Dim e1, e2 As Integer
    ' Sort vertices
    If y1 > y2 Then Swap(y1, y2) : Swap(x1, x2)
    If y1 > y3 Then Swap(y1, y3) : Swap(x1, x3)
    If y2 > y3 Then Swap(y2, y3) : Swap(x2, x3)

    t1x = x1 : t2x = x1 : y = y1 ' Starting points
    dx1 = x2 - x1
    If dx1 < 0 Then
      dx1 = -dx1
      signx1 = -1
    Else
      signx1 = 1
    End If
    dy1 = y2 - y1

    dx2 = x3 - x1
    If dx2 < 0 Then
      dx2 = -dx2
      signx2 = -1
    Else
      signx2 = 1
    End If
    dy2 = y3 - y1

    If dy1 > dx1 Then ' swap values
      Swap(dx1, dy1) : changed1 = True
    End If
    If dy2 > dx2 Then ' swap values
      Swap(dy2, dx2) : changed2 = True
    End If

    e2 = dx2 >> 1
    ' Flat top, just process the second half
    If y1 = y2 Then GoTo nextx
    e1 = dx1 >> 1

    For i = 0 To dx1 - 1
      t1xp = 0 : t2xp = 0

      If t1x < t2x Then
        minx = t1x : maxx = t2x
      Else
        minx = t2x : maxx = t1x
      End If

      ' process first line until y value is about to change
      While i < dx1
        'i += 1
        e1 += dy1
        While e1 >= dx1
          e1 -= dx1
          If changed1 Then
            t1xp = signx1
          Else
            GoTo next1
          End If
        End While
        If changed1 Then Exit While
        t1x += signx1
      End While
      ' Move line
next1:
      ' Process second line until y value is about to change
      While True
        e2 += dy2
        While e2 >= dx2
          e2 -= dx2
          If changed2 Then
            t2xp = signx2 ' t2x += signx2
          Else
            GoTo next2
          End If
        End While
        If changed2 Then Exit While
        t2x += signx2
      End While
next2:
      If minx > t1x Then minx = t1x
      If minx > t2x Then minx = t2x
      If maxx < t1x Then maxx = t1x
      If maxx < t2x Then maxx = t2x
      drawline(minx, maxx, y)    ' Draw line from min to max points found on the y
      ' Now increase y
      If Not changed1 Then t1x += signx1
      t1x += t1xp
      If Not changed2 Then t2x += signx2
      t2x += t2xp
      y += 1
      If y = y2 Then Exit For
    Next
nextx:
    ' Second half
    dx1 = x3 - x2
    If dx1 < 0 Then
      dx1 = -dx1 : signx1 = -1
    Else
      signx1 = 1
    End If
    dy1 = y3 - y2 : t1x = x2

    If dy1 > dx1 Then ' swap values
      Swap(dy1, dx1) : changed1 = True
    Else
      changed1 = False
    End If

    e1 = dx1 >> 1

    For i = 0 To dx1
      t1xp = 0
      t2xp = 0
      If t1x < t2x Then
        minx = t1x : maxx = t2x
      Else
        minx = t2x : maxx = t1x
      End If
      ' process first line until y value is about to change
      While i < dx1
        e1 += dy1
        While e1 >= dx1
          e1 -= dx1
          If changed1 Then t1xp = signx1 : Exit While ' t1x += signx1
          GoTo next3
        End While
        If changed1 Then Exit While
        t1x += signx1
        If i < dx1 Then i += 1
      End While
next3:
      ' process second line until y value is about to change
      While t2x <> x3
        e2 += dy2
        While e2 >= dx2
          e2 -= dx2
          If changed2 Then
            t2xp = signx2
          Else
            GoTo next4
          End If
        End While
        If changed2 Then Exit While
        t2x += signx2
      End While
next4:

      If minx > t1x Then minx = t1x
      If minx > t2x Then minx = t2x
      If maxx < t1x Then maxx = t1x
      If maxx < t2x Then maxx = t2x
      drawline(minx, maxx, y)
      If Not changed1 Then t1x += signx1
      t1x += t1xp
      If Not changed2 Then t2x += signx2
      t2x += t2xp
      y += 1
      If y > y3 Then Return

    Next

  End Sub

  Protected Sub DrawSprite(pos As Vi2d, sprite As Sprite, Optional scale As Integer = 1)
    DrawSprite(pos.x, pos.y, sprite, scale)
  End Sub

  Protected Sub DrawSprite(x As Integer, y As Integer, sprite As Sprite, Optional scale As Integer = 1)
    If sprite Is Nothing Then
      Return
    End If

    If scale > 1 Then
      For i = 0 To sprite.Width - 1
        For j = 0 To sprite.Height - 1
          For iIs = 0 To scale - 1
            For js = 0 To scale - 1
              Draw(x + (i * scale) + iIs, y + (j * scale) + js, sprite.GetPixel(i, j))
            Next
          Next
        Next
      Next
    Else
      For i = 0 To sprite.Width - 1
        For j = 0 To sprite.Height - 1
          Draw(x + i, y + j, sprite.GetPixel(i, j))
        Next
      Next
    End If
  End Sub

  Protected Sub DrawPartialSprite(pos As Vi2d, sprite As Sprite, sourcepos As Vi2d, size As Vi2d, Optional scale As Integer = 1)
    DrawPartialSprite(pos.x, pos.y, sprite, sourcepos.x, sourcepos.y, size.x, size.y, scale)
  End Sub

  Protected Sub DrawPartialSprite(x As Integer, y As Integer, sprite As Sprite, ox As Integer, oy As Integer, w As Integer, h As Integer, Optional scale As Integer = 1)

    If sprite Is Nothing Then
      Return
    End If

    If scale > 1 Then
      For i = 0 To w - 1
        For j = 0 To h - 1
          For iIs = 0 To scale - 1
            For js = 0 To scale - 1
              Draw(x + (i * scale) + iIs, y + (j * scale) + js, sprite.GetPixel(i + ox, j + oy))
            Next
          Next
        Next
      Next
    Else
      For i = 0 To w - 1
        For j = 0 To h - 1
          Draw(x + i, y + j, sprite.GetPixel(i + ox, j + oy))
        Next
      Next
    End If

  End Sub

  Protected Sub DrawString(pos As Vi2d, sText As String)
    DrawString(pos.x, pos.y, sText, Presets.White, 1)
  End Sub

  Protected Sub DrawString(pos As Vi2d, sText As String, col As Pixel, Optional scale As Integer = 1)
    DrawString(pos.x, pos.y, sText, col, scale)
  End Sub

  Protected Sub DrawString(x As Integer, y As Integer, sText As String)
    DrawString(x, y, sText, Presets.White, 1)
  End Sub

  Protected Sub DrawString(x As Integer, y As Integer, sText As String, col As Pixel, Optional scale As Integer = 1)

    Dim sx = 0
    Dim sy = 0
    Dim m = nPixelMode

    If col.A <> 255 Then
      SetPixelMode(Pixel.Mode.Alpha)
    Else
      SetPixelMode(Pixel.Mode.Mask)
    End If

    For Each c In sText
      If c = vbLf Then
        sx = 0
        sy += 8 * scale
      Else
        Dim ox = (Asc(c) - 32) Mod 16
        Dim oy = (Asc(c) - 32) \ 16
        If scale > 1 Then
          For i = 0 To 7
            For j = 0 To 7
              If m_fontSprite.GetPixel(i + ox * 8, j + oy * 8).R > 0 Then
                For iIs = 0 To scale - 1
                  For js = 0 To scale - 1
                    Draw(x + sx + (i * scale) + iIs, y + sy + (j * scale) + js, col)
                  Next
                Next
              End If
            Next
          Next
        Else
          For i = 0 To 7
            For j = 0 To 7
              If m_fontSprite.GetPixel(i + ox * 8, j + oy * 8).R > 0 Then
                Draw(x + sx + i, y + sy + j, col)
              End If
            Next
          Next
        End If
        sx += 8 * scale
      End If

    Next

    SetPixelMode(m)

  End Sub

  Protected Sub SetPixelMode(m As Pixel.Mode)
    nPixelMode = m
  End Sub

  Protected ReadOnly Property GetPixelMode() As Pixel.Mode
    Get
      Return nPixelMode
    End Get
  End Property

  'Private Protected Sub SetPixelMode(pixelMode As Func(Of Integer, Integer, Pixel, Pixel, Pixel), m As Pixel.Mode)
  Protected Sub SetPixelMode(pixelMode As PixelModeDelegate, m As Pixel.Mode)
    funcPixelMode = pixelMode
    nPixelMode = Pixel.Mode.Custom
  End Sub

  Protected Sub SetPixelBlend(blend As Single)
    fBlendFactor = blend
    If fBlendFactor < 0.0F Then fBlendFactor = 0.0F
    If fBlendFactor > 1.0F Then fBlendFactor = 1.0F
  End Sub

  Protected Overridable Function OnUserCreate() As Boolean
    Return True
  End Function

  Protected MustOverride Function OnUserUpdate(elapsedTime As Single) As Boolean

  Private Protected Overridable Function OnUserDestroy() As Boolean
    Return True
  End Function

  Private Sub olc_UpdateViewport()

    Dim ww = nScreenWidth * nPixelWidth
    Dim wh = nScreenHeight * nPixelHeight
    Dim wasp = CSng(ww / wh)

    nViewW = nWindowWidth
    nViewH = CInt(Fix(nViewW / wasp))

    If nViewH > nWindowHeight Then
      nViewH = nWindowHeight
      nViewW = CInt(Fix(nViewH * wasp))
    End If

    nViewX = (nWindowWidth - nViewW) \ 2
    nViewY = (nWindowHeight - nViewH) \ 2

  End Sub

  Private Sub olc_UpdateWindowSize(x As Integer, y As Integer)
    nWindowWidth = x
    nWindowHeight = y
    olc_UpdateViewport()
  End Sub

  Private Sub olc_UpdateMouseWheel(delta As Integer)
    nMouseWheelDeltaCache += delta
  End Sub

  Private Sub olc_UpdateMouse(x As Integer, y As Integer)

    ' Mouse coords come in screen space
    ' But leave in pixel space

    ' Full Screen mode may have a weird viewport we must clamp to
    x -= nViewX
    y -= nViewY

    nMousePosXcache = CInt(Fix((x / (nWindowWidth - (nViewX * 2))) * nScreenWidth))
    nMousePosYcache = CInt(Fix((y / (nWindowHeight - (nViewY * 2))) * nScreenHeight))

    If nMousePosXcache >= nScreenWidth Then nMousePosXcache = nScreenWidth - 1
    If nMousePosYcache >= nScreenHeight Then nMousePosYcache = nScreenHeight - 1

    If nMousePosXcache < 0 Then nMousePosXcache = 0
    If nMousePosYcache < 0 Then nMousePosYcache = 0

  End Sub

  Private Sub EngineThread()

    ' Start OpenGL, the context is owned by the game thread
    If IsOSPlatform(Windows) Then
      olc_OpenGLCreate_Windows()
    ElseIf IsOSPlatform(Linux) Then
      olc_OpenGLCreate_Linux()
    End If

    ' Create Screen Texture - disable filtering
    glEnable(GL_TEXTURE_2D)
    glGenTextures(1, glBuffer)
    glBindTexture(GL_TEXTURE_2D, glBuffer)
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST)
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST)
    glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_DECAL)

    If True Then
      ' NOTE: the below doesn't seem to be working as SizeOf is reporting 8 instead of 4 as I would expect given the FieldOffset overlap...
      'Dim data = pDefaultDrawTarget.GetData
      'Dim sz = Marshal.SizeOf(GetType(Pixel))
      'Dim ptr = Marshal.AllocHGlobal(sz * data.Length)
      'For i = 0 To data.Length - 1
      '  Marshal.StructureToPtr(data(i), ptr + i * sz, False)
      'Next
      Dim data = pDefaultDrawTarget.GetData ' NOTE: DO NOT INLINE... perf hit big time!
      Dim b((data.Length * 4) - 1) As Byte
      For index = 0 To data.Length - 1
        b(index * 4) = data(index).R
        b((index * 4) + 1) = data(index).G
        b((index * 4) + 2) = data(index).B
        b((index * 4) + 3) = data(index).A
      Next
      Dim ptr = Marshal.AllocHGlobal(b.Length)
      Marshal.Copy(b, 0, ptr, b.Length)
      glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, nScreenWidth, nScreenHeight, 0, GL_RGBA, GL_UNSIGNED_BYTE, ptr)
      Marshal.FreeHGlobal(ptr)
    End If

    ' Create user resources as part of this thread
    If Not OnUserCreate() Then
      Singleton.AtomActive = False
    End If

    Dim tp1 = Now()
    'Dim tp2 = Now()

    While Singleton.AtomActive

      ' Run as fast as possible
      While Singleton.AtomActive

        ' Handle Timing
        Dim tp2 = Now
        Dim difference = tp2 - tp1
        tp1 = tp2

        Dim elapsedTime = CSng(difference.TotalSeconds)

        If IsOSPlatform(Linux) Then

          'TODO:

          '' Handle Xlib Message Loop - we do this in the
          '' same thread that OpenGL was created so we don't
          '' need to worry too much about multithreading with X11
          'Dim xev As XEvent
          'While XPending(olc_Display) > 0
          '  XNextEvent(olc_Display, xev)

          '  If xev.type = Expose Then
          '    Dim gwa As XWindowAttributes
          '    XGetWindowAttributes(olc_Display, olc_Window, gwa)
          '    nWindowWidth = gwa.width
          '    nWindowHeight = gwa.height
          '    olc_UpdateViewport()
          '    glClear(GL_COLOR_BUFFER_BIT) ' Thanks Benedani!
          '  ElseIf xev.type = ConfigureNotify Then
          '    Dim xce As XConfigureEvent = xev.xconfigure
          '    nWindowWidth = xce.width
          '    nWindowHeight = xce.height
          '  ElseIf xev.type = KeyPress Then
          '    Dim sym As KeySym = XLookupKeysym(xev.xkey, 0)
          '    pKeyNewState(mapKeys(sym)) = True
          '    Dim e As XKeyEvent = CType(xev, XKeyEvent) ' Because DragonEye loves numpads
          '    XLookupString(e, Nothing, 0, sym, Nothing)
          '    pKeyNewState(mapKeys(sym)) = True
          '  ElseIf xev.type = KeyRelease Then
          '    Dim sym As KeySym = XLookupKeysym(xev.xkey, 0)
          '    pKeyNewState(mapKeys(sym)) = False
          '    Dim e As XKeyEvent = CType(xev, XKeyEvent)
          '    XLookupString(e, Nothing, 0, sym, Nothing)
          '    pKeyNewState(mapKeys(sym)) = False
          '  ElseIf xev.type = ButtonPress Then
          '    Select Case xev.xbutton.button
          '      Case 1 : pMouseNewState(0) = True
          '      Case 2 : pMouseNewState(2) = True
          '      Case 3 : pMouseNewState(1) = True
          '      Case 4 : olc_UpdateMouseWheel(120)
          '      Case 5 : olc_UpdateMouseWheel(-120)
          '      Case Else
          '    End Select
          '  ElseIf xev.type = ButtonRelease Then
          '    Select Case xev.xbutton.button
          '      Case 1 : pMouseNewState(0) = False
          '      Case 2 : pMouseNewState(2) = False
          '      Case 3 : pMouseNewState(1) = False
          '      Case Else
          '    End Select
          '  ElseIf xev.type = MotionNotify Then
          '    olc_UpdateMouse(xev.xmotion.x, xev.xmotion.y)
          '  ElseIf xev.type = FocusIn Then
          '    bHasInputFocus = True
          '  ElseIf xev.type = FocusOut Then
          '    bHasInputFocus = False
          '  ElseIf xev.type = ClientMessage Then
          '    bAtomActive = False
          '  End If
          'End While

        End If

        ' Handle User Input - Keyboard
        For i = 0 To 255
          pKeyboardState(i).Pressed = False
          pKeyboardState(i).Released = False
          If pKeyNewState(i) <> pKeyOldState(i) Then
            If pKeyNewState(i) Then
              pKeyboardState(i).Pressed = Not pKeyboardState(i).Held
              pKeyboardState(i).Held = True
            Else
              pKeyboardState(i).Released = True
              pKeyboardState(i).Held = False
            End If
          End If
          pKeyOldState(i) = pKeyNewState(i)
        Next

        ' Handle User Input - Mouse
        For i = 0 To 4
          pMouseState(i).Pressed = False
          pMouseState(i).Released = False
          If pMouseNewState(i) <> pMouseOldState(i) Then
            If pMouseNewState(i) Then
              pMouseState(i).Pressed = Not pMouseState(i).Held
              pMouseState(i).Held = True
            Else
              pMouseState(i).Released = True
              pMouseState(i).Held = False
            End If
          End If
          pMouseOldState(i) = pMouseNewState(i)
        Next

        ' Cache mouse coordinates so they remain
        ' consistent during frame
        nMousePosX = nMousePosXcache
        nMousePosY = nMousePosYcache

        nMouseWheelDelta = nMouseWheelDeltaCache
        nMouseWheelDeltaCache = 0

        ' Handle Frame Update
        If Not OnUserUpdate(elapsedTime) Then
          Singleton.AtomActive = False
        End If

        ' Display Graphics
        glViewport(nViewX, nViewY, nViewW, nViewH)

        ' Copy pixel array into texture
        Dim data = pDefaultDrawTarget.GetData ' NOTE: DO NOT INLINE... perf hit big time!
        Dim b((data.Length * 4) - 1) As Byte
        For index = 0 To data.Length - 1
          b(index * 4) = data(index).R
          b((index * 4) + 1) = data(index).G
          b((index * 4) + 2) = data(index).B
          b((index * 4) + 3) = data(index).A
        Next
        Dim ptr = Marshal.AllocHGlobal(b.Length)
        Marshal.Copy(b, 0, ptr, b.Length)
        glTexSubImage2D(GL_TEXTURE_2D, 0, 0, 0, nScreenWidth, nScreenHeight, GL_RGBA, GL_UNSIGNED_BYTE, ptr)
        Marshal.FreeHGlobal(ptr)

        ' Display texture on screen
        glBegin(GL_QUADS)
        glTexCoord2f(0.0, 1.0) : glVertex3f(-1.0F + fSubPixelOffsetX, -1.0F + fSubPixelOffsetY, 0.0F)
        glTexCoord2f(0.0, 0.0) : glVertex3f(-1.0F + fSubPixelOffsetX, 1.0F + fSubPixelOffsetY, 0.0F)
        glTexCoord2f(1.0, 0.0) : glVertex3f(1.0F + fSubPixelOffsetX, 1.0F + fSubPixelOffsetY, 0.0F)
        glTexCoord2f(1.0, 1.0) : glVertex3f(1.0F + fSubPixelOffsetX, -1.0F + fSubPixelOffsetY, 0.0F)
        glEnd()

        ' Preset Graphics to screen
        If IsOSPlatform(Windows) Then
          SwapBuffers(glDeviceContext)
        ElseIf IsOSPlatform(Linux) Then
          glXSwapBuffers(olc_Display, olc_Window)
        End If

        ' Update Title Bar
        fFrameTimer += elapsedTime
        nFrameCount += 1

        If fFrameTimer >= 1.0F Then

          fFrameTimer -= 1.0F

          Dim sTitle = $"olcPixelGameEngineVB v0.1 - AddressOf.com/OneLoneCoder.com - {AppName} - FPS: {nFrameCount}"

          If IsOSPlatform(Windows) Then
            SetWindowText(olc_hWnd, sTitle)
          ElseIf IsOSPlatform(Linux) Then
            XStoreName(olc_Display, olc_Window, sTitle)
          End If

          nFrameCount = 0

        End If

      End While

      ' Allow the user to free resources if they have overridden the destroy function
      If OnUserDestroy() Then
        ' User has permitted destroy, so exit and clean up
      Else
        ' User denied destroy for some reason, so continue running
        Singleton.AtomActive = True
      End If

    End While

    If IsOSPlatform(Windows) Then
      wglDeleteContext(glRenderContext)
      PostMessage(olc_hWnd, WM_DESTROY, IntPtr.Zero, IntPtr.Zero)
    ElseIf IsOSPlatform(Linux) Then
      glXMakeCurrent(olc_Display, IntPtr.Zero, Nothing)
      glXDestroyContext(olc_Display, glDeviceContext)
      XDestroyWindow(olc_Display, olc_Window)
      XCloseDisplay(olc_Display)
    End If

  End Sub

  ' GDIPlusStartup?

  Private Sub olc_ConstructFontSheet()

    Dim data As String = ""
    data &= "?Q`0001oOch0o01o@F40o0<AGD4090LAGD<090@A7ch0?00O7Q`0600>00000000"
    data &= "O000000nOT0063Qo4d8>?7a14Gno94AA4gno94AaOT0>o3`oO400o7QN00000400"
    data &= "Of80001oOg<7O7moBGT7O7lABET024@aBEd714AiOdl717a_=TH013Q>00000000"
    data &= "720D000V?V5oB3Q_HdUoE7a9@DdDE4A9@DmoE4A;Hg]oM4Aj8S4D84@`00000000"
    data &= "OaPT1000Oa`^13P1@AI[?g`1@A=[OdAoHgljA4Ao?WlBA7l1710007l100000000"
    data &= "ObM6000oOfMV?3QoBDD`O7a0BDDH@5A0BDD<@5A0BGeVO5ao@CQR?5Po00000000"
    data &= "Oc``000?Ogij70PO2D]??0Ph2DUM@7i`2DTg@7lh2GUj?0TO0C1870T?00000000"
    data &= "70<4001o?P<7?1QoHg43O;`h@GT0@:@LB@d0>:@hN@L0@?aoN@<0O7ao0000?000"
    data &= "OcH0001SOglLA7mg24TnK7ln24US>0PL24U140PnOgl0>7QgOcH0K71S0000A000"
    data &= "00H00000@Dm1S007@DUSg00?OdTnH7YhOfTL<7Yh@Cl0700?@Ah0300700000000"
    data &= "<008001QL00ZA41a@6HnI<1i@FHLM81M@@0LG81?O`0nC?Y7?`0ZA7Y300080000"
    data &= "O`082000Oh0827mo6>Hn?Wmo?6HnMb11MP08@C11H`08@FP0@@0004@000000000"
    data &= "00P00001Oab00003OcKP0006@6=PMgl<@440MglH@000000`@000001P00000000"
    data &= "Ob@8@@00Ob@8@Ga13R@8Mga172@8?PAo3R@827QoOb@820@0O`0007`0000007P0"
    data &= "O`000P08Od400g`<3V=P0G`673IP0`@3>1`00P@6O`P00g`<O`000GP800000000"
    data &= "?P9PL020O`<`N3R0@E4HC7b0@ET<ATB0@@l6C4B0O`H3N7b0?P01L3R000000020"

    m_fontSprite = New Sprite(128, 48)

    Dim px = 0, py = 0
    For b = 0 To 1023 Step 4

      Dim sym1 = AscW(data(b + 0)) - 48
      Dim sym2 = AscW(data(b + 1)) - 48
      Dim sym3 = AscW(data(b + 2)) - 48
      Dim sym4 = AscW(data(b + 3)) - 48
      Dim r = sym1 << 18 Or sym2 << 12 Or sym3 << 6 Or sym4

      For i = 0 To 23
        Dim k = If((r And (1 << i)) <> 0, 255, 0)
        m_fontSprite.SetPixel(px, py, New Pixel(k, k, k, k))
        If System.Threading.Interlocked.Increment(py) = 48 Then
          px += 1 : py = 0
        End If
      Next

    Next

  End Sub

#Region "Windows"

  Private Function olc_WindowCreate_Windows() As IntPtr

    Dim wc As New WNDCLASS
    wc.hIcon = LoadIcon(IntPtr.Zero, IDI_APPLICATION)
    wc.hCursor = LoadCursor(IntPtr.Zero, IDC_ARROW)
    wc.Style = CS_HREDRAW Or CS_VREDRAW Or CS_OWNDC
    wc.hInstance = GetModuleHandle(Nothing)
    wc.WndProc = Marshal.GetFunctionPointerForDelegate(m_delegWndProc)
    wc.ClsExtra = 0
    wc.WndExtra = 0
    wc.MenuName = Nothing
    wc.hBackground = CType(COLOR_BACKGROUND, IntPtr) + 1 'Nothing
    wc.ClassName = "OLC_PIXEL_GAME_ENGINE"

    Dim atom = RegisterClass(wc)
    If atom = 0 Then
      Dim er = GetLastError
      Return IntPtr.Zero
    End If

    nWindowWidth = nScreenWidth * nPixelWidth
    nWindowHeight = nScreenHeight * nPixelHeight

    ' Define window furniture
    Dim dwExStyle = WS_EX_APPWINDOW Or WS_EX_WINDOWEDGE
    Dim dwStyle = WS_CAPTION Or WS_SYSMENU Or WS_VISIBLE Or WS_THICKFRAME

    Dim nCosmeticOffset = 30
    nViewW = nWindowWidth
    nViewH = nWindowHeight

    ' Handle Fullscreen
    If bFullScreen Then
      dwExStyle = 0
      dwStyle = WS_VISIBLE Or WS_POPUP
      nCosmeticOffset = 0
      Dim hmon = MonitorFromWindow(olc_hWnd, MONITOR_DEFAULTTONEAREST)
      Dim mi = New MONITORINFO With {.cbSize = Marshal.SizeOf(GetType(MONITORINFO))}
      If Not GetMonitorInfo(hmon, mi) Then Return Nothing
      nWindowWidth = mi.rcMonitor.Right
      nWindowHeight = mi.rcMonitor.Bottom
    End If

    olc_UpdateViewport()

    ' Keep client size as requested
    Dim rWndRect = New RECT With {.Left = 0, .Top = 0, .Right = nWindowWidth, .Bottom = nWindowHeight}
    AdjustWindowRectEx(rWndRect, dwStyle, False, dwExStyle)
    Dim width = rWndRect.Right - rWndRect.Left
    Dim height = rWndRect.Bottom - rWndRect.Top

    'Singleton.Pge = Me
    olc_hWnd = CreateWindowEx(dwExStyle, atom, "", dwStyle,
                              nCosmeticOffset, nCosmeticOffset, width, height, Nothing, Nothing,
                              GetModuleHandle(Nothing), IntPtr.Zero)

    'Dim tme = New TRACKMOUSEEVENTSTRUCT
    'tme.cbSize = Marshal.SizeOf(GetType(TRACKMOUSEEVENTSTRUCT))
    'tme.dwFlags = TME_LEAVE
    'tme.hWnd = olc_hWnd
    'TrackMouseEvent(tme)

    ' Create Keyboard Mapping
    Singleton.MapKeys(&H0) = Key.NONE
    Singleton.MapKeys(&H41) = Key.A : Singleton.MapKeys(&H42) = Key.B : Singleton.MapKeys(&H43) = Key.C : Singleton.MapKeys(&H44) = Key.D : Singleton.MapKeys(&H45) = Key.E
    Singleton.MapKeys(&H46) = Key.F : Singleton.MapKeys(&H47) = Key.G : Singleton.MapKeys(&H48) = Key.H : Singleton.MapKeys(&H49) = Key.I : Singleton.MapKeys(&H50) = Key.J
    Singleton.MapKeys(&H4B) = Key.K : Singleton.MapKeys(&H4C) = Key.L : Singleton.MapKeys(&H4D) = Key.M : Singleton.MapKeys(&H4E) = Key.N : Singleton.MapKeys(&H4F) = Key.O
    Singleton.MapKeys(&H50) = Key.P : Singleton.MapKeys(&H51) = Key.Q : Singleton.MapKeys(&H52) = Key.R : Singleton.MapKeys(&H53) = Key.S : Singleton.MapKeys(&H54) = Key.T
    Singleton.MapKeys(&H55) = Key.U : Singleton.MapKeys(&H56) = Key.V : Singleton.MapKeys(&H57) = Key.W : Singleton.MapKeys(&H58) = Key.X : Singleton.MapKeys(&H59) = Key.Y
    Singleton.MapKeys(&H5A) = Key.Z

    Singleton.MapKeys(VK_F1) = Key.F1 : Singleton.MapKeys(VK_F2) = Key.F2 : Singleton.MapKeys(VK_F3) = Key.F3 : Singleton.MapKeys(VK_F4) = Key.F4
    Singleton.MapKeys(VK_F5) = Key.F5 : Singleton.MapKeys(VK_F6) = Key.F6 : Singleton.MapKeys(VK_F7) = Key.F7 : Singleton.MapKeys(VK_F8) = Key.F8
    Singleton.MapKeys(VK_F9) = Key.F9 : Singleton.MapKeys(VK_F10) = Key.F10 : Singleton.MapKeys(VK_F11) = Key.F11 : Singleton.MapKeys(VK_F12) = Key.F12

    Singleton.MapKeys(VK_DOWN) = Key.DOWN : Singleton.MapKeys(VK_LEFT) = Key.LEFT : Singleton.MapKeys(VK_RIGHT) = Key.RIGHT : Singleton.MapKeys(VK_UP) = Key.UP
    Singleton.MapKeys(VK_RETURN) = Key.ENTER 'mapKeys(VK_RETURN) = Key.RETURN

    Singleton.MapKeys(VK_BACK) = Key.BACK : Singleton.MapKeys(VK_ESCAPE) = Key.ESCAPE : Singleton.MapKeys(VK_RETURN) = Key.ENTER : Singleton.MapKeys(VK_PAUSE) = Key.PAUSE
    Singleton.MapKeys(VK_SCROLL) = Key.SCROLL : Singleton.MapKeys(VK_TAB) = Key.TAB : Singleton.MapKeys(VK_DELETE) = Key.DEL : Singleton.MapKeys(VK_HOME) = Key.HOME
    Singleton.MapKeys(VK_END) = Key.END : Singleton.MapKeys(VK_PRIOR) = Key.PGUP : Singleton.MapKeys(VK_NEXT) = Key.PGDN : Singleton.MapKeys(VK_INSERT) = Key.INS
    Singleton.MapKeys(VK_SHIFT) = Key.SHIFT : Singleton.MapKeys(VK_CONTROL) = Key.CTRL
    Singleton.MapKeys(VK_SPACE) = Key.SPACE

    Singleton.MapKeys(&H30) = Key.K0 : Singleton.MapKeys(&H31) = Key.K1 : Singleton.MapKeys(&H32) = Key.K2 : Singleton.MapKeys(&H33) = Key.K3 : Singleton.MapKeys(&H34) = Key.K4
    Singleton.MapKeys(&H35) = Key.K5 : Singleton.MapKeys(&H36) = Key.K6 : Singleton.MapKeys(&H37) = Key.K7 : Singleton.MapKeys(&H38) = Key.K8 : Singleton.MapKeys(&H39) = Key.K9

    Singleton.MapKeys(VK_NUMPAD0) = Key.NP0 : Singleton.MapKeys(VK_NUMPAD1) = Key.NP1 : Singleton.MapKeys(VK_NUMPAD2) = Key.NP2 : Singleton.MapKeys(VK_NUMPAD3) = Key.NP3 : Singleton.MapKeys(VK_NUMPAD4) = Key.NP4
    Singleton.MapKeys(VK_NUMPAD5) = Key.NP5 : Singleton.MapKeys(VK_NUMPAD6) = Key.NP6 : Singleton.MapKeys(VK_NUMPAD7) = Key.NP7 : Singleton.MapKeys(VK_NUMPAD8) = Key.NP8 : Singleton.MapKeys(VK_NUMPAD9) = Key.NP9
    Singleton.MapKeys(VK_MULTIPLY) = Key.NP_MUL : Singleton.MapKeys(VK_ADD) = Key.NP_ADD : Singleton.MapKeys(VK_DIVIDE) = Key.NP_DIV : Singleton.MapKeys(VK_SUBTRACT) = Key.NP_SUB : Singleton.MapKeys(VK_DECIMAL) = Key.NP_DECIMAL

    Return olc_hWnd

  End Function

  Private Function olc_OpenGLCreate_Windows() As Boolean

    ' Create Device Context
    glDeviceContext = GetDC(olc_hWnd)
    Dim pfd As New PIXELFORMATDESCRIPTOR With {.nSize = CUShort(Marshal.SizeOf(GetType(PIXELFORMATDESCRIPTOR))),
                                               .nVersion = 1,
                                               .dwFlags = PFD_DRAW_TO_WINDOW Or PFD_SUPPORT_OPENGL Or PFD_DOUBLEBUFFER,
                                               .iPixelType = PFD_TYPE_RGBA,
                                               .cColorBits = 32,
                                               .cRedBits = 0,
                                               .cRedShift = 0,
                                               .cGreenBits = 0,
                                               .cGreenShift = 0,
                                               .cBlueBits = 0,
                                               .cBlueShift = 0,
                                               .cAlphaBits = 0,
                                               .cAlphaShift = 0,
                                               .cAccumBits = 0,
                                               .cAccumRedBits = 0,
                                               .cAccumGreenBits = 0,
                                               .cAccumBlueBits = 0,
                                               .cAccumAlphaBits = 0,
                                               .cDepthBits = 24, '0,
                                               .cStencilBits = 8, '0,
                                               .cAuxBuffers = 0,
                                               .iLayerType = PFD_MAIN_PLANE,
                                               .bReserved = 0,
                                               .dwLayerMask = 0,
                                               .dwVisibleMask = 0,
                                               .dwDamageMask = 0}

    Dim pf = ChoosePixelFormat(glDeviceContext, pfd) : If pf = 0 Then Return False
    SetPixelFormat(glDeviceContext, pf, pfd)

    glRenderContext = wglCreateContext(glDeviceContext) : If glRenderContext = IntPtr.Zero Then Return False
    wglMakeCurrent(glDeviceContext, glRenderContext)

    glViewport(nViewX, nViewY, nViewW, nViewH)

    ' Remove Frame cap
    wglSwapInterval = CType(Marshal.GetDelegateForFunctionPointer(wglGetProcAddress("wglSwapIntervalEXT"), GetType(wglSwapInterval_t)), wglSwapInterval_t)
    If wglSwapInterval IsNot Nothing AndAlso Not bEnableVSYNC Then wglSwapInterval(0)

    Return True

  End Function

  ' Windows Event Handler
  Private Shared Function olc_WindowEvent(hWnd As IntPtr, uMsg As UInt32, wParam As IntPtr, lParam As IntPtr) As IntPtr
    'Static sge As PixelGameEngine = Nothing
    Select Case uMsg
      Case WM_CREATE
        'NOTE: swapped out trying to get a reference to the PGE by passing it through CreateWindowEx and instead
        '      modified the code (at CreateWindowEx) so that a shared reference to self (Me) is created there.
        'Dim createStruct = Marshal.PtrToStructure(Of CREATESTRUCT)(lParam)
        'sge = CType(Marshal.PtrToStructure(createStruct.lpCreateParams, GetType(PixelGameEngine)), PixelGameEngine)
        Return IntPtr.Zero
      Case WM_MOUSEMOVE
        Dim v = CInt(lParam)
        Dim x = v And &HFFFF
        Dim y = (v >> 16) And &HFFFF
        Dim ix = BitConverter.ToInt16(BitConverter.GetBytes(x), 0)
        Dim iy = BitConverter.ToInt16(BitConverter.GetBytes(y), 0)
        Singleton.Pge.olc_UpdateMouse(ix, iy)
        Return IntPtr.Zero
      Case WM_SIZE
        Dim v = CInt(lParam)
        Singleton.Pge.olc_UpdateWindowSize(v And &HFFFF, (v >> 16) And &HFFFF)
        Return IntPtr.Zero
      Case WM_MOUSEWHEEL
        Singleton.Pge.olc_UpdateMouseWheel(GET_WHEEL_DELTA_WPARAM(wParam))
        Return IntPtr.Zero
      Case WM_MOUSELEAVE
        'TODO: WM_MOUSELEAVE is working *once*, not sure why...
        Singleton.Pge.bHasMouseFocus = False
        Return IntPtr.Zero
      Case WM_SETFOCUS
        Singleton.Pge.bHasInputFocus = True
        Return IntPtr.Zero
      Case WM_KILLFOCUS
        Singleton.Pge.bHasInputFocus = False
        Return IntPtr.Zero
      Case WM_KEYDOWN
        Singleton.Pge.pKeyNewState(Singleton.MapKeys(wParam.ToInt32())) = True
        Return IntPtr.Zero
      Case WM_KEYUP
        Singleton.Pge.pKeyNewState(Singleton.MapKeys(wParam.ToInt32())) = False
        Return IntPtr.Zero
      Case WM_LBUTTONDOWN
        Singleton.Pge.pMouseNewState(0) = True
        Return IntPtr.Zero
      Case WM_LBUTTONUP
        Singleton.Pge.pMouseNewState(0) = False
        Return IntPtr.Zero
      Case WM_RBUTTONDOWN
        Singleton.Pge.pMouseNewState(1) = True
        Return IntPtr.Zero
      Case WM_RBUTTONUP
        Singleton.Pge.pMouseNewState(1) = False
        Return IntPtr.Zero
      Case WM_MBUTTONDOWN
        Singleton.Pge.pMouseNewState(2) = True
        Return IntPtr.Zero
      Case WM_MBUTTONUP
        Singleton.Pge.pMouseNewState(2) = False
        Return IntPtr.Zero
      Case WM_CLOSE
        Singleton.AtomActive = False
        Return IntPtr.Zero
      Case WM_DESTROY
        PostQuitMessage(0)
        Return IntPtr.Zero
    End Select
    Return DefWindowProc(hWnd, uMsg, wParam, lParam)
  End Function

#End Region

#Region "Linux"

  ' Do the Linux stuff!
  Private Function olc_WindowCreate_Linux() As IntPtr

    XInitThreads()

    ' Grab the deafult display and window
    olc_Display = XOpenDisplay(Nothing)
    olc_WindowRoot = DefaultRootWindow(olc_Display)

    ' Based on the display capabilities, configure the appearance of the window
    Dim olc_GLAttribs() As Integer = {GLX_RGBA, GLX_DEPTH_SIZE, 24, GLX_DOUBLEBUFFER, None}
    olc_VisualInfo = glXChooseVisual(olc_Display, 0, olc_GLAttribs)
    olc_ColourMap = XCreateColormap(olc_Display, olc_WindowRoot, olc_VisualInfo.visual, AllocNone)
    olc_SetWindowAttribs.colormap = olc_ColourMap

    ' Register which events we are interested in receiving
    olc_SetWindowAttribs.event_mask = ExposureMask Or KeyPressMask Or KeyReleaseMask Or ButtonPressMask Or ButtonReleaseMask Or PointerMotionMask Or FocusChangeMask Or StructureNotifyMask

    ' Create the window
    olc_Window = XCreateWindow(olc_Display, olc_WindowRoot, 30, 30, nScreenWidth * nPixelWidth, nScreenHeight * nPixelHeight, 0, olc_VisualInfo.depth, InputOutput, olc_VisualInfo.visual, CWColormap Or CWEventMask, olc_SetWindowAttribs)

    Dim wmDelete = XInternAtom(olc_Display, "WM_DELETE_WINDOW", True)
    XSetWMProtocols(olc_Display, olc_Window, {wmDelete}, 1)

    XMapWindow(olc_Display, olc_Window)
    XStoreName(olc_Display, olc_Window, "OneLoneCoder.com - Pixel Game Engine")

    If bFullScreen Then ' Thanks DragonEye, again :D
      Dim wm_state As IntPtr
      Dim fullscreen As Byte
      wm_state = XInternAtom(olc_Display, "_NET_WM_STATE", False)
      fullscreen = CByte(XInternAtom(olc_Display, "_NET_WM_STATE_FULLSCREEN", False))
      Dim xev As XEvent = Nothing
      xev.type = ClientMessage
      xev.xclient.window = olc_Window
      xev.xclient.message_type = wm_state
      xev.xclient.format = 32
      xev.xclient.data.l(0) = CByte(If(bFullScreen, 1, 0)) ' the action (0: off, 1: on, 2: toggle)
      xev.xclient.data.l(1) = fullscreen ' first property to alter
      xev.xclient.data.l(2) = 0 ' second property to alter
      xev.xclient.data.l(3) = 0 ' source indication
      XMapWindow(olc_Display, olc_Window)
      XSendEvent(olc_Display, DefaultRootWindow(olc_Display), False, SubstructureRedirectMask Or SubstructureNotifyMask, xev)
      XFlush(olc_Display)
      Dim gwa As XWindowAttributes
      XGetWindowAttributes(olc_Display, olc_Window, gwa)
      nWindowWidth = gwa.width
      nWindowHeight = gwa.height
      olc_UpdateViewport()
    End If

    ' Create Keyboard Mapping
    Singleton.MapKeys(&H0) = Key.NONE
    Singleton.MapKeys(&H61) = Key.A : Singleton.MapKeys(&H62) = Key.B : Singleton.MapKeys(&H63) = Key.C : Singleton.MapKeys(&H64) = Key.D : Singleton.MapKeys(&H65) = Key.E
    Singleton.MapKeys(&H66) = Key.F : Singleton.MapKeys(&H67) = Key.G : Singleton.MapKeys(&H68) = Key.H : Singleton.MapKeys(&H69) = Key.I : Singleton.MapKeys(&H6A) = Key.J
    Singleton.MapKeys(&H6B) = Key.K : Singleton.MapKeys(&H6C) = Key.L : Singleton.MapKeys(&H6D) = Key.M : Singleton.MapKeys(&H6E) = Key.N : Singleton.MapKeys(&H6F) = Key.O
    Singleton.MapKeys(&H70) = Key.P : Singleton.MapKeys(&H71) = Key.Q : Singleton.MapKeys(&H72) = Key.R : Singleton.MapKeys(&H73) = Key.S : Singleton.MapKeys(&H74) = Key.T
    Singleton.MapKeys(&H75) = Key.U : Singleton.MapKeys(&H76) = Key.V : Singleton.MapKeys(&H77) = Key.W : Singleton.MapKeys(&H78) = Key.X : Singleton.MapKeys(&H79) = Key.Y
    Singleton.MapKeys(&H7A) = Key.Z

    Singleton.MapKeys(XK_F1) = Key.F1 : Singleton.MapKeys(XK_F2) = Key.F2 : Singleton.MapKeys(XK_F3) = Key.F3 : Singleton.MapKeys(XK_F4) = Key.F4
    Singleton.MapKeys(XK_F5) = Key.F5 : Singleton.MapKeys(XK_F6) = Key.F6 : Singleton.MapKeys(XK_F7) = Key.F7 : Singleton.MapKeys(XK_F8) = Key.F8
    Singleton.MapKeys(XK_F9) = Key.F9 : Singleton.MapKeys(XK_F10) = Key.F10 : Singleton.MapKeys(XK_F11) = Key.F11 : Singleton.MapKeys(XK_F12) = Key.F12

    Singleton.MapKeys(XK_Down) = Key.DOWN : Singleton.MapKeys(XK_Left) = Key.LEFT : Singleton.MapKeys(XK_Right) = Key.RIGHT : Singleton.MapKeys(XK_Up) = Key.UP
    Singleton.MapKeys(XK_KP_Enter) = Key.ENTER : Singleton.MapKeys(XK_Return) = Key.ENTER

    Singleton.MapKeys(XK_BackSpace) = Key.BACK : Singleton.MapKeys(XK_Escape) = Key.ESCAPE : Singleton.MapKeys(XK_Linefeed) = Key.ENTER : Singleton.MapKeys(XK_Pause) = Key.PAUSE
    Singleton.MapKeys(XK_Scroll_Lock) = Key.SCROLL : Singleton.MapKeys(XK_Tab) = Key.TAB : Singleton.MapKeys(XK_Delete) = Key.DEL : Singleton.MapKeys(XK_Home) = Key.HOME
    Singleton.MapKeys(XK_End) = Key.END : Singleton.MapKeys(XK_Page_Up) = Key.PGUP : Singleton.MapKeys(XK_Page_Down) = Key.PGDN : Singleton.MapKeys(XK_Insert) = Key.INS
    Singleton.MapKeys(XK_Shift_L) = Key.SHIFT : Singleton.MapKeys(XK_Shift_R) = Key.SHIFT : Singleton.MapKeys(XK_Control_L) = Key.CTRL : Singleton.MapKeys(XK_Control_R) = Key.CTRL
    Singleton.MapKeys(XK_space) = Key.SPACE

    Singleton.MapKeys(XK_0) = Key.K0 : Singleton.MapKeys(XK_1) = Key.K1 : Singleton.MapKeys(XK_2) = Key.K2 : Singleton.MapKeys(XK_3) = Key.K3 : Singleton.MapKeys(XK_4) = Key.K4
    Singleton.MapKeys(XK_5) = Key.K5 : Singleton.MapKeys(XK_6) = Key.K6 : Singleton.MapKeys(XK_7) = Key.K7 : Singleton.MapKeys(XK_8) = Key.K8 : Singleton.MapKeys(XK_9) = Key.K9

    Singleton.MapKeys(XK_KP_0) = Key.NP0 : Singleton.MapKeys(XK_KP_1) = Key.NP1 : Singleton.MapKeys(XK_KP_2) = Key.NP2 : Singleton.MapKeys(XK_KP_3) = Key.NP3 : Singleton.MapKeys(XK_KP_4) = Key.NP4
    Singleton.MapKeys(XK_KP_5) = Key.NP5 : Singleton.MapKeys(XK_KP_6) = Key.NP6 : Singleton.MapKeys(XK_KP_7) = Key.NP7 : Singleton.MapKeys(XK_KP_8) = Key.NP8 : Singleton.MapKeys(XK_KP_9) = Key.NP9
    Singleton.MapKeys(XK_KP_Multiply) = Key.NP_MUL : Singleton.MapKeys(XK_KP_Add) = Key.NP_ADD : Singleton.MapKeys(XK_KP_Divide) = Key.NP_DIV : Singleton.MapKeys(XK_KP_Subtract) = Key.NP_SUB : Singleton.MapKeys(XK_KP_Decimal) = Key.NP_DECIMAL

    Return olc_Display

  End Function

  Function olc_OpenGLCreate_Linux() As Boolean

    Dim glDeviceContext As GLXContext = glXCreateContext(olc_Display, olc_VisualInfo, IntPtr.Zero, GL_TRUE = 1)
    glXMakeCurrent(olc_Display, olc_Window, glDeviceContext)

    Dim gwa As XWindowAttributes
    XGetWindowAttributes(olc_Display, olc_Window, gwa)
    glViewport(0, 0, gwa.width, gwa.height)

    Dim glSwapIntervalEXT As glSwapInterval_t = Nothing
    glSwapIntervalEXT = CType(Marshal.GetDelegateForFunctionPointer(glXGetProcAddress("glXSwapIntervalEXT"), GetType(glSwapInterval_t)), glSwapInterval_t)

    If glSwapIntervalEXT Is Nothing AndAlso Not bEnableVSYNC Then
      Console.WriteLine("NOTE: Could not disable VSYNC, glXSwapIntervalEXT() was not found!")
      Console.WriteLine("      Don't worry though, things will still work, it's just the")
      Console.WriteLine("      frame rate will be capped to your monitors refresh rate - javidx9")
    End If

    If glSwapIntervalEXT IsNot Nothing AndAlso Not bEnableVSYNC Then
      glSwapIntervalEXT(olc_Display, olc_Window, 0)
    End If

    Return True

  End Function

#End Region

#Region "Additional"

  Private Shared Function GET_WHEEL_DELTA_WPARAM(wParam As IntPtr) As Integer
    Dim l = wParam.ToInt64
    Dim v = CInt(If(l > Integer.MaxValue, l - UInteger.MaxValue, l))
    Return CShort(v >> 16)
  End Function

  Public Shared Sub Swap(ByRef a As Integer, ByRef b As Integer)
    Dim t = a
    a = b
    b = t
  End Sub

  Public Shared Sub Swap(ByRef a As Single, ByRef b As Single)
    Dim t = a
    a = b
    b = t
  End Sub

#End Region

#Region "C++'isms"

  Private ReadOnly m_random As New Random
  Protected Const RAND_MAX As Integer = 2147483647

  Protected ReadOnly Property Rnd As Double
    Get
      Return m_random.NextDouble
    End Get
  End Property

  ' Provide for something *similar* to C++.
  Protected ReadOnly Property Rand As Integer
    Get
      Return CInt(Fix(m_random.NextDouble * RAND_MAX))
    End Get
  End Property

#End Region

#Region "CGE"

  Private Function ConsoleColor2PixelColor(c As COLOUR) As Pixel
    Select Case c
      Case COLOUR.FG_BLACK : Return Presets.Black
      Case COLOUR.FG_DARK_BLUE : Return Presets.DarkBlue
      Case COLOUR.FG_DARK_GREEN : Return Presets.DarkGreen
      Case COLOUR.FG_DARK_CYAN : Return Presets.DarkCyan
      Case COLOUR.FG_DARK_RED : Return Presets.DarkRed
      Case COLOUR.FG_DARK_MAGENTA : Return Presets.DarkMagenta
      Case COLOUR.FG_DARK_YELLOW : Return Presets.DarkYellow
      Case COLOUR.FG_GREY : Return Presets.Grey
      Case COLOUR.FG_DARK_GREY : Return Presets.DarkGrey
      Case COLOUR.FG_BLUE : Return Presets.Blue
      Case COLOUR.FG_GREEN : Return Presets.Green
      Case COLOUR.FG_CYAN : Return Presets.Cyan
      Case COLOUR.FG_RED : Return Presets.Red
      Case COLOUR.FG_MAGENTA : Return Presets.Magenta
      Case COLOUR.FG_YELLOW : Return Presets.Yellow
      Case COLOUR.FG_WHITE : Return Presets.White
      Case COLOUR.BG_BLACK : Return Presets.Black
      Case COLOUR.BG_DARK_BLUE : Return Presets.DarkBlue
      Case COLOUR.BG_DARK_GREEN : Return Presets.DarkGreen
      Case COLOUR.BG_DARK_CYAN : Return Presets.DarkCyan
      Case COLOUR.BG_DARK_RED : Return Presets.DarkRed
      Case COLOUR.BG_DARK_MAGENTA : Return Presets.DarkMagenta
      Case COLOUR.BG_DARK_YELLOW : Return Presets.DarkYellow
      Case COLOUR.BG_GREY : Return Presets.Grey
      Case COLOUR.BG_DARK_GREY : Return Presets.DarkGrey
      Case COLOUR.BG_BLUE : Return Presets.Blue
      Case COLOUR.BG_GREEN : Return Presets.Green
      Case COLOUR.BG_CYAN : Return Presets.Cyan
      Case COLOUR.BG_RED : Return Presets.Red
      Case COLOUR.BG_MAGENTA : Return Presets.Magenta
      Case COLOUR.BG_YELLOW : Return Presets.Yellow
      Case COLOUR.BG_WHITE : Return Presets.White
      Case Else
        Return Presets.White
    End Select
  End Function

  Public Function ConstructConsole(w As Integer, h As Integer, pw As Integer, ph As Integer) As Boolean
    Return Construct(w, h, pw, ph)
  End Function

  Protected Sub Fill(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer, dummy As PIXEL_TYPE, c As COLOUR)
    Dim w = (x2 - x1) + 1
    Dim h = (y2 - y1) + 1
    FillRect(x1, y1, w, h, ConsoleColor2PixelColor(c))
  End Sub

  Protected Sub FillCircle(x As Single, y As Single, radius As Single, dummy As PIXEL_TYPE, c As COLOUR)
    FillCircle(CInt(Fix(x)), CInt(Fix(y)), CInt(Fix(radius)), ConsoleColor2PixelColor(c))
  End Sub

  Protected Sub FillCircle(x As Integer, y As Integer, radius As Single, dummy As PIXEL_TYPE, c As COLOUR)
    FillCircle(x, y, CInt(Fix(radius)), ConsoleColor2PixelColor(c))
  End Sub

  Protected Sub DrawLine(x1 As Single, y1 As Single, x2 As Single, y2 As Single, dummy As PIXEL_TYPE, c As COLOUR)
    DrawLine(CInt(Fix(x1)), CInt(Fix(y1)), CInt(Fix(x2)), CInt(Fix(y2)), ConsoleColor2PixelColor(c))
  End Sub

  Protected Sub DrawLine(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer, dummy As PIXEL_TYPE, c As COLOUR)
    DrawLine(x1, y1, x2, y2, ConsoleColor2PixelColor(c))
  End Sub

#End Region

End Class

Public MustInherit Class PgeX

  'Public Shared Property Pge As PixelGameEngine

End Class

#Region "CGE"

Public Enum COLOUR As Short
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

#End Region