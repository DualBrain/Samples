Option Explicit On
Option Strict On
Option Infer On

' The 6502 Emulation Class. This is it!
Public Class olc6502

  ' CPU Core registers, exposed as public here for ease of access from external
  ' examinors. This is all the 6502 has.
  Public a As Byte = &H0 ' Accumulator Register
  Public x As Byte = &H0 ' X Register
  Public y As Byte = &H0 ' Y Register
  Public stkp As Byte = &H0 ' Stack Pointer (points to location on bus)
  Public pc As UShort = &H0 ' Program Counter
  Public status As Byte = &H0 ' Status Register

  Private Const B0 As Byte = 0
  Private Const B1 As Byte = 1

  Sub New()

    ' Assembles the translation table. It's big, it's ugly, but it yields a convenient way
    ' to emulate the 6502. I'm certain there are some "code-golf" strategies to reduce this
    ' but I've deliberately kept it verbose for study and alteration

    ' It is 16x16 entries. This gives 256 instructions. It is arranged to that the bottom
    ' 4 bits of the instruction choose the column, and the top 4 bits choose the row.

    ' For convenience to get function pointers to members of this class, I'm using this
    ' or else it will be much much larger :D

    ' The table is one big initialiser list of initialiser lists...
    lookup = {New INSTRUCTION("BRK", AddressOf BRK, AddressOf IMM, 7), New INSTRUCTION("ORA", AddressOf ORA, AddressOf IZX, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 8), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 3), New INSTRUCTION("ORA", AddressOf ORA, AddressOf ZP0, 3), New INSTRUCTION("ASL", AddressOf ASL, AddressOf ZP0, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 5), New INSTRUCTION("PHP", AddressOf PHP, AddressOf IMP, 3), New INSTRUCTION("ORA", AddressOf ORA, AddressOf IMM, 2), New INSTRUCTION("ASL", AddressOf ASL, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 4), New INSTRUCTION("ORA", AddressOf ORA, AddressOf ABS, 4), New INSTRUCTION("ASL", AddressOf ASL, AddressOf ABS, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6),
              New INSTRUCTION("BPL", AddressOf BPL, AddressOf REL, 2), New INSTRUCTION("ORA", AddressOf ORA, AddressOf IZY, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 8), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 4), New INSTRUCTION("ORA", AddressOf ORA, AddressOf ZPX, 4), New INSTRUCTION("ASL", AddressOf ASL, AddressOf ZPX, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6), New INSTRUCTION("CLC", AddressOf CLC, AddressOf IMP, 2), New INSTRUCTION("ORA", AddressOf ORA, AddressOf ABY, 4), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 7), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 4), New INSTRUCTION("ORA", AddressOf ORA, AddressOf ABX, 4), New INSTRUCTION("ASL", AddressOf ASL, AddressOf ABX, 7), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 7),
              New INSTRUCTION("JSR", AddressOf JSR, AddressOf ABS, 6), New INSTRUCTION("AND", AddressOf [AND], AddressOf IZX, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 8), New INSTRUCTION("BIT", AddressOf BIT, AddressOf ZP0, 3), New INSTRUCTION("AND", AddressOf [AND], AddressOf ZP0, 3), New INSTRUCTION("ROL", AddressOf ROL, AddressOf ZP0, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 5), New INSTRUCTION("PLP", AddressOf PLP, AddressOf IMP, 4), New INSTRUCTION("AND", AddressOf [AND], AddressOf IMM, 2), New INSTRUCTION("ROL", AddressOf ROL, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("BIT", AddressOf BIT, AddressOf ABS, 4), New INSTRUCTION("AND", AddressOf [AND], AddressOf ABS, 4), New INSTRUCTION("ROL", AddressOf ROL, AddressOf ABS, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6),
              New INSTRUCTION("BMI", AddressOf BMI, AddressOf REL, 2), New INSTRUCTION("AND", AddressOf [AND], AddressOf IZY, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 8), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 4), New INSTRUCTION("AND", AddressOf [AND], AddressOf ZPX, 4), New INSTRUCTION("ROL", AddressOf ROL, AddressOf ZPX, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6), New INSTRUCTION("SEC", AddressOf SEC, AddressOf IMP, 2), New INSTRUCTION("AND", AddressOf [AND], AddressOf ABY, 4), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 7), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 4), New INSTRUCTION("AND", AddressOf [AND], AddressOf ABX, 4), New INSTRUCTION("ROL", AddressOf ROL, AddressOf ABX, 7), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 7),
              New INSTRUCTION("RTI", AddressOf RTI, AddressOf IMP, 6), New INSTRUCTION("EOR", AddressOf EOR, AddressOf IZX, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 8), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 3), New INSTRUCTION("EOR", AddressOf EOR, AddressOf ZP0, 3), New INSTRUCTION("LSR", AddressOf LSR, AddressOf ZP0, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 5), New INSTRUCTION("PHA", AddressOf PHA, AddressOf IMP, 3), New INSTRUCTION("EOR", AddressOf EOR, AddressOf IMM, 2), New INSTRUCTION("LSR", AddressOf LSR, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("JMP", AddressOf JMP, AddressOf ABS, 3), New INSTRUCTION("EOR", AddressOf EOR, AddressOf ABS, 4), New INSTRUCTION("LSR", AddressOf LSR, AddressOf ABS, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6),
              New INSTRUCTION("BVC", AddressOf BVC, AddressOf REL, 2), New INSTRUCTION("EOR", AddressOf EOR, AddressOf IZY, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 8), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 4), New INSTRUCTION("EOR", AddressOf EOR, AddressOf ZPX, 4), New INSTRUCTION("LSR", AddressOf LSR, AddressOf ZPX, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6), New INSTRUCTION("CLI", AddressOf CLI, AddressOf IMP, 2), New INSTRUCTION("EOR", AddressOf EOR, AddressOf ABY, 4), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 7), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 4), New INSTRUCTION("EOR", AddressOf EOR, AddressOf ABX, 4), New INSTRUCTION("LSR", AddressOf LSR, AddressOf ABX, 7), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 7),
              New INSTRUCTION("RTS", AddressOf RTS, AddressOf IMP, 6), New INSTRUCTION("ADC", AddressOf ADC, AddressOf IZX, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 8), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 3), New INSTRUCTION("ADC", AddressOf ADC, AddressOf ZP0, 3), New INSTRUCTION("ROR", AddressOf ROR, AddressOf ZP0, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 5), New INSTRUCTION("PLA", AddressOf PLA, AddressOf IMP, 4), New INSTRUCTION("ADC", AddressOf ADC, AddressOf IMM, 2), New INSTRUCTION("ROR", AddressOf ROR, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("JMP", AddressOf JMP, AddressOf IND, 5), New INSTRUCTION("ADC", AddressOf ADC, AddressOf ABS, 4), New INSTRUCTION("ROR", AddressOf ROR, AddressOf ABS, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6),
              New INSTRUCTION("BVS", AddressOf BVS, AddressOf REL, 2), New INSTRUCTION("ADC", AddressOf ADC, AddressOf IZY, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 8), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 4), New INSTRUCTION("ADC", AddressOf ADC, AddressOf ZPX, 4), New INSTRUCTION("ROR", AddressOf ROR, AddressOf ZPX, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6), New INSTRUCTION("SEI", AddressOf SEI, AddressOf IMP, 2), New INSTRUCTION("ADC", AddressOf ADC, AddressOf ABY, 4), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 7), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 4), New INSTRUCTION("ADC", AddressOf ADC, AddressOf ABX, 4), New INSTRUCTION("ROR", AddressOf ROR, AddressOf ABX, 7), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 7),
              New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 2), New INSTRUCTION("STA", AddressOf STA, AddressOf IZX, 6), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6), New INSTRUCTION("STY", AddressOf STY, AddressOf ZP0, 3), New INSTRUCTION("STA", AddressOf STA, AddressOf ZP0, 3), New INSTRUCTION("STX", AddressOf STX, AddressOf ZP0, 3), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 3), New INSTRUCTION("DEY", AddressOf DEY, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 2), New INSTRUCTION("TXA", AddressOf TXA, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("STY", AddressOf STY, AddressOf ABS, 4), New INSTRUCTION("STA", AddressOf STA, AddressOf ABS, 4), New INSTRUCTION("STX", AddressOf STX, AddressOf ABS, 4), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 4),
              New INSTRUCTION("BCC", AddressOf BCC, AddressOf REL, 2), New INSTRUCTION("STA", AddressOf STA, AddressOf IZY, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6), New INSTRUCTION("STY", AddressOf STY, AddressOf ZPX, 4), New INSTRUCTION("STA", AddressOf STA, AddressOf ZPX, 4), New INSTRUCTION("STX", AddressOf STX, AddressOf ZPY, 4), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 4), New INSTRUCTION("TYA", AddressOf TYA, AddressOf IMP, 2), New INSTRUCTION("STA", AddressOf STA, AddressOf ABY, 5), New INSTRUCTION("TXS", AddressOf TXS, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 5), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 5), New INSTRUCTION("STA", AddressOf STA, AddressOf ABX, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 5),
              New INSTRUCTION("LDY", AddressOf LDY, AddressOf IMM, 2), New INSTRUCTION("LDA", AddressOf LDA, AddressOf IZX, 6), New INSTRUCTION("LDX", AddressOf LDX, AddressOf IMM, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6), New INSTRUCTION("LDY", AddressOf LDY, AddressOf ZP0, 3), New INSTRUCTION("LDA", AddressOf LDA, AddressOf ZP0, 3), New INSTRUCTION("LDX", AddressOf LDX, AddressOf ZP0, 3), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 3), New INSTRUCTION("TAY", AddressOf TAY, AddressOf IMP, 2), New INSTRUCTION("LDA", AddressOf LDA, AddressOf IMM, 2), New INSTRUCTION("TAX", AddressOf TAX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("LDY", AddressOf LDY, AddressOf ABS, 4), New INSTRUCTION("LDA", AddressOf LDA, AddressOf ABS, 4), New INSTRUCTION("LDX", AddressOf LDX, AddressOf ABS, 4), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 4),
              New INSTRUCTION("BCS", AddressOf BCS, AddressOf REL, 2), New INSTRUCTION("LDA", AddressOf LDA, AddressOf IZY, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 5), New INSTRUCTION("LDY", AddressOf LDY, AddressOf ZPX, 4), New INSTRUCTION("LDA", AddressOf LDA, AddressOf ZPX, 4), New INSTRUCTION("LDX", AddressOf LDX, AddressOf ZPY, 4), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 4), New INSTRUCTION("CLV", AddressOf CLV, AddressOf IMP, 2), New INSTRUCTION("LDA", AddressOf LDA, AddressOf ABY, 4), New INSTRUCTION("TSX", AddressOf TSX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 4), New INSTRUCTION("LDY", AddressOf LDY, AddressOf ABX, 4), New INSTRUCTION("LDA", AddressOf LDA, AddressOf ABX, 4), New INSTRUCTION("LDX", AddressOf LDX, AddressOf ABY, 4), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 4),
              New INSTRUCTION("CPY", AddressOf CPY, AddressOf IMM, 2), New INSTRUCTION("CMP", AddressOf CMP, AddressOf IZX, 6), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 8), New INSTRUCTION("CPY", AddressOf CPY, AddressOf ZP0, 3), New INSTRUCTION("CMP", AddressOf CMP, AddressOf ZP0, 3), New INSTRUCTION("DEC", AddressOf DEC, AddressOf ZP0, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 5), New INSTRUCTION("INY", AddressOf INY, AddressOf IMP, 2), New INSTRUCTION("CMP", AddressOf CMP, AddressOf IMM, 2), New INSTRUCTION("DEX", AddressOf DEX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("CPY", AddressOf CPY, AddressOf ABS, 4), New INSTRUCTION("CMP", AddressOf CMP, AddressOf ABS, 4), New INSTRUCTION("DEC", AddressOf DEC, AddressOf ABS, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6),
              New INSTRUCTION("BNE", AddressOf BNE, AddressOf REL, 2), New INSTRUCTION("CMP", AddressOf CMP, AddressOf IZY, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 8), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 4), New INSTRUCTION("CMP", AddressOf CMP, AddressOf ZPX, 4), New INSTRUCTION("DEC", AddressOf DEC, AddressOf ZPX, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6), New INSTRUCTION("CLD", AddressOf CLD, AddressOf IMP, 2), New INSTRUCTION("CMP", AddressOf CMP, AddressOf ABY, 4), New INSTRUCTION("NOP", AddressOf NOP, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 7), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 4), New INSTRUCTION("CMP", AddressOf CMP, AddressOf ABX, 4), New INSTRUCTION("DEC", AddressOf DEC, AddressOf ABX, 7), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 7),
              New INSTRUCTION("CPX", AddressOf CPX, AddressOf IMM, 2), New INSTRUCTION("SBC", AddressOf SBC, AddressOf IZX, 6), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 8), New INSTRUCTION("CPX", AddressOf CPX, AddressOf ZP0, 3), New INSTRUCTION("SBC", AddressOf SBC, AddressOf ZP0, 3), New INSTRUCTION("INC", AddressOf INC, AddressOf ZP0, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 5), New INSTRUCTION("INX", AddressOf INX, AddressOf IMP, 2), New INSTRUCTION("SBC", AddressOf SBC, AddressOf IMM, 2), New INSTRUCTION("NOP", AddressOf NOP, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf SBC, AddressOf IMP, 2), New INSTRUCTION("CPX", AddressOf CPX, AddressOf ABS, 4), New INSTRUCTION("SBC", AddressOf SBC, AddressOf ABS, 4), New INSTRUCTION("INC", AddressOf INC, AddressOf ABS, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6),
              New INSTRUCTION("BEQ", AddressOf BEQ, AddressOf REL, 2), New INSTRUCTION("SBC", AddressOf SBC, AddressOf IZY, 5), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 8), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 4), New INSTRUCTION("SBC", AddressOf SBC, AddressOf ZPX, 4), New INSTRUCTION("INC", AddressOf INC, AddressOf ZPX, 6), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 6), New INSTRUCTION("SED", AddressOf SED, AddressOf IMP, 2), New INSTRUCTION("SBC", AddressOf SBC, AddressOf ABY, 4), New INSTRUCTION("NOP", AddressOf NOP, AddressOf IMP, 2), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 7), New INSTRUCTION("???", AddressOf NOP, AddressOf IMP, 4), New INSTRUCTION("SBC", AddressOf SBC, AddressOf ABX, 4), New INSTRUCTION("INC", AddressOf INC, AddressOf ABX, 7), New INSTRUCTION("???", AddressOf XXX, AddressOf IMP, 7)}.ToList

  End Sub

  ' External event functions. In hardware these represent pins that are asserted
  ' to produce a change in state.

  '///////////////////////////////////////////////////////////////////////////////
  ' EXTERNAL INPUTS

  ' Forces the 6502 into a known state. This is hard-wired inside the CPU. The
  ' registers are set to 0x00, the status register is cleared except for unused
  ' bit which remains at 1. An absolute address is read from location 0xFFFC
  ' which contains a second address that the program counter is set to. This
  ' allows the programmer to jump to a known and programmable location in the
  ' memory to start executing from. Typically the programmer would set the value
  ' at location 0xFFFC at compile time.
  Public Sub Reset() ' Reset Interrupt - Forces CPU into known state

    ' Get address to set program counter to
    addr_abs = &HFFFCUI
    Dim lo = Read(addr_abs + 0US)
    Dim hi = Read(addr_abs + 1US)

    ' Set it
    pc = (CUShort(hi) << 8) Or lo

    ' Reset internal registers
    a = 0
    x = 0
    y = 0
    stkp = &HFDUI
    status = &H0 Or FLAGS6502.U

    ' Clear internal helper variables
    addr_rel = &H0
    addr_abs = &H0
    fetched = &H0

    ' Reset takes time
    cycles = 8

  End Sub

  ' Interrupt requests are a complex operation and only happen if the
  ' "disable interrupt" flag is 0. IRQs can happen at any time, but
  ' you dont want them to be destructive to the operation of the running 
  ' program. Therefore the current instruction is allowed to finish
  ' (which I facilitate by doing the whole thing when cycles == 0) and 
  ' then the current program counter is stored on the stack. Then the
  ' current status register is stored on the stack. When the routine
  ' that services the interrupt has finished, the status register
  ' and program counter can be restored to how they where before it 
  ' occurred. This is impemented by the "RTI" instruction. Once the IRQ
  ' has happened, in a similar way to a reset, a programmable address
  ' is read form hard coded location 0xFFFE, which is subsequently
  ' set to the program counter.
  Public Sub Irq() ' Interrupt Request - Executes an instruction at a specific location

    ' If interrupts are allowed
    If Not GetFlag(FLAGS6502.I) <> 0 Then

      ' Push the program counter to the stack. It's 16-bits dont
      ' forget so that takes two pushes
      Write(&H100US + stkp, CByte(Fix((pc >> 8) And &HFF)))
      stkp -= B1
      Write(&H100US + stkp, CByte(Fix(pc And &HFF)))
      stkp -= B1

      ' Then Push the status register to the stack
      SetFlag(FLAGS6502.B, False)
      SetFlag(FLAGS6502.U, True)
      SetFlag(FLAGS6502.I, True)
      Write(&H100US + stkp, status)
      stkp -= B1

      ' Read new program counter location from fixed address
      addr_abs = &HFFFE
      Dim lo = Read(addr_abs + 0US)
      Dim hi = Read(addr_abs + 1US)
      pc = (hi << 8) Or lo

      ' IRQs take time
      cycles = 7

    End If

  End Sub

  ' A Non-Maskable Interrupt cannot be ignored. It behaves in exactly the
  ' same way as a regular IRQ, but reads the new program counter address
  ' from location 0xFFFA.
  Public Sub Nmi() ' Non-Maskable Interrupt Request - As above, but cannot be disabled

    Write(&H100US + stkp, CByte(Fix((pc >> 8) And &HFF)))
    stkp -= CByte(Fix(1))
    Write(&H100US + stkp, CByte(Fix(pc And &HFF)))
    stkp -= CByte(Fix(1))

    SetFlag(FLAGS6502.B, False)
    SetFlag(FLAGS6502.U, True)
    SetFlag(FLAGS6502.I, True)
    Write(&H100US + stkp, status)
    stkp -= CByte(Fix(1))

    addr_abs = &HFFFA
    Dim lo = Read(addr_abs + &H0US)
    Dim hi = Read(addr_abs + &H1US)
    pc = (hi << 8) Or lo

    cycles = 8

  End Sub

  ' Perform one clock cycles worth of emulation
  Public Sub Clock() ' Perform one clock cycle's worth of update

    ' Each instruction requires a variable number of clock cycles to execute.
    ' In my emulation, I only care about the final result and so I perform
    ' the entire computation in one hit. In hardware, each clock cycle would
    ' perform "microcode" style transformations of the CPUs state.
    '
    ' To remain compliant with connected devices, it's important that the 
    ' emulation also takes "time" in order to execute instructions, so I
    ' implement that delay by simply counting down the cycles required by 
    ' the instruction. When it reaches 0, the instruction is complete, and
    ' the next one is ready to be executed.
    If cycles = 0 Then
      ' Read next instruction byte. This 8-bit value is used to index
      ' the translation table to get the relevant information about
      ' how to implement the instruction
      opcode = Read(pc)

#If LOGMODE Then
        Dim log_pc As UShort = pc
#End If

      ' Always set the unused status flag bit to 1
      SetFlag(FLAGS6502.U, True)

      ' Increment program counter, we read the opcode byte
      pc += B1

      ' Get Starting number of cycles
      cycles = lookup(opcode).cycles

      ' Perform fetch of intermediate data using the
      ' required addressing mode
      Dim additional_cycle1 = lookup(opcode).addrmode.Invoke

      ' Perform operation
      Dim additional_cycle2 = lookup(opcode).operate.Invoke

      ' The addressmode and opcode may have altered the number
      ' of cycles this instruction requires before it's completed
      cycles += (additional_cycle1 And additional_cycle2)

      ' Always set the unused status flag bit to 1
      SetFlag(FLAGS6502.U, True)

#If LOGMODE Then
        ' This logger dumps every cycle the entire processor state for analysis.
        ' This can be used for debugging the emulation, but has little utility
        ' during emulation. Its also very slow, so only use if you have to.
        If logfile Is Nothing Then logfile = File.CreateText("olc6502.txt")
        If logfile IsNot Nothing Then
            logfile.WriteLine("{0, 10}:{1,2} PC:{2,4} {3} A:{4,2} X:{5,2} Y:{6,2} {7}{8}{9}{10}{11}{12}{13}{14} STKP:{15,2}", clock_count, 0, log_pc, "XXX", a, x, y, _
                               If(GetFlag(N), "N", "."c), If(GetFlag(V), "V", "."c), If(GetFlag(U), "U", "."c), _
                               If(GetFlag(B), "B", "."c), If(GetFlag(D), "D", "."c), If(GetFlag(I), "I", "."c), _
                               If(GetFlag(Z), "Z", "."c), If(GetFlag(C), "C", "."c), stkp)
            logfile.Flush()
        End If
#End If
    End If

    ' Increment global clock count - This is actually unused unless logging is enabled
    ' but I've kept it in because its a handy watch variable for debugging
    clock_count += B1

    ' Decrement the number of cycles remaining for this instruction
    cycles -= B1

  End Sub

  ' Indicates the current instruction has completed by returning true. This is
  ' a utility function to enable "step-by-step" execution, without manually 
  ' clocking every cycle
  Public Function Complete() As Boolean
    Return cycles = 0
  End Function

  ' Link this CPU to a communications bus
  Public Sub ConnectBus(n As Bus)
    Me.bus = n
  End Sub

  ' Produces a map of strings, with keys equivalent to instruction start locations
  ' in memory, for the specified address range

  ' This is the disassembly function. Its workings are not required for emulation.
  ' It is merely a convenience function to turn the binary instruction code into
  ' human readable form. Its included as part of the emulator because it can take
  ' advantage of many of the CPUs internal operations to do this.
  Public Function Disassemble(nStart As UShort, nStop As UShort) As Dictionary(Of UShort, String)

    Dim addr As UInteger = nStart
    Dim value As Byte = &H0, lo As Byte = &H0, hi As Byte = &H0
    Dim mapLines As New Dictionary(Of UShort, String)
    Dim line_addr As UShort = 0

    ' A convenient utility to convert variables into
    ' hex strings because "modern C++"'s method with
    ' streams is atrocious
    Dim hex = Function(n As UInteger, d As Byte)
                Return Microsoft.VisualBasic.Hex(n).PadLeft(d, "0"c)
              End Function

    ' Starting at the specified address we read an instruction
    ' byte, which in turn yields information from the lookup table
    ' as to how many additional bytes we need to read and what the
    ' addressing mode is. I need this info to assemble human readable
    ' syntax, which is different depending upon the addressing mode

    ' As the instruction is decoded, a std::string is assembled
    ' with the readable output

    While addr < nStop

      line_addr = CUShort(Fix(addr))

      ' Prefix line with instruction address
      Dim sInst = "$" + hex(addr, 4) + ": "

      ' Read instruction, and get its readable name
      Dim opcode = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
      sInst += lookup(opcode).name + " "

      ' Get oprands from desired locations, and form the
      ' instruction based upon its addressing mode. These
      ' routines mimmick the actual fetch routine of the
      ' 6502 in order to get accurate data as part of the
      ' instruction
      If lookup(opcode).addrmode?.Method.Name = NameOf(IMP) Then
        sInst += " {IMP}"
      ElseIf lookup(opcode).addrmode?.Method.Name = NameOf(IMM) Then
        value = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        sInst += "#$" + hex(value, 2) + " {IMM}"
      ElseIf lookup(opcode).addrmode?.Method.Name = NameOf(ZP0) Then
        lo = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        hi = &H0
        sInst += "$" + hex(lo, 2) + " {ZP0}"
      ElseIf lookup(opcode).addrmode?.Method.Name = NameOf(ZPX) Then
        lo = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        hi = &H0
        sInst += "$" + hex(lo, 2) + ", X {ZPX}"
      ElseIf lookup(opcode).addrmode?.Method.Name = NameOf(ZPY) Then
        lo = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        hi = &H0
        sInst += "$" + hex(lo, 2) + ", Y {ZPY}"
      ElseIf lookup(opcode).addrmode?.Method.Name = NameOf(IZX) Then
        lo = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        hi = &H0
        sInst += "($" + hex(lo, 2) + ", X) {IZX}"
      ElseIf lookup(opcode).addrmode?.Method.Name = NameOf(IZY) Then
        lo = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        hi = &H0
        sInst += "($" + hex(lo, 2) + "), Y {IZY}"
      ElseIf lookup(opcode).addrmode?.Method.Name = NameOf(ABS) Then
        lo = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        hi = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        sInst += "$" + hex((CType(hi, UInt16) << 8) Or lo, 4) + " {ABS}"
      ElseIf lookup(opcode).addrmode?.Method.Name = NameOf(ABX) Then
        lo = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        hi = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        sInst += "$" + hex((CType(hi, UInt16) << 8) Or lo, 4) + ", X {ABX}"
      ElseIf lookup(opcode).addrmode?.Method.Name = NameOf(ABY) Then
        lo = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        hi = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        sInst += "$" + hex((CType(hi, UInt16) << 8) Or lo, 4) + ", Y {ABY}"
      ElseIf lookup(opcode).addrmode?.Method.Name = NameOf(IND) Then
        lo = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        hi = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        sInst += "($" + Hex(CUInt(hi << 8 Or lo), 4) + ") {IND}"
      ElseIf lookup(opcode).addrmode?.Method.Name = NameOf(REL) Then
        value = bus.CpuRead(CUShort(Fix(addr)), True) : addr += 1US
        sInst += "$" + Hex(value, 2) + " [$" + Hex(addr + value, 4) + "] {REL}"
      End If

      ' Add the formed string to a stdmap, using the instruction's
      ' address as the key. This makes it convenient to look for later
      ' as the instructions are variable in length, so a straight up
      ' incremental index Is Not sufficient.
      mapLines(line_addr) = sInst

    End While

    Return mapLines

  End Function

  ' The status register stores 8 flags. Ive enumerated these here for ease
  ' of access. You can access the status register directly since its public.
  ' The bits have different interpretations depending upon the context and 
  ' instruction being executed.
  Enum FLAGS6502 As Byte
    C = (1 << 0) ' Carry Bit
    Z = (1 << 1) ' Zero
    I = (1 << 2) ' Disable Interrupts
    D = (1 << 3) ' Decimal Mode (unused in this implementation)
    B = (1 << 4) ' Break
    U = (1 << 5) ' Unused
    V = (1 << 6) ' Overflow
    N = (1 << 7) ' Negative
  End Enum

  '///////////////////////////////////////////////////////////////////////////////
  '// FLAG FUNCTIONS

  ' Convenience functions to access status register

  ' Returns the value of a specific bit of the status register
  Private Function GetFlag(f As FLAGS6502) As Byte
    Return If((status And f) > 0, B1, B0)
  End Function

  ' Sets or clears a specific bit of the status register
  Private Sub SetFlag(f As FLAGS6502, v As Boolean)
    If v Then
      status = status Or f
    Else
      status = status And Not f
    End If
  End Sub

  ' Assisstive variables to facilitate emulation
  Private fetched As Byte = &H0 ' Represents the working input value to the ALU
  Private temp As UShort = &H0 ' A convenience variable used everywhere
  Private addr_abs As UShort = &H0 ' All used memory addresses end up in here
  Private addr_rel As SByte = &H0 ' Represents absolute address following a branch
  Private opcode As Byte = &H0 ' Is the instruction byte
  Private cycles As Byte = &H0 ' Counts how many cycles the instruction has remaining
  Private clock_count As UInteger = &H0 ' A global accumulation of the number of clocks

  ' BUS CONNECTIVITY

  ' Linkage to the communications bus
  Private bus As Bus = Nothing

  ' Reads an 8-bit byte from the bus, located at the specified 16-bit address
  Private Function Read(a As UShort) As Byte
    ' In normal operation "read only" is set to false. This may seem odd. Some
    ' devices on the bus may change state when they are read from, and this
    ' is intentional under normal circumstances. However the disassembler will
    ' want to read the data at an address without changing the state of the
    ' devices on the bus
    Return bus.CpuRead(a, False)
  End Function

  ' Writes a byte to the bus at the specified address
  Private Sub Write(a As UShort, d As Byte)
    bus.CpuWrite(a, d)
  End Sub

  ' The read location of data can come from two sources, a memory address, or
  ' its immediately available as part of the instruction. This function decides
  ' depending on address mode of instruction byte

  ' This function sources the data used by the instruction into 
  ' a convenient numeric variable. Some instructions don't have to 
  ' fetch data as the source is implied by the instruction. For example
  ' "INX" increments the X register. There is no additional data
  ' required. For all other addressing modes, the data resides at 
  ' the location held within addr_abs, so it is read from there. 
  ' Immediate address mode exploits this slightly, as that has
  ' set addr_abs = pc + 1, so it fetches the data from the
  ' next byte for example "LDA $FF" just loads the accumulator with
  ' 256, i.e. no far-reaching memory fetch is required. "fetched"
  ' is a variable global to the CPU, and is set by calling this 
  ' function. It also returns it for convenience.
  Private Function Fetch() As Byte
    If Not (lookup(opcode).addrmode?.Method?.Name = NameOf(IMP)) Then
      fetched = Read(addr_abs)
    End If
    Return fetched
  End Function

  ' This structure and the following vector are used to compile and store
  ' the opcode translation table. The 6502 can effectively have 256
  ' different instructions. Each of these are stored in a table in numerical
  ' order so they can be looked up easily, with no decoding required.
  ' Each table entry holds:
  '   Pneumonic : A textual representation of the instruction (used for disassembly)
  '   Opcode Function: A function pointer to the implementation of the opcode
  '   Opcode Address Mode : A function pointer to the implementation of the 
  '                         addressing mechanism used by the instruction
  '   Cycle Count : An integer that represents the base number of clock cycles the
  '                 CPU requires to perform the instruction

  Private Structure INSTRUCTION
    Public name As String
    Public operate As Func(Of Byte)
    Public addrmode As Func(Of Byte)
    Public cycles As Byte
    Public Sub New(name As String, operate As Func(Of Byte), addrmode As Func(Of Byte), cycles As Byte)
      Me.name = name
      Me.operate = operate
      Me.addrmode = addrmode
      Me.cycles = cycles
    End Sub
  End Structure

  Private lookup As New List(Of INSTRUCTION)

  '///////////////////////////////////////////////////////////////////////////////
  '// ADDRESSING MODES

  ' Addressing Modes =============================================
  ' The 6502 has a variety of addressing modes to access data in
  ' memory, some of which are direct and some are indirect (like
  ' pointers in C++). Each opcode contains information about which
  ' addressing mode should be employed to facilitate the
  ' instruction, in regards to where it reads/writes the data it
  ' uses. The address mode changes the number of bytes that
  ' makes up the full instruction, so we implement addressing
  ' before executing the instruction, to make sure the program
  ' counter is at the correct location, the instruction is
  ' primed with the addresses it needs, and the number of clock
  ' cycles the instruction requires is calculated. These functions
  ' may adjust the number of cycles required depending upon where
  ' and how the memory is accessed, so they return the required
  ' adjustment.

  ' The 6502 can address between 0x0000 - 0xFFFF. The high byte is often referred
  ' to as the "page", and the low byte is the offset into that page. This implies
  ' there are 256 pages, each containing 256 bytes.
  '
  ' Several addressing modes have the potential to require an additional clock
  ' cycle if they cross a page boundary. This is combined with several instructions
  ' that enable this additional clock cycle. So each addressing function returns
  ' a flag saying it has potential, as does each instruction. If both instruction
  ' and address function return 1, then an additional clock cycle is required.

  ' Address Mode: Implied
  ' There is no additional data required for this instruction. The instruction
  ' does something very simple like like sets a status bit. However, we will
  ' target the accumulator, for instructions like PHA
  Private Function IMP() As Byte
    fetched = a
    Return 0
  End Function

  ' Address Mode: Immediate
  ' The instruction expects the next byte to be used as a value, so we'll prep
  ' the read address to point to the next byte
  Private Function IMM() As Byte
    addr_abs = pc
    pc += B1
    Return 0
  End Function

  ' Address Mode: Zero Page
  ' To save program bytes, zero page addressing allows you to absolutely address
  ' a location in first 0xFF bytes of address range. Clearly this only requires
  ' one byte instead of the usual two.
  Private Function ZP0() As Byte
    addr_abs = Read(pc)
    pc += B1
    addr_abs = addr_abs And &HFFUS
    Return 0
  End Function

  ' Address Mode: Zero Page with X Offset
  ' Fundamentally the same as Zero Page addressing, but the contents of the X Register
  ' is added to the supplied single byte address. This is useful for iterating through
  ' ranges within the first page.
  Private Function ZPX() As Byte
    addr_abs = Read(pc) + x
    pc += B1
    addr_abs = addr_abs And &HFFUS
    Return 0
  End Function

  ' Address Mode: Zero Page with Y Offset
  ' Same as above but uses Y Register for offset
  Private Function ZPY() As Byte
    addr_abs = Read(pc) + y
    pc += B1
    addr_abs = addr_abs And &HFFUS
    Return 0
  End Function

  ' Address Mode: Relative
  ' This address mode is exclusive to branch instructions. The address
  ' must reside within -128 to +127 of the branch instruction, i.e.
  ' you cant directly branch to any address in the addressable range.
  Private Function REL() As Byte
    Dim value As Integer = Read(pc) : If value > 127 Then value -= 256
    addr_rel = CSByte(value)
    pc += B1
    Return 0
  End Function

  ' Address Mode: Absolute 
  ' A full 16-bit address is loaded and used
  Private Function ABS() As Byte
    Dim lo As Byte = Read(pc)
    pc += B1
    Dim hi As Byte = Read(pc)
    pc += B1
    addr_abs = (hi << 8) Or lo
    Return 0
  End Function

  ' Address Mode: Absolute with X Offset
  ' Fundamentally the same as absolute addressing, but the contents of the X Register
  ' is added to the supplied two byte address. If the resulting address changes
  ' the page, an additional clock cycle is required
  Private Function ABX() As Byte
    Dim lo As Byte = Read(pc)
    pc += B1
    Dim hi As Byte = Read(pc)
    pc += B1
    addr_abs = (hi << 8) Or lo
    addr_abs += x
    If (addr_abs And &HFF00) <> (hi << 8) Then
      Return 1
    Else
      Return 0
    End If
  End Function

  ' Address Mode: Absolute with Y Offset
  ' Fundamentally the same as absolute addressing, but the contents of the Y Register
  ' is added to the supplied two byte address. If the resulting address changes
  ' the page, an additional clock cycle is required
  Public Function ABY() As Byte
    Dim lo As UShort = Read(pc)
    pc += B1
    Dim hi As UShort = Read(pc)
    pc += B1
    addr_abs = (hi << 8) Or lo
    addr_abs += y
    If (addr_abs And &HFF00) <> (hi << 8) Then
      Return 1
    Else
      Return 0
    End If
  End Function

  ' Note: The next 3 address modes use indirection (aka Pointers!)

  ' Address Mode: Indirect
  ' The supplied 16-bit address is read to get the actual 16-bit address. This is
  ' instruction is unusual in that it has a bug in the hardware! To emulate its
  ' function accurately, we also need to emulate this bug. If the low byte of the
  ' supplied address is 0xFF, then to read the high byte of the actual address
  ' we need to cross a page boundary. This doesnt actually work on the chip as
  ' designed, instead it wraps back around in the same page, yielding an
  ' invalid actual address
  Public Function IND() As Byte
    Dim ptr_lo As UShort = Read(pc)
    pc += B1
    Dim ptr_hi As UShort = Read(pc)
    pc += B1
    Dim ptr As UShort = (ptr_hi << 8) Or ptr_lo
    If ptr_lo = &HFF Then ' Simulate page boundary hardware bug
      addr_abs = (Read(ptr And &HFF00US) << 8) Or Read(ptr + 0US)
    Else ' Behave normally
      addr_abs = (Read(ptr + 1US) << 8) Or Read(ptr + 0US)
    End If
    Return 0
  End Function

  ' Address Mode: Indirect X
  ' The supplied 8-bit address is offset by X Register to index
  ' a location in page 0x00. The actual 16-bit address is read
  ' from this location
  Public Function IZX() As Byte
    Dim t As UShort = Read(pc)
    pc += B1
    Dim lo As UShort = Read((t + x) And &HFFUS)
    Dim hi As UShort = Read((t + x + 1US) And &HFFUS)
    addr_abs = (hi << 8) Or lo
    Return 0
  End Function

  ' Address Mode: Indirect Y
  ' The supplied 8-bit address indexes a location in page 0x00. From 
  ' here the actual 16-bit address is read, and the contents of
  ' Y Register is added to it to offset it. If the offset causes a
  ' change in page then an additional clock cycle is required.
  Public Function IZY() As Byte
    Dim t As UInt16 = Read(pc)
    pc += B1
    Dim lo As UInt16 = Read(t And &HFFUS)
    Dim hi As UInt16 = Read((t + 1US) And &HFFUS)
    addr_abs = (hi << 8) Or lo
    addr_abs += y
    If (addr_abs And &HFF00) <> (hi << 8) Then
      Return 1
    Else
      Return 0
    End If
  End Function

  ' INSTRUCTION IMPLEMENTATIONS
  '
  ' Note: I've started with the two most complicated instructions to emulate, which
  ' ironically is addition and subtraction! I've tried to include a detailed
  ' explanation as to why they are so complex, yet so fundamental. I'm also NOT
  ' going to do this through the explanation of 1 and 2's complement.

  ' Instruction: Add with Carry In
  ' Function: A = A + M + C
  ' Flags Out: C, V, N, Z
  '
  ' Explanation:
  ' The purpose of this function is to add a value to the accumulator and a carry bit. If
  ' the result is > 255 there is an overflow setting the carry bit. Ths allows you to
  ' chain together ADC instructions to add numbers larger than 8-bits. This in itself is
  ' simple, however the 6502 supports the concepts of Negativity/Positivity and Signed Overflow.
  '
  ' 10000100 = 128 + 4 = 132 in normal circumstances, we know this as unsigned and it allows
  ' us to represent numbers between 0 and 255 (given 8 bits). The 6502 can also interpret
  ' this word as something else if we assume those 8 bits represent the range -128 to +127,
  ' i.e. it has become signed.
  '
  ' Since 132 > 127, it effectively wraps around, through -128, to -124. This wraparound is
  ' called overflow, and this is a useful to know as it indicates that the calculation has
  ' gone outside the permissible range, and therefore no longer makes numeric sense.
  '
  ' Note the implementation of ADD is the same in binary, this is just about how the numbers
  ' are represented, so the word 10000100 can be both -124 and 132 depending upon the
  ' context the programming is using it in. We can prove this!
  '
  ' 10000100 = 132 or -124
  ' +00010001 = + 17 + 17
  ' ======== === === See, both are valid additions, but our interpretation of
  ' 10010101 = 149 or -107 the context changes the value, not the hardware!
  '
  ' In principle under the -128 to 127 range:
  ' 10000000 = -128, 11111111 = -1, 00000000 = 0, 00000000 = +1, 01111111 = +127
  ' therefore negative numbers have the most significant set, positive numbers do not
  '
  ' To assist us, the 6502 can set the overflow flag, if the result of the addition has
  ' wrapped around. V <- (A^M) & A^(A+M+C) :D lol, let's work out why!
  '
  ' Let's suppose we have A = 30, M = 10 and C = 0
  ' A = 30 = 00011110
  ' M = 10 = 00001010+
  ' RESULT = 40 = 00101000
  '
  ' Here we have not gone out of range. The resulting significant bit has not changed.
  ' So let's make a truth table to understand when overflow has occurred. Here I take
  ' the MSB of each component, where R is RESULT.
  '
  ' A M R | V | A^R | A^M |~(A^M) |
  ' 0 0 0 | 0 | 0 | 0 | 1 |
  ' 0 0 1 | 1 | 1 | 0 | 1 |
  ' 0 1 0 | 0 | 0 | 1 | 0 |
  ' 0 1 1 | 0 | 1 | 1 | 0 | so V = ~(A^M) & (A^R)
  ' 1 0 0 | 0 | 1 | 1 | 0 |
  ' 1 0 1 | 0 | 0 | 1 | 0 |
  ' 1 1 0 | 1 | 1 | 0 | 1 |
  ' 1 1 1 | 0 | 0 | 0 | 1 |
  '
  ' We can see how the above equation calculates V, based on A, M and R. V was chosen
  ' based on the following hypothesis:
  ' Positive Number + Positive Number = Negative Result -> Overflow
  ' Negative Number + Negative Number = Positive Result -> Overflow
  ' Positive Number + Negative Number = Either Result -> Cannot Overflow
  ' Positive Number + Positive Number = Positive Result -> OK! No Overflow
  ' Negative Number + Negative Number = Negative Result -> OK! NO Overflow

  ' Opcodes ======================================================
  ' There are 56 "legitimate" opcodes provided by the 6502 CPU. I
  ' have not modelled "unofficial" opcodes. As each opcode is
  ' defined by 1 byte, there are potentially 256 possible codes.
  ' Codes are not used in a "switch case" style on a processor,
  ' instead they are repsonisble for switching individual parts of
  ' CPU circuits on and off. The opcodes listed here are official,
  ' meaning that the functionality of the chip when provided with
  ' these codes is as the developers intended it to be. Unofficial
  ' codes will of course also influence the CPU circuitry in
  ' interesting ways, and can be exploited to gain additional
  ' functionality!
  '
  ' These functions return 0 normally, but some are capable of
  ' requiring more clock cycles when executed under certain
  ' conditions combined with certain addressing modes. If that is
  ' the case, they return 1.
  '
  ' I have included detailed explanations of each function in
  ' the class implementation file. Note they are listed in
  ' alphabetical order here for ease of finding.

  Function ADC() As Byte

    ' Grab the data that we are adding to the accumulator
    Fetch()

    ' Add is performed in 16-bit domain for emulation to capture any
    ' carry bit, which will exist in bit 8 of the 16-bit word
    temp = a + fetched + GetFlag(FLAGS6502.C)

    ' The carry flag out exists in the high byte bit 0
    SetFlag(FLAGS6502.C, temp > 255)

    ' The Zero flag is set if the result is 0
    SetFlag(FLAGS6502.Z, (temp And &HFF) = 0)

    ' The signed Overflow flag is set based on all that up there! :D
    SetFlag(FLAGS6502.V, (Not ((a Xor fetched) And (a Xor temp)) And &H80) <> 0)

    ' The negative flag is set to the most significant bit of the result
    SetFlag(FLAGS6502.N, (temp And &H80) <> 0)

    ' Load the result into the accumulator (it's 8-bit dont forget!)
    a = CByte(Fix(temp And &HFF))

    ' This instruction has the potential to require an additional clock cycle
    Return B1

  End Function

  ' Instruction: Subtraction with Borrow In
  ' Function: A = A - M - (1 - C)
  ' Flags Out: C, V, N, Z
  '
  ' Explanation:
  ' Given the explanation for ADC above, we can reorganize our data
  ' to use the same computation for addition, for subtraction by multiplying
  ' the data by -1, i.e. make it negative
  '
  ' A = A - M - (1 - C) -> A = A + -1 * (M - (1 - C)) -> A = A + (-M + 1 + C)
  '
  ' To make a signed positive number negative, we can invert the bits and add 1
  ' (OK, I lied, a little bit of 1 and 2s complement :P)
  '
  ' 5 = 00000101
  ' -5 = 11111010 + 00000001 = 11111011 (or 251 in our 0 to 255 range)
  '
  ' The range is actually unimportant, because if I take the value 15, and add 251
  ' to it, given we wrap around at 256, the result is 10, so it has effectively
  ' subtracted 5, which was the original intention. (15 + 251) % 256 = 10
  '
  ' Note that the equation above used (1-C), but this got converted to + 1 + C.
  ' This means we already have the +1, so all we need to do is invert the bits
  ' of M, the data(!) therefore we can simply add, exactly the same way we did
  ' before.
  Private Function SBC() As Byte

    Fetch()

    ' Operating in 16-bit domain to capture carry out
    ' We can invert the bottom 8 bits with bitwise xor
    Dim value As UShort = fetched Xor &HFFUS

    ' Notice this is exactly the same as addition from here!
    temp = a + value + GetFlag(FLAGS6502.C)
    SetFlag(FLAGS6502.C, (temp And &HFF00) <> 0)
    SetFlag(FLAGS6502.Z, ((temp And &HFF) = 0))
    SetFlag(FLAGS6502.V, ((temp Xor CUShort(a)) And (temp Xor value) And &H80) <> 0)
    SetFlag(FLAGS6502.N, (temp And &H80) <> 0)
    a = CByte(Fix(temp And &HFF))

    Return B1

  End Function

  ' OK! Complicated operations are done! the following are much simpler
  ' and conventional. The typical order of events is:
  ' 1) Fetch the data you are working with
  ' 2) Perform calculation
  ' 3) Store the result in desired place
  ' 4) Set Flags of the status register
  ' 5) Return if instruction has potential to require additional
  ' clock cycle

  ' Instruction: Bitwise Logic AND
  ' Function: A = A & M
  ' Flags Out: N, Z
  Private Function [AND]() As Byte
    Fetch()
    a = a And fetched
    SetFlag(FLAGS6502.Z, a = &H0)
    SetFlag(FLAGS6502.N, (a And &H80) <> 0)
    Return B1
  End Function

  ' Instruction: Arithmetic Shift Left
  ' Function: A = C <- (A << 1) <- 0
  ' Flags Out: N, Z, C
  Public Function ASL() As Byte
    Fetch()
    Dim temp As UShort = CUShort(fetched) << 1
    SetFlag(FLAGS6502.C, (temp And &HFF00) > 0)
    SetFlag(FLAGS6502.Z, (temp And &HFF) = &H0)
    SetFlag(FLAGS6502.N, (temp And &H80) = &H80)
    If lookup(opcode).addrmode?.Method.Name = NameOf(IMP) Then
      a = CByte(Fix(temp And &HFF))
    Else
      Write(addr_abs, CByte(Fix(temp And &HFF)))
    End If
    Return 0
  End Function

  ' Instruction: Branch if Carry Clear
  ' Function: if(C == 0) pc = address
  Public Function BCC() As Byte
    If GetFlag(FLAGS6502.C) = 0 Then
      cycles += B1
      addr_abs = CUShort(pc + addr_rel)
      If (addr_abs And &HFF00) <> (pc And &HFF00) Then
        cycles += B1
      End If
      pc = addr_abs
    End If
    Return 0
  End Function

  ' Instruction: Branch if Carry Set
  ' Function: if(C == 1) pc = address
  Public Function BCS() As Byte
    If GetFlag(FLAGS6502.C) = 1 Then
      cycles += B1
      addr_abs = CUShort(pc + addr_rel)
      If (addr_abs And &HFF00) <> (pc And &HFF00) Then
        cycles += B1
      End If
      pc = addr_abs
    End If
    Return 0
  End Function

  ' Instruction: Branch if Equal
  ' Function: if(Z == 1) pc = address
  Public Function BEQ() As Byte
    If GetFlag(FLAGS6502.Z) = 1 Then
      cycles += B1
      addr_abs = CUShort(pc + addr_rel)
      If (addr_abs And &HFF00) <> (pc And &HFF00) Then
        cycles += B1
      End If
      pc = addr_abs
    End If
    Return 0
  End Function

  Public Function BIT() As Byte
    Fetch()
    Dim temp As Byte = a And fetched
    SetFlag(FLAGS6502.Z, (temp And &HFF) = &H0)
    SetFlag(FLAGS6502.N, (fetched And (1 << 7)) = (1 << 7))
    SetFlag(FLAGS6502.V, (fetched And (1 << 6)) = (1 << 6))
    Return 0
  End Function

  ' Instruction: Branch if Negative
  ' Function: if(N == 1) pc = address
  Public Function BMI() As Byte
    If GetFlag(FLAGS6502.N) = 1 Then
      cycles += B1
      addr_abs = CUShort(pc + addr_rel)
      If (addr_abs And &HFF00) <> (pc And &HFF00) Then
        cycles += B1
      End If
      pc = addr_abs
    End If
    Return 0
  End Function

  ' Instruction: Branch if Not Equal
  ' Function: if(Z == 0) pc = address
  Public Function BNE() As Byte
    If GetFlag(FLAGS6502.Z) = 0 Then
      cycles += B1
      addr_abs = CUShort(pc + addr_rel)
      If (addr_abs And &HFF00) <> (pc And &HFF00) Then
        cycles += B1
      End If
      pc = addr_abs
    End If
    Return 0
  End Function

  ' Instruction: Branch if Positive
  ' Function: if(N = 0) pc = address
  Public Function BPL() As Byte
    If GetFlag(FLAGS6502.N) = 0 Then
      cycles += B1
      addr_abs = CUShort(pc + addr_rel)
      If (addr_abs And &HFF00) <> (pc And &HFF00) Then
        cycles += B1
      End If
      pc = addr_abs
    End If
    Return 0
  End Function

  ' Instruction: Break
  ' Function: Program Sourced Interrupt
  Public Function BRK() As Byte
    pc += 1US
    SetFlag(FLAGS6502.I, True)
    Write(&H100US + stkp, CByte(Fix((pc >> 8) And &HFF)))
    stkp -= B1
    Write(&H100US + stkp, CByte(Fix(pc And &HFF) <> 0))
    stkp -= B1
    SetFlag(FLAGS6502.B, True)
    Write(&H100US + stkp, status)
    stkp -= B1
    SetFlag(FLAGS6502.B, False)
    pc = Read(&HFFFE) Or (Read(&HFFFF) << 8)
    Return 0
  End Function

  ' Instruction: Branch if Overflow Clear
  ' Function: if(V == 0) pc = address
  Private Function BVC() As Byte
    If GetFlag(FLAGS6502.V) = 0 Then
      cycles += B1
      addr_abs = CUShort(pc + addr_rel)
      If (addr_abs And &HFF00) <> (pc And &HFF00) Then
        cycles += B1
      End If
      pc = addr_abs
    End If
    Return 0
  End Function

  ' Instruction: Branch if Overflow Set
  ' Function: if(V == 1) pc = address
  Private Function BVS() As Byte
    If GetFlag(FLAGS6502.V) = 1 Then
      cycles += B1
      addr_abs = CUShort(pc + addr_rel)
      If (addr_abs And &HFF00) <> (pc And &HFF00) Then
        cycles += B1
      End If
      pc = addr_abs
    End If
    Return 0
  End Function

  ' Instruction: Clear Carry Flag
  ' Function:    C = 0
  Private Function CLC() As Byte
    SetFlag(FLAGS6502.C, False)
    Return 0
  End Function

  ' Instruction: Clear Decimal Flag
  ' Function:    D = 0
  Private Function CLD() As Byte
    SetFlag(FLAGS6502.D, False)
    Return 0
  End Function

  ' Instruction: Disable Interrupts / Clear Interrupt Flag
  ' Function:    I = 0
  Private Function CLI() As Byte
    SetFlag(FLAGS6502.I, False)
    Return 0
  End Function

  ' Instruction: Clear Overflow Flag
  ' Function:    V = 0
  Private Function CLV() As Byte
    SetFlag(FLAGS6502.V, False)
    Return 0
  End Function

  ' Instruction: Compare Accumulator
  ' Function:    C <- A >= M      Z <- (A - M) == 0
  ' Flags Out:   N, C, Z
  Private Function CMP() As Byte
    Fetch()
    temp = CUShort(a) - CUShort(fetched)
    SetFlag(FLAGS6502.C, a >= fetched)
    SetFlag(FLAGS6502.Z, (temp And &HFF) = &H0)
    SetFlag(FLAGS6502.N, (temp And &H80) <> 0)
    Return 1
  End Function

  ' Instruction: Compare X Register
  ' Function:    C <- X >= M      Z <- (X - M) == 0
  ' Flags Out:   N, C, Z
  Private Function CPX() As Byte
    Fetch()
    temp = CUShort(x) - CUShort(fetched)
    SetFlag(FLAGS6502.C, x >= fetched)
    SetFlag(FLAGS6502.Z, (temp And &HFF) = &H0)
    SetFlag(FLAGS6502.N, (temp And &H80) <> 0)
    Return 0
  End Function

  ' Instruction: Compare Y Register
  ' Function:    C <- Y >= M      Z <- (Y - M) == 0
  ' Flags Out:   N, C, Z
  Private Function CPY() As Byte
    Fetch()
    temp = CUShort(y) - CUShort(fetched)
    SetFlag(FLAGS6502.C, y >= fetched)
    SetFlag(FLAGS6502.Z, (temp And &HFF) = &H0)
    SetFlag(FLAGS6502.N, (temp And &H80) <> 0)
    Return 0
  End Function

  ' Instruction: Decrement Value at Memory Location
  ' Function:    M = M - 1
  ' Flags Out:   N, Z
  Private Function DEC() As Byte
    Fetch()
    Dim temp As UInt16 = fetched - B1
    Write(addr_abs, CByte(Fix(temp And &HFF)))
    SetFlag(FLAGS6502.Z, (temp And &HFF) = &H0)
    SetFlag(FLAGS6502.N, (temp And &H80) <> 0)
    Return 0
  End Function

  ' Instruction: Decrement X Register
  ' Function:    X = X - 1
  ' Flags Out:   N, Z
  Private Function DEX() As Byte
    x -= B1
    SetFlag(FLAGS6502.Z, x = &H0)
    SetFlag(FLAGS6502.N, (x And &H80) <> 0)
    Return 0
  End Function

  ' Instruction: Decrement Y Register
  ' Function:    Y = Y - 1
  ' Flags Out:   N, Z
  Private Function DEY() As Byte
    y -= B1
    SetFlag(FLAGS6502.Z, y = &H0)
    SetFlag(FLAGS6502.N, (y And &H80) <> 0)
    Return 0
  End Function

  ' Instruction: Bitwise Logic XOR
  ' Function:    A = A xor M
  ' Flags Out:   N, Z
  Private Function EOR() As Byte
    Fetch()
    a = a Xor fetched
    SetFlag(FLAGS6502.Z, a = &H0)
    SetFlag(FLAGS6502.N, (a And &H80) <> 0)
    Return 1
  End Function

  ' Instruction: Increment Value at Memory Location
  ' Function:    M = M + 1
  ' Flags Out:   N, Z
  Private Function INC() As Byte
    Fetch()
    temp = fetched + B1
    Write(addr_abs, CByte(Fix(temp And &HFF)))
    SetFlag(FLAGS6502.Z, (temp And &HFF) = &H0)
    SetFlag(FLAGS6502.N, (temp And &H80) <> 0)
    Return 0
  End Function

  ' Instruction: Increment X Register
  ' Function:    X = X + 1
  ' Flags Out:   N, Z
  Private Function INX() As Byte
    x += B1
    SetFlag(FLAGS6502.Z, x = &H0)
    SetFlag(FLAGS6502.N, (x And &H80) <> 0)
    Return 0
  End Function

  ' Instruction: Increment Y Register
  ' Function:    Y = Y + 1
  ' Flags Out:   N, Z
  Private Function INY() As Byte
    y += B1
    SetFlag(FLAGS6502.Z, y = &H0)
    SetFlag(FLAGS6502.N, (y And &H80) <> 0)
    Return 0
  End Function

  ' Instruction: Jump To Location
  ' Function:    pc = address
  Private Function JMP() As Byte
    pc = addr_abs
    Return 0
  End Function

  ' Instruction: Jump To Sub-Routine
  ' Function:    Push current pc to stack, pc = address
  Private Function JSR() As Byte
    pc -= 1US
    Write(&H100US + stkp, CByte(Fix((pc >> 8) And &HFF)))
    stkp -= B1
    Write(&H100US + stkp, CByte(Fix(pc And &HFF)))
    stkp -= B1
    pc = addr_abs
    Return 0
  End Function

  ' Instruction: Load The Accumulator
  ' Function:    A = M
  ' Flags Out:   N, Z
  Private Function LDA() As Byte
    Fetch()
    a = fetched
    SetFlag(FLAGS6502.Z, a = &H0)
    SetFlag(FLAGS6502.N, (a And &H80) <> 0)
    Return 1
  End Function

  ' Instruction: Load The X Register
  ' Function:    X = M
  ' Flags Out:   N, Z
  Private Function LDX() As Byte
    Fetch()
    x = fetched
    SetFlag(FLAGS6502.Z, x = &H0)
    SetFlag(FLAGS6502.N, (x And &H80) <> 0)
    Return 1
  End Function

  ' Instruction: Load The Y Register
  ' Function:    Y = M
  ' Flags Out:   N, Z
  Private Function LDY() As Byte
    Fetch()
    y = fetched
    SetFlag(FLAGS6502.Z, y = &H0)
    SetFlag(FLAGS6502.N, (y And &H80) <> 0)
    Return 1
  End Function

  Private Function LSR() As Byte
    Fetch()
    SetFlag(FLAGS6502.C, (fetched And &H1) <> 0)
    temp = fetched >> 1
    SetFlag(FLAGS6502.Z, (temp And &HFF) = &H0)
    SetFlag(FLAGS6502.N, (temp And &H80) <> 0)
    If lookup(opcode).addrmode?.Method.Name = NameOf(IMP) Then
      a = CByte(Fix(temp And &HFF))
    Else
      Write(addr_abs, CByte(Fix(temp And &HFF)))
    End If
    Return 0
  End Function

  Private Function NOP() As Byte
    Select Case opcode
      Case &H1C, &H3C, &H5C, &H7C, &HDC, &HFC
        Return 1
    End Select
    Return 0
  End Function

  ' Instruction: Bitwise Logic OR
  ' Function:    A = A | M
  ' Flags Out:   N, Z
  Private Function ORA() As Byte
    Fetch()
    a = a Or fetched
    SetFlag(FLAGS6502.Z, a = &H0)
    SetFlag(FLAGS6502.N, (a And &H80) <> 0)
    Return 1
  End Function

  ' Instruction: Push Accumulator to Stack
  ' Function:    A -> stack
  Private Function PHA() As Byte
    Write(&H100US + stkp, a)
    stkp -= B1
    Return 0
  End Function

  ' Instruction: Push Status Register to Stack
  ' Function:    status -> stack
  ' Note:        Break flag is set to 1 before push
  Private Function PHP() As Byte
    Write(&H100US + stkp, status Or FLAGS6502.B Or FLAGS6502.U)
    SetFlag(FLAGS6502.B, False)
    SetFlag(FLAGS6502.U, False)
    stkp -= B1
    Return 0
  End Function

  ' Instruction: Pop Accumulator off Stack
  ' Function:    A <- stack
  ' Flags Out:   N, Z
  Private Function PLA() As Byte
    stkp += B1
    a = Read(&H100US + stkp)
    SetFlag(FLAGS6502.Z, a = &H0)
    SetFlag(FLAGS6502.N, (a And &H80) <> 0)
    Return 0
  End Function

  ' Instruction: Pop Status Register off Stack
  ' Function:    Status <- stack
  Private Function PLP() As Byte
    stkp += B1
    status = Read(&H100US + stkp)
    SetFlag(FLAGS6502.U, True)
    Return 0
  End Function

  Private Function ROL() As Byte
    Fetch()
    temp = CUShort((fetched << 1) Or GetFlag(FLAGS6502.C))
    SetFlag(FLAGS6502.C, (temp And &HFF00) <> 0)
    SetFlag(FLAGS6502.Z, (temp And &HFF) = &H0)
    SetFlag(FLAGS6502.N, (temp And &H80) <> 0)
    If lookup(opcode).addrmode?.Method.Name = NameOf(IMP) Then
      a = CByte(Fix(temp And &HFF))
    Else
      Write(addr_abs, CByte(Fix(temp And &HFF)))
    End If
    Return 0
  End Function

  Private Function ROR() As Byte
    Fetch()
    temp = CUShort((GetFlag(FLAGS6502.C) << 7) Or (fetched >> 1))
    SetFlag(FLAGS6502.C, (fetched And &H1) <> 0)
    SetFlag(FLAGS6502.Z, (temp And &HFF) = &H0)
    SetFlag(FLAGS6502.N, (temp And &H80) <> 0)
    If lookup(opcode).addrmode?.Method.Name = NameOf(IMP) Then
      a = CByte(Fix(temp And &HFF))
    Else
      Write(addr_abs, CByte(Fix(temp And &HFF)))
    End If
    Return 0
  End Function

  Private Function RTI() As Byte
    stkp += B1
    status = Read(&H100US + stkp)
    status = status And Not FLAGS6502.B
    status = status And Not FLAGS6502.U
    stkp += B1
    pc = CUShort(Read(&H100US + stkp))
    stkp += B1
    pc = pc Or (CUShort(Read(&H100US + stkp)) << 8)
    Return 0
  End Function

  Private Function RTS() As Byte
    stkp += B1
    pc = CUShort(Read(&H100US + stkp))
    stkp += B1
    pc = pc Or (CUShort(Read(&H100US + stkp)) << 8)
    pc += 1US
    Return 0
  End Function

  ' Instruction: Set Carry Flag
  ' Function:    C = 1
  Private Function SEC() As Byte
    SetFlag(FLAGS6502.C, True)
    Return 0
  End Function

  ' Instruction: Set Decimal Flag
  ' Function:    D = 1
  Private Function SED() As Byte
    SetFlag(FLAGS6502.D, True)
    Return 0
  End Function

  ' Instruction: Set Interrupt Flag / Enable Interrupts
  ' Function:    I = 1
  Private Function SEI() As Byte
    SetFlag(FLAGS6502.I, True)
    Return 0
  End Function

  ' Instruction: Store Accumulator at Address
  ' Function:    M = A
  Private Function STA() As Byte
    Write(addr_abs, a)
    Return 0
  End Function

  ' Instruction: Store X Register at Address
  ' Function:    M = X
  Private Function STX() As Byte
    Write(addr_abs, x)
    Return 0
  End Function

  ' Instruction: Store Y Register at Address
  ' Function:    M = Y
  Private Function STY() As Byte
    Write(addr_abs, y)
    Return 0
  End Function

  ' Instruction: Transfer Accumulator to X Register
  ' Function: X = A
  ' Flags Out: N, Z
  Private Function TAX() As Byte
    x = a
    SetFlag(FLAGS6502.Z, x = &H0)
    SetFlag(FLAGS6502.N, (x And &H80) <> 0)
    Return 0
  End Function

  ' Instruction: Transfer Accumulator to Y Register
  ' Function: Y = A
  ' Flags Out: N, Z
  Private Function TAY() As Byte
    y = a
    SetFlag(FLAGS6502.Z, y = &H0)
    SetFlag(FLAGS6502.N, (y And &H80) <> 0)
    Return 0
  End Function

  ' Instruction: Transfer Stack Pointer to X Register
  ' Function: X = stack pointer
  ' Flags Out: N, Z
  Private Function TSX() As Byte
    x = stkp
    SetFlag(FLAGS6502.Z, x = &H0)
    SetFlag(FLAGS6502.N, (x And &H80) <> 0)
    Return 0
  End Function

  ' Instruction: Transfer X Register to Accumulator
  ' Function: A = X
  ' Flags Out: N, Z
  Private Function TXA() As Byte
    a = x
    SetFlag(FLAGS6502.Z, a = &H0)
    SetFlag(FLAGS6502.N, (a And &H80) <> 0)
    Return 0
  End Function

  ' Instruction: Transfer X Register to Stack Pointer
  ' Function: stack pointer = X
  Private Function TXS() As Byte
    stkp = x
    Return 0
  End Function

  ' Instruction: Transfer Y Register to Accumulator
  ' Function: A = Y
  ' Flags Out: N, Z
  Private Function TYA() As Byte
    a = y
    SetFlag(FLAGS6502.Z, a = &H0)
    SetFlag(FLAGS6502.N, (a And &H80) <> 0)
    Return 0
  End Function

  ' I capture all "unofficial" opcodes with this function. It is
  ' functionally identical to a NOP
  ' This function captures illegal opcodes
  Private Function XXX() As Byte
    Return 0
  End Function

End Class