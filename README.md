# KingdomRenderer
A mod for Kingdoms and Castles that saves a render of the map X times per year (Or when you press the assigned key).  
Highly configurable! Render from 160x120 all of the way up to 7680x4320, set it to save any time of year, assign any key to manually render and more!

## Installation
Download the ZIP file from [the releases page](https://github.com/ArchieV1/KingdomRenderer/releases)  
Extract to `~\steamapps\common\Kingdoms and Castles\KingdomsAndCastles_Data\mods`  
Or subscribe to the the steam workshop page [here](https://steamcommunity.com/sharedfiles/filedetails/?id=2306848108) and it will download automatically.

You will need to restart your game
Launch the game and press "o" to access the settings at any time

## Notes
Files will be saved at:  
Windows  
`~\steamapps\common\Kingdoms and Castles\`  
Linux  
`$HOME/.local/share/Steam/steamapps/common/Kingdoms and Castles`    
Mac  
`$HOME/.local/share/Steam/steamapps/common/Kingdoms and Castles`

## Editing save directory
You cannot - The game restricts which directories can be saved to unfortunately

## Changelog
### V1.0
Mod release
### V1.1
No longer need to create "Renders" file though in %appdata% it is now all dumped in the root dir
Now supports .png! Much quicker to save.
Now also has an option to save images to the KaC SteamApps file (Or wherever KaC is installed)
### V1.2
Auto renders now correctly use resolution settings rather than always being 500x500
### V1.2.1
Renders are now generated on a different thread massively reducing gameplay impact
### V1.3
The save as Gif has now been removed as the code for it has been removed by the the game
### V2
Changing save locations has now been removed as the game now blocks it
WARNING: This may be a breaking change