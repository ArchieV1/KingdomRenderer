using System.Diagnostics;

namespace KingdomRenderer.Shared.ArchieV1.Debug.Terminal
{
    public static class Terminal
    {
        public static readonly bool RunningUnixLike = OS.RunningUnixLike();

        /// <summary>
        /// Runs an IF/ELSE statement on the given test using exist codes to determine true/false.
        /// Runs this command on a Unix-Like environment using bash.
        /// </summary>
        /// <param name="test">Must contain quotes where needed</param>
        /// <returns></returns>
        /// <example>Test could be: -e "file/path/in/quotes"</example>
        private static bool TestCommandUnix(string test)
        {
            ULogger.ULog($"Running Test: {test}", "System.IO.Terminal");
            string command = $"test {test}";
            
            Process process = RunCommandUnix(command);
            process.WaitForExit();

            bool success = process.ExitCode == 0;
            
            ULogger.ULog($"Test succeeded: {success}", "System.IO.Terminal");
            
            return success;
        }

        /// <summary>
        /// Runs an IF/ELSE statement on the given test using exit codes to determine true/false.
        /// Runs this command on a Windows environment using CMD
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool TestCommandWindows(string test)
        {
            ULogger.ULog($"Running Test: {test}", "System.IO.Terminal");
            string command = $"IF {test}";
            
            Process process = RunCommandWindows(command);
            process.WaitForExit();
            
            bool success = process.ExitCode == 0;
            
            ULogger.ULog($"Test succeeded: {success}", "System.IO.Terminal");
            
            return success;
        }

        public static bool TestCommand(string test)
        {
            if (RunningUnixLike)
            {
                return TestCommandUnix(test);
            }
            return TestCommandWindows(test);
        }

        public static bool TestCommand(string testUnix, string testWindows)
        {
            if (RunningUnixLike)
            {
                return TestCommandUnix(testUnix);
            }
            return TestCommandWindows(testWindows);
        }

        private static Process RunCommandUnix(string command)
        {
            ULogger.ULog($"Running Command: {command}", "System.IO.Terminal");
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    Arguments = $"-c \"{command}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }
            };
            
            process.Start();

            return process;
        }

        private static Process RunCommandWindows(string command)
        {
            ULogger.ULog($"Running Command: {command}", "System.IO.Terminal");
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C \"{command}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }
            };
            
            process.Start();
                
            return process;
        }

        /// <summary>
        /// Runs the given command. It must be a command that can run in both CMD and Bash.
        /// </summary>
        /// <param name="command">A CMD and Bash compatible command.</param>
        /// <returns></returns>
        public static Process RunCommand(string command)
        {
            if (RunningUnixLike)
            {
                return RunCommandUnix(command);
            }
            return RunCommandWindows(command);
        }

        /// <summary>
        /// Runs a command on the current system's terminal
        /// NOTE: Windows uses CMD not Powershell
        /// NOTE: UnixLike use Bash
        /// </summary>
        /// <param name="commandBash">The command to run in bash</param>
        /// <param name="commandCmd">The command to run in bash</param>
        /// <returns></returns>
        public static Process RunCommand(string commandBash, string commandCmd)
        {
            if (RunningUnixLike)
            {
                return RunCommandUnix(commandBash);
            }
            
            return RunCommandWindows(commandCmd);
        }
    }
}