using System;
using System.Collections;

namespace Connect4
{
    public interface IPlayer
    {
        public IEnumerator PlayTurn(PlayOutput lastPlay, Action<PlayInput> callback);
    }
}