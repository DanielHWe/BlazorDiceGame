namespace DiceGame.Model
{
    public interface IMeepleModel
    {
        IPieceModel BasePosition { get; set; }
        IPieceModel CurrentPosition { get; set; }
        global::System.Int32 MeepleId { get; set; }
        global::System.Boolean OnBase { get; set; }
        IPlayerModel Player { get; set; }
    }
}