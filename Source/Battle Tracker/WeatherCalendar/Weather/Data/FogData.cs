





using System;
using System.Collections.Generic;
using Weather_Calendar.Weather.Variation;
using Weather_Calendar.Extensions;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace Weather_Calendar.Weather.Data
{
    #region Public Enums

    [Serializable]
    public enum FogLevel { None, Light, Medium, Heavy }

    #endregion

    [Serializable]
    public class FogData : IXmlSerializable
    {
        #region Public Properties

        [XmlAttribute("Level")]
        public FogLevel Level { get; set; } = FogLevel.None;
        [XmlAttribute("Duration")]
        public double Duration { get; set; } // in hours

        #endregion

        #region Public Static Methods

        public static List<WeatherData> GenerateFog(DateTime dateTime, List<WeatherData> weatherData)
        {
            WeatherData weather = weatherData[0];
            int monthIdx = dateTime.Month - 1;
            double fogChance = PrecipVariation.PRECIP_CHANCE_MONTH[(monthIdx - 1).Mod(PrecipVariation.PRECIP_CHANCE_MONTH.Length)].Lerp(PrecipVariation.PRECIP_CHANCE_MONTH[monthIdx.Mod(PrecipVariation.PRECIP_CHANCE_MONTH.Length)], dateTime.Day / DateTime.DaysInMonth(dateTime.Year, dateTime.Month)) * 50;

            int chanceRoll = TimedChance.D100.Roll();

            if (chanceRoll <= fogChance)
            {
                int fogRoll = TimedChance.D100.Roll();
                int fogChange = 0;
                int dayTotal = 1;
                int duration = 1;

                foreach (FogVariation variation in FogVariation.FOG_VARIATION_TABLE)
                {
                    if (variation.IsWithinRange(fogRoll))
                    {
                        fogChange = variation.Fog;
                        if (variation.DurationUnit == DurationUnits.Days)
                            dayTotal = variation.Duration;
                        else
                            duration = variation.Duration;

                        break;
                    }
                }

                weather.MorningFog.Level = (FogLevel)Math.Min((int)weather.MorningFog.Level + fogChange, (int)FogLevel.Heavy);
                if (dayTotal == 1)
                {
                    weather.MorningFog.Duration = duration;
                }
                else
                {
                    weather.MorningFog.Duration = 24;
                }

                //WeatherData[] weatherData = new WeatherData[dayTotal];
                weatherData[0] = weather;

                for (int i = 1; i < dayTotal; i++)
                {
                    if (i >= weatherData.Count)
                    {
                        weatherData.Add(new WeatherData());
                    }
                    weatherData[i].MorningFog.Level = (FogLevel)Math.Min((int)weather.MorningFog.Level + fogChange, (int)FogLevel.Heavy);
                    weatherData[i].MorningFog.Duration = 24;
                }

                //MessageBox.Show(tempChange + " for " + duration + " days");
            }

            chanceRoll = TimedChance.D100.Roll();

            if (chanceRoll <= fogChance)
            {
                int fogRoll = TimedChance.D100.Roll();
                int fogChange = 0;
                int dayTotal = 1;
                int duration = 1;

                foreach (FogVariation variation in FogVariation.FOG_VARIATION_TABLE)
                {
                    if (variation.IsWithinRange(fogRoll))
                    {
                        fogChange = variation.Fog;
                        if (variation.DurationUnit == DurationUnits.Days)
                            dayTotal = variation.Duration;
                        else
                            duration = variation.Duration;

                        break;
                    }
                }

                weather.EveningFog.Level = (FogLevel)Math.Min((int)weather.EveningFog.Level + fogChange, (int)FogLevel.Heavy);
                if (dayTotal == 1)
                {
                    weather.EveningFog.Duration = duration;
                }
                else
                {
                    weather.EveningFog.Duration = 24;
                }

                for (int i = 1; i < dayTotal; i++)
                {
                    if (i >= weatherData.Count)
                    {
                        weatherData.Add(new WeatherData());
                    }
                    weatherData[i].EveningFog.Level = (FogLevel)Math.Min((int)weather.EveningFog.Level + fogChange, (int)FogLevel.Heavy);
                    weatherData[i].EveningFog.Duration = 24;
                }

                //MessageBox.Show(tempChange + " for " + duration + " days");
            }

            return weatherData;
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

                switch (reader.Name)
                {
                    case "Level":
                    temp = reader.Value;
                    FogLevel level = FogLevel.None;
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
                }

                //Console.WriteLine("\t\t\t<" + reader.Name + "=\"" + reader.Value + "\"/>");
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Fog");
            writer.WriteAttributeString("Level", Level.ToString());
            writer.WriteAttributeString("Duration", Duration.ToString("0.##"));
            writer.WriteEndElement();
        }

        public void WriteXml(XmlWriter writer, bool isAM)
        {
            if (isAM)
            {
                writer.WriteStartElement("FogMorning");
            }
            else
            {
                writer.WriteStartElement("FogEvening");
            }
            writer.WriteAttributeString("Level", Level.ToString());
            writer.WriteAttributeString("Duration", Duration.ToString("0.##"));
            writer.WriteEndElement();
        }

        #endregion

    }
}
