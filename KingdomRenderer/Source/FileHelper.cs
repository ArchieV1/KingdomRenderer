using System;
using System.Collections.Generic;
using KingdomRenderer.Shared.ArchieV1.Debug;
using KingdomRenderer.Shared.ArchieV1.System.IO;
using UnityEngine;

namespace KingdomRenderer
{
    public static class FileHelper
    {
        /// <summary>
        /// Updates _savePath to make sure it is correct for the current settings and OS
        /// </summary>
        /// <returns></returns>
        public static string GetSavePath(KCModHelper helper, FileLocation location, string kingdomName)
        {
            string dirName = Path.Join("KingdomRenderer", kingdomName);
            
            switch (location)
            {
                case FileLocation.SteamApps:
                    // Linux
                    // ~/.steam/steam/steamapps/workshop/content/569480/2306848108
                    // ~/.local/share/Steam/steamapps/common/Kingdoms and Castles/KingdomsAndCastles_Data/mods/KingdomRenderer
                    
                    // Mac
                    // ???
                    
                    // Windows
                    // $\steamapps\common\Kingdoms and Castles\KingdomsAndCastles_Data\mods\KingdomRenderer\Renders
                    // $\steamapps\workshop\content\569480\2306848108\
                    return Path.Join(helper.modPath, dirName);
                case FileLocation.AppData:
                    // Linux
                    // ~/.config/unity3d/LionShield/Kingdoms and Castles/
                    
                    // Mac
                    // ~/Library/Logs/Unity/
                    
                    // Windows
                    // C:\Users\[USERNAME]\AppData\LocalLow\LionShield\Kingdoms and Castles\
                    return Path.Join(Application.persistentDataPath, dirName);
            }
            
            
            if (OS.RunningUnixLike())
            {
                switch (location)
                {
                    case FileLocation.Desktop:
                        return Path.Join("$HOME/Desktop", dirName);
                    case FileLocation.Documents:
                        return Path.Join("$HOME/Documents", dirName);
                    case FileLocation.Pictures:
                        return Path.Join("$HOME/Pictures", dirName);
                }
            }
            
            if (OS.RunningWindows())
            {
                switch (location)
                {
                    case FileLocation.Desktop:
                        return Path.Join("%USERPROFILE%/Desktop", dirName);
                    case FileLocation.Documents:
                        return Path.Join("%USERPROFILE%/Documents", dirName);
                    case FileLocation.Pictures:
                        return Path.Join("%USERPROFILE%/Pictures", dirName);
                }
            }

            // This should not be reachable
            throw new Exception($"File location ({location}) not supported on OS {OS.GetOS()}");
        }

        public static IEnumerable<string> ListAllRendersDirectories(KCModHelper helper)
        {
            List<string> directories = new List<string>();
            foreach (FileLocation location in Enum.GetValues(typeof(FileLocation)))
            {
                directories.Add(GetSavePath(helper, location, null));
            }
            
            return directories;
        }
    }
}