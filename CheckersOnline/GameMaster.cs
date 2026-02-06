using System.Drawing;

namespace CheckersOnline;

public class GameMaster
{
    public string groupId { get; set; }
    public string userId1 { get; set; }
    public string userId2 { get; set; }
    public CheckersBoard board { get; set; }
    public Color user1Color { get; set; }
    public Color user2Color { get; set; }
    public int turnCount { get; set; }
    public int[] victories = new int[2];
    public bool[] rematchScores = new bool[2];

    public GameMaster(string groupId, string userId1, string userId2)
    {
        this.groupId = groupId;
        this.userId1 = userId1;
        this.userId2 = userId2;
        board = new CheckersBoard();
        user1Color = Color.White;
        user2Color = Color.Black;
        turnCount = 0;
        victories = [0, 0];
        rematchScores = [false ,false];
    }

    public void MakeMove(string from, string to)
    {
        
    }
}