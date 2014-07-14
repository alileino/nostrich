using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Friendly : Human
    {
        public Friendly(int id, SpriteCollection sprites, float x = 0, float y = 0)
            : base(id, sprites, x, y)
        {
            Type = "friendly";
            Hostile = false;
            Invulnerable = true;
        }

        protected override bool CanHit(Pawn pawn)
        {
            if (pawn is Friendly)
                return false;
            return true;
        }
    }
}
