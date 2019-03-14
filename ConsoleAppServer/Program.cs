using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleAppServer
{
    class Program
    {
        static void Main(string[] args)
        {
            new MyServer("named_pipe_test_server");
        }
    }
}
