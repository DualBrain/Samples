Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports OpenCvSharp
'Imports System.Drawing

Module Program

  Sub Main() 'args As String())
    Dim game As New Webcam
    game.ConstructConsole(320, 240, 4, 4)
    game.Start()
  End Sub

End Module

Class Webcam
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_capture As VideoCapture
  Private m_frame As Mat

  Public Overrides Function OnUserCreate() As Boolean

    Console.Title = "Initializing MAT"
    m_frame = New Mat()
    Console.Title = "Initializing VideoCapture"
    m_capture = New VideoCapture(0)
    Console.Title = "Configuring VideoCapture"
    m_capture.Set(VideoCaptureProperties.FrameWidth, 640) 'ScreenWidth)
    m_capture.Set(VideoCaptureProperties.FrameHeight, 480) 'ScreenHeight)
    m_capture.Set(VideoCaptureProperties.Zoom, 0)
    'm_capture.FrameHeight = ScreenHeight()
    'm_capture.FrameWidth = ScreenWidth()
    Console.Title = "Opening VideoCapture"
    m_capture?.Open(0)
    Console.Title = "Startup completed..."
    Return True

  End Function

  'Private Shared Function MatToImageArray(mat As Mat) As Integer()
  '  Dim w = mat.Width
  '  Dim h = mat.Height
  '  Dim imageData = mat.ToBytes(".bmp")
  '  Dim paddingSize = (4 - ((w * 3) Mod 4)) Mod 4
  '  Dim rgbData((w * h) - 1) As Integer
  '  Dim index = 0
  '  For y = h - 1 To 0 Step -1
  '    For x = 0 To w - 1
  '      Dim pixelIndex = (y * (w * 3 + paddingSize)) + (x * 3)
  '      Dim b = imageData(pixelIndex)
  '      Dim g = imageData(pixelIndex + 1)
  '      Dim r = imageData(pixelIndex + 2)
  '      Dim rgb = (r << 16) Or (g << 8) Or b
  '      rgbData(index) = rgb : index += 1
  '    Next
  '  Next
  '  Return rgbData
  'End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    If m_capture?.IsOpened() Then

      Dim sw = ScreenWidth()
      Dim sh = ScreenHeight()

      m_capture.Read(m_frame)

      Cv2.Resize(m_frame, m_frame, New Size(sw, sh))
      Cv2.Flip(m_frame, m_frame, FlipMode.Y)

      Dim indexer = m_frame.GetGenericIndexer(Of Vec3b)

      'Dim mat3 As New Mat(Of Vec3b)(m_frame) ' not working, and don't understand why
      'Dim indexer = mat3.GetIndexer()

      Dim fw = m_frame.Width
      Dim fh = m_frame.Height

      For x = 0 To fw - 1 'sw - 1
        For y = 0 To fh - 1 'sh - 1

          ' Get Pixel

          Dim color = indexer(y, x)
          Dim r = color.Item2 / 255.0F
          Dim g = color.Item1 / 255.0F
          Dim b = color.Item0 / 255.0F

          ' Convert into char / color combo 

          Dim sym = 0
          Dim bg_col = 0
          Dim fg_col = 0

          ClassifyPixel_Grey(r, g, b, sym, bg_col, fg_col)
          'ClassifyPixel_HSL(r, g, b, sym, bg_col, fg_col)
          'ClassifyPixel_OLC(r, g, b, sym, bg_col, fg_col)

          ' Draw Pixel
          Draw(x, y, sym, bg_col Or fg_col)

        Next
      Next
    End If

    Return True

  End Function

  Private Shared Sub ClassifyPixel_Grey(r As Single, g As Single, b As Single, ByRef ch As Integer, ByRef fg As Integer, ByRef bg As Integer)

    Dim luminance = 0.2987 * r + 0.587 * g + 0.114 * b
    Dim pixel_bw = CInt(Fix(luminance * 13))
    Select Case pixel_bw
      Case 0 : bg = BG_BLACK : fg = FG_BLACK : ch = PIXEL_SOLID

      Case 1 : bg = BG_BLACK : fg = FG_DARK_GREY : ch = PIXEL_QUARTER
      Case 2 : bg = BG_BLACK : fg = FG_DARK_GREY : ch = PIXEL_HALF
      Case 3 : bg = BG_BLACK : fg = FG_DARK_GREY : ch = PIXEL_THREEQUARTERS
      Case 4 : bg = BG_BLACK : fg = FG_DARK_GREY : ch = PIXEL_SOLID

      Case 5 : bg = BG_GREY : fg = FG_GREY : ch = PIXEL_QUARTER
      Case 6 : bg = BG_GREY : fg = FG_GREY : ch = PIXEL_HALF
      Case 7 : bg = BG_GREY : fg = FG_GREY : ch = PIXEL_THREEQUARTERS
      Case 8 : bg = BG_GREY : fg = FG_GREY : ch = PIXEL_SOLID

      Case 9 : bg = BG_GREY : fg = FG_WHITE : ch = PIXEL_QUARTER
      Case 10 : bg = BG_GREY : fg = FG_WHITE : ch = PIXEL_HALF
      Case 11 : bg = BG_GREY : fg = FG_WHITE : ch = PIXEL_THREEQUARTERS
      Case 12 : bg = BG_GREY : fg = FG_WHITE : ch = PIXEL_SOLID

    End Select

  End Sub

  ' Define RGB class
  Private Class RGB
    Public r As Single    ' a fraction between 0 and 1
    Public g As Single    ' a fraction between 0 and 1
    Public b As Single    ' a fraction between 0 and
    Public Sub New()
    End Sub
    Public Sub New(r As Single, g As Single, b As Single)
      Me.r = r
      Me.g = g
      Me.b = b
    End Sub
  End Class

  ' Define HSV class
  Private Class HSV
    Public h As Single    ' angle in degrees
    Public s As Single    ' a fraction between 0 and 1
    Public v As Single    ' a fraction between 0 and 1
  End Class

  ' Define rgb2hsv function
  Private Shared Function rgb2hsv(inRgb As RGB) As HSV

    Dim outHsv As New HSV()
    Dim minVal As Single, maxVal As Single, delta As Single

    minVal = Math.Min(inRgb.r, Math.Min(inRgb.g, inRgb.b))
    maxVal = Math.Max(inRgb.r, Math.Max(inRgb.g, inRgb.b))

    outHsv.v = maxVal                               ' v
    delta = maxVal - minVal
    If delta < 0.00001F Then
      outHsv.s = 0
      outHsv.h = 0 ' undefined, maybe nan?
      Return outHsv
    End If
    If maxVal > 0.0 Then
      outHsv.s = (delta / maxVal)                  ' s
    Else
      ' if maxVal is 0, then r = g = b = 0              
      ' s = 0, h is undefined
      outHsv.s = 0.0
      outHsv.h = Single.NaN                         ' its now undefined
      Return outHsv
    End If
    If inRgb.r >= maxVal Then                           ' > is bogus, just keeps compiler happy
      outHsv.h = (inRgb.g - inRgb.b) / delta          ' between yellow & magenta
    ElseIf inRgb.g >= maxVal Then
      outHsv.h = 2.0F + (inRgb.b - inRgb.r) / delta   ' between cyan & yellow
    Else
      outHsv.h = 4.0F + (inRgb.r - inRgb.g) / delta   ' between magenta & cyan
    End If

    outHsv.h *= 60.0F                                 ' degrees

    If outHsv.h < 0.0F Then
      outHsv.h += 360.0F
    End If

    Return outHsv

  End Function

  Private Shared Sub ClassifyPixel_HSL(r As Single, g As Single, b As Single, ByRef sym As Integer, ByRef fg_col As Integer, ByRef bg_col As Integer)

    Dim col As HSV = rgb2hsv(New RGB(r, g, b))

    Dim hues() As (c As Integer, fg As Integer, bg As Integer) =
    {
        (PIXEL_SOLID, FG_RED, BG_RED),
        (PIXEL_QUARTER, FG_YELLOW, BG_RED),
        (PIXEL_HALF, FG_YELLOW, BG_RED),
        (PIXEL_THREEQUARTERS, FG_YELLOW, BG_RED),
        (PIXEL_SOLID, FG_GREEN, BG_YELLOW),
        (PIXEL_QUARTER, FG_GREEN, BG_YELLOW),
        (PIXEL_HALF, FG_GREEN, BG_YELLOW),
        (PIXEL_THREEQUARTERS, FG_GREEN, BG_YELLOW),
        (PIXEL_SOLID, FG_CYAN, BG_GREEN),
        (PIXEL_QUARTER, FG_CYAN, BG_GREEN),
        (PIXEL_HALF, FG_CYAN, BG_GREEN),
        (PIXEL_THREEQUARTERS, FG_CYAN, BG_GREEN),
        (PIXEL_SOLID, FG_BLUE, BG_CYAN),
        (PIXEL_QUARTER, FG_BLUE, BG_CYAN),
        (PIXEL_HALF, FG_BLUE, BG_CYAN),
        (PIXEL_THREEQUARTERS, FG_BLUE, BG_CYAN),
        (PIXEL_SOLID, FG_MAGENTA, BG_BLUE),
        (PIXEL_QUARTER, FG_MAGENTA, BG_BLUE),
        (PIXEL_HALF, FG_MAGENTA, BG_BLUE),
        (PIXEL_THREEQUARTERS, FG_MAGENTA, BG_BLUE),
        (PIXEL_SOLID, FG_RED, BG_MAGENTA),
        (PIXEL_QUARTER, FG_RED, BG_MAGENTA),
        (PIXEL_HALF, FG_RED, BG_MAGENTA),
        (PIXEL_THREEQUARTERS, FG_RED, BG_MAGENTA)
    }

    Dim index = CInt(Fix((col.h / 360.0F) * 24.0F))

    If col.s > 0.2F Then
      sym = hues(index).c
      fg_col = hues(index).fg
      bg_col = hues(index).bg
    Else
      ClassifyPixel_Grey(r, g, b, sym, fg_col, bg_col)
    End If

  End Sub

  Sub ClassifyPixel_OLC(r As Single, g As Single, b As Single, ByRef sym As Integer, ByRef fg_col As Integer, ByRef bg_col As Integer)

    ' Is pixel coloured (i.e. RGB values exhibit significant variance)
    Dim mean = (r + g + b) / 3.0F
    Dim rVar = (r - mean) * (r - mean)
    Dim gVar = (g - mean) * (g - mean)
    Dim bVar = (b - mean) * (b - mean)
    Dim variance = rVar + gVar + bVar

    If variance < 0.0001F Then
      ClassifyPixel_Grey(r, g, b, sym, fg_col, bg_col)
    Else

      ' Pixel has colour so get dominant colour
      Dim y = Math.Min(r, g)
      Dim c = Math.Min(g, b)
      Dim m = Math.Min(b, r)

      Dim mean2 = (y + c + m) / 3.0F
      Dim yVar = (y - mean2) * (y - mean2)
      Dim cVar = (c - mean2) * (c - mean2)
      Dim mVar = (m - mean2) * (m - mean2)

      Dim maxPrimaryVar = Math.Max(rVar, gVar)
      maxPrimaryVar = Math.Max(maxPrimaryVar, bVar)

      Dim maxSecondaryVar = Math.Max(cVar, yVar)
      maxSecondaryVar = Math.Max(maxSecondaryVar, mVar)

      Dim shading = 0.5F

      If rVar = maxPrimaryVar AndAlso yVar = maxSecondaryVar Then
        Compare(rVar, yVar, r, y, FG_RED, FG_DARK_RED, BG_YELLOW, BG_DARK_YELLOW, shading, sym, fg_col, bg_col)
      End If

      If rVar = maxPrimaryVar AndAlso mVar = maxSecondaryVar Then
        Compare(rVar, mVar, r, m, FG_RED, FG_DARK_RED, BG_MAGENTA, BG_DARK_MAGENTA, shading, sym, fg_col, bg_col)
      End If

      If rVar = maxPrimaryVar AndAlso cVar = maxSecondaryVar Then
        Compare(rVar, cVar, r, c, FG_RED, FG_DARK_RED, BG_CYAN, BG_DARK_CYAN, shading, sym, fg_col, bg_col)
      End If

      If gVar = maxPrimaryVar AndAlso yVar = maxSecondaryVar Then
        Compare(gVar, yVar, g, y, FG_GREEN, FG_DARK_GREEN, BG_YELLOW, BG_DARK_YELLOW, shading, sym, fg_col, bg_col)
      End If

      If gVar = maxPrimaryVar AndAlso cVar = maxSecondaryVar Then
        Compare(gVar, cVar, g, c, FG_GREEN, FG_DARK_GREEN, BG_CYAN, BG_DARK_CYAN, shading, sym, fg_col, bg_col)
      End If

      If gVar = maxPrimaryVar AndAlso mVar = maxSecondaryVar Then
        Compare(gVar, mVar, g, m, FG_GREEN, FG_DARK_GREEN, BG_MAGENTA, BG_DARK_MAGENTA, shading, sym, fg_col, bg_col)
      End If

      If bVar = maxPrimaryVar AndAlso mVar = maxSecondaryVar Then
        Compare(bVar, mVar, b, m, FG_BLUE, FG_DARK_BLUE, BG_MAGENTA, BG_DARK_MAGENTA, shading, sym, fg_col, bg_col)
      End If

      If bVar = maxPrimaryVar AndAlso cVar = maxSecondaryVar Then
        Compare(bVar, cVar, b, c, FG_BLUE, FG_DARK_BLUE, BG_CYAN, BG_DARK_CYAN, shading, sym, fg_col, bg_col)
      End If

      If bVar = maxPrimaryVar AndAlso yVar = maxSecondaryVar Then
        Compare(bVar, yVar, b, y, FG_BLUE, FG_DARK_BLUE, BG_YELLOW, BG_DARK_YELLOW, shading, sym, fg_col, bg_col)
      End If

    End If

  End Sub

  Private Sub Compare(fV1 As Single, fV2 As Single, fC1 As Single, fC2 As Single, FG_LIGHT As Integer, FG_DARK As Integer, BG_LIGHT As Integer, BG_DARK As Integer,
                      fshading As Single, ByRef sym As Integer, ByRef fg_col As Integer, ByRef bg_col As Integer)
    If fV1 >= fV2 Then
      'Primary Is Dominant, Use in foreground
      fg_col = If(fC1 > 0.5F, FG_LIGHT, FG_DARK)

      If fV2 < 0.0001F Then
        'Secondary is not variant, Use greyscale in background
        If fC2 >= 0.00F AndAlso fC2 < 0.25F Then
          bg_col = BG_BLACK
        End If
        If fC2 >= 0.25F AndAlso fC2 < 0.5F Then
          bg_col = BG_DARK_GREY
        End If
        If fC2 >= 0.5F AndAlso fC2 < 0.75F Then
          bg_col = BG_GREY
        End If
        If fC2 >= 0.75F AndAlso fC2 <= 1.0F Then
          bg_col = BG_WHITE
        End If
      Else
        'Secondary is variant, Use in background
        bg_col = If(fC2 > 0.5F, BG_LIGHT, BG_DARK)
      End If

      'Shade dominant over background (100% -> 0%)
      fshading = ((fC1 - fC2) / 2.0F) + 0.5F
    End If

    If fshading >= 0.00F AndAlso fshading < 0.2F Then
      sym = AscW(" "c)
    End If
    If fshading >= 0.2F AndAlso fshading < 0.4F Then
      sym = PIXEL_QUARTER
    End If
    If fshading >= 0.4F AndAlso fshading < 0.6F Then
      sym = PIXEL_HALF
    End If
    If fshading >= 0.6F AndAlso fshading < 0.8F Then
      sym = PIXEL_THREEQUARTERS
    End If
    If fshading >= 0.8F Then
      sym = PIXEL_SOLID
    End If
  End Sub

  'Private Shared Function MatToBitmap(mat As Mat) As Bitmap
  '  Using ms = mat.ToMemoryStream()
  '    If OperatingSystem.IsWindows Then
  '      Return CType(Image.FromStream(ms), Bitmap)
  '    Else
  '      Return Nothing
  '    End If
  '  End Using
  'End Function

End Class
