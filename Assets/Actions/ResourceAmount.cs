using System;

public class ResourceAmount
{
    public readonly Resource Resource;
    public readonly int Amount;
    public readonly bool AirDrop;

    public ResourceAmount(Resource resource, int amount = 1, bool airDrop = false)
    {
        Resource = resource;
        Amount = amount;
        AirDrop = airDrop;
    }

    public static ResourceAmount[] AddFixerRequirement(ResourceAmount[] input)
    {
        for (var i = 0; i < input.Length; i++)
        {
            if (input[i].Resource != Resource.Money) continue;
            input[i] = new ResourceAmount(input[i].Resource, input[i].Amount + 1);
            return input;
        }

        var output = new ResourceAmount[input.Length + 1];
        Array.Copy(input,output, input.Length);
        output[input.Length + 1] = new ResourceAmount(Resource.Money);
        return output;
    }
}