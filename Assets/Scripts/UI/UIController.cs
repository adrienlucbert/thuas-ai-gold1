using UnityEngine;
using Connect4.Game;

namespace Connect4.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameController _gameController;
        [SerializeField] private GameObject _startMenu;

        private APlayer[] CreatePlayers()
        {
            return new APlayer[] {
                new Player{ Id = PlayerId.Player1 },
                new Player{ Id = PlayerId.Player2 }
            };
        }

        public void StartGame()
        {
            APlayer[] players = this.CreatePlayers();
            this._gameController.Player1 = players[0];
            this._gameController.Player2 = players[1];
            this._gameController.StartGame();
            this._startMenu.SetActive(false);
        }

        public void OnEndGame(PlayerId? winner)
        {
            if (!winner.HasValue)
                Debug.Log("Game was interrupted");
            else
                Debug.Log($"Player {winner.Value} won!");
        }
    }
}