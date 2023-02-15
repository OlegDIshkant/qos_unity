using CommonTools.Math;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    internal class DeckTFormCalcer
    {
        private IEnumerable<Transform> _playersTransforms;

        public Transform? Result { get; private set; }

        public DeckTFormCalcer(IEnumerable<Transform> playersTransforms)
        {
            _playersTransforms = playersTransforms;
        }

        public Transform Calc()
        {
            Result = PlaceBetweenPlayers();
            return Result.Value;
        }

        private Transform PlaceBetweenPlayers()
        {
            var position =
                _playersTransforms
                .Select(t => t.Position)
                .FindPointBetween();
            return new Transform(position);
        }
    }
}
