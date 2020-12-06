using System;
using System.Net;
using System.Threading.Tasks;

namespace NetworkTunnel
{
  public class NTClient : INTClient
  {
    // Private member variables

    ConnectionState _connectionState;

    // ------------------------------------------------------------

    // Constructors

    public NTClient()
    {
      _connectionState = ConnectionState.Disconnected;
    }

    // ------------------------------------------------------------

    // Interface member variables, properties, and methods

    public event EventHandler ConnectCompleteEvent;

    public ConnectionState ConnectionState
    {
      get
      {
        return _connectionState;
      }
    }

    public void Connect(IPAddress serverAddress, int serverPort)
    {
      // TODO
      Task mockConnectionAttempt = TriggerAfterDelay();
    }

    public void Disconnect()
    {
      // TODO
      _connectionState = ConnectionState.Disconnected;
    }

    // ------------------------------------------------------------

    // Private helper methods

    private async Task TriggerAfterDelay()
    {
      await Task.Delay(3000);
      _connectionState = ConnectionState.Connected;
      ConnectCompleteEvent(this, new EventArgs());
    }
  }
}