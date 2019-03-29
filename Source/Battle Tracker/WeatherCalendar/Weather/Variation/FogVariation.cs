using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather_Calendar.Weather.Variation
{
    public class FogVariation : TimedChance
    {
        #region Public Static Fields

        public static readonly FogVariation[] FOG_VARIATION_TABLE =
        {
            // Number on d100 1-5, Duration 3d6 Hours, Fog Level Light
            new FogVariation { ChanceRangeMin = 1, ChanceRangeMax = 5, DurationDice = new Dice { Count = 3, Sides = 6 },
                DurationUnit = DurationUnits.Hours, FogChange = new Dice { Sides = 1 } },

            // Number on d100 6-25, Duration 2d6 Hours, Fog Level Light or Medium
            new FogVariation { ChanceRangeMin = 6, ChanceRangeMax = 25, DurationDice = new Dice { Count = 2, Sides = 6 },
                DurationUnit = DurationUnits.Hours, FogChange = new Dice { Sides = 2 } },

            // Number on d100 26-50, Duration 1d8 Hours, Fog Level Light or Medium
            new FogVariation { ChanceRangeMin = 26, ChanceRangeMax = 50, DurationDice = new Dice { Sides = 8 },
                DurationUnit = DurationUnits.Hours, FogChange = new Dice { Sides = 2 } },

            // Number on d100 51-70, Duration 1d6 Hours, Fog Level Medium
            new FogVariation { ChanceRangeMin = 51, ChanceRangeMax = 70, DurationDice = new Dice { Sides = 6 },
                DurationUnit = DurationUnits.Hours, FogChange = new Dice { Sides = 1, Modifier = 1 } },

            // Number on d100 71-95, Duration 1d4 Hours, Fog Level Heavy
            new FogVariation { ChanceRangeMin = 71, ChanceRangeMax = 95, DurationDice = new Dice { Sides = 4 },
                DurationUnit = DurationUnits.Hours, FogChange = new Dice { Sides = 1, Modifier = 2 } },

            // Number on d100 96-100, Duration 3d6 Hours, Fog Level Medium or Heavy
            new FogVariation { ChanceRangeMin = 96, ChanceRangeMax = 100, DurationDice = new Dice { Count = 3, Sides = 6 },
                DurationUnit = DurationUnits.Hours, FogChange = new Dice { Sides = 2, Modifier = 1 } },
        };

        #endregion

        #region Public Properties

        public Dice FogChange { get; set; }

        public int Fog
        {
            get
            {
                return FogChange.Roll();
            }
        }

        #endregion

    }
}
