Option Explicit On
Option Strict On
Option Infer On

Imports System.IO

' https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher?view=netcore-3.1

Module Program

  Public Sub Main(args() As String)

    ' If a directory is not specified, exit the program.
    If args.Length <> 1 Then
      ' Display the proper way to call the program.
      Console.WriteLine("Usage: Watcher.exe (directory)")
      Return
    End If

    ' Create a new FileSystemWatcher and set its properties.
    Using watcher As New FileSystemWatcher()

      watcher.Path = args(0)

      ' Watch for changes in LastAccess and LastWrite times, and
      ' the renaming of files or directories. 
      watcher.NotifyFilter = (NotifyFilters.LastAccess Or
                              NotifyFilters.LastWrite Or
                              NotifyFilters.FileName Or
                              NotifyFilters.DirectoryName)

      ' Only watch text files.
      watcher.Filter = "*.txt"

      ' Add event handlers.
      AddHandler watcher.Changed, AddressOf OnChanged
      AddHandler watcher.Created, AddressOf OnChanged
      AddHandler watcher.Deleted, AddressOf OnChanged
      AddHandler watcher.Renamed, AddressOf OnRenamed

      ' Begin watching.
      watcher.EnableRaisingEvents = True

      ' Wait for the user to quit the program.
      Console.WriteLine("Press 'q' to quit the sample.")
      Do
        If Console.KeyAvailable Then
          Select Case Console.ReadKey(True).Key
            Case ConsoleKey.Q
              Exit Do
            Case Else
          End Select
        End If
      Loop

    End Using

  End Sub

  ' Define the event handlers.
  Private Sub OnChanged(source As Object, e As FileSystemEventArgs)
    ' Specify what is done when a file is changed, created, or deleted.
    Console.WriteLine($"File: {e.FullPath} {e.ChangeType}")
    Select Case e.ChangeType
      Case WatcherChangeTypes.Changed
        ' What to do when the file is updated...
      Case WatcherChangeTypes.Created
        ' What to do when the file is created...
      Case WatcherChangeTypes.Deleted
        ' What to do when the file is deleted...
      Case Else
    End Select
  End Sub

  Private Sub OnRenamed(source As Object, e As RenamedEventArgs)
    ' Specify what is done when a file is renamed.
    Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}")
  End Sub

End Module