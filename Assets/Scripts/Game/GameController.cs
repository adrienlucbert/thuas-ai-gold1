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
        public UnityEvent<GameState, PlayerId?> OnEndGame;
        public Vector2Int BoardSize = new Vector2Int(7, 6);
        public int WinningCellsInARow = 4;

        public APlayer Player1;
        public APlayer Player2;
        public APlayer[] Players => new APlayer[] { this.Player1, this.Player2 };
        public PlayerId? Winner => this._gameManager.Winner;

        private bool _gameIsRunning = false;
        private GameManager _gameManager;

        private IEnumerator RunGame()
        {
            while (this._gameIsRunning && !this._gameManager.HasEnded)
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
            GameBoard board = new GameBoard(this.BoardSize.x, this.BoardSize.y);
            this._gameManager = new GameManager(board, PlayerId.Player1, this.WinningCellsInARow);
            this._gameIsRunning = true;
            this.OnStartGame?.Invoke(board);
            StartCoroutine(this.RunGame());
        }

        private void EndGame()
        {
            this.OnEndGame?.Invoke(this._gameManager.State, this.Winner);
        }

        private void InterruptGame()
        {
            this._gameIsRunning = false;
        }

        private IEnumerator PlayNextTurn()
        {
            yield return this.Players[(int)this._gameManager.CurrentPlayerId].PlayTurn(this._gameManager, this._gameManager.LastPlay, nextPlay =>
            {
                try
                {
                    this._gameManager.PlayTurn(nextPlay, out PlayerId? _);
                    this.OnPlayTurn?.Invoke(this._gameManager.LastPlay);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                    this.InterruptGame();
                }
            });
            yield return new WaitForFixedUpdate();
        }
    }
}