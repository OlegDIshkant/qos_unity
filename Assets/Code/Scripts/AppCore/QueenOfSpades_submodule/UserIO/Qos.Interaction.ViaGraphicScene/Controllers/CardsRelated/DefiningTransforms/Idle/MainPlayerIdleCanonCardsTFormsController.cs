using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.CanonCardTransforms;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{

    /// <summary>
    /// Контроллер, определяющий канонические положения карт главного игрока, когда тот ничем особенным не занят.
    /// </summary>
    public class MainPlayerIdleCanonCardsTFormsController : CanonTFormsController<CardId>
    {
        public MainPlayerIdleCanonCardsTFormsController(
            Contexts contexts,
            PlayerId playerId,
            ICameraController cameraController,
            ICardsActionsProvider cardsActionsProvider) :
            base(
                contexts,
                new MainPlayerCanonCardsTFormsControllerImpl(
                    cameraController,
                    new MainPlayerIdleCardTFormsCalcer(),
                    new WhenIdleBeginCalcTFormsStartegy(playerId, cardsActionsProvider)),
                new CalcIndexInConstRndOrder(contexts.CardIdOrder))
        {
        }
    }



    /// <summary>
    /// Реализация контроллера, определяющего канонические положения карт главного игрока, когда тот отдыхает.
    /// </summary>
    public sealed class MainPlayerCanonCardsTFormsControllerImpl : CanonTFormsControllerImpl<CardId>
    {
        private readonly ICameraController _cameraController;
        private readonly IWithCamParams _tFormsCalcer;

        public MainPlayerCanonCardsTFormsControllerImpl(
            ICameraController cameraController,
            IWithCamParams tFormsCalcer,
            IBeginCalcTFormsStartegy<CardId> beginCalcStrategy) :
            base(
                beginCalcStrategy,
                tFormsCalcer)
        {
            _tFormsCalcer = tFormsCalcer;
            _tFormsCalcer.CamParams = cameraController.CameraInfo.Params;
            _cameraController = cameraController;
        }


        protected override void BeforeUpdate()
        {
            base.BeforeUpdate();
            CheckOutCamera();
        }


        private void CheckOutCamera()
        {
            if (_cameraController.CameraInfo?.ParamsChanged ?? false)
            {
                _tFormsCalcer.CamParams = _cameraController.CameraInfo.Params;
            }
        }
    }
}
