using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Weather_Calendar.Extensions;
using Weather_Calendar.Weather;
using Weather_Calendar.Weather.Data;

namespace Weather_Calendar
{
    public partial class WeatherNotesWindow : Window
    {
        #region Public Properties

        public WeatherNoteCalendarData CalendarData { get; set; } = new WeatherNoteCalendarData();

        #endregion

        #region Constructor

        public WeatherNotesWindow()
        {
            InitializeComponent();

            HolidayData.Initialize();

            LoadData(ExtensionMethods.LAST_CALENDAR_FILENAME);
        }

        #endregion

        #region GUI Event Handlers

        #region Menu Handlers

        private void MnuFileSave_Click(object sender, RoutedEventArgs e)
        {
            CalendarData.SaveCalendar();
        }

        private void MnuFileLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void MnuFileExit_Click(object sender, RoutedEventArgs e)
        {
            OnApplicationExit();
        }

        #endregion

        #region Calendar Handlers

        private void GregorianCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GregorianCalendar.IsInitialized)
            {
                GregorianCalendar.SelectedDate.SetSelectedDate(this);

                Mouse.Capture(null);
            }
        }

        #endregion

        #region Textbox Handlers

        private void TbxSelectedDayNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbxSelectedDayNotes.IsInitialized)
            {
                GregorianCalendar.SelectedDate.SetDailyNoteboxText(this, e);
            }
        }

        private void TbxGeneralNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbxGeneralNotes.IsInitialized)
            {
                CalendarData.GeneralNotes = TbxGeneralNotes.Text;
            }
        }

        #endregion

        #region Button Handlers

        #region Advance/Decrease Current Day

        #region One Day

        private void BtnAdvanceDay_Click(object sender, RoutedEventArgs e)
        {
            if (BtnAdvanceDay.IsInitialized)
            {
                CalendarData.AddDays(1);
                Advance();
            }
        }

        private void BtnDecreaseDay_RtClick(object sender, MouseButtonEventArgs e)
        {
            if (BtnAdvanceDay.IsInitialized)
            {
                CalendarData.AddDays(-1);
                Advance();
            }
        }

        #endregion

        #region One Week

        private void BtnAdvanceWeek_Click(object sender, RoutedEventArgs e)
        {
            if (BtnAdvanceWeek.IsInitialized)
            {
                CalendarData.AddDays(7);
                Advance();
            }
        }

        private void BtnDecreaseWeek_RtClick(object sender, MouseButtonEventArgs e)
        {
            if (BtnAdvanceDay.IsInitialized)
            {
                CalendarData.AddDays(-7);
                Advance();
            }
        }

        #endregion

        #region One Month

        private void BtnAdvanceMonth_Click(object sender, RoutedEventArgs e)
        {
            if (BtnAdvanceMonth.IsInitialized)
            {
                CalendarData.AddMonths(1);
                Advance();
            }
        }

        private void BtnDecreaseMonth_RtClick(object sender, MouseButtonEventArgs e)
        {
            if (BtnAdvanceDay.IsInitialized)
            {
                CalendarData.AddMonths(-1);
                Advance();
            }
        }

        #endregion

        #region One Year

        private void BtnAdvanceYear_Click(object sender, RoutedEventArgs e)
        {
            if (BtnAdvanceYear.IsInitialized)
            {
                CalendarData.AddYears(1);
                Advance();
            }
        }

        private void BtnDecreaseYear_RtClick(object sender, MouseButtonEventArgs e)
        {
            if (BtnAdvanceDay.IsInitialized)
            {
                CalendarData.AddYears(-1);
                Advance();
            }
        }

        #endregion

        #endregion

        #region Generate/Clear Selected Day

        #region One Day

        private void BtnGenerateDay_Click(object sender, RoutedEventArgs e)
        {
            if(BtnGenerateDay.IsInitialized)
            {
                if (GregorianCalendar.SelectedDate.HasValue)
                {
                    DateTime startTime = GregorianCalendar.SelectedDate.Value;
                    DateTime endTime = startTime.AddDays(1);

                    startTime.GenerateWeatherToDate(endTime, this, e);
                }
            }
        }

        private void BtnClearWeatherDay_RtClick(object sender, RoutedEventArgs e)
        {
            if (BtnGenerateDay.IsInitialized)
            {
                if (GregorianCalendar.SelectedDate.HasValue)
                {
                    DateTime startTime = GregorianCalendar.SelectedDate.Value;
                    DateTime endTime = startTime.AddDays(1);

                    startTime.ClearWeatherToDate(endTime, this, e);
                }
            }
        }

        #endregion

        #region One Week

        private void BtnGenerateWeek_Click(object sender, RoutedEventArgs e)
        {
            if (BtnGenerateWeek.IsInitialized)
            {
                if (GregorianCalendar.SelectedDate.HasValue)
                {
                    DateTime startTime = GregorianCalendar.SelectedDate.Value;
                    DateTime endTime = startTime.AddDays(7);

                    startTime.GenerateWeatherToDate(endTime, this, e);
                }
            }
        }

        private void BtnClearWeatherWeek_RtClick(object sender, RoutedEventArgs e)
        {
            if (BtnGenerateWeek.IsInitialized)
            {
                if (GregorianCalendar.SelectedDate.HasValue)
                {
                    DateTime startTime = GregorianCalendar.SelectedDate.Value;
                    DateTime endTime = startTime.AddDays(7);

                    startTime.ClearWeatherToDate(endTime, this, e);
                }
            }
        }

        #endregion

        #region One Month

        private void BtnGenerateMonth_Click(object sender, RoutedEventArgs e)
        {
            if (BtnGenerateMonth.IsInitialized)
            {
                if (GregorianCalendar.SelectedDate.HasValue)
                {
                    DateTime startTime = GregorianCalendar.SelectedDate.Value;
                    DateTime endTime = startTime.AddMonths(1);

                    startTime.GenerateWeatherToDate(endTime, this, e);
                }
            }
        }

        private void BtnClearWeatherMonth_RtClick(object sender, RoutedEventArgs e)
        {
            if (BtnGenerateMonth.IsInitialized)
            {
                if (GregorianCalendar.SelectedDate.HasValue)
                {
                    DateTime startTime = GregorianCalendar.SelectedDate.Value;
                    DateTime endTime = startTime.AddMonths(1);

                    startTime.ClearWeatherToDate(endTime, this, e);
                }
            }
        }

        #endregion

        #region One Year

        private void BtnGenerateYear_Click(object sender, RoutedEventArgs e)
        {
            if (BtnGenerateYear.IsInitialized)
            {
                if (GregorianCalendar.SelectedDate.HasValue)
                {
                    DateTime startTime = GregorianCalendar.SelectedDate.Value;
                    DateTime endTime = startTime.AddYears(1);

                    startTime.GenerateWeatherToDate(endTime, this, e);
                }
            }
        }

        private void BtnClearWeatherYear_RtClick(object sender, RoutedEventArgs e)
        {
            if (BtnGenerateYear.IsInitialized)
            {
                if (GregorianCalendar.SelectedDate.HasValue)
                {
                    DateTime startTime = GregorianCalendar.SelectedDate.Value;
                    DateTime endTime = startTime.AddYears(1);

                    startTime.ClearWeatherToDate(endTime, this, e);
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region Exit Handler

        protected override void OnClosing(CancelEventArgs e)
        {
            //e.Cancel = true;

            Save();
        }

        private void OnApplicationExit()
        {
            Save();

            Exit();
        }

        private void Exit(int code = 0)
        {
            Environment.Exit(code);
        }

        public void Save()
        {
            CalendarData.SaveCalendar(ExtensionMethods.LAST_CALENDAR_FILENAME);
        }

        #endregion

        #region Private Methods

        private void Advance()
        {

            CalendarData.CurrentDate.SetCurrentDateLabel(this);

            GregorianCalendar.SelectedDate = CalendarData.CurrentDate;
            GregorianCalendar.DisplayDate = CalendarData.CurrentDate;
            GregorianCalendar.SelectedDate.SetSelectedDate(this);
        }

        private void LoadData(string filename = null)
        {
            CalendarData.Clear();

            WeatherNoteCalendarData tempCalendar = WeatherNoteCalendarData.LoadCalendar(filename);

            if (tempCalendar != null)
            {
                CalendarData = tempCalendar;

                GregorianCalendar.SelectedDate = CalendarData.CurrentDate;
                GregorianCalendar.SelectedDate.SetSelectedDate(this);

                TbxGeneralNotes.Text = CalendarData.GeneralNotes;
                Advance();
            }
        }

        #endregion

    }
}
