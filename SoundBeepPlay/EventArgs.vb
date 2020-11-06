Public Class AudioEventArgs
  Inherits EventArgs

End Class

Public Class ClockEventArgs
  Inherits EventArgs

  Sub New(total As TimeSpan,
          ByVal current As TimeSpan,
          ByVal eom As TimeSpan)
    Me.Total = total
    Me.Current = current
    Dim remaining As TimeSpan = total.Subtract(current)
    Me.Remaining = remaining
    Me.Eom = eom
  End Sub

  Public ReadOnly Remaining As TimeSpan
  Public ReadOnly Current As TimeSpan
  Public ReadOnly Total As TimeSpan
  Public ReadOnly Eom As TimeSpan

  Public Overrides Function ToString() As String
    Return String.Format("{0:00}:{1:00}", Me.Remaining.Minutes, Me.Remaining.Seconds)
  End Function

End Class

Public Class VuEventArgs
  Inherits EventArgs

  Sub New(left As Single,
          ByVal right As Single)
    Me.Left = left
    Me.Right = right
  End Sub

  Public ReadOnly Left As Single
  Public ReadOnly Right As Single

End Class

Public Class AudioEventArgs2
  Inherits EventArgs

  Sub New(logIndex As Integer, path As String)
    Me.LogIndex = logIndex
    'Me.Path = path
  End Sub

  'Sub New(path As String)
  '  Me.Path = path
  'End Sub

  'Public ReadOnly Path As String
  Public ReadOnly LogIndex As Integer?

End Class

Public Class AudioDebuggerEventArgs2
  Inherits EventArgs

  Sub New(message As String)
    Me.Message = message
  End Sub

  Public ReadOnly Message As String

End Class

Public Class ClockEventArgs2
  Inherits EventArgs

  'Sub New(path As String,
  '        ByVal clock As String,
  '        ByVal total As String)
  '  Me.Path = path
  '  Me.Clock = clock
  '  Me.Total = total
  'End Sub

  Sub New(logIndex As Integer?,
          ByVal path As String,
          ByVal clock As String,
          ByVal total As String)
    Me.LogIndex = logIndex
    'Me.Path = path
    Me.Clock = clock
    Me.Total = total
  End Sub

  Public ReadOnly LogIndex As Integer?
  'Public ReadOnly Path As String
  Public ReadOnly Clock As String
  Public ReadOnly Total As String

End Class

Public Class VuEventArgs2
  Inherits EventArgs

  'Sub New(path As String,
  '        ByVal left As Single,
  '        ByVal right As Single)
  '  Me.Path = path
  '  Me.Left = left
  '  Me.Right = right
  'End Sub

  Sub New(logIndex As Integer?,
          ByVal path As String,
          ByVal left As Single,
          ByVal right As Single)
    Me.LogIndex = logIndex
    'Me.Path = path
    Me.Left = left
    Me.Right = right
  End Sub

  Public ReadOnly LogIndex As Integer?
  'Public ReadOnly Path As String
  Public ReadOnly Left As Single
  Public ReadOnly Right As Single

End Class