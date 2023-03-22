Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour

Module Program

  Sub Main() 'args As String())
    Dim game As New Empty
    game.ConstructConsole(160, 80, 10, 10)
    game.Start()
  End Sub

End Module

Class Empty
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Public Overrides Function OnUserCreate() As Boolean

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Return True

  End Function

End Class
