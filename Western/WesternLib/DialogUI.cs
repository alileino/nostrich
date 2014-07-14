using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class DialogUI
    {
        Vector2 _dialogPosition = new Vector2(120, 450);
        Vector2 _speakerPosition = new Vector2(140, 430);
        
        public Rectangle AbortRectangle = new Rectangle(670, 350, 128, 64);
        Vector2 _abortTextPosition;
        Vector3 _abortButtonposition;
        Vector3 _bubblePosition = new Vector3(100, 550, 0);
        Vector3 _posterPosition1 = new Vector3(0, 550, 0);
        Vector3 _posterPosition2 = new Vector3(700, 550, 0);
        Vector3 _headPosition1;
        Vector3 _headPosition2;
        Vector3 _backgroundPosition = new Vector3(0, 600, 0);
        float _dialogMaxWidth = 500f;
        float _abortTextMaxWidth = 70f;
        TextManager _textMgr;
        Dialog _dialog;
        Sprite _bubble;
        Sprite _bubble2;
        Sprite _poster;
        Sprite _button;
        ResourceManager _resourceMgr;

        public delegate void DialogEndedHandler(Dialog dialog);
        public event DialogEndedHandler DialogEnded;

        public Dialog Dialog { get { return _dialog; } }

        public DialogUI(TextManager textMgr, ResourceManager resourceMgr, Camera camera)
        {
            _textMgr = textMgr;
            _bubble = resourceMgr.Sprites["bubble"]["bubble"];
            _bubble2 = resourceMgr.Sprites["bubble"]["bubble2"];
            _poster = resourceMgr.Sprites["poster"]["poster"];
            _button = resourceMgr.Sprites["button"]["button"];
            _resourceMgr = resourceMgr;
            _headPosition1 = _posterPosition1 + new Vector3(20, -32, 0);
            _headPosition2 = _posterPosition2 + new Vector3(20, -32, 0);
            _abortTextPosition = new Vector2(AbortRectangle.Left+10, AbortRectangle.Top+15);
            _abortButtonposition = new Vector3(AbortRectangle.Left, AbortRectangle.Top, 0);
        }

        public Dialog CreateFromFile(string filename)
        {
            _dialog = new Dialog(filename);
            return _dialog;
        }

        public void Draw(double ticksPassed)
        {
            if (_dialog != null)
            {
                double sheriffTicks = ticksPassed, otherTicks = ticksPassed;
                if (Dialog.CurrentSpeaker == "Sheriff")
                {
                    _bubble.DrawAbsolute(_bubblePosition);
                    otherTicks = 0;
                }
                else
                {
                    _bubble2.DrawAbsolute(_bubblePosition);
                    sheriffTicks = 0;
                }
                _poster.DrawAbsolute(_posterPosition1);
                if (_dialog.NumSpeakers > 1)
                    _poster.DrawAbsolute(_posterPosition2);
                if (_dialog.Abortable)
                _button.DrawAbsolute(_abortButtonposition);
                if (Dialog.Background != null)
                    _resourceMgr.Sprites[Dialog.Background]["bg"].DrawAbsolute(_backgroundPosition);
                
                
                GL.Translate(0, 0, -0.01f);

                _resourceMgr.Sprites["faces"]["Sheriff"].DrawAbsolute(_headPosition1, sheriffTicks);
                if (_dialog.NumSpeakers > 1)
                    _resourceMgr.Sprites["faces"]["Mostacho"].DrawAbsolute(_headPosition2, otherTicks);
                
                
                _textMgr.Print(_dialog.CurrentLine, _dialogPosition, _dialogMaxWidth);
                _textMgr.Print(_dialog.AbortText, _abortTextPosition, _abortTextMaxWidth, QuickFont.QFontAlignment.Left );
                _textMgr.Print(_dialog.CurrentSpeaker, _speakerPosition, _dialogMaxWidth);
            }
        }


        public bool MoveNext()
        {
            if (_dialog != null)
            {
                bool running = _dialog.MoveNext();
                if (!running)
                    if (DialogEnded != null)
                        DialogEnded(_dialog);
                return running;
            }
            return false;
        }

        public void Abort()
        {
            if (!_dialog.Abortable)
                return;
            if (_dialog != null)
                _dialog.Abort();
            if (DialogEnded != null)
                DialogEnded(_dialog);

        }
    }
}
