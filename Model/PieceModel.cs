using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Model
{
    public class PieceModel
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public bool Changed { get; set; }
        public bool IsPossibleTargetOfMove { get; set; }
        public string ChangedColor { get; set; }
        public string Color { get; set; }

        public MeepleModel Meeple { get; set; }

        public PieceModel LastPiece { get; set; }
        public PieceModel NextPiece { get; set; }

        public PieceModel NextOptionalPiece { get; set; }


    }
}
