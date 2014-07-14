using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Texture
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TextureId { get; private set; }
        public string Filename { get; private set; }

        /// <summary>
        /// Calculates the normalized offset in range [0,1] (which OpenGL uses).
        /// </summary>
        /// <param name="xOffset"></param>
        /// <returns></returns>
        public float NormalizeOffsetX(int xOffset)
        {
            return xOffset / (float)Width;
        }

        public float NormalizeOffsetY(int yOffset)
        {
            return yOffset / (float)Height;
        }

        public int ToPixelOffsetX(float xOffset)
        {
            return (int)xOffset * Width;
        }

        public int ToPixelOffsetY(float yOffset)
        {
            return (int)yOffset * Height;
        }

        public Texture(int textureId, int width, int height, string filename)
        {
            Width = width;
            Height = height;
            TextureId = textureId;
            Filename = filename;
        }
    }
}
