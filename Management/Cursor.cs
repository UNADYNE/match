
public class Cursor : Singleton<Cursor>
{
    private Matchable[] _selected;
    private MatchableGrid _grid;

    protected override void Init()
    {
        _selected = new Matchable[2];
    }

    private void Start()
    {
        _grid = (MatchableGrid)MatchableGrid.Instance;
    }

    public void SelectFirst(Matchable toSelect)
    {
        _selected[0] = toSelect;
        if (!enabled || _selected[0] == null)
        {
            return;
        }

        transform.position = toSelect.transform.position;
    }

    public void SelectSecond(Matchable toSelect)
    {
        _selected[1] = toSelect;
        if (
            !enabled ||
            _selected[0] == null ||
            _selected[1] == null ||
            !_selected[0].Idle ||
            !_selected[1].Idle ||
            _selected[0] == _selected[1]
        )
        {
            return;
        }

        if (SelectedAreAdjacent())
        {
            StartCoroutine(_grid.TrySwap(_selected));
        }

        SelectFirst(null);
    }

    private bool SelectedAreAdjacent()
    {
        if (_selected[0].position.x == _selected[1].position.x)
        {
            if (_selected[0].position.y == _selected[1].position.y + 1)
            {
                return true;
            }
            else if (_selected[0].position.y == _selected[1].position.y - 1)
            {
                return true;
            }
        }
        else if (_selected[0].position.y == _selected[1].position.y)
        {
            if (_selected[0].position.x == _selected[1].position.x + 1)
            {
                return true;
            }
            else if (_selected[0].position.x == _selected[1].position.x - 1)
            {
                return true;
            }
        }

        return false;
    }
}