using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanMark : MonoBehaviour
{
    [SerializeField] private Sokoban.Color color;

    public Sokoban.Color Color => color;
}
