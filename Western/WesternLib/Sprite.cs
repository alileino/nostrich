using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Sprite : Tile
    {

        private float _centerX, _centerY;

        public Sprite(Texture texture, TileData tileData, float originX, float originY)
            : base(texture,tileData)
        {
            _centerX = originX;
            _centerY = originY;
        }
        

        public void Draw(OpenTK.Vector3 offset, double ticksPassed, double forceAnim)
        {
            _curAnim = forceAnim;

            Draw(offset.X - _centerX / GameConstants.TILE_SIZEf,
            offset.Y + _centerY / GameConstants.TILE_SIZEf,
            ticksPassed,
            _width,
            _height,
            _texOffsetX,
            _texOffsetY,offset.Z);
        }

        public void DrawAbsolute(OpenTK.Vector3 offset)
        {
            DrawAbsolute(offset, 0);
        }

        public void DrawAbsolute(OpenTK.Vector3 offset, double ticksPassed)
        {
            Draw(offset.X - _centerX,
                -(offset.Y + _centerY),ticksPassed,
                _width * GameConstants.TILE_SIZEf,
                -_height * GameConstants.TILE_SIZEf,
                _texOffsetX,
                _texOffsetY,
                offset.Z);
        }

        public void Draw(float xOffset, float yOffset, double ticksPassed)
        {
            Draw(new Vector3(xOffset,yOffset,0), ticksPassed);

        }

        public new void Draw(Vector3 offset, double ticksPassed)
        {
            AdvanceAnimation(ticksPassed);
            Draw(offset, ticksPassed, _curAnim);
        }

        private void AdvanceAnimation(double ticksPassed)
        {
            _curAnim += ticksPassed * _animSpeed;
            _curAnim -= (((int)(_curAnim)) / _animNum) * _animNum; // poor man's modulo
        }

        internal Sprite Copy()
        {
            Sprite spr = new Sprite(_texture, TileData, _centerX, _centerY);
            return spr;
        }
    }
}
