using System.Drawing;

namespace CheckersOnline;

public class CheckersBoard
{
    public Checker?[,] board = new Checker?[8, 8];
    
    public Checker? this[int x, int y] => board[x, y];

    public CheckersBoard()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                board[i, j] = new Checker
                {
                    color = Color.White,
                    isKing = false,
                };
            }
        }

        for (int i = 6; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                board[i, j] = new Checker
                {
                    color = Color.Black,
                    isKing = false
                };
            }
        }
    }
}