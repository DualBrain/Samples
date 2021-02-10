Imports Microsoft.Web.WebView2.Core

Public Class Form1

  Private Async Sub Me_Load(sender As Object, e As EventArgs) Handles Me.Load

    ' Enabled darkmode titlebar (if possible) - on Windows 10.
    DarkMode.ToggleImmersiveDarkMode(Handle, True)

    Me.BackColor = Color.FromArgb(255, 40, 40, 40)
    Me.ForeColor = Color.Silver

    MenuStrip1.Renderer = New ToolStripProfessionalRenderer(New DarkColorTable)
    MenuStrip1.ForeColor = Color.Silver 'Color.White
    For Each item As ToolStripMenuItem In MenuStrip1.Items
      For Each subItem In item.DropDownItems
        If TypeOf subItem Is ToolStripMenuItem Then
          CType(subItem, ToolStripMenuItem).ForeColor = Color.Silver 'Color.White
        End If
      Next
    Next

    StatusStrip1.BackColor = Me.BackColor
    StatusStrip1.ForeColor = Me.ForeColor

    'ContextMenuStrip1.Renderer = New ToolStripProfessionalRenderer(New DarkColorTable)
    'ContextMenuStrip1.ForeColor = Color.Silver 'Color.White
    'For Each item As ToolStripMenuItem In ContextMenuStrip1.Items
    '  For Each subItem In item.DropDownItems
    '    If TypeOf subItem Is ToolStripMenuItem Then
    '      CType(subItem, ToolStripMenuItem).ForeColor = Color.Silver 'Color.White
    '    End If
    '  Next
    'Next


    WebView21.Source = New Uri("https://gotbasic.com")
    Await InitializeAsync()

  End Sub

  Async Function InitializeAsync() As Task
    Await WebView21.EnsureCoreWebView2Async(Nothing)
    AddHandler WebView21.CoreWebView2.WebMessageReceived, AddressOf WebView_CoreWebView2_WebMessageReceived
    Await WebView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);")
    Await WebView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.addEventListener('message', event => alert(event.data));")
  End Function

  'Private Sub GoButton_Click(sender As Object, e As EventArgs) Handles GoButton.Click
  '  If WebView IsNot Nothing AndAlso WebView.CoreWebView2 IsNot Nothing Then
  '    Try
  '      Dim url = AddressTextBox.Text
  '      If Not (url?.ToLower.StartsWith("http://") OrElse
  '              url?.ToLower.StartsWith("https://")) Then
  '        url = $"http://{url}"
  '      End If
  '      WebView.CoreWebView2.Navigate(url)
  '    Catch ex As ArgumentException
  '      MsgBox(ex.Message)
  '    End Try
  '  End If
  'End Sub

  Private Sub WebView21_NavigationStarting(sender As Object, args As CoreWebView2NavigationStartingEventArgs) Handles WebView21.NavigationStarting
    Dim uri = args.Uri
    If Not uri.StartsWith("https://") Then
      WebView21.CoreWebView2.ExecuteScriptAsync($"alert('{uri} is not safe, try an https link')")
      args.Cancel = True
    End If
  End Sub

  Private Sub WebView_CoreWebView2_WebMessageReceived(sender As Object, e As CoreWebView2WebMessageReceivedEventArgs)

    Dim uri = e.TryGetWebMessageAsString()
    Me.Text = uri
    WebView21.CoreWebView2.PostWebMessageAsString(uri)

  End Sub

End Class