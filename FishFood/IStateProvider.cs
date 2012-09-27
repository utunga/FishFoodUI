using System.Collections.Generic;

namespace FishFood
{
    public interface IStateProvider
    {
        IEnumerable<Food> FoodNearMe(Fish fish);
    }
}