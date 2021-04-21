Public Class Form1

  Sub New()

    ' This call is required by the designer.
    InitializeComponent()

    ' Add any initialization after the InitializeComponent() call.

    WrapControls

    'ResumeLayout(False)

    'ResumeLayout(True)
    'PerformLayout()

  End Sub

  Private Sub WrapControls()
    Controls.Add(New Community.Windows.FormsEx.PanelEx(ListBox1) With {.BorderColor = Color.Silver})
  End Sub

  Protected Overrides Sub OnLoad(e As EventArgs)
    MyBase.OnLoad(e)
  End Sub

  Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load

    For x = 1 To 100
      ListBox1.Items.Add(x)
    Next

  End Sub

End Class
