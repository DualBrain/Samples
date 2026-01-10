Imports System.Diagnostics

Module Program

  Sub Main()

    QB.SCREEN(320, 200)
    X11Backend.Init(800, 600)

    Dim running As Boolean = True
    Dim sw As Stopwatch = Stopwatch.StartNew()

    Dim lastTime = sw.ElapsedMilliseconds
    Dim frameCount = 0
    Dim fps = 0

    While running

      X11Backend.PollEvents(running)

       QB.CLS(&H000000)
       QB.COLOR(&H00FF00)
       QB.LINE(0, 0, 319, 199)
       'QB.LINE(0, 199, 319, 0)

       ' Draw white border around the image
       QB.COLOR(&HFFFFFF)
       QB.LINE(0, 0, 319, 0)
       QB.LINE(319, 0, 319, 199)
       QB.LINE(319, 199, 0, 199)
       QB.LINE(0, 199, 0, 0)

       ' Example: draw a pixel if any key pressed
       If X11Backend.INKEY() <> "" Then
         'QB.PSET(160, 100)
         QB.LINE(0, 199, 319, 0)
       End If

      ' ---- FPS calculation ----
      frameCount += 1
      Dim nowMs = sw.ElapsedMilliseconds
      If nowMs - lastTime >= 1000 Then
          fps = frameCount
          frameCount = 0
          lastTime = nowMs
      End If

       ' Draw FPS in top-left corner
       BitmapFont.DrawNumber(fps, 10, 10, &HFFFFFF, 2)

      X11Backend.Present()

      'Threading.Thread.Sleep(10)

    End While

    X11Backend.Shutdown()

  End Sub

End Module
