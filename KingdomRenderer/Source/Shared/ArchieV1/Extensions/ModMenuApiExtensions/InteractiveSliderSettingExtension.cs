using System;
using System.Globalization;
using KingdomRenderer.Shared.Zat;

namespace KingdomRenderer.Shared.ArchieV1.Extensions.ModMenuApiExtensions
{
    public static class InteractiveSliderSettingExtension
    {
        /// <summary>
        /// Sets the label to be the value of the setting.
        /// </summary>
        /// <param name="setting"></param>
        public static void SetLabelValue(this InteractiveSliderSetting setting)
        {
            setting.Label = setting.Value.ToString(CultureInfo.InstalledUICulture);
        }

        public static void SetValue(this InteractiveSliderSetting setting, float value, int digits = 1, MidpointRounding rounding = MidpointRounding.AwayFromZero)
        {
            setting.Value = (float)Math.Round(value, digits, rounding);
        }

        public static void RoundValue(this InteractiveSliderSetting setting, int digits = 1, MidpointRounding rounding = MidpointRounding.AwayFromZero)
        {
            setting.Value = (float)Math.Round(setting.Value, digits, rounding);
        }

        public static float GetValue(this InteractiveSliderSetting setting)
        {
            return setting.Value;
        }

        public static float GetValueRounded(this InteractiveSliderSetting setting, int digits = 1, MidpointRounding rounding = MidpointRounding.AwayFromZero)
        {
            return (float)Math.Round(setting.Value, digits, rounding);
        }

        public static int GetValueInt(this InteractiveSliderSetting setting, MidpointRounding rounding = MidpointRounding.AwayFromZero)
        {
            return (int)Math.Round(setting.Value, rounding);
        }
    }
}