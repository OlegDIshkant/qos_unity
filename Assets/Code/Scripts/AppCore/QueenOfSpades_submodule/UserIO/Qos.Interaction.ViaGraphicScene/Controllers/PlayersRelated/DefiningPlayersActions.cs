using CommonTools;
using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    /// <summary>
    /// Контроллер, вычисляющий, что в данный момент происходит с тем или иным игроком.
    /// </summary>
    public class DefiningPlayersActions : EventController, IPlayerActionsDefiner
    {
        private IEvent _prevEvent;

        /// <summary>
        /// Действия игроков.
        /// </summary>
        private DictionaryData<PlayerId, PlayerAction>.Editable _playersActions;
        public DictionaryData<PlayerId, PlayerAction> DictionaryOutput { get; private set; }


        public DefiningPlayersActions(Contexts contexts, Func<IEvent> GetCurrentEvent) : base(contexts, GetCurrentEvent)
        {
            DictionaryOutput = new DictionaryData<PlayerId, PlayerAction>(out _playersActions);
        }


        public override void Update()
        {
            HandlePreviousEvent();
            HandleCurrentEvent();
            _prevEvent = CurrentEvent;
        }


        private void HandleCurrentEvent()
        {
            if (CanDefineNewPlayerActionByCurrentEvent(out var playersActions))
            {
                NotifyAboutNewPlayersActions(playersActions);
            }
        }


        private void HandlePreviousEvent()
        {
            if (CanDefineNewPlayerActionByPrevEvent(out var playersActions))
            {
                NotifyAboutNewPlayersActions(playersActions);
            }
        }


        private void NotifyAboutNewPlayersActions(Dictionary<PlayerId, PlayerAction> playersActions)
        {
            foreach (var item in playersActions)
            {
                Logger.Verbose($"Игрок '{item.Key}' выполняет новое действие '{item.Value}'.");
                _playersActions.SetItem(item.Key, item.Value);
            }
        }


        public bool CanDefineNewPlayerActionByCurrentEvent(out Dictionary<PlayerId, PlayerAction> playersActions)
        {
            playersActions = null;

            if (CurrentEvent.IsContiniousEvent<PlayerCreatedEvent>(out var pcEvent, out var normTime))
            {
                playersActions = new Dictionary<PlayerId, PlayerAction>() { { pcEvent.PlayerId, PlayerAction.CreatingOfPlayer(normTime) } };
            }
            else if (CurrentEvent is PlayerStartIdleEvent psiEvent)
            {
                playersActions = new Dictionary<PlayerId, PlayerAction>() { { pcEvent.PlayerId, PlayerAction.PlayerIdles() } };
            }
            else if (CurrentEvent is PlayersExpectedEvent pexEvent)
            {
                playersActions = pexEvent.PlayerIds.ToDictionary(id => id, _ => PlayerAction.AnnouncePlayer());
            }
            else if (CurrentEvent is PlayerChoosingCardsForTrasferEvent pccftEvent)
            {
                playersActions = new Dictionary<PlayerId, PlayerAction>()
                {
                    { pccftEvent.CardGiverId, PlayerAction.InTransferMode(pccftEvent.CardTakerId) },
                    { pccftEvent.CardTakerId, PlayerAction.InTransferMode(pccftEvent.CardGiverId) }
                };
            }
            else if (CurrentEvent.IsContiniousEvent<PlayersGoingToTrasferModeEvent>(out var pgttmEvent, out var nt_1))
            {
                playersActions = new Dictionary<PlayerId, PlayerAction>()
                {
                    { pgttmEvent.CardGiverId, PlayerAction.GoingToTransferMode(pgttmEvent.CardTakerId, nt_1) },
                    { pgttmEvent.CardTakerId, PlayerAction.GoingToTransferMode(pgttmEvent.CardGiverId, nt_1) }
                };
            }
            else if (CurrentEvent.IsContiniousEvent<PlayersGoingOutTrasferModeEvent>(out var pgotmEvent, out var nt_2))
            {
                playersActions = new Dictionary<PlayerId, PlayerAction>()
                {
                    { pgotmEvent.CardGiverId, PlayerAction.GoingOutTransferMode(pgotmEvent.CardTakerId, nt_2) },
                    { pgotmEvent.CardTakerId, PlayerAction.GoingOutTransferMode(pgotmEvent.CardGiverId, nt_2) }
                };
            }

            return playersActions?.Any() ?? false;
        }


        public bool CanDefineNewPlayerActionByPrevEvent(out Dictionary<PlayerId, PlayerAction> playersActions)
        {
            playersActions = null;

            if (_prevEvent.IsEndOfContiniousEvent<PlayerNotLostMatchEvent>(out var pnlmEvent))
            {
                playersActions = new Dictionary<PlayerId, PlayerAction>() { { pnlmEvent.PlayerId, PlayerAction.OutOfGame() } };
            }

            return playersActions?.Any() ?? false;
        }


        public DictionaryData<PlayerId, Transform> GetDictionaryData(string dictionaryKey) => throw new NotSupportedException();

    }



    public struct PlayerAction
    {
        public enum Types
        {
            ANNOUNCED,
            CREATING,
            IDLE,
            GOING_TO_TRANSFER_MODE,
            IN_TRANSFER_MODE,
            GOING_OUT_TRANSFER_MODE,
            OUT_OF_GAME
        }

        public Types Type { get; private set; }
        public NormValue NormTime { get; private set; }

        private PlayerId? _otherPlayer;


        public static PlayerAction AnnouncePlayer()
        {
            return new PlayerAction()
            {
                Type = Types.ANNOUNCED
            };
        }


        public static PlayerAction CreatingOfPlayer(NormValue normTime)
        {
            return new PlayerAction()
            {
                Type = Types.CREATING,
                NormTime = normTime
            };
        }


        public static PlayerAction PlayerIdles()
        {
            return new PlayerAction()
            {
                Type = Types.IDLE
            };
        }


        public static PlayerAction OutOfGame()
        {
            return new PlayerAction()
            {
                Type = Types.OUT_OF_GAME
            };
        }


        public static PlayerAction GoingToTransferMode(PlayerId otherPlayer, NormValue normTime)
        {
            return new PlayerAction()
            {
                Type = Types.GOING_TO_TRANSFER_MODE,
                NormTime = normTime,
                _otherPlayer = otherPlayer
            };
        }



        public static PlayerAction InTransferMode(PlayerId otherPlayer)
        {
            return new PlayerAction()
            {
                Type = Types.IN_TRANSFER_MODE,
                _otherPlayer = otherPlayer
            };
        }


        public static PlayerAction GoingOutTransferMode(PlayerId otherPlayer, NormValue normTime)
        {
            return new PlayerAction()
            {
                Type = Types.GOING_OUT_TRANSFER_MODE,
                NormTime = normTime,
                _otherPlayer = otherPlayer
            };
        }



        public bool IsBeginingOfPlayerCreation => Type == Types.CREATING && NormTime == NormValue.Min;


        public bool IsOutOfGame() => Type == Types.OUT_OF_GAME;


        public override string ToString()
        {
            switch (Type)
            {
                case Types.CREATING: return $"ДЕЙСТВИЕ: Появление ('{NormTime}')";
                case Types.IDLE: return "ДЕЙСТВИЕ: Idling";
            }
            return base.ToString();
        }


        public bool IsGoingToTransferMode(out PlayerId otherPlayer)
        {
            if (Type == Types.GOING_TO_TRANSFER_MODE)
            {
                otherPlayer = _otherPlayer.Value;
                return true;
            }
            otherPlayer = default;
            return false;
        }


        public bool IsGoingOutTransferMode(out PlayerId otherPlayer)
        {
            if (Type == Types.GOING_OUT_TRANSFER_MODE)
            {
                otherPlayer = _otherPlayer.Value;
                return true;
            }
            otherPlayer = default;
            return false;
        }
    }

}