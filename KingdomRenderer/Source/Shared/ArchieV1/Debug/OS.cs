using System;
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
            return runningUnixLike;
        }

        public static bool RunningWindows()
        {
            bool runningWindows = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
            return runningWindows;
        }

        public static bool RunningMac()
        {
            bool runningMac = Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor;
            return runningMac;
        }

        public static bool RunningLinux()
        {
            bool runningLinux = Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor;
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

        public static OperatingSystem GetOS()
        {
            if (RunningLinux())
            {
                return OperatingSystem.Linux;
            }

            if (RunningMac())
            {
                return OperatingSystem.Mac;
            }

            if (RunningWindows())
            {
                return OperatingSystem.Windows;
            }
            
            throw new Exception("Not running Linux, Mac or Windows.");
        }
    }
}