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
        public const string ArrayDataElement = "data";

        public const string ArrayElement = "array";

        public const string Base64Element = "base64";

        public const string BooleanElement = "boolean";

        public const string DateTimeElement = "dateTime.iso8601";

        public const string DoubleElement = "double";

        public const string FaultElement = "fault";

        public const string I4Element = "i4";

        public const string IntElement = "int";

        public const string MethodCallElement = "methodCall";

        public const string MethodNameElement = "methodName";

        public const string MethodResponseElement = "methodResponse";

        public const string ParamElement = "param";

        public const string ParamsElement = "params";

        public const string StringElement = "string";

        public const string StructElement = "struct";

        public const string StructMemberElement = "member";

        public const string StructMemberNameElement = "name";

        public const string ValueElement = "value";
    }
}