using CommonTools;
using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    ///  онтроллер, который подытоживает результаты работы нескольких разных определителей положений карт,
    /// определ€€ актуальные положени€ карт в данный момент времени.
    /// </summary>
    public class CardTFormsSelector : AbstractController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly ICardsActionsProvider _cardsActions;
        private readonly IDictionaryDataProvider<CardId, Transform> _inDeckTFormsProvider;
        private readonly IDictionaryDataProvider<CardId, Transform> _inHeapTFormsProvider;
        private readonly IDictionaryDataProvider<CardId, Transform> _idleTFormsProvider;
        private readonly IDictionaryDataProvider<CardId, Transform> _discModeTFormsProvider;
        private readonly IDictionaryDataProvider<CardId, Transform> _transferModeTFormsProvider;
        private readonly IDictionaryDataProvider<CardId, Transform> _hideModeTFormsProvider;
        private readonly IDictionaryDataProvider<CardId, Transform> _transitTFormsProvider;

        private Dictionary<CardId, Func<Transform>> _tFormUpdaters = new Dictionary<CardId, Func<Transform>>();

        private DictionaryData<CardId, Transform>.Editable _dataEdit;
        public DictionaryData<CardId, Transform> DictionaryOutput { get; private set; }

        private Func<Transform> GetDummyTForm => () => Transform.Default;


        public CardTFormsSelector(
            Contexts contexts,
            ICardsActionsProvider cardsActions,
            IDictionaryDataProvider<CardId, Transform> inDeckTFormsProvider,
            IDictionaryDataProvider<CardId, Transform> inHeapTFormsProvider,
            IDictionaryDataProvider<CardId, Transform> idleTFormsProvider,
            IDictionaryDataProvider<CardId, Transform> discModeTFormsProvider,
            IDictionaryDataProvider<CardId, Transform> transferModeTFormsProvider,
            IDictionaryDataProvider<CardId, Transform> hideModeTFormsProvider,
            IDictionaryDataProvider<CardId, Transform> transitTFormsProvider) :
            base(contexts)
        {
            _cardsActions = cardsActions;
            
            DictionaryOutput = new DictionaryData<CardId, Transform>(out _dataEdit);

            _inDeckTFormsProvider = inDeckTFormsProvider;
            _idleTFormsProvider = idleTFormsProvider;
            _discModeTFormsProvider = discModeTFormsProvider;
            _transitTFormsProvider = transitTFormsProvider;
            _hideModeTFormsProvider = hideModeTFormsProvider;
            _inHeapTFormsProvider = inHeapTFormsProvider;
            _transferModeTFormsProvider = transferModeTFormsProvider;
        }


        public override void Update()
        {        
            ActualizeCardTFormUpdaters();
            UpdateCardTForms();
        }


        private void ActualizeCardTFormUpdaters()
        {
            foreach (var item in _cardsActions.AllCardActions.AddedOrChanged)
            {
                Logger.Verbose($"—ледующа€ карта может помен€ть источник своего местоположени€: '{item.Key}' ({item.Value}).");

                if (item.Value.IsCreatingNow())
                {
                    ChangeTFormSourceForCard(item.Key, _inDeckTFormsProvider);
                }
                else if (item.Value.IsGoingFromDeckToPlayerNow(out var normTime, out var _) && normTime == NormValue.Min)
                {
                    ChangeTFormSourceForCard(item.Key, _transitTFormsProvider);
                }
                else if (item.Value.IsIdleNow(out var _))
                {
                    ChangeTFormSourceForCard(item.Key, _idleTFormsProvider);
                }
                else if (item.Value.IsGoingFromDeckToPlayerNow(out var normTime_1,out var _) && normTime_1 == NormValue.Max)
                {
                    ChangeTFormSourceForCard(item.Key, _idleTFormsProvider);
                }
                else if (item.Value.IsGoingToDiscMode(out var normTime_2, out var _) && normTime_2 == NormValue.Min)
                {
                    ChangeTFormSourceForCard(item.Key, _transitTFormsProvider);
                }
                else if (item.Value.IsInDiscMode(out var _))
                {
                    ChangeTFormSourceForCard(item.Key, _discModeTFormsProvider);
                }
                else if (item.Value.IsGoingOutDiscMode(out var normTime_3, out var _) && normTime_3 == NormValue.Min)
                {
                    ChangeTFormSourceForCard(item.Key, _transitTFormsProvider);
                }
                else if (item.Value.IsGoingToHideMode(out var normTime_4, out var _) && normTime_4 == NormValue.Min)
                {
                    ChangeTFormSourceForCard(item.Key, _transitTFormsProvider);
                }
                else if (item.Value.IsInHideMode(out var _))
                {
                    ChangeTFormSourceForCard(item.Key, _hideModeTFormsProvider);
                }
                else if (item.Value.IsGoingOutHideMode(out var normTime_5, out var _) && normTime_5 == NormValue.Min)
                {
                    ChangeTFormSourceForCard(item.Key, _transitTFormsProvider);
                }
                else if (item.Value.IsGoingFromPlayerToHeapNow(out var normTime_6, out var _) && normTime_6 == NormValue.Min)
                {
                    ChangeTFormSourceForCard(item.Key, _transitTFormsProvider);
                }
                else if (item.Value.IsInHeap())
                {
                    ChangeTFormSourceForCard(item.Key, _inHeapTFormsProvider);
                }
                else if (item.Value.IsGoingToTransferMode(out var normTime_7, out var _, out var _) && normTime_7 == NormValue.Min)
                {
                    ChangeTFormSourceForCard(item.Key, _transitTFormsProvider);
                }
                else if (item.Value.IsInTransferMode(out var _, out var _))
                {
                    ChangeTFormSourceForCard(item.Key, _transferModeTFormsProvider);
                }
                else if (item.Value.IsGoingOutTransferMode(out var normTime_8, out var _, out var _) && normTime_8 == NormValue.Min)
                {
                    ChangeTFormSourceForCard(item.Key, _transitTFormsProvider);
                }
                else if (item.Value.IsGoingFromPlayerToPlayer(out var normTime_9, out var _, out var _) && normTime_9 == NormValue.Min)
                {
                    ChangeTFormSourceForCard(item.Key, _transitTFormsProvider);
                }

            }

        }


        private void ChangeTFormSourceForCards(IEnumerable<CardId> cards, IDictionaryDataProvider<CardId, Transform> newSource)
        {
            Logger.Verbose($"ћестоположени€ дл€ карт теперь будем брать из '{newSource}'. Ёто касаетс€ следующих карт:");
            foreach (var cardId in cards)
            {
                Logger.Verbose($"\t{cardId}");
                _tFormUpdaters[cardId] = () =>
                {
                    Logger.Verbose($"„итаем местоположение дл€ '{cardId}' из '{newSource}'.");
                    return newSource.DictionaryOutput.Items[cardId];
                };
            }
        }


        private void ChangeTFormSourceForCard(CardId card, IDictionaryDataProvider<CardId, Transform> newSource)
        {
            Logger.Verbose($"ћестоположени€ дл€ карты '{card}' теперь будем брать из '{newSource}'.");
            _tFormUpdaters[card] = () =>
            {
                Logger.Verbose($"„итаем местоположение дл€ '{card}' из '{newSource}'.");
                return newSource.DictionaryOutput.Items[card];
            };
        }


        private void UpdateCardTForms()
        {
            Logger.Verbose($"ќбновл€ем местоположени€ дл€ карт.");
            foreach (var item in _tFormUpdaters)
            {
                _dataEdit.SetItem(item.Key, item.Value());
            }
        }



    }
}
