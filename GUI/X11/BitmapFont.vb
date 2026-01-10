Module BitmapFont

  ' 6x8 font for digits 0–9
  Public ReadOnly FontDigits()() As Byte = {
    New Byte() {60, 102, 102, 102, 102, 60},    ' 0
    New Byte() {24, 56, 24, 24, 24, 126},      ' 1
    New Byte() {60, 102, 12, 24, 48, 126},     ' 2
    New Byte() {60, 102, 28, 12, 102, 60},     ' 3
    New Byte() {12, 28, 44, 126, 12, 12},      ' 4
    New Byte() {126, 96, 124, 6, 102, 60},     ' 5
    New Byte() {28, 48, 124, 102, 102, 60},    ' 6
    New Byte() {126, 6, 12, 24, 48, 48},       ' 7
    New Byte() {60, 102, 60, 102, 102, 60},    ' 8
    New Byte() {60, 102, 102, 62, 12, 56}      ' 9
  }

'   ' Draw a single digit to framebuffer at (x, y) with color
'   Public Sub DrawDigit(digit As Integer, x As Integer, y As Integer, color As Integer)
'     If digit < 0 OrElse digit > 9 Then Return
'     Dim map = FontDigits(digit)
'     For col = 0 To 5
'       Dim bits = map(col)
'       For row = 0 To 7
'         If (bits And (1 << (7 - row))) <> 0 Then
'           FrameBuffer.SetPixel(x + col, y + row, color)
'         End If
'       Next
'     Next
'   End Sub

'   ' Draw an integer number to framebuffer at (x, y) with color
'   Public Sub DrawNumber(value As Integer, x As Integer, y As Integer, color As Integer)
'     Dim str = value.ToString()
'     For i = 0 To str.Length - 1
'       Dim digit = Asc(str(i)) - Asc("0"c)
'       DrawDigit(digit, x + i * 6, y, color)
'     Next
'   End Sub

  ' Draw a single digit to framebuffer at (x, y) with color and scale
'   Public Sub DrawDigit(digit As Integer, x As Integer, y As Integer, color As Integer, Optional scale As Integer = 1)
'     If digit < 0 OrElse digit > 9 Then Return
'     Dim map = FontDigits(digit)
'     For col = 0 To 5
'       Dim bits = map(col)
'       For row = 0 To 7
'         If (bits And (1 << (7 - row))) <> 0 Then
'           ' Draw a square block of size scale x scale
'           For dx = 0 To scale - 1
'             For dy = 0 To scale - 1
'               FrameBuffer.SetPixel(x + col * scale + dx, y + row * scale + dy, color)
'             Next
'           Next
'         End If
'       Next
'     Next
'   End Sub

' Public Sub DrawDigit(digit As Integer, x As Integer, y As Integer, color As Integer, Optional scale As Integer = 1)
'     If digit < 0 OrElse digit > 9 Then Return
'     Dim map = FontDigits(digit)

'     For col = 0 To 5
'         Dim bits = map(col)
'         For row = 0 To 7
'             If (bits And (1 << (7 - row))) <> 0 Then
'                 ' Draw a scale x scale block
'                 Dim px = x + col * scale
'                 Dim py = y + row * scale
'                 For dx = 0 To scale - 1
'                     For dy = 0 To scale - 1
'                         FrameBuffer.SetPixel(px + dx, py + dy, color)
'                     Next
'                 Next
'             End If
'         Next
'     Next
' End Sub

' Public Sub DrawDigit(digit As Integer, x As Integer, y As Integer, color As Integer, Optional scale As Integer = 1)
'     If digit < 0 OrElse digit > 9 Then Return
'     Dim map = FontDigits(digit)

'     For col = 0 To 5
'         Dim bits = map(col)
'         For row = 0 To 7
'             If (bits And (1 << row)) <> 0 Then
'                 Dim px = x + col * scale
'                 Dim py = y + row * scale
'                 For dx = 0 To scale - 1
'                     For dy = 0 To scale - 1
'                         FrameBuffer.SetPixel(px + dx, py + dy, color)
'                     Next
'                 Next
'             End If
'         Next
'     Next
' End Sub

' Public Sub DrawDigit(digit As Integer, x As Integer, y As Integer, color As Integer, Optional scale As Integer = 1)
'     If digit < 0 OrElse digit > 9 Then Return
'     Dim map = FontDigits(digit)

'     ' 6 columns, 8 rows
'     For col = 0 To 5
'         Dim bits = map(col)
'         For row = 0 To 7
'             If (bits And (1 << row)) <> 0 Then
'                 ' Rotate 90° clockwise:
'                 ' NewX = original Y
'                 ' NewY = width - original X
'                 Dim px = x + row * scale
'                 Dim py = y + (5 - col) * scale  ' 5 = last column index
'                 For dx = 0 To scale - 1
'                     For dy = 0 To scale - 1
'                         FrameBuffer.SetPixel(px + dx, py + dy, color)
'                     Next
'                 Next
'             End If
'         Next
'     Next
' End Sub

Public Sub DrawDigit(digit As Integer, x As Integer, y As Integer, color As Integer, Optional scale As Integer = 1)
    If digit < 0 OrElse digit > 9 Then Return
    Dim map = FontDigits(digit)

    For col = 0 To 5          ' width
        Dim bits = map(col)
        For row = 0 To 7      ' height
            If (bits And (1 << row)) <> 0 Then
                ' 90° clockwise rotation
                Dim px = x + (7 - row) * scale  ' invert row
                Dim py = y + col * scale        ' column becomes vertical
                For dx = 0 To scale - 1
                    For dy = 0 To scale - 1
                        FrameBuffer.SetPixel(px + dx, py + dy, color)
                    Next
                Next
            End If
        Next
    Next
End Sub


'   ' Draw an integer number to framebuffer at (x, y) with color and scale
'   Public Sub DrawNumber(value As Integer, x As Integer, y As Integer, color As Integer, Optional scale As Integer = 1)
'     Dim str = value.ToString()
'     For i = 0 To str.Length - 1
'       Dim digit = Asc(str(i)) - Asc("0"c)
'       DrawDigit(digit, x + i * 6 * scale, y, color, scale)
'     Next
'   End Sub

' Public Sub DrawNumber(value As Integer, x As Integer, y As Integer, color As Integer, Optional scale As Integer = 1)
' Dim str = value.ToString()
' Dim posX = x
' For i = 0 To str.Length - 1
'     Dim digit = Asc(str(i)) - Asc("0"c)
'     DrawDigit(digit, posX, y, color, scale)
'     posX += (6 + 1) * scale  ' add 1 pixel gap
' Next
' End Sub

' Public Sub DrawNumber(value As Integer, x As Integer, y As Integer, color As Integer, Optional scale As Integer = 1)
'     Dim str = value.ToString()
'     Dim posX = x
'     For i = 0 To str.Length - 1
'         Dim digit = Asc(str(i)) - Asc("0"c)
'         DrawDigit(digit, posX, y, color, scale)
'         posX += (8) * scale  ' 6 columns + 2 pixels gap for readability
'     Next
' End Sub

Public Sub DrawNumber(value As Integer, x As Integer, y As Integer, color As Integer, Optional scale As Integer = 1)
    Dim str = value.ToString()
    Dim posX = x
    For i = 0 To str.Length - 1
        Dim digit = Asc(str(i)) - Asc("0"c)
        DrawDigit(digit, posX, y, color, scale)
        posX += (8) * scale  ' 6 columns + 2 pixels gap
    Next
End Sub


End Module
