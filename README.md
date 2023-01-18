# KingdomRenderer
A mod for Kingdoms and Castles that saves a render of the map X times per year (Or when you press the assigned key).  
Highly configurable! Render from 160x120 all of the way up to 7680x4320, set it to save any time of year, assign any key to manually render and more!

# Installation
Download the ZIP file from [the releases page](https://github.com/ArchieV1/KingdomRenderer/releases)  
Extract to `~\steamapps\common\Kingdoms and Castles\KingdomsAndCastles_Data\mods`  
Or subscribe to the the steam workshop page [here](https://steamcommunity.com/sharedfiles/filedetails/?id=2306848108) and it will download automatically.

You will need to restart your game
Launch the game and press "o" to access the settings at any time

# Notes
If you choose to save the image as a .png (SteamApps) it will be saved at  
`~\steamapps\common\Kingdoms and Castles\KingdomsAndCastles_Data\mods\KingdomRenderer\Renders`   
If you choose to save the image as a .png (AppData) it will be saved at 
`C:\Users\USERNAME\AppData\LocalLow\LionShield\Kingdoms and Castles\`
  
Ignore the popup upon game load (It is creating the "Renders" directory). It will appear for less than a second

# Editing save directory
### Changing png (SteamApps)
If you would like the change the save directory for "png (SteamApps)" open `KingdomRenderer.cs` and edit line 430 to be where you want the files to be saved.  
Then remove the `//`.  
Backslashes need to be in pairs. Forward slashes do not.  

### Changing png (AppData)
You cannot change the save directory for "png (AppData)" to be anywhere except `C:\Users\green\AppData\LocalLow\LionShield\Kingdoms and Castles\SOMETHING\HERE`.  
Open `KingdomRenderer.cs` and edit line 440 in the same way as above


# Warning
The save as Gif has now been removed as the code for it has been removed by the Kingdoms and Castles. 