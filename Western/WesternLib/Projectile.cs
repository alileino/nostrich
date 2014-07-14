using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Projectile : Pawn
    {
        double angle;
        Pawn _owner;
        public Projectile(int id, SpriteCollection sprites, float x=0, float y=0)
            : base(id, sprites, x,y)
        { }
        public override bool Collides { get { return true; } }
        public override Box2 CollideBox { get { return HitBox; } }
        public bool HasHit { get; set; }
        public Pawn Owner { get { return _owner; } }
        public string Sound { get; set; }
        double _destroyTicker = 0.0;

        public Projectile(int id, SpriteCollection sprites, Vector2 position, Vector2 direction, Pawn owner)
            : base(id, sprites, position.X, position.Y)
        {
            _owner = owner;
            MaxVelocity = 10f;
            LookDirection = direction;
            VelocityVector = LookDirection * GameConstants.BULLET_SPEED;
            angle = LookDirection.Angle() / Math.PI * 180f;
            SetSprite(LookDirectionAngle.ToString());
        }

        public override void Draw(double ticksPassed)
        {
            base.Draw(ticksPassed);
            if (!Visible)
                Destroy();
        }

        public override void Update(double ticksPassed)
        {
            if (Type == "tomahawk")
            {
                float cosa = 0.99f;
                float sina = 0.01f;
                float x = VelocityVector.X * GameConstants.TOMAHAWK_BREAK_FACTOR;
                float y = VelocityVector.Y * GameConstants.TOMAHAWK_BREAK_FACTOR;
                
                VelocityVector = new Vector2(x * cosa - y * sina, x * sina + y * cosa);
                if (x + y < 0.01f)
                {
                    Type = "tomahawk_stopped";
                    VelocityVector = Vector2.Zero;
                }
            }
            else if (Type == "tomahawk_stopped")
            {
                _destroyTicker += ticksPassed;
                if (_destroyTicker > 100.0)
                {
                    HasHit = true;
                    Stop();
                    
                }

            }

            base.Update(ticksPassed);
        }

        public override void Stop()
        {
            base.Stop();
            Destroy();
        }
    }
}
