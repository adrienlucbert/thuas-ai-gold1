using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Connect4.Game
{
    public class GameController : MonoBehaviour
    {
        public UnityEvent<GameBoard> OnStartGame;
        public UnityEvent<PlayOutput> OnPlayTurn;
        public UnityEvent<PlayerId?> OnEndGame;

        public APlayer Player1;
        public APlayer Player2;
        public APlayer[] Players => new APlayer[] { this.Player1, this.Player2 };
        public PlayerId? Winner;

        private bool _gameIsRunning = false;
        private GameManager _gameManager;

        private IEnumerator RunGame()
        {
            while (this._gameIsRunning && !this.Winner.HasValue)
            {
                yield return StartCoroutine(this.PlayNextTurn());
            }
            this.EndGame();
        }

        /// <summary>
        /// Start the game and returns the winner if applicable.
        /// </summary>
        public void StartGame()
        {
            Debug.Assert(this.Player1 != null, "Player 1 must be assigned a value");
            Debug.Assert(this.Player2 != null, "Player 2 must be assigned a value");
            GameBoard board = new GameBoard();
            this._gameManager = new GameManager(board);
            this.Winner = null;
            this._gameIsRunning = true;
            this.OnStartGame?.Invoke(board);
            StartCoroutine(this.RunGame());
        }

        private void EndGame()
        {
            this.OnEndGame?.Invoke(this.Winner);
        }

        private void InterruptGame()
        {
            this._gameIsRunning = false;
        }

        private IEnumerator PlayNextTurn()
        {
            yield return this.Players[(int)this._gameManager.CurrentPlayerId].PlayTurn(this._gameManager.LastPlay, nextPlay =>
            {
                try
                {
                    this._gameManager.PlayTurn(nextPlay, out this.Winner);
                    this.OnPlayTurn?.Invoke(this._gameManager.LastPlay);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                    this.InterruptGame();
                }
            });
        }
    }
}