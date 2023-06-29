using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Transform collectionLocation;
    private MatchableGrid _grid;
    private int _score;
    private int Score => _score;
    

    protected override void Init()
    {
        _score = 0;
    }

    private void Start()
    {
        _grid = (MatchableGrid)MatchableGrid.Instance;
    }

    public void AddScore(int score)
    {
        _score += score;
        scoreText.text = _score.ToString();
    }

    public IEnumerator ResolveMatch(Match toResolve)
    {
        Matchable matchable;
        for(int i = 0; i < toResolve.Count; ++i)
        {
            matchable = toResolve.Matchables[i];
            // remove matchables from the grid
            _grid.RemoveItemAt(matchable.position);

            // move matchables to the score area
            if (i == toResolve.Count - 1)
            {
                yield return StartCoroutine(matchable.Resolve(collectionLocation));
            }
            else
            {
                StartCoroutine(matchable.Resolve(collectionLocation));
            }
        }

        // update the player's score
        AddScore(toResolve.Count * toResolve.Count);

        yield return null;
    }
}