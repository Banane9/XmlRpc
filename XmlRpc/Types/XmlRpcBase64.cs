using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.Types
{
    /// <summary>
    /// Represents an XmlRpcType containing a byte array that is formatted as base64 string.
    /// </summary>
    public sealed class XmlRpcBase64 : XmlRpcType<byte[]>
    {
        /// <summary>
        /// The name of value content elements for this XmlRpc type.
        /// </summary>
        public override string ContentElementName
        {
            get { return XmlRpcElements.Base64Element; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcBase64"/> class with a zero-length byte array for the Value property.
        /// </summary>
        public XmlRpcBase64()
            : base(new byte[0])
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcBase64"/> class with the given value.
        /// </summary>
        /// <param name="value">The data encapsulated by this.</param>
        public XmlRpcBase64(byte[] value)
            : base(value)
        { }

        /// <summary>
        /// Generates an XElement from the Value. Default implementation creates an XElement with the ElementName and the content from Value.
        /// </summary>
        /// <returns>The generated Xml.</returns>
        public override XElement GenerateXml()
        {
            return new XElement(XName.Get(XmlRpcElements.ValueElement),
                new XElement(XName.Get(ContentElementName), Convert.ToBase64String(Value)));
        }

        /// <summary>
        /// Sets the Value property with the information contained in the value-XElement.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected override bool parseXml(XElement xElement)
        {
            try
            {
                Value = Convert.FromBase64String(xElement.Elements().First().Value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}