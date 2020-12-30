using DiceGame.Interfaces.Messages;
using System;
using System.Collections.Generic;

namespace DiceGame.Model
{
    public interface IPlayerModel
    {
        String BackColor { get; set; }
        String Color { get; }
        global::System.Int32 Id { get; set; }
        global::System.Boolean IsActive { get; set; }
        global::System.Boolean IsPlaying { get; set; }
        global::System.Int32 LastDice { get; set; }
        String LastDiceSeries { get; set; }
        String MarkColor { get; set; }
        IMeepleModel[] Meeples { get; set; }
        String Name { get; set; }
        List<IMoveModel> PossibleMoves { get; set; }
        IPieceModel StartPosition { get; set; }

        void CalcMoves();
        void DiceAndCalcMove();
        IMoveModel Move();

        bool IsLocalPlayer { get; set; }

        IMoveModel TryMove(IPieceModel piece, Action resetBeforeMove);
        void RemoteDice(int dice);
        IMoveModel RemoteMove(MoveInfo move);
    }
}