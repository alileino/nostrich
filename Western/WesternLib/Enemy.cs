using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Enemy : Human
    {
        double _lastPlayerUpdate = 0;
        Vector2 _playerPos;

        public delegate int CollisionHandler(int x, int y);
        public static CollisionHandler DoesCollide;
        

        public Enemy(int id, SpriteCollection sprites, float x = 0, float y = 0, string type="enemy")
            : base(id, sprites, x, y) 
        {
            Type = type;
            MaxVelocity = GameConstants.ENEMY_SPEED; 
            Hostile = true;
            Equipped.Randomize(_random, 4.0, 5.0);
            _lastPlayerUpdate = _random.NextDouble() * 20;
        }

        

        public override void Draw(double ticksPassed)
        {
            base.Draw(ticksPassed);

            //Console.WriteLine(CurrentSprite.Name);
        }

        public override void Update(double ticksPassed)
        {
            base.Update(ticksPassed);
            if (Hostile)
            {
                Shoot();
                _lastPlayerUpdate += ticksPassed;
                if (_lastPlayerUpdate > 25)
                {
                    if (Vector2.Subtract(Position, _playerPos).LengthFast > 5)
                    {
                        float x = _playerPos.X;
                        float y = _playerPos.Y;
                        double interval = 8.5;
                        int maxIter = 5;
                        int i = 0;
                        do
                        {
                            i += 1;
                            x = (float)(_playerPos.X + _random.NextDouble() * interval - interval / 2.0);
                            y = (float)(_playerPos.Y + _random.NextDouble() * interval - interval / 2.0);
                            //Console.WriteLine("i" + i + "X:" + x + " Y: " + y);
                        }
                        while (DoesCollide((int)x, (int)y) != 0 || i < maxIter);
                        MoveTowards(new Vector2((float)(_playerPos.X + _random.NextDouble() * 7f - 3.5f), (float)(_playerPos.Y + _random.NextDouble() * 7f - 3.5f)));
                        _distanceLeft -= 5;
                        _lastPlayerUpdate = _random.NextDouble() * 10.0;
                    }
                    else
                    LookDirection = _playerPos - Position;
                }
            }
        }
        public override void PlayerMoved(Vector2 playerPos)
        {
            base.PlayerMoved(playerPos);
            _playerPos = playerPos;
            

        }

        public override void Stop()
        {
            base.Stop();
            _lastPlayerUpdate = 31;
        }

        protected override bool CanHit(Pawn pawn)
        {
            if (pawn is Enemy)
                return false;
            return true;
        }

    }
}
