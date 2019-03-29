using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather_Calendar.Weather.Data;
using System.Windows.Controls;
using System.Windows;
using Weather_Calendar.Weather;

namespace Weather_Calendar.Extensions
{
    static class ExtensionMethods
    {
        #region Public Static Fields

        public static readonly string SP = " ";
        public static readonly string TB = "\t";
        public static readonly string NL = Environment.NewLine;
        public static readonly string LAST_BATTLE_FILENAME = @".\Battles\LastBattle.xml";
        public static readonly string LAST_CALENDAR_FILENAME = @".\Calendar\LastCalendar.xml";
        public static readonly string FILE_DIALOG_FILTER = "Extensible Markup Language (.xml)|*.xml";
        public static readonly string DEFAULT_DAILY_NOTES_TEXT = "Enter daily notes here...";
        public static readonly string DEFAULT_GENERAL_NOTES_TEXT = "Enter general notes here...";
        public static readonly string DEFAULT_WEATHER_TEXT = "Click the button to generate today's weather...";
        public static readonly string CURRENT_DAY_PREFIX = "Current Day: ";
        public static readonly string[] MONTH_NAMES = { "Abadius", "Calistril", "Pharast", "Gozran", "Desnus", "Sarenith", "Erastus", "Arodus", "Rova", "Lamashan", "Neth", "Kuthona" };
        public static readonly string[] MONTH_PRONUN = { "(ah-BAY-dee-us)", "(KAHL-izz-trihl)", "(fah-RAHST)", "(GOHZ-ran)", "(DEZ-nuhs)", "(sa-REHN-ihth)", "(eh-RAS-tuhs)", "(AIR-oh-duhs)", "(ROH-va)", "(lah-MAHSH-ahn)", "(NEHTH)", "(koo-THOH-nah)" };
        public static readonly string[] DAY_NAMES = { "Sunday", "Moonday", "Toilday", "Wealday", "Oathday", "Fireday", "Starday" };

        #endregion

        #region GUI Helpers

        public static string GetDateTimeTitle(this DateTime dateTime, bool isHeader = true)
        {
            int monthIdx = dateTime.Month - 1;
            int dayIdx = (int)dateTime.DayOfWeek;
            int day = dateTime.Day;
            string daySuffix = "th";
            switch (day % 10)
            {
                case 1:
                daySuffix = (day != 11) ? "st" : "th";
                break;

                case 2:
                daySuffix = (day != 12) ? "nd" : "th";
                break;

                case 3:
                daySuffix = (day != 13) ? "rd" : "th";
                break;
            }


            StringBuilder builder = new StringBuilder(DAY_NAMES[dayIdx]);
            builder.Append(",");
            builder.Append(SP);
            builder.Append(MONTH_NAMES[monthIdx]);
            builder.Append(SP);
            if (isHeader)
            {
                builder.Append(MONTH_PRONUN[monthIdx]);
                builder.Append(SP);
            }
            builder.Append(day);
            builder.Append(daySuffix);
            builder.Append(",");
            builder.Append(SP);
            builder.Append(dateTime.Year);
            if (isHeader)
            {
                builder.Append(SP);
                builder.Append("-");
            }

            return builder.ToString();
        }

        public static void SetSelectedDayWeatherNotes(this DateTime dateTime, IDictionary<DateTime, DayData> dayDataDictionary, TextBox selectedDayNotes, Label selectedDayWeather)
        {
            if (dayDataDictionary != null && selectedDayNotes != null && selectedDayWeather != null && dayDataDictionary.ContainsKey(dateTime))
            {
                DayData dayData = dayDataDictionary[dateTime];

                selectedDayNotes.Text = dayData.Notes;
                if (dayData.WeatherGenerated && dayData.Weather != null)
                {
                    WeatherData weather = dayData.Weather;

                    StringBuilder ttBuilder = new StringBuilder("Weather Penalities - ");
                    StringBuilder builder = new StringBuilder("Today's Temperature is: ");

                    double range = 0.1; //(new WeatherGenerator.Dice { Sides = 5 }.Roll() + 7) * 0.01;
                    double tMin = weather.Temperature - range * weather.Temperature, tMax = weather.Temperature + range * weather.Temperature;
                    double temperature = weather.Temperature;
                    if (temperature > 90 || temperature < 40)
                    {
                        ttBuilder.AppendLine();
                        ttBuilder.Append("Temp.: ");
                        if (temperature > 140)
                        {
                            ttBuilder.Append(DescriptionData.TEMP_DESCRIPTIONS[6]);
                        }
                        else if (temperature > 110)
                        {
                            ttBuilder.Append(DescriptionData.TEMP_DESCRIPTIONS[5]);
                        }
                        else if (temperature > 90)
                        {
                            ttBuilder.Append(DescriptionData.TEMP_DESCRIPTIONS[4]);
                        }
                        else if (temperature < -20)
                        {
                            ttBuilder.Append(DescriptionData.TEMP_DESCRIPTIONS[0]);
                        }
                        else if (temperature < 0)
                        {
                            ttBuilder.Append(DescriptionData.TEMP_DESCRIPTIONS[1]);
                        }
                        else if (temperature < 40)
                        {
                            ttBuilder.Append(DescriptionData.TEMP_DESCRIPTIONS[2]);
                        }
                    }

                    builder.Append(tMin.ToString("0.#°"));
                    builder.Append(" to ");
                    builder.Append(tMax.ToString("0.#° F"));
                    builder.AppendLine();
                    StringBuilder subBuilder = new StringBuilder("Wind: ");
                    subBuilder.Append(weather.Wind.Level.ToString());
                    if (weather.Wind.Level != WindLevel.Calm)
                    {

                        subBuilder.Append(" (");
                        subBuilder.Append(weather.Wind.Speed.ToString("0"));
                        subBuilder.Append(" mph ");
                        subBuilder.Append(weather.Wind.Direction.ToString());
                        subBuilder.Append(")");

                        if ((int)weather.Wind.Level > (int)WindLevel.Moderate)
                        {
                            ttBuilder.AppendLine();
                            ttBuilder.Append("Wind: ");
                            ttBuilder.Append(DescriptionData.WIND_DESCRIPTIONS[(int)weather.Wind.Level]);
                        }
                    }
                    if (subBuilder.Length < 22)
                    {
                        subBuilder.Append(TB);
                    }
                    if (subBuilder.Length < 17)
                    {
                        subBuilder.Append(TB);
                    }
                    subBuilder.Append(TB);
                    subBuilder.Append(TB);
                    subBuilder.Append("Overcast: ");
                    subBuilder.Append(weather.Precipitation.CloudCover.ToString());
                    if (weather.Precipitation.CloudCover == OvercastLevel.Heavy)
                    {
                        ttBuilder.AppendLine();
                        ttBuilder.Append("Overcast: ");
                        ttBuilder.Append(DescriptionData.HEAVY_OVERCAST_DESCRIPTION);
                    }

                    builder.Append(subBuilder.ToString());


                    DescriptionData.AddPrecipitationDesciption(weather, builder, ttBuilder);

                    if (weather.MorningFog.Level != FogLevel.None)
                    {
                        builder.AppendLine();
                        builder.Append("Morning: ");
                        builder.Append(weather.MorningFog.Level.ToString());
                        builder.Append(" Fog for ");
                        builder.Append(weather.MorningFog.Duration);
                        builder.Append(" Hours.");
                        ttBuilder.AppendLine();
                        ttBuilder.Append("Morning Fog: ");
                        ttBuilder.Append(DescriptionData.FOG_DESCRIPTIONS[(int)weather.MorningFog.Level]);
                    }

                    if (weather.EveningFog.Level != FogLevel.None)
                    {
                        builder.AppendLine();
                        builder.Append("Evening: ");
                        builder.Append(weather.EveningFog.Level.ToString());
                        builder.Append(" Fog for ");
                        builder.Append(weather.EveningFog.Duration);
                        builder.Append(" Hours.");
                        ttBuilder.AppendLine();
                        ttBuilder.Append("Evening Fog: ");
                        ttBuilder.Append(DescriptionData.FOG_DESCRIPTIONS[(int)weather.EveningFog.Level]);
                    }

                    string holidayText = HolidayData.GetHolidayText(dateTime);
                    if (holidayText != null)
                    {
                        builder.AppendLine();
                        builder.Append("Holiday(s) Today - ");
                        builder.AppendLine();
                        builder.Append(holidayText);
                    }

                    selectedDayWeather.Content = builder.ToString();

                    if (ttBuilder.ToString() != "Weather Penalities - ")
                    {
                        ToolTip temp = new ToolTip();
                        temp.Content = ttBuilder.ToString();
                        selectedDayWeather.ToolTip = temp;
                    }
                    else
                    {
                        selectedDayWeather.ToolTip = null;
                    }
                }
                else
                {
                    string holidayText = HolidayData.GetHolidayText(dateTime);
                    if (holidayText != null)
                    {
                        selectedDayWeather.Content = DEFAULT_WEATHER_TEXT + NL + "Holiday(s) Today - " + NL + holidayText;
                    }
                    else
                    {
                        selectedDayWeather.Content = DEFAULT_WEATHER_TEXT;
                    }
                    selectedDayWeather.ToolTip = null;
                }
            }
            else
            {
                selectedDayNotes.Text = DEFAULT_DAILY_NOTES_TEXT;
                string holidayText = HolidayData.GetHolidayText(dateTime);
                if (holidayText != null)
                {
                    selectedDayWeather.Content = DEFAULT_WEATHER_TEXT + NL + "Holiday(s) Today - " + NL + holidayText;
                }
                else
                {
                    selectedDayWeather.Content = DEFAULT_WEATHER_TEXT;
                }
                selectedDayWeather.ToolTip = null;
            }
        }

        public static void SetSelectedDate(this DateTime? equivalentDateTime, WeatherNotesWindow window)
        {
            if (equivalentDateTime.HasValue)
            {
                DateTime dateTime = equivalentDateTime.Value;

                window.LblSelectedDayTitle.Content = dateTime.GetDateTimeTitle();

                dateTime.SetSelectedDayWeatherNotes(window.CalendarData, window.TbxSelectedDayNotes, window.LblSelectedDayWeather);
            }
        }

        public static void SetDailyNoteboxText(this DateTime? equivalentDateTime, WeatherNotesWindow window, TextChangedEventArgs e)
        {
            if (equivalentDateTime.HasValue && window.TbxSelectedDayNotes.Text != DEFAULT_DAILY_NOTES_TEXT)
            {
                DateTime dateTime = equivalentDateTime.Value;

                if (window.CalendarData.ContainsKey(dateTime))
                {
                    window.CalendarData[dateTime].Notes = window.TbxSelectedDayNotes.Text;
                }
                else
                {
                    window.CalendarData.Add(dateTime, new DayData(dateTime, window.TbxSelectedDayNotes.Text));
                }

                e.Handled = true;
            }
        }

        public static void SetCurrentDateLabel(this DateTime currentDate, WeatherNotesWindow window)
        {
            window.LblCurrentDayTitle.Content = CURRENT_DAY_PREFIX + currentDate.GetDateTimeTitle(false);
        }

        #endregion

        #region Weather Helpers

        public static void GenerateWeatherFromDateTime(this DateTime dateTime, WeatherNotesWindow window, RoutedEventArgs e)
        {
            DayData day;
            WeatherData[] weatherDatas = null;

            if (window.CalendarData.ContainsKey(dateTime))
            {
                day = window.CalendarData[dateTime];

                if (!day.WeatherGenerated)
                {
                    weatherDatas = WeatherGenerator.GenerateWeather(dateTime, day.Weather);
                    day.Weather = weatherDatas[0];
                }
            }
            else
            {
                weatherDatas = WeatherGenerator.GenerateWeather(dateTime);

                day = new DayData(dateTime, window.TbxSelectedDayNotes.Text, weatherDatas[0]);

                window.CalendarData.Add(dateTime, day);
            }

            DateTime prevDate = dateTime.AddDays(-1);

            if (!day.WeatherGenerated && window.CalendarData.ContainsKey(prevDate))
            {
                DayData prevDay = window.CalendarData[prevDate];

                if (prevDay.WeatherGenerated)
                {
                    day.Weather.Temperature = (day.Weather.Temperature + prevDay.Weather.Temperature) / 2.0;
                    day.Weather.Precipitation.SnowAccumulation = day.Weather.Precipitation.SnowAccumulation + prevDay.Weather.Precipitation.SnowAccumulation;
                    PrecipitationData.MeltSnowOverDay(day.Weather);
                }
            }

            day.WeatherGenerated = true;

            window.CalendarData.SetDictionaryWeatherDatas(dateTime, weatherDatas);

            //dateTime.SetSelectedDayWeatherNotes(window.CalendarData, window.TbxSelectedDayNotes, window.LblSelectedDayWeather);

            e.Handled = true;
        }

        public static void ClearWeatherFromDateTime(this DateTime dateTime, WeatherNotesWindow window, RoutedEventArgs e)
        {
            DayData day;
            WeatherData[] weatherDatas = null;

            if (window.CalendarData.ContainsKey(dateTime))
            {
                day = window.CalendarData[dateTime];

                if (day.WeatherGenerated)
                {
                    day.Weather = null;
                }

                day.WeatherGenerated = false;
            }

            window.CalendarData.SetDictionaryWeatherDatas(dateTime, weatherDatas);

            //dateTime.SetSelectedDayWeatherNotes(window.CalendarData, window.TbxSelectedDayNotes, window.LblSelectedDayWeather);

            e.Handled = true;
        }

        public static void SetDictionaryWeatherDatas(this Dictionary<DateTime, DayData> dayDataDictionary, DateTime dateTime, WeatherData[] weatherDatas)
        {
            if (weatherDatas != null && dateTime != null)
            {
                for (int i = 1; i < weatherDatas.Length; i++)
                {
                    DateTime nextDay = dateTime.AddDays(i);
                    DayData day;

                    if (dayDataDictionary.ContainsKey(nextDay))
                    {
                        day = dayDataDictionary[nextDay];
                        if (day.Weather == null)
                        {
                            day.Weather = weatherDatas[i];
                        }
                        else
                        {
                            day.Weather = day.Weather + weatherDatas[i];
                        }
                    }
                    else
                    {
                        day = new DayData(nextDay, DEFAULT_DAILY_NOTES_TEXT, weatherDatas[i]);
                        dayDataDictionary.Add(nextDay, day);
                    }
                }
            }
        }

        public static void GenerateWeatherToDate(this DateTime startTime, DateTime endTime, WeatherNotesWindow window, RoutedEventArgs e)
        {
            for (DateTime time = startTime; time < endTime; time = time.AddDays(1))
            {
                time.GenerateWeatherFromDateTime(window, e);
            }

            startTime.SetSelectedDayWeatherNotes(window.CalendarData, window.TbxSelectedDayNotes, window.LblSelectedDayWeather);
        }

        public static void ClearWeatherToDate(this DateTime startTime, DateTime endTime, WeatherNotesWindow window, RoutedEventArgs e)
        {
            for (DateTime time = startTime; time < endTime; time = time.AddDays(1))
            {
                time.ClearWeatherFromDateTime(window, e);
            }

            startTime.SetSelectedDayWeatherNotes(window.CalendarData, window.TbxSelectedDayNotes, window.LblSelectedDayWeather);
        }

        #endregion

        #region Math Extensions

        public static int Mod(this int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        public static double Lerp(this double v0, double v1, double t)
        {
            return (1 - t) * v0 + t * v1;
        }

        public static double Lerp(this int v0, double v1, double t)
        {
            return (1 - t) * v0 + t * v1;
        }

        public static double LerpOver(this double[] array, int index, double t)
        {
            return array[(index - 1).Mod(array.Length)].Lerp(array[index.Mod(array.Length)], t);
        }

        public static double LerpOver(this int[] array, int index, double t)
        {
            return array[(index - 1).Mod(array.Length)].Lerp(array[index.Mod(array.Length)], t);
        }

        #endregion

    }
}
