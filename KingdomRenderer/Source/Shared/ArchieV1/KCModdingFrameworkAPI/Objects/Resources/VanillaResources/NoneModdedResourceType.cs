using static KingdomRenderer.Shared.ArchieV1.PrivateFieldTools;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources.VanillaResources
{
    public class NoneModdedResourceType : ModdedResourceType
    {
        public NoneModdedResourceType() : base()
        {
            SetPrivateField(this, "DefaultResource", true);
            SetPrivateField(this, "Name", "None");
            CaveWitchMustBePlacedXTilesAway = 0;
            ResourceType = ResourceType.None;
            DoNotAssignResourceType = true;
            Registered = true;
        }
    }
}
