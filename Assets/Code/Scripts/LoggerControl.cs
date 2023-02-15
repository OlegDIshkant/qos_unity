using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = CommonTools.Logger;

/// <summary>
/// Позволяет пользователю управлять логированием (например, включать и выключать в процессе работы программы).
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
            UnityEngine.Debug.Log("Логирование включено.");
        }
        else
        {
            Logger.Pause();
            UnityEngine.Debug.Log("Логирование выключено.");
        }
    }


    private void SetUpLogger()
    {
        Application.quitting += () => Logger.Close();
    }
}
