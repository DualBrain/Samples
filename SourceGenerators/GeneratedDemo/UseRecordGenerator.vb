Option Explicit On
Option Strict On
Option Infer On

Imports RecordGenerator

<Record>
Public Class PersonRecord
  Public Property Name As String
  Public Property Age As Integer
End Class

Public Module UseRecordGenerator

  Public Sub Run()

    Dim r1 = New PersonRecord With {.Name = "A", .Age = 1}
    Dim r2 = New PersonRecord With {.Name = "B", .Age = 2}

    Dim r3 = r1.Clone()

    If r1.Equals(r2) Then

    End If

  End Sub

End Module