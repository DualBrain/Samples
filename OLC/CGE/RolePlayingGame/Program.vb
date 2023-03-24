' Inspired by "Code-It-Yourself! Role Playing Game Part #1" -- @javidx9
' https://youtu.be/xXXt3htgDok
' "Code-It-Yourself! Role Playing Game Part #2" -- @javidx9
' https://youtu.be/AWY_ITpldRk

Option Explicit On
Option Strict On
Option Infer On

Module Program

  Sub Main() 'args As String())
    'Dim game As New RPG_Engine
    Dim game As New RPG_Main
    game.ConstructConsole(256, 240, 4, 4)
    game.Start()
  End Sub

End Module