using Newtonsoft.Json;
using System.IO;

namespace Blaze.Utils.Managers
{
    public static class JsonManager
    {
        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            TextWriter writer = null;
            try
            {
                writer = new StreamWriter(filePath, append);
                writer.Write(JsonConvert.SerializeObject(objectToWrite, Formatting.Indented));
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        public static T ReadFromJsonFile<T>(string filePath)
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
}
