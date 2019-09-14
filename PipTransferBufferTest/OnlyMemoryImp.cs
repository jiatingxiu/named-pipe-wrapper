using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PipTransferBufferTest
{
    public class OnlyMemoryImp
    {
        private int _ImageDataLength = 3000;
        private uint _OneFrameSize = 960 * 720 * 2;
        private byte[] _OneFrameBuffer;

        private byte[][] list = new byte[100][];
        private List<byte[][]> listAll = new List<byte[][]>();
        private int wai = 0;
        private int nei = 0;

        public OnlyMemoryImp()
        {
            for (int i = 0; i < 30; i++)
            {
                list = new byte[100][];
                for (int j = 0; j < list.Length; j++)
                {
                    list[j] = new byte[_OneFrameSize];
                }
                listAll.Add(list);
            }
        }

        public bool AddData(byte[] dataByte)
        {
            if (nei == 100)
            {
                nei = 0;
                wai++;
            }
            if (wai == 30)
            {
                wai = 0;
            }

            Array.Copy(dataByte, 0, listAll[wai][nei], 0, dataByte.Length);
            nei++;
            return true;
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public byte[] GetData(int index)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }

}
