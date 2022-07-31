Public Class Form1

  Private ReadOnly Timer As New Stopwatch()
  'Private OtherTimer As New Threading.Timer(Sub() Invalidate(ClientRectangle), Nothing, 1000, 15)

  Private Const HorizontalMargin = 20
  Private Const VerticalMargin = 20

  Public ReadOnly Property LeftEdge As Integer = HorizontalMargin
  Public ReadOnly Property TopEdge As Integer = VerticalMargin
  Public ReadOnly Property RightEdge As Integer = ClientSize.Width - HorizontalMargin
  Public ReadOnly Property BottomEdge As Integer = ClientSize.Height - VerticalMargin

  Private m_font = New Font(Me.Font.FontFamily, 14.0, FontStyle.Bold)

  Sub New()

    ' This call is required by the designer.
    InitializeComponent()

    ' Add any initialization after the InitializeComponent() call.
    Me.SetStyle(ControlStyles.Opaque Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
    Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, False)
    Me.UpdateStyles()

  End Sub

  Protected Overrides Sub OnPaint(e As PaintEventArgs)

    Dim elapsed = Timer.ElapsedMilliseconds

    Dim value = 16 '50 '16
    If elapsed >= value Then
      Execute(e.Graphics, elapsed / 1000)
      Timer.Restart()
    ElseIf Not Timer.IsRunning Then
      Execute(e.Graphics, elapsed / 1000)
      Timer.Start()
    End If

    Invalidate(ClientRectangle)

  End Sub

  Private m_count As Integer
  Private ReadOnly m_rnd As New Random(Microsoft.VisualBasic.Timer)
  Private m_doneOnce As Boolean

  Private Class Invader

    Public Property Text As String

    Public Property X As Integer
    Public Property Y As Integer
    Public Property T As Integer

    Public Property Hit As Boolean

    Public Function Process(buffer As Graphics, font As Font) As Boolean
      If Hit Then
        Return False
      Else
        Dim s = buffer.MeasureString(Text, font)
        Dim h = s.Height
        If Y + h > buffer.VisibleClipBounds.Height Then
          Return False
        Else
          Y += 1 + T
          buffer.DrawString(Text, font, Brushes.White, X, Y)
          Return True
        End If
      End If
    End Function

  End Class

  Private ReadOnly m_invaders As New List(Of Invader)
  Private m_hits As Integer
  Private m_misses As Integer
  Private m_failed As Integer

  Public Overridable Sub Execute(buffer As Graphics, elapsedSeconds As Single)

    m_count += 1

    If m_count Mod 25 = 0 Then
      If m_invaders.Count < 6 Then
        Dim n = m_rnd.Next(2)
        For i = 1 To 1 + n
          ' Create a new invader.
          Dim y = m_rnd.Next(20)
          Dim x = m_rnd.Next(ClientSize.Width - 20)
          Dim t = m_rnd.Next(3)
          Dim text = Chr(65 + m_rnd.Next(25))
          m_invaders.Add(New Invader With {.Text = text, .X = x, .Y = y, .T = 1 + t})
        Next
      End If
    End If

    ' Clear the surface area
    'If Not m_doneOnce Then
    buffer.FillRectangle(Brushes.Black, 0, 0, ClientSize.Width, ClientSize.Height)
    '  m_doneOnce = True
    'End If

    'For index = 0 To m_invaders.Count - 1
    '  m_invaders(index).Clear(buffer, m_font)
    'Next

    Dim remove As New List(Of Integer)

    For index = 0 To m_invaders.Count - 1
      If Not m_invaders(index).Process(buffer, m_font) Then
        ' Need to remove.
        remove.Add(index)
      End If
    Next

    For Each entry In remove
      If Not m_invaders(entry).Hit Then
        ' Got past us...
        m_failed += 1
      End If
      m_invaders.RemoveAt(entry)
    Next

    Me.Text = $"Typing Invaders : Hits = {m_hits}, Misses = {m_misses}, Failed = {m_failed}"

  End Sub

  Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
    Return
  End Sub

  Protected Overrides Sub OnResize(e As EventArgs)
    MyBase.OnResize(e)
    _RightEdge = ClientSize.Width - HorizontalMargin
    _BottomEdge = ClientSize.Height - VerticalMargin
  End Sub

  Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
    If e.KeyCode = Keys.Escape Then
      Me.Close()
    End If
  End Sub

  Private Sub Form1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
    Dim letter = $"{e.KeyChar}".ToUpper
    Dim possibles = From p In m_invaders Where p.Text = letter Order By p.Y Descending
    If possibles.Any Then
      possibles(0).Hit = True
      m_hits += 1
    Else
      m_misses += 1
    End If
  End Sub

End Class