using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ConsoleMemoryMappedFile
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 3; i++)
            {
                Test(100000, 32);
                Console.WriteLine();
                Test(50000, 1024);
                Console.WriteLine();
                Test(3000, 32 * 1024);
                Console.WriteLine();
                Test(3000, 640 * 512 * 2);
                Console.WriteLine();
            }

            Console.ReadKey();
        }

        static void Test(int loops, long bufferSize, long size = (long)1024 * 1024 * 1024 * 16)
        {
            long t1, t2, t3, t4, t5;
            string worldFile = Path.GetTempFileName();
            //worldFile.Dump();
            try
            {
                using (FileStream f = File.Create(worldFile))
                {
                    // Try to mark file as sparse, if this fails then we allocate 16GB.
                    if (!MarkAsSparseFile(f))
                        Console.WriteLine("Failed to create world as sparse file.");

                    // Set file length
                    f.SetLength(size);
                    f.Close();
                }

                byte[] buffer = new byte[bufferSize];
                byte[] buffer2 = new byte[bufferSize];

                Stopwatch s = new Stopwatch();
                GC.Collect();
                GC.WaitForFullGCComplete();
                Array.Copy(buffer, buffer2, buffer.Length);
                s.Start();

                for (int i = 0; i < loops; i++)
                    Array.Copy(buffer, buffer2, buffer.Length);

                s.Stop();
                t1 = s.ElapsedTicks;
                Console.WriteLine(string.Format("{0} bytes array copy total: {1} ms. {2} ms x {3}",
                    bufferSize,
                    s.Elapsed.TotalMilliseconds,
                    s.Elapsed.TotalMilliseconds / loops,
                    loops));

                using (MemoryMappedFile m = MemoryMappedFile.CreateFromFile(worldFile))
                {
                    GC.Collect();
                    GC.WaitForFullGCComplete();

                    using (MemoryMappedViewStream a = m.CreateViewStream(1000, buffer.Length, MemoryMappedFileAccess.Read))
                    {
                        a.Read(buffer, 0, buffer.Length);
                        s.Restart();
                        for (int i = 0; i < loops; i++)
                        {
                            a.Seek(0, SeekOrigin.Begin);
                            a.Read(buffer, 0, buffer.Length);
                        }
                    }

                    s.Stop();
                    t2 = s.ElapsedTicks;
                    Console.WriteLine(string.Format("{0} bytes single view stream copy total: {1} ms. {2} ms x {3}",
                        bufferSize,
                        s.Elapsed.TotalMilliseconds,
                        s.Elapsed.TotalMilliseconds / loops,
                        loops));
                }

                using (MemoryMappedFile m = MemoryMappedFile.CreateFromFile(worldFile))
                {
                    GC.Collect();
                    GC.WaitForFullGCComplete();

                    using (MemoryMappedViewStream a = m.CreateViewStream(1000, buffer.Length, MemoryMappedFileAccess.Read))
                    {
                        a.Read(buffer, 0, buffer.Length);
                    }
                    s.Restart();

                    for (int i = 0; i < loops; i++)
                    {
                        using (MemoryMappedViewStream a = m.CreateViewStream(1000, buffer.Length, MemoryMappedFileAccess.Read))
                        {
                            a.Read(buffer, 0, buffer.Length);
                        }
                    }

                    s.Stop();
                    t3 = s.ElapsedTicks;
                    Console.WriteLine(string.Format("{0} bytes multiple view stream copy total: {1} ms. {2} ms x {3}",
                        bufferSize,
                        s.Elapsed.TotalMilliseconds,
                        s.Elapsed.TotalMilliseconds / loops,
                        loops));
                }

                using (MemoryMappedFile m = MemoryMappedFile.CreateFromFile(worldFile))
                {
                    GC.Collect();
                    GC.WaitForFullGCComplete();

                    using (MemoryMappedViewAccessor a = m.CreateViewAccessor(1000, buffer.Length))
                    {
                        a.ReadArray(0, buffer, 0, buffer.Length);
                        s.Restart();
                        for (int i = 0; i < loops; i++)
                        {
                            a.ReadArray(0, buffer, 0, buffer.Length);
                        }
                    }

                    s.Stop();
                    t4 = s.ElapsedTicks;
                    Console.WriteLine(string.Format("{0} bytes single accessor copy total: {1} ms. {2} ms x {3}",
                        bufferSize,
                        s.Elapsed.TotalMilliseconds,
                        s.Elapsed.TotalMilliseconds / loops,
                        loops));
                }

                using (MemoryMappedFile m = MemoryMappedFile.CreateFromFile(worldFile))
                {
                    GC.Collect();
                    GC.WaitForFullGCComplete();
                    using (MemoryMappedViewAccessor a = m.CreateViewAccessor(1000, buffer.Length))
                    {
                        a.ReadArray(0, buffer, 0, buffer.Length);
                    }
                    s.Restart();
                    for (int i = 0; i < loops; i++)
                    {
                        using (MemoryMappedViewAccessor a = m.CreateViewAccessor(1000, buffer.Length))
                        {
                            a.ReadArray(0, buffer, 0, buffer.Length);
                        }
                    }

                    s.Stop();
                    t5 = s.ElapsedTicks;
                    Console.WriteLine(string.Format("{0} bytes multiple accessor copy total: {1} ms. {2} ms x {3}",
                        bufferSize,
                        s.Elapsed.TotalMilliseconds,
                        s.Elapsed.TotalMilliseconds / loops,
                        loops));
                }

                Console.WriteLine();
                Console.WriteLine(string.Format("single vs/array: {0:F3}", (double)t2 / t1));
                Console.WriteLine(string.Format("multi vs/array: {0:F3}", (double)t3 / t1));
                Console.WriteLine(string.Format("multi vs/single vs: {0:F3}", (double)t3 / t2));
                Console.WriteLine();
                Console.WriteLine(string.Format("single ac/array: {0:F3}", (double)t4 / t1));
                Console.WriteLine(string.Format("multi ac/array: {0:F3}", (double)t5 / t1));
                Console.WriteLine(string.Format("multi vs/single vs: {0:F3}", (double)t4 / t5));
                Console.WriteLine();
                Console.WriteLine(string.Format("single ac/single vs: {0:F3}", (double)t4 / t2));
                Console.WriteLine(string.Format("multi ac/multi vs: {0:F3}", (double)t5 / t3));
                Console.WriteLine(new string('-', 80));
            }
            finally
            {
                File.Delete(worldFile);
                //File.Exists(worldFile).Dump();
            }
        }

        // Define other methods and classes here
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool DeviceIoControl(
          SafeFileHandle hDevice,
          int dwIoControlCode,
          IntPtr InBuffer,
          int nInBufferSize,
          IntPtr OutBuffer,
          int nOutBufferSize,
          ref int pBytesReturned,
          [In] ref NativeOverlapped lpOverlapped
        );
        public static bool MarkAsSparseFile(FileStream fileStream)
        {
            int bytesReturned = 0;
            NativeOverlapped lpOverlapped = new NativeOverlapped();
            return DeviceIoControl(
                fileStream.SafeFileHandle,
                590020, //FSCTL_SET_SPARSE,
                IntPtr.Zero,
                0,
                IntPtr.Zero,
                0,
                ref bytesReturned,
                ref lpOverlapped);
        }
    }
}
