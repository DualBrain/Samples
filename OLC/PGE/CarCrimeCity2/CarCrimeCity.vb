Option Explicit On
Option Strict On
Option Infer On

Imports Olc

Public Class cCarCrimeCity
  Inherits PixelGameEngine

  Friend Sub New()
    AppName = "Car Crime City"
  End Sub

  Protected Overrides Function OnUserCreate() As Boolean
    Return True
  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean
    Return True
  End Function

End Class
