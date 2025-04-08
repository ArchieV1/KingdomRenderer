namespace KingdomRenderer.Shared.Zat
{
    public static class Debugging
    {
        public static bool Active { get; set; }
        public static KCModHelper Helper { get; set; }

        public static void Log(string category, string content)
        {
            if (Helper == null || !Active) return;
            Helper.Log($"[{category}] {content}");
        }
    }
}
