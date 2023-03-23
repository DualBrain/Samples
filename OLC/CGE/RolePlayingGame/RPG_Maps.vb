Option Explicit On
Option Strict On
Option Infer On

Imports System.IO
Imports ConsoleGameEngine

Public Class Map

  Private m_indices() As Integer
  Private m_solids() As Boolean

  Public nWidth As Integer
  Public nHeight As Integer
  Public sName As String
  Public pSprite As Sprite = Nothing

  Public Sub New()

    sName = Nothing
    pSprite = Nothing
    nWidth = 0
    nHeight = 0
    m_solids = Nothing
    m_indices = Nothing

  End Sub

  Public Function Create(fileData As String, sprite As Sprite, name As String) As Boolean
    sName = name
    pSprite = sprite
    If File.Exists(fileData) Then

      Dim content = IO.File.ReadAllText(fileData)
      Dim entries = content.Replace(vbCrLf, " "c).Split(" "c)
      Dim index = 0
      nWidth = CInt(Val(entries(index))) : index += 1
      nHeight = CInt(Val(entries(index))) : index += 1
      ReDim m_solids((nWidth * nHeight) - 1)
      ReDim m_indices((nWidth * nHeight) - 1)
      For i = 0 To (nWidth * nHeight) - 1
        m_indices(i) = CInt(Val(entries(index))) : index += 1
        m_solids(i) = Val(entries(index)) <> 0 : index += 1
      Next

      'Using data As New FileStream(fileData, FileMode.Open, FileAccess.Read)
      '  If data IsNot Nothing Then
      '    Using reader As New BinaryReader(data)
      '      nWidth = reader.ReadInt32()
      '      nHeight = reader.ReadInt32()
      '      ReDim m_solids((nWidth * nHeight) - 1)
      '      ReDim m_indices((nWidth * nHeight) - 1)
      '      For i = 0 To nWidth * nHeight - 1
      '        m_indices(i) = reader.ReadInt32()
      '        m_solids(i) = reader.ReadBoolean()
      '      Next
      '      'reader.Close()
      '    End Using
      '    Return True
      '  End If
      'End Using

      Return True
    End If
    Return False
  End Function

  Public Function GetIndex(x As Single, y As Single) As Integer
    Dim x1 = CInt(Fix(x)) : Dim y1 = CInt(Fix(y))
    If x1 >= 0 AndAlso x1 < nWidth AndAlso y1 >= 0 AndAlso y1 < nHeight Then
      Return m_indices(y1 * nWidth + x1)
    Else
      Return 0
    End If
  End Function

  Public Function GetSolid(x As Single, y As Single) As Boolean
    Dim x1 = CInt(Fix(x)) : Dim y1 = CInt(Fix(y))
    If x1 >= 0 AndAlso x1 < nWidth AndAlso y1 >= 0 AndAlso y1 < nHeight Then
      Return m_solids(y1 * nWidth + x1)
    Else
      Return True
    End If
  End Function

End Class

Public Class Map_Village1
  Inherits Map

  Public Sub New()
    Create("./rpgdata/map/village1.lvl", RPG_Assets.Get.GetSprite("village"), "coder town")
  End Sub

End Class

Public Class Map_Home1
  Inherits Map

  Public Sub New()
    Create("./rpgdata/map/home.lvl", RPG_Assets.Get.GetSprite("hitech"), "home")
  End Sub

End Class