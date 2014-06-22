using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.MethodCalls
{
    /// <summary>
    /// The abstract base class for method calls without parameters and classes that add parameters.
    /// </summary>
    public abstract class XmlRpcMethodCall
    {
        public abstract string MethodName { get; }

        public virtual XElement GenerateXml()
        {
            return new XElement(XName.Get(XmlRpcElements.MethodCallElement),
                new XElement(XName.Get(XmlRpcElements.MethodNameElement), MethodName),
                generateParamsXml());
        }

        /// <summary>
        /// Generates Xml containing the parameter data for a method call.
        /// <para/>
        /// To be overridden by classes that add more parameters to add theirs.
        /// </summary>
        /// <returns>An XElement containing the parameter data for a method call.</returns>
        protected virtual XElement generateParamsXml()
        {
            return new XElement(XName.Get(XmlRpcElements.ParamsElement));
        }
    }
}