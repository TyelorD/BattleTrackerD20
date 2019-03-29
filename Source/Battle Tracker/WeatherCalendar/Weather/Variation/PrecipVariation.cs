using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather_Calendar.Weather.Variation
{
    public class PrecipVariation : TimedChance
    {
        #region Public Static Fields

        public static readonly double[] PRECIP_CHANCE_MONTH = { 0.4, 0.55, 0.45, 0.35, 0.3, 0.25, 0.25, 0.35, 0.5, 0.45, 0.4, 0.35 };
        public static readonly Dice[] SNOW_ACCUMULATION = {
            new Dice { Sides = 1, Factor = 0 },

            new Dice { Sides = 20, Factor = 0.025 },

            new Dice { Sides = 5, Factor = 0.05 },

            new Dice { Sides = 2, Factor = 0.4 },

            new Dice { Sides = 2 },

            new Dice { Sides = 1, Modifier = 2 }
        };

        public static readonly PrecipVariation[] PRECIP_VARIATION_TABLE =
        {
            // Number on d100 1-10, Duration 1d3 Days, Precip. Level Light
            new PrecipVariation { ChanceRangeMin = 1, ChanceRangeMax = 10, DurationDice = new Dice { Sides = 3 },
                DurationUnit = DurationUnits.Days, PrecipChange = new Dice { Sides = 1, Modifier = 1 } },

            // Number on d100 11-15, Duration 1d8 Hours, Precip. Level Drizzle or Light
            new PrecipVariation { ChanceRangeMin = 11, ChanceRangeMax = 15, DurationDice = new Dice { Sides = 8 },
                DurationUnit = DurationUnits.Hours, PrecipChange = new Dice { Sides = 2 } },

            // Number on d100 16-40, Duration 2d12 Hours, Precip. Level Drizzle
            new PrecipVariation { ChanceRangeMin = 16, ChanceRangeMax = 40, DurationDice = new Dice { Count = 2, Sides = 12 },
                DurationUnit = DurationUnits.Hours, PrecipChange = new Dice { Sides = 1 } },

            // Number on d100 41-45, Duration 1d8 Hours, Precip. Level Light or Medium
            new PrecipVariation { ChanceRangeMin = 41, ChanceRangeMax = 45, DurationDice = new Dice { Sides = 8 },
                DurationUnit = DurationUnits.Hours, PrecipChange = new Dice { Sides = 2, Modifier = 1 } },

            // Number on d100 46-75, Duration 2d12 Hours, Precip. Level Light
            new PrecipVariation { ChanceRangeMin = 46, ChanceRangeMax = 75, DurationDice = new Dice { Count = 2, Sides = 12 },
                DurationUnit = DurationUnits.Hours, PrecipChange = new Dice { Sides = 1, Modifier = 1 } },

            // Number on d100 76-85, Duration 1d6 Hours, Precip. Level Medium
            new PrecipVariation { ChanceRangeMin = 76, ChanceRangeMax = 85, DurationDice = new Dice { Sides = 6 },
                DurationUnit = DurationUnits.Hours, PrecipChange = new Dice { Sides = 1, Modifier = 2 } },

            // Number on d100 86-95, Duration 2d12 Hours, Precip. Level Medium
            new PrecipVariation { ChanceRangeMin = 86, ChanceRangeMax = 95, DurationDice = new Dice { Count = 2, Sides = 12 },
                DurationUnit = DurationUnits.Hours, PrecipChange = new Dice { Sides = 1, Modifier = 2 } },

            // Number on d100 96-99, Duration 1d3 Days, Precip. Level Medium
            new PrecipVariation { ChanceRangeMin = 96, ChanceRangeMax = 99, DurationDice = new Dice { Sides = 3 },
                DurationUnit = DurationUnits.Days, PrecipChange = new Dice { Sides = 1, Modifier = 2 } },

            // Number on d100 100-100, Duration 1d8 Hours, Precip. Level Storm
            new PrecipVariation { ChanceRangeMin = 100, ChanceRangeMax = 100, DurationDice = new Dice { Sides = 8 },
                DurationUnit = DurationUnits.Hours, PrecipChange = new Dice { Sides = 1, Modifier = 4 } },
        };

        public static readonly PrecipVariation[] OVERCAST_VARIATION_TABLE =
        {
            // Number on d100 1-50, Duration 1 Day, Cloud Cover None
            new PrecipVariation { ChanceRangeMin = 1, ChanceRangeMax = 50, DurationDice = new Dice { Sides = 1 },
                DurationUnit = DurationUnits.Days, PrecipChange = new Dice { Sides = 1, Modifier = -1 } },

            // Number on d100 51-70, Duration 1 Day, Cloud Cover Light
            new PrecipVariation { ChanceRangeMin = 51, ChanceRangeMax = 70, DurationDice = new Dice { Sides = 1 },
                DurationUnit = DurationUnits.Days, PrecipChange = new Dice { Sides = 1 } },

            // Number on d100 71-85, Duration 1 Day, Cloud Cover Medium
            new PrecipVariation { ChanceRangeMin = 71, ChanceRangeMax = 85, DurationDice = new Dice { Sides = 1 },
                DurationUnit = DurationUnits.Days, PrecipChange = new Dice { Sides = 1, Modifier = 1 } },

            // Number on d100 86-100, Duration 1 Day, Cloud Cover Heavy
            new PrecipVariation { ChanceRangeMin = 86, ChanceRangeMax = 100, DurationDice = new Dice { Sides = 1 },
                DurationUnit = DurationUnits.Days, PrecipChange = new Dice { Sides = 1, Modifier = 2 } },
        };

        #endregion

        #region Public Properties

        public Dice PrecipChange { get; set; }

        public int Precipitiation
        {
            get
            {
                return PrecipChange.Roll();
            }
        }

        #endregion

    }
}
