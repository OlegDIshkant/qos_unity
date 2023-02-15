using CommonTools;
using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    public class InHeap_CardTForms_Controller : AbstractController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly IInHeap_TFormCalcer _tFormCalcer;
        private readonly ICardHeapController _cardHeap;
        private readonly ICardsActionsProvider _actionsProvider;


        private DictionaryData<CardId, Transform>.Editable _dataEdit;
        public DictionaryData<CardId, Transform> DictionaryOutput { get; private set; }


        public InHeap_CardTForms_Controller(
            Contexts contexts, 
            IInHeap_TFormCalcer tFormCalcer,
            ICardHeapController cardHeap,
            ICardsActionsProvider actionsProvider) : 
            base(contexts)
        {
            _tFormCalcer = tFormCalcer;
            _cardHeap = cardHeap;
            _actionsProvider = actionsProvider;
            
            DictionaryOutput = new DictionaryData<CardId, Transform>(out _dataEdit);
        }


        public override void Update()
        {
            foreach (var card in CardsStartedItsWayToHeap())
            {
                DefinePlaceInHeapFor(card);
            }
        }


        private IEnumerable<CardId> CardsStartedItsWayToHeap()
        {
            if (_actionsProvider.NonPlayerCardActions.HasChanged)
            {
                foreach (var item in _actionsProvider.NonPlayerCardActions.AddedOrChanged.Where(i => HasStartedItsWayToHeap(i.Value)))
                {
                    yield return item.Key;
                }
            }
        }


        private bool HasStartedItsWayToHeap(CardAction action) =>
            action.IsGoingFromPlayerToHeapNow(out var normTime, out var _) && normTime == NormValue.Min;


        private void DefinePlaceInHeapFor(CardId cardId)
        {
            _dataEdit.SetItem(cardId, _tFormCalcer.CalcNew(_cardHeap));
        }
    }
}