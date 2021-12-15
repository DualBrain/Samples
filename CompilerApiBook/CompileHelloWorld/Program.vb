
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Public Module Program

  Public Sub Main1()

    ' (Optional) Preprocess input for "line-numbers".
    ' Parse into tokens.
    ' Evaluate syntax for "things" that need to be handled (whatever that means).
    ' Write out result.

    Dim references = {"mscorlib.dll",
                      "System.Runtime.dll",
                      "System.Console.dll",
                      "Microsoft.VisualBasic.Core.dll"}

    Dim source = File.ReadAllText("Program.bas")

    Dim tokens = Tokenizer.Parse(source.Trim)

    'For Each token In tokens
    '  'Dim trailingTrivia = token.TrailingTrivia?.Replace(vbCr, "{cr}")?.Replace(vbLf, "{lf}")
    '  'Console.WriteLine($"Leading: '{token.LeadingTrivia}' Value: '{token.Text}' Trailing: '{trailingTrivia}'")
    '  Console.Write($"{token.LeadingTrivia}{token.Text}{If(token.TrailingTrivia, " ")}")
    'Next

    Dim importsSection = ""
    Dim subMain = "    "

    Dim index = 0
    Do
      If index > tokens.Count - 1 Then Exit Do

      If String.Equals(tokens(index).Text, "IMPORTS", StringComparison.OrdinalIgnoreCase) Then
        importsSection &= $"{tokens(index).LeadingTrivia}{tokens(index).Text}{tokens(index).TrailingTrivia}" : index += 1
        Do
          importsSection &= $"{tokens(index).LeadingTrivia}{tokens(index).Text}{tokens(index).TrailingTrivia}"
          If tokens(index).TrailingTrivia?.Contains(ChrW(10)) Then
            Exit Do
          Else
            index += 1
          End If
        Loop
      ElseIf String.Equals(tokens(index).Text, "LET", StringComparison.OrdinalIgnoreCase) Then
        ' Suppress
      ElseIf String.Equals(tokens(index).Text, "PRINT", StringComparison.OrdinalIgnoreCase) OrElse
             tokens(index).Text = "?" Then
        'TODO: Need to peek forward enough (eol) to determine if a semicolon exists...
        subMain &= $"{tokens(index).LeadingTrivia}System.Console.WriteLine("
        Dim trailing = tokens(index).TrailingTrivia
        '-------
        If trailing?.Contains(vbLf) Then
          ' an empty print statement
        Else
          Do
            index += 1
            subMain &= $"{tokens(index).LeadingTrivia}{tokens(index).Text}"
            If tokens(index).TrailingTrivia?.Contains(vbLf) Then
              trailing = $"{tokens(index).TrailingTrivia}"
              Exit Do
            End If
          Loop
        End If
        '-------
        subMain &= ")"
        If trailing IsNot Nothing Then
          subMain &= $"{trailing}"
        End If
      ElseIf String.Equals(tokens(index).Text, "SUB", StringComparison.OrdinalIgnoreCase) OrElse
             String.Equals(tokens(index).Text, "FUNCTION", StringComparison.OrdinalIgnoreCase) Then
        'TODO: Subs/Functions can have precursor keywords...
        Exit Do
      Else
        subMain &= $"{tokens(index).LeadingTrivia}{tokens(index).Text}{tokens(index).TrailingTrivia}"
      End If
      index += 1
    Loop

    Dim code = $"Option Explicit Off
Option Strict Off
Option Infer Off

{importsSection}

Partial Module Program

  Sub Main(args As String())
{subMain}
  End Sub

"

    If index < tokens.Count - 1 Then
      Do
        If index > tokens.Count - 1 Then Exit Do
        Select Case tokens(index).Text
          Case Else
            code &= $"{tokens(index).LeadingTrivia}{tokens(index).Text}{tokens(index).TrailingTrivia}"
        End Select
        index += 1
      Loop
    End If

    code &= "
End Module"

    Console.WriteLine("=== VB ===")
    Console.WriteLine()
    Console.WriteLine(code)
    Console.WriteLine()

    Console.WriteLine("=== OUTPUT ===")

    ' "Compile and Execute"

    Dim coreDir = Directory.GetParent(GetType(Enumerable).GetTypeInfo().Assembly.Location)

    Dim tree = SyntaxFactory.ParseSyntaxTree(code)

    'Console.WriteLine(tree.GetRoot().DescendantNodesAndTokensAndSelf(Function() True, True).Count())

    Dim referencesMetadata = New List(Of MetadataReference) From {MetadataReference.CreateFromFile(GetType([Object]).GetTypeInfo().Assembly.Location)}
    For Each reference In references
      referencesMetadata.Add(MetadataReference.CreateFromFile(Path.Combine(coreDir.FullName, reference)))
    Next

    Dim compilation = VisualBasicCompilation.Create("HelloWorldCompiled.exe",
                                                   options:=New VisualBasicCompilationOptions(OutputKind.ConsoleApplication),
                                                   syntaxTrees:={tree},
                                                   references:=referencesMetadata)

    Using stream = New MemoryStream
      Dim compileResult = compilation.Emit(stream)
      If compileResult.Diagnostics.Any Then
        For Each diag In compileResult.Diagnostics
          Console.WriteLine(diag.ToString)
        Next
      Else
        Dim assembly1 = Assembly.Load(stream.GetBuffer())
        assembly1.EntryPoint.Invoke(Nothing,
                                  BindingFlags.NonPublic Or BindingFlags.[Static],
                                  Nothing,
                                  New Object() {Nothing},
                                  Nothing)
      End If
    End Using

  End Sub

  Public Sub Main()

    Dim code = "Option Explicit On

Imports System.Drawing

10: 
  Hello$ = ""Hello World!""
  System.Console.WriteLine(Hello$)
  PRINT ""HELLO WORLD!""
  ? Hello$
  ? ""The sky is falling...""
  ? 10 + 3
  GOTO 10
  END

Function Testing() As Integer
  Return 1
End Function

"

    Dim tree = SyntaxFactory.ParseSyntaxTree(code)

    Dim root = tree.GetRoot ' Gives us our CompiliationUnit...

    Dim syntax = tree.GetCompilationUnitRoot

    For Each entry In syntax.Options
      Console.WriteLine($"{entry.Span}:{entry}")
    Next

    For Each entry In syntax.Imports
      Console.WriteLine($"{entry.Span}:{entry}")
    Next

    Console.WriteLine("-------")

    For Each entry In syntax.Members
      If entry.Kind = SyntaxKind.ExpressionStatement Then
        Console.WriteLine("=======")
        Console.WriteLine($"{entry} ' [{entry.Kind}]")
        Console.WriteLine($"  {entry.ChildNodesAndTokens.First} ' [{entry.ChildNodesAndTokens.First.Kind}]")
        PrintTree(entry.ChildNodesAndTokens.First, 2)
        Console.WriteLine("=======")
      ElseIf entry.Kind = SyntaxKind.PrintStatement Then
        Console.WriteLine("==| '?' (aka PRINT) |==")
        Console.WriteLine($"{entry} ' [{entry.Kind}]")
        Console.WriteLine($" \ {entry.ChildNodesAndTokens.First} ' [{entry.ChildNodesAndTokens.First.Kind}]")
        PrintTree(entry.ChildNodesAndTokens.First, 2)
        Console.WriteLine("=======")
      Else
        Console.WriteLine($"{entry.Span}:{entry}")
      End If
    Next

    Return


    ' Notes:
    '   Does appear to properly separate multiple "top level node(s)" per line... ie. separated by the colon character
    '   Also appears to segment methods as a "single level node".
    '   Classes also appear to be treated as a "single level node".
    For Each node In root.ChildNodes()
      Select Case node.Kind
        Case SyntaxKind.PrintStatement
          Console.WriteLine($"{node} ---> System.Console.Writeline(...)")
        Case SyntaxKind.ExpressionStatement

          Console.WriteLine("=======")
          Console.WriteLine($"{node} ' [{node.Kind}]")
          PrintTree(node, 1)
          Console.WriteLine("=======")

          Dim expression = node.ChildNodes.First
          If expression?.Kind = SyntaxKind.InvocationExpression Then
            If expression.ChildNodesAndTokens.First.Kind = SyntaxKind.IdentifierName Then
              Dim value = expression.ChildNodesAndTokens.First.ToString
              If value.ToLower = "print" Then
                Console.WriteLine("We have a PRINT statement...")
                Continue For
              End If
            End If
          End If

          'Console.WriteLine($"{node.Kind}: {node}")

        Case Else
          Console.WriteLine($"{node.Kind}: {node}")
      End Select
    Next

    Return

    Dim nodes = root.DescendantNodesAndTokensAndSelf(Function() True, True)

    For Each node In nodes
      Console.WriteLine($"{node.Kind}, {node}")
    Next

    Console.WriteLine("-")

    Dim methods As IEnumerable(Of StatementSyntax) = tree.GetRoot().DescendantNodes(Function() True).OfType(Of StatementSyntax)()

    For Each method In methods
      If method.Kind = SyntaxKind.PrintStatement Then
        ' ? Hello$
        ' ? "HELLO WORLD!"
        ' NOTE: This does not add parens...
        Console.WriteLine("*******")
        'Dim children = method.DescendantNodesAndTokensAndSelf(Function() True, True)
        'For Each child In children
        '  Console.WriteLine($"'{child.Kind}, {child}")
        'Next

        'PrintStatement, ? Hello$
        'QuestionToken, ?
        'IdentifierName, Hello$
        'IdentifierToken, Hello$

        Console.WriteLine(method.ToString)
        Console.WriteLine("-----> ")

        Dim parameters As IEnumerable(Of IdentifierNameSyntax) = method.DescendantNodes(Function() True).OfType(Of IdentifierNameSyntax)()

        ' ExpressionStatement+InvocationExpression
        ' SimpleMemberAccessExpression
        ' IdentifierName+IdentifierToken
        Console.Write("System")
        ' DotToken
        Console.Write(".")
        ' IdentifierName+IdentifierToken
        Console.Write("Console")
        ' DotToken
        Console.Write(".")
        ' IdentifierName+IdentifierToken
        Console.Write("WriteLine")
        ' ArgumentList
        Console.Write("(") ' OpenParentToken
        Dim skipped = False
        For Each param In parameters
          If Not skipped Then
            skipped = True
          Else
            Console.Write(", ")
          End If
          Console.Write(param) ' SimpleArgument+IndentifierName+IdentifierToken
        Next
        Console.Write(")") ' CloseParenToken
        Console.WriteLine()
        Console.WriteLine("*******")
      ElseIf method.Kind = SyntaxKind.ExpressionStatement Then
        ' PRINT "HELLO WORLD!"
        ' Seen as a ExpressionStatement/InvocationExpression
        ' NOTE: Does automatically add parens...
        Console.WriteLine("*******")
        Dim children = method.DescendantNodesAndTokensAndSelf(Function() True, True)
        For Each child In children
          Console.WriteLine($"{child.Kind}, {child}")
        Next
        Console.WriteLine("*******")
      ElseIf method.Kind = SyntaxKind.FieldDeclaration Then
        ' PRINT Hello$
        ' Seen as a failed FieldDeclaration
        ' Look for a SkippedTokensTrivia
        Console.WriteLine("=======")
        Dim children = method.DescendantNodesAndTokensAndSelf(Function() True, True)
        For Each child In children
          Console.WriteLine($"{child.Kind}, {child}")
        Next
        Console.WriteLine("=======")
      Else
        Console.WriteLine($"{method.Kind}, {method}")
      End If
    Next

    'Dim c = nodes.Count
    'Console.WriteLine(c)

  End Sub

  Private Sub PrintTree(node As SyntaxNodeOrToken, level As Integer)
    For Each child In node.ChildNodesAndTokens()
      Console.WriteLine($"{If(level - 1 > 0, Space((level - 1) * 2), "")}└ {child} ' [{child.Kind}]")
      If child.ChildNodesAndTokens.Any Then PrintTree(child, level + 1)
    Next
  End Sub

End Module