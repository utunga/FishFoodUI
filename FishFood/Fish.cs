using System;
using System.Collections.Generic;
using System.Linq;

namespace FishFood
{
    public class Fish : Food
    {
        private readonly string _name;
        private readonly IList<Latch> _latchesIOwn;
        private readonly IStateProvider _fishBowl;
       
        public Fish(IStateProvider fishBowl, int initVal, string name, float x, float y) : base(initVal,x,y)
        {
            _fishBowl = fishBowl;
            _name = name;
            _latchesIOwn = new List<Latch>();
        }

        public void Draw()
        {
            System.Console.Out.WriteLine(this.ToString());
        }

        public override string ToString()
        {
            return _name + ":" + Val;
        }

        public void NotifyLatch(Latch latch)
        {
            _latchesIOwn.Add(latch);
        }

        public Move GetMove()
        {
            IList<Latch> deadLatches = _latchesIOwn.Where(x => x.State == LatchState.ToRemove).ToList();
            foreach (Latch latch in deadLatches)
            {
                _latchesIOwn.Remove(latch);
            }

            var move = new Move();
            if (_latchesIOwn.Any(x=>x.State == LatchState.Dormant))
            {
                move.Eat = _latchesIOwn.FirstOrDefault();
                return move;
            }
            var fishNearMe = _fishBowl.FoodNearMe(this);
            if (fishNearMe.Count > 0)
            {
                int which = new Random().Next(fishNearMe.Count);
                move.Latch = new Tuple<Food, int>(fishNearMe[which], fishNearMe[which].Val);
            }
            return move;
        }

    }
}