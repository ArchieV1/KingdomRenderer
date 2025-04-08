using System.Linq;
using Assets;

namespace KingdomRenderer.Shared.Zat
{
    public static class GameUtils
    {
        public static bool AddDeveloperNames(params string[] newNames)
        {
            var names = ZatsReflection.GetStaticField<NameList, string[]>("devNames");
            if (names == null) return false;
            names = names.Concat(newNames).ToArray();
            ZatsReflection.SetStaticField<NameList, string[]>("devNames", names);
            return true;
        }
    }
}
