using CommonTools.Math;
using System.Numerics;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Расчитывать положения карт игрока так, чтобы визуально показать, как они отодвигаются "на второй план".
    /// </summary>
    public class Player_HideMode_TFormsCalcer : PlayerIdleTFormsCalcer
    {
        protected override float MaxDegBetweenCards => 2f;

        public Player_HideMode_TFormsCalcer(Transform playerTransform) :
            base(
                playerTransform,
                new Params()
                {
                    Scale = new Vector3(0.2f, 0.2f, 0.2f)
                })
        {
        }

    }
}