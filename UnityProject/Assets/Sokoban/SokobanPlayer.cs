using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SokobanPlayer : MonoBehaviour, SokobanUndoable
{
    [SerializeField] private Transform modelOrigin;
    [SerializeField] private Animator modelAnimator;
    [SerializeField] private Sokoban sokoban;

    private Vector3 startPos;
    private float startWay;
    private List<Vector3> posHistory = new List<Vector3>();
    private List<float> wayHistory = new List<float>();

    private float walkSpeed = 0.2f;
    private float pushSpeed = 0.3f;
    private Ease walkEase = Ease.Linear;
    private Ease pushEase = Ease.Linear;

    private bool allowInput = true;
    private bool isControlable = true;
    private bool isWalking = false;
    private bool isPushing = false;

    private float idleAnimationDelay = 0.02f;
    private Tween idleAnimationTween;

    public bool AllowInput => allowInput;

    public bool IsControlable => isControlable;

    public bool IsWalking
    {
        get => isWalking;
        set
        {
            isWalking = value;
            modelAnimator.SetBool("IsWalking", isWalking);
        }
    }

    public bool IsPushing
    {
        get => isPushing;
        set
        {
            isPushing = value;
            modelAnimator.SetBool("IsPushing", isPushing);
        }
    }

    private void Start()
    {
        startPos = transform.position;
        posHistory.Add(startPos);
        wayHistory.Add(startWay);
    }

    public bool MoveUp()
    {
        return Move(Vector3.forward, 0f);
    }

    public bool MoveRight()
    {
        return Move(Vector3.right, 90f);
    }

    public bool MoveDown()
    {
        return Move(Vector3.back, 180f);
    }

    public bool MoveLeft()
    {
        return Move(Vector3.left, 270f);
    }

    private bool Move(Vector3 move, float rotation)
    {
        modelOrigin.localRotation = Quaternion.Euler(0f, rotation, 0f);

        if (Physics.CheckSphere(transform.position + move, Sokoban.PhysicsCheckRadius, Sokoban.BlockLayer | Sokoban.GateLayer))
        {
            return false;
        }

        if (!Physics.Raycast(transform.position + move, Vector3.down, 2f, Sokoban.BlockLayer | Sokoban.GateLayer | Sokoban.BoxLayer))
        {
            return false;
        }

        if (Physics.CheckSphere(transform.position + move, Sokoban.PhysicsCheckRadius, Sokoban.BoxLayer))
        {
            if (Physics.CheckSphere(transform.position + move + move, Sokoban.PhysicsCheckRadius, Sokoban.BlockLayer | Sokoban.GateLayer | Sokoban.BoxLayer))
            {
                return false;
            }
            if (!Physics.Raycast(transform.position + move + move, Vector3.down, 2f, Sokoban.BlockLayer | Sokoban.GateLayer | Sokoban.BoxLayer))
            {
                return false;
            }
            DoPush(move);
            return true;
        }

        DoWalk(move);

        return true;
    }

    private void DoWalk(Vector3 move)
    {
        idleAnimationTween?.Kill();
        allowInput = false;
        IsWalking = true;
        IsPushing = false;
        transform.DOMove(transform.position + move, walkSpeed)
            .SetEase(walkEase)
            .OnComplete(() =>
            {
                idleAnimationTween = DOVirtual.DelayedCall(idleAnimationDelay, () => { IsWalking = IsPushing = false; });
                if (Physics.CheckSphere(transform.position + Vector3.down, Sokoban.PhysicsCheckRadius, Sokoban.BlockLayer | Sokoban.GateLayer | Sokoban.BoxLayer))
                {
                    allowInput = true;
                    sokoban.RecordState();
                }
                else
                {
                    transform.DOMove(transform.position + Vector3.down, Sokoban.FallSpeed)
                        .SetEase(Ease.InQuad)
                        .OnComplete(() =>
                        {
                            allowInput = true;
                            sokoban.RecordState();
                        });
                }
            });
    }

    private void DoPush(Vector3 move)
    {
        idleAnimationTween?.Kill();
        var box = Physics.OverlapSphere(transform.position + move, Sokoban.PhysicsCheckRadius, Sokoban.BoxLayer).First().GetComponent<SokobanBox>();
        var box2 = Physics.OverlapSphere(transform.position + move + Vector3.up, Sokoban.PhysicsCheckRadius, Sokoban.BoxLayer).FirstOrDefault()?.GetComponent<SokobanBox>();
        allowInput = false;
        IsWalking = false;
        IsPushing = true;
        box.transform.DOMove(box.transform.position + move, pushSpeed)
            .SetEase(pushEase);
        if (box2)
        {
            box2.transform.DOMove(box.transform.position + move, pushSpeed)
                .SetEase(pushEase);
        }
        transform.DOMove(transform.position + move, pushSpeed)
            .SetEase(pushEase)
            .OnComplete(() =>
            {
                if (!box.CheckFall(() =>
                {
                    if (box.CheckMark())
                    {
                        sokoban.MoveGates(box.Color, box.IsMarked, () =>
                        {
                            allowInput = true;
                            sokoban.RecordState();
                        });
                    }
                    else
                    {
                        allowInput = true;
                        sokoban.RecordState();
                    }
                }))
                {
                    if (box.CheckMark())
                    {
                        sokoban.MoveGates(box.Color, box.IsMarked, () =>
                        {
                            allowInput = true;
                            sokoban.RecordState();
                        });
                    }
                    else
                    {
                        allowInput = true;
                        sokoban.RecordState();
                    }
                }
                idleAnimationTween = DOVirtual.DelayedCall(idleAnimationDelay, () => { IsWalking = IsPushing = false; });
            });
        
    }

    public void RecordMove(int pointer)
    {
        if (posHistory.Count > pointer)
        {
            posHistory.RemoveRange(pointer, posHistory.Count - pointer);
        }
        posHistory.Add(transform.position);
        if (wayHistory.Count > pointer)
        {
            wayHistory.RemoveRange(pointer, posHistory.Count - pointer);
        }
        wayHistory.Add(modelOrigin.localEulerAngles.y);
    }

    public void JumpMove(int pointer)
    {
        transform.position = posHistory[pointer];
        modelOrigin.localRotation = Quaternion.Euler(0f, wayHistory[pointer], 0f);
    }

    public void ResetMove()
    {
        transform.position = startPos;
        modelOrigin.localRotation = Quaternion.Euler(0f, startWay, 0f);
        posHistory.Clear();
        posHistory.Add(startPos);
        wayHistory.Clear();
        wayHistory.Add(startWay);
    }
}
