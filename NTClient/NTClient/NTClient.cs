using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetworkTunnel
{
  public class NTClient : INTClient
  {
    // Private member variables

    private ConnectionState _connectionState;

    private readonly TcpClient _tcpClient;

    private NetworkStream _networkStream;

    private readonly Queue<byte[]> _inbox;

    private readonly int _payloadSize;

    // ------------------------------------------------------------

    // Constructors

    public NTClient(int payloadSize)
    {
      _connectionState = ConnectionState.Disconnected;
      _tcpClient = new TcpClient();
      _networkStream = null;
      _inbox = new Queue<byte[]>();
      _payloadSize = payloadSize;
    }

    // ------------------------------------------------------------

    // Interface member variables, properties, and methods

    public event EventHandler ConnectCompleteEvent;

    public event EventHandler IncomingPayloadEvent;

    public ConnectionState ConnectionState
    {
      get
      {
        return _connectionState;
      }
    }

    public int InboxCount
    {
      get
      {
        return _inbox.Count;
      }
    }

    public async Task<bool> Connect(IPAddress serverAddress, int serverPort)
    {
      // Close existing connection if one exists
      if (ConnectionState != ConnectionState.Disconnected)
      {
        Disconnect();
      }

      // Start connecting
      _connectionState = ConnectionState.Connecting;
      Task connectTask = _tcpClient.ConnectAsync(serverAddress, serverPort);
      // Block thread until connection attempt completes
      await connectTask;
      // Check whether the connection was established
      bool success = _tcpClient.Connected;
      if (success)
      {
        // Successful connection
        _networkStream = _tcpClient.GetStream();
        Task updateInboxTask = UpdateInbox();
        _connectionState = ConnectionState.Connected;
      }
      else
      {
        // Unsuccessful connection
        _networkStream = null;
        _connectionState = ConnectionState.Disconnected;
      }

      // Publish event to notify subscribers that the connection process has
      // completed
      ConnectCompleteEvent.Invoke(this, new EventArgs());
      // Return whether the connection was successful
      return success;
    }

    public void Disconnect()
    {
      // Don't do anything if the client is already disconnected
      if (ConnectionState != ConnectionState.Disconnected)
      {
        // Close connection
        _connectionState = ConnectionState.Disconnecting;
        _networkStream = null;
        _tcpClient.Close();
        _connectionState = ConnectionState.Disconnected;
      }
    }

    public async Task<bool> Send(byte[] payload)
    {
      // Verify that the connection is alive
      if (!_tcpClient.Connected)
      {
        Disconnect();
        return false;
      }

      // Send the payload over the connection
      try
      {
        await _networkStream.WriteAsync(payload, 0, payload.Length);
      }
      catch
      {
        Disconnect();
        return false;
      }

      // Return that the sending was successfully initiated
      return true;
    }

    public bool Receive(out byte[] payload)
    {
      if (_inbox.Count == 0)
      {
        payload = new byte[0];
        return false;
      }
      payload = _inbox.Dequeue();
      return true;
    }

    // ------------------------------------------------------------

    // Private helper methods

    private async Task UpdateInbox()
    {
      byte[] payload = new byte[_payloadSize];
      while (true)
      {
        try
        {
          await _networkStream.ReadAsync(payload, 0, payload.Length);
        }
        catch
        {
          Disconnect();
          break;
        }
        _inbox.Enqueue(payload);
        IncomingPayloadEvent.Invoke(this, new EventArgs());
      }
    }
  }
}