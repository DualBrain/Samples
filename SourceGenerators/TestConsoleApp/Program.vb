Option Explicit On
Option Strict On
Option Infer On

Imports System.Collections.Immutable
Imports System.Diagnostics.CodeAnalysis
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports SourceGeneratorSamples

Module Program

  Public Sub Main() 'args As String())

    'Dim p As New Person With {.FirstName = "Cory", .LastName = "Smith"}
    'Console.WriteLine(p)
    'Console.ReadLine()
    'Return

    Dim source As String = "
Imports RecordGenerator

Namespace Foo

  <Record>
  Public Class Garble
    Public Property FirstName As String
    Public Property LastName As Integer
    Public ReadOnly Property Birth As Date
  End Class

End Namespace"

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

    Dim references As List(Of Microsoft.CodeAnalysis.MetadataReference) = New List(Of MetadataReference)
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

    Dim generator1 As ISourceGenerator = New SourceGeneratorSamples.RecordGenerator

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

Public Class PersonX

  Public Property FirstName As String
  Public Property LastName As String
  Public Property X1 As String
  Public Property X2 As Integer

End Class

Partial Public Class PersonX
  Implements IEquatable(Of PersonX)

  Public Overrides Function ToString() As String
    Dim sb As New System.Text.StringBuilder
    sb.Append("Person")
    sb.Append(" { ")
    If PrintMembers(sb) Then
      sb.Append(" ")
    End If
    sb.Append("}")
    Return sb.ToString()
  End Function

  Protected Overridable Function PrintMembers(builder As System.Text.StringBuilder) As Boolean
    builder.Append("FirstName")
    builder.Append(" = ")
    builder.Append(FirstName)
    builder.Append(", ")
    builder.Append("LastName")
    builder.Append(" = ")
    builder.Append(LastName)
    Return True
  End Function

  Public Shared Operator <>(r1 As PersonX, r2 As PersonX) As Boolean
    Return Not (r1 Is r2)
  End Operator

  Public Shared Operator =(r1 As PersonX, r2 As PersonX) As Boolean
    Return r1 Is r2 OrElse (r1 IsNot Nothing AndAlso r1.Equals(r2))
  End Operator

  Public Overrides Function GetHashCode() As Integer
    ' Otherwise...
    Dim result = HashCode.Combine(GetType(PersonX), FirstName)
    result = HashCode.Combine(result, LastName)
    result = HashCode.Combine(result, X1)
    result = HashCode.Combine(result, X2)
    ' Return the result...
    Return result
  End Function

  Public Shadows Function Equals(obj As Object) As Boolean
    Return Equals(TryCast(obj, PersonX))
  End Function

  Public Shadows Function Equals(<AllowNull> other As PersonX) As Boolean Implements IEquatable(Of PersonX).Equals
    'TODO: What if the object being tested against is inherited/extended beyond Person?
    Return other IsNot Nothing AndAlso
           EqualityComparer(Of String).[Default].Equals(FirstName, other.FirstName) AndAlso
           EqualityComparer(Of String).[Default].Equals(LastName, other.LastName)
  End Function

  Public Overridable Function Clone() As PersonX
    Return New PersonX(Me)
  End Function

  Protected Sub New(original As PersonX)
    FirstName = original.FirstName
    LastName = original.LastName
  End Sub

  Public Sub Deconstruct(<Out> ByRef firstName As String, <Out> ByRef lastName As String)
    firstName = Me.FirstName
    lastName = Me.LastName
  End Sub

End Class