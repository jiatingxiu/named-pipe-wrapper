//using Chison.UltraSound.Common.PluginManagement;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.IO.Pipes;
//using System.Linq;
//using System.Text;
//using System.Threading;

//namespace UnitTests
//{
//    [TestFixture]
//    class TestPipe
//    {
//        private const string PipeName = "data_test_pipe";

//        private static int _dataSize = 900 * 720 * 2;
//        private static int _dataCount = 1000;
//        private static Random _random = new Random();
//        private readonly AutoResetEvent _barrier = new AutoResetEvent(false);
//        private readonly AutoResetEvent _barrierGet = new AutoResetEvent(false);
//        private readonly AutoResetEvent _barrierAdd = new AutoResetEvent(false);

//        protected EventWaitHandle WriteWaitEvent;
//        private Stopwatch _Stopwatch;

//        NamedPipeServerStream namedPipeServerStream;
//        NamedPipeClientStream namedPipeClientStream;

//        [SetUp]
//        public void SetUp()
//        {
//            //Mutex _ServerSyncSignal = Mutex.OpenExisting(PipeName + "_ServerSyncSignal");

//            WriteWaitEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "123321_evt_write");
//            _Stopwatch = Stopwatch.StartNew();

//            namedPipeServerStream = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte);
//            namedPipeServerStream.WaitForConnection();

//            namedPipeClientStream = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut);
//            namedPipeClientStream.Connect();
//        }

//        private void _server_ClientConnected(NamedPipeConnection<byte[], byte[]> connection)
//        {
//            throw new NotImplementedException();
//        }

//        private void _client_ServerMessage(NamedPipeConnection<byte[], byte[]> connection, byte[] message)
//        {
//            //message.CopyTo(back, 0);
//            _GetBufferMessage.ToMessage(message);
//            _barrier.Set();
//        }

//        private BufferMessage _BufferMessage;
//        int index = 0;
//        private void _server_ClientMessage(NamedPipeConnection<byte[], byte[]> connection, byte[] message)
//        {
//            Thread.Sleep(100);
//            _BufferMessage.ToMessage(message);
//            switch ((BufferCommandType)_BufferMessage.BufferCommand.Value)
//            {
//                case BufferCommandType.Add:
//                    _BufferMessage.ImageBuffer.Value.CopyTo(dataBack[_BufferMessage.ImageIndex.Value], 0);

//                    break;
//                case BufferCommandType.Get:

//                    byte[] result = dataBack[_BufferMessage.ImageIndex.Value];
//                    result.CopyTo(_BufferMessage.ImageBuffer.Value, 0);
//                    byte[] data = _BufferMessage.ToBuffer();
//                    _server.PushMessage(data);

//                    break;
//                case BufferCommandType.Reset:
//                    break;
//                default:
//                    break;
//            }
//            _barrierAdd.Set();
//            _barrierGet.Set();
//            WriteWaitEvent.Set();
//            //if (message[0] == 255)
//            //{
//            //    _server.PushMessage(dataBack[message[1]]);
//            //}
//            //else
//            //{
//            //    message.CopyTo(dataBack[index], 0);
//            //    index++;
//            //}
//        }

//        static byte[] back;
//        static byte[][] dataBack;
//        static byte[][] allBuffer;

//        [Test]
//        public void CheckAllBackMessage()
//        {
//            int count = 50;

//            back = new byte[_dataSize];
//            dataBack = new byte[count][];

//            for (int i = 0; i < count; i++)
//            {
//                dataBack[i] = new byte[_dataSize];
//            }

//            allBuffer = new byte[count][];

//            for (int i = 0; i < count; i++)
//            {
//                allBuffer[i] = new byte[_dataSize];
//            }

//            for (int i = 0; i < count; i++)
//            {
//                byte[] data = new byte[_dataSize];
//                //_random.NextBytes(data);
//                for (int j = 0; j < data.Length; j++)
//                {
//                    data[j] = (byte)i;
//                }

//                data.CopyTo(allBuffer[i], 0);

//                _client.PushMessage(data);
//            }

//            for (int i = 0; i < count; i++)
//            {
//                //byte[] data2 = new byte[_dataSize];
//                //_random.NextBytes(data);
//                for (int j = 0; j < back.Length; j++)
//                {
//                    back[j] = (byte)i;
//                }
//                back[0] = 255;
//                _client.PushMessage(back);
//                _barrier.WaitOne();


//                Assert.AreEqual(dataBack[i], back, "allBuffer is not equal to back");

//                Assert.AreEqual(allBuffer[i], dataBack[i], "allBuffer is not equal to dataBack");

//                Assert.AreEqual(allBuffer[i], back, "allBuffer is not equal to back");
//            }

//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _client.Stop();
//            _server.Stop();
//        }

//        private BufferMessage _AddBufferMessage;
//        private BufferMessage _GetBufferMessage;
//        private BufferMessage _ResetBufferMessage;
//        [Test]
//        public void TestBufferMessage()
//        {
//            int count = 5;

//            back = new byte[_dataSize];
//            dataBack = new byte[count][];

//            for (int i = 0; i < count; i++)
//            {
//                dataBack[i] = new byte[_dataSize];
//            }

//            //PushMessage to server
//            byte[][] allBuffer = new byte[count][];

//            for (int i = 0; i < count; i++)
//            {
//                allBuffer[i] = new byte[_dataSize];
//            }
//            _BufferMessage = new BufferMessage(0, _dataSize, BufferCommandType.Add);

//            _AddBufferMessage = new BufferMessage(0, _dataSize, BufferCommandType.Add);
//            _GetBufferMessage = new BufferMessage(0, _dataSize, BufferCommandType.Get);
//            _ResetBufferMessage = new BufferMessage(0, _dataSize, BufferCommandType.Reset);

//            for (int i = 0; i < count; i++)
//            {
//                byte[] dataByte = new byte[_dataSize];
//                //_random.NextBytes(dataByte);
//                for (int j = 0; j < dataByte.Length; j++)
//                {
//                    dataByte[j] = (byte)i;
//                }
//                dataByte.CopyTo(allBuffer[i], 0);

//                _AddBufferMessage.SectionImageCount.Value = _dataCount;
//                _AddBufferMessage.SectionIndex.Value = 0;
//                _AddBufferMessage.ImageIndex.Value = i;
//                dataByte.CopyTo(_AddBufferMessage.ImageBuffer.Value, 0);

//                byte[] data = _AddBufferMessage.ToBuffer();
//                _client.PushMessage(data);
//                //WriteWaitEvent.WaitOne(1000);
//                //_barrierAdd.WaitOne();
//            }

//            for (int i = 0; i < count; i++)
//            {
//                //Send request message
//                _GetBufferMessage.SectionImageCount.Value = _dataCount;
//                _GetBufferMessage.SectionIndex.Value = 0;
//                _GetBufferMessage.ImageIndex.Value = i;
//                byte[] data2 = _GetBufferMessage.ToBuffer();
//                _client.PushMessage(data2);

//                //WriteWaitEvent.WaitOne(1000);
//                //_barrierGet.WaitOne();
//                //wait back message
//                _barrier.WaitOne();
//                //Thread.Sleep(50);
//                Assert.AreEqual(allBuffer[i], _GetBufferMessage.ImageBuffer.Value, $"send data is not equal the server back data, data index is {i}");


//            }
//        }
//    }
//}
