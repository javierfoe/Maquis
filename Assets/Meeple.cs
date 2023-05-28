public enum Meeple
{
    Agent,
    Militia,
    Soldier
}

public static class Character
{
    public static bool IsEnemy(Meeple? meeple)
    {
        return meeple != null && meeple != Meeple.Agent;
    }
}