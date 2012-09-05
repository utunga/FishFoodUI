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
            IList<Latch> eatable = new List<Latch>(_latchesIOwn.Where(x => x.State == LatchState.Dormant || x.State == LatchState.Activated));
            if (eatable.Any())
            {
                Latch chosen = eatable.Any(x => this.DistanceToFood(x.Target) < 20) ?
                    eatable.OrderBy(x => this.DistanceToFood(x.Target)).First():
                    eatable.OrderByDescending(x => x.Target.Val).First();
                move.MoveTowards = new Tuple<float, float>(chosen.Target.X, chosen.Target.Y); 
                if (chosen.State != LatchState.Activated)
                    move.Eat = chosen;
                return move;
            }

            var fishNearMe = _fishBowl.FoodNearMe(this);
            if (fishNearMe.Count > 0)
            {
                int which = new Random().Next(fishNearMe.Count);
                Food target = fishNearMe[which];
                if (!_latchesIOwn.Any(x => x.Target == target) && target!=this)
                {
                    move.Latch = new Tuple<Food, int>(target, target.Val);
                    move.MoveTowards = new Tuple<float, float>(target.X, target.Y);
                }
            }
            return move;
        }

    }
}