using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeeplePlacement
{
    private static readonly bool[] MoraleDecreaseDays =
            { false, false, true, false, false, true, false, false, true, false, false, true, false, true },
        MoraleDecreaseDaysHard = { false, true, false, false, true, false, false, true, false, true, false, true };

    private readonly int[] _moraleNazis = { 0, 5, 4, 4, 4, 3, 3, 2 };
    private readonly bool[] _moraleDecrease;
    private readonly int _maxAgents, _endDay;
    private int _currentMorale, _currentAgents, _readyAgents, _imprisonedAgents, _currentDay, _currentNazis, _soldierCount, _maxEnemies;
    private int MaxMorale => _moraleNazis.Length - 1;
    private readonly List<Patrol> _patrols = new(), _discardedPatrols = new();

    public List<Patrol> Patrols => _patrols;

    public MeeplePlacement(DifficultyLevel difficultyLevel)
    {
        _maxAgents = difficultyLevel is DifficultyLevel.VeryEasy or DifficultyLevel.Easy ? 5 : 4;
        _readyAgents = _maxAgents - 2;
        _currentAgents = _readyAgents;
        _moraleDecrease = difficultyLevel is DifficultyLevel.Hard or DifficultyLevel.VeryHard
            ? MoraleDecreaseDaysHard
            : MoraleDecreaseDays;
        _currentMorale = 6;
        
        _discardedPatrols.AddRange(new Patrol[]
        {
            new(Spot.RadioA, Spot.RueBaradat, Spot.Grocer),
            new(Spot.RadioA, Spot.PontLeveque, Spot.BlackMarket),
            new(Spot.RadioB, Spot.Grocer, Spot.BlackMarket),
            new(Spot.RadioB, Spot.RueBaradat, Spot.PontLeveque),
            new(Spot.PontLeveque, Spot.BlackMarket, Spot.Doctor),
            new(Spot.PontLeveque, Spot.PontDuNord, Spot.Doctor),
            new(Spot.Grocer, Spot.PontDuNord, Spot.Fence),
            new(Spot.Grocer, Spot.PoorDistrict, Spot.Doctor),
            new(Spot.Fence, Spot.PontDuNord, Spot.PontLeveque),
            new(Spot.Fence, Spot.RueBaradat, Spot.PoorDistrict)
        });

        Shuffle();
    }

    public void ResetAvailability()
    {
        foreach (var patrol in _patrols)
        {
            patrol.MockOccupied = false;
        }

        foreach (var patrol in _discardedPatrols)
        {
            patrol.MockOccupied = false;
        }
    }

    private Patrol DrawPatrol()
    {
        if (_patrols.Count == 0)
        {
            Shuffle();
        }

        var result = _patrols[0];
        _patrols.RemoveAt(0);
        _discardedPatrols.Add(result);
        return result;
    }

    private void Shuffle()
    {
        for (var i = 0; i < _discardedPatrols.Count; i++)
        {
            var temp = _discardedPatrols[i];
            var randomIndex = UnityEngine.Random.Range(i, _discardedPatrols.Count);
            _discardedPatrols[i] = _discardedPatrols[randomIndex];
            _discardedPatrols[randomIndex] = temp;
        }

        _patrols.AddRange(_discardedPatrols);
        _discardedPatrols.Clear();
    }

    public void ImprisonAgent()
    {
        _imprisonedAgents++;
    }

    public void MilitiaKilled()
    {
        _soldierCount++;
    }

    public bool PlaceAgent()
    {
        _currentAgents--;
        var moreReadyAgents = _currentAgents > 0;
        SetEnemy();
        if (!moreReadyAgents)
        {
            for (var i = _readyAgents; i < _maxEnemies; ++i)
            {
                SetEnemy();
            }
            _readyAgents -= _imprisonedAgents;
            _imprisonedAgents = 0;
            _currentAgents = _readyAgents;
        }
        return moreReadyAgents;
    }

    private void SetEnemy()
    {
        var enemy = _currentNazis++ < _maxEnemies - _soldierCount ? Meeple.Militia : Meeple.Soldier;
        var patrol = DrawPatrol();
        Maquis.SetEnemy(enemy, patrol);
    }

    public void HireAgent()
    {
        if (AreAllAgentsHired()) return;
        _currentAgents++;
    }

    public bool AreAllAgentsHired()
    {
        return _imprisonedAgents + _currentAgents == _maxAgents;
    }

    public void MaximumSoldiers()
    {
        _maxEnemies = Math.Max(_readyAgents, _moraleNazis[_currentMorale]);
    }

    public bool IsMoraleAtMaximum()
    {
        return _currentMorale == MaxMorale;
    }

    public void NextDay()
    {
        _currentDay++;
        if (!_moraleDecrease[_currentDay]) return;
        GainMorale(-1);
    }

    public void GainMorale(int amount)
    {
        _currentMorale += amount;
        if (_currentMorale + amount > MaxMorale)
        {
            _currentMorale = MaxMorale;
        }
    }
}