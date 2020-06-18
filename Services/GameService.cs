using DiceGame.Logic;
using DiceGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Services
{
    public class GameService: IDiceGameLogic
    {
        private IDiceGameLogic _logic;

        public GameService()
        {
            _logic = new SinglePlayerDiceGameLogic(new Model.GameFieldModel());
        }

        public int LastDice { get => _logic.LastDice; }

        public string LastDiceView { get => _logic.LastDiceView; }

        public bool AllowMove { get => _logic.AllowMove; }

        public bool AllowDice { get => _logic.AllowDice; }
        public GameFieldModel GameField { get => _logic.GameField; }

        public PlayerModel ActivePlayer { get => _logic.ActivePlayer; }

        public void DoDice()
        {
            _logic.DoDice();
        }

        public MoveModel DoMove()
        {
            return _logic.DoMove();
        }

        public IEnumerable<MoveModel> GetPossibleKicks()
        {
            return _logic.GetPossibleKicks();
        }

        public bool TryMove(PieceModel piece)
        {
            return _logic.TryMove(piece);
        }
    }
}
