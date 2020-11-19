using DiceGame.Interfaces.Messages;
using DiceGameSignalRServer.Data;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGameSignalRServer.Hubs
{
    public class GameHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task Join(JoinRequest message)
        {
            if (String.IsNullOrEmpty(message.GameId))
            {
                var userIdClaim = String.Empty;// claimsPrincipal != null ? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier) : null;
                message.GameId = (DataHelper.Instance.NewGame(message.GameName, message.Name, userIdClaim )).Key;
                message.Id = "0";
            }
            else
            {

                var playerId = (DataHelper.Instance.AddPlayer(message.GameId, message.Name));
                message.Id = playerId.ToString();
                Debug.WriteLine($"Player {playerId}:{message.Name} joined {message.GameName}");
            }
            await Clients.All.SendAsync("join", message);                            
        }

        public async Task StartGame(String gameid)
        {
            DataHelper.Instance.StartGame(gameid);
            await Clients.All.SendAsync("StartGame", gameid);
        }

        public async Task Move(MoveInfo move)
        {
            var game = DataHelper.Instance.GetRunningGame(move.GameId);
            game.LastMoveId++;
            move.MoveId = game.LastMoveId;
            game.Moves.Add(move);

            await Clients.All.SendAsync("move", move);
        }
    }
}
