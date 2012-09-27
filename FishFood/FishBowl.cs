using System;
using System.Collections.Generic;
using System.Linq;
using FishFood.Quadtree;
using Microsoft.Xna.Framework;

namespace FishFood
{
    public class FishBowl : IStateProvider, IStateSimulation
    {
        private const int INIT_FISH_VAL = 1000;
        private const int NUM_FISH = 800;
        private FRect _worldRect;
        private Vector2 _initFishSize;
        private QuadTree<Food> _fishPositions;
        private List<Fish> _fishes;
        private IList<Latch> _latches;
  
        public FishBowl(ISizeInfo sizeProvider)
        {
            _initFishSize = sizeProvider.InitFishSize;
            _worldRect = sizeProvider.InitWorldSize;
        }

        public FishBowl()
            : this(new StaticSizeInfo()) {}
        
        #region IStateProvider

        public IEnumerable<Food> FoodNearMe(Fish fish)
        {
            return _fishPositions.NearByItems(fish.QuadTreePosition, 5).Select(x => x.Parent);

            //.GetAllItems(ref nearEnoughToConsider);
            //if (nearEnoughToConsider.Count < 3)
            //    fish.MakeAreaNearMeBigger();
            //else if (nearEnoughToConsider.Count > 6)
            //    fish.MakeAreaNearMeSmaller();
            //return new List<Food>(nearEnoughToConsider.Where(near => fish != near.Parent).Select(near => near.Parent).AsParallel().OrderBy(x => x.DistanceToFoodSquared(fish)).Take(5));
        }

        #endregion

        #region IStateSimulation

        public void Init()
        {
            _latches = new List<Latch>();
            Random rnd = new Random();
            _fishPositions = new QuadTree<Food>(_worldRect, 5);
            _fishes = new List<Fish>();
            _latches = new List<Latch>();
            for (int i = 0; i < NUM_FISH; i++)
            {
                float x = rnd.Next((int)_worldRect.Height);
                float y = rnd.Next((int)_worldRect.Width);
                var fish = new Fish(this, INIT_FISH_VAL, "fish_" + i, new Vector2(x, y), _initFishSize);
                _fishes.Add(fish);
                _fishPositions.Insert(fish.QuadTreePosition);
            }
        }

        public IList<Fish> GetState()
        {
            return _fishes;
        }

        public FRect WorldRect
        {
            get { return _worldRect; }
        }

        public void UpdateState(bool isRunningSlowly)
        {
            GetFishMoves();
            ResolveMoves();
            UpdateFishStates();
        }

        public bool KeepGoing()
        {
            return _fishes.Count > 3;
        }

        #endregion

        #region private helper methods

        private void UpdateFishStates()
        {
            // eat 
            foreach (Latch eatRequest in _latches.Where(x=>x.State==LatchState.Activated))
            {
                if (eatRequest.Eater.DistanceToFood(eatRequest.Target) < eatRequest.Eater.Scale*2)
                {
                    int hunkEaten = Math.Min(Math.Min(eatRequest.Target.Val, eatRequest.Val), eatRequest.Eater.Val/2);
                    eatRequest.Eater.Val += hunkEaten;
                    eatRequest.Target.Val -= hunkEaten;
                    eatRequest.State = LatchState.ToRemove;
                }
            }

            //// starve all fish a little (randomly choose fish to starve each time - so they dont starve too fast)
            foreach (Fish fish in _fishes)
            {
                fish.Val = fish.Val - (int) Math.Floor(fish.Val*0.008) - 1;
            }

            // remove dead fish
            IList<Fish> deadFish = _fishes.Where(x => x.Val <= 0).ToList();
            foreach (Fish fish in deadFish)
            {
                fish.QuadTreePosition.Delete();
                _fishes.Remove(fish);
            }

            // remove dead latches
            IList<Latch> deadLatches = _latches.Where(x => x.State == LatchState.ToRemove).ToList();
            foreach (Latch latch in deadLatches)
            {
                _latches.Remove(latch);
            }
        }

        private void AddLatch(Fish eater, Food target, int val)
        {
            //if (_latches.Any(x => x.Eater == eater && x.Target == target))
            //{
            //    throw new ApplicationException("Latch already exists in some state");
            //}
            var newLatch = new Latch(eater, target, val);
            _latches.Add(newLatch);
            newLatch.Eater.NotifyLatch(newLatch);
        }

        private Latch ExistingLatch(Fish eater, Food target)
        {
            return eater.ExistingLatch(target);
        }

        private void ResolveMoves()
        {
            IList<Latch> requests = _latches.Where(x => x.State == LatchState.RequestActivation).ToList();
            while (requests.Any())
            {
                foreach (Latch latchRequest in requests)
                {
                    Fish eater = latchRequest.Eater; 
                    Food target = latchRequest.Target;
                    Latch counterRequest = (target is Fish) ? ExistingLatch((Fish)target, eater) : null; 
                    if (counterRequest==null) {
                        //  unopposed request
                        latchRequest.State = LatchState.Activated;
                    }
                    else if (eater.Val>0 && target.Val>0)
                    {
                        DoBattleAndActivateWinner(latchRequest, counterRequest);
                    }
                    else
                    {
                        latchRequest.State = LatchState.ToRemove;
                    }
                }
                requests = _latches.Where(x => x.State == LatchState.RequestActivation).ToList();
            }
        }

        private void DoBattleAndActivateWinner(Latch latchRequest, Latch counterRequest)
        {

            Random rnd = new Random();
            float rollOne = rnd.Next(0, latchRequest.Eater.Val) - rnd.Next(0, latchRequest.Val);
            float rollTwo = rnd.Next(0, counterRequest.Eater.Val) - rnd.Next(0, counterRequest.Val);
            if (rollOne > rollTwo)
            {
                latchRequest.State = LatchState.Activated;
                counterRequest.State = LatchState.ToRemove;
            }
            else
            {
                latchRequest.State = LatchState.ToRemove;
                counterRequest.State = LatchState.Activated;
            }
        }

        private void GetFishMoves()
        {
            foreach (Fish fish in _fishes)
            {
                 Move nextMove = fish.GetMove();
                 if (nextMove.Latch != null)
                 {
                     AddLatch(fish, nextMove.Latch.Item1, nextMove.Latch.Item2);
                 }

                 if (nextMove.Eat != null)
                 {
                     nextMove.Eat.State = LatchState.RequestActivation;
                 }

                 if (nextMove.MoveTowards != default(Vector2))
                 {
                     //float opposite = (nextMove.MoveTowards.X - fish.Pos.X);
                     //float adjacent = (nextMove.MoveTowards.Y - fish.Pos.Y);
                     //double angle =  (float) Math.Atan2(adjacent, opposite);
                     //fish.Pos.X = fish.X + (float)Math.Cos(angle) * fish.Scale * 3; //bigger fish move faster
                     //fish.Pos.Y = fish.Y + (float)Math.Sin(angle) * fish.Scale * 3;
                     if (!fish.Pos.Equals(nextMove.MoveTowards))
                     {
                         Vector2 direction = nextMove.MoveTowards - fish.Pos;
                         direction.Normalize();
                         fish.Pos = fish.Pos + direction*fish.Scale*3;
                         fish.Rotate = (float) Math.Atan2(direction.Y, direction.X);
                     }

                 }
             }
        }

        #endregion

        #region methods for command line usage

        //public void Run()
        //{
        //    Init();
        //    while (KeepGoing())
        //    {
        //        UpdateState();
        //        Draw();
        //    }
        //}

        //private void Draw()
        //{
        //    Console.Out.WriteLine("------------------------------------");
        //    foreach (var fish in _fishes)
        //    {
        //        fish.Draw();
        //    }
        //    Console.Out.WriteLine("------------------------------------");
        //}

        #endregion


    }
}