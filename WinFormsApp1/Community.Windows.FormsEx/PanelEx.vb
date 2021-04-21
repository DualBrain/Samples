Option Explicit On
Option Strict On
Option Infer On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

Namespace Global.Community.Windows.FormsEx

  <ToolboxBitmap(GetType(Panel))>
  Public Class PanelEx
    Inherits Panel

    Private m_borderStyle As BorderStyle = BorderStyle.FixedSingle
    Private m_borderColor As Color = SystemColors.WindowFrame
    Private m_borderWidth As Integer = 1

    Private WithEvents ContainedControl As Control = Nothing
    Private WithEvents VScrollBar1 As VScrollBarEx = Nothing
    Private m_vScrollBarVisible As Boolean = False

    Public Property VScrollBarVisible As Boolean
      Get
        Return m_vScrollBarVisible
      End Get
      Set(value As Boolean)
        m_vScrollBarVisible = value
      End Set
    End Property

    Public Sub New()
      'MyBase.New()

      ' This call is required by the Windows Form Designer.
      InitializeComponent()

      VScrollBar1 = New VScrollBarEx

      SuspendLayout()

      ' VScrollBar1
      VScrollBar1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Right
      VScrollBar1.LargeChange = 1
      VScrollBar1.Location = New Point(145, 0)
      VScrollBar1.Maximum = 10
      VScrollBar1.Minimum = 0
      VScrollBar1.MinimumSize = New Size(19, 15)
      VScrollBar1.Name = "VScrollBar1"
      VScrollBar1.Size = New Size(19, 127)
      VScrollBar1.SmallChange = 1
      VScrollBar1.TabIndex = 0
      VScrollBar1.Value = 10
      VScrollBar1.Visible = m_vScrollBarVisible

      ' Me
      MyBase.BackColor = Color.Transparent
      Controls.Add(VScrollBar1)
      BorderStyle = BorderStyle.FixedSingle

      ResumeLayout(False)

    End Sub

    Public Sub New(ctl As Control)
      Me.New()

      'ContainedControl = ctrl
      'Controls.Add(ctrl)
      '' Fix the fake scrolling control to overlap the real scrollable control
      'VScrollBar1.Size = New Size(18, ContainedControl.Height)
      'Size = New Size(ContainedControl.Width, ContainedControl.Height)
      'Location = New Point(ContainedControl.Left, ContainedControl.Top)
      'Dock = ContainedControl.Dock
      'ContainedControl.Top = 0 : ContainedControl.Left = 0
      'ContainedControl.SendToBack()
      'Name = "Skinned" + ContainedControl.Name

      ContainedControl = ctl
      Controls.Add(ctl)
      ' Fix the fake scrolling control to overlap the real scrollable control
      'VScrollBar1.Size = New Size(18, ScrollableControl.Height)
      Size = New Size(ContainedControl.Width, ContainedControl.Height)
      Location = New Point(ContainedControl.Left, ContainedControl.Top)
      Anchor = ContainedControl.Anchor
      Dock = ContainedControl.Dock
      ContainedControl.Location = New Point(1, 1)
      ContainedControl.Size = New Size(Size.Width - 2, Size.Height - 2)
      'ContainedControl.Top = 5 : ContainedControl.Left = 5
      Padding = New Padding(1)
      ContainedControl.SendToBack()
      Name = "Bordered" & ContainedControl.Name

    End Sub

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
      Try
        If disposing AndAlso components IsNot Nothing Then
          components.Dispose()
        End If
      Finally
        MyBase.Dispose(disposing)
      End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
      components = New System.ComponentModel.Container()
    End Sub


    <DefaultValue(GetType(BorderStyle), "FixedSingle"),
     Category("Appearance"),
     Description("The border style used to paint the control.")>
    Public Shadows Property BorderStyle() As BorderStyle
      Get
        Return m_borderStyle
      End Get
      Set(value As BorderStyle)
        m_borderStyle = value
        If DesignMode Then Invalidate()
      End Set
    End Property

    <DefaultValue(GetType(Color), "WindowFrame"),
     Category("Appearance"),
     Description("The border color used to paint the control.")>
    Public Property BorderColor() As Color
      Get
        Return m_borderColor
      End Get
      Set(value As Color)
        m_borderColor = value
        Invalidate()
      End Set
    End Property

    <DefaultValue(GetType(Integer), "1"),
     Category("Appearance"),
     Description("The width of the border used to paint the control.")>
    Public Property BorderWidth() As Integer
      Get
        Return m_borderWidth
      End Get
      Set(value As Integer)
        m_borderWidth = value
        If DesignMode Then Invalidate()
      End Set
    End Property

    Protected Overrides Sub OnPaintBackGround(e As PaintEventArgs)

      MyBase.OnPaintBackground(e)

      e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

      Select Case Me.m_borderStyle
        Case BorderStyle.FixedSingle
          Using borderPen = New Pen(m_borderColor, m_borderWidth)
            If m_borderWidth > 1 Then
              Dim path = New GraphicsPath
              Try
                Dim offset = If(m_borderStyle = BorderStyle.FixedSingle AndAlso
                              m_borderWidth > 1, BorderWidth \ 2, 0)
                path.AddRectangle(Rectangle.Inflate(ClientRectangle, -offset, -offset))
                e.Graphics.DrawPath(borderPen, path)
              Finally
                path.Dispose()
              End Try
            Else
              'path.AddRectangle(ClientRectangle)
              Dim rect = ClientRectangle
              rect.Width -= 1 : rect.Height -= 1
              e.Graphics.DrawRectangle(borderPen, rect)
            End If
          End Using

        Case BorderStyle.Fixed3D
          e.Graphics.SmoothingMode = SmoothingMode.Default
          e.Graphics.DrawLine(SystemPens.ControlDark, ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Y)
          e.Graphics.DrawLine(SystemPens.ControlDark, ClientRectangle.X, ClientRectangle.Y, ClientRectangle.X, ClientRectangle.Height - 1)
          e.Graphics.DrawLine(SystemPens.ControlDarkDark, ClientRectangle.X + 1, ClientRectangle.Y + 1, ClientRectangle.Width - 1, ClientRectangle.Y + 1)
          e.Graphics.DrawLine(SystemPens.ControlDarkDark, ClientRectangle.X + 1, ClientRectangle.Y + 1, ClientRectangle.X + 1, ClientRectangle.Height - 1)
          e.Graphics.DrawLine(SystemPens.ControlLight, ClientRectangle.X + 1, ClientRectangle.Height - 2, ClientRectangle.Width - 2, ClientRectangle.Height - 2)
          e.Graphics.DrawLine(SystemPens.ControlLight, ClientRectangle.Width - 2, ClientRectangle.Y + 1, ClientRectangle.Width - 2, ClientRectangle.Height - 2)
          e.Graphics.DrawLine(SystemPens.ControlLightLight, ClientRectangle.X, ClientRectangle.Height - 1, ClientRectangle.Width - 1, ClientRectangle.Height - 1)
          e.Graphics.DrawLine(SystemPens.ControlLightLight, ClientRectangle.Width - 1, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1)

        Case BorderStyle.None

      End Select

    End Sub

#Region "ScrollBar"

#Region "Interop"

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

#End Region

    Protected Overrides Sub Wndproc(ByRef m As Message)
      MyBase.WndProc(m)
      If DesignMode Then Return
      If Not Parent.CanFocus OrElse IsNothing(ContainedControl) Then Return
      si.fMask = SIF_ALL
      si.cbSize = Runtime.InteropServices.Marshal.SizeOf(si)
      Dim result = GetScrollInfo(ContainedControl.Handle, 1, si)
      If si.nMax < si.nPage Then
        VScrollBar1.Visible = False
      Else
        VScrollBar1.Visible = m_vScrollBarVisible 'True
        If si.nPage = 0 Then Return
        VScrollBar1.Maximum = si.nMax - si.nPage + 1
        VScrollBar1.Minimum = si.nMin
        VScrollBar1.Value = si.nPos
        VScrollBar1.LargeChange = CInt(si.nMax / si.nPage)
        VScrollBar1.SyncThumbPositionWithLogicalValue()
      End If
    End Sub

    Private Sub VScrollBar1_OurScroll(sender As Object, e As ScrollEventArgs) Handles VScrollBar1.OurScroll
      PostMessageA(ContainedControl.Handle, WM_VSCROLL, SB_THUMBPOSITION + 65536 * VScrollBar1.Value, 0)
      'previously explored:
      'SendMessage(_win.Handle, WM_VSCROLL, e.Type, 0)
    End Sub

    Private Sub ScrollSkin_ControlAdded(sender As Object, e As ControlEventArgs) Handles Me.ControlAdded
      If Controls.Count = 1 Then Exit Sub
      If Not IsNothing(ContainedControl) Then Exit Sub
      ContainedControl = e.Control
    End Sub

    Private Sub ScrollableControl_Resize(sender As Object, e As EventArgs) Handles ContainedControl.Resize
      VScrollBar1.Size = New Size(18, ContainedControl.Height)
      Size = New Size(ContainedControl.Width, ContainedControl.Height)
      VScrollBar1.Left = ContainedControl.Right - 18
      ContainedControl.Top = 0 : ContainedControl.Left = 0
    End Sub

#End Region

  End Class

End Namespace