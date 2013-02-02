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
        Me.components = New System.ComponentModel.Container()
        Me.btnClick = New System.Windows.Forms.Button()
        Me.btnRequery = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cbxHideIE = New System.Windows.Forms.CheckBox()
        Me.cbxAutoOn = New System.Windows.Forms.CheckBox()
        Me.txtAutoInterval = New System.Windows.Forms.TextBox()
        Me.autoCheckWorker = New System.ComponentModel.BackgroundWorker()
        Me.gbxAutoSearchOptions = New System.Windows.Forms.GroupBox()
        Me.pbar = New System.Windows.Forms.ProgressBar()
        Me.lblAutoDisabled = New System.Windows.Forms.Label()
        Me.searchWorker = New System.ComponentModel.BackgroundWorker()
        Me.gbxItemSets = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.dgvActiveTrades = New System.Windows.Forms.DataGridView()
        Me.clKeyword = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.clLevels = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.clCrafts = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.clBuyerID64 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.clOPTrade = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.clNotes = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SearchMode = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.cmsItemsView = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.GoToTF2OPTradeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GoToUserSteamPageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopySearchResultsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenTF2WHPagesOfResultsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CompareResultsToABackpackToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.cbx_stgsSLSearches = New System.Windows.Forms.CheckBox()
        Me.cbx_StgsKeywordIgnoreCase = New System.Windows.Forms.CheckBox()
        Me.cbx_StgsHighlightSuccesses = New System.Windows.Forms.CheckBox()
        Me.lblLicense = New System.Windows.Forms.Label()
        Me.gbxAutoSearchOptions.SuspendLayout()
        CType(Me.dgvActiveTrades, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.cmsItemsView.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnClick
        '
        Me.btnClick.Location = New System.Drawing.Point(852, 26)
        Me.btnClick.Margin = New System.Windows.Forms.Padding(4)
        Me.btnClick.Name = "btnClick"
        Me.btnClick.Size = New System.Drawing.Size(126, 28)
        Me.btnClick.TabIndex = 3
        Me.btnClick.Text = "Search!"
        Me.btnClick.UseVisualStyleBackColor = True
        '
        'btnRequery
        '
        Me.btnRequery.Location = New System.Drawing.Point(852, 62)
        Me.btnRequery.Margin = New System.Windows.Forms.Padding(4)
        Me.btnRequery.Name = "btnRequery"
        Me.btnRequery.Size = New System.Drawing.Size(126, 28)
        Me.btnRequery.TabIndex = 7
        Me.btnRequery.Text = "Requery TF2WH"
        Me.btnRequery.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(13, 45)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(160, 17)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Autocheck interval (min)"
        '
        'cbxHideIE
        '
        Me.cbxHideIE.AutoSize = True
        Me.cbxHideIE.Location = New System.Drawing.Point(271, 21)
        Me.cbxHideIE.Name = "cbxHideIE"
        Me.cbxHideIE.Size = New System.Drawing.Size(216, 21)
        Me.cbxHideIE.TabIndex = 9
        Me.cbxHideIE.Text = "Hide Internet Explorer window"
        Me.cbxHideIE.UseVisualStyleBackColor = True
        '
        'cbxAutoOn
        '
        Me.cbxAutoOn.AutoSize = True
        Me.cbxAutoOn.Location = New System.Drawing.Point(16, 21)
        Me.cbxAutoOn.Name = "cbxAutoOn"
        Me.cbxAutoOn.Size = New System.Drawing.Size(144, 21)
        Me.cbxAutoOn.TabIndex = 10
        Me.cbxAutoOn.Text = "Enable Autocheck"
        Me.cbxAutoOn.UseVisualStyleBackColor = True
        '
        'txtAutoInterval
        '
        Me.txtAutoInterval.Location = New System.Drawing.Point(16, 65)
        Me.txtAutoInterval.Name = "txtAutoInterval"
        Me.txtAutoInterval.Size = New System.Drawing.Size(100, 22)
        Me.txtAutoInterval.TabIndex = 11
        Me.txtAutoInterval.Text = "5"
        '
        'autoCheckWorker
        '
        Me.autoCheckWorker.WorkerReportsProgress = True
        '
        'gbxAutoSearchOptions
        '
        Me.gbxAutoSearchOptions.Controls.Add(Me.cbxAutoOn)
        Me.gbxAutoSearchOptions.Controls.Add(Me.cbxHideIE)
        Me.gbxAutoSearchOptions.Controls.Add(Me.txtAutoInterval)
        Me.gbxAutoSearchOptions.Controls.Add(Me.Label3)
        Me.gbxAutoSearchOptions.Controls.Add(Me.btnClick)
        Me.gbxAutoSearchOptions.Controls.Add(Me.pbar)
        Me.gbxAutoSearchOptions.Controls.Add(Me.btnRequery)
        Me.gbxAutoSearchOptions.Location = New System.Drawing.Point(12, 445)
        Me.gbxAutoSearchOptions.Name = "gbxAutoSearchOptions"
        Me.gbxAutoSearchOptions.Size = New System.Drawing.Size(996, 130)
        Me.gbxAutoSearchOptions.TabIndex = 12
        Me.gbxAutoSearchOptions.TabStop = False
        Me.gbxAutoSearchOptions.Text = "Search Controls"
        '
        'pbar
        '
        Me.pbar.Location = New System.Drawing.Point(6, 101)
        Me.pbar.Name = "pbar"
        Me.pbar.Size = New System.Drawing.Size(972, 23)
        Me.pbar.TabIndex = 13
        '
        'lblAutoDisabled
        '
        Me.lblAutoDisabled.AutoSize = True
        Me.lblAutoDisabled.Location = New System.Drawing.Point(505, 9)
        Me.lblAutoDisabled.Name = "lblAutoDisabled"
        Me.lblAutoDisabled.Size = New System.Drawing.Size(372, 17)
        Me.lblAutoDisabled.TabIndex = 14
        Me.lblAutoDisabled.Text = "Auto-check is disabled when changing search parameters"
        '
        'searchWorker
        '
        '
        'gbxItemSets
        '
        Me.gbxItemSets.Location = New System.Drawing.Point(1027, 126)
        Me.gbxItemSets.Name = "gbxItemSets"
        Me.gbxItemSets.Size = New System.Drawing.Size(232, 443)
        Me.gbxItemSets.TabIndex = 14
        Me.gbxItemSets.TabStop = False
        Me.gbxItemSets.Text = "Item Sets"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 12)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(121, 17)
        Me.Label4.TabIndex = 15
        Me.Label4.Text = "Active Search List"
        '
        'dgvActiveTrades
        '
        Me.dgvActiveTrades.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.dgvActiveTrades.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvActiveTrades.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.clKeyword, Me.clLevels, Me.clCrafts, Me.clBuyerID64, Me.clOPTrade, Me.clNotes, Me.SearchMode})
        Me.dgvActiveTrades.ContextMenuStrip = Me.cmsItemsView
        Me.dgvActiveTrades.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.dgvActiveTrades.Location = New System.Drawing.Point(15, 33)
        Me.dgvActiveTrades.Name = "dgvActiveTrades"
        Me.dgvActiveTrades.RowTemplate.Height = 24
        Me.dgvActiveTrades.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvActiveTrades.Size = New System.Drawing.Size(993, 406)
        Me.dgvActiveTrades.TabIndex = 16
        '
        'clKeyword
        '
        Me.clKeyword.HeaderText = "Keyword"
        Me.clKeyword.MinimumWidth = 50
        Me.clKeyword.Name = "clKeyword"
        Me.clKeyword.Width = 150
        '
        'clLevels
        '
        Me.clLevels.HeaderText = "Levels"
        Me.clLevels.MinimumWidth = 50
        Me.clLevels.Name = "clLevels"
        Me.clLevels.Width = 150
        '
        'clCrafts
        '
        Me.clCrafts.HeaderText = "Craft #s"
        Me.clCrafts.MinimumWidth = 50
        Me.clCrafts.Name = "clCrafts"
        Me.clCrafts.Width = 150
        '
        'clBuyerID64
        '
        Me.clBuyerID64.HeaderText = "Buyer ID64"
        Me.clBuyerID64.MinimumWidth = 100
        Me.clBuyerID64.Name = "clBuyerID64"
        Me.clBuyerID64.Width = 150
        '
        'clOPTrade
        '
        Me.clOPTrade.HeaderText = "Outpost ID"
        Me.clOPTrade.MinimumWidth = 50
        Me.clOPTrade.Name = "clOPTrade"
        Me.clOPTrade.ToolTipText = "Double Click to go to trade"
        '
        'clNotes
        '
        Me.clNotes.HeaderText = "Notes"
        Me.clNotes.MinimumWidth = 50
        Me.clNotes.Name = "clNotes"
        Me.clNotes.Width = 150
        '
        'SearchMode
        '
        Me.SearchMode.HeaderText = "Search Mode"
        Me.SearchMode.Items.AddRange(New Object() {"Automatic", "Manual", "Disabled"})
        Me.SearchMode.Name = "SearchMode"
        '
        'cmsItemsView
        '
        Me.cmsItemsView.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GoToTF2OPTradeToolStripMenuItem, Me.GoToUserSteamPageToolStripMenuItem, Me.CopySearchResultsToolStripMenuItem, Me.OpenTF2WHPagesOfResultsToolStripMenuItem, Me.CompareResultsToABackpackToolStripMenuItem})
        Me.cmsItemsView.Name = "cmsItemsView"
        Me.cmsItemsView.Size = New System.Drawing.Size(315, 124)
        '
        'GoToTF2OPTradeToolStripMenuItem
        '
        Me.GoToTF2OPTradeToolStripMenuItem.Name = "GoToTF2OPTradeToolStripMenuItem"
        Me.GoToTF2OPTradeToolStripMenuItem.Size = New System.Drawing.Size(314, 24)
        Me.GoToTF2OPTradeToolStripMenuItem.Text = "Go to TF2OP trade"
        '
        'GoToUserSteamPageToolStripMenuItem
        '
        Me.GoToUserSteamPageToolStripMenuItem.Name = "GoToUserSteamPageToolStripMenuItem"
        Me.GoToUserSteamPageToolStripMenuItem.Size = New System.Drawing.Size(314, 24)
        Me.GoToUserSteamPageToolStripMenuItem.Text = "Go to user steam page"
        '
        'CopySearchResultsToolStripMenuItem
        '
        Me.CopySearchResultsToolStripMenuItem.Name = "CopySearchResultsToolStripMenuItem"
        Me.CopySearchResultsToolStripMenuItem.Size = New System.Drawing.Size(314, 24)
        Me.CopySearchResultsToolStripMenuItem.Text = "Copy search results"
        '
        'OpenTF2WHPagesOfResultsToolStripMenuItem
        '
        Me.OpenTF2WHPagesOfResultsToolStripMenuItem.Name = "OpenTF2WHPagesOfResultsToolStripMenuItem"
        Me.OpenTF2WHPagesOfResultsToolStripMenuItem.Size = New System.Drawing.Size(314, 24)
        Me.OpenTF2WHPagesOfResultsToolStripMenuItem.Text = "Open results' TF2WH pages (TODO)"
        '
        'CompareResultsToABackpackToolStripMenuItem
        '
        Me.CompareResultsToABackpackToolStripMenuItem.Name = "CompareResultsToABackpackToolStripMenuItem"
        Me.CompareResultsToABackpackToolStripMenuItem.Size = New System.Drawing.Size(314, 24)
        Me.CompareResultsToABackpackToolStripMenuItem.Text = "Compare to backpack"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.cbx_stgsSLSearches)
        Me.GroupBox2.Controls.Add(Me.cbx_StgsKeywordIgnoreCase)
        Me.GroupBox2.Controls.Add(Me.cbx_StgsHighlightSuccesses)
        Me.GroupBox2.Location = New System.Drawing.Point(1027, 12)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(235, 108)
        Me.GroupBox2.TabIndex = 17
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Program Options"
        '
        'cbx_stgsSLSearches
        '
        Me.cbx_stgsSLSearches.AutoSize = True
        Me.cbx_stgsSLSearches.Location = New System.Drawing.Point(7, 77)
        Me.cbx_stgsSLSearches.Name = "cbx_stgsSLSearches"
        Me.cbx_stgsSLSearches.Size = New System.Drawing.Size(191, 21)
        Me.cbx_stgsSLSearches.TabIndex = 3
        Me.cbx_stgsSLSearches.Text = "Save/Load past searches"
        Me.cbx_stgsSLSearches.UseVisualStyleBackColor = True
        '
        'cbx_StgsKeywordIgnoreCase
        '
        Me.cbx_StgsKeywordIgnoreCase.AutoSize = True
        Me.cbx_StgsKeywordIgnoreCase.Checked = True
        Me.cbx_StgsKeywordIgnoreCase.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbx_StgsKeywordIgnoreCase.Location = New System.Drawing.Point(7, 50)
        Me.cbx_StgsKeywordIgnoreCase.Name = "cbx_StgsKeywordIgnoreCase"
        Me.cbx_StgsKeywordIgnoreCase.Size = New System.Drawing.Size(160, 21)
        Me.cbx_StgsKeywordIgnoreCase.TabIndex = 1
        Me.cbx_StgsKeywordIgnoreCase.Text = "Ignore keyword case"
        Me.cbx_StgsKeywordIgnoreCase.UseVisualStyleBackColor = True
        '
        'cbx_StgsHighlightSuccesses
        '
        Me.cbx_StgsHighlightSuccesses.AutoSize = True
        Me.cbx_StgsHighlightSuccesses.Checked = True
        Me.cbx_StgsHighlightSuccesses.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbx_StgsHighlightSuccesses.Location = New System.Drawing.Point(7, 22)
        Me.cbx_StgsHighlightSuccesses.Name = "cbx_StgsHighlightSuccesses"
        Me.cbx_StgsHighlightSuccesses.Size = New System.Drawing.Size(217, 21)
        Me.cbx_StgsHighlightSuccesses.TabIndex = 0
        Me.cbx_StgsHighlightSuccesses.Text = "Highlight successful searches"
        Me.cbx_StgsHighlightSuccesses.UseVisualStyleBackColor = True
        '
        'lblLicense
        '
        Me.lblLicense.AutoSize = True
        Me.lblLicense.Location = New System.Drawing.Point(377, 582)
        Me.lblLicense.Name = "lblLicense"
        Me.lblLicense.Size = New System.Drawing.Size(435, 17)
        Me.lblLicense.TabIndex = 18
        Me.lblLicense.Text = " by Aessa Nassri. Copyright 2012. Released under the MIT License."
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1278, 606)
        Me.Controls.Add(Me.lblLicense)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lblAutoDisabled)
        Me.Controls.Add(Me.gbxItemSets)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.gbxAutoSearchOptions)
        Me.Controls.Add(Me.dgvActiveTrades)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "Form1"
        Me.Text = "Euphoria v"
        Me.gbxAutoSearchOptions.ResumeLayout(False)
        Me.gbxAutoSearchOptions.PerformLayout()
        CType(Me.dgvActiveTrades, System.ComponentModel.ISupportInitialize).EndInit()
        Me.cmsItemsView.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnClick As System.Windows.Forms.Button
    Friend WithEvents btnRequery As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents cbxHideIE As System.Windows.Forms.CheckBox
    Friend WithEvents cbxAutoOn As System.Windows.Forms.CheckBox
    Friend WithEvents txtAutoInterval As System.Windows.Forms.TextBox
    Friend WithEvents autoCheckWorker As System.ComponentModel.BackgroundWorker
    Friend WithEvents gbxAutoSearchOptions As System.Windows.Forms.GroupBox
    Friend WithEvents pbar As System.Windows.Forms.ProgressBar
    Friend WithEvents searchWorker As System.ComponentModel.BackgroundWorker
    Friend WithEvents gbxItemSets As System.Windows.Forms.GroupBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents dgvActiveTrades As System.Windows.Forms.DataGridView
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents cbx_stgsSLSearches As System.Windows.Forms.CheckBox
    Friend WithEvents cbx_StgsKeywordIgnoreCase As System.Windows.Forms.CheckBox
    Friend WithEvents cbx_StgsHighlightSuccesses As System.Windows.Forms.CheckBox
    Friend WithEvents cmsItemsView As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents GoToTF2OPTradeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GoToUserSteamPageToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CopySearchResultsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenTF2WHPagesOfResultsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CompareResultsToABackpackToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblAutoDisabled As System.Windows.Forms.Label
    Friend WithEvents lblLicense As System.Windows.Forms.Label
    Friend WithEvents clKeyword As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents clLevels As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents clCrafts As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents clBuyerID64 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents clOPTrade As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents clNotes As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents SearchMode As System.Windows.Forms.DataGridViewComboBoxColumn

End Class
