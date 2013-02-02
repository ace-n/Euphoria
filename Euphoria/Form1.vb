Imports SHDocVw
Imports System.IO
Imports System.Text.RegularExpressions

Public Class Form1

    Declare Function Sleep Lib "kernel32" (ByVal dwMilliseconds As Integer) As Integer

    ' Version
    Public Version As Integer = 2

    ' Settings file name
    Public SettingsFile As String = "appsettings.txt"

    ' Base URLs
    Public OPBaseURL As String = "http://www.tf2outpost.com" ' TF2 Outpost

    ' List of results
    Public ResultsList As New List(Of List(Of String))

    ' List of active search structures
    Public ActiveSearchStructsList As New List(Of WHSearchClass)

    ' Auto-check values
    Public LastAutoCheckTime As Integer = My.Computer.Clock.TickCount
    Public AutoCheckInterval As Integer = 0
    Public CheckIsAuto As Boolean = False
    Public LastSearchTimestamp As Date ' Used so the user knows at what time the item was found (if they were away and something was found, thus stopping the program w/a dialog box)

    ' Indicates if selected ranges have changed
    Public SelectedRangesChanged As Boolean = False

    ' Prevents concurrent search requests
    Public SearchIsBusy As Boolean = False

    ' IE Browser instance
    Public WithEvents IEObj As New InternetExplorer

    ' List of index ranges corresponding to each item set
    Public SetRanges As New List(Of Point)

    ' List of checkboxes corresponding to each item set
    Public SetCbxes As New List(Of CheckBox)

    ' Item list (IL prefix = item list)
    Public ILNames, ILwhIds As New List(Of String) ' Data from the local system
    Public ILLevels, ILCraftNums As New List(Of List(Of Integer)) ' Data from the web

    ' Name of file with item links
    Public FileName As String = "TF2 HATS.txt"

    ' Current file location
    Public MePath As String

    ' List of stored trades
    Public StoredTradeFPath As String

    ' Misc. vars
    Public DoOnlineSearch As Boolean = True
    Private Sub Loader() Handles MyBase.Load

        ' Auto-version title (this is from ContexType, another project of mine)
        Me.Text &= CStr(Version / 100 + 1)

        ' Initial GUI functions
        lblAutoDisabled.Location = New Point(384, 502)
        lblLicense.Text = Me.Text & lblLicense.Text

        ' Update executable location (MePath) - ADD Path.GetDirectoryName()!
        MePath = Process.GetCurrentProcess.MainModule.FileName
        MePath = MePath.Remove(MePath.LastIndexOf("\") + 1)

        ' Update stored trade filepath
        StoredTradeFPath = MePath + "storedtrades.txt"

        ' Load settings
        Settings.QuerySettingsFile()

        ' Load trades, if appropriate
        If cbx_stgsSLSearches.Checked Then
            LoadTradesOnOpen()
        End If

        ' -- Load item data --
        Dim LPath1 As String = MePath + FileName

        ' Kudos to http://www.dreamincode.net/forums/topic/69080-desktop-directory/
        Dim LPath2 As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\" + FileName

        ' Check to make sure URL list exists
        Dim ListPath As String = ""
        If File.Exists(LPath2) Then
            ListPath = LPath2
        ElseIf File.Exists(LPath1) Then
            ListPath = LPath1
        End If

        ' If URL list can't be found, report error
        If ListPath.Length < 2 Then
            MsgBox("No TF2WH URL list found. File should be called " + FileName + " and be either on the desktop or this program's launch directory.")
            Me.Close()
        End If

        ' Create starting checkbox
        Dim newCbx As New CheckBox
        newCbx.Width = gbxItemSets.Width - 20
        newCbx.Location = New Point(10, 25)
        newCbx.Visible = True

        ' Add starting checkbox to checkbox lists
        SetCbxes.Add(newCbx)
        gbxItemSets.Controls.Add(newCbx)

        ' Selection range start/end values
        Dim RngStart As Integer = 0
        Dim RngEnd As Integer = 0

        ' Line counter
        Dim Cnt As Integer = 0

        Dim Sr As New StreamReader(ListPath)
        While Sr.Peek <> -1

            ' Get line
            Dim SLine2 As String = Sr.ReadLine ' DBG
            Dim SLine As String = SLine2.Trim
            Cnt += 1

            ' Remove any comments
            Dim Idx As Integer = SLine.IndexOf("//")
            Dim Comment As String = ""
            If Idx > 0 Then ' Remove the part of line that is a comment
                Comment = SLine.Remove(0, Idx + 2).Trim
                SLine = SLine.Remove(Idx).Trim
            ElseIf Idx = 0 Then ' Entire line is a comment

                Dim cbxObj As CheckBox = SetCbxes.Last

                ' If a checkbox has just been added to the list, assign a name to it
                If cbxObj.Text.Length = 0 Then
                    cbxObj.Text = SLine.Remove(0, 3)
                End If

                ' Add null value to lists
                ILNames.Add("")
                ILwhIds.Add("")

                Continue While
            End If

            If SLine.Length < 1 OrElse String.IsNullOrWhiteSpace(SLine) Then

                ' This line (probably) indicates the start of a new section
                If Chr(Sr.Peek) = "/" Then ' Nexr line is most likely a comment (either that or incorrectly formatted)

                    ' Add item index range to range list
                    RngEnd = Cnt
                    SetRanges.Add(New Point(RngStart, RngEnd))
                    RngStart = Cnt + 1

                    ' Add checkbox control to panel
                    newCbx = New CheckBox
                    newCbx.Visible = True
                    newCbx.Width = gbxItemSets.Width - 20
                    newCbx.Location = New Point(10, (SetCbxes.Count + 1) * 25)
                    AddHandler newCbx.CheckedChanged, AddressOf selectedRangesChgd

                    SetCbxes.Add(newCbx)
                    gbxItemSets.Controls.Add(newCbx)

                End If

                ' Add null value to lists
                ILNames.Add("")
                ILwhIds.Add("")

                Continue While
            End If

            ' Add item to lists
            ILNames.Add(Comment)
            ILwhIds.Add(SLine)

        End While

        ' Add last set range to list
        SetRanges.Add(New Point(RngStart, ILwhIds.Count - 1))

        ' Run auto-checking reminder worker
        autoCheckWorker.RunWorkerAsync()

        ' Continue
        IEObj.Navigate("http://www.tf2wh.com/item.php?login")

        ' -- Log-in process --
        ' Handle log-in process
        IEObj.Visible = True
        If MsgBox("Please log-in to tf2wh.com. Click OK AFTER you are logged in. Click CANCEL to close the program.", MsgBoxStyle.SystemModal + MsgBoxStyle.OkCancel, "Euphoria") = MsgBoxResult.Cancel Then
            Me.Close()
        End If

    End Sub

    Private Sub OnClosingHandler() Handles Me.FormClosing

        ' Attempt to kill the IE window - if this fails, close the form anyway
        Try
            IEObj.Quit()
        Catch
        End Try

        ' Save settings
        Settings.UpdateSettingsFile()

    End Sub

    ' Cache update
    Private Function UpdateCache() As Boolean Handles searchWorker.ProgressChanged

        ' -- End input verification --
        Try
            pbar.Maximum = ILwhIds.Count - 1
        Catch
            MsgBox("DERP!")
        End Try

        ' -- Query TF2WH (if required) --
        Dim ItemsQueried As Integer = 0 ' Moved outside the DoOnlineSearch if for DBG purposes
        If DoOnlineSearch Then

            SelectedRangesChanged = False

            ' Clear cached data
            ILLevels.Clear()
            ILCraftNums.Clear()

            ' Update progress bar's max value
            Dim TotalItemsChecked As Integer = 0
            For j = 0 To SetRanges.Count - 1
                If SetCbxes.Item(j).Checked Then
                    TotalItemsChecked += SetRanges.Item(j).Y - SetRanges.Item(j).X
                End If
            Next
            If TotalItemsChecked = 0 Then
                pbar.Maximum = ILwhIds.Count ' Use if no set ranges exist
            Else
                pbar.Maximum = TotalItemsChecked
            End If

            ' Start querying
            For i = 0 To ILwhIds.Count - 1

                ' Get WH ID
                Dim whId As String = ILwhIds.Item(i)

                ' Skip out of range IDs
                Dim InRange As Boolean = SetRanges.Count = 0 ' If no set ranges have been declared, the value is ALWAYS IN RANGE
                For j = 0 To SetRanges.Count - 1
                    If SetRanges.Item(j).X <= i AndAlso SetRanges.Item(j).Y >= i AndAlso SetCbxes.Item(j).Checked Then
                        InRange = True
                        Exit For
                    End If
                Next
                If Not InRange Then

                    ' Add null values (to preserve list order)
                    ILLevels.Add(New List(Of Integer))
                    ILCraftNums.Add(New List(Of Integer))

                    Continue For

                End If

                ' Progress bar (if statement is a mandatory exception handler)
                If pbar.Maximum >= ItemsQueried Then
                    pbar.Value = ItemsQueried
                End If

                ' Skip null IDs
                ItemsQueried += 1 ' This is here because the item ranges include both null and valid values
                If whId.Length < 1 Then

                    ' Add null values (to preserve list order)
                    ILLevels.Add(New List(Of Integer))
                    ILCraftNums.Add(New List(Of Integer))

                    Continue For

                End If

                ' Navigate
                IEObj.Navigate("http://www.tf2wh.com/item.php?id=" & whId & "&specific=1")
                While IEObj.ReadyState < 4
                    Sleep(60)
                End While

                ' Get HTML
                Dim HTML As String = ""
                For j = 0 To 5
                    Try

                        ' Attempt to read HTML
                        HTML = IEObj.Document.Body.OuterHTML

                        ' Exit reading attempt loop if successful
                        Exit For

                    Catch ex As MissingMemberException

                        ' Wait a bit and retry the scan
                        Sleep(1000)

                    End Try
                Next

                ' Report an invalid scan
                If HTML.Length = 0 Then
                    MsgBox("TF2WH query failed. Scanning process will be skipped.")
                    Return False ' Report failure
                End If

                ' Get levels
                Dim RgxLevels As MatchCollection = Regex.Matches(HTML, ", Level \d+")
                Dim LstLevels As New List(Of Integer)
                For Each M As Match In RgxLevels

                    Dim Val As String = M.Value.Remove(0, 8)
                    If Not LstLevels.Contains(Integer.Parse(Val)) Then
                        LstLevels.Add(Integer.Parse(Val))
                    End If

                Next
                ILLevels.Add(LstLevels)

                ' Get craft numbers
                '   NOTE: These are unique to each instance of an item, so checking for repeats is unnecessary
                Dim RgxCraftNums As MatchCollection = Regex.Matches(HTML, "#\d+, Level")
                Dim LstCrafts As New List(Of Integer)
                For Each M As Match In RgxCraftNums
                    Dim MStr As String = M.Value.Remove(0, 1)
                    LstCrafts.Add(CInt(MStr.Remove(MStr.Length - 7)))
                Next
                ILCraftNums.Add(LstCrafts)

            Next

        End If

        ' Reset pBar
        pbar.Value = 0

        ' Search the cache
        'SearchCache(txtLevels.Text.Replace(";", " ").Replace(",", " ").Split(" "), _
        '            txtCrafts.Text.Replace(";", " ").Replace(",", " ").Split(" "))

        ' Return success
        Return True

    End Function

    ' Perform active searches
    Private Sub SearchAgain()

        ' List of results
        ResultsList.Clear()

        ' Number of items found so far
        Dim MatchCnt As Integer = 0

        ' Perform searches
        For i As Integer = 0 To ActiveSearchStructsList.Count - 1

            ' Get search object
            Dim SearchObj As WHSearchClass = ActiveSearchStructsList.Item(i)

            ' Skip disabled searches (always) and manual searches (if search was automatically triggered)
            If SearchObj.SearchMode = 0 OrElse (SearchObj.SearchMode = 1 AndAlso CheckIsAuto) Then
                ResultsList.Add(New List(Of String)) ' Keep the indexes of all lists involved in sync
                Continue For
            End If

            ' Perform search
            ResultsList.Add(SearchCache(SearchObj.Levels, SearchObj.Crafts, SearchObj.Keyword))
            MatchCnt += ResultsList.Last.Count ' Add match count of last search to the total match count

        Next

        ' Highlight the data grid
        HighlightDataGrid()

        ' Report results
        If MatchCnt > 0 Then
            MsgBox(MatchCnt & " searches have found matching items!")
        ElseIf Not CheckIsAuto Then

            ' Notify user that nothing was found if the search was user-initiated
            MsgBox("No searches found matching items.")

        End If

    End Sub

    Public Sub HighlightDataGrid()

        ' Highlight the successful searches
        If cbx_StgsHighlightSuccesses.Checked AndAlso dgvActiveTrades.Rows.Count - 1 = ActiveSearchStructsList.Count Then

            ' Clear highlighting
            dgvActiveTrades.BackgroundColor = Color.White

            For i = 0 To dgvActiveTrades.RowCount - 2 ' This is deliberately risky code - if this produces an error, it should be obvious (to the developer - remember this is private software!)

                ' Get active row
                Dim ActiveRow As DataGridViewRow = dgvActiveTrades.Rows(i)

                ' Highlight a match if its count is > 0
                If ResultsList.Count > i AndAlso ResultsList.Item(i).Count <> 0 Then ' Yay for short circuiting
                    ActiveRow.DefaultCellStyle.BackColor = Color.LimeGreen
                    Continue For
                End If

                ' Highlight invalid rows
                If Not WHSearchClass.validateLevelsOrCrafts(ActiveRow.Cells(1).EditedFormattedValue.ToString) Or Not WHSearchClass.validateLevelsOrCrafts(ActiveRow.Cells(2).EditedFormattedValue.ToString) Then

                    ' Error detected
                    ActiveRow.DefaultCellStyle.BackColor = Color.OrangeRed
                    Continue For

                End If

                ' Recolor the row (if nothing else has colored it)
                ActiveRow.DefaultCellStyle.BackColor = Color.White

            Next

        End If

    End Sub

    ' Search cache given parameters
    Public Function SearchCache(ByVal DesiredLevels As String(), ByVal DesiredCrafts As String(), Optional ByVal Keyword As String = "") As List(Of String)

        ' -- Find matching items --
        Dim OutputList As New List(Of String)

        ' Check for level/craft matches
        For i = 0 To ILLevels.Count - 1

            Dim LevelOutputStr As String = ""

            ' Skip nulls
            If ILLevels.Item(i).Count = 0 AndAlso ILCraftNums.Item(i).Count = 0 Then
                Continue For
            End If

            ' If keyword is valid, check that current hat's comment contains the keyword
            If Keyword.Length <> 0 Then
                If cbx_StgsKeywordIgnoreCase.Checked AndAlso ILNames.Item(i).IndexOf(Keyword, StringComparison.InvariantCultureIgnoreCase) = -1 Then
                    Continue For
                ElseIf Not cbx_StgsKeywordIgnoreCase.Checked AndAlso ILNames.Item(i).IndexOf(Keyword, StringComparison.InvariantCulture) = -1 Then
                    Continue For
                End If
            End If

            ' Levels
            For Each DesdLvl In DesiredLevels

                ' Skip null conditions
                If DesdLvl.Length < 1 Then
                    Continue For
                End If

                For Each AvailableLevel In ILLevels.Item(i)

                    If EvalCondition(DesdLvl, AvailableLevel) Then

                        ' Current level is valid - add it to output
                        If LevelOutputStr.Length = 0 Then
                            LevelOutputStr &= "level(s) "
                        Else
                            LevelOutputStr &= ", "
                        End If
                        LevelOutputStr &= AvailableLevel

                    End If

                Next
            Next

            ' Crafts
            Dim CraftOutputStr As String = ""
            For Each DesdCraft In DesiredCrafts

                ' Skip null conditions
                If DesdCraft.Length < 1 Then
                    Continue For
                End If

                For Each AvailableCraft In ILCraftNums.Item(i)

                    If EvalCondition(DesdCraft, AvailableCraft) Then

                        ' Current craft # is valid - add it to output
                        If CraftOutputStr.Length = 0 Then
                            CraftOutputStr &= "craft number(s) "
                        Else
                            CraftOutputStr &= ", "
                        End If
                        CraftOutputStr &= AvailableCraft

                    End If

                Next
            Next

            ' Final output
            If CraftOutputStr.Length <> 0 OrElse LevelOutputStr.Length <> 0 Then
                Dim FinalOutStr As String = "Item " & ILwhIds.Item(i) & " [" & ILNames.Item(i) & "] found with "

                If CraftOutputStr.Length <> 0 Then
                    FinalOutStr &= CraftOutputStr
                End If
                If LevelOutputStr.Length <> 0 Then
                    FinalOutStr &= LevelOutputStr
                End If

                ' Add to output list
                OutputList.Add(FinalOutStr)

            End If

        Next

        ' Mark search as complete
        DoOnlineSearch = False

        ' Update last (automatic) search timestamp
        LastSearchTimestamp = My.Computer.Clock.LocalTime

        ' Reset auto-check/search is-busy flags
        SearchIsBusy = False

        ' Return results of search
        Return OutputList

    End Function

    ' Function that evaluates conditions and determines if they are true/false
    '   NOTE: This DOES NOT validate incoming conditions!
    Public Function EvalCondition(ByVal Condition As String, ByVal Number As Integer) As Boolean

        If Char.IsNumber(Condition.First) Then

            ' Equal
            Return Condition = Number.ToString

        ElseIf Condition.First = "<"c Then

            ' Less than
            Return Number < CInt(Condition.Remove(0, 1))

        Else

            ' More than
            Return Number > CInt(Condition.Remove(0, 1))

        End If

    End Function

    Private Sub btnRequery_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRequery.Click
        DoOnlineSearch = True
    End Sub

    Private Sub autoChgd() Handles cbxAutoOn.CheckedChanged
        Label3.Enabled = cbxAutoOn.Checked
        txtAutoInterval.Enabled = cbxAutoOn.Checked
    End Sub

    Private Sub hideIeChgd() Handles cbxHideIE.CheckedChanged
        IEObj.Visible = Not cbxHideIE.Checked
    End Sub

    Private Sub selectedRangesChgd()
        SelectedRangesChanged = True
    End Sub

    ' Automatic checking system
    Private alt As Integer = 0
    Private Sub autoCheckWorker_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles autoCheckWorker.DoWork

        While True

            ' If auto-check is off, sleep a little and continue
            If Not cbxAutoOn.Checked OrElse AutoCheckInterval < 1 Then
                Sleep(200)
                Continue While
            End If

            ' Sleep appropriate amount of time
            Sleep(AutoCheckInterval * 60000)

            ' Perform auto-check (only if the auto-check checkbox is still selected)
            If Not CheckIsAuto AndAlso cbxAutoOn.Checked AndAlso gbxAutoSearchOptions.Visible Then ' CheckIsAuto resets if the search completes - if it hasn't completed, CheckIsAuto is TRUE and no second search request is made
                CheckIsAuto = True
                DoOnlineSearch = True
                If Not SearchIsBusy Then ' Prevent the user and the system from making concurrent search requests
                    SearchIsBusy = True

                    ' Search
                    alt = Math.Abs(alt - 1)
                    autoCheckWorker.ReportProgress(alt + 1) ' Trigger the search

                End If
            End If

        End While

    End Sub

    Private Sub txtAutoInterval_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtAutoInterval.TextChanged

        Dim Int As Integer
        If Integer.TryParse(txtAutoInterval.Text, Int) Then
            If Int > 0 Then
                AutoCheckInterval = Int
                txtAutoInterval.BackColor = Color.White
            Else
                txtAutoInterval.BackColor = Color.Red
            End If
        Else
            txtAutoInterval.BackColor = Color.Red
        End If

    End Sub

    Private Sub ConductSearch() Handles btnClick.Click, autoCheckWorker.ProgressChanged
        Try
            If (DoOnlineSearch AndAlso UpdateCache()) OrElse (Not DoOnlineSearch) Then ' Query WH, if necessary

                ' Conduct search
                Try
                    SearchAgain()
                Catch
                    If CheckIsAuto Then
                        MsgBox("Something went wrong with the local search process. (TF2 Warehouse was accessed successfully.)")
                    End If
                End Try

            End If
        Catch
            If Not CheckIsAuto Then
                MsgBox("Something went wrong with the TF2 Warehouse scanning process.")
            End If
        End Try

        ' Search complete
        SearchIsBusy = False
        CheckIsAuto = False

    End Sub

    ' Initialize new rows with default values
    Private Sub InitNewRow(ByVal sender As Object, ByVal e As DataGridViewRowsAddedEventArgs) Handles dgvActiveTrades.RowsAdded

        dgvActiveTrades.Rows(e.RowIndex).Cells(6).Value = "Manual"

    End Sub

    'Private Sub txtCrafts_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    '' Combine lines
    'Dim Lines As String() = txtCrafts.Lines
    'If Lines.Count > 1 Then
    '    Dim TotalString As String = ""
    '    If txtCrafts.Text.Contains(",") Then
    '        For Each S As String In Lines
    '            TotalString &= S
    '        Next
    '    Else
    '        For Each S As String In Lines
    '            TotalString &= S & ","
    '        Next
    '        TotalString = TotalString.Remove(TotalString.Length - 2) ' Remove last comma
    '    End If

    '    ' Update text
    '    txtCrafts.Text = TotalString

    'End If


    'End Sub

    'Private Sub btnNewSearch_Click() Handles btnNewSearch.Click

    '    ' Show new search creation dialog
    '    Dialog1.ActiveSearchStruct = New WHSearchClass ' Create new search
    '    Dim DlgResult As DialogResult = Dialog1.ShowDialog() ' Show dialog

    '    ' Add search to list of active searches, if appropriate
    '    If DlgResult = DialogResult.OK Then
    '        ActiveSearchStructsList.Add(Dialog1.ActiveSearchStruct)
    '    End If

    '    ' Update the list of items
    '    updateListOfActiveSearches()

    'End Sub

    ' Update DataGridView (with changes from the list of active searches)
    Private Sub updateDataGridView()

        ' Remove any null searches from the activeSearchStructs
        For i = ActiveSearchStructsList.Count - 1 To 0 Step -1
            If ActiveSearchStructsList.Item(i).IsNull() Then
                ActiveSearchStructsList.RemoveAt(i) ' This remove operation is OK (even within a list-dependent loop) because the for loop starts at the end of the array
            End If
        Next

        ' Add new rows, if necessary
        '   NOTE: There should be (number of items + 1) rows since some functions disregard the last row (on purpose - whenever the user edits a data grid, the last row is always empty)
        If ActiveSearchStructsList.Count + 1 > dgvActiveTrades.RowCount Then
            dgvActiveTrades.Rows.Add(ActiveSearchStructsList.Count + 1 - dgvActiveTrades.RowCount)
        End If

        ' Populate existing rows
        For i = 0 To ActiveSearchStructsList.Count - 1

            ' Get active row
            Dim ActiveRow As DataGridViewRow = dgvActiveTrades.Rows.Item(i)

            ' Get active WHSearchStruct
            Dim ActiveWHSS As WHSearchClass = ActiveSearchStructsList.Item(i)

            ' Add stuff to the active row
            ActiveRow.Cells(0).Value = ActiveWHSS.Keyword
            ActiveRow.Cells(1).Value = String.Join(",", ActiveWHSS.Levels)
            ActiveRow.Cells(2).Value = String.Join(",", ActiveWHSS.Crafts)
            ActiveRow.Cells(3).Value = ActiveWHSS.OutpostID
            ActiveRow.Cells(4).Value = ActiveWHSS.SteamID64
            ActiveRow.Cells(5).Value = ActiveWHSS.Referrer
            ActiveRow.Cells(6).Value = ActiveWHSS.SearchModeString

        Next

        ' Delete any unused rows in the dataGridView
        RemoveUnusedDGVRows()

    End Sub

    ' Update list of active searches (with changes from the data grid view)
    Private Sub updateListOfActiveSearches() Handles dgvActiveTrades.RowLeave, dgvActiveTrades.RowsRemoved, dgvActiveTrades.LostFocus, dgvActiveTrades.MouseLeave

        ' Clear ActiveSearchStructsList
        ActiveSearchStructsList.Clear()

        ' Remove empty rows
        RemoveUnusedDGVRows()

        For i As Integer = 0 To dgvActiveTrades.RowCount - 2 ' Skip the last row, because it will always be empty

            ' Update active searches list
            Dim ActiveSearchObj As New WHSearchClass
            Dim ActiveRow As DataGridViewRow = dgvActiveTrades.Rows.Item(i)

            ' Check if current row is valid - skip it if it isn't
            Dim Val1 As String = ActiveRow.Cells(1).EditedFormattedValue.ToString
            Dim Val2 As String = ActiveRow.Cells(2).EditedFormattedValue.ToString
            Dim Val3 As String = ActiveRow.Cells(3).EditedFormattedValue.ToString
            If Not WHSearchClass.validateLevelsOrCrafts(Val1) Or Not WHSearchClass.validateLevelsOrCrafts(Val2) OrElse Regex.Match(Val3, "\d+").Length <> Val3.Length Then

                ' Error detected - add null search object to list (to keep list in sync with other things like the dataGridView)
                ActiveSearchStructsList.Add(New WHSearchClass)
                Continue For

            End If

            ' Update search object
            Try
                ActiveSearchObj.Keyword = CustomMethods.dgvCellVal(ActiveRow.Cells(0))
            Catch
            End Try

            ActiveSearchObj.Levels = CustomMethods.NoNullSplit(Val1, ",")
            ActiveSearchObj.Crafts = CustomMethods.NoNullSplit(Val2, ",")
            ActiveSearchObj.SteamID64 = Val3
            ActiveSearchObj.OutpostID = CustomMethods.dgvCellVal(ActiveRow.Cells(4))
            ActiveSearchObj.Referrer = CustomMethods.dgvCellVal(ActiveRow.Cells(5))
            ActiveSearchObj.SearchMode = WHSearchClass.SearchModeToNumber(CustomMethods.dgvCellVal(ActiveRow.Cells(6)))

            ' Add search object to active search list
            ActiveSearchStructsList.Add(ActiveSearchObj)

        Next

    End Sub

    ' Serialize trades and output them to a file
    Private Sub SaveTradesOnClose() Handles Me.FormClosing

        Dim sWriter As New StreamWriter(StoredTradeFPath)

        ' If save/load functionality is disabled, don't save the active trades
        If Not cbx_stgsSLSearches.Checked Then
            Exit Sub
        End If

        ' Serialize non-null trades
        For i = 0 To ActiveSearchStructsList.Count - 1
            If Not ActiveSearchStructsList.Item(i).IsNull Then

                Try
                    sWriter.WriteLine(ActiveSearchStructsList.Item(i).Serialize())
                Catch
                End Try

            End If
        Next

        ' Write to file
        sWriter.Flush()

    End Sub

    ' Load serialized trades from file
    '   This is called by the main starting procedure
    Private Sub LoadTradesOnOpen()

        ' Check to make sure that trade file exists
        If Not File.Exists(StoredTradeFPath) Then
            MsgBox("No stored trades were loaded because none could be found.")
            Exit Sub
        End If

        ' Load trades from file
        Dim sReader As New StreamReader(StoredTradeFPath)
        While sReader.Peek <> -1

            Dim sLine As String = sReader.ReadLine

            ' Error message
            Dim ErrorMsg As String = "An invalid trade (" + sLine + ") was found in the stored trades file. Click OK to skip it, or CANCEL to stop loading trades."

            ' Some really simple validation - stronger validation is done in the trade object constructor
            If sLine.Length > 4 AndAlso Not sLine.Contains(";") Then ' 4 separators * 2 characters per separator = 8 characters minimum (assuming line has no meaning)
                If MsgBox(ErrorMsg, MsgBoxStyle.OkCancel) = MsgBoxResult.Cancel Then

                    ' An error was detected
                    '   Clear the loaded items and stop loading any more
                    ActiveSearchStructsList.Clear()
                    sReader.Close()
                    Exit Sub

                Else
                    Continue While ' Skip the item
                End If
            ElseIf sLine.Length < 5 Then ' Skip lines with 4 characters or less (including lines consisting entirely of separators)
                Continue While
            End If

            ' Enter new trade into database
            Try
                ActiveSearchStructsList.Add(New WHSearchClass(sLine))
            Catch e As ArgumentException

                If MsgBox(ErrorMsg, MsgBoxStyle.OkCancel) = MsgBoxResult.Cancel Then

                    ' Clear the loaded items and stop loading any more
                    ActiveSearchStructsList.Clear()
                    sReader.Close()
                    Exit Sub

                End If

            End Try

        End While

        ' Close the StreamReader
        sReader.Close()

        ' Update the dataGridView
        updateDataGridView()

    End Sub

    ' Context menu for the active trades
    Private Sub dgvContextMenuActivate(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dgvActiveTrades.CellMouseClick

        ' Skip if button isn't correct
        If e.Button <> MouseButtons.Right Then
            Return
        End If

        ' Highlight current row
        dgvActiveTrades.ClearSelection()
        dgvActiveTrades.Rows(e.RowIndex).Selected = True

        ' Display context menu
        cmsItemsView.Visible = True

    End Sub

    ' Delete any unused rows within the dataGridView
    Private Sub RemoveUnusedDGVRows()

        ' This method doesn't work - there is no nice way to remove empty rows (at least when the removal is triggered by event handlers)
        Exit Sub

        For i = dgvActiveTrades.RowCount - 2 To 0 Step -1

            ' Get active row
            Dim curRow As DataGridViewRow = dgvActiveTrades.Rows(i)

            ' Check to see if any cells are filled
            Dim RowIsEmpty As Boolean = True
            For j = 0 To curRow.Cells.Count - 1
                If Not String.IsNullOrWhiteSpace(curRow.Cells(j).EditedFormattedValue.ToString) Then
                    RowIsEmpty = False
                    Exit For
                End If
            Next

            ' Delete any empty rows
            If RowIsEmpty Then
                'dgvActiveTrades.Rows(i).Visible = False
            End If

        Next

    End Sub

#Region "Context menu for dataGridView"

    ' Hide the context menu if it (and not one of its buttons) is left clicked
    Private Sub hideCMS() Handles cmsItemsView.MouseClick
        cmsItemsView.Visible = False
    End Sub

    ' Go to outpost trade(s) of selected row(s)
    Private Sub goToOPTrade() Handles GoToTF2OPTradeToolStripMenuItem.Click

        Dim HasOpenedOPHomepage As Boolean = False

        ' Make sure at least one row is selected
        If dgvActiveTrades.SelectedRows.Count = 0 Then
            Exit Sub
        End If

        ' Loop through selected trades
        For Each Row As DataGridViewRow In dgvActiveTrades.SelectedRows

            ' Get current trade ID
            Dim CurTradeID As String = Row.Cells(3).EditedFormattedValue.ToString

            ' Handle null trade IDs
            '   If trade ID is null
            '       If the TF2OP homepage has been launched, don't launch another one
            '       Otherwise, launch a copy of the homepage
            If CurTradeID.Length = 0 OrElse String.IsNullOrWhiteSpace(CurTradeID) Then ' This SHOULD check for white spaces (not just 0 length strings)
                If HasOpenedOPHomepage Then
                    Continue For
                Else
                    HasOpenedOPHomepage = True
                    Process.Start(OPBaseURL)
                End If
            End If

            ' Validate trade IDs (check that they are #s)
            If Integer.TryParse(CurTradeID, New Integer) Then

                ' NOTE: If OPBaseURL ends in a / (such that "...tf2outpost.com//trade..." results), the browser will adjust for this automatically (so don't worry about it)
                Process.Start(OPBaseURL + "/trade/" + CurTradeID)

            End If

        Next

    End Sub

    ' Copy search results for selected row(s)
    Private Sub copySearchResults() Handles CopySearchResultsToolStripMenuItem.Click

        ' Make sure at least one row is selected and that it has valid results
        If dgvActiveTrades.SelectedRows.Count = 0 Then
            Exit Sub
        End If

        ' Loop through selected trades
        Dim ResultsStr As String = ""
        For i = 0 To dgvActiveTrades.Rows.Count - 1

            ' Get current row
            Dim curRow As DataGridViewRow = dgvActiveTrades.Rows(i)

            ' Skip the row if it isn't selected, or if it wasn't a successful search
            If Not curRow.Selected OrElse curRow.DefaultCellStyle.BackColor <> Color.LimeGreen Then
                Continue For
            End If

            ' Copy results from selected rows to the results string
            ResultsStr &= String.Join(vbCrLf, ResultsList.Item(i))

        Next

        ' Output results to clipboard if it is non-null
        If ResultsStr.Length <> 0 Then
            My.Computer.Clipboard.SetText(ResultsStr)
        End If

    End Sub

    ' Compare search results to buyer's backpack
    Private Sub CheckBackpack() Handles CompareResultsToABackpackToolStripMenuItem.Click

        ' Misc
        Dim NopeMsg As String = "Something went wrong. The comparison was not performed."

        ' Make sure at least one row is selected
        If dgvActiveTrades.SelectedRows.Count = 0 Then
            Exit Sub
        End If

        ' Get selected (active) row
        Dim ActiveRow As DataGridViewRow = dgvActiveTrades.SelectedRows.Item(0)

        ' Get index of selected row
        Dim RowIndex As Integer = ActiveRow.Index

        ' Get active search object
        '   This contains the level/quality conditions
        Dim SearchObj As WHSearchClass
        If RowIndex < ActiveSearchStructsList.Count Then
            SearchObj = ActiveSearchStructsList.Item(RowIndex)
        Else
            MsgBox(NopeMsg)
            Exit Sub ' No matching search structure
        End If

        ' Get a list of the defIndexes in the TF2WH results for this search
        Dim DefIdxList As New List(Of Integer)
        If RowIndex < ResultsList.Count Then

            ' Get defIndexes
            For Each UniqueItemID As String In ResultsList.Item(RowIndex)
                DefIdxList.Add(Integer.Parse(UniqueItemID.Remove(UniqueItemID.IndexOf(";")).Remove(0, 5)))
            Next

        Else
            MsgBox(NopeMsg)
            Exit Sub ' No matching resultsList entry
        End If

        ' Get player ID64
        Dim PlayerID64 As String = ActiveRow.Cells(3).EditedFormattedValue.ToString
        If PlayerID64.Length < 2 OrElse Regex.Match(PlayerID64, "\d+").Length <> PlayerID64.Length Then
            Exit Sub ' Skip invalid ID64s (a valid ID64 is required by the Steam API to determine whose backpack to fetch)
        End If

        ' Query the DownloadBackpack function
        Dim ItemExistsList As List(Of Boolean) = SteamAPI.DownloadBackpack(PlayerID64, DefIdxList, "", SearchObj.Levels, SearchObj.Crafts)

        ' Print the search results for the items the user doesn't have
        Dim OutputStr As String = ""
        If ItemExistsList.Contains(False) Then

            ' The user queried is missing at least 1 of the search results
            OutputStr = "This user DOES NOT have the following items from the search results list:"

            For i = 0 To ItemExistsList.Count - 1
                If Not ItemExistsList.Item(i) Then
                    OutputStr &= vbCrLf & ResultsList.Item(RowIndex).Item(i)
                End If
            Next

        ElseIf ItemExistsList.Count <> 0 Then

            ' The user queried has all of the search results
            OutputStr = "This user DOES have all the items in the search results list."

        Else

            ' An error occurred
            MsgBox("SteamAPI.DownloadBackpack(): An error has occurred. (PlayerID64=" + PlayerID64 + ")")


        End If

        ' Copy output to clipboard
        If ItemExistsList.Count <> 0 Then
            My.Computer.Clipboard.SetText(OutputStr)
        End If

        MsgBox("Backpack comparison complete")

    End Sub

    Private Sub GoToUserSteamPageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GoToUserSteamPageToolStripMenuItem.Click

        For Each ActiveRow As DataGridViewRow In dgvActiveTrades.SelectedRows

            ' Get buyer's ID64
            Dim ID64 As String = ActiveRow.Cells(3).EditedFormattedValue.ToString

            ' If buyer's ID64 is valid, launch the relevant Steam page
            If ID64.Length <> 0 AndAlso Regex.Match(ID64, "\d+").Length = ID64.Length Then
                Process.Start("www.steamcommunity.com/profiles/" + ID64)
            End If

        Next

    End Sub

    Private Sub OpenTF2WHPagesOfResultsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenTF2WHPagesOfResultsToolStripMenuItem.Click

    End Sub

#End Region

    Private Sub dgvMoveIn() Handles dgvActiveTrades.MouseEnter

        gbxAutoSearchOptions.Visible = False
        lblAutoDisabled.Visible = True

    End Sub
    Private Sub dgvMoveOut() Handles dgvActiveTrades.MouseLeave

        gbxAutoSearchOptions.Visible = True
        lblAutoDisabled.Visible = False

    End Sub

End Class

' Class to handle settings
Public Class Settings

    ' Boolean to integer converter
    Shared Function BoolToInt(ByVal Bool As Boolean) As Integer
        If Bool Then
            Return 1
        End If

        Return 0

    End Function

    ' Integer to boolean converter
    Shared Function IntToBool(ByVal Int As Integer) As Integer
        Return (Int = 1)
    End Function

    ' Reset available settings to default
    Shared Sub DefaultSettings()

        ' Global settings variables

        ' Application settings
        Form1.cbx_StgsHighlightSuccesses.Checked = True
        Form1.cbx_StgsKeywordIgnoreCase.Checked = True
        Form1.cbx_stgsSLSearches.Checked = True

    End Sub

    ' Query settings
    '   1=success, 0=no settings file found
    Shared Function QuerySettingsFile() As Integer

        ' Get settings file location
        Dim EPath As String = Process.GetCurrentProcess.MainModule.FileName
        EPath = EPath.Substring(0, Math.Max(EPath.LastIndexOf("\"), EPath.LastIndexOf("/"))) & "\" & Form1.SettingsFile

        ' If the settings file can't be found, silently exit this part (display notifications using calling code if appropriate)
        If Not File.Exists(EPath) Then
            Return 0
        End If

        ' Get file text
        Try

            Dim Reader As New IO.StreamReader(EPath)

            ' Read each line
            While Reader.Peek <> -1

                ' Get line and its value
                Dim SLine As String = Reader.ReadLine
                Dim SValue As String = SLine.Substring(SLine.IndexOf("=") + 1)

                ' Get boolean conversion (if integer)
                Dim Bool As Boolean = False
                If SValue = "0" Or SValue = "1" Then
                    Bool = BoolToInt(CInt(SValue))
                End If

                Try

                    ' ----- Settings querying -----
                    ' "Anti-setting" setting - disables use of stored settings
                    If SLine.StartsWith("UseStoredStgs=0") Then
                        Return 1
                    End If

                    ' Application settings booleans
                    If SLine.StartsWith("HighlightSuccesses=") Then
                        Form1.cbx_StgsHighlightSuccesses.Checked = Bool
                        Continue While
                    ElseIf SLine.StartsWith("KeywordIgnoreCase=") Then
                        Form1.cbx_StgsKeywordIgnoreCase.Checked = Bool
                        Continue While
                    ElseIf SLine.StartsWith("SaveLoadSearches=") Then
                        Form1.cbx_stgsSLSearches.Checked = Bool
                    End If

                    ' Automatic search frequency
                    If SLine.StartsWith("AutoScanFreq=") Then
                        Form1.txtAutoInterval.Text = SValue
                        Continue While
                    End If

                Catch
                End Try

            End While

            Reader.Close()

        Catch

            ' Query failed
            Return 0

        End Try

        ' Success!
        Return 1

    End Function

    ' Update settings
    Shared Function UpdateSettingsFile() As Integer

        Dim StrList As New List(Of String)

        ' --- Update settings ---

        ' Application settings
        StrList.Add("HighlightSuccesses=" & BoolToInt(Form1.cbx_StgsHighlightSuccesses.Checked))
        StrList.Add("KeywordIgnoreCase=" & BoolToInt(Form1.cbx_StgsKeywordIgnoreCase.Checked))
        StrList.Add("SaveLoadSearches=" & BoolToInt(Form1.cbx_stgsSLSearches.Checked))

        ' Automatic search frequency
        StrList.Add("AutoScanFreq=" & CStr(Form1.txtAutoInterval.Text))

        ' --- Write settings to file ---
        '   Try-catch in case file write is denied
        Try

            ' Get filepath
            Dim EPath As String = Process.GetCurrentProcess.MainModule.FileName ' Executable path
            EPath = EPath.Substring(0, Math.Max(EPath.LastIndexOf("\"), EPath.LastIndexOf("/"))) ' File executable is in

            ' Write to target file
            IO.File.WriteAllLines(EPath & "\" & Form1.SettingsFile, StrList)

            ' Report success
            Return 1

        Catch

            ' Report failure
            Return 0

        End Try

    End Function

End Class

' Custom methods
Public Class CustomMethods

    Public Shared Function NoNullSplit(ByVal Str As String, ByVal Delimiter As String) As String()

        Dim Null As String() = {}
        If String.IsNullOrWhiteSpace(Str) Then
            Return Null
        End If

        Return Str.Split(Delimiter)

    End Function

    Public Shared Function dgvCellVal(ByVal Cell As DataGridViewCell) As String

        Try
            Return Cell.EditedFormattedValue.ToString
        Catch
        End Try

    End Function

End Class