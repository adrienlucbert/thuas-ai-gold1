namespace Connect4
{
    public interface IPlayer
    {
        public PlayInput PlayTurn(PlayOutput lastPlay);
    }
}