using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace XmlRpc.Types
{
    /// <summary>
    /// Represents an XmlRpcType containing a DateTime.
    /// </summary>
    public class XmlRpcDateTime : XmlRpcType<DateTime>
    {
        /// <summary>
        /// The name of Elements of this type.
        /// </summary>
        public override string ElementName
        {
            get { return "dateTime.iso8601"; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ManiaNet.XmlRpc.Types.XmlRpcDateTime"/> class with Value set to the defaut value for DateTime.
        /// </summary>
        public XmlRpcDateTime()
            : base()
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="ManiaNet.XmlRpc.Types.XmlRpcDateTime"/> class with the given value.
        /// </summary>
        /// <param name="value">The DateTime encapsulated by this.</param>
        public XmlRpcDateTime(DateTime value)
            : base(value)
        { }

        /// <summary>
        /// Generates an XElement from the Value. Default implementation creates an XElement with the ElementName and the content from Value.
        /// </summary>
        /// <returns>The generated Xml.</returns>
        public override XElement GenerateXml()
        {
            string date = Value.Year.ToString() + Value.Month.ToString() + Value.Date.ToString() + "T" + Value.Hour.ToString() + ":" + Value.Minute.ToString() + ":" + Value.Second.ToString();

            return new XElement(XName.Get(ElementName), date);
        }

        /// <summary>
        /// Sets the Value property with the information contained in the XElement. It must have a name fitting with the ElementName property.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Itself, for convenience.</returns>
        public override XmlRpcType<DateTime> ParseXml(XElement xElement)
        {
            checkName(xElement);

            string date = xElement.Value; //formatted according to ISO-8601  yyyymmddThh:mm:ss  (the T is a literal).
            int yearLength = date.IndexOf('T') - 4;

            //Rudamentary check for correct format.
            if (!Regex.IsMatch(@"\d{" + yearLength + @"}[0-1]\d[0-1]\dT[0-2]\d:[0-5]\d:[0-5]\d", date))
                throw new FormatException("Ill formed ISO-8601 timestamp. Expected Format yyyymmddThh:mm:ss");

            int year = int.Parse(date.Remove(yearLength));
            int month = int.Parse(date.Remove(0, yearLength).Remove(2));
            int day = int.Parse(date.Remove(0, yearLength + 2).Remove(2));
            int hour = int.Parse(date.Remove(0, yearLength + 5).Remove(2));
            int minute = int.Parse(date.Remove(0, yearLength + 8).Remove(2));
            int second = int.Parse(date.Remove(yearLength + 11).Remove(2));

            Value = new DateTime(year, month, day, hour, minute, second);

            return this;
        }
    }
}