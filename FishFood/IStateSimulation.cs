using System.Collections.Generic;

namespace FishFood
{
    public interface IStateSimulation
    {
        void Init();
        void UpdateState();
        IList<Food> GetState();
        bool KeepGoing();
    }
}