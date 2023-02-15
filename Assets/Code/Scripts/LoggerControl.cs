using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = CommonTools.Logger;

/// <summary>
/// ��������� ������������ ��������� ������������ (��������, �������� � ��������� � �������� ������ ���������).
/// </summary>
public class LoggerControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        SetUpLogger();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchLogger();
        }
    }


    private void SwitchLogger()
    {
        if (Logger.IsPaused)
        {
            Logger.UnPause();
            UnityEngine.Debug.Log("����������� ��������.");
        }
        else
        {
            Logger.Pause();
            UnityEngine.Debug.Log("����������� ���������.");
        }
    }


    private void SetUpLogger()
    {
        Application.quitting += () => Logger.Close();
    }
}
