Option Explicit On
Option Strict On
Option Infer On

Public Class Bus ' Devices on Main Bus

  ' The 6502 derived processor
  Public cpu As New olc6502
  ' The 2C02 Picture Processing Unit
  Public ppu As New olc2C02
  ' The Cartridge or "GamePak"
  Public cart As Cartridge
  ' 2KB of RAM
  Public cpuRam(2047) As Byte

  ' A count of how many clocks have passed
  Private nSystemClockCounter As UInteger = 0

  Public Sub New()
    cpu.ConnectBus(Me)
  End Sub

  ' Main Bus Read & Write

  Public Sub CpuWrite(addr As UShort, data As Byte)
    If cart.CpuWrite(addr, data) Then
      ' The cartridge "sees all" And has the facility to veto
      ' the propagation of the bus transaction if it requires.
      ' This allows the cartridge to map any address to some
      ' other data, including the facility to divert transactions
      ' with other physical devices. The NES does not do this
      ' but I figured it might be quite a flexible way of adding
      ' "custom" hardware to the NES in the future!
    ElseIf addr >= &H0 AndAlso addr <= &H1FFF Then
      ' System RAM Address Range. The range covers 8KB, though
      ' there is only 2KB available. That 2KB is "mirrored"
      ' through this address range. Using bitwise and to mask
      ' the bottom 11 bits Is the same as addr % 2048.
      cpuRam(addr And &H7FF) = data
    ElseIf addr >= &H2000 AndAlso addr <= &H3FFF Then
      ' PPU Address range. The PPU only has 8 primary registers
      ' and these are repeated throughout this range. We can
      ' use bitwise And operation to mask the bottom 3 bits, 
      ' which Is the equivalent of addr % 8.
      ppu.CpuWrite(addr And &H7US, data)
    End If
  End Sub

  Public Function CpuRead(addr As UShort, Optional bReadOnly As Boolean = False) As Byte
    Dim data As Byte = &H0
    If cart.CpuRead(addr, data) Then
      ' Cartridge Address Range
    ElseIf addr >= &H0 AndAlso addr <= &H1FFF Then
      ' System RAM Address Range, mirrored very 2048
      data = CpuRam(addr And &H7FF)
    ElseIf addr >= &H2000 AndAlso addr <= &H3FFF Then
      ' PPU Address range, mirrored every 8
      data = ppu.CpuRead(addr And &H7US, bReadOnly)
    End If
    Return data
  End Function

  ' System Interface

  ' Connects a cartridge object to the internal buses
  Public Sub InsertCartridge(cartridge As Cartridge)
    ' Connects cartridge to both Main Bus and CPU Bus
    cart = cartridge
    ppu.ConnectCartridge(cartridge)
  End Sub

  ' Resets the system
  Public Sub Reset()
    cpu.Reset()
    nSystemClockCounter = 0
  End Sub

  ' Clocks the system - a single whole system tick
  Public Sub Clock()

    ' Clocking. The heart And soul of an emulator. The running
    ' frequency is controlled by whatever calls this function.
    ' So here we "divide" the clock as necessary And call
    ' the peripheral devices clock() function at the correct
    ' times.

    ' The fastest clock frequency the digital system cares
    ' about is equivalent to the PPU clock. So the PPU is clocked
    ' each time this function is called.
    ppu.Clock()

    ' The CPU runs 3 times slower than the PPU so we only call its
    ' clock() function every 3 times this function is called. We
    ' have a global counter to keep track of this.
    If nSystemClockCounter Mod 3 = 0 Then
      cpu.Clock()
    End If

    nSystemClockCounter += 1UI

  End Sub

End Class