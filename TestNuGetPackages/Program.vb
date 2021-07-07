Imports System

Module Program
  Sub Main() 'args As String())
    Console.WriteLine("Hello World!")
  End Sub
End Module

Public Interface IPerson
  Property Name As String
  Function Test1(a As Integer, b As Integer, byref c As integer) As Integer
  Property Title As string
End Interface

Public Class Employee
  Implements IPerson

  Public Property Name() As String

  Public Property Title() As String

  Public shared Function Test1(a As Integer, b As Integer, byref c As Integer) As Integer
    c=a+b
    Return c
  End Function

End Class