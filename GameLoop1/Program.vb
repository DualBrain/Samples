Option Explicit On
Option Strict On
Option Infer On

Module Program

  <STAThread>
  Sub Main()
    Application.SetHighDpiMode(HighDpiMode.SystemAware)
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault(False)
    Application.Run(New Form1())
  End Sub

End Module