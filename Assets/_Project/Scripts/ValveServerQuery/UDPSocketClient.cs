using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ValveServerQuery
{
    public class UDPSocketClient
    {
        #region Basic Settings
        public string IP { get; private set; }
        public int Port { get; private set; }
        public int ReceiveBufferSize { get; private set; }
        #endregion

        #region Runtime Data
        public Action<ByteBuffer> MessageHandler;

        private byte[] receiveBuffer;
        private Socket clientSocket;
        private Thread messageThread;

        Queue<ByteBuffer> receivedDataQueue = new Queue<ByteBuffer>();
        #endregion

        #region Constructor
        public UDPSocketClient(string ip, int port, int recSize = 4096)
        {
            this.IP = ip;
            this.Port = port;
            this.ReceiveBufferSize = recSize;

            ConnectServer();
        }

        void ConnectServer()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//Valve Server Query use udp protocol
            IPAddress mIp = IPAddress.Parse(IP);
            IPEndPoint ip_end_point = new IPEndPoint(mIp, Port);
            try
            {
                clientSocket.Connect(ip_end_point);//UDP has no connection

                receiveBuffer = new byte[ReceiveBufferSize];

                if (messageThread != null)
                {
                    messageThread.Abort();
                    messageThread = null;
                }
                messageThread = new Thread(RecieveMessage);
                messageThreadFlag = true;
                messageThread.Start();
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region Async Message Processing
        public void AsyncProcessingMessage()
        {
            lock (receivedDataQueue)
            {
                while (receivedDataQueue.Count != 0)
                {
                    ByteBuffer buff = receivedDataQueue.Dequeue();
                    MessageHandler?.Invoke(buff);
                }
            }
        }
        #endregion

        #region Socket Operation
        public void Close()
        {
            messageThreadFlag = false;

            if (clientSocket == null) return;

            clientSocket.Close();
            clientSocket.Dispose();

            clientSocket = null;
        }
        #endregion

        #region Data Sending
        public void SendMessage(string data = "")
        {
            SendMessage(data.ToBytes());
        }

        public void SendMessage(byte[] data)
        {
            try
            {
                clientSocket.Send(data);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.Log("Closing Socket------" + IP + ":" + Port.ToString());
                clientSocket.Close();
                clientSocket.Dispose();
            }
        }
        #endregion

        #region Message Receiving
        bool messageThreadFlag = false;
        /// <summary>
        /// Receive message handling
        /// </summary>
        private void RecieveMessage()
        {
            while (messageThreadFlag)
            {
                try
                {
                    if (clientSocket.Available <= 0)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    if (clientSocket == null) return;
                    clientSocket.Receive(receiveBuffer);
                    ByteBuffer buff = new ByteBuffer(receiveBuffer);

                    lock (receivedDataQueue)
                    {
                        receivedDataQueue.Enqueue(buff);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                    Debug.Log("Closing Socket------" + IP + ":" + Port.ToString());
                    if (clientSocket != null)
                    {
                        lock (clientSocket)
                        {
                            clientSocket.Close();
                            clientSocket.Dispose();
                        }
                    }

                    break;
                }
            }
        }
        #endregion
    }
}
