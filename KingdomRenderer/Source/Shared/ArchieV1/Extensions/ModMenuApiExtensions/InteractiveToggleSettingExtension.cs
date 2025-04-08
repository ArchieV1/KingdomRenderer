using System.Globalization;
using KingdomRenderer.Shared.Zat;

namespace KingdomRenderer.Shared.ArchieV1.Extensions.ModMenuApiExtensions
{
    public static class InteractiveToggleSettingExtension
    {
        /// <summary>
        /// Sets the label to the be the value of the setting.
        /// </summary>
        /// <param name="setting"></param>
        public static void SetLabelValue(this InteractiveToggleSetting setting)
        {
            setting.Label = setting.Value.ToString(CultureInfo.InstalledUICulture);
        }

        public static bool GetValue(this InteractiveToggleSetting setting)
        {
            return setting.Value;
        }
    }
}