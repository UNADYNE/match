using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Matchable : Movable
{
    private MatchType _powerUp = MatchType.Invalid;
    private MatchablePool _pool;
    private MatchableGrid _grid;
    private Cursor _cursor;
    [SerializeField] private int _type;
    public Vector2Int position;
    public int Type => _type;
    private int _originalSortingOrder;

    private SpriteRenderer _spriteRenderer;

    public bool IsGem => _powerUp == MatchType.Match5;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalSortingOrder = _spriteRenderer.sortingOrder;
    }

    private void Start()
    {
        _cursor = Cursor.Instance;
        _pool = (MatchablePool) MatchablePool.Instance;
        _grid = (MatchableGrid)MatchableGrid.Instance;
    }

    public void SetType(int type, Sprite sprite)
    {
        _type = type;
        _spriteRenderer.sprite = sprite;
    }

    public override string ToString()
    {
        return gameObject.name;
    }
    
    public int SortingOrder
    {
        set => _spriteRenderer.sortingOrder = value;
    }

    private void OnMouseDown()
    {
        _cursor.SelectFirst(this);
    }

    private void OnMouseUp()
    {
        _cursor.SelectFirst(null);
    }

    private void OnMouseEnter()
    {
        _cursor.SelectSecond(this);
    }

    public IEnumerator Resolve(Transform collectionLocation)
    {
        // if matchable is power up
        
        if(_powerUp != MatchType.Invalid)
        {
            // resolve a match4 power up
            if (_powerUp == MatchType.Match4)
            {
                // score everything adjacent to this matchable
                _grid.MatchAllAdjacent(this);
                
            }
            // resolve a match5 power up
            if (_powerUp == MatchType.Match5)
            {
                
            }
            
            // resolve a cross power up
            if (_powerUp == MatchType.Cross)
            {
                _grid.MatchRow(this);
            }
            
            _powerUp = MatchType.Invalid;
        }
        
        if(collectionLocation == null)
        {
            yield break;
        }
        
        _spriteRenderer.sortingOrder = 2;
        StartCoroutine(MoveToTransform(collectionLocation));
        _spriteRenderer.sortingOrder = _originalSortingOrder;
        _pool.ReturnObjectToPool(this);
        yield return null;
    }

    //  change the sprite of this matchable to be a powerup while retaining colour and type
    public Matchable Upgrade(MatchType powerUpType, Sprite powerUpSprite)
    {
        if(powerUpType != MatchType.Invalid)
        {
            _idle = false;
            StartCoroutine(Resolve(null));
            _idle = true;
        }
        if(powerUpType == MatchType.Match5)
        {
            _type = -1;
        }
        _powerUp = powerUpType;
        _spriteRenderer.sprite = powerUpSprite;

        return this;
    }
}