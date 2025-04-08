namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Names
{
    /// <summary>
    /// Names of Methods that can be called against the KCModdingFramework object
    /// </summary>
    public static class MethodNames
    {
        /// <summary>
        /// Send by mods to register themselves, menu returns a ModConfig with all values assigned (The ResourceType they have been assigned):
        /// Parameter: ModConfigMF
        /// Return value: ModConfigMF
        /// </summary>
        public static string RegisterMod => "RegisterMod";

        /// <summary>
        /// Should be listener to by mods to get information back about all of the mods that have been loaded.
        /// </summary>
        public static string AllModsRegistered => "AllModsRegistered";
    }
}
