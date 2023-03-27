' Inspired by "Code-It-Yourself! Role Playing Game Part #1" -- @javidx9
' https://youtu.be/xXXt3htgDok
' "Code-It-Yourself! Role Playing Game Part #2" -- @javidx9
' https://youtu.be/AWY_ITpldRk
' "Code-It-Yourself! Role Playing Game Part #3" -- @javidx9
' https://youtu.be/UcNSb-m4YQU
' "Code-It-Yourself! Role Playing Game Part #4" -- @javidx9
' https://youtu.be/AnyoUfeNZ1Y

Option Explicit On
Option Strict On
Option Infer On

Module Program

  Sub Main() 'args As String())
    Dim game As New RPG_Engine
    'Dim game As New RPG_Main
    game.ConstructConsole(256, 240, 4, 4)
    game.Start()
  End Sub

End Module