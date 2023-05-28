public class Reward
{
    public readonly ResourceAmount[] Rewards;
    public readonly bool AirDrop;

    public Reward(ResourceAmount[] rewards, bool airDrop = false)
    {
        Rewards = rewards;
        AirDrop = airDrop;
    }
}