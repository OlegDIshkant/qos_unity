using System.Numerics;

namespace CommonTools.Math.Geometry3D
{
    /// <summary>
    /// Поиск пересечения плоскости и луча.
    /// </summary>
    public static class LinePlaneIntersectSolver
    {
        public static bool TrySolve(Line line, Plane plane, out Vector3 intersection)
        {
            // T * u = v
            // =>
            // u = T_inversed * v

            var (planeA, planeB) = line.AsIntersectingPlanes();
            

            var T = new Matrix4x4(
                plane.Normal.X, plane.Normal.Y, plane.Normal.Z, 0,  
                planeA.Normal.X, planeA.Normal.Y, planeA.Normal.Z, 0,
                planeB.Normal.X, planeB.Normal.Y, planeB.Normal.Z, 0,  
                0, 0, 0, 1
                );

            var v = new Vector4(
                -plane.D,                                                       
                -planeA.D,     
                -planeB.D,          
                0);


            if (!Matrix4x4.Invert(T, out var T_inversed))
            {
                intersection = default;
                return false;
            }

            var u = Vector4.Transform(v, Matrix4x4.Transpose(T_inversed));
            intersection = new Vector3(u.X, u.Y, u.Z);
            return true;
        }
    }
}
