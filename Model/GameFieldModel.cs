using DiceGame.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Model
{
    public class GameFieldModel
    {        
        
        public int _nextIdx = 1;
        public PieceModel _lastPiece;
        public IList<PieceModel> Pieces { get; } = new List<PieceModel>();
        public PlayerModel[] Player { get; } = new PlayerModel[8];

        

        

        internal void ResetChangedPieces()
        {
            foreach(var p in Pieces)
            {
                p.Changed = false;
                p.IsPossibleTargetOfMove = false;
                p.ChangedColor = "#000000";
            }
        }

        public GameFieldModel()
        {            
            var first = AddStartPiece("Red", "White", "Yellow", 5, 1, 0);
            AddPiece("White", 5, 2);
            AddPiece("White", 5, 3);
            AddPiece("White", 5, 4);
            AddPiece("White", 5, 5);
            AddPiece("White", 4, 5);
            AddPiece("White", 3, 5);
            AddPiece("White", 2, 5);
            AddPiece("White", 1, 5);
            var blueHome = AddPiece("White", 1, 6);

            AddStartPiece("Blue", "White", "Yellow", 1, 7, 1);
            AddPiece("White", 2, 7);
            AddPiece("White", 3, 7);
            AddPiece("White", 4, 7);
            AddPiece("White", 5, 7);
            AddPiece("White", 5, 8);
            AddPiece("White", 5, 9);
            AddPiece("White", 5, 10);
            AddPiece("White", 5, 11);
            var greenHome = AddPiece("White", 6, 11);

            AddStartPiece("Green", "White", "Yellow", 7, 11, 2);
            AddPiece("White", 7, 10);
            AddPiece("White", 7, 9);
            AddPiece("White", 7, 8);
            AddPiece("White", 7, 7);
            AddPiece("White", 8, 7);
            AddPiece("White", 9, 7);
            AddPiece("White", 9, 8);
            AddPiece("White", 9, 9);
            AddPiece("White", 9, 10);
            AddPiece("White", 9, 11);
            var purpleHome = AddPiece("White", 10, 11);

            AddStartPiece("Purple", "White", "Yellow", 11, 11, 3);
            AddPiece("White", 11, 10);
            AddPiece("White", 11, 9);
            AddPiece("White", 11, 8);
            AddPiece("White", 11, 7);
            AddPiece("White", 12, 7);
            AddPiece("White", 13, 7);
            AddPiece("White", 13, 8);
            AddPiece("White", 13, 9);
            AddPiece("White", 13, 10);
            AddPiece("White", 13, 11);
            var yellowHome = AddPiece("White", 14, 11);

            AddStartPiece("Yellow", "Grey", "Blue", 15, 11, 4);
            AddPiece("White", 15, 10);
            AddPiece("White", 15, 9);
            AddPiece("White", 15, 8);
            AddPiece("White", 15, 7);
            AddPiece("White", 16, 7);
            AddPiece("White", 17, 7);
            AddPiece("White", 18, 7);
            AddPiece("White", 19, 7);
            var greyHome = AddPiece("White", 19, 6);

            AddStartPiece("Gray", "White", "Yellow", 19, 5, 5);
            AddPiece("White", 18, 5);
            AddPiece("White", 17, 5);
            AddPiece("White", 16, 5);
            AddPiece("White", 15, 5);
            AddPiece("White", 15, 4);
            AddPiece("White", 15, 3);
            AddPiece("White", 15, 2);
            AddPiece("White", 15, 1);
            var orangeHome = AddPiece("White", 14, 1);

            AddStartPiece("#00ccff", "White", "Blue", 13, 1, 6);
            AddPiece("White", 13, 2);
            AddPiece("White", 13, 3);
            AddPiece("White", 13, 4);
            AddPiece("White", 13, 5);
            AddPiece("White", 12, 5);
            AddPiece("White", 11, 5);
            AddPiece("White", 11, 4);
            AddPiece("White", 11, 3);
            AddPiece("White", 11, 2);
            AddPiece("White", 11, 1);
            var pinkHome = AddPiece("White", 10, 1);

            AddStartPiece("Pink", "White", "Blue", 9, 1, 7);
            AddPiece("White", 9, 2);
            AddPiece("White", 9, 3);
            AddPiece("White", 9, 4);
            AddPiece("White", 9, 5);
            AddPiece("White", 8, 5);
            AddPiece("White", 7, 5);
            AddPiece("White", 7, 4);
            AddPiece("White", 7, 3);
            AddPiece("White", 7, 2);
            AddPiece("White", 7, 1);
            var redHome = AddPiece("White", 6, 1);
            redHome.NextPiece = first;

            redHome = AddFinalPiece("Red", 6, 2, redHome);
            redHome = AddFinalPiece("Red", 6, 3, redHome);
            redHome = AddFinalPiece("Red", 6, 4, redHome);
            redHome = AddFinalPiece("Red", 6, 5, redHome);

            blueHome = AddFinalPiece("Blue", 2, 6, blueHome);
            blueHome = AddFinalPiece("Blue", 3, 6, blueHome);
            blueHome = AddFinalPiece("Blue", 4, 6, blueHome);
            blueHome = AddFinalPiece("Blue", 5, 6, blueHome);

            greenHome = AddFinalPiece("Green", greenHome.X, greenHome.Y-1, greenHome);
            greenHome = AddFinalPiece(greenHome.Color, greenHome.X, greenHome.Y - 1, greenHome);
            greenHome = AddFinalPiece(greenHome.Color, greenHome.X, greenHome.Y - 1, greenHome);
            greenHome = AddFinalPiece(greenHome.Color, greenHome.X, greenHome.Y - 1, greenHome);

            purpleHome = AddFinalPiece("Purple", purpleHome.X, purpleHome.Y - 1, purpleHome);
            purpleHome = AddFinalPiece(purpleHome.Color, purpleHome.X, purpleHome.Y - 1, purpleHome);
            purpleHome = AddFinalPiece(purpleHome.Color, purpleHome.X, purpleHome.Y - 1, purpleHome);
            purpleHome = AddFinalPiece(purpleHome.Color, purpleHome.X, purpleHome.Y - 1, purpleHome);

            yellowHome = AddFinalPiece("Yellow", yellowHome.X, yellowHome.Y - 1, yellowHome);
            yellowHome = AddFinalPiece(yellowHome.Color, yellowHome.X, yellowHome.Y - 1, yellowHome);
            yellowHome = AddFinalPiece(yellowHome.Color, yellowHome.X, yellowHome.Y - 1, yellowHome);
            yellowHome = AddFinalPiece(yellowHome.Color, yellowHome.X, yellowHome.Y - 1, yellowHome);

            orangeHome = AddFinalPiece("#00ccff", orangeHome.X, orangeHome.Y + 1, orangeHome);
            orangeHome = AddFinalPiece(orangeHome.Color, orangeHome.X, orangeHome.Y + 1, orangeHome);
            orangeHome = AddFinalPiece(orangeHome.Color, orangeHome.X, orangeHome.Y + 1, orangeHome);
            orangeHome = AddFinalPiece(orangeHome.Color, orangeHome.X, orangeHome.Y + 1, orangeHome);

            pinkHome = AddFinalPiece("Pink", pinkHome.X, pinkHome.Y + 1, pinkHome);
            pinkHome = AddFinalPiece(pinkHome.Color, pinkHome.X, pinkHome.Y + 1, pinkHome);
            pinkHome = AddFinalPiece(pinkHome.Color, pinkHome.X, pinkHome.Y + 1, pinkHome);
            pinkHome = AddFinalPiece(pinkHome.Color, pinkHome.X, pinkHome.Y + 1, pinkHome);

            greyHome = AddFinalPiece("Gray", greyHome.X - 1, greyHome.Y, greyHome);
            greyHome = AddFinalPiece(greyHome.Color, greyHome.X - 1, greyHome.Y, greyHome);
            greyHome = AddFinalPiece(greyHome.Color, greyHome.X - 1, greyHome.Y, greyHome);
            greyHome = AddFinalPiece(greyHome.Color, greyHome.X - 1, greyHome.Y, greyHome);

            Player[0].IsActive = true;
        }

        private PieceModel AddStartPiece(String color, String backColor, String markColor, int x, int y, int playerId)
        {
            var piece = AddPiece(color, x, y);
            Player[playerId] = new PlayerModel(color, playerId);
            Player[playerId].StartPosition = piece;
            Player[playerId].IsPlaying = playerId < 2;

            Player[playerId].Meeples[0] = CreateMeeple(Player[playerId], 0, color, playerId + 1, 14);
            Player[playerId].Meeples[1] = CreateMeeple(Player[playerId], 1, color, playerId + 1, 15);
            Player[playerId].Meeples[2] = CreateMeeple(Player[playerId], 2, color, playerId + 1, 16);
            Player[playerId].Meeples[3] = CreateMeeple(Player[playerId], 3, color, playerId + 1, 17);

            Player[playerId].BackColor = backColor;
            Player[playerId].MarkColor = markColor;

            return piece;
        }

        private MeepleModel CreateMeeple(PlayerModel player, int meepleId ,string color, int x, int y)
        {
            var newPiece = new PieceModel() { Color = color, Id = _nextIdx++, X = x, Y = y };
            Pieces.Add(newPiece);
            var meeple = new MeepleModel()
            {
                Player = player,
                BasePosition = newPiece,
                CurrentPosition = newPiece,
                OnBase = true,
                MeepleId = meepleId
            };
            newPiece.Meeple = meeple;
            return meeple;
        }

        private PieceModel AddPiece(String color, int x, int y)
        {
            var newPiece = new PieceModel() { Color = color, Id = _nextIdx++, X = x, Y = y };
            Pieces.Add(newPiece);
            if (_lastPiece != null)
            {
                _lastPiece.NextPiece = newPiece;
                newPiece.LastPiece = _lastPiece;
            }            
            _lastPiece = newPiece;
            return newPiece;
        }

        private PieceModel AddFinalPiece(String color, int x, int y, PieceModel last)
        {
            var newPiece = new PieceModel() { Color = color, Id = _nextIdx++, X = x, Y = y };
            Pieces.Add(newPiece);
            if (last != null)
            {
                last.NextOptionalPiece = newPiece;
                newPiece.LastPiece = last;
            }            
            return newPiece;
        }
    }
}
