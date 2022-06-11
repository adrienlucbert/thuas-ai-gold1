using UnityEngine;
using UnityEngine.UI;
using Connect4.Game;

namespace Connect4.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private StartOptionsViewModel _startOptionsViewModel;
        [SerializeField] private GameController _gameController;
        [SerializeField] private GameObject _startMenu;
        [SerializeField] private GameObject _endScreen;

        private APlayer CreatePlayer(PlayerId Id, string type)
        {
            switch (type)
            {
                case "AI":
                    return new AIPlayer { Id = Id };
                case "Player":
                    return new Player { Id = Id };
            }
            throw new System.Exception($"Unsupported player type: {type}");
        }

        private APlayer[] CreatePlayers()
        {
            return new APlayer[] {
                this.CreatePlayer(PlayerId.Player1, this._startOptionsViewModel.Player1Type),
                this.CreatePlayer(PlayerId.Player2, this._startOptionsViewModel.Player2Type)
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
            GameObject winnerText = this._endScreen.transform.Find("Winner").gameObject;
            GameObject interruptionText = this._endScreen.transform.Find("Interruption").gameObject;
            if (!winner.HasValue)
            {
                winnerText.SetActive(false);
                interruptionText.SetActive(true);
            }
            else
            {
                interruptionText.SetActive(false);
                winnerText.SetActive(true);
                winnerText.GetComponent<Text>().text = $"{winner.Value} won!";
            }
        }
    }
}