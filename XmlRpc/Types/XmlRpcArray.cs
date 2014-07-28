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
    /// <typeparam name="TArrayBase">TBase is the base type that TArray has to derive from.</typeparam>
    public class XmlRpcArray<TArray, TArrayBase> : XmlRpcType<TArray[]>, IEnumerable<TArrayBase> where TArray : XmlRpcType<TArrayBase>, new()
    {
        /// <summary>
        /// The name of Elements of this type.
        /// </summary>
        public override string ContentElementName
        {
            get { return "array"; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcArray&lt;TArray, TArrayBase&gt;"/> class with the given value.
        /// </summary>
        /// <param name="value">The array encapsulated by this.</param>
        public XmlRpcArray(params TArray[] value)
            : base(value)
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcArray&lt;TArray, TArrayBase&gt;"/> class with a zero-length TArray array for the Value property.
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
            return new XElement(XName.Get(ContentElementName),
                new XElement(XName.Get("data"),
                    Value.Select(value => value.GenerateXml()).ToArray()));
        }

        /// <summary>
        /// Returns an enumerator that iterates the Array.
        /// </summary>
        /// <returns>An enumerator that iterates the Array.</returns>
        public IEnumerator<TArrayBase> GetEnumerator()
        {
            return Value.Select(value => value.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Sets the Value property with the information contained in the value-XElement.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected override bool parseXml(XElement xElement)
        {
            List<TArray> content = new List<TArray>();

            if (!xElement.Elements().First().Elements().First().Name.LocalName.Equals(XmlRpcElements.ArrayDataElement))
                return false;

            foreach (XElement valueElement in xElement.Elements().First().Elements().First().Elements())
            {
                TArray value = new TArray();

                if (!value.ParseXml(valueElement))
                    return false;

                content.Add(value);
            }

            Value = content.ToArray();

            return true;
        }
    }
}