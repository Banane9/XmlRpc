﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace XmlRpc.Types
{
    /// <summary>
    /// Represents an XmlRpcType containing an int.
    /// </summary>
    public class XmlRpcI4 : XmlRpcInt
    {
        /// <summary>
        /// The name of Elements of this type.
        /// </summary>
        public override string ElementName
        {
            get { return "i4"; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ManiaNet.XmlRpc.Types.XmlRpcI4"/> class with Value set to the default value for int.
        /// </summary>
        public XmlRpcI4()
            : base()
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="ManiaNet.XmlRpc.Types.XmlRpcI4"/> class with the given value.
        /// </summary>
        /// <param name="value">The int encapsulated by this.</param>
        public XmlRpcI4(int value)
            : base(value)

        { }
    }
}