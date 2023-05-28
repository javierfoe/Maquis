using System;
using System.Collections.Generic;

public class SpareRooms
{
    private readonly List<Resource> _availableSpareRooms = new();

    public SpareRooms()
    {
        foreach (Resource resource in Enum.GetValues(typeof(Resource)))
        {
            if (!ResourceType.IsSpareRoom(resource)) continue;
            _availableSpareRooms.Add(resource);
        }
    }

    public IEnumerable<Resource> GetAvailableSpareRooms()
    {
        return _availableSpareRooms;
    }

    public void UseSpareRoom(Resource resource)
    {
        if (!ResourceType.IsSpareRoom(resource))
        {
            throw new NotSupportedException($"{resource} must be a SpareRoom");
        }

        _availableSpareRooms.Remove(resource);
    }
}