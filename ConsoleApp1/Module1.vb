Option Explicit Off
Option Strict Off
Option Infer Off

Module Module1

	Public Sub Main1()

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
