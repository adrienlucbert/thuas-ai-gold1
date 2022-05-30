using System.Linq;
using UnityEngine;

namespace Connect4.Game
{
    public class GameBoard : MonoBehaviour
    {
        readonly public Vector2Int Size = new Vector2Int(7, 6);
        private PlayerId?[] _cells;

        private void Awake()
        {
            this._cells = Enumerable.Repeat<PlayerId?>(null, this.Size.x * this.Size.y).ToArray();
        }

        private ref PlayerId? GetCellOwner(int row, int column)
        {
            return ref this._cells[row * this.Size.y + column];
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