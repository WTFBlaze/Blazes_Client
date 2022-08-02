using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blaze.Utils.Managers
{
    internal static class ImageManager
    {
        internal static byte[] TextureToBytes(Texture2D tex)
        {
            return ImageConversion.EncodeToPNG(tex);
        }

        internal static Texture2D ColorToTexture(Color color)
        {
            Texture2D texture2D = new(1, 1);
            texture2D.SetPixels(new Color[]
            {
                color
            });
            texture2D.Apply();
            return texture2D;
        }
    }
}
