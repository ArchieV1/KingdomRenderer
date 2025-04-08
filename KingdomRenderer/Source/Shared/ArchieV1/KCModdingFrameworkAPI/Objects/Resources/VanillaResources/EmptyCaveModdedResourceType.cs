using static KingdomRenderer.Shared.ArchieV1.PrivateFieldTools;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources.VanillaResources
{
    public class EmptyCaveModdedResourceType : ModdedResourceType
    {
        public EmptyCaveModdedResourceType() : base()
        {
            SetPrivateField(this, "DefaultResource", true);
            SetPrivateField(this, "Name", "EmptyCave");
            CaveWitchMustBePlacedXTilesAway = 5;
            NumberTreesRequiredNearby = new TreeRequirement
            {
                NumberTreeTiles = 4,
                Distance = 1,
            };
            ResourceType = ResourceType.EmptyCave;
            DoNotAssignResourceType = true;
            Registered = true;
        }
    }
}
