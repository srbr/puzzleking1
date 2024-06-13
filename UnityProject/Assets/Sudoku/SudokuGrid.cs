using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class SudokuGrid : MonoBehaviour
{
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private TMP_Text inputText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private bool isHint = false;
    [SerializeField] private int hint = 0;

    private SudokuGrid leftGrid, upGrid, rightGrid, downGrid;

    public SudokuGrid LeftGrid
    {
        get => leftGrid;
        set => leftGrid = value;
    }
    public SudokuGrid UpGrid
    {
        get => upGrid;
        set => upGrid = value;
    }
    public SudokuGrid RightGrid
    {
        get => rightGrid;
        set => rightGrid = value;
    }
    public SudokuGrid DownGrid
    {
        get => downGrid;
        set => downGrid = value;
    }

    private Action<SudokuGrid> onSelected;
    public void AddOnSelected(Action<SudokuGrid> e)
    {
        onSelected += e;
    }

    private bool isSelected = false;
    public bool IsSelected
    {
        get => isSelected;
        set => isSelected = value;
    }

    private bool isInput = false;
    private int input = 0;

    public bool IsValid
    {
        get => isInput || isHint;
    }

    public int Number
    {
        get => isHint ? hint : input;
    }

    private bool isRedundant = false;
    public bool IsRedundant
    {
        get => isRedundant;
        set => isRedundant = value;
    }

    private bool isHighlighted = false;
    public bool IsHighlighted
    {
        get => isHighlighted;
        set => isHighlighted = value;
    }

    public void Input(int n)
    {
        if (isHint)
        {
            return;
        }
        isInput = true;
        input = n;
        inputText.SetText(input.ToString());
    }

    public void EraseInput()
    {
        if (isHint)
        {
            return;
        }
        isInput = false;
        inputText.SetText(string.Empty);
    }

    private void Start()
    {
        hintText.SetText(isHint ? hint.ToString() : string.Empty);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            hintText.SetText(isHint ? hint.ToString() : string.Empty);
        }
#endif
    }

    public void Refresh()
    {
        backgroundImage.color = isRedundant ? new Color(1f, 0.5f, 0.5f) : isHighlighted ? new Color(0.5f, 1f, 0.5f) : Color.white;
    }

    public void OnClick()
    {
        onSelected(this);
    }
}
