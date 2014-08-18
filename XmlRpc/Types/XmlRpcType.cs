using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.Types
{
    /// <summary>
    /// Abstract base class for all XmlRpcTypes.
    /// </summary>
    /// <typeparam name="TValue">The Type of the Value property.</typeparam>
    public abstract class XmlRpcType<TValue>
    {
        /// <summary>
        /// The name of value content elements for this XmlRpc type.
        /// </summary>
        public abstract string ContentElementName { get; }

        /// <summary>
        /// Gets or sets the Value contained by this XmlRpcType.
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcType&lt;TValue&gt;"/> class with Value set to the default value for TValue.
        /// </summary>
        protected XmlRpcType()
        {
            Value = default(TValue);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.Types.XmlRpcType&lt;TValue&gt;"/> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        protected XmlRpcType(TValue value)
        {
            Value = value;
        }

        /// <summary>
        /// Generates a value-XElement containing the information stored in this XmlRpc type.
        /// <para/>
        /// Default implementation creates an XElement with the ContentElementName and the content from Value, and wraps it in a value element.
        /// </summary>
        /// <returns>The generated Xml.</returns>
        public virtual XElement GenerateXml()
        {
            return new XElement(XName.Get(XmlRpcElements.ValueElement),
                new XElement(XName.Get(ContentElementName), Value));
        }

        /// <summary>
        /// Sets the Value property with the information contained in the value-XElement.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        public bool ParseXml(XElement xElement)
        {
            if (!isValueElement(xElement) || !hasValueCorrectContent(xElement))
                return false;

            return parseXml(xElement);
        }

        /// <summary>
        /// Returns a string representation of the Type.
        /// </summary>
        /// <returns>A string representation of the Type.</returns>
        public override string ToString()
        {
            return GenerateXml().ToString();
        }

        /// <summary>
        /// Checks whether the given XElement has the local name corresponding to a value element.
        /// </summary>
        /// <param name="xElement">The element to check.</param>
        /// <returns>Whether it has the correct local name.</returns>
        protected static bool isValueElement(XElement xElement)
        {
            return xElement.Name.LocalName.Equals(XmlRpcElements.ValueElement);
        }

        /// <summary>
        /// Checks whether the value-XElement has content fitting with this XmlRpc type.
        /// <para/>
        /// Can be overridden if a single child element with the correct name is not the desired check.
        /// Validity of the XElement will have already been verified.
        /// </summary>
        /// <param name="xElement">The element to check.</param>
        /// <returns>Whether it has fitting content or not.</returns>
        protected virtual bool hasValueCorrectContent(XElement xElement)
        {
            return (xElement.HasElements && xElement.Elements().Count() == 1 && xElement.Elements().First().Name.LocalName.Equals(ContentElementName))
                   || (!xElement.HasElements && !xElement.IsEmpty);
        }

        /// <summary>
        /// Sets the Value property with the information contained in the value-XElement.
        /// <para/>
        /// Gets called by the ParseXml method to do the actual parsing. Validity of the XElement will have already been verified.
        /// </summary>
        /// <param name="xElement">The element containing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected abstract bool parseXml(XElement xElement);
    }
}