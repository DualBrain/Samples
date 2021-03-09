Option Explicit On
Option Strict On
Option Infer On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

<ToolboxBitmap(GetType(Panel))>
Public Class PanelEx
  Inherits Panel

  Private m_borderStyle As BorderStyle = BorderStyle.None
  Private m_borderColor As Color = SystemColors.WindowFrame
  Private m_borderWidth As Integer = 1

  Public Sub New()
    MyBase.New()
    SetDefaultControlStyles()
    CustomInitialisation()
  End Sub

  Private Sub SetDefaultControlStyles()
    SetStyle(ControlStyles.DoubleBuffer, True)
    SetStyle(ControlStyles.AllPaintingInWmPaint, False)
    SetStyle(ControlStyles.ResizeRedraw, True)
    SetStyle(ControlStyles.UserPaint, True)
    SetStyle(ControlStyles.SupportsTransparentBackColor, True)
  End Sub

  Private Sub CustomInitialisation()
    SuspendLayout()
    MyBase.BackColor = Color.Transparent
    BorderStyle = BorderStyle.None
    ResumeLayout(False)
  End Sub

  <DefaultValue(GetType(BorderStyle), "None"),
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
      If DesignMode Then Invalidate()
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
          If m_borderStyle = BorderStyle.FixedSingle AndAlso
             m_borderWidth > 1 Then
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
            rect.Width -= 1
            rect.Height -= 1
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