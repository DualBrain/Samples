Option Explicit On
Option Strict On
Option Infer On

Friend MustInherit Class Command

  Public Sub New()
  End Sub

  Friend Property Started As Boolean = False
  Friend Property Completed As Boolean = False

  Friend MustOverride Sub Start()

  Friend Overridable Sub Update(elapsedTime As Single)
  End Sub

  Friend Shared g_engine As RPG_Engine

End Class

Friend Class ScriptProcessor

  Friend Sub New()
  End Sub

  Friend Property UserControlEnabled As Boolean = False

  Private ReadOnly m_listCommands As New List(Of Command)

  Friend Sub AddCommand(cmd As Command)
    m_listCommands.Add(cmd)
  End Sub

  Friend Sub ProcessCommands(fElapsedTime As Single)

    ' If command are available, halt user control
    UserControlEnabled = Not m_listCommands.Any

    If m_listCommands.Any Then
      ' A command is available
      If Not m_listCommands(0).Completed Then
        ' Command has not been started
        If Not m_listCommands(0).Started Then
          m_listCommands(0).Start()
          m_listCommands(0).Started = True
        Else ' Command has been started so process it
          m_listCommands(0).Update(fElapsedTime)
        End If
      Else
        ' Command has been completed
        m_listCommands(0) = Nothing
        m_listCommands.RemoveAt(0)
      End If
    End If

  End Sub

  ' Marks currently active command as complete, from external source
  Friend Sub CompleteCommand()

    If m_listCommands.Any Then
      m_listCommands(0).Completed = True
    End If

  End Sub

End Class

Friend Class Command_MoveTo
  Inherits Command

  Private ReadOnly m_obj As Dynamic
  Private m_startPosX As Single
  Private m_startPosY As Single
  Private ReadOnly m_targetPosX As Single
  Private ReadOnly m_targetPosY As Single
  Private ReadOnly m_duration As Single
  Private m_timeSoFar As Single

  Friend Sub New(obj As Dynamic, x As Single, y As Single, Optional duration As Single = 0.0F)

    m_targetPosX = x
    m_targetPosY = y
    m_timeSoFar = 0.0F
    m_duration = Single.Max(duration, 0.001F)
    m_obj = obj

  End Sub

  Friend Overrides Sub Start()
    m_startPosX = m_obj.Px
    m_startPosY = m_obj.Py
  End Sub

  Friend Overrides Sub Update(elapsedTime As Single)

    m_timeSoFar += elapsedTime
    Dim t = m_timeSoFar / m_duration

    If t > 1.0F Then t = 1.0F

    m_obj.Px = (m_targetPosX - m_startPosX) * t + m_startPosX
    m_obj.Py = (m_targetPosY - m_startPosY) * t + m_startPosY
    m_obj.Vx = (m_targetPosX - m_startPosX) / m_duration
    m_obj.Vy = (m_targetPosY - m_startPosY) / m_duration

    If m_timeSoFar >= m_duration Then
      ' object has reached destination, so stop
      m_obj.Px = m_targetPosX
      m_obj.Py = m_targetPosY
      m_obj.Vx = 0.0F
      m_obj.Vy = 0.0F
      Completed = True
    End If

  End Sub

End Class

Friend Class Command_ShowDialog
  Inherits Command

  Private ReadOnly m_lines As List(Of String)

  Friend Sub New(line As List(Of String))
    m_lines = line
  End Sub

  Friend Overrides Sub Start()
    g_engine.ShowDialog(m_lines)
  End Sub

End Class

Friend Class Command_ChangeMap
  Inherits Command

  Private ReadOnly m_mapName As String
  Private ReadOnly m_mapPosX As Single
  Private ReadOnly m_mapPosY As Single

  Friend Sub New(mapName As String, mapPosX As Single, mapPosY As Single)
    m_mapName = mapName
    m_mapPosX = mapPosX
    m_mapPosY = mapPosY
  End Sub

  Friend Overrides Sub Start()
    g_engine.ChangeMap(m_mapName, m_mapPosX, m_mapPosY)
    Completed = True
  End Sub

End Class

Friend Class Command_AddQuest
  Inherits Command

  Private ReadOnly m_quest As Quest

  Friend Sub New(quest As Quest)
    m_quest = quest
  End Sub

  Friend Overrides Sub Start()
    g_engine.AddQuest(m_quest)
    Completed = True
  End Sub

End Class