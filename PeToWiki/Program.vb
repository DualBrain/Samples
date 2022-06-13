Imports System

Module Program

  Sub Main(args As String())

    ReviewRenameFiles()

    'ConvertPeToWiki()

  End Sub

  Private Sub ReviewRenameFiles()

    Dim folder = "d:\github\qb64.wiki"

    Dim root = "d:\github\qb64.wiki\Keyword-Reference-(Alphabetical).md"
    Dim text = IO.File.ReadAllText(root)

    Dim index = 0

    Dim list As New List(Of String)

    Do

      Dim anchor = text.IndexOf("](", index)

      If anchor = -1 Then Exit Do

      Dim st = anchor
      Do
        If text(st) = "[" Then Exit Do
        st -= 1
      Loop

      Dim et = anchor + 2
      Dim c = 0
      Do
        If text(et) = ")" Then
          If c = 0 Then Exit Do
          c -= 1
        ElseIf text(et) = "(" Then
          c += 1
        End If
        et += 1
      Loop

      Dim value = text.Substring(st, (et - st) + 1)

      value = value.Substring(value.IndexOf("](") + 2)
      value = value.Substring(0, value.Length - 1)

      If value.Contains("#") Then
        ' skip
      ElseIf value.StartsWith("http") Then
        ' skip
      Else
        'Console.WriteLine(value)
        If Not list.Contains(value) Then list.Add(value)
      End If

      index = et + 1

    Loop

    'For Each file In IO.Directory.GetFiles(folder, "*.md")
    '  Dim name = IO.Path.GetFileName(file)
    '  If name.StartsWith("_") Then
    '    ' skip
    '  ElseIf name.Contains(" ") Then
    '    Console.WriteLine(name)
    '    'Dim newName = name.Replace(" ", "-")
    '    'IO.File.Move(file, IO.Path.Combine(IO.Path.GetDirectoryName(file), newName))
    '  End If
    'Next

    For Each file In IO.Directory.GetFiles(folder, "*.md")

      Dim name = IO.Path.GetFileNameWithoutExtension(file)

      If list.Contains(name) Then
        'Console.WriteLine($"            Matched: {name}")
      ElseIf list.Contains($"_{name}") Then
        ' need rename
        Dim path = IO.Path.GetDirectoryName(file)
        Dim filename = IO.Path.GetFileName(file)
        Dim newName = IO.Path.Combine(path, $"_{filename}")
        'Console.WriteLine(newName)
        IO.File.Move(file, newName)
      Else
        Dim found = False
        For Each entry In list
          If String.Compare(name, entry, True) = 0 Then
            'Console.WriteLine($"            Matched: '{name}' with '{entry}'")
            found = True
            Exit For
          End If
        Next
        For Each entry In list
          If String.Compare(name, entry.Replace("_", "-").Replace(" ", "-"), True) = 0 Then
            Console.WriteLine($"   Somewhat Matched: '{name}' with '{entry}'")
            found = True
            Exit For
          End If
        Next
        If Not found Then
          Console.WriteLine()
          Console.WriteLine(name)
          Console.WriteLine()
        End If
      End If
    Next

  End Sub

  Private Sub ConvertPeToWiki()

    Dim folder = "d:\qbpewiki2"

    For Each file In IO.Directory.GetFiles(folder, "*.txt")

      Console.WriteLine($"Processing {IO.Path.GetFileNameWithoutExtension(file)}...")

      Dim text = IO.File.ReadAllText(file)

      text = text.Replace("'''", "**")
      text = text.Replace("''", "*")

      ' ** Remove

      Dim s = text.IndexOf("{{PageAvailability}}")
      If s > -1 Then
        Dim e = text.IndexOf("{{", s + 2)
        Dim l = text.Substring(0, s)
        Dim r = text.Substring(e)
        text = l & r
      End If

      ' ** Replacements

      text = text.Replace(vbLf & ":", $"{vbLf}> ")

      text = text.Replace("{{PageErrors}}", "## Error(s)
")
      text = text.Replace("{{PageSyntax}}", "## Syntax
")
      text = text.Replace("{{PageNotes}}", "## Notes
")
      text = text.Replace("{{PageUseWith}}", "## Use With
")
      text = text.Replace("{{PageDescription}}", "## Description
")
      text = text.Replace("{{PageExamples}}", "## Example(s)
")
      text = text.Replace("{{PageSeeAlso}}", "## See Also
")
      text = text.Replace("{{PageNavigation}}", "")
      text = text.Replace("{{PageCopyright}}", "")

      text = text.Replace("{{Parameters}}", "## Parameter(s)
")
      text = text.Replace("{{CodeStart}}", "
```vb
")
      text = text.Replace("{{CodeEnd}}", "
```")
      text = text.Replace("{{OutputStart}}", "
```text

")
      text = text.Replace("{{OutputEnd}}", "
```
")
      text = text.Replace("{{TextStart}}", "
```text

")
      text = text.Replace("{{TextEnd}}", "
```
")

      text = text.Replace("{{WhiteStart}}", "
```text

")
      text = text.Replace("{{WhiteEnd}}", "
```
")

      text = text.Replace("{{LogicalTruthTable}}", "")

      text = text.Replace($"===={vbLf}", $"{vbLf}
")
      text = text.Replace($"==={vbLf}", $"{vbLf}
")
      text = text.Replace($"=={vbLf}", $"{vbLf}
")

      text = text.Replace("====", "#### ")
      text = text.Replace("===", "### ")
      text = text.Replace("==", "## ")

      Dim previousFilterStart = -1

      ' ** Change Links

      Do

        If text.Contains("[[") AndAlso text.Contains("]]") Then

          Dim st = text.IndexOf("[[")

          If previousFilterStart = st Then
            'ERROR
            Exit Do
          Else
            previousFilterStart = st
          End If

          Dim et = text.IndexOf("]]", st)
          Dim ln = (et + 2) - st

          Dim value = text.Substring(st + 2, ln - 4)

          If value.Contains("|"c) Then
            value = value.Substring(0, value.IndexOf("|"))
          End If

          value = $"[{value}]({value})"

          'Console.WriteLine(value)

          Dim l = text.Substring(0, st)
          Dim r = text.Substring(et + 2)
          text = l & value & r

        Else
          Exit Do
        End If

      Loop

      ' ** Remove {{Cl|????}}, leaving ????.

      ' {{Cl|??????}}
      ' {{Parameter|??????}}

      Do

        If text.Contains("{{") AndAlso text.Contains("}}") Then

          Dim st = text.IndexOf("{{")
          Dim et = text.IndexOf("}}", st)
          Dim ln = (et + 2) - st

          Dim value = text.Substring(st + 2, ln - 4)

          Dim pre = ""

          If value.StartsWith("{") Then
            pre = "{"
            value = value.Substring(1)
          End If

          If value.Contains("|"c) Then
            Dim values = Split(value, "|"c)
            Select Case values(0)
              Case "code"
                value = values(1)
              Case "KW"
                value = $"[{values(1)}]({values(1)})"
              Case "text", "Text"
                value = values(1)
              Case "Cl", "cl", "Cb"
                If values.Count = 3 Then
                  value = values(2)
                Else
                  value = values(1)
                End If
              Case "Parameter", "parameter"
                value = values(1)
              Case "Ot"
                value = values(1)
              Case "small", "Small"
                value = $"<sub>{values(1)}</sub>"
              Case Else
                'Stop
            End Select
          End If

          'Console.WriteLine(value)

          Dim l = text.Substring(0, st)
          Dim r = text.Substring(et + 2)
          text = l & pre & value & r

        Else
          Exit Do
        End If

      Loop

      'Console.WriteLine(text)

      Dim outputPath = IO.Path.Combine(folder, "markdown")

      If Not IO.Directory.Exists(outputPath) Then
        IO.Directory.CreateDirectory(outputPath)
      End If

      Dim filename = IO.Path.GetFileNameWithoutExtension(file)

      Dim outputFile = IO.Path.Combine(outputPath, $"{filename}.md")

      IO.File.WriteAllText(outputFile, text)

    Next

  End Sub

End Module