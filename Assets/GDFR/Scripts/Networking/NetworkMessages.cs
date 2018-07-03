using UnityEngine.Networking;

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
}

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