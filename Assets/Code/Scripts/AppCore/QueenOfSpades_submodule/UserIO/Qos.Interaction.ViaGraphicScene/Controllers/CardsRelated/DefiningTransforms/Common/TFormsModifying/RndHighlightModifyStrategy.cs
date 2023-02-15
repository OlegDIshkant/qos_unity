using CommonTools;
using CommonTools.Math;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// Случайным образом переодически визуально выделяет те или иные карты.
    /// </summary>
    public class RndHighlightModifyStrategy : HighlightModifyStrategy<HighlightsData>
    {
        private readonly int MIN_INTERVAL = 100;
        private readonly int MAX_INTERVAL = 1000;

        private readonly Random _rnd = new Random();

        private int _timer;
        private CardId? _currHighlight;


        public RndHighlightModifyStrategy(TimeContext timeContext) : base(timeContext)
        {
        }


        protected override void OnBeforeModify(DictionaryData<CardId, Transform> transforms)
        {
            if (--_timer <= 0)
            {
                ChangeHighlightedCard(transforms.Items.Keys);
                _timer = NewTimeInterval();
            }
        }


        protected override void OnSameFrameUpdates(DictionaryData<CardId, Transform> transforms)
        {
            base.OnSameFrameUpdates(transforms);
            ForgetHighlightIfCardRemoved(transforms);
        }


        private void ForgetHighlightIfCardRemoved(DictionaryData<CardId, Transform> transforms)
        {
            if (_currHighlight != null && transforms.HasChanged && transforms.Removed.Contains(_currHighlight.Value))
            {
                SetHighlight(_currHighlight.Value, null);
                _currHighlight = null;
            }
        }


        private int NewTimeInterval()
        {
            return _rnd.Next(MIN_INTERVAL, MAX_INTERVAL);
        }


        private void ChangeHighlightedCard(IEnumerable<CardId> cardIds)
        {
            if (_currHighlight != null)
            {
                SetHighlight(_currHighlight.Value, null);
            }

            if (cardIds.Count() <= 0)
            {
                _currHighlight = null;
                return;
            }

            var rndIndex = _rnd.Next(cardIds.Count());
            _currHighlight = cardIds.Skip(rndIndex).First();
            SetHighlight(_currHighlight.Value, HighlightType.LIGHT);
        }


        protected override HighlightsData GetExtraData(CardId cardId) => new HighlightsData();
    }
}
