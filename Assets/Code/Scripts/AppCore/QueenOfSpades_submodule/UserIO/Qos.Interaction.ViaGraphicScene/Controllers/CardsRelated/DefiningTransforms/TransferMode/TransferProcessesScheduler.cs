using CommonTools;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    ///  Контроллер, который помогает координировать работу нескольких разных процессов передачи карт от одного игрока к другому.
    /// </summary>
    public class TransferProcessesScheduler : AbstractController, IDictionaryDataProvider<int, TransferProcessInfo>
    {
        private readonly ICardsActionsProvider _cardsActionsProvider;
        private readonly PlayersInfo _playersInfo;

        private HashSet<int> _freeProcessIndicies;

        private DictionaryData<int, TransferProcessInfo>.Editable _outputEdit;
        public DictionaryData<int, TransferProcessInfo> DictionaryOutput { get; private set; }


        public TransferProcessesScheduler(
            Contexts contexts,
            ICardsActionsProvider cardsActionsProvider) :
            base(contexts)
        {
            _playersInfo = contexts.PlayersInfo;
            _cardsActionsProvider = cardsActionsProvider;

            DictionaryOutput = new DictionaryData<int, TransferProcessInfo>(out _outputEdit);

            _freeProcessIndicies = new HashSet<int>(Enumerable.Range(1, contexts.PlayersInfo.AllPlayersAmount / 2));
        }


        public override void Update()
        {
            LogTransferProcesses();

            TryFinishExistingTransferProcess();
            TryInitNewTransferProcess();

            LogTransferProcesses();
        }


        private void TryInitNewTransferProcess()
        {
            if (_cardsActionsProvider.PlayerCardActions != null)
            {
                foreach (var playerCardsActions in _cardsActionsProvider.PlayerCardActions)
                {
                    if (playerCardsActions.Value.HasChanged)
                    {
                        if (playerCardsActions.Value.AddedOrChanged.Any() &&
                            playerCardsActions.Value.AddedOrChanged.ElementAt(0).Value.IsGoingToTransferMode(out var normTime, out var giver, out var taker)
                            && normTime == NormValue.Min)
                        {
                            AddNewTransferProcess(giver, taker, playerCardsActions.Value.AddedOrChanged);
                        }
                    }
                }

            }
        }



        private void AddNewTransferProcess(PlayerId giver, PlayerId taker, IDictionary<CardId, CardAction> playerCardActions)
        {

            Logger.Verbose($"Запучкаем новый процесс передачи карт от '{giver}' к '{taker}'.");

            var cards = playerCardActions
                .Where(i => i.Value.IsGoingToTransferMode(out var _, out var g, out var t) && g.Equals(giver) && t.Equals(taker))
                .Select(i => i.Key)
                .ToList();

            var transferType =
                (_playersInfo.mainPLayerId.Equals(giver)) ? TransferType.FROM_MAIN_PLAYER_TO_PLAYER :
                (_playersInfo.mainPLayerId.Equals(taker)) ? TransferType.FROM_PLAYER_TO_MAIN_PLAYER :
                TransferType.FROM_PLAYER_TO_PLAYER;

            Logger.Verbose($"Тип этого процесса передачи: {transferType}.");

            var processInfo = new TransferProcessInfo(giver, taker, cards, transferType);

            var index = _freeProcessIndicies.ElementAt(0);
            _freeProcessIndicies.Remove(index);

            Logger.Verbose($"Номер этого процесса передачи: {index}.");

            _outputEdit.SetItem(index, processInfo);
        }


        private void TryFinishExistingTransferProcess()
        {
            if (DictionaryOutput.Items.Any())
            {
                var processesToFinish = new List<int>();

                foreach (var process in DictionaryOutput.Items)
                {
                    Logger.Verbose($"Возможно завершим процеcc '{process.Key}'.");
                    
                    // ориентируемся по картам "берущего", так как "дающий" может лишиться абсолютно всех карт после передачи
                    var cardTaker = process.Value.CardTaker;
                    var changedActions = _cardsActionsProvider.PreviousActions.GetAddedOrChangedPlayerActions(cardTaker);

                    if (changedActions.Any() && 
                        changedActions.ElementAt(0).Value.IsGoingOutHideMode(out var normTime, out var _) &&
                        normTime == NormValue.Min)
                    {
                        processesToFinish.Add(process.Key);
                    }
                }

                foreach (var index in processesToFinish)
                {
                    FinishTransferProcess(index);
                }
                processesToFinish.Clear();

            }
        }

        



        private void FinishTransferProcess(int processIndex)
        {
            Logger.Verbose($"Завершаем процесс передачи карт '{processIndex}' от игрока '{DictionaryOutput.Items[processIndex].CardGiver}' к игроку '{DictionaryOutput.Items[processIndex].CardTaker}'.");

            _outputEdit.RemoveItem(processIndex);
            _freeProcessIndicies.Add(processIndex);
        }


        private void LogTransferProcesses()
        {
            foreach (var process in DictionaryOutput.Items)
            {
                Logger.Verbose($"В данный момент активен процесс под номером '{process.Key}' - '{process.Value}'.");
            }
        }

    }




    /// <summary>
    /// Данные о процессе передачи карт от одного игрока к другому.
    /// </summary>
    public struct TransferProcessInfo
    {
        public TransferType TransferType { get; private set; }
        public PlayerId CardGiver { get; private set; }
        public PlayerId CardTaker { get; private set; }
        public ImmutableHashSet<CardId> GiversCards { get; private set; }

        public TransferProcessInfo(PlayerId giver, PlayerId taker, IEnumerable<CardId> giverCards, TransferType type)
        {
            CardGiver = giver;
            CardTaker = taker;
            GiversCards = ImmutableHashSet.CreateRange(giverCards);
            TransferType = type;
        }
    }


    /// <summary>
    /// Процесс, когда один игрок выбирает среди карт другого игрока те, что заберет себе, сильно зависит от того, кем являются эти игроки.
    /// Поэтому мы различаем несколько типов процессов передачи карт.
    /// </summary>
    public enum TransferType
    {
        FROM_PLAYER_TO_MAIN_PLAYER,
        FROM_MAIN_PLAYER_TO_PLAYER,
        FROM_PLAYER_TO_PLAYER
    }
}