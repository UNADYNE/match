using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Transform collectionLocation;
    private MatchableGrid _grid;
    private int _score;
    private int Score => _score;
    private MatchablePool _pool;
    

    protected override void Init()
    {
        _score = 0;
    }

    private void Start()
    {
        _grid = (MatchableGrid)MatchableGrid.Instance;
        _pool = (MatchablePool)MatchablePool.Instance;
    }

    public void AddScore(int score)
    {
        _score += score;
        scoreText.text = _score.ToString();
    }

    public IEnumerator ResolveMatch(Match toResolve, MatchType powerUpUsed = MatchType.Invalid)
    {
        Matchable powerUpFormed = null;
        Matchable matchable;
        Transform target = collectionLocation;
        
        // TODO check for cross match first
        if (powerUpUsed == MatchType.Invalid && toResolve.Count > 3)
        {
            powerUpFormed = _pool.UpgradeMatchable(toResolve.ToBeUpgraded, toResolve.Type);
            toResolve.RemoveMatchable(powerUpFormed);
            target = powerUpFormed.transform;
            powerUpFormed.SortingOrder = 3;
        }

        for(int i = 0; i != toResolve.Count; ++i)
        {
            matchable = toResolve.Matchables[i];
            
            // only allow gems used as power-ups to resolve gems
            if(powerUpUsed != MatchType.Match5 && matchable.IsGem)
            {
                continue;
            }
            
            // remove matchables from the grid
            _grid.RemoveItemAt(matchable.position);

            // move matchables to the score area
            if (i == toResolve.Count - 1)
            {
                yield return StartCoroutine(matchable.Resolve(target));
            }
            else
            {
                StartCoroutine(matchable.Resolve(target));
            }
        }

        // update the player's score
        AddScore(toResolve.Count * toResolve.Count);
        
        if(powerUpFormed != null)
        {
            powerUpFormed.SortingOrder = 1;
        }
        yield return null;
    }
}