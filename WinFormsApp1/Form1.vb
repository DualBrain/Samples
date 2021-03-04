﻿Imports Microsoft.Web.WebView2.Core

Public Class Form1

  Private m_enableDarkMode As Boolean = False

  Private Async Sub Me_Load(sender As Object, e As EventArgs) Handles Me.Load

    ToolStripStatusLabel1.Text = "Saying some status..."
    Dim nd = TreeView1.Nodes.Add("Main1")
    nd.Nodes.Add("Sub1")
    nd.Nodes.Add("Sub2")
    nd = TreeView1.Nodes.Add("Main2")
    nd.Nodes.Add("Sub3")
    nd.Nodes.Add("Sub4")

    DarkMode.ToggleImmersiveDarkMode(CType(Controls(0).Parent, Form).Handle, True)

    If m_enableDarkMode Then

      SetDarkMode(Controls)

    Else

      'Dim m As MenuStrip = DirectCast(component, MenuStrip)
      'm.Renderer = m_menuStripLightRenderer
      'm.ForeColor = Color.Black
      'For Each item As ToolStripMenuItem In m.Items
      '  For Each subItem As ToolStripItem In item.DropDownItems
      '    If TypeOf subItem Is ToolStripMenuItem Then
      '      CType(subItem, ToolStripMenuItem).ForeColor = Color.Black
      '    End If
      '  Next
      'Next

    End If

    Await InitializeAsync()

  End Sub

  Private m_capturedRenderer As ToolStripProfessionalRenderer

  Private Sub SetLightMode(controls As Control.ControlCollection)

    For Each c In controls

      If TypeOf c Is MenuStrip Then
        Dim m = CType(c, MenuStrip)
        If m_capturedRenderer IsNot Nothing Then
          m.Renderer = m_capturedRenderer
        End If
        m.ForeColor = SystemColors.ControlText
        For Each item As ToolStripMenuItem In m.Items
          For Each subItem As ToolStripItem In item.DropDownItems
            If TypeOf subItem Is ToolStripMenuItem Then
              CType(subItem, ToolStripMenuItem).ForeColor = SystemColors.ControlText
            End If
          Next
        Next
      ElseIf TypeOf c Is StatusStrip Then
        Dim s = CType(c, StatusStrip)
        s.BackColor = SystemColors.Control
        s.ForeColor = SystemColors.ControlText
      ElseIf TypeOf c Is ToolStrip Then
        Dim ts = CType(c, ToolStrip)
        If m_capturedRenderer IsNot Nothing Then
          ts.Renderer = m_capturedRenderer
        End If
        ts.ForeColor = SystemColors.ControlText
      ElseIf TypeOf c Is SplitContainer Then
        Dim s = CType(c, SplitContainer)
        s.BackColor = SystemColors.Control
        s.ForeColor = SystemColors.ControlText
        SetLightMode(s.Panel1.Controls)
        SetLightMode(s.Panel2.Controls)

      ElseIf TypeOf c Is TabControl Then
        Dim t = CType(c, TabControl)
        t.BackColor = SystemColors.Control
        t.ForeColor = SystemColors.ControlText
        For Each tab As TabPage In t.TabPages
          tab.BackColor = SystemColors.Control
          SetLightMode(tab.Controls)
        Next

      ElseIf TypeOf c Is Panel Then

        Dim p = CType(c, Panel)
        p.BackColor = SystemColors.Control
        p.ForeColor = SystemColors.ControlText
        SetLightMode(p.Controls)

      ElseIf TypeOf c Is RichTextBox Then

        Dim rtb = CType(c, RichTextBox)
        rtb.BackColor = Color.White
        rtb.ForeColor = SystemColors.ControlText

      ElseIf TypeOf c Is TreeView Then
        Dim tvw = CType(c, TreeView)
        tvw.BackColor = Color.White
        tvw.ForeColor = SystemColors.ControlText

      ElseIf TypeOf c Is TextBox Then

        Dim tb = CType(c, TextBox)
        tb.BorderStyle = BorderStyle.FixedSingle
        tb.BackColor = Color.White
        tb.ForeColor = SystemColors.ControlText

      ElseIf TypeOf c Is Button Then

        Dim btn = CType(c, Button)
        btn.BackColor = SystemColors.Control
        btn.ForeColor = SystemColors.ControlText


      ElseIf TypeOf c Is Microsoft.Web.WebView2.WinForms.WebView2 Then
        ' do nothing...

      Else
        MsgBox($"Unhandled Control: {c.ToString}")
      End If

    Next
  End Sub

  Private Sub SetDarkMode(controls As Control.ControlCollection)

    For Each c In controls

      If TypeOf c Is MenuStrip Then
        Dim m = CType(c, MenuStrip)
        If m_capturedRenderer Is Nothing Then m_capturedRenderer = m.Renderer
        m.Renderer = New ToolStripProfessionalRenderer(New DarkColorTable)
        m.ForeColor = Color.Silver 'Color.White
        For Each item As ToolStripMenuItem In m.Items
          For Each subItem As ToolStripItem In item.DropDownItems
            If TypeOf subItem Is ToolStripMenuItem Then
              CType(subItem, ToolStripMenuItem).ForeColor = Color.Silver 'Color.White
            End If
          Next
        Next
      ElseIf TypeOf c Is StatusStrip Then
        Dim s = CType(c, StatusStrip)
        s.BackColor = Color.FromArgb(40, 40, 40)
        s.ForeColor = Color.Silver
      ElseIf TypeOf c Is ToolStrip Then
        Dim ts = CType(c, ToolStrip)
        ts.Renderer = New ToolStripProfessionalRenderer(New DarkColorTable)
        ts.ForeColor = Color.Silver 'Color.White
      ElseIf TypeOf c Is SplitContainer Then
        Dim s = CType(c, SplitContainer)
        s.BackColor = Color.FromArgb(40, 40, 40)
        s.ForeColor = Color.Silver
        SetDarkMode(s.Panel1.Controls)
        SetDarkMode(s.Panel2.Controls)

      ElseIf TypeOf c Is TabControl Then
        Dim t = CType(c, TabControl)
        t.BackColor = Color.FromArgb(40, 40, 40)
        t.ForeColor = Color.Silver
        For Each tab As TabPage In t.TabPages
          tab.BackColor = Color.FromArgb(40, 40, 40)
          tab.BorderStyle = BorderStyle.None
          SetDarkMode(tab.Controls)
        Next

      ElseIf TypeOf c Is Panel Then

        Dim p = CType(c, Panel)
        p.BackColor = Color.FromArgb(40, 40, 40)
        p.ForeColor = Color.Silver
        SetDarkMode(p.Controls)

      ElseIf TypeOf c Is RichTextBox Then

        Dim rtb = CType(c, RichTextBox)
        rtb.BackColor = Color.FromArgb(60, 60, 60)
        rtb.ForeColor = Color.Silver
        rtb.BorderStyle = BorderStyle.None

      ElseIf TypeOf c Is TreeView Then
        Dim tvw = CType(c, TreeView)
        tvw.BackColor = Color.FromArgb(40, 40, 40)
        tvw.ForeColor = Color.Silver
        tvw.BorderStyle = BorderStyle.None
      ElseIf TypeOf c Is TextBox Then

        Dim tb = CType(c, TextBox)
        tb.BorderStyle = BorderStyle.FixedSingle
        tb.BackColor = Color.FromArgb(60, 60, 60)
        tb.ForeColor = Color.Silver

      ElseIf TypeOf c Is Button Then

        Dim btn = CType(c, Button)
        btn.FlatStyle = FlatStyle.Flat
        btn.BackColor = Color.FromArgb(60, 60, 60)
        btn.ForeColor = Color.Silver


      ElseIf TypeOf c Is Microsoft.Web.WebView2.WinForms.WebView2 Then
        ' do nothing...

      Else
        MsgBox($"Unhandled Control: {c.ToString}")
      End If

    Next
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

  Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
    Select Case e.KeyCode
      Case Keys.F5
        e.Handled = True
        If m_enableDarkMode Then
          SetLightMode(Controls)
        Else
          SetDarkMode(Controls)
        End If
        m_enableDarkMode = Not m_enableDarkMode
      Case Else
    End Select
  End Sub

End Class
