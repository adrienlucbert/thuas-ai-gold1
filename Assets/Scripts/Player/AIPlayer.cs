using System;
using System.Collections;
using System.Collections.Generic;
using Connect4.AI;
using Connect4.Game;

namespace Connect4
{
    public class AIPlayer : APlayer
    {
        public override IEnumerator PlayTurn(GameManager gameManager, PlayOutput lastPlay, Action<PlayInput> callback)
        {
            //GameManager game = (GameManager)gameManager.Clone();
            //game.Plays = new List<PlayOutput>();
            //GameSimulation state = new GameSimulation(game);
            //PlayInput bestPlay = MiniMax.GetBestMove(state, 5);
            PlayInput bestPlay = MCTS.GetBestMove(new GameSimulation(gameManager));
            callback(bestPlay);
            yield return null;
        }
    }
}