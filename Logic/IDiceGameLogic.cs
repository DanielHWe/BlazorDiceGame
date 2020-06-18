using DiceGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Logic
{
    public interface IDiceGameLogic
    {
        int LastDice { get; }
        String LastDiceView { get; }

        bool AllowMove { get; }
        bool AllowDice { get; }
        GameFieldModel GameField { get; }

        PlayerModel ActivePlayer { get; }

        void DoDice();

        IEnumerable<MoveModel> GetPossibleKicks();

        MoveModel DoMove();

        bool TryMove(PieceModel piece);
    }
}
