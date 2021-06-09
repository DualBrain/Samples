Option Explicit On
Option Strict On
Option Infer On

Imports System
Imports System.Runtime.CompilerServices
Imports Community.VisualBasic.Inline

' Because Visual Studio 2019 changed how you create "temporary project";
' I'm using this project as a placeholder for testing out code.

Module Program

  Sub Main() 'args As String())

    ' tab+tab
    'Dim l = New ArrayList
    'l.BinarySearch(Nothing)

    Dim a = New ClassLibrary1.A() With {.A1 = 1}
    Dim b = New ClassLibrary1.B(1)
    Dim c = New ClassLibrary1.C(1, 2)

    Console.WriteLine($"A1:{a.A1}, ToString:'{a}'")
    'Console.WriteLine($"A1:{b.A1}, ToString:'{b}'")
    'Console.WriteLine($"A1:{c.A1}, B1:{c.B1}, ToString:'{c}'")

    'Return

    'Console.WriteLine(QB.Development.STACK)

    '' https://github.com/dotnet/vblang/issues/530
    'ConsoleEx.WriteLines("Test", "Method", "Success")
    'Dim l = New List(Of String) From {{"a", "b", "c", "d"}} 'From {{{"a", "b", "c", "d"}}}
    ''Dim l = {"aaa", "bbb", "ccc", "ddd"}
    ''Dim l = {"aaa", "bbb", "ccc", "ddd"}
    'For index = 0 To l.Count - 1
    '  Console.WriteLine($"{index}:{l(index)}")
    'Next

    Dim l = New List(Of Boolean)

    Dim d = True

    If d = True Then

    End If

    'Dim a = "a"
    'Dim b = "b"
    'Dim c = "c"

    'WriteLineByRef((a))
    'WriteLineByRef((b))
    'WriteLineByRef((c))

    ClassLibrary1.Class1.InlineIncrementTest()
    Console.WriteLine()
    InlineIncrementTest()

    Dim result1 = TestingOptional("abc")
    Dim result2 = TestingOptional("xyz", result1)

  End Sub

  Private Function TestingOptional(value1 As String, Optional value2 As Microsoft.CodeAnalysis.Optional(Of String) = Nothing) As String
    If Not value2.HasValue Then
      ' Do one thing...
      Return value1
    Else
      ' Do another thing...
      If value2.Value Is Nothing Then
        Return value1
      Else
        Return value1 & value2.Value
      End If
    End If
  End Function

  Public Sub InlineIncrementTest()

    'var header = New Byte[8];
    'int Index = 0;
    'var magic_signature = ((uint)header[index++] << 0) | ((uint)header[index++] << 8) |  ((uint)header[index++] << 16) | ((uint)header[index++] << 24);

    'Dim header(8) As Byte
    'Dim index = 0
    'Dim magic_signature = (CUInt(header(Math.Max(Threading.Interlocked.Increment(index), index - 1))) << 0) Or
    '                      (CUInt(header(Math.Max(Threading.Interlocked.Increment(index), index - 1))) << 8) Or
    '                      (CUInt(header(Math.Max(Threading.Interlocked.Increment(index), index - 1))) << 16) Or
    '                      (CUInt(header(Math.Max(Threading.Interlocked.Increment(index), index - 1))) << 24)

    Dim value As Integer = 1
    Console.WriteLine($"({value}) value=5 = {Community.VisualBasic.Inline.Assign(value, 5)} ({value})")
    Console.WriteLine($"({value}) value++ = {IncrAfter(value)} ({value})")
    Console.WriteLine($"({value}) value-- = {DecrAfter(value)} ({value})")
    Console.WriteLine($"({value}) ++value = {IncrBefore(value)} ({value})")
    Console.WriteLine($"({value}) --value = {DecrBefore(value)} ({value})")

  End Sub

  ' For an interesting way to write "flags" and/or enums that are pretty much specific to a particular method.
  ' https://github.com/dotnet/vblang/issues/526#issuecomment-748856530
  Enum Order : First : Second : Last : End Enum
  Sub Foo1(x As Order)
    Select Case x
      Case Order.First
      Case Order.Second
      Case Else
    End Select
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

Class Test

  Private m_blah As Boolean

  Public Function Blah(value As Boolean) As Boolean
    Dim bb = New B
    Dim something = NameOf(B)
    CreateWidget()
    m_blah = value
    Return m_blah
  End Function

  Private Sub CreateWidget()
    Throw New NotImplementedException
  End Sub

End Class

Class B

End Class

MustInherit Class C

  Public MustOverride Sub M()

End Class

Class D
  Inherits C

  Public Overrides Sub M()
    Throw New NotImplementedException()
  End Sub
End Class

'Class Test1

'  ' https://docs.microsoft.com/en-us/dotnet/visual-basic/programming-guide/language-features/procedures/partial-methods
'  Partial Private Shared Sub Test()
'    ' Must be blank... will be implemented "elsewhere".
'  End Sub

'End Class

'Partial Class Test1

'  Private Shared Sub Test()
'    Console.WriteLine("B")
'  End Sub

'End Class