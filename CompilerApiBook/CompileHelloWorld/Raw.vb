Public Class RawCompilationUnit
  Public Property RawStatements As List(Of RawStatement)
End Class

Public Class RawStatement
  Public Property RawTokens As List(Of RawToken)
End Class

Public Class RawToken
  Public Property RawText As String
  Public Property Location As Integer
  Public Property Length As Integer
End Class