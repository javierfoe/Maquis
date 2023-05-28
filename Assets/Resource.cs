public enum Resource
{
    Agent,
    Explosive,
    FakeId,
    Food,
    Information,
    Medicine,
    Money,
    Morale,
    Poison,
    Weapon,
    ChemistsLab,
    Counterfeiter,
    Fixer,
    Forger,
    Informant,
    Propagandist,
    Pharmacist,
    SafeHouse,
    Smuggler
}

public static class ResourceType
{
    public static bool IsSpareRoom(Resource resource)
    {
        return resource is Resource.ChemistsLab or Resource.Counterfeiter or Resource.Fixer or Resource.Forger
            or Resource.Informant or Resource.Propagandist or Resource.Pharmacist or Resource.SafeHouse
            or Resource.Smuggler;
    }

    public static bool IsMaterial(Resource resource)
    {
        return resource is Resource.Explosive or Resource.Food or Resource.Information or Resource.Medicine
            or Resource.Money or Resource.Poison or Resource.Weapon or Resource.FakeId;
    }
}