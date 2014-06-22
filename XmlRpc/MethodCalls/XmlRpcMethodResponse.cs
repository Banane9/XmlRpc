using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XmlRpc.Types;
using XmlRpc.Types.Structs;

namespace XmlRpc.MethodCalls
{
    public class XmlRpcMethodResponse
    {
        /// <summary>
        /// Backing field for the Fault property.
        /// </summary>
        private XmlRpcStruct<FaultStruct> fault = new XmlRpcStruct<FaultStruct>();

        /// <summary>
        /// Gets the fault information when IsCompleted is true, and HadFault is true, or null otherwise.
        /// </summary>
        public FaultStruct Fault
        {
            get { return fault.Value; }
        }

        /// <summary>
        /// Gets whether there was a fault with the method call or not.
        /// </summary>
        public bool HadFault
        {
            get { return fault.Value != null; }
        }

        /// <summary>
        /// Gets whether the method call is complete (be it with a fault or not).
        /// </summary>
        public virtual bool IsCompleted
        {
            get { return fault.Value != null; }
        }

        public virtual XElement GenerateXml()
        {
            return new XElement(XName.Get(XmlRpcElements.MethodResponseElement),
                HadFault ? new XElement(XName.Get(XmlRpcElements.FaultElement), fault.GenerateXml())
                    : generateParamsXml());
        }

        public bool ParseXml(XElement xElement)
        {
            if (!xElement.Name.LocalName.Equals(XmlRpcElements.MethodResponseElement))
                return false;

            XElement child = xElement.Elements().FirstOrDefault();

            if (child == null || (!child.Name.LocalName.Equals(XmlRpcElements.ParamsElement) && !child.Name.LocalName.Equals(XmlRpcElements.FaultElement)))
                return false;

            switch (child.Name.LocalName)
            {
                case XmlRpcElements.ParamsElement:
                    return parseXml(child);

                case XmlRpcElements.FaultElement:
                    return fault.ParseXml(child.Elements().First());

                default:
                    return false;
            }
        }

        /// <summary>
        /// Resets the method response so IsCompleted is false.
        /// </summary>
        public virtual void Reset()
        {
            fault = null;
        }

        /// <summary>
        /// Generates Xml containing the parameter data for a method response.
        /// <para/>
        /// To be overridden by classes that add more parameters to add theirs.
        /// </summary>
        /// <returns>An XElement containing the parameter data for a method response.</returns>
        protected virtual XElement generateParamsXml()
        {
            return new XElement(XName.Get(XmlRpcElements.ParamsElement));
        }

        protected virtual bool parseXml(XElement xElement)
        {
            return !xElement.HasElements;
        }
    }
}