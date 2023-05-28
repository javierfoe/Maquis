using System;
using System.Collections.Generic;

public class SpareRoom : Location
{
    private Resource _spareRoom;
    private bool _usedSpareRoom;

    public void SetSpareRoom(Resource resource)
    {
        _spareRoom = resource;
        _usedSpareRoom = true;
    }

    public override bool IsSafeHouse()
    {
        return _spareRoom == Resource.SafeHouse;
    }

    protected override IEnumerable<Action> GetActions(DifficultyLevel difficultyLevel)
    {
        var actions = new List<Action>();

        if (!_usedSpareRoom)
        {
            var spareRooms = Maquis.GetAvailableSpareRooms();
            foreach (var spareRoom in spareRooms)
            {
                actions.Add(new Action(this, new ResourceAmount[] { new(Resource.Money, 2) },
                    new Reward(new ResourceAmount[] { new(spareRoom) }), false));
            }
        }
        else
        {
            if (!ResourceType.IsSpareRoom(_spareRoom))
            {
                throw new NotSupportedException($"{_spareRoom} must be a SpareRoom");
            }

            switch (_spareRoom)
            {
                case Resource.ChemistsLab:
                    actions.Add(ChemistsLab());
                    break;
                case Resource.Counterfeiter:
                    actions.Add(Counterfeiter());
                    break;
                case Resource.Fixer:
                    actions.AddRange(new[]
                    {
                        ChemistsLab(true),
                        Forger(true),
                        Informant(true),
                        Propagandist(true),
                        Pharmacist(true),
                    });
                    actions.AddRange(Smuggler(true));
                    break;
                case Resource.Forger:
                    actions.Add(Forger());
                    break;
                case Resource.Informant:
                    actions.Add(Informant());
                    break;
                case Resource.Propagandist:
                    actions.Add(Propagandist());
                    break;
                case Resource.Pharmacist:
                    actions.Add(Pharmacist());
                    break;
                case Resource.Smuggler:
                    actions.AddRange(Smuggler());
                    break;
            }
        }

        return actions;
    }

    private Action Propagandist(bool fixer = false)
    {
        return new Action(this, Resource.Morale, 1, fixer, false, false);
    }

    private IEnumerable<Action> Smuggler(bool fixer = false)
    {
        return new[] { new Action(this, Resource.Food, 3, fixer), new Action(this, Resource.Medicine, 3, fixer) };
    }

    private Action Forger(bool fixer = false)
    {
        return new Action(this, new ResourceAmount[] { new(Resource.Money, 2), new(Resource.Information) },
            new Reward(new[] { new ResourceAmount(Resource.FakeId) }), fixer);
    }

    private Action Counterfeiter()
    {
        return new Action(this, Resource.Money);
    }

    private Action Pharmacist(bool fixer = false)
    {
        return new Action(this, new[] { new ResourceAmount(Resource.Medicine, 2) },
            new Reward(new[] { new ResourceAmount(Resource.Poison) }), false, fixer);
    }

    private Action Informant(bool fixer = false)
    {
        return new Action(this, Resource.Information, 1, fixer);
    }

    private Action ChemistsLab(bool fixer = false)
    {
        return new Action(this, new[] { new ResourceAmount(Resource.Medicine) },
            new Reward(new ResourceAmount[] { new(Resource.Explosive) }), true, fixer);
    }
}