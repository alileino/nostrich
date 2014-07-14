using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using QuickFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class TextManager
    {
        static QFont _font;
        float _maxWidth = 16;
        float _textScaleX = 1f; //0.008f;
        float _textScaleY = 1f; //-0.008f;
        protected float TextInverseScaleX { get { return 1f / _textScaleX; } }
        protected float TextInverseScaleY { get { return 1f / _textScaleY; } }
        protected float MaxWidth { get { return _maxWidth * TextInverseScaleX; } }
        public TextManager()
        {

        }

        public void Init()
        {
            QFontBuilderConfiguration config = new QFontBuilderConfiguration();
            config.charSet += "¿¡íñ";
            _font = new QFont("georgia.ttf", 16, config);
            _font.Options.Colour = new Color4(0f, 0f, 0f, 1f);
            _font.Options.DropShadowActive = true;
            _font.Options.DropShadowOffset = new Vector2(10f, 0f);
            _font.Options.DropShadowOpacity = 0.8f;
        }

        public void Print(string text, Vector2 position, float maxWidth = 100f, QFontAlignment alignment = QFontAlignment.Left)
        {
            GL.PushMatrix();
            GL.PushAttrib(AttribMask.AllAttribBits);
            position = new Vector2(position.X * TextInverseScaleX, position.Y * TextInverseScaleY);
            GL.Scale(_textScaleX, _textScaleY, 1f);
            _font.Print(text, maxWidth * TextInverseScaleX, alignment, position);
            GL.PopAttrib();
            GL.PopMatrix();
        }
    }
}
