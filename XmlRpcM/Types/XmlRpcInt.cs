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
        /// The name of Elements of this type.
        /// </summary>
        public override string ElementName
        {
            get { return "int"; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ManiaNet.XmlRpc.Types.XmlRpcInt"/> class with Value set to the default value for int.
        /// </summary>
        public XmlRpcInt()
            : base()
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="ManiaNet.XmlRpc.Types.XmlRpcInt"/> class with the given value.
        /// </summary>
        /// <param name="value">The int encapsulated by this.</param>
        public XmlRpcInt(int value)
            : base(value)
        { }

        /// <summary>
        /// Sets the Value property with the information contained in the XElement. It must have a name fitting with the ElementName property.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Itself, for convenience.</returns>
        public override XmlRpcType<int> ParseXml(XElement xElement)
        {
            checkName(xElement);

            Value = int.Parse(xElement.Value);

            return this;
        }
    }
}