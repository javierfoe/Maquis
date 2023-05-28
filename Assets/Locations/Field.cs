using System.Collections.Generic;

public class Field : Location
{
    private ResourceAmount _resourceAmount;

    public bool IsFieldEmpty()
    {
        return _resourceAmount.Amount == 0;
    }

    public override bool IsAvailable()
    {
        return base.IsAvailable() && !IsFieldEmpty();
    }

    protected override IEnumerable<Action> GetActions(DifficultyLevel difficultyLevel)
    {
        return _resourceAmount == null ? null : new[] { new Action(this, _resourceAmount) };
    }

    public void SetResource(ResourceAmount resourceAmount)
    {
        _resourceAmount = resourceAmount;
    }
}