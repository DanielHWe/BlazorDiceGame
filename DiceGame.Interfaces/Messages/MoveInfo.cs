using System;
using System.Collections.Generic;
using System.Text;

namespace DiceGame.Interfaces.Messages
{
    public class MoveInfo
    {
        public String GameId { get; set; }
        public int MoveId { get; set; }
        public int PlayerId { get; set; }
        public int Dice { get; set; }
        public int MeepleId { get; set; }

        public bool Finished { get; set; }

        public MoveInfo Clone()
        {
            return new MoveInfo()
            {
                GameId = this.GameId,
                MoveId = this.MoveId,
                PlayerId = this.PlayerId,
                Dice = this.Dice,
                MeepleId = this.MeepleId,
                Finished = this.Finished
            };
        }
    }
}
