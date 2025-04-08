using System.Collections.Generic;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources
{
    public static class VanillaResourceType
    {
        /// <summary>
        /// All of the possible default resources
        /// </summary>
        public static List<ResourceType> List 
            => new List<ResourceType>
                {
                ResourceType.None,
                ResourceType.Stone,
                ResourceType.Water,
                ResourceType.Wood,
                ResourceType.EmptyCave,
                ResourceType.IronDeposit,
                ResourceType.UnusableStone,
                ResourceType.WitchHut,
                ResourceType.WolfDen
                };

        /// <summary>
        /// Determines whether a ResourceType is added by the game or a mod.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if the <paramref name="value"/> sent is a Vanilla resource type</returns>
        public static bool IsVanilla(ResourceType value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// Determines whether a ResourceType is added by the game or a mod.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsModded(ResourceType value)
        {
            return !IsVanilla(value);
        }

        /// <summary>
        /// Analogous to <see cref="IsVanilla(ResourceType)"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Contains(ResourceType value)
        {
            return IsVanilla(value);
        }
    }
}
