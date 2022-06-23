Option Explicit Off
Option Infer Off
Option Strict Off

Module Program


  Sub Main(args As String())
    'Console.WriteLine("Hello World!")
  End Sub

  '  Sub WikiParse(a$)
  '    'PRINT "Parsing...": _DISPLAY

  '    'wiki page interpret

  '    'clear info
  '    help_h = 0 : help_w = 0 : Help_Line$ = "" : Help_Link$ = "" : Help_LinkN = 0
  '    Help_Txt$ = Space(1000000)
  '    Help_Txt_Len = 0

  '    Help_Pos = 1 : Help_Wrap_Pos = 0
  '    Help_Line$ = MKL$(1)
  '    Help_LockWrap = 0
  '    Help_Bold = 0 : Help_Italic = 0
  '    Help_Underline = 0
  '    Help_BG_Col = 0

  '    link = 0 : elink = 0 : cb = 0

  '    col = Help_Col

  '    ' (OLD) Syntax Notes:
  '    ' '''=bold
  '    ' ''=italic
  '    ' {{macroname|macroparam}} or simply {{macroname}}
  '    '  eg. {{KW|PRINT}}=a key word, a link to a page
  '    '      {{Cl|PRINT}}=a key word in a code example, will be printed in bold and aqua
  '    '      {{Parameter|expression}}=a parameter, in italics
  '    '      {{PageSyntax}} {{PageParameters}} {{PageDescription}} {{PageExamples}}
  '    '      {{CodeStart}} {{CodeEnd}} {{OutputStart}} {{OutputEnd}}
  '    '      {{PageSeeAlso}} {{PageNavigation}} {{PageLegacySupport}}
  '    '      {{PageQBasic}} {{PageAvailability}}
  '    ' [[SPACE$]]=a link to wikipage called "SPACE$"
  '    ' [[INTEGER|integer]]=a link, link's name is on left and text to appear is on right
  '    ' *=a dot point
  '    ' **=a sub(ie. further indented) dot point
  '    ' &quot;=a quotation mark
  '    ' :=indent (if beginning a new line)
  '    ' CHR$(10)=new line character

  '    ' (NEW) Syntax Notes:
  '    ' **=bold
  '    ' *=italic
  '    ' {{macroname|macroparam}} or simply {{macroname}}
  '    '  eg. {{KW|PRINT}}=a key word, a link to a page
  '    '      {{Cl|PRINT}}=a key word in a code example, will be printed in bold and aqua
  '    '      {{Parameter|expression}}=a parameter, in italics
  '    '      {{PageSyntax}} {{PageParameters}} {{PageDescription}} {{PageExamples}}
  '    '      {{CodeStart}} {{CodeEnd}} {{OutputStart}} {{OutputEnd}}
  '    '      {{PageSeeAlso}} {{PageNavigation}} {{PageLegacySupport}}
  '    '      {{PageQBasic}} {{PageAvailability}}
  '    ' [SPACE$](url) = a link to wikipage called "SPACE$"
  '    ' [INTEGER](integer) = a link, link's name is on left and text to appear is on right
  '    ' * = a dot point
  '    ' - = a sub(ie. further indented) dot point
  '    ' &quot; = a quotation mark
  '    ' : = indent (if beginning a new line)
  '    ' CHR$(10) = new line character

  '    prefetch = 16
  '    Dim c$(prefetch)
  '    For ii = 1 To prefetch
  '      c$(ii) = Space(ii)
  '    Next

  '    i = InStr(a$, "<span ")
  '    Do While i
  '      a$ = Left(a$, i - 1) + Mid(a$, InStr(i + 1, a$, ">") + 1)
  '      i = InStr(a$, "<span ")
  '    Loop

  '    n = Len(a$)
  '    i = 1
  '    Do While i <= n

  '      c = Asc(a$, i) : c$ = Chr(c)
  '      For i1 = 1 To prefetch
  '        ii = i
  '        For i2 = 1 To i1
  '          If ii <= n Then
  '            Asc(c$(i1), i2) = Asc(a$, ii)
  '          Else
  '            Asc(c$(i1), i2) = 32
  '          End If
  '          ii = ii + 1
  '        Next
  '      Next

  '      If c = 38 Then ' "&"
  '        s$ = "&quot;"
  '        If c$(Len(s$)) = s$ Then
  '          i = i + Len(s$) - 1
  '          c$ = Chr(34) : c = Asc(c$)
  '          GoTo SpecialChr
  '        End If

  '        s$ = "&amp;"
  '        If c$(Len(s$)) = s$ Then
  '          i = i + Len(s$) - 1
  '          c$ = "&" : c = Asc(c$)
  '          GoTo SpecialChr
  '        End If

  '        s$ = "&gt;"
  '        If c$(Len(s$)) = s$ Then
  '          i = i + Len(s$) - 1
  '          c$ = ">" : c = Asc(c$)
  '          GoTo SpecialChr
  '        End If

  '        If c$(2) = Chr(194) + Chr(160) Then 'some kind of white-space formatting unicode combo
  '          i = i + 1
  '          GoTo Special
  '        End If

  '        s$ = "&lt;code>" : If c$(Len(s$)) = s$ Then i = i + Len(s$) - 1 : GoTo Special
  '        s$ = "&lt;/code>" : If c$(Len(s$)) = s$ Then i = i + Len(s$) - 1 : GoTo Special

  '        s$ = "&lt;center>"
  '        If c$(Len(s$)) = s$ Then
  '          i = i + Len(s$) - 1
  '          GoTo Special
  '        End If

  '        s$ = "&lt;/center>"
  '        If c$(Len(s$)) = s$ Then
  '          i = i + Len(s$) - 1
  '          GoTo Special
  '        End If

  '        s$ = "&lt;nowiki>"
  '        If c$(Len(s$)) = s$ Then
  '          i = i + Len(s$) - 1
  '          GoTo Special
  '        End If

  '        s$ = "&lt;/nowiki>"
  '        If c$(Len(s$)) = s$ Then
  '          i = i + Len(s$) - 1
  '          GoTo Special
  '        End If


  '        s$ = "&lt;p style="
  '        If c$(Len(s$)) = s$ Then
  '          i = i + Len(s$) - 1
  '          For ii = i To Len(a$) - 1
  '            If Mid(a$, ii, 1) = ">" Then i = ii : Exit For
  '          Next
  '          GoTo Special
  '        End If

  '        s$ = "&lt;/p"
  '        If c$(Len(s$)) = s$ Then
  '          i = i + Len(s$) - 1
  '          For ii = i To Len(a$) - 1
  '            If Mid(a$, ii, 1) = ">" Then i = ii : Exit For
  '          Next
  '          GoTo Special
  '        End If

  '        s$ = "&lt;div"
  '        If c$(Len(s$)) = s$ Then
  '          i = i + Len(s$) - 1
  '          For ii = i To Len(a$) - 1
  '            If Mid(a$, ii, 9) = "&lt;/div>" Then i = ii + 8 : Exit For
  '          Next
  '          GoTo Special
  '        End If

  '        s$ = "&lt;"
  '        If c$(Len(s$)) = s$ Then
  '          i = i + Len(s$) - 1
  '          c$ = "<" : c = Asc(c$)
  '          GoTo SpecialChr
  '        End If
  'SpecialChr:
  '      End If 'c=38 '"&"

  '      'Links
  '      If c = 91 Then '"["
  '        If c$(2) = "[[" And link = 0 Then
  '          i = i + 1
  '          link = 1
  '          link$ = ""
  '          GoTo Special
  '        End If
  '      End If
  '      If link = 1 Then
  '        If c$(2) = "]]" Or c$(2) = "}}" Then
  '          i = i + 1
  '          link = 0
  '          Text = link$
  '          i2 = InStr(link$, "|")
  '          If i2 Then
  '            Text = Right(link$, Len(link$) - i2)
  '            link$ = Left(link$, i2 - 1)
  '          End If

  '          If InStr(link$, "#") Then 'local page links not supported yet
  '            Help_AddTxt Text, 8, 0
  '                    GoTo Special
  '          End If

  '          Help_LinkN = Help_LinkN + 1
  '          Help_Link$ = Help_Link$ + "PAGE:" + link$ + Help_Link_Sep$

  '          If Help_BG_Col = 0 Then
  '            Help_AddTxt Text, Help_Col_Link, Help_LinkN
  '                Else
  '            Help_AddTxt Text, Help_Col_Bold, Help_LinkN
  '                End If
  '          GoTo Special
  '        End If
  '        link$ = link$ + c$
  '        GoTo Special
  '      End If


  '      'External links
  '      If c = 91 Then '"["
  '        If c$(6) = "[http:" And elink = 0 Then
  '          elink = 2
  '          elink$ = ""
  '          GoTo Special
  '        End If
  '      End If
  '      If elink = 2 Then
  '        If c$ = " " Then
  '          elink = 1
  '          GoTo Special
  '        End If
  '        elink$ = elink$ + c$
  '        GoTo Special
  '      End If
  '      If elink >= 1 Then
  '        If c$ = "]" Then
  '          elink = 0
  '          elink$ = " " + elink$
  '          Help_LockWrap = 1 : Help_Wrap_Pos = 0
  '          Help_AddTxt elink$, 8, 0
  '                Help_LockWrap = 0
  '          elink$ = ""
  '          GoTo Special
  '        End If
  '      End If

  '      If c = 123 Then '"{"
  '        If c$(5) = "{{KW|" Then 'this is really a link!
  '          i = i + 4
  '          link = 1
  '          link$ = ""
  '          GoTo Special
  '        End If
  '        If c$(5) = "{{Cl|" Then 'this is really a link too (in code example)
  '          i = i + 4
  '          link = 1
  '          link$ = ""
  '          GoTo Special
  '        End If
  '        If c$(2) = "{{" Then
  '          i = i + 1
  '          cb = 1
  '          cb$ = ""
  '          GoTo Special
  '        End If
  '      End If

  '      If cb = 1 Then
  '        If c$ = "|" Or c$(2) = "}}" Then
  '          If c$(2) = "}}" Then i = i + 1
  '          cb = 0

  '          If cb$ = "PageSyntax" Then Help_AddTxt "Syntax:" + Chr(13), Help_Col_Section, 0
  '                If cb$ = "PageParameters" Then Help_AddTxt "Parameters:" + Chr(13), Help_Col_Section, 0
  '                If cb$ = "PageDescription" Then Help_AddTxt "Description:" + Chr(13), Help_Col_Section, 0
  '                If cb$ = "PageAvailability" Then Help_AddTxt "Availability:" + Chr(13), Help_Col_Section, 0
  '                If cb$ = "PageExamples" Then Help_AddTxt "Code Examples:" + Chr(13), Help_Col_Section, 0
  '                If cb$ = "PageSeeAlso" Then Help_AddTxt "See also:" + Chr(13), Help_Col_Section, 0
  '                If cb$ = "PageLegacySupport" Then Help_AddTxt "Legacy support" + Chr(13), Help_Col_Section, 0
  '                If cb$ = "PageQBasic" Then Help_AddTxt "QBasic/QuickBASIC" + Chr(13), Help_Col_Section, 0

  '                If cb$ = "CodeStart" Then
  '            Help_NewLine
  '            Help_BG_Col = 1
  '            'Skip non-meaningful content before section begins
  '            ws = 1
  '            For ii = i + 1 To Len(a$)
  '              If Asc(a$, ii) = 10 Then Exit For
  '              If Asc(a$, ii) <> 32 And Asc(a$, ii) <> 39 Then ws = 0
  '            Next
  '            If ws Then i = ii
  '          End If
  '          If cb$ = "CodeEnd" Then Help_BG_Col = 0
  '          If cb$ = "OutputStart" Then
  '            Help_NewLine
  '            Help_BG_Col = 2
  '            'Skip non-meaningful content before section begins
  '            ws = 1
  '            For ii = i + 1 To Len(a$)
  '              If Asc(a$, ii) = 10 Then Exit For
  '              If Asc(a$, ii) <> 32 And Asc(a$, ii) <> 39 Then ws = 0
  '            Next
  '            If ws Then i = ii
  '          End If
  '          If cb$ = "OutputEnd" Then Help_BG_Col = 0

  '          GoTo Special

  '        End If

  '        cb$ = cb$ + c$ 'reading maro name
  '        GoTo Special
  '      End If 'cb=1

  '      If c$(2) = "}}" Then 'probably the end of a text section of macro'd text
  '        i = i + 1
  '        GoTo Special
  '      End If



  '      If c$(4) = " == " Then
  '        i = i + 3
  '        Help_Underline = 1
  '        GoTo Special
  '      End If
  '      If c$(3) = "== " Then
  '        i = i + 2
  '        Help_Underline = 1
  '        GoTo Special
  '      End If
  '      If c$(3) = " ==" Then
  '        i = i + 2
  '        GoTo Special
  '      End If
  '      If c$(2) = "==" Then
  '        i = i + 1
  '        Help_Underline = 1
  '        GoTo Special
  '      End If


  '      If c$(3) = "'''" Then
  '        i = i + 2
  '        If Help_Bold = 0 Then Help_Bold = 1 Else Help_Bold = 0
  '        col = Help_Col
  '        GoTo Special
  '      End If

  '      If c$(2) = "''" Then
  '        i = i + 1
  '        If Help_Italic = 0 Then Help_Italic = 1 Else Help_Italic = 0
  '        col = Help_Col
  '        GoTo Special
  '      End If

  '      If nl = 1 Then

  '        If c$(3) = "** " Then
  '          i = i + 2
  '          Help_AddTxt "    " + Chr(254) + " ", col, 0
  '                Help_NewLineIndent = Help_NewLineIndent + 6
  '          GoTo Special
  '        End If
  '        If c$(2) = "* " Then
  '          i = i + 1
  '          Help_AddTxt Chr(254) + " ", col, 0
  '                Help_NewLineIndent = Help_NewLineIndent + 2
  '          GoTo Special
  '        End If
  '        If c$(2) = "**" Then
  '          i = i + 1
  '          Help_AddTxt "    " + Chr(254) + " ", col, 0
  '                Help_NewLineIndent = Help_NewLineIndent + 6
  '          GoTo Special
  '        End If
  '        If c$ = "*" Then
  '          Help_AddTxt Chr(254) + " ", col, 0
  '                Help_NewLineIndent = Help_NewLineIndent + 2
  '          GoTo Special
  '        End If

  '      End If

  '      s$ = "{|"
  '      If c$(Len(s$)) = s$ Then
  '        If Mid(a$, i, 20) = "{| class=" + Chr(34) + "wikitable" + Chr(34) Then
  '          ReDim tableRow(1 To 100) As String
  '                ReDim tableCol(1 To 100) As Integer
  '                Dim totalCols As Integer
  '          Dim totalRows As Integer
  '          Dim thisCol As Integer
  '          totalCols = 0 : totalRows = 0
  '          Do
  '            l$ = wikiGetLine$(a$, i)
  '            If l$ = "|}" Or i >= Len(a$) Then Exit Do
  '            If l$ = "|-" Then _CONTINUE

  '            m$ = ""
  '            If Left(l$, 2) = "! " Then m$ = "!!"
  '            If Left(l$, 2) = "| " Then m$ = "||"

  '            If Len(m$) Then
  '              'new row
  '              totalRows = totalRows + 1
  '              If totalRows > UBound(tableRow) Then
  '                ReDim _PRESERVE tableRow(1 TO UBOUND(tableRow) + 99) AS STRING
  '                        End If

  '              'columns
  '              j = 3
  '              thisCol = 0
  '              Do
  '                p$ = wikiGetUntil$(l$, j, m$)
  '                j = j + 1
  '                If Len(_TRIM$(p$)) Then
  '                  thisCol = thisCol + 1
  '                  If totalCols < thisCol Then totalCols = thisCol
  '                  If thisCol > UBound(tableCol) Then
  '                    ReDim _PRESERVE tableCol(1 TO UBOUND(tableCol) + 99) AS INTEGER
  '                                End If
  '                  If tableCol(thisCol) < Len(_TRIM$(p$)) + 2 Then tableCol(thisCol) = Len(_TRIM$(p$)) + 2
  '                  tableRow(totalRows) = tableRow(totalRows) + _TRIM$(p$) + Chr(0)
  '                End If
  '              Loop While j < Len(l$)
  '            End If
  '          Loop
  '          backupHelp_BG_Col = Help_BG_Col
  '          backupBold = Help_Bold
  '          Help_BG_Col = 2
  '          For printTable = 1 To totalRows
  '            If printTable = 1 Then
  '              Help_Bold = 1
  '            Else
  '              Help_Bold = 0
  '            End If
  '            col = Help_Col

  '            j = 1
  '            tableOutput$ = ""
  '            For checkCol = 1 To totalCols
  '              p$ = wikiGetUntil$(tableRow(printTable), j, Chr(0))
  '              p$ = StrReplace$(p$, "&lt;", "<")
  '              p$ = StrReplace$(p$, "&gt;", ">")
  '              p$ = StrReplace$(p$, Chr(194) + Chr(160), "")
  '              p$ = StrReplace$(p$, "&amp;", "&")
  '              p$ = StrReplace$(p$, Chr(226) + Chr(136) + Chr(146), "-")
  '              p$ = StrReplace$(p$, "<nowiki>", "")
  '              p$ = StrReplace$(p$, "</nowiki>", "")
  '              p$ = StrReplace$(p$, "<center>", "")
  '              p$ = StrReplace$(p$, "</center>", "")
  '              p$ = StrReplace$(p$, "</span>", "")

  '              thisCol$ = Space(tableCol(checkCol))
  '              MID$(thisCol$, 2) = p$
  '              tableOutput$ = tableOutput$ + thisCol$
  '            Next
  '            Help_AddTxt tableOutput$, col, 0
  '                    Help_AddTxt Chr(13), col, 0
  '                Next
  '          Help_BG_Col = backupHelp_BG_Col
  '          Help_Bold = backupBold
  '          Help_AddTxt Chr(13), col, 0
  '            Else
  '          i = i + 1
  '          For ii = i To Len(a$) - 1
  '            If Mid(a$, ii, 2) = "|}" Then i = ii + 1 : Exit For
  '          Next
  '        End If
  '        GoTo Special
  '      End If

  '      If c$(3) = Chr(226) + Chr(128) + Chr(166) Then '...
  '        i = i + 2
  '        Help_AddTxt "...", col, 0
  '            GoTo Special
  '      End If

  '      If c$ = Chr(226) Then 'UNICODE UTF8 extender, it's a very good bet the following 2 characters will be 2 bytes of UNICODE
  '        i = i + 2
  '        GoTo Special
  '      End If

  '      If c$ = ":" And nl = 1 Then
  '        Help_AddTxt "    ", col, 0
  '            Help_NewLineIndent = Help_NewLineIndent + 4
  '        i = i + 1 : GoTo special2
  '      End If

  '      s$ = "__NOTOC__" + Chr(10)
  '      If c$(Len(s$)) = s$ Then
  '        i = i + Len(s$) - 1
  '        GoTo Special
  '      End If
  '      s$ = "__NOTOC__"
  '      If c$(Len(s$)) = s$ Then
  '        i = i + Len(s$) - 1
  '        GoTo Special
  '      End If

  '      If c$(4) = "----" Then
  '        i = i + 3
  '        Help_AddTxt String(100, 196), 8, 0
  '            GoTo Special
  '      End If



  '      If c$ = Chr(10) Then
  '        Help_NewLineIndent = 0

  '        If Help_Txt_Len >= 8 Then
  '          If Asc(Help_Txt$, Help_Txt_Len - 3) = 13 And Asc(Help_Txt$, Help_Txt_Len - 7) = 13 Then GoTo skipdoubleblanks
  '        End If

  '        Help_AddTxt Chr(13), col, 0

  '            skipdoubleblanks:
  '        nl = 1
  '        i = i + 1 : GoTo special2
  '      End If

  '      Help_AddTxt Chr(c), col, 0

  '        Special:
  '      i = i + 1
  '      nl = 0
  'special2:
  '    Loop

  '    'Trim Help_Txt$
  '    Help_Txt$ = Left(Help_Txt$, Help_Txt_Len) + Chr(13) 'chr13 stops reads past end of content

  '    'generate preview file
  '    'OPEN "help_preview.txt" FOR OUTPUT AS #1
  '    'FOR i = 1 TO LEN(Help_Txt$) STEP 4
  '    '    c = ASC(Help_Txt$, i)
  '    '    c$ = CHR$(c)
  '    '    IF c = 13 THEN c$ = CHR$(13) + CHR$(10)
  '    '    PRINT #1, c$;
  '    'NEXT
  '    'CLOSE #1

  '    'PRINT "Finished parsing!": _DISPLAY


  '    If Help_PageLoaded$ = "Keyword Reference (Alphabetical)" Then

  '      fh = FreeFile()
  '      OPEN "internal\help\links.bin" FOR OUTPUT AS #fh
  '        a$ = Space(1000)
  '      For cy = 1 To help_h
  '        'isolate and REVERSE select link
  '        l = CVL(Mid(Help_Line$, (cy - 1) * 4 + 1, 4))
  '        x = l
  '        x2 = 1
  '        c = Asc(Help_Txt$, x)
  '        oldlnk = 0
  '        lnkx1 = 0 : lnkx2 = 0
  '        Do Until c = 13
  '          Asc(a$, x2) = c
  '          lnk = CVI(Mid(Help_Txt$, x + 2, 2))
  '          If oldlnk = 0 And lnk <> 0 Then lnkx1 = x2
  '          If (lnk = 0 Or Asc(Help_Txt$, x + 4) = 13) And lnkx1 <> 0 Then
  '            lnkx2 = x2 : If lnk = 0 Then lnkx2 = lnkx2 - 1

  '            If lnkx1 <> 3 Then GoTo ignorelink
  '            If Asc(a$, 1) <> 254 Then GoTo ignorelink

  '            'retrieve lnk info
  '            lnk2 = lnk : If lnk2 = 0 Then lnk2 = oldlnk
  '            l1 = 1
  '            For lx = 1 To lnk2 - 1
  '              l1 = InStr(l1, Help_Link$, Help_Link_Sep$) + 1
  '            Next
  '            l2 = InStr(l1, Help_Link$, Help_Link_Sep$) - 1
  '            l$ = Mid(Help_Link$, l1, l2 - l1 + 1)
  '            'assume PAGE
  '            l$ = Right(l$, Len(l$) - 5)

  '            a2$ = Mid(a$, lnkx1, lnkx2 - lnkx1 + 1)

  '            If InStr(a2$, "(") Then a2$ = Left(a2$, InStr(a2$, "(") - 1)
  '            If InStr(a2$, " ") Then a2$ = Left(a2$, InStr(a2$, " ") - 1)
  '            If InStr(a2$, "...") Then
  '              a3$ = Right(a2$, Len(a2$) - InStr(a2$, "...") - 2)

  '              skip = 0

  '              If UCase(Left(a3$, 3)) <> "_GL" Then
  '                For ci = 1 To Len(a3$)
  '                  ca = Asc(a3$, ci)
  '                  If ca >= 97 And ca <= 122 Then skip = 1
  '                  If ca = 44 Then skip = 1
  '                Next
  '              End If

  '              If skip = 0 Then Print #fh, a3$ + "," + l$

  '                        a2$ = Left(a2$, InStr(a2$, "...") - 1)
  '            End If


  '            skip = 0
  '            If UCase(Left(a2$, 3)) <> "_GL" Then
  '              For ci = 1 To Len(a2$)
  '                ca = Asc(a2$, ci)
  '                If ca >= 97 And ca <= 122 Then skip = 1
  '                If ca = 44 Then skip = 1
  '              Next
  '            End If
  '            If skip = 0 Then Print #fh, a2$ + "," + l$
  '                    oa2$ = a2$

  '            a2$ = l$
  '            If InStr(a2$, "(") Then a2$ = Left(a2$, InStr(a2$, "(") - 1)
  '            If InStr(a2$, " ") Then a2$ = Left(a2$, InStr(a2$, " ") - 1)
  '            If InStr(a2$, "...") Then
  '              a3$ = Right(a2$, Len(a2$) - InStr(a2$, "...") - 2)

  '              skip = 0
  '              If UCase(Left(a3$, 3)) <> "_GL" Then
  '                For ci = 1 To Len(a3$)
  '                  ca = Asc(a3$, ci)
  '                  If ca >= 97 And ca <= 122 Then skip = 1
  '                  If ca = 44 Then skip = 1
  '                Next
  '              End If
  '              If skip = 0 Then Print #fh, a3$ + "," + l$

  '                        a2$ = Left(a2$, InStr(a2$, "...") - 1)
  '            End If

  '            skip = 0
  '            If UCase(Left(a2$, 3)) <> "_GL" Then
  '              For ci = 1 To Len(a2$)
  '                ca = Asc(a2$, ci)
  '                If ca >= 97 And ca <= 122 Then skip = 1
  '                If ca = 44 Then skip = 1
  '              Next
  '            End If
  '            If skip = 0 And a2$ <> oa2$ Then Print #fh, a2$ + "," + l$

  '                    ignorelink:

  '            lnkx1 = 0 : lnkx2 = 0
  '          End If
  '          x = x + 4 : c = Asc(Help_Txt$, x)
  '          x2 = x2 + 1
  '          oldlnk = lnk
  '        Loop
  '      Next
  '      CLOSE #fh

  '    End If





  '  End Sub

  '  Function wikiGetLine$(a$, i)
  '    wikiGetLine$ = wikiGetUntil(a$, i, Chr(10))
  '  End Function

  '  Function wikiGetUntil$(a$, i, separator$)
  '    If i >= Len(a$) Then Exit Function
  '    j = InStr(i, a$, separator$)
  '    If j = 0 Then
  '      wikiGetUntil$ = Mid(a$, i)
  '      i = Len(a$)
  '    Else
  '      wikiGetUntil$ = Mid(a$, i, j - i)
  '      i = j + 1
  '    End If
  '  End Function

  '  '*******************************************

  '  Private Function Asc(text As String, Optional position As Integer = 0) As Integer
  '    If position = 0 Then
  '      Return Microsoft.VisualBasic.Asc(text)
  '    Else
  '      Dim c = text.Substring(position - 1, 1)
  '      Return Microsoft.VisualBasic.Asc(c)
  '    End If
  '  End Function

End Module
