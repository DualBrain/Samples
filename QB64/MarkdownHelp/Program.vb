Imports System
'Imports Terminal.Gui
Imports System.Console
Imports System.ConsoleColor

Module Program

  Private m_lines As New List(Of String)

  Private m_index As New Dictionary(Of String, String)

  Private Enum MatchType
    Unknown
    Keyword
    Statement
    [Function]
    Metacommand
    [Operator]
    Document
  End Enum

  Private ReadOnly m_links As New Dictionary(Of String, String)
  Private ReadOnly m_alphaIndex As New Dictionary(Of Char, Integer)

  Sub Main() 'args As String())

    Dim csrLeft = 1
    Dim csrTop = 2

    If False Then

      Dim mdPath = "D:\github\qb64.wiki\Keyword-Reference-(Alphabetical).md"
      Dim content = IO.File.ReadAllText(mdPath)

      Dim st = SourceText.From(content)
      'Dim lexer = New Lexer(st)

      Dim lxr = New Lexer(st)
      Dim tokens = New List(Of SyntaxToken)
      Do
        Dim token = lxr.Lex
        If token.Kind = SyntaxKind.EndOfFileToken Then
          'root = New CompilationUnitSyntax(st, ImmutableArray(Of MemberSyntax).Empty, token)
        End If
        If token.Kind <> SyntaxKind.EndOfFileToken Then 'OrElse m_includeEndOfFile Then
          tokens.Add(token)
        End If
        If token.Kind = SyntaxKind.EndOfFileToken Then Exit Do
      Loop
      'd = l.Diagnostics.ToImmutableArray

      Dim prsr = New Parser(tokens)
      prsr.Parse()

      'For Each token In tokens
      '  Console.Write(token)
      'Next

      Return

    End If

    Dim keywords = {"ABSOLUTE", "ACCESS", "ANY", "APPEND", "AS",
                    "BASE", "BINARY",
                    "CASE",
                    "DOUBLE",
                    "ELSE", "ELSEIF", "ENVIRON$",
                    "INTEGER", "IS",
                    "LIST", "LONG", "LOOP",
                    "NEXT",
                    "OFF", "ON", "OUTPUT",
                    "RANDOM",
                    "SINGLE", "STEP",
                    "THEN", "TO",
                    "UNTIL", "USING",
                    "WEND"}
    Dim statements = {"BEEP", "BLOAD", "BSAVE",
                      "CALL", "CALL ABSOLUTE", "CHAIN", "CHDIR", "CIRCLE", "CLEAR", "CLOSE", "CLS", "COLOR", "COM", "COMMON", "CONST",
                      "DATA", "DATE$", "DECLARE", "DEF FN", "DEF SEG", "DEFDBL", "DEFINT", "DEFLNG", "DEFSNG", "DEFSTR", "DIM", "DO...LOOP", "DRAW",
                      "END", "ENVIRON", "ERASE", "ERROR", "EXIT",
                      "FIELD", "FILES", "FOR...NEXT", "FUNCTION",
                      "GET (File I/O)", "GET (Graphics)", "GOSUB", "GOTO",
                      "IF...THEN...ELSE", "INPUT", "IOCTL",
                      "KEY (Assignment)", "KEY (Event Trapping", "KILL",
                      "LET", "LINE (Graphics)", "LINE INPUT", "LOCATE", "LOCK...UNLOCK", "LPRINT", "LPRINT USING", "LSET",
                      "MID$", "MKDIR",
                      "NAME",
                      "ON COM", "ON ERROR", "ON KEY", "ON PEN", "ON PLAY", "ON STRIG", "ON TIMER", "ON...GOSUB", "ON...GOTO", "OPEN", "OPEN COM", "OPTION BASE", "OUT",
                      "PAINT", "PALETTE", "PCOPY", "PEN", "PLAY (Music)", "PLAY (Event Trapping)", "POKE", "PRESET", "PRINT", "PRINT USING", "PSET", "PUT (File I/O)", "PUT (Graphics)",
                      "RANDOMIZE", "READ", "REDIM", "REM", "RESET", "RESTORE", "RESUME", "RETURN", "RMDIR", "RSET", "RUN",
                      "SCREEN", "SEEK", "SELECT CASE", "SHARED", "SHELL", "SLEEP", "SOUND", "STATIC", "STOP", "STRIG", "SUB", "SWAP", "SYSTEM",
                      "TIME$", "TIMER", "TROFF", "TRON", "TYPE",
                      "UNLOCK",
                      "VIEW", "VIEW PRINT",
                      "WAIT", "WHILE...WEND", "WIDTH", "WINDOW", "WRITE"}
    Dim functions = {"ABS", "ASC", "ATN",
                     "CDBL", "CHR$", "CINT", "CLNG", "COS", "CSNG", "CSRLIN", "CVD", "CVDMBF", "CVI", "CVL", "CVS", "CVSMBF",
                     "DATE$",
                     "ENVIRON$", "ERDEV", "ERDEV$", "ERL", "ERR", "EXP",
                     "FILEATTR", "FIX", "FRE", "FREEFILE",
                     "HEX$",
                     "INKEY$", "INPUT$", "INSTR", "INT", "IOCTL$",
                     "LBOUND", "LCASE$", "LEFT$", "LEN", "LOC", "LOF", "LOG", "LPOS", "LTRIM$",
                     "MID$", "MKD$", "MKDMBF$", "MKI$", "MKL$", "MKS$", "MKSMBF$",
                     "OCT$",
                     "PEEK", "PEN", "PLAY", "PMAP", "POINT", "POS",
                     "RIGHT$", "RND", "RTRIM$",
                     "SCREEN", "SEEK", "SGN", "SIN", "SPACE$", "SPC", "SQR", "STEP", "STICK", "STR$", "STRIG", "STRING$",
                     "TAB", "TAN", "TIME$", "TIMER",
                     "UBOUND", "UCASE$",
                     "VAL", "VARPTR", "VARPTR$", "VARSEG"}
    Dim metacommands = {"$DYNAMIC", "$STATIC"}
    Dim operators = {"AND", "EQV", "IMP", "MOD", "NOT", "OR", "XOR"}
    Dim documents = {"Basic Character Set", "Boolean Operators", "Data Type Keywords"}

    If True Then

      Dim path = "D:\github\qb64.wiki\Keyword-Reference-(Alphabetical).md"
      Dim lines = IO.File.ReadAllLines(path)

      'For Each line In lines
      '  If line.StartsWith("* [") Then
      '    Dim text = line.Substring(3, line.IndexOf("]") - 3)
      '    Console.Write(text)
      '    Console.Write(" - ")
      '    Dim mdStart = line.IndexOf("](")
      '    If mdStart > -1 Then mdStart += 2
      '    Dim mdEnd = -1
      '    If mdStart > -1 Then
      '      ' scan...
      '      Dim mdIndex = 0
      '      Dim openParenCount = 0
      '      Do
      '        If line(mdStart + mdIndex) = "("c Then
      '          openParenCount += 1
      '        ElseIf line(mdStart + mdIndex) = ")"c Then
      '          If openParenCount > 0 Then
      '            openParenCount -= 1
      '          Else
      '            ' found closing.
      '            mdEnd = mdStart + mdIndex
      '            Exit Do
      '          End If
      '        End If
      '        mdIndex += 1
      '      Loop
      '    End If
      '    'Dim mdEnd = line.IndexOf(")", mdStart)
      '    If mdStart > -1 AndAlso mdEnd > -1 Then
      '      Dim md = line.Substring(mdStart, mdEnd - mdStart)
      '      Console.Write(md)
      '      ' Now try to determine is this a function, statement, what?

      '      Dim openParen = line.IndexOf("("c, mdEnd)

      '      If openParen > -1 AndAlso openParen - mdEnd < 4 Then
      '        Dim closeParen = line.IndexOf(")", openParen)
      '        Dim phrase = line.Substring(openParen + 1, (closeParen - (openParen + 1)))
      '        Select Case phrase
      '          Case "function", "statement", "file statement", "file mode", "file function", "logic operator"
      '          Case Else
      '            Console.WriteLine($" - {phrase}")
      '        End Select
      '      End If

      '    End If
      '      Console.WriteLine()
      '  End If
      'Next

      'Exit Sub

      m_lines.Add("  ◄Contents►  ◄Index►  ◄Back►")
      m_lines.Add(New String("─"c, 120))
      m_lines.Add("To get help on a QB keyword in the list below:")
      m_lines.Add("    1. Press the key of the first letter of the the keyword.")
      m_lines.Add("    2. Use the direction keys to move the cursor to the keyword.")
      m_lines.Add("    3. Press F1 to display the help text in the Help window.")

      Dim workingId = ""
      For Each line In lines
        If line.StartsWith("### ") Then
          workingId = line.Substring(4)?.Trim
          m_lines.Add($"{Space(65)}╔═{New String("═"c, workingId.Length)}═╗")
          m_lines.Add($"{New String("═"c, 65)}╣ {workingId} ║")
          If workingId.Length = 1 Then
            m_alphaIndex.Add(workingId, m_lines.Count - 1)
          End If
          m_lines.Add($"{Space(65)}╚═{New String("═"c, workingId.Length)}═╝")
        ElseIf line.StartsWith("* [") Then

          ' bullet entry.

          Dim category = ""

          ' Start with this...
          '* [ALIAS](ALIAS) (QB64 [DECLARE LIBRARY](DECLARE-LIBRARY) statement) denotes the actual name of an imported [FUNCTION](FUNCTION) or [SUB](SUB) procedure.
          ' Transform into...
          ' ALIAS Keyword
          ' Track "document name"?
          ' Track offset, length (for color).

          'Dim text = line.Substring(3)

          ' Determine text in the initial brackets...

          Dim offset = 3
          Dim parenCount = 0
          Dim accumulator = ""
          Do
            Dim ch = line(offset)
            Select Case ch
              'Case "["c
              Case "("c : parenCount += 1 : accumulator &= ch
              Case ")"c : parenCount -= 1 : accumulator &= ch
              Case "]"c
                If parenCount = 0 Then offset += 1 : Exit Do
              Case Else
                accumulator &= ch
            End Select
            offset += 1
          Loop

          If line(offset) = "("c Then

            Dim name = accumulator

            Select Case name
              Case "AS" : category = "Keyword"
              Case "CALL ABSOLUTE" : name = "ABSOLUTE"
              Case "AND (boolean)" : name = "AND" : category = "Operator"
              Case "ASC (statement)" : name = "ASC" : category = "Statement"
              Case "CASE ELSE", "CASE IS" : name = "CASE"
              Case "DATE$ (statement)" : name = "DATE$" : category = "Statement"
              Case "DECLARE LIBRARY", "DECLARE DYNAMIC LIBRARY" : name = "DECLARE"
              Case "FOR (file statement)" : name = "FOR" : category = "Statement"
              Case "FREE (QB64 TIMER statement)" : name = "FREE" : category = "Statement"
              Case "GET (TCP/IP statement)", "GET (graphics statement)" : name = "GET" : category = "Statement"
              Case "INPUT (file mode)", "INPUT (file statement)" : name = "INPUT" : category = "Statement"
              Case "LINE INPUT (file statement)" : name = "LINE INPUT" : category = "Statement"
              Case "MID$ (statement)" : name = "MID$" : category = "Statement"
              Case "OPTION BASE" : name = "OPTION"
              Case "OR (boolean)" : name = "OR" : category = "Operator"
              Case "PEN (statement)" : name = "PEN" : category = "Statement"
              Case "PRINT (file statement)" : name = "PRINT" : category = "Statement"
              Case "PRINT USING (file statement)" : name = "PRINT" : category = "Statement"
              Case "PUT (TCP/IP statement)", "PUT (graphics statement)" : name = "PUT" : category = "Statement"
              Case "SCREEN (function)" : name = "SCREEN" : category = "Function"
              Case "SEEK (statement)" : name = "SEEK" : category = "Statement"
              Case "SELECT CASE" : name = "SELECT"
              Case "SHELL (function)" : name = "SHELL" : category = "Function"
              Case "TIMER (statement)" : name = "TIMER" : category = "Statement"
              Case "WRITE (file statement)" : name = "WRITE" : category = "Statement"
              Case Else
            End Select

            offset += 1
            accumulator = ""

            Do
              Dim ch = line(offset)
              Select Case ch
                Case "("c : parenCount += 1 : accumulator &= ch
                Case ")"c
                  If parenCount = 0 Then
                    offset += 1
                    Exit Do
                  Else
                    parenCount -= 1 : accumulator &= ch
                  End If
                Case Else
                  accumulator &= ch
              End Select
              offset += 1
            Loop

            Dim document = accumulator

            Do
              If offset < line.Length AndAlso line(offset) = " "c Then
                offset += 1
              Else
                Exit Do
              End If
            Loop

            accumulator = ""

            If offset < line.Length AndAlso line(offset) = "(" Then

              offset += 1

              Do
                Dim ch = line(offset)
                Select Case ch
                  Case "("c : parenCount += 1 : accumulator &= ch
                  Case ")"c
                    If parenCount = 0 Then
                      offset += 1
                      Exit Do
                    Else
                      parenCount -= 1 : accumulator &= ch
                    End If
                  Case Else
                    accumulator &= ch
                End Select
                offset += 1
              Loop

            End If

            If String.IsNullOrWhiteSpace(category) AndAlso
               Not String.IsNullOrWhiteSpace(accumulator) Then
              If Not name.StartsWith("_") Then
                Select Case accumulator
                  Case "file mode" : category = "Keyword"
                  Case "[SELECT CASE](SELECT-CASE) condition", "condition" : category = "Condition"
                  Case "numerical type #" : category = "????"
                  Case "% numerical type" : category = "????"
                  Case "& numerical type" : category = "????"
                  Case "! numerical type" : category = "????"
                  Case "$ variable type" : category = "????"
                  Case "` numerical type" : category = "????"
                  Case "%% numerical type" : category = "????"
                  Case "numerical type ##" : category = "????"
                  Case "&& numerical type" : category = "????"
                  Case "variable type" : category = "????"
                  Case "floating decimal point" : category = "????"
                  Case "OS 2 event" : category = "????"
                  Case "procedure block" : category = "????"
                  Case "definition" : category = "????"
                  Case "[SHELL](SHELL) action" : category = "????"
                  Case "Pre-compiler directive", "QB64 C++ [Metacommand](Metacommand)", "QB64 [Metacommand](Metacommand)", "precompiler [metacommand](metacommand)", "Pre-Compiler [Metacommand](Metacommand)", "[Metacommand](Metacommand)", "metacommand", "[QB64 [Metacommand]]", "[Metacommand](Metacommand) - Deprecated" : category = "Metacommand"
                  Case Else
                    If accumulator.EndsWith("statement") Then
                      category = "Statement"
                    ElseIf accumulator.EndsWith("function") Then
                      category = "Function"
                    ElseIf accumulator.EndsWith("operator") Then
                      category = "Operator"
                    ElseIf accumulator.EndsWith("keyword") Then
                      category = "Keyword"
                    Else
                      category = accumulator
                    End If
                End Select
              End If
            End If

            Dim key = $"{name} {category}"
              If Not m_links.ContainsKey(key) Then
                m_links.Add(key, document)
              End If

              m_lines.Add($"  {name} ({category})")

            End If

            'Dim keyword = text.Substring(1, text.IndexOf("]"c) - 1)
            'If functions.Contains(keyword) Then
            '  keyword &= " Function"
            'ElseIf statements.Contains(keyword) Then
            '  keyword &= " Statement"
            'ElseIf keywords.Contains(keyword) Then
            '  keyword &= " Keyword"
            'ElseIf operators.Contains(keyword) Then
            '  keyword &= " Operator"
            'Else
            '  If keyword.Contains("(statement)") Then
            '    keyword = keyword.Replace("(statement)", "Statement")
            '  Else
            '    If text.Contains("(function)") Then
            '      keyword &= " Function"
            '    ElseIf text.Contains("statement)") Then
            '      keyword &= " Statement"
            '    Else
            '      'keyword &= " Keyword"
            '    End If
            '  End If
            'End If
            'm_lines.Add($"  {name}")

          End If
      Next

      'For alpha = 65 To 90

      '  m_lines.Add($"{Space(65)}╔═══╗")
      '  m_lines.Add($"{New String("═"c, 65)}╣ {ChrW(alpha)} ║")
      '  m_lines.Add($"{Space(65)}╚═══╝")

      '  Select Case alpha
      '    Case AscW("A"c)
      '      m_lines.Add("  ABS Function".PadRight(39) & "APPEND Keyword")
      '      m_lines.Add("  ABSOLUTE Keyword".PadRight(39) & "AS Keyword")
      '      m_lines.Add("  ACCESS Keyword".PadRight(39) & "ASC Keyword")
      '      m_lines.Add("  AND Operator".PadRight(39) & "ATN Keyword")
      '      m_lines.Add("  ANY Keyword")
      '    Case AscW("B"c)
      '      m_lines.Add("  BASE Keyword".PadRight(39) & "BLOAD Statement")
      '      m_lines.Add("  Basic Character Set".PadRight(39) & "Boolean Operators")
      '      m_lines.Add("  BEEP Statement".PadRight(39) & "BSAVE Statement")
      '      m_lines.Add("  BINARY Keyword")
      '    Case AscW("C"c)
      '      m_lines.Add("  CALL Statement".PadRight(39) & "COLOR Statement")
      '      m_lines.Add("  CALL ABSOLUTE Statement".PadRight(39) & "COM Statement")
      '      m_lines.Add("  CASE Keyword".PadRight(39) & "COMMON Statement")
      '      m_lines.Add("  CDBL Function".PadRight(39) & "CONST Statement")
      '      m_lines.Add("  CHAIN Statement".PadRight(39) & "COS Function")
      '      m_lines.Add("  CHDIR Statement".PadRight(39) & "CSNG Function")
      '      m_lines.Add("  CHR$ Function".PadRight(39) & "CSRLIN Function")
      '      m_lines.Add("  CINT Function".PadRight(39) & "CVD Function")
      '      m_lines.Add("  CIRCLE Statement".PadRight(39) & "CVDMBF Function")
      '      m_lines.Add("  CLEAR Statement".PadRight(39) & "CVI Function")
      '      m_lines.Add("  CLNG Function".PadRight(39) & "CVL Function")
      '      m_lines.Add("  CLOSE Statement".PadRight(39) & "CVS Function")
      '      m_lines.Add("  CLS Statement".PadRight(39) & "CVSMBF Function")
      '    Case Else
      '  End Select

      'Next

    Else

      Dim path = "D:\github\qb64.wiki\print.md"
      m_lines = IO.File.ReadAllLines(path).ToList

    End If

FullRedraw:

    Console.ForegroundColor = ConsoleColor.Gray
    Console.BackgroundColor = ConsoleColor.Black

    Dim w = Console.WindowWidth
    Dim h = Console.WindowHeight

    ' Main menu with File->Exit

    PrintEx(Space(w), 0, 0, Black, Gray)
    PrintEx("File", 1, 0, Black, Gray)

    ' ┌─┐│└┘├┤↑►◄ 

    Console.SetCursorPosition(0, 1)
    Console.Write("┌"c)
    Console.Write(New String("─"c, w - 2))
    Console.Write("┐"c)

    Console.SetCursorPosition(w - 4, 1)
    Console.Write("┤↑├")

    PrintEx("↑", w - 3, 1, Black, Gray)

    Dim title = " HELP: QB Local Help Index "
    Dim l = title.Length - 1
    Dim p = (w - l) \ 2

    'PrintEx(title, p, 1)
    PrintEx(title, p, 1, Black, Gray)

    '↓█░▒▓

    'PrintEx("◄", 3, 2, Green)
    'PrintEx("Contents", 4, 2)
    'PrintEx("►", 12, 2, Green)

    'PrintEx("◄", 15, 2, White)
    'PrintEx("Index", 16, 2, White)
    'PrintEx("►", 21, 2, White)

    'PrintEx("◄", 24, 2, Green)
    'PrintEx("Back", 25, 2)
    'PrintEx("►", 29, 2, Green)

    Dim index = 0

Redraw:

    If w <> Console.WindowWidth Then GoTo FullRedraw
    If h <> Console.WindowHeight Then GoTo FullRedraw

    Console.CursorVisible = False
    Try

      Dim top = 2
      Dim n = 0
      For l = index To index + h - 3
        'If top + n > h - 2 Then Exit For
        Dim text As String
        If l > m_lines.Count - 1 Then
          text = ""
        Else
          text = m_lines(l)
        End If
        If text.Length < w - 2 Then text &= Space((w - 2) - text.Length)
        If text.Length > w - 2 Then text = text.Substring(0, w - 2)

        'PrintEx(text, 1, top + n)
        Dim nav = False
        Dim isKeyword = False
        Dim inWord = False

        Dim clr = Gray

        For c = 0 To text.Length - 1

          Select Case text(c)
            Case "◄"c : clr = White : nav = True
            Case "►"c : clr = White : nav = False
            Case "╔"c, "═"c, "╗"c, "╚"c, "╝"c, "╣"c, "║"c, "─"c
              clr = DarkGray
            Case "_"c, "$"c
              If inWord Then
                ' do nothing
              Else
                Dim word = PeekWord(text, c)
                If word.Length > 1 Then inWord = True
                If metacommands.Contains(word) Then
                  isKeyword = True
                  clr = Blue
                End If
              End If
            Case "A"c To "Z"c
              If Not nav Then
                If Not inWord Then
                  inWord = True
                  Dim word = PeekWord(text, c)
                  If keywords.Contains(word) OrElse
                     statements.Contains(word) OrElse
                     functions.Contains(word) OrElse
                     operators.Contains(word) OrElse
                     metacommands.Contains(word) Then
                    isKeyword = True
                    clr = Blue
                  End If
                End If
              Else
                clr = ConsoleColor.Green
              End If
            Case "a"c To "z"c, "'"c, "?"c
            Case "."c
              If Not inWord Then
                inWord = False : isKeyword = False : nav = False
                clr = ConsoleColor.Gray
              End If
            Case ":"c, " "c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "("c, ")"c, "["c, "]"c, "/"c, "+"c, "&"c, "-"c, "*"c, "\"c, "^"c
              inWord = False : isKeyword = False : nav = False
              clr = ConsoleColor.Gray
            Case "<"c, ">"c, "`"c
            Case Else
              clr = ConsoleColor.Gray
          End Select
          PrintEx(text(c), 1 + c, top + n, clr)
        Next

        n += 1

      Next

      ' ║╚╝╣╗╔═

      For r = 2 To h - 2
        PrintEx("│", 0, r)
        'PrintEx("│", w - 1, r)
        If r = 2 Then
          PrintEx("↑", w - 1, r, Black, Gray)
        ElseIf r = h - 2 Then
          PrintEx("↓", w - 1, r, Black, Gray)
        Else
          'PrintEx("░", w - 1, r, Black, Gray)
          'PrintEx("▒", w - 1, r, Black, Gray)
          PrintEx("▓", w - 1, r, Black, Gray)
        End If
      Next

      Console.Write("└"c)
      Console.Write(New String("─"c, w - 2))
      Console.Write("┘"c)

    Finally
      Console.CursorVisible = True
      Console.SetCursorPosition(csrLeft, csrTop)
    End Try

    Do

      Try
        If w <> Console.WindowWidth Then GoTo FullRedraw
        If h <> Console.WindowHeight Then GoTo FullRedraw
      Catch
      End Try

      Try
        If Console.KeyAvailable Then
          Dim cki = Console.ReadKey(True)
          Select Case cki.Key

            Case ConsoleKey.PageUp
              index -= (h - 3) : If index < 0 Then index = 0
              GoTo Redraw
            Case ConsoleKey.PageDown
              index += (h - 3) : If index > m_lines.Count - 1 Then index = m_lines.Count - 1
              GoTo Redraw

            Case ConsoleKey.Home
              If cki.Modifiers = ConsoleModifiers.Control Then
                index = 0
                GoTo Redraw
              Else
                'TODO: what about empty lines?
                For i = 0 To m_lines(index + csrTop - 2).Length - 1
                  If m_lines(index + csrTop - 2)(i) <> " "c Then
                    csrLeft = 1 + i : Exit For
                  End If
                Next
                SetCursorPosition(csrLeft, csrTop)
              End If
            Case ConsoleKey.End
              If cki.Modifiers = ConsoleModifiers.Control Then
                index = m_lines.Count - 1
                GoTo Redraw
              Else
                'TODO: what about empty lines?
                For i = m_lines(index + csrTop - 2).Length - 1 To 0 Step -1
                  If m_lines(index + csrTop - 2)(i) <> " "c Then
                    csrLeft = 1 + (i + 1) : Exit For
                  End If
                Next
                SetCursorPosition(csrLeft, csrTop)
              End If
            Case ConsoleKey.RightArrow
              csrLeft += 1 : SetCursorPosition(csrLeft, csrTop)
            Case ConsoleKey.LeftArrow
              csrLeft -= 1 : SetCursorPosition(csrLeft, csrTop)
            Case ConsoleKey.UpArrow
              If csrTop = 2 OrElse cki.Modifiers = ConsoleModifiers.Control Then
                index -= 1 : If index < 0 Then index = 0
                If cki.Modifiers = ConsoleModifiers.Control Then csrTop += 1 : SetCursorPosition(csrLeft, csrTop)
                GoTo Redraw
              Else
                csrTop -= 1 : SetCursorPosition(csrLeft, csrTop)
              End If
            Case ConsoleKey.DownArrow
              If csrTop = h - 2 OrElse cki.Modifiers = ConsoleModifiers.Control Then
                index += 1 : If index > m_lines.Count - 1 Then index = m_lines.Count - 1
                If cki.Modifiers = ConsoleModifiers.Control Then csrTop -= 1 : SetCursorPosition(csrLeft, csrTop)
                GoTo Redraw
              Else
                csrTop += 1 : SetCursorPosition(csrLeft, csrTop)
              End If

            Case ConsoleKey.Tab

              If cki.Modifiers = ConsoleModifiers.Shift Then
                'TODO: Navigate to the previous keyword.
              Else
                'TODO: Navigate to the next keyword.
              End If

            Case ConsoleKey.F1, ConsoleKey.Enter

              'TODO: Navigate to the select keyword document "link".
              ' Note: This includes the "category".

              Dim line = m_lines(index + (csrTop - 2))
              Debug.WriteLine(line)

            Case ConsoleKey.A To ConsoleKey.Z

              'TODO: Jump to letter section.

              ' If cursor is already on a keyword starting with this letter, "tab" to the next word in the section.
              ' If on the last keyword in this section, loop to the first keyword.
              ' If section is partially off-screen, but currently one a keyword for this letter, "tab".
              ' If section is not visible (or partially visible) and cursor is not on a keyword for this letter, 
              '   scroll into view the first word of the section and set cursor accordingly.

              Dim key = ChrW(cki.Key)
              If m_alphaIndex.ContainsKey(key) Then
                index = m_alphaIndex(key)
                GoTo Redraw
              End If

            Case ConsoleKey.Escape
              Exit Do
            Case Else
          End Select
        End If
      Catch
        Exit Do
      End Try
    Loop

    ' Utilize the full console window.
    ' Allow (detect) resize.
    ' Support passing a "keyword".
    ' Single instance? Passing parameter?
    ' What else do we wish to do with the command line?
    ' Utilize mouse wheel to scroll?
    ' Escape to close?
    ' F5 to refresh?
    ' Standard navigation keys: PgUp, PgDn, Home, End, Up, Down, Left, Right, Tab?, Enter?
    ' Color coding?

  End Sub

  Private Function PeekWord(text As String, index As Integer) As String
    Dim word As String = ""
    For i = index To text.Length - 1
      Select Case text(i)
        Case " "c, ","
          Exit For
        Case Else
          word &= text(i)
      End Select
    Next
    Return word
  End Function

  Private Sub PrintEx(text As String, column As Integer, row As Integer, Optional foregroundColor As ConsoleColor? = Nothing, Optional backgroundColor As ConsoleColor? = Nothing)
    Dim cfg = Console.ForegroundColor
    Dim cbg = Console.BackgroundColor
    If foregroundColor IsNot Nothing Then Console.ForegroundColor = CType(foregroundColor, ConsoleColor)
    If backgroundColor IsNot Nothing Then Console.BackgroundColor = CType(backgroundColor, ConsoleColor)
    Console.SetCursorPosition(column, row)
    Console.Write(text)
    If foregroundColor IsNot Nothing Then Console.ForegroundColor = cfg
    If backgroundColor IsNot Nothing Then Console.BackgroundColor = cbg
  End Sub

End Module

Friend Class Parser

  Private m_position As Integer
  Private m_tokens As List(Of SyntaxToken)

  Public Sub New(tokens As List(Of SyntaxToken))
    m_tokens = tokens
  End Sub

  Public Sub Parse()

    Do

      If Current.Kind = SyntaxKind.EndOfFileToken Then Exit Do

      Select Case Current.Kind
        Case SyntaxKind.PoundToken
          Dim count = 1 : NextToken()
          Do
            If Current.Kind = SyntaxKind.PoundToken Then
              count += 1 : NextToken()
            Else
              Exit Do
            End If
          Loop
          Console.WriteLine($"# count = {count}")
        Case SyntaxKind.StarToken
          If Peek(1).Kind = SyntaxKind.WhiteSpace Then
            NextToken() : NextToken()
            Console.Write("Bullet")
            If Current.Kind = SyntaxKind.OpenBracketToken Then
              Console.Write(" - with possible URL")
              If Peek(1).Kind = SyntaxKind.IdentifierToken Then
                If Peek(2).Kind = SyntaxKind.CloseBracketToken Then
                  If Peek(3).Kind = SyntaxKind.OpenParenToken Then
                    If Peek(4).Kind = SyntaxKind.IdentifierToken Then
                      If Peek(5).Kind = SyntaxKind.CloseParenToken Then
                        Console.Write(" --- it is.")
                      End If
                    End If
                  End If
                End If
              End If
            End If
            Console.WriteLine()
          End If
        Case SyntaxKind.MinusToken
        Case Else
      End Select

      ' seek to the end of the line...
      Do
        If Current.Kind = SyntaxKind.LineBreak Then
          NextToken() : Exit Do
        ElseIf Current.Kind = SyntaxKind.EndOfFileToken Then
          Exit Do
        Else
          NextToken()
        End If
      Loop

    Loop

  End Sub

  Private Function Peek(offset As Integer) As SyntaxToken
    Dim index = m_position + offset
    If index >= m_tokens.Count Then
      Return m_tokens(m_tokens.Count - 1)
    End If
    Return m_tokens(index)
  End Function

  Private Function Current() As SyntaxToken
    Return Peek(0)
  End Function

  Private Function NextToken() As SyntaxToken
    Dim current = Me.Current
    m_position += 1
    Return current
  End Function

  'Private Function MatchToken(kind As SyntaxKind) As SyntaxToken
  '  If Current.Kind = kind Then
  '    Return NextToken()
  '  End If
  '  'm_diagnostics.ReportUnexpectedToken(Current.Location, Current.Kind, kind)
  '  Return New SyntaxToken(m_syntaxTree, kind, Current.Position, Nothing, Nothing, ImmutableArray(Of SyntaxTrivia).Empty, ImmutableArray(Of SyntaxTrivia).Empty)
  'End Function

End Class
