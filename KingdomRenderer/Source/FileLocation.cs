namespace KingdomRenderer
{
    public enum FileLocation
    {
        /// <summary>
        /// "$HOME/.local/share/Steam/steamapps/common/Kingdoms and Castles"
        /// </summary>
        SteamApps = 0,
        /// <summary>
        /// Config on Unix <br/>
        /// Library on Mac
        /// </summary>
        // "$HOME/.config/unity3d/LionShield/Kingdoms and Castles"
        AppData = 1,
        
        /// <summary>
        /// TODO Make these work - Likely permission issues
        /// </summary>
        Documents = 2,
        Pictures = 3,
        Desktop = 4,
    }
}