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
        private const string call = @"<methodCall>
  <methodName>system.methodHelp</methodName>
  <params>
    <param>
      <value>
        <string>system.listMethods</string>
      </value>
    </param>
  </params>
</methodCall>";

        private const string response = @"<methodResponse>
  <params>
    <param>
      <value>
        <string></string>
      </value>
    </param>
  </params>
</methodResponse>";

        [TestMethod]
        public void AreRoundTripSave()
        {
            Testing.MethodCalls.AreRoundTripSave((success, type, reason) => Assert.IsTrue(success, type + reason), typeof(SystemListMethods).Assembly);
        }

        [TestMethod]
        public void SerializesCorrectly()
        {
            var methodCall = new SystemMethodHelp("system.listMethods");
            Assert.AreEqual(call, methodCall.GenerateCallXml().ToString());
            Assert.AreEqual(response, methodCall.GenerateResponseXml().ToString());
        }
    }
}