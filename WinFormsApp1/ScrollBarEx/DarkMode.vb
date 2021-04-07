Option Explicit On
Option Strict On
Option Infer On

Namespace ScrollBarEx

  ''' <summary>
  ''' Themed  color, could be in properties...
  ''' </summary>
  ''' <remarks></remarks>
  Class DarkMode
    Public Shared ReadOnly Background As Color = Color.FromArgb(50, 50, 50)
    Public Shared ReadOnly Border As Color = Color.FromArgb(50, 50, 50)
    Public Shared ReadOnly Thumb As Color = Color.FromArgb(120, 120, 120)
    Public Shared ReadOnly ThumbHover As Color = Color.FromArgb(160, 160, 160)
  End Class

End Namespace