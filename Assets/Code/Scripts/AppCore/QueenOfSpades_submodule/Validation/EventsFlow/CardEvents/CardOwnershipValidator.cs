using CommonTools;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using System.Collections.Generic;
using System.Linq;

namespace Qos.Validation.EventsFlow
{
    public class CardOwnershipValidator : IEventsFlowValidator
    {
        private Dictionary<PlayerId, HashSet<CardId>> _ownership;


        public IEnumerable<Problem> CheckNextEvent(IEvent @event)
        {
            if (@event is PlayersExpectedEvent peEvent)
                return Handle_PlayersExpected(peEvent);
            else if (@event is PlayerGettingCardsEvent pgcEvent)
                return Handle_PlayersStartGettingCards(pgcEvent);
            else if (@event is PlayerDiscardingCardsEvent pdcEvent)
                Handle_PlayersStartDiscardingCards(pdcEvent);
            else if (@event is TrasferingCardsEvent tcEvent)
                Handle_PlayersStartTransferingCards(tcEvent);

            return null;
        }


        private IEnumerable<Problem> Handle_PlayersExpected(PlayersExpectedEvent ev)
        {
            if (_ownership != null)
            {
                return new Problem("Попытка второй раз инициализировать игроков.").WrapInNewList();
            }

            _ownership = ev.PlayerIds.ToDictionary(p => p, _ => new HashSet<CardId>());
            return null;
        }


        private IEnumerable<Problem> Handle_PlayersStartGettingCards(PlayerGettingCardsEvent ev)
        {
            foreach (var card in ev.CardIds)
            {
                if (AnyOneHasCard(card))
                {
                    return new Problem($"Карта '{card}' уже принадлежит игроку '{ev.PlayerId}', хотя должна была быть свободной.").WrapInNewList();
                }
                AddCardToPlayer(card, ev.PlayerId);
            }
            return null;
        }


        private IEnumerable<Problem> Handle_PlayersStartDiscardingCards(PlayerDiscardingCardsEvent ev)
        {
            foreach (var card in ev.DiscardingCardIds)
            {
                if (!PlayerHasCards(card, ev.PlayerId))
                {
                    return new Problem($"Карта '{card}' не принадлежит игроку '{ev.PlayerId}', и не может быть сброшеной им.").WrapInNewList();
                }
                RemoveCardFromPlayer(card, ev.PlayerId);
            }
            return null;
        }


        private IEnumerable<Problem> Handle_PlayersStartTransferingCards(TrasferingCardsEvent ev)
        {
            foreach (var card in ev.CardIds)
            {
                if (!PlayerHasCards(card, ev.CardGiverId))
                {
                    return new Problem($"Карта '{card}' не принадлежит игроку '{ev.CardGiverId}', и он не может передать её.").WrapInNewList();
                }
                if (PlayerHasCards(card, ev.CardTakerId))
                {
                    return new Problem($"Карта '{card}' уже принадлежит игроку '{ev.CardTakerId}'.").WrapInNewList();
                }

                RemoveCardFromPlayer(card, ev.CardGiverId);
                AddCardToPlayer(card, ev.CardTakerId);
            }
            return null;
        }


        private bool AnyOneHasCard(CardId card) => _ownership.Any(i => i.Value.Contains(card));
        private bool AddCardToPlayer(CardId card, PlayerId player) => _ownership[player].Add(card);
        private bool RemoveCardFromPlayer(CardId card, PlayerId player) => _ownership[player].Remove(card);
        private bool PlayerHasCards(CardId card, PlayerId player) => _ownership[player].Contains(card);
    }
}