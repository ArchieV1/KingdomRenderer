using System;
using System.ComponentModel;
using KingdomRenderer.Shared.Zat;

namespace KingdomRenderer.Shared.ArchieV1.Extensions.ModMenuApiExtensions
{
    public static class InteractiveSelectSettingExtension
    {
        /// <summary>
        /// Gets the value selected in the setting
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static string GetValue(this InteractiveSelectSetting setting)
        {
            return setting.Options[setting.Value];
        }

        public static float GetValueFloat(this InteractiveSelectSetting setting)
        {
            return float.Parse(setting.GetValue());
        }
        
        public static float GetValueFloatRounded(this InteractiveSelectSetting setting, int digits = 1, MidpointRounding rounding = MidpointRounding.AwayFromZero)
        {
            return (float)Math.Round(setting.GetValueFloat(), digits, rounding);
        }

        public static int GetValueInt(this InteractiveSelectSetting setting)
        {
            return int.Parse(setting.GetValue());
        }

        public static bool GetValueBool(this InteractiveSelectSetting setting)
        {
            return Boolean.Parse(setting.GetValue());
        }

        public static double GetValueDouble(this InteractiveSelectSetting setting)
        {
            return double.Parse(setting.GetValue());
        }

        public static decimal GetValueDecimal(this InteractiveSelectSetting setting)
        {
            return Decimal.Parse(setting.GetValue());
        }

        public static string GetValueString(this InteractiveSelectSetting setting)
        {
            // This one is just here for consistency
            return setting.GetValue();
        }

        public static T GetValueEnum<T>(this InteractiveSelectSetting setting)
        {
            return (T)Enum.Parse(typeof(T), setting.GetValue());
        }

        public static T GetValueObject<T>(this InteractiveSelectSetting setting)
        {
            string val = setting.GetValue();
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.IsValid(val))
            {
                return (T)converter.ConvertFromString(val);
            }

            throw new InvalidCastException($"Cannot convert '{val}' to type '{typeof(T)}'.");
        }
        
        public static bool GetValueYesNo(this InteractiveSelectSetting setting)
        {
            string val = setting.GetValue();
            if (val.Equals("yes", StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            if (val.Equals("no", StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }
            
            throw new ArgumentException($"Unknown option: {setting}");
        }
    }
}