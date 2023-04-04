Option Explicit On
Option Strict On
Option Infer On

Friend Module Program

  Sub Main() 'args As String())
    Dim game As New Empty
    If game.Construct(160, 80, 10, 10) Then
      game.Start()
    End If
  End Sub

End Module

Friend Class Empty
  Inherits Olc.PixelGameEngine

  Friend Sub New()
    AppName = "Empty"
  End Sub

  Protected Overrides Function OnUserCreate() As Boolean
    Return True
  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean
    Return True
  End Function

End Class
