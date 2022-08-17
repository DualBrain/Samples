Imports System.Runtime.CompilerServices

Friend Module SyntaxFacts

  Public Function GetKeywordKind(text As String) As SyntaxKind

    Select Case text.ToLower
      Case Else
        Return SyntaxKind.IdentifierToken
    End Select

  End Function

  Public Function GetText(kind As SyntaxKind) As String

    Select Case kind
      Case SyntaxKind.PoundToken : Return "#"
      Case SyntaxKind.PlusToken : Return "+"
      Case SyntaxKind.MinusToken : Return "-"
      Case SyntaxKind.StarToken : Return "*"
      Case SyntaxKind.SlashToken : Return "/"
      Case SyntaxKind.BackslashToken : Return "\"
      Case SyntaxKind.HatToken : Return "^"
      Case SyntaxKind.OpenParenToken : Return "("
      Case SyntaxKind.CloseParenToken : Return ")"
      Case SyntaxKind.OpenBraceToken : Return "{"
      Case SyntaxKind.CloseBraceToken : Return "}"
      Case SyntaxKind.OpenBracketToken : Return "["
      Case SyntaxKind.CloseBracketToken : Return "]"
      Case SyntaxKind.EqualToken : Return "="
      Case SyntaxKind.LessThanToken : Return "<"
      Case SyntaxKind.PeriodToken : Return "."
      Case SyntaxKind.ColonToken : Return ":"
      Case SyntaxKind.CommaToken : Return ","
      Case SyntaxKind.SemicolonToken : Return ";"
      Case SyntaxKind.QuestionToken : Return "?"
      Case SyntaxKind.GreaterThanEqualToken : Return ">="
      Case SyntaxKind.LessThanEqualToken : Return "<="
      Case SyntaxKind.LessThanGreaterThanToken : Return "<>"
      Case SyntaxKind.GreaterThanToken : Return ">"
      Case SyntaxKind.QuestionToken : Return "?"
      Case Else
        Return Nothing
    End Select

  End Function

  <Extension>
  Public Function Is_Keyword(kind As SyntaxKind) As Boolean
    Return kind.ToString.EndsWith("Keyword")
  End Function

  <Extension>
  Public Function IsToken(kind As SyntaxKind) As Boolean
    Return (kind.Is_Keyword OrElse kind.ToString.EndsWith("Token"))
  End Function

End Module