namespace Connect4
{
    public class Player : IPlayer
    {
        public PlayInput PlayTurn(PlayOutput lastPlay)
        {
            return new PlayInput { column = 0 };
        }
    }
}