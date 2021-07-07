Option Explicit On
Option Infer On
Option Strict On

Imports System.Text

Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Global.DualBrain

  <Generator(LanguageNames.VisualBasic)>
  Public Class ImplicitInterfaceGenerator
    Implements ISourceGenerator

    Public Sub Initialize(context As GeneratorInitializationContext) Implements ISourceGenerator.Initialize
      ' Register a syntax receiver that will be created for each generation pass
      context.RegisterForSyntaxNotifications(Function() As ISyntaxReceiver
                                               Return New SyntaxReceiver
                                             End Function)
    End Sub

    Enum InterfaceType
      [Property]
      Method
      [Event]
    End Enum

    Private Class NeededInterface
      Public Property InterfaceType As InterfaceType
      Public Property FullyQualifiedName As String
      Public Property InterfaceName As String
      Public Property Name As String
      Public Property Parameters As Immutable.ImmutableArray(Of IParameterSymbol)
      Public Property ReturnType As ITypeSymbol
      Public Property Getter As Boolean
      Public Property Setter As Boolean
    End Class

    Public Sub Execute(context As GeneratorExecutionContext) Implements ISourceGenerator.Execute

      If TypeOf context.SyntaxReceiver IsNot SyntaxReceiver Then Return
      Dim receiver = TryCast(context.SyntaxReceiver, SyntaxReceiver)
      Dim compilation = context.Compilation

      'TODO: Need to handle nullable types.

      For Each i In receiver.CandidateImplements

        ' The above collection contains all of the classes as 
        ' well as all of the implements clauses for those classes

        Dim className = CType(i.Parent, ClassBlockSyntax).ClassStatement.Identifier.Text
        If receiver.CandidateClasses.Contains(CType(i.Parent, ClassBlockSyntax).ClassStatement) Then

          Dim neededInterfaces As New List(Of NeededInterface)

          ' First we need to gather all of the desired 
          ' information for the interface implementations...

          Dim model = compilation.GetSemanticModel(i.SyntaxTree)
          For Each tp In i.Types
            Dim fullyQualifiedName = model.GetSymbolInfo(tp).Symbol.ToString
            Dim t = compilation.GetTypeByMetadataName(fullyQualifiedName)
            Dim interfaceName = DirectCast(tp, IdentifierNameSyntax).Identifier.Text

            'TODO: Looks like the addition of parameters on Properties
            '      has caused non-ReadOnly signatures to generate as 
            '      ReadOnly - so is it because the Parameter property
            '      check (LinQ) is incorrectly compared?

            For Each interfaceMember In t?.GetMembers().OfType(Of IMethodSymbol)
              Dim name = interfaceMember.Name
              Select Case interfaceMember?.MethodKind
                Case MethodKind.PropertyGet
#Region "Property Get"
                  Dim existing = (From p In neededInterfaces
                                  Where p.InterfaceType = InterfaceType.Property AndAlso
                                        p.FullyQualifiedName = fullyQualifiedName AndAlso
                                        p?.InterfaceName = interfaceName AndAlso
                                        p?.Parameters = interfaceMember.Parameters AndAlso
                                        p.Name = name?.Substring(4)).FirstOrDefault
                  If existing IsNot Nothing Then
                    existing.Getter = True
                    existing.ReturnType = interfaceMember?.ReturnType
                  Else
                    neededInterfaces.Add(New NeededInterface With {.InterfaceType = InterfaceType.Property,
                                                                        .FullyQualifiedName = fullyQualifiedName,
                                                                        .InterfaceName = interfaceName,
                                                                        .Name = name.Substring(4),
                                                                        .Parameters = interfaceMember.Parameters,
                                                                        .Getter = True,
                                                                        .ReturnType = interfaceMember?.ReturnType})
                  End If
#End Region
                Case MethodKind.PropertySet
#Region "Property Set"
                  Dim existing = (From p In neededInterfaces
                                  Where p.InterfaceType = InterfaceType.Property AndAlso
                                        p.FullyQualifiedName = fullyQualifiedName AndAlso
                                        p?.InterfaceName = interfaceName AndAlso
                                        p?.Parameters = interfaceMember.Parameters AndAlso
                                        p.Name = name.Substring(4)).FirstOrDefault
                  If existing IsNot Nothing Then
                    existing.Setter = True
                  Else
                    neededInterfaces.Add(New NeededInterface With {.InterfaceType = InterfaceType.Property,
                                                                        .FullyQualifiedName = fullyQualifiedName,
                                                                        .InterfaceName = interfaceName,
                                                                        .Name = name.Substring(4),
                                                                        .Parameters = interfaceMember.Parameters,
                                                                        .Setter = True,
                                                                        .ReturnType = interfaceMember?.ReturnType})
                  End If
#End Region
                Case MethodKind.DeclareMethod, MethodKind.Ordinary
#Region "Method"
                  neededInterfaces.Add(New NeededInterface With {.InterfaceType = InterfaceType.Method,
                                                                      .FullyQualifiedName = fullyQualifiedName,
                                                                      .InterfaceName = interfaceName,
                                                                      .Name = name,
                                                                      .Parameters = interfaceMember.Parameters,
                                                                      .ReturnType = interfaceMember?.ReturnType})
#End Region
                Case MethodKind.EventRaise
#Region "Event"
                  neededInterfaces.Add(New NeededInterface With {.InterfaceType = InterfaceType.Event,
                                                                      .FullyQualifiedName = fullyQualifiedName,
                                                                      .InterfaceName = interfaceName,
                                                                      .Name = name})
#End Region
                Case Else
              End Select
            Next
          Next

          ' Now that we have this, we need to review the current
          ' class implementation to see what, if any, has already
          ' been explicitly implemented...

          'TODO: Need to remove properties/methods that not only
          '      match in name, return type and implements... but also
          '      in parameter count and align type-wise.

          For Each prop In receiver.CandidateProperties
            If CType(prop.Parent, ClassBlockSyntax).ClassStatement.Identifier.Text = className Then
              If prop.ImplementsClause IsNot Nothing Then
                For Each impMember In prop.ImplementsClause.InterfaceMembers
                  For Each needed In neededInterfaces
                    If $"{impMember}" = $"{needed.InterfaceName}.{needed.Name}" Then
                      neededInterfaces.Remove(needed) : Exit For
                    End If
                  Next
                Next
              End If
            End If
          Next

          For Each method In receiver.CandidateMethods
            If CType(method.Parent.Parent, ClassBlockSyntax).ClassStatement.Identifier.Text = className Then
              If method.ImplementsClause IsNot Nothing Then
                For Each impMember In method.ImplementsClause.InterfaceMembers
                  For Each needed In neededInterfaces
                    If $"{impMember}" = $"{needed.InterfaceName}.{needed.Name}" Then
                      neededInterfaces.Remove(needed) : Exit For
                    End If
                  Next
                Next
              End If
            End If
          Next

          Dim need = neededInterfaces.Count > 0

          If need Then

            ' Now that we know what we still need to have a complete
            ' implementation, let's attempt to infer what we can
            ' all the while wiring up where we can.  If nothing to 
            ' wire up to, then skip providing an incomplete implementation
            ' so that VS can bubble the error appropriately.

            Dim sourceCode = $"Partial Class {className}{vbCrLf}"
            sourceCode &= $"{vbCrLf}"

            For Each entry In neededInterfaces

              Select Case entry.InterfaceType
                Case InterfaceType.Property
#Region "Property"

                  Dim found = False
                  Dim what As String = Nothing ' Holds the set/get existing item; a property or field.
                  Dim parameters As String = Nothing
                  Dim pass As String = Nothing

                  For Each prop In receiver.CandidateProperties
                    ' Does the class name match...
                    If CType(prop.Parent, ClassBlockSyntax).ClassStatement.Identifier.Text = className Then
                      Dim propertyName = prop.Identifier.Text
                      ' Does the property name match...
                      If propertyName = entry.Name Then
                        ' Does the return type match...
                        If prop.AsClause?.Type.ToString = entry.ReturnType?.ToString Then
                          ' Does the param count match...
                          If prop.ParameterList?.Parameters.Count = entry.Parameters.Length Then
                            ' Do each of the parameter modifiers/types match (align)?
                            Dim success = True ' Assumed
                            For index = 0 To entry.Parameters.Length - 1
                              Dim isByref = HasByrefModifier(prop.ParameterList.Parameters(index).Modifiers)
                              If (entry.Parameters(index).RefKind = RefKind.Ref AndAlso isByref) OrElse
                                 (entry.Parameters(index).RefKind <> RefKind.Ref AndAlso Not isByref) Then
                                If entry.Parameters(index).Type.ToString <> prop.ParameterList.Parameters(index).AsClause.Type.ToString Then
                                  success = False
                                  Exit For
                                End If
                              End If
                            Next
                            If success Then
                              found = True
                              what = $"{propertyName}"
                              parameters = ""
                              pass = ""
                              For Each param In entry.Parameters
                                If parameters <> "" Then parameters &= ", "
                                If param.IsOptional Then
                                  parameters &= "Optional "
                                End If
                                parameters &= $"{param.Name} As {param.Type}"
                                If param.IsOptional Then
                                  If param.HasExplicitDefaultValue Then
                                    If param.Type.ToString = "String" Then
                                      parameters &= $" = ""{param.ExplicitDefaultValue}"""
                                    Else
                                      parameters &= $" = {param.ExplicitDefaultValue}"
                                    End If
                                  Else
                                    parameters &= " = Nothing"
                                  End If
                                End If
                                If pass <> "" Then pass &= ", "
                                pass &= $"{param.Name}"
                              Next
                              Exit For
                            End If
                          End If
                        End If
                      End If
                    End If
                  Next

                  If Not found AndAlso entry.Parameters.Length = 0 Then
                    For Each field In receiver.CandidateFields
                      If CType(field.Parent, ClassBlockSyntax).ClassStatement.Identifier.Text = className Then
                        For Each decl In field.Declarators
                          For Each identifier In decl.Names
                            Dim fieldName = identifier.Identifier.Text
                            If fieldName = $"m_{ToCamelCase(entry.Name)}" OrElse
                               fieldName = $"_{ToCamelCase(entry.Name)}" Then
                              If decl.AsClause?.Type.ToString = entry.ReturnType?.ToString Then
                                found = True
                                what = $"{fieldName}"
                                GoTo FoundField
                              End If
                            End If
                          Next
                        Next
                      End If
                    Next
                  End If
FoundField:

                  If found AndAlso Not String.IsNullOrWhiteSpace(what) Then
                    sourceCode &= $"  Private{If(entry.Setter AndAlso entry.Getter, " ", If(entry.Getter, " ReadOnly ", " WriteOnly "))}Property {entry.InterfaceName}_{entry.Name}({parameters}) As {entry.ReturnType} Implements {entry.FullyQualifiedName}.{entry.Name}{vbCrLf}"
                    If entry.Getter Then
                      sourceCode &= $"    Get{vbCrLf}"
                      sourceCode &= $"      Return {what}{If(pass IsNot Nothing, $"({pass})", "")}{vbCrLf}"
                      sourceCode &= $"    End Get{vbCrLf}"
                    End If
                    If entry.Setter Then
                      sourceCode &= $"    Set(value As {entry.ReturnType}){vbCrLf}"
                      sourceCode &= $"      {what}{If(pass IsNot Nothing, $"({pass})", "")} = value{vbCrLf}"
                      sourceCode &= $"    End Set{vbCrLf}"
                    End If
                    sourceCode &= $"  End Property{vbCrLf}"
                    sourceCode &= $"{vbCrLf}"
                    need = True
                  End If

#End Region
                Case InterfaceType.Method
#Region "Method"

                  Dim found = False
                  Dim what As String = Nothing
                  Dim parameters As String = Nothing
                  Dim pass As String = Nothing

                  For Each method In receiver.CandidateMethods
                    Dim stuff = method.ToString
                    Dim findClassName = CType(method.Parent.Parent, ClassBlockSyntax).ClassStatement.Identifier.Text
                    ' Does the class name match...
                    If className = findClassName Then
                      Dim methodName = method.Identifier.Text
                      ' Does the method name match...
                      If methodName = entry.Name Then
                        ' Does the return (if a Function) type match...
                        If method.AsClause Is Nothing AndAlso entry.ReturnType.ToString = "Void" OrElse
                           method.AsClause?.Type.ToString = entry.ReturnType?.ToString Then
                          ' Does the param count match...
                          If method.ParameterList.Parameters.Count = entry.Parameters.Length Then
                            ' Do each of the parameter modifiers/types match (align)?
                            Dim success = True ' Assumed
                            For index = 0 To entry.Parameters.Length - 1
                              Dim isByref = HasByrefModifier(method.ParameterList.Parameters(index).Modifiers)
                              If (entry.Parameters(index).RefKind = RefKind.Ref AndAlso isByref) OrElse
                                 (entry.Parameters(index).RefKind <> RefKind.Ref AndAlso Not isByref) Then
                                If entry.Parameters(index).Type.ToString <> method.ParameterList.Parameters(index).AsClause.Type.ToString Then
                                  success = False
                                  Exit For
                                End If
                              End If
                            Next
                            If success Then
                              found = True
                              what = methodName
                              parameters = ""
                              pass = ""
                              For Each param In entry.Parameters
                                If parameters <> "" Then parameters &= ", "
                                If param.IsOptional Then
                                  parameters &= "Optional "
                                End If
                                If param.RefKind = RefKind.Ref Then
                                  parameters &= "Byref "
                                End If
                                parameters &= $"{param.Name} As {param.Type}"
                                If param.IsOptional Then
                                  If param.HasExplicitDefaultValue Then
                                    If param.Type.ToString = "String" Then
                                      parameters &= $" = ""{param.ExplicitDefaultValue}"""
                                    Else
                                      parameters &= $" = {param.ExplicitDefaultValue}"
                                    End If
                                  Else
                                    parameters &= " = Nothing"
                                  End If
                                End If
                                If pass <> "" Then pass &= ", "
                                pass &= $"{param.Name}"
                              Next
                              Exit For
                            End If
                          End If
                        End If
                      End If
                    End If
                  Next

                  If found Then
                    If $"{entry.ReturnType}" = "Void" Then ' Sub
                      sourceCode &= $"  Private Sub {entry.InterfaceName}_{entry.Name}({parameters}) Implements {entry.FullyQualifiedName}.{entry.Name}{vbCrLf}"
                      sourceCode &= $"    Call {what}({pass}){vbCrLf}"
                      sourceCode &= $"  End Sub{vbCrLf}"
                    Else ' Function
                      sourceCode &= $"  Private Function {entry.InterfaceName}_{entry.Name}({parameters}) As {entry.ReturnType} Implements {entry.FullyQualifiedName}.{entry.Name}{vbCrLf}"
                      sourceCode &= $"    Return {what}({pass}){vbCrLf}"
                      sourceCode &= $"  End Function{vbCrLf}"
                    End If
                    sourceCode &= $"{vbCrLf}"
                  End If

#End Region
                Case InterfaceType.Event
#Region "Event"
                  'Stop
#End Region
                Case Else
              End Select

            Next

            sourceCode &= $"End Class"

            context.AddSource($"{className}_ImplicitInterface.vb", SourceText.From(sourceCode, Encoding.UTF8))

          End If

        End If

      Next

    End Sub

    Private Function HasByrefModifier(modifiers As SyntaxTokenList) As Boolean
      If modifiers.Count > 0 Then
        For Each entry In modifiers
          If entry.ToString = "Byref" Then
            Return True
          End If
        Next
      End If
      Return False
    End Function

    Private Shared Function ToCamelCase(value As String) As String
      Return value(0).ToString.ToLower & value.Substring(1)
    End Function

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