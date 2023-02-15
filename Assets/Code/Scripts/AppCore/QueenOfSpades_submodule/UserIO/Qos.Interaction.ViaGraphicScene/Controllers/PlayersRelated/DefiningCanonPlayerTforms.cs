using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    /// <summary>
    /// Контроллер, определяющий канонические положения всех игроков.
    /// </summary>
    public class DefiningCanonPlayerTforms : ControllersController, IDictionaryDataProvider<PlayerId, Transform>
    {
        private readonly IDictionaryDataProvider<PlayerId, Transform> _resultTForms;

        public DictionaryData<PlayerId, Transform> DictionaryOutput => _resultTForms.DictionaryOutput;


        public DefiningCanonPlayerTforms(
            Contexts contexts,
            IPlayFieldController playField,
            IHashSetDataProvider<PlayerId> inGmaePlayersProvider) :
            base(contexts)
        {
            var queue = new ControllersQueue();
            var canonTForms = queue.AddWithoutTag(new CanonPlayerTFormsCalcer(contexts, playField, inGmaePlayersProvider));
            SetControllersQueue(queue);

            _resultTForms = canonTForms;
        }


    }
}