using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Player : Human, IShootingPawn
    {

        private bool _strafing = false;
        private double _deathCounter = 0;
        private int _conscience = 0;
        public int Conscience { get { return _conscience; } set { _conscience = value; } }

        public event PawnEvents.DestroyEventHandler PlayerDied;
        public event PawnEvents.DestroyEventHandler PlayerRespawning;
        

        public Player(SpriteCollection sprites)
          : base(0, sprites,50,50)
        {
            Type = "player";
            Equipped.EquipLeft(new PlayerRevolver());
            Equipped.EquipRight(new PlayerRevolver());
        }

        public void Move(MoveDirection direction)
        {
            Func<MoveDirection,bool> IsSet = dir => (dir & direction) == dir;
            Vector2 add = new Vector2(0,0);
            Vector2 velocity = VelocityVector;
            if (IsSet(MoveDirection.Up))
                add.Y -= GameConstants.MOVE_ACCEL;
            if (IsSet(MoveDirection.Down))
                add.Y += GameConstants.MOVE_ACCEL;
            if (IsSet(MoveDirection.Left))
                add.X -= GameConstants.MOVE_ACCEL;
            if (IsSet(MoveDirection.Right))
                add.X += GameConstants.MOVE_ACCEL;
            if (add.LengthSquared > GameConstants.MOVE_ACCEL_SQUARED)
                add *= (1 / MathConstants.SQRT2);

            VelocityVector = Vector2.Add(velocity, add);
        }

        public override void Update(double ticksPassed)
        {
            if (!Dead)
            {
                base.Update(ticksPassed);
                _health += (float)ticksPassed / 25f;
                if (_health > 1)
                    _health = 1;
            }
            else
            {
                _deathCounter += ticksPassed;
                if (_deathCounter > 40)
                {
                    if (PlayerRespawning != null)
                        PlayerRespawning(this);
                    _deathCounter = 0;
                }
            }
            //UpdateVelocity(ticksPassed);
            //Position = Vector2.Add(Position, VelocityVector);
            
            // dampen the velocity
            //VelocityVector = Vector2.Multiply(VelocityVector, GameConstants.MOVE_DAMP * GameConstants.MOVE_ACCEL);
            // actually don't dampen the velocity
            VelocityVector = Vector2.Zero;
        }

        public override void Draw(double ticksPassed)
        {
            
            base.Draw(ticksPassed);
        }



        public void Strafe(MoveDirection direction)
        {
            //_moving = true;
            //_strafing = true;
            //_distanceLeft = 0;

            //Vector2 strafeDirection = LookDirection.GetNormal()*GameConstants.STRAFE_ACCEL;
            //if (direction == MoveDirection.Left)
            //    strafeDirection *= -1;

            //VelocityVector = strafeDirection;
        }



        public void Shoot(Vector2 mousePosition)
        {
            Shoot();
            
        }


        public void LookAt(Vector2 mousePosition)
        {
            Vector2 lookDirection = mousePosition - Position;
            if(lookDirection.LengthSquared > 0) // zero vector has no normal
                SetLookDirection(lookDirection, false);
        }

        public override void Destroy()
        {
            if (PlayerDied != null)
                PlayerDied(this);
            _moving = false;
            Stop();
            
        }
    }

    [Flags]
    public enum MoveDirection
    {
        Up = 0x1,
        Right = 0x2,
        Down = 0x4,
        Left = 0x8
    }
}
