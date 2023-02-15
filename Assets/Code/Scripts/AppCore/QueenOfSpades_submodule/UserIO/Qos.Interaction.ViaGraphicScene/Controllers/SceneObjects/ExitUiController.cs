using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;


namespace Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects
{
    /// <summary>
    ///  онтроллер, управл€ющий ui дл€ выхода в главное меню.
    /// </summary>
    public class ExitUiController : AbstractController
    {
        private readonly Func<IExitUi> _ExitUiFactoryMethod;
        private readonly Action _OnExitRequested;

        private bool _uiSpawned = false;


        public ExitUiController(
            Contexts contexts,
            Func<IExitUi> ExitUiFactoryMethod,
            Action OnExitRequested) : 
            base(contexts)
        {
            _ExitUiFactoryMethod = ExitUiFactoryMethod;
            _OnExitRequested = OnExitRequested;
        }


        public override void Update()
        {
            SpawnUiIfNeeded();
        }


        private void SpawnUiIfNeeded()
        {
            if (!_uiSpawned)
            {
                SpawnUi();
                _uiSpawned = true;
            }
        }


        private void SpawnUi()
        {
            var exitUi = _ExitUiFactoryMethod();
            exitUi.OnExitRequested += ExitUi_OnExitRequested;
        }


        private void ExitUi_OnExitRequested(IExitUi exitUi)
        {
            exitUi.OnExitRequested -= ExitUi_OnExitRequested;
            _OnExitRequested?.Invoke();
        }

    }

}