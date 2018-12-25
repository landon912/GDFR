using System;
using System.Collections.Generic;
using UnityEngine.Networking;

public static class NetworkMessageHelper
{
    public static byte[] NetworkMessageToByteArray(MessageBase message)
    {
        if (message is PlayerAvatarMessage)
        {
            PlayerAvatarMessage playerMess = (PlayerAvatarMessage)message;

            short myMsgType = MsgIndexes.SetupAvatarChanged;

            NetworkWriter writer = new NetworkWriter();

            // You start the message in your writer by passing in the message type.
            // This is a short meaning that it will take up 2 bytes at the start of
            // your message.
            writer.StartMessage(myMsgType);

            // You can now begin your message.
            writer.Write(playerMess.idx);
            writer.Write(playerMess.avatarId);

            // Make sure to end your message with FinishMessage()
            writer.FinishMessage();

            return writer.ToArray();
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public static MessageBase BytesToNetworkMessage<T>(byte[] messageBytes)
    {
        // We will create the NetworkReader using the data from our previous
        // NetworkWriter.
        NetworkReader networkReader = new NetworkReader(messageBytes);

        // The first two bytes in the buffer represent the size
        // of the message. This is equal to the NetworkReader.Length
        // minus the size of the prefix.
        byte[] readerMsgSizeData = networkReader.ReadBytes(2);
        short readerMsgSize = (short)((readerMsgSizeData[1] << 8) + readerMsgSizeData[0]);

        // The message type added in NetworkWriter.StartMessage
        // is to be read now. It is a short and so consists of
        // two bytes. It is the second two bytes on the buffer.
        byte[] readerMsgTypeData = networkReader.ReadBytes(2);
        short readerMsgType = (short)((readerMsgTypeData[1] << 8) + readerMsgTypeData[0]);

        if (typeof(T) == typeof(PlayerAvatarMessage))
        {
            PlayerAvatarMessage message = new PlayerAvatarMessage();

            message.idx = networkReader.ReadInt32();
            message.avatarId = networkReader.ReadInt32();

            return message;
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}


//public static class NetworkMessageSeralizer
//{
//    public static byte[] SeralizeNetworkMessage(MessageBase mess)
//    {
//        using (MemoryStream m = new MemoryStream())
//        {
//            using (BinaryWriter writer = new BinaryWriter(m))
//            {
//                Type messageType = mess.GetType();
//                MemberInfo[] messageMembers = messageType.GetMembers();

//                foreach (MemberInfo member in messageMembers)
//                {
//                    if (member.MemberType == MemberTypes.Field)
//                    {
//                        FieldInfo field = (FieldInfo)member;

//                        Type type = field.FieldType;

//                        writer.Write(ByteConverter.ObjectToByteArray(field.GetValue(mess)));
//                    }
//                }
//            }
//            return m.ToArray();
//        }
//    }

//    public static MessageBase DeseralizeNetworkMessage<T>(byte[] bytes)
//    {
//        List<Object> objects = new List<Object>();

//        using (MemoryStream m = new MemoryStream())
//        {
//            using (BinaryReader reader = new BinaryReader(m))
//            {
//                Type messageType = typeof(T);
//                MemberInfo[] messageMembers = messageType.GetMembers();

//                foreach (MemberInfo member in messageMembers)
//                {
//                    if (member.MemberType == MemberTypes.Field)
//                    {
//                        FieldInfo field = (FieldInfo)member;

//                        Type type = field.FieldType;

//                        if(type == typeof(int))
//                        {
//                            objects.Add(reader.ReadInt32());
//                        }
//                        else if(type == typeof(string))
//                        {
//                            objects.Add(reader.ReadString());
//                        }
//                    }
//                }

//                T returnVal = new T();
//            }
//        }
//    }
//}

public class ClientInfoMessage : MessageBase
{
    public int clientId;
    public string message;
}

public class PlayerConnectInfoMessage : MessageBase
{
    public int clientId;
    public NetworkProfile profile;
}

public class ClientCommandMessage : MessageBase
{
    public short commandId;
    public byte[] message;
}

public class BooleanMessage : MessageBase
{
    public bool value;

    public BooleanMessage() { }

    public BooleanMessage(bool v)
    {
        value = v;
    }

    public override void Deserialize(NetworkReader reader)
    {
        value = reader.ReadBoolean();
    }

    public override void Serialize(NetworkWriter writer)
    {
        writer.Write(value);
    }
}

public class PlayerToggleMessage : MessageBase
{
    public PlayerToggleMessage(int idx, bool isHuman)
    {
        this.idx = idx;
        this.isHuman = isHuman;
    }

    public PlayerToggleMessage() { }

    public int idx;
    public bool isHuman;
}

[Serializable]
public class PlayerAvatarMessage : MessageBase
{
    public PlayerAvatarMessage(int idx, int avatarId)
    {
        this.idx = idx;
        this.avatarId = avatarId;
    }

    public PlayerAvatarMessage() { }

    public int idx;
    public int avatarId;

    public override void Deserialize(NetworkReader reader)
    {
        idx = reader.ReadInt32();
        avatarId = reader.ReadInt32();
    }

    public override void Serialize(NetworkWriter writer)
    {
        writer.Write(idx);
        writer.Write(avatarId);
    }
}

[Serializable]
public class PlayerNameMessage : MessageBase
{
    public PlayerNameMessage(int idx, string playerName)
    {
        this.idx = idx;
        this.playerName = playerName;
    }

    public PlayerNameMessage() { }

    public int idx;
    public string playerName;
}

public class LobbyDataMessage : MessageBase
{
    public LobbyDataMessage(NetworkProfile[] players)
    {
        this.players = players;
    }

    public LobbyDataMessage() { }

    public NetworkProfile[] players;
}

public class DrawCardMessage : MessageBase
{
    public int fromDeckId;
    public int cardId;
    public int toDeckId;

    public DrawCardMessage(Deck fromDeck, Card card, Deck toDeck)
    {
        this.fromDeckId = fromDeck.Id;
        this.cardId = card.Id;
        this.toDeckId = toDeck.Id;
    }

    public DrawCardMessage() { }
}

public class Phase1DrawMessage : MessageBase
{
    public int fromDeck;
    public int[] cardIds;
    public int[] toDeckIds;

    public Phase1DrawMessage(int fromDeck, List<int> cards, List<int> toDecks)
    {
        this.fromDeck = fromDeck;
        cardIds = cards.ToArray();
        toDeckIds = toDecks.ToArray();
    }

    public Phase1DrawMessage () {}
}

public class Phase2DrawMessage : Phase1DrawMessage
{
    public Phase2DrawMessage(int fromDeck, List<int> cards, List<int> toDecks) : base(fromDeck, cards, toDecks) {}

    public Phase2DrawMessage () {}
}

public class ChangeSceneMessage : MessageBase
{
    public string sceneName = "";

    public ChangeSceneMessage(string sceneName)
    {
        this.sceneName = sceneName;
    }

    public ChangeSceneMessage() {}
}