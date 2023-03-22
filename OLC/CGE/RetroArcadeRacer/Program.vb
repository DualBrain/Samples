' Inspired by: "Code-It-Yourself! Retro Arcade Racing Game - Programming from Scratch (Quick and Simple C++)" -- @javidx9
' https://youtu.be/KkMZI5Jbf18

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports System.Text.Encodings.Web

Module Program
  Sub Main() 'args As String())
    Dim game As New RetroArcadeRacer
    game.ConstructConsole(160, 100, 8, 8)
    game.Start()
  End Sub

End Module

Class RetroArcadeRacer
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_carPos As Double = 0.0
  Private m_distance As Double = 0.0
  Private m_speed As Double = 0.0

  Private m_curvature As Double = 0.0
  Private m_trackCurvature As Double = 0.0
  Private m_playerCurvature As Double = 0.0
  Private m_trackDistance As Double = 0.0

  Private m_currentLapTime As Double = 0.0
  Private m_lapTimes As New List(Of Double)

  Private m_track As New List(Of (Curve As Single, Distance As Single))

  Public Overrides Function OnUserCreate() As Boolean

    m_track.Add((0.0, 10.0)) ' Short section for start/finish
    m_track.Add((0.0, 200.0))
    m_track.Add((1.0, 200.0))
    m_track.Add((0.0, 400.0))
    m_track.Add((-1.0, 100.0))
    m_track.Add((0.0, 200.0))
    m_track.Add((-1.0, 200.0))
    m_track.Add((1.0, 200.0))
    m_track.Add((0.2, 500.0))
    m_track.Add((0.0, 200.0))

    For Each entry In m_track
      m_trackDistance += entry.Distance
    Next

    m_lapTimes.Add(0.0)
    m_lapTimes.Add(0.0)
    m_lapTimes.Add(0.0)
    m_lapTimes.Add(0.0)
    m_lapTimes.Add(0.0)

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    If m_keys(VK_UP).Held OrElse m_keys(VK_SPACE).Held Then
      m_speed += 2.0 * elapsedTime
    Else
      m_speed -= 1.0 * elapsedTime
    End If

    Dim carDirection = 0
    If m_keys(VK_LEFT).Held Then m_playerCurvature -= 0.7 * elapsedTime : carDirection = -1
    If m_keys(VK_RIGHT).Held Then m_playerCurvature += 0.7 * elapsedTime : carDirection = 1

    If Math.Abs(m_playerCurvature - m_trackCurvature) >= 0.8 Then m_speed -= 5.0

    ' Clamp Speed
    If m_speed < 0 Then m_speed = 0
    If m_speed > 1 Then m_speed = 1

    ' Move car along track according to car speed
    m_distance += (70 * m_speed) * elapsedTime

    ' Get Point on track
    Dim offset = 0.0
    Dim trackSection = 0

    m_currentLapTime += elapsedTime
    If m_distance >= m_trackDistance Then
      m_distance -= m_trackDistance
      m_lapTimes.Insert(0, m_currentLapTime)
      m_lapTimes.RemoveAt(m_lapTimes.Count - 1)
      m_currentLapTime = 0.0
    End If

    ' Find position on track (could optimise)
    While trackSection < m_track.Count AndAlso offset <= m_distance
      offset += m_track(trackSection).Distance
      trackSection += 1
    End While

    Dim targetCurvature = m_track(trackSection - 1).Curve
    Dim trackCurveDiff = (targetCurvature - m_curvature) * elapsedTime * m_speed
    m_curvature += trackCurveDiff

    m_trackCurvature += m_curvature * elapsedTime * m_speed

    ' Clear screen
    'Fill(0, 0, ScreenWidth, ScreenHeight, AscW(" "), 0)

    ' Draw Sky - light blue and dark blue
    For y = 0 To ScreenHeight() \ 2
      For x = 0 To ScreenWidth() - 1
        Draw(x, y, If(y < ScreenHeight() \ 4, PIXEL_HALF, PIXEL_SOLID), FG_DARK_BLUE)
      Next
    Next

    ' Draw Scenery - our hills are a rectified sine wave, where the phase
    ' accumulated track curvature
    For x = 0 To ScreenWidth() - 1
      Dim hillHeight = Math.Abs(Math.Sin(x * 0.01 + m_trackCurvature) * 16.0)
      For y = ScreenHeight() \ 2 - CInt(hillHeight) To ScreenHeight() \ 2
        Draw(x, y, PIXEL_SOLID, FG_DARK_YELLOW)
      Next
    Next

    For y = 0 To ScreenHeight() \ 2
      For x = 0 To ScreenWidth() - 1

        Dim perspective = y / (ScreenHeight() / 2)

        Dim middlePoint = 0.5 + m_curvature * Math.Pow((1.0 - perspective), 3)
        Dim roadWidth = 0.1 + (perspective * 0.8) '0.6
        Dim clipWidth = roadWidth * 0.15
        roadWidth *= 0.5

        Dim leftGrass = (middlePoint - roadWidth - clipWidth) * ScreenWidth()
        Dim leftClip = (middlePoint - roadWidth) * ScreenWidth()
        Dim rightClip = (middlePoint + roadWidth) * ScreenWidth()
        Dim rightGrass = (middlePoint + roadWidth + clipWidth) * ScreenWidth()

        Dim row = ScreenHeight() \ 2 + y

        Dim grassColour = If(Math.Sin(20.0F * Math.Pow(1.0F - perspective, 3) + m_distance * 0.1F) > 0.0F, FG_GREEN, FG_DARK_GREEN)
        Dim clipColour = If(Math.Sin(80.0F * Math.Pow(1.0F - perspective, 2) + m_distance) > 0.0F, FG_RED, FG_WHITE)

        Dim roadColour = If(trackSection - 1 = 0, FG_WHITE, FG_GREY)

        If x >= 0 AndAlso x < leftGrass Then Draw(x, row, PIXEL_SOLID, grassColour)
        If x >= leftGrass AndAlso x < leftClip Then Draw(x, row, PIXEL_SOLID, clipColour)
        If x >= leftClip AndAlso x < rightClip Then Draw(x, row, PIXEL_SOLID, roadColour)
        If x >= rightClip AndAlso x < rightGrass Then Draw(x, row, PIXEL_SOLID, clipColour)
        If x >= rightGrass AndAlso x < ScreenWidth() Then Draw(x, row, PIXEL_SOLID, grassColour)

      Next
    Next

    ' Draw Car
    m_carPos = m_playerCurvature - m_trackCurvature
    Dim carPos = ScreenWidth() \ 2 + CInt((ScreenWidth() * m_carPos) / 2) - 7

    Select Case carDirection
      Case 0
        DrawStringAlpha(carPos, 80, "   ||####||   ")
        DrawStringAlpha(carPos, 81, "      ##      ")
        DrawStringAlpha(carPos, 82, "     ####     ")
        DrawStringAlpha(carPos, 83, "     ####     ")
        DrawStringAlpha(carPos, 84, "|||  ####  |||")
        DrawStringAlpha(carPos, 85, "|||########|||")
        DrawStringAlpha(carPos, 86, "|||  ####  |||")
      Case +1
        DrawStringAlpha(carPos, 80, "      //####//")
        DrawStringAlpha(carPos, 81, "         ##   ")
        DrawStringAlpha(carPos, 82, "       ####   ")
        DrawStringAlpha(carPos, 83, "      ####    ")
        DrawStringAlpha(carPos, 84, "///  ####//// ")
        DrawStringAlpha(carPos, 85, "//#######///O ")
        DrawStringAlpha(carPos, 86, "/// #### //// ")
      Case -1
        DrawStringAlpha(carPos, 80, "\\####\\      ")
        DrawStringAlpha(carPos, 81, "   ##         ")
        DrawStringAlpha(carPos, 82, "   ####       ")
        DrawStringAlpha(carPos, 83, "    ####      ")
        DrawStringAlpha(carPos, 84, " \\\\####  \\\")
        DrawStringAlpha(carPos, 85, " O\\#######\\")
        DrawStringAlpha(carPos, 86, " \\\\ #### \\\")
    End Select

    ' Draw Stats
    DrawString(0, 0, $"Distance        : {m_distance}")
    DrawString(0, 1, $"Target Curvature: {m_curvature}")
    DrawString(0, 2, $"Player Curvature: {m_playerCurvature}")
    DrawString(0, 3, $"Player Speed    : {m_speed}")
    DrawString(0, 4, $"Track Curvature : {m_trackCurvature}")

    Dim disp_time = Function(t As Double) As String ' Lambda expression to turn floating point seconds into minutes:seconds:millis string
                      Dim nMinutes = CInt(t) \ 60
                      Dim nSeconds = t - (nMinutes * 60)
                      Dim nMilliSeconds = (t - nSeconds) * 1000.0F
                      Return $"{nMinutes}.{nSeconds}:{nMilliSeconds}"
                    End Function

    DrawString(10, 8, disp_time(m_currentLapTime))

    ' Display last 5 lap times
    Dim j = 10
    For Each lapTime In m_lapTimes
      DrawString(10, j, disp_time(lapTime))
      j += 1
    Next

    Return True

  End Function

End Class