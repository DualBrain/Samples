Public Structure Vf2d

  Public x As Single
  Public y As Single

  Public Sub New(x As Single, y As Single)
    Me.x = x
    Me.y = y
  End Sub

  Public Sub New(v As Vf2d)
    x = v.x
    y = v.y
  End Sub

  Public Function Mag() As Single
    Return MathF.Sqrt(x * x + y * y)
  End Function

  Public Function Mag2() As Single
    Return x * x + y * y
  End Function

  Public Function Norm() As Vf2d
    Dim r = CSng(1 / Mag())
    Return New Vf2d(x * r, y * r)
  End Function

  Public Function Perp() As Vf2d
    Return New Vf2d(-y, x)
  End Function

  Public Function Floor() As Vf2d
    Return New Vf2d(MathF.Floor(x), MathF.Floor(y))
  End Function

  Public Function Ceil() As Vf2d
    Return New Vf2d(MathF.Ceiling(x), MathF.Ceiling(y))
  End Function

  Public Function [Max](v As Vf2d) As Vf2d
    Return New Vf2d(MathF.Max(x, v.x), MathF.Max(y, v.y))
  End Function

  Public Function [Min](v As Vf2d) As Vf2d
    Return New Vf2d(MathF.Min(x, v.x), MathF.Min(y, v.y))
  End Function

  Public Function Cart() As Vf2d
    Return New Vf2d(MathF.Cos(y) * x, MathF.Sin(y) * x)
  End Function

  Public Function Polar() As Vf2d
    Return New Vf2d(Mag(), MathF.Atan2(y, x))
  End Function

  Public Function Clamp(v1 As Vf2d, v2 As Vf2d) As Vf2d
    Return [Max](v1).Min(v2)
  End Function

  Public Function Lerp(v1 As Vf2d, t As Double) As Vf2d
    Return Me * CSng(Fix(1.0F - t)) + (v1 * CSng(Fix(t)))
  End Function

  Public Function Dot(rhs As Vf2d) As Integer
    Return x * rhs.x + y * rhs.y
  End Function

  Public Function Cross(rhs As Vf2d) As Integer
    Return x * rhs.y - y * rhs.x
  End Function

  Public Shared Operator +(lhs As Vf2d, rhs As Vf2d) As Vf2d
    Return New Vf2d(lhs.x + rhs.x, lhs.y + rhs.y)
  End Operator

  Public Shared Operator -(left As Vf2d, right As Vf2d) As Vf2d
    Return New Vf2d(left.x - right.x, left.y - right.y)
  End Operator

  Public Shared Operator *(left As Vf2d, right As Single) As Vf2d
    Return New Vf2d(left.x * right, left.y * right)
  End Operator

  Public Shared Operator *(left As Vf2d, right As Vf2d) As Vf2d
    Return New Vf2d(left.x * right.x, left.y * right.y)
  End Operator

  Public Shared Operator /(left As Vf2d, right As Single) As Vf2d
    Return New Vf2d(left.x / right, left.y / right)
  End Operator

  Public Shared Operator /(left As Vf2d, right As Vf2d) As Vf2d
    Return New Vf2d(left.x / right.x, left.y / right.y)
  End Operator

  'Public Shared Operator +=(left As Vf2d, right As Vf2d) As Vf2d
  '  left.x += right.x
  '  left.y += right.y
  '  Return left
  'End Operator

  'Public Shared Operator -=(left As Vf2d, right As Vf2d) As Vf2d
  '  left.x -= right.x
  '  left.y -= right.y
  '  Return left
  'End Operator

  'Public Shared Operator *=(left As Vf2d, right As single) As Vf2d
  '  left.x *= right
  '  left.y *= right
  '  Return left
  'End Operator

  'Public Shared Operator /=(left As Vf2d, right As single) As Vf2d
  '  left.x /= right
  '  left.y /= right
  '  Return left
  'End Operator

  'Public Shared Operator *=(left As Vf2d, right As Vf2d) As Vf2d
  '  left.x *= right.x
  '  left.y *= right.y
  '  Return left
  'End Operator

  'Public Shared Operator /=(left As Vf2d, right As Vf2d) As Vf2d
  '  left.x /= right.x
  '  left.y /= right.y
  '  Return left
  'End Operator

  Public Shared Operator +(lhs As Vf2d) As Vf2d
    Return New Vf2d(+lhs.x, +lhs.y)
  End Operator

  Public Shared Operator -(lhs As Vf2d) As Vf2d
    Return New Vf2d(-lhs.x, -lhs.y)
  End Operator

  Public Shared Operator =(lhs As Vf2d, rhs As Vf2d) As Boolean
    Return (lhs.x = rhs.x AndAlso lhs.y = rhs.y)
  End Operator

  Public Shared Operator <>(lhs As Vf2d, rhs As Vf2d) As Boolean
    Return (lhs.x <> rhs.x OrElse lhs.y <> rhs.y)
  End Operator

  Public Function Str() As String
    Return $"({x},{y})"
  End Function

  Public Overrides Function ToString() As String
    Return Str()
  End Function

  Public Shared Widening Operator CType(v As Vf2d) As Vi2d
    Return New Vi2d(v.x, v.y)
  End Operator

  'Public Shared Widening Operator CType(v As Vf2d) As Vf2d
  '  Return New Vf2d(v.x, v.y)
  'End Operator

  'Public Shared Widening Operator CType(v As Vf2d) As v2d_generic(Of Double)
  '  Return New v2d_generic(Of Double)(v.x, v.y)
  'End Operator

End Structure