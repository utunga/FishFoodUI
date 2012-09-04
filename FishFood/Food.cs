using System;

namespace FishFood
{
    public class Food
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Val { get; set; }
        public readonly int MaxVal;
        public Food(int initVal, float x, float y)
        {
            MaxVal = initVal;
            Val = initVal;
            X = x;
            Y = y;
        }

        /// <summary>
        /// helper  to give an appropiate scale factor based on size
        /// </summary>
        /// <returns></returns>
        public float Scale 
        {
            get { return (float)Math.Log((float)Val / MaxVal+ 1)*2; }
        }

        public double DistanceToFood(Food food)
        {
            return Math.Sqrt(Math.Pow(this.X - food.X, 2) + Math.Pow(this.Y - food.Y, 2));
        }
    }
}