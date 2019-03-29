using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather_Calendar.Weather.Data
{
    public class DescriptionData
    {
        #region Public Static Fields

        public static string[] TEMP_DESCRIPTIONS = { "Fort Save (DC 15 + 1 per previous save) or take 1d4 nonlethal plus 1d6 lethal cold damage per minute (no save).", "Fort Save (DC 15 + 1 per previous save) every 10 minutes (every hour if wearing protection) or take 1d6 nonlethal & fatigue.",
                                                        "Fort Save (DC 15 + 1 per previous save) every hour (if not wearing protection) or take 1d6 nonlethal & fatigue.", "Normal", "Fort Save (DC 15 + 1 per previous save) every hour (-4 for heavy armor/clothing) or take 1d4 nonlethal & fatigue.",
                                                        "Fort Save (DC 15 + 1 per previous save) every 10 minutes (-4 for heavy armor/clothing) or take 1d4 nonlethal & fatigue.", "Fort Save (DC 15 + 1 per previous save) per 5 minutes (-4 for heavy armor/clothing) or take 1d4 nonlethal plus 1d6 lethal fire damage per minute (no save)." };
        public static string[] WIND_DESCRIPTIONS = { "Normal", "Normal", "Normal", "-2 Ranged Attacks, Check Size: Tiny.", "-2 Fly & Perception (Sound Based), -4 Ranged Attacks, Check Size: Small.", "-8 Fly & Perception (Sound Based), Ranged Attacks Impossible, Check Size: Medium." };
        public static string[] FOG_DESCRIPTIONS = { "Normal", "Vision 3/4 normal, -2 Perception & Ranged Attacks.", "Vision 1/2 normal, -4 Perception & Ranged Attacks.", "Vision 5ft, per Fog Cloud Spell." };
        public static string[] RAIN_DESCRIPTIONS = { "Normal", "Normal", "-2 Perception & Ranged Attacks.", "Vision 3/4 normal, -4 Perception & Ranged Attacks.", "Vision 1/2 normal, -6 Perception & Ranged Attacks.", "Vision 1/4 normal, -6 Perception & Ranged Attacks." };
        public static string HEAVY_OVERCAST_DESCRIPTION = "Concealment for creatures flying at high altitudes.";
        public static string[] SNOW_ACCUMULATION_DESCRIPTIONS = { "Each square of movement takes 1 extra square of movement.", "Each square of movement takes 2 extra squares of movement." };
        public static string[] DRIZZLE_DESCRIPTIONS = { "Snow Flurries", "Freezing Drizzle", "Drizzle" };
        public static string[] STORM_NAMES = { "Thunderstorm", "Blizzard" };

        #endregion

        #region Public Static Methods

        public static PrecipitationType CheckPrecipitationType(double tempAverage)
        {
            if (tempAverage > 32)
            {
                if (tempAverage < 38)
                {
                    return PrecipitationType.Sleet;
                }

                return PrecipitationType.Rain;
            }

            return PrecipitationType.Snow;
        }

        public static void AddPrecipitationLevel(WeatherData weather, StringBuilder builder)
        {
            if (weather.Precipitation.Level == PrecipitationLevel.Drizzle)
            {
                if (CheckPrecipitationType(weather.Temperature) == PrecipitationType.Snow)
                {
                    builder.Append(DRIZZLE_DESCRIPTIONS[0]);
                }
                else if (CheckPrecipitationType(weather.Temperature) == PrecipitationType.Sleet)
                {
                    builder.Append(DRIZZLE_DESCRIPTIONS[1]);
                }
                else
                {
                    builder.Append(DRIZZLE_DESCRIPTIONS[2]);
                }
            }
            else if (weather.Precipitation.Level == PrecipitationLevel.Storm)
            {
                if (CheckPrecipitationType(weather.Temperature) == PrecipitationType.Rain)
                {
                    builder.Append(STORM_NAMES[0]);
                }
                else
                {
                    builder.Append(STORM_NAMES[1]);
                }
            }
            else
            {
                builder.Append(weather.Precipitation.Level.ToString());
                builder.Append(Extensions.ExtensionMethods.SP);
                builder.Append(CheckPrecipitationType(weather.Temperature));
            }
        }

        public static void AddPrecipitationDesciption(WeatherData weather, StringBuilder builder, StringBuilder ttBuilder)
        {
            if (weather.Precipitation.SnowAccumulation > 0)
            {
                builder.AppendLine();
                builder.Append("Snow Accumulation: ");
                builder.Append(weather.Precipitation.SnowAccumulation.ToString("0.# in."));

                if (weather.Precipitation.SnowAccumulation >= 6)
                {
                    ttBuilder.AppendLine();
                    ttBuilder.Append("Ground Snow: ");
                    if (weather.Precipitation.SnowAccumulation >= 24)
                    {
                        ttBuilder.Append(SNOW_ACCUMULATION_DESCRIPTIONS[1]);
                    }
                    else
                    {
                        ttBuilder.Append(SNOW_ACCUMULATION_DESCRIPTIONS[0]);
                    }
                }
            }

            if (weather.Precipitation.Level != PrecipitationLevel.None)
            {
                builder.AppendLine();
                builder.Append("Precipitation: ");
                AddPrecipitationLevel(weather, builder);
                builder.Append(" for ");
                builder.Append(weather.Precipitation.Duration);
                builder.Append(" Hours.");

                if ((int)weather.Precipitation.Level > (int)PrecipitationLevel.Drizzle)
                {
                    ttBuilder.AppendLine();
                    ttBuilder.Append(CheckPrecipitationType(weather.Temperature));
                    ttBuilder.Append(": ");
                    ttBuilder.Append(RAIN_DESCRIPTIONS[(int)weather.Precipitation.Level]);
                }
            }
        }

        #endregion

    }
}
