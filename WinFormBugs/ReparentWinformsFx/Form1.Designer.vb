﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
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
    Me.ListBox1 = New System.Windows.Forms.ListBox()
    Me.RichTextBox1 = New System.Windows.Forms.RichTextBox()
    Me.SuspendLayout()
    '
    'ListBox1
    '
    Me.ListBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.ListBox1.FormattingEnabled = True
    Me.ListBox1.Location = New System.Drawing.Point(12, 12)
    Me.ListBox1.Name = "ListBox1"
    Me.ListBox1.Size = New System.Drawing.Size(212, 158)
    Me.ListBox1.TabIndex = 0
    '
    'RichTextBox1
    '
    Me.RichTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
    Me.RichTextBox1.Location = New System.Drawing.Point(297, 13)
    Me.RichTextBox1.Name = "RichTextBox1"
    Me.RichTextBox1.Size = New System.Drawing.Size(301, 159)
    Me.RichTextBox1.TabIndex = 1
    Me.RichTextBox1.Text = ""
    '
    'Form1
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(800, 450)
    Me.Controls.Add(Me.RichTextBox1)
    Me.Controls.Add(Me.ListBox1)
    Me.Name = "Form1"
    Me.Text = "Form1"
    Me.ResumeLayout(False)

  End Sub

  Friend WithEvents ListBox1 As ListBox
  Friend WithEvents RichTextBox1 As RichTextBox
End Class
