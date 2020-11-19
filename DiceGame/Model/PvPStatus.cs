using DiceGame.Interfaces.Messages;
using DiceGameFunction.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Model
{
    public class PvPStatus
    {
        public bool Connected { get; set; }
        public String GameId { get; set; }
        public Exception Error { get; set; }

        public PvPGameModel[] OpenGames { get; set; }
    }
}
