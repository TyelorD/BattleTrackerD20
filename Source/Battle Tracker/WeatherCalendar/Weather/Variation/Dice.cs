



using System;

namespace Weather_Calendar.Weather.Variation
{
    public class Dice
    {
        #region Private Static Fields

        private static Random random = new Random();

        #endregion

        #region Public Properties

        public int Sides { get; set; } = 100;
        public int Count { get; set; } = 1;
        public int Modifier { get; set; } = 0;
        public double Factor { get; set; } = 1;

        #endregion

        #region Public Methods

        public int Roll(int rollCount = 1)
        {
            int sum = 0;

            int count = Count;
            if(rollCount > 1)
            {
                count = rollCount;
            }
            
            for (int i = 0; i < count; i++)
            {
                sum += random.Next(0, Sides) + Modifier + 1;
            }

            sum = (int)(sum * Factor);

            return sum;
        }

        #endregion

    }
}
