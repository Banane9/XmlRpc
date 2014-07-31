using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XmlRpc.Types.Structs;

namespace XmlRpc.Types
{
    /// <summary>
    /// Represents an XmlRpcType containing a xml rpc struct that is derived from <see cref="XmlRpc.Types.Structs.BaseStruct"/>.
    /// </summary>
    /// <typeparam name="TXmlRpcStruct">The Type of the struct. Also the Type of the Value property.</typeparam>
    public sealed class XmlRpcStruct<TXmlRpcStruct> : XmlRpcType<TXmlRpcStruct>
        where TXmlRpcStruct : BaseStruct, new()
    {
        /// <summary>
        /// The name of value content elements for this XmlRpc type.
        /// </summary>
        public override string ContentElementName
        {
            get { return XmlRpcElements.StructElement; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcStruct&lt;TXmlRpcStruct&gt;"/> class with Value set to default value for TXmlRpcType.
        /// </summary>
        public XmlRpcStruct()
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcStruct&lt;TXmlRpcStruct&gt;"/> class with the given value.
        /// </summary>
        /// <param name="value">The struct encapsulated by this.</param>
        public XmlRpcStruct(TXmlRpcStruct value)
            : base(value)
        { }

        /// <summary>
        /// Generates a value-XElement capsuling the struct.
        /// </summary>
        /// <returns>The generated Xml.</returns>
        public override XElement GenerateXml()
        {
            return new XElement(XName.Get(XmlRpcElements.ValueElement),
                                Value.GenerateXml());
        }

        /// <summary>
        /// Sets the Value property with the information contained in the value-XElement.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected override bool parseXml(XElement xElement)
        {
            if (Value == null)
                Value = new TXmlRpcStruct();

            return Value.ParseXml(xElement.Elements().First());
        }
    }
}