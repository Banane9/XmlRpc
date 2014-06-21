using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace XmlRpc
{
    /// <summary>
    /// Represents an XmlRpc Client. Implements the <see cref="XmlRpc.IXmlRpcClient"/> interface.
    /// </summary>
    public sealed class XmlRpcClient : IXmlRpcClient
    {
        private Thread eventDispatcherThread;
        private ConcurrentQueue<Message> messageQueue = new ConcurrentQueue<Message>();
        private Thread receiveLoopThread;
        private uint requestHandle = XmlRpcConstants.ServerCallbackHandle;
        private Stream stream;
        private StreamWriter writer;
        private object writerLock = new object();

        /// <summary>
        /// Gets the configuration of this client.
        /// </summary>
        public Config Configuration { get; private set; }

        public string Name
        {
            get { return Configuration.Address + ":" + Configuration.Port; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRpc.XmlRpcCLient"/> class with the given configuration.
        /// </summary>
        /// <param name="config">The configuration to be used.</param>
        public XmlRpcClient(Config config)
        {
            Configuration = config;
        }

        public void Dispose()
        {
            //writer disposes stream too.
            writer.Dispose();
        }

        /// <summary>
        /// Stops the Threads for receiving data and dispatching events, and closes the connection.
        /// </summary>
        public void EndReceive()
        {
            receiveLoopThread.Abort();
            eventDispatcherThread.Abort();

            //Writer closes and disposes the stream.
            writer.Close();
            writer.Dispose();
        }

        /// <summary>
        /// Sends a method call to the server and returns the request handle associated with the call.
        /// </summary>
        /// <param name="request">The xml formatted call.</param>
        /// <returns>The request handle associated with the call.</returns>
        public uint SendRequest(string request)
        {
            lock (writerLock)
            {
                requestHandle++;

                List<byte> bytes = new List<byte>();
                bytes.AddRange(BitConverter.GetBytes((uint)request.Length));
                bytes.AddRange(BitConverter.GetBytes(requestHandle));

                stream.Write(bytes.ToArray(), 0, bytes.Count);
                stream.Flush();

                writer.Write(request);
                writer.Flush();

                return requestHandle;
            }
        }

        /// <summary>
        /// Connects to the xml rpc server and starts Threads for receiving data from the opened connection and dispatching the appropriate events.
        /// </summary>
        public void StartReceive()
        {
            connect();

            receiveLoopThread = new Thread(receiveLoop);
            receiveLoopThread.Name = Name + " Receive Loop";
            receiveLoopThread.IsBackground = true;

            eventDispatcherThread = new Thread(eventDispatcher);
            eventDispatcherThread.Name = Name + " Event Dispatcher";
            eventDispatcherThread.IsBackground = true;

            receiveLoopThread.Start();
            eventDispatcherThread.Start();
        }

        /// <summary>
        /// Opens a connection to the Address and Port specified in the Config.
        /// </summary>
        private void connect()
        {
            stream = new TcpClient(Configuration.Address, Configuration.Port).GetStream();
            writer = new StreamWriter(stream, Encoding.ASCII);

            byte[] protocolNameLengthBytes = new byte[4];
            stream.Read(protocolNameLengthBytes, 0, 4);
            uint protocolNameLength = BitConverter.ToUInt32(protocolNameLengthBytes, 0);

            if (protocolNameLength != 11)
                throw new Exception("Wrong Low-Level Protocol Header");

            string protocolName = decodeBytes(read(11));

            if (protocolName != "GBXRemote 2")
                throw new Exception("Wrong Low-Level Protocol Version");
        }

        /// <summary>
        /// Takes a byte array and decodes it into a string, based on ASCII character values.
        /// </summary>
        /// <param name="bytes">The string as byte array.</param>
        /// <returns>The decoded string.</returns>
        private string decodeBytes(byte[] bytes)
        {
            char[] chars = new char[bytes.Length];
            int usedBytes;
            int usedChars;
            bool complete;
            Encoding.ASCII.GetDecoder().Convert(bytes, 0, bytes.Length, chars, 0, bytes.Length, true, out usedBytes, out usedChars, out complete);

            return new string(chars);
        }

        private void eventDispatcher()
        {
            Message message;
            while (true)
            {
                if (messageQueue.TryDequeue(out message))
                {
                    if ((message.Handle & XmlRpcConstants.ServerCallbackHandle) == 0)
                    {
                        //Message is a server callback
                        onServerCallback(message.Content);
                    }
                    else
                    {
                        //Message is a method response
                        onMethodResponse(message.Handle, message.Content);
                    }
                }
                else //If it couldn't dequeue a Message, it means there's none waiting.
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Fires the ConnectionDroppedUnexpectedly event.
        /// </summary>
        /// <param name="cause">The Exception that caused the dropping.</param>
        private void onConnectionDroppedUnexpectedly(Exception cause)
        {
            if (ConnectionDroppedUnexpectedly != null)
                ConnectionDroppedUnexpectedly(this, cause);
        }

        /// <summary>
        /// Fires the MethodResponse event.
        /// </summary>
        /// <param name="messageHandle">The request handle of the method response.</param>
        /// <param name="methodResponse">The xml formatted method response string.</param>
        private void onMethodResponse(uint messageHandle, string methodResponse)
        {
            if (MethodResponse != null)
                MethodResponse(this, messageHandle, methodResponse);
        }

        /// <summary>
        /// Fires the ServerCallback event.
        /// </summary>
        /// <param name="serverCallback">The xml formatted server callback string.</param>
        private void onServerCallback(string serverCallback)
        {
            if (ServerCallback != null)
                ServerCallback(this, serverCallback);
        }

        /// <summary>
        /// Reads count number of bytes from the stream and returns them in an array.
        /// </summary>
        /// <param name="count">Number of bytes to read. Also size of the returned array.</param>
        /// <returns>count read bytes.</returns>
        private byte[] read(int count)
        {
            byte[] result = new byte[count];
            int offset = 0;

            while (count - offset > 0)
                offset += stream.Read(result, offset, count - offset);

            return result;
        }

        private void receiveLoop()
        {
            byte[] messageHeaderBytes = new byte[8];
            uint messageLength;
            uint messageHandle;

            while (true)
            {
                try
                {
                    stream.Read(messageHeaderBytes, 0, 8);
                    messageLength = BitConverter.ToUInt32(messageHeaderBytes, 0);
                    messageHandle = BitConverter.ToUInt32(messageHeaderBytes, 4);

                    string messageContent = decodeBytes(read((int)messageLength));

                    messageQueue.Enqueue(new Message(messageHandle, messageContent));
                }
                catch (ThreadAbortException)
                {
                    //If it's a thread abort exception, the connection didn't drop unexpectedly.
                    break;
                }
                catch (Exception ex)
                {
                    //If anything goes wrong with the message reading, it's most likely the connection dropping.
                    onConnectionDroppedUnexpectedly(ex);
                    break;
                }
            }
        }

        /// <summary>
        /// Fires when the connection is dropped unexpectedly.
        /// </summary>
        public event ConnectionDroppedUnexpectedlyEventHandler ConnectionDroppedUnexpectedly;

        /// <summary>
        /// Fires when a MethodResponse is received.
        /// </summary>
        public event MethodResponseEventHandler MethodResponse;

        /// <summary>
        /// Fires when a ServerCallback is received.
        /// </summary>
        public event ServerCallbackEventHandler ServerCallback;

        /// <summary>
        /// Represents a configuration for the XmlRpcClient.
        /// </summary>
        public class Config
        {
            /// <summary>
            /// Gets the address of the server that the client connects to.
            /// </summary>
            public string Address { get; private set; }

            /// <summary>
            /// Gets the port that the client connects to.
            /// </summary>
            public int Port { get; private set; }

            /// <summary>
            /// Creates a new instance of the <see cref="XmlRpc.XmlRpcClient.Config"/> class with the given address and port.
            /// </summary>
            /// <param name="address">The address that the client connects to; Localhost by default.</param>
            /// <param name="port">The port that the client connects to; 5000 by default.</param>
            public Config(string address = "localhost", int port = 5000)
            {
                Address = address;
                Port = port;
            }
        }

        private class Message
        {
            public string Content { get; private set; }

            public uint Handle { get; private set; }

            public Message(uint handle, string content)
            {
                Handle = handle;
                Content = content;
            }
        }
    }
}