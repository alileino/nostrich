using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Level : LevelData
    {
        static RenderingOptions _renderingOptins = new RenderingOptions();
        static Vector2 _dirT = new Vector2(1, 1);
        static Vector2 _dirS = new Vector2(1, -1);
        public static RenderingOptions RenderingOptions { get { return _renderingOptins; } set { _renderingOptins = value; } }
        PawnCreator _pawnCreator;

        public Level(ResourceManager resourceManager, PawnCreator pawnCreator)
            : base(resourceManager)
        {
            _pawnCreator = pawnCreator;
            _pawnCreator.CollidingPawnMoved += CanMove;
        }


        private Vector2 CanMove(Pawn sender, Vector2 displacement)
        {

            Vector2 A = Vector2.Multiply(_dirT,Vector2.Dot(displacement, 0.5f*_dirT));
            Vector2 B = Vector2.Multiply(_dirS,Vector2.Dot(displacement, 0.5f*_dirS));
            Point posAsPoint = (sender.Position + sender.CollideBox.Center()).ToPoint();

            bool collidesA = false;
            bool collidesB = false;
            bool collidesC = false;
            bool collidesD = false;
            Vector2 newPos = sender.Position + displacement;
            Box2 collideBox = sender.CollideBox.Displace(newPos);
            
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    int colNum = GetCollision(posAsPoint.X + dx, posAsPoint.Y + dy);

                    if (colNum != 0)
                    {
                        float colX = posAsPoint.X+dx;
                        float colY = posAsPoint.Y+dy;
                        if(!(collideBox.Top <= colY    || 
                            collideBox.Bottom >= colY+1 ||
                            collideBox.Right <= colX   ||
                            collideBox.Left >= colX + 1))
                        {
                            bool collides = false;
                            if ((colNum == 1) ||
                                   (colNum == 2 && (dx == 1 || dy == 1)) ||
                                   (colNum == 3 && (dx == 1 || dy == -1)) ||
                                   (colNum == 4 && (dx == -1 || dy == -1)) ||
                                   (colNum == 5 && (dx == -1 || dy == 1)))
                                collides = true;
                            if ((colNum == 1 && (dy == -1 || dy == 1)) ||
                                 (colNum == 2 && (dy == 1)) ||
                                 (colNum == 3 && (dy == -1)) ||
                                 (colNum == 4 && (dy == -1)) ||
                                 (colNum == 5 && (dy == 1)))
                                collidesC = true;
                            if ((colNum == 1 && (dx == -1 || dx == 1)) ||
                                (colNum == 2 && (dx == 1)) ||
                                (colNum == 3 && dx == 1) ||
                                (colNum == 4 && dx == -1) ||
                                (colNum == 5 && dx == -1))
                                collidesD = true;
                            if (colNum == 2 && (collideBox.Left - colX + collideBox.Bottom - colY <= 1))
                                collidesA = true;
                            if (colNum == 3 && (collideBox.Left - colX - (collideBox.Top - colY) <= 0))
                                collidesB = true;
                            if (colNum == 4 && (collideBox.Right - colX + collideBox.Top - colY >= 1))
                                collidesA = true;
                            if (colNum == 5 && (collideBox.Right - colX - (collideBox.Bottom - colY) >= 0))
                                collidesB = true;
                            //Console.WriteLine("Collides: " + collidesA + collidesB + collidesC + collidesD);
                            if ((collidesA || collidesB || collidesC || collidesD)) /**&& sender is Projectile)*/ 
                            { // this commenting effectively nullifies the new collision system.
                                sender.Stop();
                                return Vector2.Zero;
                            }
                                //sender.Destroy();
                            if (collidesC)
                            {
                                A = new Vector2(A.X, 0);
                                B = new Vector2(B.X, 0);
                            }
                            if (collidesD)
                            {
                                A = new Vector2(0, A.Y);
                                B = new Vector2(0, B.Y);
                            }
                            if (collidesA && (collidesB || collidesC || collidesD))
                            {
                                sender.Stop();
                                return Vector2.Zero;
                            }
                            if (collidesA)
                                A = Vector2.Zero;
                            if (collidesB)
                                B = Vector2.Zero;
                            
                        }
                    }
                    
                }
            }
            return A+B;

        }

        public void Draw(Rectangle visibleRect, double ticksPassed)
        {
            visibleRect.X -= 14;
            visibleRect.Width += 14;
            visibleRect.Height += 14;
            visibleRect = visibleRect.ClampToRange(0,0,Width-1, Height-1);
            GL.Color4(1f, 1f, 1f, 1f);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            for (int layer = 0; layer < Layers; layer++)
            {
                for(int i = visibleRect.Y+visibleRect.Height; i >= visibleRect.Y; i--)
                {
                    var row = _tiles[i];

                    for (int j = visibleRect.X; j < visibleRect.X + visibleRect.Width; j++)
                    {
                        float z = layer==_renderingOptins.BackLayer ? -i+visibleRect.Y-10 : -layer;
                        Vector3 tr = new Vector3(j, i+1, z);

                        Tile tile = row[j * Layers + layer];
                        if (tile != null)
                            row[j * Layers + layer].Draw(tr, ticksPassed);
                    }
                }
            }

            foreach (Pawn pawn in _pawnCreator)
            {
                if (pawn.Position.In(visibleRect))
                {
                    pawn.Visible = true;
                    float zOffset = -pawn.Position.Y + visibleRect.Y - 10 + 0.1f;
                    GL.Translate(0, 0, zOffset);

                    GL.Color4(1f, 1f, 1f, 1f);
                    pawn.Draw(ticksPassed);
                    if (_renderingOptins.DrawHitbox)
                    {
                        GL.Disable(EnableCap.Texture2D);
                        GL.Color4(0.2, 1, 0.1, 0.5);
                        Box2 boundingBox = pawn.HitBox;
                        GL.Begin(BeginMode.LineLoop);
                        boundingBox = boundingBox.Displace(pawn.Position);
                        boundingBox = boundingBox.LevelToGraphicsCoordinates();
                        GL.Vertex3(boundingBox.Left, boundingBox.Bottom, -0.1f);
                        GL.Vertex3(boundingBox.Left, boundingBox.Top, -0.1f);
                        GL.Vertex3(boundingBox.Right, boundingBox.Top, -0.1f);
                        GL.Vertex3(boundingBox.Right, boundingBox.Bottom, -0.1f);
                        GL.End();
                    }
                        if (_renderingOptins.DrawCollision && pawn.Collides)
                        {
                            GL.Disable(EnableCap.Texture2D);

                            GL.Color4(1, 0.2, 0.2, 1);
                            Box2 collideBox = pawn.CollideBox;
                            GL.Begin(BeginMode.LineLoop);
                            collideBox = collideBox.Displace(pawn.Position);
                            collideBox = collideBox.LevelToGraphicsCoordinates();
                            GL.Vertex3(collideBox.Left, collideBox.Bottom, -0.1f);
                            GL.Vertex3(collideBox.Left, collideBox.Top, -0.1f);
                            GL.Vertex3(collideBox.Right, collideBox.Top, -0.1f);
                            GL.Vertex3(collideBox.Right, collideBox.Bottom, -0.1f);
                            GL.End();
                        }
                    
                    GL.Translate(0, 0, -zOffset);
                }
                else
                    pawn.Visible = false;
            }
            if (_renderingOptins.DrawBounds)
            {
                GL.Disable(EnableCap.Texture2D);
                GL.LineWidth(1f);
                Vector3 upperBounds = VectorExtensions.LevelToGraphicsCoordinates(Width - 1, Height - 1);
                GL.Begin(BeginMode.LineLoop);

                GL.Color4(System.Drawing.Color.LimeGreen);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, upperBounds.Y, 0);

                GL.Vertex3(upperBounds.X, upperBounds.Y, 0);
                GL.Vertex3(upperBounds.X, 0, 0);

                GL.End();
            }
            if (_renderingOptins.DrawTriggers)
            {
                GL.Color4(0, 0, 1, 0.5);
                float z = -99f;
                foreach (Trigger trigger in _triggers)
                {
                    if (trigger is AreaTrigger)
                    {
                        Box2 area = (trigger as AreaTrigger).Area.ToBox2();
                        area = area.LevelToGraphicsCoordinates();
                        GL.Begin(BeginMode.LineLoop);
                        GL.Vertex3(area.Left, area.Bottom, z);
                        GL.Vertex3(area.Left, area.Top, z);
                        GL.Vertex3(area.Right, area.Top, z);
                        GL.Vertex3(area.Right, area.Bottom, z);
                        GL.End();
                    }
                }
            }
            if (_renderingOptins.DrawCollision)
                DrawCollisionMap(visibleRect);

        }

        private void DrawCollisionMap(Rectangle visibleRect)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Color4(0.9, 0.5, 0.5, 0.3);
            float z = -99f;
            for (int i = visibleRect.Y; i < visibleRect.Y + visibleRect.Height; i++)
            {
                
                for (int j = visibleRect.X; j < visibleRect.X + visibleRect.Width; j++)
                {
                    int colNum =_collision[i][j];
                    if (colNum != 0)
                    {
                        GL.Begin(BeginMode.TriangleFan);
                        if(colNum!=5)
                        GL.Vertex3(j, -i - 1, z);
                        if(colNum !=4)
                        GL.Vertex3(j, -i, z);
                        if(colNum !=3)
                        GL.Vertex3(j + 1, -i, z);
                        if(colNum !=2)
                        GL.Vertex3(j + 1, -i - 1, z);

                        GL.End();
                    }
                }
            }
        }

        internal void Update(double ticks)
        {
            _pawnCreator.Update(ticks);
        }

        protected override void LevelLoaded()
        {
            base.LevelLoaded();

        }


    }
}
