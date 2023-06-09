using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Maquis
{
    public static readonly UnityEvent<ResourcesDto> ResourcesEvent = new ();
    
    private static readonly Field[] Fields = new Field[2];
    private static readonly Dictionary<Resource, int> Resources = new();
    private static readonly Dictionary<Spot, Location> Locations = new();
    private static Path[] _paths;
    private static Pathfinding _pathfinding;
    private static SpareRooms _spareRooms;
    private static MeeplePlacement _meeplePlacement;
    private static DifficultyLevel _difficultyLevel;

    public static void StartGame(Location[] locations, Path[] paths, DifficultyLevel difficultyLevel)
    {
        foreach (var location in locations)
        {
            Locations.Add(location.SpotName, location);
        }

        _paths = paths;
        _difficultyLevel = difficultyLevel;
        _meeplePlacement = new MeeplePlacement(difficultyLevel);
        _pathfinding = new Pathfinding(locations, paths);
        _spareRooms = new SpareRooms();
        foreach (Resource resource in Enum.GetValues(typeof(Resource)))
        {
            if (!ResourceType.IsMaterial(resource)) continue;
            Resources.Add(resource, 0);
        }

        foreach (var location in locations)
        {
            try
            {
                switch (location.SpotName)
                {
                    case Spot.FieldWest:
                        Fields[0] = (Field)location;
                        break;
                    case Spot.FieldEast:
                        Fields[1] = (Field)location;
                        break;
                }
            }
            catch (InvalidCastException)
            {
                Debug.LogError($"{location.gameObject} is not a Field", location.gameObject);
            }
        }

        ReadyDay();
        InvokeResourcesEvent();
    }

    public static bool CheckPathSafeHouse(Location location)
    {
        return _pathfinding.FindPathSafeHouse(location);
    }

    public static bool NextDay()
    {
        foreach (var location in Locations.Values)
        {
            if (location.Character == Meeple.Agent && !CheckPathSafeHouse(location))
            {
                _meeplePlacement.ImprisonAgent();
            }

            location.Character = null;
        }

        var result = _meeplePlacement.NextDay();
        ReadyDay();
        return result;
    }

    private static void ReadyDay()
    {
        foreach (var location in Locations.Values)
        {
            location.ResetActions(_difficultyLevel);
        }
    }

    public static bool SetAgent(Spot spot)
    {
        SetMeeple(spot, Meeple.Agent);
        return _meeplePlacement.PlaceAgent();
    }

    public static void SetEnemy(Meeple meeple, Patrol patrol)
    {
        Location empty = null, notEmpty = null;
        foreach (Spot spot in patrol.Spots)
        {
            var aux = Locations[spot];
            if (aux.IsAvailable())
            {
                empty = aux;
                notEmpty = null;
                break;
            }

            if (notEmpty == null && !Character.IsEnemy(aux.Character))
            {
                notEmpty = aux;
            }
        }

        var result = empty != null ? empty : notEmpty;
        if (result == null) return;
        SetMeeple(result, meeple);

        if (empty != null) return;
        _meeplePlacement.ImprisonAgent();
    }

    private static void SetMeeple(Location location, Meeple meeple)
    {
        location.Character = meeple;
    }

    private static void SetMeeple(Spot spot, Meeple meeple)
    {
        Locations[spot].Character = meeple;
    }

    public static bool HasResources(ResourceAmount[] resources)
    {
        foreach (var resourceAmount in resources)
        {
            if (Resources[resourceAmount.Resource] < resourceAmount.Amount)
            {
                return false;
            }
        }

        return true;
    }

    public static IEnumerable<Resource> GetAvailableSpareRooms()
    {
        return _spareRooms.GetAvailableSpareRooms();
    }

    public static void SpendResources(ResourceAmount[] resources)
    {
        foreach (var resourceAmount in resources)
        {
            Resources[resourceAmount.Resource] -= resourceAmount.Amount;
        }
    }

    public static bool IsFieldAvailableForAirDrop()
    {
        return Fields[0].IsFieldEmpty() || Fields[1].IsFieldEmpty();
    }

    public static void AirDrop(ResourceAmount resourceAmount)
    {
        var field = Fields[0];
        if (!field.IsFieldEmpty())
        {
            field = Fields[1];
        }

        field.SetResource(resourceAmount);
    }

    private static void GainResource(ResourceAmount resourceAmount, Location location = null)
    {
        var resource = resourceAmount.Resource;

        if (ResourceType.IsMaterial(resource))
        {
            var currentResource = Resources[resource] + resourceAmount.Amount;
            Resources[resource] = currentResource > 4 ? 4 : currentResource;
            return;
        }

        if (ResourceType.IsSpareRoom(resource))
        {
            ((SpareRoom)location)!.SetSpareRoom(resource);
            _spareRooms.UseSpareRoom(resource);
            return;
        }

        switch (resource)
        {
            case Resource.Agent:
                _meeplePlacement.HireAgent();
                break;
            case Resource.Morale:
                _meeplePlacement.GainMorale(resourceAmount.Amount);
                break;
        }
    }

    public static void GainReward(Reward reward, Location location)
    {
        if (reward.AirDrop)
        {
            AirDrop(reward.Rewards[0]);
            return;
        }

        foreach (var resourceAmount in reward.Rewards)
        {
            GainResource(resourceAmount, location);
        }

        location.Character = null;

        var field = location as Field;
        if (field)
        {
            field.SetResource(null);
        }
        
        InvokeResourcesEvent();
    }

    private static void InvokeResourcesEvent()
    {
        ResourcesEvent.Invoke(new ResourcesDto(Resources));
    }

    private static void LoseGame()
    {
    }
}