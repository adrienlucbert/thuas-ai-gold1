using System;
using UnityEngine;
using UnityEngine.Events;

namespace Connect4.Game
{
    [RequireComponent(typeof(GameManager))]
    public class GameController : MonoBehaviour
    {
        public UnityEvent<PlayOutput> OnPlayTurn;
        public UnityEvent OnEndGame;

        public IPlayer Player1;
        public IPlayer Player2;
        public IPlayer[] Players => new IPlayer[] { this.Player1, this.Player2 };

        private int _currentPlayerId = (int)PlayerId.Player1;
        private bool _gameIsRunning = false;
        private GameManager _gameManager;

        public void Awake()
        {
            Debug.Assert(this.TryGetComponent(out this._gameManager));
        }

        /// <summary>
        /// Start the game and returns the winner if applicable.
        /// </summary>
        /// <param name="winner">Winner player id if the game was won</param>
        /// <returns>True if the game was won, false if it is invalid</returns>
        public void StartGame()
        {
            Debug.Assert(this.Player1 != null, "Player 1 must be assigned a value");
            Debug.Assert(this.Player2 != null, "Player 2 must be assigned a value");
            this._gameIsRunning = true;
            while (this._gameIsRunning)
            {
                if (this.PlayNextTurn(out PlayerId? winner))
                {
                    Debug.Log($"{winner} won the game!");
                    return;
                }
            }
            Debug.LogWarning("Game interrupted");
        }

        private void EndGame()
        {
            this._gameIsRunning = false;
        }

        private bool PlayNextTurn(out PlayerId? winner)
        {
            PlayInput newPlay = this.Players[this._currentPlayerId].PlayTurn(this._gameManager.LastPlay);
            /*try
            {*/
            bool isWon = this._gameManager.PlayTurn(newPlay, out winner);
            this.OnPlayTurn?.Invoke(this._gameManager.LastPlay);
            return isWon;
            /*}
            catch (Exception e)
            {
                winner = null;
                Debug.LogWarning(e.Message);
                this.EndGame();
                return false;
            }*/
        }
    }
}