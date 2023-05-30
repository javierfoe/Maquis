using System;
using System.Collections.Generic;

public class Action
{
    public readonly bool PathToSafeHouseRequired;
    private readonly ResourceAmount[] _requirements;
    private readonly Reward _reward;
    private readonly Location _location;

    public Action(Location location, ResourceAmount[] requirements, Reward reward, bool safeHouse = true,
        bool fixer = false)
    {
        _location = location;
        _requirements = fixer ? ResourceAmount.AddFixerRequirement(requirements) : requirements;
        _reward = reward;
        PathToSafeHouseRequired = safeHouse;
    }

    public Action(Location location, ResourceAmount reward) : this(location, reward.Resource, reward.Amount)
    {
    }

    public Action(Location location, Resource reward, int amount = 1, bool fixer = false, bool airDrop = false,
        bool safeHouse = true) : this(location, null, new Reward(new[] { new ResourceAmount(reward, amount) }, airDrop),
        safeHouse, fixer)
    {
    }

    public bool AreRequirementsMet()
    {
        return (!PathToSafeHouseRequired || Maquis.CheckPathSafeHouse(_location)) &&
               (_requirements == null || Maquis.HasResources(_requirements)) &&
               (!_reward.AirDrop || Maquis.IsFieldAvailableForAirDrop());
    }

    public void PerformAction()
    {
        if (_requirements != null)
        {
            Maquis.SpendResources(_requirements);
        }

        Maquis.GainReward(_reward, _location);
    }
}