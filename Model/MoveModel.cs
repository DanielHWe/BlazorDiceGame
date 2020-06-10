using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Model
{
    public class MoveModel
    {
        public MeepleModel Meeple { get; set; }
        public PieceModel StartPosition { get; set; }
        public PieceModel EndPosition { get; set; }
        public MeepleModel ThrownMeeple { get; set; }

        internal void Perform()
        {
            if (ThrownMeeple != null)
            {
                var oldMeeple = ThrownMeeple;
                oldMeeple.CurrentPosition = oldMeeple.BasePosition;
                oldMeeple.BasePosition.Meeple = oldMeeple;
                oldMeeple.OnBase = true;
                EndPosition.Meeple = null;

                EndPosition.Changed = true;
                oldMeeple.BasePosition.Changed = true;
            }

            StartPosition.Meeple = null;
            StartPosition.Changed = true;
            StartPosition.ChangedColor = Meeple.Player.Color;

            EndPosition.Meeple = Meeple;
            EndPosition.Changed = true;
            EndPosition.ChangedColor = Meeple.Player.Color;

            Meeple.CurrentPosition = EndPosition;
            Meeple.OnBase = false;
        }
    }
}
