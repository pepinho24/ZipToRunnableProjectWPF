namespace ZipToRunnableProjectWPF.Common
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Web; // You may need to add a reference to System.Web.dll

    /// <summary>
    /// Author: Svetlin Nakov
    /// URL: https://nakov.com/blog/2009/07/14/universal-relative-to-physical-path-resolver-for-console-wpf-and-aspnet-apps/
    /// </summary>
    public class UniversalFilePathResolver
    {
        /// <summary>
        /// Resolves a relative path starting with tilde to a physical file system path. In Web application
        /// scenario the "~" denotes the root of the Web application. In desktop application scenario (e.g.
        /// Windows Forms) the "~" denotes the directory where the currently executing assembly is located
        /// excluding "\bin\Debug" and "\bin\Release" folders (if present).
        ///
        /// For example: the path "~\config\example.txt" will be resolved to a physical path like
        /// "C:\Projects\MyProject\config\example.txt".
        ///
        /// </summary>
        /// <param name="relativePath">the relative path to the resource starting with "~"</param>
        /// <returns>Full physical path to the specified resource.</returns>
        public static string ResolvePath(string relativePath)
        {
            if (relativePath == null || !relativePath.StartsWith("~"))
            {
                throw new ArgumentException("The path '" + relativePath +
                    "' should be relative path and should start with '~'");
            }

            HttpContext httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                // We are in a Web application --> use Server.MapPath to get the physical path
                string fullPath = httpContext.Server.MapPath(relativePath);
                return fullPath;
            }
            else
            {
                // We are in a console / Windows desktop application -->
                // use currently executing assembly directory to find the full path
                Assembly assembly = Assembly.GetExecutingAssembly();
                string assemblyDir = assembly.CodeBase;
                assemblyDir = assemblyDir.Replace("file:///", "");
                assemblyDir = Path.GetDirectoryName(assemblyDir);

                // Remove "bin\debug" and "bin\release" directories from the path
                string applicationDir = RemoveStringAtEnd(@"\bin\debug", assemblyDir);
                applicationDir = RemoveStringAtEnd(@"\bin\release", applicationDir);

                string fullPath = relativePath.Replace("~", applicationDir);
                return fullPath;
            }
        }

        private static string RemoveStringAtEnd(string searchStr, string targetStr)
        {
            if (targetStr.ToLower().EndsWith(searchStr.ToLower()))
            {
                string resultStr = targetStr.Substring(0, targetStr.Length - searchStr.Length);
                return resultStr;
            }
            return targetStr;
        }
    }
}
