using System.Drawing.Imaging;
using System.Drawing;
using UnityEngine;
using System.IO;
using System.Net;

namespace Blaze.Utils.Managers
{
    /*
     
        Credits: MayuLayuPayu
        Link: https://github.com/MayuLayuPayu/Gifify

     */
    public static class GifManager
    {
        public static Texture2D[] GetGifFrameFromURL(string url)
        {
            WebClient wc = new WebClient();
            Image[] images = getFrames(Image.FromStream(new MemoryStream(wc.DownloadData(url))));
            Texture2D[] frames = new Texture2D[images.Length];
            for (int i = 0; i < images.Length; i++)
            {
                MemoryStream memoryStream = new MemoryStream();
                images[i].Save(memoryStream, ImageFormat.Png);
                var texture = new Texture2D(1, 1);
                ImageConversion.LoadImage(texture, memoryStream.ToArray());
                texture.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                texture.wrapMode = TextureWrapMode.Clamp;
                frames[i] = texture;
            }

            return frames;
        }
        public static Texture2D[] GetGifFrameFromMemory(byte[] bytes)
        {
            Image[] images = getFrames(Image.FromStream(new MemoryStream(bytes)));
            Texture2D[] frames = new Texture2D[images.Length];
            for (int i = 0; i < images.Length; i++)
            {
                MemoryStream memoryStream = new MemoryStream();
                images[i].Save(memoryStream, ImageFormat.Png);
                var texture = new Texture2D(1, 1);
                ImageConversion.LoadImage(texture, memoryStream.ToArray());
                texture.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                texture.wrapMode = TextureWrapMode.Clamp;
                frames[i] = texture;
            }
            return frames;
        }
        public static Texture2D[] GetGifFrameFromFile(string filepath)
        {
            Image[] images = getFrames(Image.FromFile(filepath));
            Texture2D[] frames = new Texture2D[images.Length];
            for (int i = 0; i < images.Length; i++)
            {
                MemoryStream memoryStream = new MemoryStream();
                images[i].Save(memoryStream, ImageFormat.Png);
                var texture = new Texture2D(1, 1);
                ImageConversion.LoadImage(texture, memoryStream.ToArray());
                texture.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                texture.wrapMode = TextureWrapMode.Clamp;
                frames[i] = texture;
            }
            return frames;
        }
        private static Image[] getFrames(Image originalImg)
        {
            int numberOfFrames = originalImg.GetFrameCount(FrameDimension.Time);
            Image[] frames = new Image[numberOfFrames];

            for (int i = 0; i < numberOfFrames; i++)
            {
                originalImg.SelectActiveFrame(FrameDimension.Time, i);
                frames[i] = ((Image)originalImg.Clone());
            }

            return frames;
        }
    }
}
