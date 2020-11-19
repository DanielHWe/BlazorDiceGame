using System.Collections.Generic;

namespace DiceGame.Model
{
    public interface IGameFieldModel
    {
        IList<IPieceModel> Pieces { get; }
        IPlayerModel[] Player { get; }

        void ResetChangedPieces();
    }
}