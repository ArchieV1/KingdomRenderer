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
            // This cannot be used by AppData or SteamApps because of permission issues? TODO check this seems unlikely
            string kingdomRendererPath = Path.Join("KingdomRenderer", kingdomName);
            
            if (location == FileLocation.AppData)
            {
                // Linux
                // ~/.config/unity3d/LionShield/Kingdoms and Castles/
                
                // Mac
                // ~/Library/Logs/Unity/
                
                // Windows
                // C:\Users\[USERNAME]\AppData\LocalLow\LionShield\Kingdoms and Castles\
                
                // Just save to the root of the path
                return Path.Join(Application.persistentDataPath, "");
            }

            if (location == FileLocation.SteamApps)
            {
                return Path.Join(helper.modPath, kingdomRendererPath);
            }
            
            
            if (OS.RunningUnixLike())
            {
                switch (location)
                {
                    case FileLocation.Desktop:
                        return Path.Join("$HOME/Desktop", kingdomRendererPath);
                    case FileLocation.Documents:
                        return Path.Join("$HOME/Documents", kingdomRendererPath);
                    case FileLocation.Pictures:
                        return Path.Join("$HOME/Pictures", kingdomRendererPath);
                }
            }
            
            if (OS.RunningWindows())
            {
                switch (location)
                {
                    case FileLocation.Desktop:
                        return Path.Join("%USERPROFILE%/Desktop", kingdomRendererPath);
                    case FileLocation.Documents:
                        return Path.Join("%USERPROFILE%/Documents", kingdomRendererPath);
                    case FileLocation.Pictures:
                        return Path.Join("%USERPROFILE%/Pictures", kingdomRendererPath);
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
                helper.Log($"Listing loc for file {location}");
                directories.Add(GetSavePath(helper, location, null));
            }
            
            return directories;
        }

        /// <summary>
        /// Returns $/steamapps/common/Kingdoms and Castles/ <br/>
        /// Linux: ~/.local/share/Steam/steamapps/common/Kingdoms and Castles/
        /// </summary>
        /// <returns></returns>
        public static string GetKingdomsAndCastlesSteamAppsDirectory()
        {
            return Path2.GetParentPath(Application.dataPath);
        }
    }
}