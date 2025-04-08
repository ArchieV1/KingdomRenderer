using static KingdomRenderer.Shared.ArchieV1.PrivateFieldTools;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources.VanillaResources
{
    public class WaterModdedResourceTypeBase : ModdedResourceType
    {
        public WaterModdedResourceTypeBase() : base() 
        {
            SetPrivateField(this, "DefaultResource", true);
            SetPrivateField(this, "Name", "Water");
            CaveWitchMustBePlacedXTilesAway = 0;
            ResourceType = ResourceType.Water;
            DoNotAssignResourceType = true;
            Registered = true;
        }
    }
}
