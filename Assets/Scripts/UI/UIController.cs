using UnityEngine;
using UnityEngine.UI;
using Connect4.Game;

namespace Connect4.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameController _gameController;
        [SerializeField] private GameObject _startMenu;
        [SerializeField] private GameObject _endScreen;

        private APlayer[] CreatePlayers()
        {
            return new APlayer[] {
                new Player{ Id = PlayerId.Player1 },
                new Player{ Id = PlayerId.Player2 }
            };
        }

        public void Restart()
        {
            this._endScreen.SetActive(false);
            this._startMenu.SetActive(true);
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
            this._endScreen.SetActive(true);
            if (!winner.HasValue)
            {
                this._endScreen.transform.Find("Interruption").gameObject.SetActive(true);
            }
            else
            {
                GameObject winnerText = this._endScreen.transform.Find("Winner").gameObject;
                winnerText.gameObject.SetActive(true);
                winnerText.GetComponent<Text>().text = $"{winner.Value} won!";
            }
        }
    }
}