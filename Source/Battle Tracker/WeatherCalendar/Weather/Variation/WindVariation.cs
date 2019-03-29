using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather_Calendar.Weather.Variation
{
    public class WindVariation : TimedChance
    {
        #region Public Static Fields

        public static readonly Dice[] WIND_SPEED_TABLE = {
            new Dice { Sides = 3 },

            new Dice { Sides = 7, Modifier = 3 },

            new Dice { Sides = 10, Modifier = 10 },

            new Dice { Sides = 10, Modifier = 20 },

            new Dice { Sides = 20, Modifier = 30 },

            new Dice { Sides = 50, Modifier = 50 },
        };

        public static readonly WindVariation[] WIND_VARIATION_TABLE =
        {
            // Number on d100 1-50, Duration 1 Day, Wind Level Calm
            new WindVariation { ChanceRangeMin = 1, ChanceRangeMax = 50, DurationDice = new Dice { Sides = 1 },
                DurationUnit = DurationUnits.Days, WindChange = new Dice { Sides = 1, Modifier = -1 } },

            // Number on d100 51-75, Duration 1 Day, Wind Level Light
            new WindVariation { ChanceRangeMin = 51, ChanceRangeMax = 75, DurationDice = new Dice { Sides = 1 },
                DurationUnit = DurationUnits.Days, WindChange = new Dice { Sides = 1 } },

            // Number on d100 76-90, Duration 1 Day, Wind Level Moderate
            new WindVariation { ChanceRangeMin = 76, ChanceRangeMax = 90, DurationDice = new Dice { Sides = 1 },
                DurationUnit = DurationUnits.Days, WindChange = new Dice { Sides = 1, Modifier = 1 } },

            // Number on d100 91-96, Duration 1 Day, Wind Level Strong
            new WindVariation { ChanceRangeMin = 91, ChanceRangeMax = 96, DurationDice = new Dice { Sides = 1 },
                DurationUnit = DurationUnits.Days, WindChange = new Dice { Sides = 1, Modifier = 2 } },

            // Number on d100 97-99, Duration 1 Day, Wind Level Severe
            new WindVariation { ChanceRangeMin = 97, ChanceRangeMax = 99, DurationDice = new Dice { Sides = 1 },
                DurationUnit = DurationUnits.Days, WindChange = new Dice { Sides = 1, Modifier = 3 } },

            // Number on d100 100, Duration 1 Day, Wind Level Windstorm
            new WindVariation { ChanceRangeMin = 100, ChanceRangeMax = 100, DurationDice = new Dice { Sides = 1 },
                DurationUnit = DurationUnits.Days, WindChange = new Dice { Sides = 1, Modifier = 4 } },
        };

        #endregion

        #region Public Properties

        public Dice WindChange { get; set; }

        public int Wind
        {
            get
            {
                return WindChange.Roll();
            }
        }

        #endregion

    }
}
