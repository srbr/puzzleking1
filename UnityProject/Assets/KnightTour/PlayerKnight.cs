using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnight : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.2f;
    public void Move(Vector3 position, TweenCallback onComplete)
    {
        transform.DOMove(position, moveSpeed).SetEase(Ease.InOutCubic).OnComplete(onComplete);
    }
}
