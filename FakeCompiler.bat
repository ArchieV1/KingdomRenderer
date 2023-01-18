:: Set up
ECHO ==================Starting "compiler"==================
SET "kac-mod-directory=D:\SteamLibrary\steamapps\common\Kingdoms and Castles\KingdomsAndCastles_Data\mods\KingdomRenderer"
SET "wip-mod-directory=C:\Users\green\RiderProjects\KingdomRenderer\KingdomRenderer"
SET "kac-appdata-directory=C:\Users\green\AppData\LocalLow\LionShield\Kingdoms and Castles"

:: Go to KaC mod directory and delete old modfile and old output.txt
CD /D %kac-mod-directory%
DEL *.cs
:: Delete the old log
:: This is the log from the mod
DEL output.txt

:: Go to KaC gamedata folder and delete log
:: This is the log that 
CD /D %kac-appdata-directory%
DEL output_log.txt

:: Copy files from WIP folder to KaC mod folder so that KaC can compile it
FOR /R "%wip-mod-directory%" %%f IN (*.cs) DO COPY %%f "%kac-mod-directory%"

:: Launch KaC
:: Will launch even if steam has not been opened
start "" steam://run/569480