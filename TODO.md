#To Do / Notes

##Notes
Where steam install all of the mods  
D:\SteamLibrary\steamapps\workshop\content\569480  


###Logs

**Logs from Debug.Log**  
C:\Users\green\AppData\LocalLow\LionShield\Kingdoms and Castles  

**Logs from mods such as build errors**  
D:\SteamLibrary\steamapps\common\Kingdoms and Castles\KingdomsAndCastles_Data\mods  

##To do
When taking render:  
- disable clouds (Current method doesn't work)  
- disable notifications on screen such as "stone" (Current method doesn't work)  
- ~~change angle to be more from above~~  
- ~~renders it at an angle so it doesnt fit the whole map (45deg for some reason)~~ (I think)    
- ~~if loaded a map beforehand it will render world creation maps as if they had that name (Of the previously loaded map)~~( I think)  
- ~~Manual press doesnt work~~   
- ~~After quitting it renders the home screen and gives it the name of the last loaded map~~ (GameState.inst.CurrMode == GameState.inst.playingMode fixed this)  
- Options for where to save renders ❎ (nah)
- Save renders as .png/.jpeg not .gif
- ~~While creating a world it renders every frame (With no name and going above settings val)~~ (GameState.inst.CurrMode == GameState.inst.playingMode fixed this)  
- Could be made faster by moving camera once at Map load though its pretty instant I think.

###Check settings work:  
- Enabled  ✅  
- Resolution scaling  ❎ (Replaced by resolution options)
- Render Height  ✅
- Auto Timer (By seconds) ✅  
- Auto enabled  ✅  
- Manual Key  ✅  
- Changing Manual key ✅  
- Manual Enabled✅  
- Resolution options ✅
- Auto Timer (By number per year) ✅
- Change resolution to pairs rather than two options? (Just do the same string and split it with a comma ",")