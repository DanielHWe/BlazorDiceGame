using DiceGame.Interfaces.Messages;
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
        IGameFieldModel GameField { get; }

        IPlayerModel ActivePlayer { get; }
        int LastMoveId { get; }

        Task DoDice();

        IEnumerable<IMoveModel> GetPossibleKicks();

        Task<IMoveModel> DoMove();

        Task<bool> TryMove(IPieceModel piece);
        Task<IEnumerable<DiceGame.Model.MoveInfoExtended>> GetMovesFromServer(String gameId);

        Task<IEnumerable<DiceGame.Model.MoveInfoExtended>> GetLocalMoves();
    }
}
