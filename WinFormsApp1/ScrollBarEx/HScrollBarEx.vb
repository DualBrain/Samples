Option Explicit On
Option Strict On
Option Infer On

Namespace ScrollBarEx

  Public Class HScrollBarEx
    Inherits ScrollBar

    Public Sub New()
      Height = 15
    End Sub

    Protected Overloads Overrides ReadOnly Property SmallDecrementRectangle() As Rectangle
      Get
        Dim x As Integer, y As Integer, w As Integer, h As Integer
        Dim r As Rectangle = ClientRectangle

        x = 2
        y = 2
        w = r.Height - 2 - 2
        If w > (r.Width - 2 - 2) \ 2 Then
          w = (r.Width - 2 - 2) \ 2
        End If
        h = r.Height - 2 - 2

        Return New Rectangle(x, y, w, h)
      End Get
    End Property

    Protected Overloads Overrides ReadOnly Property SmallIncrementRectangle() As Rectangle
      Get
        Dim x As Integer, y As Integer, w As Integer, h As Integer
        Dim r As Rectangle = ClientRectangle

        y = 2
        h = r.Height - 2 - 2
        w = r.Height - 2 - 2
        If w > (r.Width - 2 - 2) \ 2 Then
          w = (r.Width - 2 - 2) \ 2
          If (r.Width And 1) = 1 Then
            w += 1
          End If
        End If
        x = r.Right - (w + 2)

        Return New Rectangle(x, y, w, h)
      End Get
    End Property

    Protected Overloads Overrides ReadOnly Property LargeDecrementRectangle() As Rectangle
      Get
        Dim x As Integer, y As Integer, w As Integer, h As Integer
        Dim r As Rectangle = ClientRectangle

        x = r.Height - 2 + 1
        y = 2
        w = _thumbpos
        h = Height - 2 - 2

        Return New Rectangle(x, y, w, h)
      End Get
    End Property

    Protected Overloads Overrides ReadOnly Property LargeIncrementRectangle() As Rectangle
      Get
        Dim x As Integer, y As Integer, w As Integer, h As Integer
        Dim r As Rectangle = ClientRectangle
        Dim thumb As Rectangle = ThumbRectangle

        x = thumb.X + thumb.Width
        y = 2
        w = r.Right - (r.Height - 2) - x
        h = Height - 2 - 2

        Return New Rectangle(x, y, w, h)
      End Get
    End Property

    Protected Overloads Overrides ReadOnly Property ThumbRectangle() As Rectangle
      Get
        Dim x As Integer, y As Integer, w As Integer, h As Integer
        Dim r As Rectangle = ClientRectangle
        Dim arrowrect As Rectangle = SmallDecrementRectangle

        ' We have to add 1 because when Minimum = 0 and Maximum = 0,
        ' we want a thumb which is half the size of the bar.
        Dim domain As Double = _maximum - _minimum + 1

        ' If LargeChange >= Maximum, we want a full thumb.
        If _largechange >= _maximum Then
          w = Width - (2 + SmallDecrementRectangle.Width + 1) * 2
        Else
          w = CInt((Width - (2 + SmallDecrementRectangle.Width + 1) * 2) / (domain / _largechange))
        End If

        ' Minimum size of thumb is 6 pixels.
        If w < 6 Then
          w = 6
        End If

        y = 2
        h = r.Height - 2 - 2
        x = (2 + arrowrect.Width + 1) + _thumbpos

        Return New Rectangle(x, y, w, h)
      End Get
    End Property

    Protected Overloads Overrides ReadOnly Property ThumbBarRectangle() As Rectangle
      Get
        Dim arrow As Rectangle = SmallDecrementRectangle
        Return New Rectangle(2 + arrow.Width + 1, 2, Width - (2 + arrow.Width + 1) * 2, Height - 2 - 2)
      End Get
    End Property

    Protected Overloads Overrides ReadOnly Property SmallDecrementArrowDirection() As ArrowDirections
      Get
        Return ArrowDirections.Left
      End Get
    End Property

    Protected Overloads Overrides ReadOnly Property SmallIncrementArrowDirection() As ArrowDirections
      Get
        Return ArrowDirections.Right
      End Get
    End Property

    Protected Overloads Overrides ReadOnly Property PixelDomain() As Integer
      Get
        Return (Width - (2 + SmallDecrementRectangle.Width + 1) * 2) - ThumbRectangle.Width
      End Get
    End Property

    Protected Overloads Overrides Sub OnResize(e As EventArgs)
      MyBase.OnResize(e)

      _thumbvisible = Width - (2 + SmallDecrementRectangle.Width + 1) * 2 >= 6
    End Sub
  End Class

End Namespace