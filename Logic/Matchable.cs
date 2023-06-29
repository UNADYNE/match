using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Matchable : Movable
{
    private MatchablePool _pool;
    private Cursor _cursor;
    private int _type;
    public Vector2Int position;
    public int Type => _type;
    private int _originalSortingOrder;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _cursor = Cursor.Instance;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _pool = (MatchablePool) MatchablePool.Instance;
        _originalSortingOrder = _spriteRenderer.sortingOrder;
        
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
        _spriteRenderer.sortingOrder = 2;
        yield return StartCoroutine(MoveToPosition(collectionLocation.position, true));
        _spriteRenderer.sortingOrder = _originalSortingOrder;
        _pool.ReturnObjectToPool(this);
        yield return null;
    }
}