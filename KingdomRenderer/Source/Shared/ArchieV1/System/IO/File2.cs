using System.Diagnostics;
using KingdomRenderer.Shared.ArchieV1.Debug.Terminal;

namespace KingdomRenderer.Shared.ArchieV1.System.IO
{
    public static class File2
    {
        private const string Category = "System.IO.File2";
        
        public static bool SetReadWritePermissions(string path)
        {
            ULogger.ULog($"Setting ReadWrite permissions for all users to: {path}", Category);
            // Doesn't make sense on windows so just do nothing
            Process process = Terminal.RunCommand($"chmod o+rw '{path}'", "dir");
            process.WaitForExit();
            
            return process.ExitCode == 0;
        }
    }
}