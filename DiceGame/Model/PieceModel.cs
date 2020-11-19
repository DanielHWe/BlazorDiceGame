using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Model
{
    public class PieceModel : IPieceModel
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public bool Changed { get; set; }
        public bool IsPossibleTargetOfMove { get; set; }
        public string ChangedColor { get; set; }
        public string Color { get; set; }

        public IMeepleModel Meeple { get; set; }

        public IPieceModel LastPiece { get; set; }
        public IPieceModel NextPiece { get; set; }

        public IPieceModel NextOptionalPiece { get; set; }


    }
}
