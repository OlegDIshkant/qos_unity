using Qos.Domain;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene;
using Qos.Interaction.ViaGraphicScene.SceneObjects;


namespace GameScene
{
    public class GameSceneObjectsFactory : IGraphicSceneFactory
    {
        private IPlayerAppearenceGeneration _playerAppearenceProvider = new RndPlayerAppearenceProvider();

        public IPlayer CreatePlayer(PlayerModel model) => new Player(model, _playerAppearenceProvider.CreateNew());
        public ICard CreateCard(CardModel cardModel) => new StandartPlayingCard((StandartPlayingCardModel)cardModel, new StandartPlayingCardTexturesProvider());
        public IDeck CreateDeck() => new Deck();
        public ICamera CreateCamera() => new Camera();
        public IWinLooseAnnouncer CreateWinLooseAnnouncer() => new WinLooseAnnouncer();
        public IPlayField CreatePlayField() => new PlayField();
        public ICursor CreateCursor() => new Cursor();
        public IDiscModeUi CreateDiscModeUi() => new DiscModeUI();
        public ICardHeap CreateCardHeap() => new CardHeap();
        public IExitUi CreateExitUi() => new ExitUi();
    }

}