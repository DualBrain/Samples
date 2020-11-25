Option Explicit Off
Option Strict Off
Option Infer Off

Partial Module Program

  Public Function CreateA(value As Integer) As ClassLibrary1.A
#Disable Warning IDE0017 ' Simplify object initialization
    Dim newA = New ClassLibrary1.A
#Enable Warning IDE0017 ' Simplify object initialization
    newA.A1 = value
    Return newA
  End Function

End Module
