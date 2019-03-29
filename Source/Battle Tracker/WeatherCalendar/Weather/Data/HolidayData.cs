using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather_Calendar.Weather.Data
{
    public class HolidayData
    {
        #region Public Static Fields

        public static Dictionary<DateTime, string> HOLIDAYS = new Dictionary<DateTime, string>();

        #endregion

        #region Public static Properties

        public static bool IS_INITIALIZED { get; private set; } = false;

        #endregion

        #region Public Static Methods

        public static void Initialize()
        {
            if (!IS_INITIALIZED)
            {
                HOLIDAYS.Add(new DateTime(4710, 1, 6), "Vault Day (Abadar)");

                HOLIDAYS.Add(new DateTime(4710, 2, 2), "Merrymead (Cayden Cailean)");

                HOLIDAYS.Add(new DateTime(4710, 3, 5), "Day of Bones (Pharasma)");
                HOLIDAYS.Add(new DateTime(4710, 3, 20), "Firstbloom (Gozreh)" + Extensions.ExtensionMethods.NL + "1st Day of Planting Week (Erastil)");

                HOLIDAYS.Add(new DateTime(4710, 4, 1), "Currentseve (Gozreh)");
                HOLIDAYS.Add(new DateTime(4710, 4, 10), "Taxfest (Abadar/Brigh)");

                HOLIDAYS.Add(new DateTime(4710, 5, 2), "Ascendance Night (Norgorber)");

                HOLIDAYS.Add(new DateTime(4710, 6, 1), "1st Day of Summer (Sarenrae)");
                HOLIDAYS.Add(new DateTime(4710, 6, 10), "Burning Blades (Sarenrae)");
                HOLIDAYS.Add(new DateTime(4710, 6, 21), "Sunwrought Festival (Sarenrae/Brigh)" + Extensions.ExtensionMethods.NL + "Ritual of Stardust (Desna)");

                HOLIDAYS.Add(new DateTime(4710, 7, 3), "Archer's Day (Erastil)");

                HOLIDAYS.Add(new DateTime(4710, 8, 1), "Inheritor's Ascendance (Iomedae)");
                HOLIDAYS.Add(new DateTime(4710, 8, 31), "Taxfest (Sarenrae)");

                HOLIDAYS.Add(new DateTime(4710, 9, 9), "First Brewing (Cayden Cailean)");
                HOLIDAYS.Add(new DateTime(4710, 9, 16), "Market's Door (Abadar)");
                HOLIDAYS.Add(new DateTime(4710, 9, 19), "Day of the Inheritor (Iomedae)");
                HOLIDAYS.Add(new DateTime(4710, 9, 23), "Swallowtail Festival (Desna)");

                HOLIDAYS.Add(new DateTime(4710, 10, 6), "Ascendance Day (Iomedae)");
                HOLIDAYS.Add(new DateTime(4710, 10, 30), "Allbirth (Lamashtu)");

                HOLIDAYS.Add(new DateTime(4710, 11, 8), "Abjurant Day (Nethys)");
                HOLIDAYS.Add(new DateTime(4710, 11, 18), "Evoking Day (Nethys)");
                HOLIDAYS.Add(new DateTime(4710, 11, 28), "Transmutatum (Nethys)");

                HOLIDAYS.Add(new DateTime(4710, 12, 1), "The Shadow Chaining (Zon Kuthon)");
                HOLIDAYS.Add(new DateTime(4710, 12, 11), "Ascendance Day (Cayden Cailean)");
                HOLIDAYS.Add(new DateTime(4710, 12, 21), "Candlemark (Sarenrae)" + Extensions.ExtensionMethods.NL + "Ritual of Stardust (Desna)" + Extensions.ExtensionMethods.NL + "Crystalhue (Shelyn)");
                HOLIDAYS.Add(new DateTime(4710, 12, 31), "Night of the Pale (Zon Kuthon)" + Extensions.ExtensionMethods.NL + "The Final Day (Groetus)");

                IS_INITIALIZED = true;
            }
        }

        public static bool IsHoliday(DateTime dateTime)
        {
            DateTime tempDate = new DateTime(4710, dateTime.Month, dateTime.Day);

            return HOLIDAYS.ContainsKey(tempDate);
        }

        public static string GetHolidayText(DateTime dateTime)
        {
            DateTime tempDate = new DateTime(4710, dateTime.Month, dateTime.Day);

            if (IsHoliday(tempDate))
            {
                return HOLIDAYS[tempDate];
            }

            return null;
        }

        #endregion

    }
}
