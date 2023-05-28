public class Patrol
{
    private static int _idCount;
    public readonly Spot[] Spots;
    public readonly int Id;

    public bool MockOccupied
    {
        get;
        set;
    }

    public Patrol(Spot first, Spot second, Spot third)
    {
        Id = _idCount++;
        Spots = new[] { first, second, third };
    }
}