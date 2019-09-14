using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NamedPipeWrapper;
using CommonModel;
using System.Threading;

namespace ConsoleAppClient
{
    class MyClient
    {
        private bool KeepRunning
        {
            get
            {
                var key = Console.ReadLine();
                if (key == "Q")
                    return false;
                
                for (int i = 0; i < 10000; i++)
                {
                    client.PushMessage(new MyMessage() { Text = i.ToString() });
                }
                while (client.Count != 0)
                {
                    Thread.Sleep(50);
                    Console.WriteLine("=====================================" + client.Count);
                }
                return true;
            }
        }
        NamedPipeClient<MyMessage> client;
        public MyClient(string pipeName)
        {
            client = new NamedPipeClient<MyMessage>(pipeName);
            client.ServerMessage += OnServerMessage;
            client.Error += OnError;
            client.Start();
            while (KeepRunning)
            {
                // Do nothing - wait for user to press 'q' key
            }
            client.Stop();
        }

        private void OnServerMessage(NamedPipeConnection<MyMessage, MyMessage> connection, MyMessage message)
        {
            Console.WriteLine("Server says: {0}", message);
        }

        private void OnError(Exception exception)
        {
            Console.Error.WriteLine("ERROR: {0}", exception);
        }
    }
}
