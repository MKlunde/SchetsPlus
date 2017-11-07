using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checksum
{
    class checksum
    {
        bool controleren (int x)
        {
            return true;
        }

        static void Main()
        {
            int studentNummer;
            string input;
            Console.WriteLine(DateTime.Now);
            Console.WriteLine("Hallo wat is je studenten nummer?");
            input = Console.ReadLine();
            studentNummer = Int32.Parse(input);
            Console.WriteLine("test");
            Console.ReadKey();
        }
    }
}
