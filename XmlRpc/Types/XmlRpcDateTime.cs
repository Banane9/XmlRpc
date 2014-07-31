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
    public sealed class XmlRpcDateTime : XmlRpcType<DateTime>
    {
        /// <summary>
        /// The name of Elements of this type.
        /// </summary>
        public override string ContentElementName
        {
            get { return XmlRpcElements.DateTimeElement; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcDateTime"/> class with Value set to the defaut value for DateTime.
        /// </summary>
        public XmlRpcDateTime()
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcDateTime"/> class with the given value.
        /// </summary>
        /// <param name="value">The DateTime encapsulated by this.</param>
        public XmlRpcDateTime(DateTime value)
            : base(value)
        { }

        /// <summary>
        /// Generates a value-XElement containing the information stored in this date time.
        /// </summary>
        /// <returns>The generated Xml.</returns>
        public override XElement GenerateXml()
        {
            string date = string.Format("{0}{1}{2}T{3}:{4}:{5}", Value.Year, Value.Month, Value.Day, Value.Hour, Value.Minute, Value.Second);

            return new XElement(XName.Get(XmlRpcElements.ValueElement),
                                new XElement(XName.Get(ContentElementName), date));
        }

        /// <summary>
        /// Sets the Value property with the information contained in the value-XElement.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected override bool parseXml(XElement xElement)
        {
            string date = xElement.Value; //formatted according to ISO-8601  yyyymmddThh:mm:ss  (the T is a literal).
            int yearLength = date.IndexOf('T') - 4;

            //Rudamentary check for correct format.
            if (!Regex.IsMatch(@"\d{" + yearLength + @"}[0-1]\d[0-1]\dT[0-2]\d:[0-5]\d:[0-5]\d", date))
                return false;

            try
            {
                int year = int.Parse(date.Remove(yearLength));
                int month = int.Parse(date.Remove(0, yearLength).Remove(2));
                int day = int.Parse(date.Remove(0, yearLength + 2).Remove(2));
                int hour = int.Parse(date.Remove(0, yearLength + 5).Remove(2));
                int minute = int.Parse(date.Remove(0, yearLength + 8).Remove(2));
                int second = int.Parse(date.Remove(yearLength + 11).Remove(2));

                Value = new DateTime(year, month, day, hour, minute, second);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}