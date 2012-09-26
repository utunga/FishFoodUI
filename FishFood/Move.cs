using System;
using Microsoft.Xna.Framework;

namespace FishFood
{
    public class Move
    {
        public Tuple<Food, int> Latch;
        public Latch Eat;
        public Tuple<Latch, Fish> Give;
        public Vector2 MoveTowards;
    }
}