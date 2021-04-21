Public Class Form1

  Sub New()

    ' This call is required by the designer.
    InitializeComponent()

    ' Add any initialization after the InitializeComponent() call.

    ResumeLayout(False)

    Controls.Remove(ListBox1)
    Controls.Add(New PanelEx(ListBox1) With {.BorderColor = Color.Pink})

    Controls.Remove(RichTextBox1)
    Controls.Add(New PanelEx(RichTextBox1) With {.BorderColor = Color.Pink})

    ResumeLayout(True)
    PerformLayout()

  End Sub

End Class
