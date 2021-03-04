Option Explicit On
Option Strict On
Option Infer On

Imports System.Runtime.InteropServices

Public NotInheritable Class DarkMode

  Private Sub New()
  End Sub

  <DllImport("dwmapi.dll")>
  Private Shared Function DwmSetWindowAttribute(hwnd As IntPtr, attr As Integer, ByRef attrValue As Integer, attrSize As Integer) As Integer
  End Function

  Private Const DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 As Integer = 19
  Private Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20

  Public Shared Function IsWindows10(Optional build As Integer = -1) As Boolean
    Dim windowsMajor = Environment.OSVersion.Version.Major
    Dim windowsBuild = Environment.OSVersion.Version.Build
    Return windowsMajor >= 10 AndAlso windowsBuild >= build
  End Function

  Public Shared Function ToggleImmersiveDarkMode(handle As IntPtr, enabled As Boolean) As Boolean
    If RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
      If IsWindows10(17763) Then
        Dim attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1
        If IsWindows10(18985) Then
          attribute = DWMWA_USE_IMMERSIVE_DARK_MODE
        End If
        Dim useImmersiveDarkMode = If(enabled, 1, 0)
        Return DwmSetWindowAttribute(handle, attribute, useImmersiveDarkMode, 4) = 0
      End If
    End If
    Return False
  End Function

End Class
