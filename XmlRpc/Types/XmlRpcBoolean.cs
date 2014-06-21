using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.Types
{
    /// <summary>
    /// Represents an XmlRpcType containing a boolean.
    /// </summary>
    public class XmlRpcBoolean : XmlRpcType<bool>
    {
        /// <summary>
        /// The name of Elements of this type.
        /// </summary>
        public override string ElementName
        {
            get { return "boolean"; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ManiaNet.XmlRpc.Types.XmlRpcBoolean"/> class with Value set to the default value for bool.
        /// </summary>
        public XmlRpcBoolean()
            : base()
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="ManiaNet.XmlRpc.Types.XmlRpcBoolean"/> class with the given value.
        /// </summary>
        /// <param name="value">The bool encapsulated by this.</param>
        public XmlRpcBoolean(bool value)
            : base(value)
        { }

        /// <summary>
        /// Generates an XElement from the Value. Default implementation creates an XElement with the ElementName and the content from Value.
        /// </summary>
        /// <returns>The generated Xml.</returns>
        public override XElement GenerateXml()
        {
            return new XElement(XName.Get(ElementName), Value ? 1 : 0);
        }

        /// <summary>
        /// Sets the Value property with the information contained in the XElement. It must have a name fitting with the ElementName property.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Itself, for convenience.</returns>
        public override XmlRpcType<bool> ParseXml(XElement xElement)
        {
            checkName(xElement);

            switch (xElement.Value.ToLower())
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
                    throw new FormatException("Boolean format not recognized");
            }

            return this;
        }
    }
}