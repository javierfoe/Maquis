using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Location : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Spot spotName;

    private SpriteRenderer _spriteRenderer;
    private Meeple? _character;
    private bool _cannon;

    public Spot SpotName => spotName;
    public readonly List<Action> Actions = new();

    public virtual bool IsSafeHouse()
    {
        return spotName == Spot.SafeHouse;
    }

    public Meeple? Character
    {
        get => _character;
        set
        {
            _character = value;
            var color = _character == null
                ? Color.white
                : Character switch
                {
                    Meeple.Agent => Color.gray,
                    Meeple.Militia => Color.blue,
                    Meeple.Soldier => Color.red,
                    _ => throw new NotImplementedException()
                };
            _spriteRenderer.color = color;
        }
    }

    public bool IsBlocked()
    {
        return _character != null && global::Character.IsEnemy(_character);
    }

    public virtual bool IsAvailable()
    {
        return _character == null;
    }

    public void ResetActions(DifficultyLevel difficultyLevel)
    {
        Actions.Clear();
        var actions = GetActions(difficultyLevel);
        if (actions == null) return;
        Actions.AddRange(GetActions(difficultyLevel));
    }

    protected virtual IEnumerable<Action> GetActions(DifficultyLevel difficultyLevel)
    {
        switch (SpotName)
        {
            case Spot.BlackMarket:
                var blackMarketReward = new Reward(new ResourceAmount[]
                    { new(Resource.Morale, -1), new(Resource.Money) });
                return new[]
                {
                    new Action(this, new ResourceAmount[] { new(Resource.Food) }, blackMarketReward),
                    new Action(this, new ResourceAmount[] { new(Resource.Medicine) }, blackMarketReward)
                };
            case Spot.Cafe:
                return new[]
                {
                    new Action(this, new ResourceAmount[] { new(Resource.Food) },
                        new Reward(new ResourceAmount[] { new(Resource.Agent) }))
                };
            case Spot.Doctor:
                return new[]
                {
                    new Action(this, Resource.Medicine)
                };
            case Spot.Fence:
                return new[]
                {
                    new Action(this, new ResourceAmount[] { new(Resource.Money) },
                        new Reward(new ResourceAmount[] { new(Resource.Weapon) }))
                };
            case Spot.Grocer:
                return new[]
                {
                    new Action(this, Resource.Food)
                };
            case Spot.PoorDistrict:
                return new[]
                {
                    new Action(this, new ResourceAmount[] { new(Resource.Medicine), new(Resource.Food) }, new Reward(
                        new ResourceAmount[]
                            { new(Resource.Morale) }))
                };
            case Spot.RadioA:
            case Spot.RadioB:
                return AirDropActions(this, difficultyLevel);
            default:
                return null;
        }
    }

    private IEnumerable<Action> AirDropActions(Location location, DifficultyLevel difficultyLevel)
    {
        return new[]
        {
            new Action(location, Resource.Information),
            new Action(location, Resource.Food,
                difficultyLevel is DifficultyLevel.VeryEasy or DifficultyLevel.Easy ? 4 : 3,
                false, true),
            new Action(location, Resource.Money,
                difficultyLevel is DifficultyLevel.VeryEasy or DifficultyLevel.Easy ? 2 : 1,
                false, true),
            new Action(location, Resource.Weapon, difficultyLevel is DifficultyLevel.VeryEasy ? 2 : 1, false, true)
        };
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnValidate()
    {
        gameObject.name = SpotName.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!MaquisBehaviour.PlaceAgents && Character == Meeple.Agent && eventData.button == PointerEventData.InputButton.Right)
        {
            MaquisBehaviour.SelectedLocation = this;
            var debug = "";
            foreach (var action in Actions)
            {
                debug += $"{action.AreRequirementsMet()} ";
            }
            Debug.Log(debug);
            return;
        }

        if (!IsAvailable() || !MaquisBehaviour.PlaceAgents) return;
        
        if (MaquisBehaviour.SelectedLocation != null)
        {
            MaquisBehaviour.PlaceAgents = Maquis.SetAgent(SpotName);
            MaquisBehaviour.SelectedLocation = null;
        }
        else
        {
            MaquisBehaviour.SelectedLocation = this;
        }
    }
}