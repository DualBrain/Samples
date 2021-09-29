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
  Public Class FormExGenerator
    Implements ISourceGenerator

    Private Const ATTRIBUTE_TEXT As String = "
Namespace Global.FormExGenerator

  <AttributeUsage(AttributeTargets.Class, Inherited:=False, AllowMultiple:=False)>
  Friend NotInheritable Class FormExAttribute
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
      If TypeOf context.SyntaxReceiver IsNot SyntaxReceiver Then Return
      Dim receiver = TryCast(context.SyntaxReceiver, SyntaxReceiver)

      ' we're going to create a new compilation that contains the attribute.
      ' TODO: we should allow source generators to provide source during initialize, so that this step isn't required.
      Dim options = context.Compilation.SyntaxTrees.First().Options
      Dim compilation = context.Compilation

      ' get the newly bound attribute, and INotifyPropertyChanged
      Dim attributeSymbol = compilation.GetTypeByMetadataName("FormExGenerator.FormExAttribute")

      For Each i In receiver.CandidateClasses

        Dim className = CType(i.Parent, ClassBlockSyntax).ClassStatement.Identifier.Text

        Dim enterAccept = False

        Dim classStatement = CType(i.Parent, ClassBlockSyntax).ClassStatement
        For Each attribute In classStatement.AttributeLists
          If attribute.ToString.StartsWith("<FormEx") Then
            Dim a = attribute.Attributes
            For Each aa In a
              For Each arg In aa.ArgumentList.Arguments
                If arg.GetFirstToken.ToString = "EnterAccept" AndAlso arg.GetExpression.ToString = "True" Then
                  enterAccept = True
                End If
              Next
            Next
          End If
        Next

        Dim layoutControlExists = FieldExists(receiver, className, "LayoutControl1", "DevExpress.XtraLayout.LayoutControl")
        Dim dataWatcher1Exists = FieldExists(receiver, className, "DataWatcher1", "SSDD.Windows.Forms.DataWatcher")
        Dim dxErrorProvider1Exists = FieldExists(receiver, className, "DxErrorProvider1", "DXErrorProvider")
        Dim nameTextEditExists = FieldExists(receiver, className, "NameTextEdit", "DevExpress.XtraEditors.TextEdit")
        Dim applyActionButtonExists = FieldExists(receiver, className, "ApplyActionButton", "DevExpress.XtraEditors.SimpleButton")
        Dim cancelActionButtonExists = FieldExists(receiver, className, "CancelActionButton", "DevExpress.XtraEditors.SimpleButton")
        Dim acceptActionButtonExists = FieldExists(receiver, className, "AcceptActionButton", "DevExpress.XtraEditors.SimpleButton")

        ' Options
        Dim sourceCode = $"Option Explicit On
Option Strict On
Option Infer On{vbCrLf}{vbCrLf}"

        ' Imports
        If applyActionButtonExists OrElse
           cancelActionButtonExists OrElse
           acceptActionButtonExists OrElse
           nameTextEditExists Then
          sourceCode &= $"Imports DevExpress.XtraEditors{vbCrLf}"
        End If
        If dxErrorProvider1Exists Then
          sourceCode &= $"Imports DevExpress.XtraEditors.DXErrorProvider{vbCrLf}"
        End If
        If layoutControlExists Then
          sourceCode &= $"Imports DevExpress.XtraLayout{vbCrLf}"
        End If
        sourceCode &= $"Imports SSDD.Data
Imports SSDD.Windows.Forms
Imports SSDD.Windows.Forms.Utils
Imports SSDD.Extensions{vbCrLf}{vbCrLf}"

        ' Class
        sourceCode &= $"Partial Class {className}{vbCrLf}{vbCrLf}"

        ' Private variables
        sourceCode &= $"  Private m_dataTable As System.Data.DataTable
  Private m_default As System.Data.DataTable
  Private m_data As SSDD.Data.BaseEntity
  Private ReadOnly m_login As SSDD.Data.Login

  Private m_itemId As Long?
  Private ReadOnly m_itemName As String

  Private ReadOnly m_duplicate As Boolean = False
  Private m_loading As Boolean = False
  Private m_applied As Boolean = False{vbCrLf}{vbCrLf}"

#Region "Properties"

        sourceCode &= $"#Region ""Public Properties""

  Public ReadOnly Property IsLoading As Boolean
    Get
      Return m_loading
    End Get
  End Property  
  Public ReadOnly Property ItemId() As Long?
    Get
      Return m_itemId
    End Get
  End Property  
  Public ReadOnly Property ItemName() As String
    Get
      Return {If(nameTextEditExists, "NameTextEdit?.Text", "Nothing")}
    End Get
  End Property
      
#End Region{vbCrLf}{vbCrLf}"

#End Region

#Region "Initialization"

        sourceCode &= $"#Region ""Initialization""

  Public Sub New(login As SSDD.Data.Login, data As BaseEntity)

    ' This call is required by the Windows Form Designer.
    InitializeComponent()

    ' Add any initialization after the InitializeComponent() call.
    m_login = login
    m_itemId = New Long?

    m_data = data

    Initialize()

  End Sub

  Public Sub New(login As SSDD.Data.Login, data As BaseEntity, itemId As Long)

    ' This call is required by the Windows Form Designer.
    InitializeComponent()

    ' Add any initialization after the InitializeComponent() call.
    m_login = login
    m_itemId = itemId

    m_data = data

    Initialize()

  End Sub

  Public Sub New(login As SSDD.Data.Login, data As SSDD.Data.BaseEntity, itemId As Long, duplicate As Boolean)

    ' This call is required by the Windows Form Designer.
    InitializeComponent()

    ' Add any initialization after the InitializeComponent() call.
    m_login = login
    m_itemId = itemId

    m_data = data

    m_duplicate = duplicate

    Initialize()

  End Sub

#End Region{vbCrLf}{vbCrLf}"

#End Region

#Region "Generated_Load"

        sourceCode &= $"  Private Async Sub Generated_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    Dim handle = ShowProgressPanel(Me)

    Try{vbCrLf}{vbCrLf}"

        If dataWatcher1Exists Then
          sourceCode &= $"      If DataWatcher1 IsNot Nothing Then
        DataWatcher1.Visualize = True
        If Not m_duplicate Then
          DataWatcher1.SuspendWatcher()
        End If
      End If{vbCrLf}{vbCrLf}"
        End If

        sourceCode &= $"      If m_itemId Is Nothing Then
        Text = String.Format(Text, ""NEW"")
      Else
        Text = String.Format(Text, m_itemId.ToString)
      End If

      LoadWindow(m_login, Me, SpellChecker1)

      FixLayout()

      Try

        Await InternalInitializeDataAsync()

      Finally{vbCrLf}{vbCrLf}"

        If dataWatcher1Exists Then
          sourceCode &= $"        If DataWatcher1 IsNot Nothing Then
          If Not m_duplicate Then
            DataWatcher1.ResumeWatcher()
            AcceptChanges()
          End If
        End If{vbCrLf}{vbCrLf}"
        End If

        sourceCode &= $"        InternalUpdateEnabledState()
        InternalUpdateButtonState()

      End Try{vbCrLf}{vbCrLf}"

        If MethodExists(receiver, className, "LoadCompletedAsync", New List(Of String), "Task") Then
          sourceCode &= $"      Await LoadCompletedAsync(){vbCrLf}{vbCrLf}"
        End If

        If MethodExists(receiver, className, "LoadCompleted", New List(Of String), "Void") Then
          sourceCode &= $"      LoadCompleted(){vbCrLf}{vbCrLf}"
        End If

        sourceCode &= $"    Finally
      CloseProgressPanel(handle)
    End Try

    End Sub{vbCrLf}{vbCrLf}"
#End Region

#Region "Generated_KeyDown"

        sourceCode &= $"  Private Sub Generated_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
    If e.KeyCode = Keys.Escape Then
      e.Handled = True
      Me.Close(){vbCrLf}"
        If acceptActionButtonExists Then
          sourceCode &= $"    ElseIf e.KeyCode = Keys.Enter Then
      If {If(enterAccept, "True", "False")} AndAlso AcceptActionButton.Enabled Then
        e.Handled = True
        AcceptActionButton.PerformClick()
      End If{vbCrLf}"
        End If
        sourceCode &= $"    End If
  End Sub{vbCrLf}{vbCrLf}"

#End Region

#Region "Generated_FormClosing"

        sourceCode &= $"  Private Sub Generated_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

    If IsDataChanged() Then 'DataWatcher1?.ChangedList.Count > 0 Then
      If DevExpress.XtraEditors.XtraMessageBox.Show(""Are you sure you want to cancel any changes made?"", ""Abort Changes"", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.No Then
        e.Cancel = True
        Return
      End If
    End If

    SaveWindow(m_login, Me)

    If m_applied Then
      DialogResult = DialogResult.OK
    End If

  End Sub{vbCrLf}{vbCrLf}"

#End Region

        ' SetApplied
        sourceCode &= $"  Private Sub SetApplied()
    m_applied = True
  End Sub{vbCrLf}{vbCrLf}"

        ' IsDataChanged
        If Not MethodExists(receiver, className, "IsDataChanged", New List(Of String), "Boolean") Then
          sourceCode &= $"  Private Function IsDataChanged() As Boolean
    Return {If(dataWatcher1Exists, "If(DataWatcher1?.ChangedList.Count > 0, False)", "False")}
  End Function{vbCrLf}{vbCrLf}"
        End If

        ' AcceptActionButton_Click
        If acceptActionButtonExists Then
          sourceCode &= $"  Private Async Sub AcceptActionButton_Click(sender As Object, e As EventArgs) Handles AcceptActionButton.Click
    If IsDataChanged() Then
      Await InternalApplyChangesAsync()
    End If
    Me.DialogResult = DialogResult.OK
    Me.Close()
  End Sub{vbCrLf}{vbCrLf}"
        End If

        ' ApplyActionButton_Click
        If applyActionButtonExists Then
          sourceCode &= $"  Private Async Sub ApplyActionButton_Click(sender As Object, e As EventArgs) Handles ApplyActionButton.Click
    Await InternalApplyChangesAsync()
  End Sub{vbCrLf}{vbCrLf}"
        End If

        ' CancelActionButton_Click
        If cancelActionButtonExists Then
          sourceCode &= $"  Private Sub CancelActionButton_Click(sender As Object, e As EventArgs) Handles CancelActionButton.Click
    Me.DialogResult = DialogResult.Cancel
    Me.Close()
  End Sub{vbCrLf}{vbCrLf}"
        End If

        ' DataWatcher1_Changed
        If dataWatcher1Exists Then
          sourceCode &= $"  Private Sub DataWatcher1_Changed(sender As Object, e As SSDD.Windows.Forms.DataEntryChangedEventArgs) Handles DataWatcher1.Changed
    InternalValidateControl(e.Control)
  End Sub{vbCrLf}{vbCrLf}"
        End If

#Region "InternalInitializeDataAsync"

        sourceCode &= $"  Private Async Function InternalInitializeDataAsync() As Task

    Await InitializeLookupsAsync(m_login)

    If m_itemId IsNot Nothing Then

      Do
        Try
          m_dataTable = Await m_data.ReadAsync(CLng(m_itemId))
          Exit Do
        Catch ex As Exception
          Select Case SqlRetryCancel(False, ex)
            Case DialogResult.Retry
            Case DialogResult.Cancel
              m_dataTable = Nothing
              Exit Do
            Case Else ' Rethrow the exception...
              Throw
          End Select
        End Try
      Loop

      If m_dataTable IsNot Nothing Then{vbCrLf}{vbCrLf}"

        If nameTextEditExists Then
          sourceCode &= $"        If NameTextEdit IsNot Nothing Then NameTextEdit.Text = m_dataTable?.Rows(0)(""Name"").ToString{vbCrLf}{vbCrLf}"
        End If

        sourceCode &= $"        InitializeData(m_login, m_dataTable)

        If m_duplicate Then
          m_itemId = New Long?
        End If

      End If

    Else

      ' Otherwise, define defaults.

      InitializeDefault(m_login)

    End If

    ValidateAllControls()

  End Function{vbCrLf}{vbCrLf}"

#End Region

#Region "InternalUpdateButtonState"

        sourceCode &= $"  Private Sub InternalUpdateButtonState(){vbCrLf}{vbCrLf}"

        If cancelActionButtonExists Then
          sourceCode &= $"    If Not CancelActionButton.Visible Then CancelActionButton.Visible = True{vbCrLf}{vbCrLf}"
        End If

        sourceCode &= $"    If m_itemId IsNot Nothing AndAlso
       Not IsDataChanged() Then{vbCrLf}{vbCrLf}"

        If acceptActionButtonExists Then sourceCode &= $"      If AcceptActionButton.Visible Then AcceptActionButton.Visible = False{vbCrLf}"
        If cancelActionButtonExists Then
          sourceCode &= $"      CancelActionButton.Text = ""&Close""
      CancelActionButton.Enabled = True{vbCrLf}"
        End If
        If applyActionButtonExists Then sourceCode &= $"      If ApplyActionButton.Visible Then ApplyActionButton.Visible = False{vbCrLf}"
        sourceCode &= $"{vbCrLf}"
        sourceCode &= $"    Else{vbCrLf}{vbCrLf}"

        If acceptActionButtonExists Then sourceCode &= $"      If Not AcceptActionButton.Visible Then AcceptActionButton.Visible = True{vbCrLf}"
        If cancelActionButtonExists Then
          sourceCode &= $"      CancelActionButton.Text = ""&Cancel""
      CancelActionButton.Enabled = True{vbCrLf}"
        End If
        If applyActionButtonExists Then sourceCode &= $"      If Not ApplyActionButton.Visible Then ApplyActionButton.Visible = True{vbCrLf}"
        sourceCode &= $"{vbCrLf}"
        sourceCode &= $"      If IsDataChanged() AndAlso
         IsDataValid() Then{vbCrLf}{vbCrLf}"

        If acceptActionButtonExists Then sourceCode &= $"        AcceptActionButton.Enabled = True{vbCrLf}"
        If cancelActionButtonExists Then sourceCode &= $"        CancelActionButton.Enabled = True{vbCrLf}"
        If applyActionButtonExists Then sourceCode &= $"        ApplyActionButton.Enabled = True{vbCrLf}"
        sourceCode &= $"{vbCrLf}"
        sourceCode &= $"      Else{vbCrLf}{vbCrLf}"

        If acceptActionButtonExists Then sourceCode &= $"        AcceptActionButton.Enabled = Not IsDataChanged(){vbCrLf}"
        If cancelActionButtonExists Then sourceCode &= $"        CancelActionButton.Enabled = Not AcceptActionButton.Enabled{vbCrLf}"
        If applyActionButtonExists Then sourceCode &= $"        ApplyActionButton.Enabled = False{vbCrLf}"
        sourceCode &= $"{vbCrLf}"
        sourceCode &= $"      End If

    End If

    FixLayout()

  End Sub{vbCrLf}{vbCrLf}"

#End Region

#Region "InternalUpdateEnabledState"

        sourceCode &= $"  Private Sub InternalUpdateEnabledState(){vbCrLf}{vbCrLf}"

        If nameTextEditExists Then
          sourceCode &= $"    If NameTextEdit IsNot Nothing Then NameTextEdit.Enabled = True{vbCrLf}{vbCrLf}"
        End If

        sourceCode &= $"    UpdateTabs({If(dxErrorProvider1Exists, "DXErrorProvider1", "Nothing")}, {If(layoutControlExists, "LayoutControl1", "Nothing")})

    UpdateEnabledState()

  End Sub{vbCrLf}{vbCrLf}"

#End Region

        ' UpdateButtonState
        sourceCode &= $"  Private Sub UpdateButtonState()
    InternalUpdateButtonState()
  End Sub{vbCrLf}{vbCrLf}"

#Region "InternalValidateControl"

        sourceCode &= $"  Private Sub InternalValidateControl(control As Control){vbCrLf}{vbCrLf}"

        If dxErrorProvider1Exists AndAlso nameTextEditExists Then
          sourceCode &= $"    If DXErrorProvider1 IsNot Nothing AndAlso
       NameTextEdit IsNot Nothing AndAlso
       control Is NameTextEdit Then

      If NameTextEdit?.Text.NotEmpty Then
        DXErrorProvider1.SetError(control, "")
      Else
        DXErrorProvider1.SetError(control, ""Required."")
      End If

    Else

      ValidateControl(control)

    End If{vbCrLf}{vbCrLf}"
        Else
          sourceCode &= $"    ValidateControl(control){vbCrLf}{vbCrLf}"
        End If

        sourceCode &= $"    If Not m_loading Then
      UpdateTabs({If(dxErrorProvider1Exists, "DXErrorProvider1", "Nothing")}, {If(layoutControlExists, "LayoutControl1", "Nothing")})
    End If

    InternalUpdateButtonState()

  End Sub{vbCrLf}{vbCrLf}"

#End Region

#Region "InternalApplyChangesAsync"

        sourceCode &= $"  Private Async Function InternalApplyChangesAsync() As Task{vbCrLf}{vbCrLf}"

        If acceptActionButtonExists Then sourceCode &= "    Dim acceptActionEnabled = AcceptActionButton.Enabled{vbCrLf}"
        If cancelActionButtonExists Then sourceCode &= "    Dim cancelActionEnabled = CancelActionButton.Enabled{vbCrLf}"
        If applyActionButtonExists Then sourceCode &= "    Dim applyActionEnabled = ApplyActionButton.Enabled{vbCrLf}"
        If acceptActionButtonExists OrElse
           cancelActionButtonExists OrElse
           applyActionButtonExists Then
          sourceCode &= $"{vbCrLf}"
        End If
        If acceptActionButtonExists Then sourceCode &= "    AcceptActionButton.Enabled = False{vbCrLf}"
        If cancelActionButtonExists Then sourceCode &= "    CancelActionButton.Enabled = False{vbCrLf}"
        If applyActionButtonExists Then sourceCode &= "    ApplyActionButton.Enabled = False{vbCrLf}"
        If acceptActionButtonExists OrElse
           cancelActionButtonExists OrElse
           applyActionButtonExists Then
          sourceCode &= $"{vbCrLf}"
        End If
        sourceCode &= $"    Try

      Dim changeType As ChangeNotification.ChangeType

      If m_itemId IsNot Nothing Then

        ' Update

        Dim historyComment As String = Nothing

        Using ff As New SSDD.Windows.Forms.ReasonDialog(SSDD.Windows.Data.Singleton.REASON_CAPTION, SSDD.Windows.Data.Singleton.REASON_PROMPT, g_license.SpellCheck)
          If ff.ShowDialog(Me) = DialogResult.OK Then
            historyComment = ff.Reason
          Else
            Return
          End If
        End Using

        Await UpdateAsync(m_data, CLng(m_itemId), historyComment)

        changeType = SSDD.Windows.Forms.ChangeNotification.ChangeType.Updated

      Else

        ' Add

        m_itemId = Await CreateAsync(m_data)

        changeType = SSDD.Windows.Forms.ChangeNotification.ChangeType.Created

      End If{vbCrLf}{vbCrLf}"

        If dataWatcher1Exists Then
          sourceCode &= $"      If DataWatcher1 IsNot Nothing Then AcceptChanges(){vbCrLf}{vbCrLf}"
        End If

        sourceCode &= $"      InternalUpdateEnabledState()
      InternalUpdateButtonState()

      If m_data IsNot Nothing AndAlso
         changeType <> SSDD.Windows.Forms.ChangeNotification.ChangeType.None Then
        SSDD.Windows.Data.Singleton.ChangeNotifier.EntityChange(Me, m_data.Name, CLng(m_itemId), m_itemName, changeType)
      End If

      m_applied = True

    Finally{vbCrLf}{vbCrLf}"

        If acceptActionButtonExists Then sourceCode &= $"      AcceptActionButton.Enabled = acceptActionEnabled{vbCrLf}"
        If cancelActionButtonExists Then sourceCode &= $"      CancelActionButton.Enabled = cancelActionEnabled{vbCrLf}"
        If applyActionButtonExists Then sourceCode &= $"      ApplyActionButton.Enabled = applyActionEnabled{vbCrLf}"
        If acceptActionButtonExists OrElse
           cancelActionButtonExists OrElse
           applyActionButtonExists Then
          sourceCode &= $"{vbCrLf}"
        End If

        sourceCode &= $"    End Try

  End Function{vbCrLf}{vbCrLf}"

#End Region

        ' CreateAsync
        If Not MethodExists(receiver, className, "CreateAsync", New List(Of String) From {"BaseEntity"}, "Task(Of Long?)") Then
          sourceCode &= $"  Private Async Function CreateAsync(data As BaseEntity) As Task(Of Long?)
    Throw New NotImplementedException(""'Async Function CreateAsync(data As BaseEntity) As Task(Of Long?)' not implemented."")
  End Function{vbCrLf}{vbCrLf}"
        End If

        ' UpdateAsync
        If Not MethodExists(receiver, className, "UpdateAsync", New List(Of String) From {"BaseEntity", "Long", "String"}, "Task") Then
          sourceCode &= $"  Private Async Function UpdateAsync(data As BaseEntity, id As Long, historyComment As String) As Task
    Throw New NotImplementedException(""'Async Function UpdateAsync(data As BaseEntity, id As Long, historyComment As String) As Task' not implemented."")
  End Function{vbCrLf}{vbCrLf}"
        End If

#Region "IsDataValid"

        If Not MethodExists(receiver, className, "IsDataValid", New List(Of String), "Boolean") Then
          sourceCode &= $"  Private Function IsDataValid() As Boolean
    Throw New NotImplementedException(""'Function IsDataValid() As Boolean' not implemented; be sure to include InternalDataValid()."")
  End Function{vbCrLf}{vbCrLf}"
        End If

#End Region

#Region "InternalIsDataValid"

        sourceCode &= $"  Private Function InternalIsDataValid() As Boolean{vbCrLf}"
        If nameTextEditExists Then
          sourceCode &= $"    Return NameTextEdit Is Nothing OrElse Not String.IsNullOrWhiteSpace(NameTextEdit?.Text){vbCrLf}"
        Else
          sourceCode &= $"    Return True{vbCrLf}"
        End If
        sourceCode &= $"  End Function{vbCrLf}{vbCrLf}"

#End Region

#Region "AcceptChanges"

        If Not MethodExists(receiver, className, "AcceptChanges", New List(Of String), "Void") Then
          sourceCode &= $"  Private Sub AcceptChanges(){vbCrLf}"
          If dataWatcher1Exists Then
            sourceCode &= $"    If DataWatcher1 IsNot Nothing Then DataWatcher1.AcceptChanges(){vbCrLf}"
          Else
            sourceCode &= $"{vbCrLf}"
          End If
          sourceCode &= $"  End Sub{vbCrLf}{vbCrLf}"
        End If

#End Region

        ' ValidateAllControls
        If Not MethodExists(receiver, className, "ValidateAllControls", New List(Of String), "Void") Then
          sourceCode &= $"  Private Sub ValidateAllControls(){vbCrLf}"
          If nameTextEditExists Then
            sourceCode &= $"    Throw New NotImplementedException(""'Sub ValidateAllControls()' not implemented; be sure to include 'Call InternalValidateControl(NameTextEdit)'.""){vbCrLf}"
          Else
            sourceCode &= $"    Throw New NotImplementedException(""'Sub ValidateAllControls()' not implemented.""){vbCrLf}"
          End If
          sourceCode &= $"  End Sub{vbCrLf}{vbCrLf}"
        End If

        ' UpdateEnabledState
        If Not MethodExists(receiver, className, "UpdateEnabledState", New List(Of String), "Void") Then
          sourceCode &= $"Private Sub UpdateEnabledState()
    Throw New NotImplementedException(""'Sub UpdateEnabledState()' not implemented."")
  End Sub{vbCrLf}{vbCrLf}"
        End If

        ' Close of Class
        sourceCode &= $"End Class"

        context.AddSource($"{className}_FormEx.vb", SourceText.From(sourceCode, Encoding.UTF8))

      Next

    End Sub

    Private Function FieldExists(receiver As SyntaxReceiver,
                                 className As String,
                                 findName As String,
                                 findType As String) As Boolean

      Dim success = False

      For Each field In receiver.CandidateFields
        Dim findClassName = CType(field.Parent, ClassBlockSyntax).ClassStatement.Identifier.Text
        ' Does the class name match...
        If className = findClassName Then
          For Each declarator In field.Declarators
            ' Does the method name match...
            If findName = declarator.GetFirstToken.ToString Then
              ' Does the return (if a Function) type match...
              If findType = declarator.AsClause.Type.ToString Then
                ' Does the param count match...
                success = True ' Assumed
                Exit For
              End If
            End If
          Next
          If success Then
            Exit For
          End If
        End If
      Next

      Return success

    End Function

    Private Function MethodExists(receiver As SyntaxReceiver,
                                  className As String,
                                  findMethodName As String,
                                  findParams As List(Of String),
                                  findReturnType As String) As Boolean

      Dim success = False

      For Each method In receiver.CandidateMethods
        Dim stuff = method.ToString
        Dim findClassName = CType(method.Parent.Parent, ClassBlockSyntax).ClassStatement.Identifier.Text
        ' Does the class name match...
        If className = findClassName Then
          Dim methodName = method.Identifier.Text
          ' Does the method name match...
          If methodName = findMethodName Then
            ' Does the return (if a Function) type match...
            If method.AsClause Is Nothing AndAlso findReturnType = "Void" OrElse
               method.AsClause?.Type.ToString = findReturnType Then
              ' Does the param count match...
              If method.ParameterList.Parameters.Count = findParams.Count Then
                success = True ' Assumed
                ' Do each of the parameter modifiers/types match (align)?
                For index = 0 To findParams.Count - 1
                  'Dim isByref = HasByrefModifier(method.ParameterList.Parameters(index).Modifiers)
                  'If (entry.Parameters(index).RefKind = RefKind.Ref AndAlso isByref) OrElse
                  '   (entry.Parameters(index).RefKind <> RefKind.Ref AndAlso Not isByref) Then
                  If findParams(index) <> method.ParameterList.Parameters(index).AsClause.Type.ToString Then
                    success = False
                    Exit For
                  End If
                  'End If
                Next
                If success Then
                  Exit For
                End If
              End If
            End If
          End If
        End If
      Next

      Return success

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
        If TypeOf syntaxNode Is ClassStatementSyntax Then
          Dim node = TryCast(syntaxNode, ClassStatementSyntax)
          'If node.AttributeLists.Count > 0 Then
          '  CandidateClasses.Add(node)
          'End If
          For Each attribute In node.AttributeLists
            If attribute.ToString.StartsWith("<FormEx") Then
              CandidateClasses.Add(node)
            End If
          Next
          'ElseIf TypeOf syntaxNode Is ImplementsStatementSyntax Then
          '  Dim node = TryCast(syntaxNode, ImplementsStatementSyntax)
          '  Dim parent = TryCast(node.Parent.ChildNodes(0), ClassStatementSyntax)
          '  If parent IsNot Nothing Then
          '    If Not CandidateClasses.Contains(parent) Then
          '      CandidateClasses.Add(parent)
          '    End If
          '    CandidateImplements.Add(node)
          '  End If
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