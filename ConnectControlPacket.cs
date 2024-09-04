using System.Diagnostics;
using System.Text;

namespace MockQTT;

internal class ConnectControlPacket : ControlPacket
{
    private ConnectControlPacket()
    {
        ProtocolName = "MQTT";
        ClientId = string.Empty;
        WillTopic = string.Empty;
        WillMessage = string.Empty;
        UserName = string.Empty;
        Password = string.Empty;
    }

    public string ProtocolName { get; set; }
    public byte ProtocolLevel { get; set; }
    public bool UserNameFlag { get; set; }
    public bool PasswordFlag { get; set; }
    public bool WillRetain { get; set; }
    public byte WillQoS { get; set; }
    public bool WillFlag { get; set; }
    public bool CleanSession { get; set; }
    public ushort KeepAlive { get; set; }

    public string ClientId { get; set; }
    public string WillTopic { get; set; }
    public string WillMessage { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public static ConnectControlPacket Parse(byte[] buffer, int length)
    {
        var packet = new ConnectControlPacket();
        packet.Type = ControlPacketType.Connect;
        packet.Flags = buffer[0] & 0x0F;
        var offset = packet.CalculateLength(buffer, 1);

        Debug.Assert(buffer[offset] == 0x00 && buffer[offset + 1] == 0x04);
        packet.ProtocolName = Encoding.UTF8.GetString(buffer, offset + 2, 4);
        Debug.Assert(packet.ProtocolName == "MQTT");
        packet.ProtocolLevel = buffer[offset + 6];

        packet.UserNameFlag = (buffer[offset + 7] & 0x80) == 0x80;
        packet.PasswordFlag = (buffer[offset + 7] & 0x40) == 0x40;
        packet.WillRetain = (buffer[offset + 7] & 0x20) == 0x20;
        packet.WillQoS = (byte)((buffer[offset + 7] & 0x18) >> 3);
        packet.WillFlag = (buffer[offset + 7] & 0x04) == 0x04;
        packet.CleanSession = (buffer[offset + 7] & 0x02) == 0x02;

        packet.KeepAlive = (ushort)((buffer[offset + 8] << 8) + buffer[offset + 9]);

        var strLen = (buffer[offset + 10] << 8) + buffer[offset + 11];
        packet.ClientId = Encoding.UTF8.GetString(buffer, offset + 12, strLen);
        offset += 12 + strLen;

        if (packet.WillFlag)
        {
            strLen = (buffer[offset] << 8) + buffer[offset + 1];
            packet.WillTopic = Encoding.UTF8.GetString(buffer, offset + 2, strLen);
            offset += 2 + strLen;
            strLen = (buffer[offset] << 8) + buffer[offset + 1];
            packet.WillMessage = Encoding.UTF8.GetString(buffer, offset + 2, strLen);
            offset += 2 + strLen;
        }
        if (packet.UserNameFlag)
        {
            strLen = (buffer[offset] << 8) + buffer[offset + 1];
            packet.UserName = Encoding.UTF8.GetString(buffer, offset + 2, strLen);
            offset += 2 + strLen;
        }
        if (packet.PasswordFlag)
        {
            strLen = (buffer[offset] << 8) + buffer[offset + 1];
            packet.Password = Encoding.UTF8.GetString(buffer, offset + 2, strLen);
            offset += 2 + strLen;
        }

        Debug.Assert(offset == length);

        return packet;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Type: {Type}");
        sb.AppendLine($"Flags: {Flags}");
        sb.AppendLine($"Remaining Length: {RemainingLength}");
        sb.AppendLine($"Protocol Name: {ProtocolName}");
        sb.AppendLine($"Protocol Level: {ProtocolLevel}");
        sb.AppendLine($"User Name Flag: {UserNameFlag}");
        sb.AppendLine($"Password Flag: {PasswordFlag}");
        sb.AppendLine($"Will Retain: {WillRetain}");
        sb.AppendLine($"Will QoS: {WillQoS}");
        sb.AppendLine($"Will Flag: {WillFlag}");
        sb.AppendLine($"Clean Session: {CleanSession}");
        sb.AppendLine($"Keep Alive: {KeepAlive}");
        sb.AppendLine($"Client Id: {ClientId}");
        sb.AppendLine($"Will Topic: {WillTopic}");
        sb.AppendLine($"Will Message: {WillMessage}");
        sb.AppendLine($"Username: {UserName}");
        sb.AppendLine($"Password: {Password}");

        return sb.ToString();
    }
}
