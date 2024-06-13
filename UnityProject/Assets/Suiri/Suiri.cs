using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Suiri : MonoBehaviour
{
    [SerializeField] private GameObject judgeDialog;
    [SerializeField] private GameObject failureDialog;
    [SerializeField] private GameObject clearedObject;
    [SerializeField] private SuiriGridBlock[] blocks;
    [SerializeField] private GameObject exitDialog;

    public static bool IsEraseMode = false;

    private void Update()
    {
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            IsEraseMode = false;
        }
    }

    public void Judge()
    {
        judgeDialog.SetActive(true);
    }

    public void JudgeCancel()
    {
        judgeDialog.SetActive(false);
    }

    public void JudgeYes()
    {
        judgeDialog.SetActive(false);
        if (blocks.Any(b => !b.IsCorrect()))
        {
            failureDialog.SetActive(true);
        }
        else
        {
            clearedObject.SetActive(true);
        }
    }

    public void Retry()
    {
        foreach (var block in blocks)
        {
            block.Clear();
        }
        failureDialog.SetActive(false);
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
