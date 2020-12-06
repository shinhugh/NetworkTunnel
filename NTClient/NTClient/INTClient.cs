using System;
using System.Net;

namespace NetworkTunnel
{
  public interface INTClient
  {
    // Event publisher that triggers when a connection attempt completes
    // (regardless of success/failure).
    event EventHandler ConnectCompleteEvent;

    // Get the state of the connection.
    ConnectionState ConnectionState { get; }

    // Initiate an attempt to connect to a server.
    // Non-blocking; starts the connection process and immediately returns.
    // Temporarily puts the client in the connecting state.
    // If a connection already exists, it is closed.
    // Notification of the attempt's completion is published via
    // ConnectCompleteEvent.
    void Connect(IPAddress serverAddress, int serverPort);

    // Close the existing connection.
    // Does nothing if there is no existing connection.
    void Disconnect();
  }
}