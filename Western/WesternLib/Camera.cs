using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Camera
    {
        readonly int TILE_SIZE = 32;
        Vector3 _position = new Vector3(0,0,0);
        int _screenWidth = 800;
        int _screenHeight = 600;

        private float HorizontalTiles { get { return _screenWidth / (float)TILE_SIZE; } }
        private float VerticalTiles { get { return _screenHeight / (float)TILE_SIZE; } }

        public Vector3 Position { get { return _position; } set { _position = value; } }
        public Vector4 Viewport
        {
            get
            {
                return new Vector4(-HorizontalTiles / 2f, HorizontalTiles / 2f, -VerticalTiles / 2f, VerticalTiles / 2f);
            }
        }
        public int ScreenWidth { get { return _screenWidth; } }
        public int ScreenHeight { get { return _screenHeight; } }

        public Rectangle VisibleRectangle
        {
            get
            {
                Rectangle rect = new Rectangle {
                    X =  (int)(_position.X - HorizontalTiles/2f),
                    Y =  (int)(-_position.Y - VerticalTiles/2f),
                    Width=(int)HorizontalTiles+2,
                    Height=(int)VerticalTiles+2
                };
                return rect;
            }
        }


        public void LookAt(float x, float y)
        {
            _position.X = x;
            _position.Y = y;
        }

        public void LookAt(Vector2 v)
        {
            LookAt(v.X, -v.Y);
        }
        public void Translate()
        {
            GL.Translate(-_position);
        }

        public void Offset(Vector3 delta)
        {
            _position += delta;
        }

        public void Offset(float x, float y)
        {
            _position.X += x;
            _position.Y += y;
        }

        public void SetScreenDimensions(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;
            Ready3D();
        }

        public void Ready3D()
        {
            GL.Viewport(0, 0, _screenWidth, _screenHeight);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.AlphaFunc(AlphaFunction.Greater, 0.24f);
            GL.Enable(EnableCap.AlphaTest);
            
            GL.Ortho(Viewport.X, Viewport.Y, Viewport.Z, Viewport.W, 100.0, -100.0);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void Ready2D()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.Ortho(0.0f, _screenWidth, _screenHeight, 0 ,100, -100.0);


            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        public Vector2 ToTileCoordinates(Point screenLocation)
        {
            return ToTileCoordinates(screenLocation.X, screenLocation.Y);
        }

        public Vector2 ToTileCoordinates(int x, int y)
        {
            Vector2 v = new Vector2(x, y);
            v *= 1 / GameConstants.TILE_SIZEf;
            v += new Vector2(_position.X - HorizontalTiles / 2f, -(_position.Y + VerticalTiles / 2f));

            return v;
        }

        public Vector2 ScreenToGLTileOffset(int x, int y)
        {
            //float xf = Viewport.Y/2f - (int)(Viewport.Y/2f);
            float xo = x / GameConstants.TILE_SIZEf;
            xo = (int)xo + Viewport.X;
            float yo = (int)(-(y) / GameConstants.TILE_SIZEf) + Viewport.W;
            return new Vector2(xo,yo);
        }


        public Vector2 ScreenToGraphicCoordinates(int x, int y)
        {
            float xo = x / GameConstants.TILE_SIZEf + Viewport.X;
            float yo = y / GameConstants.TILE_SIZEf + Viewport.Z;
            return new Vector2(xo, yo);
        }
    }

    public static class RectangleExtensions
    {
        public static Rectangle Parse(string value)
        {
            int[] v = (from x in value.Split(',') select int.Parse(x)).ToArray<int>();
            if (v.Length < 4)
                throw new NotImplementedException();
            return new Rectangle(v[0], v[1], v[2], v[3]);
        }

        public static bool Contains(this Rectangle rect, float x, float y)
        {
            return rect.Contains((int)x, (int)y);
        }

        public static Rectangle ClampToRange(this Rectangle rect, int x1, int y1, int x2, int y2)
        {
            if (rect.X < x1)
            {
                rect.Width += rect.X;
                rect.X = 0;
            }
            if (rect.Y < y1)
            {
                rect.Height += rect.Y;
                rect.Y = 0;
            }
            if (rect.Width + rect.X > x2)
                rect.Width = x2 - rect.X;
            if (rect.Height + rect.Y > y2)
                rect.Height = y2 - rect.Y;
            return rect;
        }
        public static Box2 LevelToGraphicsCoordinates(this Box2 box)
        {
            box.Top = -box.Top;
            box.Bottom = -box.Bottom;
            return box;
        }

        public static Box2 ToBox2(this Rectangle rect)
        {
            return Box2.FromTLRB(rect.Top, rect.Left, rect.Right, rect.Bottom);
        }

        public static Vector2 Center(this Box2 box)
        {
            return new Vector2((box.Left + box.Right) / 2, (box.Top + box.Bottom) / 2);
        }

        public static bool Collides(this Box2 left, Box2 right)
        {
            return !(left.Top <= right.Bottom    || 
            left.Bottom >= right.Top ||
            left.Right <= right.Left   ||
            left.Left >= right.Right);
        }
    }

    public static class PointExtensions
    {
        public static Point Parse(string value)
        {
            string[] split = value.Split(',');
            return new Point(int.Parse(split[0]), int.Parse(split[1]));
        }

        public static string ToString(this Point p, char separator)
        {
            return p.X.ToString() + separator + p.Y;
        }

        public static Point ClampToRange(this Point p, int minX, int minY, int maxX, int maxY)
        {
            if (p.X < minX)
                p.X = minX;
            if (p.Y < minY)
                p.Y = minY;
            if (p.X >= maxX)
                p.X = maxX-1;
            if(p.Y >= maxY)
                p.Y = maxY-1;
            return p;
        }

    }

    public static class VectorExtensions
    {
        public static Box2 Displace(this Box2 box, Point p)
        {
            return Displace(box, p.X, p.Y);
        }

        public static Box2 Displace(this Box2 box, Vector2 v)
        {
            return Displace(box, v.X, v.Y);
        }

        public static Box2 Displace(this Box2 box, float x, float y)
        {
            box.Left += x;
            box.Right += x;
            box.Top += y;
            box.Bottom += y;
            return box;
        }

        public static int RoundToLower(float f)
        {
            return f > 0 ? (int)f : (int)(f - 1);
        }
        public static Point ToPoint(this Vector2 v)
        {
            return new Point(RoundToLower(v.X), RoundToLower(v.Y));
        }

        public static Vector2 ToVector2(this Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }


        public static Vector3 ToVector3(this Vector2 v)
        {
            return new Vector3(v.X, v.Y, 0f);
        }

        public static double Angle(this Vector2 v)
        {
            double angle = Math.Acos(v.X);
            if (v.Y > 0)
                angle = 2 * Math.PI - angle;
            return angle;
        }

        public static Vector2 GetNormal(this Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }

        public static Vector3 LevelToGraphicsCoordinates(this Vector2 tileCoordinates)
        {
            //tileCoordinates.Y = -tileCoordinates.Y;
            return LevelToGraphicsCoordinates(tileCoordinates.X, tileCoordinates.Y);
        }

        public static Vector3 LevelToGraphicsCoordinates(float x, float y)
        {
            return new Vector3(x, -y, 0);
        }



        public static bool In(this Vector2 v, Rectangle rect)
        {
            if (v.X < rect.X || v.X > rect.X + rect.Width || v.Y < rect.Y || v.Y > rect.Y + rect.Height)
                return false;
            return true;
        }
    }
}
