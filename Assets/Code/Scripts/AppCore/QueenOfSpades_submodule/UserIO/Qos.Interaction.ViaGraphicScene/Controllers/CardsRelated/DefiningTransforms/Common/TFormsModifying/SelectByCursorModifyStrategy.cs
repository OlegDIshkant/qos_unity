using CommonTools;
using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using Qos.Interaction.ViaGraphicScene.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// ��������� ���������� �������������� ���� � �������� ������������, 
    /// � � ��� ����� ��������� �������� �� ��������.
    /// </summary>
    public sealed class SelectByCursorModifyStrategy : HighlightModifyStrategy<HighlightsData>
    {
        private readonly ICardsRaycaster _cardsRaycaster;
        private readonly ISelectCardsStartegy _selectCardsStrategy;
        private readonly ICursorController _cursorController;
        private readonly ICameraController _cameraController;
        private readonly HighlightSettings _highlightSettings;

        private CardId? _overlaidCardId;


        public SelectByCursorModifyStrategy(
            TimeContext timeContext,
            ICameraController cameraController,
            ICursorController cursorController,
            ICardsRaycaster cardsRaycaster, 
            ISelectCardsStartegy selectCardsStrategy,
            HighlightSettings highlightSettings) :
            base(timeContext)
        {
            _cameraController = cameraController;
            _cursorController = cursorController;
            _cardsRaycaster = cardsRaycaster;
            _selectCardsStrategy = selectCardsStrategy;
            _highlightSettings = highlightSettings; 
        }


        protected override void OnBeforeModify(DictionaryData<CardId, Transform> transforms)
        {
            _selectCardsStrategy.Update();
            UpdateSelections(transforms);
        }


        private void UpdateSelections(DictionaryData<CardId, Transform> transforms)
        {
            if (_cardsRaycaster.HasIntersection(transforms.Items, _cursorController.CursorInfo.NdcPosition, _cameraController.CameraInfo.Params, out var intersected))
            {
                Logger.Verbose($"������ ��������� ��� ������ '{intersected}'.");
                if (IsSelectedNow(intersected))
                {
                    Logger.Verbose($"����� '{intersected}' ��� ���� ������� �����.");
                    if (ShouldBeDeSelected(intersected))
                    {
                        Logger.Verbose($"������� ����� � ����� '{intersected}'.");
                        SetOverlaidByCursor(intersected);
                    }
                }
                else
                {
                    Logger.Verbose($"����� '{intersected}' �� ���� ������� �����.");
                    if (ShouldBeSelected(intersected))
                    {
                        Logger.Verbose($"�������� ����� '{intersected}'.");
                        SetOverlaidByCursor(null); // �� ��������� �����, ������� ������� 'cursor over card' ��������� � �����
                        SetSelected(intersected);
                    }
                    else if (!IsOvelaid(intersected))
                    {
                        Logger.Verbose($"������ �������� ����� '{intersected}'.");
                        SetOverlaidByCursor(intersected);
                    }

                }
            }
            else
            {
                Logger.Verbose($"������ �� ��������� ��� �����-���� ������.");
                SetOverlaidByCursor(null);
            }
        }


        private bool ShouldBeDeSelected(CardId cardId) => _cursorController.CursorInfo.LeftClick && _selectCardsStrategy.CanDeSelect(cardId);


        private bool ShouldBeSelected(CardId cardId) => _cursorController.CursorInfo.LeftClick && _selectCardsStrategy.CanSelect(cardId);


        private bool IsSelectedNow(CardId cardId) => GetHighlight(cardId) == _highlightSettings.SelectHighlight;


        private void SetSelected(CardId cardId) => SetHighlight(cardId, _highlightSettings.SelectHighlight);


        private void SetDeselected(CardId cardId) => SetHighlight(cardId, null);

        private bool IsOvelaid(CardId cardId) => _overlaidCardId != null && _overlaidCardId.Value.Equals(cardId);


        private void SetOverlaidByCursor(CardId? cardId)
        {
            if (_overlaidCardId != null)
            {
                SetDeselected(_overlaidCardId.Value);
            }
            _overlaidCardId = cardId;

            if (cardId != null)
            {
                SetHighlight(_overlaidCardId.Value, _highlightSettings.OverlayHighlight);
            }
        }

        protected override HighlightsData GetExtraData(CardId cardId) => new HighlightsData();
    }


    public interface ISelectCardsStartegy
    {
        bool CanSelect(CardId cardId);
        bool CanDeSelect(CardId cardId);
        public void Update();
    }



    public class NeverSelectCards : ISelectCardsStartegy
    {
        public bool CanDeSelect(CardId cardId) => throw new System.NotSupportedException();

        public bool CanSelect(CardId cardId) => false;

        public void Update() { }
    }



    public class FreelySelectCards : ISelectCardsStartegy
    {
        public bool CanDeSelect(CardId cardId) => true;

        public bool CanSelect(CardId cardId) => true;

        public void Update() { }
    }



    public class SelectCardJustOnce : ISelectCardsStartegy
    {
        private bool _wasSelected = false;

        public bool CanDeSelect(CardId cardId) => false;

        public bool CanSelect(CardId cardId)
        {
            if (!_wasSelected)
            {
                _wasSelected = true;
                return true;
            }
            return false;
        }


        public virtual void Update() { }


        protected void Reset()
        {
            _wasSelected = false;
        }
    }



    public class SelectCardJustOncePerTransferChoice : SelectCardJustOnce
    {
        private readonly ISingleDataProvider<TransferProcessInfo> _processInfoProvider;

        private bool _waitingForProcessStarted = true;

        public SelectCardJustOncePerTransferChoice(ISingleDataProvider<TransferProcessInfo> processInfoProvider)
        {
            _processInfoProvider = processInfoProvider;
        }

        public override void Update()
        {
            base.Update();

            if (_waitingForProcessStarted)
            {
                if (TransferProcessStarted())
                {
                    LetUserSelectCard();
                    _waitingForProcessStarted = false;
                }
            }
            else if(TransferProcessFinished())
            {
                _waitingForProcessStarted = true;
            }
        }


        private bool TransferProcessStarted()
        {
            return _processInfoProvider.SingleOutput.HasChanged  && _processInfoProvider.SingleOutput.Value != null;
        }


        private bool TransferProcessFinished()
        {
            return _processInfoProvider.SingleOutput.HasChanged && _processInfoProvider.SingleOutput.Value == null;
        }


        private void LetUserSelectCard() => Reset();
    }
}
