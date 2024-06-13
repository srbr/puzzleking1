using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class SokobanGate : MonoBehaviour, SokobanUndoable
{
    [SerializeField] private Sokoban.Color color;
    [SerializeField] private Transform movablePart;
    [SerializeField] private bool isFlip;

    private bool isOn = false;

    private Vector3 startPos;
    private bool startOn;
    private List<Vector3> posHistory = new List<Vector3>();
    private List<bool> onHistory = new List<bool>();

    public Sokoban.Color Color => color;

    private void Start()
    {
        startPos = transform.position;
        startOn = isOn;
        posHistory.Add(startPos);
        onHistory.Add(startOn);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            movablePart.localPosition = isFlip ? Vector3.up : Vector3.zero;
        }
#endif
    }

    public void Move(bool isOn)
    {
        foreach (var rider in Physics.OverlapSphere(movablePart.position + Vector3.up, Sokoban.PhysicsCheckRadius, Sokoban.BoxLayer | Sokoban.PlayerLayer))
        {
            rider.transform.DOMove(rider.transform.position + (isOn == isFlip ? Vector3.down : Vector3.up), Sokoban.GateMoveSpeed)
                .SetEase(Ease.Linear);
        }
        movablePart.DOMove(transform.position + (isOn == isFlip ? Vector3.zero : Vector3.up), Sokoban.GateMoveSpeed)
            .SetEase(Ease.Linear);
        this.isOn = isOn;
    }

    public void RecordMove(int pointer)
    {
        if (posHistory.Count > pointer)
        {
            posHistory.RemoveRange(pointer, posHistory.Count - pointer);
        }
        posHistory.Add(transform.position);

        if (onHistory.Count > pointer)
        {
            onHistory.RemoveRange(pointer, posHistory.Count - pointer);
        }
        onHistory.Add(isOn);
    }

    public void JumpMove(int pointer)
    {
        transform.position = posHistory[pointer];
        isOn = onHistory[pointer];
        movablePart.position = transform.position + (isOn == isFlip ? Vector3.zero : Vector3.up);
    }

    public void ResetMove()
    {
        transform.position = startPos;
        isOn = startOn;
        movablePart.position = transform.position + (isOn == isFlip ? Vector3.zero : Vector3.up);
        posHistory.Clear();
        posHistory.Add(startPos);
        onHistory.Clear();
        onHistory.Add(startOn);
    }
}
