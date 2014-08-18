using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.Types
{
    /// <summary>
    /// Represents an XmlRpcType containing a boolean.
    /// </summary>
    public sealed class XmlRpcBoolean : XmlRpcType<bool>
    {
        /// <summary>
        /// The name of Elements of this type.
        /// </summary>
        public override string ContentElementName
        {
            get { return XmlRpcElements.BooleanElement; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcBoolean"/> class with Value set to the default value for bool.
        /// </summary>
        public XmlRpcBoolean()
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcBoolean"/> class with the given value.
        /// </summary>
        /// <param name="value">The bool encapsulated by this.</param>
        public XmlRpcBoolean(bool value)
            : base(value)
        { }

        /// <summary>
        /// Generates a value-XElement containing the information stored in this XmlRpc type.
        /// </summary>
        /// <returns>The generated Xml.</returns>
        public override XElement GenerateXml()
        {
            return new XElement(XName.Get(XmlRpcElements.ValueElement),
                new XElement(XName.Get(ContentElementName), Value ? 1 : 0));
        }

        /// <summary>
        /// Sets the Value property with the information contained in the value-XElement.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected override bool parseXml(XElement xElement)
        {
            switch (xElement.Elements().First().Value.ToLower())
            {
                case "false":
                case "0":
                    Value = false;
                    break;

                case "true":
                case "1":
                    Value = true;
                    break;

                default:
                    return false;
            }

            return true;
        }
    }
}