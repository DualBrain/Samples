Option Explicit On
Option Strict On
Option Infer On

Module Program

  ' pass 1 - parse/tokenize
  ' pass 2 - execute tokens

  Sub Main() 'args As String())

    Dim a%, b%, charIndex%, d%, errorLine%, lineCount%, number%, stackIndex%, t%, variableIndex%, TICKS%, TICKSPERSEC%

    ' A          temp
    ' B          temp
    ' C          character index in line
    ' E          line number for error msg
    ' I          temp (loops)
    ' L          number of lines
    ' N          number
    ' S          expression stack index
    ' T          temp
    ' V          variable index

    Dim tempA$ = ""
    Dim tempB$ = ""
    Dim character$ = ""
    Dim singleStatement$ = ""
    Dim error$ = ""

    Dim diffI%, N1% ', AAA$

    ' Astring$         temp
    ' Bstring$         temp
    ' Cstring$         character
    ' Dstring$         single statement
    ' Estring$         error message
    ' Gstring$         string code (")
    ' Hstring$         HALT code (Line Feed)
    ' Zstring$=Astringarray$(26)  statement input

    Dim data$(125)      ' [27-125] = 99 program lines
    Dim variables(82) As Integer   ' [27-52] = 26 variables, [53-82] = 30 items math stack

    data$(9) = "BYE, EXIT, QUIT, CLEAR, CLS, END"
    data$(10) = "HELP, MEM, NEW, RUN"
    data$(11) = "GOTO | LOAD [<exp>] | SAVE <exp>"
    data$(12) = "IF <exp> THEN <statement>"
    data$(13) = "INPUT <var>"
    data$(14) = "[LET] <var>=<exp>"
    data$(15) = "LIST [<exp>|PAUSE]"
    data$(16) = "PRINT or ? <exp|str>[,<exp|str>][;]"
    data$(17) = "REM or ' <any>"

    'BEGIN
    Dim quot$ = Chr(34) ': lineFeed$ = Chr(10)
    ' B=FILEEXISTS("tinyBas0")
    ' IF B=1 THEN
    '  Zstring$="load0:run"
    '  GOTO _AutoRun
    ' END IF

    ' Startup, "version info"

    Console.WriteLine("Tiny BASIC v0.0.0")
    Console.WriteLine("Type HELP for commands.")

    Do

      ' display an error if one exists...

      If error$ <> "" Then
        Console.WriteLine($"#Err{If(errorLine > 0, $" in {errorLine}", "")}: {error$}")
        error$ = ""
      End If

      ' display the "prompt"

      Console.WriteLine("Ready")

      ' line entry mode (mode)...

      Do

        Dim statement$ = Console.ReadLine()
        data$(26) = statement$

        'AutoRun:
        lineCount = 26
        charIndex = 1
        GetLineNumber(b, tempA, tempB, data, lineCount, charIndex, number, character)
        errorLine = number
        If number = 0 Then
          If character$ = "" Then
            Continue Do
          Else
            GoTo NextStatement
          End If
        Else
          EnterLine(data, number, statement$, error$)
          If error$ <> "" Then
            Exit Do
          End If
        End If
      Loop
      If error$ <> "" Then Continue Do

ExecuteLine:
      GetLineNumber(b, tempA, tempB, data, lineCount, charIndex, number, character)
      errorLine = number

NextStatement:
      GetKeyword(data, error$, lineCount, charIndex, character, singleStatement, b, tempA)

      If error$ <> "" Then
        Continue Do
      End If

      Select Case singleStatement$
        Case "if"
          GetExpression(a, b, tempA, tempB, character, data, variables, error$, errorLine, lineCount, charIndex, number, d, singleStatement, variableIndex, stackIndex, TICKS, TICKSPERSEC)
          If error$ = "" Then
            If number < 1 Then
              tempB$ = data$(lineCount) : charIndex = Len(tempB$) + 1
            Else
              GetKeyword(data, error$, lineCount, charIndex, character, singleStatement, b, tempA)
              If error$ = "" Then
                If singleStatement$ <> "then" Then
                  error$ = "'THEN' expected"
                  Continue Do
                Else
                  GoTo NextStatement
                End If
              Else
                Continue Do
              End If
            End If
          Else
            Continue Do
          End If
        Case "rem", "'"
          tempB$ = data$(lineCount) : charIndex = Len(tempB$) + 1
        Case "input"
          GetVariable(a, b, tempA, data, error$, lineCount, charIndex, character, singleStatement, variableIndex)
          If error$ = "" Then
            number = CInt(Console.ReadLine()) : variables(variableIndex) = number
          Else
            Continue Do
          End If
        Case "print", "?"

          Do

            SkipSpace(data, lineCount, charIndex)
            GetChar(data, lineCount, charIndex, character, tempA)

            If character$ = quot$ Then
              tempB$ = ""

              Do
                charIndex += 1 : character$ = Mid(tempA$, charIndex, 1)
                If character$ = "" Then
                  error$ = "Unterminated string"
                  Exit Do
                Else
                  If character$ <> quot$ Then
                    tempB$ &= character$
                    Continue Do
                  End If
                End If
                charIndex += 1 : character$ = Mid(tempA$, charIndex, 1)
                If character$ = quot$ Then
                  tempB$ &= character$
                  Continue Do
                Else
                  Exit Do
                End If

              Loop
              If error$ <> "" Then Exit Do
              Console.Write(tempB$)
            Else
              GetExpression(a, b, tempA, tempB, character, data, variables, error$, errorLine, lineCount, charIndex, number, d, singleStatement, variableIndex, stackIndex, TICKS, TICKSPERSEC)
              If error$ <> "" Then
                Exit Do
              End If
              b = N1
              If b = number Then
                Console.Write($"{number}*")
              Else
                Console.Write(number)
              End If
            End If
            SkipSpace(data, lineCount, charIndex)
            GetChar(data, lineCount, charIndex, character, tempA)
            If character$ = "," Then
              charIndex += 1
            Else
              Exit Do
            End If

          Loop
          If error$ <> "" Then Continue Do
          SkipSpace(data, lineCount, charIndex)
          GetChar(data, lineCount, charIndex, character, tempA)

          If character$ <> ";" Then
            Console.WriteLine()
          Else
            charIndex += 1
          End If
        Case "clear"
          For i = 27 To 52 : variables(i) = 0 : Next i
        Case "run"

          For i = 27 To 52 : variables(i) = 0 : Next i
          lineCount = 27 : charIndex = 1

          ' If the first line contains code...
          If data$(lineCount) <> "" Then
            ' populate the tempB$ variable with the line of code...
            tempB$ = data$(lineCount)
            ' jump to the Exec portion of the loop.
            GoTo ExecuteLine
          Else
            ' no code... nothing to run... abort.
            Continue Do
          End If

        Case "goto"
          GetExpression(a, b, tempA, tempB, character, data, variables, error$, errorLine, lineCount, charIndex, number, d, singleStatement, variableIndex, stackIndex, TICKS, TICKSPERSEC)
          If error$ <> "" Then Continue Do
          If errorLine >= number Then lineCount = 27
          charIndex = 1 : t = number
          Do
            If lineCount = 126 Then
              error$ = "Line not found"
              Exit Do
            End If
            GetLineNumber(b, tempA, tempB, data, lineCount, charIndex, number, character)
            If number = t Then
              errorLine = number
              GoTo NextStatement
            End If
            lineCount += 1 : charIndex = 1
          Loop
          Continue Do
        Case "new"
          For i = 27 To 125 : data$(i) = "" : Next i
          For i = 27 To 52 : variables(i) = 0 : Next i
          If errorLine <> 0 Then
            Continue Do
          End If
        Case "cls"
          Console.Clear()
        Case "help"
          For i = 9 To 18
            tempB$ = data$(i) : Console.WriteLine(tempB$)
          Next i
        Case "mem"
          b = 126
          For i = 27 To 125
            diffI = 152 - i  'Cheating here
            tempB$ = data$(diffI) : If tempB$ = "" Then b = diffI
          Next
          b = 126 - b : Console.Write($"{b}*")
          Console.WriteLine(" lines free")
        Case "end"
          Continue Do
        Case "bye", "exit", "quit"
          Exit Do
        Case "list"
          Dim i%
          GetLineNumber(b, tempA, tempB, data, lineCount, charIndex, number, character) : t = number : a = lineCount : i = charIndex
          If t = 0 Then
            GetKeyword(data, error$, lineCount, charIndex, character, singleStatement, b, tempA)
            If error$ = "" AndAlso singleStatement$ = "pause" Then i = charIndex
            error$ = ""
          End If
          For lineCount = 27 To 125
            charIndex = 1 : GetLineNumber(b, tempA, tempB, data, lineCount, charIndex, number, character)
            b = If((t = 0) OrElse (number = t), 1, 0)
            If b = 1 Then
              If tempA$ <> "" Then
                Console.WriteLine(tempA$)
                If singleStatement$ = "pause" Then
                  b = (lineCount - 26) Mod 10
                  If b = 0 Then Console.Write("Pause...") : 
                  Console.ReadLine()
                End If
              End If
            End If
          Next lineCount
          lineCount = a : charIndex = i
        Case "save"
          GetExpression(a, b, tempA, tempB, character, data, variables, error$, errorLine, lineCount, charIndex, number, d, singleStatement, variableIndex, stackIndex, TICKS, TICKSPERSEC)
          If error$ <> "" Then Continue Do
          tempA = $"tinyBas{number}" : a = 0
          Dim content = ""
          For i = 27 To 125
            If data(i) <> "" Then
              content &= data(i) : a = 1
            Else
              Exit For
            End If
          Next
          If a = 0 Then
            IO.File.Delete(tempA)
          Else
            IO.File.WriteAllText(tempA, content)
          End If
        Case "load"
          GetExpression(a, b, tempA, tempB, character, data, variables, error$, errorLine, lineCount, charIndex, number, d, singleStatement, variableIndex, stackIndex, TICKS, TICKSPERSEC)
          If error$ <> "" Then Continue Do
          tempA = $"tinyBas{number}"
          If Not IO.File.Exists(tempA) Then
            error$ = $"File {tempA} not found" : Continue Do
          End If
          Dim i = 27
          For Each line In IO.File.ReadAllLines(tempA)
            data(i) = line : i += 1
          Next
          While i <= 125
            data(i) = "" : i += 1
          End While
          Continue Do
        Case "let"
          GetKeyword(data, error$, lineCount, charIndex, character, singleStatement, b, tempA)
          If error$ <> "" Then
            Continue Do
          Else
            ReturnVariable(a, b, error$, singleStatement, variableIndex)
            If error$ <> "" Then Continue Do
            SkipSpace(data, lineCount, charIndex)
            GetChar(data, lineCount, charIndex, character, tempA)
            If character$ <> "=" Then
              error$ = "'=' expected"
              Continue Do
            End If
            charIndex += 1 : t = variableIndex
            GetExpression(a, b, tempA, tempB, character, data, variables, error$, errorLine, lineCount, charIndex, number, d, singleStatement, variableIndex, stackIndex, TICKS, TICKSPERSEC)
            If error$ <> "" Then Continue Do
            variables(t) = number
          End If

        Case Else

      End Select

      SkipSpace(data, lineCount, charIndex)

      GetChar(data, lineCount, charIndex, character, tempA)

      If character = ":" Then
        charIndex += 1
        GoTo NextStatement
      Else
        If character <> "" Then
          error$ = "End of statement expected"
          Continue Do
        Else
          If lineCount = 26 Then
            Continue Do
          Else
            lineCount += 1 : charIndex = 1
            If lineCount = 126 Then
              error$ = "Program Overflow"
              Continue Do
            End If
          End If
        End If
      End If

      ' "pop" the next line...
      tempB$ = data$(lineCount)
      ' If the line is blank...
      If tempB$ = "" Then
        ' abort...
        Continue Do
      Else
        ' otherwise, continue execution...
        GoTo ExecuteLine
      End If

    Loop

    Console.WriteLine("Thanks for using Tiny BASIC.")

  End Sub

  Private Sub GetExpression(ByRef a%, ByRef b%, ByRef Astring$, ByRef Bstring$, ByRef Cstring$, Astringarray$(), Aarray%(), ByRef Estring$, ByRef e%, l%, ByRef c%, ByRef n%, ByRef d%, Dstring$, ByRef v%, ByRef s%, TICKS%, TICKSPERSEC%)
    Aarray(53) = 0 : s = 53
    BoolExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, l, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
    n = Aarray(s)
  End Sub

  Private Sub BoolExpression(ByRef a%, ByRef b%, ByRef Astring$, ByRef Bstring$, ByRef Cstring$, Astringarray$(), Aarray%(), ByRef Estring$, ByRef e%, l%, ByRef c%, ByRef n%, ByRef d%, Dstring$, ByRef v%, ByRef s%, TICKS%, TICKSPERSEC%)
    AddExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, l, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
    SkipSpace(Astringarray, l, c)
    GetChar(Astringarray, l, c, Cstring, Astring)
    Do
      Select Case Cstring$
        Case "="
          c += 1 : AddExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, l, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
          b = s - 1 : Aarray(b) = If(Aarray(b) = Aarray(s), 1, 0) : s -= 1
        Case ">"
          c += 1 : GetChar(Astringarray, l, c, Cstring, Astring)
          If Cstring$ = "=" Then
            c += 1 : AddExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, l, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
            b = s - 1 : Aarray(b) = If(Aarray(b) >= Aarray(s), 1, 0) : s -= 1
          Else
            AddExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, l, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
            b = s - 1 : Aarray(b) = If(Aarray(b) > Aarray(s), 1, 0) : s -= 1
          End If
        Case "<"
          c += 1 : GetChar(Astringarray, l, c, Cstring, Astring)
          Select Case Cstring$
            Case "="
              c += 1 : AddExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, l, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
              b = s - 1 : Aarray(b) = If(Aarray(b) <= Aarray(s), 1, 0) : s -= 1
            Case ">"
              c += 1 : AddExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, l, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
              b = s - 1 : Aarray(b) = If(Aarray(b) <> Aarray(s), 1, 0) : s -= 1
            Case Else
              AddExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, l, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
              b = s - 1 : Aarray(b) = If(Aarray(b) < Aarray(s), 1, 0) : s -= 1
          End Select
      End Select
      SkipSpace(Astringarray, l, c)
      GetChar(Astringarray, l, c, Cstring, Astring)
      b = If(String.IsNullOrEmpty(Cstring), 0, Asc(Cstring$)) : b = If((b >= 60) AndAlso (b <= 62), 1, 0)
      If Not b = 1 Then
        Exit Do
      End If
    Loop
  End Sub

  Private Sub AddExpression(ByRef a%, ByRef b%, ByRef Astring$, ByRef Bstring$, ByRef Cstring$, Astringarray$(), Aarray%(), ByRef Estring$, ByRef e%, L%, ByRef c%, ByRef n%, ByRef d%, Dstring$, ByRef v%, ByRef s%, TICKS%, TICKSPERSEC%)
    MulExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, L, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
    SkipSpace(Astringarray, L, c)
    GetChar(Astringarray, L, c, Cstring, Astring)
    Do
      Select Case Cstring$
        Case "+"
          c += 1 : MulExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, L, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
          b = s - 1 : Aarray(b) = Aarray(b) + Aarray(s) : s -= 1
        Case "-"
          c += 1 : MulExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, L, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
          b = s - 1 : Aarray(b) = Aarray(b) - Aarray(s) : s -= 1
      End Select
      SkipSpace(Astringarray, L, c)
      GetChar(Astringarray, L, c, Cstring, Astring)
      b = If(String.IsNullOrEmpty(Cstring), 0, Asc(Cstring$)) : b = If((b = 43) OrElse (b = 45), 1, 0)
      If Not b = 1 Then
        Exit Do
      End If
    Loop
  End Sub

  Private Sub MulExpression(ByRef a%, ByRef b%, ByRef Astring$, ByRef Bstring$, ByRef Cstring$, Astringarray$(), Aarray%(), ByRef Estring$, ByRef e%, l%, ByRef c%, ByRef n%, ByRef d%, Dstring$, ByRef v%, ByRef s%, TICKS%, TICKSPERSEC%)
    GroupExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, l, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
    SkipSpace(Astringarray, l, c)
    GetChar(Astringarray, l, c, Cstring, Astring)
    Do
      Select Case Cstring$
        Case "*"
          c += 1 : GroupExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, l, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
          b = s - 1 : Aarray(b) = Aarray(b) * Aarray(s) : s -= 1
        Case "/"
          c += 1 : GroupExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, l, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
          b = Aarray(s)
          If b = 0 Then
            If Estring$ = "" Then Estring$ = "Division by zero"
            s -= 1 : Return
          Else
            b = s - 1 : Aarray(b) = Aarray(b) \ Aarray(s) : s -= 1
          End If
        Case " "
          c += 1 : GroupExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, l, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
          b = Aarray(s)
          If b = 0 Then
            If Estring$ = "" Then Estring$ = "Division by zero"
            s -= 1 : Return
          Else
            b = s - 1 : Aarray(b) = Aarray(b) Mod Aarray(s) : s -= 1
          End If
      End Select
      SkipSpace(Astringarray, l, c)
      GetChar(Astringarray, l, c, Cstring, Astring)
      b = If(String.IsNullOrEmpty(Cstring$), 0, Asc(Cstring$))
      b = If((b = 42) OrElse (b = 47) OrElse (b = 92), 1, 0)
      If Not b = 1 Then
        Exit Do
      End If
    Loop
  End Sub

  Private Sub GroupExpression(ByRef a%, ByRef b%, ByRef Astring$, ByRef Bstring$, ByRef Cstring$, Astringarray$(), Aarray%(), ByRef Estring$, ByRef e%, l%, ByRef c%, ByRef n%, ByRef d%, Dstring$, ByRef v%, ByRef s%, TICKS%, TICKSPERSEC%)
    SkipSpace(Astringarray, l, c)
    GetChar(Astringarray, l, c, Cstring, Astring)
    Select Case Cstring$
      Case "("
        c += 1 : BoolExpression(a, b, Astring, Bstring, Cstring, Astringarray, Aarray, Estring, e, l, c, n, d, Dstring, v, s, TICKS, TICKSPERSEC)
        SkipSpace(Astringarray, l, c)
        GetChar(Astringarray, l, c, Cstring, Astring)
        If Cstring$ <> ")" Then
          If Estring$ = "" Then Estring$ = "Missing ')'"
          Return
        End If
        c += 1
      Case ""
        If Estring$ = "" Then Estring$ = "Invalid Factor"
      Case Else
        b = Asc(Cstring$) : b = If((b < 48) OrElse (b > 57), 1, 0)
        If b = 0 Then
          GetLineNumber(b, Astring, Bstring, Astringarray, l, c, n, Cstring)
          s += 1 : Aarray(s) = n
        Else
          GetKeyword(Astringarray, Estring, l, c, Cstring, Dstring, b, Astring)
          If Estring$ <> "" Then Return
          b = Len(Dstring$)
          If b = 1 Then
            ReturnVariable(a, b, Estring, Dstring, v)
            s += 1 : Aarray(s) = Aarray(v)
          Else
            Select Case Dstring$
              Case "ticks"
                s += 1 : Aarray(s) = TICKS
              Case "tickspersec"
                s += 1 : Aarray(s) = TICKSPERSEC
              Case Else
                If Estring$ = "" Then Estring$ = "Function expected"
            End Select
          End If
        End If
    End Select
  End Sub

  'Private Sub EnterLine(ByRef b%, ByRef Astring$, ByRef Bstring$, ByRef Cstring$, data$(), ByRef Estring$, lineEntered$, ByRef l%, ByRef c%, ByRef n%, ByRef t%, ByRef diffI%)
  'Private Sub EnterLine(b%, Astring$, Bstring$, Cstring$, data$(), ByRef Estring$, lineEntered$, l%, c%, n%, t%, diffI%)
  Private Sub EnterLine(data$(), lineNumber%, statement$, ByRef error$)
    Dim t = lineNumber
    Dim l = 27
    Dim c = 1
    Dim b = 0
    Dim astring$ = ""
    Dim bstring$ = ""
    Dim cstring$ = ""
    Do
      GetLineNumber(b, astring, bstring, data, l, c, lineNumber, cstring)
      b = If((lineNumber < t) AndAlso (lineNumber <> 0) AndAlso (l < 126), 1, 0)
      If b = 1 Then
        l += 1 : c = 1
      Else
        Exit Do
      End If
    Loop
    If l = 126 Then
      error$ = "Program Overflow"
      Return
    End If
    If t <> lineNumber Then
      For i = l To 125
        Dim diffI = (125 + l) - i
        b = diffI - 1 : data$(diffI) = data$(b)
      Next
    End If
    data$(l) = statement$
    SkipSpace(data, l, c)
    GetChar(data, l, c, cstring, Nothing)
    If cstring = "" Then
      For i = l To 124
        b = i + 1 : data$(i) = data$(b)
      Next
    End If
  End Sub

  Private Sub GetLineNumber(ByRef b%, ByRef Astring$, ByRef Bstring$, data$(), l%, ByRef characterIndex%, ByRef lineNumber%, ByRef Cstring$)

    SkipSpace(data, l, characterIndex)
    Bstring$ = ""

    Do
      GetChar(data, l, characterIndex, Cstring, Astring)
      If Cstring$ = "" Then Exit Do
      b = Asc(Cstring$)
      b = If((b < 48 OrElse b > 57) AndAlso b <> 46, 1, 0)
      If b = 1 Then Exit Do
      Bstring$ &= Cstring$
      characterIndex += 1
    Loop

    If IsNumeric(Bstring) Then
      lineNumber = CInt(Bstring)
    Else
      lineNumber = 0
    End If

  End Sub

  'Private Sub GetVar(ByRef a%, ByRef b%, ByRef Astring$, Astringarray$(), ByRef Estring$, l%, ByRef c%, ByRef Cstring$, ByRef d%, Dstring$, ByRef v%)
  Private Sub GetVariable(ByRef a%, ByRef b%, ByRef Astring$, Astringarray$(), ByRef Estring$, l%, ByRef c%, ByRef Cstring$, Dstring$, ByRef v%)
    GetKeyword(Astringarray, Estring, l, c, Cstring, Dstring, b, Astring)
    If Estring$ <> "" Then Return
    ReturnVariable(a, b, Estring, Dstring, v)
  End Sub

  Private Sub ReturnVariable(ByRef a%, ByRef b%, ByRef error$, value$, ByRef variable%)
    b = Asc(value$) : a = Len(value$)
    a = If(a <> 1 OrElse b < 97 OrElse b > 122, 1, 0)
    If a = 0 Then
      variable = b - 70
    Else
      If error$ = "" Then error$ = "Variable expected"
    End If
  End Sub

  Private Sub GetKeyword(Astringarray$(), ByRef Estring$, l%, ByRef c%, ByRef Cstring$, ByRef Dstring$, ByRef b%, ByRef Astring$)

    SkipSpace(Astringarray, l, c)
    GetChar(Astringarray, l, c, Cstring, Astring)
    Dstring$ = ""
    If Cstring$ <> "" Then
      b = Asc(Cstring$)
      'b = If((b < 97) OrElse (b > 122), 1, 0)
      'If b = 1 Then GoTo GetLabelError
      If Not InvalidKeywordCharacter(b) Then
        Do
          Dstring$ &= Cstring$ : c += 1
          GetChar(Astringarray, l, c, Cstring, Astring)
          If Cstring$ = "" Then
            Return
          End If
          b = Asc(Cstring$)
          'b = If((b >= 97) AndAlso (b <= 122), 1, 0)
          'If Not b = 1 Then Return
          If InvalidKeywordCharacter(b) Then
            Return
          End If
        Loop
        'Else
        '  GoTo GetLabelError
      End If
      'Else
      '  GoTo GetLabelError
    End If

    'GetLabelError:
    If Estring$ = "" Then
      Estring$ = "Invalid label"
    End If

  End Sub

  Private Function InvalidKeywordCharacter(b As Integer) As Boolean
    Select Case b
      Case 97 To 122 ' a - z
        Return False
      Case 39 ' ' (aka REM)
        Return False
      Case 63 ' ?
        Return False
      Case Else
        Return True
    End Select
  End Function

  Private Sub SkipSpace(data$(), lineIndex%, ByRef characterIndex%)
    Dim character As String = ""
    Dim line As String = ""
    Do
      GetChar(data, lineIndex, characterIndex, character, line)
      If character = " " Then
        characterIndex += 1
      Else
        Return
      End If
    Loop
  End Sub

  Private Sub GetChar(data$(), lineIndex%, characterIndex%, ByRef character$, ByRef line$)
    line$ = data$(lineIndex)
    character$ = Mid(line$, characterIndex, 1) : character$ = character$.ToLower
  End Sub

End Module