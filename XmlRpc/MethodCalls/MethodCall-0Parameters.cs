using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XmlRpc.Types;
using XmlRpc.Types.Structs;

namespace XmlRpc.MethodCalls
{
    /// <summary>
    /// Abstract base class for method calls that don't have parameters and the base classes for those that do.
    /// </summary>
    /// <typeparam name="TReturn">The returned XmlRpcType.</typeparam>
    /// <typeparam name="TReturnBase">The type of the return value.</typeparam>
    public abstract class MethodCall<TReturn, TReturnBase> where TReturn : XmlRpcType<TReturnBase>, new()
    {
        public const string ElementName = "methodCall";
        public const string FaultElement = "fault";
        public const string MethodNameElement = "methodName";
        public const string MethodResponseElement = "methodResponse";
        public const string ParamElement = "param";
        public const string ParamsElement = "params";
        public const string ValueElement = "value";

        /// <summary>
        /// Backing field for the Returned property.
        /// </summary>
        protected TReturn returned;

        /// <summary>
        /// Backing field for the Fault property.
        /// </summary>
        private XmlRpcStruct<FaultStruct> fault;

        /// <summary>
        /// Gets the fault information when IsCompleted is true, and HadFault is true, or null otherwise.
        /// </summary>
        public FaultStruct Fault
        {
            get { return fault != null ? fault.Value : null; }
        }

        /// <summary>
        /// Gets whether there was a fault with the method call or not.
        /// </summary>
        public bool HadFault
        {
            get { return fault != null; }
        }

        /// <summary>
        /// Gets whether the method call is complete (be it with a fault or not).
        /// </summary>
        public bool IsCompleted
        {
            get { return fault != null || returned != null; }
        }

        /// <summary>
        /// Gets the name of the method this call is for.
        /// </summary>
        public abstract string MethodName { get; }

        /// <summary>
        /// Gets the returned information when IsCompleted is true, and HadFault is false, or the default otherwise.
        /// </summary>
        public TReturnBase ReturnValue
        {
            get { return returned != null ? returned.Value : default(TReturnBase); }
        }

        /// <summary>
        /// Generates the Xml to send to the server for executing the method call.
        /// </summary>
        /// <returns>An XElement containing the method call.</returns>
        public XElement GenerateXml()
        {
            return new XElement(XName.Get(ElementName),
                new XElement(XName.Get(MethodNameElement), MethodName),
                generateParamsXml());
        }

        /// <summary>
        /// Fills the Returned or Fault information from the given method response data.
        /// <para/>
        /// This makes IsCompleted true and the method call has to be Reset before using this again.
        /// </summary>
        /// <param name="xElement">The XElement containing the method response.</param>
        public void ParseXml(XElement xElement)
        {
            if (IsCompleted)
                throw new Exception("MethodCall was completed already. Use Reset before filling it again.");

            if (!xElement.Name.LocalName.Equals(MethodResponseElement))
                throw new ArgumentException("Element has to have the name " + MethodResponseElement, "xElement");

            XElement child = xElement.Elements().FirstOrDefault();

            if (child == null || (!child.Name.LocalName.Equals(ParamsElement) && !child.Name.LocalName.Equals(FaultElement)))
                throw new FormatException("Child of " + MethodResponseElement + " has to be " + ParamsElement + " or " + FaultElement);

            XElement value = child.Element(ParamElement).Element(ValueElement);

            if (value == null)
                throw new FormatException("Child of " + MethodResponseElement + " has to have a " + ValueElement + " child.");

            switch (child.Name.LocalName)
            {
                case ParamsElement:
                    returned = new TReturn();
                    returned.ParseXml(getValueContent(value, returned.ElementName));
                    break;

                case FaultElement:
                    fault = new XmlRpcStruct<FaultStruct>();
                    fault.ParseXml(getValueContent(value, fault.ElementName));
                    break;

                default:
                    throw new FormatException("Unexpected child with name " + child.Name.LocalName);
            }
        }

        /// <summary>
        /// Resets the method call so IsCompleted is false and Returned and Fault are null.
        /// </summary>
        public void Reset()
        {
            fault = null;
            returned = null;
        }

        public override string ToString()
        {
            return GenerateXml().ToString();
        }

        /// <summary>
        /// Generates Xml containing the parameter data.
        /// <para/>
        /// To be overridden by classes that add more parameters to add theirs.
        /// </summary>
        /// <returns>An XElement containing the parameter data.</returns>
        protected virtual XElement generateParamsXml()
        {
            return new XElement(XName.Get(ParamsElement));
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
    }
}