using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public class MatchablePool : ObjectPool<Matchable>
{
    [SerializeField] private int howManyTypes;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private List<string> colors = new List<string>();

    void Start()
    {
        for (int i = 0; i < sprites.Length; ++i)
        {
            string[] strArr = sprites[i].name.Split("_");
            colors.Add(strArr[strArr.Length - 1]);
        }
    }

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
}