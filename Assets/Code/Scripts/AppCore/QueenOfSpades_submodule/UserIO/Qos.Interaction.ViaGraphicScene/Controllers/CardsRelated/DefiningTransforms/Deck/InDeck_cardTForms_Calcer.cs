using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using System.Linq;
using System.Numerics;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    internal class InDeck_cardTForms_Calcer : ITFormsCalcer
    {
        private IDeckController _deckController;

        private Transform DeckTForm => _deckController.DeckInfo.GetTransform();


        public InDeck_cardTForms_Calcer(IDeckController deckController)
        {
            _deckController = deckController;
        }


        public Transform[] Calc(int objectsAmmount) => Enumerable.Range(0, objectsAmmount).Select(i => TFormForCardWithIndex(i)).ToArray();


        private Transform TFormForCardWithIndex(int i) => new Transform(DeckTForm.Position + Height(i), GeometryUtils.FromVectors(Vector3.UnitZ, -Vector3.UnitY));


        private Vector3 Height(int i) => new Vector3(0, i * 0.015f, 0);
    }
}
