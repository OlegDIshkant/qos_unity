using Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using System;


namespace Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects
{
    public interface IDeckController
    {
        IDeck_ReadOnly DeckInfo { get; }
    }



    public class DeckController : AbstractController, IDeckController
    {
        private readonly Func<IDeck> _DeckFactoryMethod;
        private readonly IPlayersTFormsDefiner _playerTformsDefiner;

        private IDeck _deck;

        public IDeck_ReadOnly DeckInfo => _deck;


        public DeckController(
            Contexts contexts,
            Func<IDeck> DeckFactoryMethod,
            IPlayersTFormsDefiner playerTformsDefiner) :
            base(contexts)
        {
            _DeckFactoryMethod = DeckFactoryMethod;
            _playerTformsDefiner = playerTformsDefiner;
        }


        public override void Update()
        {
            if (PositionShouldBeChanged())
            {
                UpdatePosition();
            }
        }

        private bool PositionShouldBeChanged() => _playerTformsDefiner?.CanonTForms?.HasChanged ?? false;


        private void UpdatePosition()
        {
            MakeSureDeckCreated();

            var canonPlayerTransforms = _playerTformsDefiner.CanonTForms.Items;
            var deckPosition = new DeckTFormCalcer(canonPlayerTransforms.Values).Calc();
            _deck.SetTransform(deckPosition);
        }



        private void MakeSureDeckCreated()
        {
            if (_deck == null)
            {
                _deck = _DeckFactoryMethod();
            }
        }

    }
}