using System;

namespace Pomelo.DotNetClient
{

    public enum TransportState
    {
        //#头一般存放的是报文大小;
        readHead = 1,		// on read head
        readBody = 2,		// on read body
        closed = 3			// connection closed, will ignore all the message and wait for clean up
    }
}