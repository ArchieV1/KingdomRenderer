using static KingdomRenderer.Shared.ArchieV1.PrivateFieldTools;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources.VanillaResources
{
    public class UnusableStoneModdedResourceType : ModdedResourceType
    {
        public UnusableStoneModdedResourceType() : base()
        {
            SetPrivateField(this, "DefaultResource", true);
            SetPrivateField(this, "Name", "UnusableStone");
            CaveWitchMustBePlacedXTilesAway = 2;
            ResourceType = ResourceType.UnusableStone;
            DoNotAssignResourceType = true;
            Registered = true;
        }
    }
}
