using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public static class GameConstants
    {
        public static readonly int TILE_SIZE = 32;
        public static readonly float TILE_SIZEf = TILE_SIZE;
        public static readonly float MOVE_ACCEL = 0.4f;
        public static readonly float MOVE_ACCEL_SQUARED = MOVE_ACCEL * MOVE_ACCEL;
        public static readonly float STRAFE_ACCEL = 0.3f;
        public static readonly float MOVE_DAMP = 0.1f; // percentage of ACCEL 
        public static readonly float BULLET_SPEED = 1.7f;
        public static readonly int TICKS_PER_SECOND = 10;
        public static readonly float ANIM_SPEED_MOD = 0.1f;

        public static readonly float HUMAN_WIDTH = 0.7f;
        public static readonly float HUMAN_HEIGHT = 1.8f;
        public static readonly float ENEMY_SPEED = 0.2f;

        public static float TOMAHAWK_BREAK_FACTOR = 0.98f;
    }

    public static class MathConstants
    {
        public static readonly float SQRT2 = (float)Math.Sqrt(2);
       
    }
}
