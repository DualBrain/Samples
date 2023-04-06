Option Explicit On
Option Strict On
Option Infer On

Friend Module Randoms

  Private m_rnd As New Random
  Private m_seed As Integer

  Public Property Seed As Integer
    Get
      Return m_seed
    End Get
    Set(value As Integer)
      m_seed = value
      m_rnd = New Random(value)
    End Set
  End Property

  Sub New()
    m_seed = Environment.TickCount
  End Sub

  Public Function RandomByte(count As Integer) As Byte
    Return RandomBytes(1)(0)
  End Function

  Public Function RandomBytes(count As Integer) As Byte()
    Dim b = New Byte(count - 1) {}
    m_rnd.NextBytes(b)
    Return b
  End Function

  Public Function RandomInt(min As Integer, max As Integer) As Integer
    Return m_rnd.[Next](min, max)
  End Function

  Public Function RandomFloat(Optional min As Single = 0, Optional max As Single = 1) As Single
    Return CSng(m_rnd.NextDouble()) * (max - min) + min
  End Function

End Module