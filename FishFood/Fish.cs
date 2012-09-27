using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace FishFood
{
    public class Fish : Food
    {
        private readonly string _name;
        private readonly IList<Latch> _latchesIOwn;
        private readonly IStateProvider _fishBowl;
       
        public Fish(IStateProvider fishBowl, int initVal, string name, Vector2 initPos, Vector2 initSize) : base(initVal,initPos, initSize)
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

        public Latch ExistingLatch(Food target)
        {
            return _latchesIOwn.FirstOrDefault(x => x.Target == target);
        }

        public Move GetMove()
        {
            IList<Latch> deadLatches = _latchesIOwn.Where(x => x.State == LatchState.ToRemove).ToList();
            foreach (Latch latch in deadLatches)
            {
                _latchesIOwn.Remove(latch);
            }

            var move = new Move();
            move.MoveTowards = default(Vector2);
            IList<Latch> eatable = new List<Latch>(_latchesIOwn.Where(x => x.State == LatchState.Dormant || x.State == LatchState.Activated));
            if (eatable.Any())
            {
                Latch chosen = eatable.Any(x => this.DistanceToFood(x.Target) < this.Scale*50) ? // switch target if food is close
                    eatable.OrderBy(x => this.DistanceToFoodSquared(x.Target)).First() :
                    eatable.OrderByDescending(x => x.Target.Val).First();
                if (chosen.State != LatchState.Activated)
                    move.Eat = chosen;
                move.MoveTowards = chosen.Target.Pos;
            }

            // if not currently hunting something, find a new target
            if (!_latchesIOwn.Any(x => x.State==LatchState.Activated))
            {
                var fishNearMe = _fishBowl.FoodNearMe(this);
                int nearMeCount = fishNearMe.Count();
                if (nearMeCount > 0)
                {
                    int which = new Random().Next(nearMeCount);
                    Food target = fishNearMe.Skip(which - 1).Take(1).First();
                    
                    if (!_latchesIOwn.Any(x => x.Target == target) && target != this)
                    {
                        move.Latch = new Tuple<Food, int>(target, target.Val);
                    }
                    move.MoveTowards = target.Pos;
                }
            }
            return move;
        }

    }
}