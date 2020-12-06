using NetworkTunnel;
using System;
using System.Net;
using System.Threading;

namespace ChatClientApp
{
  class Program
  {
    const string serverAddress = "192.168.1.50";
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
        INTClient client = new NTClient();

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

        Console.WriteLine("Network tunnel established.\n");

        // Continue until user quits
        while (true)
        {
          // Prompt user for input
          Console.Write("Message: ");
          string userMessage = Console.ReadLine();

          // Ctrl + C breaks the loop
          if (userMessage == null)
          {
            Console.WriteLine("\n");
            break;
          }

          // Send user input to server through network tunnel
          // TODO

          // ?
          // TODO
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