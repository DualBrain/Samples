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

  <FormEx(EnterAccept:=False)>
  Public Class Form1
    Inherits System.Windows.Forms.Form

    'Private WithEvents ApplyActionButton As DevExpress.XtraEditors.SimpleButton
    'Private WithEvents CancelActionButton As DevExpress.XtraEditors.SimpleButton
    'Private WithEvents AcceptActionButton As DevExpress.XtraEditors.SimpleButton
    'Private WithEvents SpellChecker1 As DevExpress.XtraSpellChecker.SpellChecker
    'Private WithEvents DataWatcher1 As SSDD.Windows.Forms.DataWatcher
    'Private WithEvents DxErrorProvider1 As DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider
    'Private WithEvents NameTextEdit As DevExpress.XtraEditors.TextEdit
    'Private WithEvents LayoutControl1 As DevExpress.XtraLayout.LayoutControl

    Private Sub Initialize()
      LayoutControl1.Enabled = False
    End Sub

    Private Sub UpdateEnabledState()

      Dim agencyId = ObjectToInteger(AgencyComboBoxEditEx.SelectedValue)

      NameTextEdit.Enabled = True
      BillingNameTextEdit.Enabled = True
      ContactTextEdit.Enabled = agencyId Is Nothing
      EmailTextEdit.Enabled = agencyId Is Nothing
      StreetTextEdit.Enabled = agencyId Is Nothing
      PostalTextEditEx.Enabled = agencyId Is Nothing
      StateComboBoxEditEx.Enabled = agencyId Is Nothing
      CityTextEdit.Enabled = agencyId Is Nothing
      PhoneTextEdit.Enabled = agencyId Is Nothing
      MobileTextEdit.Enabled = agencyId Is Nothing
      FaxTextEdit.Enabled = agencyId Is Nothing
      StatusComboBoxEditEx.Enabled = True
      ExternalCodeTextEdit.Enabled = True

      ' Defaults

      AgencyComboBoxEditEx.Enabled = True
      AccountRepComboBoxEditEx.Enabled = agencyId Is Nothing
      ProductProtectComboBoxEditEx.Enabled = agencyId Is Nothing
      MinimumSeparationTextEditEx.Enabled = agencyId Is Nothing
      ConfirmationDeliveryComboBoxEditEx.Enabled = agencyId Is Nothing
      ConfirmationEmailTextEdit.Enabled = agencyId Is Nothing

      ' Billing

      RevenueSourceComboBoxEditEx.Enabled = agencyId Is Nothing
      RevenueTypeComboBoxEditEx.Enabled = agencyId Is Nothing
      TaxComboBoxEditEx.Enabled = agencyId Is Nothing
      TaxNumberTextEdit.Enabled = agencyId Is Nothing
      LateChargeTextEditEx.Enabled = agencyId Is Nothing
      CreditLimitTextEditEx.Enabled = agencyId Is Nothing
      CreditPolicyComboBoxEditEx.Enabled = agencyId Is Nothing
      InvoiceCycleComboBoxEditEx.Enabled = agencyId Is Nothing
      InvoiceTypeComboBoxEditEx.Enabled = agencyId Is Nothing
      InvoiceDeliveryComboBoxEditEx.Enabled = agencyId Is Nothing
      InvoiceEmailTextEdit.Enabled = agencyId Is Nothing
      StatementCycleComboBoxEditEx.Enabled = agencyId Is Nothing
      StatementTypeComboBoxEditEx.Enabled = agencyId Is Nothing
      StatementDeliveryComboBoxEditEx.Enabled = agencyId Is Nothing
      StatementEmailTextEdit.Enabled = agencyId Is Nothing

    End Sub

    Private Async Function InitializeLookupsAsync(login As SSDD.Data.Login) As Task

      m_loading = True

      Cursor = Cursors.WaitCursor
      Try

        m_default = Nothing
        Dim stateData As DataTable = Nothing
        Dim statusData As DataTable = Nothing
        Dim agencyData As DataTable = Nothing
        Dim accountRepData As DataTable = Nothing
        Dim productProtectData As DataTable = Nothing
        Dim confirmationDeliveryData As DataTable = Nothing
        Dim creditPolicyData As DataTable = Nothing
        Dim revenueSourceData As DataTable = Nothing
        Dim revenueTypeData As DataTable = Nothing
        Dim taxData As DataTable = Nothing
        Dim invoiceCycleData As DataTable = Nothing
        Dim invoiceTypeData As DataTable = Nothing
        Dim invoiceDeliveryData As DataTable = Nothing
        Dim statementCycleData As DataTable = Nothing
        Dim statementTypeData As DataTable = Nothing
        Dim statementDeliveryData As DataTable = Nothing

        Do

          Try

            If m_default Is Nothing Then m_default = Await New DataAccess.Defaults(m_login).ReadAsync(0)
            If stateData Is Nothing Then stateData = Await Lookup.StateTypeAsync(m_login, True)
            If statusData Is Nothing Then statusData = Await Lookup.CustomerStatusAsync(m_login)
            If agencyData Is Nothing Then agencyData = Await New Agency(login).ComboBoxListAsync(True, True, False)
            If accountRepData Is Nothing Then accountRepData = Await New AccountRep(login).ComboBoxListAsync(True, True, False)
            If productProtectData Is Nothing Then productProtectData = Await New ProductCode(login).ComboBoxListAsync(True, True, False)
            If confirmationDeliveryData Is Nothing Then confirmationDeliveryData = Await Lookup.DeliveryTypeAsync(login, True)
            If creditPolicyData Is Nothing Then creditPolicyData = Await Lookup.CreditPolicyTypeAsync(login)
            If revenueSourceData Is Nothing Then revenueSourceData = Await New RevenueSource(login).ComboBoxListAsync(True, True, False)
            If revenueTypeData Is Nothing Then revenueTypeData = Await New RevenueType(login).ComboBoxListAsync(True, True, False)
            If taxData Is Nothing Then taxData = Await New Tax(login).ComboBoxListAsync(True, True, False)
            If invoiceCycleData Is Nothing Then invoiceCycleData = Await New BillingCycle(login).ComboBoxListAsync(True, True, False)
            If invoiceTypeData Is Nothing Then invoiceTypeData = Await Lookup.InvoiceTypeAsync(login, True)
            If invoiceDeliveryData Is Nothing Then invoiceDeliveryData = Await Lookup.DeliveryTypeAsync(login, True)
            If statementCycleData Is Nothing Then statementCycleData = Await New BillingCycle(login).ComboBoxListAsync(True, True, False)
            If statementTypeData Is Nothing Then statementTypeData = Await Lookup.StatementTypeAsync(login, True)
            If statementDeliveryData Is Nothing Then statementDeliveryData = Await Lookup.DeliveryTypeAsync(login, True)

            Exit Do

          Catch ex As Exception

            Select Case SqlRetryCancel(False, ex)
              Case DialogResult.Retry
              Case DialogResult.Cancel

  #Disable Warning IDE0059 ' Unnecessary assignment of a value
                m_default = Nothing
                stateData = Nothing
                statusData = Nothing
                agencyData = Nothing
                accountRepData = Nothing
                productProtectData = Nothing
                confirmationDeliveryData = Nothing
                creditPolicyData = Nothing
                revenueSourceData = Nothing
                revenueTypeData = Nothing
                taxData = Nothing
                invoiceCycleData = Nothing
                invoiceTypeData = Nothing
                invoiceDeliveryData = Nothing
                statementCycleData = Nothing
                statementTypeData = Nothing
                statementDeliveryData = Nothing
  #Enable Warning IDE0059 ' Unnecessary assignment of a value

                Return
              Case Else ' Rethrow the exception...
                Throw
            End Select

          End Try

        Loop

        StateComboBoxEditEx.DataSource = stateData
        StatusComboBoxEditEx.DataSource = statusData

        AgencyComboBoxEditEx.DataSource = agencyData
        AccountRepComboBoxEditEx.DataSource = accountRepData
        ProductProtectComboBoxEditEx.DataSource = productProtectData
        ConfirmationDeliveryComboBoxEditEx.DataSource = confirmationDeliveryData

        CreditPolicyComboBoxEditEx.DataSource = creditPolicyData

        RevenueSourceComboBoxEditEx.DataSource = revenueSourceData
        RevenueTypeComboBoxEditEx.DataSource = revenueTypeData
        TaxComboBoxEditEx.DataSource = taxData
        InvoiceCycleComboBoxEditEx.DataSource = invoiceCycleData
        InvoiceTypeComboBoxEditEx.DataSource = invoiceTypeData
        InvoiceDeliveryComboBoxEditEx.DataSource = invoiceDeliveryData
        StatementCycleComboBoxEditEx.DataSource = statementCycleData
        StatementTypeComboBoxEditEx.DataSource = statementTypeData
        StatementDeliveryComboBoxEditEx.DataSource = statementDeliveryData

      Finally
        Cursor = Cursors.Default
        m_loading = False
      End Try

    End Function

    Private Sub InitializeDefault(login As SSDD.Data.Login)

      'AgencyComboBoxEditEx.SelectedValue = If(CObj(m_default.Rows(0).Field(Of Integer?)("AgencyId")), DBNull.Value)
      AccountRepComboBoxEditEx.SetValue(m_default.Rows(0).Field(Of Integer?)("AccountRepId"))
      ProductProtectComboBoxEditEx.SetValue(m_default.Rows(0).Field(Of Integer?)("ProductCodeId"))
      CreditPolicyComboBoxEditEx.SetValue(m_default.Rows(0).Field(Of Integer?)("CreditPolicyId"))
      MinimumSeparationTextEditEx.Text = m_default.Rows(0).Field(Of Integer?)("SpotSeparation")?.ToString()
      'ConfirmationDeliveryComboBoxEditEx.SelectedValue = If(CObj(m_default.Rows(0).Field(Of Integer?)("ConfirmationDeliveryId")), DBNull.Value)

      RevenueSourceComboBoxEditEx.SetValue(m_default.Rows(0).Field(Of Integer?)("RevenueSourceId"))
      RevenueTypeComboBoxEditEx.SetValue(m_default.Rows(0).Field(Of Integer?)("RevenueTypeId"))
      TaxComboBoxEditEx.SetValue(m_default.Rows(0).Field(Of Integer?)("TaxId"))
      LateChargeTextEditEx.Text = m_default.Rows(0).Field(Of Decimal?)("LateCharge")?.ToString("F2")
      CreditLimitTextEditEx.Text = m_default.Rows(0).Field(Of Decimal?)("CreditLimit")?.ToString("F2")
      ConfirmationDeliveryComboBoxEditEx.SetValue(m_default.Rows(0).Field(Of Integer?)("ConfirmationDeliveryId"))
      InvoiceCycleComboBoxEditEx.SetValue(m_default.Rows(0).Field(Of Integer?)("InvoiceCycleId"))
      InvoiceTypeComboBoxEditEx.SetValue(m_default.Rows(0).Field(Of Integer?)("InvoiceTypeId"))
      InvoiceDeliveryComboBoxEditEx.SetValue(m_default.Rows(0).Field(Of Integer?)("InvoiceDeliveryId"))
      StatementCycleComboBoxEditEx.SetValue(m_default.Rows(0).Field(Of Integer?)("StatementCycleId"))
      StatementTypeComboBoxEditEx.SetValue(m_default.Rows(0).Field(Of Integer?)("StatementTypeId"))
      StatementDeliveryComboBoxEditEx.SetValue(m_default.Rows(0).Field(Of Integer?)("StatementDeliveryId"))

    End Sub

    Private Sub InitializeData(login As SSDD.Data.Login, dt As DataTable)

      ' General

      NameTextEdit.Text = dt.Rows(0).Field(Of String)("Name")
      BillingNameTextEdit.Text = dt.Rows(0).Field(Of String)("BillingName")
      ContactTextEdit.Text = dt.Rows(0).Field(Of String)("Contact")
      EmailTextEdit.Text = dt.Rows(0).Field(Of String)("Email")
      StreetTextEdit.Text = dt.Rows(0).Field(Of String)("Street")
      PostalTextEditEx.Text = dt.Rows(0).Field(Of String)("Postal")
      StateComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("StateId"))
      CityTextEdit.Text = dt.Rows(0).Field(Of String)("City")
      PhoneTextEdit.Text = dt.Rows(0).Field(Of String)("Phone")
      MobileTextEdit.Text = dt.Rows(0).Field(Of String)("Mobile")
      FaxTextEdit.Text = dt.Rows(0).Field(Of String)("Fax")
      StatusComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("CustomerStatusId"))
      ExternalCodeTextEdit.Text = dt.Rows(0).Field(Of String)("ExternalCode")

      ' Defaults

      AgencyComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("AgencyId"))
      AccountRepComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("AccountRepId"))
      ProductProtectComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("ProductCodeId"))
      MinimumSeparationTextEditEx.Text = dt.Rows(0).Field(Of Integer?)("SpotSeparation")?.ToString()
      ConfirmationDeliveryComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("ConfirmationDeliveryId"))
      ConfirmationEmailTextEdit.Text = dt.Rows(0).Field(Of String)("ConfirmationEmail")

      ' Billing

      RevenueSourceComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("RevenueSourceId"))
      RevenueTypeComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("RevenueTypeId"))
      TaxComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("TaxId"))
      TaxNumberTextEdit.Text = dt.Rows(0).Field(Of String)("TaxNumber")
      LateChargeTextEditEx.Text = dt.Rows(0).Field(Of Decimal?)("LateCharge")?.ToString("F2")
      CreditLimitTextEditEx.Text = dt.Rows(0).Field(Of Decimal?)("CreditLimit")?.ToString("F2")
      CreditPolicyComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("CreditPolicyId"))
      InvoiceCycleComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("InvoiceCycleId"))
      InvoiceTypeComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("InvoiceTypeId"))
      InvoiceDeliveryComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("InvoiceDeliveryId"))
      InvoiceEmailTextEdit.Text = dt.Rows(0).Field(Of String)("InvoiceEmail")
      StatementCycleComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("StatementCycleId"))
      StatementTypeComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("StatementTypeId"))
      StatementDeliveryComboBoxEditEx.SetValue(dt.Rows(0).Field(Of Integer?)("StatementDeliveryId"))
      StatementEmailTextEdit.Text = dt.Rows(0).Field(Of String)("StatementEmail")

    End Sub

    Private Sub ValidateControl(control As Control)

      If control Is AccountRepComboBoxEditEx OrElse
         control Is ProductProtectComboBoxEditEx OrElse
         control Is CreditPolicyComboBoxEditEx OrElse
         control Is RevenueSourceComboBoxEditEx OrElse
         control Is RevenueTypeComboBoxEditEx OrElse
         control Is TaxComboBoxEditEx OrElse
         control Is ConfirmationDeliveryComboBoxEditEx OrElse
         control Is InvoiceCycleComboBoxEditEx OrElse
         control Is InvoiceTypeComboBoxEditEx OrElse
         control Is InvoiceDeliveryComboBoxEditEx OrElse
         control Is StatementCycleComboBoxEditEx OrElse
         control Is StatementTypeComboBoxEditEx OrElse
         control Is StatementDeliveryComboBoxEditEx Then

        Dim column As String = Nothing

        If control Is AccountRepComboBoxEditEx Then column = "AccountRepId"
        If control Is ProductProtectComboBoxEditEx Then column = "ProductCodeId"
        If control Is CreditPolicyComboBoxEditEx Then column = "CreditPolicyId"
        If control Is RevenueSourceComboBoxEditEx Then column = "RevenueSourceId"
        If control Is RevenueTypeComboBoxEditEx Then column = "RevenueTypeId"
        If control Is TaxComboBoxEditEx Then column = "TaxId"
        If control Is ConfirmationDeliveryComboBoxEditEx Then column = "ConfirmationDeliveryId"
        If control Is InvoiceCycleComboBoxEditEx Then column = "InvoiceCycleId"
        If control Is InvoiceTypeComboBoxEditEx Then column = "InvoiceTypeId"
        If control Is InvoiceDeliveryComboBoxEditEx Then column = "InvoiceDeliveryId"
        If control Is StatementCycleComboBoxEditEx Then column = "StatementCycleId"
        If control Is StatementTypeComboBoxEditEx Then column = "StatementTypeId"
        If control Is StatementDeliveryComboBoxEditEx Then column = "StatementDeliveryid"

        If column IsNot Nothing Then

          Dim cbx = DirectCast(control, ComboBoxEditEx)
          Dim id = ObjectToInteger(cbx.SelectedValue)

          Dim defaultId = m_default?.Rows(0).Field(Of Integer?)(column)

          If cbx.SelectedIndex > -1 Then
            If g_license.Information AndAlso
               Not defaultId = -1 AndAlso
               If(id, -1) <> If(defaultId, -1) Then
              Dim defaultName = If((From p In cbx.GetaDataTable Where p.Field(Of Integer?)("Id") = defaultId Select p.Field(Of String)("Name")).FirstOrDefault, "None")
              DxErrorProvider1.SetError(control, $"Does not match system default. ('{defaultName}')")
              DxErrorProvider1.SetErrorType(control, ErrorType.Information)
            Else
              DxErrorProvider1.SetError(control, "")
            End If
          Else
            DxErrorProvider1.SetError(control, "Required.")
          End If

        Else
          DxErrorProvider1.SetError(control, "NOT IMPLEMENTED!!!!")
        End If

      ElseIf control Is StatusComboBoxEditEx OrElse
             control Is AgencyComboBoxEditEx OrElse
             control Is ConfirmationDeliveryComboBoxEditEx OrElse
             control Is CreditPolicyComboBoxEditEx Then
        If DirectCast(control, ComboBoxEditEx).SelectedIndex > -1 Then
          DxErrorProvider1.SetError(control, "")
        Else
          DxErrorProvider1.SetError(control, "Required.")
        End If

      ElseIf control Is NameTextEdit Then
        If m_nameTextEdit.Text.NotEmpty Then
          DxErrorProvider1.SetError(control, "")
        Else
          DxErrorProvider1.SetError(control, "Required.")
        End If
      ElseIf control Is MinimumSeparationTextEditEx Then
        Dim value = DirectCast(control, DevExpress.XtraEditors.TextEdit).Text
        If Not String.IsNullOrWhiteSpace(value) AndAlso
           IsNumeric(value) Then
          Dim defaultValue = m_default?.Rows(0).Field(Of Integer?)("SpotSeparation")
          If g_license.Information AndAlso IsNumeric(value) AndAlso CInt(value) <> defaultValue Then
            DxErrorProvider1.SetError(control, $"Does not match system default. ('{defaultValue}')")
            DxErrorProvider1.SetErrorType(control, ErrorType.Information)
          Else
            DxErrorProvider1.SetError(control, "")
          End If
        Else
          DxErrorProvider1.SetError(control, "Required.")
        End If
      ElseIf control Is LateChargeTextEditEx OrElse
             control Is CreditLimitTextEditEx Then
        Dim column As String = Nothing
        If control Is LateChargeTextEditEx Then column = "LateCharge"
        If control Is CreditLimitTextEditEx Then column = "CreditLimit"
        If column IsNot Nothing Then
          Dim value = DirectCast(control, DevExpress.XtraEditors.TextEdit).Text
          If Not String.IsNullOrWhiteSpace(value) Then
            Dim defaultValue = m_default?.Rows(0).Field(Of Decimal?)(column)
            If g_license.Information AndAlso CDec(value) <> defaultValue Then
              DxErrorProvider1.SetError(control, $"Does not match system default. ('{defaultValue}')")
              DxErrorProvider1.SetErrorType(control, ErrorType.Information)
            Else
              DxErrorProvider1.SetError(control, "")
            End If
          Else
            DxErrorProvider1.SetError(control, "Required.")
          End If
        Else
          DxErrorProvider1.SetError(control, "NOT IMPLEMENTED!!!!")
        End If

      ElseIf control Is EmailTextEdit OrElse
             control Is ConfirmationEmailTextEdit OrElse
             control Is InvoiceEmailTextEdit OrElse
             control Is StatementEmailTextEdit Then
        Dim email = DirectCast(control, DevExpress.XtraEditors.TextEdit).Text
        If String.IsNullOrWhiteSpace(email) Then
          DxErrorProvider1.SetError(control, "")
        ElseIf SSDD.Information.IsEmailAddress(email, True) Then
          DxErrorProvider1.SetError(control, "")
        Else
          DxErrorProvider1.SetError(control, "Email appears to be invalid.")
        End If

      ElseIf control Is PhoneTextEdit OrElse
             control Is MobileTextEdit OrElse
             control Is FaxTextEdit Then
        Dim number = DirectCast(control, DevExpress.XtraEditors.TextEdit).Text
        If String.IsNullOrWhiteSpace(number) Then
          DxErrorProvider1.SetError(control, "")
        ElseIf SSDD.Information.IsPhoneNumber(number) Then
          DxErrorProvider1.SetError(control, "")
        Else
          DxErrorProvider1.SetError(control, "Number appears to be invalid.")
        End If

      ElseIf control Is StreetTextEdit OrElse
             control Is CityTextEdit OrElse
             control Is StateComboBoxEditEx OrElse
             control Is PostalTextEditEx Then

  #Region "Address"

        Dim street = StreetTextEdit.Text
        Dim city = CityTextEdit.Text
        Dim state = StateComboBoxEditEx.Text
        Dim postal = PostalTextEditEx.Text

        If String.IsNullOrWhiteSpace(street) AndAlso
           String.IsNullOrWhiteSpace(city) AndAlso
           IsDBNull(StateComboBoxEditEx.SelectedValue) AndAlso
           String.IsNullOrWhiteSpace(postal) Then

          DxErrorProvider1.SetError(StreetTextEdit, "")
          DxErrorProvider1.SetError(CityTextEdit, "")
          DxErrorProvider1.SetError(StateComboBoxEditEx, "")
          DxErrorProvider1.SetError(PostalTextEditEx, "")

          DxErrorProvider1.SetErrorType(StreetTextEdit, ErrorType.None)
          DxErrorProvider1.SetErrorType(CityTextEdit, ErrorType.None)
          DxErrorProvider1.SetErrorType(StateComboBoxEditEx, ErrorType.None)
          DxErrorProvider1.SetErrorType(PostalTextEditEx, ErrorType.None)

        Else

          If String.IsNullOrWhiteSpace(street) Then
            DxErrorProvider1.SetError(StreetTextEdit, "Street required.")
            DxErrorProvider1.SetErrorType(StreetTextEdit, ErrorType.Critical)
          Else
            DxErrorProvider1.SetError(StreetTextEdit, "")
            DxErrorProvider1.SetErrorType(StreetTextEdit, ErrorType.None)
          End If

          If String.IsNullOrWhiteSpace(city) Then
            DxErrorProvider1.SetError(CityTextEdit, "City required.")
            DxErrorProvider1.SetErrorType(CityTextEdit, ErrorType.Critical)
          ElseIf g_license.Information AndAlso
                 Not String.IsNullOrWhiteSpace(m_zipCodeApiCity) AndAlso
                 CityTextEdit.Text <> m_zipCodeApiCity Then
            DxErrorProvider1.SetError(CityTextEdit, "City doesn't appear to match lookup result.")
            DxErrorProvider1.SetErrorType(CityTextEdit, ErrorType.Information)
          Else
            DxErrorProvider1.SetError(CityTextEdit, "")
            DxErrorProvider1.SetErrorType(CityTextEdit, ErrorType.None)
          End If

          If IsDBNull(StateComboBoxEditEx.SelectedValue) OrElse
             StateComboBoxEditEx.SelectedValue Is Nothing Then
            DxErrorProvider1.SetError(StateComboBoxEditEx, "State required.")
            DxErrorProvider1.SetErrorType(StateComboBoxEditEx, ErrorType.Critical)
          ElseIf g_license.Information AndAlso
                 Not IsDBNull(m_zipCodeApiStateId) AndAlso
                 Not StateComboBoxEditEx.SelectedValue.Equals(m_zipCodeApiStateId) Then
            DxErrorProvider1.SetError(StateComboBoxEditEx, "State doesn't appear to match lookup result.")
            DxErrorProvider1.SetErrorType(StateComboBoxEditEx, ErrorType.Information)
          Else
            DxErrorProvider1.SetError(StateComboBoxEditEx, "")
            DxErrorProvider1.SetErrorType(StateComboBoxEditEx, ErrorType.None)
          End If

          If String.IsNullOrWhiteSpace(postal) Then
            DxErrorProvider1.SetError(PostalTextEditEx, "Postal required.")
            DxErrorProvider1.SetErrorType(PostalTextEditEx, ErrorType.Critical)
          ElseIf g_license.Information AndAlso
                 Not SSDD.Information.IsUsPostal(postal) Then
            DxErrorProvider1.SetError(PostalTextEditEx, "Valid postal required.")
            DxErrorProvider1.SetErrorType(PostalTextEditEx, ErrorType.Critical)
          Else
            DxErrorProvider1.SetError(PostalTextEditEx, "")
            DxErrorProvider1.SetErrorType(PostalTextEditEx, ErrorType.None)
          End If

        End If

  #End Region

      End If

    End Sub

    Private Sub ValidateAllControls()

      ' General

      ValidateControl(NameTextEdit)
      ValidateControl(BillingNameTextEdit)
      ValidateControl(ContactTextEdit)
      ValidateControl(EmailTextEdit)
      'ValidateControl(StreetTextEdit)
      'ValidateControl(CityTextEdit)
      'ValidateControl(StateComboBoxEditEx)
      ValidateControl(PostalTextEditEx) ' The above three are handled by this one entry as all four are evaluated together...
      ValidateControl(PhoneTextEdit)
      ValidateControl(MobileTextEdit)
      ValidateControl(FaxTextEdit)
      ValidateControl(StatusComboBoxEditEx)
      ValidateControl(ExternalCodeTextEdit)

      ' Defaults

      ValidateControl(AgencyComboBoxEditEx)
      ValidateControl(AccountRepComboBoxEditEx)
      ValidateControl(ProductProtectComboBoxEditEx)
      ValidateControl(MinimumSeparationTextEditEx)
      ValidateControl(ConfirmationDeliveryComboBoxEditEx)
      ValidateControl(ConfirmationEmailTextEdit)

      ' Billing

      ValidateControl(RevenueSourceComboBoxEditEx)
      ValidateControl(RevenueTypeComboBoxEditEx)
      ValidateControl(TaxComboBoxEditEx)
      ValidateControl(TaxNumberTextEdit)
      ValidateControl(LateChargeTextEditEx)
      ValidateControl(CreditLimitTextEditEx)
      ValidateControl(CreditPolicyComboBoxEditEx)
      ValidateControl(InvoiceCycleComboBoxEditEx)
      ValidateControl(InvoiceTypeComboBoxEditEx)
      ValidateControl(InvoiceDeliveryComboBoxEditEx)
      ValidateControl(InvoiceEmailTextEdit)
      ValidateControl(StatementCycleComboBoxEditEx)
      ValidateControl(StatementTypeComboBoxEditEx)
      ValidateControl(StatementDeliveryComboBoxEditEx)
      ValidateControl(StatementEmailTextEdit)

    End Sub

    Private Function IsDataValid() As Boolean

      Dim addressValid = (String.IsNullOrWhiteSpace(StreetTextEdit.Text) AndAlso
                          String.IsNullOrWhiteSpace(CityTextEdit.Text) AndAlso
                          If(ObjectToInteger(StateComboBoxEditEx.SelectedValue), 0) = 0 AndAlso
                          String.IsNullOrWhiteSpace(PostalTextEditEx.Text)) OrElse
                         (Not String.IsNullOrWhiteSpace(StreetTextEdit.Text) AndAlso
                          Not String.IsNullOrWhiteSpace(CityTextEdit.Text) AndAlso
                          If(ObjectToInteger(StateComboBoxEditEx.SelectedValue), 0) > 0 AndAlso
                          SSDD.Information.IsUsPostal(PostalTextEditEx.Text))

      Return MyBase.IsDataValid() AndAlso
             NameTextEdit.Text.NotEmpty AndAlso
             addressValid AndAlso
             ConfirmationDeliveryComboBoxEditEx.SelectedIndex > -1 AndAlso
             AgencyComboBoxEditEx.SelectedIndex > -1 AndAlso
             AccountRepComboBoxEditEx.SelectedIndex > -1 AndAlso
             ProductProtectComboBoxEditEx.SelectedIndex > -1 AndAlso
             StatusComboBoxEditEx.SelectedIndex > -1 AndAlso
             CreditPolicyComboBoxEditEx.SelectedIndex > -1 AndAlso
             RevenueSourceComboBoxEditEx.SelectedIndex > -1 AndAlso
             RevenueTypeComboBoxEditEx.SelectedIndex > -1 AndAlso
             InvoiceCycleComboBoxEditEx.SelectedIndex > -1 AndAlso
             InvoiceTypeComboBoxEditEx.SelectedIndex > -1 AndAlso
             InvoiceDeliveryComboBoxEditEx.SelectedIndex > -1 AndAlso
             StatementCycleComboBoxEditEx.SelectedIndex > -1 AndAlso
             StatementTypeComboBoxEditEx.SelectedIndex > -1 AndAlso
             StatementDeliveryComboBoxEditEx.SelectedIndex > -1 AndAlso
             (String.IsNullOrWhiteSpace(EmailTextEdit.Text) OrElse SSDD.Information.IsEmailAddress(EmailTextEdit.Text, True)) AndAlso
             (String.IsNullOrWhiteSpace(ConfirmationEmailTextEdit.Text) OrElse SSDD.Information.IsEmailAddress(ConfirmationEmailTextEdit.Text, True)) AndAlso
             (String.IsNullOrWhiteSpace(InvoiceEmailTextEdit.Text) OrElse SSDD.Information.IsEmailAddress(InvoiceEmailTextEdit.Text, True)) AndAlso
             (String.IsNullOrWhiteSpace(StatementEmailTextEdit.Text) OrElse SSDD.Information.IsEmailAddress(StatementEmailTextEdit.Text, True)) AndAlso
             (String.IsNullOrWhiteSpace(PhoneTextEdit.Text) OrElse SSDD.Information.IsPhoneNumber(PhoneTextEdit.Text)) AndAlso
             (String.IsNullOrWhiteSpace(MobileTextEdit.Text) OrElse SSDD.Information.IsPhoneNumber(MobileTextEdit.Text)) AndAlso
             (String.IsNullOrWhiteSpace(FaxTextEdit.Text) OrElse SSDD.Information.IsPhoneNumber(FaxTextEdit.Text))

    End Function

    Private Async Function CreateAsync(data As BaseEntity) As Task(Of Long?)

      Dim name = NameTextEdit.Text
      Dim tag As String = Nothing
      Dim customerStatusId = ObjectToInteger(StatusComboBoxEditEx.SelectedValue)
      Dim billingName = BillingNameTextEdit.Text
      Dim contact = ContactTextEdit.Text
      Dim agencyId = ObjectToInteger(AgencyComboBoxEditEx.SelectedValue)
      Dim street = StreetTextEdit.Text
      Dim city = CityTextEdit.Text
      Dim stateId = ObjectToInteger(StateComboBoxEditEx.SelectedValue)
      Dim postal = PostalTextEditEx.Text
      Dim phone = PhoneTextEdit.Text
      Dim mobile = MobileTextEdit.Text
      Dim fax = FaxTextEdit.Text
      Dim email = EmailTextEdit.Text
      Dim confirmationDeliveryId = New Integer? 'TranslateToInteger(ConfirmationComboBoxEditEx.SelectedValue)
      Dim confirmationEmail = ConfirmationEmailTextEdit.Text
      Dim invoiceTypeId = ObjectToInteger(InvoiceTypeComboBoxEditEx.SelectedValue)
      Dim invoiceCycleId = ObjectToInteger(InvoiceCycleComboBoxEditEx.SelectedValue)
      Dim invoiceDeliveryId = ObjectToInteger(InvoiceDeliveryComboBoxEditEx.SelectedValue)
      Dim invoiceEmail = InvoiceEmailTextEdit.Text
      Dim invoicePath As String = Nothing
      Dim statementTypeId = ObjectToInteger(StatementTypeComboBoxEditEx.SelectedValue)
      Dim statementCycleId = ObjectToInteger(StatementCycleComboBoxEditEx.SelectedValue)
      Dim statementDeliveryId = ObjectToInteger(StatementDeliveryComboBoxEditEx.SelectedValue)
      Dim statementEmail = StatementEmailTextEdit.Text
      Dim loggedTimesEmail As String = Nothing
      Dim accountRepId = ObjectToInteger(AccountRepComboBoxEditEx.SelectedValue)
      Dim revenueSourceId = ObjectToInteger(RevenueSourceComboBoxEditEx.SelectedValue)
      Dim revenueTypeId = ObjectToInteger(RevenueTypeComboBoxEditEx.SelectedValue)
      Dim productCodeId = ObjectToInteger(ProductProtectComboBoxEditEx.SelectedValue)
      Dim spotSeparation = If(IsNumeric(MinimumSeparationTextEditEx.Text), CInt(MinimumSeparationTextEditEx.Text), 0)
      Dim taxId = ObjectToInteger(TaxComboBoxEditEx.SelectedValue)
      Dim taxNumber = TaxNumberTextEdit.Text
      Dim orderCount As Integer?
      Dim creditLimit As Decimal = If(IsNumeric(CreditLimitTextEditEx.Text), CDec(CreditLimitTextEditEx.Text), 0)
      Dim creditPolicyId = ObjectToInteger(CreditPolicyComboBoxEditEx.SelectedValue)
      Dim lateCharge As Decimal = If(IsNumeric(LateChargeTextEditEx.Text), CDec(LateChargeTextEditEx.Text), 0)
      Dim balance As Decimal?
      Dim userId As Integer?
      Dim memo As String = Nothing
      Dim extraInfo As String = Nothing
      Dim externalCode = ExternalCodeTextEdit.Text
      Dim customerSince As Date? = Today
      Dim lastActive As Date?
      Dim lastPayment As Date?
      Return Await CType(data, Account).CreateAsync(name,
                                                     tag,
                                                     customerStatusId,
                                                     billingName,
                                                     contact,
                                                     agencyId,
                                                     street,
                                                     city,
                                                     stateId,
                                                     postal,
                                                     phone,
                                                     mobile,
                                                     fax,
                                                     email,
                                                     confirmationDeliveryId,
                                                     confirmationEmail,
                                                     invoiceTypeId,
                                                     invoiceCycleId,
                                                     invoiceDeliveryId,
                                                     invoiceEmail,
                                                     invoicePath,
                                                     statementTypeId,
                                                     statementCycleId,
                                                     statementDeliveryId,
                                                     statementEmail,
                                                     loggedTimesEmail,
                                                     accountRepId,
                                                     revenueSourceId,
                                                     revenueTypeId,
                                                     productCodeId,
                                                     spotSeparation,
                                                     taxId,
                                                     taxNumber,
                                                     orderCount,
                                                     creditLimit,
                                                     creditPolicyId,
                                                     lateCharge,
                                                     balance,
                                                     userId,
                                                     memo,
                                                     extraInfo,
                                                     externalCode,
                                                     customerSince,
                                                     lastActive,
                                                     lastPayment)
    End Function

    Private Async Function UpdateAsync(data As BaseEntity, id As Long, historyComment As String) As Task
      Dim name = NameTextEdit.Text
      Dim tag As String = Nothing
      Dim customerStatusId = ObjectToInteger(StatusComboBoxEditEx.SelectedValue)
      Dim billingName = BillingNameTextEdit.Text
      Dim contact = ContactTextEdit.Text
      Dim agencyId = ObjectToInteger(AgencyComboBoxEditEx.SelectedValue)
      Dim street = StreetTextEdit.Text
      Dim city = CityTextEdit.Text
      Dim stateId = ObjectToInteger(StateComboBoxEditEx.SelectedValue)
      Dim postal = PostalTextEditEx.Text
      Dim phone = PhoneTextEdit.Text
      Dim mobile = MobileTextEdit.Text
      Dim fax = FaxTextEdit.Text
      Dim email = EmailTextEdit.Text
      Dim confirmationDeliveryId = New Integer? 'TranslateToInteger(ConfirmationComboBoxEditEx.SelectedValue)
      Dim confirmationEmail = ConfirmationEmailTextEdit.Text
      Dim invoiceTypeId = ObjectToInteger(InvoiceTypeComboBoxEditEx.SelectedValue)
      Dim invoiceCycleId = ObjectToInteger(InvoiceCycleComboBoxEditEx.SelectedValue)
      Dim invoiceDeliveryId = ObjectToInteger(InvoiceDeliveryComboBoxEditEx.SelectedValue)
      Dim invoiceEmail = InvoiceEmailTextEdit.Text
      Dim invoicePath As String = Nothing
      Dim statementTypeId = ObjectToInteger(StatementTypeComboBoxEditEx.SelectedValue)
      Dim statementCycleId = ObjectToInteger(StatementCycleComboBoxEditEx.SelectedValue)
      Dim statementDeliveryId = ObjectToInteger(StatementDeliveryComboBoxEditEx.SelectedValue)
      Dim statementEmail = StatementEmailTextEdit.Text
      Dim loggedTimesEmail As String = Nothing
      Dim accountRepId = ObjectToInteger(AccountRepComboBoxEditEx.SelectedValue)
      Dim revenueSourceId = ObjectToInteger(RevenueSourceComboBoxEditEx.SelectedValue)
      Dim revenueTypeId = ObjectToInteger(RevenueTypeComboBoxEditEx.SelectedValue)
      Dim productCodeId = ObjectToInteger(ProductProtectComboBoxEditEx.SelectedValue)
      Dim spotSeparation = If(IsNumeric(MinimumSeparationTextEditEx.Text), CInt(MinimumSeparationTextEditEx.Text), 0)
      Dim taxId = ObjectToInteger(TaxComboBoxEditEx.SelectedValue)
      Dim taxNumber = TaxNumberTextEdit.Text
      Dim orderCount As Integer?
      Dim creditLimit As Decimal = If(IsNumeric(CreditLimitTextEditEx.Text), CDec(CreditLimitTextEditEx.Text), 0)
      Dim creditPolicyId = ObjectToInteger(CreditPolicyComboBoxEditEx.SelectedValue)
      Dim lateCharge As Decimal = If(IsNumeric(LateChargeTextEditEx.Text), CDec(LateChargeTextEditEx.Text), 0)
      Dim balance As Decimal?
      Dim userId As Integer?
      Dim memo As String = Nothing
      Dim extraInfo As String = Nothing
      Dim externalCode = ExternalCodeTextEdit.Text
      Dim customerSince As Date?
      Dim lastActive As Date?
      Dim lastPayment As Date?
      Await CType(data, Account).UpdateAsync(id,
                                              name,
                                              tag,
                                              customerStatusId,
                                              billingName,
                                              contact,
                                              agencyId,
                                              street,
                                              city,
                                              stateId,
                                              postal,
                                              phone,
                                              mobile,
                                              fax,
                                              email,
                                              confirmationDeliveryId,
                                              confirmationEmail,
                                              invoiceTypeId,
                                              invoiceCycleId,
                                              invoiceDeliveryId,
                                              invoiceEmail,
                                              invoicePath,
                                              statementTypeId,
                                              statementCycleId,
                                              statementDeliveryId,
                                              statementEmail,
                                              loggedTimesEmail,
                                              accountRepId,
                                              revenueSourceId,
                                              revenueTypeId,
                                              productCodeId,
                                              spotSeparation,
                                              taxId,
                                              taxNumber,
                                              orderCount,
                                              creditLimit,
                                              creditPolicyId,
                                              lateCharge,
                                              balance,
                                              userId,
                                              memo,
                                              extraInfo,
                                              externalCode,
                                              customerSince,
                                              lastActive,
                                              lastPayment,
                                              historyComment)
    End Function

    Private Sub LoadCompleted()
      If LayoutControl1 IsNot Nothing Then
        Try
          LayoutControl1.Enabled = True
        Catch ex As NullReferenceException
          ' Ignore - can occur due to the window being closed before it reaches this point.
        End Try
      End If
      UpdateEnabledState()
      If ItemId Is Nothing AndAlso
         NameTextEdit.CanFocus Then
        NameTextEdit.Focus()
      End If
    End Sub

#Region "Postal"

    Private m_zipCodeApiStateId As Object = DBNull.Value
    Private m_zipCodeApiCity As String

    Private Async Sub PostalTextEdit_EditValueChanged(sender As Object, e As EventArgs) Handles PostalTextEditEx.EditValueChanged

      Dim postal = PostalTextEditEx.Text

      If String.IsNullOrWhiteSpace(postal) Then
        m_zipCodeApiCity = Nothing
        m_zipCodeApiStateId = DBNull.Value
        Return
      ElseIf Not SSDD.Information.IsUsPostal(postal) Then
        m_zipCodeApiCity = Nothing
        m_zipCodeApiStateId = DBNull.Value
        Return
      End If

      If postal.Contains("-") Then
        postal = postal.Substring(0, postal.IndexOf("-"))
      End If

      Dim zip = Await GetZipInfoAsync(postal)

      If zip Is Nothing OrElse
         zip.Rows.Count = 0 Then
        m_zipCodeApiCity = Nothing
        m_zipCodeApiStateId = DBNull.Value
        Return
      End If

      Dim city = zip.Rows(0).Field(Of String)("City")
      Dim stateId = zip.Rows(0).Field(Of Integer?)("StateId")

      m_zipCodeApiCity = city
      m_zipCodeApiStateId = If(CObj(stateId), DBNull.Value)

      If (IsDBNull(StateComboBoxEditEx.SelectedValue) OrElse StateComboBoxEditEx.SelectedValue Is Nothing) AndAlso
         Not IsDBNull(m_zipCodeApiStateId) Then
        StateComboBoxEditEx.SelectedValue = m_zipCodeApiStateId
      End If

      If String.IsNullOrWhiteSpace(CityTextEdit.Text) AndAlso
         Not String.IsNullOrWhiteSpace(m_zipCodeApiCity) Then
        CityTextEdit.Text = m_zipCodeApiCity
      End If

      'ValidateControl(StateComboBoxEditEx)
      ValidateControl(CityTextEdit) ' Only need to call once since the "address" is evaluated as a collection.

    End Sub

#End Region

    Private Sub AgencyComboBoxEditEx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles AgencyComboBoxEditEx.SelectedIndexChanged
      If m_loading Then Return
      UpdateEnabledState()
    End Sub

  End Class

]]>.Value


    '    Dim source = <![CDATA[

    'Namespace TopOne.LevelTwo

    'Public Interface IJob

    '  Event OurEvent As EventHandler(Of EventArgs)

    '  Property Description As String
    '  Property Pay As Currency
    '  Sub WishHappyBirthday()
    '  Function SampleFunction() As Integer

    '  Property SamplePropertyWithParam(param1 As Integer) As Integer
    '  Property SamplePropertyWithParams(param1 As Integer, param2 As Integer) As Integer

    '  Function SampleFunctionWithParam(param1 As String) As Integer
    '  Function SampleFunctionWithParam(Byref param1 As Integer) As Integer
    '  Function SampleFunctionWithParams(param1 As Integer, param2 As String) As Integer

    '  Sub Something(ByRef test1 As Integer)

    'End Interface

    'Public Interface IPerson
    '  Property FirstName As String
    '  Property LastName As String
    '  Property Birth As Date
    '  Sub WishHappyBirthday()
    '  Function Age() As Integer
    'End Interface

    'Public Class Boss
    '  Implements IPerson, IJob

    '  Public Event OurEvent As EventHandler(Of EventArgs)

    '  Private m_birth As Date

    '  Public Property SamplePropertyWithParam(param1 As Integer) As Integer
    '  Public Property SamplePropertyWithParams(param1 As Integer, param2 As Integer) As Integer

    '  Public Property FirstName As String Implements IPerson.FirstName
    '  Public Property LastName As String

    '  Public Property Description As String
    '  Public Property Pay As Currency

    '  Public Sub WishHappyBirthday() Implements IPerson.WishHappyBirthday
    '    ' Do something
    '  End Sub

    '  Public Function Age() As Integer
    '    Return m_birth.Year
    '  End Function

    '  Public Function SampleFunctionWithParam(param1 As String) As Integer
    '    Return CInt(param1?.Length)
    '  End Function

    '  Function SampleFunctionWithParam(param1 As Integer) As Integer
    '    Return param1
    '  End Function

    '  Function SampleFunctionWithParams(Optional Byref byrefParam1 as Long = 5) As Integer
    '    return 0
    '  End Function

    '  Function SampleFunctionWithParams(param1 As Integer, param2 As String) As Integer
    '    Return param1 + CInt(param2?.Length)
    '  End Function

    '  Sub Something(Optional ByRef something1 As Integer = 5)
    '  End Sub

    'End Class

    'End Namespace

    ']]>.Value

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
    'Dim generator1 As ISourceGenerator = New DualBrain.ImplicitInterfaceGenerator
    Dim generator1 As ISourceGenerator = New DualBrain.FormExGenerator

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