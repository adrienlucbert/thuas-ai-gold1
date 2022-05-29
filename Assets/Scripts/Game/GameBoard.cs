using System.Linq;
using UnityEngine;

namespace Connect4.Game
{
    public class GameBoard : MonoBehaviour
    {
        public enum CellState
        {
            Empty,
            Player1,
            Player2
        }

        readonly public Vector2Int Size = new Vector2Int(7, 6);
        private CellState[] _cells;

        private void Awake()
        {
            this._cells = Enumerable.Repeat(CellState.Empty, this.Size.x * this.Size.y).ToArray();
        }

        private ref CellState GetCellState(int row, int column)
        {
            return ref this._cells[row * this.Size.y + column];
        }

        public CellState this[int row, int column]
        {
            get { return this.GetCellState(row, column); }
            set { this.GetCellState(row, column) = value; }
        }
    }
}