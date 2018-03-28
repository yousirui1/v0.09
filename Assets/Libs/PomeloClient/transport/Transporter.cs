using System;
using System.Net.Sockets;

namespace Pomelo.DotNetClient
{
    class StateObject
    {
        public const int BufferSize = 1024;
        internal byte[] m_buffer = new byte[BufferSize];
    }

    //#通过socket收发数据;
    public class Transporter
    {
        public const int HeadLength = 4;

        //#通过这个socket来传输数据;
        private Socket m_initedSocket;

        //#监听器; 消息处理器; 策略模式???
        private Action<byte[]> m_initedMessageProcesser;

        //Used for get message
        private StateObject m_stateObject = new StateObject();
        private TransportState m_transportState;
        private IAsyncResult m_asyncReceive;
        private IAsyncResult m_asyncSend;
        //#正在发送;
        private bool m_onSending = false;
        //#正在接收;
        private bool m_onReceiving = false;
        //#头的4个字节;
        private byte[] m_headBuffer = new byte[4];
        private byte[] m_buffer;
        //#从buffer的哪里开始写;
        private int m_bufferOffset = 0;
        private int m_pkgLength = 0;

        //#断开连接回调;
        internal Action m_onDisconnect = null;

        //private TransportQueue<byte[]> _receiveQueue = new TransportQueue<byte[]>();
        private System.Object m_lock = new System.Object();

        public Transporter(Socket socket, Action<byte[]> processer)
        {
            this.m_initedSocket = socket;
            this.m_initedMessageProcesser = processer;
            m_transportState = TransportState.readHead;
        }

        public void start()
        {
            this.receive();
        }

        public void send(byte[] buffer)
        {
            if (this.m_transportState != TransportState.closed)
            {
                //string str = "";
                //foreach (byte code in buffer)
                //{
                //    str += code.ToString();
                //}
                //Console.WriteLine("send:" + buffer.Length + " " + str.Length + "  " + str);
                //#发送1024个字节;

				//buffer = CryptUtils.Encrypt3DesBytes("0123456789abcd0123456789", buffer);
				//Log.d<Transporter>("buffer: " + BitConverter.ToString(buffer));

                this.m_asyncSend = m_initedSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(sendCallback), m_initedSocket);

                this.m_onSending = true;
            }
        }

        private void sendCallback(IAsyncResult asyncSend)
        {
            //UnityEngine.Debug.Log("sendCallback " + this.transportState);
            if (this.m_transportState == TransportState.closed) return;
            m_initedSocket.EndSend(asyncSend);
            this.m_onSending = false;
        }

        public void receive()
        {
            //Console.WriteLine("receive state : {0}, {1}", this.transportState, socket.Available);
            //#读取1024个字节;
            this.m_asyncReceive = m_initedSocket.BeginReceive(m_stateObject.m_buffer, 0, m_stateObject.m_buffer.Length, SocketFlags.None, new AsyncCallback(endReceive), m_stateObject);
            this.m_onReceiving = true;
        }

        internal void close()
        {
            this.m_transportState = TransportState.closed;
            /*try{
                if(this.onReceiving) socket.EndReceive (this.asyncReceive);
                if(this.onSending) socket.EndSend(this.asyncSend);
            }catch (Exception e){
                Console.WriteLine(e.Message);
            }*/
        }

        //#接收结束;
        private void endReceive(IAsyncResult asyncReceive)
        {
            if (this.m_transportState == TransportState.closed)
                return;
            StateObject state = (StateObject)asyncReceive.AsyncState;
            Socket socket = this.m_initedSocket;

            try
            {
                int length = socket.EndReceive(asyncReceive);

                this.m_onReceiving = false;

                if (length > 0)
                {
					//Log.d<Transporter>("d buffer1: " + BitConverter.ToString(state.m_buffer));
					//state.m_buffer = CryptUtils.Decrypt3DesBytes("0123456789abcd0123456789", state.m_buffer);
					//Log.d<Transporter>("d buffer2: " + BitConverter.ToString(state.m_buffer));
                    processBytes(state.m_buffer, 0, length);
                    //Receive next message
                    if (this.m_transportState != TransportState.closed) receive();
                }
                else
                {
                    //#没有数据了, 就是断开连接了;
                    if (this.m_onDisconnect != null) this.m_onDisconnect();
                }

            }
            catch (System.Net.Sockets.SocketException)
            {
                if (this.m_onDisconnect != null)
                    this.m_onDisconnect();
            }
        }

        internal void processBytes(byte[] bytes, int startIDX, int endIDXPlus1)
        {
            if (this.m_transportState == TransportState.readHead)
            {
                readHead(bytes, startIDX, endIDXPlus1);
            }
            else if (this.m_transportState == TransportState.readBody)
            {
                readBody(bytes, startIDX, endIDXPlus1);
            }
        }

        //#读取数组中的offset~limit部分的字节;
        private bool readHead(byte[] bytes, int startIDX, int endIDXPlus1)
        {
            int length = endIDXPlus1 - startIDX;
            int headNum = HeadLength - m_bufferOffset;

            //#实际的长度比头长度大;
            if (length >= headNum)
            {
                //Write head buffer
                writeBytes(bytes, startIDX, headNum, m_bufferOffset, m_headBuffer);
                //Get package length
                m_pkgLength = (m_headBuffer[1] << 16) + (m_headBuffer[2] << 8) + m_headBuffer[3];

                //Init message buffer
                m_buffer = new byte[HeadLength + m_pkgLength];
                writeBytes(m_headBuffer, 0, HeadLength, m_buffer); //写出头到数据buffer中;
                startIDX += headNum;
                m_bufferOffset = HeadLength;
                this.m_transportState = TransportState.readBody;

                if (startIDX <= endIDXPlus1) processBytes(bytes, startIDX, endIDXPlus1);
                return true;
            }
            else
            {
                //从向m_headBuffer中写;
                writeBytes(bytes, startIDX, length, m_bufferOffset, m_headBuffer);
                m_bufferOffset += length;
                return false;
            }
        }

        private void readBody(byte[] bytes, int startIDX, int endIDXPlus1)
        {
            int length = m_pkgLength + HeadLength - m_bufferOffset;
            if ((startIDX + length) <= endIDXPlus1)
            {
                writeBytes(bytes, startIDX, length, m_bufferOffset, m_buffer);
                startIDX += length;
                //Invoke the protocol api to handle the message
                this.m_initedMessageProcesser.Invoke(m_buffer);
                this.m_bufferOffset = 0;
                this.m_pkgLength = 0;

                //#一个packet读完了;
                if (this.m_transportState != TransportState.closed)
                    this.m_transportState = TransportState.readHead;

                //#继续读下一个包;
                if (startIDX < endIDXPlus1)
                    processBytes(bytes, startIDX, endIDXPlus1);
            }
            else
            {
                writeBytes(bytes, startIDX, endIDXPlus1 - startIDX, m_bufferOffset, m_buffer);
                m_bufferOffset += endIDXPlus1 - startIDX;
                this.m_transportState = TransportState.readBody;
            }
        }

        private void writeBytes(byte[] source, int srcStart, int length, byte[] target)
        {
            writeBytes(source, srcStart, length, 0, target);
        }

        private void writeBytes(byte[] source, int srcStart, int length, int targetStart, byte[] target)
        {
            for (int i = 0; i < length; i++)
            {
                target[targetStart + i] = source[srcStart + i];
            }
        }

        private void print(byte[] bytes, int offset, int length)
        {
            for (int i = offset; i < length; i++)
                Console.Write(Convert.ToString(bytes[i], 16) + " ");
            Console.WriteLine();
        }
    }
}