'https://www.codeguru.com/visual-basic/user-account-control-message-box/

Public Class Form1

  Private Const DESKTOP_SECURE As Integer = 131527
  Private Const DESKTOP_SWITCHDESKTOP As Integer = 256
  Private Const SND_ASYNC As Integer = 1
  Private Const SND_NOSTOP As Integer = 16
  Private Const SND_PURGE As Integer = 64
  Private Const SND_FILENAME As Integer = 131072
  Private Const SPI_SETDESKWALLPAPER As Integer = 20
  Private Const SPIF_UPDATEINIFILE As Integer = 1
  Private Const SPIF_SENDWININICHANGE As Integer = 2
  Private Const DESKTOP_LOGON As String = "Winlogon"
  Private Const DESKTOP_WINSTATION0 As String = "WinSta0"
  Private Const DESKTOP_DEFAULT As String = "Default"

  Private Structure SECURITY_ATTRIBUTES
    Public nLength As Integer
    Public lpSecurityDescriptor As Integer
    Public bInheritHandle As Integer
  End Structure

  Private Declare Function apiCloseDesktop Lib "user32" Alias "CloseDesktop" (hDesktop As Integer) As Integer
  Private Declare Function apiCreateDesktop Lib "user32" Alias "CreateDesktopA" (lDesktop As String, lDevice As Integer, devmode As Integer, dwFlags As Integer, desiredAccess As Integer, ByRef secAttribute As SECURITY_ATTRIBUTES) As Integer
  Private Declare Function apiGetCurrentThreadId Lib "kernel32" Alias "GetCurrentThreadId" () As Integer
  Private Declare Function apiGetDC Lib "user32" Alias "GetDC" (hWnd As Integer) As Integer
  Private Declare Function apiGetProcessWindowStation Lib "user32" Alias "GetProcessWindowStation" () As Integer
  Private Declare Function apiGetSystemDirectory Lib "kernel32" Alias "GetSystemDirectoryA" (lpBuffer As String, nSize As Integer) As Integer
  Private Declare Function apiGetThreadDesktop Lib "user32" Alias "GetThreadDesktop" (dwThread As Integer) As Integer
  Private Declare Function apiGetWindowDC Lib "user32" Alias "GetWindowDC" (hWnd As Integer) As Integer
  Private Declare Function apiOpenInputDesktop Lib "user32" Alias "OpenInputDesktop" (dwFlags As Integer, fInherit As Boolean, dwDesiredAccess As Integer) As Integer
  Private Declare Function apiPaintDesktop Lib "user32" Alias "PaintDesktop" (hDC As Integer) As Integer
  Private Declare Function apiPlaySound Lib "winmm" Alias "PlaySoundA" (lpszName As String, hModule As Integer, dwFlags As Integer) As Integer
  Private Declare Function apiSetThreadDesktop Lib "user32" Alias "SetThreadDesktop" (hDesktop As Integer) As Integer
  Private Declare Function apiSwitchDesktop Lib "user32" Alias "SwitchDesktop" (hDesktop As Integer) As Integer
  Private Declare Function apiSystemParametersInfo Lib "user32" Alias "SystemParametersInfoA" (uAction As Integer, uParam As Integer, lParam As String, fuWinIni As Integer) As Integer
  Private Declare Function apiWaitForSingleObject Lib "kernel32" Alias "WaitForSingleObject" (hHandle As Integer, dwMilliseconds As Integer) As Integer

  Private oldDskThread As Integer
  Private oldDskInput As Integer
  Private hwnDsk As Integer

  Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

  End Sub

  Public Function CreateDesktop(sDesktopName As String) As Integer
    On Error Resume Next
    Dim sa As SECURITY_ATTRIBUTES
    hwnDsk = apiCreateDesktop(sDesktopName, 0, 0, 0, DESKTOP_SECURE, sa)
    If hwnDsk = 0 Then
      Return 0
    Else
      Return hwnDsk
    End If
  End Function

  Public Function SwitchToDeskTop() As Integer
    On Error Resume Next
    Dim st = apiSetThreadDesktop(hwnDsk)
    Dim sd = apiSwitchDesktop(hwnDsk)
    If sd <> 0 Then
      Return 1
    End If
  End Function

  Public Sub CloseDeskTop()
    On Error Resume Next
    apiCloseDesktop(hwnDsk)
  End Sub

  Public Function PromptMessageUAC(message As String, title As String, timeout As Integer) As MB_RESULT
    On Error Resume Next
    'DoEvents

    Dim dskname As String
    Dim rn As Integer
    Randomize()
    rn = Rnd() * (2147483647 - 1) + 1
    dskname = CStr(rn)
    oldDskThread = apiGetThreadDesktop(apiGetCurrentThreadId)
    oldDskInput = apiOpenInputDesktop(0, False, DESKTOP_SWITCHDESKTOP)
    If CreateDesktop(dskname) = 0 Then Exit Function
    SwitchToDeskTop()
    PromptMessageUAC = MessageBoxShow(message, title, MB_YES_NO_SECURE, 20000, 0)
    CloseDeskTop()
    apiSetThreadDesktop(oldDskThread)
    apiSwitchDesktop(oldDskInput)
    'DoEvents
  End Function

  Private Const MB_OK As Integer = &H0
  Private Const MB_OKCANCEL As Integer = &H1
  Private Const MB_ABORTRETRYIGNORE As Integer = &H2
  Private Const MB_YESNOCANCEL As Integer = &H3
  Private Const MB_YESNO As Integer = &H4
  Private Const MB_RETRYCANCEL As Integer = &H5
  Private Const MB_MAX_TIMEOUT As Integer = &HFFFFFFFF
  Private Const MB_ICONERROR As Integer = &H10
  Private Const MB_ICONQUESTION As Integer = &H20
  Private Const MB_ICONWARNING As Integer = &H30
  Private Const MB_ICONINFORMATION As Integer = &H40
  Private Const MB_SERVICE_NOTIFICATION As Integer = &H200000

  Public Const MB_YES_NO_SECURE As Integer = MB_YESNO Or MB_ICONQUESTION Or MB_SERVICE_NOTIFICATION

  Public Enum MB_RESULT
    IOK = 1
    ICANCEL = 2
    IABORT = 3
    IRETRY = 4
    IIGNORE = 5
    IYES = 6
    INO = 7
    ITRYAGAIN = 10
    ICONTINUE = 11
  End Enum

  Private Declare Function apiMessageBoxTimeOut Lib "user32" Alias "MessageBoxTimeoutA" (prmlngWindowHandle As Integer, prmstrMessage As String, prmstrCaption As String, prmlngType As Integer, prmwLanguage As Integer, prmdwMiliseconds As Integer) As Integer

  Public Function MessageBoxShow(message As String, Caption As String, flags As Integer, TimeOutMilliseconds As Integer, hWnd As Integer) As MB_RESULT
    On Error GoTo poop
    MessageBoxShow = apiMessageBoxTimeOut(hWnd, message, Caption, flags, 0, TimeOutMilliseconds)
    Exit Function
poop:
    MessageBoxShow = -1
  End Function

  Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    Dim o As MB_RESULT
    o = PromptMessageUAC("Are you sure?", "Secure message transaction", 20000)
    If o = MB_RESULT.IYES Then
      MsgBox("User is sure")
    ElseIf o = MB_RESULT.INO Then
      MsgBox("user is unsure")
    ElseIf o = 32000 Then
      MsgBox("User did not decide. Message box has timed out")
    ElseIf IsNumeric(o) = False Then
      MsgBox(o)
    End If

  End Sub

End Class
