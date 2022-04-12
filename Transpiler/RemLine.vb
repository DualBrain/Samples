Module RemLine

  ' Global and constant data
  Const MaxLines = 10000

  Private ReadOnly LineTable!(MaxLines)
  Private LineCount As Integer
  Private Seps$, InputFile$, OutputFile$, TmpFile$

  ' Keyword search data
  Const KeyWordCount = 9
  Private ReadOnly KeywordTable$() = {"", "THEN", "ELSE", "GOSUB", "GOTO", "RESUME", "RETURN", "RESTORE", "RUN", "ERL", ""}

  Private m_outputFile$

  Public Function RemLine(inputFile$) As String

    ' Start of module-level program code
    Seps$ = " ,:=<>()" + Chr(9)
    'GetFileNames()
    BuildTable(inputFile$)
    Return GenOutFile(inputFile$)

  End Function

  '
  ' BuildTable:
  '   Examines the entire text file looking for line numbers that are
  '   the object of GOTO, GOSUB, etc. As each is found, it is entered
  '   into a table of line numbers. The table is used during a second
  '   pass (see GenOutFile), when all line numbers not in the list
  '   are removed.
  ' Input:
  '   Uses globals KeyWordTable$, KeyWordCount, and Seps$
  ' Output:
  '   Modifies LineTable! and LineCount
  '
  Sub BuildTable(inputFile$) REM Static

    'Static Inlin$
    Dim Token$

    Dim lines = inputFile$.Split(vbLf)
    Dim index = 0

    Do While index < lines.Count  ' While Not EOF(1)

      ' Get line and first token
      Dim inLin$ = lines(index) : index += 1 '"" : LINE_INPUT(1, inLin$)
      Token$ = GetToken$(inLin, Seps$)
      Do While (Token$ <> "")
        For KeyIndex = 1 To KeyWordCount
          ' See if token is keyword
          If (KeywordTable(KeyIndex) = UCase(Token$)) Then
            ' Get possible line number after keyword
            Token$ = GetToken$("", Seps$)
            ' Check each token to see if it is a line number
            ' (the LOOP is necessary for the multiple numbers
            ' of ON GOSUB or ON GOTO). A non-numeric token will
            ' terminate search.
            Do While (IsDigit(Left(Token$, 1)))
              LineCount += 1
              LineTable!(LineCount) = CSng(Val(Token$))
              Token$ = GetToken$("", Seps$)
              If Token$ <> "" Then KeyIndex = 0
            Loop
          End If
        Next KeyIndex
        ' Get next token
        Token$ = GetToken$("", Seps$)
      Loop
    Loop

  End Sub

  '
  ' GenOutFile:
  '  Generates an output file with unreferenced line numbers removed.
  ' Input:
  '  Uses globals LineTable!, LineCount, and Seps$
  ' Output:
  '  Processed file
  '
  Function GenOutFile(inputFile$) As String

    Dim output$ = ""

    ' Speed up by eliminating comma and colon (can't separate first token)
    Dim sep$ = $" {vbTab}"

    Dim lines = inputFile$.Split(vbLf)
    Dim lineIndex = 0

    Do While lineIndex < lines.Count  ' While Not EOF(1)

      Dim inLin$ = lines(lineIndex) : lineIndex += 1 '"" : LINE_INPUT(1, inLin$)

      If (inLin$ <> "") Then

        ' Get first token and process if it is a line number
        Dim token$ = GetToken$(inLin$, sep$)

        If IsDigit(Left(token, 1)) Then

          Dim lineNumber! = CSng(Val(token))
          Dim foundNumber = False

          ' See if line number is in table of referenced line numbers
          For index = 1 To LineCount
            If (lineNumber = LineTable!(index)) Then
              foundNumber = True
            End If
          Next

          ' Modify line strings
          If 1 = 0 Then

            If Not foundNumber Then
              token = Space(Len(token))
              MID$(inLin$, StrSpn(inLin$, sep$), Len(token)) = token
            End If

          Else

            ' You can replace the previous lines with your own
            ' code to reformat output. For example, try these lines:

            Dim tmpPos1 = StrSpn(inLin$, sep$) + Len(token)
            Dim tmpPos2 = tmpPos1 + StrSpn(Mid(inLin$, tmpPos1), sep$) - 1

            If foundNumber Then
              inLin$ = $"{Left(inLin$, tmpPos1 - 1)}:{vbCrLf}{vbTab}{Mid(inLin$, tmpPos2)}"
            Else
              inLin$ = $"{vbTab}{Mid(inLin$, tmpPos2)}"
            End If
          End If

        End If
      End If

      ' Print line to file or console (PRINT is faster than console device)
      'If OutputFile$ = "CON" Then
      '  Print(inLin$)
      'Else
      'Print(2, inLin$)
      output$ &= inLin$ & vbLf
      'End If

    Loop

    Return output$

  End Function

  '
  ' GetFileNames:
  '  Gets a file name by prompting the user.
  ' Input:
  '  User input
  ' Output:
  '  Defines InputFiles$ and OutputFiles$
  '
  '  Sub GetFileNames()

  '    CLS()
  '    Print(" Microsoft RemLine: Line Number Removal Utility")
  '    Print("       (.BAS assumed if no extension given)")
  '    Print()
  '    Input("      Input file name (ENTER to terminate): ", InputFile$)
  '    If InputFile$ = "" Then End
  '    Input("      Output file name (ENTER to print to screen): ", OutputFile$)
  '    Print()
  '    If (OutputFile$ = "") Then OutputFile$ = "CON"

  '    If InStr(InputFile$, ".") = 0 Then
  '      InputFile$ &= ".BAS"
  '    End If

  '    If InStr(OutputFile$, ".") = 0 Then
  '      Select Case OutputFile$
  '        Case "CON", "SCRN", "PRN", "COM1", "COM2", "LPT1", "LPT2", "LPT3"
  '          Exit Sub
  '        Case Else
  '          OutputFile$ &= ".BAS"
  '      End Select
  '    End If

  '    Do While InputFile$ = OutputFile$
  '      TmpFile$ = Left(InputFile$, InStr(InputFile$, ".")) + "BAK"
  '      On Error GoTo FileErr1
  '      NAME(InputFile$, TmpFile$)
  '      On Error GoTo 0
  '      If TmpFile$ <> "" Then InputFile$ = TmpFile$
  '    Loop

  '    Return

  'FileErr1:
  '    CLS()
  '    Print("      Invalid file name") : Print()
  '    Input("      New input file name (ENTER to terminate): ", InputFile$)
  '    If InputFile$ = "" Then End

  '  End Sub

  ''' <summary>
  ''' Extracts tokens from a string. A token is a word that is surrounded
  ''' by separators, such as spaces or commas. Tokens are extracted and
  ''' analyzed when parsing sentences or commands. To use the GetToken$
  ''' function, pass the string to be parsed on the first call, then pass
  ''' a null string on subsequent calls until the function returns a null
  ''' to indicate that the entire string has been parsed.
  ''' </summary>
  ''' <param name="search$">string to search</param>
  ''' <param name="delim$">String of separators</param>
  ''' <returns>next token</returns>
  Function GetToken$(search$, delim$) REM Static

    Static begPos As Integer
    Static saveStr$

    ' Note that SaveStr$ and BegPos must be static from call to call
    ' (other variables are only static for efficiency).
    ' If first call, make a copy of the string
    If (search$ <> "") Then
      begPos = 1
      saveStr$ = search$
    End If

    ' Find the start of the next token
    Dim newPos = StrSpn(Mid(saveStr$, begPos, Len(saveStr$)), delim$)
    If newPos <> 0 Then
      ' Set position to start of token
      begPos = newPos + begPos - 1
    Else
      ' If no new token, quit and return null
      Return ""
    End If

    ' Find end of token
    newPos = StrBrk(Mid(saveStr$, begPos, Len(saveStr$)), delim$)
    If newPos <> 0 Then
      ' Set position to end of token
      newPos = begPos + newPos - 1
    Else
      ' If no end of token, return set to end a value
      newPos = Len(saveStr$) + 1
    End If

    ' Cut token out of search string
    Dim result = Mid(saveStr$, begPos, newPos - begPos)
    ' Set new starting position
    begPos = newPos

    Return result

  End Function

  '
  ' InitKeyTable:
  '  Initializes a keyword table. Keywords must be recognized so that
  '  line numbers can be distinguished from numeric constants.
  ' Input:
  '  Uses KeyData
  ' Output:
  '  Modifies global array KeyWordTable$
  '
  'Sub InitKeyTable()

  '  RESTORE() REM 'KeyData
  '  For Count = 1 To KeyWordCount
  '    Dim Keyword$ = Nothing
  '    READ(Keyword)
  '    KeyWordTable$(Count) = Keyword
  '  Next

  'End Sub

  ''' <summary>
  ''' Returns true if character passed is a decimal digit. Since any 
  ''' Basic token starting with a digit is a number, the function only
  ''' needs to check the first digit. Doesn't check for negative numbers,
  ''' but that's not needed here.
  ''' </summary>
  ''' <param name="char$">initial character of string to check</param>
  ''' <returns>true if within 0 - 9</returns>
  Function IsDigit(char$) As Boolean

    If (char$ = "") Then
      Return False
    Else
      Dim charAsc = Asc(char$)
      Return (charAsc >= Asc("0")) AndAlso (charAsc <= Asc("9"))
    End If

  End Function

  ''' <summary>
  ''' Searches InString$ to find the first character from among those in
  ''' Separator$. Returns the index of that character. This function can
  ''' be used to find the end of a token.
  ''' </summary>
  ''' <param name="inString$">string to search</param>
  ''' <param name="separator$">characters to search for</param>
  ''' <returns>index to first match in InString$ or 0 if none match</returns>
  Function StrBrk(inString$, separator$) As Integer

    Dim ln = Len(inString$)
    Dim begPos = 1
    ' Look for end of token (first character that is a delimiter).
    Do While InStr(separator$, Mid(inString$, begPos, 1)) = 0
      If begPos > ln Then
        Return 0
      Else
        begPos += 1
      End If
    Loop
    Return begPos

  End Function

  ''' <summary>
  ''' Searches InString$ to find the first character that is not one of
  ''' those in Separator$. Returns the index of that character. This
  ''' function can be used to find the start of a token.
  ''' </summary>
  ''' <param name="inString$">string to search</param>
  ''' <param name="separator$">characters to search for</param>
  ''' <returns>index to first nonmatch in InString$ or 0 if all match</returns>
  Function StrSpn%(inString$, separator$)

    Dim ln = Len(inString$)
    Dim begPos = 1
    ' Look for start of a token (character that isn't a delimiter).
    Do While InStr(separator$, Mid(inString$, begPos, 1)) <> 0
      If begPos > ln Then
        Return 0
      Else
        begPos += 1
      End If
    Loop
    Return begPos

  End Function

End Module
