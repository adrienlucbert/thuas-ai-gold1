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

    public enum GameState
    {
        Ongoing,
        Won,
        Draw
    }

    public class GameManager : ICloneable
    {
        public List<PlayOutput> Plays { get; set; }
        public PlayerId CurrentPlayerId { get; private set; }
        public GameState State = GameState.Ongoing;
        public bool HasEnded => this.State != GameState.Ongoing;

        public PlayOutput LastPlay => this.Plays.Count > 0 ? this.Plays[this.Plays.Count - 1] : null;
        public PlayerId? Winner = null;
        public GameBoard GameBoard;
        private int _winningRow; // number of owned cells in a row needed to win

        public GameManager(GameBoard board, PlayerId currentPlayer = PlayerId.Player1, int winningRow = 4)
        {
            this.GameBoard = board;
            this.Plays = new List<PlayOutput>();
            this.CurrentPlayerId = currentPlayer;
            this._winningRow = winningRow;
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
                while (this.GameBoard[pos.y, pos.x] == this.GameBoard[lastPlay.y, lastPlay.x])
                {
                    pos += direction.Item1;
                    ++alignedCells;
                }
                pos = new Vector2Int(lastPlay.x, lastPlay.y) + direction.Item2;
                while (this.GameBoard[pos.y, pos.x] == this.GameBoard[lastPlay.y, lastPlay.x])
                {
                    pos += direction.Item2;
                    ++alignedCells;
                }
                if (alignedCells >= this._winningRow)
                {
                    winner = lastPlay.player;
                    return true;
                }
            }
            return false;
        }

        public bool GetFirstAvailableRow(int col, out int row)
        {
            for (row = 0; row < this.GameBoard.Size.y; ++row)
            {
                if (!this.GameBoard[row, col].HasValue)
                    return true;
            }
            return false;
        }

        private bool ValidatePlay(PlayInput playInput, out PlayOutput playOutput)
        {
            playOutput = null;
            if (playInput.column < 0 || playInput.column >= this.GameBoard.Size.x)
                return false;
            if (!this.GetFirstAvailableRow(playInput.column, out int row))
                return false;
            if (this.GameBoard[row, playInput.column].HasValue)
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
        /// if the turn won the game or led to a draw.
        /// </summary>
        /// <param name="play">Player input</param>
        /// <param name="winner">Winner player id if the turn won the game</param>
        /// <returns>True if the game has ended, false otherwise</returns>
        /// <exception cref="Exception">Thrown if player input is invalid</exception>
        public bool PlayTurn(PlayInput play, out PlayerId? winner)
        {
            if (!this.ValidatePlay(play, out PlayOutput playOutput))
                throw new Exception($"Invalid play (column {play.column})");
            this.GameBoard[playOutput.y, playOutput.x] = this.CurrentPlayerId;
            this.Plays.Add(playOutput);
            this.CurrentPlayerId = (PlayerId)(((int)this.CurrentPlayerId + 1) % 2);
            bool isWon = this.TryGetWinner(out this.Winner);
            winner = this.Winner;
            if (isWon)
                this.State = GameState.Won;
            else if (this.GameBoard.IsFull())
                this.State = GameState.Draw;
            return this.State != GameState.Ongoing;
        }

        public object Clone()
        {
            GameManager clone = new GameManager((GameBoard)this.GameBoard.Clone(), this.CurrentPlayerId);
            clone.Plays = new List<PlayOutput>(this.Plays);
            clone.State = this.State;
            clone.Winner = this.Winner;
            return clone;
        }
    }
}