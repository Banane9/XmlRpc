using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.Types
{
    /// <summary>
    /// Represents an XmlRpcType containing a string.
    /// </summary>
    public sealed class XmlRpcString : XmlRpcType<string>
    {
        /// <summary>
        /// The name of value content elements for this XmlRpc type.
        /// </summary>
        public override string ContentElementName
        {
            get { return XmlRpcElements.StringElement; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcString"/> class with Value set to an empty string.
        /// </summary>
        public XmlRpcString()
            : base(string.Empty)
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcString"/> class with the given value.
        /// </summary>
        /// <param name="value">The string encapsulated by this.</param>
        public XmlRpcString(string value)
            : base(value)
        { }

        /// <summary>
        /// Checks whether the value-XElement has content fitting with this XmlRpc type.
        /// </summary>
        /// <param name="xElement">The element to check.</param>
        /// <returns>Whether it has fitting content or not.</returns>
        protected override bool hasValueCorrectContent(XElement xElement)
        {
            return (!xElement.HasElements && !xElement.IsEmpty) || base.hasValueCorrectContent(xElement);
        }

        /// <summary>
        /// Sets the Value property with the information contained in the value-XElement.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected override bool parseXml(XElement xElement)
        {
            if (xElement.HasElements)
                Value = xElement.Elements().First().Value;
            else if (!xElement.IsEmpty)
                Value = xElement.Value;
            else return false;

            return true;
        }
    }
}