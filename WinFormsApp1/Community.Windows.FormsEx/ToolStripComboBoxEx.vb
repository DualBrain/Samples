Option Explicit On
Option Infer On
Option Strict On

Namespace Global.Community.Windows.FormsEx

  Public Class ToolStripComboBoxEx
    Inherits ToolStripControlHost

    Public Sub New()
      MyBase.New(New ComboBoxEx With {.BorderDrawMode = ComboBoxEx.ControlBorderDrawMode.InternalFaded, .FlatStyle = FlatStyle.Flat})
    End Sub

  End Class

End Namespace