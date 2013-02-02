Imports System.Windows.Forms
Imports System.Text.RegularExpressions

Public Class Dialog1

    Public Shared ActiveSearchStruct As WHSearchClass

    Private Sub cancelClick() Handles btnCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    ' Load the active search structure into the textboxes
    Private Sub LoadSearchStruct() Handles MyBase.Load

        ' Levels
        Dim LString As String = ""
        If ActiveSearchStruct.Levels IsNot Nothing Then ' Null check
            For Each L As String In ActiveSearchStruct.Levels
                LString &= L & ","
            Next
            LString = LString.Remove(LString.Length - 1)
        End If

        ' Craft #s
        Dim CString As String = ""
        If ActiveSearchStruct.Crafts IsNot Nothing Then ' Null check
            For Each C As String In ActiveSearchStruct.Crafts
                CString &= C & ","
            Next
            CString = CString.Remove(CString.Length - 1)
        End If

        ' Update textboxes
        txtLevels.Text = LString
        txtCrafts.Text = CString
        txtKeyword.Text = ActiveSearchStruct.Keyword

    End Sub

    ' Update the search structure with the data from the textboxes
    Private Sub UpdateSearchStruct()

        ' Validate input data
        Dim EndErrorText As String = "text is incorrectly formatted. No update has been performed."
        If Not WHSearchClass.validateLevelsOrCrafts(txtLevels.Text) Then
            MsgBox("The levels " + EndErrorText)
            Exit Sub
        ElseIf Not WHSearchClass.validateLevelsOrCrafts(txtCrafts.Text) Then
            MsgBox("The crafts " + EndErrorText)
            Exit Sub
        End If

        ' Update search structure
        ActiveSearchStruct.Crafts = txtCrafts.Text.Split(","c)
        ActiveSearchStruct.Levels = txtLevels.Text.Split(","c)
        ActiveSearchStruct.Keyword = txtKeyword.Text

        ' Close the form
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

End Class
