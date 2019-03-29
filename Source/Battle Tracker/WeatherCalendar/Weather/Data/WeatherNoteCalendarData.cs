using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Weather_Calendar.Extensions;
using wforms = System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Weather_Calendar.Weather.Data
{
    [XmlRoot("CalendarData")]
    public class WeatherNoteCalendarData : Dictionary<DateTime, DayData>, IXmlSerializable
    {
        #region Public Properties

        [XmlAttribute("GeneralNotes")]
        public string GeneralNotes { get; set; } = ExtensionMethods.DEFAULT_GENERAL_NOTES_TEXT;
        [XmlAttribute("CurrentDate")]
        public DateTime CurrentDate { get; set; } = new DateTime(4710, 2, 24);

        #endregion

        #region Public Helper Methods

        public void AddDays(int days)
        {
            CurrentDate = CurrentDate.AddDays(days);
        }

        public void AddMonths(int months)
        {
            CurrentDate = CurrentDate.AddMonths(months);
        }

        public void AddYears(int years)
        {
            CurrentDate = CurrentDate.AddYears(years);
        }

        #region Save/Load Calendar

        public void SaveCalendar(string filename = null)
        {
            if (string.IsNullOrEmpty(filename))
            {
                using (var dialog = new wforms.SaveFileDialog())
                {
                    dialog.Filter = ExtensionMethods.FILE_DIALOG_FILTER;

                    if (dialog.ShowDialog() == wforms.DialogResult.OK)
                        SaveCalendar(dialog.FileName);
                }
            }
            else if (Count > 0 || GeneralNotes != ExtensionMethods.DEFAULT_GENERAL_NOTES_TEXT || CurrentDate != new DateTime(4710, 2, 24))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(WeatherNoteCalendarData));

                FileInfo finfo = new FileInfo(filename);
                finfo.Directory.Create();

                using (FileStream stream = new FileStream(filename, FileMode.Create))
                {
                    serializer.Serialize(stream, this);
                }
            }
        }

        public static WeatherNoteCalendarData LoadCalendar(string filename = null)
        {
            if (string.IsNullOrEmpty(filename))
            {
                using (var dialog = new wforms.OpenFileDialog())
                {
                    dialog.Filter = ExtensionMethods.FILE_DIALOG_FILTER;

                    if (dialog.ShowDialog() == wforms.DialogResult.OK)
                        return LoadCalendar(dialog.FileName);
                    else
                        return null;
                }
            }
            else if (File.Exists(filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(WeatherNoteCalendarData));

                using (FileStream stream = new FileStream(filename, FileMode.Open))
                {
                    return (WeatherNoteCalendarData)serializer.Deserialize(stream);
                }
            }

            return new WeatherNoteCalendarData();
        }

        #endregion

        #endregion

        #region IXmlSerializable 

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            DateTime currentDate = new DateTime(4710, 2, 24);
            string generalNotes = ExtensionMethods.DEFAULT_GENERAL_NOTES_TEXT;

            for (int attInd = 0; attInd < reader.AttributeCount; attInd++)
            {
                reader.MoveToAttribute(attInd);

                string temp;
                switch (reader.Name)
                {
                    case "Date":
                    temp = reader.Value;
                    DateTime.TryParse(temp, out currentDate);
                    break;

                    case "Notes":
                    generalNotes = reader.Value;
                    break;
                }
            }

            CurrentDate = currentDate;
            GeneralNotes = generalNotes;


            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element && reader.LocalName.StartsWith("DayData") && reader.IsStartElement())
                {
                    string name = reader.Name;
                    //Console.WriteLine("<" + name + ">");

                    DateTime dateTime = new DateTime(4710, 2, 24);
                    string notes = ExtensionMethods.DEFAULT_DAILY_NOTES_TEXT;
                    bool generated = false;

                    for (int attInd = 0; attInd < reader.AttributeCount; attInd++)
                    {
                        reader.MoveToAttribute(attInd);

                        string temp;
                        switch(reader.Name)
                        {
                            case "Date":
                            temp = reader.Value;
                            DateTime.TryParse(temp, out dateTime);
                            break;

                            case "Notes":
                            notes = reader.Value;
                            break;

                            case "Generated":
                            temp = reader.Value;
                            bool.TryParse(temp, out generated);
                            break;
                        }
                    }

                    DayData dayData = new DayData(dateTime, notes);
                    dayData.WeatherGenerated = generated;

                    reader.Read();

                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName.StartsWith("Weather") && reader.IsStartElement())
                    {
                        string name2 = reader.Name;

                        //Console.WriteLine("\t<" + name2 + ">");

                        WeatherData weatherData = new WeatherData();
                        PrecipitationData precipitation = new PrecipitationData();
                        FogData fogDataMorning = new FogData();
                        FogData fogDataEvening = new FogData();
                        WindData windData = new WindData();

                        for (int attInd = 0; attInd < reader.AttributeCount; attInd++)
                        {
                            reader.MoveToAttribute(attInd);
                            string temp;

                            switch (reader.Name)
                            {
                                case "Temperature":
                                temp = reader.Value;
                                double temperature = 0;
                                if (double.TryParse(temp, out temperature))
                                {
                                    weatherData.Temperature = temperature;
                                }
                                break;
                            }
                        }

                        reader.Read();

                        while (reader.NodeType == XmlNodeType.Element && !reader.LocalName.StartsWith("Weather"))
                        {
                            //Console.WriteLine("\t\t<" + reader.Name + "/>");

                            switch(reader.Name)
                            {
                                case "Precipitation":
                                precipitation.ReadXml(reader);
                                break;

                                case "FogMorning":
                                fogDataMorning.ReadXml(reader);
                                break;

                                case "FogEvening":
                                fogDataEvening.ReadXml(reader);
                                break;

                                case "Wind":
                                windData.ReadXml(reader);
                                break;

                                default:
                                    for (int attInd = 0; attInd < reader.AttributeCount; attInd++)
                                    {
                                        reader.MoveToAttribute(attInd);
                                        Console.WriteLine("<" + reader.Name + "=\"" + reader.Value + "\"/>");
                                    }
                                break;
                            }
                            

                            reader.Read();
                        }

                        //Console.WriteLine("\t</" + name2 + "> ");

                        weatherData.Precipitation = precipitation;
                        weatherData.MorningFog = fogDataMorning;
                        weatherData.EveningFog = fogDataEvening;
                        weatherData.Wind = windData;

                        dayData.Weather = weatherData;
                    }

                    //Console.WriteLine("</" + name + "> ");

                    Add(dayData.Date, dayData);
                }

            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Date", CurrentDate.ToString());
            if (GeneralNotes != ExtensionMethods.DEFAULT_GENERAL_NOTES_TEXT)
            {
                writer.WriteAttributeString("Notes", GeneralNotes);
            }
            foreach (var key in Keys)
            {
                this[key].WriteXml(writer);
            }
        }

        #endregion

    }
}
