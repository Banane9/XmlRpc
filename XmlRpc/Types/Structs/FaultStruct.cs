using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.Types.Structs
{
    /// <summary>
    /// Gets the struct returned when a method call has a fault.
    /// </summary>
    public sealed class FaultStruct : BaseStruct<FaultStruct>
    {
        /// <summary>
        /// Backing field for the FaultCode property.
        /// </summary>
        private XmlRpcInt faultCode = new XmlRpcInt();

        /// <summary>
        /// Backing field for the FaultString property.
        /// </summary>
        private XmlRpcString faultString = new XmlRpcString();

        /// <summary>
        /// Gets the fault code.
        /// </summary>
        public int FaultCode
        {
            get { return faultCode.Value; }
        }

        /// <summary>
        /// Gets the description of the fault.
        /// </summary>
        public string FaultString
        {
            get { return faultString.Value; }
        }

        /// <summary>
        /// Generates an XElement storing the information in this struct.
        /// </summary>
        /// <returns>The generated XElement.</returns>
        public override XElement GenerateXml()
        {
            return new XElement(XName.Get(ElementName),
                makeMemberElement("faultCode", faultCode.GenerateXml()),
                makeMemberElement("faultString", faultString.GenerateXml()));
        }

        /// <summary>
        /// Fills the properties of this struct with the information contained in the element.
        /// </summary>
        /// <param name="xElement">The struct element storing the information.</param>
        /// <returns>Itself, for convenience.</returns>
        public override FaultStruct ParseXml(XElement xElement)
        {
            checkName(xElement);

            foreach (XElement member in xElement.Descendants(XName.Get(MemberElement)))
            {
                checkIsValidMemberElement(member);

                XElement value = getMemberValueElement(member);

                switch (getMemberName(member))
                {
                    case "faultCode":
                        faultCode.ParseXml(getValueContent(value, faultCode.ElementName));
                        break;

                    case "faultString":
                        faultString.ParseXml(getValueContent(value, faultString.ElementName));
                        break;

                    default:
                        throw new FormatException("Unexpected member with name " + getMemberName(member));
                }
            }

            return this;
        }
    }
}