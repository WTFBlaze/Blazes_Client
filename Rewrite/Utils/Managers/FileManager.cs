using System.IO;
using System.Text;

namespace Blaze.Utils.Managers
{
    public static class FileManager
    {
        public static void CreateFile(string location)
        {
            File.Create(location).Close();
        }

        public static void DeleteFile(string location)
        {
            File.Delete(location);
        }

        public static void AppendLineToFile(string location, string text)
        {
            using FileStream file = new FileStream(location, FileMode.Append, FileAccess.Write, FileShare.Read);
            using StreamWriter writer = new StreamWriter(file, Encoding.Unicode);
            writer.Write(text + "\n");
        }

        public static void AppendTextToFile(string location, string text)
        {
            using FileStream file = new FileStream(location, FileMode.Append, FileAccess.Write, FileShare.Read);
            using StreamWriter writer = new StreamWriter(file, Encoding.Unicode);
            writer.Write(text);
        }

        public static void WriteAllToFile(string location, string text)
        {
            using var file = new FileStream(location, FileMode.Open, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(file, Encoding.Unicode);
            writer.Write(text);
        }

        public static void WriteAllBytesToFile(string location, byte[] bytes)
        {
            File.WriteAllBytes(location, bytes);
        }

        public static void WipeTextFromFile(string location)
        {
            File.WriteAllText(location, string.Empty);
        }

        public static void CopyFile(string originalFile, string newFile)
        {
            File.Copy(originalFile, newFile);
        }

        public static void RenameFile(string file, string newName)
        {
            File.Move(file, newName);
        }

        public static string ReadAllOfFile(string location)
        {
            return File.ReadAllText(location);
        }

        public static string[] ReadAllLinesOfFile(string location)
        {
            return File.ReadAllLines(location);
        }

        public static byte[] ReadAllBytesOfFile(string location)
        {
            return File.ReadAllBytes(location);
        }
    }
}
