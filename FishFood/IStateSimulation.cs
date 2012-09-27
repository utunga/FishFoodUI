using System.Collections.Generic;

namespace FishFood
{
    public interface IStateSimulation
    {
        void Init();
        void UpdateState(bool isRunningSlowly);
        IList<Fish> GetState();
        FRect WorldRect { get; }
        bool KeepGoing();
    }
}