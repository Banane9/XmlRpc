using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.Types
{
    /// <summary>
    /// Represents an XmlRpcType containing a double.
    /// </summary>
    public sealed class XmlRpcDouble : XmlRpcType<double>
    {
        /// <summary>
        /// The name of value content elements for this XmlRpc type.
        /// </summary>
        public override string ContentElementName
        {
            get { return XmlRpcElements.DoubleElement; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcDouble"/> class with Value set to the default value for double.
        /// </summary>
        public XmlRpcDouble()
            : base()
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcDouble"/> class with the given value.
        /// </summary>
        /// <param name="value">The double encapsulated by this.</param>
        public XmlRpcDouble(double value)
            : base(value)
        { }

        /// <summary>
        /// Sets the Value property with the information contained in the value-XElement.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected override bool parseXml(XElement xElement)
        {
            double value;

            if (double.TryParse(xElement.Elements().First().Value, out value))
            {
                Value = value;
                return true;
            }

            return false;
        }
    }
}