Option Explicit On
Option Strict On
Option Infer On

Public Class Cartridge

  Private bImageValid As Boolean = False
  Private nMapperID As Byte = 0
  Private nPRGBanks As Byte = 0
  Private nCHRBanks As Byte = 0
  Private vPRGMemory As New List(Of Byte)
  Private vCHRMemory As New List(Of Byte)
  Private pMapper As Mapper

  Private Structure sHeader
    Public name As Char()
    Public prg_rom_chunks As Byte
    Public chr_rom_chunks As Byte
    Public mapper1 As Byte
    Public mapper2 As Byte
    Public prg_ram_size As Byte
    Public tv_system1 As Byte
    Public tv_system2 As Byte
    Public unused As Char()
  End Structure

  Public Enum MIRRORS
    HORIZONTAL
    VERTICAL
    ONESCREEN_LO
    ONESCREEN_HI
  End Enum

  Public mirror As MIRRORS = MIRRORS.HORIZONTAL

  Public Sub New(sFileName As String)

    Dim header As sHeader
    bImageValid = False

    Dim ifs As New IO.FileStream(sFileName, IO.FileMode.Open)
    Dim br As New IO.BinaryReader(ifs)

    If ifs IsNot Nothing Then

      header.name = br.ReadChars(4)
      header.prg_rom_chunks = br.ReadByte()
      header.chr_rom_chunks = br.ReadByte()
      header.mapper1 = br.ReadByte()
      header.mapper2 = br.ReadByte()
      header.prg_ram_size = br.ReadByte()
      header.tv_system1 = br.ReadByte()
      header.tv_system2 = br.ReadByte()
      header.unused = br.ReadChars(5)

      If (header.mapper1 And &H4) <> 0 Then
        ifs.Seek(512, IO.SeekOrigin.Current)
      End If

      nMapperID = ((header.mapper2 >> 4) << 4) Or (header.mapper1 >> 4)
      mirror = If((header.mapper1 And &H1) <> 0, MIRRORS.VERTICAL, MIRRORS.HORIZONTAL)

      Dim nFileType As Byte = 1

      If nFileType = 0 Then

      End If

      If nFileType = 1 Then
        nPRGBanks = header.prg_rom_chunks
        vPRGMemory.Capacity = nPRGBanks * 16384
        vPRGMemory.AddRange(br.ReadBytes(vPRGMemory.Capacity))

        nCHRBanks = header.chr_rom_chunks
        vCHRMemory.Capacity = nCHRBanks * 8192
        vCHRMemory.AddRange(br.ReadBytes(vCHRMemory.Capacity))
      End If

      If nFileType = 2 Then

      End If

      Select Case nMapperID
        Case 0 : pMapper = New Mapper_000(nPRGBanks, nCHRBanks)
      End Select

      bImageValid = True

      ifs.Close()

    End If

  End Sub

  Public Sub New()
  End Sub

  Public Function ImageValid() As Boolean
    Return bImageValid
  End Function

  Public Function CpuRead(addr As UShort, ByRef data As Byte) As Boolean
    Dim mapped_addr As UInteger = 0
    If pMapper.CpuMapRead(addr, mapped_addr) Then
      data = vPRGMemory(CInt(mapped_addr))
      Return True
    Else
      Return False
    End If
  End Function

  Public Function CpuWrite(addr As UShort, data As Byte) As Boolean
    Dim mapped_addr As UInteger = 0
    If pMapper.CpuMapWrite(addr, mapped_addr) Then
      vPRGMemory(CInt(mapped_addr)) = data
      Return True
    Else
      Return False
    End If
  End Function

  Public Function PpuRead(addr As UShort, data As Byte) As Boolean
    Dim mapped_addr As UInteger = 0
    If pMapper.PpuMapRead(addr, mapped_addr) Then
      data = vCHRMemory(CInt(mapped_addr))
      Return True
    Else
      Return False
    End If
  End Function

  Public Function PpuWrite(addr As UShort, data As Byte) As Boolean
    Dim mapped_addr As UInteger = 0
    If pMapper.PpuMapRead(addr, mapped_addr) Then
      vCHRMemory(CInt(mapped_addr)) = data
      Return True
    Else
      Return False
    End If
  End Function

End Class