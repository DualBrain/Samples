Module Program

  Private m_source As String = "e:\music_prev"
  Private m_target As String = "d:\music"

  Sub Main() 'args As String())

    CompareFiles(m_source, m_target)

  End Sub

  Private Sub CompareFiles(source As String, target As String)

    If Not IO.Directory.Exists(source) Then
      Console.Write("TARGET: ")
      Console.WriteLine(source) : Return
    End If
    If Not IO.Directory.Exists(target) Then
      Console.Write("SOURCE: ")
      Console.WriteLine(target) : Return
    End If

    Dim sourceFiles = Filter(IO.Directory.GetFiles(source).ToList)
    Dim targetFiles = Filter(IO.Directory.GetFiles(target).ToList)

    Dim sourceNames As New List(Of String)
    Dim targetNames As New List(Of String)
    If sourceFiles.Any Then
      For Each file In sourceFiles
        sourceNames.Add(IO.Path.GetFileName(file))
      Next
    End If
    If targetFiles.Any Then
      For Each file In sourceFiles
        Dim name = IO.Path.GetFileName(file)
        targetNames.Add(IO.Path.GetFileName(file))
      Next
    End If

    Dim handled As New List(Of String)

    For Each file In sourceFiles
      Dim name = IO.Path.GetFileName(file)
      If targetNames.Contains(name) Then
        ' found... possible match
      Else
        ' not found...
        If file.StartsWith(m_source, StringComparison.InvariantCultureIgnoreCase) Then
          Console.Write("SOURCE: ")
        Else
          Console.Write("TARGET: ")
        End If
      End If
      handled.Add(name)
    Next

    'For Each file In targetFiles
    '  Dim name = IO.Path.GetFileName(file)
    '  If Not handled.Contains(name) Then
    '    If sourceNames.Contains(name) Then
    '      ' found... possible match
    '    Else
    '      ' not found...
    '      If file.StartsWith(m_source, StringComparison.InvariantCultureIgnoreCase) Then
    '        Console.Write("SOURCE: ")
    '      Else
    '        Console.Write("TARGET: ")
    '      End If
    '      Console.WriteLine(file)
    '    End If
    '    handled.Add(name)
    '  End If
    'Next

    ' Folders

    Dim directories As New List(Of String)

    If IO.Directory.Exists(source) Then
      For Each folder In IO.Directory.GetDirectories(source)
        Dim dir = folder.Substring(m_source.Length + 1)
        CompareFiles(IO.Path.Combine(m_source, dir), IO.Path.Combine(m_target, dir))
        directories.Add(dir)
      Next
    End If

    'If IO.Directory.Exists(target) Then
    '  For Each folder In IO.Directory.GetDirectories(target)
    '    Dim dir = folder.Substring(m_target.Length + 1)
    '    If directories.Contains(dir) Then
    '      CompareFiles(IO.Path.Combine(m_source, dir), IO.Path.Combine(m_target, dir))
    '      directories.Add(dir)
    '    End If
    '  Next
    'End If

  End Sub

  Private Function Filter(entries As List(Of String)) As List(Of String)
    Dim result = New List(Of String)
    For Each file In entries
      Dim name = IO.Path.GetFileName(file)
      If name.StartsWith("AlbumArt_", StringComparison.InvariantCultureIgnoreCase) Then Continue For
      If String.Equals(name, "albumartsmall.jpg", StringComparison.InvariantCultureIgnoreCase) Then Continue For
      If String.Equals(name, "folder.jpg", StringComparison.InvariantCultureIgnoreCase) Then Continue For
      If String.Equals(name, "desktop.ini", StringComparison.InvariantCultureIgnoreCase) Then Continue For
      If String.Equals(name, "thumbs.db", StringComparison.InvariantCultureIgnoreCase) Then Continue For
      result.Add(file)
    Next
    Return result
  End Function

End Module
