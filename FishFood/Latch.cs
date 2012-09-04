namespace FishFood
{
    public class Latch
    {
        public readonly Fish Eater;
        public readonly Food Target;
        public readonly int Val;
        public LatchState State;

        public Latch(Fish eater, Food target, int val)
        {
            State = LatchState.Dormant;
            Eater = eater;
            Target = target;
            Val = val;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Latch other = (Latch) obj;
            return Target == other.Target &&
                   Eater == other.Eater &&
                   Val == other.Val;
        }

        public override int GetHashCode()
        {
            return ((397 ^ Target.GetHashCode()) * 397 ^ Eater.GetHashCode()) + Val;
        }
    }
}