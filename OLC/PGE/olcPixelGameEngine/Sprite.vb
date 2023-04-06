Option Explicit On
Option Strict On
Option Infer On

Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.OSPlatform
Imports System.Runtime.InteropServices.RuntimeInformation

Public Class Sprite

#If OLC_DBG_OVERDRAW Then
  Private Shared nOverdrawCount As Integer
#End If

  Private pColData As Pixel()
  Private modeSample As Mode

  Public Enum Mode
    NORMAL
    PERIODIC
  End Enum

  Public Property Width As Integer
  Public Property Height As Integer

  Public Sub New()
    pColData = Nothing
    Width = 0
    Height = 0
  End Sub

  Public Sub New(sImageFile As String, Optional pack As ResourcePack = Nothing)
    LoadFromFile(sImageFile, pack)
  End Sub

  Public Sub New(w As Integer, h As Integer)
    If pColData IsNot Nothing Then
      Erase pColData
    End If
    Width = w
    Height = h
    pColData = New Pixel(Width * Height - 1) {}
    For i As Integer = 0 To Width * Height - 1
      pColData(i) = New Pixel()
    Next
  End Sub

  Protected Overrides Sub Finalize()
    If pColData IsNot Nothing Then
      Erase pColData
    End If
  End Sub

  Public Function LoadFromPGESprFile(sImageFile As String, Optional pack As ResourcePack = Nothing) As PixelGameEngine.RCode

    If pColData IsNot Nothing Then
      Erase pColData
    End If

    Dim ReadData As Action(Of Stream) = Sub(iis As Stream)
                                          Dim bytes As Byte() = New Byte(3) {}
                                          iis.Read(bytes, 0, 4)
                                          Width = BitConverter.ToInt32(bytes, 0)
                                          iis.Read(bytes, 0, 4)
                                          Height = BitConverter.ToInt32(bytes, 0)
                                          pColData = New Pixel(Width * Height - 1) {}
                                          bytes = New Byte(Width * Height * 4 - 1) {}
                                          iis.Read(bytes, 0, Width * Height * 4)
                                          Buffer.BlockCopy(bytes, 0, pColData, 0, Width * Height * 4)
                                        End Sub

    If pack Is Nothing Then
      Using ifs As New FileStream(sImageFile, FileMode.Open, FileAccess.Read)
        If ifs IsNot Nothing Then
          ReadData(ifs)
          Return PixelGameEngine.RCode.OK
        Else
          Return PixelGameEngine.RCode.FAIL
        End If
      End Using
    Else
      Dim rb As ResourceBuffer = pack.GetFileBuffer(sImageFile)
      Dim iss As New MemoryStream(rb.Data)
      ReadData(iss)
    End If

    Return PixelGameEngine.RCode.FAIL

  End Function

  Public Function SaveToPGESprFile(sImageFile As String) As PixelGameEngine.RCode

    If pColData Is Nothing Then
      Return PixelGameEngine.RCode.FAIL
    End If

    Using ofs As New FileStream(sImageFile, FileMode.Create, FileAccess.Write)
      ofs.Write(BitConverter.GetBytes(Width), 0, 4)
      ofs.Write(BitConverter.GetBytes(Height), 0, 4)
      Dim bytes As Byte() = New Byte(Width * Height * 4 - 1) {}
      Buffer.BlockCopy(pColData, 0, bytes, 0, Width * Height * 4)
      ofs.Write(bytes, 0, Width * Height * 4)
    End Using

    Return PixelGameEngine.RCode.OK

  End Function

  Function LoadFromFile(imageFile As String, Optional pack As ResourcePack = Nothing) As PixelGameEngine.RCode

    Dim bmp As Bitmap = Nothing

    If IsOSPlatform(Windows) Then
      If pack IsNot Nothing Then
        ' Load sprite from input stream
        Dim rb = pack.GetFileBuffer(imageFile)
        Dim ms = New MemoryStream(rb.vMemory.ToArray())
        bmp = DirectCast(Bitmap.FromStream(ms), Bitmap)
      Else
        ' Load sprite from file
        bmp = DirectCast(Bitmap.FromFile(imageFile), Bitmap)
      End If
    End If

    If bmp Is Nothing Then Return PixelGameEngine.RCode.NO_FILE
    If IsOSPlatform(Windows) Then
      Width = bmp.Width
      Height = bmp.Height
    End If
    pColData = New Pixel(Width * Height - 1) {}

    For x = 0 To Width - 1
      For y = 0 To Height - 1
        Dim c = Color.Black
        If IsOSPlatform(Windows) Then
          c = bmp.GetPixel(x, y)
        End If
        SetPixel(x, y, New Pixel(c.R, c.G, c.B, c.A))
      Next
    Next

    If IsOSPlatform(Windows) Then
      bmp.Dispose()
    End If

    Return PixelGameEngine.RCode.OK

  End Function

  Public Function SetPixel(x As Integer, y As Integer, p As Pixel) As Boolean
    If x >= 0 AndAlso x < Width AndAlso y >= 0 AndAlso y < Height Then
      pColData(y * Width + x) = p
      Return True
    Else
      Return False
    End If
  End Function

  Public Function GetPixel(x As Integer, y As Integer) As Pixel
    If modeSample = Mode.NORMAL Then
      If x >= 0 AndAlso x < Width AndAlso y >= 0 AndAlso y < Height Then
        Return pColData(y * Width + x)
      Else
        Return New Pixel(0, 0, 0, 0)
      End If
    Else
      Return pColData(Math.Abs(y Mod Height) * Width + Math.Abs(x Mod Width))
    End If
  End Function

  Public Function Sample(x As Single, y As Single) As Pixel
    Dim sx = Math.Min(CInt(Fix(x * Width)), Width - 1)
    Dim sy = Math.Min(CInt(Fix(y * Height)), Height - 1)
    Return GetPixel(sx, sy)
  End Function

  Public Function SampleBL(u As Single, v As Single) As Pixel

    u = u * Width - 0.5F
    v = v * Height - 0.5F
    Dim x = CInt(Fix(u))
    Dim y = CInt(Fix(v))
    Dim u_ratio = u - x
    Dim v_ratio = v - y
    Dim u_opposite = 1.0F - u_ratio
    Dim v_opposite = 1.0F - v_ratio

    Dim p1 = GetPixel(Math.Max(x, 0), Math.Max(y, 0))
    Dim p2 = GetPixel(Math.Min(x + 1, Width - 1), Math.Max(y, 0))
    Dim p3 = GetPixel(Math.Max(x, 0), Math.Min(y + 1, Height - 1))
    Dim p4 = GetPixel(Math.Min(x + 1, Width - 1), Math.Min(y + 1, Height - 1))

    Return New Pixel(CByte((p1.R * u_opposite + p2.R * u_ratio) * v_opposite + (p3.R * u_opposite + p4.R * u_ratio) * v_ratio),
                     CByte((p1.G * u_opposite + p2.G * u_ratio) * v_opposite + (p3.G * u_opposite + p4.G * u_ratio) * v_ratio),
                     CByte((p1.B * u_opposite + p2.B * u_ratio) * v_opposite + (p3.B * u_opposite + p4.B * u_ratio) * v_ratio))

  End Function

  Public ReadOnly Property GetData() As Pixel()
    Get
      Return pColData
    End Get
  End Property

  '*********

  'Private m_colorData As Pixel()

  'Public Sub New(w As Integer, h As Integer)
  '  m_colorData = Nothing
  '  Width = w
  '  Height = h
  'End Sub

  'Private Sub LoadFromBitmap(bmp As Bitmap)

  '  Dim depth As Integer
  '  Dim scan0 As IntPtr
  '  Dim stride As Integer
  '  Dim bmpData As BitmapData

  '  If IsOSPlatform(OSPlatform.Windows) Then

  '    Width = bmp.Width
  '    Height = bmp.Height
  '    m_colorData = New Pixel(Width * Height - 1) {}

  '    Dim rect = New Rectangle(0, 0, bmp.Width, bmp.Height)
  '    bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat)

  '    depth = Image.GetPixelFormatSize(bmp.PixelFormat)
  '    scan0 = bmpData.Scan0
  '    stride = bmpData.Stride

  '  End If

  '  Dim bytesPerPixel = depth \ 8

  '  For y = 0 To Height - 1
  '    For x = 0 To Width - 1

  '      Dim pixelOffset = y * stride + x * bytesPerPixel

  '      Dim c = Color.Empty

  '      Select Case depth
  '        Case 32
  '          Dim b = Marshal.ReadByte(scan0, pixelOffset)
  '          Dim g = Marshal.ReadByte(scan0, pixelOffset + 1)
  '          Dim r = Marshal.ReadByte(scan0, pixelOffset + 2)
  '          Dim a = Marshal.ReadByte(scan0, pixelOffset + 3)
  '          c = Color.FromArgb(a, r, g, b)

  '        Case 24
  '          Dim b = Marshal.ReadByte(scan0, pixelOffset)
  '          Dim g = Marshal.ReadByte(scan0, pixelOffset + 1)
  '          Dim r = Marshal.ReadByte(scan0, pixelOffset + 2)
  '          c = Color.FromArgb(r, g, b)

  '        Case 8
  '          Dim b = Marshal.ReadByte(scan0, pixelOffset)
  '          c = Color.FromArgb(b, b, b)

  '      End Select

  '      Me(x, y) = New Pixel(c.R, c.G, c.B, c.A)

  '    Next

  '  Next

  '  If IsOSPlatform(OSPlatform.Windows) Then
  '    bmp.UnlockBits(bmpData)
  '  End If

  'End Sub

  'Private Shared Function LoadFromSpr(path As String) As Sprite

  '  Dim parse As Func(Of Short, Pixel) = Function(col As Short) As Pixel
  '                                         Select Case col And &HF
  '                                           Case &H0 : Return Pixel.Presets.Black
  '                                           Case &H1 : Return Pixel.Presets.DarkBlue
  '                                           Case &H2 : Return Pixel.Presets.DarkGreen
  '                                           Case &H3 : Return Pixel.Presets.DarkCyan
  '                                           Case &H4 : Return Pixel.Presets.DarkRed
  '                                           Case &H5 : Return Pixel.Presets.DarkMagenta
  '                                           Case &H6 : Return Pixel.Presets.DarkYellow
  '                                           Case &H7 : Return Pixel.Presets.Grey
  '                                           Case &H8 : Return Pixel.Presets.DarkGrey
  '                                           Case &H9 : Return Pixel.Presets.Blue
  '                                           Case &HA : Return Pixel.Presets.Green
  '                                           Case &HB : Return Pixel.Presets.Cyan
  '                                           Case &HC : Return Pixel.Presets.Red
  '                                           Case &HD : Return Pixel.Presets.Magenta
  '                                           Case &HE : Return Pixel.Presets.Yellow
  '                                           Case &HF : Return Pixel.Presets.White
  '                                         End Select
  '                                         Return Pixel.Empty
  '                                       End Function

  '  Dim spr As Sprite
  '  Using stream1 = File.OpenRead(path)
  '    Using reader = New BinaryReader(stream1)
  '      Dim w = reader.ReadInt32()
  '      Dim h = reader.ReadInt32()
  '      spr = New Sprite(w, h)
  '      For i = 0 To h - 1
  '        For j = 0 To w - 1
  '          spr(j, i) = parse(reader.ReadInt16())
  '        Next
  '      Next
  '    End Using
  '  End Using

  '  Return spr

  'End Function

  'Public Shared Function Load(path As String) As Sprite
  '  If Not File.Exists(path) Then
  '    Return New Sprite(8, 8)
  '  End If
  '  If path.EndsWith(".spr") Then
  '    Return LoadFromSpr(path)
  '  Else
  '    Using bmp = CType(Image.FromFile(path), Bitmap)
  '      Dim spr As New Sprite(0, 0)
  '      spr.LoadFromBitmap(bmp)
  '      Return spr
  '    End Using
  '  End If
  'End Function

  'Public Shared Sub Save(spr As Sprite, path As String)
  '  Using bmp As New Bitmap(spr.Width, spr.Height)
  '    For x = 0 To spr.Width - 1
  '      For y = 0 To spr.Height - 1
  '        Dim p = spr(x, y)
  '        bmp.SetPixel(x, y, Color.FromArgb(p.A, p.R, p.G, p.B))
  '      Next
  '    Next
  '    bmp.Save(path)
  '  End Using
  'End Sub

  'Public Shared Sub Copy(src As Sprite, dest As Sprite)
  '  If src.m_colorData.Length <> dest.m_colorData.Length Then
  '    Return
  '  End If
  '  src.m_colorData.CopyTo(dest.m_colorData, 0)
  'End Sub

  'Default Public Property Item(x As Integer, y As Integer) As Pixel
  '  Get
  '    Return GetPixel(x, y)
  '  End Get
  '  Set(value As Pixel)
  '    Call SetPixel(x, y, value)
  '  End Set
  'End Property

  'Private Function GetPixel(x As Integer, y As Integer) As Pixel
  '  If x >= 0 AndAlso x < Width AndAlso y >= 0 AndAlso y < Height Then
  '    Return m_colorData(y * Width + x)
  '  Else Return Pixel.Empty
  '  End If
  'End Function

  'Private Sub SetPixel(x As byte, y As Integer, p As Pixel)
  '  If x >= 0 AndAlso x < Width AndAlso y >= 0 AndAlso y < Height Then
  '    m_colorData(y * Width + x) = p
  '  End If
  'End Sub

  'Friend Function GetData() As Pixel()
  '  Return m_colorData
  'End Function

End Class