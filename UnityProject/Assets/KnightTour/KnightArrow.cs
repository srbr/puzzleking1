using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KnightArrow : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Vector2Int moveVector;

    public Vector2Int MoveVector => moveVector;
    private Action<Vector2Int> onClickEvent;

    public void AddClickEvent(Action<Vector2Int> e)
    {
        onClickEvent += e;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickEvent(moveVector);
    }
}
