using System;
using System.Collections.Generic;
using KingdomRenderer.Shared.ArchieV1.Debug;

namespace KingdomRenderer.Shared.ArchieV1
{
    /// <summary>
    /// An alternative logger where all logs universally go to the same place.
    /// </summary>
    /// <example>
    /// using static NameSpace.Shared.ArchieV1.ULogger;
    /// <br /> <br />
    /// ULog("Use me like this")
    /// </example> 
    public static class ULogger
    {
        private static readonly string NameSpace = CallStackTools.GetCallingNamespace(2);
        
        /// <summary>
        /// Logs to <c>~\steamapps\common\Kingdoms and Castles\KingdomsAndCastles_Data\mods\log.txt</c>. <br />
        /// This is where compiler logging happens.
        /// </summary>
        /// <param name="category">The category/mod name. Max 25 chars.</param>
        /// <param name="message">The message to post. Newlines will be split over multiple logs.</param>
        public static void ULog(object message, object category)
        {
            string categoryString = FormatCategory(category);
            string messageString = message.ToString();

            if (messageString.Contains("\n"))
            {
                foreach (string line in messageString.Split('\n'))
                {
                    ULog(line, categoryString);
                } 
            }
            else
            {
                string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine("[ULOG|{0}] {1, -25} | {2}", dateTime, categoryString, messageString);
            }
        }
        
        /// <summary>
        /// Logs the given message with namespace used as the category.
        /// </summary>
        /// <param name="message">The message to post. Newlines will be split over multiple logs.</param>
        public static void ULog(object message)
        {
            ULog(message.ToString(), NameSpace);
        }

        public static void ULogMultiLine(IEnumerable<object> messages, object category)
        {
            string categoryString = FormatCategory(category);
            foreach (var message in messages)
            {
                ULog(message.ToString(), categoryString);
            }
        }

        /// <summary>
        /// Logs a newline with the given category.
        /// </summary>
        /// <param name="category">The category/mod name. Max 25 chars.</param>
        public static void ULogNewline(object category = null)
        {
            string categoryString = category.ToString();
            
            if (categoryString == string.Empty)
            {
                categoryString = NameSpace;
            }
            
            ULog(string.Empty, categoryString);
        }

        /// <summary>
        /// Log the CallStack in the form <c>Namespace.ClassName.MethodName</c>
        /// </summary>
        /// <param name="category"></param>
        /// <param name="frameSkip"></param>
        /// <remarks>WARNING: Runs on main thread.</remarks>
        public static void ULogCallStack(object category = null, int frameSkip = 2)
        {
            category = FormatCategory(category);
            
            ULog(CallStackTools.GetCallStack(frameSkip), category);
        }

        /// <summary>
        /// Log the calling method in the form <c>Namespace.ClassName.MethodName</c>
        /// </summary>
        /// <param name="category"></param>
        /// <param name="frameSkip"></param>
        /// <remarks>WARNING: Runs on main thread.</remarks>
        public static void ULogCallingMethod(object category = null, int frameSkip = 4)
        {
            category = FormatCategory(category);
            
            ULog(CallStackTools.GetCallingMethodName(frameSkip), category);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="frameSkip"></param>
        /// <remarks>WARNING: Runs on main thread.</remarks>
        public static void ULogCallingClassName(object category = null, int frameSkip = 4)
        {
            category = FormatCategory(category);
            
            ULog(CallStackTools.GetCallingClassName(frameSkip), category);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="frameSkip"></param>
        /// <remarks>WARNING: Runs on main thread.</remarks>
        public static void ULogCallingNamespace(object category = null, int frameSkip = 4)
        {
            category = FormatCategory(category);
            
            ULog(CallStackTools.GetCallingNamespace(frameSkip), category);
        }

        private static string FormatCategory(object category)
        {
            string categoryString = category.ToString().Replace("\n", "");
            
            if (categoryString == string.Empty)
            {
                categoryString = NameSpace;
            }

            if (categoryString.Length > 25)
            {
                // Select the last 25 chars of the category instead of first 25
                categoryString = categoryString.Substring(categoryString.Length - 25, 25);
            }
            return categoryString;
        }
    }
}