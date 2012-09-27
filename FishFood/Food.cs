using System;
//using FishFood.Quadtree;
using System.Collections.Generic;
using FishFood.Quadtree;
using Microsoft.Xna.Framework;

namespace FishFood
{
    public class Food
    {
        //private const float DEFAULT_AREA_NEAR_ME = 10;
        //private float _sizeOfAreaNearMe;
        private readonly Vector2 _initSize;
        private QuadTreePositionItem<Food> _indexedPos;
        
        public float Rotate { get; set; }

        private int _initVal; 
        private int _val;
        private int _lastVal;
        public int Val
        {
            get { return _val; }
            set
            {
                _val = value;
                if (_lastVal!=_val)
                    UpdateSize();
                _lastVal = _val;
            }
        }

        public float Scale
        {
            get { return ScaleFromValue(Val, _initVal); }
        }

        public QuadTreePositionItem<Food> QuadTreePosition
        {
            get { return _indexedPos; }
        }

        public Vector2 Pos
        {
            get
            {
                return _indexedPos.Position;
            }
            set
            {
                _indexedPos.Position = value;
            }
        }
 
        public Food(int initVal, Vector2 initPos, Vector2 initSize)
        {
            //FIXME order of these initializations is painfully fragile
            _initVal = initVal;
            _initSize = initSize;
            //_sizeOfAreaNearMe = DEFAULT_AREA_NEAR_ME;
            _indexedPos = new QuadTreePositionItem<Food>(this, initPos, initSize);
            Val = initVal;
        }

        //public void MakeAreaNearMeBigger()
        //{
        //    _sizeOfAreaNearMe = (float) (_sizeOfAreaNearMe*1.1);
        //}
        //
        //public void MakeAreaNearMeSmaller()
        //{
        //    _sizeOfAreaNearMe = (float)(_sizeOfAreaNearMe * .9);
        //}
        //
        //public Rectangle AreaNearMe()
        //{
        //    Vector2 nearFishTopLeft = Pos + new Vector2(-1, -1) * Scale * _sizeOfAreaNearMe;
        //    Vector2 nearFishBottomRight = Pos + new Vector2(+1, +1) * Scale * _sizeOfAreaNearMe;
        //    return new FRect(nearFishTopLeft, nearFishBottomRight);
        //}

        private void UpdateSize()
        {
            // hopefully this updates the quadtree data as well
            _indexedPos.Size = _initSize * ScaleFromValue(Val, _initVal);
        }

        public double DistanceToFood(Food food)
        {
            return Vector2.Distance(this.Pos,food.Pos);
        }

        public double DistanceToFoodSquared(Food food)
        {
            return Vector2.DistanceSquared(this.Pos,food.Pos);
        }

        /// <summary>
        /// helper to give an appropiate scale factor based on value of fish
        /// </summary>
        /// <returns></returns>
        static SortedList<float, float> _memo = new SortedList<float, float>();
        private static float ScaleFromValue(float val, float maxVal)
        {
            if (_memo.ContainsKey(val))
                return _memo[val];

            float scale = Math.Min(1.2f, (float) Math.Log((float) val/maxVal + 1, 8.2));
            _memo.Add(val, scale);
            //foreach (KeyValuePair<float, float> pair in _memo)
            //{
            //    Console.Out.WriteLine(pair.Key + "," + pair.Value);
            //}
            //Console.Out.WriteLine(" -------------------- ");
            return scale;
        }
    }
}