using UnityEngine;

namespace KingdomRenderer.Shared.ArchieV1.Debug
{
    public static class OS
    {
        public const string WindowsSeparator = @"\";
        
        public const string MacSeparator = UnixSeparator;
        
        public const string LinuxSeparator = UnixSeparator;
        
        public const string GenericSeparator = UnixSeparator;
        
        public const string UnixSeparator = "/";
        
        public static bool RunningUnixLike()
        {
            bool runningUnixLike = RunningLinux() || RunningMac();
            ULogger.ULog($"Running Unix Like: {runningUnixLike}", "System.IO.OS");
            return runningUnixLike;
        }

        public static bool RunningWindows()
        {
            bool runningWindows = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
            ULogger.ULog($"Running Windows: {runningWindows}", "System.IO.OS");
            return runningWindows;
        }

        public static bool RunningMac()
        {
            bool runningMac = Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor;
            ULogger.ULog($"Running Mac: {runningMac}", "System.IO.OS");
            return runningMac;
        }

        public static bool RunningLinux()
        {
            bool runningLinux = Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor;
            ULogger.ULog($"Running Linux: {runningLinux}", "System.IO.OS");
            return runningLinux;
        }
        
        public static string GetDirectorySeparator()
        {
            return RunningWindows() ? WindowsSeparator : UnixSeparator;
        }

        public static char GetDirectorySeparatorChar()
        {
            return RunningWindows() ? WindowsSeparator.ToCharArray()[0] : UnixSeparator.ToCharArray()[0];
        }
    }
}