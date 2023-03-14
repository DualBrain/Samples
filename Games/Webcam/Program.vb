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
    game.ConstructConsole(160, 80, 8, 8)
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

          ' Draw Pixel

          If g > 0.5 Then
            Draw(x, y, PIXEL_SOLID, FG_WHITE)
          Else
            Draw(x, y, PIXEL_SOLID, FG_BLACK)
          End If
        Next
      Next
    End If

    Return True

  End Function

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
