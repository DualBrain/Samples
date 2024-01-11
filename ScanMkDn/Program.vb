Imports System
Imports System.Reflection.Metadata

Module Program

  Private m_list As New List(Of String)
  Private m_lookup As New Dictionary(Of String, String)

  Sub Main(args As String())

    Dim root = "c:\jekyll\addressof"

    m_lookup.Add("/blog/archive/2005/11/01/8879.aspx", "/posts/vb-2005-snippet-annoyance-a-solution/")
    m_lookup.Add("/blog/archive/2006/02/16/9876.aspx", "/posts/diamonddash-boulderdash-clone-in-vb-2005-revised-yet-again/")
    m_lookup.Add("/blog/archive/2005/05/06/1889.aspx", "/posts/bringing-a-20-year-old-draw-command-back-to-life/")
    m_lookup.Add("/blog/archive/2005/04/07/1674.aspx", "/posts/checkers-solitaire/")
    m_lookup.Add("/blog/articles/checkerssolitaire.aspx", "/posts/checkers-solitaire-1/")
    m_lookup.Add("/blog/archive/2005/03/16/1471.aspx", "/posts/solitaire-written-in-vb-net/")
    m_lookup.Add("/blog/archive/2003/10/30/233.aspx", "/posts/windows-forms-disable-the-close-button-1/")
    m_lookup.Add("/blog/articles/232.aspx", "/posts/windows-forms-disable-the-close-button/")
    m_lookup.Add("/blog/archive/2004/02/07/369.aspx", "/posts/ms-research-treemap-net/")
    m_lookup.Add("/blog/archive/2003/11/23/287.aspx", "/posts/new-article-series-walkthrough-developing-owner-drawn-user-controls-part-1-transpanel/")
    m_lookup.Add("/blog/articles/282.aspx", "/posts/walkthrough-developing-owner-drawn-user-controls-part-1-transpanel/")
    m_lookup.Add("/blog/archive/2003/11/25/296.aspx", "/posts/translabel-and-transpanel-s-revenge-developing-owner-drawn-user-controls-part-2/")
    m_lookup.Add("/blog/articles/293.aspx", "/posts/walkthrough-developing-owner-drawn-user-controls-part-2-translabel-and-transpanel-s-revenge/")
    m_lookup.Add("/blog/archive/2004/10/01/955.aspx", "/posts/treeview-listview-drag-and-drop-and-command-pattern-example/")
    m_lookup.Add("/blog/archive/2004/02/07/368.aspx", "/posts/coding-4-fun-drum-machine-in-vb-net/")
    m_lookup.Add("/blog/archive/2004/02/13/385.aspx", "/posts/creating-a-reusable-error-dialog-for-net-applications-vb-net/")
    m_lookup.Add("/blog/archive/2004/02/14/392.aspx", "/posts/revenge-of-driveinfo-drive-serial-numbers/")
    m_lookup.Add("/blog/archive/2005/10/06/8486.aspx", "/posts/gathering-computer-information-using-wmi/")
    m_lookup.Add("/blog/archive/2004/02/13/386.aspx", "/posts/determine-hard-drive-serial-numbers/")
    m_lookup.Add("/blog/archive/2004/06/18/772.aspx", "/posts/codesmith-strongly-typed-arraylist-collection-for-vb-net/")
    m_lookup.Add("/blog/archive/2004/02/15/400.aspx", "/posts/how-to-check-to-see-if-visual-styles-are-enabled/")
    m_lookup.Add("/blog/archive/2004/10/25/1019.aspx", "/posts/converting-between-string-and-char-arrays/")
    'm_lookup.Add("/blog/archive/2004/10/25/1019.aspx#1022", "")
    m_lookup.Add("/blog/archive/2004/06/18/774.aspx", "/posts/my-name-is-cory-smith-i-make-mistakes-and-am-extremely-proud-of-it/")
    m_lookup.Add("/blog/archive/2003/11/19/281.aspx", "/posts/net-application-memory-flush-via-interop/")
    m_lookup.Add("/blog/archive/2003/05/21/189.aspx", "/posts/windows-application-verifier-rah-roh-net-doesn-t-qualify/")
    m_lookup.Add("/blog/articles/CodingGuidelines.aspx", "/posts/vb-net-coding-guidelines/")
    m_lookup.Add("/blog/archive/2005/03/17/1473.aspx", "/posts/vb-net-coding-guidelines-1/")
    m_lookup.Add("/blog/articles/guidelinecomments.aspx", "/posts/vb-net-coding-guidelines-comments/")
    'm_lookup.Add("/blog/archive/2005/03/29/1612.aspx", "")
    m_lookup.Add("/blog/archive/2005/04/15/1761.aspx", "/posts/i-guess-i-just-don-t-get-it/")
    m_lookup.Add("/blog/archive/2005/03/30/1627.aspx", "/posts/split-ing-strings-and-the-performance-implications/")
    m_lookup.Add("/blog/articles/Form2Form.aspx", "/posts/windows-forms-5-ways-of-interaction-between-forms/")
    m_lookup.Add("/blog/articles/samples.aspx", "/articles/samples.html")
    m_lookup.Add("/blog/archive/0001/01/01/1454.aspx", "/posts/coming-soon-coding-guidelines-for-vb-net/")
    m_lookup.Add("/blog/archive/0001/01/01/1473.aspx", "/posts/vb-net-coding-guidelines-1/")
    m_lookup.Add("/blog/archive/2005/07/20/6894.aspx", "/posts/geek-gathering-dallas-fort-worth-metroplex-4/")
    'm_lookup.Add("/blog/archive/2005/03/10/1430.aspx#8407", "")
    m_lookup.Add("/blog/archive/2005/03/10/1430.aspx", "/posts/vb-net-vs-quot-vb-classic-quot-or-quot-vb-com-quot/")
    'm_lookup.Add("/blog/articles/where.aspx", "/posts/where-i-ll-be-where-i-ve-been/")
    m_lookup.Add("/blog/contact.aspx", "/about/")
    m_lookup.Add("/blog/category/24.aspx", "/tags/geekgathering/")
    m_lookup.Add("/blog/archive/2005/03/25/1577.aspx", "/posts/what-s-wrong-with-this-picture/")
    m_lookup.Add("/blog/archive/2006/03/22/10375.aspx", "/posts/i-know-it-s-free-but-still/")
    m_lookup.Add("/blog/articles/ToolTipVoodoo.aspx", "/articles/tooltip-voodoo.html")
    m_lookup.Add("/blog/archive/2006/08/17/12654.aspx", "/posts/hmmm-open-source-recognizes-vb-but-xbox-360-doesn-t/")
    m_lookup.Add("/blog/archive/2006/06/23/11848.aspx", "/posts/not-feelin-the-vb-love-from-microsoft/")
    m_lookup.Add("/blog/archive/2006/11/04/Twin-Cities-Code-Camp.aspx", "/posts/twin-cities-code-camp/")
    m_lookup.Add("/blog/archive/2006/11/07/XboxFriends-_2B00_-Community-Server.aspx", "/posts/xboxfriends-community-server/")
    m_lookup.Add("/blog/archive/2006/11/09/What-would-you-do-for-a-Visual-Studio-2005-_2800_Team-System_2900_-and-MSDN-license_3F00_.aspx", "/posts/what-would-you-do-for-a-visual-studio-2005-team-system-suite-and-msdn-license/")
    m_lookup.Add("/blog/pages/Exploring-WinSAT.aspx", "/posts/leveraging-windows-vista-s-windows-system-assessment-tool-winsat-api-in-visual-basic/")
    m_lookup.Add("/blog/archive/2004/03/05/474.aspx", "/posts/using-com-releasecomobject-is-your-friend/")
    m_lookup.Add("/blog/pages/ConsoleEventHandling.aspx", "/posts/console-event-handling/")
    m_lookup.Add("/blog/archive/2011/05/30/VB10-vs-VB6_3A00_-VarPtr_2C00_-StrPtr_2C00_-ObjPtr.aspx", "/posts/vb10-vs-vb6-varptr-strptr-objptr/")
    m_lookup.Add("/blog/archive/2011/05/31/VB10-vs-VB6_3A00_-As-Any.aspx", "/posts/vb10-vs-vb6-as-any/")
    m_lookup.Add("/blog/archive/2011/06/02/VB10-vs-VB6_3A00_-ByVal_2F00_ByRef-within-API-calls.-.aspx", "/posts/vb10-vs-vb6-byval-byref-within-api-calls/")
    m_lookup.Add("/blog/archive/2011/06/04/VB10-vs-VB6_3A00_-Private-Class-Variables-.aspx", "/posts/vb10-vs-vb6-private-class-variables/")
    m_lookup.Add("/blog/archive/2011/06/04/VB10-vs-VB6_3A00_-Array-Lower-Bounds.aspx", "/posts/vb10-vs-vb6-array-lower-bounds/")
    m_lookup.Add("/blog/archive/2011/06/20/VB10-vs-VB6_3A00_-Dynamic-Arrays-Usage-In-Structures.aspx", "/posts/vb10-vs-vb6-dynamic-arrays-usage-in-structures/")
    m_lookup.Add("/blog/archive/2011/06/29/VB10-vs-VB6_3A00_-Option-Base.aspx", "/posts/vb10-vs-vb6-option-base/")
    m_lookup.Add("/blog/archive/2011/07/27/VB10-vs-VB6_3A00_-Variants-are-not-supported_2E00_.aspx", "/posts/vb10-vs-vb6-variants-are-not-supported/")
    m_lookup.Add("/blog/archive/2011/05/29/VB10-vs-VB6-_2800_aka-VB.FRED_2900_.aspx", "/posts/vb10-vs-vb6-aka-vfred-vs-classic-vb/")

    m_lookup.Add("/blog/archive/2009/01/03/Action-Movie-Night-_2D00_-January-17th.aspx", "")

    'Scan(root)

    'For Each entry In m_list
    '  'Console.WriteLine(entry)
    '  Console.WriteLine($"m_lookup.Add(""{entry}"", """")")
    'Next

    For Each key In m_lookup.Keys

      Dim match = "^" & key.Substring(1).Replace(".", "\.") & "$"
      Dim target = m_lookup(key).Substring(1)

      Dim output = $"<rule name = ""{key}"" stopProcessing=""true"">
  <match url = ""{match}"" ignoreCase=""true"" />
  <action type = ""Rewrite"" url=""posts/revenge-of-driveinfo-drive-serial-numbers/"" redirectType=""Permanent"" />
</rule>"

      Console.WriteLine(output)
    Next


  End Sub

  Private Sub Scan(path As String)

    Dim files = IO.Directory.GetFiles(path, "*.md")

    For Each file In files
      Process(file)
    Next

    Dim directories = IO.Directory.GetDirectories(path)

    For Each directory In directories
      Scan(directory)
    Next

  End Sub

  Private Sub Process(filename As String)

    'Console.WriteLine(IO.Path.GetFileName(filename))

    Dim content = IO.File.ReadAllText(filename)

    Dim offset = 0
    Do
      offset = content.IndexOf("](", offset)
      If offset >= 0 Then
        offset += 2
        Dim nextOffset = content.IndexOf(")"c, offset)
        If nextOffset >= 0 Then
          Dim value = content.Substring(offset, nextOffset - offset)
          If value.StartsWith("/blog") Then
            'Console.WriteLine($"  {value}")
            If Not m_list.Contains(value) AndAlso
               Not m_lookup.ContainsKey(value) Then
              m_list.Add(value)
            End If
          End If
          offset = nextOffset + 1
        Else
          offset += 1
        End If
      Else
        Exit Do
      End If
    Loop

  End Sub


End Module
