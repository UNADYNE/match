using System.Collections.Generic;
using UnityEngine;

public enum Orientation
{
    None,
    Horizontal,
    Vertical,
    Both
}

public enum MatchType
{
    Invalid,
    Match3,
    Match4,
    Match5,
    Cross,
    Column,
    Row
}

/// <summary>
/// Represent a single match of matchables - Match contains a list of matchables
/// which the list of matchables' count is 3 or more the match is valid.
/// </summary>
public class Match
{
    private int _unlisted = 0;
    public Orientation Orientation = Orientation.None;

    private List<Matchable> _matchables;

    private Matchable _toBeUpgraded = null;
    public List<Matchable> Matchables => _matchables;
    public int Count => _matchables.Count + _unlisted;

    public bool Contains(Matchable toCompare) => _matchables.Contains(toCompare);

    public Match()
    {
        _matchables = new List<Matchable>(5);
    }

    public Match(Matchable original) : this()
    {
        AddMatchable(original);
        _toBeUpgraded = original;
    }

    public MatchType Type
    {
        get
        {
            if (Orientation == Orientation.Both)  return MatchType.Cross;
            
            if (_matchables.Count > 4)  return MatchType.Match5;
            
            if (_matchables.Count == 4) return MatchType.Match4;
          
            if (_matchables.Count == 3) return MatchType.Match3;
           
            
            return MatchType.Invalid;
            
        }
    }

    public Matchable ToBeUpgraded
    {
        get
        {
            if (_toBeUpgraded != null)
            {
                return _toBeUpgraded;
            }

            return _matchables[Random.Range(0, _matchables.Count)];
        }
    }

    public void AddMatchable(Matchable toMatch)
    {
        _matchables.Add(toMatch);
    }

    public void AddUnlisted()
    {
        ++_unlisted;
    }

    // merge another math into this one
    public void Merge(Match toMerge)
    {
        _matchables.AddRange(toMerge.Matchables);

        // update the match orientation
        if
        (
            Orientation == Orientation.Both ||
            toMerge.Orientation == Orientation.None ||
            (Orientation == Orientation.Horizontal && toMerge.Orientation == Orientation.Vertical) ||
            (Orientation == Orientation.Vertical && toMerge.Orientation == Orientation.Horizontal)
        )
        {
            Orientation = Orientation.Both;
        }
        else if (toMerge.Orientation == Orientation.Horizontal)
        {
            Orientation = Orientation.Horizontal;
        }
        else if (toMerge.Orientation == Orientation.Vertical)
        {
            Orientation = Orientation.Vertical;
        }
    }

    public override string ToString()
    {
        string s = $"Match of Type{_matchables[0].Type} : ";
        foreach (Matchable matchable in _matchables)
        {
            s += $"({matchable.position.x}, {matchable.position.y}) ";
        }

        return s;
    }

    public void RemoveMatchable(Matchable toBeRemoved)
    {
        _matchables.Remove(toBeRemoved);
    }
}