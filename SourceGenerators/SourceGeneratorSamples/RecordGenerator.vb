Option Explicit On
Option Infer On
Option Strict On

Imports System.Text

Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Global.SourceGeneratorSamples

  <Generator(LanguageNames.VisualBasic)>
  Public Class RecordGenerator
    Implements ISourceGenerator

    Private Const ATTRIBUTE_TEXT As String = "Option Explicit On
Option Strict On
Option Infer On

Namespace Global.RecordGenerator

  <AttributeUsage(AttributeTargets.Class, Inherited:=False, AllowMultiple:=False)>
  Friend NotInheritable Class RecordAttribute
    Inherits Attribute

  End Class

End Namespace"

    Public Sub Initialize(context As GeneratorInitializationContext) Implements ISourceGenerator.Initialize
      ' Register a syntax receiver that will be created for each generation pass
      context.RegisterForSyntaxNotifications(Function() As ISyntaxReceiver
                                               Return New SyntaxReceiver
                                             End Function)
    End Sub

    Public Sub Execute(context As GeneratorExecutionContext) Implements ISourceGenerator.Execute

      ' add the attribute text
      context.AddSource("RecordAttribute", SourceText.From(ATTRIBUTE_TEXT, Encoding.UTF8))

      ' retrieve the populated receiver 
      If Not (TypeOf context.SyntaxReceiver Is SyntaxReceiver) Then Return
      Dim receiver = TryCast(context.SyntaxReceiver, SyntaxReceiver)

      ' we're going to create a new compilation that contains the attribute.
      ' TODO: we should allow source generators to provide source during initialize, so that this step isn't required.
      Dim options = context.Compilation.SyntaxTrees.First().Options
      Dim compilation = context.Compilation.AddSyntaxTrees(VisualBasicSyntaxTree.ParseText(SourceText.From(ATTRIBUTE_TEXT, Encoding.UTF8), CType(options, VisualBasicParseOptions)))

      ' get the newly bound attribute, and INotifyPropertyChanged
      Dim attributeSymbol = compilation.GetTypeByMetadataName("RecordGenerator.RecordAttribute")

      ' loop over the candidate fields, and keep the ones that are actually annotated
      Dim propertySymbols As New List(Of IPropertySymbol)

      For Each prop In receiver.CandidateProperties
        Dim model = compilation.GetSemanticModel(prop.SyntaxTree)
        Dim name = prop.TryGetInferredMemberName
        Dim propertySymbol = TryCast(model.GetDeclaredSymbol(prop), IPropertySymbol)
        If propertySymbol IsNot Nothing Then
          For Each c In receiver.CandidateClasses
            Dim className = c.Identifier.Text
            If className = propertySymbol.ContainingType.Name Then
              propertySymbols.Add(propertySymbol)
              Exit For
            End If
          Next
        End If
      Next

      'For Each prop In receiver.CandidateProperties
      '  Dim model = compilation.GetSemanticModel(prop.SyntaxTree)
      '  For Each variable In prop.Declarators
      '    For Each name In variable.Names
      '      ' Get the symbol being decleared by the field, and keep it if its annotated
      '      Dim propertySymbol = TryCast(model.GetDeclaredSymbol(name), IPropertySymbol)
      '      If propertySymbol IsNot Nothing AndAlso
      '         propertySymbol.GetAttributes().Any(Function(ad) ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.[Default])) Then
      '        propertySymbols.Add(propertySymbol)
      '      End If
      '    Next
      '  Next
      'Next

      ' group the fields by class, and generate the source
      For Each group In propertySymbols.GroupBy(Function(f) f.ContainingType)
        Dim classSource = ProcessClass(group.Key, group.ToList(), attributeSymbol)
        context.AddSource($"{group.Key.Name}_Record.vb", SourceText.From(classSource, Encoding.UTF8))
      Next

    End Sub

    Private Function ProcessClass(classSymbol As INamedTypeSymbol, properties As List(Of IPropertySymbol), attributeSymbol As ISymbol) As String

      If Not classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.[Default]) Then
        Return Nothing 'TODO: issue a diagnostic that it must be top level
      End If

      Dim namespaceName = classSymbol.ContainingNamespace.ToDisplayString()

      ' begin building the generated source
      Dim source = New StringBuilder($"Option Explicit On
Option Strict On
Option Infer On

Namespace Global.{namespaceName}

  Partial Public Class {classSymbol.Name}
  
")

      If False Then
        'TODO: Need to create a New to initialize all params (maybe).
        source.Append($"
    'Sub New(A1 As Integer, B2 As Integer)
    '    Me.A1 = A1
    '    Me.B2 = B2
    'End Sub

")
      End If

      source.Append($"    Protected Overrides Sub Finalize()
      MyBase.Finalize()
    End Sub

    Friend Function Clone() As {classSymbol.Name}
      Return Me.Clone
    End Function

    Public Overrides Function Equals(target As Object) As Boolean
      Dim target As Object = TryCast(target, {classSymbol.Name})
      If target Is Nothing Then Return False
      Return Me.Equals(target)
    End Function

    Public Overloads Function Equals(target As {classSymbol.Name}) As Boolean
")

      ' create properties for each field 
      If properties?.Count > 0 Then
        source.Append("
")
      End If
      For Each propertySymbol In properties
        ProcessProperty(source, propertySymbol, attributeSymbol)
      Next

      source.Append($"
      Return True

    End Function

    Public Overrides Function GetHashCode() As Integer
      Return MyBase.GetHashCode()
    End Function

    Public Overrides Function ToString() As String
      Return MyBase.ToString()
    End Function
")

      source.Append($"
  End Class

End Namespace")

      Return source.ToString()

    End Function

    Private Sub ProcessProperty(source As StringBuilder, propertySymbol As IPropertySymbol, attributeSymbol As ISymbol)

      Dim chooseName As Func(Of String, TypedConstant, String) =
        Function(fieldName1 As String, overridenNameOpt1 As TypedConstant) As String

          If Not overridenNameOpt1.IsNull Then
            Return overridenNameOpt1.Value.ToString()
          End If

          fieldName1 = fieldName1.TrimStart("_"c)
          If fieldName1.Length = 0 Then
            Return String.Empty
          End If

          If fieldName1.Length = 1 Then
            Return fieldName1.ToUpper()
          End If

          Return fieldName1.Substring(0, 1).ToUpper() & fieldName1.Substring(1)

        End Function

      ' get the name and type of the field
      Dim propertyName = propertySymbol.Name
      'Dim propertyType = propertySymbol.Type

      ' get the AutoNotify attribute from the field, and any associated data
      'Dim attributeData = propertySymbol.GetAttributes().[Single](Function(ad) ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.[Default]))
      'Dim overridenNameOpt = attributeData.NamedArguments.SingleOrDefault(Function(kvp) kvp.Key = "PropertyName").Value

      source.Append($"      If Not Me.{propertyName}.Equals(target.{propertyName}) Then Return False
")

    End Sub

    ''' <summary>
    ''' Created on demand before each generation pass
    ''' </summary>
    Class SyntaxReceiver
      Implements ISyntaxReceiver

      'Public ReadOnly Property CandidateFields As List(Of FieldDeclarationSyntax) = New List(Of FieldDeclarationSyntax)
      Public ReadOnly Property CandidateClasses As List(Of ClassStatementSyntax) = New List(Of ClassStatementSyntax)
      Public ReadOnly Property CandidateProperties As List(Of PropertyStatementSyntax) = New List(Of PropertyStatementSyntax)

      ''' <summary>
      ''' Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
      ''' </summary>
      Public Sub OnVisitSyntaxNode(syntaxNode As SyntaxNode) Implements ISyntaxReceiver.OnVisitSyntaxNode
        ' any field with at least one attribute is a candidate for property generation
        If TypeOf syntaxNode Is ClassStatementSyntax Then
          Dim node = TryCast(syntaxNode, ClassStatementSyntax)
          If node.AttributeLists.Count > 0 Then
            CandidateClasses.Add(node)
          End If
        ElseIf TypeOf syntaxNode Is PropertyStatementSyntax Then
          Dim node = TryCast(syntaxNode, PropertyStatementSyntax)
          CandidateProperties.Add(node)
        End If
        'If TypeOf syntaxNode Is FieldDeclarationSyntax Then
        '  Dim fieldDeclarationSyntax = TryCast(syntaxNode, FieldDeclarationSyntax)
        '  If fieldDeclarationSyntax.AttributeLists.Count > 0 Then
        '    CandidateFields.Add(fieldDeclarationSyntax)
        '  End If
        'End If
      End Sub

    End Class

  End Class

End Namespace