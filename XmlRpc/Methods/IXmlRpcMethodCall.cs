//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Xml.Linq;

//namespace XmlRpc.Methods
//{
//    /// <summary>
//    /// Non-generic base interface for XmlRpcMethodCall-s.
//    /// </summary>
//    public interface IXmlRpcMethodCall
//    {
//        /// <summary>
//        /// Generates the Xml to send to the server for executing the method call.
//        /// </summary>
//        /// <returns>An XElement containing the method call.</returns>
//        XElement GenerateCallXml();

//        /// <summary>
//        /// Generates an XElement storing the information for the method response.
//        /// </summary>
//        /// <returns>The generated XElement.</returns>
//        XElement GenerateResponseXml();

//        /// <summary>
//        /// Fills the parameters of this method call with the information contained in the XElement.
//        /// </summary>
//        /// <param name="xElement">The method call element storing the information.</param>
//        /// <returns>Whether it was successful or not.</returns>
//        bool ParseCallXml(XElement xElement);

//        /// <summary>
//        /// Fills the Returned or Fault information from the given method response data.
//        /// <para/>
//        /// This makes IsCompleted true and the method call has to be Reset before using this again.
//        /// </summary>
//        /// <param name="xElement">The XElement containing the method response.</param>
//        /// <returns>Whether it was successful or not.</returns>
//        bool ParseResponseXml(XElement xElement);
//    }
//}