using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public class MatchablePool : ObjectPool<Matchable>
{
    [SerializeField] private int howManyTypes;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite[] match4PowerUp;
    [SerializeField] private Sprite match5PowerUp;
    [SerializeField] private Sprite[] crossPowerUp;
    
    public void RandomizeType(Matchable toRandomize)
    {
        int random = Random.Range(0, howManyTypes);
        toRandomize.SetType(random, sprites[random]);
    }

    public Matchable GetRandomMatchable()
    {
        Matchable matchable = GetPooledObject();
        RandomizeType(matchable);
        return matchable;
    }

    public int NextType(Matchable matchable)
    {
        int nextType = (matchable.Type + 1) % howManyTypes;

        matchable.SetType(nextType, sprites[nextType]);
        return 0;
    }

    public Matchable UpgradeMatchable(Matchable toBeUpgraded, MatchType type)
    {
        switch (type)
        {
            case MatchType.Cross:
                return toBeUpgraded.Upgrade(MatchType.Cross, crossPowerUp[toBeUpgraded.Type]);
            case MatchType.Match4:
                return toBeUpgraded.Upgrade(MatchType.Match4, match4PowerUp[toBeUpgraded.Type]);
            case MatchType.Match5:
                return toBeUpgraded.Upgrade(MatchType.Match5, match5PowerUp);
            default:
                Debug.LogWarning("Tired to upgrade a matchable with invalid match type.");
                return toBeUpgraded;
        }
    }
}