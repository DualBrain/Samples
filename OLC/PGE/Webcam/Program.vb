Option Explicit On
Option Strict On
Option Infer On

Imports Olc
Imports OpenCvSharp

Module Program

  Sub Main()
    Dim demo As New Webcam
    If demo.Construct(640, 480, 2, 2) Then
      demo.Start()
    End If
  End Sub

End Module

Class Webcam
  Inherits PixelGameEngine

  Private m_capture As VideoCapture
  Private m_frame As Mat

  Sub New()
    AppName = "Webcam"
  End Sub

  Protected Overrides Function OnUserCreate() As Boolean

    'Console.Title = "Initializing MAT"
    m_frame = New Mat()

    'Console.Title = "Initializing VideoCapture"
    'm_capture = New VideoCapture(0, VideoCaptureAPIs.ANY) ' Looks like this one will end up picking MSMF, but suffers that same 
    m_capture = New VideoCapture(0, VideoCaptureAPIs.DSHOW) ' Starts up immediately, but framerate is horrible (between 10-15 fps)
    'm_capture = New VideoCapture(0, VideoCaptureAPIs.MSMF) ' Takes like 30+ seconds to get started, but perf is almost 3x when (finally) initialized

    ' The following, if desired, needs to be called before the Read method.
    'Console.Title = "Configuring VideoCapture"
    'm_capture.Set(VideoCaptureProperties.FrameWidth, 640) 'ScreenWidth)
    'm_capture.Set(VideoCaptureProperties.FrameHeight, 480) 'ScreenHeight)
    'm_capture.Set(VideoCaptureProperties.Zoom, 0)

    ' It turns out that the following is redundant...
    'Console.Title = "Opening VideoCapture"
    'm_capture?.Open(0) ', VideoCaptureAPIs.msmf)

    'Console.Title = "Startup completed..."
    Return True

  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    If m_capture?.IsOpened() Then

      Dim sw = ScreenWidth()
      Dim sh = ScreenHeight()

      m_capture.Read(m_frame)

      'Cv2.Resize(m_frame, m_frame, New Size(sw, sh))
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
          Dim r = color.Item2
          Dim g = color.Item1
          Dim b = color.Item0

          ' Draw Pixel
          Draw(x, y, New Pixel(r, g, b))

        Next
      Next

    End If

    Return True

  End Function

End Class