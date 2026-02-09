using System.Drawing;
using Microsoft.AspNetCore.SignalR;

namespace CheckersOnline;

public class GameHub(QuickGameQueue quickGameQueue, GameDictionary gameDictionary) : Hub
{
    private readonly Random _random = new Random();

    public async Task QuickGame()
    {

        List<string>? queueAnswer = quickGameQueue.EnqueuePlayer(Context.ConnectionId);

        if (queueAnswer == null) await Clients.Caller.SendAsync("WaitingForOpponent");
        else
        {
            string groupId = "";
            for (int i = 0; i < 7; i++)
            {
                groupId += (char)_random.Next(48, 123);
            }
            gameDictionary.CreateGame(groupId, queueAnswer[0], queueAnswer[1]);
            
            await Clients.Client(queueAnswer[0]).SendAsync("GameStarted", groupId, "white");
            await Clients.Client(queueAnswer[1]).SendAsync("GameStarted", groupId, "black");
            await Groups.AddToGroupAsync(queueAnswer[0], groupId);
            await Groups.AddToGroupAsync(queueAnswer[1], groupId);
            
            Console.WriteLine($"Created game {groupId}: {gameDictionary.Count}");
        }
    }

    public async Task GetAllPositions(string groupId)
    {
        string data = gameDictionary.GetAllPositions(groupId);
        await Clients.Group(groupId).SendAsync("AllPositions",data);
    }

    public async Task Concede(string groupId)
    {
        string player = Context.ConnectionId;
        string score = gameDictionary.Concede(groupId, player);

        await Clients.Client(player).SendAsync("YouConceded", score);
        await Clients.OthersInGroup(groupId).SendAsync("EnemyConceded", new string(score.Reverse().ToArray()));
        Console.WriteLine($"Conceded game {groupId}");
    }

    public async Task Rematch(string groupId)
    {
        string player = Context.ConnectionId;
        bool data = gameDictionary.Rematch(groupId, player);

        if (data)
        {
            gameDictionary.ShuffleColors(groupId);
            GameMaster game = gameDictionary[groupId];

            await Clients.Client(game.userId1).SendAsync("GameStarted", groupId, (game.user1Color == Color.White ? "white" : "black"));
            await Clients.Client(game.userId2).SendAsync("GameStarted", groupId, (game.user2Color == Color.White ? "white" : "black"));
        }
        else await Clients.Client(player).SendAsync("WaitingForOpponent", groupId);
    }
    
    public async Task NoRematch(string groupId)
    {
        try
        {
            await Clients.OthersInGroup(groupId).SendAsync("OpponentDisconnected");
            await DeleteGroup(groupId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string player = Context.ConnectionId;
        string? groupId = gameDictionary.FindKeyByPlayerId(player);
        
        if(quickGameQueue.TryRemove(player)) await base.OnDisconnectedAsync(exception);
        if (groupId != null)
        {
            string player2 = (gameDictionary[groupId].userId1 == player) ? gameDictionary[groupId].userId2 : gameDictionary[groupId].userId1;
            await Groups.RemoveFromGroupAsync(groupId, player);
            await Groups.RemoveFromGroupAsync(groupId, player2);
            
            string score = gameDictionary.Score(groupId, player2);
            gameDictionary.Remove(groupId);
            
            await Clients.Client(player2).SendAsync("OpponentDisconnected", score);
        }
        await base.OnDisconnectedAsync(exception);
    }

    private async Task DeleteGroup(string groupId)
    {
        string player1 = gameDictionary[groupId].userId1;
        string player2 = gameDictionary[groupId].userId2;
        
        await Groups.RemoveFromGroupAsync(groupId, player1);
        await Groups.RemoveFromGroupAsync(groupId, player2);

        Console.WriteLine("DELETED GROUP");
    }
}