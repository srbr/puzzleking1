using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Sokoban : MonoBehaviour
{
    public enum Color
    {
        Red,
        Green,
        Blue,
        White
    }

    [System.Serializable]
    public class ColorMaterialPair
    {
        [SerializeField] private Color color;
        [SerializeField] private Material material;

        public Color Color => color;
        public Material Material => material;
    }

    [SerializeField] private SokobanPlayer player;
    [SerializeField] private GameObject exitDialog;
    [SerializeField] private GameObject clearedObject;
    [SerializeField] private Button undoButton;
    [SerializeField] private Button redoButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private TMP_Text clearTimeText;

    private bool isCleared = false;

    private float timer = 0f;
    private float nextAutoSaveTime = 0f;
    private float autoSaveInterval = 10f;

    private SokobanBox[] boxes;
    private SokobanGate[] gates;
    private SokobanMark[] marks;

    private int pointer;
    private int pointerMax;

    public static float GateMoveSpeed = 0.2f;
    public static float FallSpeed = 0.2f;

    public static int BlockLayer = 1 << 6;
    public static int BoxLayer = 1 << 7;
    public static int GateLayer = 1 << 8;
    public static int MarkLayer = 1 << 9;
    public static int PlayerLayer = 1 << 10;

    public static float PhysicsCheckRadius = 0.1f;

    private void Start()
    {
        boxes = FindObjectsOfType<SokobanBox>();
        gates = FindObjectsOfType<SokobanGate>();
        marks = FindObjectsOfType<SokobanMark>();
        if (PlayerPrefs.HasKey("SokobanTime"))
        {
            timer = PlayerPrefs.GetFloat("SokobanTime");
        }
        nextAutoSaveTime = timer + autoSaveInterval;
    }

    private void Update()
    {
        if (isCleared)
        {
            return;
        }
        if (player.IsControlable)
        {
            CheckInput();
        }
        timer += Time.deltaTime;
        if (timer > nextAutoSaveTime)
        {
            nextAutoSaveTime = timer + autoSaveInterval;
            PlayerPrefs.SetFloat("SokobanTime", timer);
        }
    }

    private void CheckInput()
    {
        if (!player.AllowInput)
        {
            return;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (player.MoveUp())
            {
                return;
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (player.MoveRight())
            {
                return;
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (player.MoveDown())
            {
                return;
            }
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (player.MoveLeft())
            {
                return;
            }
        }
    }

    public void MoveGates(Color color, bool isOn, TweenCallback callback)
    {
        foreach (var g in gates.Where(i => i.Color == color))
        {
            g.Move(isOn);
        }
        DOVirtual.DelayedCall(GateMoveSpeed, callback);
    }

    public void RecordState()
    {
        ++pointer;
        pointerMax = pointer;
        player.RecordMove(pointer);
        foreach (var b in boxes)
        {
            b.RecordMove(pointer);
        }
        foreach (var g in gates)
        {
            g.RecordMove(pointer);
        }
        undoButton.interactable = true;
        redoButton.interactable = false;

        if (!boxes.Any(b => !b.IsMarked))
        {
            isCleared = true;
            var timeSpan = TimeSpan.FromSeconds(timer);
            clearTimeText.text = string.Format("クリアタイム：{0:D2}時間{1:D2}分{2:D2}秒{3:D3}",
                             timeSpan.Hours,
                             timeSpan.Minutes,
                             timeSpan.Seconds,
                             timeSpan.Milliseconds);
            clearedObject.SetActive(true);
        }
    }

    public void ResetInput()
    {
        player.ResetMove();
        foreach (var b in boxes)
        {
            b.ResetMove();
        }
        foreach (var g in gates)
        {
            g.ResetMove();
        }
        pointer = pointerMax = 0;
        undoButton.interactable = false;
        redoButton.interactable = false;
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

    public void Undo()
    {
        if (pointer <= 0)
        {
            return;
        }
        --pointer;
        player.JumpMove(pointer);
        foreach (var b in boxes)
        {
            b.JumpMove(pointer);
        }
        foreach (var g in gates)
        {
            g.JumpMove(pointer);
        }

        redoButton.interactable = true;
        if (pointer <= 0)
        {
            undoButton.interactable = false;
        }
    }

    public void Redo()
    {
        if (pointer >= pointerMax)
        {
            return;
        }
        ++pointer;
        player.JumpMove(pointer);
        foreach (var b in boxes)
        {
            b.JumpMove(pointer);
        }
        foreach (var g in gates)
        {
            g.JumpMove(pointer);
        }

        undoButton.interactable = true;
        if (pointer >= pointerMax)
        {
            redoButton.interactable = false;
        }
    }
}