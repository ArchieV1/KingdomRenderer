using static KingdomRenderer.Shared.ArchieV1.PrivateFieldTools;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources.VanillaResources
{
    public class WoodModdedResourceType : ModdedResourceType
    {
        public WoodModdedResourceType() : base()
        {
            SetPrivateField(this, "DefaultResource", true);
            SetPrivateField(this, "Name", "Wood");
            CaveWitchMustBePlacedXTilesAway = 0;
            ResourceType = ResourceType.Wood;
            DoNotAssignResourceType = true;
            Registered = true;
        }
    }
}
