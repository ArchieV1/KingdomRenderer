using System.Collections.Generic;
using KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Generators
{
    public class EmptyCaveLikeGenerator : GeneratorBase
    {
        public EmptyCaveLikeGenerator(IEnumerable<ModdedResourceType> resourceTypeBases) : base(resourceTypeBases)
        {
        }

        public override bool Generate(World world)
        {
            return base.Generate(world);
        }


    }
}
