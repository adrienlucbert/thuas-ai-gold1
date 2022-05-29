using UnityEngine;
using Connect4.Game;

namespace Connect4.UI
{
    public class GameBoardRenderer : MonoBehaviour
    {
        [SerializeField] private GameObject _emptyTilePrefab;
        [SerializeField] private GameObject _player1TilePrefab;
        [SerializeField] private GameObject _player2TilePrefab;
        [SerializeField] private GameBoard _gameBoard;

        private void Awake()
        {
            if (this._gameBoard == null)
                Debug.Assert(this.TryGetComponent(out this._gameBoard));
            this.DrawBoard();
        }

        private GameObject GetCellPrefab(GameBoard.CellState state)
        {
            switch (state)
            {
                case GameBoard.CellState.Empty:
                    return this._emptyTilePrefab;
                case GameBoard.CellState.Player1:
                    return this._player1TilePrefab;
                case GameBoard.CellState.Player2:
                    return this._player2TilePrefab;
            }
            throw new System.Exception($"Unsupported cell state: {state}");
        }

        private void DrawCell(int x, int y, GameBoard.CellState state)
        {
            GameObject prefab = this.GetCellPrefab(state);
            GameObject tile = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity, this.transform);
            tile.name = $"{prefab.name} {x} {y}";
        }

        private void ReplaceCell(int x, int y, GameBoard.CellState state)
        {
            throw new System.NotImplementedException();
        }

        private void DrawBoard()
        {
            for (int row = 0; row < this._gameBoard.Size.y; ++row)
                for (int col = 0; col < this._gameBoard.Size.x; ++col)
                    this.DrawCell(col, row, this._gameBoard[row, col]);
        }

        public void OnPlayTurn(PlayOutput lastPlay)
        {
            GameBoard.CellState state = lastPlay.player == PlayerId.Player1
                ? GameBoard.CellState.Player1
                : GameBoard.CellState.Player2;
            this.ReplaceCell(lastPlay.x, lastPlay.y, state);
        }
    }
}