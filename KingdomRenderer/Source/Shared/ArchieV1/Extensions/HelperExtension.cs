namespace KingdomRenderer.Shared.ArchieV1.Extensions
{
    public static class HelperExtension
    {
        /// <summary>
        /// Logs a line of 20 hyphens
        /// </summary>
        /// <param name="helper"></param>
        public static void LogLine(this KCModHelper helper)
        {
            helper.Log("--------------------");
        }

        // TODO use this
        public static void LogInLine(this KCModHelper helper, string log)
        {
            // Calculate the number of hyphens needed on each side
            int remainingLength = 20 - log.Length;

            if (remainingLength <= 0)
            {
                helper.Log(log);
            }

            // Calculate how many hyphens to add on each side
            int leftPadding = remainingLength / 2;
            int rightPadding = remainingLength - leftPadding;

            // Create the padded string
            string paddedString = new string('-', leftPadding) + log + new string('-', rightPadding);
            
            helper.Log(paddedString);
        }
    }
}