



using System;
using System.Collections.Generic;
using Weather_Calendar.Weather.Variation;
using Weather_Calendar.Extensions;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace Weather_Calendar.Weather.Data
{
    [Serializable]
    public class WeatherData : IXmlSerializable
    {
        #region Public Properties

        [XmlAttribute("Temperature")]
        public double Temperature { get; set; }
        [XmlAttribute("FogMorning")]
        public FogData MorningFog { get; set; } = new FogData();
        [XmlAttribute("FogEvening")]
        public FogData EveningFog { get; set; } = new FogData();
        [XmlAttribute("Precipitation")]
        public PrecipitationData Precipitation { get; set; } = new PrecipitationData();
        [XmlAttribute("Wind")]
        public WindData Wind { get; set; } = new WindData();

        #endregion

        #region Operators

        public static WeatherData operator +(WeatherData left, WeatherData right)
        {
            WeatherData weatherData = new WeatherData();
            weatherData.Temperature = left.Temperature + right.Temperature;
            weatherData.MorningFog.Level = (FogLevel)Math.Min((int)left.MorningFog.Level + (int)right.MorningFog.Level, (int)FogLevel.Heavy);
            weatherData.EveningFog.Level = (FogLevel)Math.Min((int)left.EveningFog.Level + (int)right.EveningFog.Level, (int)FogLevel.Heavy);
            if (right.Precipitation.Level > 0)
                weatherData.Precipitation.Level = (PrecipitationLevel)Math.Min((int)left.Precipitation.Level + 1, (int)PrecipitationLevel.Storm);
            else
                weatherData.Precipitation.Level = left.Precipitation.Level;
            weatherData.MorningFog.Duration = Math.Max(left.MorningFog.Duration, right.MorningFog.Duration);
            weatherData.EveningFog.Duration = Math.Max(left.EveningFog.Duration, right.EveningFog.Duration);
            weatherData.Precipitation.Duration = Math.Max(left.Precipitation.Duration, right.Precipitation.Duration);
            weatherData.Precipitation.SnowAccumulation = left.Precipitation.SnowAccumulation + right.Precipitation.SnowAccumulation;

            return weatherData;
        }

        #endregion

        #region Public Helper Methods

        public static List<WeatherData> GenerateTemperature(DateTime dateTime, WeatherData weather)
        {
            int monthIdx = dateTime.Month - 1;
            weather.Temperature += TemperatureVariation.BASE_TEMP_MONTH[(monthIdx - 1).Mod(TemperatureVariation.BASE_TEMP_MONTH.Length)].Lerp(TemperatureVariation.BASE_TEMP_MONTH[monthIdx.Mod(TemperatureVariation.BASE_TEMP_MONTH.Length)], dateTime.Day / DateTime.DaysInMonth(dateTime.Year, dateTime.Month));

            int tempRoll = TimedChance.D100.Roll();
            int tempChange = 0;
            int duration = 1;

            foreach (TemperatureVariation variation in TemperatureVariation.TEMP_VARIATION_TABLE)
            {
                if (variation.IsWithinRange(tempRoll))
                {
                    tempChange = variation.Temperature;
                    duration = variation.Duration;

                    break;
                }
            }

            weather.Temperature += tempChange;

            List<WeatherData> weatherData = new List<WeatherData>(duration);
            weatherData.Add(weather);

            for (int i = 1; i < duration; i++)
            {
                weatherData.Add(new WeatherData { Temperature = tempChange });
            }

            //MessageBox.Show(tempChange + " for " + duration + " days");

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
            
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Weather");
            writer.WriteAttributeString("Temperature", Temperature.ToString("0.##"));
            Precipitation.WriteXml(writer);
            MorningFog.WriteXml(writer, true);
            EveningFog.WriteXml(writer, false);
            Wind.WriteXml(writer);
            writer.WriteEndElement();
        }

        #endregion

    }
}
