' Inspired by "8-Bits Of Image Processing You Should Know!" -- @javidx9
' https://youtu.be/mRM5Js3VLCk

Option Explicit On
Option Strict On
Option Infer On

Imports Olc
Imports OpenCvSharp

Module Program

  Sub Main()
    Dim demo As New ImageProcessing
    If demo.Construct(670, 460, 2, 2) Then
      demo.Start()
    End If
  End Sub

End Module

Friend Class ImageProcessing
  Inherits PixelGameEngine

  Private Const FRAME_WIDTH As Integer = 320
  Private Const FRAME_HEIGHT As Integer = 240

  Private m_capture As VideoCapture
  Private m_frame As Mat

  Public Enum Algorithm
    Threshold
    Motion
    LowPass
    Convolution
    Sobel
    Morpho
    Median
    Adaptive
  End Enum

  Public Enum MorphOp
    Dilation
    Erosion
    Edge
  End Enum

  Private ReadOnly m_input, m_output, m_prevInput, m_activity, m_threshold As New Frame

  ' Algorithm Currently Running
  Private m_algo As Algorithm = Algorithm.Threshold
  Private m_morph As MorphOp = MorphOp.Dilation
  Private m_morphCount As Integer = 1

  Private m_thresholdValue As Single = 0.5F
  Private m_lowPassRC As Single = 0.1F
  Private m_adaptiveBias As Single = 1.1F

  Private ReadOnly m_kernelBlur As New List(Of Single) From {0.0F, 0.125F, 0.0F,
                                                             0.125F, 0.5F, 0.125F,
                                                             0.0F, 0.125F, 0.0F}

  Private ReadOnly m_kernelSharpen As New List(Of Single) From {0.0F, -1.0F, 0.0F,
                                                               -1.0F, 5.0F, -1.0F,
                                                                0.0F, -1.0F, 0.0F}

  Private ReadOnly kernelSobelV As New List(Of Single) From {-1.0F, 0.0F, +1.0F,
                                                             -2.0F, 0.0F, +2.0F,
                                                             -1.0F, 0.0F, +1.0F}

  Private ReadOnly kernelSobelH As New List(Of Single) From {-1.0F, -2.0F, -1.0F,
                                                              0.0F, 0.0F, 0.0F,
                                                             +1.0F, +2.0F, +1.0F}

  Private m_convoKernel As List(Of Single) = m_kernelBlur

  Sub New()
    AppName = "ImageProcessing"
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
    m_capture.Set(VideoCaptureProperties.FrameWidth, FRAME_WIDTH) 'ScreenWidth)
    m_capture.Set(VideoCaptureProperties.FrameHeight, FRAME_HEIGHT) 'ScreenHeight)
    'm_capture.Set(VideoCaptureProperties.Zoom, 0)

    ' It turns out that the following is redundant...
    'Console.Title = "Opening VideoCapture"
    'm_capture?.Open(0) ', VideoCaptureAPIs.msmf)

    'Console.Title = "Startup completed..."
    Return True

  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    If Not m_capture?.IsOpened() Then Return False

    ' CAPTURING WEBCAM IMAGE
    m_capture.Read(m_frame)
    'Cv2.Resize(m_frame, m_frame, New Size(sw, sh))
    'Cv2.Flip(m_frame, m_frame, FlipMode.Y)
    Dim indexer = m_frame.GetGenericIndexer(Of Vec3b)

    'prev_input = input
    m_prevInput.CopyFrom(m_input)

    ' CONVERT TO FRAME (black and white)
    Dim fw = m_frame.Width : Dim fh = m_frame.Height
    For y = 0 To fh - 1
      For x = 0 To fw - 1
        Dim id = y * fw + x
        Dim color = indexer(y, x) : Dim r = color.Item2 : Dim g = color.Item1 : Dim b = color.Item0 : Dim rgb = New Pixel(r, g, b)
        m_input.Pixels(y * FRAME_WIDTH + x) = CSng(rgb.G) / 255.0F
      Next
    Next

    If GetKey(Key.K1).Released Then m_algo = Algorithm.Threshold
    If GetKey(Key.K2).Released Then m_algo = Algorithm.Motion
    If GetKey(Key.K3).Released Then m_algo = Algorithm.LowPass
    If GetKey(Key.K4).Released Then m_algo = Algorithm.Convolution
    If GetKey(Key.K5).Released Then m_algo = Algorithm.Sobel
    If GetKey(Key.K6).Released Then m_algo = Algorithm.Morpho
    If GetKey(Key.K7).Released Then m_algo = Algorithm.Median
    If GetKey(Key.K8).Released Then m_algo = Algorithm.Adaptive

    Select Case m_algo

      Case Algorithm.Threshold

        ' Respond to user input
        If GetKey(Key.Z).Held Then m_thresholdValue -= 0.1F * elapsedTime
        If GetKey(Key.X).Held Then m_thresholdValue += 0.1F * elapsedTime
        If m_thresholdValue > 1.0F Then m_thresholdValue = 1.0F
        If m_thresholdValue < 0.0F Then m_thresholdValue = 0.0F
        ' Perform threshold per pixel
        For i = 0 To FRAME_WIDTH - 1
          For j = 0 To FRAME_HEIGHT - 1
            m_output.SetPixel(i, j, If(m_input.GetPixel(i, j) >= m_thresholdValue, 1.0F, 0.0F))
          Next
        Next

      Case Algorithm.Motion

        ' Returns the absolute difference between successive frames per pixel
        For i = 0 To FRAME_WIDTH - 1
          For j = 0 To FRAME_HEIGHT - 1
            m_output.SetPixel(i, j, MathF.Abs(m_input.GetPixel(i, j) - m_prevInput.GetPixel(i, j)))
          Next
        Next

      Case Algorithm.LowPass

        ' Respond to user input
        If GetKey(Key.Z).Held Then m_lowPassRC -= 0.1F * elapsedTime
        If GetKey(Key.X).Held Then m_lowPassRC += 0.1F * elapsedTime
        If m_lowPassRC > 1.0F Then m_lowPassRC = 1.0F
        If m_lowPassRC < 0.0F Then m_lowPassRC = 0.0F

        ' Pass each pixel through a temporal RC filter
        For i = 0 To FRAME_WIDTH - 1
          For j = 0 To FRAME_HEIGHT - 1
            Dim pixel = m_input.GetPixel(i, j) - m_output.GetPixel(i, j)
            pixel *= m_lowPassRC
            m_output.SetPixel(i, j, pixel + m_output.GetPixel(i, j))
          Next
        Next

      Case Algorithm.Convolution

        ' Respond to user input
        If GetKey(Key.Z).Held Then m_convoKernel = m_kernelBlur
        If GetKey(Key.X).Held Then m_convoKernel = m_kernelSharpen

        For i = 0 To FRAME_WIDTH - 1
          For j = 0 To FRAME_HEIGHT - 1
            Dim sum = 0.0F
            For n = -1 To +1
              For m = -1 To +1
                sum += m_input.GetPixel(i + n, j + m) * m_convoKernel((m + 1) * 3 + (n + 1))
              Next
            Next
            m_output.SetPixel(i, j, sum)
          Next
        Next

      Case Algorithm.Sobel

        For i = 0 To FRAME_WIDTH - 1
          For j = 0 To FRAME_HEIGHT - 1
            Dim kernelSumH = 0.0F
            Dim kernelSumV = 0.0F
            For n = -1 To 1
              For m = -1 To 1
                kernelSumH += m_input.GetPixel(i + n, j + m) * kernelSobelH((m + 1) * 3 + (n + 1))
                kernelSumV += m_input.GetPixel(i + n, j + m) * kernelSobelV((m + 1) * 3 + (n + 1))
              Next
            Next
            m_output.SetPixel(i, j, MathF.Abs((kernelSumH + kernelSumV) / 2.0F))
          Next
        Next

      Case Algorithm.Morpho

        ' Respond to user input
        If GetKey(Key.Z).Held Then m_morph = MorphOp.Dilation
        If GetKey(Key.X).Held Then m_morph = MorphOp.Erosion
        If GetKey(Key.C).Held Then m_morph = MorphOp.Edge

        If GetKey(Key.A).Released Then m_morphCount -= 1
        If GetKey(Key.S).Released Then m_morphCount += 1
        If m_morphCount > 10.0F Then m_morphCount = 10
        If m_morphCount < 1.0F Then m_morphCount = 1

        ' Threshold First to binarise image
        For i = 0 To FRAME_WIDTH - 1
          For j = 0 To FRAME_HEIGHT - 1
            m_activity.SetPixel(i, j, If(m_input.GetPixel(i, j) > m_thresholdValue, 1.0F, 0.0F))
          Next
        Next

        m_threshold.CopyFrom(m_activity)

        Select Case m_morph

          Case MorphOp.Dilation

            For n = 0 To m_morphCount - 1
              m_output.CopyFrom(m_activity)
              For i = 0 To FRAME_WIDTH - 1
                For j = 0 To FRAME_HEIGHT - 1
                  If m_activity.GetPixel(i, j) = 1.0F Then
                    m_output.SetPixel(i, j, 1.0F)
                    m_output.SetPixel(i - 1, j, 1.0F)
                    m_output.SetPixel(i + 1, j, 1.0F)
                    m_output.SetPixel(i, j - 1, 1.0F)
                    m_output.SetPixel(i, j + 1, 1.0F)
                    m_output.SetPixel(i - 1, j - 1, 1.0F)
                    m_output.SetPixel(i + 1, j + 1, 1.0F)
                    m_output.SetPixel(i + 1, j - 1, 1.0F)
                    m_output.SetPixel(i - 1, j + 1, 1.0F)
                  End If
                Next
              Next
              m_activity.CopyFrom(m_output)
            Next

          Case MorphOp.Erosion

            For n = 0 To m_morphCount - 1
              m_output.CopyFrom(m_activity)
              For i = 0 To FRAME_WIDTH - 1
                For j = 0 To FRAME_HEIGHT - 1
                  Dim sum = m_activity.GetPixel(i - 1, j) + m_activity.GetPixel(i + 1, j) + m_activity.GetPixel(i, j - 1) + m_activity.GetPixel(i, j + 1) + m_activity.GetPixel(i - 1, j - 1) + m_activity.GetPixel(i + 1, j + 1) + m_activity.GetPixel(i + 1, j - 1) + m_activity.GetPixel(i - 1, j + 1)
                  If m_activity.GetPixel(i, j) = 1.0F AndAlso sum < 8.0F Then
                    m_output.SetPixel(i, j, 0.0F)
                  End If
                Next
              Next
              m_activity.CopyFrom(m_output)
            Next

          Case MorphOp.Edge

            m_output.CopyFrom(m_activity)
            For i = 0 To FRAME_WIDTH - 1
              For j = 0 To FRAME_HEIGHT - 1
                Dim sum = m_activity.GetPixel(i - 1, j) + m_activity.GetPixel(i + 1, j) + m_activity.GetPixel(i, j - 1) + m_activity.GetPixel(i, j + 1) + m_activity.GetPixel(i - 1, j - 1) + m_activity.GetPixel(i + 1, j + 1) + m_activity.GetPixel(i + 1, j - 1) + m_activity.GetPixel(i - 1, j + 1)
                If m_activity.GetPixel(i, j) = 1.0F AndAlso sum = 8.0F Then
                  m_output.SetPixel(i, j, 0.0F)
                End If
              Next
            Next

        End Select

      Case Algorithm.Median

        For i = 0 To FRAME_WIDTH - 1
          For j = 0 To FRAME_HEIGHT - 1
            Dim v As New List(Of Single)
            For n = -2 To 2
              For m = -2 To 2
                v.Add(m_input.GetPixel(i + n, j + m))
              Next
            Next
            v.Sort()
            m_output.SetPixel(i, j, v(12))
          Next
        Next

      Case Algorithm.Adaptive

        ' Respond to user input
        If GetKey(Key.Z).Pressed Then m_adaptiveBias -= 0.1F * elapsedTime
        If GetKey(Key.X).Pressed Then m_adaptiveBias += 0.1F * elapsedTime
        If m_adaptiveBias > 1.5F Then m_adaptiveBias = 1.5F
        If m_adaptiveBias < 0.5F Then m_adaptiveBias = 0.5F

        For i = 0 To FRAME_WIDTH - 1
          For j = 0 To FRAME_HEIGHT - 1
            Dim regionSum = 0.0F
            For n = -2 To 2
              For m = -2 To 2
                regionSum += m_input.GetPixel(i + n, j + m)
              Next
            Next
            regionSum /= 25.0F
            m_output.SetPixel(i, j, If(m_input.GetPixel(i, j) > (regionSum * m_adaptiveBias), 1.0F, 0.0F))
          Next
        Next

    End Select

    ' DRAW STUFF ONLY HERE

    Clear(Presets.DarkBlue)

    DrawFrame(If(m_algo = Algorithm.Morpho, m_threshold, m_input), 10, 10)
    DrawFrame(m_output, 340, 10)
    DrawString(150, 255, "INPUT")
    DrawString(480, 255, "OUTPUT")
    DrawString(10, 275, "1) Threshold", If(m_algo = Algorithm.Threshold, Presets.Yellow, Presets.White))
    DrawString(10, 285, "2) Absolute Motion", If(m_algo = Algorithm.Motion, Presets.Yellow, Presets.White))
    DrawString(10, 295, "3) Low-Pass Temporal Filtering", If(m_algo = Algorithm.LowPass, Presets.Yellow, Presets.White))
    DrawString(10, 305, "4) Convolution (Blurring/Sharpening)", If(m_algo = Algorithm.Convolution, Presets.Yellow, Presets.White))
    DrawString(10, 315, "5) Sobel Edge Detection", If(m_algo = Algorithm.Sobel, Presets.Yellow, Presets.White))
    DrawString(10, 325, "6) Binary Morphological Operations (Erosion/Dilation)", If(m_algo = Algorithm.Morpho, Presets.Yellow, Presets.White))
    DrawString(10, 335, "7) Median Filter", If(m_algo = Algorithm.Median, Presets.Yellow, Presets.White))
    DrawString(10, 345, "8) Adaptive Threshold", If(m_algo = Algorithm.Adaptive, Presets.Yellow, Presets.White))

    Select Case m_algo
      Case Algorithm.Threshold
        DrawString(10, 375, "Change threshold value with Z and X keys")
        DrawString(10, 385, $"Current value = {m_thresholdValue}")
      Case Algorithm.LowPass
        DrawString(10, 375, "Change RC constant value with Z and X keys")
        DrawString(10, 385, $"Current value = {m_lowPassRC}")
      Case Algorithm.Convolution
        DrawString(10, 375, "Change convolution kernel with Z and X keys")
        DrawString(10, 385, $"Current kernel = {If(m_convoKernel Is m_kernelBlur, "Blur", "Sharpen")}")
      Case Algorithm.Morpho
        DrawString(10, 375, "Change operation with Z and X and C keys")
        Select Case m_morph
          Case MorphOp.Dilation
            DrawString(10, 385, "Current operation = DILATION")
          Case MorphOp.Erosion
            DrawString(10, 385, "Current operation = EROSION")
          Case MorphOp.Edge
            DrawString(10, 385, "Current operation = EDGE")
        End Select
        DrawString(10, 395, "Change Iterations with A and S keys")
        DrawString(10, 405, $"Current iteration count = {m_morphCount}")
      Case Algorithm.Adaptive
        DrawString(10, 375, "Change adaptive threshold bias with Z and X keys")
        DrawString(10, 385, $"Current value = {m_adaptiveBias}")
      Case Else
    End Select

    If GetKey(Key.ESCAPE).Pressed Then Return False

    Return True

  End Function

  Private Sub DrawFrame(f As Frame, x As Integer, y As Integer)
    For i = 0 To FRAME_WIDTH - 1
      For j = 0 To FRAME_HEIGHT - 1
        Dim c = CInt(Fix(Math.Min(Math.Max(0.0F, f.GetPixel(i, j) * 255.0F), 255.0F)))
        Draw(x + i, y + j, New Pixel(c, c, c))
      Next
    Next
  End Sub

  Private Class Frame

    Public Property Pixels As Single() = Nothing

    Public Sub New()
      Pixels = New Single(FRAME_WIDTH * FRAME_HEIGHT - 1) {}
    End Sub

    Public Function GetPixel(x As Integer, y As Integer) As Single
      If x >= 0 AndAlso x < FRAME_WIDTH AndAlso y >= 0 AndAlso y < FRAME_HEIGHT Then
        Return Pixels(y * FRAME_WIDTH + x)
      Else
        Return 0.0F
      End If
    End Function

    Public Sub SetPixel(x As Integer, y As Integer, p As Single)
      If x >= 0 AndAlso x < FRAME_WIDTH AndAlso y >= 0 AndAlso y < FRAME_HEIGHT Then
        Pixels(y * FRAME_WIDTH + x) = p
      End If
    End Sub

    Public Sub CopyFrom(source As Frame)
      Array.Copy(source.Pixels, Pixels, FRAME_WIDTH * FRAME_HEIGHT)
    End Sub

  End Class

End Class