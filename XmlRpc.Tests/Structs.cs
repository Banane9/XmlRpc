using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using XmlRpc.Types.Structs;

namespace XmlRpc.Tests
{
    [TestClass]
    public class Structs
    {
        [TestMethod]
        public void AreRoundTripSave()
        {
            Testing.Structs.AreRoundTripSave((success, type, reason) => Assert.IsTrue(success, type + reason), typeof(BaseStruct).Assembly);
        }
    }
}