using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] private int passcode;
    [SerializeField] private int sceneId;
    [SerializeField] private PasscodeDialog dialog;
    public void OnClickedButton()
    {
        dialog.ShowDialog(passcode, sceneId);
    }
}
