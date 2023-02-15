using CommonTools;
using Qos.Domain.Entities;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated
{
    /// <summary>
    /// Действие, которое сейчас выполняет та или иная карта.
    /// </summary>
    public struct CardAction
    {
        private enum State
        {
            NONE,
            CREATING,
            IN_DECK,
            PLAYER_TO_HEAP,
            IN_HEAP,
            DECK_TO_PLAYER,
            IDLE,
            TO_DISC_MODE,
            IN_DISC_MODE,
            SELECTED_FOR_DISCARD,
            NOT_DISCARDING,
            OUT_DISC_MODE,
            TO_TRANSFER_MODE,
            IN_TRANSFER_MODE,
            PLAYER_TO_PLAYER,
            OUT_TRANSFER_MODE,
            TO_HIDE_MODE,
            IN_HIDE_MODE,
            OUT_HIDE_MODE
        }

        private State _state;
        private PlayerId _owner;
        private NormValue _normTime;
        private PlayerId _otherPlayer;


        public static CardAction Creating() =>
            new CardAction() { _state = State.CREATING };

        public static CardAction Discarding(NormValue normTime, PlayerId lastOwner) =>
            new CardAction() { _state = State.PLAYER_TO_HEAP, _owner = lastOwner, _normTime = normTime };

        public static CardAction LyingInHeap() =>
            new CardAction() { _state = State.IN_HEAP };

        public static CardAction GoingFromDeckToPlayer(NormValue normTime, PlayerId player) =>
            new CardAction() { _state = State.DECK_TO_PLAYER, _owner = player, _normTime = normTime };

        public static CardAction Idle(PlayerId player) =>
            new CardAction() { _state = State.IDLE, _owner = player };

        public static CardAction GoingToDiscMode(NormValue normTime, PlayerId player) =>
            new CardAction() { _state = State.TO_DISC_MODE, _owner = player, _normTime = normTime };

        public static CardAction InDiscMode(PlayerId player) =>
            new CardAction() { _state = State.IN_DISC_MODE, _owner = player };

        public static CardAction SelectedForDiscard(PlayerId player) =>
            new CardAction() { _state = State.SELECTED_FOR_DISCARD, _owner = player };

        public static CardAction NotDiscarding(NormValue normTime, PlayerId player) =>
            new CardAction() { _state = State.NOT_DISCARDING, _owner = player, _normTime = normTime };

        public static CardAction GoingOutDiscMode(NormValue normTime, PlayerId player) =>
            new CardAction() { _state = State.OUT_DISC_MODE, _owner = player, _normTime = normTime };

        public static CardAction GoingToTransferMode(NormValue normTime, PlayerId player, PlayerId cardTaker) =>
            new CardAction() { _state = State.TO_TRANSFER_MODE, _owner = player, _normTime = normTime, _otherPlayer = cardTaker };

        public static CardAction InTransferMode(PlayerId player, PlayerId cardTaker) =>
            new CardAction() { _state = State.IN_TRANSFER_MODE, _owner = player, _otherPlayer = cardTaker };

        public static CardAction Transfering(NormValue normTime, PlayerId player, PlayerId cardTaker) =>
            new CardAction() { _state = State.PLAYER_TO_PLAYER, _owner = player, _normTime = normTime, _otherPlayer = cardTaker };

        public static CardAction GoingOutTransferMode(NormValue normTime, PlayerId player, PlayerId cardTaker) =>
            new CardAction() { _state = State.OUT_TRANSFER_MODE, _owner = player, _normTime = normTime, _otherPlayer = cardTaker };

        public static CardAction GoingToHideMode(NormValue normTime, PlayerId player) =>
            new CardAction() { _state = State.TO_HIDE_MODE, _owner = player, _normTime = normTime };

        public static CardAction InHideMode(PlayerId player) =>
            new CardAction() { _state = State.IN_HIDE_MODE, _owner = player };

        public static CardAction GoingOutHideMode(NormValue normTime, PlayerId player) =>
            new CardAction() { _state = State.OUT_HIDE_MODE, _owner = player, _normTime = normTime };

        //-------------------------------------------------------------------

        public override string ToString() => $"ДЕЙСТВИЕ КАРТЫ - состояние: '{_state}'    время:'{_normTime}'";

        public bool IsCreatingNow() => _state == State.CREATING;

        public bool IsGoingFromDeckToPlayerNow(out NormValue normTime, out PlayerId owner) { owner = _owner; normTime = _normTime; return _state == State.DECK_TO_PLAYER; }

        public bool IsGoingFromPlayerToHeapNow(out NormValue normTime, out PlayerId prevOwner) { prevOwner = _owner; normTime = _normTime; return _state == State.PLAYER_TO_HEAP; }

        public bool IsInHeap() { return _state == State.IN_HEAP; }

        public bool IsIdleNow(out PlayerId owner) { owner = _owner; return _state == State.IDLE; }

        public bool IsGoingToDiscMode(out NormValue normTime, out PlayerId owner) { owner = _owner; normTime = _normTime; return _state == State.TO_DISC_MODE; }

        public bool IsInDiscMode(out PlayerId owner) { owner = _owner; return _state == State.IN_DISC_MODE; }

        public bool IsSelectedForDiscard(out PlayerId owner) { owner = _owner; return _state == State.SELECTED_FOR_DISCARD; }

        public bool IsGoingOutDiscMode(out NormValue normTime, out PlayerId owner) { owner = _owner; normTime = _normTime; return _state == State.OUT_DISC_MODE; }

        public bool IsGoingToHideMode(out NormValue normTime, out PlayerId owner) { owner = _owner; normTime = _normTime; return _state == State.TO_HIDE_MODE; }

        public bool IsInHideMode(out PlayerId owner) { owner = _owner; return _state == State.IN_HIDE_MODE; }

        public bool IsGoingOutHideMode(out NormValue normTime, out PlayerId owner) { owner = _owner; normTime = _normTime; return _state == State.OUT_HIDE_MODE; }

        public bool IsGoingToTransferMode(out NormValue normTime, out PlayerId giver, out PlayerId taker) { giver = _owner; taker = _otherPlayer; normTime = _normTime; return _state == State.TO_TRANSFER_MODE; }

        public bool IsInTransferMode(out PlayerId giver, out PlayerId taker) { giver = _owner; taker = _otherPlayer; return _state == State.IN_TRANSFER_MODE; }

        public bool IsGoingOutTransferMode(out NormValue normTime, out PlayerId giver, out PlayerId taker) { giver = _owner; taker = _otherPlayer; normTime = _normTime; return _state == State.OUT_TRANSFER_MODE; }

        public bool IsGoingFromPlayerToPlayer(out NormValue normTime, out PlayerId giver, out PlayerId taker) { giver = _owner; taker = _otherPlayer; normTime = _normTime; return _state == State.PLAYER_TO_PLAYER; }

        //---------------------------

        public NormValue GetNormTime() => _normTime;
    }
}