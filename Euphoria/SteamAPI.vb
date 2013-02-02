Imports System.Net
Imports System.Text.RegularExpressions
Imports System.IO

Public Class SteamAPI

    ' STEAM API DETAILS
    ' Key: EF64585D95B0B771ABEE56A94E3816F8
    ' Domain Name: notawebsite

    ' GRAB BACKPACK: http://api.steampowered.com/ITFItems_440/GetPlayerItems/v0001/?key=EF64585D95B0B771ABEE56A94E3816F8&format=json&SteamID=76561198002208943
    ' GRAB SCHEMA: http://api.steampowered.com/ITFItems_440/GetSchema/v0001/?key=EF64585D95B0B771ABEE56A94E3816F8&format=json&SteamID=76561198002208943

    ' Get online/offline status: GetPlayerSummaries (v0002)
    ' AKA http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=XXXXXXXXXXXXXXXXXXXXXXX&steamids=id1,id2, - list is comma delimited and can take up to 100 ids

    Private Shared wc1 As New WebClient

    ' Downloads and data-mines an updated version of Valve's item schema
    '      Run this whenever Valve adds new items to TF2
    Public Shared Sub DownloadSchema()

        ' ----- Get item schema from Valve -----
        ' Downloads an updated item schema from Valve
        Dim rawSchema As String = wc1.DownloadString("http://api.steampowered.com/ITFItems_440/GetSchema/v0001/?key=EF64585D95B0B771ABEE56A94E3816F8&format=json&SteamID=76561198002208943")
        rawSchema = Regex.Replace(rawSchema, "\t{1,}", " ")

        ' Isolate item data
        ' REGEX: "name": ".+",\s+"defindex":.+
        Dim Q As String = Chr(34)
        Dim itemMatches As MatchCollection = Regex.Matches(rawSchema, Q + "name" + Q + ": " + Q + ".+" + Q + ",\s+" + Q + "defindex" + Q + ":.+", RegexOptions.Multiline)

        ' Convert into a key-value system
        Dim itemDefs As New List(Of Integer)
        Dim itemNames As New List(Of String)

        For i = 0 To itemMatches.Count - 1

            ' Get name
            Dim name As String = Regex.Match(itemMatches.Item(i).Value, ": " + Q + ".+" + Q).Value.Replace(Q, "").Remove(0, 2)

            ' Get item index (defindex)
            Dim defIndex As String = Regex.Match(itemMatches.Item(i).Value, "defindex" + Q + ":\s\d+,").Value.Remove(0, 11)
            defIndex = defIndex.Remove(defIndex.Length - 1) ' Remove ending comma

            ' Add data to proper lists
            itemNames.Add(name)
            itemDefs.Add(Integer.Parse(defIndex))

        Next

        ' Save schema to a file
        Dim sw As New StreamWriter(Form1.MePath & "valve_schema_list.txt")
        For i = 0 To itemNames.Count - 1
            sw.WriteLine(itemNames.Item(i) & ";;" & itemDefs.Item(i))
        Next
        sw.Close()


        ' ||| TODO! |||
        ' VVV       VVV
        ' ----- Convert schema names to actual item names -----
        '      For hats, the two are USUALLY, but NOT ALWAYS, the same


    End Sub

    ' Loads the item schema into memory
    '   Uses a parallel array based key-value system
    'Public Shared Sub LoadSchemaFromFile()

    '    ' Check to make sure schema file exists; if it doesn't, try to download it
    '    If Not File.Exists(Form1.MePath & "valve_schema_list.txt") Then
    '        DownloadSchema()
    '    End If

    'End Sub

    ' Gets the backpack of a player and extracts the item indexes ("defindexes") from it
    '   NOTE: We don't need the item schema anymore - just the defindexes! (TF2WH URLs start with the relevant item's defindex - so we can just check which of those defindexes is/is not present in the backpack)
    '   NOTE 2: If quality doesn't matter, it should be set to -1
    Public Shared Function DownloadBackpack(ByVal PlayerID64 As String, ByVal WHDefIndexList As List(Of Integer), ByVal quality As String, ByVal levels As String(), ByVal Crafts As String()) As List(Of Boolean)

        ' Quotation mark
        Dim Q As String = Chr(34)

        ' Download player's backpack from Valve
        Dim backpackJson As String = wc1.DownloadString("http://api.steampowered.com/ITFItems_440/GetPlayerItems/v0001/?key=EF64585D95B0B771ABEE56A94E3816F8&format=json&SteamID=" + PlayerID64) _
                                     .Replace(vbCr, "")

        ' Remove attribute data from backpackJson
        '   If not removed, this data can cause problems
        backpackJson = Regex.Replace(backpackJson, Q + "attribute" + Q + ":\s\[(.|\n)+?\]", "", RegexOptions.Multiline)

        ' Check to make sure the query is valid
        If Not backpackJson.Contains(Q + "status" + Q + ": 1") Then

            ' Something went wrong
            Return New List(Of Boolean) ' Return null

        End If

        ' Misc. stuff
        quality = quality.ToLowerInvariant ' All the qualities are in lowercase anyways - this prevents "Vintage" or "Unusual" from screwing up the system
        Dim ItemExistsList As New List(Of Boolean) ' List that contains information on which items exist in the player's backpack

        ' Get regex matches
        Dim rgxDefIndexes As MatchCollection = Regex.Matches(backpackJson, Q + "defindex" + Q + ":\s\d+")
        Dim rgxLevels As MatchCollection = Regex.Matches(backpackJson, Q + "level" + Q + ":\s\d+")
        Dim rgxQualities As MatchCollection = Regex.Matches(backpackJson, Q + "quality" + Q + ":\s\d+")

        ' --- Extract data from regex matches ---
        Dim dataDefIndexes As New List(Of Integer)
        Dim dataLevels As New List(Of Integer)
        Dim dataQualities As New List(Of String)

        ' Extract defIndexes
        For Each M As Match In rgxDefIndexes
            dataDefIndexes.Add(Integer.Parse(M.Value.Substring(12)))
        Next

        ' Extract levels
        For Each M As Match In rgxLevels
            dataLevels.Add(Integer.Parse(M.Value.Substring(9)))
        Next

        ' Extract qualities
        For Each M As Match In rgxQualities
            dataQualities.Add(M.Value.Substring(11))
        Next

        ' --- Check for common items ---
        For i = 0 To WHDefIndexList.Count - 1

            Dim ItemExistsInBackpack As Boolean = False

            ' --- Check to see if the current item exists in the user's backpack ---
            Dim indexOfItemInRegexMatches As Integer = -1
            For j = 0 To dataQualities.Count - 1

                ' Equality checks
                If dataDefIndexes.Item(j) = WHDefIndexList.Item(i) Then ' Initial equality check (defIndex)

                    ' Equality check 1 (quality)
                    If quality.Length > 2 AndAlso dataQualities.Item(j) <> quality Then
                        Continue For ' Skip the item
                    End If

                    ' A few variable declarations (used in subsequent equality checks)
                    Dim ItemLevelCraftIsValid As Boolean = False
                    Dim ItemLevel As Integer = dataLevels.Item(j)
                    Dim ItemCraft As Integer = -1 ' THIS DOES NOT WORK YET!

                    ' DBG NOTIFIER
                    If Crafts.Length <> 0 Then
                        'MsgBox("SteamAPI.DownloadBackpack() does not currently search for items based on craft number.")
                    End If

                    ' Equality check 2 (level)
                    If levels.Length > 0 Then
                        For Each Cond As String In levels

                            If Form1.EvalCondition(Cond, ItemLevel) Then

                                ' Item passed a condition
                                '   Note that items must have EITHER a matching level OR a matching craft, but NOT BOTH to be picked up by the search system
                                ItemExistsInBackpack = True

                            End If

                        Next
                    End If

                    ' Equality check 3 (craft #)
                    'If Not ItemExistsInBackpack AndAlso crafts.Length > 0 Then ' No point in checking the backpack if the item is already known to exist
                    '    For Each Cond As String In crafts

                    '        If Form1.EvalCondition(Cond, ItemLevel) Then

                    '            ' Item passed a condition
                    '            '   Note that items must have EITHER a matching level OR a matching craft, but NOT BOTH to be picked up by the search system
                    '            ItemExistsInBackpack = True

                    '        End If

                    '    Next
                    'End If

                    ' If item has passed all the equality checks, stop searching the backpack for that defIndex
                    If ItemExistsInBackpack Then
                        Exit For
                    End If

                End If
            Next

            ' Add result to output list
            ItemExistsList.Add(ItemExistsInBackpack)

        Next

        ' --- Return the item existence list ---
        Return ItemExistsList

    End Function

    ' Searches the backpack of a player

End Class
