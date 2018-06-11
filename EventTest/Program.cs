using System;
using System.Threading;

namespace EventTest
{
    class Program
    {
        static void Main(string[] args)
        {
            FishingRod fishingRod = new FishingRod();

            FishingMan fishingMan = new FishingMan("yuanweidong") { FishingRod = fishingRod };
            
            while (fishingMan.FishCount < 5)
            {
                fishingMan.Fishing();

                Console.WriteLine("-------------------");

                Thread.Sleep(5000);
            }
        }
    }
}
