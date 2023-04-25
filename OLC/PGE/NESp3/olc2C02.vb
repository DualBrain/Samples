Option Explicit On
Option Strict On
Option Infer On

Imports Olc

Public Class olc2C02

  Private tblName(1, 1023) As Byte
  Private tblPattern(1, 4095) As Byte
  Private tblPalette(31) As Byte

  Private palScreen(63) As Pixel
  Private sprScreen As Sprite
  Private sprNameTable(1) As Sprite
  Private sprPatternTable(1) As Sprite

  Public frame_complete As Boolean = False
  Private scanline As Short = 0
  Private cycle As Short = 0

  Private cart As Cartridge

  Public Sub New()

    palScreen(&H0) = New Olc.Pixel(84, 84, 84)
    palScreen(&H1) = New Olc.Pixel(0, 30, 116)
    palScreen(&H2) = New Olc.Pixel(8, 16, 144)
    palScreen(&H3) = New Olc.Pixel(48, 0, 136)
    palScreen(&H4) = New Olc.Pixel(68, 0, 100)
    palScreen(&H5) = New Olc.Pixel(92, 0, 48)
    palScreen(&H6) = New Olc.Pixel(84, 4, 0)
    palScreen(&H7) = New Olc.Pixel(60, 24, 0)
    palScreen(&H8) = New Olc.Pixel(32, 42, 0)
    palScreen(&H9) = New Olc.Pixel(8, 58, 0)
    palScreen(&HA) = New Olc.Pixel(0, 64, 0)
    palScreen(&HB) = New Olc.Pixel(0, 60, 0)
    palScreen(&HC) = New Olc.Pixel(0, 50, 60)
    palScreen(&HD) = New Olc.Pixel(0, 0, 0)
    palScreen(&HE) = New Olc.Pixel(0, 0, 0)
    palScreen(&HF) = New Olc.Pixel(0, 0, 0)

    palScreen(&H10) = New Olc.Pixel(152, 150, 152)
    palScreen(&H11) = New Olc.Pixel(8, 76, 196)
    palScreen(&H12) = New Olc.Pixel(48, 50, 236)
    palScreen(&H13) = New Olc.Pixel(92, 30, 228)
    palScreen(&H14) = New Olc.Pixel(136, 20, 176)
    palScreen(&H15) = New Olc.Pixel(160, 20, 100)
    palScreen(&H16) = New Olc.Pixel(152, 34, 32)
    palScreen(&H17) = New Olc.Pixel(120, 60, 0)
    palScreen(&H18) = New Olc.Pixel(84, 90, 0)
    palScreen(&H19) = New Olc.Pixel(40, 114, 0)
    palScreen(&H1A) = New Olc.Pixel(8, 124, 0)
    palScreen(&H1B) = New Olc.Pixel(0, 118, 40)
    palScreen(&H1C) = New Olc.Pixel(0, 102, 120)
    palScreen(&H1D) = New Olc.Pixel(0, 0, 0)
    palScreen(&H1E) = New Olc.Pixel(0, 0, 0)
    palScreen(&H1F) = New Olc.Pixel(0, 0, 0)

    palScreen(&H20) = New Olc.Pixel(236, 238, 236)
    palScreen(&H21) = New Olc.Pixel(76, 154, 236)
    palScreen(&H22) = New Olc.Pixel(120, 124, 236)
    palScreen(&H23) = New Olc.Pixel(176, 98, 236)
    palScreen(&H24) = New Olc.Pixel(228, 84, 236)
    palScreen(&H25) = New Olc.Pixel(236, 88, 180)
    palScreen(&H26) = New Olc.Pixel(236, 106, 100)
    palScreen(&H27) = New Olc.Pixel(212, 136, 32)
    palScreen(&H28) = New Olc.Pixel(160, 170, 0)
    palScreen(&H29) = New Olc.Pixel(116, 196, 0)
    palScreen(&H2A) = New Olc.Pixel(76, 208, 32)
    palScreen(&H2B) = New Olc.Pixel(56, 204, 108)
    palScreen(&H2C) = New Olc.Pixel(56, 180, 204)
    palScreen(&H2D) = New Olc.Pixel(60, 60, 60)
    palScreen(&H2E) = New Olc.Pixel(0, 0, 0)
    palScreen(&H2F) = New Olc.Pixel(0, 0, 0)

    palScreen(&H30) = New Olc.Pixel(236, 238, 236)
    palScreen(&H31) = New Olc.Pixel(168, 204, 236)
    palScreen(&H32) = New Olc.Pixel(188, 188, 236)
    palScreen(&H33) = New Olc.Pixel(212, 178, 236)
    palScreen(&H34) = New Olc.Pixel(236, 174, 236)
    palScreen(&H35) = New Olc.Pixel(236, 174, 212)
    palScreen(&H36) = New Olc.Pixel(236, 180, 176)
    palScreen(&H37) = New Olc.Pixel(228, 196, 144)
    palScreen(&H38) = New Olc.Pixel(204, 210, 120)
    palScreen(&H39) = New Olc.Pixel(180, 222, 120)
    palScreen(&H3A) = New Olc.Pixel(168, 226, 144)
    palScreen(&H3B) = New Olc.Pixel(152, 226, 180)
    palScreen(&H3C) = New Olc.Pixel(160, 214, 228)
    palScreen(&H3D) = New Olc.Pixel(160, 162, 160)
    palScreen(&H3E) = New Olc.Pixel(0, 0, 0)
    palScreen(&H3F) = New Olc.Pixel(0, 0, 0)

    sprScreen = New Sprite(256, 240)
    sprNameTable(0) = New Sprite(256, 240)
    sprNameTable(1) = New Sprite(256, 240)
    sprPatternTable(0) = New Sprite(128, 128)
    sprPatternTable(1) = New Sprite(128, 128)

  End Sub

  Public Function GetScreen() As Sprite
    Return sprScreen
  End Function

  Public Function GetNameTable(i As Byte) As Sprite
    Return sprNameTable(i)
  End Function

  Public Function GetPatternTable(i As Byte) As Sprite
    Return sprPatternTable(i)
  End Function

  Public Function CpuRead(addr As UShort, Optional rdonly As Boolean = False) As Byte

    Dim data As Byte = &H0

    Select Case addr
      Case &H0 ' Control
        ' ...
      Case &H1 ' Mask
        ' ...
      Case &H2 ' Status
        ' ...
      Case &H3 ' OAM Address
        ' ...
      Case &H4 ' OAM Data
        ' ...
      Case &H5 ' Scroll
        ' ...
      Case &H6 ' PPU Address
        ' ...
      Case &H7 ' PPU Data
        ' ...
    End Select

    Return data

  End Function

  Public Sub CpuWrite(addr As UShort, data As Byte)

    Select Case addr
      Case &H0 ' Control
        ' ...
      Case &H1 ' Mask
        ' ...
      Case &H2 ' Status
        ' ...
      Case &H3 ' OAM Address
        ' ...
      Case &H4 ' OAM Data
        ' ...
      Case &H5 ' Scroll
        ' ...
      Case &H6 ' PPU Address
        ' ...
      Case &H7 ' PPU Data
        ' ...
    End Select

  End Sub

  Public Function PpuRead(addr As UShort, Optional rdonly As Boolean = False) As Byte

    Dim data As Byte = &H0
    addr = addr And &H3FFFUS

    If cart.PpuRead(addr, data) Then

    End If

    Return data

  End Function

  Public Sub PpuWrite(addr As UShort, data As Byte)

    addr = addr And &H3FFFUS

    If cart.PpuWrite(addr, data) Then

    End If

  End Sub

  Public Sub ConnectCartridge(cartridge As Cartridge)
    cart = cartridge
  End Sub

  Public Sub Clock()

    ' Fake some noise for now
    sprScreen.SetPixel(cycle - 1, scanline, palScreen(If((Rnd() >= 0.5), &H3F, &H30)))

    ' Advance renderer - it never stops, it's relentless
    cycle += 1S
    If cycle >= 341 Then
      cycle = 0
      scanline += 1S
      If scanline >= 261 Then
        scanline = -1
        frame_complete = True
      End If
    End If

  End Sub

End Class