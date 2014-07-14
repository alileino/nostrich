using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Human : Pawn, IShootingPawn, IDamageTakingPawn
    {
        public static SpriteCollection Corpses;
        protected float _distanceLeft;
        protected Vector2 _moveDirection;
        protected Equipped _equipped;
        protected float _health = 1f;
        protected bool _weaponDrawn = false;
        protected bool _moving = false;
        protected string _name = "Pawn.Human";
        static Box2 _collideBox = new Box2(-0.30f, 1.05f, 0.30f, 0.45f);
        public float Health { get { return _health; } set { _health = value; } }
        public bool Dead { get { return _health < 0; } }
        public override bool Collides { get { return true; } }
        public override string Name { get { return _name; } set { _name = value; } }
        public bool Hostile { get; set; }
        public Equipped Equipped { get { return _equipped; } }
        private List<Point> _destinations = new List<Point>();
        private int _destPos = 0;
        private bool _loop = true;
        public bool LoopDestinations { get { return _loop; } set { _loop = value; } }
        public bool LookAtPlayer { get; set; }
        public event PawnEvents.DeathEventHandler Died;

        public Human(int id, SpriteCollection sprites, float x=0, float y=0)
            : base(id, sprites, x,y)
        {
            Type = "Human";
            LookAtPlayer = true;
            MaxVelocity = 0.4f;
            Weapon w = new Weapon();
            _equipped = new Equipped(this,w);
            HitBox = new Box2(-Width / 2, 0, Width / 2, -Height/2);
        }

        public override float Width { get { return GameConstants.HUMAN_WIDTH; } }
        public override float Height { get { return GameConstants.HUMAN_HEIGHT; } }

        public void Shoot()
        {
            _weaponDrawn = true;
            if (Equipped.Trigger())
                if (OnShooting != null)
                     OnShooting(this);
        }

        public void AddDestination(int x, int y)
        {
            _destinations.Add(new Point(x, y));
        }

        public override void Update(double ticksPassed)
        {
            if (Dead)
                return;
            base.Update(ticksPassed);
            Equipped.Update(ticksPassed);
            VelocityVector = Vector2.Zero;

            if (_distanceLeft <= 0)
            {
                
                if (_destinations.Count > 0)
                {
                    if((Vector2.Subtract(Position, _destinations[_destPos].ToVector2()).LengthFast < 0.2))
                        _destPos += 1;
                    if (_loop)
                        _destPos %= _destinations.Count;
                    else if (_destPos >= _destinations.Count)
                    {
                        _destinations.Clear();
                        return;
                    }
                    var newDest = _destinations[_destPos];
                   // Console.WriteLine("Moving towards:" + newDest.X + "/" + newDest.Y);
                    MoveTowards(newDest.X, newDest.Y);  
                }
            }
        }

        public override void Draw(double ticksPassed)
        {
            //_sprites["shadow"].Draw(X, Y + 1, ticksPassed);
            //_sprites["shadow"].Draw(new OpenTK.Vector3(X, Y, 0.05f), ticksPassed, 0);
            if (Dead)
            {
                Corpses[_sprites.Name].Draw(new Vector3(X,Y,0),ticksPassed,0);
                return;
            }
            string animationString;
            if (!_weaponDrawn)
                animationString = "walk";
            else
                animationString = "revolver";
            SetSprite(animationString + "_" + LookDirectionAngle);
            if (_moving)
                CurrentSprite.Draw(X, Y, ticksPassed);
            else
                CurrentSprite.Draw(new Vector3(X, Y, 0), ticksPassed, 0);
            if (animationString == "walk" && !_moving)
                return;

        }

        public void MoveTowards(int x, int y)
        {
            MoveTowards(new Vector2(x, y));
        }

        public void MoveTowards(Vector2 mousePosition)
        {
            _weaponDrawn = false;
            Vector2 destination = mousePosition - Position;
            LookDirection = destination;
            _distanceLeft = destination.LengthFast;
            if (_distanceLeft < 0.001)
                _distanceLeft = 0;
            else
                _moving = true;
            _moveDirection = LookDirection;
        }

        protected override void UpdateVelocity(double ticksPassed)
        {
            if (_distanceLeft > 0)
            {
                float mult = 1f; //(float)GameConstants.MOVE_ACCEL*10f;

                VelocityVector = Vector2.Add(VelocityVector, _moveDirection * mult);
                _distanceLeft -= VelocityVector.LengthFast * (float)ticksPassed;
            }
            else
            {
                _moving = false;
                Stop();
            }
        }

        public override void Stop()
        {
            base.Stop();
            _distanceLeft = 0;
            //_moving = false;

        }

        public virtual void PlayerMoved(Vector2 playerPos)
        {
            if (Dead)
                return; 
            Vector2 playerOffset = playerPos - Position;
            if(_destinations.Count == 0 && LookAtPlayer)
                LookDirection = playerOffset;

        }

        public override bool Hit(Projectile p)
        {
            if (Dead)
                return false;   
            base.Hit(p);
            if (!CanHit(p.Owner))
                return false;
            if (Invulnerable)
                return false;
            
            _health -= 0.5f;
            if (DamageTaken != null)
                DamageTaken(this);
            if (_health < 0)
                Kill();
            return true;
        }

        public void Kill()
        {
            _health = -0.1f;
            if (Died != null)
                Died(this);
        }

        protected virtual bool CanHit(Pawn pawn)
        {
            return true;
        }


        public event PawnEvents.ShootEventHandler OnShooting;


        public override Box2 CollideBox
        {
            get { return _collideBox; }
        }

        public event PawnEvents.DamageTakenHandler DamageTaken;

    }
}
