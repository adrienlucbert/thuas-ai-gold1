using System;
using System.Collections;
using Connect4.Game;

namespace Connect4
{
    public abstract class APlayer
    {
        public PlayerId Id;
        public abstract IEnumerator PlayTurn(PlayOutput lastPlay, Action<PlayInput> callback);
    }
}