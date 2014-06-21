using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.Types
{
    /// <summary>
    /// Represents an XmlRpcType containing an array of XmlRpcTypes that derive from TBase.
    /// For example for a string array, TArray would be XmlRpcString and TBase would be string.
    /// TBase enforces TArray. TArray has to be XmlRpcString because it derives from XmlRpcType&lt;string&gt;
    /// </summary>
    /// <typeparam name="TArray">TArray[] is the Type of the Value property.</typeparam>
    /// <typeparam name="TBase">TBase is the base type that TArray has to derive from.</typeparam>
    public class XmlRpcArray<TArray, TBase> : XmlRpcType<TArray[]>, IEnumerable<TBase> where TArray : XmlRpcType<TBase>, new()
    {
        public const string DataElement = "data";

        public const string ValueElement = "value";

        /// <summary>
        /// The name of Elements of this type.
        /// </summary>
        public override string ElementName
        {
            get { return "array"; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ManiaNet.XmlRpc.Types.XmlRpcArray"/> class with the given value.
        /// </summary>
        /// <param name="value">The array encapsulated by this.</param>
        public XmlRpcArray(TArray[] value)
            : base(value)
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="ManiaNet.XmlRpc.Types.XmlRpcArray"/> class with a zero-length TArray array for the Value property.
        /// </summary>
        public XmlRpcArray()
            : base(new TArray[0])
        { }

        /// <summary>
        /// Generates an XElement from the Value. Default implementation creates an XElement with the ElementName and the content from Value.
        /// </summary>
        /// <returns>The generated Xml.</returns>
        public override XElement GenerateXml()
        {
            XElement array = new XElement(XName.Get(this.ElementName));

            XElement data = new XElement(XName.Get("data"));
            foreach (TArray value in Value)
            {
                XElement valueElement = new XElement(XName.Get("value"));
                valueElement.Add(value.GenerateXml());
                data.Add(valueElement);
            }

            array.Add(data);
            return array;
        }

        public IEnumerator<TBase> GetEnumerator()
        {
            return Value.Select(value => value.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Sets the Value property with the information contained in the XElement. It must have a name fitting with the ElementName property.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Itself, for convenience.</returns>
        public override XmlRpcType<TArray[]> ParseXml(XElement xElement)
        {
            checkName(xElement);
            List<TArray> content = new List<TArray>();

            foreach (XElement valueElement in xElement.Element(XName.Get(DataElement)).Elements(XName.Get(ValueElement)))
            {
                TArray value = new TArray();

                if (valueElement.HasElements)
                {
                    XElement valueContentElement = valueElement.Element(XName.Get(value.ElementName));

                    if (valueContentElement == null)
                        throw new FormatException("Value Content isn't of the expected type. Expected: " + value.ElementName);

                    value.ParseXml(valueElement.Elements().First());
                }
                else if (value.ElementName == "string") //default type for value is string
                {
                    value.ParseXml(new XElement(XName.Get("string"), valueElement.Value));
                }
                else
                {
                    throw new FormatException("Content of value tag is not a tag, and type of the array isn't String");
                }

                content.Add(value);
            }

            Value = content.ToArray();
            return this;
        }
    }
}