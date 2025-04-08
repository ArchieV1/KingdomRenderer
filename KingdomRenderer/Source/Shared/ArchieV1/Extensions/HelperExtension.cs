using System.Collections.Generic;

namespace KingdomRenderer.Shared.ArchieV1.Extensions
{
    public static class HelperExtension
    {
        public const int HyphenCount = 20;
        
        /// <summary>
        /// Logs a line of 20 hyphens
        /// </summary>
        /// <param name="helper"></param>
        public static void LogLine(this KCModHelper helper)
        {
            helper.Log("--------------------");
        }

        public static void LogMultiLine(this KCModHelper helper, string log)
        {
            foreach (string line in log.Split('\n'))
            {
                helper.Log(line);
            }
        }

        public static void LogMultiLine(this KCModHelper helper, IEnumerable<object> log)
        {
            foreach (string line in log)
            {
                helper.Log(line);
            }
        }

        /// <summary>
        /// Logs an empty line
        /// </summary>
        /// <param name="helper"></param>
        public static void Log(this KCModHelper helper)
        {
            helper.Log("");
        }
        
        public static void Log(this KCModHelper helper, object log)
        {
            helper.Log(log.ToString());
        }
        
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