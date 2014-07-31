using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.Types.Structs
{
    /// <summary>
    /// Gets the struct returned when a method call has a fault.
    /// </summary>
    public sealed class FaultStruct : BaseStruct
    {
        /// <summary>
        /// Backing field for the FaultCode property.
        /// </summary>
        private readonly XmlRpcInt faultCode = new XmlRpcInt();

        /// <summary>
        /// Backing field for the FaultString property.
        /// </summary>
        private readonly XmlRpcString faultString = new XmlRpcString();

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
            return new XElement(XName.Get(XmlRpcElements.StructElement),
                                makeMemberElement("faultCode", faultCode),
                                makeMemberElement("faultString", faultString));
        }

        /// <summary>
        /// Fills the property of this struct that has the correct name with the information contained in the member-XElement.
        /// </summary>
        /// <param name="member">The member element storing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected override bool parseXml(XElement member)
        {
            XElement value = getMemberValueElement(member);

            switch (getMemberName(member))
            {
                case "faultCode":
                    if (!faultCode.ParseXml(value))
                        return false;
                    break;

                case "faultString":
                    if (!faultString.ParseXml(value))
                        return false;
                    break;

                default:
                    return false;
            }

            return true;
        }
    }
}