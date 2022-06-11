using System;
using System.Collections.Generic;
using UnityEngine;

namespace Connect4.Game
{
    public enum PlayerId
    {
        Player1 = 0,
        Player2 = 1
    }

    [RequireComponent(typeof(GameBoard))]
    public class GameManager : MonoBehaviour
    {
        public List<PlayOutput> Plays { get; private set; }
        public PlayerId CurrentPlayerId { get; private set; }

        public PlayOutput LastPlay => this.Plays.Count > 0 ? this.Plays[this.Plays.Count - 1] : null;
        private GameBoard _gameBoard;

        private void Awake()
        {
            Debug.Assert(this.TryGetComponent(out this._gameBoard));
        }

        public void OnGameStart()
        {
            this.Plays = new List<PlayOutput>();
            this.CurrentPlayerId = PlayerId.Player1;
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
                new Tuple<Vector2Int, Vector2Int>(new Vector2Int(-1,  1), new Vector2Int( 1, -1))  // left diagonal
            };
            foreach (var direction in directions)
            {
                int alignedCells = 1;
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

        private bool GetFirstAvailableRow(int col, out int row)
        {
            for (row = 0; row < this._gameBoard.Size.y; ++row)
            {
                if (!this._gameBoard[row, col].HasValue)
                    return true;
            }
            return false;
        }

        private bool ValidatePlay(PlayInput playInput, out PlayOutput playOutput)
        {
            playOutput = null;
            if (playInput.column < 0 || playInput.column >= this._gameBoard.Size.x)
                return false;
            if (!this.GetFirstAvailableRow(playInput.column, out int row))
                return false;
            if (this._gameBoard[row, playInput.column].HasValue)
                return false;
            playOutput = new PlayOutput
            {
                player = this.CurrentPlayerId,
                x = playInput.column,
                y = row
            };
            return true;
        }

        /// <summary>
        /// Plays a turn based on player input if valid and checks
        /// if the turn won the game.
        /// </summary>
        /// <param name="play">Player input</param>
        /// <param name="winner">Winner player id if the turn won the game</param>
        /// <returns>True if the game was won, false otherwise</returns>
        /// <exception cref="Exception">Thrown if player input is invalid</exception>
        public bool PlayTurn(PlayInput play, out PlayerId? winner)
        {
            if (!this.ValidatePlay(play, out PlayOutput playOutput))
                throw new Exception($"Invalid play (column {play.column})");
            this._gameBoard[playOutput.y, playOutput.x] = this.CurrentPlayerId;
            this.Plays.Add(playOutput);
            this.CurrentPlayerId = (PlayerId)(((int)this.CurrentPlayerId + 1) % 2);
            return this.TryGetWinner(out winner);
        }
    }
}