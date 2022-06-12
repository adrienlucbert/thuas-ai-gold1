using System.Collections.Generic;
using UnityEngine;
using Connect4.Game;

namespace Connect4.AI
{
    public static class MiniMax
    {
        private struct EvaluationContext
        {
            public PlayerId Player;
            public int MaxDepth;
            public int Depth;
            public int EvaluatedScenarios;
        }

        public static PlayInput GetBestMove(GameSimulation state, int maxDepth = 4)
        {
            List<PlayInput> availablePlays = state.GetAvailablePlays();
            PlayInput bestMove = null;
            int? bestScore = null;
            EvaluationContext context = new EvaluationContext
            {
                Player = state.CurrentPlayerId,
                MaxDepth = maxDepth,
                EvaluatedScenarios = 0
            };
            foreach (PlayInput play in availablePlays)
            {
                int score = MiniMax.RunMiniMax(state.Next(play), maxDepth - 1, int.MinValue, int.MaxValue, true, ref context);
                if (!bestScore.HasValue || score > bestScore.Value)
                {
                    bestMove = play;
                    bestScore = score;
                }
            }
            return bestMove;
        }

        private static int EvaluateState(GameSimulation state, int depth, ref EvaluationContext context)
        {
            context.EvaluatedScenarios++;
            if (state.State == GameState.Draw)
                return 1;
            if (state.State == GameState.Won)
            {
                if (state.Winner.Value == context.Player)
                    return 2;
                else
                    return -1;
            }
            return 0;
        }

        private static int RunMiniMax(GameSimulation state, int maxDepth, int alpha, int beta, bool lookForMinimum, ref EvaluationContext context)
        {
            if (maxDepth == 0 || state.HasEnded)
                return MiniMax.EvaluateState(state, context.MaxDepth - maxDepth, ref context);

            List<PlayInput> availablePlays = state.GetAvailablePlays();
            if (lookForMinimum)
            {
                int minScore = int.MaxValue;
                foreach (PlayInput play in availablePlays)
                {
                    int score = MiniMax.RunMiniMax(state.Next(play), maxDepth - 1, alpha, beta, false, ref context);
                    minScore = Mathf.Min(minScore, score);
                    beta = Mathf.Min(beta, score);
                    if (beta <= alpha)
                        break;
                }
                return minScore;
            }
            else
            {
                int maxScore = int.MinValue;
                foreach (PlayInput play in availablePlays)
                {
                    int score = MiniMax.RunMiniMax(state.Next(play), maxDepth - 1, alpha, beta, true, ref context);
                    maxScore = Mathf.Max(maxScore, score);
                    alpha = Mathf.Max(alpha, score);
                    if (beta <= alpha)
                        break;
                }
                return maxScore;
            }
        }
    }
}