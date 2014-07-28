using System;
using System.Collections.Generic;
using System.Linq;

namespace XmlRpc
{
    /// <summary>
    /// Contains constant names for the various elements that make up the XmlRpc messages.
    /// </summary>
    public static class XmlRpcElements
    {
        /// <summary>
        /// data
        /// </summary>
        public const string ArrayDataElement = "data";

        /// <summary>
        /// array
        /// </summary>
        public const string ArrayElement = "array";

        /// <summary>
        /// base64
        /// </summary>
        public const string Base64Element = "base64";

        /// <summary>
        /// boolean
        /// </summary>
        public const string BooleanElement = "boolean";

        /// <summary>
        /// dateTime.iso8601
        /// </summary>
        public const string DateTimeElement = "dateTime.iso8601";

        /// <summary>
        /// double
        /// </summary>
        public const string DoubleElement = "double";

        /// <summary>
        /// fault
        /// </summary>
        public const string FaultElement = "fault";

        /// <summary>
        /// i4
        /// </summary>
        public const string I4Element = "i4";

        /// <summary>
        /// int
        /// </summary>
        public const string IntElement = "int";

        /// <summary>
        /// methodCall
        /// </summary>
        public const string MethodCallElement = "methodCall";

        /// <summary>
        /// methodName
        /// </summary>
        public const string MethodNameElement = "methodName";

        /// <summary>
        /// methodResponse
        /// </summary>
        public const string MethodResponseElement = "methodResponse";

        /// <summary>
        /// param
        /// </summary>
        public const string ParamElement = "param";

        /// <summary>
        /// params
        /// </summary>
        public const string ParamsElement = "params";

        /// <summary>
        /// string
        /// </summary>
        public const string StringElement = "string";

        /// <summary>
        /// struct
        /// </summary>
        public const string StructElement = "struct";

        /// <summary>
        /// member
        /// </summary>
        public const string StructMemberElement = "member";

        /// <summary>
        /// name
        /// </summary>
        public const string StructMemberNameElement = "name";

        /// <summary>
        /// value
        /// </summary>
        public const string ValueElement = "value";
    }
}