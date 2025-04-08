using KingdomRenderer.Shared.Zat;
using Newtonsoft.Json;

namespace KingdomRenderer.Shared.ArchieV1.Extensions
{
    public static class JsonExtensions
    {
        private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings();
        
        public static object Deserialise(this string str, JsonSerializerSettings settings = null)
        {
            if (settings == null)
            {
                // settings = IMCPort.serializerSettings;
                settings = _jsonSerializerSettings;
            }

            return JsonConvert.DeserializeObject(str, settings);
        }

        public static string Serialise(this object obj, JsonSerializerSettings settings = null)
        {
            if (settings == null)
            {
                // settings = IMCPort.serializerSettings;
                settings = _jsonSerializerSettings;
            }

            return JsonConvert.SerializeObject(obj, settings);
        }

        public static bool IsDeserialisable(this string str)
        {
            if (str == null) return false;

            try
            {
                Deserialise(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsSerialiseable(this object obj)
        {
            try
            {
                //JsonConvert.SerializeObject(obj, IMCPort.serializerSettings);
                JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsJSONable(this object obj)
        {
            try
            {
                // If Encode/Decode/Encode == Encode then it can be sent and received without issue
                return Serialise(Deserialise(Serialise(obj))) ==
                       Serialise(obj);
            }
            catch
            {
                return false;
            }
        }
    }
}