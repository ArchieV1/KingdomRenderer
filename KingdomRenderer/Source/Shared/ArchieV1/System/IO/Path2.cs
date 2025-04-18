using System.Linq;
using KingdomRenderer.Shared.ArchieV1.Debug;

namespace KingdomRenderer.Shared.ArchieV1.System.IO
{
    /// <summary>
    /// Extension methods for <see cref="Path"/>
    /// </summary>
    public static class Path2
    {
        private const string Category = "System.IO.Path2";
        
        /// <summary>
        /// Replaces Windows specific path separators with generic path separators.
        /// </summary>
        /// <param name="path">The path to replace separators.</param>
        /// <returns>The new path.</returns>
        public static string ToGenericPath(string path)
        {
            return path.Replace(OS.WindowsSeparator, OS.GenericSeparator);
        }

        // TODO this is dubious at best
        public static char[] GetInvalidDirectoryNameChars()
        {
            return new char[] { OS.GetDirectorySeparatorChar() };
        }
        
        public static string GetParentPath(string path)
        {
            var pathParts = path.Split(OS.GetDirectorySeparatorChar());
            return string.Join(OS.GetDirectorySeparator(), pathParts.Take(pathParts.Length - 1)) ;
        }
    }
}