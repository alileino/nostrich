using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Pawn : IDestroyablePawn, IMovingPawn
    {
        protected SpriteCollection _sprites;
        protected static Random _random = new Random();
        Sprite _currentSprite;
        Vector2 _position;
        Vector2 _velocity;
        float _maxVelocity = 1f;
        int _id;
        Vector2 _lookDirection = new Vector2(1,0);
        int _lookDirectionAngle = 0;
        Box2 _hitBox;
        protected int LookDirectionAngle { get { return _lookDirectionAngle; } }
        protected Sprite CurrentSprite { get { return _currentSprite; } }
        public bool Visible { get; internal set; }

        public string Type { get; set; }
        public event PawnEvents.DestroyEventHandler OnDestroy;

        public float X { get { return _position.X; } set { _position.X = value; } }
        public float Y { get { return _position.Y; } set { _position.Y = value; } }
        public virtual float Width { get { return _sprites.SpriteWidth / GameConstants.TILE_SIZEf; } }
        public virtual float Height { get { return _sprites.SpriteHeight / GameConstants.TILE_SIZEf; } } 
        public float MaxVelocity { get { return _maxVelocity; } set { _maxVelocity = value; } }
        public Vector2 Position { get { return _position; } set { _position = value; } }
        public Box2 HitBox { get { return _hitBox; } set { _hitBox = value;}}
        public virtual bool Collides { get { return false; } }
        public virtual Box2 CollideBox { get { return new Box2(0f, 0f, 0f, 0f); } }
        public virtual string Name { get { return "Pawn.Generic"; } set { } }
        public bool Destroyed { get; set; }
        public bool Invulnerable { get; set; }

        public Vector2 LookDirection
        {
            get { return _lookDirection; }
            set { SetLookDirection(value, false); }
        }

        public Vector2 VelocityVector 
        { 
            get 
            { 
                return _velocity; 
            } 
            set 
            { 
                _velocity = value;
                VelocityChanged();
            } 
        }

        private void VelocityChanged()
        {
            float v = _velocity.LengthSquared;
            if (v > MaxVelocity * MaxVelocity)
                _velocity *= (MaxVelocity / _velocity.Length);
            else if (v < 0.0001)
                _velocity = Vector2.Zero;
        }

        public float Velocity 
        { 
            get { return _velocity.LengthFast; } 
            set { throw new NotImplementedException(); } 
        }

        public float VelocitySquared
        {
            get { return _velocity.LengthSquared; }
        }
        
        public int Id { get { return _id; } private set { _id = value; } }

        protected virtual Sprite DefaultSprite
        {
            get
            {
                return _sprites["default"];
            }
        }

        public Pawn(int id, SpriteCollection sprites, float x=0, float y=0)
        {
            Type = "pawn";
            _id = id;
            _sprites = sprites;
            X = x;
            Y = y;
            _currentSprite = DefaultSprite;
            _hitBox = new Box2(-Width / 2f, Height / 2f, Width / 2f, -Height / 2f);
        }


        public virtual void Draw(double ticksPassed)
        {
            _currentSprite.Draw(X, Y, ticksPassed);
        }

        public virtual void Update(double ticksPassed)
        {
            UpdateVelocity(ticksPassed);
            //_position = Vector2.Add(_position, VelocityVector * (float)ticksPassed);
            if (VelocitySquared > 0)
            {
                Vector2 displacement = VelocityVector * (float)ticksPassed;
                DoMove(displacement);
            }
        }

        protected virtual void UpdateVelocity(double ticksPassed)
        {
        }

        protected void SetSprite(string name)
        {
            if( _currentSprite.Name != name)
                _currentSprite = _sprites[name];
        }

        protected void SetLookDirection(Vector2 dir, bool normalized=false)
        {
            if (!normalized)
                dir.Normalize();
            double angle = dir.Angle();
            int animNum = (int)((angle / MathHelper.TwoPi + 1 / 16f) * 8) % 8;
            
            _lookDirection = dir;
            _lookDirectionAngle = animNum;
        }

        public virtual void Destroy()
        {
            if (OnDestroy != null)
                OnDestroy(this);
            OnDestroy = null;
            Destroyed = true;
        }
        
        

        protected void DoMove(Vector2 displacement)
        {

            if (Moved != null)
                displacement = Moved(this, displacement);
            _position = Vector2.Add(_position, displacement);
        }

        public virtual void Stop()
        {
            _velocity = Vector2.Zero;
        }

        public virtual bool Hit(Projectile projectile)
        {
            return true;
        }


        public event PawnEvents.MoveEventHandler Moved;
    }
}
