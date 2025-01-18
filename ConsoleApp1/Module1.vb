Option Explicit Off
Option Strict Off
Option Infer Off

Module Module1

	Public Sub Main1()

		Dim a(10) As Integer
		Dim b(0 To 10) As Integer
		'Dim c(10 To 10) As Integer

		Dim l = LBound(b)

		FirstName$ = "Cory"
		LastName$ = "Smth"
		GoTo 30
20:
		System.Console.WriteLine($"{FirstName$} {LastName$}") ' This is a test
		End
30:
		LastName$ = "Smith"
		GoTo 20

	End Sub

End Module
