Option Explicit On
Option Strict On
Option Infer On

Imports System.IO
Imports ConsoleGameEngine
Imports RolePlayingGame.Quest

Friend Class Map

  Friend Shared g_script As ScriptProcessor

  Private m_indices() As Integer
  Private m_solids() As Boolean

  Friend Property Width As Integer
  Friend Property Height As Integer
  Friend Property Name As String
  Friend Property Sprite As Sprite = Nothing

  Friend Sub New()
    Sprite = Nothing
    Width = 0
    Height = 0
    m_solids = Nothing
    m_indices = Nothing
  End Sub

  Friend Function GetIndex(x As Single, y As Single) As Integer
    Dim x1 = CInt(Fix(x)) : Dim y1 = CInt(Fix(y))
    If x1 >= 0 AndAlso x1 < Width AndAlso y1 >= 0 AndAlso y1 < Height Then
      Return m_indices(y1 * Width + x1)
    Else
      Return 0
    End If
  End Function

  Friend Function GetSolid(x As Single, y As Single) As Boolean
    Dim x1 = CInt(Fix(x)) : Dim y1 = CInt(Fix(y))
    If x1 >= 0 AndAlso x1 < Width AndAlso y1 >= 0 AndAlso y1 < Height Then
      Return m_solids(y1 * Width + x1)
    Else
      Return True
    End If
  End Function

  Friend Function Create(path As String, sprite As Sprite, name As String) As Boolean
    Me.Name = name
    Me.Sprite = sprite
    If File.Exists(path) Then
      Dim content = IO.File.ReadAllText(path)
      Dim entries = content.Replace(vbCrLf, " "c).Split(" "c)
      Dim index = 0
      Width = CInt(Val(entries(index))) : index += 1
      Height = CInt(Val(entries(index))) : index += 1
      ReDim m_solids((Width * Height) - 1)
      ReDim m_indices((Width * Height) - 1)
      For i = 0 To (Width * Height) - 1
        m_indices(i) = CInt(Val(entries(index))) : index += 1
        m_solids(i) = Val(entries(index)) <> 0 : index += 1
      Next
      Return True
    End If
    Return False
  End Function

  Friend Overridable Function PopulateDynamics(ByRef dynamics As List(Of Dynamic)) As Boolean
    Return False
  End Function

  Friend Overridable Function OnInteraction(ByRef dynamics As List(Of Dynamic), target As Dynamic, nature As NATURE) As Boolean
    Return False
  End Function

End Class

Friend Class Map_Village1
  Inherits Map

  Friend Sub New()
    Create("./rpgdata/map/village1.lvl", RPG_Assets.Get.GetSprite("village"), "coder town")
  End Sub

  Friend Overrides Function PopulateDynamics(ByRef dynamics As List(Of Dynamic)) As Boolean

    ' Add Teleporters
    dynamics.Add(New Dynamic_Teleport(12.0F, 6.0F, "home", 5.0F, 12.0F))

    ' Add Items
    dynamics.Add(New Dynamic_Item(10, 10, RPG_Assets.Get().GetItem("Small Health")))
    dynamics.Add(New Dynamic_Item(12, 10, RPG_Assets.Get().GetItem("Health Boost")))

    For i As Integer = 0 To 2 Step 1
      Dim g1 = New Dynamic_Creature_Skelly()
      dynamics.Add(g1)
      g1.Px = Rnd() * 10 + 5.0F
      g1.Py = Rnd() * 10 + 5.0F
    Next

    Return True
  End Function

  Friend Overrides Function OnInteraction(ByRef dynamics As List(Of Dynamic), target As Dynamic, nature As NATURE) As Boolean
    If target.Name = "Teleport" Then
      g_script.AddCommand(New Command_ChangeMap(CType(target, Dynamic_Teleport).MapName, CType(target, Dynamic_Teleport).MapPosX, CType(target, Dynamic_Teleport).MapPosY))
    End If
    Return False
  End Function

End Class

Friend Class Map_Home1
  Inherits Map

  Friend Sub New()
    Create("./rpgdata/map/home.lvl", RPG_Assets.Get.GetSprite("hitech"), "home")
  End Sub

  Friend Overrides Function PopulateDynamics(ByRef dynamics As List(Of Dynamic)) As Boolean

    ' Front door
    dynamics.Add(New Dynamic_Teleport(5.0F, 13.0F, "coder town", 12.0F, 7.0F))
    dynamics.Add(New Dynamic_Teleport(4.0F, 13.0F, "coder town", 12.0F, 7.0F))

    'Dynamic_Creature
    'Dim c1 As Dynamic_Creature = New Dynamic_Creature("bob", RPG_Assets.Get().GetSprite("skelly"))
    'c1.Px = 12.0F
    'c1.Py = 4.0F
    'vecDyns.Add(c1)

    Return True

  End Function

  Friend Overrides Function OnInteraction(ByRef dynamics As List(Of Dynamic), target As Dynamic, nature As NATURE) As Boolean

    If target.Name = "Teleport" Then
      g_script.AddCommand(New Command_ChangeMap(
            DirectCast(target, Dynamic_Teleport).MapName,
            DirectCast(target, Dynamic_Teleport).MapPosX,
            DirectCast(target, Dynamic_Teleport).MapPosY))
    End If

    'If target.sName = "bob" Then
    '  g_script.AddCommand(New Command_ShowDialog(New List(Of String) From {"Hello!", "I'm Bob!"}))
    'End If

    Return False

  End Function

End Class