using Chison.UltraSound.Common.PluginManagement;
using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace UnitTestsServer
{
    class Program
    {
        static void Main(string[] args)
        {
            new ServerTest();

            Console.ReadKey();
        }
    }

    class ServerTest
    {
        private const string PipeName = "data_test_pipe";
        private static NamedPipeServer<byte[]> _server;
        private static int _dataSize = 900 * 720 * 2;
        private static int _dataCount = 5;
        private static Random _random = new Random();
        private readonly AutoResetEvent _barrier = new AutoResetEvent(false);

        public ServerTest()
        {
            //Mutex _ServerSyncSignal = new Mutex(false, PipeName + "_ServerSyncSignal");

            dataBack = new byte[_dataCount][];

            for (int i = 0; i < _dataCount; i++)
            {
                dataBack[i] = new byte[_dataSize];
            }

            _server = new NamedPipeServer<byte[]>(PipeName);
            _server.ClientMessage += _server_ClientMessage;
            _server.ClientConnected += _server_ClientConnected;
            _server.ClientDisconnected += _server_ClientDisconnected;
            _server.Start();

            Console.WriteLine("A+ _BufferMessage.ImageIndex.Value");

            _BufferMessage = new BufferMessage(0, _dataSize, BufferCommandType.Add);
        }

        private void _server_ClientDisconnected(NamedPipeConnection<byte[], byte[]> connection)
        {
            Console.WriteLine("_server_ClientDisconnected");
        }

        private void _server_ClientConnected(NamedPipeConnection<byte[], byte[]> connection)
        {
            Console.WriteLine("_server_ClientConnected");
        }

        static byte[][] dataBack;
        private BufferMessage _BufferMessage;
        int index = 0;
        private void _server_ClientMessage(NamedPipeConnection<byte[], byte[]> connection, byte[] message)
        {
            Thread.Sleep(100);
            _BufferMessage.ToMessage(message);
            switch ((BufferCommandType)_BufferMessage.BufferCommand.Value)
            {
                case BufferCommandType.Add:
                    _BufferMessage.ImageBuffer.Value.CopyTo(dataBack[_BufferMessage.ImageIndex.Value], 0);
                    Console.WriteLine("Add ImageIndex===========================" + _BufferMessage.ImageIndex.Value);
                    break;
                case BufferCommandType.Get:

                    byte[] result = dataBack[_BufferMessage.ImageIndex.Value];
                    result.CopyTo(_BufferMessage.ImageBuffer.Value, 0);
                    byte[] data = _BufferMessage.ToBuffer();
                    _server.PushMessage(data);
                    Console.WriteLine("Get ImageIndex===========================" + _BufferMessage.ImageIndex.Value);
                    break;
                case BufferCommandType.Reset:
                    break;
                default:
                    break;
            }
            Console.WriteLine("_server_ClientMessage");
        }
    }
}
