Imports System

Module Program

  Sub Main() 'args As String())

    'Console.WriteLine("Hello World!")

    Dim consumerKey As String = Nothing
    Dim consumerKeySecret As String = Nothing
    Dim accessToken As String = Nothing
    Dim accessTokenSecret As String = Nothing

    Dim twitter = New TwitterApi(consumerKey, consumerKeySecret, accessToken, accessTokenSecret)
    Dim response = twitter.Tweet("This is my first automated tweet!")
    Console.WriteLine(response)

  End Sub

End Module
