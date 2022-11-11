Imports System

Module Program

  Sub Main() 'args As String())

    'ConvertTxtToWiki()
    'ReviewRenameFiles()

    If False Then
      PageLinkVerifier()
    Else

      'Dim entries As New List(Of (Search As String, Replace As String)) _
      '  From {("[_ALL](_ALL)", "[_ALL](_ALL)"),
      '        ("[_ARCCOT](_ARCCOT)", "[_ARCCOT](_ARCCOT)"),
      '        ("[_ARCCSC](_ARCCSC)", "[_ARCCSC](_ARCCSC)"),
      '        ("[_ARCSEC](_ARCSEC)", "[_ARCSEC](_ARCSEC)"),
      '        ("[_CLIPBOARD$ (function)](_CLIPBOARD$-(function))", "[_CLIPBOARD$ (function)](_CLIPBOARD$-(function))"),
      '        ("[_COT](_COT)", "[_COT](_COT)"),
      '        ("[_COTH](_COTH)", "[_COTH](_COTH)"),
      '        ("[_CSC](_CSC)", "[_CSC](_CSC)"),
      '        ("[_CSCH](_CSCH)", "[_CSCH](_CSCH)"),
      '        ("[_CVD](_CVD)", "[_CVD](_CVD)"),
      '        ("[_CVS](_CVS)", "[_CVS](_CVS)"),
      '        ("[_EXIT](_EXIT)", "[_EXIT](_EXIT)"),
      '        ("[_EXIT (function)](_EXIT (function))", "[_EXIT (function)](_EXIT (function))"),
      '        ("[_FILE$](_FILE$)", "[_FILE$](_FILE$)"),
      '        ("[_FILELIST$ (function)](_FILELIST$-(function))", "[_FILELIST$ (function)](_FILELIST$-(function))"),
      '        ("[_FONTWIDTH (function)](_FONTWIDTH (function))", "[_FONTWIDTH (function)](_FONTWIDTH-(function))"),
      '        ("[_GL_FLOAT](_GL-FLOAT)", "[_GL_FLOAT](_GL-FLOAT)"),
      '        ("[_LONG](_LONG)", "[_LONG](_LONG)"),
      '        ("[_MKD$](_MKD$)", "[_MKD$](_MKD$)"),
      '        ("[_MKS$](_MKS$)", "[_MKS$](_MKS$)"),
      '        ("[_OFF](_OFF)", "[_OFF](_OFF)"),
      '        ("[_PRINT USING](_PRINT-USING)", "[_PRINT USING](_PRINT-USING)"),
      '        ("[_SEC](_SEC)", "[_SEC](_SEC)"),
      '        ("[_SECH](_SECH)", "[_SECH](_SECH)"),
      '        ("[_SMOOTH](_SMOOTH)", "[_SMOOTH](_SMOOTH)"),
      '        ("[_SMOOTH (function)](_SMOOTH-(function))", "[_SMOOTH (function)](_SMOOTH-(function))"),
      '        ("[_SQUAREPIXELS](_SQUAREPIXELS)", "[_SQUAREPIXELS](_SQUAREPIXELS)"),
      '        ("[_STRETCH](_STRETCH)", "[_STRETCH](_STRETCH)"),
      '        ("[_TIMER](_TIMER)", "[_TIMER](_TIMER)"),
      '        ("[_WIDTH](_WIDTH)", "[_WIDTH](_WIDTH)"),
      '        ("[_UNSIGNED]([_UNSIGNED)", "[_UNSIGNED]([_UNSIGNED)"),
      '        ("[*](*)", "[*](*)"),
      '        ("[/](/)", "[/](/)"),
      '        ("[\](\)", "[\](\)"),
      '        ("[\\](\\)", "[\\](\\)"),
      '        ("[+](+)", "[+](+)"),
      '        ("[$END IF]($END IF)", "[$END IF]($END-IF)"),
      '        ("[ABSOLUTE](ABSOLUTE)", "[ABSOLUTE](ABSOLUTE)"),
      '        ("[ALPHA](ALPHA)", "[ALPHA](ALPHA)"),
      '        ("[ANY](ANY)", "[ANY](ANY)"),
      '        ("[ASC (statement)](ASC (statement))", "[ASC (statement)](ASC-(statement))"),
      '        ("[ASCII Table](ASCII Table)", "[ASCII](ASCII)"),
      '        ("[ASCII#Two Byte Codes](ASCII#Two Byte Codes)", "[ASCII](ASCII#Two Byte Codes)"),
      '        ("[ASCII#Two_Byte_Codes](ASCII#Two-Byte-Codes)", "[ASCII](ASCII)"),
      '        ("[AXIS](AXIS)", "[AXIS](AXIS)"),
      '        ("[BUTTON](BUTTON)", "[BUTTON](BUTTON)"),
      '        ("[C_Libraries#Console_Window](C-Libraries#Console-Window)", "[C Libraries](C-Libraries)"),
      '        ("[COMMON SHARED](COMMON_SHARED)", "[COMMON SHARED](COMMON-SHARED)"),
      '        ("[Connecting to printer via TCP/IP](Connecting-to-printer-via-TCP-IP)", "[Connecting to printer via TCP/IP](Connecting-to-printer-via-TCP-IP)"),
      '        ("[CONSOLE](CONSOLE)", "[CONSOLE](CONSOLE)"),
      '        ("[Creating Sprite Masks](Creating_Sprite_Masks)", "[Creating Sprite Masks](Creating_Sprite_Masks)"),
      '        ("[Creating_Icon_Bitmaps#Icon_to_Bitmap_Conversion_Function](Creating-Icon-Bitmaps#Icon-to-Bitmap-Conversion-Function)", "[Creating_Icon_Bitmaps#Icon_to_Bitmap_Conversion_Function](Creating-Icon-Bitmaps#Icon-to-Bitmap-Conversion-Function)"),
      '        ("[Creating Sprite Masks](Creating-Sprite Masks)", "[Creating Sprite Masks](Creating-Sprite Masks)"),
      '        ("[DATE$ (statement)](DATE$ (statement))", "[DATE$ (statement)](DATE$ (statement))"),
      '        ("[DEF SEG = 0](DEF SEG = 0)", "[DEF SEG = 0](DEF SEG = 0)"),
      '        ("[DEVICE$](DEVICE$)", "[DEVICE$](DEVICE$)"),
      '        ("[DEVICES](DEVICES)", "[DEVICES](DEVICES)"),
      '        ("[/](division)", "[/](division)"),
      '        ("[DO](DO)", "[DO](DO)"),
      '        ("[DTMF Phone Demo](DTMF-Phone-Demo)", "[DTMF Phone Demo](DTMF-Phone-Demo)"),
      '        ("[Email Demo](Email-Demo)", "[Email Demo](Email-Demo)"),
      '        ("[END FUNCTION](END-FUNCTION)", "[END FUNCTION](END-FUNCTION)"),
      '        ("[END IF](END-IF)", "[END IF](END-IF)"),
      '        ("[END SUB](END-SUB)", "[END SUB](END-SUB)"),
      '        ("[END TYPE](END-TYPE)", "[END TYPE](END-TYPE)"),
      '        ("[EXIT DO](EXIT DO)", "[EXIT DO](EXIT DO)"),
      '        ("[EXIT FOR](EXIT FOR)", "[EXIT FOR](EXIT FOR)"),
      '        ("[EXIT SUB](EXIT SUB)", "[EXIT SUB](EXIT SUB)"),
      '        ("[EXIT WHILE](EXIT-WHILE)", "[EXIT WHILE](EXIT-WHILE)"),
      '        ("[extended keys](extended keys)", "[extended keys](extended keys)"),
      '        ("[GET (TCP/IP statement)](GET-(TCP/IP-statement))", "[GET (TCP/IP statement)](GET-(TCP/IP-statement))"),
      '        ("[GET #](GET-#)", "[GET #](GET-#)"),
      '        ("[Grey Scale Bitmaps](Grey-Scale-Bitmaps)", "[Grey Scale Bitmaps](Grey-Scale-Bitmaps)"),
      '        ("[Illegal Function](Illegal Function)", "[Illegal Function](Illegal Function)"),
      '        ("[$INCLUDE](INCLUDE)", "[$INCLUDE](INCLUDE)"),
      '        ("[INKEY$#Two_Byte_Combinations](INKEY$#Two_Byte_Combinations)", "[INKEY$#Two_Byte_Combinations](INKEY$#Two_Byte_Combinations)"),
      '        ("[INPUT (file statement)](INPUT (file statement))", "[INPUT (file statement)](INPUT (file statement))"),
      '        ("[INPUT #](INPUT-#)", "[INPUT #](INPUT-#)"),
      '        ("[\](Integer-division)", "[\](Integer-division)"),
      '        ("[INTEGER64](INTEGER64)", "[INTEGER64](INTEGER64)"),
      '        ("[Inter-Program Data Sharing Demo](Inter-Program-Data-Sharing-Demo)", "[Inter-Program Data Sharing Demo](Inter-Program-Data-Sharing-Demo)"),
      '        ("[IS](IS)", "[IS](IS)"),
      '        ("[Keyword Reference - Alphabetical](Keyword-Reference---Alphabetical)", "[Keyword Reference - Alphabetical](Keyword-Reference---Alphabetical)"),
      '        ("[Keywords currently not supported by QB64](Keywords currently not supported by QB64)", "[Keywords currently not supported by QB64](Keywords currently not supported by QB64)"),
      '        ("[Keywords currently not supported by QB64](Keywords_currently_not_supported_by_QB64)", "[Keywords currently not supported by QB64](Keywords_currently_not_supported_by_QB64)"),
      '        ("[Keywords currently not supported](Keywords_currently_not_supported_by_QB64)", "[Keywords currently not supported](Keywords_currently_not_supported_by_QB64)"),
      '        ("[Libraries#C++_Variable_Types](Libraries#C++_Variable_Types)", "[Libraries#C++_Variable_Types](Libraries#C++_Variable_Types)"),
      '        ("[LINE INPUT (file statement)](LINE INPUT (file statement))", "[LINE INPUT (file statement)](LINE INPUT (file statement))"),
      '        ("[Line_number](Line_number)", "[Line_number](Line_number)"),
      '        ("[LOADFONT](LOADFONT)", "[LOADFONT](LOADFONT)"),
      '        ("[Mathematical_Operations#Derived_Mathematical_Functions](Mathematical-Operations#Derived-Mathematical-Functions)", "[Mathematical_Operations#Derived_Mathematical_Functions](Mathematical-Operations#Derived-Mathematical-Functions)"),
      '        ("[MEM](MEM)", "[MEM](MEM)"),
      '        ("[MEM (function)](MEM-(function))", "[MEM (function)](MEM-(function))"),
      '        ("[MID$ (statement)](MID$ (statement))", "[MID$ (statement)](MID$ (statement))"),
      '        ("[*](multiplication)", "[*](multiplication)"),
      '        ("[device description](NAME] [device description)", "[device description](NAME] [device description)"),
      '        ("[manufacturer name](NAME][manufacturer name)", "[manufacturer name](NAME][manufacturer name)"),
      '        ("[Microsoft Sidewinder Precision Pro (USB)](NAME][Microsoft Sidewinder Precision Pro (USB))", "[Microsoft Sidewinder Precision Pro (USB)](NAME][Microsoft Sidewinder Precision Pro (USB))"),
      '        ("[Microsoft Sidewinder Precision Pro (USB)](NAME][Microsoft-Sidewinder-Precision-Pro-(USB))", "[Microsoft Sidewinder Precision Pro (USB)](NAME][Microsoft-Sidewinder-Precision-Pro-(USB))"),
      '        ("[OFFSET](OFFSET)", "[OFFSET](OFFSET)"),
      '        ("[ON ERROR](ON ERROR)", "[ON ERROR](ON ERROR)"),
      '        ("[ON UEVENT](ON UEVENT)", "[ON UEVENT](ON UEVENT)"),
      '        ("[ON COM (n)](ON-COM-(n))", "[ON COM (n)](ON-COM-(n))"),
      '        ("[ON PLAY (n)](ON-PLAY-(n))", "[ON PLAY (n)](ON-PLAY-(n))"),
      '        ("[OPEN COM](OPEN COM)", "[OPEN COM](OPEN COM)"),
      '        ("[OPEN#File_Access_Modes](OPEN#File-Access-Modes)", "[OPEN#File_Access_Modes](OPEN#File-Access-Modes)"),
      '        ("[OPENCLIENT](OPENCLIENT)", "[OPENCLIENT](OPENCLIENT)"),
      '        ("[OPTION BASE](OPTION BASE)", "[OPTION BASE](OPTION BASE)"),
      '        ("[OPTION BASE](OPTION_BASE)", "[OPTION BASE](OPTION_BASE)"),
      '        ("[PDS_(7.1)_Procedures#DIR.24](PDS-(7.1)-Procedures#DIR.24)", "[PDS_(7.1)_Procedures#DIR.24](PDS-(7.1)-Procedures#DIR.24)"),
      '        ("[PDS(7.1) Procedures](PDS(7.1)-Procedures)", "[PDS(7.1) Procedures](PDS(7.1)-Procedures)"),
      '        ("[PEN (statement)](PEN (statement))", "[PEN (statement)](PEN (statement))"),
      '        ("[PRINT USING](PRINT USING)", "[PRINT USING](PRINT USING)"),
      '        ("[QB64 FAQ](QB64_FAQ)", "[QB64 FAQ](QB64_FAQ)"),
      '        ("[question mark](question mark)", "[question mark](question mark)"),
      '        ("[Resource_Table_extraction#Extract_Icon](Resource-Table-extraction#Extract_Icon)", "[Resource_Table_extraction#Extract_Icon](Resource-Table-extraction#Extract_Icon)"),
      '        ("[RGB](RGB)", "[RGB](RGB)"),
      '        ("[RGB32](RGB32)", "[RGB32](RGB32)"),
      '        ("[RGBA32](RGBA32)", "[RGBA32](RGBA32)"),
      '        ("[Screen Memory](Screen Memory)", "[Screen Memory](Screen Memory)"),
      '        ("[, border](SCREEN] (column1, row1)-(column2, row2)[, color][, border)", "[, border](SCREEN] (column1, row1)-(column2, row2)[, color][, border)"),
      '        ("[SDL Libraries](SDL-Libraries)", "[SDL Libraries](SDL-Libraries)"),
      '        ("[SELECT CASE](SELECT CASE)", "[SELECT CASE](SELECT CASE)"),
      '        ("[STRING](STIRNG)", "[STRING](STIRNG)"),
      '        ("[SUB _GL](SUB-_GL)", "[SUB _GL](SUB-_GL)"),
      '        ("[SysWOW64](SysWOW64)", "[SysWOW64](SysWOW64)"),
      '        ("[UNSIGNED](UNSIGNED)", "[UNSIGNED](UNSIGNED)"),
      '        ("[Using _OFFSET](Using--OFFSET)", "[Using _OFFSET](Using--OFFSET)"),
      '        ("[VB Script](VB-Script)", "[VB Script](VB-Script)"),
      '        ("[WGET](WGET)", "[WGET](WGET)"),
      '        ("[[WHEEL](WHEEL)", "[[WHEEL](WHEEL)"),
      '        ("[WHEEL](WHEEL)", "[WHEEL](WHEEL)"),
      '        ("[WRITE-(file-statement)](WRITE (file statement))", "[WRITE-(file-statement)](WRITE (file statement))"),
      '        ("[XOR (boolean)](XOR (boolean))", "[XOR (boolean)](XOR (boolean))")}

      Dim entries As New List(Of (Search As String, Replace As String)) _
        From {("[DEVICE$](DEVICE$)", "[_DEVICE$](_DEVICE$)"),
              ("[DEVICES](DEVICES)", "[_DEVICES](_DEVICES)")}

      For Each entry In entries
        SearchReplace(entry.Search, entry.Replace)
      Next

    End If

  End Sub

  Private Sub SearchReplace(match, replace)

    Dim root = "d:\github\qb64.wiki"

    For Each file In IO.Directory.GetFiles(root, "*.md")
      Dim contents = IO.File.ReadAllText(file)
      If contents.IndexOf(match) > -1 Then
        contents = contents.Replace(match, replace)
        IO.File.WriteAllText(file, contents)
      End If
    Next

  End Sub

  Private Sub PageLinkVerifier()

    Dim root = "d:\github\qb64.wiki"

    Dim contents As List(Of String) = Nothing
    If IO.File.Exists(IO.Path.Combine(root, "Keyword-Reference---Alphabetical.md")) Then
      contents = IO.File.ReadAllLines(IO.Path.Combine(root, "Keyword-Reference---Alphabetical.md")).ToList
    ElseIf IO.File.Exists(IO.Path.Combine(root, "Keyword-Reference-(Alphabetical).md")) Then
      contents = IO.File.ReadAllLines(IO.Path.Combine(root, "Keyword-Reference-(Alphabetical).md")).ToList
    End If

    Dim scanned As New List(Of String)
    Dim missing As New List(Of String)
    Dim allPages As New List(Of (DisplayName As String, PageName As String))

    If contents IsNot Nothing Then
      For Each line In contents
        'Console.WriteLine(line)
        Dim pages = ExtractLinks(line)
        If pages?.Count > 0 Then
          'If pages.Count > 0 Then
          '  Console.Write(pages.Count) : Console.Write(" - ") : Console.WriteLine(line)
          'End If
          For Each page In pages
            If Not allPages.Contains((page.DisplayName, page.PageName)) Then allPages.Add((page.DisplayName, page.PageName))
          Next
        End If
      Next
    End If

    Do
      Dim temp = (From p In allPages Where Not scanned.Contains(p.PageName) AndAlso Not missing.Contains(p.PageName) Select p.PageName)
      Dim pages = temp.ToList
      If pages.Count = 0 Then Exit Do
      For Each page In pages
        Dim filename = IO.Path.Combine(root, $"{page}.md")
        If IO.File.Exists(filename) Then
          contents = IO.File.ReadAllLines(filename).ToList
          For Each line In contents
            Dim results = ExtractLinks(line)
            If results?.Count > 0 Then
              For Each result In results
                If Not allPages.Contains((result.DisplayName, result.PageName)) Then allPages.Add((result.DisplayName, result.PageName))
              Next
            End If
          Next
          scanned.Add(page)
        Else
          missing.Add(page)
        End If
      Next
    Loop

    For Each page In (From p In allPages Order By p.PageName Select p)
      If Not IO.File.Exists(IO.Path.Combine(root, $"{page.PageName}.md")) Then
        Console.WriteLine($"[{page.DisplayName}]({page.PageName})")
        'Console.WriteLine($"(""[{page.DisplayName}]({page.PageName})"", ""[{page.DisplayName}]({page.PageName})""),")
      End If
    Next

    'For Each file In IO.Directory.GetFiles(root, "*.md")
    '  Console.WriteLine(file)
    'Next

  End Sub

  Private Function ExtractLinks(text As String) As List(Of (DisplayName As String, PageName As String))

    Dim result As List(Of (DisplayName As String, PageName As String)) = Nothing

    Dim index = 0
    Do

      Dim innerParenCount = 0

      If index > text.Length - 1 Then Exit Do

      If text(index) = "["c Then
        Dim openBracket = index
        Dim subIndex = index + 1
        Do
          If subIndex > text.Length - 1 Then Exit Do
          If text(subIndex) = "]"c Then
            If subIndex + 1 > text.Length - 1 Then Exit Do
            If text(subIndex + 1) = "(" Then
              Dim closeBracket = subIndex
              subIndex += 1
              Dim openParen = subIndex
              subIndex += 1 : innerParenCount = 0
              Do
                If subIndex > text.Length - 1 Then Exit Do
                If text(subIndex) = "(" Then
                  innerParenCount += 1
                End If
                If text(subIndex) = ")" Then
                  Dim wordStart, wordEnd As Integer
                  If innerParenCount = 0 Then
                    Dim closeParen = subIndex
                    wordStart = openBracket + 1 : wordEnd = closeBracket - 1
                    Dim name = text.Substring(wordStart, wordEnd - wordStart + 1)
                    wordStart = openParen + 1 : wordEnd = closeParen - 1
                    Dim page = text.Substring(wordStart, wordEnd - wordStart + 1)
                    If result Is Nothing Then result = New List(Of (DisplayName As String, PageName As String))
                    If page.StartsWith("https:") OrElse
                       page.StartsWith("http:") OrElse
                       page.StartsWith("ftp:") OrElse
                       page.StartsWith("_gl") OrElse
                       page.StartsWith("#") Then
                      ' skip
                    Else
                      result.Add((name, page))
                    End If
                    Exit Do
                  Else
                    innerParenCount -= 1
                  End If
                End If
                subIndex += 1
              Loop
            End If
            Exit Do
          End If
          subIndex += 1
        Loop
      End If

      index += 1

    Loop

    Return result

  End Function

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

      'Console.WriteLine($"Processing {IO.Path.GetFileNameWithoutExtension(file)}...")
      'Console.WriteLine($"-------------------------------")

      Dim result = ""

      Dim text = IO.File.ReadAllText(file)

      text = text.Replace("&lt;p style=""text-align: center"">([[#toc|Return to Table of Contents]])&lt;/p>", "")
      text = text.Replace("&lt;p style=""text-align: center"">([[#toc|Return to FAQ topics]])&lt;/p>", "")
      text = text.Replace("[[#toc|Return to Top]]", "")

      text = text.Replace(vbCrLf, vbLf)
      text = text.Replace(vbCr, vbLf)
      text = text.Replace("&lt;", "<")
      text = text.Replace("&amp;", "&")
      text = text.Replace("&nbsp;", " ")

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

      text = text.Replace($"{vbLf}{{{{PageExamples]]", $"{vbLf}{{{{PageExamples}}}}")

      text = text.Replace($"{vbLf}*See also:*", $"{vbLf}{{{{PageSeeAlso}}}}")
      text = text.Replace($"{vbLf}*Example:* ", $"{vbLf}")
      text = text.Replace($"{vbLf}*Example: ", $"{vbLf}")
      text = text.Replace($"{vbLf}Example: ", $"{vbLf}")
      text = text.Replace($"{vbLf}*Example 1:* ", $"{vbLf}")
      text = text.Replace($"{vbLf}*Example 2:* ", $"{vbLf}")
      text = text.Replace($"{vbLf}*Example 2: ", $"{vbLf}")
      text = text.Replace($"{vbLf}*Example 3:* ", $"{vbLf}")
      text = text.Replace($"{vbLf}*Example 4:* ", $"{vbLf}")
      text = text.Replace($"{vbLf}*Example 5:* ", $"{vbLf}")
      text = text.Replace($"{vbLf}*Example 6:* ", $"{vbLf}")
      text = text.Replace($"{vbLf}*Example 7:* ", $"{vbLf}")
      text = text.Replace($"{vbLf}*Example 8:* ", $"{vbLf}")
      text = text.Replace($"{vbLf}*Example 9:* ", $"{vbLf}")
      text = text.Replace($"{vbLf}{{{{Parameter|Example:* ", $"{vbLf}")

      'TODO: Parse for any tables and convert these first...

      If text.Contains("<table ") Then
        Dim tableStart = text.IndexOf("<table ")
        Dim tableEnd = text.IndexOf("</table>", tableStart) + 8
        Dim table = text.Substring(tableStart, tableEnd - tableStart)
        'Console.WriteLine(table)

        ' Get html tag...

        Dim translated = ""

        Dim index = 0
        Dim openAngle = 0
        Dim closeAngle = 0
        Do

          If index > table.Length - 1 Then Exit Do

          If table(index) = "<" Then
            openAngle = index
            Do
              If table(index) = ">" Then
                closeAngle = index + 1

                Dim tag = table.Substring(openAngle, closeAngle - openAngle)
                'Console.WriteLine(tag)

                If tag.StartsWith("<table ") Then
                ElseIf tag.StartsWith("<tr ") Then
                ElseIf tag.StartsWith("<td") Then
                  translated &= "| "
                ElseIf tag.StartsWith("<p") Then
                ElseIf tag.StartsWith("<span") Then
                ElseIf tag.StartsWith("</span") Then
                ElseIf tag.StartsWith("</p") Then
                ElseIf tag.StartsWith("</td") Then
                  translated &= " |"
                ElseIf tag.StartsWith("</tr") Then
                  translated &= vbLf
                ElseIf tag.StartsWith("</table") Then
                Else
                  Stop
                End If

                index += 1
                Exit Do

              Else
                index += 1
              End If
            Loop
          Else
            If table(index) <> vbLf Then
              translated &= table(index)
            End If
            index += 1
          End If
        Loop

        Dim leftSide = If(tableStart > 0, text.Substring(0, tableStart), "")
        Dim rightSide = text.Substring(tableEnd)

        text = $"{leftSide}{translated}{rightSide}"

        Console.WriteLine($"Processing {IO.Path.GetFileNameWithoutExtension(file)}...")

      End If

      Dim lines = text.Split(vbLf)

      Dim previousLineBlank = False

      For Each line In lines

        line = ReplaceLinks(line)

        line = line.Replace("[Keywords_currently_not_supported_by_QB64#Keywords_Not_Supported_in_Linux_or_MAC_OSX_versions](Keywords-currently-not-supported-by-QB64#Keywords-Not-Supported-in-Linux-or-MAC-OSX-versions)", "[Keywords currently not supported](Keywords-currently-not-supported-by-QB64)")
        line = line.Replace("[Windows_Libraries#Color_Dialog_Box](Windows-Libraries#Color-Dialog-Box)", "[Windows Libraries](Windows-Libraries)")
        line = line.Replace("[Windows_Libraries#Font_Dialog_Box](Windows-Libraries#Font-Dialog-Box)", "[Windows Libraries](Windows-Libraries)")
        line = line.Replace("[Keyboard_scancodes#INP_Scan_Codes](Keyboard-scancodes#INP-Scan-Codes)", "[Keyboard scancodes](Keyboard-scancodes)")
        line = line.Replace("[ASCII#Control_Characters](ASCII#Control-Characters)", "[ASCII](ASCII)")
        line = line.Replace("[PDS (7.1) Procedures#CURRENCY](PDS-(7.1)-Procedures#CURRENCY)", "[PDS (7.1) Procedures](PDS-(7.1)-Procedures)")

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
            line = $"See{line}."
          End If
        End If

        ' Fix line starts...
        If line.StartsWith(":* ") Then
          line = "  * " & line.Substring(3).Trim
        ElseIf line.StartsWith("::* ") Then
          line = "  * " & line.Substring(4).Trim
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

        If line = "===Details===" Then
          line = "### Details"
        ElseIf line = "===Alternative syntax===" Then
          line = $"### Alternative Syntax{vbLf}"
        ElseIf line = "*Usage:*" Then
          line = $"## Usage{vbLf}"
        ElseIf line = "== *SCREEN* Syntax ==" Then
          line = $"## *SCREEN* Syntax{vbLf}"
        ElseIf line = "== *File* Syntax ==" Then
          line = $"## *File* Syntax{vbLf}"
        ElseIf line = "See also:*" Then
          line = $"## See Also{vbLf}"
        ElseIf line.StartsWith("=== ") AndAlso line.EndsWith(" ===") Then
          line = "### " & line.Substring(4, line.Length - 8).Trim & vbLf
        ElseIf line.StartsWith("===") AndAlso line.EndsWith("===") Then
          line = "### " & line.Substring(3, line.Length - 6).Trim & vbLf
        ElseIf line.StartsWith("== ") AndAlso line.EndsWith(" ==") Then
          line = "## " & line.Substring(3, line.Length - 6).Trim & vbLf
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
                        Case "Template:RelationalTable"
                          'abort = True : index += 1 : Exit Do  'TODO

                          Dim relational = "**Relational Operators:**

| Symbol | Condition | Example Usage |
| -- | -- | -- |
| = | Equal | IF a = b THEN |
| <> | NOT equal | IF a <> b THEN |
| < | Less than | IF a < b THEN |
| > | Greater than | IF a > b THEN |
| <= | Less than or equal | IF a <= b THEN |
| >= | Greater than or equal | IF a >= b THEN |
"

                          innerValue = relational

                        Case "Template:LogicalTruthTable"
                          'abort = True : index += 1 : Exit Do  'TODO

                          Dim truth = "The results of the bitwise logical operations, where *A* and *B* are operands, and *T* and *F* indicate that a bit is set or not set:

| A | B |   | [NOT](NOT) B | A [AND](AND) B | A [OR](OR) B | A [XOR](XOR) B | A [EQV](EQV) B | A [IMP](IMP) B |
| - | - | - | - | - | - | - | - | - |
| T | T |   | F | T | T | F | T | T |
| T | F |   | T | F | T | T | F | F |
| F | T |   | F | F | T | T | F | T |
| F | F |   | T | F | F | F | T | T |

**[Relational Operations](Relational-Operations) return negative one (-1, all bits set) and zero (0, no bits set) for *true* and *false*, respectively.**
This allows relational tests to be inverted and combined using the bitwise logical operations.
"
                          innerValue = truth

                        Case "PageSyntax" : innerValue = $"## Syntax{vbLf}"
                        Case "PageParameters", "Parameters" : innerValue = $"## Parameter(s){vbLf}"
                        Case "PageDescription", "Description" : innerValue = $"## Description{vbLf}"
                        Case "PageAvailability" : innerValue = $"## Availability{vbLf}"
                        Case "PageErrors" : innerValue = $"## Error(s){vbLf}"
                        Case "PageQBasic" : innerValue = $"## QBasic{vbLf}"
                        Case "PageLegacySupport" : innerValue = $"## Legacy Support{vbLf}"
                        Case "PageExamples" : innerValue = $"## Example(s){vbLf}"

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
                                  innerValue = "" '$"{If(previousLineBlank, "", vbLf)}{values(1)}{vbLf}"
                                Case Else
                                  Stop
                              End Select
                            Case 3
                              Select Case values(0)
                                Case "Cl", "Cb"
                                  innerValue = $"{values(2)}"
                                Case "KW"
                                  innerValue = $"{values(2)}"
                                Case "Text", "text" ' parameter 3 is "color"
                                  innerValue = $"{values(1)}"
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
        Case "/" : ch = "-"
        Case Else
      End Select
      result &= ch
    Next
    Return result
  End Function

End Module