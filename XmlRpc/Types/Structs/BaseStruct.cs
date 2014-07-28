using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.Types.Structs
{
    /// <summary>
    /// Abstract base class for all xml rpc structs.
    /// </summary>
    public abstract class BaseStruct
    {
        /// <summary>
        /// Generates an XElement storing the information in this struct.
        /// </summary>
        /// <returns>The generated XElement.</returns>
        public abstract XElement GenerateXml();

        /// <summary>
        /// Fills the properties of this struct with the information contained in the XElement.
        /// </summary>
        /// <param name="xElement">The struct element storing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        public bool ParseXml(XElement xElement)
        {
            isStructElement(xElement);

            foreach (XElement member in xElement.Elements())
            {
                if (!isValidMemberElement(member))
                    return false;

                if (!parseXml(member))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a string representation of the struct.
        /// </summary>
        /// <returns>A string representation of the struct.</returns>
        public override string ToString()
        {
            return GenerateXml().ToString();
        }

        /// <summary>
        /// Gets the name of the member from a member element.
        /// </summary>
        /// <param name="member">The member element to get the name from.</param>
        /// <returns>The name of the member.</returns>
        protected static string getMemberName(XElement member)
        {
            isValidMemberElement(member);

            return member.Element(XName.Get(XmlRpcElements.StructMemberNameElement)).Value;
        }

        /// <summary>
        /// Gets the value element of a member from a member element.
        /// </summary>
        /// <param name="member">The member element to get the value from.</param>
        /// <returns>The value element of the member or null if not a valid member.</returns>
        protected static XElement getMemberValueElement(XElement member)
        {
            return isValidMemberElement(member) ? member.Element(XName.Get(XmlRpcElements.ValueElement)) : null;
        }

        /// <summary>
        /// Checks whether the given XElement has the local name corresponding to a struct element.
        /// </summary>
        /// <param name="xElement">The element to check.</param>
        /// <returns>Whether it has the correct local name.</returns>
        protected static bool isStructElement(XElement xElement)
        {
            return xElement.Name.LocalName.Equals(XmlRpcElements.StructElement);
        }

        /// <summary>
        /// Checks if an element is a valid member element.
        /// </summary>
        /// <param name="member">The element to check.</param>
        protected static bool isValidMemberElement(XElement member)
        {
            return member.Name.LocalName.Equals(XmlRpcElements.StructMemberElement)
                && member.HasElements
                && member.Elements(XName.Get(XmlRpcElements.StructMemberNameElement)).Any()
                && member.Elements(XName.Get(XmlRpcElements.ValueElement)).Any();
        }

        /// <summary>
        /// Creates a member element from the name and the value content element.
        /// </summary>
        /// <param name="name">The name of the member.</param>
        /// <param name="value">The value XmlRpcType.</param>
        /// <typeparam name="T">The XmlRpType's base type.</typeparam>
        /// <returns>The member element with the given name and value content.</returns>
        protected static XElement makeMemberElement<T>(string name, XmlRpcType<T> value)
        {
            return new XElement(XName.Get(XmlRpcElements.StructMemberElement), makeNameXElement(name), value.GenerateXml());
        }

        /// <summary>
        /// Fills the property of this struct that has the correct name with the information contained in the member-XElement.
        /// </summary>
        /// <param name="member">The member element storing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected abstract bool parseXml(XElement member);

        /// <summary>
        /// Creates a name element with the given content.
        /// </summary>
        /// <param name="name">The value of the name element.</param>
        /// <returns>The name element with the given value.</returns>
        private static XElement makeNameXElement(string name)
        {
            return new XElement(XName.Get(XmlRpcElements.StructMemberNameElement), name);
        }
    }
}