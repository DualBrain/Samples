' Inspired by "NES Emulator Part #2: The CPU (6502 Implementation)" -- @javidx9
' https://youtu.be/8XmxKPJDGU0

Option Explicit On
Option Strict On
Option Infer On

Imports System.Text

Friend Module Program

  Sub Main()
    Dim demo As New Demo6502
    If demo.Construct(680, 480, 2, 2) Then
      demo.Start()
    End If
  End Sub

End Module

Friend Class Demo6502
  Inherits Olc.PixelGameEngine

  Public nes As New Bus
  Public mapAsm As New Dictionary(Of UShort, String)

  Friend Sub New()
    AppName = "6502 Demonstration"
  End Sub

  Protected Overrides Function OnUserCreate() As Boolean

    ' Load Program (assembled at https://www.masswerk.at/6502/assembler.html)
    '
    '     *=$8000
    '     LDX #10
    '     STX $0000
    '     LDX #3
    '     STX $0001
    '     LDY $0000
    '     LDA #0
    '     CLC
    '     loop
    '     ADC $0001
    '     DEY
    '     BNE loop
    '     STA $0002
    '     NOP
    '     NOP
    '     NOP

    ' Convert hex string into bytes for RAM
    Dim hexString = "A2 0A 8E 00 00 A2 03 8E 01 00 AC 00 00 A9 00 18 6D 01 00 88 D0 FA 8D 02 00 EA EA EA"
    Dim nOffset = &H8000US
    Dim hexValues = hexString.Split(" "c)
    For Each hexValue In hexValues
      nes.ram(nOffset) = CByte(Convert.ToUInt32(hexValue, 16)) : nOffset += 1US
    Next

    ' Set Reset Vector
    nes.ram(&HFFFC) = &H0
    nes.ram(&HFFFD) = &H80

    ' Dont forget to set IRQ and NMI vectors if you want to play with those

    ' Extract dissassembly
    mapAsm = nes.cpu.Disassemble(&H0, &HFFFF)

    ' Reset
    nes.cpu.Reset()

    Return True

  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Clear(Olc.Presets.DarkBlue)

    If GetKey(Key.SPACE).Pressed Then
      Do
        nes.cpu.clock()
      Loop While Not nes.cpu.complete()
    End If

    If GetKey(Key.R).Pressed Then nes.cpu.reset()
    If GetKey(Key.I).Pressed Then nes.cpu.irq()
    If GetKey(Key.N).Pressed Then nes.cpu.nmi()

    ' Draw Ram Page 0x00		
    DrawRam(2, 2, &H0, 16, 16)
    DrawRam(2, 182, &H8000, 16, 16)
    DrawCpu(448, 2)
    DrawCode(448, 72, 26)

    DrawString(10, 370, "SPACE = Step Instruction    R = RESET    I = IRQ    N = NMI")

    Return True

  End Function

  Public Shared Function Hex(n As UInteger, d As Byte) As String
    Return Microsoft.VisualBasic.Hex(n).PadLeft(d, "0"c)
  End Function

  Public Sub DrawRam(x As Integer, y As Integer, nAddr As UShort, nRows As Integer, nColumns As Integer)
    Dim nRamX = x, nRamY As Integer = y
    For row = 0 To nRows - 1
      Dim sOffset = "$" & Hex(nAddr, 4) & ":"
      For col = 0 To nColumns - 1
        sOffset &= " " & Hex(nes.read(nAddr, True), 2)
        nAddr += 1US
      Next
      DrawString(nRamX, nRamY, sOffset)
      nRamY += 10
    Next
  End Sub

  Private Sub DrawCpu(x As Integer, y As Integer)
    'Dim status = "STATUS: "
    DrawString(x, y, "STATUS:", Olc.Presets.White)
    DrawString(x + 64, y, "N", If((nes.cpu.status And olc6502.FLAGS6502.N) <> 0, Olc.Presets.Green, Olc.Presets.Red))
    DrawString(x + 80, y, "V", If((nes.cpu.status And olc6502.FLAGS6502.V) <> 0, Olc.Presets.Green, Olc.Presets.Red))
    DrawString(x + 96, y, "-", If((nes.cpu.status And olc6502.FLAGS6502.U) <> 0, Olc.Presets.Green, Olc.Presets.Red))
    DrawString(x + 112, y, "B", If((nes.cpu.status And olc6502.FLAGS6502.B) <> 0, Olc.Presets.Green, Olc.Presets.Red))
    DrawString(x + 128, y, "D", If((nes.cpu.status And olc6502.FLAGS6502.D) <> 0, Olc.Presets.Green, Olc.Presets.Red))
    DrawString(x + 144, y, "I", If((nes.cpu.status And olc6502.FLAGS6502.I) <> 0, Olc.Presets.Green, Olc.Presets.Red))
    DrawString(x + 160, y, "Z", If((nes.cpu.status And olc6502.FLAGS6502.Z) <> 0, Olc.Presets.Green, Olc.Presets.Red))
    DrawString(x + 178, y, "C", If((nes.cpu.status And olc6502.FLAGS6502.C) <> 0, Olc.Presets.Green, Olc.Presets.Red))
    DrawString(x, y + 10, "PC: $" & Hex(nes.cpu.pc, 4))
    DrawString(x, y + 20, "A: $" & Hex(nes.cpu.a, 2) & "  [" & nes.cpu.a.ToString() & "]")
    DrawString(x, y + 30, "X: $" & Hex(nes.cpu.x, 2) & "  [" & nes.cpu.x.ToString() & "]")
    DrawString(x, y + 40, "Y: $" & Hex(nes.cpu.y, 2) & "  [" & nes.cpu.y.ToString() & "]")
    DrawString(x, y + 50, "Stack P: $" & hex(nes.cpu.stkp, 4))
  End Sub

  Private Sub DrawCode(x As Integer, y As Integer, nLines As Integer)

    Dim value As String = Nothing
    If mapAsm.TryGetValue(nes.cpu.pc, value) Then
      ' Draw the middle code (current)
      Dim nLineY = (nLines \ 2) * 10 + y
      DrawString(x, nLineY, value, Olc.Presets.Cyan) : nLineY += 10
      ' Draw up to 10 lines after to the current
      Dim nextItems = mapAsm.SkipWhile(Function(kvp) kvp.Key <= nes.cpu.pc).Take(10)
      For Each kvp In nextItems
        DrawString(x, nLineY, kvp.Value) : nLineY += 10
      Next
      ' Draw up to 10 lines prior to the current
      nLineY = (nLines \ 2) * 10 + y : nLineY -= 10
      Dim prevItems = mapAsm.TakeWhile(Function(kvp) kvp.Key < nes.cpu.pc).Reverse.Take(10)
      For Each kvp In prevItems
        DrawString(x, nLineY, kvp.Value) : nLineY -= 10
      Next
    End If

  End Sub

End Class