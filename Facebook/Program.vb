Imports System

Module Program

  Sub Main(args As String())

    'Console.WriteLine("Hello World!")

    Dim consumerKey As String
    Dim consumerKeySecret As String
    Dim accessToken As String
    Dim accessTokenSecret As String

    Dim twitter = New TwitterApi(consumerKey, consumerKeySecret, accessToken, accessTokenSecret)
    Dim response = twitter.Tweet("This is my first automated tweet!")
    Console.WriteLine(response)

  End Sub

End Module
