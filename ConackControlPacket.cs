namespace MockQTT;

internal class ConackControlPacket : ControlPacket
{
    public ConackControlPacket()
    {
        Type = ControlPacketType.ConnAck;
        Flags = 0;
    }

    public byte SessionPresent { get; set; }
    public byte ReturnCode { get; set; }

    public byte[] Serialize()
    {
        var buffer = new byte[]
        {
            (byte)ControlPacketType.ConnAck << 4,
            2,
            SessionPresent,
            ReturnCode
        };

        return buffer;
    }
}
