using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Blaze.Utils.Managers
{
    public static class ImageManager
    {
        public static byte[] TextureToBytes(Texture2D tex)
        {
            return ImageConversion.EncodeToPNG(tex);
        }

        public static Texture2D ColorToTexture(Color color)
        {
            Texture2D texture2D = new(1, 1);
            texture2D.SetPixels(new Color[]
            {
                color
            });
            texture2D.Apply();
            return texture2D;
        }

        public static Texture2D URLToTexture(string url)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Logs.Error($"There was a problem downloading image from url [{url}]");
                return null;
            }
            else
            {
                return ((DownloadHandlerTexture)request.downloadHandler).texture;
            }
        }

        public static Sprite URLToSprite(string url)
        {
            Texture2D tex = URLToTexture(url);
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        }
    }
}
