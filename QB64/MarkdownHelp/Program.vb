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

  Sub Main() 'args As String())

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
          m_lines.Add($"{Space(65)}╚═{New String("═"c, workingId.Length)}═╝")
        ElseIf line.StartsWith("* ") Then
          ' bullet entry.
          Dim text = line.Substring(2)
          Dim keyword = text.Substring(1, text.IndexOf("]"c) - 1)
          If functions.Contains(keyword) Then
            keyword &= " Function"
          ElseIf statements.Contains(keyword) Then
            keyword &= " Statement"
          ElseIf keywords.Contains(keyword) Then
            keyword &= " Keyword"
          ElseIf operators.Contains(keyword) Then
            keyword &= " Operator"
          Else
            If keyword.Contains("(statement)") Then
              keyword = keyword.Replace("(statement)", "Statement")
            Else
              If text.Contains("(function)") Then
                keyword &= " Function"
              ElseIf text.Contains("statement)") Then
                keyword &= " Statement"
              Else
                'keyword &= " Keyword"
              End If
            End If
          End If
          m_lines.Add($"  {keyword}")
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
      PrintEx(text, 1, top + n) : n += 1
    Next

    ' ║╚╝╣╗╔═

    'PrintEx("ABS Function", 3, 11) : PrintEx("APPEND Keyword", 40, 11)
    'PrintEx("ABSOLUTE Keyword", 3, 12) : PrintEx("AS Keyword", 40, 12)
    'PrintEx("ACCESS Keyword", 3, 13) : PrintEx("ASCKeyword", 40, 13)
    'PrintEx("AND Operator", 3, 14) : PrintEx("ATN Keyword", 40, 14)
    'PrintEx("ANY Keyword", 3, 15) : PrintEx("", 40, 15)

    'PrintEx("╔═══╗", 66, 16)
    'PrintEx(New String("═"c, 65), 1, 17)
    'Console.Write("╣ B ║")
    'PrintEx("╚═══╝", 66, 18)

    'PrintEx("BASE Keyword", 3, 19) : PrintEx("BLOAD Statement", 40, 19)
    'PrintEx("Basic Character Set", 3, 20) : PrintEx("Boolean Operators", 40, 20)
    'PrintEx("BEEP Statement", 3, 21) : PrintEx("BSAVE Statement", 40, 21)
    'PrintEx("BINARY Keyword", 3, 22) : PrintEx("", 40, 22)

    'PrintEx("╔═══╗", 66, 23)
    'PrintEx(New String("═"c, 65), 1, 24)
    'Console.Write("╣ C ║")
    'PrintEx("╚═══╝", 66, 25)

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
        'PrintEx("X", w - 1, r)
      End If
    Next

    Console.Write("└"c)
    Console.Write(New String("─"c, w - 2))
    Console.Write("┘"c)

    Do

      Try
        If w <> Console.WindowWidth Then GoTo FullRedraw
        If h <> Console.WindowHeight Then GoTo FullRedraw
      Catch
      End Try

      Try
        If Console.KeyAvailable Then
          Dim cki = Console.ReadKey()
          Select Case cki.Key
            Case ConsoleKey.UpArrow
              index -= 1 : If index < 0 Then index = 0
              GoTo Redraw
            Case ConsoleKey.DownArrow
              index += 1 : If index > m_lines.Count - 1 Then index = m_lines.Count - 1
              GoTo Redraw
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
