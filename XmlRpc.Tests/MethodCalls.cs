using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using XmlRpc.Methods;

namespace XmlRpc.Tests
{
    [TestClass]
    public class MethodCalls
    {
        [TestMethod]
        public void AreRoundTripSave()
        {
            Testing.MethodCalls.AreRoundTripSave((success, type, reason) => Assert.IsTrue(success, type + reason), typeof(SystemListMethods).Assembly);
        }
    }
}