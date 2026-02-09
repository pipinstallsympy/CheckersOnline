using System.Drawing;

namespace CheckersOnline;

public class GameDictionary
{
    public Dictionary<string , GameMaster> games = new ();
    public int Count => games.Count;
    public GameMaster this[string groupId] => games[groupId];
    public bool Remove(string groupId) => games.Remove(groupId);

    public string? FindKeyByPlayerId(string playerId)
    {
        string? answer = null;

        foreach (var game in games)
        {
            GameMaster g = game.Value;
            if(playerId == g.userId1 || playerId == g.userId2) return game.Key;
        }
        
        return answer;
    }
    public void CreateGame(string groupId, string userId1, string userId2)
    {
        games.Add(groupId, new GameMaster(groupId, userId1, userId2));
    }

    public string GetAllPositions(string groupId)
    {
        CheckersBoard boardCopy = games[groupId].board;
        string answer = "";

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (boardCopy[i, j] != null)
                {
                    answer += $"{i}|{j}|{(boardCopy[i, j].color == Color.White ? 0 : 1)}|{(boardCopy[i, j].isKing ? 1: 0)} ";
                }
            }
        }

        return answer;
    }

    public string Score(string groupId, string playerId)
    {
        GameMaster game = this[groupId];
        string score;
        if (playerId == game.userId1)
        {
            score = game.victories[0] + "|" + game.victories[1];
        }
        else if (playerId == game.userId2)
        {
            score = game.victories[1] + "|" + game.victories[0];
        }
        else throw new ArgumentException("playerId does not match");
        return score;
    }

    public string Concede(string groupId, string playerId)
    {
        GameMaster game = games[groupId];
        string score;
        if (playerId == game.userId1)
        {
            game.victories[1]++;
            score = game.victories[0] + "|" + game.victories[1];
        }
        else if (playerId == game.userId2)
        {
            game.victories[0]++;
            score = game.victories[1] + "|" + game.victories[0];
        }
        else throw new ArgumentException("playerId does not match");
        return score;
    }

    public bool Rematch(string groupId, string playerId)
    {
        GameMaster game = games[groupId];
        if (playerId == game.userId1)
        {
            game.rematchScores[0] = true;
        }
        else if (playerId == game.userId2)
        {
            game.rematchScores[1] = true;
        }

        if (!game.rematchScores[0] || !game.rematchScores[1]) return false;
        
        game.rematchScores[0] = false;
        game.rematchScores[1] = false;
        return true;
    }

    public void ShuffleColors(string groupId)
    {
        Random rand = new Random();
        GameMaster game = games[groupId];
        if (rand.Next(0, 2) == 0) (game.user1Color, game.user2Color) = (game.user2Color, game.user1Color);
    }
}