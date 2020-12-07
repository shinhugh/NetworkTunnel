using NetworkTunnel;
using System;
using System.Net;
using System.Threading;

namespace ChatClientApp
{
  class Program
  {
    const string serverAddress = "127.0.0.1";
    const int serverPort = 13000;

    static void Main(string[] args)
    {
      try
      {
        // Anticipate Ctrl + C; don't allow it to kill the process
        Console.CancelKeyPress += delegate (object sender,
        ConsoleCancelEventArgs args)
        {
          args.Cancel = true;
        };

        // Create network tunnel client
        INTClient client = new NTClient(2048);

        // Event used to receive notification from NTClient that the connection
        // process has completed (successful or not)
        ManualResetEvent connectCompleteEvent = new ManualResetEvent(false);
        client.ConnectCompleteEvent += delegate (object sender, EventArgs args)
        {
          connectCompleteEvent.Set();
        };

        Console.WriteLine("Initiating connection with server..");

        // Initiate connection to server (non-blocking)
        client.Connect(IPAddress.Parse(serverAddress), serverPort);

        // Block thread until connection process completes
        connectCompleteEvent.WaitOne();

        // Verify that the tunnel was successfully established
        if (client.ConnectionState != ConnectionState.Connected)
        {
          throw new Exception("Unable to establish the network tunnel.");
        }

        // Subscribe to inbox notifications so that incoming messages can be
        // written to the console
        client.IncomingPayloadEvent += delegate (object sender, EventArgs args)
        {
          client.Receive(out byte[] incomingMessage);
          Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
          Console.WriteLine(System.Text.Encoding.ASCII.GetString(incomingMessage, 0, incomingMessage.Length));
        };

        Console.WriteLine("Network tunnel established.\n");
        Console.WriteLine("Send any message:");

        // Continue until user quits or connection is unexpectedly terminated
        while (true)
        {
          // Verify that the connection is still alive
          if (client.ConnectionState != ConnectionState.Connected)
          {
            Console.WriteLine("The connection was unexpectedly terminated.");
            break;
          }

          // Take user input
          string userMessage = Console.ReadLine();

          // Ctrl + C breaks the loop
          if (userMessage == null)
          {
            Console.WriteLine("\n");
            break;
          }

          // Send user input to server through network tunnel
          client.Send(System.Text.Encoding.ASCII.GetBytes(userMessage));
        }

        // Close the tunnel
        client.Disconnect();
      }
      catch (Exception e)
      {
        Console.WriteLine("Error: {0}", e.Message);
      }
      finally
      {
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
      }
    }
  }
}