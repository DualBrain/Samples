' Inspired by: "Augmenting Reality #1 - Optical Flow (C++)" -- @javidx9
' https://youtu.be/aNtzgoEGC1Y

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports OpenCvSharp

Module Program

  Sub Main() 'args As String())
    Dim game As New AR_OpticFlow
    game.ConstructConsole(80, 60, 16, 16)
    game.Start()
  End Sub

End Module

Class AR_OpticFlow
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_capture As VideoCapture
  Private m_frame As Mat

  ' 2D Maps for image processing
  Private fOldCamera As Single() = Nothing ' Previous raw frame from camera
  Private fNewCamera As Single() = Nothing ' Recent raw frame from camera
  Private fFilteredCamera As Single() = Nothing ' low-pass filtered image
  Private fOldFilteredCamera As Single() = Nothing ' low-pass filtered image
  Private fOldMotionImage As Single() = Nothing ' previous motion image
  Private fMotionImage As Single() = Nothing ' recent motion image
  Private fFlowX As Single() = Nothing ' x-component of flow field vector
  Private fFlowY As Single() = Nothing ' y-component of flow field vector

  ' Object Physics Variables
  Private fBallX As Single = 0.0F ' Ball position 2D
  Private fBallY As Single = 0.0F
  Private fBallVX As Single = 0.0F ' Ball Velocity 2D
  Private fBallVY As Single = 0.0F

  Public Overrides Function OnUserCreate() As Boolean

    Dim sw = ScreenWidth()
    Dim sh = ScreenHeight()

    ' Initialise webcam to capture dimensions
    Console.Title = "Initializing MAT"
    m_frame = New Mat()
    Console.Title = "Initializing VideoCapture"
    m_capture = New VideoCapture(0) 'With {.FrameHeight = sh, .FrameWidth = sw}
    Console.Title = "Configuring VideoCapture"
    m_capture.Set(VideoCaptureProperties.FrameWidth, 640) 'sw)
    m_capture.Set(VideoCaptureProperties.FrameHeight, 480) 'sh)
    m_capture.Set(VideoCaptureProperties.Zoom, 0)
    Console.Title = "Opening VideoCapture"
    m_capture?.Open(0)
    Console.Title = "Startup completed..."

    ' Allocate memory for images
    fOldCamera = New Single(sw * sh - 1) {}
    fNewCamera = New Single(sw * sh - 1) {}
    fFilteredCamera = New Single(sw * sh - 1) {}
    fOldFilteredCamera = New Single(sw * sh - 1) {}
    fFlowX = New Single(sw * sh - 1) {}
    fFlowY = New Single(sw * sh - 1) {}
    fOldMotionImage = New Single(sw * sh - 1) {}
    fMotionImage = New Single(sw * sh - 1) {}

    ' Initialise images to 0
    Array.Clear(fOldCamera, 0, fOldCamera.Length)
    Array.Clear(fNewCamera, 0, fNewCamera.Length)
    Array.Clear(fFilteredCamera, 0, fFilteredCamera.Length)
    Array.Clear(fOldFilteredCamera, 0, fOldFilteredCamera.Length)
    Array.Clear(fFlowX, 0, fFlowX.Length)
    Array.Clear(fFlowY, 0, fFlowY.Length)
    Array.Clear(fOldMotionImage, 0, fOldMotionImage.Length)
    Array.Clear(fMotionImage, 0, fMotionImage.Length)

    ' Set ball position to middle of frame
    fBallX = sw / 2.0F
    fBallY = sh / 2.0F

    Return True

  End Function

  Private Sub DrawImage(arry As Single())

    Dim sw = ScreenWidth()
    Dim sh = ScreenHeight()

    For x = 0 To sw - 1
      For y = 0 To sh - 1

        Dim pixel_bw = CInt(Fix(arry(y * sw + x) * 13.0F))

        Dim sym = 32 'AscW(" "c)
        Dim bg_col = BG_BLACK
        Dim fg_col = FG_BLACK
        Select Case pixel_bw
          Case 0 : sym = PIXEL_SOLID
          Case 1 : fg_col = FG_DARK_GREY : sym = PIXEL_QUARTER
          Case 2 : fg_col = FG_DARK_GREY : sym = PIXEL_HALF
          Case 3 : fg_col = FG_DARK_GREY : sym = PIXEL_THREEQUARTERS
          Case 4 : fg_col = FG_DARK_GREY : sym = PIXEL_SOLID
          Case 5 : bg_col = BG_DARK_GREY : fg_col = FG_GREY : sym = PIXEL_QUARTER
          Case 6 : bg_col = BG_DARK_GREY : fg_col = FG_GREY : sym = PIXEL_HALF
          Case 7 : bg_col = BG_DARK_GREY : fg_col = FG_GREY : sym = PIXEL_THREEQUARTERS
          Case 8 : bg_col = BG_DARK_GREY : fg_col = FG_GREY : sym = PIXEL_SOLID
          Case 9 : bg_col = BG_GREY : fg_col = FG_WHITE : sym = PIXEL_QUARTER
          Case 10 : bg_col = BG_GREY : fg_col = FG_WHITE : sym = PIXEL_HALF
          Case 11 : bg_col = BG_GREY : fg_col = FG_WHITE : sym = PIXEL_THREEQUARTERS
          Case 12 : bg_col = BG_GREY : fg_col = FG_WHITE : sym = PIXEL_SOLID
          Case Else
        End Select

        Draw(x, y, sym, bg_col Or fg_col)

      Next
    Next

  End Sub

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Dim sw = ScreenWidth()
    Dim sh = ScreenHeight()

    ' Lambda function to read from a 2D array without error
    Dim get_pixel As Func(Of Single(), Integer, Integer, Single) = Function(img As Single(), x As Integer, y As Integer)
                                                                     If x >= 0 AndAlso x < sw AndAlso y >= 0 AndAlso y < sh Then
                                                                       Return img(y * sw + x)
                                                                     Else
                                                                       Return 0.0F
                                                                     End If
                                                                   End Function

    ' === Capture & Filter New Input Image ================================================

    ' Get Image from webcam
    If m_capture?.IsOpened() Then
      m_capture.Read(m_frame)
      Cv2.Resize(m_frame, m_frame, New Size(sw, sh))
      Cv2.Flip(m_frame, m_frame, FlipMode.Y)
    End If

    Dim indexer = m_frame.GetGenericIndexer(Of Vec3b)

    ' Do Temporal Filtering per pixel
    For y = 0 To sh - 1
      For x = 0 To sw - 1

        Dim color = indexer(y, x)
        Dim r = color.Item2
        Dim g = color.Item1
        Dim b = color.Item0

        Dim fR = r / 255.0F
        Dim fG = g / 255.0F
        Dim fB = b / 255.0F

        Dim i = y * sw + x

        ' Store previous camera frame for temporal processing
        fOldCamera(i) = fNewCamera(i)

        ' Store previous filtered camera frame for temporal processing
        fOldFilteredCamera(i) = fFilteredCamera(i)

        ' Store previous motion only frame
        fOldMotionImage(i) = fMotionImage(i)

        ' Calculate luminance (greyscale equivalent) of pixel
        Dim fLuminance = 0.2987F * fR + 0.587F * fG + 0.114F * fB
        fNewCamera(i) = fLuminance

        ' Low-Pass filter camera image, to remove pixel jitter
        fFilteredCamera(i) += (fNewCamera(i) - fFilteredCamera(i)) * 0.8F

        ' Create motion image as difference between two successive camera frames
        Dim fDiff = Math.Abs(get_pixel(fFilteredCamera, x, y) - get_pixel(fOldFilteredCamera, x, y))

        ' Threshold motion image to remove filter out camera noise
        fMotionImage(i) = If(fDiff >= 0.05F, fDiff, 0.0F)

      Next
    Next

    ' === Calculate Optic Flow Vector Map ==========================================

    ' Brute Force Local Spatial Pattern Matching
    Dim nPatchSize = 9
    Dim nSearchSize = 7

    For x = 0 To sw - 1
      For y = 0 To sh - 1

        Dim i = y * sw + x

        ' Initialise search variables
        Dim fPatchDifferenceMax = Single.PositiveInfinity
        'Dim fPatchDifferenceX As Single '= 0.0F
        'Dim fPatchDifferenceY As Single '= 0.0F
        fFlowX(i) = 0.0F
        fFlowY(i) = 0.0F

        ' Search over a given rectangular area for a "patch" of old image
        ' that "resembles" a patch of the new image.
        For sx = 0 To nSearchSize - 1
          For sy = 0 To nSearchSize - 1

            ' Search vector is centre of patch test
            Dim nSearchVectorX = x + (sx - nSearchSize \ 2)
            Dim nSearchVectorY = y + (sy - nSearchSize \ 2)

            Dim fAccumulatedDifference = 0.0F

            ' For each pixel in search patch, accumulate difference with base patch
            For px = 0 To nPatchSize - 1
              For py = 0 To nPatchSize - 1

                ' Work out search patch offset indices
                Dim nPatchPixelX = nSearchVectorX + (px - nPatchSize \ 2)
                Dim nPatchPixelY = nSearchVectorY + (py - nPatchSize \ 2)

                ' Work out base patch indices
                Dim nBasePixelX = x + (px - nPatchSize \ 2)
                Dim nBasePixelY = y + (py - nPatchSize \ 2)

                ' Get adjacent values for each patch
                Dim fPatchPixel = get_pixel(fNewCamera, nPatchPixelX, nPatchPixelY)
                Dim fBasePixel = get_pixel(fOldCamera, nBasePixelX, nBasePixelY)

                ' Accumulate difference
                fAccumulatedDifference += Math.Abs(fPatchPixel - fBasePixel)

              Next
            Next

            ' Record the vector offset for the search patch that is the
            ' least different to the base patch
            If fAccumulatedDifference <= fPatchDifferenceMax Then
              fPatchDifferenceMax = fAccumulatedDifference
              fFlowX(i) = nSearchVectorX - x
              fFlowY(i) = nSearchVectorY - y
            End If

          Next
        Next

      Next
    Next

    ' Modulate Optic Flow Vector Map with motion map, to remove vectors that
    ' errornously indicate large local motion
    For i = 0 To sw * sh - 1
      fFlowX(i) *= If(fMotionImage(i) > 0, 1.0F, 0.0F)
      fFlowY(i) *= If(fMotionImage(i) > 0, 1.0F, 0.0F)
    Next

    ' === Update Ball Physics ========================================================

    ' Ball velocity is updated by optic flow vector field
    fBallVX += 100.0F * fFlowX(CInt(Fix(fBallY)) * sw + CInt(Fix(fBallX))) * elapsedTime
    fBallVY += 100.0F * fFlowY(CInt(Fix(fBallY)) * sw + CInt(Fix(fBallX))) * elapsedTime

    ' Ball position is updated by velocity
    fBallX += 1.0F * fBallVX * elapsedTime
    fBallY += 1.0F * fBallVY * elapsedTime

    ' Add "drag" effect to ball velocity
    fBallVX *= 0.85F
    fBallVY *= 0.85F

    ' Wrap ball around screen
    If fBallX >= sw Then fBallX -= sw
    If fBallY >= sh Then fBallY -= sh
    If fBallX < 0 Then fBallX += sw
    If fBallY < 0 Then fBallY += sh

    ' === Update Screen =================================================================

    ' Draw Camera Image
    DrawImage(fMotionImage)
    'DrawImage(fNewCamera)

    ' Draw "Ball"
    Fill(fBallX - 4, fBallY - 4, fBallX + 4, fBallY + 4, PIXEL_SOLID, FG_RED)

    Return True

  End Function

End Class
