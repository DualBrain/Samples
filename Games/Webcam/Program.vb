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
    m_capture.Open(0)

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    If m_capture.IsOpened() Then
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

          Classify_Grey(r, g, b, sym, bg_col, fg_col)

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

  Private Sub Classify_Grey(r As Double, g As Double, b As Double, ByRef ch As Integer, ByRef fg As Integer, ByRef bg As Integer)

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
