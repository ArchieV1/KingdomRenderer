using System;
using System.Diagnostics;

namespace KingdomRenderer.Shared.ArchieV1.Debug.Terminal
{
    public static class Terminal
    {
        public const int TrueCode = 42;
        public const int FalseCode = 43;
        public static readonly bool RunningUnixLike = OS.RunningUnixLike();

        /// <summary>
        /// Runs an IF/ELSE statement on the given test using exist codes to determine true/false.
        /// Runs this command on a Unix-Like environment using bash.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="success"></param>
        /// <param name="failed"></param>
        /// <returns></returns>
        private static bool TestCommandUnix(string test, int success = TrueCode, int failed = FalseCode)
        {
            ULogger.ULog($"Running Test: {test}", "System.IO.Terminal");
            string command = $"[ {test} ] && exit {success} || exit {failed}";
            
            Process process = RunCommandUnix(command);
            process.WaitForExit();
            
            return process.IsTrueCode();
        }

        /// <summary>
        /// Runs an IF/ELSE statement on the given test using exit codes to determine true/false.
        /// Runs this command on a Windows environment using CMD
        /// </summary>
        /// <param name="test"></param>
        /// <param name="success"></param>
        /// <param name="failed"></param>
        /// <returns></returns>
        private static bool TestCommandWindows(string test, int success = TrueCode, int failed = FalseCode)
        {
            ULogger.ULog($"Running Test: {test}", "System.IO.Terminal");
            string command = $"IF {test} (EXIT /B {success}) ELSE (EXIT /B {failed}))";
            
            Process process = RunCommandWindows(command);
            process.WaitForExit();
            
            return process.IsTrueCode();
        }

        public static bool TestCommand(string test, int success = TrueCode, int failed = FalseCode)
        {
            if (RunningUnixLike)
            {
                return TestCommandUnix(test, success, failed);
            }
            return TestCommandWindows(test, success, failed);
        }

        public static bool TestCommand(string testUnix, string testWindows, int success = TrueCode,
            int failed = FalseCode)
        {
            if (RunningUnixLike)
            {
                return TestCommandUnix(testUnix, success, failed);
            }
            return TestCommandWindows(testWindows, success, failed);
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

        public static Process RunCommand(string commandBash, string commandCmd)
        {
            if (RunningUnixLike)
            {
                return RunCommandUnix(commandBash);
            }
            return RunCommandWindows(commandCmd);
        }

        public static bool IsTrueCode(this Process process, int successCode = TrueCode)
        {
            return process.ExitCode == successCode;
        }

        public static bool IsFalseCode(this Process process, int failedCode = FalseCode)
        {
            return process.ExitCode == failedCode;
        }
    }
}