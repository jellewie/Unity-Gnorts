# https://github.com/jellewie/Unity-Gnorts

# This is an Real Time Strategy game in Unity
Although the Code is public for people to see, if you want to use it in any commercial way you need to contact me.
I do NOT allow you or annyone to use my codes and sell it without my permission.

If you want to help on this game, or create your own with it, just contact me and fork it.
v
![Early alpha screenshot of the game](https://i.imgur.com/PZfCLHh.png)


?Im not sure about this option?

MENU LAYOUT
====================
* __1 Battle__																																			//Change this to "Castle"?
* 1 Bridge for moat (only posible attached to a gate)
* 2 Moat
* 3 Mangonel_Tower (for on top of stone towers)
* 4 Balista_Tower (for on top of stone towers)
* 5 Town_Square (Boost happiness based on amount of homes near. Can have a party to temponary boost more)
* 6 Fire_Pit (This will make arrows in radkius X be on fire and do a little bit more dammage)(can only be build on stone structures)
* 7 Trap_Pit
* 8 Castle [C key] (Hide if already placed, can change tax for happiness) (Will hold 8 people)

* __2 Military__
* 1 Armory
* 2 Barracks (Creates soldiers, with option to buy items if an soldier is ordered but the items are missing)
* 3 Swords_Maker
* 4 Bow_Maker
* 5 Spear_Maker
* 6 Leather_Jacket_Maker
* 7 Blacksmith_Armor (Iron/steel jacket maker)
* 8 Blacksmith_Tools (Iron/steel Ace maker / Iron/steel Sword maker) 

* __3 Castle structures__																																//Change this to "Wall_Buildings"?
* 1 Wooden_Wall
* 2 Wooden_Wall_Spiked
* 3 Wooden_Gate			(Gates give a pop-up window to open/close the gate)
* 4 Wooden_Tower
* 5 Wooden_Stair
* 6 Stone_Wall
* 7 Stone_Wall_Spiked
* 8 Stone_Gate
* 9 Stone_Tower
* 10 Stone_Stair

* __4 Industial__
* 1 Stockpile [F key]
* 2 Lumberjack_Hut
* 3 Stone_Quarry
* 4 Iron_Mine
* 5 Ox_Transport (moves wood, stone, or iron/steel. This is an option, and buildings like the woodcutter do have an option to move them thereself)
* 6 Repair_Building (repairs structures for a little of the original cost, needs to be quite op compaired to real live, I dont want this to be a big focus point in the game)
* 7 
* 8 

* __5 Food__
* 1 Granary [G key]
* 2 Apple_Farm
* 3 Cow_Farm
* 4 Hunter (Hunters can shoor ground animals (Deers, Rabbits, wolfs, Bears)
* 5 Wheat_Farm
* 6 Mill (for wheat/grain)
* 7 Baker (for flour)
* 8 Fischer

* __6 MISC__
* 1 Home (Closer to the castle = bigger home??)
* 2 Trading_House [T key] (This building enables trading + Auto trading ?with an upgrade?; like buy wood if below 100, sell if above 200)
* 3 Church [H key] (Boost happiness based on amount of homes near. can be bribed to temponary boost more)
* 4 Water_Well (exsinquise fire. ?and makes people more happy?)
* 5 Alchemist
* 6 
* 7 
* 8 

* __7 REMOVE TOOL_
ITEMS
=====

* __Items__
* Wood
* Stone
* Iron/Steel
* __Food__
* Apples
* Cheese
* Wheat/grain
* -Flour
* --Bread
* Meat
* __Tools__
* Bow
* Speer
* Crosbow
* Leather armor
* Iron armor
and more...

KEYBOARD LAYOUT
=====

__QturnL__ __Qforward__ __EturnR__ __Rotatebuilding__ __Trading__ y u i o __Pause__

__Aleft__ __Sback__ __Dright__ __Fstockpile__ __Granary__ __cHurch__ j k l 

z x __Castle__ v __Barracks__ n m










__Woodcutter __
2x3 with a 2*2 building and a 2 wide wood storage next to it
When clicked on a option 'move by Oz' can be clicked (this also sets the new place default). This will disable the woodcutter itzelf from moving wood to the stockpile, and store a little wood (1 load) at the woodcutter building that can be moved by Ox.
AI to the building, 4x get a log (stored in building). Then saw the logs, then either place then on the side of the building, or walk them to the stockpile  
When no exist and no planks in stock this building is free (probably to hard to exploit?).
Production, the woodcutter will produce 5 planks for each log (thus 20 planks per run)

__Ox tether__
2*2 with a 2 wide for the storage and the rest for the ox. 
When clicked it can be set to move; wood or stone or iron.
When (first of this strike is) placed its set the the closest building that produces one of those (if <20m) else it's zet to wood. (this also really needs to be in the hints) 

__Stone quarry__
5*5?
Like the woodcutter with options but than with stone

__Iron Mine__
4*4?
Like the woodcutter with options but than with iron bars
(This building smelts the stuff to iron bars, wich is just called "Iron" for simplicity sake) 


__Trees__ are 1*1 and spread slowly, big one drops 3 logs

__Remove building__
When building is removed and not used before (used tag is set when the fist NPC has been there) the return cost is 100%.
Else it's 50%? Also it will move 75% of the stock to the stockpile and 90% of what the NPC is carrying if possible. 


__Still to think about__
How to go about estates? 
How do you expand estates? 
What happens to his/her estate when you kill a enemy? 
