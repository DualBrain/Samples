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

  'Private m_sourceCode As New List(Of (File As String, Text As String))

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

  'Private Sub Walk(parentNode As SyntaxNode)

  '  For Each node In parentNode.ChildNodes
  '    Console.WriteLine("---")
  '    Console.WriteLine($"{node.Kind} {node}")
  '    If node.ContainsDiagnostics Then
  '      For Each diag In node.GetDiagnostics
  '        Console.WriteLine($"DIAG: {diag}")
  '      Next
  '    End If
  '    Walk(node)
  '  Next

  'End Sub

End Module

#Disable Warning IDE1006 ' Naming Styles
Namespace QB64

  Public Module Functions

    Public Function _PI#(Optional multiplier# = 1.0)
      Return Math.PI * multiplier
    End Function

    Public Function _R2D#(radian#)
      Return radian# / (_PI() / 180)
    End Function

    Public Function _R2G#(radian#)
      Return radian# * 0.01571
    End Function

    Public Function _RED&(rgbaColorIndex&, Optional imageHandle& = -1)
      If imageHandle& <> -1 Then
        Throw New NotImplementedException
      Else
        Return System.Drawing.Color.FromArgb(CInt(rgbaColorIndex)).R
      End If
    End Function

    Public Function _RED32&(rgbaColor&)
      Return System.Drawing.Color.FromArgb(CInt(rgbaColor&)).R
    End Function

    Public Function _READBIT(numericalVariable As Byte, numericalvalue%) As Boolean
      Return (numericalVariable And (1 << numericalvalue%)) <> 0
    End Function

    Public Function _READBIT(numericalVariable As Short, numericalvalue%) As Boolean
      Return (numericalVariable And (1 << numericalvalue%)) <> 0
    End Function

    Public Function _READBIT(numericalVariable As Integer, numericalvalue%) As Boolean
      Return (numericalVariable And (1 << numericalvalue%)) <> 0
    End Function

    Public Function _READBIT(numericalVariable As Long, numericalvalue%) As Boolean
      Return (numericalVariable And (1 << numericalvalue%)) <> 0
    End Function

    Public Function _RESETBIT(numericalVariable As Byte, numericalvalue%) As Byte
      Return CByte(numericalVariable Xor (1 << numericalvalue%))
    End Function

    Public Function _RESETBIT(numericalVariable As Short, numericalvalue%) As Short
      Return CShort(numericalVariable Xor (1 << numericalvalue%))
    End Function

    Public Function _RESETBIT(numericalVariable As Integer, numericalvalue%) As Integer
      Return CInt(numericalVariable Xor (1 << numericalvalue%))
    End Function

    Public Function _RESETBIT(numericalVariable As Long, numericalvalue%) As Long
      Return numericalVariable Xor (1 << numericalvalue%)
    End Function

    Public Enum OnOff
      [ON]
      OFF
    End Enum

    Public Enum StretchSmooth
      _STRETCH
      _SMOOTH
    End Enum

    Public Sub _RESIZE(Optional onOff As OnOff = OnOff.ON, Optional stretchSmooth As StretchSmooth = StretchSmooth._STRETCH)
      Throw New NotImplementedException
    End Sub

    Public Function _RESIZE() As Boolean
      Throw New NotImplementedException
    End Function

    Public Function _RESIZEHEIGHT%()
      Throw New NotImplementedException
    End Function

    Public Function _RESIZEWIDTH%()
      Throw New NotImplementedException
    End Function

    Public Function _RGB&(red&, green&, blue&, Optional imageHandle& = -1)
      Throw New NotImplementedException
    End Function

    Public Function _RGB32(red&, green&, blue&, Optional alpha& = 255) As ULong
      Return _RGBA32(red&, green&, blue&, alpha&)
    End Function

    Public Function _RGB32(intensity&, Optional alpha& = 255) As ULong
      Return _RGB32(intensity&, intensity&, intensity&, alpha&)
    End Function

    Public Function _RGBA&(red&, green&, blue&, alpha&, Optional imageHandle& = -1)
      Throw New NotImplementedException
    End Function

    Public Function _RGBA32(red&, green&, blue&, alpha&) As ULong
      red& = red& And &HFF
      green& = green& And &HFF
      blue& = blue& And &HFF
      alpha& = alpha& And &HFF
      Return CULng((red& << 24) + (green& << 16) + (blue& << 8) + alpha&)
    End Function

    Public Function _ROUND&(number#)
      Return CLng(Math.Round(number#))
    End Function

    Public Function _TRIM$(text$)
      Return text$.Trim
    End Function

  End Module

End Namespace
#Enable Warning IDE1006 ' Naming Styles
