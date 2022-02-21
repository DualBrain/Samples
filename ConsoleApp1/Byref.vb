Option Explicit On
Option Strict On
Option Infer On
Module ByrefTest

  Structure PDQ
    Public P As Integer
    Public D As Decimal
    Public Q As Long
    Sub FooBar()
      ByrefTest.FooBar(Me)
    End Sub
  End Structure

  Sub FooBar(ByRef dst As PDQ)
    dst.P = 123
  End Sub

  Sub Main1()

    'Dim a As Short = 1
    'Dim b As Short = a + 1

    Dim o As PDQ = Nothing
    o.FooBar() ' Call FooBar within the structure...
    Console.WriteLine($"{o.P}") ' Output: 0
    FooBar(o) ' Call FooBar outside of the structure...
    Console.WriteLine($"{o.P}") ' Output: 123
  End Sub

End Module