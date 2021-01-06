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

    Private Const ATTRIBUTE_TEXT As String = "
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
        Dim name = prop.Identifier.Text
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

      Dim methodSymbols As New List(Of IMethodSymbol)

      For Each method In receiver.CandidateMethods
        Dim model = compilation.GetSemanticModel(method.SyntaxTree)
        Dim name = method.Identifier.Text
        Dim methodSymbol = TryCast(model.GetDeclaredSymbol(method), IMethodSymbol)
        If methodSymbol IsNot Nothing Then
          For Each c In receiver.CandidateClasses
            Dim className = c.Identifier.Text
            If className = methodSymbol.ContainingType.Name Then
              methodSymbols.Add(methodSymbol)
              Exit For
            End If
          Next
        End If
      Next

      ' group the properties by class and generate the source
      For Each entry In From p In propertySymbols Group p By Key = p.ContainingType.Name, Type = p.ContainingType Into Group ' propertySymbols.GroupBy(Function(f) f.ContainingType)
        Dim methods = From p In methodSymbols Where p.ContainingType.Name = entry.Group(0).ContainingType.Name
        Dim classSource = ProcessClass(entry.Type, entry.Group.ToList(), methods.ToList())
        context.AddSource($"{entry.Key}_Record.vb", SourceText.From(classSource, Encoding.UTF8))
      Next

    End Sub

    Private Function ProcessClass(classSymbol As INamedTypeSymbol, properties As List(Of IPropertySymbol), methods As List(Of IMethodSymbol)) As String

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
    Implements IEquatable(Of {classSymbol.Name})
  
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

      source.Append($"    'Protected Overrides Sub Finalize()
    '  MyBase.Finalize()
    'End Sub

    Public Overridable Function Clone() As {classSymbol.Name}
      Return New {classSymbol.Name}(Me)
    End Function
")

      Dim readonlyCount = Aggregate a In properties Where a.IsReadOnly Into Count()

      If readonlyCount = 0 Then
        source.Append($"
    Public Sub New()

    End Sub
")
      Else

        source.Append($"
    Public Sub New(")

        Dim insertComma = False
        For Each propertySymbol In From p In properties Where p.IsReadOnly
          If insertComma Then
            source.Append(", ")
          Else
            insertComma = True
          End If
          source.Append($"{ToCamelCase(propertySymbol.Name)} As {propertySymbol.Type}")
        Next

        source.Append($")
")

        For Each propertySymbol In From p In properties Where p.IsReadOnly
          source.Append($"      Me.{propertySymbol.Name} = {ToCamelCase(propertySymbol.Name)}
")
        Next

        source.Append($"    End Sub
")
      End If

      source.Append($"
    Protected Sub New(original As {classSymbol.Name})
")

      For Each propertySymbol In properties
        ProcessPropertyForClone(source, propertySymbol)
      Next

      source.Append($"    End Sub

    Public Shadows Function Equals(target As Object) As Boolean
      Return Equals(TryCast(target, {classSymbol.Name}))
    End Function

    Public Shadows Function Equals(target As {classSymbol.Name}) As Boolean Implements IEquatable(Of {classSymbol.Name}).Equals
      'TODO: What if the object being tested against is inherited/extended beyond {classSymbol.Name}?

      If target Is Nothing Then Return False
")

      ' create properties for each field 
      If properties?.Count > 0 Then
        source.Append("
")
      End If
      For Each propertySymbol In properties
        ProcessPropertyForEquals(source, propertySymbol)
      Next

      source.Append($"
      Return True

    End Function

    Public Overrides Function GetHashCode() As Integer
")

      If properties.Count < 8 Then
        source.Append($"      Return HashCode.Combine(GetType({classSymbol.Name})")
        For Each propertySymbol In properties
          source.Append($", {propertySymbol.Name}")
        Next
        source.Append($")")
      Else
        ' The "hard" way...
        Dim first = True
        For Each propertySymbol In properties
          If first Then
            source.Append($"      Dim result = HashCode.Combine(GetType({classSymbol.Name}), {propertySymbol.Name})
")
            first = False
          Else
            source.Append($"      result = HashCode.Combine(result, {propertySymbol.Name})
")
          End If
        Next
        source.Append($"      Return result
")
      End If

      source.Append($"
    End Function

    Public Shared Operator <>(r1 As {classSymbol.Name}, r2 As {classSymbol.Name}) As Boolean
      Return Not (r1 Is r2)
    End Operator

    Public Shared Operator =(r1 As {classSymbol.Name}, r2 As {classSymbol.Name}) As Boolean
      Return r1 Is r2 OrElse (r1 IsNot Nothing AndAlso r1.Equals(r2))
    End Operator
    ")

      Dim hasToString = (Aggregate a In methods Where a.Name = "ToString" Into Count()) > 0

      If Not hasToString Then

        source.Append($"
    Public Overrides Function ToString() As String
      Dim sb As New System.Text.StringBuilder
      sb.Append(""{classSymbol.Name}"")
      sb.Append("" {{ "")
      If PrintMembers(sb) Then
        sb.Append("" "")
      End If
      sb.Append(""}}"")
      Return sb.ToString()
    End Function

    Protected Overridable Function PrintMembers(builder As System.Text.StringBuilder) As Boolean

")

        Dim skip = True
        For Each propertySymbol In properties
          If Not skip Then
            source.Append($"      builder.Append("", "")
")
          Else
            skip = False
          End If
          ProcessPropertyForToString(source, propertySymbol)
        Next

        source.Append($"
      Return True

    End Function
")

      End If

      source.Append($"
  End Class

End Namespace")

      Return source.ToString()

    End Function

    Private Function ToCamelCase(value As String) As String
      Return value(0).ToString.ToLower & value.Substring(1)
    End Function

    Private Sub ProcessPropertyForToString(source As StringBuilder, propertySymbol As IPropertySymbol)

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

      source.Append($"      builder.Append($""{propertyName} = {{{propertyName}}}"")
")

    End Sub

    Private Sub ProcessPropertyForClone(source As StringBuilder, propertySymbol As IPropertySymbol)

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

      source.Append($"      {propertyName} = original.{propertyName}
")

    End Sub

    Private Sub ProcessPropertyForEquals(source As StringBuilder, propertySymbol As IPropertySymbol)

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

      source.Append($"      If Not EqualityComparer(Of String).[Default].Equals({propertyName}, target.{propertyName}) Then Return False
")

      '      source.Append($"      If Not Me.{propertyName}.Equals(target.{propertyName}) Then Return False
      '")

    End Sub

    ''' <summary>
    ''' Created on demand before each generation pass
    ''' </summary>
    Class SyntaxReceiver
      Implements ISyntaxReceiver

      'Public ReadOnly Property CandidateFields As List(Of FieldDeclarationSyntax) = New List(Of FieldDeclarationSyntax)
      Public ReadOnly Property CandidateClasses As List(Of ClassStatementSyntax) = New List(Of ClassStatementSyntax)
      Public ReadOnly Property CandidateMethods As List(Of MethodStatementSyntax) = New List(Of MethodStatementSyntax)
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
        ElseIf TypeOf syntaxNode Is MethodStatementSyntax Then
          Dim node = TryCast(syntaxNode, MethodStatementSyntax)
          Dim parent = TryCast(node.Parent.Parent.ChildNodes(0), ClassStatementSyntax)
          If parent IsNot Nothing AndAlso CandidateClasses.Contains(parent) Then
            Dim name = node.Identifier.Text
            If name = "ToString" Then
              CandidateMethods.Add(node)
            End If
          End If
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