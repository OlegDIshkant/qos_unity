using CommonTools;
using Qos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace Qos.Domain.Events
{
    public interface IEvent
    {
    }


    /// <summary>
    /// Событие, расстянутое во времени.
    /// </summary>
    public interface IContiniousEvent : IEvent
    {
    }



    /// <summary>
    /// Оповещает о том, что определенное <see cref="IContiniousEvent"/> на определенное кол-во процентов завершено.
    /// </summary>
    public struct EventContinuation : IEvent
    {
        public IContiniousEvent RootEvent { get; private set; }
        public NormValue NormalizedTime { get; private set; }

        public EventContinuation(IContiniousEvent rootEvent, NormValue normalizedTime)
        {
            RootEvent = rootEvent;
            NormalizedTime = normalizedTime;    
        }

        public override string ToString() => $"ПРОДОЛЖЕНИЕ СОБЫТИЯ ({(int)(NormalizedTime.AsFloat * 100)}%) - '{RootEvent}'";
    }


    /// <summary>
    /// Any event which involves a match entity.
    /// </summary>
    public interface IEventWithMatch : IEvent
    {
        IMatchId MatchId { get; }
    }


    public struct MatchInitedEvent : IEventWithMatch, IContiniousEvent
    {
        public IMatchId MatchId { get; private set; }

        public MatchInitedEvent(IMatchId matchId)
        {
            MatchId = matchId ?? throw new NullReferenceException();
        }
    }


    public struct PlayersExpectedEvent : IEventWithMatch
    {
        public IMatchId MatchId { get; private set; }
        public ReadOnlyCollection<PlayerId> PlayerIds { get; private set; }
        public Dictionary<PlayerId, BaseInteraction> Confirmers { get; private set; }

        public PlayersExpectedEvent(IMatchId matchId, IEnumerable<PlayerId> players, Dictionary<PlayerId, BaseInteraction> confirmers)
        {
            MatchId = matchId ?? throw new NullReferenceException();
            PlayerIds = new ReadOnlyCollection<PlayerId>(new List<PlayerId>(players));
            Confirmers = new Dictionary<PlayerId, BaseInteraction>(confirmers);
        }
    }


    public struct PlayerCreatedEvent : IContiniousEvent
    {
        public PlayerId PlayerId { get; private set; }
        public PlayerModel PlayerModel { get; private set; }

        public PlayerCreatedEvent(PlayerId playerId, PlayerModel playerModel)
        {
            PlayerId = playerId;
            PlayerModel = playerModel;
        }
    }


    public class BaseInteraction
    {
        private Action _OnExecute;

        public bool IsAlive { get; private set; } = true;

        public BaseInteraction(Action OnExecute)
        {
            _OnExecute = OnExecute;
        }

        public void Execute()
        {
            if (!IsAlive)
            {
                throw new InvalidOperationException("Interaction has expired!");
            }
            _OnExecute?.Invoke();
            IsAlive = false;
        }


        public void Kill()
        {
            IsAlive = false;
        }
    }


    public class BaseInteraction<T>
    {
        private Action<T> _OnExecute;

        public bool IsAlive { get; private set; } = true;

        public BaseInteraction(Action<T> OnExecute)
        {
            _OnExecute = OnExecute;
        }

        public void Execute(T arg)
        {
            if (!IsAlive)
            {
                throw new InvalidOperationException("Interaction has expired!");
            }
            _OnExecute?.Invoke(arg);
            IsAlive = false;
        }


        public void Kill()
        {
            IsAlive = false;
        }
    }


    public struct CardCreatedEvent : IEvent, IContiniousEvent
    {
        public CardId CardId { get; private set; }

        public CardModel CardModel { get; private set; }

        public CardCreatedEvent(CardId cardId, CardModel cardModel)
        {
            CardId = cardId;
            CardModel = cardModel;
        }
    }


    public struct PlayerGettingCardsEvent : IEvent, IContiniousEvent
    {
        public IEnumerable<CardId> CardIds { get; private set; }
        public PlayerId PlayerId { get; private set; }

        public PlayerGettingCardsEvent(PlayerId playerId, IEnumerable<CardId> cardIds)
        {
            PlayerId = playerId;
            CardIds = cardIds;
        }
    }


    public struct PlayerHasCardsEvent : IEvent
    {
        public IEnumerable<CardId> CardIds { get; private set; }
        public PlayerId PlayerId { get; private set; }

        public PlayerHasCardsEvent(PlayerId playerId, IEnumerable<CardId> cardIds)
        {
            PlayerId = playerId;
            CardIds = new List<CardId>(cardIds);
        }
    }


    public struct MatchStartedEvent : IEvent, IContiniousEvent
    {
        public IMatchId MatchId { get; private set; }
        public IEnumerable<PlayerId> PlayerIds { get; private set; }

        public MatchStartedEvent(IMatchId matchId, IEnumerable<PlayerId> playerIds)
        {
            MatchId = matchId;
            PlayerIds = playerIds;
        }
    }


    public struct PlayerStartIdleEvent : IEvent
    {

        public PlayerId PlayerId { get; private set; }

        public BaseInteraction<InteractionArg> Interaction { get; private set; }

        public PlayerStartIdleEvent(PlayerId playerId)
        {
            PlayerId = playerId;
            Interaction = null;
        }

        public PlayerStartIdleEvent(PlayerId playerId, BaseInteraction<InteractionArg> interaction)
        {
            PlayerId = playerId;
            Interaction = interaction;
        }

        public override string ToString()
        {
            return $"Event - Player '{PlayerId}' starts idle.";
        }


        public class InteractionArg
        {
            public enum Message
            {
                REQUEST_DISCARD_MODE,
                WONT_REQUEST_DISCARD_MODE
            }

            public Message Msg { get; private set; }

            public InteractionArg(Message msg) { Msg = msg; }
        }
    }


    public struct PlayerGoingToDiscardModeEvent : IEvent, IContiniousEvent
    {
        public PlayerId PlayerId { get; private set; }


        public PlayerGoingToDiscardModeEvent(PlayerId playerId)
        {
            PlayerId = playerId;
        }
    }


    public struct PlayerGoingOutDiscardModeEvent : IEvent, IContiniousEvent
    {
        public PlayerId PlayerId { get; private set; }


        public PlayerGoingOutDiscardModeEvent(PlayerId playerId)
        {
            PlayerId = playerId;
        }
    }


    public struct PlayerChoosingCardsForDiscardEvent : IEvent
    {
        public PlayerId PlayerId { get; private set; }

        public BaseInteraction<InteractionArg> Interaction { get; private set; }


        public PlayerChoosingCardsForDiscardEvent(PlayerId playerId, BaseInteraction<InteractionArg> interaction)
        {
            PlayerId = playerId;
            Interaction = interaction;
        }

        public class InteractionArg
        {
            public bool cancelDiscarding;
            public CardGroups selectedCardGroups;
        }

        public static class InteractionArgValidator
        {
            public static bool IsValid(InteractionArg arg, IEnumerable<CardId> allowedCards)
            {
                var requestsCancellation = arg.cancelDiscarding;
                var selectsCards = arg.selectedCardGroups == null ? false : arg.selectedCardGroups.Any();

                if (selectsCards == requestsCancellation)
                {
                    return false; // нужно делать либо, одно, либо другое
                }

                if (selectsCards)
                {
                    if (HasForbiddenCards())
                    {
                        return false;
                    }
                }

                return true;

                bool HasForbiddenCards() =>
                    arg.selectedCardGroups.Any(group => group.Any(card => !allowedCards.Contains(card)));
            }
        }
    }





    public struct PlayerChoseCardsForDiscardEvent : IEvent
    {
        public PlayerId PlayerId { get; private set; }

        public IEnumerable<CardId> SelectedCardIds { get; private set; }


        public PlayerChoseCardsForDiscardEvent(PlayerId playerId, IEnumerable<CardId> selectedCardIds)
        {
            PlayerId = playerId;
            SelectedCardIds = selectedCardIds;
        }
    }


    public struct PlayerDiscardingCardsEvent : IEvent, IContiniousEvent
    {
        public PlayerId PlayerId { get; private set; }

        public IEnumerable<CardId> DiscardingCardIds { get; private set; }


        public PlayerDiscardingCardsEvent(PlayerId playerId, IEnumerable<CardId> discardingCardIds)
        {
            PlayerId = playerId;
            DiscardingCardIds = discardingCardIds;
        }
    }


    public struct PlayerNotDiscardingCardsEvent : IEvent, IContiniousEvent
    {
        public PlayerId PlayerId { get; private set; }

        public IEnumerable<CardId> DiscardingCardIds { get; private set; }


        public PlayerNotDiscardingCardsEvent(PlayerId playerId, IEnumerable<CardId> discardingCardIds)
        {
            PlayerId = playerId;
            DiscardingCardIds = discardingCardIds;
        }
    }


    public struct PlayerLostCardsEvent : IEvent
    {
        public IEnumerable<CardId> CardIds { get; private set; }
        public PlayerId PlayerId { get; private set; }

        public PlayerLostCardsEvent(PlayerId playerId, IEnumerable<CardId> cardIds)
        {
            PlayerId = playerId;
            CardIds = new List<CardId>(cardIds);
        }
    }


    public struct PlayersGoingToTrasferModeEvent : IContiniousEvent
    {
        public PlayerId CardGiverId { get; private set; }
        public PlayerId CardTakerId { get; private set; }

        public PlayersGoingToTrasferModeEvent(PlayerId cardGiverId, PlayerId cardTakerId)
        {
            CardTakerId = cardTakerId;
            CardGiverId = cardGiverId;
        }

        public override string ToString() => $"Событие '{nameof(PlayersGoingToTrasferModeEvent)}' ('{CardTakerId}' берет карты у '{CardGiverId}')";
    }


    public struct PlayerChoosingCardsForTrasferEvent : IEvent
    {
        public BaseInteraction<InteractionArg> Interaction { get; private set; }
        public PlayerId CardGiverId { get; private set; }
        public PlayerId CardTakerId { get; private set; }
        public List<CardId> PossibleCards { get; private set; }


        public PlayerChoosingCardsForTrasferEvent(PlayerId cardGiverId, PlayerId cardTakerId, IEnumerable<CardId> possibleCards , BaseInteraction<InteractionArg> interaction)
        {
            CardTakerId = cardTakerId;
            CardGiverId = cardGiverId;
            Interaction = interaction;
            PossibleCards = possibleCards.ToList();
        }


        public class InteractionArg
        {
            public IEnumerable<CardId> SelectedCards { get; private set; }

            public InteractionArg(IEnumerable<CardId> cards) { SelectedCards = cards.ToList(); }
        }

        public override string ToString() => $"Событие '{nameof(PlayerChoosingCardsForTrasferEvent)}' ('{CardTakerId}' берет карты у '{CardGiverId}')";
    }


    public struct CardsChosenForTrasferEvent : IEvent
    {
        public IEnumerable<CardId> CardIds { get; private set; }
        public PlayerId CardGiverId { get; private set; }
        public PlayerId CardTakerId { get; private set; }

        public CardsChosenForTrasferEvent(PlayerId cardGiverId, PlayerId cardTakerId, IEnumerable<CardId> cardIds)
        {
            CardTakerId = cardTakerId;
            CardGiverId = cardGiverId;
            CardIds = cardIds;
        }
    }


    public struct TrasferingCardsEvent : IContiniousEvent
    {
        public IEnumerable<CardId> CardIds { get; private set; }
        public PlayerId CardGiverId { get; private set; }
        public PlayerId CardTakerId { get; private set; }

        public TrasferingCardsEvent(PlayerId cardGiverId, PlayerId cardTakerId, IEnumerable<CardId> cardIds)
        {
            CardTakerId = cardTakerId;
            CardGiverId = cardGiverId;
            CardIds = cardIds;
        }
    }


    public struct PlayersGoingOutTrasferModeEvent : IContiniousEvent
    {
        public PlayerId CardGiverId { get; private set; }
        public PlayerId CardTakerId { get; private set; }

        public PlayersGoingOutTrasferModeEvent(PlayerId cardGiverId, PlayerId cardTakerId)
        {
            CardTakerId = cardTakerId;
            CardGiverId = cardGiverId;
        }
    }



    public struct PlayerLostAllCardsEvent : IEvent
    {
        public PlayerId PlayerId { get; private set; }

        public PlayerLostAllCardsEvent(PlayerId playerId)
        {
            PlayerId = playerId;
        }
    }


    public struct PlayerNotLostMatchEvent : IEvent, IContiniousEvent
    {
        public PlayerId PlayerId { get; private set; }

        public PlayerNotLostMatchEvent(PlayerId playerId)
        {
            PlayerId = playerId;
        }
    }


    public struct PlayerLostMatchEvent : IEvent, IContiniousEvent
    {
        public PlayerId PlayerId { get; private set; }

        public PlayerLostMatchEvent(PlayerId playerId)
        {
            PlayerId = playerId;
        }
    }


    public struct MatchFinsihedEvent : IEventWithMatch, IContiniousEvent
    {
        public IMatchId MatchId { get; private set; }

        public MatchFinsihedEvent(IMatchId matchId)
        {
            MatchId = matchId ?? throw new NullReferenceException();
        }
    }


}
