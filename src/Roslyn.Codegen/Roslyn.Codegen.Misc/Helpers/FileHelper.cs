using System.IO;

namespace Roslyn.Codegen.Engine
{
    public static class FileHelper
    {
        /// <summary>
        /// Create/replace generated file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileBody"></param>
        public static void GenerateFile(string fileName, string fileBody)
        {
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Roslyn.Codegen");
            path = Path.Combine(path, "Generated");
            path = Path.Combine(path, $"{fileName}.cs");

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var file = File.CreateText(path))
            {
                file.WriteLine(fileBody);
            }
        }
    }
}
