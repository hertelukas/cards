namespace cards.Data.Game.Implementations.President
{
    public class PersistentInformation : IPersistentInformation
    {
        internal readonly int PresidentIndex;
        internal readonly int VicePresidentIndex;
        internal readonly int HighScumIndex;
        internal readonly int ScumIndex;

        public PersistentInformation(int presidentIndex, int vicePresidentIndex, int highScumIndex, int scumIndex)
        {
            PresidentIndex = presidentIndex;
            VicePresidentIndex = vicePresidentIndex;
            HighScumIndex = highScumIndex;
            ScumIndex = scumIndex;
        }
    }
}
