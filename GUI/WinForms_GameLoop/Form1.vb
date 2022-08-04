Option Explicit On
Option Strict On
Option Infer On

' https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/how-to-manually-manage-buffered-graphics
' https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/how-to-manually-render-buffered-graphics

Public Class Form1

  Private m_rectangle As New Rectangle()
  Private ReadOnly m_rectangleBrush As New SolidBrush(System.Drawing.Color.Black)
  Private ReadOnly m_colorBrush As New SolidBrush(System.Drawing.Color.Aqua)
  Private ReadOnly m_random As New Random
  Private ReadOnly m_context As New BufferedGraphicsContext
  Private mybuff As BufferedGraphics

  Private ReadOnly m_stopwatch As New Stopwatch

  Private m_shutdown As Boolean
  Private m_shutdownGood As Boolean

  Private m_gscr As QbScreen

  'Dim mousepos As New Point

  Private Async Sub Me_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    Cursor.Hide()

    ' adjust the form to be full screen
    WindowState = FormWindowState.Normal
    BackColor = Color.Black
    FormBorderStyle = Windows.Forms.FormBorderStyle.None
    Left = Screen.AllScreens(0).Bounds.Left
    Top = Screen.AllScreens(0).Bounds.Top
    Width = Screen.AllScreens(0).Bounds.Width 'twidth
    Height = SystemInformation.VirtualScreen.Height
    BringToFront()
    TopMost = True

    m_gscr = New QbScreen(Width, Height, 1.0)

    Await BootStrapAsync()

  End Sub

  Private Async Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
    m_shutdown = True
    Do
      Await Task.Delay(1)
      If m_shutdownGood Then
        Exit Do
      End If
    Loop
  End Sub

  Private mode As Integer = 1

  Private Async Function BootStrapAsync() As Task
    'mousepos = Cursor.HotSpot

    ' Create drawing rectangle...
    m_rectangle.Width = PictureBox1.Width
    m_rectangle.Height = PictureBox1.Height

    mybuff = m_context.Allocate(PictureBox1.CreateGraphics, m_rectangle)

    ' Set some optimization drawing "stuff"...
    mybuff.Graphics.CompositingMode = Drawing2D.CompositingMode.SourceOver
    mybuff.Graphics.CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
    mybuff.Graphics.SmoothingMode = Drawing2D.SmoothingMode.None
    mybuff.Graphics.InterpolationMode = CType(Drawing2D.QualityMode.Low, Drawing2D.InterpolationMode)
    mybuff.Graphics.PixelOffsetMode = Drawing2D.PixelOffsetMode.None

    ' Create a "black" brush...
    m_rectangleBrush.Color = System.Drawing.Color.Black

    Dim sx = PictureBox1.Width
    Dim sy = PictureBox1.Height

    Randomize()
    Dim rnd = New Random(0)

    Dim start As Double = 0
    Dim [end] As Double = Math.PI * 2
    Dim aspect = 4.0 * (sy / sx) / 3.0

    Do

      m_stopwatch.Reset()
      m_stopwatch.Start()

      m_gscr.Cls()
      For i = 1 To 10
        Dim x1 = rnd.Next(sx)
        Dim y1 = rnd.Next(sy)
        Dim x2 = rnd.Next(sx)
        Dim y2 = rnd.Next(sy)
        Dim radius = rnd.Next(50)
        Dim r = rnd.Next(255)
        Dim g = rnd.Next(255)
        Dim b = rnd.Next(255)
        Dim a = 255
        Dim c = Drawing.Color.FromArgb(a, r, g, b)
        Select Case mode
          Case 1 : m_gscr.Line(x1, y1, x2, y2, c, True, False, Nothing)
          Case 2 : m_gscr.Circle(x1, y1, radius, c, start, [end], aspect)
          Case 3 : m_gscr.Pset(x1, y1, c)
          Case Else
        End Select
      Next

      m_gscr.Render(mybuff, PictureBox1)

      m_stopwatch.Stop()
      Label1.Text = $"frame @ {1000 \ m_stopwatch.ElapsedMilliseconds} FPS"

      'Application.DoEvents() ' This is bad, but it's quick and easy to use...
      Await Task.Delay(1) ' So, instead, let's take advantage of a "state machine" approach to the same thing.

      If m_shutdown Then Exit Do

    Loop
    mybuff.Dispose()
    m_shutdownGood = True
    Close()

  End Function

  Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

    If e.KeyCode = Keys.Escape Then
      m_shutdown = True
    ElseIf e.KeyCode = Keys.Space Then
      mode += 1
      If mode > 3 Then mode = 1
    End If

  End Sub

  'Private Sub Clear()
  '  mybuff.Graphics.FillRectangle(m_rectangleBrush, m_rectangle)
  'End Sub

  'Private Sub Draw()

  '  'Dim colorTest = False ' creating and destroying solid brushes is very inefficient
  '  'Using brush = New SolidBrush(Drawing.Color.Blue) ' by creating a brush outside of the loop and changing the color, the impact is noticably less.
  '  ' Note: the Using brush statement here and the associated End Using doesn't noticably to impact performance.
  '  For f = 1 To 20000
  '    'If colorTest Then
  '    '  Brush.Color = Color.FromArgb(255, m_random.Next(256), m_random.Next(256), m_random.Next(256))
  '    '  'Dim c = Color.FromArgb(255, m_random.Next(256), m_random.Next(256), m_random.Next(256))
  '    '  'Using brush = New SolidBrush(c)
  '    '  mybuff.Graphics.FillRectangle(Brush, (PictureBox1.Width >> 1) + m_random.Next(-450, 451), (PictureBox1.Height >> 1) + m_random.Next(-450, 451), 4, 4)
  '    '  'End Using
  '    'Else
  '    ' The following line impacts performance to 250fps, remarked out... 333fps.
  '    m_colorBrush.Color = Color.FromArgb(255, m_random.Next(256), m_random.Next(256), m_random.Next(256))
  '    mybuff.Graphics.FillRectangle(m_colorBrush, (PictureBox1.Width >> 1) + m_random.Next(-450, 451), (PictureBox1.Height >> 1) + m_random.Next(-450, 451), 4, 4)
  '    'End If
  '  Next
  '  'End Using

  'End Sub

  'Private Sub Render()
  '  mybuff.Render(PictureBox1.CreateGraphics)
  'End Sub

End Class