# KingdomRenderer
A mod for Kingdoms and Castles that saves a render of the map X times per year (Or when you press the assigned key)

# Important
You ***must*** create the "Renders" folder at location (Capitals and spelling ***do*** matter):  
C:\Users\USERNAME\AppData\LocalLow\LionShield\Kingdoms and Castles\Renders  
KaC mods cannot access System.IO to create folders so you must or it will not save any images.  

# Installation
Download the ZIP file from [the releases page](https://github.com/ArchieV1/KingdomRenderer/releases)  
Extract to `~\steamapps\common\Kingdoms and Castles\KingdomsAndCastles_Data\mods`  
Launch the game and press "o" to access the settings at any time

# Notes
If you do not want to create a renders folder; when you download the mod edit 
KingdomRenderer.SaveRenderAsGif(Texture2D texture2D, string filename) so that timelapseGif.gifPath does not contain "/Renders/" (Approx line 330.)

`timelapseGif.gifPath = "/Renders/" + filename + ".gif";`  
`timelapseGif.gifPath = filename + ".gif";`