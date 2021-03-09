<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
  Inherits System.Windows.Forms.Form

  'Form overrides dispose to clean up the component list.
  <System.Diagnostics.DebuggerNonUserCode()> _
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    Try
      If disposing AndAlso components IsNot Nothing Then
        components.Dispose()
      End If
    Finally
      MyBase.Dispose(disposing)
    End Try
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> _
  Private Sub InitializeComponent()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
    Me.WebView = New Microsoft.Web.WebView2.WinForms.WebView2()
    Me.AddressTextBox = New System.Windows.Forms.TextBox()
    Me.GoButton = New System.Windows.Forms.Button()
    Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
    Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.NewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.toolStripSeparator = New System.Windows.Forms.ToolStripSeparator()
    Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.SaveAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
    Me.PrintToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.PrintPreviewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.toolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
    Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.UndoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.RedoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.toolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
    Me.CutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.PasteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.toolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
    Me.SelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.CustomizeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.ContentsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.IndexToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.SearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.toolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
    Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
    Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
    Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
    Me.Panel2 = New WinFormsApp1.PanelEx()
    Me.TreeView1 = New System.Windows.Forms.TreeView()
    Me.Panel3 = New WinFormsApp1.PanelEx()
    Me.Panel1 = New WinFormsApp1.PanelEx()
    Me.RichTextBox2 = New System.Windows.Forms.RichTextBox()
    Me.TextBox1 = New System.Windows.Forms.TextBox()
    Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
    Me.NewToolStripButton = New System.Windows.Forms.ToolStripButton()
    Me.OpenToolStripButton = New System.Windows.Forms.ToolStripButton()
    Me.SaveToolStripButton = New System.Windows.Forms.ToolStripButton()
    Me.PrintToolStripButton = New System.Windows.Forms.ToolStripButton()
    Me.toolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
    Me.CutToolStripButton = New System.Windows.Forms.ToolStripButton()
    Me.CopyToolStripButton = New System.Windows.Forms.ToolStripButton()
    Me.PasteToolStripButton = New System.Windows.Forms.ToolStripButton()
    Me.toolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
    Me.HelpToolStripButton = New System.Windows.Forms.ToolStripButton()
    Me.MenuStrip1.SuspendLayout()
    Me.StatusStrip1.SuspendLayout()
    CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SplitContainer1.Panel1.SuspendLayout()
    Me.SplitContainer1.Panel2.SuspendLayout()
    Me.SplitContainer1.SuspendLayout()
    Me.Panel2.SuspendLayout()
    Me.Panel3.SuspendLayout()
    Me.Panel1.SuspendLayout()
    Me.ToolStrip1.SuspendLayout()
    Me.SuspendLayout()
    '
    'WebView
    '
    Me.WebView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.WebView.CreationProperties = Nothing
    Me.WebView.Location = New System.Drawing.Point(2, 41)
    Me.WebView.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
    Me.WebView.Name = "WebView"
    Me.WebView.Size = New System.Drawing.Size(590, 263)
    Me.WebView.Source = New System.Uri("https://gotbasic.com", System.UriKind.Absolute)
    Me.WebView.TabIndex = 0
    Me.WebView.Text = "WebView21"
    Me.WebView.ZoomFactor = 1.0R
    '
    'AddressTextBox
    '
    Me.AddressTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.AddressTextBox.Location = New System.Drawing.Point(4, 5)
    Me.AddressTextBox.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
    Me.AddressTextBox.Name = "AddressTextBox"
    Me.AddressTextBox.Size = New System.Drawing.Size(500, 27)
    Me.AddressTextBox.TabIndex = 1
    '
    'GoButton
    '
    Me.GoButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.GoButton.Location = New System.Drawing.Point(506, 3)
    Me.GoButton.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
    Me.GoButton.Name = "GoButton"
    Me.GoButton.Size = New System.Drawing.Size(86, 31)
    Me.GoButton.TabIndex = 2
    Me.GoButton.Text = "Go!"
    Me.GoButton.UseVisualStyleBackColor = True
    '
    'MenuStrip1
    '
    Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
    Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.EditToolStripMenuItem, Me.ToolsToolStripMenuItem, Me.HelpToolStripMenuItem})
    Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
    Me.MenuStrip1.Name = "MenuStrip1"
    Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(7, 3, 0, 3)
    Me.MenuStrip1.Size = New System.Drawing.Size(902, 30)
    Me.MenuStrip1.TabIndex = 3
    Me.MenuStrip1.Text = "MenuStrip1"
    '
    'FileToolStripMenuItem
    '
    Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewToolStripMenuItem, Me.OpenToolStripMenuItem, Me.toolStripSeparator, Me.SaveToolStripMenuItem, Me.SaveAsToolStripMenuItem, Me.toolStripSeparator1, Me.PrintToolStripMenuItem, Me.PrintPreviewToolStripMenuItem, Me.toolStripSeparator2, Me.ExitToolStripMenuItem})
    Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
    Me.FileToolStripMenuItem.Size = New System.Drawing.Size(46, 24)
    Me.FileToolStripMenuItem.Text = "&File"
    '
    'NewToolStripMenuItem
    '
    Me.NewToolStripMenuItem.Image = CType(resources.GetObject("NewToolStripMenuItem.Image"), System.Drawing.Image)
    Me.NewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.NewToolStripMenuItem.Name = "NewToolStripMenuItem"
    Me.NewToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
    Me.NewToolStripMenuItem.Size = New System.Drawing.Size(181, 26)
    Me.NewToolStripMenuItem.Text = "&New"
    '
    'OpenToolStripMenuItem
    '
    Me.OpenToolStripMenuItem.Image = CType(resources.GetObject("OpenToolStripMenuItem.Image"), System.Drawing.Image)
    Me.OpenToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
    Me.OpenToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
    Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(181, 26)
    Me.OpenToolStripMenuItem.Text = "&Open"
    '
    'toolStripSeparator
    '
    Me.toolStripSeparator.Name = "toolStripSeparator"
    Me.toolStripSeparator.Size = New System.Drawing.Size(178, 6)
    '
    'SaveToolStripMenuItem
    '
    Me.SaveToolStripMenuItem.Image = CType(resources.GetObject("SaveToolStripMenuItem.Image"), System.Drawing.Image)
    Me.SaveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
    Me.SaveToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
    Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(181, 26)
    Me.SaveToolStripMenuItem.Text = "&Save"
    '
    'SaveAsToolStripMenuItem
    '
    Me.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem"
    Me.SaveAsToolStripMenuItem.Size = New System.Drawing.Size(181, 26)
    Me.SaveAsToolStripMenuItem.Text = "Save &As"
    '
    'toolStripSeparator1
    '
    Me.toolStripSeparator1.Name = "toolStripSeparator1"
    Me.toolStripSeparator1.Size = New System.Drawing.Size(178, 6)
    '
    'PrintToolStripMenuItem
    '
    Me.PrintToolStripMenuItem.Image = CType(resources.GetObject("PrintToolStripMenuItem.Image"), System.Drawing.Image)
    Me.PrintToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.PrintToolStripMenuItem.Name = "PrintToolStripMenuItem"
    Me.PrintToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.P), System.Windows.Forms.Keys)
    Me.PrintToolStripMenuItem.Size = New System.Drawing.Size(181, 26)
    Me.PrintToolStripMenuItem.Text = "&Print"
    '
    'PrintPreviewToolStripMenuItem
    '
    Me.PrintPreviewToolStripMenuItem.Image = CType(resources.GetObject("PrintPreviewToolStripMenuItem.Image"), System.Drawing.Image)
    Me.PrintPreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.PrintPreviewToolStripMenuItem.Name = "PrintPreviewToolStripMenuItem"
    Me.PrintPreviewToolStripMenuItem.Size = New System.Drawing.Size(181, 26)
    Me.PrintPreviewToolStripMenuItem.Text = "Print Pre&view"
    '
    'toolStripSeparator2
    '
    Me.toolStripSeparator2.Name = "toolStripSeparator2"
    Me.toolStripSeparator2.Size = New System.Drawing.Size(178, 6)
    '
    'ExitToolStripMenuItem
    '
    Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
    Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(181, 26)
    Me.ExitToolStripMenuItem.Text = "E&xit"
    '
    'EditToolStripMenuItem
    '
    Me.EditToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UndoToolStripMenuItem, Me.RedoToolStripMenuItem, Me.toolStripSeparator3, Me.CutToolStripMenuItem, Me.CopyToolStripMenuItem, Me.PasteToolStripMenuItem, Me.toolStripSeparator4, Me.SelectAllToolStripMenuItem})
    Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
    Me.EditToolStripMenuItem.Size = New System.Drawing.Size(49, 24)
    Me.EditToolStripMenuItem.Text = "&Edit"
    '
    'UndoToolStripMenuItem
    '
    Me.UndoToolStripMenuItem.Name = "UndoToolStripMenuItem"
    Me.UndoToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
    Me.UndoToolStripMenuItem.Size = New System.Drawing.Size(179, 26)
    Me.UndoToolStripMenuItem.Text = "&Undo"
    '
    'RedoToolStripMenuItem
    '
    Me.RedoToolStripMenuItem.Name = "RedoToolStripMenuItem"
    Me.RedoToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Y), System.Windows.Forms.Keys)
    Me.RedoToolStripMenuItem.Size = New System.Drawing.Size(179, 26)
    Me.RedoToolStripMenuItem.Text = "&Redo"
    '
    'toolStripSeparator3
    '
    Me.toolStripSeparator3.Name = "toolStripSeparator3"
    Me.toolStripSeparator3.Size = New System.Drawing.Size(176, 6)
    '
    'CutToolStripMenuItem
    '
    Me.CutToolStripMenuItem.Image = CType(resources.GetObject("CutToolStripMenuItem.Image"), System.Drawing.Image)
    Me.CutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.CutToolStripMenuItem.Name = "CutToolStripMenuItem"
    Me.CutToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.X), System.Windows.Forms.Keys)
    Me.CutToolStripMenuItem.Size = New System.Drawing.Size(179, 26)
    Me.CutToolStripMenuItem.Text = "Cu&t"
    '
    'CopyToolStripMenuItem
    '
    Me.CopyToolStripMenuItem.Image = CType(resources.GetObject("CopyToolStripMenuItem.Image"), System.Drawing.Image)
    Me.CopyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
    Me.CopyToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
    Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(179, 26)
    Me.CopyToolStripMenuItem.Text = "&Copy"
    '
    'PasteToolStripMenuItem
    '
    Me.PasteToolStripMenuItem.Image = CType(resources.GetObject("PasteToolStripMenuItem.Image"), System.Drawing.Image)
    Me.PasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.PasteToolStripMenuItem.Name = "PasteToolStripMenuItem"
    Me.PasteToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.V), System.Windows.Forms.Keys)
    Me.PasteToolStripMenuItem.Size = New System.Drawing.Size(179, 26)
    Me.PasteToolStripMenuItem.Text = "&Paste"
    '
    'toolStripSeparator4
    '
    Me.toolStripSeparator4.Name = "toolStripSeparator4"
    Me.toolStripSeparator4.Size = New System.Drawing.Size(176, 6)
    '
    'SelectAllToolStripMenuItem
    '
    Me.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem"
    Me.SelectAllToolStripMenuItem.Size = New System.Drawing.Size(179, 26)
    Me.SelectAllToolStripMenuItem.Text = "Select &All"
    '
    'ToolsToolStripMenuItem
    '
    Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CustomizeToolStripMenuItem, Me.OptionsToolStripMenuItem})
    Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
    Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(58, 24)
    Me.ToolsToolStripMenuItem.Text = "&Tools"
    '
    'CustomizeToolStripMenuItem
    '
    Me.CustomizeToolStripMenuItem.Name = "CustomizeToolStripMenuItem"
    Me.CustomizeToolStripMenuItem.Size = New System.Drawing.Size(161, 26)
    Me.CustomizeToolStripMenuItem.Text = "&Customize"
    '
    'OptionsToolStripMenuItem
    '
    Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
    Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(161, 26)
    Me.OptionsToolStripMenuItem.Text = "&Options"
    '
    'HelpToolStripMenuItem
    '
    Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ContentsToolStripMenuItem, Me.IndexToolStripMenuItem, Me.SearchToolStripMenuItem, Me.toolStripSeparator5, Me.AboutToolStripMenuItem})
    Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
    Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(55, 24)
    Me.HelpToolStripMenuItem.Text = "&Help"
    '
    'ContentsToolStripMenuItem
    '
    Me.ContentsToolStripMenuItem.Name = "ContentsToolStripMenuItem"
    Me.ContentsToolStripMenuItem.Size = New System.Drawing.Size(150, 26)
    Me.ContentsToolStripMenuItem.Text = "&Contents"
    '
    'IndexToolStripMenuItem
    '
    Me.IndexToolStripMenuItem.Name = "IndexToolStripMenuItem"
    Me.IndexToolStripMenuItem.Size = New System.Drawing.Size(150, 26)
    Me.IndexToolStripMenuItem.Text = "&Index"
    '
    'SearchToolStripMenuItem
    '
    Me.SearchToolStripMenuItem.Name = "SearchToolStripMenuItem"
    Me.SearchToolStripMenuItem.Size = New System.Drawing.Size(150, 26)
    Me.SearchToolStripMenuItem.Text = "&Search"
    '
    'toolStripSeparator5
    '
    Me.toolStripSeparator5.Name = "toolStripSeparator5"
    Me.toolStripSeparator5.Size = New System.Drawing.Size(147, 6)
    '
    'AboutToolStripMenuItem
    '
    Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
    Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(150, 26)
    Me.AboutToolStripMenuItem.Text = "&About..."
    '
    'StatusStrip1
    '
    Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
    Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
    Me.StatusStrip1.Location = New System.Drawing.Point(0, 609)
    Me.StatusStrip1.Name = "StatusStrip1"
    Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(1, 0, 16, 0)
    Me.StatusStrip1.Size = New System.Drawing.Size(902, 26)
    Me.StatusStrip1.TabIndex = 4
    Me.StatusStrip1.Text = "StatusStrip1"
    '
    'ToolStripStatusLabel1
    '
    Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
    Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(153, 20)
    Me.ToolStripStatusLabel1.Text = "ToolStripStatusLabel1"
    '
    'SplitContainer1
    '
    Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
    Me.SplitContainer1.Location = New System.Drawing.Point(0, 57)
    Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
    Me.SplitContainer1.Name = "SplitContainer1"
    '
    'SplitContainer1.Panel1
    '
    Me.SplitContainer1.Panel1.Controls.Add(Me.Panel2)
    '
    'SplitContainer1.Panel2
    '
    Me.SplitContainer1.Panel2.Controls.Add(Me.Panel3)
    Me.SplitContainer1.Size = New System.Drawing.Size(902, 552)
    Me.SplitContainer1.SplitterDistance = 300
    Me.SplitContainer1.SplitterWidth = 5
    Me.SplitContainer1.TabIndex = 5
    '
    'Panel2
    '
    Me.Panel2.BackColor = System.Drawing.Color.Transparent
    Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.Panel2.Controls.Add(Me.TreeView1)
    Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
    Me.Panel2.Location = New System.Drawing.Point(0, 0)
    Me.Panel2.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
    Me.Panel2.Name = "Panel2"
    Me.Panel2.Size = New System.Drawing.Size(300, 552)
    Me.Panel2.TabIndex = 1
    '
    'TreeView1
    '
    Me.TreeView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.TreeView1.BorderStyle = System.Windows.Forms.BorderStyle.None
    Me.TreeView1.Location = New System.Drawing.Point(2, 2)
    Me.TreeView1.Name = "TreeView1"
    Me.TreeView1.Size = New System.Drawing.Size(296, 548)
    Me.TreeView1.TabIndex = 0
    '
    'Panel3
    '
    Me.Panel3.BackColor = System.Drawing.Color.Transparent
    Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.Panel3.Controls.Add(Me.Panel1)
    Me.Panel3.Controls.Add(Me.TextBox1)
    Me.Panel3.Controls.Add(Me.AddressTextBox)
    Me.Panel3.Controls.Add(Me.WebView)
    Me.Panel3.Controls.Add(Me.GoButton)
    Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
    Me.Panel3.Location = New System.Drawing.Point(0, 0)
    Me.Panel3.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
    Me.Panel3.Name = "Panel3"
    Me.Panel3.Size = New System.Drawing.Size(597, 552)
    Me.Panel3.TabIndex = 4
    '
    'Panel1
    '
    Me.Panel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.Panel1.BackColor = System.Drawing.Color.Transparent
    Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.Panel1.Controls.Add(Me.RichTextBox2)
    Me.Panel1.Location = New System.Drawing.Point(2, 351)
    Me.Panel1.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
    Me.Panel1.Name = "Panel1"
    Me.Panel1.Size = New System.Drawing.Size(590, 196)
    Me.Panel1.TabIndex = 1
    '
    'RichTextBox2
    '
    Me.RichTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None
    Me.RichTextBox2.Dock = System.Windows.Forms.DockStyle.Fill
    Me.RichTextBox2.Location = New System.Drawing.Point(0, 0)
    Me.RichTextBox2.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
    Me.RichTextBox2.Name = "RichTextBox2"
    Me.RichTextBox2.Size = New System.Drawing.Size(590, 196)
    Me.RichTextBox2.TabIndex = 1
    Me.RichTextBox2.Text = ""
    '
    'TextBox1
    '
    Me.TextBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
    Me.TextBox1.Location = New System.Drawing.Point(2, 312)
    Me.TextBox1.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
    Me.TextBox1.Name = "TextBox1"
    Me.TextBox1.Size = New System.Drawing.Size(209, 27)
    Me.TextBox1.TabIndex = 2
    '
    'ToolStrip1
    '
    Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
    Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewToolStripButton, Me.OpenToolStripButton, Me.SaveToolStripButton, Me.PrintToolStripButton, Me.toolStripSeparator6, Me.CutToolStripButton, Me.CopyToolStripButton, Me.PasteToolStripButton, Me.toolStripSeparator7, Me.HelpToolStripButton})
    Me.ToolStrip1.Location = New System.Drawing.Point(0, 30)
    Me.ToolStrip1.Name = "ToolStrip1"
    Me.ToolStrip1.Size = New System.Drawing.Size(902, 27)
    Me.ToolStrip1.TabIndex = 6
    Me.ToolStrip1.Text = "ToolStrip1"
    '
    'NewToolStripButton
    '
    Me.NewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.NewToolStripButton.Image = CType(resources.GetObject("NewToolStripButton.Image"), System.Drawing.Image)
    Me.NewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.NewToolStripButton.Name = "NewToolStripButton"
    Me.NewToolStripButton.Size = New System.Drawing.Size(29, 24)
    Me.NewToolStripButton.Text = "&New"
    '
    'OpenToolStripButton
    '
    Me.OpenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.OpenToolStripButton.Image = CType(resources.GetObject("OpenToolStripButton.Image"), System.Drawing.Image)
    Me.OpenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.OpenToolStripButton.Name = "OpenToolStripButton"
    Me.OpenToolStripButton.Size = New System.Drawing.Size(29, 24)
    Me.OpenToolStripButton.Text = "&Open"
    '
    'SaveToolStripButton
    '
    Me.SaveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.SaveToolStripButton.Image = CType(resources.GetObject("SaveToolStripButton.Image"), System.Drawing.Image)
    Me.SaveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.SaveToolStripButton.Name = "SaveToolStripButton"
    Me.SaveToolStripButton.Size = New System.Drawing.Size(29, 24)
    Me.SaveToolStripButton.Text = "&Save"
    '
    'PrintToolStripButton
    '
    Me.PrintToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.PrintToolStripButton.Image = CType(resources.GetObject("PrintToolStripButton.Image"), System.Drawing.Image)
    Me.PrintToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.PrintToolStripButton.Name = "PrintToolStripButton"
    Me.PrintToolStripButton.Size = New System.Drawing.Size(29, 24)
    Me.PrintToolStripButton.Text = "&Print"
    '
    'toolStripSeparator6
    '
    Me.toolStripSeparator6.Name = "toolStripSeparator6"
    Me.toolStripSeparator6.Size = New System.Drawing.Size(6, 27)
    '
    'CutToolStripButton
    '
    Me.CutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.CutToolStripButton.Image = CType(resources.GetObject("CutToolStripButton.Image"), System.Drawing.Image)
    Me.CutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.CutToolStripButton.Name = "CutToolStripButton"
    Me.CutToolStripButton.Size = New System.Drawing.Size(29, 24)
    Me.CutToolStripButton.Text = "C&ut"
    '
    'CopyToolStripButton
    '
    Me.CopyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.CopyToolStripButton.Image = CType(resources.GetObject("CopyToolStripButton.Image"), System.Drawing.Image)
    Me.CopyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.CopyToolStripButton.Name = "CopyToolStripButton"
    Me.CopyToolStripButton.Size = New System.Drawing.Size(29, 24)
    Me.CopyToolStripButton.Text = "&Copy"
    '
    'PasteToolStripButton
    '
    Me.PasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.PasteToolStripButton.Image = CType(resources.GetObject("PasteToolStripButton.Image"), System.Drawing.Image)
    Me.PasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.PasteToolStripButton.Name = "PasteToolStripButton"
    Me.PasteToolStripButton.Size = New System.Drawing.Size(29, 24)
    Me.PasteToolStripButton.Text = "&Paste"
    '
    'toolStripSeparator7
    '
    Me.toolStripSeparator7.Name = "toolStripSeparator7"
    Me.toolStripSeparator7.Size = New System.Drawing.Size(6, 27)
    '
    'HelpToolStripButton
    '
    Me.HelpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.HelpToolStripButton.Image = CType(resources.GetObject("HelpToolStripButton.Image"), System.Drawing.Image)
    Me.HelpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.HelpToolStripButton.Name = "HelpToolStripButton"
    Me.HelpToolStripButton.Size = New System.Drawing.Size(29, 24)
    Me.HelpToolStripButton.Text = "He&lp"
    '
    'Form1
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(902, 635)
    Me.Controls.Add(Me.SplitContainer1)
    Me.Controls.Add(Me.StatusStrip1)
    Me.Controls.Add(Me.ToolStrip1)
    Me.Controls.Add(Me.MenuStrip1)
    Me.KeyPreview = True
    Me.MainMenuStrip = Me.MenuStrip1
    Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
    Me.Name = "Form1"
    Me.Text = "Form1"
    Me.MenuStrip1.ResumeLayout(False)
    Me.MenuStrip1.PerformLayout()
    Me.StatusStrip1.ResumeLayout(False)
    Me.StatusStrip1.PerformLayout()
    Me.SplitContainer1.Panel1.ResumeLayout(False)
    Me.SplitContainer1.Panel2.ResumeLayout(False)
    CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
    Me.SplitContainer1.ResumeLayout(False)
    Me.Panel2.ResumeLayout(False)
    Me.Panel3.ResumeLayout(False)
    Me.Panel3.PerformLayout()
    Me.Panel1.ResumeLayout(False)
    Me.ToolStrip1.ResumeLayout(False)
    Me.ToolStrip1.PerformLayout()
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub

  Friend WithEvents WebView As Microsoft.Web.WebView2.WinForms.WebView2
    Friend WithEvents AddressTextBox As TextBox
    Friend WithEvents GoButton As Button
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripSeparator As ToolStripSeparator
    Friend WithEvents SaveToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveAsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripSeparator1 As ToolStripSeparator
    Friend WithEvents PrintToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PrintPreviewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EditToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents UndoToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RedoToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripSeparator3 As ToolStripSeparator
    Friend WithEvents CutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CopyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PasteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripSeparator4 As ToolStripSeparator
    Friend WithEvents SelectAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CustomizeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OptionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ContentsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents IndexToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SearchToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripSeparator5 As ToolStripSeparator
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents TreeView1 As TreeView
    Private WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Private WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents NewToolStripButton As ToolStripButton
    Friend WithEvents OpenToolStripButton As ToolStripButton
    Friend WithEvents SaveToolStripButton As ToolStripButton
    Friend WithEvents PrintToolStripButton As ToolStripButton
    Friend WithEvents toolStripSeparator6 As ToolStripSeparator
    Friend WithEvents CutToolStripButton As ToolStripButton
    Friend WithEvents CopyToolStripButton As ToolStripButton
    Friend WithEvents PasteToolStripButton As ToolStripButton
    Friend WithEvents toolStripSeparator7 As ToolStripSeparator
    Friend WithEvents HelpToolStripButton As ToolStripButton
  Friend WithEvents Panel1 As PanelEx
  Friend WithEvents RichTextBox2 As RichTextBox
    Friend WithEvents TextBox1 As TextBox
  Friend WithEvents Panel2 As PanelEx
  Friend WithEvents Panel3 As PanelEx
End Class
