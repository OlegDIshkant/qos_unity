using System.Numerics;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Расчитывать положения карт главного игрока (пользователя) так, чтобы визуально показать, как они отодвигаются "на второй план".
    /// </summary>
    internal class MainPlayer_HideMode_TFormsCalcer : MainPlayerIdleCardTFormsCalcer
    {
        protected override Vector3 LocalPositionOffset => new Vector3(0, -0.9f, 0);
    }
}