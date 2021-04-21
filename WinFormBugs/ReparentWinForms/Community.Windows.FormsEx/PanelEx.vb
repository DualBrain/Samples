Option Explicit On
Option Strict On
Option Infer On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

Namespace Global.Community.Windows.FormsEx

  Public Class PanelEx
    Inherits Panel

    'UserControl overrides dispose to clean up the component list.
    <DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
      Try
        If disposing AndAlso components IsNot Nothing Then
          components.Dispose()
        End If
      Finally
        MyBase.Dispose(disposing)
      End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As IContainer

    <DebuggerStepThrough()>
    Private Sub InitializeComponent()
      components = New Container()
    End Sub

    Private m_borderStyle As BorderStyle = BorderStyle.FixedSingle
    Private m_borderColor As Color = SystemColors.WindowFrame
    Private m_borderWidth As Integer = 1

    Private WithEvents ContainedControl As Control

    Private m_magicScrollBar As Boolean
    Private WithEvents VScrollBarEx1 As VScrollBarEx

    Sub New(ctl As Control)

      ' This call is required by the Windows Form Designer.
      InitializeComponent()

      ctl.Parent.Controls.Remove(ctl)

      BackColor = Color.Transparent

      ContainedControl = ctl
      If TypeOf ctl Is ListBox Then
        CType(ctl, ListBox).BorderStyle = BorderStyle.None
        m_magicScrollBar = True
      ElseIf TypeOf ctl Is RichTextBox Then
        CType(ctl, RichTextBox).BorderStyle = BorderStyle.None
        m_magicScrollBar = True
      ElseIf TypeOf ctl Is TextBoxBase Then
        CType(ctl, TextBoxBase).BorderStyle = BorderStyle.None
      End If

      If m_magicScrollBar Then
        VScrollBarEx1 = New VScrollBarEx
        VScrollBarEx1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Right
        VScrollBarEx1.LargeChange = 1
        VScrollBarEx1.Size = New Size(19, Height - 2)
        VScrollBarEx1.Location = New Point(Width - VScrollBarEx1.Width - 1, 1)
        VScrollBarEx1.Maximum = 10
        VScrollBarEx1.Minimum = 0
        VScrollBarEx1.Name = "VScrollBarEx1"
        VScrollBarEx1.SmallChange = 1
        VScrollBarEx1.TabIndex = 1
        VScrollBarEx1.Value = 10
        VScrollBarEx1.Visible = True
        Controls.Add(VScrollBarEx1)
      End If

      Controls.Add(ctl)
      TabIndex = ctl.TabIndex
      Size = New Size(ContainedControl.Width, ContainedControl.Height)
      Location = New Point(ContainedControl.Left, ContainedControl.Top)
      Anchor = ContainedControl.Anchor
      Dock = ContainedControl.Dock
      ' Assumes a initial border of 1...
      ContainedControl.Location = New Point(1, 1)
      ContainedControl.Size = New Size(Size.Width - 2, Size.Height - 2)
      Padding = New Padding(1)
      ContainedControl.SendToBack()
      Name = "Bordered" & ContainedControl.Name
      ctl.TabIndex = 0

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
        'TODO: Handle "padding" based on border width?
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

  End Class

End Namespace