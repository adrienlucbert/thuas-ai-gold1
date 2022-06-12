using System;
using System.Collections.Generic;
using Connect4.Game;

namespace Connect4.AI
{
    public class MCTS
    {
        private class Node
        {
            public PlayInput Play;
            public List<Node> Children { get; private set; }
            private GameSimulation _state;
            private Node _parent;
            private Queue<PlayInput> _untriedPlays;
            private Dictionary<int, int> _results;
            private int _visits = 0;
            private int _wins => this._results[((int)this._state.CurrentPlayerId + 1) % 2];
            private int _losses => this._results[(int)this._state.CurrentPlayerId];
            public float Score => (float)(this._wins - this._losses) / (float)this._visits;
            public bool IsTerminalNode => this._state.State != GameState.Ongoing;
            public bool IsFullyExpanded => this._untriedPlays.Count == 0;

            public Node(GameSimulation state, Node parent = null, PlayInput play = null)
            {
                this._state = state;
                this._parent = parent;
                this.Play = play;
                this.Children = new List<Node>();
                this._untriedPlays = new Queue<PlayInput>(this._state.GetAvailablePlays());
                this._results = new Dictionary<int, int>
                {
                    { -1, 0 },
                    { (int)PlayerId.Player1, 0 },
                    { (int)PlayerId.Player2, 0 }
                };
            }

            public Node Expand()
            {
                PlayInput move = this._untriedPlays.Dequeue();
                GameSimulation nextState = this._state.Next(move);
                Node child = new Node(nextState, this, move);
                this.Children.Add(child);
                return child;
            }

            public int Rollout()
            {
                GameSimulation currentRolloutState = this._state;
                while (currentRolloutState.State == GameState.Ongoing)
                {
                    List<PlayInput> possibleMoves = currentRolloutState.GetAvailablePlays();
                    PlayInput randomMove = possibleMoves[UnityEngine.Random.Range(0, possibleMoves.Count)];
                    currentRolloutState = currentRolloutState.Next(randomMove);
                }
                if (this._state.State == GameState.Won)
                    return (int)this._state.Winner;
                return -1;
            }

            public void BackPropagate(int result)
            {
                this._visits += 1;
                this._results[result] += 1;
                if (this._parent != null)
                    this._parent.BackPropagate(result);
            }

            public Node GetBestChild()
            {
                Node bestChild = null;
                float bestScore = int.MinValue;
                foreach (Node child in this.Children)
                {
                    float score = child.Score;
                    if (bestChild == null || score > bestScore)
                    {
                        bestChild = child;
                        bestScore = score;
                    }
                }
                return bestChild;
            }
        }

        private Node _root;

        private MCTS(GameSimulation initialState)
        {
            this._root = new Node(initialState);
        }

        private Node SelectNextNode()
        {
            Node currentNode = this._root;
            while (!currentNode.IsTerminalNode)
            {
                if (!currentNode.IsFullyExpanded)
                    return currentNode.Expand();
                else
                    currentNode = currentNode.Children[UnityEngine.Random.Range(0, currentNode.Children.Count)];
            }
            return currentNode;
        }

        private PlayInput RunMCTS(int iterations, float maxTime)
        {
            DateTime endTime = DateTime.Now.AddSeconds(maxTime);
            for (int i = 0; i < iterations && DateTime.Now < endTime; ++i)
            {
                Node nextNode = this.SelectNextNode();
                int reward = nextNode.Rollout();
                nextNode.BackPropagate(reward);
            }
            return this._root.GetBestChild().Play;
        }

        public static PlayInput GetBestMove(GameSimulation state, int iterations = 20000, float maxTime = 3f)
        {
            return new MCTS(state).RunMCTS(iterations, maxTime);
        }
    }
}