Option Explicit On
Option Strict On
Option Infer On

Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
'Imports Microsoft.CodeAnalysis.Emit
'Imports Microsoft.CodeAnalysis.MSBuild

Imports System.IO
Imports System.Threading.Tasks
Imports System.Diagnostics
Imports Microsoft.CodeAnalysis.Formatting
'Imports System.Reflection

Module Program

  Private m_sourceCode As New List(Of (File As String, Text As String))

  Sub Main(args As String())
    'Console.WriteLine("Hello World!")

    'Dim sourceCode = New List(Of Source)

    Dim basePath = "d:\test"

    Dim code = <code><![CDATA[
10 FirstName$ = "Cory"
15 LastName$ = "Smth"
16 GOTO 30
20 PRINT FirstName$ + LastName$ ' This is a test
'21 PRINT FirstName$ LastName$
'22 PRINT 3+3
25 END
30 LastName$ = "Smith"
35 GOTO 20
]]></code>.Value.Trim

    ' "Detect" whether the source contains line numbers.
    If "01234567890".Contains(code.Substring(0, 1)) Then
      ' If so, convert to "structured" (aka QB).
      code = RemLine.RemLine(code)
    End If

    code = $"Option Explicit Off
Option Strict Off
Option Infer Off

Public Module Program
 
  Public Sub Main()

{code}
  End Sub

End Module
"

    'm_sourceCode.Add((Nothing, code.Trim))

    Dim vbTrees = New List(Of SyntaxTree)
    Dim rewriter = New VbRewriter()

    'Parallel.ForEach(m_sourceCode, Sub(sc)
    '                                 Console.WriteLine($"Completing transpilation of {Path.GetFileName(sc.file)}")
    '                                 vbTrees.Add(rewriter.Visit(VisualBasicSyntaxTree.ParseText(sc.text).GetCompilationUnitRoot()).SyntaxTree.WithFilePath(sc.file))
    '                               End Sub)

    Dim parsed = VisualBasicSyntaxTree.ParseText(code.Trim)
    Dim unit = parsed.GetCompilationUnitRoot()
    'Dim formatted = Formatter.Format(unit)

    For Each t In unit.SyntaxTree.GetRoot.DescendantTokens
      Console.Write($"[{t}] ")
    Next
    Console.WriteLine("---")

    Dim tree = rewriter.Visit(unit).SyntaxTree

    'Walk(tree.GetRoot)

    vbTrees.Add(tree.WithFilePath("Program"))

    'vbTrees.Add(VisualBasicSyntaxTree.ParseText(File.ReadAllText($"{basePath}/Libs/Runtime.vb")).WithFilePath($"{basePath}/Libs/Runtime.vb"))

    For Each vt In vbTrees

      Dim fileName = Path.GetFileName(vt.FilePath)
      If fileName.LastIndexOf(".") <> -1 Then
        fileName = fileName.Substring(0, fileName.LastIndexOf("."))
      End If
      fileName &= ".vb"

      File.WriteAllText(IO.Path.Combine(basePath, fileName), vt.ToString())

    Next


  End Sub

  Private Sub Walk(parentNode As SyntaxNode)

    Return

    For Each node In parentNode.ChildNodes
      Console.WriteLine("---")
      Console.WriteLine($"{node.Kind} {node}")
      If node.ContainsDiagnostics Then
        For Each diag In node.GetDiagnostics
          Console.WriteLine($"DIAG: {diag}")
        Next
      End If
      Walk(node)
    Next

  End Sub

End Module
