Imports System

Module Program

  Sub Main() 'args As String())

    ConvertTxtToWiki()
    'ReviewRenameFiles()

  End Sub

  Private Sub ReviewRenameFiles()

    Dim folder = "C:\Users\dualb\Desktop\qb64-root\internal\help"

    Dim root = "C:\Users\dualb\Desktop\qb64-root\internal\help\markdown\Keyword-Reference---Alphabetical.md"
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

  Private Sub ConvertTxtToWiki()

    Dim folder = "C:\Users\dualb\Desktop\qb64-root\internal\help"

    Dim files = IO.Directory.GetFiles(folder, "*.txt")
    'Dim files = {IO.Path.Combine(folder, "_ALLOWFULLSCREEN.txt")}

    For Each file In files 'IO.Directory.GetFiles(folder, "*.txt")

      Console.WriteLine($"Processing {IO.Path.GetFileNameWithoutExtension(file)}...")
      Console.WriteLine($"-------------------------------")

      Dim result = ""

      Dim text = IO.File.ReadAllText(file)

      text = text.Replace("&lt;p style=""text-align: center"">([[#toc|Return to Table of Contents]])&lt;/p>", "")
      text = text.Replace("&lt;p style=""text-align: center"">([[#toc|Return to FAQ topics]])&lt;/p>", "")
      text = text.Replace("[[#toc|Return to Top]]", "")

      text = text.Replace(vbCrLf, vbLf)
      text = text.Replace(vbCr, vbLf)
      text = text.Replace("&lt;", "<")
      text = text.Replace("&amp;", "&")

      text = text.Replace("`", "\`")
      text = text.Replace("'' ''", "")
      text = text.Replace("''' '''", "")
      text = text.Replace("'''", "**")
      text = text.Replace("''", "*")

      text = text.Replace("<code>", "")
      text = text.Replace("</code>", "")
      text = text.Replace("<center>", "")
      text = text.Replace("</center>", "")
      text = text.Replace("<sup>", "")
      text = text.Replace("</sup>", "")

      text = text.Replace("{{PageExamples]]", "{{PageExamples}}")

      text = text.Replace("*See also:*", "{{PageSeeAlso}}")

      Dim lines = text.Split(vbLf)

      Dim previousLineBlank = False

      For Each line In lines

        line = ReplaceLinks(line)

        Do
          If line.EndsWith(" ") Then
            line = line.Substring(0, line.Length - 1)
          Else
            Exit Do
          End If
        Loop

        If line.StartsWith("#") Then
          ' Metacommand???
          Dim mc = line.Substring(0, line.IndexOf(" "c))
          line = line.Substring(mc.Length)
          If mc.StartsWith("#REDIRECT") Then
            '#REDIRECT [$IF]($IF)
            line = $"See{line}"
          End If
        End If

        ' Fix line starts...
        If line.StartsWith("::*") Then
          line = "  * " & line.Substring(3).Trim
        ElseIf line.StartsWith(":::::") Then
          line = "> " & line.Substring(5).Trim
        ElseIf line.StartsWith("::::") Then
          line = "> " & line.Substring(4).Trim
        ElseIf line.StartsWith(":::") Then
          line = "> " & line.Substring(3).Trim
        ElseIf line.StartsWith("::") Then
          line = "> " & line.Substring(2).Trim
        ElseIf line.StartsWith(":") Then
          line = "> " & line.Substring(1).Trim
        ElseIf line.StartsWith("** ") Then
          line = "  * " & line.Substring(3).Trim
        ElseIf line.StartsWith("*[") Then
          line = "* [" & line.Substring(2).Trim
        End If

        If line.StartsWith("{{DISPLAYTITLE:") Then
          Continue For
        ElseIf String.IsNullOrWhiteSpace(line) AndAlso previousLineBlank Then
          Continue For
        Else

          ' Parse Curly

          Dim abort = False
          Dim index = 0
          Do

            If abort OrElse index > line.Length - 1 Then Exit Do

            If line(index) = "{" Then
              If line(index + 1) = "{" Then
                Dim index2 = index + 2
                If line(index2) = "{" Then
                  index += 1
                  index2 += 1
                End If
                Do
                  If index2 > line.Length - 1 Then abort = True : index += 1 : Exit Do
                  If line(index2) = "}" Then
                    If line(index2 + 1) = "}" Then
                      Dim innerValue = line.Substring(index + 2, index2 - (index + 2))
                      index2 += 2
                      Dim outerValue = line.Substring(index, index2 - index)

                      Dim leftSide = If(index > 0, line.Substring(0, index), "")
                      Dim rightSide = line.Substring(index2)

                      Select Case innerValue

                        Case "DISPLAYTITLE:" : Continue For
                        Case "DataTypeTable" : abort = True : index += 1 : Exit Do  'TODO
                        Case "PrintUsing" : abort = True : index += 1 : Exit Do  'TODO
                        Case "Template:RelationalTable" : abort = True : index += 1 : Exit Do  'TODO
                        Case "Template:LogicalTruthTable" : abort = True : index += 1 : Exit Do  'TODO

                        Case "PageSyntax" : innerValue = $"## Syntax{vbLf}"
                        Case "PageParameters", "Parameters" : innerValue = $"## Parameters{vbLf}"
                        Case "PageDescription", "Description" : innerValue = $"## Description{vbLf}"
                        Case "PageAvailability" : innerValue = $"## Availability{vbLf}"
                        Case "PageErrors" : innerValue = $"## Errors{vbLf}"
                        Case "PageQBasic" : innerValue = $"## QBasic{vbLf}"
                        Case "PageLegacySupport" : innerValue = $"## Legacy Support{vbLf}"
                        Case "PageExamples" : innerValue = $"## Examples{vbLf}"

                        Case "CodeStart", "codeStart" : innerValue = $"{If(previousLineBlank, "", vbLf)}```vb{vbLf}"
                          If index2 + 1 < line.Length AndAlso line(index2 + 1) <> vbLf Then
                            innerValue &= vbLf
                          End If
                        Case "CodeEnd"
                          If index - 1 > -1 AndAlso line(index - 1) <> vbLf Then
                            innerValue = $"{vbLf}{vbLf}```{vbLf}"
                          Else
                            innerValue = $"{vbLf}```{vbLf}"
                          End If

                        Case "BlueStart", "OutputStart", "TextStart", "WhiteStart"
                          innerValue = $"{If(previousLineBlank, "", vbLf)}```text{vbLf}"
                          If index2 + 1 < line.Length AndAlso line(index2 + 1) <> vbLf Then
                            innerValue &= vbLf
                          End If
                        Case "BlueEnd", "OutputEnd", "TextEnd", "WhiteEnd"
                          If index - 1 > -1 AndAlso line(index - 1) <> vbLf Then
                            innerValue = $"{vbLf}{vbLf}```{vbLf}"
                          Else
                            innerValue = $"{vbLf}```{vbLf}"
                          End If

                        'Case "OutputStart" : innerValue = $"{If(previousLineBlank, "", vbLf)}```text{vbLf}"
                        '  If index2 + 1 < line.Length AndAlso line(index + 1) <> vbLf Then
                        '    innerValue &= vbLf
                        '  End If

                        'Case "OutputEnd"
                        '  If index - 1 > -1 AndAlso line(index - 1) <> vbLf Then
                        '    innerValue = $"{vbLf}{vbLf}```{vbLf}"
                        '  Else
                        '    innerValue = $"{vbLf}```{vbLf}"
                        '  End If

                        'Case "TextStart" : innerValue = $"{If(previousLineBlank, "", vbLf)}```text{vbLf}"
                        '  If index2 + 1 < line.Length AndAlso line(index + 1) <> vbLf Then
                        '    innerValue &= vbLf
                        '  End If

                        'Case "TextEnd"
                        '  If index - 1 > -1 AndAlso line(index - 1) <> vbLf Then
                        '    innerValue = $"{vbLf}{vbLf}```{vbLf}"
                        '  Else
                        '    innerValue = $"{vbLf}```{vbLf}"
                        '  End If

                        Case "InlineCode" : innerValue = "`"
                        Case "InlineCodeEnd" : innerValue = "`"

                        Case "PageSeeAlso" : innerValue = $"## See Also{vbLf}"
                        Case "PageNavigation" : innerValue = $""
                        Case Else
                          Dim values = innerValue.Split("|"c)
                          Select Case values.Length
                            Case 1
                              innerValue = $"||{values(0)}||"
                            Case 2
                              Select Case values(0)
                                Case "Cl", "cl", "Cb"
                                  innerValue = $"{values(1)}"
                                Case "KW"
                                  innerValue = $"{values(1)}"
                                Case "Parameter", "parameter"
                                  innerValue = $"{values(1)}"
                                Case "Text", "text"
                                  innerValue = $"{values(1)}"
                                Case "KW"
                                  innerValue = $"{values(1)}"
                                Case "WBG"
                                  innerValue = $"{values(1)}"
                                Case "small"
                                  innerValue = $"{If(previousLineBlank, "", vbLf)}{values(1)}{vbLf}"
                                Case Else
                                  Stop
                              End Select
                            Case 3
                              Select Case values(0)
                                Case "Cl", "Cb"
                                  innerValue = $"{values(2)}"
                                Case "KW"
                                  innerValue = $"{values(2)}"
                                Case "Text", "text"
                                  innerValue = $"{values(2)}"
                                Case "Parameter"
                                  innerValue = $"{values(2)}"
                                Case Else
                                  Stop
                              End Select
                            Case Else
                              Stop
                          End Select
                      End Select
                      line = $"{leftSide}{innerValue}{rightSide}"
                      index += innerValue.Length
                      Exit Do
                    End If
                  End If
                  index2 += 1
                Loop
                Continue Do
              Else
                index += 1
              End If
            Else
              index += 1
            End If

          Loop


          'Select Case line
          '  Case "{{PageSyntax}}" : line = $"## Syntax{vbLf}"
          '  Case "{{PageDescription}}" : line = $"## Description{vbLf}"
          '  Case "{{PageAvailability}}" : line = $"## Availability{vbLf}"
          '  Case "{{PageExamples}}" : line = $"## Examples{vbLf}"

          '  Case "{{CodeStart}}" : line = $"{If(previousLineBlank, "", vbLf)}```vb{vbLf}"
          '  Case "{{CodeEnd}}" : line = $"{If(previousLineBlank, "", vbLf)}```{vbLf}"

          '  Case "{{OutputStart}}" : line = $"{If(previousLineBlank, "", vbLf)}```text{vbLf}"
          '  Case "{{OutputEnd}}" : line = $"{If(previousLineBlank, "", vbLf)}```{vbLf}"

          '  Case "{{PageSeeAlso}}" : line = $"## See Also{vbLf}"
          '  Case "{{PageNavigation}}" : Continue For

          '  Case ""
          '    If previousLineBlank Then
          '      Continue For
          '    End If
          '  Case Else
          'End Select
        End If

        'line = ReplaceCurly(line, False)

        'Console.WriteLine(line)
        result &= line & vbLf

        If String.IsNullOrWhiteSpace(line) OrElse
           line.EndsWith(vbLf) Then
          previousLineBlank = True
        Else
          previousLineBlank = False
        End If

      Next

      For index = result.Length - 1 To 0 Step -1
        If result(index) <> vbLf Then
          result = result.Substring(0, index + 1) & vbLf
          Exit For
        End If
      Next

      Dim outputPath = IO.Path.Combine(folder, "markdown")

      If Not IO.Directory.Exists(outputPath) Then
        IO.Directory.CreateDirectory(outputPath)
      End If

      Dim filename = IO.Path.GetFileNameWithoutExtension(file)

      If Not String.IsNullOrWhiteSpace(filename) Then
        filename = FixFilename(filename)
        Dim outputFile = IO.Path.Combine(outputPath, $"{filename}.md")
        IO.File.WriteAllText(outputFile, result)
      End If

      '      Continue For

      '      text = text.Replace("'''", "**")
      '      text = text.Replace("''", "*")

      '      ' ** Remove

      '      Dim s = text.IndexOf("{{PageAvailability}}")
      '      If s > -1 Then
      '        Dim e = text.IndexOf("{{", s + 2)
      '        Dim l = text.Substring(0, s)
      '        Dim r = text.Substring(e)
      '        text = l & r
      '      End If

      '      ' ** Replacements

      '      text = text.Replace(vbLf & ":", $"{vbLf}> ")

      '      text = text.Replace("{{PageErrors}}", "## Error(s)
      '")
      '      text = text.Replace("{{PageSyntax}}", "## Syntax
      '")
      '      text = text.Replace("{{PageNotes}}", "## Notes
      '")
      '      text = text.Replace("{{PageUseWith}}", "## Use With
      '")
      '      text = text.Replace("{{PageDescription}}", "## Description
      '")
      '      text = text.Replace("{{PageExamples}}", "## Example(s)
      '")
      '      text = text.Replace("{{PageSeeAlso}}", "## See Also
      '")
      '      text = text.Replace("{{PageNavigation}}", "")
      '      text = text.Replace("{{PageCopyright}}", "")

      '      text = text.Replace("{{Parameters}}", "## Parameter(s)
      '")
      '      text = text.Replace("&lt;source lang=""qbasic"">", "
      '```vb
      '")
      '      text = text.Replace("&lt;/source>", "
      '```
      '")
      '      text = text.Replace("{{CodeStart}}", "
      '```vb
      '")
      '      text = text.Replace("{{CodeEnd}}", "
      '```
      '")
      '      text = text.Replace("{{OutputStart}}", "
      '```text

      '")
      '      text = text.Replace("{{OutputEnd}}", "
      '```
      '")
      '      text = text.Replace("{{TextStart}}", "
      '```text

      '")
      '      text = text.Replace("{{TextEnd}}", "
      '```
      '")

      '      text = text.Replace("{{WhiteStart}}", "
      '```text

      '")
      '      text = text.Replace("{{WhiteEnd}}", "
      '```
      '")

      '      text = text.Replace("{{LogicalTruthTable}}", "")

      '      text = text.Replace($"===={vbLf}", $"{vbLf}
      '")
      '      text = text.Replace($"==={vbLf}", $"{vbLf}
      '")
      '      text = text.Replace($"=={vbLf}", $"{vbLf}
      '")

      '      text = text.Replace("====", "#### ")
      '      text = text.Replace("===", "### ")
      '      text = text.Replace("==", "## ")

      '      Dim previousFilterStart = -1

      '      ' ** Change Links

      '      Do

      '        If text.Contains("[[") AndAlso text.Contains("]]") Then

      '          Dim st = text.IndexOf("[[")

      '          If previousFilterStart = st Then
      '            'ERROR
      '            Exit Do
      '          Else
      '            previousFilterStart = st
      '          End If

      '          Dim et = text.IndexOf("]]", st)
      '          Dim ln = (et + 2) - st

      '          Dim value = text.Substring(st + 2, ln - 4)

      '          If value.Contains("|"c) Then
      '            value = value.Substring(0, value.IndexOf("|"))
      '          End If

      '          Dim page = FixPageName(value)

      '          value = $"[{value}]({page})"

      '          'Console.WriteLine(value)

      '          Dim l = text.Substring(0, st)
      '          Dim r = text.Substring(et + 2)
      '          text = l & value & r

      '        Else
      '          Exit Do
      '        End If

      '      Loop

      '      ' ** Remove {{Cl|????}}, leaving ????.

      '      ' {{Cl|??????}}
      '      ' {{Parameter|??????}}

      '      Do

      '        If text.Contains("{{") AndAlso text.Contains("}}") Then

      '          Dim st = text.IndexOf("{{")
      '          Dim et = text.IndexOf("}}", st)
      '          Dim ln = (et + 2) - st

      '          Dim value = text.Substring(st + 2, ln - 4)

      '          Dim pre = ""

      '          If value.StartsWith("{") Then
      '            pre = "{"
      '            value = value.Substring(1)
      '          End If

      '          If value.Contains("|"c) Then
      '            Dim values = Split(value, "|"c)
      '            Select Case values(0)
      '              Case "code"
      '                value = values(1)
      '              Case "KW"
      '                value = $"[{values(1)}]({values(1)})"
      '              Case "text", "Text"
      '                value = values(1)
      '              Case "Cl", "cl", "Cb"
      '                If values.Count = 3 Then
      '                  value = values(2)
      '                Else
      '                  value = values(1)
      '                End If
      '              Case "Parameter", "parameter"
      '                value = values(1)
      '              Case "Ot"
      '                value = values(1)
      '              Case "small", "Small"
      '                value = $"{vbLf}{values(1)}"
      '              Case Else
      '                'Stop
      '            End Select
      '          End If

      '          'Console.WriteLine(value)

      '          Dim l = text.Substring(0, st)
      '          Dim r = text.Substring(et + 2)
      '          text = l & pre & value & r

      '        Else
      '          Exit Do
      '        End If

      '      Loop

      '      'Console.WriteLine(text)

      '      text = text.Replace(vbCrLf, vbLf)
      '      text = text.Replace(vbCr, vbLf)

      '      If text.StartsWith("#") Then
      '        ' Metacommand???
      '        Dim mc = text.Substring(0, text.IndexOf(vbLf) + 1)
      '        text = text.Substring(mc.Length)
      '        If mc.StartsWith("#REDIRECT ") Then
      '          '#REDIRECT [$IF]($IF)
      '          text = $"See {mc.Substring(10)}{vbLf}{text}"
      '        Else
      '          Stop
      '        End If
      '      End If

      '      text = text.Replace("centre", "center")
      '      text = text.Replace("##  ", "## ")
      '      text = text.Replace("> :: ", "> ")
      '      text = text.Replace(">  ", "> ")
      '      text = text.Replace("&amp;", "&")
      '      text = text.Replace(" " & vbLf, vbLf)
      '      Do
      '        If text.Contains(vbLf & vbLf & vbLf) Then
      '          text = text.Replace(vbLf & vbLf & vbLf, vbLf & vbLf)
      '        Else
      '          Exit Do
      '        End If
      '      Loop

      '      Dim outputPath = IO.Path.Combine(folder, "markdown")

      '      If Not IO.Directory.Exists(outputPath) Then
      '        IO.Directory.CreateDirectory(outputPath)
      '      End If

      '      Dim filename = IO.Path.GetFileNameWithoutExtension(file)

      '      If Not String.IsNullOrWhiteSpace(filename) Then
      '        filename = FixFilename(filename)
      '        Dim outputFile = IO.Path.Combine(outputPath, $"{filename}.md")
      '        IO.File.WriteAllText(outputFile, text)
      '      End If

    Next

  End Sub

  Private Function FixFilename(filename) As String
    If String.IsNullOrWhiteSpace(filename) Then Return filename
    Dim result As String = filename(0)
    For index = 1 To filename.length - 1
      Dim ch = filename(index)
      Select Case ch
        Case " "c : ch = "-"
        Case "_"c : ch = "-"
        Case Else
      End Select
      result &= ch
    Next
    Return result
  End Function

  Private Function ReplaceLinks(text As String) As String

    Dim previousFilterStart = -1

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

        Dim page = FixPageName(value)

        value = $"[{value}]({page})"

        Dim l = text.Substring(0, st)
        Dim r = text.Substring(et + 2)
        text = l & value & r

      Else
        Exit Do
      End If

    Loop

    Return text

  End Function

  Private Function ReplaceCurly(text As String, asLinks As Boolean) As String

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

        Dim isPage = False

        If value.Contains("|"c) Then
          Dim values = Split(value, "|"c)
          Select Case values(0)
            Case "code"
              value = values(1)
            Case "KW"
              value = $"[{values(1)}]({values(1)})"
            Case "text", "Text"
              value = values(1)
            Case "Cl", "cl"
              isPage = True
              If values.Count = 3 Then
                value = values(2)
              Else
                value = values(1)
              End If
            Case "Cb"
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
              value = $"{vbLf}{values(1)}"
            Case Else
              'Stop
          End Select
        End If

        Dim l = text.Substring(0, st)
        Dim r = text.Substring(et + 2)
        If isPage AndAlso asLinks Then
          text = l & pre & $"[{value}]({FixFilename(value)})" & r
        Else
          text = l & pre & value & r
        End If

      Else
        Exit Do
      End If

    Loop

    Return text

  End Function

  Private Function FixPageName(pageName As String) As String
    Dim result As String = pageName(0)
    For index = 1 To pageName.Length - 1
      Dim ch = pageName(index)
      Select Case ch
        Case " "c : ch = "-"
        Case "_"c : ch = "-"
        Case Else
      End Select
      result &= ch
    Next
    Return result
  End Function

End Module