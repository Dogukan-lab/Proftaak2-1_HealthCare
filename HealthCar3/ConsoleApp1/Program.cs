using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Decoder decoder = new Decoder();
            Bluetooth bluetooth = new Bluetooth("Avans Bike AC74", "Avans Bike AC74", decoder);

            Console.Read();
        }
    }
}
