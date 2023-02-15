using CommonTools.Math;
using Qos.Domain.Entities;
using System.Collections.Generic;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using CommonTools;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// ������������ ��������� ���� ���, ����� ��������� �������� ��������� �� ���.
    /// </summary>
    /// <typeparam name="ExtraData"></typeparam>
    public abstract class HighlightModifyStrategy<ExtraData> : OncePerFrameModifyStrategy<CardId, ExtraData>
        where ExtraData : struct, IHighlightsData
    {
        private Dictionary<CardId, HighlightType> _highlights = new Dictionary<CardId, HighlightType>();


        private DictionaryData<CardId, ExtraData>.Editable _extraDataEdit;
        private DictionaryData<CardId, ExtraData> _extraData;
        public override sealed DictionaryData<CardId, ExtraData> DictionaryOutput => _extraData;


        protected abstract ExtraData GetExtraData(CardId cardId);


        protected HighlightModifyStrategy(TimeContext timeContext) : base(timeContext)
        {
            _extraData = new DictionaryData<CardId, ExtraData>(out _extraDataEdit);
        }


        protected override sealed IDictionary<CardId, Transform> ModifyOncePerFrame(DictionaryData<CardId, Transform> transforms)
        {
            ForgetRemovedTForms(transforms);
            OnBeforeModify(transforms);

            var result = new Dictionary<CardId, Transform>();
            foreach (var highligth in _highlights)
            {
                if (transforms.Items.TryGetValue(highligth.Key, out var originalTForm))
                {
                    result.Add(highligth.Key, Highlight(originalTForm, highligth.Value));
                }
                else
                {
                    Logger.Error($"��� ��������� ������� ��� ����� '{highligth.Key}', ���� ��� ��� ����� ���� ������.");
                }
            }
            return result;
        }


        protected override void OnSameFrameUpdates(DictionaryData<CardId, Transform> transforms)
        {
            base.OnSameFrameUpdates(transforms);
            ForgetRemovedTForms(transforms);
        }


        private void ForgetRemovedTForms(DictionaryData<CardId, Transform> transforms)
        {
            if (transforms.HasChanged)
            {
                foreach (var removed in transforms.Removed)
                {
                    SetHighlight(removed, null);
                }
            }
        }


        /// <summary>
        /// WARNING! �� ����������� <paramref name="transforms"/>.
        /// </summary>
        protected abstract void OnBeforeModify(DictionaryData<CardId, Transform> transforms);


        private Transform Highlight(Transform tForm, HighlightType highlightType)
        {
            var scaleFactor = highlightType switch
            {
                HighlightType.LIGHT => 1.2f,
                HighlightType.HEAVY => 1.4f,
                _ => throw new System.Exception()
            };
            tForm.Scale = tForm.Scale * scaleFactor;
            return tForm;
        }


        /// <summary>
        /// ����� ������ �����, ������� ������ ������ ��������� ��� ��� ��� ���� �����.
        /// </summary>
        protected void SetHighlight(CardId cardId, HighlightType? highlightType)
        {
            if (highlightType == null)
            {
                Logger.Verbose($"������� ��������� � ����� '{cardId}' � �������� ��� ��.");
                _highlights.Remove(cardId);
                _extraDataEdit.RemoveItem(cardId);
            }
            else
            {
                Logger.Verbose($"������ ��������� ��� ����� '{cardId}' �� '{highlightType.Value}'.");
                _highlights[cardId] = highlightType.Value;

                var extraData = GetExtraData(cardId);
                extraData.Highlight = highlightType.Value;
                _extraDataEdit.SetItem(cardId, extraData);
            }
        }


        /// <summary>
        /// ����� ������ �����, ������� ������ ������, ����� ��������� ������ � ��� ��� ���� �����.
        /// </summary>
        protected HighlightType? GetHighlight(CardId cardId)
        {
            if (_highlights.TryGetValue(cardId, out var highlight))
            {
                return highlight;
            }
            return null;
        }
    }
}
