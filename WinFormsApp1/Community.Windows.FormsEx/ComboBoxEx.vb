Option Explicit On
Option Infer On
Option Strict On

Imports System.ComponentModel

Namespace Global.Community.Windows.FormsEx

  Public Class ComboBoxEx
    Inherits ComboBox

    Private Const WM_PAINT As Integer = &HF
    Private Const WM_NCPAINT As Integer = &H85
    Private Const WM_SYNCPAINT As Integer = &H88

    'Private ReadOnly m_buttonWidth As Integer = SystemInformation.HorizontalScrollBarArrowWidth
    Private m_BorderColor As Color = Color.Red
    'Private m_FadedBorderColor As Color = Color.Red
    Private m_BorderDrawMode As ControlBorderDrawMode = ControlBorderDrawMode.Full
    'Private m_BorderDrawArea As Rectangle = Rectangle.Empty

    Public Enum ControlBorderDrawMode As Integer
      Full = 0
      Internal
      InternalFaded
    End Enum

    Public Sub New()
      InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
      SetStyle(ControlStyles.ResizeRedraw Or
             ControlStyles.SupportsTransparentBackColor, True)
      DrawMode = DrawMode.Normal
      BorderColor = SystemColors.WindowFrame
    End Sub

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
      SetBorderArea()
      MyBase.OnHandleCreated(e)
    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)
      MyBase.WndProc(m)
      If m.Msg = WM_PAINT Or m.Msg = WM_SYNCPAINT Or m.Msg = WM_NCPAINT Then
        Using g As Graphics = Graphics.FromHwnd(Handle)

          'SetBorderArea()
          ''Using pen1 As New Pen(If(Me.m_BorderDrawMode = ControlBorderDrawMode.InternalFaded, m_FadedBorderColor, m_BorderColor), 1)
          'Dim pen1 = Pens.Purple
          'If Me.BorderDrawMode = ControlBorderDrawMode.Full Then
          '    g.DrawLine(pen1, ClientRectangle.Width - m_buttonWidth, 0,
          '                    ClientRectangle.Width - m_buttonWidth, ClientRectangle.Height - 1)
          '  End If
          'g.DrawRectangle(pen1, m_BorderDrawArea)

          Using p = New Pen(m_BorderColor, 1)

            Dim rect = ClientRectangle
            rect.Width -= 1
            rect.Height -= 1
            g.DrawRectangle(p, rect)

            Dim bRect = New Rectangle(rect.Left + rect.Width - 17, rect.Top, 17, rect.Height)
            Using b = New SolidBrush(BackColor)
              g.FillRectangle(b, bRect)
            End Using
            g.DrawRectangle(p, bRect)

            Dim x1 = bRect.Left + 7
            Dim x2 = x1 + 4
            Dim y = bRect.Top + 10
            g.DrawLine(If(BackColor = Color.White, Pens.Black, p), x1, y, x2, y)
            x1 += 1 : x2 -= 1 : y += 1
            g.DrawLine(If(BackColor = Color.White, Pens.Black, p), x1, y, x2, y)
            x1 += 1 : x2 -= 1 : y += 1
            g.DrawLine(If(BackColor = Color.White, Pens.Black, p), x1, y - 1, x2, y)

          End Using

          'rect.Location = New Point(rect.Left + 1, rect.Top + 1)
          'rect.Width -= 2
          'rect.Height -= 2
          'g.DrawRectangle(Pens.Purple, rect)

          'End Using
        End Using
        m.Result = IntPtr.Zero
      End If
    End Sub

    Private Sub SetBorderArea()
      Select Case m_BorderDrawMode
        Case ControlBorderDrawMode.Full
          'm_BorderDrawArea = New Rectangle(Point.Empty,
          '                               New Size(ClientRectangle.Width - 1, ClientRectangle.Height - 1))
        Case ControlBorderDrawMode.Internal
          'm_BorderDrawArea = New Rectangle(Point.Empty,
          '                               New Size(ClientRectangle.Width - m_buttonWidth, ClientRectangle.Height - 1))
        Case ControlBorderDrawMode.InternalFaded
          'm_FadedBorderColor = Color.FromArgb(96, m_BorderColor)
          'm_BorderDrawArea = New Rectangle(New Point(0, 0),
          '                               New Size(ClientRectangle.Width - m_buttonWidth - 2, ClientRectangle.Height - 2))
      End Select
    End Sub

    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    <EditorBrowsable(EditorBrowsableState.Always), Category("Appearance")>
    <Description("Determines how the colored border is drawn. 
Full: specifies the control's client area and the DropDown Button
Internal: the intenal section of the control, excluding the DropDown Button.")>
    Public Property BorderDrawMode As ControlBorderDrawMode
      Get
        Return m_BorderDrawMode
      End Get
      Set(Value As ControlBorderDrawMode)
        m_BorderDrawMode = Value
        Invalidate()
      End Set
    End Property

    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    <EditorBrowsable(EditorBrowsableState.Always), Category("Appearance")>
    <Description("Get or Set the Color of the Control's border")>
    Public Property BorderColor As Color
      Get
        Return m_BorderColor
      End Get
      Set(Value As Color)
        m_BorderColor = Value
        Invalidate()
      End Set
    End Property

  End Class

End Namespace