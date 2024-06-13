using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuChunk : MonoBehaviour
{
    [SerializeField] private SudokuBlock[] blocks;

    public SudokuBlock this[int i]
    {
        get => blocks[i];
    }

    public SudokuGrid this[int iy, int ix]
    {
        get => blocks[(iy / 3) * 3 + (ix / 3)][(iy % 3) * 3 + (ix % 3)];
    }

    private void Start()
    {
        for (var iy = 0; iy < 9; iy++)
        {
            for (var ix = 0; ix < 9; ix++)
            {
                if (0 < ix)
                {
                    this[iy, ix].LeftGrid = this[iy, ix - 1];
                }
                if (0 < iy)
                {
                    this[iy, ix].UpGrid = this[iy - 1, ix];
                }
                if (ix < 8)
                {
                    this[iy, ix].RightGrid = this[iy, ix + 1];
                }
                if (iy < 8)
                {
                    this[iy, ix].DownGrid = this[iy + 1, ix];
                }
            }
        }
    }
}
