using System.Collections.Generic;

public class ResourcesDto
{
    public readonly int Food, Money, Weapons, Medicine, Information, Explosives, Poison, FakeId;

    public ResourcesDto(Dictionary<Resource, int> resources)
    {
        foreach (var keyValue in resources)
        {
            var key = keyValue.Key;
            var value = keyValue.Value;

            switch (key)
            {
                case Resource.Explosive:
                    Explosives = value;
                    break;
                case Resource.FakeId:
                    FakeId = value;
                    break;
                case Resource.Food:
                    Food = value;
                    break;
                case Resource.Information:
                    Information = value;
                    break;
                case Resource.Medicine:
                    Medicine = value;
                    break;
                case Resource.Money:
                    Money = value;
                    break;
                case Resource.Poison:
                    Poison = value;
                    break;
                case Resource.Weapon:
                    Weapons = value;
                    break;
            }
        }
    }
}