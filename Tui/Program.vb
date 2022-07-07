Imports System.Linq
Imports Terminal.Gui
Imports System
Imports NStack
Imports System.Text
Imports Rune = System.Rune

Module Program

  Public MouseLabel As Label

  Private m_scrollViewCheckMenuBarItem As MenuBarItem
  Private m_isBox10x As Boolean = True
  Private m_window As Window
  Private m_scrollView As ScrollView

  Public Sub Main(args As String())

    If args.Length > 0 AndAlso args.Contains("-usc") Then
      Application.UseSystemConsole = True
    End If

    Console.OutputEncoding = Encoding.[Default]

    Application.Init()

    Dim top1 = Application.Top

    m_window = New Window("Hello") With
    {
    .X = 0,
    .Y = 1,
    .Width = [Dim].Fill(),
    .Height = [Dim].Fill() - 1
    }

    Dim menu As New MenuBar(New MenuBarItem() {
        New MenuBarItem("_File", New MenuItem() {
            New MenuItem("_New", "Creates new file", AddressOf NewFile),
            New MenuItem("_Open", "", Nothing),
            New MenuItem("_Close", "", Sub() Close()),
            New MenuItem("_Quit", "", Sub()
                                        If Quit() Then
                                          top1.Running = False
                                        End If
                                      End Sub)
        }),
        New MenuBarItem("_Edit", New MenuItem() {
            New MenuItem("_Copy", "", Nothing),
            New MenuItem("C_ut", "", Nothing),
            New MenuItem("_Paste", "", Nothing)
        }),
        New MenuBarItem("A_ssorted", New MenuItem() {
            New MenuItem("_Show text alignments", "", Sub() ShowTextAlignments(), Nothing, Nothing, Terminal.Gui.Key.AltMask Or Terminal.Gui.Key.CtrlMask Or CType(71, Key))
        }),
        __InlineAssignHelper(m_scrollViewCheckMenuBarItem, New MenuBarItem("ScrollView", New MenuItem() {
New MenuItem("Box10x", "", Sub() ScrollViewCheck()) With
{
        .CheckType = MenuItemCheckStyle.Radio,
        .Checked = True},
New MenuItem("Filler", "", Sub() ScrollViewCheck()) With
{
        .CheckType = MenuItemCheckStyle.Radio}
        }))
    })

    ShowEntries(m_window)
    Dim count As Integer = 0
    MouseLabel = New Label(New Rect(3, 17, 47, 1), "Mouse: ")
    'Application.RootMouseEvent += Sub([me]) MouseLabel.Text = $"Mouse: ({[me].X},{[me].Y}) - {[me].Flags} {Math.Min(Threading.Interlocked.Increment(count), count - 1)}"

    m_window.Add(MouseLabel)

    Dim statusBar1 As New StatusBar(New StatusItem() {
        New StatusItem(Key.F1, "~F1~ Help", Function() MessageBox.Query(50, 7, "Help", "Helping", "Ok")),
        New StatusItem(Key.F2, "~F2~ Load", Function() MessageBox.Query(50, 7, "Load", "Loading", "Ok")),
        New StatusItem(Key.F3, "~F3~ Save", Function() MessageBox.Query(50, 7, "Save", "Saving", "Ok")),
        New StatusItem(Key.CtrlMask Or CType(81, Key), "~^Q~ Quit", Sub()
                                                                      If Quit() Then
                                                                        top1.Running = False
                                                                      End If
                                                                    End Sub),
        New StatusItem(Key.Null, Application.Driver.[GetType]().Name, Nothing)
    })

    top1.Add(m_window, menu, statusBar1)
    Application.Run()

    Application.Shutdown()
  End Sub

  <Obsolete("Please refactor code that uses this function, it is a simple work-around to simulate inline assignment in VB!")>
  Private Function __InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
    target = value
    Return value
  End Function

  Class Box10x
    Inherits View
    Public Sub New(x As Integer, y As Integer)
      MyBase.New(New Rect(x, y, 10, 10))
    End Sub
    Public Overrides Sub Redraw(region As Rect)
      Driver.SetAttribute(ColorScheme.Focus)

      For y As Integer = 0 To 10 - 1
        Move(0, y)
        For x As Integer = 0 To 10 - 1
          Driver.AddRune(CType((AscW("0"c) + ((x + y) Mod 10)), Rune))
        Next
      Next
    End Sub
  End Class
  Class Filler
    Inherits View
    Public Sub New(rect As Rect)
      MyBase.New(rect)
    End Sub
    Public Overrides Sub Redraw(region As Rect)
      Driver.SetAttribute(ColorScheme.Focus)
      Dim f = Frame

      For y As Integer = 0 To f.Width - 1
        Move(0, y)
        For x As Integer = 0 To f.Height - 1
          Dim tempVar As Char

          Select Case (x Mod 3)
            Case = 0
              tempVar = "."c
            Case = 1
              tempVar = "o"c
            Case Else
              tempVar = "O"c
          End Select

          Dim r As Char = tempVar
          Driver.AddRune(r)
        Next
      Next
    End Sub
  End Class
  Private Sub ShowTextAlignments()
    Dim container As New Window("Show Text Alignments - Press Esc to return") With
    {
    .X = 0,
    .Y = 0,
    .Width = [Dim].Fill(),
    .Height = [Dim].Fill()
    }
    AddHandler container.KeyUp, Sub(e)
                                  If e.KeyEvent.Key = Key.Esc Then
                                    container.Running = False
                                  End If
                                End Sub

    Const i As Integer = 0
    Const txt As String = "Hello world, how are you doing today?"
    container.Add(
New Label($"{i + 1}-{txt}") With
{
        .TextAlignment = TextAlignment.Left,
        .Y = 3,
        .Width = [Dim].Fill()},
New Label($"{i + 2}-{txt}") With
{
        .TextAlignment = TextAlignment.Right,
        .Y = 5,
        .Width = [Dim].Fill()},
New Label($"{i + 3}-{txt}") With
{
        .TextAlignment = TextAlignment.Centered,
        .Y = 7,
        .Width = [Dim].Fill()},
New Label($"{i + 4}-{txt}") With
{
        .TextAlignment = TextAlignment.Justified,
        .Y = 9,
        .Width = [Dim].Fill()}
        )

    Application.Run(container)
  End Sub

  Private m_progress As ProgressBar

  Private Sub ShowEntries(container As View)
    ' TODO Check: Local function was replaced with Lambda
    Dim timer As Func(Of MainLoop, Boolean) = Function(__ As MainLoop) As Boolean
                                                m_progress.Pulse()
                                                Return True
                                              End Function
    m_scrollView = New ScrollView(New Rect(50, 10, 20, 8)) With
    {
    .ContentSize = New Size(100, 100),
    .ContentOffset = New Point(-1, -1),
    .ShowVerticalScrollIndicator = True,
    .ShowHorizontalScrollIndicator = True
    }

    AddScrollViewChild()

    ' This is just to debug the visuals of the scrollview when small
    Dim scrollView2 As New ScrollView(New Rect(72, 10, 3, 3)) With
    {
    .ContentSize = New Size(100, 100),
    .ShowVerticalScrollIndicator = True,
    .ShowHorizontalScrollIndicator = True
    }
    scrollView2.Add(New Box10x(0, 0))
    'Dim progress As New ProgressBar(New Rect(68, 1, 10, 1))
    m_progress = New ProgressBar(New Rect(68, 1, 10, 1))

    Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(300), timer)

    ' A little convoluted, this is because I am using this to test the
    ' layout based on referencing elements of another view:

    Dim login As New Label("Login: ") With
{
        .X = 3,
        .Y = 6}
    Dim password As New Label("Password: ") With
    {
    .X = Pos.Left(login),
    .Y = Pos.Bottom(login) + 1
    }
    Dim loginText As New TextField("") With
    {
    .X = Pos.Right(password),
    .Y = Pos.Top(login),
    .Width = 40
    }
    Dim passText As New TextField("") With
    {
    .Secret = True,
    .X = Pos.Left(loginText),
    .Y = Pos.Top(password),
    .Width = [Dim].Width(loginText)
    }

    ' Add some content
    container.Add(
        login,
        loginText,
        password,
        passText,
        New FrameView(New Rect(3, 10, 25, 6), "Options", New View() {
        New CheckBox(1, 0, "Remember me"),
        New RadioGroup(1, 2, New ustring() {"_Personal", "_Company"})}
        ),
        New ListView(New Rect(60, 6, 16, 4), New String() {
        "First row",
        "<>",
        "This is a very long row that should overflow what is shown",
        "4th",
        "There is an empty slot on the second row",
        "Whoa",
        "This is so cool"
        }),
        m_scrollView,
        scrollView2,
New Button("Ok") With
{
        .X = 3,
        .Y = 19},
New Button("Cancel") With
{
        .X = 10,
        .Y = 19},
        m_progress,
New Label("Press F9 (on Unix ESC+9 is an alias) to activate the menubar") With
{
        .X = 3,
        .Y = 22}
    )
  End Sub
  Private Sub AddScrollViewChild()
    If m_isBox10x Then
      m_scrollView.Add(New Box10x(0, 0))
    Else
      m_scrollView.Add(New Filler(New Rect(0, 0, 40, 40)))
    End If
    m_scrollView.ContentOffset = Point.Empty
  End Sub

  Private Sub NewFile()
    Dim okButton As New Button("Ok", is_default:=True)
    AddHandler okButton.Clicked, Sub() Application.RequestStop()
    Dim cancelButton As New Button("Cancel")
    AddHandler cancelButton.Clicked, Sub() Application.RequestStop()

    Dim d As New Dialog(
        "New File", 50, 20,
        okButton,
        cancelButton)

    Dim ml2 As New Label(1, 1, "Mouse Debug Line")
    d.Add(ml2)
    Application.Run(d)
  End Sub

  Private Function Quit() As Boolean
    Dim n = MessageBox.Query(50, 7, "Quit Demo", "Are you sure you want to quit this demo?", "Yes", "No")
    Return n = 0
  End Function
  Private Sub Close()
    MessageBox.ErrorQuery(50, 7, "Error", "There is nothing to close", "Ok")
  End Sub
  Private Sub ScrollViewCheck()
    m_isBox10x = __InlineAssignHelper(m_scrollViewCheckMenuBarItem.Children(0).Checked, Not m_scrollViewCheckMenuBarItem.Children(0).Checked)
    m_scrollViewCheckMenuBarItem.Children(1).Checked = Not m_scrollViewCheckMenuBarItem.Children(1).Checked

    m_scrollView.RemoveAll()
    AddScrollViewChild()
  End Sub

End Module