'Public MustInherit Class Token

'  Public Location As Integer
'  Public Length As Integer
'  Public RawText As String

'  Public Overrides Function ToString() As String
'    Return $"'{RawText}',{Location},{Length}"
'  End Function

'  Public MustOverride Function Copy() As Token

'  Public ReadOnly Property Keyword() As String
'    Get

'      ' If a reserved word, return the word, otherwise return nothing.

'      If TypeOf Me Is KeywordToken Then
'        Return DirectCast(Me, KeywordToken).Value
'      ElseIf TypeOf Me Is FunctionToken Then
'        Return DirectCast(Me, FunctionToken).Value
'      ElseIf TypeOf Me Is CommandToken Then
'        Return DirectCast(Me, CommandToken).Value
'      ElseIf TypeOf Me Is VariableToken Then
'        Return DirectCast(Me, VariableToken).Value
'      ElseIf TypeOf Me Is CommentToken Then
'        Return "REM"
'      Else
'        Return Nothing
'      End If

'    End Get
'  End Property

'  Public ReadOnly Property Literal() As String
'    Get

'      ' If a literal, returns the value of the literal, otherwise returns nothing.

'      If TypeOf Me Is StringLiteralToken Then
'        Return DirectCast(Me, StringLiteralToken).Value
'      ElseIf TypeOf Me Is NumericLiteralToken Then
'        Return DirectCast(Me, NumericLiteralToken).Value
'      ElseIf TypeOf Me Is LabelToken Then
'        Return DirectCast(Me, LabelToken).Value
'      Else
'        Return Nothing
'      End If

'    End Get
'  End Property

'End Class

'Friend Class EndOfStatementToken
'  Inherits Token
'  Public Overrides Function ToString() As String
'    Return ":"
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New EndOfStatementToken
'  End Function
'End Class

'Friend Class KeywordToken
'  Inherits Token
'  Public Property Value As String
'  Public Overrides Function ToString() As String
'    Return Value
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New KeywordToken With {.Value = Value}
'  End Function
'End Class

'Friend Class FunctionToken
'  Inherits Token
'  Public Property Value As String
'  Public Overrides Function ToString() As String
'    Return Value
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New FunctionToken With {.Value = Value}
'  End Function
'End Class

'Friend Class CommandToken
'  Inherits Token
'  Public Property Value As String
'  Public Overrides Function ToString() As String
'    Return Value
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New CommandToken With {.Value = Value}
'  End Function
'End Class

'Friend Class VariableToken
'  Inherits Token
'  Public Property Value As String
'  Public Overrides Function ToString() As String
'    Return Value
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New VariableToken With {.Value = Value}
'  End Function
'End Class

''Public Class OperatorToken
''  Inherits Token
''  Public Property Value As String
''  Public Overrides Function ToString() As String
''    Return Value
''  End Function
''End Class

'Friend Class ArithmaticOperatorToken
'  Inherits Token
'  Public Property Value As String
'  Public Overrides Function ToString() As String
'    Return Value
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New ArithmaticOperatorToken With {.Value = Value}
'  End Function
'End Class

'Friend Class RelationalOperatorToken
'  Inherits Token
'  Public Property Value As String
'  Public Overrides Function ToString() As String
'    Return Value
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New RelationalOperatorToken With {.Value = Value}
'  End Function
'End Class

'Friend Class LabelToken
'  Inherits Token
'  Public Property Value As String
'  Public Overrides Function ToString() As String
'    Return String.Format("{0}:", Value)
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New LabelToken With {.Value = Value}
'  End Function
'End Class

'Friend Class IdentifierToken
'  Inherits Token
'  Public Property Value As String
'  Public Overrides Function ToString() As String
'    Return Value
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New IdentifierToken With {.Value = Value}
'  End Function
'End Class

'Friend Class NumericLiteralToken
'  Inherits Token
'  Public Property Value As String
'  Public Overrides Function ToString() As String
'    Return Value
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New NumericLiteralToken With {.Value = Value}
'  End Function
'End Class

'Friend Class StringLiteralToken
'  Inherits Token
'  Public Property Value As String
'  Public Overrides Function ToString() As String
'    Return Value
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New StringLiteralToken With {.Value = Value}
'  End Function
'End Class

'Friend Class CommentToken
'  Inherits Token
'  Public Property Value As String
'  Public Overrides Function ToString() As String
'    If Value Is Nothing Then
'      Return String.Format("'")
'    Else
'      Return String.Format("'{0}", Value)
'    End If
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New CommentToken With {.Value = Value}
'  End Function
'End Class

'Friend Class DataToken
'  Inherits Token
'  Public Property Value As String
'  Public Overrides Function ToString() As String
'    If Value Is Nothing Then
'      Return ""
'    Else
'      Return Value
'    End If
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New CommentToken With {.Value = Value}
'  End Function
'End Class

''Public Class AdditionToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return "+"
''  End Function
''End Class

''Public Class AmpersandToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return "&"
''  End Function
''End Class

''Public Class AtToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return "@"
''  End Function
''End Class

'Friend Class CommaToken
'  Inherits Token
'  Public Overrides Function ToString() As String
'    Return ","
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New CommaToken
'  End Function
'End Class

'Friend Class PeriodToken
'  Inherits Token
'  Public Overrides Function ToString() As String
'    Return "."
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New PeriodToken
'  End Function
'End Class

''Public Class DivisionFloatingPointToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return "/"
''  End Function
''End Class

''Public Class DivisionIntegerToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return "\"
''  End Function
''End Class

''Public Class EqualityToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return "="
''  End Function
''End Class

''Public Class ExponentiationToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return "^"
''  End Function
''End Class

''Public Class GreaterThanOrEqualToToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return ">="
''  End Function
''End Class

''Public Class LessThanOrEqualToToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return "<="
''  End Function
''End Class

''Public Class GreaterThanToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return ">"
''  End Function
''End Class

''Public Class LessThanToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return "<"
''  End Function
''End Class

''Public Class InequalityToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return "<>"
''  End Function
''End Class

''Public Class MultiplicationToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return "*"
''  End Function
''End Class

'Friend Class HashToken
'  Inherits Token
'  Public Overrides Function ToString() As String
'    Return "#"
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New HashToken
'  End Function
'End Class

'Friend Class SemiColonToken
'  Inherits Token
'  Public Overrides Function ToString() As String
'    Return ";"
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New SemiColonToken
'  End Function
'End Class

''Public Class SubtractionToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return "-"
''  End Function
''End Class

''Public Class NegationToken
''  Inherits Token
''  Public Overrides Function ToString() As String
''    Return "-"
''  End Function
''End Class

'Friend Class ParenOpenToken
'  Inherits Token
'  Public Overrides Function ToString() As String
'    Return "("
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New ParenOpenToken
'  End Function
'End Class

'Friend Class ParenCloseToken
'  Inherits Token
'  Public Overrides Function ToString() As String
'    Return ")"
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New ParenCloseToken
'  End Function
'End Class

''Public Class UnknownToken
''  Inherits Token
''  Public Property Value As String
''  Public Overrides Function ToString() As String
''    Return Me.Value
''  End Function
''  Public Overrides Function Copy() As Token
''    Return New UnknownToken With {.Value = Me.Value}
''  End Function
''End Class

'Friend Class SyntaxErrorToken
'  Inherits Token
'  Public Property Code As Short
'  Public Property Message As String
'  Public Property Reason As String
'  Public Overrides Function ToString() As String
'    If Reason IsNot Nothing Then
'      Return String.Format("Syntax: {0} ({1})", Message, Reason)
'    Else
'      Return String.Format("Syntax: {0}", Message)
'    End If
'  End Function
'  Public Overrides Function Copy() As Token
'    Return New SyntaxErrorToken With {.Code = Code,
'                                      .Message = Message,
'                                      .Reason = Reason}
'  End Function
'End Class