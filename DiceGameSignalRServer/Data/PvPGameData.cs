using DiceGame.Interfaces.Messages;
using DiceGameFunction.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGameSignalRServer.Data
{
    public class PvPGameData
    {
        public PvPGameModel Model { get; set; }
        public IList<PlayerInfo> Players { get; set; } = new List<PlayerInfo>();

        public IList<MoveInfo> Moves { get; set; } = new List<MoveInfo>();
        public string Key { get; internal set; }
        public string HostId { get; internal set; }
        public int LastMoveId { get; set; } = 0;
    }
}
