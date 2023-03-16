Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports OpenCvSharp
Imports System.Drawing

Module Program

  Sub Main() 'args As String())
    Dim game As New Webcam
    game.ConstructConsole(640, 480, 2, 2)
    game.Start()
  End Sub

End Module

Class Webcam
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_capture As VideoCapture
  Private m_frame As Mat
  Private m_image As Bitmap

  Public Overrides Function OnUserCreate() As Boolean

    m_frame = New Mat()
    m_capture = New VideoCapture(0)
    m_capture.FrameHeight = ScreenHeight()
    m_capture.FrameWidth = ScreenWidth()
    m_capture?.Open(0)

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    If m_capture?.IsOpened() Then
      m_capture.Read(m_frame)
      m_image = MatToBitmap(m_frame)
      For x = 0 To ScreenWidth() - 1
        For y = 0 To ScreenHeight() - 1

          ' Get Pixel

          Dim c = m_image.GetPixel(x, y)
          Dim r = c.R / 255
          Dim g = c.G / 255
          Dim b = c.B / 255

          ' Convert into char / color combo 

          Dim sym = 0
          Dim bg_col = 0
          Dim fg_col = 0

          ClassifyPixel_Grey(r, g, b, sym, bg_col, fg_col)
          'ClassifyPixel_HSL(r, g, b, sym, bg_col, fg_col)
          'ClassifyPixel_OLC(r, g, b, sym, bg_col, fg_col)

          ' Draw Pixel
          Draw(x, y, sym, bg_col Or fg_col)
          'If g > 0.5 Then
          '  Draw(x, y, PIXEL_SOLID, FG_WHITE)
          'Else
          '  Draw(x, y, PIXEL_SOLID, FG_BLACK)
          'End If

        Next
      Next
    End If

    Return True

  End Function

  Private Shared Sub ClassifyPixel_Grey(r As Double, g As Double, b As Double, ByRef ch As Integer, ByRef fg As Integer, ByRef bg As Integer)

    Dim luminance = 0.2987 * r + 0.587 * g + 0.114 * b
    Dim pixel_bw = CInt(Fix(luminance * 13.0))
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
    Public r As Double    ' a fraction between 0 and 1
    Public g As Double    ' a fraction between 0 and 1
    Public b As Double    ' a fraction between 0 and
    Public Sub New()
    End Sub
    Public Sub New(r As Double, g As Double, b As Double)
      Me.r = r
      Me.g = g
      Me.b = b
    End Sub
  End Class

  ' Define HSV class
  Private Class HSV
    Public h As Double    ' angle in degrees
    Public s As Double    ' a fraction between 0 and 1
    Public v As Double    ' a fraction between 0 and 1
  End Class

  ' Define rgb2hsv function
  Private Shared Function rgb2hsv(inRgb As RGB) As HSV

    Dim outHsv As New HSV()
    Dim minVal As Double, maxVal As Double, delta As Double

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
      outHsv.h = Double.NaN                         ' its now undefined
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

  Private Shared Sub ClassifyPixel_HSL(r As Double, g As Double, b As Double, ByRef sym As Integer, ByRef fg_col As Integer, ByRef bg_col As Integer)

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

    Dim index As Integer = CInt(Fix((col.h / 360.0F) * 24.0F))

    If col.s > 0.2F Then
      sym = hues(index).c
      fg_col = hues(index).fg
      bg_col = hues(index).bg
    Else
      ClassifyPixel_Grey(r, g, b, sym, fg_col, bg_col)
    End If

  End Sub

  Sub ClassifyPixel_OLC(r As Double, g As Double, b As Double, ByRef sym As Integer, ByRef fg_col As Integer, ByRef bg_col As Integer)

    ' Is pixel coloured (i.e. RGB values exhibit significant variance)
    Dim fMean As Double = (r + g + b) / 3.0F
    Dim fRVar As Double = (r - fMean) * (r - fMean)
    Dim fGVar As Double = (g - fMean) * (g - fMean)
    Dim fBVar As Double = (b - fMean) * (b - fMean)
    Dim fVariance As Double = fRVar + fGVar + fBVar

    If fVariance < 0.0001F Then
      ClassifyPixel_Grey(r, g, b, sym, fg_col, bg_col)
    Else
      ' Pixel has colour so get dominant colour
      Dim y As Double = Math.Min(r, g)
      Dim c As Double = Math.Min(g, b)
      Dim m As Double = Math.Min(b, r)

      Dim fMean2 As Double = (y + c + m) / 3.0F
      Dim fYVar As Double = (y - fMean2) * (y - fMean2)
      Dim fCVar As Double = (c - fMean2) * (c - fMean2)
      Dim fMVar As Double = (m - fMean2) * (m - fMean2)

      Dim fMaxPrimaryVar As Double = Math.Max(fRVar, fGVar)
      fMaxPrimaryVar = Math.Max(fMaxPrimaryVar, fBVar)

      Dim fMaxSecondaryVar As Double = Math.Max(fCVar, fYVar)
      fMaxSecondaryVar = Math.Max(fMaxSecondaryVar, fMVar)

      Dim fShading As Double = 0.5F

      If fRVar = fMaxPrimaryVar AndAlso fYVar = fMaxSecondaryVar Then
        compare(fRVar, fYVar, r, y, FG_RED, FG_DARK_RED, BG_YELLOW, BG_DARK_YELLOW, fShading, sym, fg_col, bg_col)
      End If

      If fRVar = fMaxPrimaryVar AndAlso fMVar = fMaxSecondaryVar Then
        compare(fRVar, fMVar, r, m, FG_RED, FG_DARK_RED, BG_MAGENTA, BG_DARK_MAGENTA, fShading, sym, fg_col, bg_col)
      End If

      If fRVar = fMaxPrimaryVar AndAlso fCVar = fMaxSecondaryVar Then
        compare(fRVar, fCVar, r, c, FG_RED, FG_DARK_RED, BG_CYAN, BG_DARK_CYAN, fShading, sym, fg_col, bg_col)
      End If

      If fGVar = fMaxPrimaryVar AndAlso fYVar = fMaxSecondaryVar Then
        compare(fGVar, fYVar, g, y, FG_GREEN, FG_DARK_GREEN, BG_YELLOW, BG_DARK_YELLOW, fShading, sym, fg_col, bg_col)
      End If

      If fGVar = fMaxPrimaryVar AndAlso fCVar = fMaxSecondaryVar Then
        compare(fGVar, fCVar, g, c, FG_GREEN, FG_DARK_GREEN, BG_CYAN, BG_DARK_CYAN, fShading, sym, fg_col, bg_col)
      End If

      If fGVar = fMaxPrimaryVar AndAlso fMVar = fMaxSecondaryVar Then
        compare(fGVar, fMVar, g, m, FG_GREEN, FG_DARK_GREEN, BG_MAGENTA, BG_DARK_MAGENTA, fShading, sym, fg_col, bg_col)
      End If

      If fBVar = fMaxPrimaryVar AndAlso fMVar = fMaxSecondaryVar Then
        compare(fBVar, fMVar, b, m, FG_BLUE, FG_DARK_BLUE, BG_MAGENTA, BG_DARK_MAGENTA, fShading, sym, fg_col, bg_col)
      End If

      If fBVar = fMaxPrimaryVar AndAlso fCVar = fMaxSecondaryVar Then
        compare(fBVar, fCVar, b, c, FG_BLUE, FG_DARK_BLUE, BG_CYAN, BG_DARK_CYAN, fShading, sym, fg_col, bg_col)
      End If

      If fBVar = fMaxPrimaryVar AndAlso fYVar = fMaxSecondaryVar Then
        compare(fBVar, fYVar, b, y, FG_BLUE, FG_DARK_BLUE, BG_YELLOW, BG_DARK_YELLOW, fShading, sym, fg_col, bg_col)
      End If

    End If

  End Sub

  Private Sub Compare(fV1 As Double, fV2 As Double, fC1 As Double, fC2 As Double, FG_LIGHT As Integer, FG_DARK As Integer, BG_LIGHT As Integer, BG_DARK As Integer,
                         fshading As Double, sym As Integer, fg_col As Integer, bg_col As Integer)
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

  Private Shared Function MatToBitmap(mat As Mat) As Bitmap
    Using ms = mat.ToMemoryStream()
      If OperatingSystem.IsWindows Then
        Return CType(Image.FromStream(ms), Bitmap)
      Else
        Return Nothing
      End If
    End Using
  End Function

End Class
