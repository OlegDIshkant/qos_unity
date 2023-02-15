using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Модифицирует положения карт игрока так, чтобы создать визуальный эффект будто они слегка парят в воздухе.
    /// </summary>
    public class Player_CardTForm_FloatAnimModifier : TFormsModifier<CardId>
    {
        public Player_CardTForm_FloatAnimModifier(
            Contexts contexts,
            IDictionaryDataProvider<CardId, Transform> originalTFormsProvider) :
            base(
                contexts,
                originalTFormsProvider,
                new AllwaysModifyStrategy(),
                new FloatAnimModifyStrategy<CardId>(contexts.TimeContext))
        {
        }
    }


    /// <summary>
    /// Модифицирует положения карт главного игрока (пользователя) так, чтобы создать визуальный эффект будто они слегка парят в воздухе.
    /// </summary>
    public class MainPlayer_CardTForm_FloatAnimModifier : TFormsModifier<CardId>
    {
        public MainPlayer_CardTForm_FloatAnimModifier(
            Contexts contexts,
            IDictionaryDataProvider<CardId, Transform> originalTFormsProvider) :
            base(
                contexts,
                originalTFormsProvider,
                new AllwaysModifyStrategy(),
                new FloatAnimModifyStrategy<CardId>(contexts.TimeContext))
        {
        }
    }
}