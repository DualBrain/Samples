Imports Microsoft.Web.WebView2.Core

Public Class Form1

  Private Async Sub Me_Load(sender As Object, e As EventArgs) Handles Me.Load
    Await InitializeAsync()
  End Sub

  Async Function InitializeAsync() As Task
    Await WebView.EnsureCoreWebView2Async(Nothing)
    AddHandler WebView.CoreWebView2.WebMessageReceived, AddressOf WebView_CoreWebView2_WebMessageReceived
    Await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);")
    Await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.addEventListener('message', event => alert(event.data));")
  End Function

  Private Sub GoButton_Click(sender As Object, e As EventArgs) Handles GoButton.Click
    If WebView IsNot Nothing AndAlso WebView.CoreWebView2 IsNot Nothing Then
      Try
        Dim url = AddressTextBox.Text
        If Not (url?.ToLower.StartsWith("http://") OrElse
                url?.ToLower.StartsWith("https://")) Then
          url = $"http://{url}"
        End If
        WebView.CoreWebView2.Navigate(url)
      Catch ex As ArgumentException
        MsgBox(ex.Message)
      End Try
    End If
  End Sub

  Private Sub WebView_NavigationStarting(sender As Object, args As CoreWebView2NavigationStartingEventArgs) Handles WebView.NavigationStarting
    Dim uri = args.Uri
    If Not uri.StartsWith("https://") Then
      WebView.CoreWebView2.ExecuteScriptAsync($"alert('{uri} is not safe, try an https link')")
      args.Cancel = True
    End If
  End Sub

  Private Sub WebView_CoreWebView2_WebMessageReceived(sender As Object, e As CoreWebView2WebMessageReceivedEventArgs)

    Dim uri = e.TryGetWebMessageAsString()
    AddressTextBox.Text = uri
    WebView.CoreWebView2.PostWebMessageAsString(uri)

  End Sub

End Class
