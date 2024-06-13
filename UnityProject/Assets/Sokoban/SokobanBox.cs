using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SokobanBox : MonoBehaviour, SokobanUndoable
{
    [SerializeField] private Sokoban.Color color;
    [SerializeField] private MeshRenderer meshRenderer;

    private Vector3 startPos;
    private List<Vector3> posHistory = new List<Vector3>();

    private float colorChangeDuration = 0.2f;

    private bool isMarked = false;
    public bool IsMarked
    {
        get => isMarked;
        set
        {
            if (isMarked == value)
            {
                return;
            }
            isMarked = value;
            meshRenderer.material.DOColor(GetEmissionColor(), "_EmissionColor", colorChangeDuration);
        }
    }

    public Sokoban.Color Color => color;

    private void Start()
    {
        startPos = transform.position;
        CheckMark();
        posHistory.Add(startPos);
    }

    private void Update()
    {
        
    }

    private Color GetEmissionColor()
    {
        if (!IsMarked)
        {
            return new Color(0f, 0f, 0f);
        }
        return color switch
        {
            Sokoban.Color.Red => new Color(0.35f, 0f, 0f),
            Sokoban.Color.Green => new Color(0f, 0.35f, 0f),
            Sokoban.Color.Blue => new Color(0f, 0f, 0.35f),
            Sokoban.Color.White => new Color(0.35f, 0.35f, 0.35f),
            _ => new Color(0f, 0f, 0f)
        };
    }

    public bool CheckMark()
    {
        var mark = Physics.OverlapSphere(transform.position, Sokoban.PhysicsCheckRadius, Sokoban.MarkLayer).FirstOrDefault()?.GetComponent<SokobanMark>();
        var hit = mark && mark.Color == color;
        if (IsMarked == hit)
        {
            return false;
        }
        IsMarked = hit;
        return true;
    }

    public bool CheckFall(TweenCallback callback)
    {
        if (Physics.CheckSphere(transform.position + Vector3.down, Sokoban.PhysicsCheckRadius, Sokoban.BlockLayer | Sokoban.BoxLayer | Sokoban.GateLayer))
        {
            return false;
        }
        transform.DOMove(transform.position + Vector3.down, Sokoban.FallSpeed)
            .SetEase(Ease.InQuad)
            .OnComplete(callback);
        return true;
    }

    public void RecordMove(int pointer)
    {
        if (posHistory.Count > pointer)
        {
            posHistory.RemoveRange(pointer, posHistory.Count - pointer);
        }
        posHistory.Add(transform.position);
    }

    public void JumpMove(int pointer)
    {
        transform.position = posHistory[pointer];
        CheckMark();
    }

    public void ResetMove()
    {
        transform.position = startPos;
        CheckMark();
        posHistory.Clear();
        posHistory.Add(startPos);
    }
}
