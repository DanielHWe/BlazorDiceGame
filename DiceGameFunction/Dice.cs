using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using DiceGame.Interfaces.Messages;
using DiceGameFunction.Model;
using DiceGameFunction.Data;
using System.Security.Claims;

namespace DiceGameFunction
{
    public static class Dice
    {
        private static PvPGameModel _game = new PvPGameModel();

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "chat")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("messages")]
        public static Task SendMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] object message,
            [SignalR(HubName = "chat")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "newMessage",
                    Arguments = new[] { message }
                });
        }

        [FunctionName("Join")]
        public static async Task Join(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] JoinRequest message, ClaimsPrincipal claimsPrincipal,
            [SignalR(HubName = "chat")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            if (String.IsNullOrEmpty(message.GameId))
            {
                var userIdClaim = claimsPrincipal != null ? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier) : null;
                message.GameId = (await TableHelper.Instance.NewGame(message.GameName, message.Name, userIdClaim != null ? userIdClaim.Value : "null")).RowKey;
                message.Id = "0";
            } else
            {

                var playerId = (await TableHelper.Instance.AddPlayer(message.GameId, message.Name));
                message.Id = playerId.ToString();
            }           
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "join",
                    Arguments = new[] { message }
                });
        }

        [FunctionName("Move")]
        public static async Task Move(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] MoveInfo message,
            [SignalR(HubName = "chat")] IAsyncCollector<SignalRMessage> signalRMessages)
        {            
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "move",
                    Arguments = new[] { message }
                });
        }

        [FunctionName("StartGame")]
        public static async Task StartGame(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] MoveInfo message,
            [SignalR(HubName = "chat")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "StartGame",
                    Arguments = new[] { message }
                });
        }

        [FunctionName("ListGames")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            var openGames = await TableHelper.Instance.GetOpenGames();

            return new OkObjectResult(openGames);
        }

        [FunctionName("PlayerInfo")]
        public static async Task<IActionResult> PlayerInfo([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            string gameId = req.Query["gameId"];

            if (String.IsNullOrEmpty(gameId)) return new BadRequestResult();

            var openGames = await TableHelper.Instance.GetPlayerInfo(gameId);

            return new OkObjectResult(openGames);
        }
        
    }
}
