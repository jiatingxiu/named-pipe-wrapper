using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NamedPipeWrapper;
using CommonModel;

namespace ConsoleAppClient
{
    class MyClient
    {
        private bool KeepRunning
        {
            get
            {
                var key = Console.ReadLine();
                client.PushMessage(new MyMessage() { Text = "Client Say:" + key });
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
