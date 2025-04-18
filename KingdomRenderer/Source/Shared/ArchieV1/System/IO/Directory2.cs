using System;
using System.Diagnostics;
using KingdomRenderer.Shared.ArchieV1.Debug.Terminal;

namespace KingdomRenderer.Shared.ArchieV1.System.IO
{
    public static class Directory2
    {
        private const string Category = "System.IO.Directory2";
        
        /// <summary>
        /// Creates directories from the root to the requested directory if needed.
        /// Unlike CreateDirectory() there are no error if the directories already exist.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <exception cref="Exception"></exception>
        public static void TryCreate(string directoryPath)
        {
            ULogger.ULog($"Creating Directory at: {directoryPath}", Category);
            
            // Recursive mode
            string command = $"mkdir -p \"{directoryPath}\"";
            
            Process process = Terminal.RunCommand(command);
            process.WaitForExit();
            
            if (process.ExitCode != 0)
            {
                throw new Exception($"Directory Creation Failed: {command}");
            }
        }

        public static bool SetReadWritePermissions(string path)
        {
            return File2.SetReadWritePermissions(path);
        }
    }
}