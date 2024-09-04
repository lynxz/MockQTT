using System.Net;
using System.Net.Sockets;

namespace MockQTT;

public class Worker : BackgroundService
{
    private const int BUFFER_SIZE = 2048;
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var ipEndPoint = new IPEndPoint(IPAddress.Any, 1883);
        var listener = new TcpListener(ipEndPoint);

        try
        {
            listener.Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync();
                _logger.LogInformation("Client connected");
                var stream = client.GetStream();
                var buffer = new byte[BUFFER_SIZE];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, stoppingToken);
                var packet = Parse(buffer, bytesRead);
                _logger.LogInformation("Packet received:\n {message}", packet.ToString());
                var conack = new ConackControlPacket { SessionPresent = 0, ReturnCode = 0 };
                var response = conack.Serialize();
                await stream.WriteAsync(response, 0, response.Length, stoppingToken);
                _logger.LogInformation("Response sent");
                client.Close();
            }
        }
        finally
        {
            listener.Stop();
        }
    }

    private ControlPacket Parse(byte[] buffer, int length)
    {
        var type = (ControlPacketType)(buffer[0] >> 4);
        switch (type)
        {
            case ControlPacketType.Connect:
                return ConnectControlPacket.Parse(buffer, length);
        }
        throw new InvalidOperationException("Invalid control packet type");
    }
}
