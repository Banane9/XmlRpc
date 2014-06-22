using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlRpc.MethodCalls
{
    /// <summary>
    /// Abstract base class for method calls that don't have parameters and the base classes for those that do.
    /// </summary>
    /// <typeparam name="TCall">The returned XmlRpcType.</typeparam>
    /// <typeparam name="TResponse">The type of the return value.</typeparam>
    public abstract class XmlRpcMethod<TCall, TResponse>
        where TCall : XmlRpcMethodCall, new()
        where TResponse : XmlRpcMethodResponse, new()
    {
        public TCall Call { get; private set; }

        /// <summary>
        /// Gets whether the method call is complete (be it with a fault or not).
        /// </summary>
        public bool IsCompleted
        {
            get { return Response.IsCompleted; }
        }

        public TResponse Response { get; private set; }

        public XmlRpcMethod(TCall call)
        {
            Call = call;
            Response = new TResponse();
        }

        public XmlRpcMethod(TResponse response)
        {
            Response = response;
            Call = new TCall();
        }

        public XmlRpcMethod()
        {
            Call = new TCall();
            Response = new TResponse();
        }

        /// <summary>
        /// Generates the Xml to send to the server for executing the method call.
        /// </summary>
        /// <returns>An XElement containing the method call.</returns>
        public XElement GenerateCallXml()
        {
            return Call.GenerateXml();
        }

        public XElement GenerateResponseXml()
        {
            return Response.GenerateXml();
        }

        /// <summary>
        /// Fills the Returned or Fault information from the given method response data.
        /// <para/>
        /// This makes IsCompleted true and the method call has to be Reset before using this again.
        /// </summary>
        /// <param name="xElement">The XElement containing the method response.</param>
        public bool ParseResponseXml(XElement xElement)
        {
            return Response.ParseXml(xElement);
        }

        /// <summary>
        /// Resets the method call so IsCompleted is false and Returned and Fault are null.
        /// </summary>
        public void Reset()
        {
            Response.Reset();
        }

        public override string ToString()
        {
            return GenerateCallXml().ToString();
        }
    }
}