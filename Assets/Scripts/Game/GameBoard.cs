using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Connect4.Game
{
    public class GameBoard : ICloneable
    {
        public Vector2Int Size { get; private set; }
        private PlayerId?[] _cells;

        public GameBoard(int columns = 7, int rows = 6)
        {
            this.Size = new Vector2Int(columns, rows);
            this._cells = Enumerable.Repeat<PlayerId?>(null, columns * rows).ToArray();
        }

        private ref PlayerId? GetCellOwner(int row, int column)
        {
            return ref this._cells[row * this.Size.x + column];
        }

        public PlayerId? this[int row, int column]
        {
            get
            {
                if (row < 0 || row >= this.Size.y || column < 0 || column >= this.Size.x)
                    return null;
                return this.GetCellOwner(row, column);
            }
            set { this.GetCellOwner(row, column) = value; }
        }

        public bool IsFull()
        {
            for (int col = 0; col < this.Size.x; ++col)
            {
                if (!this[this.Size.y - 1, col].HasValue)
                    return false;
            }
            return true;
        }

        public List<PlayInput> GetAvailablePlays()
        {
            List<PlayInput> plays = new List<PlayInput>();
            for (int col = 0; col < this.Size.x; ++col)
            {
                if (this[this.Size.y - 1, col].HasValue)
                    continue;
                plays.Add(new PlayInput { column = col });
            }
            return plays;
        }

        public object Clone()
        {
            GameBoard clone = new GameBoard(this.Size.x, this.Size.y);
            for (int i = 0; i < this._cells.Length; ++i)
                clone._cells[i] = this._cells[i];
            return clone;
        }
    }
}