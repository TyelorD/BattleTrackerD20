using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Weather_Calendar.Extensions;
using Weather_Calendar.Weather.Data;
using Weather_Calendar.Weather.Variation;

namespace Weather_Calendar.Weather
{
    public class WeatherGenerator
    {
        #region Public Static Methods

        public static WeatherData[] GenerateWeather(DateTime? equivalentDateTime, WeatherData weather = null)
        {
            if (equivalentDateTime.HasValue)
            {
                DateTime dateTime = equivalentDateTime.Value;
                int monthIdx = dateTime.Month - 1;
                if (weather == null)
                {
                    weather = new WeatherData();
                }

                List<WeatherData> weatherDatas = WeatherData.GenerateTemperature(dateTime, weather);

                weatherDatas = FogData.GenerateFog(dateTime, weatherDatas);

                weatherDatas = WindData.GenerateWind(dateTime, weatherDatas);

                weatherDatas = PrecipitationData.GeneratePrecipitation(dateTime, weatherDatas);

                return weatherDatas.ToArray();
            }

            return null;
        }

        #endregion

    }
}
