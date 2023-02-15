using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;
using CommonTools.Math;
using CommonTools;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Контроллер, вычисляющий, какое расположение в пространстве должны иметь карты, когда они переходят от одного действия к другому.
    /// </summary>
    public class CardsTransitTFormsDefiner : AbstractController, IDictionaryDataProvider<CardId, Transform>
    {

        private readonly IDictionaryDataProvider<CardId, Transform> _prevCardsTFormsProvider;
        private readonly ICardsActionsProvider _cardsActionsProvider;
        private readonly IDictionaryDataProvider<CardId, Transform> _idleTFormsProvider;
        private readonly IDictionaryDataProvider<CardId, Transform> _discModeTFormsProvider;
        private readonly IDictionaryDataProvider<CardId, Transform> _hideModeTFormsProvider;
        private readonly IDictionaryDataProvider<CardId, Transform> _inHeapTFormsProvider;
        private readonly IDictionaryDataProvider<CardId, Transform> _transferModeTFormsProvider;


        private Dictionary<CardId, TransitTFormsCalcer> _tFormCalcers = new Dictionary<CardId, TransitTFormsCalcer>();


        private DictionaryData<CardId, Transform>.Editable _outputEdit;
        public DictionaryData<CardId, Transform> DictionaryOutput { get; private set; }


        public CardsTransitTFormsDefiner(
            Contexts contexts,
            ICardsActionsProvider cardsActions,
            IDictionaryDataProvider<CardId, Transform> prevCardsTFormsProvider,
            IDictionaryDataProvider<CardId, Transform> inDeckTFormsProvider,
            IDictionaryDataProvider<CardId, Transform> inHeapTFormsProvider,
            IDictionaryDataProvider<CardId, Transform> idleTFormsProvider,
            IDictionaryDataProvider<CardId, Transform> discModeTFormsProvider,
            IDictionaryDataProvider<CardId, Transform> transferModeTFormsProvider,
            IDictionaryDataProvider<CardId, Transform> hideModeTFormsProvider) :
            base(contexts)
        {
            _prevCardsTFormsProvider = prevCardsTFormsProvider;
            _cardsActionsProvider = cardsActions;
            _idleTFormsProvider = idleTFormsProvider;
            _discModeTFormsProvider = discModeTFormsProvider;
            _hideModeTFormsProvider = hideModeTFormsProvider;
            _inHeapTFormsProvider = inHeapTFormsProvider;
            _transferModeTFormsProvider = transferModeTFormsProvider;

            DictionaryOutput = new DictionaryData<CardId, Transform>(out _outputEdit);

        }


        public override void Update()
        {
            UpdateTransitCalcers();
            UpdateTForms();
        }


        private void UpdateTransitCalcers()
        {
            foreach (var (card, newCalcer) in NewCalcersToApply())
            {
                _tFormCalcers[card] = newCalcer;
            }
        }


        private void UpdateTForms()
        {
            foreach (var (card, normTime, targetTForm) in ChangedTransitionData())
            {
                _outputEdit.SetItem(card, CalcTransitionTForm(card, normTime, targetTForm));
            }
        }


        private Transform CalcTransitionTForm(CardId card, NormValue normTime, Transform targetTForm)
        {
            var transitCalcer = _tFormCalcers[card];
            if (!targetTForm.Equals(transitCalcer.TargetTForm))
            {
                transitCalcer.ChangeTarget(targetTForm);
            }
            return transitCalcer.Calc(normTime);
        }


        private Transform GetPrevTForm(CardId card) =>
            _prevCardsTFormsProvider.DictionaryOutput.Items.TryGetValue(card, out var tForm) ? tForm : Transform.Default;


        private IEnumerable<(CardId, NormValue, Transform)> ChangedTransitionData()
        {
            if (_cardsActionsProvider.AllCardActions.HasChanged)
            {
                foreach (var item in _cardsActionsProvider.AllCardActions.AddedOrChanged)
                {
                    if (item.Value.IsGoingFromDeckToPlayerNow(out var normTime, out var _))
                    {
                        yield return (item.Key, normTime, _idleTFormsProvider.DictionaryOutput.Items[item.Key]);
                    }
                    else if (item.Value.IsGoingToDiscMode(out var normTime_1, out var _))
                    {
                        yield return (item.Key, normTime_1, _discModeTFormsProvider.DictionaryOutput.Items[item.Key]);
                    }
                    else if (item.Value.IsGoingOutDiscMode(out var normTime_2, out var _))
                    {
                        yield return (item.Key, normTime_2, _idleTFormsProvider.DictionaryOutput.Items[item.Key]);
                    }
                    else if (item.Value.IsGoingToHideMode(out var normTime_3, out var _))
                    {
                        yield return (item.Key, normTime_3, _hideModeTFormsProvider.DictionaryOutput.Items[item.Key]);
                    }
                    else if (item.Value.IsGoingOutHideMode(out var normTime_4, out var _))
                    {
                        yield return (item.Key, normTime_4, _idleTFormsProvider.DictionaryOutput.Items[item.Key]);
                    }
                    else if (item.Value.IsGoingFromPlayerToHeapNow(out var normTime_5, out var _))
                    {
                        yield return (item.Key, normTime_5, _inHeapTFormsProvider.DictionaryOutput.Items[item.Key]);
                    }
                    else if (item.Value.IsGoingToTransferMode(out var normTime_6, out var _, out var _))
                    {
                        yield return (item.Key, normTime_6, _transferModeTFormsProvider.DictionaryOutput.Items[item.Key]);
                    }
                    else if (item.Value.IsGoingOutTransferMode(out var normTime_7, out var _, out var _))
                    {
                        yield return (item.Key, normTime_7, _idleTFormsProvider.DictionaryOutput.Items[item.Key]);
                    }
                    else if (item.Value.IsGoingFromPlayerToPlayer(out var normTime_8, out var _, out var _))
                    {
                        yield return (item.Key, normTime_8, _hideModeTFormsProvider.DictionaryOutput.Items[item.Key]);
                    }
                }
            }
        }


        private IEnumerable<(CardId, CardAction)> ChangedCardActions()
        {
            if (_cardsActionsProvider.AllCardActions.HasChanged)
            {
                foreach (var item in _cardsActionsProvider.AllCardActions.AddedOrChanged)
                {
                    yield return (item.Key, item.Value);
                }
            }
        }


        private IEnumerable<(CardId, CardAction)> JustStartedCardActions() =>
            ChangedCardActions().Where(i => i.Item2.GetNormTime() == NormValue.Min);


        private IEnumerable<(CardId, TransitTFormsCalcer)> NewCalcersToApply()
        {
            foreach (var (card, action) in JustStartedCardActions())
            {
                if (action.IsGoingFromPlayerToHeapNow(out var normTime_5, out var _))
                {
                    yield return (card, new ToHeapTransitTFormsCalcer(GetPrevTForm(card), _inHeapTFormsProvider.GetFor(card)));
                }
                else if (action.IsGoingFromDeckToPlayerNow(out var _, out var _))
                {
                    yield return (card, new SimpleTransitTFormsCalcer(GetPrevTForm(card), _idleTFormsProvider.GetFor(card)));
                }
                else if (action.IsGoingToDiscMode(out var _, out var _))
                {
                    yield return (card, new SimpleTransitTFormsCalcer(GetPrevTForm(card), _discModeTFormsProvider.GetFor(card)));
                }
                else if (action.IsGoingOutDiscMode(out var _, out var _))
                {
                    yield return (card, new SimpleTransitTFormsCalcer(GetPrevTForm(card), _idleTFormsProvider.GetFor(card)));
                }
                else if (action.IsGoingToHideMode(out var _, out var _))
                {
                    yield return (card, new SimpleTransitTFormsCalcer(GetPrevTForm(card), _hideModeTFormsProvider.GetFor(card)));
                }
                else if (action.IsGoingOutHideMode(out var _, out var _))
                {
                    yield return (card, new SimpleTransitTFormsCalcer(GetPrevTForm(card), _idleTFormsProvider.GetFor(card)));
                }
                else if (action.IsGoingToTransferMode(out var _, out var _, out var _))
                {
                    yield return (card, new SimpleTransitTFormsCalcer(GetPrevTForm(card), _transferModeTFormsProvider.GetFor(card)));
                }
                else if (action.IsGoingOutTransferMode(out var _, out var _, out var _))
                {
                    yield return (card, new SimpleTransitTFormsCalcer(GetPrevTForm(card), _idleTFormsProvider.GetFor(card)));
                }
                else if (action.IsGoingFromPlayerToPlayer(out var _, out var _, out var _))
                {
                    yield return (card, new BetweenPlayersTransitTFormsCalcer(GetPrevTForm(card), _hideModeTFormsProvider.GetFor(card)));
                }
            }



        }
    }
}