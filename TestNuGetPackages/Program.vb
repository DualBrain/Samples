Imports System

Module Program
  Sub Main() 'args As String())
    Console.WriteLine("Hello World!")
  End Sub
End Module

Public Interface IPerson
  Property Name As String

  Property Title As string
End Interface

Public Class Employee
  Implements IPerson

  'Private _Title As string

  Public Property Name As String

  Public property Title As String Implements IPerson.title

End Class