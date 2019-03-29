using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather_Calendar.Weather.Variation
{
    public class TemperatureVariation : TimedChance
    {
        #region Public Static Fields

        public static readonly int[] BASE_TEMP_MONTH = { 36, 44, 53, 59, 64, 68, 66, 60, 50, 40, 33, 30 };
        public static readonly int[] OVERCAST_TEMP_CHANGE_MONTH = { 10, 5, 0, -5, -10, -10, -10, -5, 0, 5, 10, 10 };


        public static readonly TemperatureVariation[] TEMP_VARIATION_TABLE =
        {
            // Number on d100 1-5, Duration 1d2 Days, Temp. Change -3d10 degrees 
            new TemperatureVariation { ChanceRangeMin = 1, ChanceRangeMax = 5, DurationDice = new Dice { Sides = 2 },
                DurationUnit = DurationUnits.Days, TemperatureChange = new Dice { Count = 3, Sides = 10, Factor = -1 } },

            // Number on d100 6-15, Duration 1d4 Days, Temp. Change -2d10 degrees 
            new TemperatureVariation { ChanceRangeMin = 6, ChanceRangeMax = 15, DurationDice = new Dice { Sides = 4 },
                DurationUnit = DurationUnits.Days, TemperatureChange = new Dice { Count = 2, Sides = 10, Factor = -1 } },

            // Number on d100 16-35, Duration 1d4+1 Days, Temp. Change -1d10 degrees 
            new TemperatureVariation { ChanceRangeMin = 16, ChanceRangeMax = 35, DurationDice = new Dice { Sides = 4, Modifier = 1 },
                DurationUnit = DurationUnits.Days, TemperatureChange = new Dice { Sides = 10, Factor = -1 } },

            // Number on d100 36-50, Duration 1d6+1 Days, Temp. Change -(1d4 - 1) degrees 
            new TemperatureVariation { ChanceRangeMin = 36, ChanceRangeMax = 50, DurationDice = new Dice { Sides = 6, Modifier = 1 },
                DurationUnit = DurationUnits.Days, TemperatureChange = new Dice { Sides = 4, Modifier = -1, Factor = -1 } },

            // Number on d100 51-65, Duration 1d6+1 Days, Temp. Change 1d4 - 1 degrees 
            new TemperatureVariation { ChanceRangeMin = 51, ChanceRangeMax = 65, DurationDice = new Dice { Sides = 6, Modifier = 1 },
                DurationUnit = DurationUnits.Days, TemperatureChange = new Dice { Sides = 4, Modifier = -1 } },

            // Number on d100 66-85, Duration 1d4+1 Days, Temp. Change 1d10 degrees 
            new TemperatureVariation { ChanceRangeMin = 66, ChanceRangeMax = 85, DurationDice = new Dice { Sides = 4, Modifier = 1 },
                DurationUnit = DurationUnits.Days, TemperatureChange = new Dice { Sides = 10 } },

            // Number on d100 86-95, Duration 1d4 Days, Temp. Change 2d10 degrees 
            new TemperatureVariation { ChanceRangeMin = 86, ChanceRangeMax = 95, DurationDice = new Dice { Sides = 4 },
                DurationUnit = DurationUnits.Days, TemperatureChange = new Dice { Count = 2, Sides = 10 } },

            // Number on d100 96-100, Duration 1d2 Days, Temp. Change 3d10 degrees 
            new TemperatureVariation { ChanceRangeMin = 96, ChanceRangeMax = 100, DurationDice = new Dice { Sides = 2 },
                DurationUnit = DurationUnits.Days, TemperatureChange = new Dice { Count = 3, Sides = 10 } },
        };

        #endregion

        #region Public Properties

        public Dice TemperatureChange { get; set; }

        public int Temperature
        {
            get
            {
                return TemperatureChange.Roll();
            }
        }

        #endregion
    }
}
