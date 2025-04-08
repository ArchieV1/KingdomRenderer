using static KingdomRenderer.Shared.ArchieV1.PrivateFieldTools;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources.VanillaResources
{
    public class StoneModdedResourceType : ModdedResourceType
    {
        public StoneModdedResourceType() : base()
        {
            SetPrivateField(this, "DefaultResource", true);
            SetPrivateField(this, "Name", "Stone");
            CaveWitchMustBePlacedXTilesAway = 2;
            ResourceType = ResourceType.Stone;
            DoNotAssignResourceType = true;
            Registered = true;
        }
    }
}
