' Inspired by: "Programming Perlin-like Noise (C++)" -- @javidx9
' https://youtu.be/6-0UaeJBumA

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour

Module Program

  Sub Main() 'args As String())
    Dim game As New PerlinNoise
    game.ConstructConsole(256, 256, 3, 3)
    game.Start()
  End Sub

End Module

Class PerlinNoise
  Inherits ConsoleGameEngine.ConsoleGameEngine

  ' 2D noise variables
  Private m_outputWidth As Integer = 256
  Private m_utputHeight As Integer = 256
  Private m_noiseSeed2D As Double() = Nothing
  Private m_perlinNoise2D As Double() = Nothing

  ' 1D noise variables
  Private m_noiseSeed1D As Double() = Nothing
  Private m_perlinNoise1D As Double() = Nothing
  Private m_outputSize As Integer = 256

  Private m_octaveCount As Integer = 1
  Private m_scalingBias As Double = 2.0
  Private m_mode As Integer = 1

  Public Overrides Function OnUserCreate() As Boolean

    m_outputWidth = ScreenWidth()
    m_utputHeight = ScreenHeight()

    m_noiseSeed2D = New Double(m_outputWidth * m_utputHeight - 1) {}
    m_perlinNoise2D = New Double(m_outputWidth * m_utputHeight - 1) {}
    For i = 0 To m_outputWidth * m_utputHeight - 1
      m_noiseSeed2D(i) = Rand / RAND_MAX
    Next

    m_outputSize = ScreenWidth()
    m_noiseSeed1D = New Double(m_outputSize - 1) {}
    m_perlinNoise1D = New Double(m_outputSize - 1) {}
    For i = 0 To m_outputSize - 1
      m_noiseSeed1D(i) = Rand / RAND_MAX
    Next

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Cls()

    If m_keys(VK_SPACE).Released Then m_octaveCount += 1
    If m_keys(AscW("1"c)).Released Then m_mode = 1
    If m_keys(AscW("2"c)).Released Then m_mode = 2
    If m_keys(AscW("3"c)).Released Then m_mode = 3
    If m_keys(AscW("Q"c)).Released Then m_scalingBias += 0.2
    If m_keys(AscW("A"c)).Released Then m_scalingBias -= 0.2

    If m_scalingBias < 0.2 Then m_scalingBias = 0.2
    If m_octaveCount = 9 Then m_octaveCount = 1

    If m_mode = 1 Then ' 1D Noise

      If m_keys(AscW("Z"c)).Released Then ' Noise Between 0 and +1
        For i = 0 To m_outputSize - 1
          m_noiseSeed1D(i) = Rand / RAND_MAX
        Next
      End If

      If m_keys(AscW("X"c)).Released Then ' Noise Between -1 and +1
        For i = 0 To m_outputSize - 1
          m_noiseSeed1D(i) = 2.0F * (Rand / RAND_MAX) - 1.0F
        Next
      End If

      PerlinNoise1D(m_outputSize, m_noiseSeed1D, m_octaveCount, m_scalingBias, m_perlinNoise1D)

      For x = 0 To m_outputSize - 1

        Dim y = -CInt(Fix((m_perlinNoise1D(x) * (ScreenHeight() / 2)))) + (ScreenHeight() / 2)

        If y < ScreenHeight() \ 2 Then
          For f = y To CInt(Fix(ScreenHeight() / 2)) - 1
            Draw(x, f, PIXEL_SOLID, FG_GREEN)
          Next
        Else
          For f = ScreenHeight() \ 2 To y
            Draw(x, f, PIXEL_SOLID, FG_RED)
          Next
        End If

      Next

    End If

    If m_mode = 2 Then ' 2D Noise

      If m_keys(AscW("Z"c)).Released Then ' Noise Between 0 and +1
        For i = 0 To m_outputWidth * m_utputHeight - 1
          m_noiseSeed2D(i) = Rand / RAND_MAX
        Next
      End If

      PerlinNoise2D(m_outputWidth, m_utputHeight, m_noiseSeed2D, m_octaveCount, m_scalingBias, m_perlinNoise2D)

      For x = 0 To m_outputWidth - 1
        For y = 0 To m_utputHeight - 1

          Dim bg_col, fg_col As Short
          Dim sym As Integer
          Dim pixel_bw = CInt(Fix(m_perlinNoise2D(y * m_outputWidth + x) * 12.0))

          Select Case pixel_bw
            Case 0 : bg_col = BG_BLACK : fg_col = FG_BLACK : sym = PIXEL_SOLID
            Case 1 : bg_col = BG_BLACK : fg_col = FG_DARK_GREY : sym = PIXEL_QUARTER
            Case 2 : bg_col = BG_BLACK : fg_col = FG_DARK_GREY : sym = PIXEL_HALF
            Case 3 : bg_col = BG_BLACK : fg_col = FG_DARK_GREY : sym = PIXEL_THREEQUARTERS
            Case 4 : bg_col = BG_BLACK : fg_col = FG_DARK_GREY : sym = PIXEL_SOLID
            Case 5 : bg_col = BG_DARK_GREY : fg_col = FG_GREY : sym = PIXEL_QUARTER
            Case 6 : bg_col = BG_DARK_GREY : fg_col = FG_GREY : sym = PIXEL_HALF
            Case 7 : bg_col = BG_DARK_GREY : fg_col = FG_GREY : sym = PIXEL_THREEQUARTERS
            Case 8 : bg_col = BG_DARK_GREY : fg_col = FG_GREY : sym = PIXEL_SOLID
            Case 9 : bg_col = BG_GREY : fg_col = FG_WHITE : sym = PIXEL_QUARTER
            Case 10 : bg_col = BG_GREY : fg_col = FG_WHITE : sym = PIXEL_HALF
            Case 11 : bg_col = BG_GREY : fg_col = FG_WHITE : sym = PIXEL_THREEQUARTERS
            Case 12 : bg_col = BG_GREY : fg_col = FG_WHITE : sym = PIXEL_SOLID
          End Select

          Draw(x, y, sym, fg_col Or bg_col)

        Next
      Next

    End If

    If m_mode = 3 Then ' 2D Noise - colourised

      If m_keys(AscW("Z"c)).Released Then ' Noise Between 0 and +1
        For i = 0 To m_outputWidth * m_utputHeight - 1
          m_noiseSeed2D(i) = Rand / RAND_MAX
        Next
      End If

      PerlinNoise2D(m_outputWidth, m_utputHeight, m_noiseSeed2D, m_octaveCount, m_scalingBias, m_perlinNoise2D)

      For x = 0 To m_outputWidth - 1
        For y = 0 To m_utputHeight - 1

          Dim bg_col, fg_col As Short
          Dim sym As Integer
          Dim pixel_bw = CInt(m_perlinNoise2D(y * m_outputWidth + x) * 16.0)

          Select Case pixel_bw
            Case 0 : bg_col = BG_DARK_BLUE : fg_col = FG_DARK_BLUE : sym = PIXEL_SOLID
            Case 1 : bg_col = BG_DARK_BLUE : fg_col = FG_BLUE : sym = PIXEL_QUARTER
            Case 2 : bg_col = BG_DARK_BLUE : fg_col = FG_BLUE : sym = PIXEL_HALF
            Case 3 : bg_col = BG_DARK_BLUE : fg_col = FG_BLUE : sym = PIXEL_THREEQUARTERS
            Case 4 : bg_col = BG_DARK_BLUE : fg_col = FG_BLUE : sym = PIXEL_SOLID
            Case 5 : bg_col = BG_BLUE : fg_col = FG_GREEN : sym = PIXEL_QUARTER
            Case 6 : bg_col = BG_BLUE : fg_col = FG_GREEN : sym = PIXEL_HALF
            Case 7 : bg_col = BG_BLUE : fg_col = FG_GREEN : sym = PIXEL_THREEQUARTERS
            Case 8 : bg_col = BG_BLUE : fg_col = FG_GREEN : sym = PIXEL_SOLID
            Case 9 : bg_col = BG_GREEN : fg_col = FG_DARK_GREY : sym = PIXEL_QUARTER
            Case 10 : bg_col = BG_GREEN : fg_col = FG_DARK_GREY : sym = PIXEL_HALF
            Case 11 : bg_col = BG_GREEN : fg_col = FG_DARK_GREY : sym = PIXEL_THREEQUARTERS
            Case 12 : bg_col = BG_GREEN : fg_col = FG_DARK_GREY : sym = PIXEL_SOLID
            Case 13 : bg_col = BG_DARK_GREY : fg_col = FG_WHITE : sym = PIXEL_QUARTER
            Case 14 : bg_col = BG_DARK_GREY : fg_col = FG_WHITE : sym = PIXEL_HALF
            Case 15 : bg_col = BG_DARK_GREY : fg_col = FG_WHITE : sym = PIXEL_THREEQUARTERS
            Case 16 : bg_col = BG_DARK_GREY : fg_col = FG_WHITE : sym = PIXEL_SOLID
          End Select

          Draw(x, y, sym, fg_col Or bg_col)

        Next
      Next

    End If

    Return True

  End Function

  Private Shared Sub PerlinNoise1D(count As Integer, seed As Double(), octaves As Integer, bias As Double, output As Double())

    ' Used 1D Perlin Noise

    For x = 0 To count - 1

      Dim noise = 0.0
      Dim scale = 1.0
      Dim scaleAcc = 0.0

      For o = 0 To octaves - 1

        Dim pitch = count >> o
        Dim sample1 = CInt(Fix((x / pitch))) * pitch
        Dim sample2 = (sample1 + pitch) Mod count

        Dim blend = (x - sample1) / pitch
        Dim sample = (1.0 - blend) * seed(sample1) + blend * seed(sample2)
        noise += sample * scale
        scaleAcc += scale
        scale /= bias

      Next

      ' Scale to seed range
      output(x) = noise / scaleAcc

    Next

  End Sub

  Private Shared Sub PerlinNoise2D(width As Integer, height As Integer, seed() As Double, octaves As Integer, bias As Double, output() As Double)

    ' Used 1D Perlin Noise

    For x = 0 To width - 1
      For y = 0 To height - 1

        Dim noise = 0.0
        Dim scaleAcc = 0.0
        Dim scale = 1.0

        For o = 0 To octaves - 1

          Dim pitch = width >> o
          Dim sampleX1 = CInt(Fix(x / pitch)) * pitch
          Dim sampleY1 = CInt(Fix(y / pitch)) * pitch

          Dim sampleX2 = (sampleX1 + pitch) Mod width
          Dim sampleY2 = (sampleY1 + pitch) Mod width

          Dim blendX = (x - sampleX1) / pitch
          Dim blendY = (y - sampleY1) / pitch

          Dim sampleT = (1.0F - blendX) * seed(sampleY1 * width + sampleX1) + blendX * seed(sampleY1 * width + sampleX2)
          Dim sampleB = (1.0F - blendX) * seed(sampleY2 * width + sampleX1) + blendX * seed(sampleY2 * width + sampleX2)

          scaleAcc += scale
          noise += (blendY * (sampleB - sampleT) + sampleT) * scale
          scale /= bias

        Next

        ' Scale to seed range
        output(y * width + x) = noise / scaleAcc

      Next
    Next

  End Sub

End Class