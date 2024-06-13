using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChessPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void SetText(string t)
    {
        text.text = t;
    }
}
