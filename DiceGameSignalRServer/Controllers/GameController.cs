using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiceGame.Interfaces.Messages;
using DiceGameFunction.Model;
using DiceGameSignalRServer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DiceGameSignalRServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {        
        private readonly ILogger<GameController> _logger;

        public GameController(ILogger<GameController> logger)
        {
            _logger = logger;
        }

        [HttpGet("ListGames")]
        public IEnumerable<PvPGameModel> ListGames()
        {            
            return  DataHelper.Instance.GetOpenGames();
        }

        [HttpGet("PlayerInfo")]
        public IEnumerable<PlayerInfo> PlayerInfo(String gameId)
        {
            return DataHelper.Instance.GetPlayerInfo(gameId);
        }

        [HttpGet("MoveInfo")]
        public IEnumerable<MoveInfo> MoveInfo(String gameId)
        {
            return DataHelper.Instance.GetRunningGame(gameId).Moves;
        }
    }
}
