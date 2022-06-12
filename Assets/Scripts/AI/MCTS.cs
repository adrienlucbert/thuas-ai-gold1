using System;
using System.Collections.Generic;
using UnityEngine;
using Connect4.Game;

namespace Connect4.AI
{
    public class MCTS
    {
        private class State
        {
            public GameSimulation simulation;

            public PlayerId NextToMove => this.simulation.CurrentPlayerId;

            public int? GameResult
            {
                get
                {
                    if (this.simulation.State == GameState.Won)
                        return (int)this.simulation.Winner;
                    if (this.simulation.State == GameState.Draw)
                        return -1;
                    return null;
                }
            }

            public bool IsGameOver => this.simulation.State != GameState.Ongoing;

            public State Move(PlayInput move)
            {
                return new State { simulation = this.simulation.Next(move) };
            }

            public List<PlayInput> GetLegalActions()
            {
                return this.simulation.GetAvailablePlays();
            }
        };

        private class Node
        {
            public State state;
            public Node parent;
            public PlayInput play;
            public List<Node> children;
            Queue<PlayInput> untriedPlays;
            Dictionary<int, int> results;
            public int visits = 0;
            public int wins => this.results[(int)this.state.NextToMove];
            public int losses => this.results[((int)this.state.NextToMove + 1) % 2];
            public int draws => this.results[-1];

            float q
            {
                get
                {
                    return this.wins - this.losses;
                }
            }
            float n => this.visits;

            public float UCB1(float C)
            {
                if (this.parent == null)
                    throw new Exception("Cannot calculate UCB1 of a root node");
                return (this.q / this.n) + C * Mathf.Sqrt(Mathf.Log(this.parent.n) / this.n);
            }

            public Node(State state, Node parent = null, PlayInput play = null)
            {
                this.state = state;
                this.parent = parent;
                this.play = play;
                this.children = new List<Node>();
                this.untriedPlays = new Queue<PlayInput>(state.GetLegalActions());
                this.results = new Dictionary<int, int>
                {
                    { -1, 0 },
                    { (int)PlayerId.Player1, 0 },
                    { (int)PlayerId.Player2, 0 }
                };
            }

            public Node Expand()
            {
                PlayInput move = this.untriedPlays.Dequeue();
                State nextState = this.state.Move(move);
                Node child = new Node(nextState, this, move);
                this.children.Add(child);
                return child;
            }

            public bool IsTerminalNode => this.state.IsGameOver;

            public int Rollout()
            {
                State currentRolloutState = this.state;
                while (!currentRolloutState.IsGameOver)
                {
                    List<PlayInput> possibleMoves = currentRolloutState.GetLegalActions();
                    PlayInput move = this.RolloutPolicy(possibleMoves);
                    currentRolloutState = currentRolloutState.Move(move);
                }
                return currentRolloutState.GameResult.Value;
            }

            public void BackPropagate(int result)
            {
                this.visits += 1;
                this.results[result] += 1;
                if (this.parent != null)
                    this.parent.BackPropagate(result);
            }

            public bool IsFullyExpanded => this.untriedPlays.Count == 0;

            public Node GetBestChild(float C = 1.4f)
            {
                Node bestChild = null;
                float bestUCB1 = int.MinValue;
                foreach (Node child in this.children)
                {
                    float ucb1 = child.UCB1(C);
                    if (bestChild == null || ucb1 > bestUCB1)
                    {
                        bestChild = child;
                        bestUCB1 = ucb1;
                    }
                }
                return bestChild;
            }

            public PlayInput RolloutPolicy(List<PlayInput> possibleMoves)
            {
                return possibleMoves[UnityEngine.Random.Range(0, possibleMoves.Count - 1)];
            }

            public override string ToString()
            {
                return this.state.simulation.ToString();
            }
        }

        private Node root;

        private MCTS(GameSimulation initialState)
        {
            this.root = new Node(new State { simulation = initialState });
        }

        private Node TreePolicy()
        {
            Node currentNode = this.root;
            while (!currentNode.IsTerminalNode)
            {
                if (!currentNode.IsFullyExpanded)
                    return currentNode.Expand();
                else
                    currentNode = currentNode.GetBestChild();
            }
            return currentNode;
        }

        private PlayInput GetBestMove(int iterations, float maxTime)
        {
            DateTime endTime = DateTime.Now.AddSeconds(maxTime);
            for (int i = 0; i < iterations && DateTime.Now < endTime; ++i)
            {
                Node nextNode = this.TreePolicy();
                int reward = nextNode.Rollout();
                nextNode.BackPropagate(reward);
            }
            Node bestChild = null;
            foreach (Node child in this.root.children)
            {
                if (bestChild == null || bestChild.losses / Mathf.Max(1, bestChild.wins) < child.losses / Mathf.Max(1, child.wins))
                    bestChild = child;
                Debug.Log($"Move: {child.play.column} Visits: {child.visits} W/L/D: {child.losses}/{child.wins}/{child.draws} ({child.losses / Mathf.Max(1, child.wins)})\n{child}");
            }
            Debug.Log($"Best move: {bestChild.play.column} Visits: {bestChild.visits} W/L/D: {bestChild.losses}/{bestChild.wins}/{bestChild.draws} ({bestChild.losses / Mathf.Max(1, bestChild.wins)})\n{bestChild}");
            return bestChild.play;
        }

        public static PlayInput GetBestMove(GameSimulation state, int iterations = 10000, float maxTime = 2.5f)
        {
            MCTS instance = new MCTS(state);
            return instance.GetBestMove(iterations, maxTime);
        }
    }
}