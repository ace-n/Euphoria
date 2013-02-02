Public Class WHSearchClass

    ' Search object
    Public Levels As String()
    Public Crafts As String()
    Public Keyword As String
    Public OutpostID As String
    Public Referrer As String
    Public SteamID64 As String
    Public SearchMode As Integer = 0

    ' SEARCH MODE GUIDE
    '   0 = Disabled
    '   1 = Manual
    '   2 = Automatic

    ' Serialization method
    Public Function Serialize() As String
        Return String.Join(",", Levels) & ";" & String.Join(",", Crafts) & ";" + Keyword & ";" + SteamID64 & ";" + Referrer + ";" & OutpostID + ";" & SearchMode
    End Function

    ' Deserialization method (a constructor)
    Public Sub New(ByVal ConstructorString As String)

        Dim SSplit As String() = ConstructorString.Split(";"c)

        ' Validation
        If SSplit.Length <> 7 OrElse Not validateLevelsOrCrafts(SSplit.GetValue(0).ToString) OrElse Not validateLevelsOrCrafts(SSplit.GetValue(1).ToString) Then
            Throw New ArgumentException("Invalid serialization of WHSearchClass.")
            Return
        End If

        ' Save data into new object
        Levels = CustomMethods.NoNullSplit(SSplit.GetValue(0).ToString, ",")
        Crafts = CustomMethods.NoNullSplit(SSplit.GetValue(1).ToString, ",")
        Keyword = SSplit.GetValue(2).ToString
        OutpostID = SSplit.GetValue(3).ToString
        Referrer = SSplit.GetValue(4).ToString
        SteamID64 = SSplit.GetValue(5).ToString
        Integer.TryParse(SSplit.GetValue(6), SearchMode)

    End Sub

    ' Parameter-supplied constructor
    Public Sub New(ByVal Levels As String, ByVal Crafts As String, ByVal Keyword As String, ByVal OutpostID As String, ByVal Referrer As String)

        ' Save data into new object
        Me.Levels = Levels.Split(","c)
        Me.Crafts = Crafts.Split(","c)
        Me.Keyword = Keyword
        Me.OutpostID = OutpostID
        Me.Referrer = Referrer

    End Sub

    ' Null constructor
    Public Sub New()
    End Sub

    ' --- Input validator ---
    Public Shared Function validateLevelsOrCrafts(ByVal Text As String) As Boolean

        ' Validate text
        Dim SSplit = Text.Split(","c)
        For Each Str As String In SSplit

            If System.Text.RegularExpressions.Regex.Match(Str, "(<|>|)\d+").Length <> Str.Length Then
                Return False ' Error spotted!
            End If

        Next

        ' No error spotted - return true
        Return True

    End Function

    ' Null checker
    Public Function IsNull() As Boolean

        Return Levels.Count = 0 AndAlso Crafts.Count = 0 AndAlso String.IsNullOrWhiteSpace(Keyword & OutpostID & Referrer) ' Concatenation saves space

    End Function

    ' Search mode type conversion (#1)
    Public Function SearchModeString() As String
        Select Case SearchMode

            Case 0
                Return "Disabled"
            Case 1
                Return "Manual"
            Case Else ' 2
                Return "Automatic"

        End Select
    End Function

    ' Search mode type conversion (#2)
    Public Shared Function SearchModeToNumber(ByVal SMString As String) As String

        SMString = SMString.ToLower
        Select Case SMString

            Case "automatic"
                Return 2
            Case "manual"
                Return 1
            Case Else
                Return 0

        End Select
    End Function

End Class
