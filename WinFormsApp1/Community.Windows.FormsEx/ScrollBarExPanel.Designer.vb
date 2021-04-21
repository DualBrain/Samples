Namespace Global.Community.Windows.FormsEx

  Partial Class ScrollBarExPanel_Remove
    Inherits System.Windows.Forms.Panel

    <System.Diagnostics.DebuggerNonUserCode()>
    Public Sub New(ByVal container As System.ComponentModel.IContainer)
      MyClass.New()

      'Required for Windows.Forms Class Composition Designer support
      If (container IsNot Nothing) Then
        container.Add(Me)
      End If

    End Sub

    <System.Diagnostics.DebuggerNonUserCode()>
    Public Sub New()
      MyBase.New()

      'This call is required by the Component Designer.
      InitializeComponent()

    End Sub

    'Component overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
      Try
        If disposing AndAlso components IsNot Nothing Then
          components.Dispose()
        End If
      Finally
        MyBase.Dispose(disposing)
      End Try
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()

      components = New System.ComponentModel.Container()

      Me.VScrollBar1 = New VScrollBarEx
      Me.SuspendLayout()
      '
      'VScrollBar1
      '
      Me.VScrollBar1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
      Me.VScrollBar1.LargeChange = 1
      Me.VScrollBar1.Location = New System.Drawing.Point(145, 0)
      Me.VScrollBar1.Maximum = 10
      Me.VScrollBar1.Minimum = 0
      Me.VScrollBar1.MinimumSize = New System.Drawing.Size(19, 15)
      Me.VScrollBar1.Name = "VScrollBar1"
      Me.VScrollBar1.Size = New System.Drawing.Size(19, 127)
      Me.VScrollBar1.SmallChange = 1
      Me.VScrollBar1.TabIndex = 0
      Me.VScrollBar1.Value = 10
      '
      'ScrollBarExPanel
      '
      Me.BackColor = System.Drawing.Color.Gainsboro
      Me.Controls.Add(Me.VScrollBar1)
      Me.Size = New System.Drawing.Size(164, 127)
      Me.ResumeLayout(False)

    End Sub

    Friend WithEvents VScrollBar1 As VScrollBarEx

  End Class

End Namespace