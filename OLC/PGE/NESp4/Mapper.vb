Option Explicit On
Option Strict On
Option Infer On

Public Class Mapper

  Protected nPRGBanks As Byte = 0
  Protected nCHRBanks As Byte = 0

  Public Sub New(prgBanks As Byte, chrBanks As Byte)
    nPRGBanks = prgBanks
    nCHRBanks = chrBanks
    Reset()
  End Sub

  Protected Sub New()
  End Sub

  Public Overridable Function CpuMapRead(addr As UShort, ByRef mapped_addr As UInteger) As Boolean
    Return False
  End Function

  Public Overridable Function CpuMapWrite(addr As UShort, ByRef mapped_addr As UInteger, data As Byte) As Boolean
    Return False
  End Function

  Public Overridable Function PpuMapRead(addr As UShort, ByRef mapped_addr As UInteger) As Boolean
    Return False
  End Function

  Public Overridable Function PpuMapWrite(addr As UShort, ByRef mapped_addr As UInteger) As Boolean
    Return False
  End Function

  Public Overridable Sub Reset()

  End Sub

End Class