using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{

    public class Dialog
    {
        List<Tuple<string, string>> _lines = new List<Tuple<string, string>>();
        int _currentLineIndex = 0;
        string _abortText = "";
        public delegate void DialogEndedHandler(Dialog dialog);
        public event DialogEndedHandler DialogEnded;
        public bool Aborted { get; set; }
        public bool Abortable { get { return _abortText != ""; } }

        public Dialog(string filename)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine("Quests", "Dialog"));
            FileInfo file = dir.GetFiles(filename + "*")[0];
            string dialogText;
            using (TextReader reader = new StreamReader(file.FullName))
            {
                dialogText = reader.ReadToEnd();
            }
            string[] lines = dialogText.Split('\n');
            string someSpeaker = null;
            foreach (string lineIter in lines)
            {
                string line = lineIter.Trim();
                if (line == "")
                    continue;
                string[] splitLine = line.Split(':');
                string speaker = splitLine[0];
                if (someSpeaker != null && someSpeaker != speaker)
                    NumSpeakers = 2;
                else
                    someSpeaker = speaker;
                AddLine(speaker, splitLine[1]);
            }
            if (NumSpeakers == 0)
                NumSpeakers = 1;
        }

        public string CurrentLine { get { return !Ended ? _lines[_currentLineIndex].Item2 : ""; } }
        public string CurrentSpeaker { get { return !Ended ? _lines[_currentLineIndex].Item1 : ""; } }
        public string AbortText { get { return _abortText; } set { _abortText = value; } }
        public string Background { get; set; }
        public int NumSpeakers { get; private set; }
        public void AddLine(string speaker, string line)
        {
            _lines.Add(Tuple.Create(speaker.Trim(), line.Trim()));
        }

        public bool MoveNext()
        {
            _currentLineIndex++;
            if (_currentLineIndex >= _lines.Count)
            {
                if (DialogEnded != null)
                    DialogEnded(this);
                DialogEnded = null;
                return false;
            }
            return true;
        }

        public void Abort()
        {
            if (!Abortable)
                return;
            _currentLineIndex = _lines.Count;
            Aborted = true;
            MoveNext();
        }

        public bool Ended { get { return _currentLineIndex >= _lines.Count; } }
    }
}
