Option Explicit On
Option Strict On
Option Infer On

' https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/how-to-manually-manage-buffered-graphics
' https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/how-to-manually-render-buffered-graphics

Imports QB.Video

Public Class Form1

  'Private m_rectangle As New Rectangle()
  'Private ReadOnly m_rectangleBrush As New SolidBrush(System.Drawing.Color.Black)
  'Private ReadOnly m_colorBrush As New SolidBrush(System.Drawing.Color.Aqua)
  Private ReadOnly m_random As New Random
  'Private ReadOnly m_context As New BufferedGraphicsContext
  'Private ReadOnly mybuff As BufferedGraphics
  'Private ReadOnly numberOfMonitors As Integer = 1 'Screen.AllScreens.Length
  'Private ReadOnly picBox(0) As PictureBox 'numberOfMonitors) As PictureBox
  'Private m_screenNumber As Integer = 0
  ' Dim colpen As New Pen(colbrush.Color)
  ' Dim mousepos As New Point
  Private ReadOnly m_stopwatch As New Stopwatch

  'Private m_shutdown As Boolean
  Private ReadOnly m_shutdownGood As Boolean

  Private Async Sub Me_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    Init(Me, PictureBox1, True)

    If False Then

      Await Task.Delay(100) ' HACK: Let WinForms "do it's thing..." for 100ms

      Screen(9)
      Color(14)

      LOCATE(1, 1)
      Print("Hello world!")

      CIRCLE(200, 200, 50)
      Color(10)
      QB.Video.PAINT(200, 200)

      LINE(50, 50, 100, 100, 9, QB.LineOption.BF)

      LOCATE(2, 1) : Print($"{Point(51, 51)}")

      Render() ' Draw scene...
    Else
      Label1.Visible = True
      Screen(9)
      Timer1.Enabled = True
    End If

    'Configdisplay()
    'Await BootStrapAsync()

  End Sub

  Private Async Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
    'm_shutdown = True
    Do
      Await Task.Delay(1)
      If m_shutdownGood Then
        Exit Do
      End If
    Loop
  End Sub

  'Private Shared Sub ConfigDisplay()

  '  'find the number of displays and create/configure a picturebox for each

  '  '   Cursor.Hide()
  '  'Dim twidth = 0
  '  'Dim theight = SystemInformation.VirtualScreen.Height
  '  'Dim minheight = 999999
  '  'Dim minwidth = 999999

  '  'find smallest screen display and work out the size of the picture boxes
  '  'For f = 0 To numberOfMonitors - 1
  '  'Dim f = 0
  '  'If (Screen.AllScreens(f).Bounds.Width < minwidth) AndAlso (Screen.AllScreens(f).Bounds.Height < minheight) Then
  '  '  m_screenNumber = f 'use this (smallest display) as main drawing surface.
  '  '  minheight = Screen.AllScreens(f).Bounds.Height
  '  '  minwidth = Screen.AllScreens(f).Bounds.Width
  '  'End If
  '  'Next

  '  ' generate a picturebox for each display
  '  'For f = 0 To numberOfMonitors - 1
  '  'twidth += Screen.AllScreens(f).Bounds.Width
  '  'picBox(f) = New PictureBox With {.Width = minwidth,
  '  '                                 .Height = minheight,
  '  '                                 .Parent = Me}
  '  'Next

  '  ' adjust the form
  '  'WindowState = FormWindowState.Normal
  '  'BackColor = Color.Black
  '  'FormBorderStyle = Windows.Forms.FormBorderStyle.None
  '  'Left = Screen.AllScreens(0).Bounds.Left
  '  'Top = Screen.AllScreens(0).Bounds.Top
  '  'Width = twidth
  '  'Height = SystemInformation.VirtualScreen.Height
  '  'BringToFront()
  '  'TopMost = True

  '  ' position the pictureboxes
  '  'twidth = 0
  '  'For f = 0 To numberOfMonitors - 1
  '  '  picBox(f).Left = twidth
  '  '  If Screen.AllScreens(f).Bounds.Width > minwidth Then
  '  '    picBox(f).Left = CInt(picBox(f).Left + ((Screen.AllScreens(f).Bounds.Width - minwidth) / 2))
  '  '  End If
  '  '  If Screen.AllScreens(f).Bounds.Height > minheight Then
  '  '    picBox(f).Top = CInt((Screen.AllScreens(f).Bounds.Height - minheight) / 2)
  '  '  End If
  '  '  twidth += Screen.AllScreens(f).Bounds.Width
  '  '  Controls.Add(picBox(f))
  '  '  '   AddHandler picbox(f).MouseMove, AddressOf pb_mousemove 'add mouse move handler for each picbox
  '  '  '   AddHandler picbox(f).Click, AddressOf pb_Click 'add mouse click handler for each picbox
  '  'Next
  'End Sub

  'Private Async Function BootStrapAsync() As Task
  '  '  mousepos = Cursor.HotSpot
  '  ' Initialize a "drawing" surface.
  '  ' pb = New Bitmap(640, 480)
  '  ' Using g = System.Drawing.Graphics.FromImage(pb)
  '  '   g.FillRectangle(...)
  '  ' End Using
  '  ' In a separate section; in other words, not every single (individual) draw...
  '  ' PictureBox1.Image = pb

  '  ' Create drawing rectangle...
  '  m_rectangle.Width = PictureBox1.Width
  '  m_rectangle.Height = PictureBox1.Height
  '  mybuff = m_context.Allocate(PictureBox1.CreateGraphics, m_rectangle)
  '  ' Set some optimization drawing "stuff"...
  '  mybuff.Graphics.CompositingMode = Drawing2D.CompositingMode.SourceOver
  '  mybuff.Graphics.CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
  '  mybuff.Graphics.SmoothingMode = Drawing2D.SmoothingMode.None
  '  mybuff.Graphics.InterpolationMode = CType(Drawing2D.QualityMode.Low, Drawing2D.InterpolationMode)
  '  mybuff.Graphics.PixelOffsetMode = Drawing2D.PixelOffsetMode.None
  '  ' Create a "black" brush...
  '  m_rectangleBrush.Color = System.Drawing.Color.Black ' Color.FromArgb(255, 0, 0, 0)
  '  Do
  '    m_stopwatch.Start()
  '    ' Do drawing stuff.
  '    MainBit()
  '    m_stopwatch.Stop()
  '    Label1.Text = $"20,000 rectangles per frame @ {1000 \ m_stopwatch.ElapsedMilliseconds} FPS"
  '    m_stopwatch.Reset()
  '    'Application.DoEvents() ' This is bad, but it's quick and easy to use...
  '    Await Task.Delay(1) ' So, instead, let's take advantage of a "state machine" approach to the same thing.
  '    If m_shutdown Then
  '      Exit Do
  '    End If
  '  Loop
  '  mybuff.Dispose()
  '  m_shutdownGood = True
  '  'Close()
  'End Function

  'Private Sub MainBit()
  '  mybuff.Graphics.FillRectangle(m_rectangleBrush, m_rectangle)
  '  'draw stuff  eg...
  '  For f = 1 To 20000
  '    '   colbrush.Color = Color.FromArgb(255, rng.Next(256), rng.Next(256), rng.Next(256))
  '    mybuff.Graphics.FillRectangle(m_colorBrush, (PictureBox1.Width >> 1) + m_random.Next(-450, 451), (PictureBox1.Height >> 1) + m_random.Next(-450, 451), 4, 4)
  '  Next
  '  '...
  '  '...
  '  'render buffer to each picbox in turn (one picbox for each display)
  '  'For f = 0 To numberOfMonitors - 1
  '  mybuff.Render(PictureBox1.CreateGraphics)
  '  'Next
  'End Sub

  Private m_timerActive As Boolean

  'Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
  '  Render()
  '  Timer2.Enabled = False
  'End Sub

  Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

    If m_timerActive Then Return

    m_timerActive = True
    Try

      m_stopwatch.Start()

      Dim rectCount = 20000

      LINE(0, 0, 639, 349, 0, QB.LineOption.BF)
      'draw stuff  eg...
      For f = 1 To rectCount
        Dim x = (640 >> 1) + m_random.Next(-450, 451)
        Dim y = (350 >> 1) + m_random.Next(-450, 451)
        Dim a = m_random.Next(1, 8)
        LINE(x, y, x + 4, y + 4, a, QB.LineOption.BF)
      Next
      Render()

      m_stopwatch.Stop()
      Label1.Text = $"{rectCount} rectangles per frame @ {1000 \ m_stopwatch.ElapsedMilliseconds} FPS"
      m_stopwatch.Reset()

    Finally
      m_timerActive = False
    End Try

  End Sub

End Class