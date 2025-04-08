using static KingdomRenderer.Shared.ArchieV1.PrivateFieldTools;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources.VanillaResources
{
    public class IronDepositModdedResourceType : ModdedResourceType
    {
        public IronDepositModdedResourceType() : base() 
        {
            SetPrivateField(this, "DefaultResource", true);
            SetPrivateField(this, "Name", "IronDeposit");
            CaveWitchMustBePlacedXTilesAway = 2;
            ResourceType = ResourceType.IronDeposit;
            DoNotAssignResourceType = true;
            Registered = true;
        }
    }
}
