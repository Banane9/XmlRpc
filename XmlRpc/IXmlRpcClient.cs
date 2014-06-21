using System;
using System.Collections.Generic;
using System.Linq;

namespace XmlRpc
{
    /// <summary>
    /// EventHandler for the ConnectionDroppedUnexpectedly event.
    /// </summary>
    /// <param name="sender">The xml rpc client of which the connection dropped.</param>
    public delegate void ConnectionDroppedUnexpectedlyEventHandler(IXmlRpcClient sender, Exception cause);

    /// <summary>
    /// EventHandler for the MethodResponse event.
    /// </summary>
    /// <param name="sender">The xml rpc client that received the method response.</param>
    /// <param name="requestHandle">The handle of the method call that the response is for.</param>
    /// <param name="methodResponse">The xml formatted content of the method response.</param>
    public delegate void MethodResponseEventHandler(IXmlRpcClient sender, uint requestHandle, string methodResponse);

    /// <summary>
    /// EventHandler for the ServerCallback event.
    /// </summary>
    /// <param name="sender">The xml rpc client that reveived the server callback.</param>
    /// <param name="serverCallback">The xml formatted content of the method response.</param>
    public delegate void ServerCallbackEventHandler(IXmlRpcClient sender, string serverCallback);

    /// <summary>
    /// Interface for XmlRpc Clients.
    /// </summary>
    public interface IXmlRpcClient : IDisposable
    {
        /// <summary>
        /// Gets client's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Stop reading data from the interface connection.
        /// </summary>
        void EndReceive();

        /// <summary>
        /// Send a xml formatted request to the XmlRpc interface.
        /// </summary>
        /// <param name="request">The xml formatted request.</param>
        /// <returns>The handle associated with the request.</returns>
        uint SendRequest(string request);

        /// <summary>
        /// Start reading data from the interface connection.
        /// </summary>
        void StartReceive();

        /// <summary>
        /// Fires when the connection drops unexpectedly.
        /// </summary>
        event ConnectionDroppedUnexpectedlyEventHandler ConnectionDroppedUnexpectedly;

        /// <summary>
        /// Fires when a MethodResponse is received.
        /// </summary>
        event MethodResponseEventHandler MethodResponse;

        /// <summary>
        /// Fires when a ServerCallback is received.
        /// </summary>
        event ServerCallbackEventHandler ServerCallback;
    }
}