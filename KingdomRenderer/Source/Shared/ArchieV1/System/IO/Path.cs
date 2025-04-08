using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KingdomRenderer.Shared.ArchieV1.Debug;

namespace KingdomRenderer.Shared.ArchieV1.System.IO
{
    /// <summary>
    /// A simple custom implementation of System.IO's Path <br />
    /// This implementation does not include of System.IO's overloads <br />
    /// </summary>
    /// <remarks>WARNING: This does not have any error checking. Make sure your inputs are valid</remarks>
    public class Path
    {
        private static readonly bool RunningUnixLike = OS.RunningUnixLike();
        
        /// <summary>
        /// Changes the extension of the given file to the newExtension. 
        /// </summary>
        /// <param name="file">The file</param>
        /// <param name="newExtension"></param>
        /// <returns></returns>
        public static string ChangeExtension(string file, string newExtension)
        {
            string filename = GetFileName(file);
            string extension = GetExtension(filename);
            
            return filename.Replace(extension, newExtension);
        }

        /// <summary>
        /// Combines the given paths with OS specific directory separators.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string Combine(params string[] paths)
        {
            return CombineParts(paths);
        }

        /// <summary>
        /// Indicates whether the given path ends in a directory separator.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool EndsInDirectorySeparator(string path)
        {
            return path.Substring(OS.GetDirectorySeparator().Length) == OS.GetDirectorySeparator();
        }

        /// <summary>
        /// Indicates whether the given file/directory exists on disk.
        /// </summary>
        /// <param name="path">Path to the file/directory.</param>
        /// <returns>True if the file/directory exists on disk.</returns>
        public static bool Exists(string path)
        {
            if (HasExtension(path))
            {
                return File.Exists(path);
            }
            return Directory.Exists(path);
        }
        
        /// <summary>
        /// Gets the path to the parent directory if a file or the current directory if a directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string path)
        {
            var pathParts = GetPathParts(path);
            if (HasExtension(path))
            {
                pathParts = RemovePathParts(pathParts, 1);
            }

            return CombineParts(pathParts);
        }
        
        /// <summary>
        /// Gets the extension of the given file. <br />
        /// A file in form "file.tar.gz" with return "tar.gz"
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetExtension(string filename)
        {
            IEnumerable<string> fileParts = filename.Split('.');
            IEnumerable<string> extensions = fileParts.Skip(1);
            return string.Join(".", extensions);
        }
        
        /// <summary>
        /// Gets the name of the file, including extensions, for the given path
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>The name of the file, including extensions.</returns>
        public static string GetFileName(string filePath)
        {
            return GetPathParts(filePath).Last();
        }

        /// <summary>
        /// Get the filename of the file without the extension.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtension(string filePath)
        {
            string filename = GetFileName(filePath);
            string extension = GetExtension(filename);
            
            return filename.Replace($".{extension}", string.Empty);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFullPath(string path)
        {
            // TODO get current dir
            if (IsPathFullyQualified(path))
            {
                return path;
            }

            List<string> pathParts = GetPathParts(path).ToList();
            pathParts.Add(Directory.GetCurrentDirectory());
            return CombineParts(pathParts);
        }

        /// <summary>
        /// Returns an array of invalid characters for filenames on the current OS. 
        /// </summary>
        /// <returns></returns>
        public static char[] GetInvalidFileNameChars()
        {
            if (RunningUnixLike)
            {
                return new [] { '\0', '/' };
            }
            return new []
            {
                '\"', '<', '>', '|', '\0',
                (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
                (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
                (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
                (char)31, ':', '*', '?', '\\', '/'
            };
        }

        /// <summary>
        /// Returns an array of invalid characters for paths on the current OS.
        /// </summary>
        /// <returns></returns>
        public static char[] GetInvalidPathChars()
        {
            if (RunningUnixLike)
            {
                return new [] { '\0' };
            }
            return new []
            {
                '|', '\0',
                (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
                (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
                (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
                (char)31
            };
        }

        /// <summary>
        /// Returns string.empty if path does not have a root.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPathRoot(string path)
        {
            if (RunningUnixLike)
            {
                // "/" (Unix: path specified an absolute path on the current drive).
                return path.Substring(0, 1) == "/" ? "/" : string.Empty;
            }

            // "X:" (Windows: path specified a relative path on a drive, where X represents a drive or volume letter).
            Match match = Regex.Match(path, @"^\w:");
            if (match.Success)
            {
                return match.Value;
            }
            
            // "X:\" (Windows: path specified an absolute path on a given drive).
            match = Regex.Match(path, @"^\w:\\");
            if (match.Success)
            {
                return match.Value;
            }
            
            // "\ComputerName\SharedFolder" (Windows: a UNC path).
            match = Regex.Match(path, @"^\\\\\w*\\\w*");
            if (match.Success)
            {
                return match.Value;
            }
            
            // "\?\C:" (Windows: a DOS device path, supported in .NET Core 1.1 and later versions, and in .NET Framework
            // 4.6.2 and later versions).
            
            // Examples:
            // \\.\C:\Test\Foo.txt
            // \\?\C:\Test\Foo.txt
            match = Regex.Match(path, @"^\\\\[\. \?]\\\w:\\");
            if (match.Success)
            {
                return match.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetRandomFileName(string extension)
        {
            // The default method you don't pass an extension
            return Guid.NewGuid() + extension;
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="relativeTo"></param>
        /// <param name="toPath"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string GetRelativePath(string relativeTo, string toPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetTempFileName()
        {
            return Combine(GetTempPath(), GetRandomFileName(".tmp"));
        }

        /// <summary>
        /// Get the path to the Temp directory.
        /// </summary>
        /// <returns></returns>
        public static string GetTempPath()
        {
            string tempDir = Environment.GetEnvironmentVariable("TMPDIR")
                             ?? Environment.GetEnvironmentVariable("TEMP")
                             ?? Environment.GetEnvironmentVariable("TMP");

            if (string.IsNullOrEmpty(tempDir))
            {
                if (RunningUnixLike)
                {
                    return "/tmp";
                }

                return @"C:\Temp";
            }
            
            return tempDir;
        }

        /// <summary>
        /// Returns a value that indicates whether a path has an extension. <br />
        /// An extension is any file with a suffix in form".xyz" 
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns>True if the path has an extension.</returns>
        public static bool HasExtension(string path)
        {
            return GetExtension(path).Length > 0;
        }

        /// <summary>
        /// Returns a value that indicates whether a path is fully qualified. <br />
        /// The path starts at the root directory. It is absolute.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsPathFullyQualified(string path)
        {
            return GetPathRoot(path) != string.Empty;
        }

        /// <summary>
        /// Returns a value that indicates whether a file path contains a root. <br />
        /// May not always be fully qualified. Can be relative paths.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns>True if the path contains a root.</returns>
        public static bool IsPathRooted(string path)
        {
            if (IsPathFullyQualified(path))
            {
                return true;
            }

            return path.Substring(OS.GetDirectorySeparator().Length) == OS.GetDirectorySeparator();
        }

        /// <summary>
        /// Joins the given paths. <br />
        /// Note: Identical to <see cref="Path.Combine"/> in this implementation.
        /// </summary>
        /// <param name="paths">The paths to join.</param>
        /// <returns>The joined paths.</returns>
        public static string Join(params string[] paths)
        {
            return Combine(paths);
        }

        /// <summary>
        /// Removes any trailing directory separators at the end of the path.
        /// </summary>
        /// <param name="path">The path to trim.</param>
        /// <returns>The trimmed path.</returns>
        public static string TrimEndingDirectorySeparator(string path)
        {
            string pattern = $"{OS.GetDirectorySeparator()}*$";
            return Regex.Replace(path, pattern, string.Empty);
        }

        /// <summary>
        /// Not Implemented.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string TryJoin(params string[] paths)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<string> GetPathParts(string path)
        {
            return path.Split('/', '\\');
        }

        private static IEnumerable<string> RemovePathParts(IEnumerable<string> pathParts, int numberParts)
        {
            var list = pathParts.ToList();
            return list.Take(list.Count() - numberParts);
        }

        private static string CombineParts(IEnumerable<string> pathParts)
        {
            var separator = OS.GetDirectorySeparator();
            return string.Join(separator, pathParts);
        }
    }
}