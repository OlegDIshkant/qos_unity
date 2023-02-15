using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated;
using System;


namespace Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects
{

    public interface ICardHeapController
    {
        ICardHeap_ReadOnly CardHeapInfo { get; }
    }

    public class CardHeapController : AbstractController, ICardHeapController
    {
        private readonly Func<ICardHeap> _CardHeapFactoryMethod;
        private readonly IPlayFieldController _playField;
        private readonly ICardsActionsProvider _cardsActionsProvider;

        private ICardHeap _cardHeap;
        private bool _playFieldWasInited = false;

        public ICardHeap_ReadOnly CardHeapInfo => _cardHeap;


        public CardHeapController(
            Contexts contexts, 
            Func<ICardHeap> CardHeapFactoryMethod,
            IPlayFieldController playField) : 
            base(contexts)
        {
            _playField = playField;
            _CardHeapFactoryMethod = CardHeapFactoryMethod;
        }


        public override void Update()
        {
            if (PlayFieldInited())
            {
                InitHeap();
            }
        }


        private bool PlayFieldInited()
        {
            if (!_playFieldWasInited)
            {
                if (_playField.PlayFieldInfo != null)
                {
                    _playFieldWasInited = true;
                    return true;
                }
            }
            return false;
        }


        private void InitHeap()
        {
            _cardHeap = _CardHeapFactoryMethod();

            var cardHeapPosition = _playField.PlayFieldInfo.CardHeapCenter;
            _cardHeap.SetTransform(new Transform(cardHeapPosition));
        }
    }

}