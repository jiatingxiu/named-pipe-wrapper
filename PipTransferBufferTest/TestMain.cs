using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipTransferBufferTest
{
    internal class TestMain
    {
        private OnlyMemoryImp _OnlyMemoryImp = new OnlyMemoryImp();
        private Random _Random = new Random();
        private byte[] _imagebyte;

        private NamedPipeClient<byte[]> namedPipeClient;
        private NamedPipeServer<byte[]> namedPipeServer;

        public TestMain()
        {
            InitServer();
            InitClient();

            _imagebyte = new byte[960 * 720 * 2];


            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(45);
                    Console.WriteLine("1");
                    _Random.NextBytes(_imagebyte);
                    namedPipeClient.PushMessage(_imagebyte);
                    //GC.Collect();
                }
            });

            
        }

        private void InitClient()
        {
            namedPipeClient = new NamedPipeClient<byte[]>("Pip_Lotus_Buffer");
            namedPipeClient.ServerMessage += NamedPipeClient_ServerMessage;
            namedPipeClient.Start();
        }

        private void NamedPipeClient_ServerMessage(NamedPipeConnection<byte[], byte[]> connection, byte[] message)
        {
            
        }

        private void InitServer()
        {
            namedPipeServer = new NamedPipeServer<byte[]>("Pip_Lotus_Buffer");
            namedPipeServer.ClientMessage += NamedPipeServer_ClientMessage;
            namedPipeServer.Start();
        }

        private void NamedPipeServer_ClientMessage(NamedPipeConnection<byte[], byte[]> connection, byte[] message)
        {
            _OnlyMemoryImp.AddData(message);
        }
    }
}
