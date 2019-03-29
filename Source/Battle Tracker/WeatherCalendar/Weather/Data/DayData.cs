using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Weather_Calendar.Weather.Data;

namespace Weather_Calendar.Weather.Data
{
    [Serializable]
    public class DayData : IXmlSerializable
    {
        #region Public Properties

        [XmlAttribute("Date")]
        public DateTime Date { get; set; }
        [XmlAttribute("Notes")]
        public string Notes { get; set; }
        [XmlAttribute("Weather")]
        public WeatherData Weather { get; set; }
        [XmlAttribute("Generated")]
        public bool WeatherGenerated { get; set; } = false;

        #endregion

        #region Public Constructors

        public DayData(DateTime equivalentDateTime, string notes, WeatherData weather = null)
        {
            Date = equivalentDateTime;
            Notes = notes;
            Weather = weather;
        }

        #endregion

        #region Comparison Overrides

        public override bool Equals(object obj)
        {
            if (obj is DayData dayData)
            {
                return Date.Equals(dayData.Date);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Date.GetHashCode();
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
            writer.WriteStartElement("DayData");
            writer.WriteAttributeString("Date", Date.ToString());
            if (Notes != Extensions.ExtensionMethods.DEFAULT_DAILY_NOTES_TEXT)
            {
                writer.WriteAttributeString("Notes", Notes);
            }
            writer.WriteAttributeString("Generated", WeatherGenerated.ToString());
            if (Weather != null)
            {
                Weather.WriteXml(writer);
            }
            writer.WriteEndElement();
        }

        #endregion

    }
}
