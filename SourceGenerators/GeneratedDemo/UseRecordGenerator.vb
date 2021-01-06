Option Explicit On
Option Strict On
Option Infer On

Imports RecordGenerator

<Record>
Public Class Person
  Public Property FirstName As String
  Public Property LastName As String
  Public Property Birth As Date
End Class

<Record>
Public Class Animal
  Public Property Name As String
End Class

Public Module UseRecordGenerator

  Public Sub Run()

    Dim p1 = New Person() With {.FirstName = "Cory", .LastName = "Smith"}
    Dim p2 = New Person() With {.FirstName = "Bill", .LastName = "Gates"}

    Dim p3 = p2.Clone()

    Dim p4 = New Person() With {.FirstName = "Bill", .LastName = "Gates", .Birth = Today}

    'Dim p5 = New Person()
    'With p5
    '  .FirstName = "Bill"
    '  .LastName = "Gates"
    '  .Birth = Today
    'End With

    'Dim p6 = New Person()
    'p6.FirstName = "Bill"
    'p6.LastName = "Gates"
    'p6.Birth = Today

    Dim h = p1.GetHashCode()

    If p1 = p2 Then
      Console.WriteLine("p1 = p2")
    End If
    If p1 <> p3 Then
      Console.WriteLine("p1 <> p3")
    End If

    Console.WriteLine(p1)
    Console.WriteLine(p3)

    Dim a1 = New Animal With {.Name = "Dog"}

    Dim h2 = a1.GetHashCode()

    Console.WriteLine(a1)

    'If p1 = a1 Then
    '  Console.WriteLine("p1 = a1")
    'End If
    'If p1 <> a1 Then
    '  Console.WriteLine("p1 <> a1")
    'End If

  End Sub

End Module