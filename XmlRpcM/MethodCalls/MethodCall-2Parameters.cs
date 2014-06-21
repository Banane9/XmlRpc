using XmlRpc.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.MethodCalls
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
    public abstract class MethodCall<TParam1, TParam1Base, TParam2, TParam2Base, TReturn, TReturnBase> : MethodCall<TParam1, TParam1Base, TReturn, TReturnBase>
        where TParam1 : XmlRpcType<TParam1Base>, new()
        where TParam2 : XmlRpcType<TParam2Base>, new()
        where TReturn : XmlRpcType<TReturnBase>, new()
    {
        /// <summary>
        /// Field for the second parameter.
        /// </summary>
        protected TParam2 param2 = new TParam2();

        protected MethodCall(TParam1Base param1, TParam2Base param2)
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
        protected override XElement generateParamsXml()
        {
            XElement paramsElement = base.generateParamsXml();
            paramsElement.Add(new XElement(XName.Get(ParamElement), new XElement(XName.Get(ValueElement), param2.GenerateXml())));
            return paramsElement;
        }
    }
}