using System.Linq;
using UnityEngine;

namespace Connect4.Game
{
    public class GameBoard
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
    }
}