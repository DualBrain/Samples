Option Explicit On
Option Strict On
Option Infer On

'Imports ImplicitInterfacGenerator

Public Interface IPerson
  Property FirstName As String
  Property LastName As String
  Property Birth As Date

  Sub WishHappyBirthday()
  Function Age() As Integer

  Event ThisHappened(sender As Object, e As EventArgs)

  Interface WhatInTheSamH
    Sub Dude()
  End Interface

  Class EvenMoreWtf
    Public Property Here As Integer
  End Class

  Structure HolyWtf
    Public Item1 As Integer
  End Structure

End Interface

'<Implicit>
Public Class Boss
  Implements IPerson

  ' This is what we start with...

  Public Property FirstName As String
  Public Property LastName As String

  Public Sub WishHappyBirthday()
    ' Do something
  End Sub

  Public Function Age() As Integer
    Return 1
  End Function

End Class

Partial Class Boss

  Private Event IPerson_ThisHappened(sender As Object, e As EventArgs) Implements IPerson.ThisHappened

  Private Property IPerson_FirstName As String Implements IPerson.FirstName
    Get
      Return Me.FirstName
    End Get
    Set(value As String)
      Me.FirstName = value
    End Set
  End Property

  Private Property IPerson_LastName As String Implements IPerson.LastName
    Get
      Return Me.LastName
    End Get
    Set(value As String)
      Me.LastName = value
    End Set
  End Property

  Private Property IPerson_Birth As Date Implements IPerson.Birth
    Get
      Throw New NotImplementedException
    End Get
    Set(value As Date)
      Throw New NotImplementedException
    End Set
  End Property

  Private Sub IPerson_WishHappyBirthday() Implements IPerson.WishHappyBirthday
    WishHappyBirthday()
  End Sub

  Private Function IPerson_Age() As Integer Implements IPerson.Age
    Return Age()
  End Function

End Class