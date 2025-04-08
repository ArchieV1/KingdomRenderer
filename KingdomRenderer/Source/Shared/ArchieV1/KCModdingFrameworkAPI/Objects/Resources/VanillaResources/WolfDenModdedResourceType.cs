using static KingdomRenderer.Shared.ArchieV1.PrivateFieldTools;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources.VanillaResources
{
    public class WolfDenModdedResourceType : ModdedResourceType
    {
        public WolfDenModdedResourceType() : base()
        {
            SetPrivateField(this, "DefaultResource", true);
            SetPrivateField(this, "Name", "WolfDen");
            CaveWitchMustBePlacedXTilesAway = 5;
            // TODO check this:
            NumberTreesRequiredNearby = new TreeRequirement
            {
                NumberTreeTiles = 4,
                Distance = 1,
            };
            ResourceType = ResourceType.WolfDen;
            DoNotAssignResourceType = true;
            Registered = true;
        }
    }
}
