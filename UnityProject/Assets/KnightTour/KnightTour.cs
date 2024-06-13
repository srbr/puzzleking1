using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KnightTour : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Vector2Int start;
    [SerializeField, Multiline] private string board;
    [SerializeField] private GameObject whitePanelPrefab;
    [SerializeField] private GameObject blackPanelPrefab;
    [SerializeField] private PlayerKnight player;
    [SerializeField] private KnightArrow[] arrows;
    [SerializeField] private GameObject clearedObject;
    [SerializeField] private GameObject exitDialog;

    private Dictionary<Vector2Int, ChessPanel> panels = new Dictionary<Vector2Int, ChessPanel>();
    private Stack<Vector2Int> moves = new Stack<Vector2Int>();
    private bool isMoving = false;
    private Vector2Int currentPosition;
    private float offsetX;
    private float offsetZ;

    private void Start()
    {
        offsetX = (1 - width) * 0.5f;
        offsetZ = (1 - height) * 0.5f;
        var boardRows = board.Split("\n");
        for (var iy = 0; iy < height; ++iy)
        {
            for (var ix = 0; ix < width; ++ix)
            {
                if (boardRows[iy][ix] == '¡')
                {
                    var panel = Instantiate((ix + iy) % 2 == 0 ? whitePanelPrefab : blackPanelPrefab).GetComponent<ChessPanel>();
                    panel.transform.position = new Vector3(offsetX + ix, 0, offsetZ + iy);
                    panels.Add(new Vector2Int(ix, iy), panel);
                }
            }
        }
        player.transform.position = new Vector3(offsetX + start.x, 0, offsetZ + start.y);
        moves.Push(start);
        foreach (var a in arrows)
        {
            a.AddClickEvent(Move);
        }
        currentPosition = start;
        panels[currentPosition].SetText(moves.Count.ToString());
        UpdateArrows();
    }

    public void Move(Vector2Int moveVector)
    {
        if (isMoving)
        {
            return;
        }
        if (!panels.ContainsKey(currentPosition + moveVector))
        {
            return;
        }
        if (moves.Contains(currentPosition + moveVector))
        {
            return;
        }
        isMoving = true;
        HideArrows();
        currentPosition += moveVector;
        moves.Push(currentPosition);
        player.Move(new Vector3(offsetX + currentPosition.x, 0, offsetZ + currentPosition.y), () =>
        {
            isMoving = false;
            UpdateArrows();
            panels[currentPosition].SetText(moves.Count.ToString());
            if (panels.Count == moves.Count)
            {
                clearedObject.SetActive(true);
            }
        });
    }

    public void UndoMove()
    {
        if (isMoving)
        {
            return;
        }
        if (moves.Count <= 1)
        {
            return;
        }
        isMoving = true;
        HideArrows();
        panels[currentPosition].SetText(string.Empty);
        moves.Pop();
        currentPosition = moves.Peek();
        player.Move(new Vector3(offsetX + currentPosition.x, 0, offsetZ + currentPosition.y), () =>
        {
            isMoving = false;
            UpdateArrows();
        });
    }

    public void ResetMoves()
    {
        if (isMoving)
        {
            return;
        }
        if (moves.Count <= 1)
        {
            return;
        }
        isMoving = true;
        HideArrows();
        moves.Clear();
        moves.Push(start);
        currentPosition = start;
        foreach (var p in panels.Values)
        {
            p.SetText(string.Empty);
        }
        panels[start].SetText("1");
        player.Move(new Vector3(offsetX + currentPosition.x, 0, offsetZ + currentPosition.y), () =>
        {
            isMoving = false;
            UpdateArrows();
        });
    }

    private void HideArrows()
    {
        foreach (var a in arrows)
        {
            a.gameObject.SetActive(false);
        }
    }

    private void UpdateArrows()
    {
        foreach (var a in arrows)
        {
            a.gameObject.SetActive(panels.ContainsKey(currentPosition + a.MoveVector) && !moves.Contains(currentPosition + a.MoveVector));
        }
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
