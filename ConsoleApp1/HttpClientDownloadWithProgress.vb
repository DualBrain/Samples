Option Strict On
Option Explicit On
Option Infer On

Imports System.IO
Imports System.Net.Http

' https://newbedev.com/progress-bar-with-httpclient#:~:text=You%20can%20write%20an%20extension%20method%20for%20downloading,data%20using%20%28var%20client%20%3D%20new%20HttpClient%28%29%29%20%7B

Public NotInheritable Class HttpClientDownloadWithProgress
  Implements IDisposable

  Private ReadOnly m_downloadUrl As String
  Private ReadOnly m_destinationFilePath As String
  Private m_httpClient As HttpClient

  Public Delegate Sub ProgressChangedHandler(totalFileSize As Long?, totalBytesDownloaded As Long, progressPercentage As Double?)

  Public Event ProgressChanged As ProgressChangedHandler

  Public Sub New(downloadUrl As String, destinationFilePath As String)
    m_downloadUrl = downloadUrl
    m_destinationFilePath = destinationFilePath
  End Sub

  Public Async Function StartDownload() As Task
    m_httpClient = New HttpClient With {.Timeout = TimeSpan.FromDays(1)}
    Using response = Await m_httpClient.GetAsync(m_downloadUrl, HttpCompletionOption.ResponseHeadersRead)
      Await DownloadFileFromHttpResponseMessage(response)
    End Using
  End Function

  Private Async Function DownloadFileFromHttpResponseMessage(response As HttpResponseMessage) As Task
    response.EnsureSuccessStatusCode()
    Dim totalBytes = response.Content.Headers.ContentLength
    Using contentStream = Await response.Content.ReadAsStreamAsync()
      Await ProcessContentStream(totalBytes, contentStream)
    End Using
  End Function

  Private Async Function ProcessContentStream(totalDownloadSize As Long?, contentStream As Stream) As Task
    Dim totalBytesRead = 0L
    Dim readCount = 0L
    Dim buffer = New Byte(8191) {}
    Dim isMoreToRead = True
    Using fileStream1 = New FileStream(m_destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, True)
      Do
        Dim bytesRead = Await contentStream.ReadAsync(buffer)
        If bytesRead = 0 Then
          isMoreToRead = False
          TriggerProgressChanged(totalDownloadSize, totalBytesRead)
          Continue Do
        End If
        Await fileStream1.WriteAsync(buffer.AsMemory(0, bytesRead))
        totalBytesRead += bytesRead
        readCount += 1
        If readCount Mod 100 = 0 Then
          TriggerProgressChanged(totalDownloadSize, totalBytesRead)
        End If
      Loop While isMoreToRead
    End Using
  End Function

  Private Sub TriggerProgressChanged(totalDownloadSize As Long?, totalBytesRead As Long)
    Dim progressPercentage = New Double?
    If totalDownloadSize.HasValue Then
      progressPercentage = Math.Round(CDbl(totalBytesRead) / totalDownloadSize.Value * 100, 2)
    End If
    RaiseEvent ProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage)
  End Sub

  Public Sub Dispose() Implements IDisposable.Dispose
    m_httpClient?.Dispose()
  End Sub

End Class