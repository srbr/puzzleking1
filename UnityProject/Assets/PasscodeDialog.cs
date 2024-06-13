using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PasscodeDialog : MonoBehaviour
{
    [SerializeField] private GameObject backgroundObject;
    [SerializeField] private TMP_InputField passcodeInput;
    [SerializeField] private GameObject errorText;
    private int passcode;
    private int sceneId;
    public void ShowDialog(int passcode, int sceneId)
    {
        this.passcode = passcode;
        this.sceneId = sceneId;
        backgroundObject.SetActive(true);
    }

    public void OnClickedStart()
    {
        if (passcodeInput.text.Equals(passcode.ToString()))
        {
            SceneManager.LoadScene(sceneId);
        }
        else
        {
            errorText.SetActive(true);
        }
    }

    public void OnClickedCancel()
    {
        backgroundObject.SetActive(false);
        errorText.SetActive(false);
        passcodeInput.text = string.Empty;
    }
}
