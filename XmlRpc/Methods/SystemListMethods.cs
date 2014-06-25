using System;
using System.Collections.Generic;
using System.Linq;
using XmlRpc.Types;

namespace XmlRpc.Methods
{
    /// <summary>
    /// Represents a call to the system.listMethods method.
    /// </summary>
    public sealed class SystemListMethods : XmlRpcMethodCall<XmlRpcArray<XmlRpcString, string>, XmlRpcString[]>
    {
        /// <summary>
        /// Gets the name of the method this call is for.
        /// </summary>
        public override string MethodName
        {
            get { return "system.listMethods"; }
        }
    }
}