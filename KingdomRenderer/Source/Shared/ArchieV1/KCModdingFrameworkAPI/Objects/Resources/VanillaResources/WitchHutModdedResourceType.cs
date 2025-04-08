using static KingdomRenderer.Shared.ArchieV1.PrivateFieldTools;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources.VanillaResources
{
    public class WitchHutModdedResourceType : ModdedResourceType
    {
        public WitchHutModdedResourceType() : base()
        {
            SetPrivateField(this, "DefaultResource", true);
            SetPrivateField(this, "Name", "WitchHut");
            CaveWitchMustBePlacedXTilesAway = 5;
            NumberTreesRequiredNearby = new TreeRequirement
            {
                Distance = 1,
                NumberTreeTiles = 4,
            };
            ResourceType = ResourceType.WitchHut;
            DoNotAssignResourceType = true;
            Registered = true;
        }
    }
}
