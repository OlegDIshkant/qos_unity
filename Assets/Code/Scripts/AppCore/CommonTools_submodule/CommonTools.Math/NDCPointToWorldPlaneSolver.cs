using CommonTools.Math.Geometry3D;
using System.Numerics;

namespace CommonTools.Math
{
    /// <summary>
    /// Проецирует точку (NDC) (z = -1) на плоскость (мировая система координат или локальная относительно мира) 
    /// и получает новую точку на этой плоскости (мировая система координат или локальная относительно мира).
    /// </summary>
    public static class NDCPointToPlaneSolver
    {
        /// <param name="inverseProjMatrix">
        /// WARNING! Нижняя строка матрицы должна быть в виде "0  0  -1  0".
        /// </param>
        public static bool TrySolveForWorldPlane(Plane worldPlane, Vector2 ndcPoint, CameraParams camParams, out Vector3 projectionPoint)
        {
            var worldLine = NdcPointToWorldLine(ndcPoint, camParams);

            return LinePlaneIntersectSolver.TrySolve(worldLine, worldPlane, out projectionPoint);
        }


        /// <param name="inverseProjMatrix">
        /// WARNING! Нижняя строка матрицы должна быть в виде "0  0  -1  0".
        /// </param>
        public static bool TrySolveForLocalPlane(Matrix4x4 worldToLocMatrix, Plane localPlane, Vector2 ndcPoint, CameraParams camParams, out Vector3 projectionPoint)
        {
            var localLine = NdcPointToWorldLine(ndcPoint, camParams).ToLocalSpace(worldToLocMatrix);

            return LinePlaneIntersectSolver.TrySolve(localLine, localPlane, out projectionPoint);
        }


        private static Line NdcPointToWorldLine(Vector2 ndcPoint, CameraParams camParams)
        {
            var projRayPoint = NdcToProjSpace(ndcPoint, camParams.nearPlaneDist);
            var camRayPoint = Vector4.Transform(projRayPoint, Matrix4x4.Transpose(camParams.inverseProjMatrix));
            var rayPoint = Vector4.Transform(camRayPoint, Matrix4x4.Transpose(camParams.inverseViewMatrix)).ToVector3();

            var rayOrigin = new Vector3(        //camera position
                camParams.inverseViewMatrix.M14,
                camParams.inverseViewMatrix.M24,
                camParams.inverseViewMatrix.M34);
            var rayDir = rayPoint - rayOrigin;

            return new Line(rayOrigin, rayDir);
        }


        private static Vector4 NdcToProjSpace(Vector2 ndcPoint, float nearClipSpace)
        {
            var w = nearClipSpace;
            return new Vector4(ndcPoint.X, ndcPoint.Y, -1, 1) * w;
        }


        public struct CameraParams
        {
            public Matrix4x4 inverseProjMatrix;
            public Matrix4x4 inverseViewMatrix;
            public float nearPlaneDist;

            public Matrix4x4 ViewMatrix()
            {
                if (!Matrix4x4.Invert(inverseViewMatrix, out var result)) throw new System.Exception();
                return result;
            }

            public Matrix4x4 ProjMatrix()
            {
                if (!Matrix4x4.Invert(inverseProjMatrix, out var result)) throw new System.Exception();
                return result;
            }
        }
    }
}
