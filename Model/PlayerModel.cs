using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Model
{
    public class PlayerModel
    {
        private Random _r = new Random();        
        public int Id { get; set; }
        public String Name { get; set; }
        public int LastDice { get; set; }

        public String LastDiceSeries { get; set; }

        public bool IsActive { get; set; }
        public bool IsPlaying { get; set; }

        public String Color { get; }
        public String BackColor { get; set; }
        public String MarkColor { get; set; }
        public PieceModel StartPosition { get; set; }
        public MeepleModel[] Meeples { get; set; } = new MeepleModel[4];
        public List<MoveModel> PossibleMoves { get; set; } = new List<MoveModel>();

        public PlayerModel(string color, int id)
        {
            this.Color = color;
            this.Id = id;
            this.Name = "Player " + id;
            this.IsPlaying = true;
        }

        public void DiceAndCalcMove()
        {
            if (LastDice != 6) LastDiceSeries = "";

            LastDice = Math.Min(6, 1 + _r.Next(6));

            LastDiceSeries += LastDiceSeries.Length == 0 ? LastDice.ToString() : " - " + LastDice;

            PossibleMoves.Clear();
            CalcMoves();

            
        }

        public void Move()
        {
            if (PossibleMoves.Any()) PossibleMoves.First().Perform();
        }

        public bool TryMove(PieceModel piece)
        {
            foreach(var move in PossibleMoves)
            {
                if (move.EndPosition == piece)
                {
                    move.Perform();
                    return true;
                }
                if (move.StartPosition == piece)
                {
                    move.Perform();
                    return true;
                }
            }
            return false;
        }

        public void CalcMoves()
        {
            

            if (LastDice == 6)
            {
                var gone = TryGoOut();
                if (gone) return;
            }

            if (this.StartPosition.Meeple != null && this.StartPosition.Meeple.Player.Color == this.Color)
            {
                for (int meepleId = 0; meepleId < Meeples.Length; meepleId++)
                {
                    if (Meeples[meepleId].CurrentPosition != StartPosition) continue;                   
                    var gone = TryGo(meepleId);
                    if (gone) return;
                }
            }

            for (int meepleId = 0; meepleId < Meeples.Length; meepleId++)
            {
                if (Meeples[meepleId].OnBase) continue;
                var gone = TryGo(meepleId);
                //if (gone) return;
            }            
        }

        private bool TryGo(int meepleId)
        {
            var orgPosition = Meeples[meepleId].CurrentPosition;
            var newPos = Meeples[meepleId].CurrentPosition;
            for (int i = 0; i < LastDice; i++)
            {
                if (newPos.NextOptionalPiece != null && newPos.NextOptionalPiece.Color == this.Color)
                {
                    newPos = newPos.NextOptionalPiece;
                    continue;
                }
                if (orgPosition.NextPiece == null) return false;
                newPos = newPos.NextPiece;                
            }
            if (newPos == null) return false;
            if (newPos.Meeple != null)
            {
                if (newPos.Meeple.Player.Color == this.Color) return false;
                //ThrowOut(newPos);
            }

            PossibleMoves.Add(
                             new MoveModel()
                             {
                                 Meeple = Meeples[meepleId],
                                 StartPosition = orgPosition,
                                 EndPosition = newPos,
                                 ThrownMeeple = newPos.Meeple
                             }
                             );

            orgPosition.Changed = true;
            orgPosition.ChangedColor = this.Color;
            newPos.Changed = true;
            newPos.ChangedColor = this.Color;

            return true;
        }

        private bool TryGoOut()
        {
            for (int meepleId = 0; meepleId < Meeples.Length; meepleId++)
            {
                if (Meeples[meepleId].OnBase)
                {
                    if (StartPosition.Meeple == null)
                    {
                        
                        PossibleMoves.Add(
                            new MoveModel()
                            {
                                Meeple = Meeples[meepleId],
                                StartPosition = Meeples[meepleId].BasePosition,
                                EndPosition = StartPosition,
                                ThrownMeeple = StartPosition.Meeple
                            }
                            );
                        StartPosition.Changed = true;
                        return true;
                    } else if (StartPosition.Meeple.Player.Color == this.Color)
                    {
                        
                        return false;
                    } else
                    {
                        //ThrowOut(StartPosition);

                        PossibleMoves.Add(
                             new MoveModel()
                             {
                                 Meeple = Meeples[meepleId],
                                 StartPosition = Meeples[meepleId].BasePosition,
                                 EndPosition = StartPosition,
                                 ThrownMeeple = StartPosition.Meeple
                             }
                             );
                        StartPosition.Changed = true;
                        return true;
                    }
                }
                
            }

            return false;
        }

        private void ThrowOut(PieceModel position)
        {
            var oldMeeple = position.Meeple;
            oldMeeple.CurrentPosition = oldMeeple.BasePosition;
            oldMeeple.BasePosition.Meeple = oldMeeple;
            oldMeeple.OnBase = true;
            position.Meeple = null;

            position.Changed = true;
            oldMeeple.BasePosition.Changed = true;

            position.ChangedColor = this.Color;
            oldMeeple.BasePosition.ChangedColor = this.Color;
        }
    }
}
