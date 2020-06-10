using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Model
{
    public class MeepleModel
    {
        public int MeepleId { get; set; }
        public bool OnBase { get; set; } = true;
        public PlayerModel Player { get; set; }
        public PieceModel CurrentPosition { get; set; }

        public PieceModel BasePosition { get; set; }
    }
}
