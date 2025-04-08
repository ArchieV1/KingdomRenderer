namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources
{
    /// <summary>
    /// Represents <see cref="NumberTreeTiles"/> required within <see cref="Distance"/> tiles of the object in order for the
    /// resource to be placed.
    /// </summary>
    public struct TreeRequirement
    {
        /// <summary>
        /// The number of tree tiles that must surround the Resource.
        /// </summary>
        public int NumberTreeTiles;

        /// <summary>
        /// The square radius that the trees will be counted in.
        /// </summary>
        public int Distance;
    }
}
