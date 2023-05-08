' Inspired by "Procedural Generation: Programming the Universe" -- @javidx9
' https://youtu.be/ZZY9YE7rZJw

Option Explicit On
Option Strict On
Option Infer On

' NOTE: Disabled Overflow Checks...

Imports Olc

Friend Module Program

  Sub Main()
    Dim demo As New OlcGalaxy
    If demo.Construct(512, 480, 2, 2, False) Then
      demo.Start()
    End If
  End Sub

End Module

Friend Class OlcGalaxy
  Inherits PixelGameEngine

  Private m_galaxyOffset As New Vf2d(0, 0)
  Private m_starSelected As Boolean = False
  Private m_selectedStarSeed1 As UInteger = 0
  Private m_selectedStarSeed2 As UInteger = 0

  Friend Sub New()
    AppName = "olcGalaxy"
  End Sub

  Protected Overrides Function OnUserCreate() As Boolean
    Return True
  End Function

  'Private nLehmer As UInteger = 0
  'Private Function Lehmer32() As UInteger
  '  nLehmer += &HE120FC15UI
  '  Dim tmp = nLehmer * &H4A39B70DUL
  '  Dim m1 = CUInt((tmp >> 32) Xor tmp)
  '  tmp = m1 * &H12FAD5C9UL
  '  Dim m2 = CUInt((tmp >> 32) Xor tmp)
  '  Return m2
  'End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    If elapsedTime <= 0.0001F Then Return True
    Clear(Presets.Black)

    'If GetKey(Key.SPACE).Released Then

    '  Dim rd As New Random()
    '  Dim mt As New Random(1000)
    '  Dim dist As New Random() : dist = New Random(dist.Next(0, 256))
    '  Dim tp1 = Now

    '  ' Randomness Tests
    '  For x = 0 To ScreenWidth() - 1
    '    For y = 0 To ScreenHeight() - 1

    '      Dim bIsStar = False
    '      Dim nSeed = y << 16 Or x

    '      ' Standard VB rand()
    '      'Randomize(nSeed)
    '      'bIsStar = CInt(Int((256 * Rnd()) + 1)) < 32

    '      ' System.Random
    '      'mt = New Random(nSeed)
    '      'bIsStar = dist.Next(0, 256) < 32

    '      ' Lehmer32
    '      nLehmer = nSeed
    '      bIsStar = Lehmer32() Mod 256 < 32

    '      Draw(x, y, If(bIsStar, Presets.White, Presets.Black))

    '    Next
    '  Next

    '  Dim tp2 = Now
    '  Dim elapsed = tp2 - tp1
    '  DrawString(3, 3, "Time: " + elapsed.TotalSeconds.ToString(), Presets.Red, 2)

    'End If

    'Return True


    If GetKey(Key.W).Held Then m_galaxyOffset.y -= 50.0F * elapsedTime
    If GetKey(Key.S).Held Then m_galaxyOffset.y += 50.0F * elapsedTime
    If GetKey(Key.A).Held Then m_galaxyOffset.x -= 50.0F * elapsedTime
    If GetKey(Key.D).Held Then m_galaxyOffset.x += 50.0F * elapsedTime

    Dim nSectorsX = ScreenWidth() \ 16
    Dim nSectorsY = ScreenHeight() \ 16

    Dim mouse As New Vi2d(GetMouseX() \ 16, GetMouseY() \ 16)
    Dim galaxy_mouse = mouse + m_galaxyOffset
    Dim screen_sector As New Vi2d(0, 0)

    For screen_sector.x = 0 To nSectorsX - 1
      For screen_sector.y = 0 To nSectorsY - 1

        Dim seed1 = CUInt(m_galaxyOffset.x) + CUInt(screen_sector.x)
        Dim seed2 = CUInt(m_galaxyOffset.y) + CUInt(screen_sector.y)

        Dim star As New StarSystem(seed1, seed2)
        If star.StarExists Then

          FillCircle(screen_sector.x * 16 + 8, screen_sector.y * 16 + 8, CInt(star.Diameter) \ 8, star.StarColour)

          ' For convenience highlight hovered star
          If mouse.x = screen_sector.x AndAlso mouse.y = screen_sector.y Then
            DrawCircle(screen_sector.x * 16 + 8, screen_sector.y * 16 + 8, 12, Presets.Yellow)
          End If

        End If

      Next
    Next

    ' Handle Mouse Click
    If GetMouse(0).Pressed Then

      Dim seed1 = CUInt(m_galaxyOffset.x) + CUInt(mouse.x)
      Dim seed2 = CUInt(m_galaxyOffset.y) + CUInt(mouse.y)

      Dim star As New StarSystem(seed1, seed2)
      If star.StarExists Then
        m_starSelected = True
        m_selectedStarSeed1 = seed1
        m_selectedStarSeed2 = seed2
      Else
        m_starSelected = False
      End If

    End If

    ' Draw Details of selected star system
    If m_starSelected Then

      ' Generate full star system
      Dim star As New StarSystem(m_selectedStarSeed1, m_selectedStarSeed2, True)

      ' Draw Window
      FillRect(8, 240, 496, 232, Presets.DarkBlue)
      DrawRect(8, 240, 496, 232, Presets.White)

      ' Draw Star
      Dim vBody As New Vi2d(14, 356)

      vBody.x += CInt(star.Diameter * 1.375)
      FillCircle(vBody, CInt(star.Diameter * 1.375), star.StarColour)
      vBody.x += CInt((star.Diameter * 1.375) + 8)

      ' Draw Planets
      For Each planet In star.Planets
        If vBody.x + planet.Diameter >= 496 Then Exit For

        vBody.x += CInt(Fix(planet.Diameter))
        FillCircle(vBody, CInt(planet.Diameter * 1.0), Presets.Red)

        Dim vMoon = vBody
        vMoon.y += CInt(Fix(planet.Diameter + 10))

        ' Draw Moons
        For Each moon In planet.Moons
          vMoon.y += CInt(Fix(moon))
          FillCircle(vMoon, CInt(moon * 1.0), Presets.Grey)
          vMoon.y += CInt(Fix(moon + 10))
        Next

        vBody.x += CInt(Fix(planet.Diameter + 8))

      Next

    End If

    Return True

  End Function

End Class

Public Module Constants
  Public ReadOnly g_starColours As UInteger() = {&HFFFFFFFFUI, &HFFD9FFFFUI, &HFFA3FFFFUI, &HFFFFC8C8UI, &HFFFFCB9DUI, &HFF9F9FFFUI, &HFF415EFFUI, &HFF28199DUI}
End Module

Public Class Planet
  Public Property Distance As Double
  Public Property Diameter As Double
  Public Property Foliage As Double
  Public Property Minerals As Double
  Public Property Water As Double
  Public Property Gases As Double
  Public Property Temperature As Double
  Public Property Population As Double
  Public Property Ring As Boolean
  Public Property Moons As New List(Of Double)
End Class

Public Class StarSystem

  Public Property Planets As New List(Of Planet)()
  Public Property StarExists As Boolean = False
  Public Property Diameter As Double = 0.0
  Public Property StarColour As Pixel = Presets.White

  Private m_procGen As UInteger = 0

  Public Sub New(x As UInteger, y As UInteger, Optional generateFullSystem As Boolean = False)

    ' Set seed based on location of star system
    m_procGen = (x And &HFFFFUI) << 16 Or (y And &HFFFFUI)

    ' Not all locations contain a system
    StarExists = (RndInt(0, 20) = 1)
    If Not StarExists Then
      Return
    End If

    ' Generate Star
    Diameter = RndDouble(10.0, 40.0)
    Dim c = RndInt(0, 8)
    _StarColour.N = g_starColours(c)

    ' When viewing the galaxy map, we only care about the star
    ' so abort early
    If Not generateFullSystem Then
      Return
    End If

    ' If we are viewing the system map, we need to generate the
    ' full system

    ' Generate Planets
    Dim dDistanceFromStar As Double = RndDouble(60.0, 200.0)
    Dim nPlanets As Integer = RndInt(0, 10)

    For i = 0 To nPlanets - 1

      Dim p As New Planet()
      p.Distance = dDistanceFromStar
      dDistanceFromStar += RndDouble(20.0, 200.0)
      p.Diameter = RndDouble(4.0, 20.0)

      ' Could make temperature a function of distance from star
      p.Temperature = RndDouble(-200.0, 300.0)

      ' Composition of planet
      p.Foliage = RndDouble(0.0, 1.0)
      p.Minerals = RndDouble(0.0, 1.0)
      p.Gases = RndDouble(0.0, 1.0)
      p.Water = RndDouble(0.0, 1.0)

      ' Normalize to 100%
      Dim dSum = 1.0 / (p.Foliage + p.Minerals + p.Gases + p.Water)
      p.Foliage *= dSum
      p.Minerals *= dSum
      p.Gases *= dSum
      p.Water *= dSum

      ' Population could be a function of other habitat encouraging
      ' properties, such as temperature and water
      p.Population = Math.Max(RndInt(-5000000, 20000000), 0)

      ' 10% of planets have a ring
      p.Ring = RndInt(0, 10) = 1

      ' Satellites (Moons)
      Dim nMoons = Math.Max(RndInt(-5, 5), 0)
      For n = 0 To nMoons - 1
        ' A moon is just a diameter for now, but it could be
        ' whatever you want!
        p.Moons.Add(RndDouble(1.0, 5.0))
      Next

      ' Add planet to list
      Planets.Add(p)

    Next

  End Sub

  Private Function RndDouble(min As Double, max As Double) As Double
    Return (Rnd() / &H7FFFFFFF) * (max - min) + min
  End Function

  Private Function RndInt(min As Integer, max As Integer) As Integer
    Return CInt(Fix((Rnd() Mod (max - min)) + min))
  End Function

  ' Modified from this for 64-bit systems:
  ' https://lemire.me/blog/2019/03/19/the-fastest-conventional-random-number-generator-that-can-pass-big-crush/
  ' Now I found the link again - Also, check out his blog, it's a fantastic resource!
  Private Function Rnd() As UInteger
    ' To handle overlow...
    If CULng(m_procGen) + &HE120FC15UL > UInteger.MaxValue Then
      m_procGen = CUInt((CULng(m_procGen) + &HE120FC15UL) - UInteger.MaxValue)
    Else
      m_procGen += &HE120FC15UI
    End If
    Dim tmp As ULong = CULng(m_procGen) * &H4A39B70DUL
    Dim m1 = (tmp >> 32UL) Xor tmp
    ' To handle overlow...
    If m1 > UInteger.MaxValue Then m1 -= UInteger.MaxValue
    tmp = CULng(Fix(m1)) * &H12FAD5C9UL                  ' Have to enable disable overflow checks...
    Dim m2 = (tmp >> 32) Xor tmp
    ' To handle overlow...
    If m2 > UInteger.MaxValue Then m2 -= UInteger.MaxValue
    Return CUInt(m2)
  End Function

End Class