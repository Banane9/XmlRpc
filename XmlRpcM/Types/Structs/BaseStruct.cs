using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.Types.Structs
{
    /// <summary>
    /// Abstract base class for all xml rpc structs.
    /// </summary>
    /// <typeparam name="TStruct">The type of the derived struct.</typeparam>
    public abstract class BaseStruct<TStruct> where TStruct : BaseStruct<TStruct>
    {
        public const string ElementName = "struct";

        protected const string MemberElement = "member";

        protected const string NameElement = "name";

        protected const string ValueElement = "value";

        /// <summary>
        /// Generates an XElement storing the information in this struct.
        /// </summary>
        /// <returns>The generated XElement.</returns>
        public abstract XElement GenerateXml();

        /// <summary>
        /// Fills the properties of this struct with the information contained in the XElement.
        /// </summary>
        /// <param name="xElement">The struct element storing the information.</param>
        /// <returns>Itself, for convenience.</returns>
        public abstract TStruct ParseXml(XElement xElement);

        public override string ToString()
        {
            return GenerateXml().ToString();
        }

        /// <summary>
        /// Checks if an element is a valid member element.
        /// </summary>
        /// <param name="member">The element to check.</param>
        protected void checkIsValidMemberElement(XElement member)
        {
            if (!member.Name.LocalName.Equals(MemberElement))
                throw new FormatException("Member Element has to have the name " + MemberElement);

            if (!member.HasElements)
                throw new FormatException("Member Element in struct has to have " + NameElement + " and " + ValueElement + " child-elements.");

            if (member.Element(XName.Get(NameElement)) == null)
                throw new FormatException("Member Element in struct has to have a " + NameElement + "child-element.");

            if (member.Element(XName.Get(ValueElement)) == null)
                throw new FormatException("Member Element in struct has to have a " + ValueElement + "child-element.");
        }

        /// <summary>
        /// Checks if an element's name fits the name in ElementName (struct)
        /// </summary>
        /// <param name="xElement">The element to check.</param>
        protected void checkName(XElement xElement)
        {
            if (!xElement.Name.LocalName.Equals(ElementName))
                throw new ArgumentException("Element has to have the name " + ElementName, "xElement");
        }

        /// <summary>
        /// Gets the name of the member from a member element.
        /// </summary>
        /// <param name="member">The member element to get the name from.</param>
        /// <returns>The name of the member.</returns>
        protected string getMemberName(XElement member)
        {
            checkIsValidMemberElement(member);

            return member.Element(XName.Get(NameElement)).Value;
        }

        /// <summary>
        /// Gets the value element of a member from a member element.
        /// </summary>
        /// <param name="member">The member element to get the vlaue from.</param>
        /// <returns>The value element of the member.</returns>
        protected XElement getMemberValueElement(XElement member)
        {
            checkIsValidMemberElement(member);

            return member.Element(XName.Get(ValueElement));
        }

        /// <summary>
        /// Returns the value element's content element if it's name fits stringElement or wraps it in an element of that name if it doesn't have child elements.
        /// This is because value tags with only content that is not inside another tag are string by default.
        /// </summary>
        /// <param name="value">The value element to get the content from.</param>
        /// <param name="elementName">The name for the string element (string).</param>
        /// <returns>The content as an element.</returns>
        protected XElement getValueContent(XElement value, string elementName)
        {
            if (!value.Name.LocalName.Equals(ValueElement))
                throw new FormatException("Value Element has to have the name " + ValueElement);

            if (value.HasElements)
                return value.Element(XName.Get(elementName));

            return new XElement(XName.Get(elementName), value.Value);
        }

        /// <summary>
        /// Creates a member element from the name and the value content element.
        /// </summary>
        /// <param name="name">The name of the member.</param>
        /// <param name="value">The value content element.</param>
        /// <returns>The member element with the given name and value content.</returns>
        protected XElement makeMemberElement(string name, XElement value)
        {
            return new XElement(XName.Get("member"), makeNameXElement(name), makeValueXElement(value));
        }

        /// <summary>
        /// Creates a name element with the given content.
        /// </summary>
        /// <param name="name">The value of the name element.</param>
        /// <returns>The name element with the given value.</returns>
        private XElement makeNameXElement(string name)
        {
            return new XElement(XName.Get(NameElement), name);
        }

        /// <summary>
        /// Creates a value element with the given content.
        /// </summary>
        /// <param name="value">The value content elment.</param>
        /// <returns>The value element with the given content.</returns>
        private XElement makeValueXElement(XElement value)
        {
            return new XElement(XName.Get(ValueElement), value);
        }
    }
}