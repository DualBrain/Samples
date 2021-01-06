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

Imports TestConsoleApp.PersonX.WithPositions

Module Program

  Public Sub Main() 'args As String())

    'Dim p1 As New PersonX("Cory", "Smith", "Hazel", 50)
    'Dim p2 = p1.With(FirstName Or Age, firstName:="Bill", lastName:="Gates", eyeColor:="Brown", 65)
    'Console.WriteLine(p1)
    'Console.WriteLine(p2)
    'Console.ReadLine()
    'Return

    Dim source As String = "
Imports RecordGenerator

Namespace Foo

  <Record>
  Public Class Person
    Public ReadOnly Property FirstName As String
    Public ReadOnly Property LastName As Integer
    Public Overrides Function ToString() As String
    End Function
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
  Public ReadOnly Property FirstName As String
  Public ReadOnly Property LastName As String
  Public ReadOnly Property EyeColor As String
  Public ReadOnly Property Age As Integer
End Class

Partial Public Class PersonX
  Implements IEquatable(Of PersonX)

  Public Sub New(firstName As String, lastName As String, eyeColor As String, age As Integer)
    Me.FirstName = firstName
    Me.LastName = lastName
    Me.EyeColor = eyeColor
    Me.Age = age
  End Sub

  Protected Sub New(original As PersonX)
    FirstName = original.FirstName
    LastName = original.LastName
    EyeColor = original.EyeColor
    Age = original.Age
  End Sub

  <Flags>
  Public Enum WithPositions
    None = 0
    FirstName = 1
    LastName = 2
    EyeColor = 4
    Age = 8
  End Enum

  Public Function [With](flags As WithPositions,
                         Optional firstName As String = Nothing,
                         Optional lastName As String = Nothing,
                         Optional eyeColor As String = Nothing,
                         Optional age As Integer = 0) As PersonX
    Return New PersonX(If((flags And WithPositions.FirstName) = WithPositions.FirstName, firstName, Me.FirstName),
                       If((flags And WithPositions.LastName) = WithPositions.LastName, lastName, Me.LastName),
                       If((flags And WithPositions.EyeColor) = WithPositions.EyeColor, eyeColor, Me.EyeColor),
                       If((flags And WithPositions.Age) = WithPositions.Age, age, Me.Age))
  End Function

  Public Overridable Shadows Function ToString() As String
    Dim sb As New System.Text.StringBuilder
    sb.Append("PersonX")
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
    builder.Append(", ")
    builder.Append("EyeColor")
    builder.Append(" = ")
    builder.Append(EyeColor)
    builder.Append(", ")
    builder.Append("Age")
    builder.Append(" = ")
    builder.Append(Age)
    Return True
  End Function

  Public Shared Operator <>(r1 As PersonX, r2 As PersonX) As Boolean
    Return Not (r1 Is r2)
  End Operator

  Public Shared Operator =(r1 As PersonX, r2 As PersonX) As Boolean
    Return r1 Is r2 OrElse (r1 IsNot Nothing AndAlso r1.Equals(r2))
  End Operator

  Public Overrides Function GetHashCode() As Integer
    Return HashCode.Combine(GetType(PersonX), FirstName, LastName, EyeColor)
  End Function

  Public Shadows Function Equals(obj As Object) As Boolean
    Return Equals(TryCast(obj, PersonX))
  End Function

  Public Shadows Function Equals(<AllowNull> other As PersonX) As Boolean Implements IEquatable(Of PersonX).Equals
    'TODO: What if the object being tested against is inherited/extended beyond Person?
    Return other IsNot Nothing AndAlso
           EqualityComparer(Of String).[Default].Equals(FirstName, other.FirstName) AndAlso
           EqualityComparer(Of String).[Default].Equals(LastName, other.LastName) AndAlso
           EqualityComparer(Of String).[Default].Equals(EyeColor, other.EyeColor) AndAlso
           Age = other.Age
  End Function

  Public Overridable Function Clone() As PersonX
    Return New PersonX(Me)
  End Function

  'Public Sub Deconstruct(<Out> ByRef firstName As String, <Out> ByRef lastName As String, <Out> Byref eyeColor As String)
  '  firstName = Me.FirstName
  '  lastName = Me.LastName
  '  eyeColor = Me.EyeColor
  'End Sub

End Class