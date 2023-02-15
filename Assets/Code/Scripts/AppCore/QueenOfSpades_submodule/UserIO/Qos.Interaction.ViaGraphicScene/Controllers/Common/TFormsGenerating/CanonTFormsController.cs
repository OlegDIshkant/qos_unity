using CommonTools;
using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.Common
{
    /// <summary>
    /// ����������, ������������ ������������ ��������� ������������ �������� � ������������ ������ �������.
    /// </summary>
    /// <remarks>
    /// "������������" - �� �������� "��������": ��������������, ��� �� �� ������ ����� ��������� �����, ��������, ��� �������� ��������.
    /// </remarks>
    public class CanonTFormsController<Key> : AbstractController, IDictionaryDataProvider<Key, Transform>
    {
        private readonly ICanonTFormsControllerImpl<Key> _impl;
        private readonly TFormsMapper<Key> _mapper;

        private DictionaryData<Key, Transform>.Editable _dataEdit;
        public DictionaryData<Key, Transform> DictionaryOutput { get; private set; }


        public CanonTFormsController(
            Contexts contexts,
            ICanonTFormsControllerImpl<Key> impl) :
            base(contexts)
        {
            _impl = impl;

            DictionaryOutput = new DictionaryData<Key, Transform>(out _dataEdit);

            _mapper = new TFormsMapper<Key>();
        }


        public CanonTFormsController(
            Contexts contexts,
            ICanonTFormsControllerImpl<Key> impl,
            ICalcIndexStrategy<Key> tFormsMapperCalcIndexStrategy) :
            base(contexts)
        {
            _impl = impl;

            DictionaryOutput = new DictionaryData<Key, Transform>(out _dataEdit);

            _mapper = new TFormsMapper<Key>(tFormsMapperCalcIndexStrategy);
        }


        public override void Update()
        {
            _impl.Update(out var objectsToAdd, out var objectsToRemove, out var recaclNeeded);

            // ������� ������ �������� �����, � ����� ��������� �����.
            // ��� ������� ����� ������ ������ ���� ���������.
            if (objectsToRemove?.Any() ?? false)
            {
                _mapper.ForgetObjects(objectsToRemove);
                foreach (var cardId in objectsToRemove)
                {
                    _dataEdit.RemoveItem(cardId);
                }
            }
            if (objectsToAdd?.Any() ?? false)
            {
                _mapper.RememberObjects(objectsToAdd);
            }

            if (recaclNeeded && _mapper.ObjectsAmmount > 0)
            {
                RecalcCardsTForms();
            }
        }


        private void RecalcCardsTForms()
        {
            var newTForms = _mapper.Map(_impl.TFormsCalcer.Calc(_mapper.ObjectsAmmount));
            foreach (var item in newTForms)
            {
                _dataEdit.SetItem(item.Key, item.Value);
            }
        }


    }



    /// <summary>
    /// ���������� ����������� <seealso cref="CanonTFormsController{Key}"/>.
    /// </summary>
    public interface ICanonTFormsControllerImpl<Key>
    {
        ITFormsCalcer TFormsCalcer { get; }
        void Update(out IEnumerable<Key> objectsToStartCalc, out IEnumerable<Key> objectsToStopCalc, out bool recalcNeeded);
    }



    /// <summary>
    /// ���������� <seealso cref="ICanonTFormsControllerImpl{Key}"/> ��� ������� �������.
    /// </summary>
    public class CanonTFormsControllerImpl<Key> : ICanonTFormsControllerImpl<Key>
    {
        private readonly IBeginCalcTFormsStartegy<Key> _beginCalcStrategy;
        public ITFormsCalcer TFormsCalcer { get; private set; }


        public CanonTFormsControllerImpl(IBeginCalcTFormsStartegy<Key> beginCalcStrategy, ITFormsCalcer tFormsCalcer)
        {
            _beginCalcStrategy = beginCalcStrategy;
            TFormsCalcer = tFormsCalcer;
        }


        public void Update(out IEnumerable<Key> cardsToStartCalc, out IEnumerable<Key> cardsToStopCalc, out bool recalcNeeded)
        {
            BeforeUpdate();
            recalcNeeded = !_beginCalcStrategy.ToSkipRecal;
            cardsToStartCalc = null;
            cardsToStopCalc = null;

            // ����� ��� ��� ����������, ��� ����� ���� �������� ������� ���������, ������� ����� ���������� - ��� ����� ��� ����� ���������.
            // ��� ������� ����� ������ ������ ���� ���������.
            if (_beginCalcStrategy.IsStopCalcTFormsEvent(out var toStopCalc)) 
            {
                Logger.Verbose($"��������� ������� ������������ ��������� ���: {toStopCalc.MembersToString()}");
                cardsToStopCalc = toStopCalc.ToList();
            }

            if (_beginCalcStrategy.IsBeginCalcTFormsEvent(out var toStartCalc))
            {
                Logger.Verbose($"�������� ������� ������������ ��������� ���: {toStartCalc.MembersToString()}");
                cardsToStartCalc = toStartCalc.ToList();
            }

        }


        protected virtual void BeforeUpdate() { }
    }




    /// <summary>
    /// ���������, ������������, ����� ��������/����������� ������������ ������������ ��� ��������, � ����� - ��� ����� ��������� ��������.
    /// </summary>
    public interface IBeginCalcTFormsStartegy<Key>
    {
        bool IsBeginCalcTFormsEvent(out IEnumerable<Key> objects);
        bool IsStopCalcTFormsEvent(out IEnumerable<Key> objects);
        bool ToSkipRecal { get; }
    }



}