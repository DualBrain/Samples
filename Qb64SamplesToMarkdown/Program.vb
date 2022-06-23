Option Explicit On
Option Strict On
Option Infer On

Imports System.IO
Imports System.IO.Path

Module Program

  'Private rootQbjsUrl = "https://v6p9d9t4.ssl.hwcdn.net/html/5963335/index.html?"
  Private m_rootQbjsUrl As String = "https://v6p9d9t4.ssl.hwcdn.net/html/6022890/index.html?"

  Private Class Page

    Public Sub New(pageTitle As String, depth As Integer)
      Content = PageHeader(pageTitle, depth)
      Count = 0
    End Sub

    Private m_doneFirst As Boolean

    Public Property Count As Integer
    Public Property Content As String

    Public Sub AddContent(folder As String,
                          title As String,
                          authors As List(Of String),
                          source As String,
                          urls As List(Of String),
                          version As String,
                          description As String,
                          tags As List(Of String))

      Dim images As New List(Of String)
      Dim imgFolder = Combine(folder, "img")
      For Each file In Directory.GetFiles(imgFolder)
        Dim filename = IO.Path.GetFileName(file)
        images.Add(filename)
      Next

      ' Title (taken care of at creation time.)

      If images.Any Then
        Content &= $"![{images(0)}](img/{images(0)}){vbCrLf}{vbCrLf}"
      End If

      ' Authors

      Dim authorSegment = ""
      If authors Is Nothing Then
        authorSegment &= $"[🐝 *missing*](../author-missing.md) "
      Else
        For Each author In authors
          If Not String.IsNullOrWhiteSpace(author) Then
            authorSegment &= $"[🐝 {author}](../{author.ToLower.Replace(" ", "-")}.md) "
          Else
            authorSegment &= $"[🐝 *missing*](../author-missing.md) "
          End If
        Next
      End If

      If authors?.Any Then
        Content &= $"### Author{If(authors.Count > 1, "s", "")}

{authorSegment}

"
      End If

      ' Description

      Content &= $"### Description

```text
{description}
```

"

      ' Source Files

      Dim srcFolder = Combine(folder, "src")

      If True Then

        ' Possibly provide a link to QBjs...

        Dim files = Directory.GetFiles(srcFolder, "*.*").ToList

        Dim filteredCount = 0
        Dim basFile As String = Nothing
        For Each file In files
          Select Case Path.GetExtension(file).ToLower
            Case ".bas" : filteredCount += 1 : basFile = file
            Case ".zip", ".7z"
              ' exclude from this count...
            Case Else
              filteredCount += 1
          End Select
        Next


        If filteredCount = 1 AndAlso
           basFile IsNot Nothing Then

          Content &= $"### QBjs{vbCrLf}{vbCrLf}"

          Content &= $"> Please note that QBjs is still in early development and support for these examples is extremely experimental (meaning will most likely not work). With that out of the way, give it a try!{vbCrLf}{vbCrLf}"

          Dim filename = IO.Path.GetFileName(basFile)
          Content &= $"* [LOAD ""{filename}""]({m_rootQbjsUrl}src=https://qb64.com/samples/{Path.GetFileName(folder)}/src/{filename}){vbCrLf}"
          Content &= $"* [RUN ""{filename}""]({m_rootQbjsUrl}mode=auto&src=https://qb64.com/samples/{Path.GetFileName(folder)}/src/{filename}){vbCrLf}"
          Content &= $"* [PLAY ""{filename}""]({m_rootQbjsUrl}mode=play&src=https://qb64.com/samples/{Path.GetFileName(folder)}/src/{filename}){vbCrLf}"

          Content &= $"{vbCrLf}"

        End If

      End If

      If False Then

        Content &= $"### Code{vbCrLf}{vbCrLf}"

        For Each file In Directory.GetFiles(srcFolder, "*.bas")
          Dim filename = IO.Path.GetFileName(file)
          Dim code = IO.File.ReadAllText(file).Trim
          Content &= $"#### {filename}{vbCrLf}{vbCrLf}```vb{vbCrLf}{vbCrLf}{code}{vbCrLf}{vbCrLf}```{vbCrLf}{vbCrLf}"
        Next

        For Each file In Directory.GetFiles(srcFolder, "*.bi")
          Dim filename = IO.Path.GetFileName(file)
          Dim code = IO.File.ReadAllText(file).Trim
          Content &= $"#### {filename}{vbCrLf}{vbCrLf}```vb{vbCrLf}{vbCrLf}{code}{vbCrLf}{vbCrLf}```{vbCrLf}{vbCrLf}"
        Next

        For Each file In Directory.GetFiles(srcFolder, "*.bm")
          Dim filename = IO.Path.GetFileName(file)
          Dim code = IO.File.ReadAllText(file).Trim
          Content &= $"#### {filename}{vbCrLf}{vbCrLf}```vb{vbCrLf}{vbCrLf}{code}{vbCrLf}{vbCrLf}```{vbCrLf}{vbCrLf}"
        Next

      End If

      ' Download(s)

      Content &= $"### File(s){vbCrLf}{vbCrLf}"

      For Each file In Directory.GetFiles(srcFolder)
        Dim filename = IO.Path.GetFileName(file)
        Content &= $"* [{filename}](src/{filename}){vbCrLf}"
      Next
      Content &= $"{vbCrLf}"

      ' Additional Screen shots?

      If images.Count > 1 Then

        Content &= $"### Additional Image(s){vbCrLf}{vbCrLf}"

        For index = 1 To images.Count - 1
          Content &= $"![{images(index)}](img/{images(index)}){vbCrLf}"
        Next

        Content &= $"{vbCrLf}"

      End If

      Content &= $"🔗 {TagsToString(tags, 1)}
"

      ' In "fine print"...

      If urls?.Any Then

        Content &= "

<sub>Reference: "

        Dim urlIndex = 1
        For Each url In urls

          Dim skip = False

          Dim lowered = url.ToLower
          Dim location = ""
          If lowered.StartsWith("https://github.com/") Then
            location = "github.com"
          ElseIf lowered.StartsWith("http://www.totaldoscollection.org/") Then
            location = "totaldoscollection.org"
          ElseIf lowered.StartsWith("https://qb64forum.alephc.xyz/") Then
            location = "qb64forum"
          ElseIf lowered.StartsWith("https://www.qb64.org/") Then
            location = "~~qb64.org~~"
          ElseIf lowered.StartsWith("https://djoffe.com/") Then
            location = "djoffe.com"
          ElseIf lowered.StartsWith("https://qb64phoenix.com/") Then
            location = "qb64phoenix.com" : skip = True
          Else
            If String.IsNullOrWhiteSpace(source) Then
              location = $"{urlIndex}"
            Else
              location = source
            End If
          End If
          If Not skip Then
            Content &= $"{If(urlIndex > 1, ", ", "")}[{location}]({url}) "
            urlIndex += 1
          End If
        Next
        Content &= $"</sub>{vbCrLf}"

      End If

    End Sub

    Public Sub AddLine(folder As String,
                       title As String,
                       authors As List(Of String),
                       source As String,
                       urls As List(Of String),
                       version As String,
                       description As String,
                       tags As List(Of String))

      If m_doneFirst Then
        Content &= "
"
      Else
        m_doneFirst = True
      End If

      Dim summary = description.Replace(vbCr, "").Replace(vbLf, " ")
      If summary.Length > 100 Then summary = summary.Substring(0, 97) & "..."

      Dim authorSegment = ""
      For Each author In authors
        If Not String.IsNullOrWhiteSpace(author) Then
          authorSegment &= $"[🐝 {author}]({author.ToLower.Replace(" ", "-")}.md) "
        Else
          authorSegment &= $"[🐝 *missing*](author-missing.md) "
        End If
      Next

      Content &= $"**[{title}]({Path.GetFileNameWithoutExtension(folder).ToLower.Replace(" ", "-")}/index.md)**{If(Not String.IsNullOrEmpty(version), $" <sup>v{version}</sup>", "")}

{authorSegment}🔗 {TagsToString(tags, 0)}

{summary}
"
      Count += 1

    End Sub

    Private Shared Function TagsToString(tags As List(Of String), depth As Integer) As String

      Dim dots = ""
      Select Case depth
        Case 0 : dots = ""
        Case 1 : dots = "../"
        Case Else
          Stop
      End Select

      Dim result = ""
      For Each tag In tags
        result &= $"{If(result <> "", ", ", "")}[{tag.Trim}]({dots}{tag.Trim.Replace(" ", "-")}.md)"
      Next
      Return result
    End Function

  End Class

  Sub Main() 'args As String())

    Dim globalAuthorNames As New Dictionary(Of String, String)
    Dim globalAuthors As New Dictionary(Of String, Page)
    Dim globalTags As New Dictionary(Of String, Page)

    Dim path = "D:\github\qb64.samples\Samples"
    Dim outputPath = "D:\test"

    Dim indexPage = PageHeader("SAMPLES", 0)
    '| Samples                                                                |                                |
    '| ---------------------------------------------------------------------- | ------------------------------:|
    '"

    Dim qbjsList = {"American Flag",
                    "Tile Demo",
                    "Plumeria",
                    "Chaotic Scattering",
                    "Turtle Graphics"}

    Dim exclusionList = {"Flappy Bird"}

    For Each folder In Directory.GetDirectories(path)

      'Dim titleFilespec = Combine(folder, "title.txt")
      Dim authorFilespec = Combine(folder, "author.txt")
      Dim descriptionFilespec = Combine(folder, "description.txt")
      Dim tagsFilespec = Combine(folder, "tags.txt")
      Dim urlFilespec = Combine(folder, "url.txt")

      Dim imgFolder = Combine(folder, "img")
      Dim srcFolder = Combine(folder, "src")

      If Not (Directory.Exists(imgFolder) AndAlso
              Directory.Exists(srcFolder)) Then Continue For

      Dim folderName = GetFileNameWithoutExtension(folder)

      If exclusionList.Contains(folderName) Then Continue For

      Dim tags = SplitCommaValues(ReadData(tagsFilespec))

      If qbjsList.Contains(folderName) AndAlso
         Not tags.Contains("qbjs") Then
        tags.Add("qbjs")
      End If

      Dim title = folderName 'ReadData(titleFilespec)
      Dim authors = SplitCommaValues(ReadData(authorFilespec))
      Dim description = ReadData(descriptionFilespec)
      Dim source As String = Nothing
      Dim urls = ReadLines(urlFilespec)
      Dim version As String = Nothing

      'For Each line In meta

      '  If line.StartsWith("Title:") Then
      '    title = line.Substring(line.IndexOf(":") + 1).Trim
      '  ElseIf line.StartsWith("Author:") Then
      '    Dim value = line.Substring(line.IndexOf(":") + 1).Trim
      '    If value.Contains(","c) Then
      '      Dim entries = Split(value, ","c)
      '      For index = 0 To entries.Length - 1
      '        entries(index) = entries(index).Trim
      '      Next
      '      authors = entries.ToList
      '    Else
      '      authors = New List(Of String) From {value}
      '    End If
      '  ElseIf line.StartsWith("Source:") Then
      '    source = line.Substring(line.IndexOf(":") + 1).Trim
      '  ElseIf line.StartsWith("URL:") Then
      '    Dim value = line.Substring(line.IndexOf(":") + 1).Trim
      '    If Not String.IsNullOrWhiteSpace(value) Then
      '      urls = New List(Of String) From {value}
      '    End If
      '  ElseIf line.StartsWith("https://") Then
      '    Dim value = line
      '    If urls IsNot Nothing Then
      '      urls.Add(value)
      '    Else
      '      urls = New List(Of String) From {value}
      '    End If
      '  ElseIf line.StartsWith("Version:") Then
      '    Dim value = line.Substring(line.IndexOf(":") + 1).Trim
      '    If value <> "QB64" Then
      '      version = value
      '    End If
      '  Else
      '    Stop
      '  End If

      'Next

      ' Copy all images and source files to target...

      Dim targetPath = Combine(outputPath, "samples", folderName.ToLower.Replace(" ", "-"))
      If Not IO.Directory.Exists(targetPath) Then
        Directory.CreateDirectory(targetPath)
      End If
      Dim targetImgFolder = Combine(targetPath, "img")
      If Not IO.Directory.Exists(targetImgFolder) Then
        Directory.CreateDirectory(targetImgFolder)
      End If
      Dim targetSrcFolder = Combine(targetPath, "src")
      If Not IO.Directory.Exists(targetSrcFolder) Then
        Directory.CreateDirectory(targetSrcFolder)
      End If
      For Each file In Directory.GetFiles(imgFolder)
        FileCopy(file, Combine(targetImgFolder, IO.Path.GetFileName(file).ToLower.Replace(" ", "-")))
      Next
      For Each file In Directory.GetFiles(srcFolder)
        FileCopy(file, Combine(targetSrcFolder, IO.Path.GetFileName(file).ToLower.Replace(" ", "-")))
      Next

      Dim entryPage = New Page($"SAMPLE: {title}", 2)
      entryPage.AddContent(targetPath, title, authors, source, urls, version, description, tags)

      Dim entryContent = entryPage.Content
      File.WriteAllText(Combine(targetPath, "index.md"), entryContent)

      If authors Is Nothing Then
        authors = New List(Of String) From {""}
      End If

      For Each author In authors
        Dim key = author.ToLower
        If key.Contains("\"c) Then key = key.Replace("\"c, "-")
        If key.Contains("/"c) Then key = key.Replace("/"c, "-")
        If Not globalAuthors.ContainsKey(key) Then
          Dim temp = If(String.IsNullOrWhiteSpace(author), "*missing*", author)
          Dim pageName = $"SAMPLES BY {temp}"
          globalAuthors.Add(key, New Page(pageName, 1))
          globalAuthorNames.Add(key, author)
        Else
          globalAuthors(key).Count += 1
        End If
        globalAuthors(key).AddLine(folder,
                                   title,
                                   authors,
                                   source,
                                   urls,
                                   version,
                                   description,
                                   tags)
      Next

      For index = 0 To tags.Count - 1
        tags(index) = tags(index).Trim
        If Not globalTags.ContainsKey(tags(index)) Then
          Dim temp = If(String.IsNullOrWhiteSpace(tags(index)), "*missing*", tags(index))
          Dim pageName = $"SAMPLES: {temp}"
          globalTags.Add(tags(index), New Page(pageName, 1))
        End If
        globalTags(tags(index)).AddLine(folder,
                                            title,
                                            authors,
                                            source,
                                            urls,
                                            version,
                                            description,
                                            tags)
      Next

      Dim authorSegment = ""
      For Each author In authors
        'If String.IsNullOrWhiteSpace(author) Then
        '  authorSegment &= $"🐝 [*missing*](samples/author-missing.md) "
        'Else
        '  authorSegment &= $"🐝 [{author}](samples/{author.ToLower.Replace(" ", "-")}.md) "
        'End If
        If String.IsNullOrWhiteSpace(author) Then
          authorSegment &= $" • [*missing*](samples/author-missing.md) "
        Else
          authorSegment &= $" • [{author}](samples/{author.ToLower.Replace(" ", "-")}.md) "
        End If
      Next

      'Dim tagsSegment = $"🔗 {TagsToString(tags)}"
      Dim tagsSegment = TagsToString(tags)

      indexPage &= $"- **[{title}](samples/{folderName.ToLower.Replace(" ", "-")}/index.md)**{authorSegment}<span style=""float: right;"">{tagsSegment}</span>{vbCrLf}"

      'Dim firstColumn = $""
      'Dim padding1 = 90 - firstColumn.Length
      'Dim secondColumn = $"{TagsToString(tags)}"
      'Dim padding2 = 30 - secondColumn.Length

      'indexPage &= $"| {firstColumn}{Space(padding1)} | {Space(padding2)}{secondColumn} |{vbCrLf}"

    Next

    ' *****************************************
    ' * samples.md
    ' *****************************************

    Dim content = indexPage
    File.WriteAllText(Combine(outputPath, "samples.md"), content)

    ' *****************************************
    ' * samples/tag_cloud.md
    ' *****************************************

    Dim tagCloud = ""
    Dim sortedTags = (From entry In globalTags Order By entry.Value.Count Descending, entry.Key Ascending Select entry)
    For Each tag In sortedTags
      tagCloud &= $"{If(tagCloud = "", "", " • ")}[{tag.Key}:{tag.Value.Count}]({tag.Key.ToLower.Replace(" ", "-")}.md)"
    Next

    content = PageHeader("TAGS", 1) & tagCloud
    File.WriteAllText(Combine(outputPath, "samples", "tag-cloud.md"), content)

    ' *****************************************
    ' * samples/author_cloud.md
    ' *****************************************

    Dim authorCloud = ""
    Dim sortedAuthors = (From entry In globalAuthors Order By entry.Value.Count Descending, entry.Key Ascending Select entry)
    For Each author In sortedAuthors
      Dim key = author.Key
      Dim name = globalAuthorNames(key)
      If Not String.IsNullOrWhiteSpace(name) Then
        authorCloud &= $"{If(authorCloud = "", "", " • ")}[{name}:{author.Value.Count}]({author.Key.ToLower.Replace(" ", "-")}.md)"
      Else
        authorCloud &= $"{If(authorCloud = "", "", " • ")}[*missing*:{author.Value.Count}](author-missing.md)"
      End If
    Next

    content = PageHeader("AUTHORS", 1) & authorCloud
    File.WriteAllText(Combine(outputPath, "samples", "author-cloud.md"), content)

    ' *****************************************
    ' * samples/*author_page*.md
    ' *****************************************

    For Each entry In globalAuthors
      Dim page = entry.Value
      Dim pageName = entry.Key
      If String.IsNullOrWhiteSpace(pageName) Then pageName = "author-missing"
      File.WriteAllText(Combine(outputPath, "samples", $"{pageName.ToLower.Replace(" ", "-")}.md"), page.Content)
    Next

    ' *****************************************
    ' * samples/*tag_page*.md
    ' *****************************************

    For Each entry In globalTags
      Dim filename = entry.Key
      Dim page = entry.Value
      File.WriteAllText(Combine(outputPath, "samples", $"{filename.Replace(" ", "-")}.md"), page.Content)
    Next

  End Sub

  Private Function SplitCommaValues(text As String) As List(Of String)
    If String.IsNullOrWhiteSpace(text) Then Return Nothing
    If text.Contains(","c) Then
      Dim values = text.Split(","c)
      For index = 0 To values.Length - 1
        values(index) = values(index).Trim
      Next
      Return values.ToList
    Else
      Return New List(Of String) From {text}
    End If
  End Function

  Private Function ReadLines(filespec As String) As List(Of String)
    If IO.File.Exists(filespec) Then
      Dim result = IO.File.ReadLines(filespec).ToList
      For index = result.Count - 1 To 0 Step -1
        result(index) = result(index).Trim
        If String.IsNullOrWhiteSpace(result(index)) Then
          result.RemoveAt(index)
        End If
      Next
      Return result
    Else
      Return Nothing
    End If
  End Function

  Private Function ReadData(filespec As String) As String
    Dim result = If(File.Exists(filespec), File.ReadAllText(filespec), Nothing)
    Return result?.Trim
  End Function

  Private Function TagsToString(tags As List(Of String)) As String
    Dim result = ""
    For Each tag In tags
      result &= If(result <> "", ", ", "") & $"[{tag.Trim}](samples/{tag.Trim.Replace(" ", "-")}.md)"
    Next
    Return $"{result}"
  End Function

  Private Function PageHeader(pageName As String, depth As Integer) As String

    Dim dots = ""

    Select Case depth
      Case 0 : dots = ""
      Case 1 : dots = "../"
      Case 2 : dots = "../../"
      Case Else
        Stop
    End Select

    Dim navigation = $"[Home](https://qb64.com) • [News]({dots}news.md) • [GitHub](https://github.com/QB64Official/qb64) • [Wiki](https://github.com/QB64Official/qb64/wiki) • [Samples]({dots}samples.md) • [InForm]({dots}inform.md) • [GX]({dots}gx.md) • [QBjs]({dots}qbjs.md) • [Community]({dots}community.md) • [More...]({dots}more.md)"
    'Dim navigation = $"[Home](https://qb64.com) • [News]({dots}news.md) • [GitHub]({dots}github.md) • [Wiki]({dots}wiki.md) • [Samples]({dots}samples.md) • [Media]({dots}media.md) • [Community]({dots}community.md) • [Rolodex]({dots}rolodex.md) • [More...]({dots}more.md)"

    If Not String.IsNullOrWhiteSpace(pageName) Then
      Return $"{navigation}

## {pageName.ToUpper}

"
    Else
      Return $"{navigation}

"
    End If

  End Function

End Module
