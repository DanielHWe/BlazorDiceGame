using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Model
{
    public class MeepleModel : IMeepleModel
    {
        public int MeepleId { get; set; }
        public bool OnBase { get; set; } = true;
        public IPlayerModel Player { get; set; }
        public IPieceModel CurrentPosition { get; set; }

        public IPieceModel BasePosition { get; set; }
    }
}
