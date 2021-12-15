Module Tokenizer

  Private m_text As String
  Private m_cursor As Integer = 0

  Public Function Parse(text As String) As List(Of Token) 'RawCompilationUnit

    m_text = text

    If String.IsNullOrWhiteSpace(text) Then
      Return Nothing
    End If

    Dim result = New List(Of Token) 'RawCompilationUnit

    'Dim statement = New RawStatement

    Do

      Dim rawResult = PopRawText()

      Dim location = rawResult.Location
      Dim leadingTrivia As String = Nothing
      Dim value As String = rawResult.Text
      Dim trailingTrivia As String = Nothing

      If value Is Nothing Then
        Exit Do
      End If

      Dim index = 0
      Do
        If index > rawResult.Text.Length - 1 Then Exit Do
        Select Case rawResult.Text(index)
          Case " "c, ChrW(9)
            If leadingTrivia Is Nothing Then leadingTrivia = ""
            leadingTrivia &= rawResult.Text(index)
          Case Else
            Exit Do
        End Select
        index += 1
      Loop

      index = rawResult.Text.Length - 1
      Do
        If index < 0 Then Exit Do
        Select Case rawResult.Text(index)
          Case " "c, ChrW(9), ChrW(10), ChrW(13)
            If trailingTrivia Is Nothing Then trailingTrivia = ""
            trailingTrivia = rawResult.Text(index) & trailingTrivia
          Case Else
            Exit Do
        End Select
        index -= 1
      Loop

      Dim lt = If(leadingTrivia Is Nothing, 0, leadingTrivia.Length)
      Dim tt = If(trailingTrivia Is Nothing, 0, trailingTrivia.Length)
      value = rawResult.Text.Substring(lt, rawResult.Text.Length - (lt + tt))

      Dim token = New Token With {.Location = CInt(location),
                                  .LeadingTrivia = leadingTrivia,
                                  .Text = value,
                                  .TrailingTrivia = trailingTrivia,
                                  .Length = rawResult.Text.Length}

      result.Add(token)

      'CollectToken(statement, token)
      'Select Case token.RawText
      '  Case ":", vbCrLf, vbCr, vbLf
      '    If result.RawStatements Is Nothing Then
      '      result.RawStatements = New List(Of RawStatement)
      '    End If
      '    result.RawStatements.Add(statement)
      '    statement = New RawStatement
      '  Case Else
      'End Select

    Loop

    Return result

  End Function

  Private Function PopRawText() As (Text As String, Location As Integer?)

    Dim text As String = ""

    Dim currentIndex = m_cursor
    Dim whitespace = True
    Dim quoted = False
    Do
      Dim ch = PeekChar()
      If ch Is Nothing Then Exit Do
      Select Case ch
        Case ":"c, "("c, ")"c, "{"c, "}"c, "."c,
             "+"c, "-"c, "*"c, "/"c, "\"c, "%"c, "^"c,
             "!"c, "&"c, "#"c, "$"c, "="c, "<"c, ">"c, ","c, "'"c,
             "?"c, ";"c
          If quoted Then
            text &= ch : m_cursor += 1
          Else
            If whitespace Then
              ' was still collecting whitespace...
              text &= ch : m_cursor += 1
              ' check to see if whitespace and/or crlf???
              Do
                ch = PeekChar()
                Select Case ch
                  Case " "c, ChrW(9)
                    text &= ch : m_cursor += 1
                  Case ChrW(13)
                    text &= ch : m_cursor += 1
                    ch = PeekChar()
                    If ch = ChrW(10) Then
                      text &= ch : m_cursor += 1
                    End If
                    Exit Do
                  Case ChrW(10)
                    text &= ch : m_cursor += 1
                    Exit Do
                  Case Else
                    Exit Do
                End Select
              Loop
              Exit Do
            Else
              ' we have some other sort of token collect...
              Exit Do
            End If
          End If
        Case " "c
          text &= ch : m_cursor += 1
          If Not quoted AndAlso Not whitespace Then Exit Do
        Case ChrW(9) ' vbTab
          ' append to "current token"
          text &= ch : m_cursor += 1
          If Not quoted AndAlso Not whitespace Then Exit Do
        Case ChrW(13) ' vbCr
          text &= ch : m_cursor += 1
          If Not quoted Then
            ch = PeekChar()
            If ch = ChrW(10) Then
              text &= ch : m_cursor += 1
              Exit Do
            Else
              Exit Do
            End If
          End If
        Case ChrW(10) ' vbLf
          If Not quoted Then
            text &= ch : m_cursor += 1
            Exit Do
          End If
        Case ChrW(34)
          'TODO: need to handle 'escaped' quote
          If Not quoted Then
            ' beginning of quoted string...
            text &= ch : m_cursor += 1
            quoted = True
          Else
            ' finish up quoted string...
            text &= ch : m_cursor += 1
            quoted = False
          End If
        Case Else
          text &= ch : m_cursor += 1
          If Not quoted Then whitespace = False
      End Select
    Loop

    If text = "" Then
      Return (Nothing, Nothing)
    Else
      Return (text, currentIndex)
    End If

  End Function

  Private Function PeekChar() As Char?
    If m_cursor > m_text.Length - 1 Then Return Nothing
    Return m_text(m_cursor)
  End Function

  Private Function PopChar() As Char?
    If m_cursor > m_text.Length - 1 Then Return Nothing
    Try
      Return m_text(m_cursor)
    Finally
      m_cursor += 1
    End Try
  End Function

  'Private Sub CollectToken(statement As RawStatement, token As RawToken)
  '  If statement.RawTokens Is Nothing Then
  '    statement.RawTokens = New List(Of RawToken)
  '  End If
  '  statement.RawTokens.Add(token)
  'End Sub

End Module

Class Token
  Public Property Location As Integer
  Public Property LeadingTrivia As String
  Public Property Text As String
  Public Property TrailingTrivia As String
  Public Property Length As Integer
End Class

'Class Token
'  Public Location As Integer
'  Public Length As Integer
'  Public RawText As String
'  Public Overrides Function ToString() As String
'    Return $"'{RawText}',{Location},{Length}"
'  End Function
'End Class