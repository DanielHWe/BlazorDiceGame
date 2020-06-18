using DiceGame.Helper;
using DiceGame.Model;
using DiceGame.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Logic
{
    public class SinglePlayerDiceGameLogic: IDiceGameLogic
    {
        public int _currentPlayerId = 0;

        #region Properties
        public int LastDice { get; set; }
        public String LastDiceView { get; set; }

        public bool AllowMove { get; set; }
        public bool AllowDice { get { return !AllowMove; } }

        public GameFieldModel GameField { get; }

        public PlayerModel ActivePlayer
        {
            get
            {
                return GameField.Player[_currentPlayerId];
            }
        }
        #endregion

        public SinglePlayerDiceGameLogic(GameFieldModel gameFiled)
        {
            this.GameField = gameFiled;
        }

        public void DoDice()
        {
            GameField.ResetChangedPieces();
            var curr = ActivePlayer;
            curr.DiceAndCalcMove();
            SetLastDiceInfo(curr);

            if (curr.PossibleMoves.Any())
            {
                AllowMove = true;
            }
            else
            {
                if (LastDice != 6)
                {
                    SelectNextPlayer();
                }
            }

        }

        private void SetLastDiceInfo(PlayerModel curr)
        {
            LastDice = curr.LastDice;
            LastDiceView = DiceUnicode.GetUnicode(LastDice).ToString();
        }

        private void SelectNextPlayer()
        {
            ActivePlayer.IsActive = false;
            do
            {
                _currentPlayerId++;
                if (_currentPlayerId >= GameField.Player.Length || GameField.Player[_currentPlayerId] == null)
                {
                    _currentPlayerId = 0;
                }
            } while (!GameField.Player[_currentPlayerId].IsPlaying);
            ActivePlayer.IsActive = true;
        }

        public IEnumerable<MoveModel> GetPossibleKicks()
        {
            if (ActivePlayer.PossibleMoves == null) return new List<MoveModel>();
            return ActivePlayer.PossibleMoves.Where(m => m.ThrownMeeple != null).ToList();
        }

        public MoveModel DoMove()
        {
            var curr = ActivePlayer;
            if (!curr.PossibleMoves.Any()) return null;
            GameField.ResetChangedPieces();
            var move = curr.Move();

            AllowMove = false;
            if (LastDice != 6)
            {
                SelectNextPlayer();
            }
            return move;
        }

        public bool TryMove(PieceModel piece)
        {
            var curr = ActivePlayer;
            if (curr.TryMove(piece, GameField.ResetChangedPieces))
            {

                AllowMove = false;
                if (LastDice != 6)
                {
                    SelectNextPlayer();
                }
                return true;
            }
            return false;
        }
    }
}
