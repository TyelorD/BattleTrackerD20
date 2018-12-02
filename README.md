Battle Tracker D20
Originally built by Tyelor D. Klein - November 2018

This is a small tool built for tracking initative and battle order in table top roleplaying games like Pathfinder and D&amp;D 3.5e

How to use:

To use the tool input the name, initiative, initiative modifier, and optionally the health and any additional notes (such as poison, curses, etc.), then click the "Sort List" button in order to get the initiative order and start the first turn timer.

The current combatant is shown in green on the data grid, and the combatant up next is shown in blue.

To go to the next player in the initiative order click the "Next Turn" button.

The number next to the "Next Turn" button is the Round Counter, and will track the total number of rounds since the list was first sorted. You can adjust this number manually, and with the up and down arrows.

To pause the turn timer click the turn timer text or near it.

To move a combatant in the intiative order click and drag that combatant's row.

To duplicate a combatant entry click the combatant's row and then hit the "Insert" button on your keyboard. Note if the combatant has a number at the end of their name, i.e "Combatant 1", then when you duplicate that entry the number will automatically be incrememented by 1.

To delete a combatant entry click that combatant's row and hit the "Delete" button on your keyboard.

To manually save or load a battle click the "File" button on the menu bar, and then choose "Save Battle..." or "Load Battle...". Note that the current battle will automatically be saved to "Battles\LastBattle.xml", and will create the Battles directory in the current directory if it doesn't already exist. (In the future I'll likely make the directory and file name customizable by the user in case they want to have it saved somewhere other than where the program is currently located.)

The turn timer length is currently only customizable through manually editing the "Battle Tracker.exe.xml" config file, and then changing the value under the settings key "TurnLength" from the default of 60 to whatever you'd like. Note that the timer counts one to zero making the default of 60 seconds actually 61 seconds, so subtract one from this number if you'd like your timer to be exactly 60 seconds. (In the future I may change it so that a TurnLength of 60 would make each turn 60 seconds not 61 seconds.)

Credits:
Xceed for their Extended WPF Toolkit. GitHub: https://github.com/xceedsoftware/wpftoolkit

Philipp Sumi for their Drag and Drop example for WPF Data Grids. Website: http://www.hardcodet.net/2009/03/moving-data-grid-rows-using-drag-and-drop




