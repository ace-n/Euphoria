Euphoria
========

A .NET application for finding uniquely-leveled/numbered Team Fortress 2 items on TF2WH.com.

<br><br><b>Getting started</b><br><br>
To start Euphoria, download and compile all the source code files. Make sure that the file called "TF2 HATS.txt" is in the same directory as the compiled .EXE. Then, run the .EXE.
<br><br>
When Euphoria ia opened, you should see one of two things.<pre>
A message asking you to log into TF2WH.com (Good - log-in and then click OK)
A message saying that the "TF2 HATS.txt" file cannot be found. Download the "TF2 HATS.txt" file from GitHub (this project) and place it in the same directory as the Euphoria executable
</pre>Once Euphoria is running, go to the Active Search List and click on the top row to make a new search.<br>
Each search consists of 7 parameters. If a particular parameter is not specified (i.e. its box is blank), then it will not affect the search
<pre>
Keyword - only item names that contain this value will be searched. (e.x. A keyword of "huntsman" would exclude any item without "huntsman" in their name).
Levels - a comma-separated list of levels to search for. Only items that have matching levels will be searched.
Craft #s - identical to levels, except this uses craft numbers.
Buyer ID64 - the Steam ID64 of the person buying/collecting the items.
Outpost ID - the numerical ID of a TF2 Outpost trade relevant to the search.
Notes - a place for additional information about the search.
Search mode - specifies when the search will be performed.
  Automatic - search will always be carried out
  Manual - search will only be carried out when the user clicks the "Search" button (i.e. it won't be carried out during automatic searching)
  Disabled - search will never be carried out
</pre>
<br><br>
Once you have your first search set up, check one or more of the checkboxes in the Item Sets section. If a given set is checked, the items it contains will be searched for; all other items will not be searched.<br><br>
When you're ready to search, go to the Search Controls section and click "Search". Euphoria will then conduct the specified search(es).
You will receive one of two messages:
<pre>
  No matching items were found
  X searches have found matching items [TODO]
</pre>
<br><br>
If you receive the latter and "Highlight successful searches" is checked in the options menu, some of the searches in the Active Search List will appear green. To retrieve the results of the search, right click on it. A context menu should appear with the following options:
<pre>
Go to TF2OP trade - directs a web browser to the TF2 Outpost Trade specified in the Outpost ID column. If this ID is invalid, it will go to TF2Outpost.com.
Go to user Steam page - directs a web browser to the Steam page of the user specified in the Steam ID64 column. If this ID is invalid, it will not do anything.
Copy search results - copies the search results to the clipboard
Open results' TF2WH pages - this doesn't work yet. When it does, it will open the TF2WH pages of the matching items.
Compare to backpack - compares the search results against the backpack of the player specified in the Steam ID64 column. Copies to the clipboard a list of found items that the user doesn't already have, if any.
</pre>
Note that this context menu and its commands can be used when any search is right clicked, regardless of whether or not the search found anything.

<br><br><b>The item list</b><br>
The item list is stored in the "TF2 HATS.txt" file. It is a list of TF2 Warehouse item IDs that tell Euphoria where to search. Its syntax is as follows.
<pre>
// - Comment
Comment above a list of item IDs - item set name
Comment after an item ID - item name (e.x. 123:456:789 // Random Item Name )
Item IDs - the TF2 Warehouse ID of the item to be searched. This can be found in the URL of the item's TF2WH page. (TF2WH URLs have the format "tf2wh.com/item.php?id=[Item ID here]")
</pre>

