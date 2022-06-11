using System;
using System.Collections;
using UnityEngine;
using Connect4.UI;

namespace Connect4
{
    public class Player : APlayer
    {
        private PlayInput _play;

        public override IEnumerator PlayTurn(PlayOutput lastPlay, Action<PlayInput> callback)
        {
            this._play = null;
            yield return this.PreparePlayUI();
            yield return new WaitUntil(() => this._play != null);
            callback(this._play);
        }

        private IEnumerator PreparePlayUI()
        {
            yield return GameObject.Find("Game")
                .GetComponent<GameBoardRenderer>()
                ?.ShowControls(this.Id, play => this._play = play);
        }
    }
}