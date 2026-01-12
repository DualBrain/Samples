Imports FFMediaToolkit
Imports FFMediaToolkit.Decoding

Module Program
  Sub Main() 'args As String())

    ' Files that works...
    ' ffmpeg-n6.0-32-gd4a7a6e7fa-win64-lgpl-shared-6.0.zip
    ' from
    ' https://github.com/BtbN/FFmpeg-Builds/releases/tag/autobuild-2023-07-31-12-50
    FFmpegLoader.FFmpegPath = "C:\ffmpeg6"

    'Dim rootPath = "\\athena\movies_d"
    Dim rootPath = "\\athena\tv_f"

    Dim folders = IO.Directory.GetDirectories(rootPath)

    Dim bitrate = 5000

    For Each folder In folders
      For Each subfolder In IO.Directory.GetDirectories(folder)
        'For Each subsubfolder In IO.Directory.GetDirectories(subfolder)
        For Each filespec In IO.Directory.GetFiles(subfolder)
          Dim ext = IO.Path.GetExtension(filespec)
          If String.Compare(ext, ".mp4", True) = 0 Then
            Dim file = MediaFile.Open(filespec)
            'Dim frame5s = file.Video.GetFrame(TimeSpan.FromSeconds(5))
            'Console.WriteLine($"Bitrate: {file.Info.Bitrate / 1000.0} kb/s")
            Dim info = file.Video.Info
            'Console.WriteLine($"Duration: {info.Duration}")
            'Console.WriteLine($"Frames count: {If(info.NumberOfFrames?.ToString, "N/A")}")
            Dim frameRateInfo = If(info.IsVariableFrameRate, "average", "constant")
            'Console.WriteLine($"Frame rate: {info.AvgFrameRate} fps ({frameRateInfo})")
            'Console.WriteLine($"Frame size: {info.FrameSize}")
            'Console.WriteLine($"Pixel format: {info.PixelFormat}")
            'Console.WriteLine($"Codec: {info.CodecName}")
            'Console.WriteLine($"Is interlaced: {info.IsInterlaced}")
            Dim title = IO.Path.GetFileNameWithoutExtension(filespec)
            'If info.FrameSize.Height > 480 Then
            '  Console.WriteLine($"{title},{info.Duration},{file.Info.Bitrate / 1000.0} kb/s,{info.FrameSize}")
            'End If
            If file.Info.Bitrate / 1000 > bitrate Then
              Console.WriteLine($"{title},{info.Duration},{file.Info.Bitrate / 1000.0} kb/s,{info.FrameSize}")
            End If
          ElseIf String.Compare(ext, ".mkv", True) = 0 Then
            Dim file = MediaFile.Open(filespec)
            Dim info = file.Video.Info
            Dim frameRateInfo = If(info.IsVariableFrameRate, "average", "constant")
            Dim title = IO.Path.GetFileNameWithoutExtension(filespec)
            If file.Info.Bitrate / 1000 > bitrate Then
              Console.WriteLine($"{title},{info.Duration},{file.Info.Bitrate / 1000.0} kb/s,{info.FrameSize}")
            End If
          ElseIf String.Compare(ext, ".avi", True) = 0 Then
            Dim file = MediaFile.Open(filespec)
            Dim info = file.Video.Info
            Dim frameRateInfo = If(info.IsVariableFrameRate, "average", "constant")
            Dim title = IO.Path.GetFileNameWithoutExtension(filespec)
            If file.Info.Bitrate / 1000 > bitrate Then
              Console.WriteLine($"{title},{info.Duration},{file.Info.Bitrate / 1000.0} kb/s,{info.FrameSize}")
            End If
          ElseIf String.Compare(ext, ".txt", True) = 0 Then
          Else
            Console.WriteLine(filespec)
          End If
        Next
        'Next
      Next
    Next

    '' Opens a multimedia file.
    '' You can use the MediaOptions properties to set decoder options.
    'Dim file = MediaFile.Open("\\athena\movies_d\10 Minutes Gone (2019)\10.minutes.gone.2019.dvd.480p.mp4")

    '' Gets the frame at 5th second of the video.
    'Dim frame5s = file.Video.GetFrame(TimeSpan.FromSeconds(5))

    '' Print informations about the video stream.
    'Console.WriteLine($"Bitrate: {file.Info.Bitrate / 1000.0} kb/s")
    'Dim info = file.Video.Info
    'Console.WriteLine($"Duration: {info.Duration}")
    'Console.WriteLine($"Frames count: {If(info.NumberOfFrames?.ToString, "N/A")}")
    'Dim frameRateInfo = If(info.IsVariableFrameRate, "average", "constant")
    'Console.WriteLine($"Frame rate: {info.AvgFrameRate} fps ({frameRateInfo})")
    'Console.WriteLine($"Frame size: {info.FrameSize}")
    'Console.WriteLine($"Pixel format: {info.PixelFormat}")
    'Console.WriteLine($"Codec: {info.CodecName}")
    'Console.WriteLine($"Is interlaced: {info.IsInterlaced}")

  End Sub
End Module
