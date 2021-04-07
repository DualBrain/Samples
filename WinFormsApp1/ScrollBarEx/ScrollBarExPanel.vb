Option Explicit On
Option Strict On
Option Infer On

Namespace ScrollBarEx

  Public Class ScrollBarExPanel

    Private WithEvents ScrollableControl As Control = Nothing

    'Private Const WM_HSCROLL As Integer = 276
    Private Const WM_VSCROLL As Integer = 277
    Private Const SB_THUMBPOSITION As Integer = 4
    Private Const SIF_TRACKPOS As Integer = 16
    Private Const SIF_RANGE As Integer = 1
    Private Const SIF_POS As Integer = 4
    Private Const SIF_PAGE As Integer = 2
    Private Const SIF_ALL As Integer = SIF_RANGE Or SIF_PAGE Or SIF_POS Or SIF_TRACKPOS

    Private Declare Function SetScrollInfo Lib "user32" (hwnd As Integer, n As Integer, ByRef lpcScrollInfo As ScrollInfoStruct, bool As Boolean) As Integer

    <Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function SetScrollPos(hWnd As IntPtr, nBar As Integer, nPos As Integer, bRedraw As Boolean) As Integer
    End Function

    <Runtime.InteropServices.DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetScrollInfo(hWnd As IntPtr, n As Integer, ByRef lpScrollInfo As ScrollInfoStruct) As Integer
    End Function

    <Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function GetScrollPos(hWnd As IntPtr, nBar As Integer) As Integer
    End Function

    <Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function PostMessageA(hWnd As IntPtr,
                                      wMsg As Integer,
                                      wParam As Integer,
                                      lParam As Integer) As Boolean
    End Function

    <Runtime.InteropServices.DllImport("user32.DLL", EntryPoint:="SendMessage")>
    Private Shared Sub SendMessage(hWnd As IntPtr,
                                 wMsg As Integer,
                                 wParam As Integer,
                                 lParam As Integer)
    End Sub

    Public si As ScrollInfoStruct
    Public si2 As ScrollInfoStruct

    Public Structure ScrollInfoStruct
      Public cbSize As Integer
      Public fMask As Integer
      Public nMin As Integer
      Public nMax As Integer
      Public nPage As Integer
      Public nPos As Integer
      Public nTrackPos As Integer
    End Structure

    ''' <summary>
    ''' Coded manually lets you to use this New(control)
    ''' </summary>
    ''' <param name="ctl"></param>
    ''' <remarks></remarks>
    Sub New(ctl As Control)
      ' This call is required by the Windows Form Designer.
      InitializeComponent()
      ScrollableControl = ctl
      Controls.Add(ctl)
      ' Fix the fake scrolling control to overlap the real scrollable control
      VScrollBar1.Size = New Size(18, ScrollableControl.Height)
      Size = New Size(ScrollableControl.Width, ScrollableControl.Height)
      Location = New Point(ScrollableControl.Left, ScrollableControl.Top)
      Dock = ScrollableControl.Dock
      ScrollableControl.Top = 0 : ScrollableControl.Left = 0
      ScrollableControl.SendToBack()
      Name = "Skinned" + ScrollableControl.Name
    End Sub

    ''' <summary>
    ''' Overrided to controll del scrolling of the customControl VScrollBar1
    ''' </summary>
    ''' <param name="m"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub Wndproc(ByRef m As Message)
      MyBase.WndProc(m)
      If DesignMode Then Return
      If Not Parent.CanFocus OrElse IsNothing(ScrollableControl) Then Return
      si.fMask = SIF_ALL
      si.cbSize = Runtime.InteropServices.Marshal.SizeOf(si)
      Dim result = GetScrollInfo(ScrollableControl.Handle, 1, si)
      If si.nMax < si.nPage Then
        VScrollBar1.Visible = False
      Else
        VScrollBar1.Visible = True
        If si.nPage = 0 Then Return
        VScrollBar1.Maximum = si.nMax - si.nPage + 1
        VScrollBar1.Minimum = si.nMin
        VScrollBar1.Value = si.nPos
        VScrollBar1.LargeChange = CInt(si.nMax / si.nPage)
        VScrollBar1.SyncThumbPositionWithLogicalValue()
      End If
    End Sub

    ''' <summary>
    ''' Comming from the customControl
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub VScrollBar1_OurScroll(sender As Object, e As ScrollEventArgs) Handles VScrollBar1.OurScroll
      PostMessageA(ScrollableControl.Handle, WM_VSCROLL, SB_THUMBPOSITION + 65536 * VScrollBar1.Value, 0)
      'previously explored:
      'SendMessage(_win.Handle, WM_VSCROLL, e.Type, 0)
    End Sub

    ''' <summary>
    ''' Linking the Scrollable control with Me
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ScrollSkin_ControlAdded(sender As Object, e As ControlEventArgs) Handles Me.ControlAdded
      If Controls.Count = 1 Then Exit Sub
      If Not IsNothing(ScrollableControl) Then Exit Sub
      ScrollableControl = e.Control

    End Sub

    ''' <summary>
    ''' Almost done move and resize the Scrollable control over Me 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Win_Resize(sender As Object, e As EventArgs) Handles ScrollableControl.Resize
      VScrollBar1.Size = New Size(18, ScrollableControl.Height)
      Size = New Size(ScrollableControl.Width, ScrollableControl.Height)
      VScrollBar1.Left = ScrollableControl.Right - 18
      ScrollableControl.Top = 0 : ScrollableControl.Left = 0
    End Sub

  End Class

End Namespace