# This is an Real Time Strategy game in Unity
Although the Code is public for people to see, if you want to use it in any commercial way you need to contact me.
I do NOT allow you or annyone to use my codes and sell it without my permission.

If you want to help on this game, or create your own with it, just contact me and fork it.


#Todo list
(just some ideas, and things that need to be done)




###1. UI interface placement 

* ~~Minimap~~ (Is made)
* Buildmenu
* Fight/command armor menu (just sums up the soliers you have selected)
* Stock overview  (wood, food, stone etc) 
* Move the settings button, add remove button, ?info button/ trade button in mp?
  
###2. Key to hide UI bar 

###3. Add prefab for buildings;

?Im not sure about this?

* __1 Battle__
* 1 Bridge for moat (only posible attached to a gate)
* 2 Moat
* 3 Mangonel tower top
* 4 Balista tower top
* 5 Town square (Boost happiness based on amount of homes near. Can have a party to temponary boost more)
* 6 Fire pit (This will make arrows in radkius X be on fire and do a little bit more dammage)
* 7 Trap pit
* 8 __C__astle (Hide if already placed, can change tax for happiness) 

* __2 Military__
* 1 Armory
* 2 Barracks (Creates soldiers, with option to buy items if an soldier is ordered but the items are missing)
* 3 Swords maker
* 4 Bow maker
* 5 Spear maker
* 6 Leather jacket maker
* 7 Blacksmith (Iron/steel jacket maker)
* 8 BlackSmith (Iron/steel Ace maker / Iron/steel Sword maker) 

* __3 Castle structures__
* 1 Wooden walls
* 2 Wooden gate
* 3 Wooden tower
* 4 Wooden stair
* 5 Stone walls
* 6 Stone gate
* 7 Stone tower
* 8 Stone stair

* __4 Industial__
* 1 __f__Stockpile
* 2 Lumberjack
* 3 Stone quarry
* 4 Iron mine
* 5 Ox transport (moves wood, stone, or iron/steel. This is an option, and buildings like the woodcutter do have an option to move them thereself)
* 6 
* 7 
* 8 

* __5 Food__
* 1 __G__ranary
* 2 Apple farm
* 3 Cheese/Cow farm
* 4 Hunter (Hunters can shoor ground animals (Deers, Rabbits, wolfs, Bears)
* 5 Wheat/grain farm
* 6 Mill (for wheat/grain)
* 7 Baker (for floar)
* 8 ?Fischer?


* __6 MISC__
* 1 __T__rading house (This building enables trading + Auto trading ?with an upgrade?; like buy wood if below 100, sell if above 200)
* 2 C__h__urch (Boost happiness based on amount of homes near. can be bribed to temponary boost more)
* 3 Homes (Closer to the castle = bigger home??)
* 4 Water well (exsinquise fire. ?and makes people more happy?)
* 5 
* 6 
* 7 ?Hospital/ alchemist?	
* 8 ?Treasury?

###4. Add items

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

###5. Double click to select all of the same thing on your screen (soldiers, buildings etc) 

###6. Hide heath above death troops, show above troops who are not yet death, (?Also create a 15/30min to full regen?)

###7. You can not build in a radius X of troops of the enemy

###8. Keyboard layout

__QturnL__ __Qforward__ __EturnR__ __Rotatebuilding__ __Trading__ y u i o __Pause__

__Aleft__ __Sback__ __Dright__ __Fstockpile__ __Granary__ __cHurch__ j k l 

z x __Castle__ v __Barracks__ n m

###10. __p__ to pause game

###11. Center the minimap on the camera is looking at instead of where the camera is + ?Add a ourline of the screen range on the minimap?

###12. Add Konami

###13. FPSDisplay

###14. Max fps
	Application.targetFrameRate = PlayerPrefs.GetInt("MaxFrameRate");           //Set MaxFrame rate