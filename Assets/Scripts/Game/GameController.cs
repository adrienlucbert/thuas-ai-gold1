using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Connect4.Game
{
    [RequireComponent(typeof(GameManager))]
    public class GameController : MonoBehaviour
    {
        public UnityEvent<PlayOutput> OnPlayTurn;
        public UnityEvent<PlayerId?> OnEndGame;

        public APlayer Player1;
        public APlayer Player2;
        public APlayer[] Players => new APlayer[] { this.Player1, this.Player2 };
        public PlayerId? winner;

        private bool _gameIsRunning = false;
        private GameManager _gameManager;

        public void Awake()
        {
            Debug.Assert(this.TryGetComponent(out this._gameManager));
        }

        private IEnumerator RunGame()
        {
            while (this._gameIsRunning && !this.winner.HasValue)
            {
                Debug.Log(this._gameIsRunning);
                yield return StartCoroutine(this.PlayNextTurn());
            }
            this.EndGame();
            yield return null;
        }

        /// <summary>
        /// Start the game and returns the winner if applicable.
        /// </summary>
        public void StartGame()
        {
            Debug.Assert(this.Player1 != null, "Player 1 must be assigned a value");
            Debug.Assert(this.Player2 != null, "Player 2 must be assigned a value");
            this._gameIsRunning = true;
            StartCoroutine(this.RunGame());
        }

        private void EndGame()
        {
            this.OnEndGame?.Invoke(this.winner);
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
                    bool isWon = this._gameManager.PlayTurn(nextPlay, out this.winner);
                    this.OnPlayTurn?.Invoke(this._gameManager.LastPlay);
                    if (isWon)
                        Debug.Log($"WINNER {this.winner.Value}");
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