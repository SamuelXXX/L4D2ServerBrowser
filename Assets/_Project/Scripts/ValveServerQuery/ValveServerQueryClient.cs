using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ValveServerQuery
{
    public enum ValveServerResponseType
    {
        A2S_Info = 0x49,
        A2S_ChallengeNumber = 0x41,
        A2S_Player = 0x44
    }

    public enum ValveServerRequestType
    {
        A2S_Info = 0x54,
        A2S_Player = 0x55
    }

    public class ValveServerResponseData
    {
        #region Definations
        [System.Serializable]
        public class A2S_Info
        {
            public byte protocol;
            public string serverName;
            public string serverMap;
            public string serverFolder;
            public string gameName;
            public short appId;
            public byte players;
            public byte maxPlayers;
            public byte botsCount;
            public byte serverType;
            public byte environment;
            public byte visibility;
            public byte vac;

            public A2S_Info(ByteBuffer byteBuffer)
            {
                //protocol = byteBuffer.ReadByte();  //Left 4 Dead 2 has no such field
                serverName = byteBuffer.ReadString();
                serverMap = byteBuffer.ReadString();
                serverFolder = byteBuffer.ReadString();
                gameName = byteBuffer.ReadString();

                appId = byteBuffer.ReadShort();
                players = byteBuffer.ReadByte();
                maxPlayers = byteBuffer.ReadByte();
                botsCount = byteBuffer.ReadByte();
                serverType = byteBuffer.ReadByte();
                environment = byteBuffer.ReadByte();
                visibility = byteBuffer.ReadByte();
                vac = byteBuffer.ReadByte();
            }
        }

        [System.Serializable]
        public class A2S_ChallengeNumber
        {
            public int challengeNumber;

            public A2S_ChallengeNumber(ByteBuffer byteBuffer)
            {
                challengeNumber = byteBuffer.ReadInt();
            }
        }

        [System.Serializable]
        public class PlayerInfo
        {
            public byte index;
            public string name;
            public int score;
            public float duration;

            public PlayerInfo(ByteBuffer byteBuffer)
            {
                index = byteBuffer.ReadByte();
                name = byteBuffer.ReadString();
                score = byteBuffer.ReadInt();
                duration = byteBuffer.ReadFloat();
            }

        }

        [System.Serializable]
        public class A2S_Player
        {
            public byte players;
            [PowerInspector.PowerList(Key = "name")]
            public List<PlayerInfo> playerInfos = new List<PlayerInfo>();

            public A2S_Player(ByteBuffer byteBuffer)
            {
                players = byteBuffer.ReadByte();

                for (int i = 0; i < players; i++)
                {
                    playerInfos.Add(new PlayerInfo(byteBuffer));
                }
            }
        }
        #endregion

        public byte header;
        public A2S_Info serverInfoBody;
        public A2S_ChallengeNumber challengeNumberBody;
        public A2S_Player playersInfoBody;


        public ValveServerResponseData(ByteBuffer buffer)
        {
            buffer.ReadByte();//0xff
            buffer.ReadByte();//0xff
            buffer.ReadByte();//0xff
            buffer.ReadByte();//0xff

            header = buffer.ReadByte();

            switch ((ValveServerResponseType)header)
            {
                case ValveServerResponseType.A2S_Info:
                    serverInfoBody = new A2S_Info(buffer);
                    break;
                case ValveServerResponseType.A2S_ChallengeNumber:
                    challengeNumberBody = new A2S_ChallengeNumber(buffer);
                    break;
                case ValveServerResponseType.A2S_Player:
                    playersInfoBody = new A2S_Player(buffer);
                    break;
                default: break;
            }
        }
    }


    public class ValveServerQueryClient : UDPSocketClient
    {
        public ValveServerQueryClient(string ip, int port, int recSize = 4096) : base(ip, port, recSize) { }

        public void SendQueryMessage(byte header, int challengeNumber)
        {
            ByteBuffer buff = new ByteBuffer();

            buff.WriteByte(0xff);
            buff.WriteByte(0xff);
            buff.WriteByte(0xff);
            buff.WriteByte(0xff);

            buff.WriteByte(header);

            switch ((ValveServerRequestType)header)
            {
                case ValveServerRequestType.A2S_Info:
                    buff.WriteString("Source Engine Query");
                    break;
                case ValveServerRequestType.A2S_Player:
                    buff.WriteInt(challengeNumber);
                    break;
                default: break;
            }
            SendMessage(buff.ToBytes());
        }
    }

}


