using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FishFood
{
    public interface ISizeInfo
    {
        FRect InitWorldSize { get; }
        Vector2 InitFishSize { get; }
    }

    // eventually get this info from Game
    public class StaticSizeInfo : ISizeInfo
    {
        public const int DEFAULT_WIDTH = 1000;
        public const int DEFAULT_HEIGHT = 1000;
        public const int DEFAULT_FISH_WIDTH = 6;//35;
        public const int DEFAULT_FISH_HEIGHT = 5;//27;

        public FRect InitWorldSize
        {
            get { return new FRect(0, 0, DEFAULT_HEIGHT, DEFAULT_WIDTH); }
        }

        public Vector2 InitFishSize
        {
            get { return new Vector2(DEFAULT_FISH_WIDTH, DEFAULT_FISH_HEIGHT); }
        }

    }
}
