using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XmlRpc.Types;

namespace XmlRpc.Methods
{
    /// <summary>
    /// Abstract base class for method calls that have one parameter and the base classes for those that have more.
    /// </summary>
    /// <typeparam name="TParam1">The XmlRpcType of the parameter.</typeparam>
    /// <typeparam name="TParam1Base">The type of the parameter value.</typeparam>
    /// <typeparam name="TReturn">The returned XmlRpcType.</typeparam>
    /// <typeparam name="TReturnBase">The type of the return value.</typeparam>
    public abstract class XmlRpcMethodCall<TParam1, TParam1Base, TReturn, TReturnBase> : XmlRpcMethodCall<TReturn, TReturnBase>
        where TParam1 : XmlRpcType<TParam1Base>, new()
        where TReturn : XmlRpcType<TReturnBase>, new()
    {
        /// <summary>
        /// Field for the first parameter.
        /// </summary>
        protected TParam1 param1 = new TParam1();

        protected XmlRpcMethodCall(TParam1Base param1)
        {
            this.param1.Value = param1;
        }

        /// <summary>
        /// Generates Xml containing the parameter data.
        /// <para/>
        /// To be overridden by classes that add more parameters to add theirs.
        /// </summary>
        /// <returns>An XElement containing the parameter data.</returns>
        protected override XElement generateCallParamsXml()
        {
            XElement paramsElement = base.generateCallParamsXml();
            paramsElement.Add(makeParamElement(param1));
            return paramsElement;
        }

        /// <summary>
        /// Fills the properties of this method call with the information contained in the XElement.
        /// </summary>
        /// <param name="xElement">The params element storing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected override bool parseCallParamsXml(XElement paramsElement)
        {
            if (!parseCallParamXml(paramsElement, param1))
                return false;

            return base.parseCallParamsXml(paramsElement);
        }
    }
}