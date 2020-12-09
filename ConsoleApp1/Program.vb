Option Explicit On
Option Strict On
Option Infer On

Imports System
Imports System.Runtime.CompilerServices

' Because Visual Studio 2019 changed how you create "temporary project";
' I'm using this project as a placeholder for testing out code.

Module Program

  Sub Main() 'args As String())

    'Console.WriteLine(QB.Development.STACK)

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


  Public Sub SpanTest1()

    '' Create a span over an array.
    'Dim array As Byte() = New Byte(99) {}
    'Dim arraySpan = New Span(Of Byte)(array)

    'Dim data As Byte = 0

    'For ctr As Integer = 0 To arraySpan.Length - 1
    '  arraySpan(ctr) = Math.Min(Threading.Interlocked.Increment(data), data - 1)
    'Next

    'Dim arraySum As Integer = 0

    'For Each value As Byte In array
    '  arraySum += value
    'Next

    'Console.WriteLine($"The sum is {arraySum}")
    '' Output:  The sum is 4950

  End Sub


  Public Sub WriteLineByRef(ByRef value As String)
    Console.WriteLine(value)
    value &= "..."
  End Sub

  Private ReadOnly s_map As Byte() = New Byte() {Byte.Parse("0"), Byte.Parse("1"), Byte.Parse("2"), Byte.Parse("3"), Byte.Parse("4"), Byte.Parse("5"), Byte.Parse("6"), Byte.Parse("7"), Byte.Parse("8"), Byte.Parse("9"), Byte.Parse("A"), Byte.Parse("B"), Byte.Parse("C"), Byte.Parse("D"), Byte.Parse("E"), Byte.Parse("F")}

  Public Sub CanWeUseReadOnlySpans()

    'Dim map As ReadOnlySpan(Of Byte) = s_map
    'If map.Length <> 0 Then
    'End If

    'Dim test = " test ".AsParallel

    Dim aa = CreateA(1) ' New ClassLibrary1.A With {.A1 = 1}
    Dim bb = New ClassLibrary1.B(1)

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