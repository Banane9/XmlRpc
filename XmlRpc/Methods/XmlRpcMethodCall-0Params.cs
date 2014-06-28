using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XmlRpc.Types;
using XmlRpc.Types.Structs;

namespace XmlRpc.Methods
{
    /// <summary>
    /// Abstract base class for method calls that don't have parameters and the base classes for those that do.
    /// </summary>
    /// <typeparam name="TReturn">The returned XmlRpcType.</typeparam>
    /// <typeparam name="TReturnBase">The type of the return value.</typeparam>
    public abstract class XmlRpcMethodCall<TReturn, TReturnBase>// : IXmlRpcMethodCall
        where TReturn : XmlRpcType<TReturnBase>, new()
    {
        /// <summary>
        /// Backing field for the Returned property.
        /// </summary>
        protected TReturn returned = new TReturn();

        /// <summary>
        /// Backing field for the Fault property.
        /// </summary>
        private XmlRpcStruct<FaultStruct> fault = new XmlRpcStruct<FaultStruct>();

        /// <summary>
        /// Gets or sets the fault information. May be the default of <see cref="XmlRpc.Types.Structs.FaultStruct"/>.
        /// </summary>
        public FaultStruct Fault
        {
            get { return fault.Value; }
            set { fault.Value = value; }
        }

        /// <summary>
        /// Gets whether there was a fault with the method call or not.
        /// </summary>
        public bool HadFault
        {
            get { return fault.Value != default(FaultStruct); }
        }

        /// <summary>
        /// Gets the name of the method this call is for.
        /// </summary>
        public abstract string MethodName { get; }

        /// <summary>
        /// Gets or sets the return value of the call. May be the default of TReturnBase.
        /// </summary>
        public TReturnBase ReturnValue
        {
            get { return returned.Value; }
            set { returned.Value = value; }
        }

        /// <summary>
        /// Generates the Xml to send to the server for executing the method call.
        /// </summary>
        /// <returns>An XElement containing the method call.</returns>
        public XElement GenerateCallXml()
        {
            return new XElement(XName.Get(XmlRpcElements.MethodCallElement),
                new XElement(XName.Get(XmlRpcElements.MethodNameElement), MethodName),
                generateCallParamsXml());
        }

        /// <summary>
        /// Generates an XElement storing the information for the method response.
        /// </summary>
        /// <returns>The generated XElement.</returns>
        public XElement GenerateResponseXml()
        {
            return new XElement(XName.Get(XmlRpcElements.MethodResponseElement),
                HadFault ? new XElement(XName.Get(XmlRpcElements.FaultElement), fault.GenerateXml())
                    : new XElement(XName.Get(XmlRpcElements.ParamsElement),
                        new XElement(XName.Get(XmlRpcElements.ParamElement),
                            returned.GenerateXml())));
        }

        /// <summary>
        /// Fills the parameters of this method call with the information contained in the XElement.
        /// </summary>
        /// <param name="xElement">The method call element storing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        public bool ParseCallXml(XElement xElement)
        {
            if (!xElement.Name.LocalName.Equals(XmlRpcElements.MethodCallElement)
             || !xElement.HasElements
             || xElement.Elements().Count() != 2)
                return false;

            XElement methodNameElement = xElement.Elements().First();

            if (!methodNameElement.Name.LocalName.Equals(XmlRpcElements.MethodNameElement)
             || methodNameElement.IsEmpty
             || !methodNameElement.Value.Equals(MethodName))
                return false;

            methodNameElement.Remove();
            XElement paramsElement = xElement.Elements().First();

            if (!paramsElement.Name.LocalName.Equals(XmlRpcElements.ParamsElement))
                return false;

            var reversedParams = paramsElement.Elements().Reverse().ToArray();
            paramsElement.RemoveAll();
            paramsElement.Add(reversedParams);

            return parseCallParamsXml(paramsElement);
        }

        /// <summary>
        /// Fills the Returned or Fault information from the given method response data.
        /// <para/>
        /// This makes IsCompleted true and the method call has to be Reset before using this again.
        /// </summary>
        /// <param name="xElement">The XElement containing the method response.</param>
        /// <returns>Whether it was successful or not.</returns>
        public bool ParseResponseXml(XElement xElement)
        {
            if (!xElement.Name.LocalName.Equals(XmlRpcElements.MethodResponseElement) || xElement.Elements().Count() != 1)
                return false;

            XElement child = xElement.Elements().First();

            if (child.Elements().Count() != 1 || (!child.Name.LocalName.Equals(XmlRpcElements.ParamsElement) && !child.Name.LocalName.Equals(XmlRpcElements.FaultElement)))
                return false;

            XElement value = child.Elements().First().Elements().First();

            if (value == null)
                return false;

            switch (child.Name.LocalName)
            {
                case XmlRpcElements.ParamsElement:
                    if (!returned.ParseXml(value))
                        return false;
                    break;

                case XmlRpcElements.FaultElement:
                    if (!fault.ParseXml(value))
                        return false;
                    break;

                default:
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return GenerateCallXml().ToString();
        }

        /// <summary>
        /// Generates Xml containing the call parameter data.
        /// <para/>
        /// To be overridden by classes that add more call parameters to add theirs.
        /// </summary>
        /// <returns>An XElement containing the call parameter data.</returns>
        protected virtual XElement generateCallParamsXml()
        {
            return new XElement(XName.Get(XmlRpcElements.ParamsElement));
        }

        protected bool isValidParamElement(XElement xElement)
        {
            return xElement != null
                && xElement.Name.LocalName.Equals(XmlRpcElements.ParamElement)
                && xElement.Elements().Count() == 1
                && xElement.Elements().First().Name.LocalName.Equals(XmlRpcElements.ValueElement);
        }

        /// <summary>
        /// Creates a param-Element with the given XmlRpcType's value as content.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The XmlRpcType to be wrapped.</param>
        /// <returns>A param-Element containing the value as content.</returns>
        protected XElement makeParamElement<T>(XmlRpcType<T> value)
        {
            return new XElement(XName.Get(XmlRpcElements.ParamElement), value.GenerateXml());
        }

        /// <summary>
        /// Fills the properties of this method call with the information contained in the XElement.
        /// </summary>
        /// <param name="xElement">The params element storing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected virtual bool parseCallParamsXml(XElement paramsElement)
        {
            return !paramsElement.HasElements;
        }

        protected bool parseCallParamXml<T>(XElement paramsElement, XmlRpcType<T> param)
        {
            if (!paramsElement.HasElements)
                return false;

            XElement paramElement = paramsElement.Elements().First();

            if (!isValidParamElement(paramElement))
                return false;

            paramElement.Remove();

            return param.ParseXml(paramElement.Elements().First());
        }
    }
}