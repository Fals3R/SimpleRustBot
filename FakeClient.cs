using Network;
using System;
using System.Collections.Generic;
using System.IO;
using RustBotnet.SteamAuth;

namespace RustBotnet
{
    public class FakeClient : INetworkCryptocraphy, IClientCallback
    {
        private MemoryStream network_buffer = new MemoryStream();

        private Facepunch.Network.Raknet.Client raknet_client;

        private EConnectionStatus connection_status;

        private uint game_version;
        private uint user_index;
        private string username;
        private ulong steam_id;
        private byte[] steam_token;

        private long lastMessageMillis;

        public FakeClient(uint game_version, uint user_index, string username, ulong steam_id)
        {
            this.raknet_client = new Facepunch.Network.Raknet.Client();
            this.raknet_client.cryptography = this;
            this.raknet_client.callbackHandler = this;
            this.game_version = game_version;
            this.user_index = user_index;
            this.username = username;
            this.steam_id = steam_id;
            this.steam_token = Token.CreateToken(this.steam_id);
            this.connection_status = EConnectionStatus.DISCONNECTED;
            this.lastMessageMillis = 0;
        }

        public EConnectionStatus GetConnectionStatus()
        {
            return this.connection_status;
        }

        public long GetLastMessageMillis()
        {
            return this.lastMessageMillis;
        }

        public void SetLastMessageMillis(long millis)
        {
            this.lastMessageMillis = millis;
        }

        public void ConnectToServer(string ip, int port)
        {
            Console.WriteLine("[User {0}] Connecting to {1}:{2}", this.user_index, ip, port);
            this.raknet_client.Connect(ip, port);
            this.connection_status = EConnectionStatus.CONNECTING;
        }

        public void DisconnectFromServer(string reason)
        {
            this.raknet_client.Disconnect(reason, true);
            this.connection_status = EConnectionStatus.DISCONNECTED;
        }

        public void CycleUpdate()
        {
            this.raknet_client.Cycle();
        }

        public void SendChatMessage(string text)
        {
            if (this.raknet_client.write.Start())
            {
                this.raknet_client.write.PacketID(Message.Type.ConsoleCommand);
                this.raknet_client.write.String("chat.say " + text);
                this.raknet_client.write.Send(new SendInfo());
            }
        }

        /* Callbacks */

        public void OnNetworkMessage(Message message)
        {
            if (message.type == Message.Type.RequestUserInformation)
            {
                Console.WriteLine("[User {0}] Sending user information", this.user_index);

                if (this.raknet_client.write.Start())
                {
                    this.raknet_client.write.PacketID(Message.Type.GiveUserInformation);
                    this.raknet_client.write.UInt8(228);
                    this.raknet_client.write.UInt64(this.steam_id);
                    this.raknet_client.write.UInt32(this.game_version);
                    this.raknet_client.write.String("windows");
                    this.raknet_client.write.String(this.username);
                    this.raknet_client.write.String("public");
                    this.raknet_client.write.BytesWithSize(this.steam_token);
                    this.raknet_client.write.Send(new SendInfo());
                }
            }
            else if (message.type == Message.Type.Approved)
            {
                Console.WriteLine("[User {0}] Connection approved", this.user_index);

                if (this.raknet_client.write.Start())
                {
                    this.raknet_client.write.PacketID(Message.Type.Ready);
                    ProtoBuf.ClientReady protobuf_client_ready = new ProtoBuf.ClientReady
                    {
                        clientInfo = new List<ProtoBuf.ClientReady.ClientInfo>()
                    };
                    protobuf_client_ready.WriteToStream(this.raknet_client.write);
                    this.raknet_client.write.Send(new SendInfo());
                }
                message.connection.decryptIncoming = true;
                message.connection.encryptOutgoing = true;
                message.connection.encryptionLevel = 1;

                this.connection_status = EConnectionStatus.CONNECTED;
            }
            else if (message.type == Message.Type.ConsoleCommand)
            {
                Console.WriteLine("[User {0}] ConsoleCommand: {1}", this.user_index, message.read.String());
            }
            else if (message.type == Message.Type.ConsoleMessage)
            {
                Console.WriteLine("[User {0}] ConsoleMessage: {1}", this.user_index, message.read.String());
            }
        }

        public void OnClientDisconnected(string reason)
        {
            this.connection_status = EConnectionStatus.DISCONNECTED;
            Console.WriteLine("[User {0}] Disconnected with reason: {1}", this.user_index, reason);
        }

        public MemoryStream EncryptCopy(Connection connection, MemoryStream stream, int offset)
        {
            this.network_buffer.Position = 0L;
            this.network_buffer.SetLength(0L);
            this.network_buffer.Write(stream.GetBuffer(), 0, offset);
            CryptographyUtils.xor(this.game_version, stream, offset, this.network_buffer, offset);
            return this.network_buffer;
        }

        public MemoryStream DecryptCopy(Connection connection, MemoryStream stream, int offset)
        {
            this.network_buffer.Position = 0L;
            this.network_buffer.SetLength(0L);
            this.network_buffer.Write(stream.GetBuffer(), 0, offset);
            CryptographyUtils.xor(this.game_version, stream, offset, this.network_buffer, offset);
            return this.network_buffer;
        }

        public void Encrypt(Connection connection, MemoryStream stream, int offset)
        {
            CryptographyUtils.xor(this.game_version, stream, offset, stream, offset);
        }

        public void Decrypt(Connection connection, MemoryStream stream, int offset)
        {
            CryptographyUtils.xor(this.game_version, stream, offset, stream, offset);
        }

        public bool IsEnabledIncoming(Connection connection)
        {
            return connection != null && connection.encryptionLevel > 0u && connection.decryptIncoming;
        }

        public bool IsEnabledOutgoing(Connection connection)
        {
            return connection != null && connection.encryptionLevel > 0u && connection.encryptOutgoing;
        }
    }
}
