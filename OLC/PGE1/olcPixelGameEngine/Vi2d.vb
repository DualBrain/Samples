Friend Structure Vi2d

  Public x As Integer
  Public y As Integer

  Public Sub New(x As Integer, y As Integer)
    Me.x = x
    Me.y = y
  End Sub

  Public Sub New(v As Vi2d)
    x = v.x
    y = v.y
  End Sub

  Public Function Mag() As Integer
    Return CInt(Math.Sqrt(x * x + y * y))
  End Function

  Public Function Mag2() As Integer
    Return x * x + y * y
  End Function

  Public Function Norm() As Vi2d
    Dim r = CInt(1 / Mag())
    Return New Vi2d(x * r, y * r)
  End Function

  Public Function perp() As Vi2d
    Return New Vi2d(-y, x)
  End Function

  Public Function floor() As Vi2d
    Return New Vi2d(Math.Floor(x), Math.Floor(y))
  End Function

  Public Function ceil() As Vi2d
    Return New Vi2d(Math.Ceiling(x), Math.Ceiling(y))
  End Function

  Public Function [max](v As Vi2d) As Vi2d
    Return New Vi2d(Math.Max(x, v.x), Math.Max(y, v.y))
  End Function

  Public Function [min](v As Vi2d) As Vi2d
    Return New Vi2d(Math.Min(x, v.x), Math.Min(y, v.y))
  End Function

  Public Function Cart() As Vi2d
    Return New Vi2d(Math.Cos(y) * x, Math.Sin(y) * x)
  End Function

  Public Function Polar() As Vi2d
    Return New Vi2d(Mag(), Math.Atan2(y, x))
  End Function

  Public Function Clamp(v1 As Vi2d, v2 As Vi2d) As Vi2d
    Return [max](v1).min(v2)
  End Function

  Public Function Lerp(v1 As Vi2d, t As Double) As Vi2d
    Return Me * CInt(Fix(1.0F - t)) + (v1 * CInt(Fix(t)))
  End Function

  Public Function Dot(rhs As Vi2d) As Integer
    Return x * rhs.x + y * rhs.y
  End Function

  Public Function Cross(rhs As Vi2d) As Integer
    Return x * rhs.y - y * rhs.x
  End Function

  Public Shared Operator +(lhs As Vi2d, rhs As Vi2d) As Vi2d
    Return New Vi2d(lhs.x + rhs.x, lhs.y + rhs.y)
  End Operator

  Public Shared Operator -(left As Vi2d, right As Vi2d) As Vi2d
    Return New Vi2d(left.x - right.x, left.y - right.y)
  End Operator

  Public Shared Operator *(left As Vi2d, right As Integer) As Vi2d
    Return New Vi2d(left.x * right, left.y * right)
  End Operator

  Public Shared Operator *(left As Vi2d, right As Vi2d) As Vi2d
    Return New Vi2d(left.x * right.x, left.y * right.y)
  End Operator

  Public Shared Operator /(left As Vi2d, right As Integer) As Vi2d
    Return New Vi2d(left.x / right, left.y / right)
  End Operator

  Public Shared Operator /(left As Vi2d, right As Vi2d) As Vi2d
    Return New Vi2d(left.x / right.x, left.y / right.y)
  End Operator

  'Public Shared Operator +=(left As Vi2d, right As Vi2d) As Vi2d
  '  left.x += right.x
  '  left.y += right.y
  '  Return left
  'End Operator

  'Public Shared Operator -=(left As Vi2d, right As Vi2d) As Vi2d
  '  left.x -= right.x
  '  left.y -= right.y
  '  Return left
  'End Operator

  'Public Shared Operator *=(left As Vi2d, right As Integer) As Vi2d
  '  left.x *= right
  '  left.y *= right
  '  Return left
  'End Operator

  'Public Shared Operator /=(left As Vi2d, right As Integer) As Vi2d
  '  left.x /= right
  '  left.y /= right
  '  Return left
  'End Operator

  'Public Shared Operator *=(left As Vi2d, right As Vi2d) As Vi2d
  '  left.x *= right.x
  '  left.y *= right.y
  '  Return left
  'End Operator

  'Public Shared Operator /=(left As Vi2d, right As Vi2d) As Vi2d
  '  left.x /= right.x
  '  left.y /= right.y
  '  Return left
  'End Operator

  Public Shared Operator +(lhs As Vi2d) As Vi2d
    Return New Vi2d(+lhs.x, +lhs.y)
  End Operator

  Public Shared Operator -(lhs As Vi2d) As Vi2d
    Return New Vi2d(-lhs.x, -lhs.y)
  End Operator

  Public Shared Operator =(lhs As Vi2d, rhs As Vi2d) As Boolean
    Return (lhs.x = rhs.x AndAlso lhs.y = rhs.y)
  End Operator

  Public Shared Operator <>(lhs As Vi2d, rhs As Vi2d) As Boolean
    Return (lhs.x <> rhs.x OrElse lhs.y <> rhs.y)
  End Operator

  Public Function Str() As String
    Return $"({x},{y})"
  End Function

  Public Overrides Function ToString() As String
    Return Str()
  End Function

  'Public Shared Widening Operator CType(v As Vi2d) As Vi2d
  '  Return New Vi2d(v.x, v.y)
  'End Operator

  Public Shared Widening Operator CType(v As Vi2d) As Vf2d
    Return New Vf2d(v.x, v.y)
  End Operator

  'Public Shared Widening Operator CType(v As Vi2d) As v2d_generic(Of Double)
  '  Return New v2d_generic(Of Double)(v.x, v.y)
  'End Operator

End Structure