using System;
using System.Collections.Generic;
using System.Linq;

namespace FishFood
{
    public class FishBowl : IStateProvider, IStateSimulation
    {
        private const int INIT_FISH_VAL = 10;
        private const int NUM_FISH = 200;
        private readonly int _maxHeight;
        private readonly int _maxWidth;
        private readonly IList<Fish> _fishes;
        private readonly IList<Latch> _latches;
        public const int DEFAULT_WIDTH = 1000;
        public const int DEFAULT_HEIGHT = 1000;

        public FishBowl(int width,int height)
        {
            _maxWidth = width;
            _maxHeight = height; 
            _fishes = new List<Fish>();
            _latches = new List<Latch>();
        }

        public FishBowl()
            : this(DEFAULT_WIDTH, DEFAULT_HEIGHT) {}
        
        #region IStateProvider

        public IList<Food> FoodNearMe(Fish fish)
        {
            return new List<Food>(_fishes.AsParallel().Where(x => x.DistanceToFood(fish) < 50).OrderByDescending(x => x.DistanceToFood(fish)).Take(10));
        }

        #endregion

        #region IStateSimulation

        public void Init()
        {
            Random rnd = new Random();
            for (int i = 0; i < NUM_FISH; i++)
            {
                float x = rnd.Next(_maxHeight);
                float y = rnd.Next(_maxWidth);
                _fishes.Add(new Fish(this, INIT_FISH_VAL, "fish_" + i,x,y));
            }
        }

        public IList<Food> GetState()
        {
            return new List<Food>(_fishes);
        }

        public void UpdateState()
        {
            GetFishMoves();
            ResolveMoves();
            UpdateFishStates();
        }

        public bool KeepGoing()
        {
            return _fishes.Any();
        }

        #endregion


        #region private helper methods

        private void UpdateFishStates()
        {
            // eat 
            foreach (Latch eatRequest in _latches.Where(x=>x.State==LatchState.Activated))
            {
                int hunkEaten= Math.Min(eatRequest.Target.Val, eatRequest.Val);
                eatRequest.Eater.Val += hunkEaten;
                eatRequest.Target.Val -= hunkEaten;
                eatRequest.State = LatchState.ToRemove;
            }

            // starve all fish a little
            foreach (Fish fish in _fishes)
            {
                fish.Val = fish.Val - 1;
            }
            
            // remove dead fish
            IList<Fish> deadFish = _fishes.Where(x => x.Val <= 0).ToList();
            foreach (Fish fish in deadFish)
            {
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
            if (_latches.Any(x => x.Eater == eater && x.Target == target))
            {
                throw new ApplicationException("Latch already exists in some state");
            }
            var newLatch = new Latch(eater, target, val);
            _latches.Add(newLatch);
            newLatch.Eater.NotifyLatch(newLatch);
        }

        private Latch ExistingLatch(Fish eater, Food target)
        {
            return _latches.FirstOrDefault(x => x.Eater == eater && x.Target == target);
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
                    else
                    {
                        DoBattleAndActivateWinner(latchRequest, counterRequest);
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
                if (nextMove.Latch!=null)
                {
                    AddLatch(fish, nextMove.Latch.Item1, nextMove.Latch.Item2);
                }

                if (nextMove.Eat != null)
                {
                    nextMove.Eat.State = LatchState.RequestActivation;
                }
            }
        }

        #endregion

        #region methods for command line usage

        public void Run()
        {
            Init();
            while (KeepGoing())
            {
                UpdateState();
                Draw();
            }
        }

        private void Draw()
        {
            Console.Out.WriteLine("------------------------------------");
            foreach (var fish in _fishes)
            {
                fish.Draw();
            }
            Console.Out.WriteLine("------------------------------------");
        }

        #endregion


    }
}