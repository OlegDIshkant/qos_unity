using CommonTools;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    /// <summary>
    /// Контроллер, сообщающий, какие карты (объекты, олицетворяющие карты) в данный момент должны существовать в игре.
    /// </summary>
    public class CardsSpawnDirector : AbstractController, IObjectsSpawnDirector<CardId>
    {
        private ICardsActionsProvider _cardsActionsProvider;


        private ListData<CardId>.Editable _shouldBeSpawnedNowEdit;
        public ListData<CardId> ShouldBeSpawnedNow { get; private set; }


        public CardsSpawnDirector(
            Contexts contexts,
            ICardsActionsProvider cardsActionsProvider) : 
            base(contexts)
        {
            _cardsActionsProvider = cardsActionsProvider;

            ShouldBeSpawnedNow = new ListData<CardId>(out _shouldBeSpawnedNowEdit);
        }


        public override void Update()
        {
            foreach (var card in CardsSupposedToStartAppear())
            {
                AskToSpawnCard(card);
            }
        }


        private IEnumerable<CardId> CardsSupposedToStartAppear()
        {
            if (_cardsActionsProvider.NonPlayerCardActions?.HasChanged ?? false)
            {
                return _cardsActionsProvider.NonPlayerCardActions.AddedOrChanged.Where(i => i.Value.IsCreatingNow()).Select(i => i.Key);
            }
            return Enumerable.Empty<CardId>();
        }


        private void AskToSpawnCard(CardId card)
        {
            Logger.Verbose($"Игровой объект для карты '{card}' должен быть создан.");
            _shouldBeSpawnedNowEdit.AddItem(card);
        }

    }
}