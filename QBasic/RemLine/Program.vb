Option Explicit On
Option Strict On
Option Infer On

' Requires a reference to QB.Compatibility
' Can be found at https://github.com/DualBrain/QB.Compatibility

Imports QB.Console
Imports QB.File

Module RemLineNew

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

  ' Global and constant data
  Const MaxLines = 10000

  Private ReadOnly LineTable(MaxLines) As Single
  Private LineCount As Integer
  Private Seps As String, InputFile As String, OutputFile As String, TmpFile As String

  ' Keyword search data
  Const KeyWordCount = 9
  Private ReadOnly KeywordTable() As String = {"", "THEN", "ELSE", "GOSUB", "GOTO", "RESUME", "RETURN", "RESTORE", "RUN", "ERL", ""}

  Public Sub Main()

    ' Start of module-level program code
    Seps = " ,:=<>()" + Chr(9)
    'GetFileNames()
    InputFile = "c:\bas\cube.bas"
    OutputFile = "c:\bas\cubeqb45.bas"
    On Error GoTo FileErr1
    OPEN(InputFile, OpenMode.Input, 1)
    On Error GoTo 0
    COLOR(7) : PRINT("Working", True) : COLOR(23) : PRINT(" . . .") : COLOR(7) : PRINT()
    BuildTable()
    CLOSE(1)
    OPEN(InputFile, OpenMode.Input, 1)
    On Error GoTo FileErr2
    OPEN(OutputFile, OpenMode.Output, 2)
    On Error GoTo 0
    GenOutFile()
    CLOSE(1, 2)
    If OutputFile <> "CON" Then CLS()

    End

FileErr1:
    CLS()
    PRINT("      Invalid file name") : PRINT()
    INPUT("      New input file name (ENTER to terminate): ", InputFile)
    If InputFile = "" Then End
FileErr2:
    INPUT("      Output file name (ENTER to print to screen) :", OutputFile)
    PRINT()
    If (OutputFile = "") Then OutputFile = "CON"
    If TmpFile = "" Then
      Resume
    Else
      TmpFile = ""
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
  '   Uses globals KeywordTable, KeyWordCount, and Seps
  ' Output:
  '   Modifies LineTable and LineCount
  '
  Sub BuildTable() REM Static

    'Static inlin As String
    'Static token As String

    Dim inlin As String
    Dim token As String

    Do While Not EOF(1)
      ' Get line and first token
      LINE_INPUT(1, Inlin)
      token = GetToken(inlin, Seps)
      Do While (token <> "")
        For KeyIndex = 1 To KeyWordCount
          ' See if token is keyword
          If (KeywordTable(KeyIndex) = UCase(token)) Then
            ' Get possible line number after keyword
            token = GetToken("", Seps)
            ' Check each token to see if it is a line number
            ' (the LOOP is necessary for the multiple numbers
            ' of ON GOSUB or ON GOTO). A non-numeric token will
            ' terminate search.
            Do While (IsDigit(Left(token, 1)))
              LineCount += 1
              LineTable(LineCount) = CSng(Val(token))
              token = GetToken("", Seps)
              If token <> "" Then KeyIndex = 0
            Loop
          End If
        Next KeyIndex
        ' Get next token
        token = GetToken("", Seps)
      Loop
    Loop

  End Sub

  '
  ' GenOutFile:
  '  Generates an output file with unreferenced line numbers removed.
  ' Input:
  '  Uses globals LineTable, LineCount, and Seps
  ' Output:
  '  Processed file
  '
  Sub GenOutFile() REM Static

    ' Speed up by eliminating comma and colon (can't separate first token)
    Dim sep = $" {vbTab}"

    Do While Not EOF(1)

      Dim inLin = "" : LINE_INPUT(1, inLin)

      If (inLin <> "") Then

        ' Get first token and process if it is a line number
        Dim token = GetToken(inLin, sep)

        If IsDigit(Left(token, 1)) Then

          Dim lineNumber! = CSng(Val(token))
          Dim foundNumber = False

          ' See if line number is in table of referenced line numbers
          For index = 1 To LineCount
            If (lineNumber = LineTable(index)) Then
              foundNumber = True
            End If
          Next

          ' Modify line strings
          If 1 = 0 Then

            If Not foundNumber Then
              token = Space(Len(token))
              MID(inLin, StrSpn(inLin, sep), Len(token)) = token
            End If

          Else

            ' You can replace the previous lines with your own
            ' code to reformat output. For example, try these lines:

            Dim tmpPos1 = StrSpn(inLin, sep) + Len(token)
            Dim tmpPos2 = tmpPos1 + StrSpn(Mid(inLin, tmpPos1), sep) - 1

            If foundNumber Then
              inLin = $"{Left(inLin, tmpPos1 - 1)}:{vbCrLf}{vbTab}{Mid(inLin, tmpPos2)}"
            Else
              inLin = $"{vbTab}{Mid(inLin, tmpPos2)}"
            End If
          End If

        End If
      End If

      ' Print line to file or console (PRINT is faster than console device)
      If OutputFile = "CON" Then
        PRINT(inLin)
      Else
        PRINT(2, inLin)
      End If

    Loop

  End Sub

  '
  ' GetFileNames:
  '  Gets a file name by prompting the user.
  ' Input:
  '  User input
  ' Output:
  '  Defines InputFiles and OutputFiles
  '
  Sub GetFileNames()

    CLS()
    PRINT(" Microsoft RemLine: Line Number Removal Utility")
    PRINT("       (.BAS assumed if no extension given)")
    PRINT()
    INPUT("      Input file name (ENTER to terminate): ", InputFile)
    If InputFile = "" Then End
    INPUT("      Output file name (ENTER to print to screen): ", OutputFile)
    PRINT()
    If OutputFile = "" Then OutputFile = "CON"

    If InStr(InputFile, ".") = 0 Then
      InputFile &= ".BAS"
    End If

    If InStr(OutputFile, ".") = 0 Then
      Select Case OutputFile
        Case "CON", "SCRN", "PRN", "COM1", "COM2", "LPT1", "LPT2", "LPT3"
          Exit Sub
        Case Else
          OutputFile &= ".BAS"
      End Select
    End If

    Do While InputFile = OutputFile
      TmpFile = Left(InputFile, InStr(InputFile, ".")) + "BAK"
      On Error GoTo FileErr1
      NAME(InputFile, TmpFile)
      On Error GoTo 0
      If TmpFile <> "" Then InputFile = TmpFile
    Loop

    Return

FileErr1:
    CLS()
    PRINT("      Invalid file name") : PRINT()
    INPUT("      New input file name (ENTER to terminate): ", InputFile)
    If InputFile = "" Then End

  End Sub

  ''' <summary>
  ''' Extracts tokens from a string. A token is a word that is surrounded
  ''' by separators, such as spaces or commas. Tokens are extracted and
  ''' analyzed when parsing sentences or commands. To use the GetToken
  ''' function, pass the string to be parsed on the first call, then pass
  ''' a null string on subsequent calls until the function returns a null
  ''' to indicate that the entire string has been parsed.
  ''' </summary>
  ''' <param name="search">string to search</param>
  ''' <param name="delim">String of separators</param>
  ''' <returns>next token</returns>
  Function GetToken(search As String, delim As String) As String REM Static

    Static begPos As Integer
    Static saveStr As String

    ' Note that SaveStr and BegPos must be static from call to call
    ' (other variables are only static for efficiency).
    ' If first call, make a copy of the string
    If (search <> "") Then
      begPos = 1
      saveStr = search
    End If

    ' Find the start of the next token
    Dim newPos = StrSpn(Mid(saveStr, begPos, Len(saveStr)), delim)
    If newPos <> 0 Then
      ' Set position to start of token
      begPos = newPos + begPos - 1
    Else
      ' If no new token, quit and return null
      Return ""
    End If

    ' Find end of token
    newPos = StrBrk(Mid(saveStr, begPos, Len(saveStr)), delim)
    If newPos <> 0 Then
      ' Set position to end of token
      newPos = begPos + newPos - 1
    Else
      ' If no end of token, return set to end a value
      newPos = Len(saveStr) + 1
    End If

    ' Cut token out of search string
    Dim result = Mid(saveStr, begPos, newPos - begPos)
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
  '  Modifies global array KeywordTable
  '
  'Sub InitKeyTable()

  '  RESTORE() REM 'KeyData
  '  For Count = 1 To KeyWordCount
  '    Dim Keyword = Nothing
  '    READ(Keyword)
  '    KeywordTable(Count) = Keyword
  '  Next

  'End Sub

  ''' <summary>
  ''' Returns true if character passed is a decimal digit. Since any 
  ''' Basic token starting with a digit is a number, the function only
  ''' needs to check the first digit. Doesn't check for negative numbers,
  ''' but that's not needed here.
  ''' </summary>
  ''' <param name="char">initial character of string to check</param>
  ''' <returns>true if within 0 - 9</returns>
  Function IsDigit([char] As String) As Boolean

    If ([char] = "") Then
      Return False
    Else
      Dim charAsc = Asc([char])
      Return (charAsc >= Asc("0")) AndAlso (charAsc <= Asc("9"))
    End If

  End Function

  ''' <summary>
  ''' Searches InString to find the first character from among those in
  ''' Separator. Returns the index of that character. This function can
  ''' be used to find the end of a token.
  ''' </summary>
  ''' <param name="inString">string to search</param>
  ''' <param name="separator">characters to search for</param>
  ''' <returns>index to first match in InString or 0 if none match</returns>
  Function StrBrk(inString As String, separator As String) As Integer

    Dim ln = Len(inString)
    Dim begPos = 1
    ' Look for end of token (first character that is a delimiter).
    Do While InStr(separator, Mid(inString, begPos, 1)) = 0
      If begPos > ln Then
        Return 0
      Else
        begPos += 1
      End If
    Loop
    Return begPos

  End Function

  ''' <summary>
  ''' Searches InString to find the first character that is not one of
  ''' those in Separator. Returns the index of that character. This
  ''' function can be used to find the start of a token.
  ''' </summary>
  ''' <param name="inString">string to search</param>
  ''' <param name="separator">characters to search for</param>
  ''' <returns>index to first nonmatch in InString or 0 if all match</returns>
  Function StrSpn(inString As String, separator As String) As Integer

    Dim ln = Len(inString)
    Dim begPos = 1
    ' Look for start of a token (character that isn't a delimiter).
    Do While InStr(separator, Mid(inString, begPos, 1)) <> 0
      If begPos > ln Then
        Return 0
      Else
        begPos += 1
      End If
    Loop
    Return begPos

  End Function

End Module