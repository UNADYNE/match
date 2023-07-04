using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace Management
{
    public class GameManager : Singleton<GameManager>
    {
        private MatchablePool _pool;
        private MatchableGrid _grid;

        [SerializeField] private Vector2Int dimensions = Vector2Int.one;


        private void Start()
        {
            _pool = (MatchablePool)MatchablePool.Instance;
            _grid = (MatchableGrid)MatchableGrid.Instance;

            StartCoroutine(Setup());

        }

        private IEnumerator Setup()
        {
            // display loading screen

            // pool the matchables
            _pool.PoolObjects(dimensions.x * dimensions.y * 10);
            
            // initialize the grid
            _grid.InitializeGrid(dimensions);
            
            yield return null;
            
            StartCoroutine(_grid.PopulateGrid(false, true));
            
            // hide loading screen
        }
    }
}