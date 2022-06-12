# Connect 4 AI

This is the first gold achievements for the Artificial Intelligence class in 
THUAS' Game Development & Simulation minor.

## Instructions

Write an AI for the opponent in a game of Connect 4 with the help of Monte Carlo
simulation and the MiniMax algorithm.

### Requirements

- the field is 7 columns x 6 rows
- the waiting time for the opponent to react is between 1-3 seconds

### Result

<div align="center">
  <img src="https://user-images.githubusercontent.com/42178413/173247724-341f5e0d-0628-4f99-afde-11e4c3d6373a.gif" alt="project demo"/>
  <p><i>Project demo</i></p>
</div>

The player is first prompted with a menu that lets them choose to play against an AI, another player, or let 2 AI play against each other.
The first half of the game, the AI uses MCTS with 20000 iterations and a maximum time of 3 seconds. The second half of the game, it uses MiniMax with Alpha-Beta pruning and a maximum lookahead depth of 7 moves.

## Credits

This project is the work of [Adrien Lucbert](https://github.com/adrienlucbert),
and the Artificial Intelligence class was given by Dave Stikkolorum.
