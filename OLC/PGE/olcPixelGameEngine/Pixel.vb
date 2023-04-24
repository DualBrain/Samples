Option Explicit On
Option Strict On
Option Infer On

Imports System.Runtime.InteropServices

Public Enum Presets As UInteger
  White = &HFFFFFF
  Grey = &HA9A9A9
  Red = &HFF0000
  Yellow = &HFFFF00
  Green = &HFF00
  Cyan = &HFFFF
  Blue = &HFF
  Magenta = &HFF00FF
  Brown = &H9A6324
  Orange = &HF58231
  Purple = &H911EB4
  Lime = &HBFEF45
  Pink = &HFABEBE
  Snow = &HFFFAFA
  Teal = &H469990
  Lavender = &HE6BEFF
  Beige = &HFFFAC8
  Maroon = &H800000
  Mint = &HAAFFC3
  Olive = &H808000
  Apricot = &HFFD8B1
  Navy = &H75
  Black = &H0
  DarkGrey = &H8B8B8B
  DarkRed = &H8B0000
  DarkYellow = &H8B8B00
  DarkGreen = &H8B00
  DarkCyan = &H8B8B
  DarkBlue = &H8B
  DarkMagenta = &H8B008B
End Enum

<StructLayout(LayoutKind.Explicit)>
Public Structure Pixel
  <FieldOffset(0)> Public N As Integer
  <FieldOffset(1)> Public R As Byte
  <FieldOffset(2)> Public G As Byte
  <FieldOffset(3)> Public B As Byte
  <FieldOffset(4)> Public A As Byte

  Public Sub New(red As Single, green As Single, blue As Single, Optional alpha As Single = 255.0F)
    R = CByte(Fix(red))
    G = CByte(Fix(green))
    B = CByte(Fix(blue))
    A = CByte(Fix(alpha))
  End Sub

  Public Sub New(red As Integer, green As Integer, blue As Integer, Optional alpha As Integer = 255)
    R = CByte(red)
    G = CByte(green)
    B = CByte(blue)
    A = CByte(alpha)
  End Sub

  Public Enum Mode
    Normal
    Mask
    Alpha
    Custom
  End Enum

  Public Shared Function Random() As Pixel
    Dim vals = RandomBytes(3)
    Return New Pixel(vals(0), vals(1), vals(2))
  End Function

  Public Shared Function RandomAlpha() As Pixel
    Dim vals = RandomBytes(4)
    Return New Pixel(vals(0), vals(1), vals(2), vals(3))
  End Function

#Region "Presets"

  Public Shared ReadOnly Empty As New Pixel(0, 0, 0, 0)

  Private Shared ReadOnly m_presetPixels As Dictionary(Of Presets, Pixel)

  Public Shared ReadOnly Property PresetPixels As Pixel()
    Get
      Return m_presetPixels.Values.ToArray()
    End Get
  End Property

#End Region

  Public Shared Function FromRgb(rgb As UInteger) As Pixel
    Dim a1 = CByte((rgb And &HFF))
    Dim b1 = CByte(((rgb >> 8) And &HFF))
    Dim g1 = CByte(((rgb >> 16) And &HFF))
    Dim r1 = CByte(((rgb >> 24) And &HFF))
    Return New Pixel(r1, g1, b1, a1)
  End Function

  Public Shared Function FromHsv(h As Single, s As Single, v As Single) As Pixel

    Dim c = v * s
    Dim nh = (h / 60) Mod 6
    Dim x = c * (1 - Math.Abs(nh Mod 2 - 1))
    Dim m = v - c

    Dim r1, g1, b1 As Single

    If 0 <= nh AndAlso nh < 1 Then
      r1 = c : g1 = x : b1 = 0
    ElseIf 1 <= nh AndAlso nh < 2 Then
      r1 = x : g1 = c : b1 = 0
    ElseIf 2 <= nh AndAlso nh < 3 Then
      r1 = 0 : g1 = c : b1 = x
    ElseIf 3 <= nh AndAlso nh < 4 Then
      r1 = 0 : g1 = x : b1 = c
    ElseIf 4 <= nh AndAlso nh < 5 Then
      r1 = x : g1 = 0 : b1 = c
    ElseIf 5 <= nh AndAlso nh < 6 Then
      r1 = c : g1 = 0 : b1 = x
    Else
      r1 = 0 : g1 = 0 : b1 = 0
    End If

    r1 += m : g1 += m : b1 += m

    Return New Pixel(CByte(Fix(r1 * 255)), CByte(Fix(g1 * 255)), CByte(Fix(b1 * 255)))

  End Function

  Shared Sub New()

    Dim ToPixel As Func(Of Presets, Pixel) = Function(p)
                                               Dim hex = p.ToString("X")
                                               Dim r = Convert.ToByte(hex.Substring(2, 2), 16)
                                               Dim g = Convert.ToByte(hex.Substring(4, 2), 16)
                                               Dim b = Convert.ToByte(hex.Substring(6, 2), 16)
                                               Return New Pixel(r, g, b)
                                             End Function

    Dim presets As Presets() = DirectCast([Enum].GetValues(GetType(Presets)), Presets())
    m_presetPixels = presets.ToDictionary(Function(p) p, Function(p) ToPixel(p))

  End Sub

  Public Shared Operator =(a1 As Pixel, b1 As Pixel) As Boolean
    Return (a1.R = b1.R) AndAlso (a1.G = b1.G) AndAlso (a1.B = b1.B) AndAlso (a1.A = b1.A)
  End Operator

  Public Shared Operator <>(a1 As Pixel, b1 As Pixel) As Boolean
    Return Not (a1 = b1)
  End Operator

  Public Shared Widening Operator CType(p As Presets) As Pixel
    Dim pix As Pixel = Nothing
    If m_presetPixels.TryGetValue(p, pix) Then
      Return pix
    End If
    Return Empty
  End Operator

  Public Overrides Function Equals(obj As Object) As Boolean
    If TypeOf obj Is Pixel Then
      Return Me = CType(obj, Pixel)
    End If
    Return False
  End Function

  Public Overrides Function GetHashCode() As Integer
    Dim hashCode = 196078
    hashCode = hashCode * -152113 + R.GetHashCode()
    hashCode = hashCode * -152113 + G.GetHashCode()
    hashCode = hashCode * -152113 + B.GetHashCode()
    hashCode = hashCode * -152113 + A.GetHashCode()
    Return hashCode
  End Function

End Structure