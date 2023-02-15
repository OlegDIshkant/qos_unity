using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;
using CommonTools.Math;
using System;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using CommonTools;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    ///  онтроллер, определ€ющий канонические положени€ карт игрока, когда другой игрок выбирает, какие из них забрать себе.
    /// </summary>
    public abstract class TransferMode_CanonTForms_Controller : AbstractController, IDictionaryDataProvider<CardId, Transform>
    {
        private readonly ITFormsCalcer _cardTFormsCalcer;
        private readonly ISingleDataProvider<TransferProcessInfo> _processInfoProvider;
        private readonly TFormsMapper<CardId> _mapper;

        private Action _UpdateAction;
        private PlayerId _giverId;
        private PlayerId _takerId;
        private IEnumerable<CardId> _cards;

        protected abstract TransferType TransferType { get; }
        private bool ShouldResumeWork => ProcessInfo != null && ProcessInfo.Value.TransferType == TransferType; // ѕотомок начинаем работать, только если соответствует требуемому типу процесса
        private TransferProcessInfo? ProcessInfo => _processInfoProvider.SingleOutput.Value;


        private DictionaryData<CardId, Transform>.Editable _outputEdit;
        public DictionaryData<CardId, Transform> DictionaryOutput { get; private set; }


        public TransferMode_CanonTForms_Controller(
            Contexts contexts,
            ISingleDataProvider<TransferProcessInfo> processInfoProvider,
            ICalcIndexStrategy<CardId> cardMapperStrategy) :
            base(contexts)
        {
            _processInfoProvider = processInfoProvider;
            _UpdateAction = Update_WaitingForTransferProcessStart;

            DictionaryOutput = new DictionaryData<CardId, Transform>(out _outputEdit);

            _mapper = cardMapperStrategy != null ? 
                new TFormsMapper<CardId>(cardMapperStrategy) : new TFormsMapper<CardId>();
            _cardTFormsCalcer = NewCalcer();
        }


        public override void Update() => _UpdateAction();


        private void Update_WaitingForTransferProcessStart()
        {
            Logger.Verbose("ѕровер€ем не начать ли работать.");
            if (ShouldResumeWork)
            {
                Logger.Verbose("Ќачинаем работать.");
                ResumeWork();
            }
        }


        private void ResumeWork()
        {
            _giverId = ProcessInfo.Value.CardGiver;
            _takerId = ProcessInfo.Value.CardTaker;
            _cards = ProcessInfo.Value.GiversCards;
            OnNewGiverAndTaker(_giverId, _takerId);

            ResetTForms();

            _UpdateAction = Update_HandlingTransferProcess;

        }


        protected virtual void OnNewGiverAndTaker(PlayerId giver, PlayerId taker) { }


        private void Update_HandlingTransferProcess()
        {
            if (!ShouldResumeWork)
            {
                StopWork();
                return;
            }

            if (ToResetTForms())
            {
                ResetTForms();
            }
        }


        private void ResetTForms()
        {
            _outputEdit.Clear();

            foreach (var item in RecalcTForms(_cards))
            {
                _outputEdit.SetItem(item.Key, item.Value);
            }
        }


        private Dictionary<CardId, Transform> RecalcTForms(IEnumerable<CardId> cards)
        {
            ActualizeGiverInfoForCalcer(_cardTFormsCalcer);
            ActualizeTakerInfoForCalcer(_cardTFormsCalcer);

            _mapper.ForgetAllKnownObjects();
            _mapper.RememberObjects(cards);

            var tForms = _cardTFormsCalcer.Calc(_mapper.ObjectsAmmount);
            return _mapper.Map(tForms); ;
        }


        protected abstract ITFormsCalcer NewCalcer();


        protected abstract void ActualizeGiverInfoForCalcer(ITFormsCalcer tFormsCalcer);


        protected abstract void ActualizeTakerInfoForCalcer(ITFormsCalcer tFormsCalcer);


        private void StopWork()
        {
            _outputEdit.Clear();
            _UpdateAction = Update_WaitingForTransferProcessStart;
        }


        private bool ToResetTForms() => GiverHasBeenAltered() || TakerHasBeenAltered();


        protected abstract bool GiverHasBeenAltered();


        protected abstract bool TakerHasBeenAltered();

    }
}