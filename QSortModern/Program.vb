' https://youtu.be/OKc2hAmMOY4?si=vYLl5nRcOvpeW-W_
'
' qsort []     = []
' qsort (x:xs) = qsort smaller ++ [x] ++ qsort larger
'                where
'                   smaller = filter (<=x) xs
'                   larger = filter (>x) xs

Module Program

  Sub Main()
    Dim list = {3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5}.ToList
    Dim sortedList = QSort(list)
    For Each item In sortedList
      Console.Write(item & " ")
    Next
  End Sub

  Function QSort(Of T As IComparable)(list As IEnumerable(Of T)) As IEnumerable(Of T)
    If If(list?.Count, 0) <= 1 Then Return list
    Dim x = list(0), xs = list.Skip(1)
    Dim smaller = From y In xs Where y.CompareTo(x) <= 0
    Dim larger = From y In xs Where y.CompareTo(x) > 0
    Return QSort(smaller).Concat({x}).Concat(QSort(larger))
  End Function

End Module