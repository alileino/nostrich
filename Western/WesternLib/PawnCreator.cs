using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class PawnCreator : IEnumerable<Pawn>
    {
        private Random random = new Random();
        SoundManager _sound; // todo: doesn't really belong here.
        private Dictionary<int, Pawn> _pawns = new Dictionary<int,Pawn>();
        private Queue<int> _destroyQueue = new Queue<int>();
        private Queue<Pawn> _createQueue = new Queue<Pawn>();
        private List<Projectile> _projectiles = new List<Projectile>();
        private Queue<Projectile> _createProjectileQueue = new Queue<Projectile>();
        private Queue<Projectile> _destroyProjectileQueue = new Queue<Projectile>();
        private Player _player;
        private ResourceManager _resourceMgr;
        private int _maxId = 0;
        private System.Drawing.Point _lastPlayerPos;
        public event PawnEvents.PlayerMoveHandler OnPlayerMove;
        public event PawnEvents.PlayerMoveHandler PlayerChangedTile;
        public event PawnEvents.MoveEventHandler CollidingPawnMoved;
        #region Enumerators
        public Pawn this[int id]
        {
            get { return _pawns[id]; }
            private set { _pawns[id] = value; }
        }

        public IEnumerator<Pawn> GetEnumerator()
        {
            foreach (Pawn p in _pawns.Values)
                yield return p;
            foreach (Projectile p in _projectiles)
                yield return p;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

        public PawnCreator(ResourceManager resourceMgr)
        {
            _resourceMgr = resourceMgr;
            _sound = new SoundManager();
            Human.Corpses = _resourceMgr.Sprites["corpses"];
            CreatePlayer();
            ScriptBindings.ScriptSpawn += ScriptSpawn;
            //_sound.LoadSound("revolver", "revolver.wav");
            _sound.LoadSound("death", "death.wav", true, 1f);
            _sound.LoadSound("damage_taken", "flesh_01.wav");
            _sound.LoadMusic("western", "westerners_original_v1_loop.mp3");
            _sound.LoadSound("flesh_1", "flesh_01.wav", true, 1f);
            _sound.LoadSound("flesh_2", "flesh_02.wav", true, 1f);
            _sound.LoadSound("flesh_3", "flesh_03.wav", true, 1f);
            _sound.LoadSound("flesh_4", "flesh_04.wav", true, 1f);
            _sound.LoadSound("revolver_1", "magnum_shot_01.wav", true, 0.55f);
            _sound.LoadSound("revolver_2", "magnum_shot_02.wav", true, 0.55f);
            _sound.LoadSound("revolver_3", "magnum_shot_03.wav", true, 0.55f);
            _sound.LoadSound("revolver_4", "magnum_shot_04.wav", true, 0.55f);
            _sound.LoadSound("ricochet_1", "ricochet_01.wav", true, 0.65f);
            _sound.LoadSound("ricochet_2", "ricochet_02.wav", true, 0.65f);
            _sound.LoadSound("ricochet_3", "ricochet_03.wav", true, 0.65f);
            _sound.LoadSound("ricochet_4", "ricochet_04.wav", true, 0.65f);
            //_sound.LoadSound("walk", "walk_loop_150bpm.wav", false);
            _sound.LoadSound("walk", "walk_loop_210bpm.wav", false, 0.120f);
            _sound.LoadSound("explosion", "explosion.wav", true, 0.95f);
            _sound.LoadSound("reload", "magnum_reload.wav", true, 0.65f);
            _sound.LoadSound("scream_1", "wilhelm_02.wav", true, 0.9f);
            _sound.LoadSound("scream_2", "wilhelm_03.wav", true, 0.9f);
            _sound.LoadSound("tomahawk_hit", "tomahawk_hit.wav", true, 0.9f);
            
            Player.Equipped.Reloading += PlayerReloading;
            Player.Died += PlayerDied;
            _sound.PlayMusic("western");
        }

        private void PlayerDied(Pawn sender)
        {
            _sound.PlaySound("scream_1");
        }

        private void PlayerReloading(Pawn sender)
        {
            _sound.PlaySound("reload");
        }

        public void Update(double ticks)
        {
            foreach (Pawn pawn in _pawns.Values)
            {
                pawn.Update(ticks);
            }
            foreach (Projectile projectile in _projectiles)
            {
                projectile.Update(ticks);
                foreach (Pawn victim in _pawns.Values)
                {
                    if ( projectile.Owner == victim)
                    {
                        continue;
                    }
                    if (victim.HitBox.Displace(victim.Position).Collides(projectile.HitBox.Displace(projectile.Position)))
                    {
                        if (victim.Hit(projectile))
                        {
                            projectile.Destroy();
                            projectile.HasHit = true;
                        }
                        break;
                    }
                }
            }

            while (_createQueue.Count > 0)
            {
                Pawn p = _createQueue.Dequeue();
                //Console.WriteLine("Created pawn: " + p.Id);
                _pawns.Add(p.Id, p);
            }
            while (_destroyQueue.Count > 0)
            {
                int destroyed = _destroyQueue.Dequeue();
                _pawns.Remove(destroyed);
                //Console.WriteLine("Removed pawn: " + destroyed);
            }

            while (_destroyProjectileQueue.Count > 0)
            {
                Projectile p = _destroyProjectileQueue.Dequeue();
                _projectiles.Remove(p);
                if (p.HasHit == false && random.Next(1, 4) < 2)
                {
                    if (p.Type == "bullet")
                        _sound.PlaySound("ricochet_" + random.Next(1, 5));
                    else
                        _sound.PlaySound("tomahawk_hit");   
                }
            }
            
            while (_createProjectileQueue.Count > 0)
            {
                Projectile p = _createProjectileQueue.Dequeue();
                _projectiles.Add(p);
            }
        }

        public void RemovePawns()
        {
            foreach (var p in _pawns)
            {
                if (p.Value != Player)
                {
                    p.Value.Destroy();
                    _destroyQueue.Enqueue(p.Key);
                }
            }
        }

        public Player Player { get{return _player;}  }

        private void CreatePlayer()
        { 
            _player = new Player(GetSprites("player"));
            _createQueue.Enqueue(_player);
            _player.Moved += PlayerMoved;
            RegisterHuman(_player);
        }

        private Enemy CreateEnemy(string type)
        {
            Enemy e = new Enemy(GetId(), GetSprites(type), 0,0, type);
            _createQueue.Enqueue(e);
            RegisterHuman(e);
            RegisterEnemy(e);
            return e;
        }


        private Friendly CreateMarshall()
        {
            return  CreateFriendly("marshall");
        }

        private Friendly CreateFriendly(string sprite)
        {
            Friendly f = new Friendly(GetId(), GetSprites(sprite));
            _createQueue.Enqueue(f);
            RegisterHuman(f);
            return f;
        }

        private Projectile CreateProjectile(Human shooter)
        {
            Vector2 position = shooter.Position + shooter.LookDirection;
            position.Y -= 0.4f;
            string type = "bullet";
            if (shooter.Type == "indian")
            {
                type = "tomahawk";
                _sound.PlaySound("flesh_3");
            }
            else
                _sound.PlaySound("revolver_" + random.Next(1, 5));
            Projectile p = new Projectile(GetId(), GetSprites(type), position, shooter.LookDirection, shooter);
            p.Type = type;
            _createProjectileQueue.Enqueue(p);
            RegisterPawn(p);
            return p;
        }

        private SpriteCollection GetSprites(string spriteCollection)
        {
            return _resourceMgr.Sprites[spriteCollection].Copy();
        }

        private int GetId()
        {
            _maxId++;
            return _maxId;
        }

        private void RegisterHuman(Human human)
        {
            human.OnShooting += PawnShooted;
            human.DamageTaken += PawnTookDamage;
            if(!(human is Player))
                OnPlayerMove += human.PlayerMoved;
            RegisterPawn(human);
        }

        private Vector2 HandleCollidingPawnMoved(Pawn human, Vector2 displacement)
        {
            if (CollidingPawnMoved != null)
                return CollidingPawnMoved(human, displacement);
            return displacement;
        }

        private void RegisterPawn(Pawn pawn)
        {
            pawn.OnDestroy += PawnDestroyed;
            if(pawn.Collides)
                pawn.Moved += HandleCollidingPawnMoved;
        }

        private void RegisterEnemy(Enemy enemy)
        {
           
        }
        
        private void PawnShooted(Human shooter)
        {
            CreateProjectile(shooter);
        }

        private void PawnTookDamage(IDamageTakingPawn pawn)
        {
            if (pawn.Health > 0)
                _sound.PlaySound("flesh_" + (new Random()).Next(1, 5));
            else
                _sound.PlaySound("death");
        }

        private void PawnDestroyed(Pawn pawn) // TODO: This clearly does not remove all the events
        {
            if (pawn is Player)
                return;
            pawn.OnDestroy -= PawnDestroyed;
            if (pawn is Projectile)
                _destroyProjectileQueue.Enqueue(pawn as Projectile);
            else
                _destroyQueue.Enqueue(pawn.Id);
        }

        private Vector2 PlayerMoved(Pawn player, Vector2 displacement)
        {
            _sound.PlaySound("walk");
            if (OnPlayerMove != null)
                OnPlayerMove(player.Position);
            if (PlayerChangedTile != null )
            {
                Point posAsPoint = player.Position.ToPoint();
                if (_lastPlayerPos != posAsPoint)
                {
                    PlayerChangedTile(player.Position);
                    _lastPlayerPos = posAsPoint;
                }
            }
            return displacement;
        }

        private Human ScriptSpawn(string type, double xpos, double ypos,string name)
        {
            Human pawn;
            switch (type)
            {
                case "bandit":
                    pawn = CreateEnemy(type);
                    break;
                case "indian":
                    pawn = CreateEnemy(type);
                    break;
                //case "projectile":
                //    pawn = CreateProjectile(Player);
                //    break;
                default:
                    pawn = CreateFriendly(type);
                    break;
            }
            pawn.Name = name;

            pawn.Position = new OpenTK.Vector2((float)xpos, (float)ypos);
            return pawn;
        }
    }

    public interface IShootingPawn
    {
        event PawnEvents.ShootEventHandler OnShooting;
    }

    public interface IDestroyablePawn
    {
        event PawnEvents.DestroyEventHandler OnDestroy;
    }

    public interface IMovingPawn
    {
        event PawnEvents.MoveEventHandler Moved;
    }

    public interface IDamageTakingPawn
    {
        event PawnEvents.DamageTakenHandler DamageTaken;
        float Health { get; set; }
    }

    public static class PawnEvents
    {
        public delegate void ShootEventHandler(Human sender);
        public delegate void ReloadEventHandler(Pawn sender);
        public delegate void DestroyEventHandler(Pawn sender);
        public delegate void DeathEventHandler(Pawn sender);
        public delegate Vector2 MoveEventHandler(Pawn sender, Vector2 displacement);
        public delegate void PlayerMoveHandler(OpenTK.Vector2 playerPosition);
        public delegate void DamageTakenHandler(IDamageTakingPawn sender);
    }
}
