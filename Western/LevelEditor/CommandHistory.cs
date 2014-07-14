using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WesternLib;

namespace LevelEditor
{
    internal class CommandHistory
    {
        private Stack<EditorCommand> _history = new Stack<EditorCommand>();
        private Stack<EditorCommand> _redoHistory = new Stack<EditorCommand>();

        public bool CanUndo { get { return _history.Count > 0; } }
        public bool CanRedo { get { return _redoHistory.Count > 0; } }

        public void AddAndExecute(EditorCommand command)
        {
            if (CanUndo && _history.Peek().CompareTo(command) == 0)
                return;
            _history.Push(command);
            command.Execute();
            _redoHistory.Clear();
        }

        public void Undo()
        {
            if (!CanUndo)
                return;
            EditorCommand command = _history.Pop();
            
            command.UnExecute();
            _redoHistory.Push(command);
        }

        public void Redo()
        {
            if (!CanRedo)
                return;
            EditorCommand command = _redoHistory.Pop();
            command.Execute();
            _history.Push(command);
        }

        public void Clear()
        {
            _history.Clear();
            _redoHistory.Clear();
        }
    }

    abstract internal class EditorCommand : IComparable<EditorCommand>
    {
        LevelData _level;

        protected LevelData Level { get { return _level; } }

        public EditorCommand(LevelData level)
        {
            _level = level;
        }
        public abstract void Execute();
        public abstract void UnExecute();

        public abstract int CompareTo(EditorCommand other);
        
    }

    internal class ChangeCollisionCommand : EditorCommand
    {
        int _col;
        int _oldCol;
        int _x, _y;

        public ChangeCollisionCommand(LevelData level, int colNum, int x, int y)
            : base(level)
        {
            _x = x;
            _y = y;
            _col = colNum;
        }

        public override void Execute()
        {
            _oldCol = Level.GetCollision(_x, _y);
            Level.SetCollision(_col, _x, _y);
        }

        public override void UnExecute()
        {
            Level.SetCollision(_oldCol, _x, _y);
        }

        public override int CompareTo(EditorCommand otherCommand)
        {
            ChangeCollisionCommand other = otherCommand as ChangeCollisionCommand;
            if (other == null)
                return -1;
            int value = 0;
            value += (other._x - _x)*0xFFFF;
            value += (other._y - _y)*0x1;
            value += (other._col - _col)*0xFFFFFFF;
            return value;
        }
    }

    internal class ChangeTileCommand : EditorCommand
    {
        Tile _tile;
        Tile _oldTile;
        int _x, _y, _layer;

        public ChangeTileCommand(LevelData level, Tile tile, int x, int y, int layer) 
            : base(level) 
        {
            SetParameters(tile, x, y, layer);
        }

        public void SetParameters(Tile tile, int x, int y, int layer)
        {
            _tile = tile;
            _x = x;
            _y = y;
            _layer = layer;
        }

        public override void Execute()
        {
            _oldTile = Level.GetTile(_x, _y, _layer);
            Level.SetTile(_tile, _x, _y, _layer);
        }

        public override void UnExecute()
        {
            Level.SetTile(_oldTile, _x, _y, _layer);
        }

        public override int CompareTo(EditorCommand otherCommand)
        {
            ChangeTileCommand other = otherCommand as ChangeTileCommand;
            if (other == null)
                return -1;
            int value = 0;
            value += (other._x - _x) * 0xFFFF;
            value += (other._y - _y) * 0xF;
            value += (other._layer - _layer)*2;
            value += _tile == other._tile ? 0 : 1;
            return value;
        }
    }
}
