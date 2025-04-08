using System;
using System.Diagnostics;
using KingdomRenderer.Shared.ArchieV1.Debug.Terminal;

namespace KingdomRenderer.Shared.ArchieV1.System.IO
{
    /// <summary>
    /// Enables basic CRUD operations on Files without using System.IO
    /// </summary>
    public static class File
    {
        /// <summary>
        /// Creates a file at the given location with no contents.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True if file creation succeeded</returns>
        public static bool Create(string path)
        {
            ULogger.ULog($"Creating File: {path}", "System.IO.File");
            Process process = Terminal.RunCommand($"touch \"{path}\"", $"type NUL > \"{path}\"");
            process.WaitForExit();

            return process.ExitCode == 0;
        }

        /// <summary>
        /// Reads the file at the given location.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The text content of the given file.</returns>
        public static string Read(string path)
        {
            Process process = Terminal.RunCommand($"cat \"{path}\"", $"type \"{path}\"");
            process.WaitForExit();
            // TODO fix this ref error
            //process.StandardOutput.ReadToEnd();
            // Can probably be done with an unmanaged section of code reading a memory stream? Or unity
            throw new Exception("Cannot read files as System.IO is not allowed. TODO find work around");
        }
        
        /// <summary>
        /// Updates (Replaces) the contents of a given file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newContent"></param>
        /// <returns>True if update succeeded</returns>
        public static bool Update(string path, string newContent)
        {
            string command = $"echo \"{newContent}\" > \"{path}\"";
            
            Process process = Terminal.RunCommand(command);
            process.WaitForExit();

            return process.ExitCode == 0;
        }

        /// <summary>
        /// Appends the contents of the given file with <paramref name="newContent"/> <br />
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newContent"></param>
        /// <param name="newLine"></param>
        /// <returns></returns>
        public static bool Append(string path, string newContent, bool newLine = true)
        {
            if (newLine)
            {
                newContent = @"\n" + newContent;
            }
            
            string echoList = "( ";
            foreach (var section in newContent.Split('\n'))
            {
                echoList += $"echo {section} &";
            }
            
            echoList = echoList.TrimEnd('&');
            echoList += " )";
                
            Process process = Terminal.RunCommand($"echo -e \"{newContent}\" >> \"{path}\"", $"{echoList} >> \"{path}\"");
            process.WaitForExit();
            return process.ExitCode == 0;
        }

        public static bool Rename(string path, string newName)
        {
            string dirPath = Path.GetDirectoryName(path);
            return Move(path, dirPath);
        }

        public static bool Move(string filePath, string newDirectoryPath)
        {
            string fileName = Path.GetFileName(filePath);
            string newPath = Path.Combine(newDirectoryPath, fileName);

            Process process = Terminal.RunCommand($"mv \"{filePath}\" \"{newPath}\"", $"MOVE \"{filePath}\" \"{newPath}\"");
            process.WaitForExit();
            return process.ExitCode == 0;
        }

        public static bool Exists(string directoryPath)
        {
            return Terminal.TestCommand($"-e \"{directoryPath}\"", $"EXIST \"{directoryPath}\"");
        }
        
        /// <summary>
        /// Deletes the file at the given location
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True if deletion succeeded</returns>
        public static bool Delete(string path)
        {
            Process process = Terminal.RunCommand($"rm \"{path}\"", $"del \"{path}\"");
            process.WaitForExit();
            return process.ExitCode == 0;
        }
    }
}