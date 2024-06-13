using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Sudoku : MonoBehaviour
{
    [SerializeField] private SudokuChunk[] chunks;
    [SerializeField] private Button[] numberButtons;
    [SerializeField] private Button eraseButton;
    [SerializeField] private GameObject clearedObject;
    [SerializeField] private GameObject selectFrame;
    [SerializeField] private GameObject exitDialog;
    private List<List<SudokuGrid>> distinctGroups = new List<List<SudokuGrid>>();
    private HashSet<SudokuGrid> allGrids = new HashSet<SudokuGrid>();
    private SudokuGrid selectedGrid;

    private void Start()
    {
        var indexer = Enumerable.Range(0, 9);
        foreach (var chunk in chunks)
        {
            foreach (var i in indexer)
            {
                distinctGroups.Add(indexer.Select(n => chunk[i][n]).ToList());
                distinctGroups.Add(indexer.Select(n => chunk[i, n]).ToList());
                distinctGroups.Add(indexer.Select(n => chunk[n, i]).ToList());
                foreach (var n in indexer)
                {
                    allGrids.Add(chunk[i][n]);
                }
            }
            distinctGroups.Add(indexer.Select(i => chunk[i, i]).ToList());
            distinctGroups.Add(indexer.Select(i => chunk[i, 8 - i]).ToList());
        }
        foreach (var grid in allGrids)
        {
            grid.AddOnSelected(OnSelectedGrid);
        }
        for (var i = 0; i < 9; ++i)
        {
            var ii = i;
            numberButtons[ii].onClick.AddListener(() => OnClickedNumberButton(ii + 1));
        }
        eraseButton.onClick.AddListener(OnClickedEraseButton);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
        {
            OnClickedEraseButton();
        }
        for (var i = 1; i <= 9; ++i)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0 + i) || Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                OnClickedNumberButton(i);
            }
        }
        if (selectedGrid?.LeftGrid != null && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnSelectedGrid(selectedGrid.LeftGrid);
        }
        if (selectedGrid?.UpGrid != null && Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnSelectedGrid(selectedGrid.UpGrid);
        }
        if (selectedGrid?.RightGrid != null && Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnSelectedGrid(selectedGrid.RightGrid);
        }
        if (selectedGrid?.DownGrid != null && Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnSelectedGrid(selectedGrid.DownGrid);
        }
    }

    private void CheckDistinct()
    {
        foreach (var grid in allGrids)
        {
            grid.IsRedundant = false;
            grid.IsHighlighted = false;
            if (selectedGrid.IsValid && grid.IsValid)
            {
                grid.IsHighlighted = grid.Number == selectedGrid.Number;
            }
        }
        foreach (var group in distinctGroups)
        {
            foreach (var grid in group
                .Where(i => i.IsValid)
                .GroupBy(i => i.Number)
                .Where(i => i.Count() > 1)
                .SelectMany(i => i.ToArray()))
            {
                grid.IsRedundant = true;
            }
        }
        RefreshAllGrids();
    }

    private void RefreshAllGrids()
    {
        foreach (var grid in allGrids)
        {
            grid.Refresh();
        }
        if (!allGrids.Any(i => !i.IsValid || i.IsRedundant))
        {
            clearedObject.SetActive(true);
        }
    }

    private void OnSelectedGrid(SudokuGrid grid)
    {
        selectedGrid = grid;
        foreach (var g in allGrids)
        {
            g.IsSelected = false;
            g.IsHighlighted = false;
            if (selectedGrid.IsValid && g.IsValid)
            {
                g.IsHighlighted = g.Number == selectedGrid.Number;
            }
        }
        selectFrame.SetActive(true);
        selectFrame.transform.position = selectedGrid.transform.position;
        selectedGrid.IsSelected = true;
        RefreshAllGrids();
    }

    private void OnClickedNumberButton(int number)
    {
        if (selectedGrid == null)
        {
            return;
        }
        selectedGrid.Input(number);
        CheckDistinct();
    }

    private void OnClickedEraseButton()
    {
        if (selectedGrid == null)
        {
            return;
        }
        selectedGrid.EraseInput();
        CheckDistinct();
    }

    public void ResetInput()
    {
        foreach (var g in allGrids)
        {
            g.EraseInput();
        }
        CheckDistinct();
    }

    public void Exit()
    {
        exitDialog.SetActive(true);
    }

    public void ExitYes()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitCancel()
    {
        exitDialog.SetActive(false);
    }
}
