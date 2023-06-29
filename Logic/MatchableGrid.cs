using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public class MatchableGrid : GridSystem<Matchable>
{
    private MatchablePool _pool;

    // private Vector3 _onScreenPosition;
    [SerializeField] private Vector3 offScreenOffSet;
    [SerializeField] private float delay = 0.05f;
    private ScoreManager _scoreManager;

    private void Start()
    {
        _pool = (MatchablePool)MatchablePool.Instance;
        _scoreManager = ScoreManager.Instance;
    }

    public IEnumerator PopulateGrid(bool allowMatches = false, bool initial = false)
    {
        List<Matchable> newMatchables = new List<Matchable>();
        Matchable newMatchable;
        Vector3 onScreenPosition;

        for (int y = 0; y != Dimensions.y; ++y)
        {
            for (int x = 0; x != Dimensions.x; ++x)
            {
                if (IsEmpty(x, y))
                {
                    // get a matchable from the pool
                    newMatchable = _pool.GetRandomMatchable();

                    // position new matchable off screen
                    newMatchable.transform.position = transform.position + new Vector3(x, y) + offScreenOffSet;
                    
                    // activate the matchable game object
                    newMatchable.gameObject.SetActive(true);

                    // tell the matchable where it is on the grid
                    newMatchable.position = new Vector2Int(x, y);

                    // set the matchable's grid position
                    PutItemAt(newMatchable, x, y);

                    // add the new matchable to the list
                    newMatchables.Add(newMatchable);

                    // cache new matchable's type to check for matches during initial population
                    int initialType = newMatchable.Type;

                    while (!allowMatches && IsPartOfMatch(newMatchable))
                    {
                        // change matchable type until no matches

                        if (_pool.NextType(newMatchable) == initialType)
                        {
                            // if we've gone through all the types, just break
                            Debug.Log($"Failed to find a matchable type for {newMatchable} at {x}, {y}");
                            yield return null;
                            break;
                        }
                    }
                }

                // move the matchables to their new positions and wait until the last has finished
                for (int i = 0; i != newMatchables.Count; ++i)
                {
                    // calculate the future position of matchables[i]
                    onScreenPosition = transform.position + new Vector3(newMatchables[i].position.x, newMatchables[i].position.y, 0);
                    if (i == newMatchables.Count - 1)
                    {
                        yield return StartCoroutine(newMatchables[i].MoveToPosition(onScreenPosition));
                    }
                    else
                    {
                        StartCoroutine(newMatchables[i].MoveToPosition(onScreenPosition));
                    }

                    if (initial)
                    {
                        yield return new WaitForSeconds(delay);
                    }
                }
            }
        }

        // yield return null;
    }

    // check for matches while populating the grid
    private bool IsPartOfMatch(Matchable toMatch)
    {
        int horizontalMatches = 0;
        int verticalMatches = 0;

        // check left
        horizontalMatches += CountMatchesInDirection(toMatch, Vector2Int.left);

        // check right
        horizontalMatches += CountMatchesInDirection(toMatch, Vector2Int.right);

        if (horizontalMatches > 1)
        {
            return true;
        }

        // check up
        verticalMatches += CountMatchesInDirection(toMatch, Vector2Int.up);

        // check down
        verticalMatches += CountMatchesInDirection(toMatch, Vector2Int.down);

        if (verticalMatches > 1)
        {
            return true;
        }

        return false;
    }

    // count the number of matches in a direction
    private int CountMatchesInDirection(Matchable toMatch, Vector2Int direction)
    {
        int matches = 0;
        Vector2Int position = toMatch.position + direction;

        while (CheckBounds(position) && !IsEmpty(position) && GetItemAt(position).Type == toMatch.Type)
        {
            ++matches;
            position += direction;
        }

        return matches;
    }

    public IEnumerator TrySwap(Matchable[] toBeSwapped)
    {
        // make a copy of the matchables to so the cursor doesn't overwrite them
        Matchable[] copies = new Matchable[2];
        copies[0] = toBeSwapped[0];
        copies[1] = toBeSwapped[1];

        // yield until matchables animate swapping
        yield return StartCoroutine(Swap(copies));

        // check for match
        Match[] matches = new Match[2];
        matches[0] = GetMatch(copies[0]);
        matches[1] = GetMatch(copies[1]);

        if (matches[0] != null)
        {
            // resolve match
            StartCoroutine(_scoreManager.ResolveMatch(matches[0]));
        }

        if (matches[1] != null)
        {
            // resolve match
            StartCoroutine(_scoreManager.ResolveMatch(matches[1]));
        }

        if (matches[0] == null && matches[1] == null)
        {
            // no match, swap back
            StartCoroutine(Swap(copies));
        }
        else
        {
            CollapseGrid();
            yield return StartCoroutine(PopulateGrid(true));
            // scan for matches when a match is removed and the grid back-fills those spaces
            ScanForMatches();
        }
    }

    // add each matchable in the direction and return it
    private Match GetMatchesInDirection(Matchable toMatch, Vector2Int direction)
    {
        Match match = new Match();
        Vector2Int position = toMatch.position + direction;
        Matchable next;

        while (CheckBounds(position) && !IsEmpty(position))
        {
            next = GetItemAt(position);

            if (next.Type == toMatch.Type && next.Idle)
            {
                match.AddMatchable(next);
                position += direction;
            }
            else
            {
                break;
            }
        }

        return match;
    }

    private Match GetMatch(Matchable toMatch)
    {
        Match match = new Match(toMatch);

        Match horizontalMatch = GetMatchesInDirection(toMatch, Vector2Int.left);
        horizontalMatch.Merge(GetMatchesInDirection(toMatch, Vector2Int.right));

        if (horizontalMatch.Count > 1)
        {
            match.Merge(horizontalMatch);
        }

        Match verticalMatch = GetMatchesInDirection(toMatch, Vector2Int.up);
        verticalMatch.Merge(GetMatchesInDirection(toMatch, Vector2Int.down));
        if (verticalMatch.Count > 1)
        {
            match.Merge(verticalMatch);
        }

        if (match.Count == 1)
        {
            return null;
        }

        return match;
    }

    private IEnumerator Swap(Matchable[] toBeSwapped)
    {
        // swap in grid data struct
        SwapItemsAt(toBeSwapped[0].position, toBeSwapped[1].position);

        // tell matchables their new positions
        Vector2Int temp = toBeSwapped[0].position;
        toBeSwapped[0].position = toBeSwapped[1].position;
        toBeSwapped[1].position = temp;


        // get world position of matchables
        Vector3[] worldPosition = new Vector3[2];
        worldPosition[0] = toBeSwapped[0].transform.position;
        worldPosition[1] = toBeSwapped[1].transform.position;
        // animate matchables to new positions
        StartCoroutine(toBeSwapped[0].MoveToPosition(worldPosition[1]));
        yield return StartCoroutine(toBeSwapped[1].MoveToPosition(worldPosition[0]));
    }

    /// <summary>
    /// Search grid from left to right, bottom to top for empty spaces.
    /// move matchables down to fill empty spaces.
    /// </summary>
    private void CollapseGrid()
    {
        for (int x = 0; x != Dimensions.x; ++x)
        {
            for (int yEmpty = 0; yEmpty != Dimensions.y - 1; ++yEmpty)
            {
                if (IsEmpty(x, yEmpty))
                {
                    for (int yNotEmpty = yEmpty + 1; yNotEmpty != Dimensions.y; ++yNotEmpty)
                    {
                        if (!IsEmpty(x, yNotEmpty) && GetItemAt(x, yNotEmpty).Idle)
                        {
                            MoveMatchableToPosition(GetItemAt(x, yNotEmpty), x, yEmpty);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void MoveMatchableToPosition(Matchable toMove, int x, int y)
    {
        MoveItemTo(toMove.position, new Vector2Int(x, y));
        // tell matchable its new position
        toMove.position = new Vector2Int(x, y);

        // animate matchable to new position
        StartCoroutine(toMove.MoveToPosition(transform.position + new Vector3(x, y, 0)));
    }

    /// <summary>
    /// Scan the grid for matches and resolve them.
    /// </summary>
    private void ScanForMatches()
    {
        Matchable toMatch;
        Match match;
        // iterate through grid looking for non-idle matchables
        for (int y = 0; y != Dimensions.y; ++y)
        {
            for (int x = 0; x != Dimensions.x; ++x)
            {
                if (!IsEmpty(x, y))
                {
                    toMatch = GetItemAt(x, y);
                    if (!toMatch.Idle)
                    {
                        continue;
                    }

                    // try to get a match and resolve it
                    match = GetMatch(toMatch);
                    if (match != null)
                    {
                        StartCoroutine(_scoreManager.ResolveMatch(match));
                    }
                }
            }
        }
    }
}