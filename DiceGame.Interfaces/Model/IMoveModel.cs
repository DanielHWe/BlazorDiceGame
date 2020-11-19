namespace DiceGame.Model
{
    public interface IMoveModel
    {
        IPieceModel EndPosition { get; set; }
        IMeepleModel Meeple { get; set; }
        IPieceModel StartPosition { get; set; }
        IMeepleModel ThrownMeeple { get; set; }

        void Perform();
    }
}