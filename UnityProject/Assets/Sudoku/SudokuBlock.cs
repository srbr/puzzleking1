using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuBlock : MonoBehaviour
{
    [SerializeField] private SudokuGrid[] grids;

    public SudokuGrid this [int i]
    {
        get => grids[i];
    }
}
