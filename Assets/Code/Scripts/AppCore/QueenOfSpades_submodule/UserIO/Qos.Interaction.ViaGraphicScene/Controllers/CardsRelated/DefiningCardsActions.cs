using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using CommonTools;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated
{
    /// <summary>
    /// ������������� ���������� � ���, ��� � ������ ������ ������� ������ ����������� � ��� ��� ���� ������,
    /// � �����, ��� ������ ���� � ���� ����������� � ���������� ������ �������.
    /// </summary>
    public interface ICardsActionsProvider : ICardActionsDefiner // ����� �������� ����� ���������� ����� ��������������� ���������� � ������� ������� �������
    {
        ICardActionsDefiner PreviousActions { get; } // ����� ���� ���� ����� ��������������� ���������� � ��������� � ���������� ������ �������
    }


    /// <summary>
    /// ���������, ��� � ������������ ������ ������� ������ ���������� � ��� ��� ���� ������.
    /// </summary>
    /// <remarks>
    public interface ICardActionsDefiner
    {
        /// <summary>
        /// �������� ����, ������� � ������ ������ �� ����������� ������-���� ������ (��� ������ � �����).
        /// </summary>
        DictionaryData<CardId, CardAction> NonPlayerCardActions { get; }

        /// <summary>
        /// �������� ����, ������� � ������ ������ ����������� ������-���� ������ (��� ����� �����������).
        /// </summary>
        ImmutableDictionary<PlayerId, DictionaryData<CardId, CardAction>> PlayerCardActions { get; }
        /// <summary>
        /// �������� ���� ���� (��� ������� ����������� ��� ������ ��� ���).
        /// </summary>
        DictionaryData<CardId, CardAction> AllCardActions { get; }
    }




    /// <summary>
    /// ����������, ����������� �� ������������ ��� ������� ��������, ��� � ��������������� ������ ������� ���������� � ��� ��� ���� ������.
    /// </summary>
    public class DefiningCardsActions : EventController, ICardActionsDefiner
    {
        private IEvent _prevEvent = null;

        private Dictionary<PlayerId, HashSet<CardId>> _playerCards;


        private DictionaryData<CardId, CardAction>.Editable _nonPlayerCardActions;
        public DictionaryData<CardId, CardAction> NonPlayerCardActions { get; private set; }


        private Dictionary<PlayerId, DictionaryData<CardId, CardAction>.Editable> _playerCardActions;
        public ImmutableDictionary<PlayerId, DictionaryData<CardId, CardAction>> PlayerCardActions { get; private set; }


        private DictionaryData<CardId, CardAction>.Editable _allCardActions;
        public DictionaryData<CardId, CardAction> AllCardActions { get; private set; }

        public DefiningCardsActions(Contexts contexts, Func<IEvent> GetCurrentEvent) : base(contexts, GetCurrentEvent)
        {
            NonPlayerCardActions = new DictionaryData<CardId, CardAction>(out _nonPlayerCardActions);
            AllCardActions = new DictionaryData<CardId, CardAction>(out _allCardActions);
        }


        public override void Update()
        {
            ExtraReactionOnPrevEvent(_prevEvent); // ������ ����� ������� ������������ ���� ����� ���� ��� ������� ������
            var currentEvent = CurrentEvent;
            ReactionOnCurrentEvent(currentEvent);
            _prevEvent = currentEvent;
        }


        private void ExtraReactionOnPrevEvent(IEvent prevEvent)
        {
            if (prevEvent.IsEndOfContiniousEvent<PlayerGettingCardsEvent>(out var pgcEvent))
            {
                Logger.Verbose($"�������, ��� ����� '{pgcEvent.CardIds.MembersToString()}' ������ ��������� ����������� ������.");
                foreach (var card in pgcEvent.CardIds)
                {
                    _nonPlayerCardActions.RemoveItem(card);
                }
            }
            else if (prevEvent.IsEndOfContiniousEvent<PlayerDiscardingCardsEvent>(out var pdcEvent))
            {
                Logger.Verbose($"�������, ��� ����� '{pdcEvent.PlayerId}' ��������� ������� ����� '{pdcEvent.DiscardingCardIds.MembersToString()}'.");
                foreach (var card in pdcEvent.DiscardingCardIds)
                {
                    _playerCardActions[pdcEvent.PlayerId].RemoveItem(card);
                    SetNonPlayerAction(card, CardAction.LyingInHeap());
                }
            }
            else if (prevEvent.IsEndOfContiniousEvent<TrasferingCardsEvent>(out var tcEvent))
            {
                Logger.Verbose($"�������, ��� ����� '{tcEvent.CardGiverId}' ��������� ������� ����� '{tcEvent.CardIds.MembersToString()}'.");
                foreach (var transferingCard in tcEvent.CardIds)
                {
                    _playerCardActions[tcEvent.CardGiverId].RemoveItem(transferingCard);
                }
            }
        }


        private void ReactionOnCurrentEvent(IEvent currentEvent)
        {
            if (currentEvent is CardCreatedEvent ccEvent) 
                HandleAs_CardCreated(ccEvent);
            else if (currentEvent is PlayersExpectedEvent peEvent) 
                HandleAs_PlayersExpected(peEvent);
            else if (currentEvent.IsContiniousEvent<PlayerGettingCardsEvent>(out var pgcEvent, out var normTime)) 
                HandleAs_PlayerGettingCard(pgcEvent, normTime);
            else if (currentEvent is PlayerStartIdleEvent psiEvent) 
                HandleAs_CardStartedIdle(psiEvent);
            else if (currentEvent.IsContiniousEvent<PlayerGoingToDiscardModeEvent>(out var pgtdmEvent, out var pgtdmeNormTime)) 
                HandleAs_CardGoingToDiscMode(pgtdmEvent, pgtdmeNormTime);
            else if (currentEvent.IsContiniousEvent<PlayerGoingOutDiscardModeEvent>(out var pgodmEvent, out var pgodmeNormTime)) 
                HandleAs_CardGoingOutDiscMode(pgodmEvent, pgodmeNormTime);
            else if (currentEvent is PlayerChoosingCardsForDiscardEvent pccfdEvent) 
                HandleAs_CardInDiscMode(pccfdEvent);
            else if (currentEvent is PlayerChoseCardsForDiscardEvent pchcfdEvent) 
                HandleAs_CardSelectedForDiscarding(pchcfdEvent);
            else if (currentEvent.IsContiniousEvent<PlayerDiscardingCardsEvent>(out var pdcEvent, out var pdceNormTime)) 
                HandleAs_CardDiscarding(pdcEvent, pdceNormTime);
            else if (currentEvent.IsContiniousEvent<PlayersGoingToTrasferModeEvent>(out var pgttmEvent, out var pgttmeNormTime)) 
                HandleAs_CardGoingToTransferMode(pgttmEvent, pgttmeNormTime);
            else if (currentEvent is PlayerChoosingCardsForTrasferEvent pccftEvent) 
                HandleAs_CardGoingOutTransferMode(pccftEvent);
            else if (currentEvent.IsContiniousEvent<TrasferingCardsEvent>(out var tcEvent, out var tceNormTime)) 
                HandleAs_CardTransfering(tcEvent, tceNormTime);
            else if (currentEvent.IsContiniousEvent<PlayersGoingOutTrasferModeEvent>(out var pgotmEvent, out var pgotmeNormTime)) 
                HandleAs_CardGoingOutTransferMode(pgotmEvent, pgotmeNormTime);
        }


        private void HandleAs_CardCreated(CardCreatedEvent ccEvent)
        {
            Logger.Verbose($"�������, ��� ����� '{ccEvent.CardId}' � ������ ������ ���������.");
            SetNonPlayerAction(ccEvent.CardId, CardAction.Creating());
        }


        private void HandleAs_PlayersExpected(PlayersExpectedEvent peEvent)
        {
            InitCardsOwnershipCollection(peEvent.PlayerIds);
            InitPlayerCardActionsOutput(peEvent.PlayerIds);
        }


        private void HandleAs_PlayerGettingCard(PlayerGettingCardsEvent pgcEvent, NormValue normTime)
        {
            bool isStart = normTime == NormValue.Min;

            foreach (var card in pgcEvent.CardIds)
            {
                if (isStart)
                {
                    Logger.Verbose($"�������, ��� ����� '{pgcEvent.PlayerId}' ����� ������� ������ '{card}'.");
                    _playerCards[pgcEvent.PlayerId].Add(card);
                }

                Logger.Verbose($"�������, ��� ����� '{card}' �� ���� �� ������ � ���� ������ '{pgcEvent.PlayerId}' ({normTime}).");
                // ��� ��� �����, ��� �� �� ����� ������� ������, ��������� �������� � ���� ���������
                var action = CardAction.GoingFromDeckToPlayer(normTime, pgcEvent.PlayerId);
                SetPlayerAction(pgcEvent.PlayerId, card, action);
                SetNonPlayerAction(card, action);
            }
        }


        private void HandleAs_CardStartedIdle(PlayerStartIdleEvent psiEvent)
        {
            Logger.Verbose($"�������, ��� ����� '{psiEvent.PlayerId}' ������� ������� '{_playerCards[psiEvent.PlayerId].MembersToString()}' � ������ � ���� ������� �� ������.");
            foreach (var card in _playerCards[psiEvent.PlayerId])
            {
                SetPlayerAction(psiEvent.PlayerId, card, CardAction.Idle(psiEvent.PlayerId));
            }
        }


        private void HandleAs_CardGoingToDiscMode(PlayerGoingToDiscardModeEvent pgtdmEvent, NormValue normTime)
        {
            Logger.Verbose($"�������, ��� ����� '{pgtdmEvent.PlayerId}' ��������� � ����� ������ ���� ������ � '{_playerCards[pgtdmEvent.PlayerId].MembersToString()}' ({normTime}).");
            foreach (var card in _playerCards[pgtdmEvent.PlayerId])
            {
                SetPlayerAction(pgtdmEvent.PlayerId, card, CardAction.GoingToDiscMode(normTime, pgtdmEvent.PlayerId));
            }
        }


        private void HandleAs_CardGoingOutDiscMode(PlayerGoingOutDiscardModeEvent pgodmEvent, NormValue normTime)
        {
            Logger.Verbose($"�������, ��� ����� '{pgodmEvent.PlayerId}' ������� �� ������ ������ ���� ������ � '{_playerCards[pgodmEvent.PlayerId].MembersToString()}' ({normTime}).");
            foreach (var card in _playerCards[pgodmEvent.PlayerId])
            {
                SetPlayerAction(pgodmEvent.PlayerId, card, CardAction.GoingOutDiscMode(normTime, pgodmEvent.PlayerId));
            }
        }


        private void HandleAs_CardInDiscMode(PlayerChoosingCardsForDiscardEvent pccfdEvent)
        {
            Logger.Verbose($"�������, ��� ����� '{pccfdEvent.PlayerId}' � ������ ������ � ������� '{_playerCards[pccfdEvent.PlayerId].MembersToString()}'.");
            foreach (var card in _playerCards[pccfdEvent.PlayerId])
            {
                SetPlayerAction(pccfdEvent.PlayerId, card, CardAction.InDiscMode(pccfdEvent.PlayerId));
            }
        }


        private void HandleAs_CardSelectedForDiscarding(PlayerChoseCardsForDiscardEvent pchcfdEvent)
        {
            Logger.Verbose($"�������, ��� ����� '{pchcfdEvent.PlayerId}' ������ ����� ��� ������: '{pchcfdEvent.SelectedCardIds.MembersToString()}'.");
            foreach (var card in pchcfdEvent.SelectedCardIds)
            {
                SetPlayerAction(pchcfdEvent.PlayerId, card, CardAction.SelectedForDiscard(pchcfdEvent.PlayerId));
            }
        }


        private void HandleAs_CardDiscarding(PlayerDiscardingCardsEvent pdcEvent, NormValue normTime)
        {

            Logger.Verbose($"�������, ��� ����� '{pdcEvent.PlayerId}' ���������� ����� '{pdcEvent.DiscardingCardIds.MembersToString()}'. ({normTime})");
            foreach (var card in pdcEvent.DiscardingCardIds)
            {
                bool isStart = normTime == NormValue.Min;

                if (isStart)
                {
                    Logger.Verbose($"�������, ��� ����� '{pdcEvent.PlayerId}' �������� ������� ������ '{card}'.");
                    _playerCards[pdcEvent.PlayerId].Remove(card);
                }

                var action = CardAction.Discarding(normTime, pdcEvent.PlayerId);
                SetPlayerAction(pdcEvent.PlayerId, card, action);
                SetNonPlayerAction(card, action);
            }
        }


        private void HandleAs_CardGoingToTransferMode(PlayersGoingToTrasferModeEvent pgttmEvent, NormValue normTime)
        {
            Logger.Verbose($"�������, ��� ����� '{pgttmEvent.CardGiverId}' ������� ���� ����� '{_playerCards[pgttmEvent.CardGiverId].MembersToString()}' � ������ ������. ({normTime})");
            foreach (var giversCard in _playerCards[pgttmEvent.CardGiverId])
            {
                SetPlayerAction(pgttmEvent.CardGiverId, giversCard, CardAction.GoingToTransferMode(normTime, pgttmEvent.CardGiverId, pgttmEvent.CardTakerId));
            }

            Logger.Verbose($"�������, ��� ����� '{pgttmEvent.CardTakerId}' ������ ���� ����� '{_playerCards[pgttmEvent.CardTakerId].MembersToString()}', �������� � ����� ������. ({normTime})");
            foreach (var takersCard in _playerCards[pgttmEvent.CardTakerId])
            {
                SetPlayerAction(pgttmEvent.CardTakerId, takersCard, CardAction.GoingToHideMode(normTime, pgttmEvent.CardTakerId));
            }
        }


        private void HandleAs_CardGoingOutTransferMode(PlayerChoosingCardsForTrasferEvent pccftEvent)
        {
            Logger.Verbose($"�������, ��� ����� '{pccftEvent.CardGiverId}' ������� ���� ����� '{_playerCards[pccftEvent.CardGiverId].MembersToString()}' � ����� ������.");
            foreach (var giversCard in _playerCards[pccftEvent.CardGiverId])
            {
                SetPlayerAction(pccftEvent.CardGiverId, giversCard, CardAction.InTransferMode(pccftEvent.CardGiverId, pccftEvent.CardTakerId));
            }

            Logger.Verbose($"�������, ��� ����� '{pccftEvent.CardTakerId}' ������� ���� ����� '{_playerCards[pccftEvent.CardTakerId].MembersToString()}', ������� � ����� ������.");
            foreach (var takersCard in _playerCards[pccftEvent.CardTakerId])
            {
                SetPlayerAction(pccftEvent.CardTakerId, takersCard, CardAction.InHideMode(pccftEvent.CardTakerId));
            }
        }


        private void HandleAs_CardTransfering(TrasferingCardsEvent tcEvent, NormValue normTime)
        {
            bool isStart = normTime == NormValue.Min;

            Logger.Verbose($"�������, ��� ����� '{tcEvent.CardGiverId}' �������� ����� '{tcEvent.CardIds.MembersToString()}' ������ '{tcEvent.CardTakerId}'. ({normTime})");
            foreach (var transferingCard in tcEvent.CardIds)
            {
                if (isStart)
                {
                    Logger.Verbose($"�������, ��� ����� '{tcEvent.CardGiverId}' �������� ������� ������ '{transferingCard}'.");
                    _playerCards[tcEvent.CardGiverId].Remove(transferingCard);
                    Logger.Verbose($"�������, ��� ����� '{tcEvent.CardTakerId}' ����� ������� ������ '{transferingCard}'.");
                    _playerCards[tcEvent.CardTakerId].Add(transferingCard);
                }

                var action = CardAction.Transfering(normTime, tcEvent.CardGiverId, tcEvent.CardTakerId);
                SetPlayerAction(tcEvent.CardGiverId, transferingCard, action);
                SetPlayerAction(tcEvent.CardTakerId, transferingCard, action);
            }
        }


        private void HandleAs_CardGoingOutTransferMode(PlayersGoingOutTrasferModeEvent pgotmEvent, NormValue normTime)
        {
            Logger.Verbose($"�������, ��� ����� '{pgotmEvent.CardGiverId}' �� ������ ������� '{_playerCards[pgotmEvent.CardGiverId].MembersToString()}' ������� �� ������ ������. ({normTime})");
            foreach (var giversCard in _playerCards[pgotmEvent.CardGiverId])
            {
                SetPlayerAction(pgotmEvent.CardGiverId, giversCard, CardAction.GoingOutTransferMode(normTime, pgotmEvent.CardGiverId, pgotmEvent.CardTakerId));
            }

            Logger.Verbose($"�������, ��� ����� '{pgotmEvent.CardTakerId}' ������� ���� ����� '{_playerCards[pgotmEvent.CardTakerId].MembersToString()}', ����� �� ������ ������. ({normTime})");
            foreach (var takersCard in _playerCards[pgotmEvent.CardTakerId])
            {
                SetPlayerAction(pgotmEvent.CardTakerId, takersCard, CardAction.GoingOutHideMode(normTime, pgotmEvent.CardTakerId));
            }
        }


        private void SetPlayerAction(PlayerId playerId, CardId cardId, CardAction action)
        {
            _playerCardActions[playerId].SetItem(cardId, action);
            _allCardActions.SetItem(cardId, action);
        }


        private void SetNonPlayerAction(CardId cardId, CardAction action)
        {
            _nonPlayerCardActions.SetItem(cardId, action);
            _allCardActions.SetItem(cardId, action);
        }


        private void InitPlayerCardActionsOutput(IEnumerable<PlayerId> playerIds)
        {
            _playerCardActions = new Dictionary<PlayerId, DictionaryData<CardId, CardAction>.Editable>();
            var builder = ImmutableDictionary.CreateBuilder<PlayerId, DictionaryData<CardId, CardAction>>();

            foreach (var playerId in playerIds)
            {
                builder.Add(playerId, new DictionaryData<CardId, CardAction>(out var edit));
                _playerCardActions.Add(playerId, edit);
            }

            PlayerCardActions = builder.ToImmutable();
        }


        private void InitCardsOwnershipCollection(IEnumerable<PlayerId> playerIds)
        {
            _playerCards = playerIds.ToDictionary(id => id, _ => new HashSet<CardId>());
        }

    }

    public struct CardCreationArg
    {
    }


    public struct InHeapCardArg
    {
        public NormValue NormTime { get; private set; }
    }

    public struct CardGoingFromDeckToPlayerArg
    {
        public NormValue NormTime { get; private set; }
        public PlayerId PlayerId { get; private set; }

        public CardGoingFromDeckToPlayerArg(PlayerId playerId, NormValue normTime)
        {
            PlayerId = playerId; NormTime = normTime;
        }
    }

    public struct CardGoingFromPlayerToHeapArg
    {
        public NormValue NormTime { get; private set; }
        public PlayerId PlayerId { get; private set; }
    }


    public struct CardNotDiscardingNowArg
    {
        public NormValue NormTime { get; private set; }
        public PlayerId PlayerId { get; private set; }
    }

    /*public struct CardAction
    {
        private enum Types
        {
            CREATING, 
            IN_DECK,
        }

        private Types _type;
        private NormValue _normTime;


        public static CardAction CreationOfCard(NormValue normTime)
        {
            return new CardAction()
            {
                _type = Types.CREATING,
                _normTime = normTime
            };
        }


        public bool IsCreationOfCards(out NormValue normTime)
        {
            normTime = _normTime;
            return _type == Types.CREATING;
        }
    }*/



}