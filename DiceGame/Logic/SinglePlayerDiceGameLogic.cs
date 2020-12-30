using DiceGame.Helper;
using DiceGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public int LastMoveId { get => -1; }

        public IGameFieldModel GameField { get; }

        public IPlayerModel ActivePlayer
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

        public async Task DoDice()
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

        private void SetLastDiceInfo(IPlayerModel curr)
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

        public IEnumerable<IMoveModel> GetPossibleKicks()
        {
            if (ActivePlayer.PossibleMoves == null) return new List<MoveModel>();
            return ActivePlayer.PossibleMoves.Where(m => m.ThrownMeeple != null).ToList();
        }

        public async Task<IMoveModel> DoMove()
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

        public async Task<IMoveModel> TryMove(IPieceModel piece)
        {
            var curr = ActivePlayer;
            var move = curr.TryMove(piece, GameField.ResetChangedPieces);
            if (move !=null)
            {

                AllowMove = false;
                if (LastDice != 6)
                {
                    SelectNextPlayer();
                }
                return move;
            }
            return null;
        }

        public Task<IEnumerable<MoveInfoExtended>> GetMovesFromServer(string gameId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MoveInfoExtended>> GetLocalMoves()
        {
            throw new NotImplementedException();
        }
    }
}
