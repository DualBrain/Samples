Option Explicit On
Option Strict On
Option Infer On

' Requires a reference to QB.Compatibility
' Can be found at https://github.com/DualBrain/QB.Compatibility

Imports System.Windows.Forms

Module Program

  <STAThread>
  Sub Main()
    Application.SetHighDpiMode(HighDpiMode.SystemAware)
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault(False)
    Application.Run(New Form1())
  End Sub

End Module