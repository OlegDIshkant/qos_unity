using CGA.MainMenu;
using CGA.MainMenu.GUI;
using GameScene;
using GUI;
using QosGameApp.Build;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Главная точка входа для всего приложения на Unity.
/// Она запускает ядро приложения.
/// </summary>
public class EntryPoint : MonoBehaviour
{

    void Start()
    {
        PrepareToDisplayUnobservedExceptions();
        var app = new AppBuilder().BuildApplication(ProvideMainMenu(), () => new GameSceneObjectsFactory());
        app.LaunchAsync().ContinueWith(OnAppFinished, TaskContinuationOptions.ExecuteSynchronously);
    }


    private void OnAppFinished(Task appRunning)
    {
        if (appRunning.IsFaulted)
        {
            Debug.LogError(appRunning.Exception);
        }

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }


    private AbstractMainMenu ProvideMainMenu()
    {
        return new MainMenuWithGUI(new UnityGui("Main menu"));
    }


    private void PrepareToDisplayUnobservedExceptions()
    {
        TaskScheduler.UnobservedTaskException +=
            (sender, args) =>
            {
                Debug.LogError(args.Exception);
            };
    }

}
