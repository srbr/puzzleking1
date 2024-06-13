using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuiriGrid : MonoBehaviour
{
    public enum State
    {
        Empty,
        True,
        False,
    }

    [SerializeField] private GameObject trueObject, falseObject;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private bool answer;

    private SuiriGridBlock parent;
    public SuiriGridBlock Parent
    {
        get => parent;
        set => parent = value;
    }

    private bool isRedundant = false;
    public bool IsRedundant
    {
        get => isRedundant;
        set => isRedundant = value;
    }

    private State currentState = State.Empty;
    public State CurrentState
    {
        get => currentState;
        set
        {
            currentState = value;
            trueObject.SetActive(currentState == State.True);
            falseObject.SetActive(currentState == State.False);
            Parent.RefreshGrids();
        }
    }

    public void PointerDown()
    {
        if (currentState == State.Empty)
        {
            if (Input.GetMouseButton(0))
            {
                CurrentState = State.True;
                Suiri.IsEraseMode = false;
            }
            else if (Input.GetMouseButton(1))
            {
                CurrentState = State.False;
                Suiri.IsEraseMode = false;
            }
        }
        else
        {
            CurrentState = State.Empty;
            Suiri.IsEraseMode = true;
        }
    }

    public void PointerEnter()
    {
        if (Suiri.IsEraseMode)
        {
            CurrentState = State.Empty;
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                CurrentState = State.True;
            }
            else if (Input.GetMouseButton(1))
            {
                CurrentState = State.False;
            }
        }
    }

    public void Refresh()
    {
        backgroundImage.color = isRedundant ? new Color(1f, 0.5f, 0.5f) : Color.white;
    }

    public bool IsCorrect()
    {
        return CurrentState == State.True == answer;
    }
}
