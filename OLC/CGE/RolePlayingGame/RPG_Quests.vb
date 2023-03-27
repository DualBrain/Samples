Option Explicit On
Option Strict On
Option Infer On

Friend Class Quest

  Friend Shared g_script As ScriptProcessor
  Friend Shared g_engine As RPG_Engine

  Friend Enum NATURE
    TALK = 0
    ATTACK = 1
    KILL = 2
    WALK = 3
  End Enum

  Friend Property Name As String
  Friend Property Completed As Boolean = False

  Friend Sub New()
  End Sub

  Friend Overridable Function OnInteraction(ByRef dynamics As List(Of Dynamic), target As Dynamic, nature As NATURE) As Boolean
    Return True
  End Function

  Friend Overridable Function PopulateDynamics(ByRef dynamics As List(Of Dynamic), sMap As String) As Boolean
    Return True
  End Function

End Class

Friend Class Quest_MainQuest
  Inherits Quest

  Friend Overrides Function PopulateDynamics(ByRef dynamics As List(Of Dynamic), map As String) As Boolean

    If map = "coder town" Then
      Dim c1 As New Dynamic_Creature("sarah", RPG_Assets.Get().GetSprite("purple")) With {.Px = 6.0F, .Py = 4.0F, .Friendly = True}
      dynamics.Add(c1)
    End If

    If map = "home" Then
      Dim c1 As New Dynamic_Creature("bob", RPG_Assets.Get().GetSprite("skelly")) With {.Px = 12.0F, .Py = 4.0F}
      dynamics.Add(c1)
    End If

    Return True

  End Function

  Friend Overrides Function OnInteraction(ByRef dynamics As List(Of Dynamic), target As Dynamic, nature As NATURE) As Boolean

    If target.Name = "sarah" Then
      'g_script.AddCommand(New Command_ShowDialog(New List(Of String) From {"[Sarah]", "You have no additional", "quests!"}))
      If g_engine.HasItem(RPG_Assets.Get().GetItem("Health Boost")) Then
        g_script.AddCommand(New Command_ShowDialog(New List(Of String) From {"[Sarah]", "Woooooow! You have a health", "boost!"}))
      Else
        g_script.AddCommand(New Command_ShowDialog(New List(Of String) From {"[Sarah]", "Boooooo! You dont have a health", "boost!"}))
      End If
    End If

    If target.Name = "bob" Then
      g_script.AddCommand(New Command_ShowDialog(New List(Of String) From {"[Bob]", "I need you to do", "something for me!"}))
      g_script.AddCommand(New Command_ShowDialog(New List(Of String) From {"[Bob]", "Predictably, there are", "rats in my basement!"}))
      g_script.AddCommand(New Command_AddQuest(New Quest_BobsQuest()))
    End If

    Return False

  End Function

End Class

Friend Class Quest_BobsQuest
  Inherits Quest

  Friend Overrides Function PopulateDynamics(ByRef dynamics As List(Of Dynamic), map As String) As Boolean
    Return True
  End Function

  Friend Overrides Function OnInteraction(ByRef dynamics As List(Of Dynamic), target As Dynamic, nature As NATURE) As Boolean
    If target.Name = "sarah" Then
      g_script.AddCommand(New Command_ShowDialog(New List(Of String) From {"[Sarah]", "You are doing Bob's", "quest!"}))
      Return True
    End If
    Return False
  End Function

End Class