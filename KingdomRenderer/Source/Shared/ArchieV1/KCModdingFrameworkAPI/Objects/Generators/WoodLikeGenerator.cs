using System.Collections.Generic;
using KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Generators
{
    public class WoodLikeGenerator : GeneratorBase
    {
        public WoodLikeGenerator(IEnumerable<ModdedResourceType> resourceTypeBases) : base(resourceTypeBases)
        {
        }

        public override bool Generate(World world)
        {
            return base.Generate(world);
        }
    }
}
