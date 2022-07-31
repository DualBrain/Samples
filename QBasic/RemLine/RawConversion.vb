Option Explicit Off 'On
Option Strict Off 'On
Option Infer On

Imports QB.Core
Imports QB.Console
Imports QB.File

Module RemLine_RawConversion

  '
  '   Microsoft RemLine - Line Number Removal Utility
  '   Copyright (C) Microsoft Corporation 1985-1990
  '
  '   REMLINE.BAS is a program to remove line numbers from Microsoft Basic
  '   Programs. It removes only those line numbers that are not the object
  '   of one of the following statements: GOSUB, RETURN, GOTO, THEN, ELSE,
  '   RESUME, RESTORE, or RUN.
  '
  '   When REMLINE is run, it will ask for the name of the file to be
  '   processed and the name of the file or device to receive the
  '   reformatted output. If no extension is given, .BAS is assumed (except
  '   for output devices). If filenames are not given, REMLINE prompts for
  '   file names. If both filenames are the same, REMLINE saves the original
  '   file with the extension .BAK.
  '
  '   REMLINE makes several assumptions about the program:
  '
  '     1. It must be correct syntactically, and must run in BASICA or
  '        GW-BASIC interpreter.
  '     2. There is a 400 line limit. To process larger files, change
  '        MaxLines constant.
  '     3. The first number encountered on a line is considered a line
  '        number; thus some continuation lines (in a compiler-specific
  '        construction) may not be handled correctly.
  '     4. REMLINE can handle simple statements that test the ERL function
  '        using  relational operators such as =, <, and >. For example,
  '        the following statement is handled correctly:
  '
  '             IF ERL = 100 THEN END
  '
  '        Line 100 is not removed from the source code. However, more
  '        complex expressions that contain the +, -, AND, OR, XOR, EQV,
  '        MOD, or IMP operators may not be handled correctly. For example,
  '        in the following statement REMLINE does not recognize line 105
  '        as a referenced line number and removes it from the source code:
  '
  '             IF ERL + 5 = 105 THEN END
  '
  '   If you do not like the way REMLINE formats its output, you can modify
  '   the output lines in SUB GenOutFile. An example is shown in comments.
  REM DEFINT A-Z

  ' Function and Subprocedure declarations
  REM Declare Function GetToken$ (Search$, Delim$)
  REM Declare Function StrSpn% (InString$, Separator$)
  REM Declare Function StrBrk% (InString$, Separator$)
  REM Declare Function IsDigit% (Char$)
  REM Declare Sub GetFileNames ()
  REM Declare Sub BuildTable ()
  REM Declare Sub GenOutFile ()
  REM Declare Sub InitKeyTable ()

  ' Global and constant data
  REM Const True = -1
  REM Const False = 0
  Const MaxLines = 400

  Private ReadOnly LineTable!(MaxLines)
  Private LineCount
  Private Seps$, InputFile$, OutputFile$, TmpFile$

  ' Keyword search data
  Const KeyWordCount = 9
  Private ReadOnly KeyWordTable$(KeyWordCount)

  Public Sub SubMain()

    REM Shared LineTable!(MaxLines)
    REM Shared LineCount
    REM Shared Seps$, InputFile$, OutputFile$, TmpFile$

    ' Keyword search data
    REM Const KeyWordCount = 9
    REM Shared KeyWordTable$(KeyWordCount)

KeyData:
    DATA("THEN", "ELSE", "GOSUB", "GOTO", "RESUME", "RETURN", "RESTORE", "RUN", "ERL", "")

    ' Start of module-level program code
    Seps$ = " ,:=<>()" + Chr(9)
    InitKeyTable()
    GetFileNames()
    On Error GoTo FileErr1
    OPEN(InputFile$, OpenMode.Input, 1)
    On Error GoTo 0
    COLOR(7) : PRINT("Working", True) : COLOR(23) : PRINT(" . . .") : COLOR(7) : PRINT()
    BuildTable()
    CLOSE(1)
    OPEN(InputFile$, OpenMode.Input, 1)
    On Error GoTo FileErr2
    OPEN(OutputFile$, OpenMode.Output, 2)
    On Error GoTo 0
    GenOutFile()
    CLOSE(1, 2)
    If OutputFile$ <> "CON" Then CLS()

    End

FileErr1:
    CLS()
    PRINT("      Invalid file name") : PRINT()
    INPUT("      New input file name (ENTER to terminate): ", InputFile$)
    If InputFile$ = "" Then End
FileErr2:
    INPUT("      Output file name (ENTER to print to screen) :", OutputFile$)
    PRINT()
    If (OutputFile$ = "") Then OutputFile$ = "CON"
    If TmpFile$ = "" Then
      Resume
    Else
      TmpFile$ = ""
      Resume Next
    End If

  End Sub

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
  Sub BuildTable() REM Static

    Static Inlin$
    Static Token$
    Static KeyIndex

    Do While Not EOF(1)
      ' Get line and first token
      LINE_INPUT(1, Inlin)
      Token$ = GetToken$(Inlin, Seps$)
      Do While (Token$ <> "")
        For KeyIndex = 1 To KeyWordCount
          ' See if token is keyword
          If (KeyWordTable$(KeyIndex) = UCase(Token$)) Then
            ' Get possible line number after keyword
            Token$ = GetToken$("", Seps$)
            ' Check each token to see if it is a line number
            ' (the LOOP is necessary for the multiple numbers
            ' of ON GOSUB or ON GOTO). A non-numeric token will
            ' terminate search.
            Do While (IsDigit(Left(Token$, 1)))
              LineCount += 1
              LineTable!(LineCount) = Val(Token$)
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
  Sub GenOutFile() REM Static

    ' Speed up by eliminating comma and colon (can't separate first token)
    Dim Sep$ = " " + Chr(9)
    Do While Not EOF(1)
      Dim InLin$ = ""
      LINE_INPUT(1, InLin$)
      If (InLin$ <> "") Then
        ' Get first token and process if it is a line number
        Dim Token$ = GetToken$(InLin$, Sep$)
        If IsDigit(Left(Token$, 1)) Then
          Dim LineNumber! = Val(Token$)
          Dim FoundNumber = False
          ' See if line number is in table of referenced line numbers
          For index = 1 To LineCount
            If (LineNumber! = LineTable!(index)) Then
              FoundNumber = True
            End If
          Next index
          ' Modify line strings
          If (Not FoundNumber) Then
            Token$ = Space(Len(Token$))
            MID$(InLin$, StrSpn(InLin$, Sep$), Len(Token$)) = Token$
          End If

          ' You can replace the previous lines with your own
          ' code to reformat output. For example, try these lines:

          'TmpPos1 = StrSpn(InLin$, Sep$) + LEN(Token$)
          'TmpPos2 = TmpPos1 + StrSpn(MID$(InLin$, TmpPos1), Sep$)
          '
          'IF FoundNumber THEN
          '   InLin$ = LEFT$(InLin$, TmpPos1 - 1) + CHR$(9) + MID$(InLin$, TmpPos2)
          'ELSE
          '   InLin$ = CHR$(9) + MID$(InLin$, TmpPos2)
          'END IF

        End If
      End If
      ' Print line to file or console (PRINT is faster than console device)
      If OutputFile$ = "CON" Then
        PRINT(InLin$)
      Else
        PRINT(2, InLin$)
      End If
    Loop

  End Sub

  '
  ' GetFileNames:
  '  Gets a file name by prompting the user.
  ' Input:
  '  User input
  ' Output:
  '  Defines InputFiles$ and OutputFiles$
  '
  Sub GetFileNames() REM Static

    CLS()
    PRINT(" Microsoft RemLine: Line Number Removal Utility")
    PRINT("       (.BAS assumed if no extension given)")
    PRINT()
    INPUT("      Input file name (ENTER to terminate): ", InputFile$)
    If InputFile$ = "" Then End
    INPUT("      Output file name (ENTER to print to screen): ", OutputFile$)
    PRINT()
    If (OutputFile$ = "") Then OutputFile$ = "CON"

    If InStr(InputFile$, ".") = 0 Then
      InputFile$ += ".BAS"
    End If

    If InStr(OutputFile$, ".") = 0 Then
      Select Case OutputFile$
        Case "CON", "SCRN", "PRN", "COM1", "COM2", "LPT1", "LPT2", "LPT3"
          Exit Sub
        Case Else
          OutputFile$ += ".BAS"
      End Select
    End If

    Do While InputFile$ = OutputFile$
      TmpFile$ = Left(InputFile$, InStr(InputFile$, ".")) + "BAK"
      On Error GoTo FileErr1
      Rename(InputFile$, TmpFile$)
      On Error GoTo 0
      If TmpFile$ <> "" Then InputFile$ = TmpFile$
    Loop

    Return

FileErr1:
    CLS()
    PRINT("      Invalid file name") : PRINT()
    INPUT("      New input file name (ENTER to terminate): ", InputFile$)
    If InputFile$ = "" Then End

  End Sub

  '
  ' GetToken$:
  '  Extracts tokens from a string. A token is a word that is surrounded
  '  by separators, such as spaces or commas. Tokens are extracted and
  '  analyzed when parsing sentences or commands. To use the GetToken$
  '  function, pass the string to be parsed on the first call, then pass
  '  a null string on subsequent calls until the function returns a null
  '  to indicate that the entire string has been parsed.
  ' Input:
  '  Search$ = string to search
  '  Delim$  = String of separators
  ' Output:
  '  GetToken$ = next token
  '
  Function GetToken$(Search$, Delim$) REM Static

    Static BegPos
    Static SaveStr$
    Static NewPos

    ' Note that SaveStr$ and BegPos must be static from call to call
    ' (other variables are only static for efficiency).
    ' If first call, make a copy of the string
    If (Search$ <> "") Then
      BegPos = 1
      SaveStr$ = Search$
    End If

    ' Find the start of the next token
    NewPos = StrSpn(Mid(SaveStr$, BegPos, Len(SaveStr$)), Delim$)
    If NewPos Then
      ' Set position to start of token
      BegPos = NewPos + BegPos - 1
    Else
      ' If no new token, quit and return null
      GetToken$ = ""
      Exit Function
    End If

    ' Find end of token
    NewPos = StrBrk(Mid(SaveStr$, BegPos, Len(SaveStr$)), Delim$)
    If NewPos Then
      ' Set position to end of token
      NewPos = BegPos + NewPos - 1
    Else
      ' If no end of token, return set to end a value
      NewPos = Len(SaveStr$) + 1
    End If
    ' Cut token out of search string
    GetToken$ = Mid(SaveStr$, BegPos, NewPos - BegPos)
    ' Set new starting position
    BegPos = NewPos

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
  Sub InitKeyTable() REM Static

    Static KeyWord$
    Static Count

    RESTORE() REM 'KeyData
    For Count = 1 To KeyWordCount
      READ(KeyWord$)
      KeyWordTable$(Count) = KeyWord$
    Next

  End Sub

  '
  ' IsDigit:
  '  Returns true if character passed is a decimal digit. Since any
  '  Basic token starting with a digit is a number, the function only
  '  needs to check the first digit. Doesn't check for negative numbers,
  '  but that's not needed here.
  ' Input:
  '  Char$ - initial character of string to check
  ' Output:
  '  IsDigit - true if within 0 - 9
  '
  Function IsDigit(Char$) REM Static

    Static CharAsc

    If (Char$ = "") Then
      IsDigit = False
    Else
      CharAsc = Asc(Char$)
      IsDigit = (CharAsc >= Asc("0")) And (CharAsc <= Asc("9"))
    End If

  End Function

  '
  ' StrBrk:
  '  Searches InString$ to find the first character from among those in
  '  Separator$. Returns the index of that character. This function can
  '  be used to find the end of a token.
  ' Input:
  '  InString$ = string to search
  '  Separator$ = characters to search for
  ' Output:
  '  StrBrk = index to first match in InString$ or 0 if none match
  '
  Function StrBrk(InString$, Separator$) REM Static

    Static Ln
    Static BegPos

    Ln = Len(InString$)
    BegPos = 1
    ' Look for end of token (first character that is a delimiter).
    Do While InStr(Separator$, Mid(InString$, BegPos, 1)) = 0
      If BegPos > Ln Then
        StrBrk = 0
        Exit Function
      Else
        BegPos += 1
      End If
    Loop
    StrBrk = BegPos

  End Function

  '
  ' StrSpn:
  '  Searches InString$ to find the first character that is not one of
  '  those in Separator$. Returns the index of that character. This
  '  function can be used to find the start of a token.
  ' Input:
  '  InString$ = string to search
  '  Separator$ = characters to search for
  ' Output:
  '  StrSpn = index to first nonmatch in InString$ or 0 if all match
  '
  Function StrSpn%(InString$, Separator$) REM Static

    Dim Ln = Len(InString$)
    Dim BegPos = 1
    ' Look for start of a token (character that isn't a delimiter).
    Do While InStr(Separator$, Mid(InString$, BegPos, 1))
      If BegPos > Ln Then
        StrSpn% = 0
        Exit Function
      Else
        BegPos += 1
      End If
    Loop
    StrSpn% = BegPos

  End Function

End Module