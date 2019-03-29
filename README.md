Battle Tracker D20
Originally built by Tyelor D. Klein - November 2018

Battle Tracker:
This is a small tool built for tracking initative and battle order in table top roleplaying games like Pathfinder and D&amp;D 3.5e

Weather Calendar:
This is a small tool built for tracking dates, notes, and generating weather in table top roleplaying games like Pathfinder and D&amp;D 3.5e. This specific build is for Paizo's Pathfinder world Golarion.


How to use Battle Tracker:
To use the tool input the name, initiative, initiative modifier, and optionally the health and any additional notes (such as poison, curses, etc.), then click the "Sort List" button in order to get the initiative order and start the first turn timer.

The current combatant is shown in green on the data grid, and the combatant up next is shown in blue.

To go to the next player in the initiative order click the "Next Turn" button.

The number next to the "Next Turn" button is the Round Counter, and will track the total number of rounds since the list was first sorted. You can adjust this number manually, and with the up and down arrows.

To pause the turn timer click the turn timer text or near it.

To move a combatant in the intiative order click and drag that combatant's row.

To quickly insert a combatant entry hit either "Ctrl + A" or the "Insert" button on your keyboard. This will create a new entry under the currently selected entry (or the bottom if none are selected).

To duplicate a combatant entry click the combatant's row and then hit either "Ctrl + D" or "Ctrl + C" on your keyboard. Note if the combatant has a number at the end of their name, i.e "Combatant 1", then when you duplicate that entry the number will automatically be incrememented by 1.

To delete a combatant entry click that combatant's row and hit either the "Delete" or "Backspace" buttons on your keyboard.

To manually save or load a battle click the "File" button on the menu bar, and then choose "Save Battle..." or "Load Battle...". Note that the current battle will automatically be saved to "Battles\LastBattle.xml", and will create the Battles directory in the current directory if it doesn't already exist. (In the future I'll likely make the directory and file name customizable by the user in case they want to have it saved somewhere other than where the program is currently located.)

The turn timer length is currently only customizable through manually editing the "Battle Tracker.exe.xml" config file, and then changing the value under the settings key "TurnLength" from the default of 60 to whatever you'd like. Note that the timer counts one to zero making the default of 60 seconds actually 61 seconds, so subtract one from this number if you'd like your timer to be exactly 60 seconds. (In the future I may change it so that a TurnLength of 60 would make each turn 60 seconds not 61 seconds.)

To open the Weather Calendar click the "Weather/Note Calendar..." button on the menu bar.

How to use Weather Calendar:
To use the tool use the Advance Day buttons to change the current day, select dates on the calendar to view that date and enter notes or generate weather for the selected day.

Right click on the Advance Day buttons to decrease the current day by that button's amount.

Use the Generate Weather buttons to generate weather for the select day and on. Currently the weather is based on a colder northern climate, but can be modified in code to generate different climates (I will be making this easier in future iterations).

Right click on the Generate Weather buttons to clear the weather from the selected day and the next days based on the button's amount.

Type into the general notes box in order to keep track of notes that you want to persist from day to day.

Type into the daily notes box to change the notes for the selected day.

Hover the mouse over the weather in order to see the penalties that the creatures exposed to the weather will incur (the penalties shouldn't stack, use the worse penalty from any weather element's penalty).

Use the menu buttons to Save and Load specific Calendars. Note that the currently opened calendar will automatically be saved on exiting the application, and the last opened calendar will automatically be loaded upon starting the application.

To change the calendar to fit a different world requires changing several static fields throughout the application, and in the future customization will be much easier.


Updates:
Version 1.2.0:
Added Weather/Note Calendar for Paizo's Pathfinder Golarion play setting.

Version 1.1.0:
Added quick insert of combatant entries using Ctrl + A or the Insert button on your keyboard, your choice (insert no longer duplicates, see below).

Changed duplicate entry hotkey to Ctrl + D or Ctrl + C , your choice (duplicate functionality remains the same as before).

Changed how entries are deleted to be smarter, and will automatically select the element above the deleted one if deleted from the bottom.

Added delete hotkey, now you can use either Delete or Backspace to delete entries.

Added a new icon to the program (hopefully a much better one).

Fixed many small bugs and edge cases, such as moving an element to the very top of a list, and selecting the moved element.

Version 1.0.0:
Added project to GitHub.


Credits:
Xceed for their Extended WPF Toolkit, used under MS License. GitHub: https://github.com/xceedsoftware/wpftoolkit

Philipp Sumi for their Drag and Drop example for WPF Data Grids. Website: http://www.hardcodet.net/2009/03/moving-data-grid-rows-using-drag-and-drop




