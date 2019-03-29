





using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Weather_Calendar.Extensions;
using Weather_Calendar.Weather.Variation;

namespace Weather_Calendar.Weather.Data
{
    #region Public Enums

    [Serializable]
    public enum PrecipitationType { Rain, Sleet, Snow }
    [Serializable]
    public enum PrecipitationLevel { None, Drizzle, Light, Medium, Heavy, Storm }
    [Serializable]
    public enum OvercastLevel { None, Light, Medium, Heavy }

    #endregion

    [Serializable]
    [XmlRoot("Precipitation")]
    public class PrecipitationData : IXmlSerializable
    {
        #region Public Properties

        [XmlAttribute("Level")]
        public PrecipitationLevel Level { get; set; } = PrecipitationLevel.None;
        [XmlAttribute("Duration")]
        public double Duration { get; set; } // in hours
        [XmlAttribute("SnowAccumulation")]
        public double SnowAccumulation { get; set; } // in inches
        [XmlAttribute("CloudCover")]
        public OvercastLevel CloudCover { get; set; } = OvercastLevel.None;

        #endregion

        #region Public Static Methods

        public static List<WeatherData> GeneratePrecipitation(DateTime dateTime, List<WeatherData> weatherData)
        {
            int monthIdx = dateTime.Month - 1;
            //double precipChance = PrecipVariation.PRECIP_CHANCE_MONTH[(monthIdx - 1).Mod(PrecipVariation.PRECIP_CHANCE_MONTH.Length)].Lerp(PrecipVariation.PRECIP_CHANCE_MONTH[monthIdx.Mod(PrecipVariation.PRECIP_CHANCE_MONTH.Length)], dateTime.Day / DateTime.DaysInMonth(dateTime.Year, dateTime.Month)) * 100;
            double precipChance = PrecipVariation.PRECIP_CHANCE_MONTH.LerpOver(monthIdx, dateTime.Day / DateTime.DaysInMonth(dateTime.Year, dateTime.Month)) * 100;

            int chanceRoll = TimedChance.D100.Roll();

            if (chanceRoll <= precipChance)
            {
                int precipRoll = TimedChance.D100.Roll();
                int precipChange = 0;
                int dayTotal = 1;
                int duration = 1;

                foreach (PrecipVariation variation in PrecipVariation.PRECIP_VARIATION_TABLE)
                {
                    if (variation.IsWithinRange(precipRoll))
                    {
                        precipChange = variation.Precipitiation;
                        if (variation.DurationUnit == DurationUnits.Days)
                            dayTotal = variation.Duration;
                        else
                            duration = variation.Duration;

                        break;
                    }
                }

                WeatherData weather = weatherData[0];
                if (weather.Precipitation.Level > 0)
                    precipChange = 1;

                weather.Precipitation.Level = (PrecipitationLevel)Math.Min((int)weather.Precipitation.Level + precipChange, (int)PrecipitationLevel.Heavy);
                weather.Precipitation.CloudCover = OvercastLevel.Heavy;
                if (dayTotal == 1)
                {
                    weather.Precipitation.Duration = duration;
                }
                else
                {
                    weather.Precipitation.Duration = 24;
                }

                if (DescriptionData.CheckPrecipitationType(weather.Temperature) == PrecipitationType.Snow)
                {
                    weather.Precipitation.SnowAccumulation = PrecipVariation.SNOW_ACCUMULATION[(int)weather.Precipitation.Level].Roll((int)weather.Precipitation.Duration);
                }
                else if (DescriptionData.CheckPrecipitationType(weather.Temperature) == PrecipitationType.Sleet)
                {
                    weather.Precipitation.SnowAccumulation = PrecipVariation.SNOW_ACCUMULATION[(int)weather.Precipitation.Level].Roll((int)weather.Precipitation.Duration) * 0.5;
                }

                if(weather.Wind.Level >= WindLevel.Strong && weather.Precipitation.Level >= PrecipitationLevel.Heavy)
                {
                    if(DescriptionData.CheckPrecipitationType(weather.Temperature) == PrecipitationType.Snow && weather.Wind.Level >= WindLevel.Severe)
                    {
                        weather.Precipitation.Level = PrecipitationLevel.Storm;
                    }
                    else if (DescriptionData.CheckPrecipitationType(weather.Temperature) != PrecipitationType.Snow)
                    {
                        weather.Precipitation.Level = PrecipitationLevel.Storm;
                    }
                }
                
                for (int i = 1; i < dayTotal; i++)
                {
                    if (i >= weatherData.Count)
                    {
                        weatherData.Add(new WeatherData());
                    }
                    weatherData[i].Precipitation.Level = (PrecipitationLevel)Math.Min(Math.Max((int)weather.Precipitation.Level + precipChange - i, 0), (int)PrecipitationLevel.Heavy);
                    weatherData[i].Precipitation.CloudCover = OvercastLevel.Heavy;
                    weatherData[i].Precipitation.Duration = 24;
                }


                return weatherData;
            }
            else
            {
                int cloudRoll = TimedChance.D100.Roll();
                WeatherData weather = weatherData[0];

                foreach (PrecipVariation variation in PrecipVariation.OVERCAST_VARIATION_TABLE)
                {
                    if (variation.IsWithinRange(cloudRoll))
                    {
                        weather.Precipitation.CloudCover = (OvercastLevel)variation.Precipitiation;

                        break;
                    }
                }

                double overcastTempChange = TemperatureVariation.OVERCAST_TEMP_CHANGE_MONTH.LerpOver(monthIdx, dateTime.Day / DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
                if (weather.Precipitation.CloudCover > OvercastLevel.None)
                {
                    weather.Temperature += overcastTempChange * ((int)weather.Precipitation.CloudCover/(int)OvercastLevel.Heavy);
                }
                else if (weather.Precipitation.CloudCover == OvercastLevel.None)
                {
                    weather.Temperature -= overcastTempChange;
                }
            }

            return weatherData;
        }

        public static void MeltSnowOverDay(WeatherData weather)
        {
            double meltTemp = 30;

            if (weather.Precipitation.CloudCover == OvercastLevel.None)
            {
                meltTemp = 24;
            }

            if (weather.Temperature > meltTemp)
            {
                double meltRate = weather.Temperature / 1200.0;
                double meltAmount = 0;

                if (weather.Precipitation.CloudCover == OvercastLevel.None)
                {
                    meltRate *= 1.5;
                }

                if (DescriptionData.CheckPrecipitationType(weather.Temperature) == PrecipitationType.Rain)
                {
                    meltAmount = (int)weather.Precipitation.Level * weather.Precipitation.Duration * meltRate * 6;
                }
                else if (DescriptionData.CheckPrecipitationType(weather.Temperature) == PrecipitationType.Sleet)
                {
                    meltAmount = (int)weather.Precipitation.Level * weather.Precipitation.Duration * meltRate * 5;
                }

                meltAmount += meltRate * 24;

                weather.Precipitation.SnowAccumulation = Math.Max(weather.Precipitation.SnowAccumulation - meltAmount, 0);
            }
        }

        #endregion

        #region IXmlSerializable

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            for (int attInd = 0; attInd < reader.AttributeCount; attInd++)
            {
                reader.MoveToAttribute(attInd);
                string temp;

                switch(reader.Name)
                {
                    case "Level":
                    temp = reader.Value;
                    PrecipitationLevel level = PrecipitationLevel.None;
                    if (Enum.TryParse(temp, out level))
                    {
                        Level = level;
                    }
                    break;

                    case "Duration":
                    temp = reader.Value;
                    double duration = 0;
                    if (double.TryParse(temp, out duration))
                    {
                        Duration = duration;
                    }
                    break;

                    case "SnowAccumulation":
                    temp = reader.Value;
                    double snowAccum = 0;
                    if (double.TryParse(temp, out snowAccum))
                    {
                        SnowAccumulation = snowAccum;
                    }
                    break;

                    case "CloudCover":
                    temp = reader.Value;
                    OvercastLevel clouds = OvercastLevel.None;
                    if (Enum.TryParse(temp, out clouds))
                    {
                        CloudCover = clouds;
                    }
                    break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Precipitation");
            writer.WriteAttributeString("Level", Level.ToString());
            writer.WriteAttributeString("Duration", Duration.ToString("0.##"));
            writer.WriteAttributeString("SnowAccumulation", SnowAccumulation.ToString("0.##"));
            writer.WriteAttributeString("CloudCover", CloudCover.ToString());
            writer.WriteEndElement();
        }

        #endregion

    }
}
