using CommonTools.Math;
using System;
using System.Numerics;
using static CommonTools.Math.NDCPointToPlaneSolver;

namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    internal static class CardsPlaneCalcer
    {
        public static void FindMatricies(CameraParams camParams, out Matrix4x4 modelMatrix, out Matrix4x4 inverseModelMatrix)
        {
            modelMatrix = CardsPlaneModelMatrix(camParams.inverseViewMatrix);
            if (!Matrix4x4.Invert(modelMatrix, out inverseModelMatrix))
            {
                throw new Exception();
            }
        }


        private static Matrix4x4 CardsPlaneModelMatrix(Matrix4x4 invViewMatrix)
        {
            var planeMMatrixInCamSpace = new Transform(new Vector3(0, 0, -2)).CalcTRS();
            return invViewMatrix * planeMMatrixInCamSpace;
        }
    }
}
