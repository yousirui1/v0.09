using System;

namespace Pomelo.DotNetClient
{

    public enum ProtocolState
    {
        //#刚打开还没建立通信;
        start = 1,          // Just open, need to send handshaking

        //#通信前的3次握手状态;
        handshaking = 2,    // on handshaking process

        //#连接已建立;
        working = 3,		// can receive and send data 

        //#连接关闭;
        closed = 4,		    // on read body
    }
}