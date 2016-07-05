# SpaceChallenge
Code for the SpaceChallenge game
App: AIM - Space Challenge
Developed for: ESA

Platforms: Android (4.0.3 and up) iOS (5.0 and up) (Ipad run the game, but not supported)
Bundle size(at 23/06/2016): ~250MB
Platform: Unity

Document written by: Michel Dijkstra (game dev)
Contact writer: michel@ldijkstra.nl

Document written version: 1.1
Gamebuild: 1.11

Required knowledge:
- Unity in order to work on this game.
- C# & Unity in order to add certain items to the game.
- Sketch & Adobe Illustrator to add new sprites to the game.
- Logic Pro X (or any other DAW) to add new music and sound effects to the game.

Install the code:

Install GitHub Desktop (mac) OR GitHub extensions (windows)
Log-in and get access to the accounts
Install Unity
Log-in with the pro version account
Connect the github project to the unity project

Upload the game:

iOS:
Get all licences ready to be able to upload the game and make changes in the app store.
Pull the newest version of the game on your iMac.
Open unity and build the game.
Open Xcode and load the new game.
Archive the game.
Upload the game from the archiver.

Android:
Get all licences ready to be able to upload the game and make changes in the app store.
Pull the newest version of the game on your windows computer.
Open unity and build the game.
Upload the game in the developer portal from google.


Adding/changing aspects to the game:

Make new building:
1.	Make new gameObject
2.	Give name
3.	Add sprite
a.	check if the sprite has the right pivot point (should be around X=0.5 ; Y=0.1
b.	Change sprite pixels per unit to 400
4.	Make it a pre-fab
5.	Change Canvas →  Buildbutton →  Buildmenu → builderMenu:
a.	Size + 1
b.	Drag new building prefab in the new slot
6.	Inspector of the new building prefab:
a.	Give it a name
b.	Choose size
c.	Price
d.	Level needed to build the first one
e.	List of tasks and the time they need
f.	List of times for the upgrades of this building
g.	If there is a mini game, uncheck the resourceBuilding boolean and give the name of the scene of the new minigame below
h.	If it’s a decoration, check the decoration boolean
7.	Change in inspector new building under DON’T CHANGE
a.	Give it a builderplacer
b.	Give it a circle collider

Add Quests: (a bit of coding knowledge needed)
1.	Go to Quests.cs script
2.	Open QuestRequirements
a.	For a new questline
i.	Change new int[3,10,10](default) to new int[4,10,10]
ii.	Go to next step
b.	Change the 0 to desired number at location of the new quest
c.	Check comments to see fetch results for numbers
3.	Open QuestRewards
a.	Go to same row as previous
b.	Change reward as needed (Money, Researchpoints)
c.	At location 5 (*,*,4), put in the dialog to trigger row (4 = nothing)
4.	If you want dialogs, check next step

Adding dialogs
1.	Go to Dialogs.cs scripts
2.	Change new string[16,10] (default) to [17,10]
3.	Add new string row
4.	Change first letter to character talking
a.	A = astronaut
b.	F = future people
5.	Add dialogs (9 max)

Add new levels for player
1.	Account →  Exp needed for level → size + new levels, input into empty field the new exp needed
2.	Change new buildings numbers if you added new buildings after that level as well
3.	Change Badge UI
4.	Added new badges for each new level
5.	Canvas →  level →  badgescreen →  allbadges →  copy a badge, paste it into the same path and change the icon
6.	Go 2 paths back to Case
7.	Add 1 to the size and add the new badge icon with border

 

Style Guide v0.3


