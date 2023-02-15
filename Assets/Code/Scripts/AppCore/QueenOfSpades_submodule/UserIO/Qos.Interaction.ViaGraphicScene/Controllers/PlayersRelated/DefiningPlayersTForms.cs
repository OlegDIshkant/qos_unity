using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    
    /// <summary>
    /// Контроллер, вычисляющий, какое расположение в пространстве должен в данный момент времени иметь тот или иной игрок,
    /// и некоторую другую сопутствующую информацию.
    /// </summary>
    public interface IPlayersTFormsDefiner
    {
        /// <summary>
        /// Какие положения игроков в пространстве должны быть сейчас.
        /// </summary>
        DictionaryData<PlayerId, Transform> TFormsToApply { get; }
        /// <summary>
        /// Канонические положения игроков в пространстве.
        /// </summary>
        DictionaryData<PlayerId, Transform> CanonTForms { get; }
    }



    public class DefiningPlayersTForms : ControllersController, IPlayersTFormsDefiner
    {
        private readonly IDictionaryDataProvider<PlayerId, Transform> _canonTFormsProvider;
        private readonly IDictionaryDataProvider<PlayerId, Transform> _actualTFormsProvider;

        public DictionaryData<PlayerId, Transform> TFormsToApply => _actualTFormsProvider.DictionaryOutput;
        public DictionaryData<PlayerId, Transform> CanonTForms => _canonTFormsProvider.DictionaryOutput;


        public DefiningPlayersTForms(
            Contexts contexts,
            ICameraController cam,
            IPlayFieldController playField,
            IPlayerActionsProvider playersActionsProvider,
            IDictionaryDataProvider<PlayerId, Transform> prevPlayersTFormsProvider,
            IHashSetDataProvider<PlayerId> inGamePlayersProvider) :
            base(contexts)
        {
            var queue = new ControllersQueue();
            var canonTformsProvider = queue.AddWithoutTag(new DefiningCanonPlayerTforms(contexts, playField, inGamePlayersProvider));
            var transferTFormsProvider = queue.AddWithoutTag(new DefiningTransferModePlayerTforms(contexts, cam, canonTformsProvider, playersActionsProvider));
            var transitTforms = queue.AddWithoutTag(new DefiningTransitPlayerTForms(contexts, playersActionsProvider, canonTformsProvider, transferTFormsProvider, prevPlayersTFormsProvider));
            var finalTForms = queue.AddWithoutTag(new PlayersTFormsSelector(contexts, playersActionsProvider, canonTformsProvider, transferTFormsProvider, transitTforms));
            SetControllersQueue(queue);

            _canonTFormsProvider = canonTformsProvider;
            _actualTFormsProvider = finalTForms;
        }

    }


}