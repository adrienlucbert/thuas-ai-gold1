using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Connect4.Game
{
    public enum PlayerId
    {
        Player1 = 0,
        Player2 = 1
    }

    public class GameManager : MonoBehaviour
    {
        public UnityEvent<PlayOutput> OnPlayTurn;
        public UnityEvent OnEndGame;
        public List<PlayOutput> Plays { get; private set; } = new List<PlayOutput>();
        public PlayOutput LastPlay => this.Plays.Count > 0 ? this.Plays[this.Plays.Count - 1] : null;
        public PlayerId CurrentPlayerId => (PlayerId)this._currentPlayerId;

        [SerializeField] private GameBoard _gameBoard;
        [SerializeField] private IPlayer[] _players = new IPlayer[2];
        private int _currentPlayerId = (int)PlayerId.Player1;
        private bool _gameIsRunning = false;

        private void Awake()
        {
            if (this._gameBoard == null)
                Debug.Assert(this.TryGetComponent(out this._gameBoard));
        }

        private void OnValidate()
        {
            if (this._players.Length != 2)
            {
                Debug.LogWarning("There must be exactly 2 players");
                Array.Resize(ref this._players, 2);
            }
        }

        private bool TryGetWinner(out PlayerId? winner)
        {
            winner = null;
            PlayOutput lastPlay = this.LastPlay;
            if (lastPlay == null)
                return false;
            var directions = new Tuple<Vector2Int, Vector2Int>[]
            {
                new Tuple<Vector2Int, Vector2Int>(new Vector2Int(-1,  0), new Vector2Int( 1,  0)), // horizontal
                new Tuple<Vector2Int, Vector2Int>(new Vector2Int( 0, -1), new Vector2Int( 0,  1)), // vertical
                new Tuple<Vector2Int, Vector2Int>(new Vector2Int(-1, -1), new Vector2Int( 1,  1)), // right diagonal
                new Tuple<Vector2Int, Vector2Int>(new Vector2Int( 1,  1), new Vector2Int(-1, -1))  // left diagonal
            };
            foreach (var direction in directions)
            {
                int alignedCells = 0;
                Vector2Int pos = new Vector2Int(lastPlay.x, lastPlay.y) + direction.Item1;
                while (this._gameBoard[pos.y, pos.x] == this._gameBoard[lastPlay.y, lastPlay.x])
                {
                    pos += direction.Item1;
                    ++alignedCells;
                }
                pos = new Vector2Int(lastPlay.x, lastPlay.y) + direction.Item2;
                while (this._gameBoard[pos.y, pos.x] == this._gameBoard[lastPlay.y, lastPlay.x])
                {
                    pos += direction.Item2;
                    ++alignedCells;
                }
                if (alignedCells >= 4)
                {
                    winner = lastPlay.player;
                    return true;
                }
            }
            return false;
        }

        public bool StartGame(out PlayerId? winner)
        {
            winner = null;
            this._gameIsRunning = true;
            while (this._gameIsRunning)
            {
                this.PlayNextTurn();
                if (this.TryGetWinner(out winner))
                    return true;
            }
            return false;
        }

        private void EndGame()
        {
            this.OnEndGame?.Invoke();
        }

        private bool GetFirstAvailableRow(int col, out int row)
        {
            for (row = 0; row < this._gameBoard.Size.y; ++row)
                if (this._gameBoard[row, col] == GameBoard.CellState.Empty)
                    return true;
            return false;
        }

        private bool ValidatePlay(PlayInput playInput, out PlayOutput playOutput)
        {
            playOutput = null;
            if (playInput.column < 0 || playInput.column >= this._gameBoard.Size.x)
                return false;
            if (this.GetFirstAvailableRow(playInput.column, out int row))
                return false;
            if (this._gameBoard[row, playInput.column] != GameBoard.CellState.Empty)
                return false;
            playOutput = new PlayOutput
            {
                player = this.CurrentPlayerId,
                x = playInput.column,
                y = row
            };
            return true;
        }

        private void PlayNextTurn()
        {
            PlayInput newPlay = this._players[this._currentPlayerId].PlayTurn(this.LastPlay);
            if (this.ValidatePlay(newPlay, out PlayOutput playOutput))
            {
                Debug.LogWarning($"Invalid play (column {newPlay.column}), aborting game");
                this.EndGame();
            }
            this.Plays.Add(playOutput);
            this.OnPlayTurn?.Invoke(playOutput);
        }
    }
}