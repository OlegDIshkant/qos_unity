using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /*

    /// <summary>
    /// Определяет принадлежность карты тому или иному игроку.
    /// </summary>
    public interface ICardsOwnership
    {
        public ImmutableDictionary<PlayerId, ListData<CardId>> PlayersCards { get; }
    }


    /// <summary>
    /// Контроллер, определяющий принадлежность карты тому или иному игроку.
    /// </summary>
    public class CardsOwnershipController : AbstractController, ICardsOwnership
    {
        private readonly ICardsActionsProvider _cardsActions;


        public Dictionary<PlayerId, ListData<CardId>.Editable> _playersCardsEdits;
        public ImmutableDictionary<PlayerId, ListData<CardId>> PlayersCards { get; private set; }


        public CardsOwnershipController(
            Contexts contexts,
            ICardsActionsProvider cardsActions) : 
            base(contexts)
        {
            _cardsActions = cardsActions;

            _playersCardsEdits = new Dictionary<PlayerId, ListData<CardId>.Editable>();
            PlayersCards = ImmutableDictionary.Create<PlayerId, ListData<CardId>>();
        }


        public override void Update()
        {
            if (_cardsActions.CardsGoingFromDeckToPlayer.HasChanged)
            {
                foreach (var item in _cardsActions.CardsGoingFromDeckToPlayer.AddedOrChanged)
                {
                    if (item.Value.NormTime == NormValue.Min)
                    {
                        AssignCardToPlayer(item.Key, item.Value.PlayerId);
                    }
                }
            }
            else if (_cardsActions.CardsGoingFromDeckToPlayer.HasChanged)
            {
                foreach (var item in _cardsActions.CardsGoingFromDeckToPlayer.AddedOrChanged)
                {
                    if (item.Value.NormTime == NormValue.Min)
                    {
                        TakeCardFromPlayer(item.Key, item.Value.PlayerId);
                    }
                }
            }
        }


        public void AssignCardToPlayer(CardId cardId, PlayerId playerId)
        {
            if (!PlayersCards.ContainsKey(playerId))
            {
                PlayersCards.Add(playerId, new ListData<CardId>(out var cardsEdit));
                _playersCardsEdits.Add(playerId, cardsEdit);
            }

            _playersCardsEdits[playerId].AddItem(cardId);
        }


        public void TakeCardFromPlayer(CardId cardId, PlayerId playerId)
        {
            _playersCardsEdits[playerId].RemoveItem(cardId);
        }

    }

    */



}
