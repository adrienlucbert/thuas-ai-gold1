using UnityEngine;
using Connect4.Game;

namespace Connect4.UI
{
    [RequireComponent(typeof(GameBoard))]
    public class GameBoardRenderer : MonoBehaviour
    {
        [SerializeField] private GameObject _emptyTilePrefab;
        [SerializeField] private GameObject _player1TilePrefab;
        [SerializeField] private GameObject _player2TilePrefab;
        private GameBoard _gameBoard;

        private void Awake()
        {
            Debug.Assert(this.TryGetComponent(out this._gameBoard));
            this.DrawBoard();
        }

        private GameObject GetCellPrefab(PlayerId? owner)
        {
            switch (owner)
            {
                case null:
                    return this._emptyTilePrefab;
                case PlayerId.Player1:
                    return this._player1TilePrefab;
                case PlayerId.Player2:
                    return this._player2TilePrefab;
            }
            throw new System.Exception($"Unsupported cell owner: {owner}");
        }

        private void CreateCell(int x, int y, PlayerId? owner)
        {
            GameObject prefab = this.GetCellPrefab(owner);
            GameObject tile = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity, this.transform);
            tile.name = $"{x} {y}";
        }

        private void ReplaceCell(int x, int y, PlayerId owner)
        {
            Transform oldCell = this.transform.Find($"{x} {y}");
            Debug.Assert(oldCell != null, $"Couldn't find cell at coordinates {x} {y}");
            this.CreateCell(x, y, owner);
            DestroyImmediate(oldCell.gameObject);
        }

        private void DrawBoard()
        {
            for (int row = 0; row < this._gameBoard.Size.y; ++row)
                for (int col = 0; col < this._gameBoard.Size.x; ++col)
                    this.CreateCell(col, row, this._gameBoard[row, col]);
        }

        public void OnPlayTurn(PlayOutput lastPlay)
        {
            this.ReplaceCell(lastPlay.x, lastPlay.y, lastPlay.player);
        }
    }
}