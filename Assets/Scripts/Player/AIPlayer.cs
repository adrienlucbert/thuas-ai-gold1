using System;
using System.Collections;

namespace Connect4
{
    public class AIPlayer : APlayer
    {
        public override IEnumerator PlayTurn(PlayOutput lastPlay, Action<PlayInput> callback)
        {
            callback(new PlayInput { column = 0 });
            yield return null;
        }
    }
}