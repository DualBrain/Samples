Option Explicit On
Option Infer On
Option Strict On

Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
'Imports Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory

Public Class VbRewriter
  Inherits VisualBasicSyntaxRewriter

  Public Sub New()

  End Sub

  '  Private Function CreateInitializer(identifier As String, type As String, isArray As Boolean) As StatementSyntax
  '    If isArray Then
  '      Return SyntaxFactory.ParseExecutableStatement($"For Index = 0 To {identifier}.Length - 1{Environment.NewLine}" &
  '$"Call {identifier}(Index).Init{type}(){Environment.NewLine}" &
  '$"Next{Environment.NewLine}")
  '    Else
  '      Return SyntaxFactory.ParseExecutableStatement($"Call {identifier}.Init{type}()")
  '    End If
  '  End Function

  Public Overrides Function VisitModuleBlock(node As ModuleBlockSyntax) As SyntaxNode

    Dim initInvocations = New SyntaxList(Of StatementSyntax)
    Dim space = SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ")
    Dim newline1 = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, Environment.NewLine)

    For a = 0 To node.Members.Count - 1
      If node.Members(a).IsKind(SyntaxKind.FieldDeclaration) Then
        For Each d In TryCast(node.Members(a), FieldDeclarationSyntax).Declarators
          'If StructuresWithInitializer.Contains(d?.AsClause?.Type().WithoutTrivia().ToFullString()) Then
          '  For Each name In d.Names
          '    If name.ArrayBounds IsNot Nothing Then
          '      initInvocations = initInvocations.Add(CreateInitializer(name.Identifier.ToFullString().Trim(), d.AsClause.Type().WithoutTrivia().ToString(), True).WithTrailingTrivia(newline1))
          '    Else
          '      initInvocations = initInvocations.Add(CreateInitializer(name.Identifier.ToFullString().Trim(), d.AsClause.Type().WithoutTrivia().ToString(), False).WithTrailingTrivia(newline1))
          '    End If
          '  Next
          'End If
        Next
      End If
    Next

    If initInvocations.Count > 0 Then
      Dim subStart = SyntaxFactory.SubStatement(SyntaxFactory.Identifier("New()").WithLeadingTrivia(space)).WithLeadingTrivia(newline1).WithTrailingTrivia(newline1)
      Dim subEnd = SyntaxFactory.EndSubStatement(
          SyntaxFactory.Token(SyntaxKind.EndKeyword, "End ").WithLeadingTrivia(newline1), SyntaxFactory.Token(SyntaxKind.SubKeyword, "Sub")).WithTrailingTrivia(newline1)

      Dim moduleConstructor = SyntaxFactory.SubBlock(subStart, initInvocations, subEnd)

      node = node.WithMembers(node.Members.Add(moduleConstructor))
    End If

    Return MyBase.VisitModuleBlock(node)
  End Function

  Public Overrides Function VisitExpressionStatement(node As ExpressionStatementSyntax) As SyntaxNode
    If node.ContainsDiagnostics Then
      Console.WriteLine("===")
      Console.WriteLine($"[{node}]")
      Console.WriteLine("---")
      For Each diag In node.GetDiagnostics
        Console.WriteLine($"DIAG: {diag}")
      Next
      Console.WriteLine("---")
      Select Case node.GetFirstToken.ToString.ToLower
        Case "print"

          ' PRINT Name$;
          ' ---->
          ' CALL System.Console.Write(Name$)

          ' PRINT Name$
          ' ---->
          ' CALL System.Console.WriteLine(Name$)

          ' Replace first token with "CALL System.Console.Write" or "CALL System.Console.WriteLine"
          ' Insert ( before following expression.
          ' Remove ending ; (if exists).
          ' Add (append) ) to the end of the line (before EndOfLineTrivia).

          Console.WriteLine("Found a PRINT statement!")
          Console.WriteLine("---")
          For Each n In node.DescendantTokens
            For Each entry In n.LeadingTrivia
              Console.Write($"-{entry.Kind}:{entry}- ")
            Next
            Console.Write($"[{n.Kind}:{n}] ")
            For Each entry In n.TrailingTrivia
              Console.Write($"+{entry.Kind}:{entry}+ ")
            Next
          Next
          Console.WriteLine($"{vbCrLf}---")
          'For Each n In node.DescendantNodes
          '  Console.Write($"[{node.Kind}:{n}] ")
          'Next
          'Console.WriteLine($"{vbCrLf}---")
          'Console.WriteLine(node.HasLeadingTrivia)
          'If node.HasTrailingTrivia Then
          '  For Each entry In node.GetTrailingTrivia
          '    If entry.IsKind(SyntaxKind.EndOfLineTrivia) Then
          '      Console.WriteLine($"[{entry.Kind}]")
          '    Else
          '      Console.WriteLine($"[{entry.Kind}:{entry}]")
          '    End If
          '  Next
          'End If

          Dim invokedExpression = node.Expression

          Dim collector As String = ""

          Dim noCr = False
          Dim argsFound = New List(Of String)
          For Each n In node.DescendantTokens
            Dim text = $"{n}"
            If n.HasLeadingTrivia Then
              For Each entry In n.LeadingTrivia
                If entry.IsKind(SyntaxKind.SkippedTokensTrivia) Then
                  If $"{entry}" = ";" Then
                    noCr = True
                  End If
                End If
              Next
            End If
            If n.IsKind(SyntaxKind.IdentifierToken) Then
              If text.ToLower = "print" Then
              Else
                argsFound.Add(text)
              End If
            End If
            If n.HasTrailingTrivia Then
              For Each entry In n.TrailingTrivia
                If entry.IsKind(SyntaxKind.SkippedTokensTrivia) Then
                  If $"{entry}" = ";" Then
                    noCr = True
                  End If
                End If
              Next
            End If
          Next

          Dim systemIdentifier = SyntaxFactory.IdentifierName("System")
          Dim consoleIdentifier = SyntaxFactory.IdentifierName("Console")
          Dim writeLineIdentifier = SyntaxFactory.IdentifierName(If(noCr, "Write", "WriteLine"))
          Dim s = SyntaxFactory.Token(SyntaxKind.DotToken)
          Dim systemConsoleIdentifier = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, systemIdentifier, s, consoleIdentifier)
          Dim method = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, systemConsoleIdentifier, s, writeLineIdentifier)

          Dim args As ArgumentListSyntax = Nothing
          If Not argsFound.Any Then
          ElseIf argsFound.Count = 1 Then
            Dim param1 = SyntaxFactory.IdentifierName(argsFound(0))
            Dim arg = SyntaxFactory.SimpleArgument(param1)
            Dim sepList = SyntaxFactory.SingletonSeparatedList(Of ArgumentSyntax)(arg)
            args = SyntaxFactory.ArgumentList(sepList)
          Else
            Dim param1 = SyntaxFactory.IdentifierName(argsFound(0))
            Dim arg = SyntaxFactory.SimpleArgument(param1)
            Dim sepList = SyntaxFactory.SingletonSeparatedList(Of ArgumentSyntax)(arg)
            For more = 1 To argsFound.Count - 1
              Dim paramMore = SyntaxFactory.IdentifierName(argsFound(more))
              Dim argMore = SyntaxFactory.SimpleArgument(paramMore)
              sepList = sepList.Add(argMore)
            Next
            args = SyntaxFactory.ArgumentList(sepList)
          End If
          Dim invocation = SyntaxFactory.InvocationExpression(method, args)
          Dim statement = SyntaxFactory.ExpressionStatement(invocation)

          If invokedExpression.HasLeadingTrivia Then
            statement = statement.WithLeadingTrivia(invokedExpression.GetLeadingTrivia())
          Else
            Dim trivia = invokedExpression.GetLeadingTrivia()
            trivia = trivia.Insert(0, SyntaxFactory.Whitespace("    "))
            statement = statement.WithLeadingTrivia(trivia)
          End If
          If invokedExpression.HasTrailingTrivia Then
            Dim trivia = invokedExpression.GetTrailingTrivia()
            trivia = trivia.Insert(0, SyntaxFactory.Whitespace(" "))
            statement = statement.WithTrailingTrivia(trivia)
          End If

          ' Replace the whole original expression with the New expression
          Return statement 'Document.WithSyntaxRoot((await document.GetSyntaxRootAsync()).ReplaceNode(invokedExpression, replaced))

        Case Else
          Return MyBase.VisitExpressionStatement(node)
      End Select
    Else
      Return MyBase.VisitExpressionStatement(node)
    End If
  End Function

  Public Overrides Function VisitMethodBlock(node As MethodBlockSyntax) As SyntaxNode
    For a As Integer = 0 To node.Statements.Count - 1
      If node.Statements(a).IsKind(SyntaxKind.LocalDeclarationStatement) Then
        Dim initInvocations As Microsoft.CodeAnalysis.SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)

        For Each d In TryCast(node.Statements(a), LocalDeclarationStatementSyntax).Declarators
          'If StructuresWithInitializer.Contains(d?.AsClause?.Type().WithoutTrivia().ToString()) Then
          '  For Each name In d.Names
          '    If name.ArrayBounds IsNot Nothing Then
          '      initInvocations = initInvocations.Add(CreateInitializer(name.Identifier.ToFullString().Trim(), d.AsClause.Type().WithoutTrivia().ToString(), True))
          '    Else
          '      initInvocations = initInvocations.Add(CreateInitializer(name.Identifier.ToFullString().Trim(), d.AsClause.Type().WithoutTrivia().ToString(), False))
          '    End If
          '  Next
          'End If
        Next

        For Each i As StatementSyntax In initInvocations
          node = node.WithStatements(node.Statements.Insert(a + 1, i.WithTrailingTrivia(SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, Environment.NewLine))))
        Next
      End If
    Next

    Return MyBase.VisitMethodBlock(node)
  End Function

  Public Overrides Function VisitArgumentList(node As ArgumentListSyntax) As SyntaxNode
    Dim arguments1 As SeparatedSyntaxList(Of ArgumentSyntax) = node.Arguments

    For i As Integer = 0 To arguments1.Count - 1
      If arguments1(i).IsKind(SyntaxKind.RangeArgument) Then
        If arguments1(i).GetFirstToken().IsKind(SyntaxKind.IntegerLiteralToken) Then
          If arguments1(i).GetFirstToken().ValueText = "1" Then
            Dim newStart = SyntaxFactory.IntegerLiteralToken("0 ", LiteralBase.[Decimal], TypeCharacter.IntegerLiteral, 0)
            Dim newArgument = arguments1(i).ReplaceToken(arguments1(i).GetFirstToken(), newStart)
            arguments1 = arguments1.Replace(arguments1(i), newArgument)
          End If
        End If
      End If
    Next

    Return node.WithArguments(arguments1)

  End Function

End Class