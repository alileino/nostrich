using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Tile
    {
        protected static int _lastBoundtexture = -1;

        protected Texture _texture;
        protected float _texOffsetX = 0, _texOffsetY = 0;
        protected float _texWidth = 0, _texHeight = 0;
        protected int _animNum = 1;
        protected double _curAnim = 0;
        protected double _animSpeed = 1;

        protected float _width, _height;

        public string Name { get; private set; }
        public float Width { get { return _width; } }
        public float Height { get { return _height; } }

        public TileData TileData
        {
            get;
            set;
            //{
            //    return new TileData()
            //    {
            //        Name = Name,
            //        Width = _texture.ToPixelOffsetX(_width),
            //        Height = _texture.ToPixelOffsetY(_height),
            //        OffsetX = _texture.ToPixelOffsetX(_texOffsetX),
            //        OffsetY = _texture.ToPixelOffsetY(_texOffsetY),
            //        AnimNum = _animNum,
            //        AnimSpeed = _animSpeed
            //    };
            //}
        }

        public Tile(Texture texture, TileData tileData)
        {
            _texture = texture;

            _width = tileData.Width / GameConstants.TILE_SIZEf;
            _height = tileData.Height / GameConstants.TILE_SIZEf;
            Name = tileData.Name;
            _texOffsetX = texture.NormalizeOffsetX(tileData.OffsetX);
            _texOffsetY = texture.NormalizeOffsetY(tileData.OffsetY);
            _texWidth = texture.NormalizeOffsetX(tileData.Width);
            _texHeight = texture.NormalizeOffsetY(tileData.Height);
            _animNum = tileData.AnimNum;
            _animSpeed = tileData.AnimSpeed;
            TileData = tileData;
        }


        [Obsolete]
        public Tile(Texture texture, string name, int offsetX, int offsetY, int width, int height, int animNum, double animSpeed)
        {
            _width = width / (float)GameConstants.TILE_SIZE;
            _height = height / (float)GameConstants.TILE_SIZE;
            Name = name;
            _texture = texture;
            _texOffsetX = texture.NormalizeOffsetX(offsetX);
            _texOffsetY = texture.NormalizeOffsetY(offsetY);
            _texWidth = texture.NormalizeOffsetX(width);
            _texHeight = texture.NormalizeOffsetY(height);
            _animNum = animNum;
            _animSpeed = animSpeed;
        }

        public void Draw(Vector3 offset, double ticksPassed)
        {
            Draw(offset.X, offset.Y, ticksPassed, _width, _height, _texOffsetX, _texOffsetY, offset.Z);
        }

        protected void Draw(float xOffset, float yOffset, double ticksPassed, float width, float height, float texOffsetX, float texOffsetY, float z=0f)
        {
            AdvanceAnimation(ticksPassed);
            Vector3 offset = VectorExtensions.LevelToGraphicsCoordinates(xOffset, yOffset);
            offset.Z = z;
            float finalTexOffsetX =  _texOffsetX +((int)_curAnim) * _texWidth;
            GL.Enable(EnableCap.Texture2D);
            if (_lastBoundtexture != _texture.TextureId)
            {
                GL.BindTexture(TextureTarget.Texture2D, _texture.TextureId);
                _lastBoundtexture = _texture.TextureId;
            }
            
            GL.Begin(BeginMode.Quads);
            {
                GL.TexCoord2(finalTexOffsetX, _texOffsetY + _texHeight);
                GL.Vertex3(offset);

                GL.TexCoord2(finalTexOffsetX + _texWidth, texOffsetY + _texHeight);
                GL.Vertex3(offset.X + width, offset.Y,offset.Z);

                GL.TexCoord2(finalTexOffsetX + _texWidth, texOffsetY);
                GL.Vertex3(offset.X + width, offset.Y + height,offset.Z);

                GL.TexCoord2(finalTexOffsetX, texOffsetY);
                GL.Vertex3(offset.X, offset.Y + height,offset.Z);
            }
            GL.End();
        }

        private void AdvanceAnimation(double ticksPassed)
        {
            _curAnim += ticksPassed * _animSpeed*GameConstants.ANIM_SPEED_MOD;
            _curAnim -= (((int)(_curAnim)) / _animNum) * _animNum; // poor man's modulo
        }

    }

    public class TileData
    {
        public string Name { get; set; }
        public bool Animated { get { return AnimNum != 1; } }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int AnimNum { get; set; }
        public double AnimSpeed { get; set; }

        public TileData(string name = "", int offsetX = 0, int offsetY = 0, int width = 0, int height = 0, int animNum = 1, double animSpeed = 0.0)
        {
            Name = name;
            Width = width;
            Height = height;

            OffsetX = offsetX;
            OffsetY = offsetY;
            AnimNum = animNum;
            AnimSpeed = animSpeed;
        }
    }

}
