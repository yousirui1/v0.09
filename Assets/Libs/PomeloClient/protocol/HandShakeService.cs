using System;
using System.Text;
using SimpleJson;
using System.Net;
using System.Net.Sockets;

namespace Pomelo.DotNetClient
{
    //#发送握手包的负责;
    public class HandShakeService
    {
        private Protocol m_initedProtocol;
        private Action<JsonObject> m_callback;

        public const string Version = "0.3.0";
        public const string Type = "unity-socket";


        public HandShakeService(Protocol protocol)
        {
            this.m_initedProtocol = protocol;
        }

        public void request(JsonObject user, Action<JsonObject> callback)
        {
            byte[] body = Encoding.UTF8.GetBytes(buildMsg(user).ToString());

            m_initedProtocol.send(PackageType.PKG_HANDSHAKE, body);

            this.m_callback = callback;
        }

        internal void invokeCallback(JsonObject data)
        {
            //Invoke the handshake callback
            if (m_callback != null) m_callback.Invoke(data);
        }

        public void ack()
        {
            m_initedProtocol.send(PackageType.PKG_HANDSHAKE_ACK, new byte[0]);
        }

        private JsonObject buildMsg(JsonObject user)
        {
            if (user == null) user = new JsonObject();

            JsonObject msg = new JsonObject();

            //Build sys option
            JsonObject sys = new JsonObject();
            sys["version"] = Version;
            sys["type"] = Type;

            //Build handshake message
            msg["sys"] = sys;
            msg["user"] = user;

            return msg;
        }
    }
}