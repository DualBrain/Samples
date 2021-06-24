Imports System

Module Program
  Sub Main() 'args As String())
    Console.WriteLine("Hello World!")
  End Sub
End Module

Public Interface IPerson
  Property Name As String
End Interface

Public Class Employee
  Implements IPerson

  Public Property Name As String

End Class