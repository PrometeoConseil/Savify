using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Savify.Core;

namespace Savify.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            Mp3Downloader audio = new Mp3Downloader("Inifinity 2008", "Guru Josh", @"D:\", "wav");
            audio.Download();
            Console.ReadLine();
        }
    }
}
