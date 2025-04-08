using System;
using System.Diagnostics;
using KingdomRenderer.Shared.ArchieV1.Debug.Terminal;

namespace KingdomRenderer.Shared.ArchieV1.System.IO
{
    /// <summary>
    /// Enables basic CRUD operations on Directories without using System.IO
    /// </summary>
    public static class Directory
    {
        /// <summary>
        /// Creates folder at specified path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="directoryName">Name of new directory without any slashes</param>
        /// <returns>The path of the new folder</returns>
        public static string TryCreate(string path, string directoryName)
        {
            ULogger.ULog($"Creating Directory at: {path}/{directoryName}", "System.IO.Directory");
            string command = $"cd \"{path}\" && mkdir \"{directoryName}\"";
            
            Process process = Terminal.RunCommand(command);
            process.WaitForExit();
            
            if (process.ExitCode != 0)
            {
                return string.Empty;
            }
            
            return Path.Combine(path, directoryName);
        }

        public static bool Exists(string directoryPath)
        {
            return Terminal.TestCommand($"-d \"{directoryPath}\"", $"EXIST \"{directoryPath}\"");
        }

        public static string GetCurrentDirectory()
        {
            Process process = Terminal.RunCommand("pwd", "cd");
            process.WaitForExit();
            throw new Exception("Not allowed StandardOutput as uses System.IO. TODO find work around");
            return process.StandardOutput.ReadToEnd();
        }

        public static string Pwd()
        {
            return GetCurrentDirectory();
        }
    }
}