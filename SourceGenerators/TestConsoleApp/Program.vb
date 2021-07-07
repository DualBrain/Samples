Option Explicit On
Option Strict On
Option Infer On

Imports System.Collections.Immutable
Imports System.Diagnostics.CodeAnalysis
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
'Imports SourceGeneratorSamples

'Imports TestConsoleApp.PersonX.WithPositions

'Interface Test
'  Property Whatever1(index As Integer) As Long
'  Property Whatever2(index As Integer, anotherThing As String) As Long

'End Interface

'Interface ITestClass
'  Event OurEvent As EventHandler(Of EventArgs)
'End Interface

'Class TestClass
'  Implements ITestClass
'  Public Event OurEvent As EventHandler(Of EventArgs) Implements ITestClass.OurEvent
'End Class

Module Program

  Public Sub Main() 'args As String())

    'Dim p1 As New PersonX("Cory", "Smith", "Hazel", 50)
    'Dim p2 = p1.With(FirstName Or Age, firstName:="Bill", lastName:="Gates", eyeColor:="Brown", 65)
    'Console.WriteLine(p1)
    'Console.WriteLine(p2)
    'Console.ReadLine()
    'Return

    '    Dim source = <![CDATA[
    'Imports RecordGenerator

    'Namespace Foo

    '  <Record>
    '  Public Class Person
    '    Public ReadOnly Property FirstName As String
    '    Public ReadOnly Property LastName As Integer
    '    Public Overrides Function ToString() As String
    '      Return "Yo!"
    '    End Function
    '  End Class

    'End Namespace]]>.Value

    Dim source = <![CDATA[

Namespace TopOne.LevelTwo

Public Interface IJob

  Event OurEvent As EventHandler(Of EventArgs)

  Property Description As String
  Property Pay As Currency
  Sub WishHappyBirthday()
  Function SampleFunction() As Integer

  Property SamplePropertyWithParam(param1 As Integer) As Integer
  Property SamplePropertyWithParams(param1 As Integer, param2 As Integer) As Integer

  Function SampleFunctionWithParam(param1 As String) As Integer
  Function SampleFunctionWithParam(Byref param1 As Integer) As Integer
  Function SampleFunctionWithParams(param1 As Integer, param2 As String) As Integer

  Sub Something(ByRef test1 As Integer)

End Interface

Public Interface IPerson
  Property FirstName As String
  Property LastName As String
  Property Birth As Date
  Sub WishHappyBirthday()
  Function Age() As Integer
End Interface

Public Class Boss
  Implements IPerson, IJob

  Public Event OurEvent As EventHandler(Of EventArgs)

  Private m_birth As Date

  Public Property SamplePropertyWithParam(param1 As Integer) As Integer
  Public Property SamplePropertyWithParams(param1 As Integer, param2 As Integer) As Integer

  Public Property FirstName As String Implements IPerson.FirstName
  Public Property LastName As String

  Public Property Description As String
  Public Property Pay As Currency

  Public Sub WishHappyBirthday() Implements IPerson.WishHappyBirthday
    ' Do something
  End Sub

  Public Function Age() As Integer
    Return m_birth.Year
  End Function

  Public Function SampleFunctionWithParam(param1 As String) As Integer
    Return CInt(param1?.Length)
  End Function

  Function SampleFunctionWithParam(param1 As Integer) As Integer
    Return param1
  End Function

  Function SampleFunctionWithParams(Optional Byref byrefParam1 as Long = 5) As Integer
    return 0
  End Function

  Function SampleFunctionWithParams(param1 As Integer, param2 As String) As Integer
    Return param1 + CInt(param2?.Length)
  End Function

  Sub Something(Optional ByRef something1 As Integer = 5)
  End Sub

End Class

End Namespace

]]>.Value

    Dim result = GetGeneratedOutput(source)

    If result.Diagnostics.Length > 0 Then
      Console.WriteLine("Diagnostics:")
      For Each diag In result.Diagnostics
        Console.WriteLine("   " & diag.ToString())
      Next
      Console.WriteLine()
      Console.WriteLine("Output:")
    End If

    Console.WriteLine(result.Output)

  End Sub

  Private Function GetGeneratedOutput(source As String) As (Diagnostics As ImmutableArray(Of Diagnostic), Output As String)

    Dim syntaxTree = VisualBasicSyntaxTree.ParseText(source)

    Dim references As New List(Of MetadataReference)
    Dim assemblies As Assembly() = AppDomain.CurrentDomain.GetAssemblies()
    For Each assembly As Assembly In assemblies
      If Not assembly.IsDynamic Then
        references.Add(MetadataReference.CreateFromFile(assembly.Location))
      End If
    Next

    Dim compilation = VisualBasicCompilation.Create("Foo", New SyntaxTree() {syntaxTree}, references, New VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary))

    ' TODO: Uncomment these lines if you want to return immediately if the injected program isn't valid _before_ running generators
    '
    'Dim compilationDiagnostics = compilation.GetDiagnostics()
    'If compilationDiagnostics.Any() Then
    '  Return (compilationDiagnostics, "")
    'End If

    'Dim generator1 As ISourceGenerator = New SourceGeneratorSamples.RecordGenerator
    Dim generator1 As ISourceGenerator = New DualBrain.ImplicitInterfaceGenerator

    Dim iaGenerator = {generator1}.ToImmutableArray
    'Dim iaGenerator = New ImmutableArray(Of ISourceGenerator) From {generator1}

    Dim driver = VisualBasicGeneratorDriver.Create(iaGenerator,
                                                   Nothing,
                                                   Nothing,
                                                   Nothing)

    Dim outputCompilation As Compilation = Nothing
    Dim generateDiagnostics As ImmutableArray(Of Diagnostic) = Nothing
    driver.RunGeneratorsAndUpdateCompilation(compilation, outputCompilation, generateDiagnostics)

    Return (generateDiagnostics, outputCompilation.SyntaxTrees.Last().ToString())

  End Function

End Module