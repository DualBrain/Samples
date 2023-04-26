Option Explicit On
Option Strict On
Option Infer On

Imports System.Collections.Specialized
Imports System.Runtime.InteropServices
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

  Public Class StatusBits

    Public Property Reg As Byte

    Public Property SpriteOverflow As Boolean
      Get
        Return ((Reg And &H20) >> 5) = 1
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H20)
        Else
          Reg = CByte(Reg And Not &H20)
        End If
      End Set
    End Property

    Public Property SpriteZeroHit As Boolean
      Get
        Return ((Reg And &H40) >> 6) = 1
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H40)
        Else
          Reg = CByte(Reg And Not &H40)
        End If
      End Set
    End Property

    Public Property vertical_blank As Boolean
      Get
        Return ((Reg And &H80) >> 7) = 1
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H80)
        Else
          Reg = CByte(Reg And Not &H80)
        End If
      End Set
    End Property

    Public Sub New()
    End Sub

    Public Sub New(reg As Byte)
      Me.Reg = reg
    End Sub

  End Class

  Private status As New StatusBits()

  Public Class MaskBits

    Public Property Reg As Byte

    Public Property Grayscale() As Boolean
      Get
        Return (Reg And &H1) = &H1
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H1)
        Else
          Reg = CByte(Reg And Not &H1)
        End If
      End Set
    End Property

    Public Property RenderBackgroundLeft() As Boolean
      Get
        Return (Reg And &H2) = &H2
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H2)
        Else
          Reg = CByte(Reg And Not &H2)
        End If
      End Set
    End Property

    Public Property RenderSpritesLeft() As Boolean
      Get
        Return (Reg And &H4) = &H4
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H4)
        Else
          Reg = CByte(Reg And Not &H4)
        End If
      End Set
    End Property

    Public Property render_background() As Boolean
      Get
        Return (Reg And &H8) = &H8
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H8)
        Else
          Reg = CByte(Reg And Not &H8)
        End If
      End Set
    End Property

    Public Property render_sprites() As Boolean
      Get
        Return (Reg And &H10) = &H10
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H10)
        Else
          Reg = CByte(Reg And Not &H10)
        End If
      End Set
    End Property

    Public Property EnhanceRed() As Boolean
      Get
        Return (Reg And &H20) = &H20
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H20)
        Else
          Reg = CByte(Reg And Not &H20)
        End If
      End Set
    End Property

    Public Property EnhanceGreen() As Boolean
      Get
        Return (Reg And &H40) = &H40
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H40)
        Else
          Reg = CByte(Reg And Not &H40)
        End If
      End Set
    End Property

    Public Property EnhanceBlue() As Boolean
      Get
        Return (Reg And &H80) = &H80
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H80)
        Else
          Reg = CByte(Reg And Not &H80)
        End If
      End Set
    End Property

  End Class

  Private mask As New MaskBits()

  Public Structure PPUCTRL

    Public Property Reg As Byte

    Public Property nametable_x As Boolean
      Get
        Return (Reg And &H1) = &H1
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H1)
        Else
          Reg = CByte(Reg And &HFE)
        End If
      End Set
    End Property

    Public Property nametable_y As Boolean
      Get
        Return (Reg And &H2) = &H2
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H2)
        Else
          Reg = CByte(Reg And &HFD)
        End If
      End Set
    End Property

    Public Property increment_mode As Boolean
      Get
        Return (Reg And &H4) = &H4
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H4)
        Else
          Reg = CByte(Reg And &HB)
        End If
      End Set
    End Property

    Public Property pattern_sprite As Boolean
      Get
        Return (Reg And &H8) = &H8
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H8)
        Else
          Reg = CByte(Reg And &HF7)
        End If
      End Set
    End Property

    Public Property pattern_background As Boolean
      Get
        Return (Reg And &H10) = &H10
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H10)
        Else
          Reg = CByte(Reg And &HEF)
        End If
      End Set
    End Property

    Public Property sprite_size As Boolean
      Get
        Return (Reg And &H20) = &H20
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H20)
        Else
          Reg = CByte(Reg And &HDF)
        End If
      End Set
    End Property

    Public Property slave_mode As Boolean ' unused
      Get
        Return (Reg And &H40) = &H40
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H40)
        Else
          Reg = CByte(Reg And &HB)
        End If
      End Set
    End Property

    Public Property enable_nmi As Boolean
      Get
        Return (Reg And &H80) = &H80
      End Get
      Set(value As Boolean)
        If value Then
          Reg = CByte(Reg Or &H80)
        Else
          Reg = CByte(Reg And &H7F)
        End If
      End Set
    End Property

  End Structure

  Private control As New PPUCTRL()

  Public Class LoopyRegister

    Public Property Reg As UShort = &H0

    Public Property coarse_x As UShort
      Get
        Return (Reg And &HFUS)
      End Get
      Set(value As UShort)
        Reg = (Reg And &HFFE0US) Or (value And &HFUS)
      End Set
    End Property

    Public Property coarse_y As UShort
      Get
        Return ((Reg >> 5) And &HFUS)
      End Get
      Set(value As UShort)
        Reg = (Reg And &HFC1FUS) Or ((value And &HFUS) << 5)
      End Set
    End Property

    Public Property nametable_x As UShort
      Get
        Return ((Reg >> 10) And &H1US)
      End Get
      Set(value As UShort)
        Reg = (Reg And &HFBFFUS) Or ((value And &H1US) << 10)
      End Set
    End Property

    Public Property nametable_y As UShort
      Get
        Return ((Reg >> 11) And &H1US)
      End Get
      Set(value As UShort)
        Reg = (Reg And &HFBFFUS) Or ((value And &H1US) << 11)
      End Set
    End Property

    Public Property fine_y As UShort
      Get
        Return ((Reg >> 12) And &H7US)
      End Get
      Set(value As UShort)
        Reg = (Reg And &H8FFFUS) Or ((value And &H7US) << 12)
      End Set
    End Property

    Public Property Unused As UShort
      Get
        Return ((Reg >> 15) And &H1US)
      End Get
      Set(value As UShort)
        Reg = (Reg And &H7FFFUS) Or ((value And &H1US) << 15)
      End Set
    End Property

  End Class

  Private vram_addr As New LoopyRegister() ' Active "pointer" address into nametable to extract background tile info
  Private tram_addr As New LoopyRegister() ' Temporary store of information to be "transferred" into "pointer" at various times

  ' Pixel offset horizontally
  Private fine_x As Byte = &H0

  ' Internal communications
  Private address_latch As Byte = &H0
  Private ppu_data_buffer As Byte = &H0

  ' Pixel "dot" position information
  Private scanline As Short = 0
  Private cycle As Short = 0

  ' Background rendering
  Private bg_next_tile_id As Byte = &H0
  Private bg_next_tile_attrib As Byte = &H0
  Private bg_next_tile_lsb As Byte = &H0
  Private bg_next_tile_msb As Byte = &H0
  Private bg_shifter_pattern_lo As UShort = &H0
  Private bg_shifter_pattern_hi As UShort = &H0
  Private bg_shifter_attrib_lo As UShort = &H0
  Private bg_shifter_attrib_hi As UShort = &H0

  ' The Cartridge or "GamePak"
  Private cart As Cartridge

  ' Interface
  Public nmi As Boolean = False

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
    ' Simply returns the current sprite holding the rendered screen
    Return sprScreen
  End Function

  Public Function GetPatternTable(i As Byte, palette As Integer) As Sprite

    ' This function draw the CHR ROM for a given pattern table into
    ' an olc:Sprite, using a specified palette. Pattern tables consist
    ' of 16x16 "tiles or characters". It Is independent of the running
    ' emulation And using it does Not change the systems state, though
    ' it gets all the data it needs from the live system. Consequently,
    ' if the game has Not yet established palettes Or mapped to relevant
    ' CHR ROM banks, the sprite may look empty. This approach permits a 
    ' "live" extraction of the pattern table exactly how the NES, And 
    ' ultimately the player would see it.
    '
    ' A tile consists of 8x8 pixels. On the NES, pixels are 2 bits, which
    ' gives an index into 4 different colours of a specific palette. There
    ' are 8 palettes to choose from. Colour "0" in each palette Is effectively
    ' considered transparent, as those locations in memory "mirror" the global
    ' background colour being used. This mechanics of this are shown in 
    ' detail in ppuRead() & ppuWrite()
    '
    ' Characters on NES
    ' ~~~~~~~~~~~~~~~~~
    ' The NES stores characters using 2-bit pixels. These are Not stored sequentially
    ' but in singular bit planes. For example:
    '
    ' 2-Bit Pixels       LSB Bit Plane     MSB Bit Plane
    ' 0 0 0 0 0 0 0 0	   0 0 0 0 0 0 0 0   0 0 0 0 0 0 0 0
    ' 0 1 1 0 0 1 1 0	   0 1 1 0 0 1 1 0   0 0 0 0 0 0 0 0
    ' 0 1 2 0 0 2 1 0	   0 1 1 0 0 1 1 0   0 0 1 0 0 1 0 0
    ' 0 0 0 0 0 0 0 0 =  0 0 0 0 0 0 0 0 + 0 0 0 0 0 0 0 0
    ' 0 1 1 0 0 1 1 0	   0 1 1 0 0 1 1 0   0 0 0 0 0 0 0 0
    ' 0 0 1 1 1 1 0 0	   0 0 1 1 1 1 0 0   0 0 0 0 0 0 0 0
    ' 0 0 0 2 2 0 0 0	   0 0 0 1 1 0 0 0   0 0 0 1 1 0 0 0
    ' 0 0 0 0 0 0 0 0	   0 0 0 0 0 0 0 0   0 0 0 0 0 0 0 0
    '
    ' The planes are stored as 8 bytes of LSB, followed by 8 bytes of MSB

    ' Loop through all 16x16 tiles
    For nTileY = 0 To 15
      For nTileX = 0 To 15

        ' Convert the 2D title coordinate into a 1D offset into the pattern table memory
        Dim nOffset = nTileY * 256US + nTileX * 16US

        ' Now loop through 8 rows of 8 pixels
        For row = 0 To 7

          ' For each row, we need to read both bit planes of the character
          ' in order to extract the least significant And most significant 
          ' bits of the 2 bit pixel value. in the CHR ROM, each character
          ' Is stored as 64 bits of lsb, followed by 64 bits of msb. This
          ' conveniently means that two corresponding rows are always 8
          ' bytes apart in memory.
          Dim tile_lsb As Integer = PpuRead(CUShort(i * &H1000US + nOffset + row + &H0US))
          Dim tile_msb As Integer = PpuRead(CUShort(i * &H1000US + nOffset + row + &H8US))

          ' Now we have a single row of the two bit planes for the character
          ' we need to iterate through the 8-bit words, combining them to give
          ' us the final pixel index
          For col = 0 To 7
            ' We can get the index value by simply adding the bits together
            ' but we're only interested in the lsb of the row words because...
            Dim pixel = (tile_lsb And &H1) + (tile_msb And &H1)

            ' ...we will shift the row words 1 bit right for each column of
            ' the character
            tile_lsb >>= 1 : tile_msb >>= 1

            ' Now we know the location and NES pixel value for a specific location
            ' in the pattern table, we can translate that to a screen colour, and an
            ' (x,y) location in the sprite
            sprPatternTable(i).SetPixel(nTileX * 8 + (7 - col), ' Because we are using the lsb of the row word first we are effectively reading the row from right to left, so we need to draw the row "backwards"
                                        nTileY * 8 + row,
                                        GetColourFromPaletteRam(palette, pixel))

          Next
        Next
      Next
    Next

    ' Finally return the updated sprite representing the pattern table
    Return sprPatternTable(i)

  End Function

  Public Function GetColourFromPaletteRam(palette As Integer, pixel As Integer) As Pixel

    ' This is a convenience function that takes a specified palette and pixel
    ' index and returns the appropriate screen colour.
    ' "0x3F00"       - Offset into PPU addressable range where palettes are stored
    ' "palette << 2" - Each palette is 4 bytes in size
    ' "pixel"        - Each pixel index is either 0, 1, 2 or 3
    ' "& 0x3F"       - Stops us reading beyond the bounds of the palScreen array
    Return palScreen(PpuRead(CUShort(&H3F00US + (palette << 2US) + pixel) And &H3FUS))

    ' Note: We don't access tblPalette directly here, instead we know that ppuRead()
    ' will map the address onto the separate small RAM attached to the PPU bus.

  End Function

  Public Function GetNameTable(i As Byte) As Sprite
    ' As of now unused, but a placeholder for nametable visualisation in the future
    Return sprNameTable(i)
  End Function

  Public Function CpuRead(addr As UShort, Optional rdonly As Boolean = False) As Byte

    Dim data As Byte = &H0

    If rdonly Then
      ' Reading from PPU registers can affect their contents
      ' so this read only option is used for examining the
      ' state of the PPU without changing its state. This is
      ' really only used in debug mode.
      Select Case addr
        Case &H0 ' Control
          data = control.Reg
        Case &H1 ' Mask
          data = mask.Reg
        Case &H2 ' Status
          data = status.Reg
        Case &H3 ' OAM Address
        Case &H4 ' OAM Data
        Case &H5 ' Scroll
        Case &H6 ' PPU Address
        Case &H7 ' PPU Data
      End Select

    Else

      Select Case addr

        Case &H0 ' Control - Not readable

        Case &H1 ' Mask - Not Readable

        Case &H2 ' Status

          ' Reading from the status register has the effect of resetting
          ' different parts of the circuit. Only the top three bits
          ' contain status information, however it is possible that
          ' some "noise" gets picked up on the bottom 5 bits which 
          ' represent the last PPU bus transaction. Some games "may"
          ' use this noise as valid data (even though they probably
          ' shouldn't)
          data = CByte((status.Reg And &HE0) Or (ppu_data_buffer And &H1F))

          ' Clear the vertical blanking flag
          status.vertical_blank = False

          ' Reset Loopy's Address latch flag
          address_latch = 0

        Case &H3 ' OAM Address

        Case &H4 ' OAM Data

        Case &H5 ' Scroll - Not Readable

        Case &H6 ' PPU Address - Not Readable

        Case &H7 ' PPU Data

          ' Reads from the NameTable ram get delayed one cycle, 
          ' so output buffer which contains the data from the 
          ' previous read request
          data = ppu_data_buffer
          ' then update the buffer for next time
          ppu_data_buffer = PpuRead(vram_addr.Reg)
          ' However, if the address was in the palette range, the
          ' data is not delayed, so it returns immediately
          If vram_addr.Reg >= &H3F00 Then data = ppu_data_buffer
          ' All reads from PPU data automatically increment the nametable
          ' address depending upon the mode set in the control register.
          ' If set to vertical mode, the increment is 32, so it skips
          ' one whole nametable row; in horizontal mode it just increments
          ' by 1, moving to the next column
          vram_addr.Reg += If(control.increment_mode, 32US, 1US)

      End Select

    End If

    Return data

  End Function

  Public Sub CpuWrite(addr As UShort, data As Byte)

    Select Case addr
      Case &H0 ' Control
        control.Reg = data
        tram_addr.nametable_x = If(control.nametable_x, 1US, 0US)
        tram_addr.nametable_y = If(control.nametable_y, 1US, 0US)
      Case &H1 ' Mask
        mask.Reg = data
      Case &H2 ' Status
      Case &H3 ' OAM Address
      Case &H4 ' OAM Data
      Case &H5 ' Scroll
        If address_latch = 0 Then
          ' First write to scroll register contains X offset in pixel space
          ' which we split into coarse And fine x values
          fine_x = CByte(data And &H7)
          tram_addr.coarse_x = data >> 3
          address_latch = 1
        Else
          ' First write to scroll register contains Y offset in pixel space
          ' which we split into coarse And fine Y values
          tram_addr.fine_y = CByte(data And &H7)
          tram_addr.coarse_y = data >> 3
          address_latch = 0
        End If
      Case &H6 ' PPU Address
        If address_latch = 0 Then
          ' PPU address bus can be accessed by CPU via the ADDR And DATA
          ' registers. The fisrt write to this register latches the high byte
          ' of the address, the second Is the low byte. Note the writes
          ' are stored in the tram register...
          tram_addr.Reg = CUShort(((data And &H3F) << 8) Or (tram_addr.Reg And &HFF))
          address_latch = 1
        Else
          ' ...when a whole address has been written, the internal vram address
          ' buffer Is updated. Writing to the PPU Is unwise during rendering
          ' as the PPU will maintam the vram address automatically whilst
          ' rendering the scanline position.
          tram_addr.Reg = CUShort((tram_addr.Reg And &HFF00) Or data)
          vram_addr = tram_addr
          address_latch = 0
        End If
      Case &H7 ' PPU Data
        PpuWrite(vram_addr.Reg, data)
        ' All writes from PPU data automatically increment the nametable
        ' address depending upon the mode set in the control register.
        ' If set to vertical mode, the increment Is 32, so it skips
        ' one whole nametable row; in horizontal mode it just increments
        ' by 1, moving to the next column
        vram_addr.Reg += If(control.increment_mode, 32US, 1US)
    End Select

  End Sub

  Public Function PpuRead(addr As UShort, Optional rdonly As Boolean = False) As Byte

    Dim data As Byte = &H0
    addr = addr And &H3FFFUS

    If cart.PpuRead(addr, data) Then

    ElseIf addr >= &H0 AndAlso addr <= &H1FFF Then

      ' If the cartridge cant map the address, have
      ' a physical location ready here
      data = tblPattern((addr And &H1000US) >> 12, addr And &HFFF)

    ElseIf addr >= &H2000 AndAlso addr <= &H3EFF Then

      addr = addr And &HFFFUS
      If cart.mirror = Cartridge.MIRRORS.VERTICAL Then
        ' Vertical
        If addr >= &H0 AndAlso addr <= &H3FF Then data = tblName(0, addr And &H3FF)
        If addr >= &H400 AndAlso addr <= &H7FF Then data = tblName(1, addr And &H3FF)
        If addr >= &H800 AndAlso addr <= &HBFF Then data = tblName(0, addr And &H3FF)
        If addr >= &HC00 AndAlso addr <= &HFFF Then data = tblName(1, addr And &H3FF)
      ElseIf cart.mirror = Cartridge.MIRRORS.HORIZONTAL Then
        ' Horizontal
        If addr >= &H0 AndAlso addr <= &H3FF Then data = tblName(0, addr And &H3FF)
        If addr >= &H400 AndAlso addr <= &H7FF Then data = tblName(0, addr And &H3FF)
        If addr >= &H800 AndAlso addr <= &HBFF Then data = tblName(1, addr And &H3FF)
        If addr >= &HC00 AndAlso addr <= &HFFF Then data = tblName(1, addr And &H3FF)
      End If

    ElseIf addr >= &H3F00 AndAlso addr <= &H3FFF Then

      addr = addr And &H1FUS
      If addr = &H10 Then addr = &H0
      If addr = &H14 Then addr = &H4
      If addr = &H18 Then addr = &H8
      If addr = &H1C Then addr = &HC
      data = CByte(tblPalette(addr) And (If(mask.Grayscale, &H30, &H3F)))

    End If

    Return data

  End Function

  Public Sub PpuWrite(addr As UShort, data As Byte)

    addr = addr And &H3FFFUS

    If cart.PpuWrite(addr, data) Then

    ElseIf addr >= &H0 AndAlso addr <= &H1FFF Then

      tblPattern((addr And &H1000US) >> 12, addr And &HFFF) = data

    ElseIf addr >= &H2000 AndAlso addr <= &H3EFF Then

      addr = addr And &HFFFUS
      If cart.mirror = Cartridge.MIRRORS.VERTICAL Then
        ' Vertical
        If addr >= &H0 AndAlso addr <= &H3FF Then tblName(0, addr And &H3FF) = data
        If addr >= &H400 AndAlso addr <= &H7FF Then tblName(1, addr And &H3FF) = data
        If addr >= &H800 AndAlso addr <= &HBFF Then tblName(0, addr And &H3FF) = data
        If addr >= &HC00 AndAlso addr <= &HFFF Then tblName(1, addr And &H3FF) = data
      ElseIf cart.mirror = Cartridge.MIRRORS.HORIZONTAL Then
        ' Horizontal
        If addr >= &H0 AndAlso addr <= &H3FF Then tblName(0, addr And &H3FF) = data
        If addr >= &H400 AndAlso addr <= &H7FF Then tblName(0, addr And &H3FF) = data
        If addr >= &H800 AndAlso addr <= &HBFF Then tblName(1, addr And &H3FF) = data
        If addr >= &HC00 AndAlso addr <= &HFFF Then tblName(1, addr And &H3FF) = data
      End If

    ElseIf addr >= &H3F00 AndAlso addr <= &H3FFF Then

      addr = addr And &H1FUS
      If addr = &H10 Then addr = &H0
      If addr = &H14 Then addr = &H4
      If addr = &H18 Then addr = &H8
      If addr = &H1C Then addr = &HC
      tblPalette(addr) = data

    End If

  End Sub

  Public Sub ConnectCartridge(cartridge As Cartridge)
    cart = cartridge
  End Sub

  Public Sub Reset()
    fine_x = &H0
    address_latch = &H0
    ppu_data_buffer = &H0
    scanline = 0
    cycle = 0
    bg_next_tile_id = &H0
    bg_next_tile_attrib = &H0
    bg_next_tile_lsb = &H0
    bg_next_tile_msb = &H0
    bg_shifter_pattern_lo = &H0
    bg_shifter_pattern_hi = &H0
    bg_shifter_attrib_lo = &H0
    bg_shifter_attrib_hi = &H0
    status.Reg = &H0
    mask.Reg = &H0
    control.Reg = &H0
    vram_addr.Reg = &H0
    tram_addr.Reg = &H0
  End Sub

  Public Sub Clock()

    ' As we progress through scanlines and cycles, the PPU is effectively
    ' a state machine going through the motions of fetching background 
    ' information and sprite information, compositing them into a pixel
    ' to be output.

    ' The lambda functions (functions inside functions) contain the various
    ' actions to be performed depending upon the output of the state machine
    ' for a given scanline/cycle combination

    ' ==============================================================================
    ' Increment the background tile "pointer" one tile/column horizontally
    Dim incrementScrollX = Sub()

                             ' Note: pixel perfect scrolling horizontally is handled by the 
                             ' data shifters. Here we are operating in the spatial domain of 
                             ' tiles, 8x8 pixel blocks.

                             ' Only if rendering is enabled
                             If mask.render_background OrElse mask.render_sprites Then
                               ' A single name table is 32x30 tiles. As we increment horizontally
                               ' we may cross into a neighbouring nametable, or wrap around to
                               ' a neighbouring nametable
                               If vram_addr.coarse_x = 31 Then
                                 ' Leaving nametable so wrap address round
                                 vram_addr.coarse_x = 0
                                 ' Flip target nametable bit
                                 vram_addr.nametable_x = Not vram_addr.nametable_x
                               Else
                                 ' Staying in current nametable, so just increment
                                 vram_addr.coarse_x += 1US
                               End If
                             End If

                           End Sub

    ' Increment the background tile "pointer" one scanline vertically
    Dim incrementScrollY = Sub()

                             ' Incrementing vertically is more complicated. The visible nametable
                             ' is 32x30 tiles, but in memory there is enough room for 32x32 tiles.
                             ' The bottom two rows of tiles are in fact not tiles at all, they
                             ' contain the "attribute" information for the entire table. This is
                             ' information that describes which palettes are used for different 
                             ' regions of the nametable.

                             ' In addition, the NES doesnt scroll vertically in chunks of 8 pixels
                             ' i.e. the height of a tile, it can perform fine scrolling by using
                             ' the fine_y component of the register. This means an increment in Y
                             ' first adjusts the fine offset, but may need to adjust the whole
                             ' row offset, since fine_y is a value 0 to 7, and a row is 8 pixels high

                             ' Only if rendering is enabled
                             If mask.render_background OrElse mask.render_sprites Then
                               ' If possible, just increment the fine y offset
                               If vram_addr.fine_y < 7 Then
                                 vram_addr.fine_y += 1US
                               Else
                                 ' If we have gone beyond the height of a row, we need to
                                 ' increment the row, potentially wrapping into neighbouring
                                 ' vertical nametables. Dont forget however, the bottom two rows
                                 ' do not contain tile information. The coarse y offset is used
                                 ' to identify which row of the nametable we want, and the fine
                                 ' y offset is the specific "scanline"

                                 ' Reset fine y offset
                                 vram_addr.fine_y = 0

                                 ' Check if we need to swap vertical nametable targets
                                 If vram_addr.coarse_y = 29 Then
                                   ' We do, so reset coarse y offset
                                   vram_addr.coarse_y = 0
                                   ' And flip the target nametable bit
                                   vram_addr.nametable_y = Not vram_addr.nametable_y
                                 ElseIf vram_addr.coarse_y = 31 Then
                                   ' In case the pointer is in the attribute memory, we
                                   ' just wrap around the current nametable
                                   vram_addr.coarse_y = 0
                                 Else
                                   ' None of the above boundary/wrapping conditions apply
                                   ' so just increment the coarse y offset
                                   vram_addr.coarse_y += 1US
                                 End If
                               End If
                             End If

                           End Sub

    ' ==============================================================================
    ' Transfer the temporarily stored horizontal nametable access information
    ' into the "pointer". Note that fine x scrolling is not part of the "pointer"
    ' addressing mechanism
    Dim transferAddressX = Sub()
                             ' Only if rendering is enabled
                             If mask.render_background OrElse mask.render_sprites Then
                               vram_addr.nametable_x = tram_addr.nametable_x
                               vram_addr.coarse_x = tram_addr.coarse_x
                             End If
                           End Sub

    ' ==============================================================================
    ' Transfer the temporarily stored vertical nametable access information
    ' into the "pointer". Note that fine y scrolling is part of the "pointer"
    ' addressing mechanism
    Dim transferAddressY = Sub()
                             ' Only if rendering is enabled
                             If mask.render_background OrElse mask.render_sprites Then
                               vram_addr.fine_y = tram_addr.fine_y
                               vram_addr.nametable_y = tram_addr.nametable_y
                               vram_addr.coarse_y = tram_addr.coarse_y
                             End If
                           End Sub

    ' ==============================================================================
    ' Prime the "in-effect" background tile shifters ready for outputting next
    ' 8 pixels in scanline.
    Dim loadBackgroundShifters = Sub()
                                   ' Each PPU update we calculate one pixel. These shifters shift 1 bit along
                                   ' feeding the pixel compositor with the binary information it needs. Its
                                   ' 16 bits wide, because the top 8 bits are the current 8 pixels being drawn
                                   ' and the bottom 8 bits are the next 8 pixels to be drawn. Naturally this means
                                   ' the required bit is always the MSB of the shifter. However, "fine x" scrolling
                                   ' plays a part in this too, whcih is seen later, so in fact we can choose
                                   ' any one of the top 8 bits.
                                   bg_shifter_pattern_lo = (bg_shifter_pattern_lo And &HFF00US) Or bg_next_tile_lsb
                                   bg_shifter_pattern_hi = (bg_shifter_pattern_hi And &HFF00US) Or bg_next_tile_msb
                                   ' Attribute bits do not change per pixel, rather they change every 8 pixels
                                   ' but are synchronised with the pattern shifters for convenience, so here
                                   ' we take the bottom 2 bits of the attribute word which represent which 
                                   ' palette is being used for the current 8 pixels and the next 8 pixels, and 
                                   ' "inflate" them to 8 bit words.
                                   bg_shifter_attrib_lo = (bg_shifter_attrib_lo And &HFF00US) Or If((bg_next_tile_attrib And &B1) = &B1, &HFFUS, &H0US)
                                   bg_shifter_attrib_hi = (bg_shifter_attrib_hi And &HFF00US) Or If((bg_next_tile_attrib And &B10) = &B10, &HFFUS, &H0US)
                                 End Sub

    ' ==============================================================================
    ' Every cycle the shifters storing pattern and attribute information shift
    ' their contents by 1 bit. This is because every cycle, the output progresses
    ' by 1 pixel. This means relatively, the state of the shifter is in sync
    ' with the pixels being drawn for that 8 pixel section of the scanline.
    Dim UpdateShifters = Sub()
                           If mask.render_background Then
                             ' Shifting background tile pattern row
                             bg_shifter_pattern_lo <<= 1
                             bg_shifter_pattern_hi <<= 1
                             ' Shifting palette attributes by 1
                             bg_shifter_attrib_lo <<= 1
                             bg_shifter_attrib_hi <<= 1
                           End If
                         End Sub

    ' All but 1 of the secanlines is visible to the user. The pre-render scanline
    ' at -1, is used to configure the "shifters" for the first visible scanline, 0.
    If scanline >= -1 AndAlso scanline < 240 Then

      If scanline = 0 AndAlso cycle = 0 Then
        ' "Odd Frame" cycle skip
        cycle = 1
      End If

      If scanline = -1 AndAlso cycle = 1 Then
        ' Effectively start of new frame, so clear vertical blank flag
        status.vertical_blank = False
      End If

      If (cycle >= 2 AndAlso cycle < 258) OrElse (cycle >= 321 AndAlso cycle < 338) Then
        UpdateShifters()

        ' In these cycles we are collecting and working with visible data
        ' The "shifters" have been preloaded by the end of the previous
        ' scanline with the data for the start of this scanline. Once we
        ' leave the visible region, we go dormant until the shifters are
        ' preloaded for the next scanline.

        ' Fortunately, for background rendering, we go through a fairly
        ' repeatable sequence of events, every 2 clock cycles.
        Select Case (cycle - 1) Mod 8
          Case 0
            ' Load the current background tile pattern and attributes into the "shifter"
            loadBackgroundShifters()

            ' Fetch the next background tile ID
            ' "(vram_addr.reg And &H0FFF)" : Mask to 12 bits that are relevant
            ' "| &H2000"                 : Offset into nametable space on PPU address bus
            bg_next_tile_id = PpuRead(&H2000US Or (vram_addr.Reg And &HFFFUS))

            ' Explanation:
            ' The bottom 12 bits of the loopy register provide an index into
            ' the 4 nametables, regardless of nametable mirroring configuration.
            ' nametable_y(1) nametable_x(1) coarse_y(5) coarse_x(5)
            '
            ' Consider a single nametable is a 32x32 array, and we have four of them
            '   0                1
            ' 0 +----------------+----------------+
            '   |                |                |
            '   |                |                |
            '   |    (32x32)     |    (32x32)     |
            '   |                |                |
            '   |                |                |
            ' 1 +----------------+----------------+
            '   |                |                |
            '   |                |                |
            '   |    (32x32)     |    (32x32)     |
            '   |                |                |
            '   |                |                |
            '   +----------------+----------------+
            '
            ' This means there are 4096 potential locations in this array, which 
            ' just so happens to be 2^12!
          Case 2
            ' Fetch the next background tile attribute. OK, so this one is a bit
            ' more involved :P

            ' Recall that each nametable has two rows of cells that are not tile
            ' information, instead they represent the attribute information that
            ' indicates which palettes are applied to which area on the screen.
            ' Importantly (and frustratingly) there is not a 1 to 1 correspondance
            ' between background tile and palette. Two rows of tile data holds
            ' 64 attributes. Therfore we can assume that the attributes affect
            ' 8x8 zones on the screen for that nametable. Given a working resolution
            ' of 256x240, we can further assume that each zone is 32x32 pixels
            ' in screen space, or 4x4 tiles. Four system palettes are allocated
            ' to background rendering, so a palette can be specified using just
            ' 2 bits. The attribute byte therefore can specify 4 distinct palettes.
            ' Therefore we can even further assume that a single palette is
            ' applied to a 2x2 tile combination of the 4x4 tile zone. The very fact
            ' that background tiles "share" a palette locally is the reason why
            ' in some games you see distortion in the colours at screen edges.

            ' As before when choosing the tile ID, we can use the bottom 12 bits of
            ' the loopy register, but we need to make the implementation "coarser"
            ' because instead of a specific tile, we want the attribute byte for a
            ' group of 4x4 tiles, or in other words, we divide our 32x32 address
            ' by 4 to give us an equivalent 8x8 address, and we offset this address
            ' into the attribute section of the target nametable.

            ' Reconstruct the 12 bit loopy address into an offset into the
            ' attribute memory

            ' "(vram_addr.coarse_x >> 2)" : integer divide coarse x by 4,
            ' from 5 bits to 3 bits
            ' "((vram_addr.coarse_y >> 2) << 3)" : integer divide coarse y by 4,
            ' from 5 bits to 3 bits,
            ' shift to make room for coarse x

            ' Result so far: YX00 00yy yxxx

            ' All attribute memory begins at 0x03C0 within a nametable, so OR with
            ' result to select target nametable, and attribute byte offset. Finally
            ' OR with 0x2000 to offset into nametable address space on PPU bus.
            bg_next_tile_attrib = PpuRead(&H23C0US Or (vram_addr.nametable_y << 11) Or (vram_addr.nametable_x << 10) Or ((vram_addr.coarse_y >> 2) << 3) Or (vram_addr.coarse_x >> 2))

            ' Right we've read the correct attribute byte for a specified address,
            ' but the byte itself is broken down further into the 2x2 tile groups
            ' in the 4x4 attribute zone.

            ' The attribute byte is assembled thus: BR(76) BL(54) TR(32) TL(10)
            '
            ' +----+----+ +----+----+
            ' | TL | TR | | ID | ID |
            ' +----+----+ where TL = +----+----+
            ' | BL | BR | | ID | ID |
            ' +----+----+ +----+----+
            '
            ' Since we know we can access a tile directly from the 12 bit address, we
            ' can analyse the bottom bits of the coarse coordinates to provide us with
            ' the correct offset into the 8-bit word, to yield the 2 bits we are
            ' actually interested in which specifies the palette for the 2x2 group of
            ' tiles. We know if "coarse y % 4" < 2 we are in the top half else bottom half.
            ' Likewise if "coarse x % 4" < 2 we are in the left half else right half.
            ' Ultimately we want the bottom two bits of our attribute word to be the
            ' palette selected. So shift as required...
            If (vram_addr.coarse_y And &H2) <> 0 Then
              bg_next_tile_attrib >>= 4
            End If
            If (vram_addr.coarse_x And &H2) <> 0 Then
              bg_next_tile_attrib >>= 2
            End If
            bg_next_tile_attrib = bg_next_tile_attrib And CByte(&H3)

            ' Compared to the last two, the next two are the easy ones... :P

          Case 4

            ' Fetch the next background tile LSB bit plane from the pattern memory
            ' The Tile ID has been read from the nametable. We will use this id to 
            ' index into the pattern memory to find the correct sprite (assuming
            ' the sprites lie on 8x8 pixel boundaries in that memory, which they do
            ' even though 8x16 sprites exist, as background tiles are always 8x8).
            '
            ' Since the sprites are effectively 1 bit deep, but 8 pixels wide, we 
            ' can represent a whole sprite row as a single byte, so offsetting
            ' into the pattern memory is easy. In total there is 8KB so we need a 
            ' 13 bit address.

            ' "(control.pattern_background << 12)"  : the pattern memory selector 
            '                                         from control register, either 0K
            '                                         or 4K offset
            ' "((uint16_t)bg_next_tile_id << 4)"    : the tile id multiplied by 16, as
            '                                         2 lots of 8 rows of 8 bit pixels
            ' "(vram_addr.fine_y)"                  : Offset into which row based on
            '                                         vertical scroll offset
            ' "+ 0"                                 : Mental clarity for plane offset
            ' Note: No PPU address bus offset required as it starts at 0x0000
            bg_next_tile_lsb = PpuRead(CUShort((If(control.pattern_background, 1, 0) << 12) + (CUShort(bg_next_tile_id) << 4) + (vram_addr.fine_y) + 0))

          Case 6

            ' Fetch the next background tile MSB bit plane from the pattern memory
            ' This is the same as above, but has a +8 offset to select the next bit plane
            bg_next_tile_msb = PpuRead((If(control.pattern_background, 1US, 0US) << 12) + ((CUShort(bg_next_tile_id) << 4)) + (vram_addr.fine_y) + 8US)

          Case 7

            ' Increment the background tile "pointer" to the next tile horizontally
            ' in the nametable memory. Note this may cross nametable boundaries which
            ' is a little complex, but essential to implement scrolling
            incrementScrollX()

        End Select

      End If

      ' End of a visible scanline, so increment downwards...
      If cycle = 256 Then
        incrementScrollY()
      End If

      ' ...and reset the x position
      If cycle = 257 Then
        loadBackgroundShifters()
        transferAddressX()
      End If

      ' Superfluous reads of tile id at end of scanline
      If cycle = 338 Or cycle = 340 Then
        bg_next_tile_id = PpuRead(&H2000US Or (vram_addr.Reg And &HFFFUS))
      End If

      If scanline = -1 AndAlso cycle >= 280 AndAlso cycle < 305 Then
        ' End of vertical blank period so reset the Y address ready for rendering
        transferAddressY()
      End If

    End If

    If scanline = 240 Then
      ' Post Render Scanline - Do Nothing!
    End If

    If scanline >= 241 AndAlso scanline < 261 Then
      If scanline = 241 AndAlso cycle = 1 Then
        ' Effectively end of frame, so set vertical blank flag
        status.vertical_blank = True

        ' If the control register tells us to emit a NMI when
        ' entering vertical blanking period, do it! The CPU
        ' will be informed that rendering is complete so it can
        ' perform operations with the PPU knowing it wont
        ' produce visible artefacts
        If control.enable_nmi Then
          nmi = True
        End If
      End If
    End If

    ' Composition - We now have background pixel information for this cycle
    ' At this point we are only interested in background

    Dim bg_pixel As Byte = &H0 ' The 2-bit pixel to be rendered
    Dim bg_palette As Byte = &H0 ' The 3-bit index of the palette the pixel indexes

    ' We only render backgrounds if the PPU is enabled to do so. Note if
    ' background rendering is disabled, the pixel and palette combine
    ' to form 0x00. This will fall through the colour tables to yield
    ' the current background colour in effect
    If mask.render_background Then
      ' Handle Pixel Selection by selecting the relevant bit
      ' depending upon fine x scolling. This has the effect of
      ' offsetting ALL background rendering by a set number
      ' of pixels, permitting smooth scrolling
      Dim bit_mux As UInt16 = &H8000US >> fine_x
      ' Select Plane pixels by extracting from the shifter 
      ' at the required location. 
      Dim p0_pixel As Byte = CByte(If((bg_shifter_pattern_lo And bit_mux) > 0, 1, 0))
      Dim p1_pixel As Byte = CByte(If((bg_shifter_pattern_hi And bit_mux) > 0, 1, 0))

      ' Combine to form pixel index
      bg_pixel = (p1_pixel << 1) Or p0_pixel

      ' Get palette
      Dim bg_pal0 As Byte = CByte(If((bg_shifter_attrib_lo And bit_mux) > 0, 1, 0))
      Dim bg_pal1 As Byte = CByte(If((bg_shifter_attrib_hi And bit_mux) > 0, 1, 0))
      bg_palette = (bg_pal1 << 1) Or bg_pal0
    End If

    ' Now we have a final pixel colour, and a palette for this cycle
    ' of the current scanline. Let's at long last, draw that ^&%*er :P
    sprScreen.SetPixel(cycle - 1, scanline, GetColourFromPaletteRam(bg_palette, bg_pixel))

    ' Fake some noise for now
    'sprScreen.SetPixel(cycle - 1, scanline, palScreen(If(Rnd() >= 0.5, &H3F, &H30)))

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

    '' Fake some noise for now
    'sprScreen.SetPixel(cycle - 1, scanline, palScreen(If((Rnd() >= 0.5), &H3F, &H30)))

    '' Advance renderer - it never stops, it's relentless
    'cycle += 1S
    'If cycle >= 341 Then
    '  cycle = 0
    '  scanline += 1S
    '  If scanline >= 261 Then
    '    scanline = -1
    '    frame_complete = True
    '  End If
    'End If

  End Sub

End Class