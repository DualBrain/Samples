Option Explicit On
Option Strict On
Option Infer On

Namespace ScrollBarEx

  ''' <summary>
  ''' CustomControl give the guide and the base for VScroll
  ''' </summary>
  ''' <remarks></remarks>
  Public MustInherit Class ScrollBar
    Inherits UserControl

    Protected _minimum As Integer = 0
    Protected _maximum As Integer = 100
    Protected _value As Integer = 0
    Protected _smallchange As Integer = 1
    Protected _largechange As Integer = 10
    Protected _thumbpos As Integer = 0
    Protected _diffmousedownthumboffset As Integer
    Protected _sba As ScrollEventType? 'ScrollBarAction = ScrollBarAction.None
    Protected _sbaflag As Boolean = False
    Protected _thumbvisible As Boolean = True
    Protected _timer As Timer
    Protected _mousex As Integer
    Protected _mousey As Integer

    Protected Sub New()
      SetStyle(ControlStyles.ResizeRedraw Or
               ControlStyles.DoubleBuffer Or
               ControlStyles.UserPaint Or
               ControlStyles.AllPaintingInWmPaint, True)
    End Sub

    ' The pixel domain is the number of pixels the thumb can actually move.

    Protected MustOverride ReadOnly Property SmallDecrementRectangle() As Rectangle
    Protected MustOverride ReadOnly Property LargeDecrementRectangle() As Rectangle
    Protected MustOverride ReadOnly Property ThumbRectangle() As Rectangle
    Protected MustOverride ReadOnly Property ThumbBarRectangle() As Rectangle
    Protected MustOverride ReadOnly Property LargeIncrementRectangle() As Rectangle
    Protected MustOverride ReadOnly Property SmallIncrementRectangle() As Rectangle
    Protected MustOverride ReadOnly Property SmallDecrementArrowDirection() As ArrowDirections
    Protected MustOverride ReadOnly Property SmallIncrementArrowDirection() As ArrowDirections
    Protected MustOverride ReadOnly Property PixelDomain() As Integer

    Public Property Minimum() As Integer
      Get
        Return _minimum
      End Get
      Set(value As Integer)
        If value > _maximum Then
          Throw New ArgumentOutOfRangeException("Minimum", "Minimum cannot be greater than Maximum.")
        End If
        _minimum = value
        Invalidate()
      End Set
    End Property

    Public Property Maximum() As Integer
      Get
        Return _maximum
      End Get
      Set(value As Integer)
        If value < _minimum Then
          Throw New ArgumentOutOfRangeException("Maximum", "Maximum cannot be less than Minimum.")
        End If
        _maximum = value
        Invalidate()
      End Set
    End Property

    Public Property Value() As Integer
      Get
        Return _value
      End Get
      Set(value As Integer)
        If value < _minimum OrElse value > _maximum Then
          Throw New ArgumentOutOfRangeException(NameOf(value), "Value must be in range [Minimum,Maximum].")
        End If
        If _value <> value Then
          _value = value
          ' Do not invalidate arrows.
          Invalidate(ThumbBarRectangle)
          OnValueChanged(EventArgs.Empty)
        End If
      End Set
    End Property

    Public Property SmallChange() As Integer
      Get
        Return _smallchange
      End Get
      Set(value As Integer)
        If value < 0 Then
          Throw New ArgumentOutOfRangeException(NameOf(SmallChange), String.Format("Value must be in range [0,{0}].", Integer.MaxValue))
        End If
        _smallchange = value
      End Set
    End Property

    Public Property LargeChange() As Integer
      Get
        Return _largechange
      End Get
      Set(value As Integer)
        If value < 0 Then
          Throw New ArgumentOutOfRangeException(NameOf(LargeChange), String.Format("Value must be in range [0,{0}].", Integer.MaxValue))
        End If
        _largechange = value
      End Set
    End Property

    ' The thumb position is used when the thumb is moved with the mouse.
    ' It is not possible to directly calculate the new Value and then
    ' invalidate the thumb bar because the thumb could be moved a bit to
    ' the left or to the right. So, we keep the actual position the user
    ' has moved the thumb in a separate variable.

    Private Property ThumbPosition() As Integer
      Get
        Return _thumbpos
      End Get
      Set(value As Integer)
        _thumbpos = value
        Dim pixeldomain = Me.PixelDomain
        If value < 0 Then
          _thumbpos = 0
        End If
        If value > pixeldomain Then
          _thumbpos = pixeldomain
        End If
        ' Translate thumb position to logical value.
        Dim domain As Double = _maximum - _minimum
        If domain > 0 Then
          Me.Value = CInt(_thumbpos / pixeldomain * domain)
        Else
          Me.Value = _minimum
        End If
      End Set
    End Property

    Protected Overloads Overrides Sub OnPaint(e As PaintEventArgs)
      MyBase.OnPaint(e)
      ' Background
      Using brush = New SolidBrush(DarkMode.Background)
        e.Graphics.FillRectangle(brush, ClientRectangle)
      End Using
      ' Border
      DrawBorder(e.Graphics, ClientRectangle)
      ' Up Arrow
      DrawArrow(e.Graphics, SmallDecrementRectangle, ArrowDirections.Up)
      ' Down Arrow
      DrawArrow(e.Graphics, SmallIncrementRectangle, ArrowDirections.Down)
      ' Thumb
      If _thumbvisible Then
        Using brush = New SolidBrush(DarkMode.Thumb)
          Dim r = ThumbRectangle
          r.Inflate(-2, -2) ' Actually... deflating.
          e.Graphics.FillRectangle(brush, r)
        End Using
      End If
    End Sub

    Private Shared Sub DrawArrow(g As Graphics, r As Rectangle, dir As ArrowDirections)
      Dim w = r.Width
      Dim h = r.Height
      If w <= 0 OrElse h <= 0 Then Return
      Using brush = New SolidBrush(DarkMode.Background)
        g.FillRectangle(brush, r)
      End Using
      Select Case dir
        Case ArrowDirections.Up
          Using brush = New SolidBrush(DarkMode.Thumb)
            Using pen = New Pen(brush)
              Dim cx = r.Left + (w \ 2)
              Dim cy = r.Top + (h \ 2)
              Dim dy = -2
              For dx = 0 To 4
                g.DrawLine(pen, cx - dx, cy + dy, cx + dx, cy + dy)
                dy += 1
              Next
            End Using
          End Using
        Case ArrowDirections.Down
          Using brush = New SolidBrush(DarkMode.Thumb)
            Using pen = New Pen(brush)
              Dim cx = r.Left + (w \ 2)
              Dim cy = r.Top + (h \ 2)
              Dim dy = -2
              For dx = 4 To 0 Step -1
                g.DrawLine(pen, cx - dx, cy + dy, cx + dx, cy + dy)
                dy += 1
              Next
            End Using
          End Using
        Case ArrowDirections.Left
          Stop
        Case ArrowDirections.Right
          Stop
        Case Else
      End Select
    End Sub

    Private Shared Sub DrawBorder(g As Graphics, r As Rectangle)
      Using pen = New Pen(DarkMode.Border)
        g.DrawLine(pen, r.X, r.Y, r.Right - 1, r.Y)
        g.DrawLine(pen, r.X, r.Y + 1, r.Right - 1, r.Y + 1)
        g.DrawLine(pen, r.X, r.Y, r.X, r.Bottom - 1)
        g.DrawLine(pen, r.X + 1, r.Y, r.X + 1, r.Bottom - 2)
        g.DrawLine(pen, r.X + 2, r.Bottom - 2, r.Right - 1, r.Bottom - 2)
        g.DrawLine(pen, r.X + 1, r.Bottom - 1, r.Right - 1, r.Bottom - 1)
        g.DrawLine(pen, r.Right - 1, r.Y + 1, r.Right - 1, r.Bottom - 1)
        g.DrawLine(pen, r.Right - 2, r.Y + 2, r.Right - 2, r.Bottom - 1)
      End Using
    End Sub

    Public Sub SyncThumbPositionWithLogicalValue()
      Dim domain As Double = _maximum - _minimum
      If domain > 0 Then
        _thumbpos = CInt(_value / domain * PixelDomain)
      Else
        _thumbpos = 0
      End If
      Debug.WriteLine($"Thumb Pos = {_thumbpos},{_maximum},{_minimum},{_value},{PixelDomain}")
      Invalidate()
    End Sub

    Private Sub SmallIncrement()
      Dim newvalue = _value + _smallchange
      If newvalue > _maximum Then
        newvalue = _maximum
      End If
      Value = newvalue
      SyncThumbPositionWithLogicalValue()
      RaiseEvent OurScroll(Me, New ScrollEventArgs(ScrollEventType.SmallIncrement, 1))
    End Sub

    Private Sub SmallDecrement()
      Dim newvalue = _value - _smallchange
      If newvalue < _minimum Then
        newvalue = _minimum
      End If
      Value = newvalue
      SyncThumbPositionWithLogicalValue()
    End Sub

    Private Sub LargeIncrement()
      Dim newvalue = _value + _largechange
      If newvalue > _maximum Then
        newvalue = _maximum
      End If
      Value = newvalue
      SyncThumbPositionWithLogicalValue()
    End Sub

    Private Sub LargeDecrement()
      Dim newvalue = _value - _largechange
      If newvalue < _minimum Then
        newvalue = _minimum
      End If
      Value = newvalue
      SyncThumbPositionWithLogicalValue()
    End Sub

    Private Sub StartTimer()
      If _timer Is Nothing Then
        _timer = New Timer()
        AddHandler _timer.Tick, AddressOf OnTimerTick
      End If
      _timer.Interval = 250
      _timer.Start()
    End Sub

    Protected Overloads Overrides Sub OnMouseDown(e As MouseEventArgs)
      MyBase.OnMouseDown(e)
      If e.Button = MouseButtons.Left Then
        Dim r = ThumbRectangle
        If _thumbvisible AndAlso _largechange < _maximum AndAlso r.Contains(e.X, e.Y) Then
          ' Thumb clicked.
          If TypeOf Me Is HScrollBarEx Then
            _diffmousedownthumboffset = e.X - r.Left
          Else
            _diffmousedownthumboffset = e.Y - r.Top
          End If
          Capture = True
          _sba = ScrollEventType.ThumbTrack
        Else
          r = SmallDecrementRectangle
          If r.Contains(e.X, e.Y) Then
            ' Left arrow clicked.
            Capture = True
            _sba = ScrollEventType.SmallDecrement
            _sbaflag = True
            Invalidate(r)
            If _value <> _minimum Then
              ' Decrement one SmallChange.
              SmallDecrement()
              ' Start timer for repeated decrements.
              StartTimer()
            End If
          Else
            r = SmallIncrementRectangle
            If r.Contains(e.X, e.Y) Then
              ' Right arrow clicked.
              Capture = True
              _sba = ScrollEventType.SmallIncrement
              _sbaflag = True
              Invalidate(r)
              If _value <> _maximum Then
                ' Increment one SmallChange
                SmallIncrement()
                ' Start timer for repeated increments.
                StartTimer()
              End If
            Else
              r = LargeDecrementRectangle
              If r.Contains(e.X, e.Y) Then
                ' Left page clicked.
                Capture = True
                _sba = ScrollEventType.LargeDecrement
                _sbaflag = True
                _mousex = e.X
                _mousey = e.Y
                If _value <> _minimum Then
                  ' Decrement one LargeChange.
                  LargeDecrement()
                  ' Start timer for repeated decrements.
                  StartTimer()
                End If
              Else
                r = LargeIncrementRectangle
                If r.Contains(e.X, e.Y) Then
                  ' Right page clicked.
                  Capture = True
                  _sba = ScrollEventType.LargeIncrement
                  _sbaflag = True
                  _mousex = e.X
                  _mousey = e.Y
                  If _value <> _maximum Then
                    ' Increment one LargeChange.
                    LargeIncrement()
                    ' Start timer for repeated increments.
                    StartTimer()
                  End If
                End If
              End If
            End If
          End If
        End If
        RaiseEvent OurScroll(Me, New ScrollEventArgs(CType(_sba, ScrollEventType), _value))
        'Debug.WriteIf(_sba = ScrollBarAction.ThumbTrack, _value)
      End If
    End Sub

    Protected Overloads Overrides Sub OnMouseMove(e As MouseEventArgs)
      MyBase.OnMouseMove(e)
      If e.Button = MouseButtons.Left Then
        Select Case _sba
          Case ScrollEventType.ThumbTrack
            If _thumbvisible Then
              If TypeOf Me Is HScrollBarEx Then
                ThumbPosition = (e.X - _diffmousedownthumboffset) - (2 + SmallDecrementRectangle.Width + 1)
              Else
                ThumbPosition = (e.Y - _diffmousedownthumboffset) - (2 + SmallDecrementRectangle.Height + 1)
              End If
              RaiseEvent OurScroll(Me, New ScrollEventArgs(CType(_sba, ScrollEventType), _value))
            End If
          Case ScrollEventType.SmallDecrement
            Dim r = SmallDecrementRectangle
            _sbaflag = r.Contains(e.X, e.Y)
            Invalidate(r)
            If _timer IsNot Nothing Then
              _timer.Enabled = _sbaflag
            End If
          Case ScrollEventType.SmallIncrement
            Dim r = SmallIncrementRectangle
            _sbaflag = r.Contains(e.X, e.Y)
            Invalidate(r)
            If _timer IsNot Nothing Then
              _timer.Enabled = _sbaflag
            End If
          Case ScrollEventType.LargeDecrement
            Dim r = LargeDecrementRectangle
            If _timer IsNot Nothing Then
              _timer.Enabled = r.Contains(e.X, e.Y)
            End If
            _mousex = e.X
            _mousey = e.Y
          Case ScrollEventType.LargeIncrement
            Dim r = LargeIncrementRectangle
            If _timer IsNot Nothing Then
              _timer.Enabled = r.Contains(e.X, e.Y)
            End If
            _mousex = e.X
            _mousey = e.Y
        End Select
      End If
    End Sub

    Protected Overloads Overrides Sub OnMouseUp(e As MouseEventArgs)
      MyBase.OnMouseUp(e)
      If e.Button = MouseButtons.Left Then
        Capture = False
        _sba = New ScrollEventType?
        _sbaflag = False
        Invalidate()
        If _timer IsNot Nothing Then
          _timer.Dispose()
          _timer = Nothing
        End If
      End If
    End Sub

    Private Sub OnTimerTick(sender As Object, e As EventArgs)
      If _timer.Interval = 250 Then
        _timer.Interval = 50
        ' inital delay
      End If
      ' repeated delay
      Select Case _sba
        Case ScrollEventType.SmallDecrement
          If _value <> _minimum Then
            SmallDecrement()
          End If
        Case ScrollEventType.SmallIncrement
          If _value <> _maximum Then
            SmallIncrement()
          End If
        Case ScrollEventType.LargeDecrement
          Dim r = LargeDecrementRectangle
          If r.Contains(_mousex, _mousey) Then
            If _value <> _minimum Then
              LargeDecrement()
            End If
          End If
        Case ScrollEventType.LargeIncrement
          Dim r = LargeIncrementRectangle
          If r.Contains(_mousex, _mousey) Then
            If _value <> _maximum Then
              LargeIncrement()
            End If
          End If
      End Select
      RaiseEvent OurScroll(Me, New ScrollEventArgs(CType(_sba, ScrollEventType), 1))
    End Sub

    Protected Overloads Overrides Sub OnResize(e As EventArgs)
      MyBase.OnResize(e)
      SyncThumbPositionWithLogicalValue()
    End Sub

    Public Event ValueChanged As EventHandler

    Protected Overridable Sub OnValueChanged(e As EventArgs)
      RaiseEvent ValueChanged(Me, e)
    End Sub

    Public Event OurScroll As ScrollEventHandler

    'Protected Overloads Sub OnScroll(ByVal sender As Object, ByVal e As ScrollEventArgs)
    '    RaiseEvent Scroll(Me, e)
    'End Sub

  End Class

End Namespace