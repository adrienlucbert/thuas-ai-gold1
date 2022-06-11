using UnityWeld.Binding;

[Binding]
public class StartOptionsViewModel : ViewModel
{
    public string[] PlayerTypesOptions => new string[]
    {
            "Player",
            "AI"
    };

    private string _player1Type = "Player";
    [Binding]
    public string Player1Type
    {
        get { return this._player1Type; }
        set
        {
            this._player1Type = value;
            this.OnPropertyChanged("Player1Type");
        }
    }

    private string _player2Type = "AI";
    [Binding]
    public string Player2Type
    {
        get { return this._player2Type; }
        set
        {
            this._player2Type = value;
            this.OnPropertyChanged("Player2Type");
        }
    }
}