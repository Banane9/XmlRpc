using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XmlRpc.Types;

namespace XmlRpc.Methods
{
    /// <summary>
    /// Abstract base class for method calls that have two parameters and the base classes for those that have more.
    /// </summary>
    /// <typeparam name="TParam1">The XmlRpcType of the first parameter.</typeparam>
    /// <typeparam name="TParam1Base">The type of the first parameter value.</typeparam>
    /// <typeparam name="TParam2">The XmlRpcType of the second parameter.</typeparam>
    /// <typeparam name="TParam2Base">The type of the second parameter value.</typeparam>
    /// <typeparam name="TReturn">The returned XmlRpcType.</typeparam>
    /// <typeparam name="TReturnBase">The type of the return value.</typeparam>
    public abstract class XmlRpcMethodCall<TParam1, TParam1Base, TParam2, TParam2Base, TReturn, TReturnBase> : XmlRpcMethodCall<TParam1, TParam1Base, TReturn, TReturnBase>
        where TParam1 : XmlRpcType<TParam1Base>, new()
        where TParam2 : XmlRpcType<TParam2Base>, new()
        where TReturn : XmlRpcType<TReturnBase>, new()
    {
        /// <summary>
        /// Field for the second parameter.
        /// </summary>
        protected TParam2 param2 = new TParam2();

        protected XmlRpcMethodCall(TParam1Base param1, TParam2Base param2)
            : base(param1)
        {
            this.param2.Value = param2;
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
            paramsElement.Add(makeParamElement(param2));
            return paramsElement;
        }

        /// <summary>
        /// Fills the properties of this method call with the information contained in the XElement.
        /// </summary>
        /// <param name="xElement">The params element storing the information.</param>
        /// <returns>Whether it was successful or not.</returns>
        protected override bool parseCallParamsXml(XElement paramsElement)
        {
            if (!parseCallParamXml(paramsElement, param2))
                return false;

            return base.parseCallParamsXml(paramsElement);
        }
    }
}