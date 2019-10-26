using System;
using Client = hackabot.Client.Client;
namespace hackabot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new Client.Client("1006287706:AAE2Tz_bkAz0wW956JYqAEqrfQRg0zk-Cj8", typeof(Commands.StartCommand).Assembly);

            Console.ReadLine();
        }
    }
}