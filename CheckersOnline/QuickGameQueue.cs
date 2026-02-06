using System.Collections.Concurrent;
using System.Web;

namespace CheckersOnline;

public class QuickGameQueue
{
    private readonly ConcurrentDictionary<string, byte> _waitingPlayers = new ();

    public List<string>? EnqueuePlayer(string connectionId)
    {
        if (!_waitingPlayers.TryAdd(connectionId, 0)) return null; 
        Console.WriteLine($"{connectionId} joined the game queue. {_waitingPlayers.Count}");
        
        if (_waitingPlayers.Count >= 2)
        {
            List<string> opponents = new List<string>();
            int i = 0;
            string key;
            foreach (var player in _waitingPlayers)
            {
                key = player.Key;
                if(!_waitingPlayers.TryRemove(player.Key, out _)) continue;
                opponents.Add(key);
                i++;
                if (i == 2) break;
            }

            if (opponents.Count == 2) return opponents;
            foreach (var player in opponents)
            {
                _waitingPlayers.TryAdd(player, 0);
            }
        }
        return null;
    }
    
    public int Count() => _waitingPlayers.Count;
    public bool TryRemove(string playerId) => _waitingPlayers.TryRemove(playerId, out _);
}