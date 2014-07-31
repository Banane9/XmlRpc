using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.Types
{
    /// <summary>
    /// Represents an XmlRpcType containing an int.
    /// </summary>
    public class XmlRpcInt : XmlRpcType<int>
    {
        /// <summary>
        /// The name of value content elements for this XmlRpc type.
        /// </summary>
        public override string ContentElementName
        {
            get { return XmlRpcElements.IntElement; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcInt"/> class with Value set to the default value for int.
        /// </summary>
        public XmlRpcInt()
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcInt"/> class with the given value.
        /// </summary>
        /// <param name="value">The int encapsulated by this.</param>
        public XmlRpcInt(int value)
            : base(value)
        { }

        /// <summary>
        /// Sets the Value property with the information contained in the value-XElement.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected override bool parseXml(XElement xElement)
        {
            int value;

            if (int.TryParse(xElement.Elements().First().Value, out value))
            {
                Value = value;
                return true;
            }

            return false;
        }
    }
}