using System.Collections.Generic;
using NUnit.Framework;

public class Match
{
    private List<Matchable> _matchables;
    public List<Matchable> Matchables => _matchables;
    public int Count => _matchables.Count;
    public Match()
    {
        _matchables = new List<Matchable>(5);
    }
    
    public Match(Matchable original) : this()
    {
        AddMatchable(original);
    }
    
    public void AddMatchable(Matchable toMatch)
    {
        _matchables.Add(toMatch);
    }
    
    public void Merge(Match toMatch)
    {
        _matchables.AddRange(toMatch.Matchables);
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
}
