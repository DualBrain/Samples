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
  Public Class ImplicitInterfaceGenerator
    Implements ISourceGenerator

    '    Private Const ATTRIBUTE_TEXT As String = "
    'Namespace Global.ImplicitInterfaceGenerator

    '  <AttributeUsage(AttributeTargets.Class, Inherited:=False, AllowMultiple:=False)>
    '  Friend NotInheritable Class ImplicitAttribute
    '    Inherits Attribute

    '  End Class

    'End Namespace"

    Public Sub Initialize(context As GeneratorInitializationContext) Implements ISourceGenerator.Initialize
      ' Register a syntax receiver that will be created for each generation pass
      context.RegisterForSyntaxNotifications(Function() As ISyntaxReceiver
                                               Return New SyntaxReceiver
                                             End Function)
    End Sub

    Private Class Prop
      Public Property InterfaceName As String
      Public Property Name As String
      Public Property ReturnType As ITypeSymbol
      Public Property Getter As Boolean
      Public Property Setter As Boolean
    End Class

    Public Sub Execute(context As GeneratorExecutionContext) Implements ISourceGenerator.Execute

      ' add the attribute text
      'context.AddSource("ImplicitAttribute", SourceText.From(ATTRIBUTE_TEXT, Encoding.UTF8))

      ' retrieve the populated receiver 
      If Not (TypeOf context.SyntaxReceiver Is SyntaxReceiver) Then Return
      Dim receiver = TryCast(context.SyntaxReceiver, SyntaxReceiver)

      ' we're going to create a new compilation that contains the attribute.
      ' TODO: we should allow source generators to provide source during initialize, so that this step isn't required.
      Dim options = context.Compilation.SyntaxTrees.First().Options
      Dim compilation = context.Compilation '.AddSyntaxTrees(VisualBasicSyntaxTree.ParseText(SourceText.From(ATTRIBUTE_TEXT, Encoding.UTF8), CType(options, VisualBasicParseOptions)))

      ' For each Class...
      '   If the Class has an Implements XXX
      '     Find the Interface????
      '     Determine Properties, Subs, Functions for the Interface.
      '     Determine existing Private Member Definitions and Private Subs/Functions for Class
      '     If Interface Property, Sub or Function not "manually" defined...
      '       Create IXXX_Property, Sub or Function for Interface...
      '       If similar member definition or private sub/function exists, add call in created property, sub or function.

      ' For now, attempting to "just create" the interface implementation
      ' assuming everything exists, named properly and nothing is wrong
      ' or duplicated.  We can then circle back to start validating one
      ' way or the other.

      Dim sourceCode = ""

      ' For each and every Implements that we find....
      For Each i In receiver.CandidateImplements
        ' Determine which class...
        Dim interfaceName = DirectCast(i.Types(0), IdentifierNameSyntax).Identifier.Text
        Dim className = CType(i.Parent, ClassBlockSyntax).ClassStatement.Identifier.Text
        If receiver.CandidateClasses.Contains(CType(i.Parent, ClassBlockSyntax).ClassStatement) Then

          Dim need = False

          sourceCode &= $"Partial Class {className}{vbCrLf}"
          sourceCode &= $"{vbCrLf}"

          Dim t = compilation.GetTypeByMetadataName(interfaceName)

          ' First pass, farm all property getter and setters:

          Dim props As New List(Of Prop)

          For Each interfaceMember In t.GetMembers().OfType(Of IMethodSymbol)
            Dim name = interfaceMember.Name
            Select Case interfaceMember.MethodKind
              Case MethodKind.PropertyGet
                Dim existing = (From p In props Where p.InterfaceName = interfaceName AndAlso p.Name = name.Substring(4)).FirstOrDefault
                If existing IsNot Nothing Then
                  existing.Getter = True
                  existing.ReturnType = interfaceMember.ReturnType
                Else
                  props.Add(New Prop With {.InterfaceName = interfaceName,
                                                .Name = name.Substring(4),
                                                .Getter = True,
                                                .ReturnType = interfaceMember.ReturnType})
                End If
              Case MethodKind.PropertySet
                Dim existing = (From p In props Where p.InterfaceName = interfaceName AndAlso p.Name = name.Substring(4)).FirstOrDefault
                If existing IsNot Nothing Then
                  existing.Setter = True
                Else
                  props.Add(New Prop With {.InterfaceName = interfaceName,
                                                .Name = name.Substring(4),
                                                .Setter = True,
                                                .ReturnType = interfaceMember.ReturnType})
                End If
              Case Else
            End Select
          Next

          For Each entry In props

            'TODO: Need to handle nullable types.

            ' Do we already have an implemented property for this interface?
            ' If not, does the class have a property with the same name and return type that we can infer?
            ' If not, does the class have a field with a "similar" name and type?

            Dim found = False
            Dim skip = False

            Dim what As String = Nothing '= $"m_{entry.Name(0).ToString().ToLower()}{entry.Name.Substring(1)}"

            'TODO: Need to handle...
            ' Public Property IInterface_Name() As String Implements IInterface.Name
            ' Need to review all of the properties for any matching interface match regardless
            ' of method name to handle collision scenarios in implementing interfaces.
            ' If found, skip.
            For Each prop In receiver.CandidateProperties
              ' Determine which class...
              Dim findClassName = CType(prop.Parent, ClassBlockSyntax).ClassStatement.Identifier.Text
              If className = findClassName Then
                Dim propertyName = prop.Identifier.Text
                If propertyName = entry.Name Then
                  ' Just a "regular property" or "property that implements target interface"
                  found = True
                  what = $"{propertyName}"
                  Dim imp = prop.ImplementsClause
                  If imp IsNot Nothing Then
                    For Each impMember In imp.InterfaceMembers
                      Dim nm = impMember.ToString
                      If nm = $"{entry.InterfaceName}.{entry.Name}" Then
                        skip = True
                        Exit For
                      End If
                    Next
                  End If
                  If found Then Exit For
                End If
              End If
            Next

            If Not found Then
              For Each field In receiver.CandidateFields
                ' Determine which class...
                Dim findClassName = CType(field.Parent, ClassBlockSyntax).ClassStatement.Identifier.Text
                If className = findClassName Then
                  'TODO: Need to determine variable type matches target...
                  For Each decl In field.Declarators
                    For Each identifier In decl.Names
                      Dim fieldName = identifier.Identifier.Text
                      If fieldName = $"m_{ToCamelCase(entry.Name)}" OrElse
                         fieldName = $"_{ToCamelCase(entry.Name)}" Then
                        found = True
                        what = $"{fieldName}"
                        Exit For
                      End If
                    Next
                    If found Then Exit For
                  Next
                  If found Then Exit For
                End If
              Next
            End If

            If found AndAlso Not skip AndAlso Not String.IsNullOrWhiteSpace(what) Then
              sourceCode &= $"  Private{If(entry.Setter AndAlso entry.Getter, " ", If(entry.Getter, " ReadOnly ", " WriteOnly "))}Property {entry.InterfaceName}_{entry.Name} As {entry.ReturnType} Implements {entry.InterfaceName}.{entry.Name}{vbCrLf}"
              If entry.Getter Then
                sourceCode &= $"    Get{vbCrLf}"
                sourceCode &= $"      Return {what}{vbCrLf}"
                sourceCode &= $"    End Get{vbCrLf}"
              End If
              If entry.Setter Then
                sourceCode &= $"    Set(value As {entry.ReturnType}){vbCrLf}"
                sourceCode &= $"      {what} = value{vbCrLf}"
                sourceCode &= $"    End Set{vbCrLf}"
              End If
              sourceCode &= $"  End Property{vbCrLf}"
              sourceCode &= $"{vbCrLf}"
              need = True
            End If

          Next

          ' Second pass, look for everything but properties...

          For Each interfaceMember In t.GetMembers().OfType(Of IMethodSymbol)
            Dim name = interfaceMember.Name
            Select Case interfaceMember.MethodKind
              Case MethodKind.PropertyGet
              Case MethodKind.PropertySet
              Case MethodKind.DeclareMethod, MethodKind.Ordinary

                ' Do we already have an implemented subroutine for this interface?
                ' If not, does the class have a subroutine that we can infer?

                'TODO: Are there any parameters, if so... do they match?
                'TODO: If a function, does the return type match?

                Dim found = False
                Dim skip = False
                For Each method In receiver.CandidateMethods
                  ' Determine which class...
                  Dim findClassName = CType(method.Parent.Parent, ClassBlockSyntax).ClassStatement.Identifier.Text
                  If className = findClassName Then
                    Dim methodName = method.Identifier.Text
                    If methodName = interfaceMember.Name Then
                      ' Just a "regular method" or "method that implements a interface"
                      found = True
                      Dim imp = method.ImplementsClause
                      If imp IsNot Nothing Then
                        For Each impMember In imp.InterfaceMembers
                          Dim nm = impMember.ToString
                          If nm = $"{interfaceName}.{interfaceMember.Name}" Then
                            skip = True
                            Exit For
                          End If
                        Next
                      End If
                      If found Then Exit For
                    End If
                  End If
                Next

                If found AndAlso Not skip Then
                  If interfaceMember.ReturnsVoid Then ' Sub
                    sourceCode &= $"  Private Sub {interfaceName}_{interfaceMember.Name}() Implements {interfaceName}.{interfaceMember.Name}{vbCrLf}"
                    sourceCode &= $"    Call {interfaceMember.Name}(){vbCrLf}"
                    sourceCode &= $"  End Sub{vbCrLf}"
                  Else ' Function
                    sourceCode &= $"  Private Function {interfaceName}_{interfaceMember.Name}() As {interfaceMember.ReturnType} Implements {interfaceName}.{interfaceMember.Name}{vbCrLf}"
                    sourceCode &= $"    Return {interfaceMember.Name}(){vbCrLf}"
                    sourceCode &= $"  End Function{vbCrLf}"
                  End If
                  sourceCode &= $"{vbCrLf}"
                  need = True
                End If

              Case Else
                ' Not sure what will hit here...
                'TODO: Events?
            End Select
          Next

          sourceCode &= $"End Class"

          If need Then
            context.AddSource($"{className}_ImplicitInterface.vb", SourceText.From(sourceCode, Encoding.UTF8))
          End If

        End If
      Next

      '' get the newly bound attribute, and INotifyPropertyChanged
      'Dim attributeSymbol = compilation.GetTypeByMetadataName("ImplicitInterfaceGenerator.ImplicitAttribute")

      '' loop over the candidate fields, and keep the ones that are actually annotated
      'Dim propertySymbols As New List(Of IPropertySymbol)

      'For Each prop In receiver.CandidateProperties
      '  Dim model = compilation.GetSemanticModel(prop.SyntaxTree)
      '  Dim name = prop.Identifier.Text
      '  Dim propertySymbol = TryCast(model.GetDeclaredSymbol(prop), IPropertySymbol)
      '  If propertySymbol IsNot Nothing Then
      '    For Each c In receiver.CandidateClasses
      '      Dim className = c.Identifier.Text
      '      If className = propertySymbol.ContainingType.Name Then
      '        propertySymbols.Add(propertySymbol)
      '        Exit For
      '      End If
      '    Next
      '  End If
      'Next

      'Dim methodSymbols As New List(Of IMethodSymbol)

      'For Each method In receiver.CandidateMethods
      '  Dim model = compilation.GetSemanticModel(method.SyntaxTree)
      '  Dim name = method.Identifier.Text
      '  Dim methodSymbol = TryCast(model.GetDeclaredSymbol(method), IMethodSymbol)
      '  If methodSymbol IsNot Nothing Then
      '    For Each c In receiver.CandidateClasses
      '      Dim className = c.Identifier.Text
      '      If className = methodSymbol.ContainingType.Name Then
      '        methodSymbols.Add(methodSymbol)
      '        Exit For
      '      End If
      '    Next
      '  End If
      'Next

      '' group the properties by class and generate the source
      'For Each entry In From p In propertySymbols Group p By Key = p.ContainingType.Name, Type = p.ContainingType Into Group ' propertySymbols.GroupBy(Function(f) f.ContainingType)
      '  Dim methods = From p In methodSymbols Where p.ContainingType.Name = entry.Group(0).ContainingType.Name
      '  Dim classSource = ProcessClass(entry.Type, entry.Group.ToList(), methods.ToList())
      '  context.AddSource($"{entry.Key}_ImplicitInterface.vb", SourceText.From(classSource, Encoding.UTF8))
      'Next

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

'Test

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

      Public ReadOnly Property CandidateClasses As List(Of ClassStatementSyntax) = New List(Of ClassStatementSyntax)
      Public ReadOnly Property CandidateImplements As List(Of ImplementsStatementSyntax) = New List(Of ImplementsStatementSyntax)

      Public ReadOnly Property CandidateProperties As List(Of PropertyStatementSyntax) = New List(Of PropertyStatementSyntax)
      Public ReadOnly Property CandidateFields As List(Of FieldDeclarationSyntax) = New List(Of FieldDeclarationSyntax)
      Public ReadOnly Property CandidateMethods As List(Of MethodStatementSyntax) = New List(Of MethodStatementSyntax)
      ''' <summary>
      ''' Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
      ''' </summary>
      Public Sub OnVisitSyntaxNode(syntaxNode As SyntaxNode) Implements ISyntaxReceiver.OnVisitSyntaxNode
        If TypeOf syntaxNode Is ImplementsStatementSyntax Then
          Dim node = TryCast(syntaxNode, ImplementsStatementSyntax)
          Dim parent = TryCast(node.Parent.ChildNodes(0), ClassStatementSyntax)
          If parent IsNot Nothing Then
            If Not CandidateClasses.Contains(parent) Then
              CandidateClasses.Add(parent)
            End If
            CandidateImplements.Add(node)
          End If
        ElseIf TypeOf syntaxNode Is PropertyStatementSyntax Then
          Dim node = TryCast(syntaxNode, PropertyStatementSyntax)
          Dim parent = TryCast(node.Parent.ChildNodes(0), ClassStatementSyntax)
          If parent IsNot Nothing AndAlso CandidateClasses.Contains(parent) Then
            CandidateProperties.Add(node)
          End If
        ElseIf TypeOf syntaxNode Is FieldDeclarationSyntax Then
          Dim node = TryCast(syntaxNode, FieldDeclarationSyntax)
          Dim parent = TryCast(node.Parent.ChildNodes(0), ClassStatementSyntax)
          If parent IsNot Nothing AndAlso CandidateClasses.Contains(parent) Then
            CandidateFields.Add(node)
          End If
        ElseIf TypeOf syntaxNode Is MethodStatementSyntax Then
          Dim node = TryCast(syntaxNode, MethodStatementSyntax)
          Dim parent = TryCast(node.Parent.Parent.ChildNodes(0), ClassStatementSyntax)
          If parent IsNot Nothing AndAlso CandidateClasses.Contains(parent) Then
            Dim name = node.Identifier.Text
            CandidateMethods.Add(node)
          End If
        End If
      End Sub

    End Class

  End Class

End Namespace