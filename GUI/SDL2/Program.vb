Imports System
Imports SDL2.SDL

Module Program

  ' Screen
  '   - Driver1: WinForms
  '   - Driver2: SDL2
  '   - Driver3: FNA-XNA
  '   - Driver4: Qt
  '   - Driver5: GTK
  '   - Driver6: OpenTK?
  '   - Driver7: OpenGL
  '   - Driver8: WPF
  '   - Driver9: Blazor/Canvas?
  '   - Driver10: Xamarin/MAUI
  '   - Driver11: WinUI3

  ' CLS
  ' CSRLIN
  ' LOCATE
  ' POS
  ' PRINT
  ' PRINT USING
  ' SPC
  ' TAB
  ' VIEW PRINT
  ' WIDTH (screen)
  ' WRITE

  ' CIRCLE
  ' COLOR
  ' DRAW
  ' GET
  ' LINE
  ' PAINT
  ' PALETTE
  ' PALETTE USING
  ' PCOPY
  ' PMAP
  ' POINT
  ' PRESET
  ' PSET
  ' PUT
  ' SCREEN (function)
  ' SCREEN (statement)
  ' VIEW
  ' WINDOW

  Sub Main() 'args As String())

    Dim sx = 160
    Dim sy = 100

    Dim screen As New Screen(sx, sy, 4)

    Randomize()
    Dim rnd = New Random(CInt(Timer))

    Dim start As Double = 0
    Dim [end] As Double = Math.PI * 2
    Dim aspect = 4.0 * (sy / sx) / 3.0

    Try
      Do
        screen.Cls()
        'Dim max = sx * sy
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
          'screen.Pset(x1, y1, c)
          screen.Line(x1, y1, x2, y2, c, True, False, Nothing)
          'screen.Circle(x1, y1, radius, c, start, [end], aspect)
        Next
        screen.Render()
        Select Case screen.Input()
          Case ConsoleKey.Escape
            Exit Do
          Case Else
        End Select
      Loop
    Finally
      screen.Close()
    End Try

  End Sub

  'Private Sub Line(screen As Screen, x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer, c As Drawing.Color)

  '  Dim dx = x2 - x1
  '  Dim dy = y2 - y1

  '  Dim length = Math.Sqrt(dx * dx + dy * dy)
  '  Dim angle = Math.Atan2(dy, dx)
  '  Dim cx = Math.Cos(angle)
  '  Dim sx = Math.Sin(angle)
  '  For i = 0 To length - 1
  '    screen.Pset(x1 + CInt(Fix(cx * i)), y1 + CInt(Fix(sx * i)), c)
  '  Next

  'End Sub

End Module

Class Screen

  Private m_evt As SDL2.SDL.SDL_Event
  Private ReadOnly m_window As IntPtr
  Private ReadOnly m_renderer As IntPtr

  Private ReadOnly m_points As New List(Of (X As Integer, Y As Integer, C As System.Drawing.Color))

  Private m_cx As Integer = 0
  Private m_cy As Integer = 0
  Private m_foregroundColor As System.Drawing.Color = System.Drawing.Color.Gray
  Private m_backgroundColor As System.Drawing.Color = System.Drawing.Color.Black

  Public Sub New(sx As Integer, sy As Integer, scale As Integer)
    Dim unused1 = SDL_Init(SDL_INIT_VIDEO)
    'Dim unused2 = SDL_CreateWindowAndRenderer(sx * 4, sy * 4, SDL_WindowFlags.SDL_WINDOW_RESIZABLE, m_window, m_renderer)
    Dim unused2 = SDL_CreateWindowAndRenderer(sx * scale, sy * scale, 0, m_window, m_renderer)
    Dim unused3 = SDL_RenderSetScale(m_renderer, scale, scale)
    'SDL_SetWindowTitle(m_window, "Game")
  End Sub

  Public Sub Color(fg As Drawing.Color)
    m_foregroundColor = fg
  End Sub

  Public Sub Color(fg As Integer)
    m_foregroundColor = ColorToRgba(fg)
  End Sub

  Public Sub Color(fg As Drawing.Color, bg As Drawing.Color)
    m_foregroundColor = fg
    m_backgroundColor = bg
  End Sub

  Public Sub Color(fg As Integer, bg As Integer)
    m_foregroundColor = ColorToRgba(fg)
    m_backgroundColor = ColorToRgba(bg)
  End Sub

  Public Sub Pset(x As Integer, y As Integer, Optional rgba As System.Drawing.Color? = Nothing)
    If rgba Is Nothing Then rgba = m_foregroundColor
    m_points.Add((x, y, CType(rgba, System.Drawing.Color)))
    m_cx = x
    m_cy = y
  End Sub

  Private Shared Function ColorToRgba(c As Integer) As System.Drawing.Color
    Select Case c
      Case ConsoleColor.Black : Return System.Drawing.Color.Black
      Case ConsoleColor.Black : Return System.Drawing.Color.Black
      Case ConsoleColor.DarkBlue : Return System.Drawing.Color.DarkBlue
      Case ConsoleColor.DarkGreen : Return System.Drawing.Color.DarkGreen
      Case ConsoleColor.DarkCyan : Return System.Drawing.Color.DarkCyan
      Case ConsoleColor.DarkRed : Return System.Drawing.Color.DarkRed
      Case ConsoleColor.DarkMagenta : Return System.Drawing.Color.DarkMagenta
      Case ConsoleColor.DarkYellow : Return System.Drawing.Color.Brown
      Case ConsoleColor.Gray : Return System.Drawing.Color.Gray
      Case ConsoleColor.DarkGray : Return System.Drawing.Color.DarkGray
      Case ConsoleColor.Blue : Return System.Drawing.Color.Blue
      Case ConsoleColor.Green : Return System.Drawing.Color.Green
      Case ConsoleColor.Cyan : Return System.Drawing.Color.Cyan
      Case ConsoleColor.Red : Return System.Drawing.Color.Red
      Case ConsoleColor.Magenta : Return System.Drawing.Color.Magenta
      Case ConsoleColor.Yellow : Return System.Drawing.Color.Yellow
      Case ConsoleColor.White : Return System.Drawing.Color.White
      Case Else
        Throw New ArgumentOutOfRangeException(NameOf(c))
    End Select
  End Function

  Public Sub Pset(x As Integer, y As Integer, c As Integer)
    Pset(x, y, ColorToRgba(c))
  End Sub

  Public Sub Preset(x As Integer, y As Integer)
    For Each p In m_points
      If p.X = x AndAlso p.Y = y Then
        p.C = m_backgroundColor
        Exit For
      End If
    Next
  End Sub

  Public Function Point(n As Integer) As Integer
    Select Case n
      Case 0 ' The current viewport x coordinate
        Return m_cx
      Case 1 ' The current viewport y coordinate
        Return m_cy
      Case 2 ' The current window x coordinate
        Return m_cx
      Case 3 ' The current window y coordinate
        Return m_cy
      Case Else
        Throw New ArgumentException("Allowed range is 0 to 3.", NameOf(n))
    End Select
  End Function

  Public Function Point(x As Integer, y As Integer) As System.Drawing.Color
    For Each p In m_points
      If p.X = x AndAlso p.Y = y Then
        Return p.C
      End If
    Next
    Return m_backgroundColor
  End Function

  Public Sub Cls()
    m_points.Clear()
  End Sub

  Friend Sub Line(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer, attribute As Drawing.Color, box As Boolean, fill As Boolean, style As Short?, Optional ByRef styleBit As Integer = 0)

    'If attribute = -1 Then
    '  attribute = Me.m_foreColor
    'End If

    If box Then

      ' Box

      If fill Then

        For y As Integer = y1 To y2
          Me.Line(x1, y, x2, y, attribute, False, False, style)
        Next

      Else

        ' Top of box.
        Me.Line(x1, y1, x2, y1, attribute, False, False, style, styleBit)
        ' Left side of box.
        Me.Line(x1, y1, x1, y2, attribute, False, False, style, styleBit)
        ' Right side of box.
        Me.Line(x2, y1, x2, y2, attribute, False, False, style, styleBit)
        ' Bottom of box.
        Me.Line(x1, y2, x2, y2, attribute, False, False, style, styleBit)

      End If

    Else

      ' Line

      If x1 <> x2 Then

        Dim m = (y1 - y2) / (x1 - x2)

        If Math.Abs(m) <= 1 Then

          For i = 0 To (x2 - x1) Step Math.Sign(x2 - x1)

            Dim x = x1 + i
            Dim y = CInt(Fix(y1 + i * m + 0.5))

            Dim checkBit As UShort = CUShort(1 << styleBit) : styleBit += 1 : If styleBit > 15 Then styleBit = 0
            Dim doIt As Boolean = style Is Nothing
            If Not doIt Then
              doIt = ((If(style, 0) And checkBit) = checkBit)
            End If

            If doIt Then
              Me.Pset(x, y, attribute)
            End If

          Next

        Else

          For i = 0 To (y2 - y1) Step Math.Sign(y2 - y1)

            Dim y = y1 + i
            Dim x = CInt(Fix(x1 + i / m + 0.5))

            Dim checkBit As UShort = CUShort(1 << styleBit) : styleBit += 1 : If styleBit > 15 Then styleBit = 0
            Dim doIt As Boolean = style Is Nothing
            If Not doIt Then
              doIt = ((If(style, 0) And checkBit) = checkBit)
            End If

            If doIt Then
              Me.Pset(x, y, attribute)
            End If

          Next

        End If

      Else

        If y2 = y1 Then
          Me.Pset(x1, y2, attribute)
        Else
          For i = 0 To (y2 - y1) Step Math.Sign(y2 - y1)

            Dim x = x1
            Dim y = (y1 + i)

            Dim checkBit As UShort = CUShort(1 << styleBit) : styleBit += 1 : If styleBit > 15 Then styleBit = 0
            Dim doIt As Boolean = style Is Nothing
            If Not doIt Then
              doIt = ((If(style, 0) And checkBit) = checkBit)
            End If

            If doIt Then
              Me.Pset(x, y, attribute)
            End If

          Next
        End If

      End If

    End If

  End Sub

  Public Sub Circle(xcenter As Integer, ycenter As Integer, radius As Double, attribute As Drawing.Color, start As Double, [end] As Double, aspect As Double)

    '                                     1                                  2               4                 8               16
    ' passed is used to notify the routine whether or not the optional values are actually provided or not.
    ' it's a bit field.

    'Select Case Me.m_screenMode
    '    'Case 0
    '    '????
    '  Case 1
    '    If Not attribute.Between(0, 3) Then attribute = 3
    '  Case 2
    '    If Not attribute.Between(0, 1) Then attribute = 1
    '  Case 7
    '    If Not attribute.Between(0, 15) Then attribute = 15
    '  Case 8
    '    If Not attribute.Between(0, 15) Then attribute = 15
    '  Case 9
    '    If Not attribute.Between(0, 15) Then attribute = 15
    '  Case 10
    '    If Not attribute.Between(0, 3) Then attribute = 3
    '  Case Else
    '    Stop
    'End Select

    Static pi As Double = 3.1415926535897931
    Static pi2 As Double = 6.2831853071795862
    Static line_to_start, line_from_end As Integer
    Static ix, iy As Integer ' integer screen co-ordinates of circle's centre
    Static xspan, yspan As Double
    Static c As Double ' circumference
    Static px, py As Double
    Static sinb, cosb As Double ' second angle used in double-angle-formula
    Static pixels As Integer
    Static tmp As Double
    Static tmpi As Integer
    Static i As Integer
    Static exclusive As Integer
    Static arc1, arc2, arc3, arc4, arcinc As Double
    Static px2 As Double ', py2 As Double
    Static x2, y2 As Integer
    Static lastplotted_x2, lastplotted_y2 As Integer
    Static lastchecked_x2, lastchecked_y2 As Integer

    'If m_writePage.Text Then Throw New InvalidOperationException

    ' lines to & from centre
    'If (Not ((passed And 4) = 4)) Then start = 0
    'If (Not ((passed And 8) = 8)) Then [end] = pi2
    line_to_start = 0 : If (start < 0) Then line_to_start = 1 : start = -start
    line_from_end = 0 : If ([end] < 0) Then line_from_end = 1 : [end] = -[end]

    ' error checking
    If (start > pi2) Then Throw New InvalidOperationException
    If ([end] > pi2) Then Throw New InvalidOperationException

    ' when end<start, the arc of the circle that wouldn't have been drawn if start & end 
    ' were swapped is drawn
    exclusive = 0
    If [end] < start Then
      tmp = start : start = [end] : [end] = tmp
      tmpi = line_to_start : line_to_start = line_from_end : line_from_end = tmpi
      exclusive = 1
    End If

    ' calc. centre
    'If (passed And 1) = 1 Then x = m_writePage.X + x : y = m_writePage.Y + y
    'm_writePage.X = x : m_writePage.Y = y ' set graphics cursor position to circle's centre

    Dim r As Double = radius
    Dim x As Integer = xcenter
    Dim y As Integer = ycenter

    r = x + r ' the differece between x & x+r in pixels will be the radius in pixels
    ' resolve coordinates (but keep as floats)
    'If m_writePage.ClippingOrScaling <> 0 Then
    '  If m_writePage.ClippingOrScaling = 2 Then
    '    x = x * m_writePage.ScalingX + m_writePage.ScalingOffsetX + m_writePage.ViewOffsetX
    '    y = y * m_writePage.ScalingY + m_writePage.ScalingOffsetY + m_writePage.ViewOffsetY
    '    r = r * m_writePage.ScalingX + m_writePage.ScalingOffsetX + m_writePage.ViewOffsetX
    '  Else
    '    x = x + m_writePage.ViewOffsetX
    '    y = y + m_writePage.ViewOffsetY
    '    r = r + m_writePage.ViewOffsetX
    '  End If
    'End If
    'If x < 0 Then ix = CInt(x - 0.05) Else ix = CInt(x + 0.5)
    'If y < 0 Then iy = CInt(y - 0.05) Else iy = CInt(y + 0.05)
    If x < 0 Then ix = CInt(x) Else ix = CInt(x)
    If y < 0 Then iy = CInt(y) Else iy = CInt(y)
    r = Math.Abs(r - x) ' r is now a radius in pixels

    ' adjust vertical and horizontal span of the circle based on aspect ratio
    xspan = r : yspan = r
    'If Not ((passed And 16) = 16) Then
    '  aspect = 1 ' Note: default aspect ratio is 1:1 for QB64 specific modes (256/32)
    '  If (m_writePage.CompatibleMode = 1) Then aspect = 4.0 * (200.0 / 320.0) / 3.0
    '  If (m_writePage.CompatibleMode = 2) Then aspect = 4.0 * (200.0 / 640.0) / 3.0
    '  If (m_writePage.CompatibleMode = 7) Then aspect = 4.0 * (200.0 / 320.0) / 3.0
    '  If (m_writePage.CompatibleMode = 8) Then aspect = 4.0 * (200.0 / 640.0) / 3.0
    '  If (m_writePage.CompatibleMode = 9) Then aspect = 4.0 * (350.0 / 640.0) / 3.0
    '  If (m_writePage.CompatibleMode = 10) Then aspect = 4.0 * (350.0 / 640.0) / 3.0
    '  If (m_writePage.CompatibleMode = 11) Then aspect = 4.0 * (480.0 / 640.0) / 3.0
    '  If (m_writePage.CompatibleMode = 12) Then aspect = 4.0 * (480.0 / 640.0) / 3.0
    '  If (m_writePage.CompatibleMode = 13) Then aspect = 4.0 * (200.0 / 320.0) / 3.0
    '  ' Old method: aspect = 4.0 * (m_writePage.Height / m_writePage.width) / 3.0
    'End If
    If aspect >= 0 Then
      If aspect < 1 Then
        ' aspect: 0 to 1
        yspan *= aspect
      End If
      If aspect > 1 Then
        ' aspect: 1 to infinity
        xspan /= aspect
      End If
    Else
      If (aspect > -1) Then
        ' aspect: -1 to 0
        yspan *= (1 + aspect)
      End If
      ' if aspect<-1 no change is required
    End If

    ' skip everything if none of the circle is inside current viwport
    'If ((x + xspan + 0.5) < m_writePage.ViewX1) Then Return
    'If ((y + yspan + 0.5) < m_writePage.ViewY1) Then Return
    'If ((x - xspan - 0.5) > m_writePage.ViewX2) Then Return
    'If ((y - yspan - 0.5) > m_writePage.ViewY2) Then Return

    'If Not ((passed And 2) = 2) Then col = m_writePage.Color
    'm_writePage.DrawColor = col

    ' pre-set/pre-calcualate values
    c = pi2 * r
    pixels = CInt(c / 4.0) ' + 0.5)
    arc1 = 0
    arc2 = pi
    arc3 = pi
    arc4 = pi2
    arcinc = (pi / 2) / CDbl(pixels)
    sinb = Math.Sin(arcinc)
    cosb = Math.Cos(arcinc)
    lastplotted_x2 = -1
    lastchecked_x2 = -1
    i = 0

    If CBool(line_to_start) Then
      px = Math.Cos(start) : py = Math.Sin(start)
      x2 = CInt(px * xspan + 0.5) : y2 = CInt(py * yspan - 0.5)
      'FastLine(ix, iy, ix + x2, iy - y2, col)
      Me.Line(ix, iy, ix + x2, iy - y2, attribute, False, False, New Short?)
    End If

    px = 1
    py = 0

drawcircle:
    x2 = CInt(px * xspan) ' + 0.5)
    y2 = CInt(py * yspan) ' - 0.5)

    If (i = 0) Then lastchecked_x2 = x2 : lastchecked_y2 = y2 : GoTo plot

    If ((Math.Abs(x2 - lastplotted_x2) >= 2) OrElse (Math.Abs(y2 - lastplotted_y2) >= 2)) Then
plot:
      If CBool(exclusive) Then
        If ((arc1 <= start) OrElse (arc1 >= [end])) Then Me.Pset(ix + lastchecked_x2, iy + lastchecked_y2, attribute)
        If ((arc2 <= start) OrElse (arc2 >= [end])) Then Me.Pset(ix - lastchecked_x2, iy + lastchecked_y2, attribute)
        If ((arc3 <= start) OrElse (arc3 >= [end])) Then Me.Pset(ix - lastchecked_x2, iy - lastchecked_y2, attribute)
        If ((arc4 <= start) OrElse (arc4 >= [end])) Then Me.Pset(ix + lastchecked_x2, iy - lastchecked_y2, attribute)
      Else ' inclusive
        If ((arc1 >= start) AndAlso (arc1 <= [end])) Then Me.Pset(ix + lastchecked_x2, iy + lastchecked_y2, attribute)
        If ((arc2 >= start) AndAlso (arc2 <= [end])) Then Me.Pset(ix - lastchecked_x2, iy + lastchecked_y2, attribute)
        If ((arc3 >= start) AndAlso (arc3 <= [end])) Then Me.Pset(ix - lastchecked_x2, iy - lastchecked_y2, attribute)
        If ((arc4 >= start) AndAlso (arc4 <= [end])) Then Me.Pset(ix + lastchecked_x2, iy - lastchecked_y2, attribute)
      End If
      If (i > pixels) Then GoTo allplotted
      lastplotted_x2 = lastchecked_x2 : lastplotted_y2 = lastchecked_y2
    End If
    lastchecked_x2 = x2 : lastchecked_y2 = y2

    If (i <= pixels) Then
      i += 1
      If (i > pixels) Then GoTo plot
      px2 = px * cosb + py * sinb
      py = py * cosb - px * sinb
      px = px2
      If CBool(i) Then arc1 += arcinc : arc2 -= arcinc : arc3 += arcinc : arc4 -= arcinc
      GoTo drawcircle
    End If

allplotted:

    If CBool(line_from_end) Then
      px = Math.Cos([end]) : py = Math.Sin([end])
      x2 = CInt(px * xspan + 0.5) : y2 = CInt(py * yspan - 0.5)
      'FastLine(ix, iy, ix + x2, iy - y2, col)
      Me.Line(ix, iy, ix + x2, iy - y2, attribute, False, False, New Short?)
    End If

  End Sub

  Public Sub Render()
    Dim unused1 = SDL_SetRenderDrawColor(m_renderer, m_backgroundColor.R, m_backgroundColor.G, m_backgroundColor.B, m_backgroundColor.A)
    Dim unused2 = SDL_RenderClear(m_renderer)
    For Each p In m_points
      Dim unused3 = SDL_SetRenderDrawColor(m_renderer, p.C.R, p.C.G, p.C.B, p.C.A)
      Dim unused4 = SDL_RenderDrawPointF(m_renderer, p.X, p.Y)
    Next
    SDL_RenderPresent(m_renderer)
  End Sub

  Public Sub Close()
    SDL_Quit()
  End Sub

  Private m_fullscreen As Boolean = False

  Public Function Input() As System.ConsoleKey

    While SDL_PollEvent(m_evt) <> 0
      If m_evt.type = SDL_EventType.SDL_QUIT Then
        Return ConsoleKey.Escape
      ElseIf m_evt.type = SDL_EventType.SDL_KEYDOWN Then
        Select Case m_evt.key.keysym.sym
          Case SDL_Keycode.SDLK_ESCAPE
            Return ConsoleKey.Escape
          Case SDL_Keycode.SDLK_F11
            m_fullscreen = Not m_fullscreen
            Dim flag = CUInt(m_fullscreen) 'If(m_fullscreen, 1UI, 0UI)
            'Dim unused1 = SDL_SetWindowFullscreen(m_window, flag)
          Case Else
        End Select
      End If
    End While

    Return 0

  End Function

End Class

Public Interface IScreen

  Sub Color(fg As Integer)
  Sub Color(fg As Integer, bg As Integer)
  Sub PSet(x As Integer, y As Integer)
  Sub PSet(x As Integer, y As Integer, c As Integer)
  Sub Preset(x As Integer, y As Integer)
  Sub Point(n As Integer)
  Sub Point(x As Integer, y As Integer)

  'Sub Line(stepped As Boolean, x2 As Integer, y2 As Integer)
  'Sub Line(stepped As Boolean, x2 As Integer, y2 As Integer, box As Boolean)
  'Sub Line(stepped As Boolean, x2 As Integer, y2 As Integer, box As Boolean, fill As Boolean)
  'Sub Line(stepped As Boolean, x2 As Integer, y2 As Integer, c As Drawing.Color)
  'Sub Line(stepped As Boolean, x2 As Integer, y2 As Integer, c As Drawing.Color, box As Boolean)
  'Sub Line(stepped As Boolean, x2 As Integer, y2 As Integer, c As Drawing.Color, box As Boolean, fill As Boolean, style As Integer)
  Sub Line(stepped As Boolean, x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer, c As Drawing.Color, box As Boolean, fill As Boolean, style As Integer)

  Sub Circle(stepped As Boolean, x As Integer, y As Integer, radius As Single, c As Drawing.Color, start As Single, finish As Single, aspect As Single)

End Interface