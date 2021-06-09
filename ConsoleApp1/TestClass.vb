Public Interface IPerson
  Property FirstName As String
  Property LastName As String
  Property Birth As Date
  Sub WishHappyBirthday()
  Function Age() As Integer
End Interface

Public Class Boss
  Implements IPerson

  Private m_whatever, m_birth, m_death As Date

  Public Property FirstName As String
  Public Property LastName As String

  'Public Property Birth As Date

  Public Property Birth As Date Implements IPerson.Birth
    Get
      Return m_birth
    End Get
    Set(value As Date)
      m_birth = value
    End Set
  End Property

  Public Shared Sub WishHappyBirthday()
    ' Do something
  End Sub

  Public Function Age() As Integer
    Return m_birth.Year
  End Function

End Class

Partial Class Boss

  Private Property IPerson_FirstName As String Implements IPerson.FirstName
    Get
      Return FirstName
    End Get
    Set(value As String)
      FirstName = value
    End Set
  End Property

  Private Property IPerson_LastName As String Implements IPerson.LastName
    Get
      Return LastName
    End Get
    Set(value As String)
      LastName = value
    End Set
  End Property

  'Private Property IPerson_Birth As Date Implements IPerson.Birth
  '  Get
  '    Return m_birth
  '  End Get
  '  Set(value As Date)
  '    m_birth = value
  '  End Set
  'End Property

  Private Sub IPerson_WishHappyBirthday() Implements IPerson.WishHappyBirthday
    Call WishHappyBirthday()
  End Sub

  Private Function IPerson_Age() As Integer Implements IPerson.Age
    Return Age()
  End Function

End Class
