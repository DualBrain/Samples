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
    Me.TreeViewPanel = New WinFormsApp1.PanelEx()
    Me.TreeView1 = New System.Windows.Forms.TreeView()
    Me.RightSidePanel = New WinFormsApp1.PanelEx()
        Me.ComboBoxEx1 = New WinFormsApp1.ComboBoxEx()
        Me.PanelEx1 = New WinFormsApp1.PanelEx()
        Me.RichTextBoxPanel = New WinFormsApp1.PanelEx()
        Me.RichTextBox2 = New System.Windows.Forms.RichTextBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripComboBox1 = New WinFormsApp1.ToolStripComboBoxEx()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripComboBox2 = New WinFormsApp1.ToolStripComboBoxEx()
        Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TreeViewPanel.SuspendLayout()
        Me.RightSidePanel.SuspendLayout()
        Me.PanelEx1.SuspendLayout()
        Me.RichTextBoxPanel.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'WebView
        '
        Me.WebView.CreationProperties = Nothing
        Me.WebView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WebView.Location = New System.Drawing.Point(1, 1)
        Me.WebView.Name = "WebView"
        Me.WebView.Size = New System.Drawing.Size(515, 182)
        Me.WebView.Source = New System.Uri("https://gotbasic.com", System.UriKind.Absolute)
        Me.WebView.TabIndex = 0
        Me.WebView.Text = "None"
        Me.WebView.ZoomFactor = 1.0R
        '
        'AddressTextBox
        '
        Me.AddressTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.AddressTextBox.Location = New System.Drawing.Point(4, 4)
        Me.AddressTextBox.Name = "AddressTextBox"
        Me.AddressTextBox.Size = New System.Drawing.Size(439, 23)
        Me.AddressTextBox.TabIndex = 1
        '
        'GoButton
        '
        Me.GoButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GoButton.Location = New System.Drawing.Point(444, 2)
        Me.GoButton.Name = "GoButton"
        Me.GoButton.Size = New System.Drawing.Size(75, 23)
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
        Me.MenuStrip1.Size = New System.Drawing.Size(789, 24)
        Me.MenuStrip1.TabIndex = 3
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewToolStripMenuItem, Me.OpenToolStripMenuItem, Me.toolStripSeparator, Me.SaveToolStripMenuItem, Me.SaveAsToolStripMenuItem, Me.toolStripSeparator1, Me.PrintToolStripMenuItem, Me.PrintPreviewToolStripMenuItem, Me.toolStripSeparator2, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'NewToolStripMenuItem
        '
        Me.NewToolStripMenuItem.Image = CType(resources.GetObject("NewToolStripMenuItem.Image"), System.Drawing.Image)
        Me.NewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.NewToolStripMenuItem.Name = "NewToolStripMenuItem"
        Me.NewToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
        Me.NewToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.NewToolStripMenuItem.Text = "&New"
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Image = CType(resources.GetObject("OpenToolStripMenuItem.Image"), System.Drawing.Image)
        Me.OpenToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.OpenToolStripMenuItem.Text = "&Open"
        '
        'toolStripSeparator
        '
        Me.toolStripSeparator.Name = "toolStripSeparator"
        Me.toolStripSeparator.Size = New System.Drawing.Size(143, 6)
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Image = CType(resources.GetObject("SaveToolStripMenuItem.Image"), System.Drawing.Image)
        Me.SaveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.SaveToolStripMenuItem.Text = "&Save"
        '
        'SaveAsToolStripMenuItem
        '
        Me.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem"
        Me.SaveAsToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.SaveAsToolStripMenuItem.Text = "Save &As"
        '
        'toolStripSeparator1
        '
        Me.toolStripSeparator1.Name = "toolStripSeparator1"
        Me.toolStripSeparator1.Size = New System.Drawing.Size(143, 6)
        '
        'PrintToolStripMenuItem
        '
        Me.PrintToolStripMenuItem.Image = CType(resources.GetObject("PrintToolStripMenuItem.Image"), System.Drawing.Image)
        Me.PrintToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.PrintToolStripMenuItem.Name = "PrintToolStripMenuItem"
        Me.PrintToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.P), System.Windows.Forms.Keys)
        Me.PrintToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.PrintToolStripMenuItem.Text = "&Print"
        '
        'PrintPreviewToolStripMenuItem
        '
        Me.PrintPreviewToolStripMenuItem.Image = CType(resources.GetObject("PrintPreviewToolStripMenuItem.Image"), System.Drawing.Image)
        Me.PrintPreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.PrintPreviewToolStripMenuItem.Name = "PrintPreviewToolStripMenuItem"
        Me.PrintPreviewToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.PrintPreviewToolStripMenuItem.Text = "Print Pre&view"
        '
        'toolStripSeparator2
        '
        Me.toolStripSeparator2.Name = "toolStripSeparator2"
        Me.toolStripSeparator2.Size = New System.Drawing.Size(143, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'EditToolStripMenuItem
        '
        Me.EditToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UndoToolStripMenuItem, Me.RedoToolStripMenuItem, Me.toolStripSeparator3, Me.CutToolStripMenuItem, Me.CopyToolStripMenuItem, Me.PasteToolStripMenuItem, Me.toolStripSeparator4, Me.SelectAllToolStripMenuItem})
        Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        Me.EditToolStripMenuItem.Size = New System.Drawing.Size(39, 20)
        Me.EditToolStripMenuItem.Text = "&Edit"
        '
        'UndoToolStripMenuItem
        '
        Me.UndoToolStripMenuItem.Name = "UndoToolStripMenuItem"
        Me.UndoToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.UndoToolStripMenuItem.Size = New System.Drawing.Size(144, 22)
        Me.UndoToolStripMenuItem.Text = "&Undo"
        '
        'RedoToolStripMenuItem
        '
        Me.RedoToolStripMenuItem.Name = "RedoToolStripMenuItem"
        Me.RedoToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Y), System.Windows.Forms.Keys)
        Me.RedoToolStripMenuItem.Size = New System.Drawing.Size(144, 22)
        Me.RedoToolStripMenuItem.Text = "&Redo"
        '
        'toolStripSeparator3
        '
        Me.toolStripSeparator3.Name = "toolStripSeparator3"
        Me.toolStripSeparator3.Size = New System.Drawing.Size(141, 6)
        '
        'CutToolStripMenuItem
        '
        Me.CutToolStripMenuItem.Image = CType(resources.GetObject("CutToolStripMenuItem.Image"), System.Drawing.Image)
        Me.CutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.CutToolStripMenuItem.Name = "CutToolStripMenuItem"
        Me.CutToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.X), System.Windows.Forms.Keys)
        Me.CutToolStripMenuItem.Size = New System.Drawing.Size(144, 22)
        Me.CutToolStripMenuItem.Text = "Cu&t"
        '
        'CopyToolStripMenuItem
        '
        Me.CopyToolStripMenuItem.Image = CType(resources.GetObject("CopyToolStripMenuItem.Image"), System.Drawing.Image)
        Me.CopyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
        Me.CopyToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(144, 22)
        Me.CopyToolStripMenuItem.Text = "&Copy"
        '
        'PasteToolStripMenuItem
        '
        Me.PasteToolStripMenuItem.Image = CType(resources.GetObject("PasteToolStripMenuItem.Image"), System.Drawing.Image)
        Me.PasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.PasteToolStripMenuItem.Name = "PasteToolStripMenuItem"
        Me.PasteToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.V), System.Windows.Forms.Keys)
        Me.PasteToolStripMenuItem.Size = New System.Drawing.Size(144, 22)
        Me.PasteToolStripMenuItem.Text = "&Paste"
        '
        'toolStripSeparator4
        '
        Me.toolStripSeparator4.Name = "toolStripSeparator4"
        Me.toolStripSeparator4.Size = New System.Drawing.Size(141, 6)
        '
        'SelectAllToolStripMenuItem
        '
        Me.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem"
        Me.SelectAllToolStripMenuItem.Size = New System.Drawing.Size(144, 22)
        Me.SelectAllToolStripMenuItem.Text = "Select &All"
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CustomizeToolStripMenuItem, Me.OptionsToolStripMenuItem})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(46, 20)
        Me.ToolsToolStripMenuItem.Text = "&Tools"
        '
        'CustomizeToolStripMenuItem
        '
        Me.CustomizeToolStripMenuItem.Name = "CustomizeToolStripMenuItem"
        Me.CustomizeToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.CustomizeToolStripMenuItem.Text = "&Customize"
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.OptionsToolStripMenuItem.Text = "&Options"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ContentsToolStripMenuItem, Me.IndexToolStripMenuItem, Me.SearchToolStripMenuItem, Me.toolStripSeparator5, Me.AboutToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.HelpToolStripMenuItem.Text = "&Help"
        '
        'ContentsToolStripMenuItem
        '
        Me.ContentsToolStripMenuItem.Name = "ContentsToolStripMenuItem"
        Me.ContentsToolStripMenuItem.Size = New System.Drawing.Size(122, 22)
        Me.ContentsToolStripMenuItem.Text = "&Contents"
        '
        'IndexToolStripMenuItem
        '
        Me.IndexToolStripMenuItem.Name = "IndexToolStripMenuItem"
        Me.IndexToolStripMenuItem.Size = New System.Drawing.Size(122, 22)
        Me.IndexToolStripMenuItem.Text = "&Index"
        '
        'SearchToolStripMenuItem
        '
        Me.SearchToolStripMenuItem.Name = "SearchToolStripMenuItem"
        Me.SearchToolStripMenuItem.Size = New System.Drawing.Size(122, 22)
        Me.SearchToolStripMenuItem.Text = "&Search"
        '
        'toolStripSeparator5
        '
        Me.toolStripSeparator5.Name = "toolStripSeparator5"
        Me.toolStripSeparator5.Size = New System.Drawing.Size(119, 6)
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(122, 22)
        Me.AboutToolStripMenuItem.Text = "&About..."
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 454)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(789, 22)
        Me.StatusStrip1.TabIndex = 4
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(119, 17)
        Me.ToolStripStatusLabel1.Text = "ToolStripStatusLabel1"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 50)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TreeViewPanel)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.RightSidePanel)
        Me.SplitContainer1.Size = New System.Drawing.Size(789, 404)
        Me.SplitContainer1.SplitterDistance = 262
        Me.SplitContainer1.TabIndex = 5
        '
        'TreeViewPanel
        '
        Me.TreeViewPanel.BackColor = System.Drawing.Color.Transparent
        Me.TreeViewPanel.Controls.Add(Me.TreeView1)
        Me.TreeViewPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeViewPanel.Location = New System.Drawing.Point(0, 0)
        Me.TreeViewPanel.Name = "TreeViewPanel"
        Me.TreeViewPanel.Padding = New System.Windows.Forms.Padding(1)
        Me.TreeViewPanel.Size = New System.Drawing.Size(262, 404)
        Me.TreeViewPanel.TabIndex = 1
        '
        'TreeView1
        '
        Me.TreeView1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeView1.Location = New System.Drawing.Point(1, 1)
        Me.TreeView1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.Size = New System.Drawing.Size(260, 402)
        Me.TreeView1.TabIndex = 0
        '
        'RightSidePanel
        '
        Me.RightSidePanel.BackColor = System.Drawing.Color.Transparent
        Me.RightSidePanel.Controls.Add(Me.CheckBox1)
        Me.RightSidePanel.Controls.Add(Me.ComboBoxEx1)
        Me.RightSidePanel.Controls.Add(Me.PanelEx1)
        Me.RightSidePanel.Controls.Add(Me.RichTextBoxPanel)
        Me.RightSidePanel.Controls.Add(Me.TextBox1)
        Me.RightSidePanel.Controls.Add(Me.AddressTextBox)
        Me.RightSidePanel.Controls.Add(Me.GoButton)
        Me.RightSidePanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RightSidePanel.Location = New System.Drawing.Point(0, 0)
        Me.RightSidePanel.Name = "RightSidePanel"
        Me.RightSidePanel.Size = New System.Drawing.Size(523, 404)
        Me.RightSidePanel.TabIndex = 4
        '
        'ComboBoxEx1
        '
        Me.ComboBoxEx1.BorderColor = System.Drawing.Color.SteelBlue
        Me.ComboBoxEx1.BorderDrawMode = WinFormsApp1.ComboBoxEx.ControlBorderDrawMode.Full
        Me.ComboBoxEx1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ComboBoxEx1.FormattingEnabled = True
        Me.ComboBoxEx1.Location = New System.Drawing.Point(192, 224)
        Me.ComboBoxEx1.Name = "ComboBoxEx1"
        Me.ComboBoxEx1.Size = New System.Drawing.Size(121, 23)
        Me.ComboBoxEx1.TabIndex = 4
        '
        'PanelEx1
        '
        Me.PanelEx1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelEx1.BackColor = System.Drawing.Color.Transparent
        Me.PanelEx1.Controls.Add(Me.WebView)
        Me.PanelEx1.Location = New System.Drawing.Point(2, 34)
        Me.PanelEx1.Name = "PanelEx1"
        Me.PanelEx1.Padding = New System.Windows.Forms.Padding(1)
        Me.PanelEx1.Size = New System.Drawing.Size(517, 184)
        Me.PanelEx1.TabIndex = 3
        '
        'RichTextBoxPanel
        '
        Me.RichTextBoxPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RichTextBoxPanel.BackColor = System.Drawing.Color.Transparent
        Me.RichTextBoxPanel.Controls.Add(Me.RichTextBox2)
        Me.RichTextBoxPanel.Location = New System.Drawing.Point(2, 253)
        Me.RichTextBoxPanel.Name = "RichTextBoxPanel"
        Me.RichTextBoxPanel.Padding = New System.Windows.Forms.Padding(1)
        Me.RichTextBoxPanel.Size = New System.Drawing.Size(517, 147)
        Me.RichTextBoxPanel.TabIndex = 1
        '
        'RichTextBox2
        '
        Me.RichTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.RichTextBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RichTextBox2.Location = New System.Drawing.Point(1, 1)
        Me.RichTextBox2.Name = "RichTextBox2"
        Me.RichTextBox2.Size = New System.Drawing.Size(515, 145)
        Me.RichTextBox2.TabIndex = 1
        Me.RichTextBox2.Text = ""
        '
        'TextBox1
        '
        Me.TextBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.TextBox1.Location = New System.Drawing.Point(2, 224)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(183, 23)
        Me.TextBox1.TabIndex = 2
        '
        'ToolStrip1
        '
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripComboBox1, Me.ToolStripSeparator8, Me.ToolStripLabel1, Me.ToolStripComboBox2, Me.ToolStripSeparator9})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 24)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(789, 26)
        Me.ToolStrip1.TabIndex = 6
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripComboBox1
        '
        Me.ToolStripComboBox1.Name = "ToolStripComboBox1"
        Me.ToolStripComboBox1.Size = New System.Drawing.Size(121, 23)
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        Me.ToolStripSeparator8.Size = New System.Drawing.Size(6, 26)
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(49, 23)
        Me.ToolStripLabel1.Text = "Look in:"
        '
        'ToolStripComboBox2
        '
        Me.ToolStripComboBox2.Name = "ToolStripComboBox2"
        Me.ToolStripComboBox2.Size = New System.Drawing.Size(121, 23)
        '
        'ToolStripSeparator9
        '
        Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
        Me.ToolStripSeparator9.Size = New System.Drawing.Size(6, 26)
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(320, 224)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(85, 19)
        Me.CheckBox1.TabIndex = 5
        Me.CheckBox1.Text = "CheckBox1"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(789, 476)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.KeyPreview = True
        Me.MainMenuStrip = Me.MenuStrip1
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
        Me.TreeViewPanel.ResumeLayout(False)
        Me.RightSidePanel.ResumeLayout(False)
        Me.RightSidePanel.PerformLayout()
        Me.PanelEx1.ResumeLayout(False)
        Me.RichTextBoxPanel.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
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
  Friend WithEvents RichTextBox2 As RichTextBox
  Friend WithEvents TextBox1 As TextBox
  Private WithEvents RichTextBoxPanel As PanelEx
  Private WithEvents TreeViewPanel As PanelEx
  Private WithEvents RightSidePanel As PanelEx
  Private WithEvents PanelEx1 As PanelEx
  Private WithEvents WebView As Microsoft.Web.WebView2.WinForms.WebView2
  Friend WithEvents ToolStripComboBox1 As ToolStripComboBoxEx
  Friend WithEvents ToolStripSeparator8 As ToolStripSeparator
  Friend WithEvents ToolStripLabel1 As ToolStripLabel
  Friend WithEvents ToolStripComboBox2 As ToolStripComboBoxEx
  Friend WithEvents ToolStripSeparator9 As ToolStripSeparator
  Friend WithEvents ComboBoxEx1 As ComboBoxEx
    Friend WithEvents CheckBox1 As CheckBox
End Class
