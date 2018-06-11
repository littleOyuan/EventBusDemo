using System;
using System.Collections.Generic;
using System.Text;


namespace EventTest
{
    public enum FishType
    {
        鲫鱼,
        鲤鱼,
        黑鱼,
        青鱼,
        草鱼,
        鲈鱼
    }

    public class FishingMan
    {
        public string Name { get; set; }
        public int FishCount { get; set; }

        public FishingRod FishingRod { get; set; }

        public FishingMan(string name)
        {
            Name = name;
        }

        public void Fishing()
        {
            FishingRod.ThrowHook(this);
        }
    }
}
