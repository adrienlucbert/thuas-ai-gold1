using System;
using System.Collections;
using Connect4.AI;
using Connect4.Game;

namespace Connect4
{
    public class AIPlayer : APlayer
    {
        public override IEnumerator PlayTurn(GameManager gameManager, PlayOutput lastPlay, Action<PlayInput> callback)
        {
            int totalCells = gameManager.GameBoard.Size.x * gameManager.GameBoard.Size.y;
            int emptyCells = 0;
            for (int row = 0; row < gameManager.GameBoard.Size.y; ++row)
                for (int col = 0; col < gameManager.GameBoard.Size.x; ++col)
                    if (!gameManager.GameBoard[row, col].HasValue)
                        ++emptyCells;
            float freeSpace = (float)emptyCells / (float)totalCells;
            if (freeSpace < 0.5f)
                callback(MiniMax.GetBestMove(new GameSimulation(gameManager), 7));
            else
                callback(MCTS.GetBestMove(new GameSimulation(gameManager)));
            yield return null;
        }
    }
}