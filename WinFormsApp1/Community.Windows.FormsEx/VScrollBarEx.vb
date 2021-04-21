Option Explicit On
Option Strict On
Option Infer On

Namespace Global.Community.Windows.FormsEx

  Public Class VScrollBarEx
    Inherits ScrollBar

    Public Sub New()
      Width = 15
    End Sub

    Protected Overloads Overrides ReadOnly Property SmallDecrementRectangle() As Rectangle
      Get
        Dim r = ClientRectangle
        Dim x = 2
        Dim y = 2
        Dim w = r.Width - 2 - 2
        Dim h = r.Width - 2 - 2
        If h > (r.Height - 2 - 2) \ 2 Then
          h = (r.Height - 2 - 2) \ 2
        End If
        Return New Rectangle(x, y, w, h)
      End Get
    End Property

    Protected Overloads Overrides ReadOnly Property SmallIncrementRectangle() As Rectangle
      Get
        Dim r = ClientRectangle
        Dim x = 2
        Dim h = r.Width - 2 - 2
        If h > (r.Height - 2 - 2) \ 2 Then
          h = (r.Height - 2 - 2) \ 2
          If (r.Height And 1) = 1 Then
            h += 1
          End If
        End If
        Dim w = r.Width - 2 - 2
        Dim y = r.Bottom - (h + 2)
        Return New Rectangle(x, y, w, h)
      End Get
    End Property

    Protected Overloads Overrides ReadOnly Property LargeDecrementRectangle() As Rectangle
      Get
        Dim r = ClientRectangle
        Dim x = 2
        Dim y = r.Width - 2 + 1
        Dim w = Width - 2 - 2
        Dim h = _thumbpos
        Return New Rectangle(x, y, w, h)
      End Get
    End Property

    Protected Overloads Overrides ReadOnly Property LargeIncrementRectangle() As Rectangle
      Get
        Dim r = ClientRectangle
        Dim thumb = ThumbRectangle
        Dim x = 2
        Dim y = thumb.Y + thumb.Height
        Dim w = Width - 2 - 2
        Dim h = r.Bottom - (r.Width - 2) - y
        Return New Rectangle(x, y, w, h)
      End Get
    End Property

    Protected Overloads Overrides ReadOnly Property ThumbRectangle() As Rectangle
      Get
        Dim r = ClientRectangle
        Dim arrowrect = SmallDecrementRectangle
        ' We have to add 1 because when Minimum = 0 and Maximum = 0,
        ' we want a thumb which is half the size of the bar.
        Dim domain As Double = _maximum - _minimum + 1
        ' If LargeChange > Maximum, we want a full thumb.
        Dim h = If(_largechange > _maximum,
                   Height - (2 + SmallDecrementRectangle.Height + 1) * 2,
                   (Height - (2 + SmallDecrementRectangle.Height + 1) * 2) / (domain / _largechange))
        ' Minimum size of thumb is 6 pixels.
        If h < 6 Then h = 6
        Dim x = 2
        Dim w = r.Width - 2 - 2
        Dim y = ((2 + arrowrect.Height + 1) + _thumbpos)
        Return New Rectangle(x, y, w, CInt(h))
      End Get
    End Property

    Protected Overloads Overrides ReadOnly Property ThumbBarRectangle() As Rectangle
      Get
        Dim arrow = SmallDecrementRectangle
        Return New Rectangle(2, 2 + arrow.Height + 1, Width - 2 - 2, Height - (2 + arrow.Height + 1) * 2)
      End Get
    End Property

    'Protected Friend Overloads Overrides ReadOnly Property SmallDecrementArrowDirection() As ArrowDirections
    '  Get
    '    Return ArrowDirections.Up
    '  End Get
    'End Property

    'Protected Friend Overloads Overrides ReadOnly Property SmallIncrementArrowDirection() As ArrowDirections
    '  Get
    '    Return ArrowDirections.Down
    '  End Get
    'End Property

    Protected Overloads Overrides ReadOnly Property PixelDomain() As Integer
      Get
        Return (Height - (2 + SmallDecrementRectangle.Height + 1) * 2) - ThumbRectangle.Height
      End Get
    End Property

    Protected Overloads Overrides Sub OnResize(e As EventArgs)
      MyBase.OnResize(e)
      _thumbvisible = Height - (2 + SmallDecrementRectangle.Height + 1) * 2 >= 6
    End Sub

  End Class

End Namespace