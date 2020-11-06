Option Explicit On
Option Infer On
Option Strict On

Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text

''' <summary>
''' Simple class for sending tweets to Twitter using Single-user OAuth.
''' https://dev.twitter.com/oauth/overview/single-user
''' 
''' Get your access keys by creating an app at apps.twitter.com then visiting the
''' "Keys and Access Tokens" section for your app. They can be found under the
''' "Your Access Token" heading.
''' </summary>
Class TwitterApi
  Const TwitterApiBaseUrl As String = "https://api.twitter.com/1.1/"
  ReadOnly consumerKey, consumerKeySecret, accessToken, accessTokenSecret As String
  ReadOnly sigHasher As HMACSHA1
  ReadOnly epochUtc As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

  ''' <summary>
  ''' Creates an object for sending tweets to Twitter using Single-user OAuth.
  ''' 
  ''' Get your access keys by creating an app at apps.twitter.com then visiting the
  ''' "Keys and Access Tokens" section for your app. They can be found under the
  ''' "Your Access Token" heading.
  ''' </summary>
  Public Sub New(consumerKey As String, consumerKeySecret As String, accessToken As String, accessTokenSecret As String)
    Me.consumerKey = consumerKey
    Me.consumerKeySecret = consumerKeySecret
    Me.accessToken = accessToken
    Me.accessTokenSecret = accessTokenSecret
    Dim tempVar As New ASCIIEncoding
    sigHasher = New HMACSHA1(tempVar.GetBytes(String.Format("{0}&{1}", consumerKeySecret, accessTokenSecret)))
  End Sub

  ''' <summary>
  ''' Sends a tweet with the supplied text and returns the response from the Twitter API.
  ''' </summary>
  Public Async Function TweetAsync(text As String) As Task(Of String)
    Dim data = New Dictionary(Of String, String) From {
        {"status", text},
        {"trim_user", "1"}}
    Return Await SendRequestAsync("statuses/update.json", data)
  End Function

  Public Function Tweet(text As String) As String
    Dim data = New Dictionary(Of String, String) From {
        {"status", text},
        {"trim_user", "1"}}
    Return SendRequest("statuses/update.json", data)
  End Function

  Private Function SendRequest(url As String, data As Dictionary(Of String, String)) As String
    Dim fullUrl As String = TwitterApiBaseUrl & url
    ' TODO: Check, VB does not directly support MemberAccess off a Conditional If Expression
    Dim tempVar1 = DateTime.UtcNow - epochUtc
    Dim timestamp As Integer = CInt(Fix((tempVar1.TotalSeconds)))

    ' Add all the OAuth headers we'll need to use when constructing the hash.
    data.Add("oauth_consumer_key", consumerKey)
    data.Add("oauth_signature_method", "HMAC-SHA1")
    data.Add("oauth_timestamp", timestamp.ToString())
    data.Add("oauth_nonce", "a")
    ' Required, but Twitter doesn't appear to use it, so "a" will do.
    data.Add("oauth_token", accessToken)
    data.Add("oauth_version", "1.0")

    ' Generate the OAuth signature and add it to our payload.
    data.Add("oauth_signature", GenerateSignature(fullUrl, data))

    ' Build the OAuth HTTP Header from the data.
    Dim oAuthHeader As String = GenerateOAuthHeader(data)

    ' Build the form data (exclude OAuth stuff that's already in the header).
    Dim formData = New FormUrlEncodedContent(data.Where(Function(kvp) Not kvp.Key.StartsWith("oauth_")))

    Return SendRequest(fullUrl, oAuthHeader, formData)
  End Function

  Private Async Function SendRequestAsync(url As String, data As Dictionary(Of String, String)) As Task(Of String)
    Dim fullUrl As String = TwitterApiBaseUrl & url
    ' TODO: Check, VB does not directly support MemberAccess off a Conditional If Expression
    Dim tempVar1 = DateTime.UtcNow - epochUtc
    Dim timestamp As Integer = CInt(Fix((tempVar1.TotalSeconds)))

    ' Add all the OAuth headers we'll need to use when constructing the hash.
    data.Add("oauth_consumer_key", consumerKey)
    data.Add("oauth_signature_method", "HMAC-SHA1")
    data.Add("oauth_timestamp", timestamp.ToString())
    data.Add("oauth_nonce", "a")
    ' Required, but Twitter doesn't appear to use it, so "a" will do.
    data.Add("oauth_token", accessToken)
    data.Add("oauth_version", "1.0")

    ' Generate the OAuth signature and add it to our payload.
    data.Add("oauth_signature", GenerateSignature(fullUrl, data))

    ' Build the OAuth HTTP Header from the data.
    Dim oAuthHeader As String = GenerateOAuthHeader(data)

    ' Build the form data (exclude OAuth stuff that's already in the header).
    Dim formData = New FormUrlEncodedContent(data.Where(Function(kvp) Not kvp.Key.StartsWith("oauth_")))

    Return SendRequest(fullUrl, oAuthHeader, formData)
  End Function

  ''' <summary>
  ''' Generate an OAuth signature from OAuth header values.
  ''' </summary>
  Private Function GenerateSignature(url As String, data As Dictionary(Of String, String)) As String
    Dim sigString As String = String.Join(
        "&",
        data.
Union(data).
[Select](Function(kvp) String.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value))).
OrderBy(Function(s) s)
    )

    Dim fullSigData As String = String.Format(
        "{0}&{1}&{2}",
        "POST",
        Uri.EscapeDataString(url),
        Uri.EscapeDataString(sigString.ToString())
    )
    Dim tempVar2 As New ASCIIEncoding
    Return Convert.ToBase64String(sigHasher.ComputeHash(tempVar2.GetBytes(fullSigData.ToString())))
  End Function

  ''' <summary>
  ''' Generate the raw OAuth HTML header from the values (including signature).
  ''' </summary>
  Private Function GenerateOAuthHeader(data As Dictionary(Of String, String)) As String
    Return "OAuth " & String.Join(
        ", ",
        data.
Where(Function(kvp) kvp.Key.StartsWith("oauth_")).
[Select](Function(kvp) String.Format("{0}=""{1}""", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value))).
OrderBy(Function(s) s)
    )

  End Function

  ''' <summary>
  ''' Send HTTP Request and return the response.
  ''' </summary>
  Private Function SendRequest(fullUrl As String, oAuthHeader As String, formData As FormUrlEncodedContent) As String
    Using http = New HttpClient
      http.DefaultRequestHeaders.Add("Authorization", oAuthHeader)

      Dim httpResp = http.PostAsync(fullUrl, formData)
      Dim respBody = httpResp.Result.Content.ReadAsStringAsync()

      Return respBody.Result
    End Using
  End Function

  Private Async Function SendRequestAsync(fullUrl As String, oAuthHeader As String, formData As FormUrlEncodedContent) As Task(Of String)
    Using http = New HttpClient
      http.DefaultRequestHeaders.Add("Authorization", oAuthHeader)

      Dim httpResp = Await http.PostAsync(fullUrl, formData)
      Dim respBody = Await httpResp.Content.ReadAsStringAsync()

      Return respBody
    End Using
  End Function

End Class