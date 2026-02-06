using System.Collections.Concurrent;
using System.Web;

namespace CheckersOnline;

public class QuickGameQueue
{
    private readonly ConcurrentQueue<string> _waitingPlayers = new ConcurrentQueue<string>();

    public List<string>? EnqueuePlayer(string connectionId)
    {
        if(!_waitingPlayers.Contains(connectionId)) _waitingPlayers.Enqueue(connectionId);
        Console.WriteLine($"{connectionId} joined the game queue. {_waitingPlayers.Count}");
        
        if (_waitingPlayers.Count >= 2)
        {
            List<string> opponents = new List<string>();
            if (_waitingPlayers.TryDequeue(out string user1) && _waitingPlayers.TryDequeue(out string user2))
            {
                opponents.Add(user1);
                opponents.Add(user2);
                Console.WriteLine($"{user1} {user2} left the queue");
            }

            if (opponents.Count < 2) return null;
            return opponents;
        }

        return null;
        
    }
    
    public int GetQueueCount() => _waitingPlayers.Count;
    
    public List<string> GetNextPlayers()
    {
        List<string> players = new List<string>();
        if(GetQueueCount() < 2) return players;
        for (int i = 0; i < 2; i++)
        {
            if(_waitingPlayers.TryDequeue(out string connectionId)) players.Add(connectionId);
        }
        return players;
    }
}