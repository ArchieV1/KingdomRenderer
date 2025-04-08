using System.Collections.Generic;
using KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Generators
{
    public class WitchHutLikeGenerator : GeneratorBase
    {
        private readonly ResourceType witchHut;

        // This is done at the same time as Wolves?
        // Method: DoPlaceCaves
        public WitchHutLikeGenerator(ModdedResourceType resourceTypeBases) : base(new List<ModdedResourceType> { resourceTypeBases })
        {
            witchHut = resourceTypeBases.ResourceType;
        }

        public override bool Generate(World world)
        {
            return base.Generate(world);
        }
    }
}
