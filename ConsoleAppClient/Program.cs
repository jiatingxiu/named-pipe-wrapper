using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleAppClient
{
    class Program
    {
        static void Main(string[] args)
        {
            new MyClient("named_pipe_test_server");
        }
    }
}
