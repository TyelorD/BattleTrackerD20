





namespace Weather_Calendar.Weather.Variation
{
    #region Public Enums

    public enum DurationUnits { Hours, Days }

    #endregion

    public abstract class TimedChance
    {
        #region Public Static Fields

        public static readonly Dice D100 = new Dice();

        #endregion

        #region Public Properties

        public int ChanceRangeMin { get; set; }
        public int ChanceRangeMax { get; set; }
        public Dice DurationDice { get; set; }
        public DurationUnits DurationUnit { get; set; }

        public int Duration
        {
            get
            {
                return DurationDice.Roll();
            }
        }

        #endregion

        #region Public Methods

        public bool IsWithinRange(int diceRoll)
        {
            return (diceRoll >= ChanceRangeMin && diceRoll <= ChanceRangeMax);
        }

        #endregion

    }
}
