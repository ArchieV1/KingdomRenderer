cd "D:\SteamLibrary\steamapps\common\Kingdoms and Castles\KingdomsAndCastles_Data\mods\KingdomRenderer"
del *.cs
echo %TIME%
for /R "C:\Users\green\RiderProjects\KingdomRenderer\KingdomRenderer" %%f in (*.cs) do copy %%f "D:\SteamLibrary\steamapps\common\Kingdoms and Castles\KingdomsAndCastles_Data\mods\KingdomRenderer"

cd "D:\SteamLibrary\steamapps\common\Kingdoms and Castles\KingdomsAndCastles_Data\mods\KingdomRenderer"
del *.txt

cd "C:\Users\green\AppData\LocalLow\LionShield\Kingdoms and Castles"
del *.txt

start "" steam://run/569480
