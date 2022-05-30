using UnityEngine;
using Connect4.Game;

namespace Connect4.UI
{
    public class StartMenuController : MonoBehaviour
    {
        [SerializeField] private GameController _gameController;
        [SerializeField] private StartOptionsViewModel _vm;

        private IPlayer[] CreatePlayers()
        {
            return new IPlayer[] {
                new Player(),
                new Player()
            };
        }

        public void StartGame()
        {
            IPlayer[] players = this.CreatePlayers();
            this._gameController.Player1 = players[0];
            this._gameController.Player2 = players[1];
            this.gameObject.SetActive(false);
            this._gameController.StartGame();
        }
    }
}