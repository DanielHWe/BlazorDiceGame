namespace DiceGame.Model
{
    public interface IPieceModel
    {
        global::System.Boolean MovedOver { get; set; }
        global::System.Boolean Changed { get; set; }
        global::System.String ChangedColor { get; set; }
        global::System.String Color { get; set; }
        string BaseColor { get; set; }
        global::System.Int32 Id { get; set; }
        global::System.Boolean IsPossibleTargetOfMove { get; set; }
        IPieceModel LastPiece { get; set; }
        IMeepleModel Meeple { get; set; }
        IPieceModel NextOptionalPiece { get; set; }
        IPieceModel NextPiece { get; set; }
        global::System.Int32 X { get; set; }
        global::System.Int32 Y { get; set; }
    }
}