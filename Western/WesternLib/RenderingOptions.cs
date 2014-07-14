using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Toggleable
    {
        private bool _value;
        public bool Value { get { return _value; } set { _value = value; } }
        public Toggleable(bool b)
        {
            _value = b;
        }
        public void Toggle()
        {
            Value = !Value;
        }
        public static implicit operator bool(Toggleable t)
        {
            return t.Value;
        }
        public static implicit operator Toggleable(bool b)
        {
            return new Toggleable(b);
        }
    }
    public class RenderingOptions
    {
        public int BackLayer { get; set; }

        public Toggleable DrawBounds { get; set; }
        public Toggleable DrawCollision { get; set; }
        public Toggleable DrawTriggers { get; set; }
        public Toggleable DrawHitbox { get; set; }

        public RenderingOptions()
        {
#if DEBUG
            DrawBounds = true; DrawCollision = true; DrawTriggers = true; DrawHitbox = true;
#else
            DrawBounds = false; DrawCollision = false; DrawTriggers = false; DrawHitbox = false;
#endif
            BackLayer = 3;
        }
    }
}
