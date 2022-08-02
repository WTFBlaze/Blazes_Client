using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Utils.Managers
{
    internal static class FileManager
    {
        internal static void CreateFile(string location)
        {
            File.Create(location).Close();
        }

        internal static void DeleteFile(string location)
        {
            File.Delete(location);
        }

        internal static void AppendLineToFile(string location, string text)
        {
            using FileStream file = new FileStream(location, FileMode.Append, FileAccess.Write, FileShare.Read);
            using StreamWriter writer = new StreamWriter(file, Encoding.Unicode);
            writer.Write(text + "\n");
        }

        internal static void AppendTextToFile(string location, string text)
        {
            using FileStream file = new FileStream(location, FileMode.Append, FileAccess.Write, FileShare.Read);
            using StreamWriter writer = new StreamWriter(file, Encoding.Unicode);
            writer.Write(text);
        }

        internal static void WriteAllToFile(string location, string text)
        {
            using var file = new FileStream(location, FileMode.Open, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(file, Encoding.Unicode);
            writer.Write(text);
        }

        internal static void WriteAllBytesToFile(string location, byte[] bytes)
        {
            File.WriteAllBytes(location, bytes);
        }

        internal static void WipeTextFromFile(string location)
        {
            File.WriteAllText(location, string.Empty);
        }

        internal static void CopyFile(string originalFile, string newFile)
        {
            File.Copy(originalFile, newFile);
        }

        internal static void RenameFile(string file, string newName)
        {
            File.Move(file, newName);
        }

        internal static string ReadAllOfFile(string location)
        {
            return File.ReadAllText(location);
        }

        internal static string[] ReadAllLinesOfFile(string location)
        {
            return File.ReadAllLines(location);
        }

        internal static byte[] ReadAllBytesOfFile(string location)
        {
            return File.ReadAllBytes(location);
        }
    }
}
