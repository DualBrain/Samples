Option Explicit On
Option Strict On
Option Infer On

Module Program

	Sub Main(args As String())
		'Create an array of 1000 numbers
		Dim x1(1000) As Double
		Dim x2(1000) As Double
		'Add some random numbers to the Array
		Randomize(Timer)
		For i = 0 To 999
			x1(i) = Rnd(1000)
			x2(i) = x1(i)
		Next
		Print("Quick Sort in VB! ..............")
		'Get start time in milliseconds
		Dim startTime = Timer()
		'Run the sort function on the array
		Sort(x1, 1000) 'sorts an array of 10 numbers
		'Get the end time in milliseconds
		Dim endTime = Timer()
		'Convert total runtime to seconds
		Dim time_in_secs = (endTime - startTime) / 1000
		Print($"Time: {time_in_secs}")
		Print("\n----------------------------------\n")
		Print("Bubble Sort in VB! ..............")
		'Get start time in milliseconds
		startTime = Timer()
		'Run the sort function on the array
		Bubble_Sort(x2, 1000) 'sorts an array of 10 numbers
		'Get the end time in milliseconds
		endTime = Timer()
		'Convert total runtime to seconds
		time_in_secs = (endTime - startTime) / 1000
		Print($"Time: {time_in_secs}")
		Print("\n\n")
		Console.ReadLine() 'This will prevent the window from closing when the program ends
	End Sub

	Private Sub Print(value As String)
		If value.Contains("\n") Then value = value.Replace("\n", vbCrLf)
		Console.WriteLine(value)
	End Sub

	Sub Sort(ByRef arr() As Double, n As Integer)
		For i = 0 To n - 1
			For j = i To n - 1
				If arr(j) < arr(i) Then
					Dim temp = arr(i)
					arr(i) = arr(j)
					arr(j) = temp
				End If
			Next
		Next
	End Sub

	Sub Bubble_Sort(ByRef item() As Double, itemCount As Integer)
		Dim counter = itemCount - 1 'rcbasic arrays start at 0 so the last index will be 1 less
		'label doLoop
		Dim hasChanged As Boolean
		Do
			hasChanged = False
			Dim i = 0 'rcbasic arrays start at index 0
			While i < counter
				If item(i) > item(i + 1) Then
					Dim temp = item(i)
					item(i) = item(i + 1)
					item(i + 1) = temp
					hasChanged = True
				End If
				i += 1
			End While

			counter -= 1
		Loop While hasChanged
	End Sub

End Module