<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
  Inherits System.Windows.Forms.Form

  'Form overrides dispose to clean up the component list.
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

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()>
  Private Sub InitializeComponent()
    Button1 = New Button()
    PictureBox1 = New PictureBox()
    CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
    SuspendLayout()
    ' 
    ' Button1
    ' 
    Button1.Location = New Point(29, 19)
    Button1.Margin = New Padding(1, 1, 1, 1)
    Button1.Name = "Button1"
    Button1.Size = New Size(79, 22)
    Button1.TabIndex = 0
    Button1.Text = "Button1"
    Button1.UseVisualStyleBackColor = True
    ' 
    ' PictureBox1
    ' 
    PictureBox1.Dock = DockStyle.Fill
    PictureBox1.Location = New Point(0, 0)
    PictureBox1.Margin = New Padding(1, 1, 1, 1)
    PictureBox1.Name = "PictureBox1"
    PictureBox1.Size = New Size(933, 444)
    PictureBox1.TabIndex = 1
    PictureBox1.TabStop = False
    ' 
    ' Form1
    ' 
    AutoScaleDimensions = New SizeF(7F, 15F)
    AutoScaleMode = AutoScaleMode.Font
    ClientSize = New Size(933, 444)
    Controls.Add(Button1)
    Controls.Add(PictureBox1)
    Margin = New Padding(4, 3, 4, 3)
    Name = "Form1"
    Text = "Webcam Example"
    CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
    ResumeLayout(False)
  End Sub

  Friend WithEvents Button1 As Button
  Friend WithEvents PictureBox1 As PictureBox
End Class
