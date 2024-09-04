namespace MockQTT;

public enum ControlPacketType
{
    Connect = 1,
    ConnAck = 2,
    Publish = 3,
    PubAck = 4,
    PubRec = 5,
    PubRel = 6,
    PubComp = 7,
    Subscribe = 8,
    SubAck = 9,
    Unsubscribe = 10,
    UnsubAck = 11,
    PingReq = 12,
    PingResp = 13,
    Disconnect = 14
}

internal abstract class ControlPacket
{
    public ControlPacketType Type { get; set; }
    public int Flags { get; set; }
    public int RemainingLength { get; set; }

    public int CalculateLength(byte[] buffer, int offset)
    {
        var i = 0;
        var value = 0;
        var multiplier = 1;
        do
        {
            value += (buffer[offset + i] & 0x7F) * multiplier;
            multiplier *= 128;
            i++;
        } while ((buffer[offset + i] & 0x80) != 0);

        RemainingLength = value;
        return i + 1;
    }
}
