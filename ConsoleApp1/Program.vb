Imports System
Imports System.Runtime.CompilerServices

' Because Visual Studio 2019 changed how you create "temporary project";
' I'm using this project as a placeholder for testing out code.

Module Program

  Sub Main() 'args As String())

    ' https://github.com/dotnet/vblang/issues/530
    ConsoleEx.WriteLines("Test", "Method", "Success")
    Dim l = New List(Of String) From {{"a", "b", "c", "d"}} 'From {{{"a", "b", "c", "d"}}}
    'Dim l = {"aaa", "bbb", "ccc", "ddd"}
    'Dim l = {"aaa", "bbb", "ccc", "ddd"}
    For index = 0 To l.Count - 1
      Console.WriteLine($"{index}:{l(index)}")
    Next

    Dim a = "a"
    Dim b = "b"
    Dim c = "c"

    WriteLineByRef((a))
    WriteLineByRef((b))
    WriteLineByRef((c))

  End Sub

  Public Sub WriteLineByRef(ByRef value As String)
    Console.WriteLine(value)
    value &= "..."
  End Sub

  Private ReadOnly s_map = New Byte() {Byte.Parse("0"), Byte.Parse("1"), Byte.Parse("2"), Byte.Parse("3"), Byte.Parse("4"), Byte.Parse("5"), Byte.Parse("6"), Byte.Parse("7"), Byte.Parse("8"), Byte.Parse("9"), Byte.Parse("A"), Byte.Parse("B"), Byte.Parse("C"), Byte.Parse("D"), Byte.Parse("E"), Byte.Parse("F")}

  Public Sub CanWeUseReadOnlySpans()

    Dim map As ReadOnlySpan(Of Byte) = s_map
    If map.Length <> 0 Then
    End If

    'Dim test = " test ".AsParallel

  End Sub

End Module

Public NotInheritable Class ConsoleEx

  ' https://github.com/dotnet/vblang/issues/530
  Public Shared Sub WriteLines(ParamArray lines As String())
    For Each line In lines
      Console.WriteLine(line)
    Next
  End Sub

End Class

Public Module Extensions

  ' https://github.com/dotnet/vblang/issues/530
  <Extension>
  Sub Add(l As List(Of String), ParamArray a() As String)
    For Each entry In a
      l.Add(entry)
    Next
  End Sub

End Module