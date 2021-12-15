Friend Interface IDialect
  ReadOnly Property Keywords As List(Of String)
  ReadOnly Property Functions As List(Of String)
  ReadOnly Property Commands As List(Of String)
  ReadOnly Property Variables As List(Of String)
  ReadOnly Property Operators As List(Of String)
  ReadOnly Property Symbols As List(Of Char)
  ReadOnly Property GroupingOperators As List(Of Char)
  ReadOnly Property ArithmaticOperators As List(Of Char)
  ReadOnly Property RelationalOperators As List(Of String)
  ReadOnly Property NumericSuffix As List(Of Char)
  ReadOnly Property StringSuffix As List(Of Char)
  ReadOnly Property ReservedWords As List(Of String)
  ReadOnly Property IgnoreAllWhiteSpace As Boolean
  ReadOnly Property SupportsLabels As Boolean
End Interface