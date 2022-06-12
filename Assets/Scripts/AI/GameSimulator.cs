using System;
using System.Collections.Generic;
using Connect4.Game;

namespace Connect4.AI
{
    public class GameSimulation : ICloneable
    {
        private GameManager _manager;
        public List<PlayOutput> Plays => this._manager.Plays;
        public PlayerId CurrentPlayerId => this._manager.CurrentPlayerId;
        public PlayerId? Winner => this._manager.Winner;
        public GameState State => this._manager.State;
        public bool HasEnded => this._manager.HasEnded;

        public GameSimulation(GameManager gameManager)
        {
            this._manager = (GameManager)gameManager.Clone();
        }

        public List<PlayInput> GetAvailablePlays()
        {
            return this._manager.GameBoard.GetAvailablePlays();
        }

        public bool PlayTurn(PlayInput play, out PlayerId? winner)
        {
            return this._manager.PlayTurn(play, out winner);
        }

        public GameSimulation Next(PlayInput play)
        {
            GameSimulation newState = (GameSimulation)this.Clone();
            newState.PlayTurn(play, out PlayerId? _);
            return newState;
        }

        public object Clone()
        {
            return new GameSimulation(this._manager);
        }

        public override string ToString()
        {
            string res = $"State: {this.State}\n";
            for (int row = this._manager.GameBoard.Size.y - 1; row >= 0; --row)
            {
                for (int col = 0; col < this._manager.GameBoard.Size.x; ++col)
                {
                    string c;
                    if (!this._manager.GameBoard[row, col].HasValue)
                        c = "   ";
                    else if (this._manager.GameBoard[row, col].Value == PlayerId.Player1)
                        c = " x ";
                    else
                        c = " o ";
                    if (this._manager.LastPlay.x == col && this._manager.LastPlay.y == row)
                        c = c.ToUpper();
                    res += c;
                }
                res += "\n";
            }
            return res;
        }
    }
}