using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Weather_Calendar.Extensions;
using Weather_Calendar.Weather.Variation;

namespace Weather_Calendar.Weather.Data
{
    #region Public Enums

    [Serializable]
    public enum WindLevel { Calm, Light, Moderate, Strong, Severe, Windstorm }
    [Serializable]
    public enum WindDirection { N, NE, E, SE, S, SW, W, NW }

    #endregion

    [Serializable]
    public class WindData : IXmlSerializable
    {
        #region Private Static Fields

        private static Dice D8M1 = new Dice { Sides = 8, Modifier = -1 };
        private static Dice P1M1 = new Dice { Sides = 3, Modifier = -2 };

        #endregion

        #region Public Properties

        [XmlAttribute("Level")]
        public WindLevel Level { get; set; } = WindLevel.Calm;
        [XmlAttribute("Direction")]
        public WindDirection Direction { get; set; } = WindDirection.NE;
        [XmlAttribute("Speed")]
        public double Speed { get; set; } = 0; // in MPH

        #endregion

        #region Public Static Methods

        public static List<WeatherData> GenerateWind(DateTime dateTime, List<WeatherData> weatherData)
        {
            int windRoll = TimedChance.D100.Roll();
            int windChange = 0;
            int dayTotal = 1;
            int duration = 1;

            foreach (WindVariation variation in WindVariation.WIND_VARIATION_TABLE)
            {
                if (variation.IsWithinRange(windRoll))
                {
                    windChange = variation.Wind;
                    if (variation.DurationUnit == DurationUnits.Days)
                        dayTotal = variation.Duration;
                    else
                        duration = variation.Duration;

                    break;
                }
            }

            WeatherData weather = weatherData[0];
            if (weather.Wind.Level > 0)
                windChange = 1;

            if (weather.MorningFog.Level == FogLevel.None && weather.EveningFog.Level == FogLevel.None)
            {
                weather.Wind.Level = (WindLevel)Math.Min((int)weather.Wind.Level + windChange, (int)WindLevel.Windstorm);
                weather.Wind.Speed = WindVariation.WIND_SPEED_TABLE[(int)weather.Wind.Level].Roll();
                weather.Wind.Direction = (WindDirection)D8M1.Roll();
                weather.Temperature = CalculateWindChill(weather.Wind.Speed, weather.Temperature);
            }

            for (int i = 1; i < dayTotal; i++)
            {
                if (i >= weatherData.Count)
                {
                    weatherData.Add(new WeatherData());
                }
                if (weatherData[i].MorningFog.Level == FogLevel.None && weatherData[i].EveningFog.Level == FogLevel.None)
                {
                    weatherData[i].Wind.Level = (WindLevel)Math.Min(Math.Max((int)weatherData[i].Wind.Level + windChange, 0), (int)WindLevel.Windstorm);
                    weatherData[i].Wind.Speed = WindVariation.WIND_SPEED_TABLE[(int)weatherData[i].Wind.Level].Roll();
                    weatherData[i].Wind.Direction = (WindDirection)((int)weatherData[i].Wind.Direction + P1M1.Roll()).Mod((int)WindDirection.NW + 1);

                    weather.Temperature = CalculateWindChill(weather.Wind.Speed, weather.Temperature);
                }
            }

            return weatherData;
        }

        public static double CalculateWindChill(double velocity, double temperature)
        {
            double vPow = Math.Pow(velocity, 0.16);

            return 35.75 + 0.6215 * temperature - 35.75 * vPow + 0.4275 * temperature * vPow;
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
                    WindLevel level = WindLevel.Calm;
                    if (Enum.TryParse(temp, out level))
                    {
                        Level = level;
                    }
                    break;

                    case "Direction":
                    temp = reader.Value;
                    WindDirection direction = WindDirection.NE;
                    if (Enum.TryParse(temp, out direction))
                    {
                        Direction = direction;
                    }
                    break;

                    case "Speed":
                    temp = reader.Value;
                    double speed = 0;
                    if (double.TryParse(temp, out speed))
                    {
                        Speed = speed;
                    }
                    break;
                }

                //Console.WriteLine("\t\t\t<" + reader.Name + "=\"" + reader.Value + "\"/>");
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Wind");
            writer.WriteAttributeString("Level", Level.ToString());
            writer.WriteAttributeString("Direction", Direction.ToString());
            writer.WriteAttributeString("Speed", Speed.ToString("0.##"));
            writer.WriteEndElement();
        }

        #endregion
    }
}
