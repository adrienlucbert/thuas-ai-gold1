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
            foreach (PlayOutput play in this._manager.Plays)
                res += $"{play.player}: ({play.x} {play.y})\n";
            return res;
        }
    }
}