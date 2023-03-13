' Inspired by: "Code-It-Yourself! Frogger - Programming from Scratch (Quick and Simple C++)" -- @javidx9
' https://youtu.be/QJnZ5QmpXOE

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine

Module Program

  Sub Main() 'args As String())
    Dim game As New Frogger
    game.ConstructConsole(128, 80, 12, 12)
    game.Start()
  End Sub

End Module

Class Frogger
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_lanes As New List(Of (Velocity As Single, String))
  Private m_timeSinceStart As Single = 0.0
  Private m_cellSize As Integer = 8

  Private m_frogX As Single = 8.0
  Private m_frogY As Single = 9.0

  Private m_danger() As Boolean

  Private m_spriteBus As Sprite
  Private m_spriteLog As Sprite
  Private m_spriteWater As Sprite
  Private m_spriteFrog As Sprite
  Private m_spritePavement As Sprite
  Private m_spriteCar1 As Sprite
  Private m_spriteCar2 As Sprite
  Private m_spriteWall As Sprite
  Private m_spriteHome As Sprite

  Public Overrides Function OnUserCreate() As Boolean

    m_lanes.Add((0.0, "wwwhhwwwhhwwwhhwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww")) ' 64 elements per lane
    m_lanes.Add((-3.0, ",,,jllk,,jllllk,,,,,,,jllk,,,,,jk,,,jlllk,,,,jllllk,,,,jlllk,,,,"))
    m_lanes.Add((3.0, ",,,,jllk,,,,,jllk,,,,jllk,,,,,,,,,jllk,,,,,jk,,,,,,jllllk,,,,,,,"))
    m_lanes.Add((2.0, ",,jlk,,,,,jlk,,,,,jk,,,,,jlk,,,jlk,,,,jk,,,,jllk,,,,jk,,,,,,jk,,"))
    m_lanes.Add((0.0, "pppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppp"))
    m_lanes.Add((-3.0, "....asdf.......asdf....asdf..........asdf........asdf....asdf..."))
    m_lanes.Add((3.0, ".....ty..ty....ty....ty.....ty........ty..ty.ty......ty.......ty"))
    m_lanes.Add((-4.0, "..zx.....zx.........zx..zx........zx...zx...zx....zx...zx...zx.."))
    m_lanes.Add((2.0, "..ty.....ty.......ty.....ty......ty..ty.ty.......ty....ty......."))
    m_lanes.Add((0.0, "pppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppp"))

    'm_spriteCar2 = New Sprite("../../../../SpriteEditor/car2.spr")
    m_spriteBus = New Sprite("bus.spr")
    m_spriteLog = New Sprite("log.spr")
    m_spriteWater = New Sprite("water.spr")
    m_spritePavement = New Sprite("pavement.spr")
    m_spriteWall = New Sprite("wall.spr")
    m_spriteFrog = New Sprite("frog.spr")
    m_spriteHome = New Sprite("home.spr")
    m_spriteCar1 = New Sprite("car1.spr")
    m_spriteCar2 = New Sprite("car2.spr")
    m_spriteFrog = New Sprite("frog.spr")

    ReDim m_danger(ScreenWidth() * ScreenHeight())

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    m_timeSinceStart += elapsedTime

    'Cls()

    ' Handle input
    If m_keys(VK_UP).bReleased Then m_frogY -= 1.0!
    If m_keys(VK_DOWN).bReleased Then m_frogY += 1.0!
    If m_keys(VK_LEFT).bReleased Then m_frogX -= 1.0!
    If m_keys(VK_RIGHT).bReleased Then m_frogX += 1.0!

    ' Frog is moved by platforms
    If m_frogY <= 3 Then m_frogX -= elapsedTime * m_lanes(CInt(m_frogY)).Velocity

    ' Collision detection - check four corners of frog against danger buffer
    Dim tl = m_danger(CInt(m_frogY * m_cellSize + 1) * ScreenWidth() + CInt(m_frogX * m_cellSize + 1))
    Dim tr = m_danger(CInt(m_frogY * m_cellSize + 1) * ScreenWidth() + CInt((m_frogX + 1) * m_cellSize - 1))
    Dim bl = m_danger(CInt((m_frogY + 1) * m_cellSize - 1) * ScreenWidth() + CInt(m_frogX * m_cellSize + 1))
    Dim br = m_danger(CInt((m_frogY + 1) * m_cellSize - 1) * ScreenWidth() + CInt((m_frogX + 1) * m_cellSize - 1))

    If tl OrElse tr OrElse bl OrElse br Then
      ' Frogs been hit :-(
      m_frogX = 8.0
      m_frogY = 9.0
    End If

    ' Draw Lanes
    Dim x = -1, y = 0
    For Each lane In m_lanes
      ' Find offset
      Dim startPos = CInt(Fix(m_timeSinceStart * lane.Velocity)) Mod 64
      If startPos < 0 Then startPos = 64 - (Math.Abs(startPos) Mod 64)

      Dim cellOffset = CInt(Fix(m_cellSize * m_timeSinceStart * lane.Velocity)) Mod m_cellSize

      For i = 0 To (ScreenWidth() \ m_cellSize) + 1
        Dim graphic = lane.Item2((startPos + i) Mod 64)
        Select Case graphic

          Case "a"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteBus, 0, 0, 8, 8) ' Bus 1
          Case "s"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteBus, 8, 0, 8, 8) ' Bus 2
          Case "d"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteBus, 16, 0, 8, 8) ' Bus 3
          Case "f"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteBus, 24, 0, 8, 8) ' Bus 4

          Case "j"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteLog, 0, 0, 8, 8) ' Log Start
          Case "l"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteLog, 8, 0, 8, 8) ' Log Middle
          Case "k"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteLog, 16, 0, 8, 8) ' Log End

          Case "z"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteCar1, 0, 0, 8, 8) ' Car1 Back
          Case "x"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteCar1, 8, 0, 8, 8) ' Car1 Front
          Case "t"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteCar2, 0, 0, 8, 8) ' Car2 Back
          Case "y"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteCar2, 8, 0, 8, 8) ' Car2 Front

          Case "w"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteWall, 0, 0, 8, 8) ' Wall
          Case "h"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteHome, 0, 0, 8, 8) ' Home
          Case ","c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spriteWater, 0, 0, 8, 8) ' Water
          Case "p"c : DrawPartialSprite((x + i) * m_cellSize - cellOffset, y * m_cellSize, m_spritePavement, 0, 0, 8, 8) ' Pavement

          Case Else
            Fill((x + i) * m_cellSize - cellOffset, y * m_cellSize, (x + i + 1) * m_cellSize - cellOffset, (y + 1) * m_cellSize, AscW(" ")) ' Road

        End Select

        For j = (x + i) * m_cellSize - cellOffset To ((x + i + 1) * m_cellSize - cellOffset) - 1
          For k = y * m_cellSize To ((y + 1) * m_cellSize) - 1
            If j >= 0 AndAlso j < ScreenWidth() AndAlso k >= 0 AndAlso k < ScreenHeight() Then
              m_danger(k * ScreenWidth() + j) = If(graphic = "." OrElse graphic = "j" OrElse graphic = "k" OrElse graphic = "l" OrElse graphic = "p" OrElse graphic = "h", False, True)
            End If
          Next
        Next

      Next
        y += 1
    Next

    ' Draw Frog
    DrawSprite(m_frogX * m_cellSize, m_frogY * m_cellSize, m_spriteFrog)

    Return True

  End Function

End Class
