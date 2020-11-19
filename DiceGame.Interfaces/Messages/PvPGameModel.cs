using DiceGame.Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiceGameFunction.Model
{
    public class PvPGameModel
    {
        public String Id { get; set; }

        public String GameName { get; set; }

        public String HostName { get; set; }

        public DateTime StartTime { get; set; }
    }
}
