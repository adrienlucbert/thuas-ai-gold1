using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Connect4.Game;

namespace Connect4.UI
{
    public class GameBoardRenderer : MonoBehaviour
    {
        [SerializeField] private GameObject _emptyTilePrefab;
        [SerializeField] private GameObject _player1TilePrefab;
        [SerializeField] private GameObject _player1PlayButtonPrefab;
        [SerializeField] private GameObject _player2TilePrefab;
        [SerializeField] private GameObject _player2PlayButtonPrefab;
        private GameObject _controls;
        private GameBoard _gameBoard;

        public void OnStartGame(GameBoard board)
        {
            this._gameBoard = board;
            this.ClearBoard();
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

        private GameObject GetPlayButtonPrefab(PlayerId player)
        {
            switch (player)
            {
                case PlayerId.Player1:
                    return this._player1PlayButtonPrefab;
                case PlayerId.Player2:
                    return this._player2PlayButtonPrefab;
            }
            throw new System.Exception($"Unsupported player id: {player}");
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

        private void ClearBoard()
        {
            for (int i = this.transform.childCount; i > 0; --i)
                DestroyImmediate(this.transform.GetChild(0).gameObject);
            this._controls = Instantiate(new GameObject(), this.transform);
            this._controls.name = "Controls";
        }

        private void ClearControls()
        {
            for (int i = this._controls.transform.childCount; i > 0; --i)
                DestroyImmediate(this._controls.transform.GetChild(0).gameObject);
        }

        public IEnumerator ShowControls(PlayerId currentPlayer, Action<PlayInput> onPlayCallback)
        {
            GameObject prefab = this.GetPlayButtonPrefab(currentPlayer);
            this.ClearControls();
            for (int col = 0; col < this._gameBoard.Size.x; ++col)
            {
                if (this._gameBoard[this._gameBoard.Size.y - 1, col].HasValue)
                    continue;
                PlayInput play = new PlayInput { column = col };
                GameObject button = Instantiate(prefab, new Vector3(col, this._gameBoard.Size.y, 0), Quaternion.identity, this._controls.transform);
                button.GetComponentInChildren<MouseClickHandler>().onMouseClick.AddListener(() =>
                {
                    this.ClearControls();
                    onPlayCallback(play);
                });
            }
            yield return new WaitForEndOfFrame();
        }

        public void OnPlayTurn(PlayOutput lastPlay)
        {
            this.ReplaceCell(lastPlay.x, lastPlay.y, lastPlay.player);
        }
    }
}