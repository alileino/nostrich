using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Trigger
    {
        string _quest;
        string _event;
        int _triggerCount = 0;
        int _totalTimes = 1;
        Dictionary<string, string> _parameters = new Dictionary<string, string>();
        public string Quest { get { return _quest; } set { _quest = value; } }
        public string Event { get { return _event; } set { _event = value; } }
        public string Type { get; set; }
        public Dictionary<string, string> Parameters { get { return _parameters; }  }
        public int TimesTriggered { get { return _triggerCount; } }
        public int TotalTimes { get { return _totalTimes; } set { _totalTimes = value; }}
        public bool Completed { get { return _totalTimes == _triggerCount; } }

        public delegate void TriggerHandler(Trigger trigger, out bool cancel);
        public event TriggerHandler Triggered;

        protected void Launch()
        {
            if (Completed)
                return;
            if (Triggered != null)
            {
                bool cancel;
                _triggerCount++;
                Triggered(this, out cancel);
                if (cancel)
                    _triggerCount--;
            }
        }

        internal virtual void Unload()
        {
            Triggered = null;
        }

        public virtual string ToXmlString()
        {
            return "";
        }
        
        public void SetParameters(string parameterString)
        {
            if (parameterString == "")
                return;
            parameterString = parameterString.TrimEnd(new[] { ',' });
            string[] splitParamPairs = parameterString.Split(',');
            
            foreach (string paramPair in splitParamPairs)
            {
                string[] parameter = paramPair.Split('=');
                _parameters.Add(parameter[0], parameter[1]);
            }
        }

        public string GetParameterString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var parameter in _parameters)
            {
                sb.Append(parameter.Key);
                sb.Append('=');
                sb.Append(parameter.Value);
                sb.Append(',');
            }
            return sb.ToString();
        }

        public virtual void MoveTrigger(int x, int y) { }
        
    }


    public class AreaTrigger : Trigger
    {
        Rectangle _area;
        public Rectangle Area { get { return _area; } set { _area = value; } }

        public void TryLaunch(Vector2 position)
        {
            if(_area.Contains(position.X, position.Y))
                Launch();
        }

        public override string ToXmlString()
        {
            string value = _area.Left + "," + _area.Top + "," + _area.Width + "," + _area.Height;
            return value;
        }

        public override void MoveTrigger(int x, int y)
        {
            base.MoveTrigger(x, y);
            _area.Offset(x, y);
        }
    }

    public class ChangeLevelTrigger : AreaTrigger
    {
        public String Level { get; set; }
    }
}
