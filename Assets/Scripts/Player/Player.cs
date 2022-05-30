using System;
using System.Collections;

namespace Connect4
{
    public class Player : IPlayer
    {
        public IEnumerator PlayTurn(PlayOutput lastPlay, Action<PlayInput> callback)
        {
            callback(new PlayInput { column = 0 });
            yield return null;
        }
    }
}