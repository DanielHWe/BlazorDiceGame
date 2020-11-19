using DiceGame.Helper;
using DiceGame.Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Model
{
    public class MoveInfoExtended: MoveInfo
    {        

        public MoveInfoExtended(MoveInfo p, String pName)
        {
            this.PlayerId = p.PlayerId;
            this.GameId = p.GameId;
            this.Dice = p.Dice;
            this.Finished = p.Finished;
            this.MeepleId = p.MeepleId;
            this.PlayerName = pName;
            this.MoveId = p.MoveId;
            DiceView = DiceUnicode.GetUnicode(p.Dice).ToString();
        }

        public String PlayerName { get; set; }

        public String DiceView { get; set; }
    }
}
