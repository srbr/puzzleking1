using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SuiriGridBlock : MonoBehaviour
{
    [SerializeField] private SuiriGrid[] grids;

    private void Start()
    {
        foreach (var grid in grids)
        {
            grid.Parent = this;
        }
    }

    public void RefreshGrids()
    {
        foreach (var grid in grids)
        {
            grid.IsRedundant = false;
        }
        for (var i = 0; i < 5; ++i)
        {
            var rows = Enumerable.Range(0, 5).Select(n => grids[i * 5 + n]).Where(n => n.CurrentState == SuiriGrid.State.True);
            if (2 <= rows.Count())
            {
                foreach (var r in rows)
                {
                    r.IsRedundant = true;
                }
            }
            var cols = Enumerable.Range(0, 5).Select(n => grids[i + n * 5]).Where(n => n.CurrentState == SuiriGrid.State.True);
            if (2 <= cols.Count())
            {
                foreach (var c in cols)
                {
                    c.IsRedundant = true;
                }
            }
        }
        foreach (var grid in grids)
        {
            grid.Refresh();
        }
    }

    public bool IsCorrect()
    {
        return !grids.Any(g => !g.IsCorrect());
    }

    public void Clear()
    {
        foreach (var grid in grids)
        {
            grid.CurrentState = SuiriGrid.State.Empty;
        }
        foreach (var grid in grids)
        {
            grid.Refresh();
        }
    }
}
