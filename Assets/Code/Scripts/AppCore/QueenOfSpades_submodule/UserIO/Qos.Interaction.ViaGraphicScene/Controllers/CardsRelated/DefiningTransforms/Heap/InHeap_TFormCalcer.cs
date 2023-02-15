using CommonTools.Math;
using Qos.Interaction.ViaGraphicScene.Controllers.SceneObjects;
using System;
using System.Numerics;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    public interface IInHeap_TFormCalcer
    {
        Transform CalcNew(ICardHeapController heapController);
    }


    public class InHeap_TFormCalcer : IInHeap_TFormCalcer
    {
        private static readonly float PLACEMENT_RANGE = 1f;
        private static readonly float HALF_PLACEMENT_RANGE = PLACEMENT_RANGE / 2;

        private readonly Random _rnd = new Random();



        public Transform CalcNew(ICardHeapController heapController)
        {
            var locPosition = new Vector4 (RndLocPointOnYPlane(), 1);
            var locDirection = new Vector4 (- Vector3.UnitY, 0);
            var locUp = new Vector4 (- Vector3.UnitX, 0);

            var locToWorldMatrix = heapController.CardHeapInfo.GetTransform().CalcTRS();

            return new Transform(
                position: locToWorldMatrix.ApplyTo(locPosition).ToVector3(),
                rotation: GeometryUtils.InitialRotation(locToWorldMatrix.ApplyTo(locDirection).ToVector3(), locToWorldMatrix.ApplyTo(locUp).ToVector3()));

        }


        private Vector3 RndLocPointOnYPlane()
        {
            return new Vector3(RndValue(), 0, RndValue()); 
        }


        private float RndValue() => -HALF_PLACEMENT_RANGE + (float)_rnd.NextDouble() * PLACEMENT_RANGE;
    }
}