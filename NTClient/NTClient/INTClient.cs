using System;
using System.Net;
using System.Threading.Tasks;

namespace NetworkTunnel
{
  public interface INTClient
  {
    // Event publisher that triggers when a connection attempt completes
    // (regardless of success/failure).
    event EventHandler ConnectCompleteEvent;

    // Event publisher that triggers when the inbox receives a new payload.
    event EventHandler IncomingPayloadEvent;

    // Get the state of the connection.
    ConnectionState ConnectionState { get; }

    // Get the number of payloads in the inbox.
    int InboxCount { get; }

    // Initiate an attempt to connect to a server.
    // Non-blocking; starts the connection process and immediately returns.
    // Temporarily puts the client in the connecting state.
    // If a connection already exists, it is closed.
    // Notification of the attempt's completion is published via
    // ConnectCompleteEvent.
    // Returned task evaluates to true upon a successful connection.
    Task<bool> Connect(IPAddress serverAddress, int serverPort);

    // Close the existing connection.
    // If an attempt to connect is ongoing, it is canceled.
    // Does nothing if there is no existing connection or ongoing attempt to
    // connect.
    void Disconnect();

    // Asynchronously send a payload through the active connection.
    // Returned task evaluates to false if there is no existing connection.
    Task<bool> Send(byte[] payload);

    // Extract the next payload from the inbox.
    // Even after a connection is terminated, any payloads that are yet to
    // have been extracted remain in the queue, available for extraction.
    // Returns false if there is no existing connection.
    // Returns false if the inbox is empty.
    bool Receive(out byte[] payload);
  }
}