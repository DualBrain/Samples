<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
  Inherits System.Windows.Forms.Form

  'Form overrides dispose to clean up the component list.
  <System.Diagnostics.DebuggerNonUserCode()> _
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    Try
      If disposing AndAlso components IsNot Nothing Then
        components.Dispose()
      End If
    Finally
      MyBase.Dispose(disposing)
    End Try
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> _
  Private Sub InitializeComponent()
        Me.WebView = New Microsoft.Web.WebView2.WinForms.WebView2()
        Me.AddressTextBox = New System.Windows.Forms.TextBox()
        Me.GoButton = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'WebView
        '
        Me.WebView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.WebView.CreationProperties = Nothing
        Me.WebView.Location = New System.Drawing.Point(12, 41)
        Me.WebView.Name = "WebView"
        Me.WebView.Size = New System.Drawing.Size(776, 397)
        Me.WebView.Source = New System.Uri("https://gotbasic.com", System.UriKind.Absolute)
        Me.WebView.TabIndex = 0
        Me.WebView.Text = "WebView21"
        Me.WebView.ZoomFactor = 1.0R
        '
        'AddressTextBox
        '
        Me.AddressTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.AddressTextBox.Location = New System.Drawing.Point(12, 12)
        Me.AddressTextBox.Name = "AddressTextBox"
        Me.AddressTextBox.Size = New System.Drawing.Size(695, 23)
        Me.AddressTextBox.TabIndex = 1
        '
        'GoButton
        '
        Me.GoButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GoButton.Location = New System.Drawing.Point(713, 11)
        Me.GoButton.Name = "GoButton"
        Me.GoButton.Size = New System.Drawing.Size(75, 23)
        Me.GoButton.TabIndex = 2
        Me.GoButton.Text = "Go!"
        Me.GoButton.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.GoButton)
        Me.Controls.Add(Me.AddressTextBox)
        Me.Controls.Add(Me.WebView)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents WebView As Microsoft.Web.WebView2.WinForms.WebView2
    Friend WithEvents AddressTextBox As TextBox
    Friend WithEvents GoButton As Button
End Class
