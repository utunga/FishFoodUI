using System;
//using FishFood.Quadtree;
using Microsoft.Xna.Framework;

namespace FishFood
{
    public class Food
    {
        public Vector2 Pos { get; set; }
        public float Rotate { get; set; }
        public int Val { get; set; }
        public readonly int MaxVal;
        public Food(int initVal, Vector2 initPos)
        {
            MaxVal = initVal;
            Val = initVal;
            //Pos = new QuadTreePositionItem<Food>(this,initPos,new Vector2(1));//FIXME fix size
            Pos = initPos;
        }

        /// <summary>
        /// helper  to give an appropiate scale factor based on size
        /// </summary>
        /// <returns></returns>
        public float Scale
        {
            get { return Math.Min(1.2f, (float) Math.Log((float) Val/MaxVal + 1, 3.2)); }
        }

        public double DistanceToFood(Food food)
        {
            return Vector2.Distance(this.Pos,food.Pos);
        }

        public double DistanceToFoodSquared(Food food)
        {
            return Vector2.DistanceSquared(this.Pos,food.Pos);
        }
    }
}