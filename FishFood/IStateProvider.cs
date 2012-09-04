using System.Collections.Generic;

namespace FishFood
{
    public interface IStateProvider
    {
        IList<Food> FoodNearMe(Fish fish);
    }
}