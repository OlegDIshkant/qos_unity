using System.Numerics;

namespace CommonTools.Math.Geometry3D
{
    public static class LineExtensions
    {
        /// <summary>
        /// Представляет линию в трехмерном пространстве как 2 пересекающиеся плоскости.
        /// </summary>
        public static (Plane planeA, Plane planeB) AsIntersectingPlanes(this Line line)
        {
            var planeA =
                new Plane(
                    line.NormDirection.Y,
                    -line.NormDirection.X,
                    0,
                    line.Center.Y * line.NormDirection.X - line.Center.X * line.NormDirection.Y);


            var planeB =
                new Plane(
                    0,
                    -line.NormDirection.Z,
                     line.NormDirection.Y,
                    line.Center.Y * line.NormDirection.Z - line.Center.Z * line.NormDirection.Y);

            return (planeA, planeB);
        }


        /// <summary>
        /// Переносит линию в локаьную систему координат.
        /// </summary>
        public static Line ToLocalSpace(this Line line, Matrix4x4 worldToLocMatrix)
        {
            var transpWorldToLocMatrix = Matrix4x4.Transpose(worldToLocMatrix);
            return new Line(
                center: Vector4.Transform(new Vector4(line.Center, 1), transpWorldToLocMatrix).ToVector3(),
                dir: Vector4.Transform(new Vector4(line.NormDirection, 0), transpWorldToLocMatrix).ToVector3()
                );
        }


        /// <summary>
        /// Переносит линию в локаьную систему координат, используя транспонированную матрицу.
        /// </summary>
        public static Line ToLocalSpace_Transpose(this Line line, Matrix4x4 transpWorldToLocMatrix)
        {
            return new Line(
                center: Vector4.Transform(new Vector4(line.Center, 1), transpWorldToLocMatrix).ToVector3(),
                dir: Vector4.Transform(new Vector4(line.NormDirection, 0), transpWorldToLocMatrix).ToVector3()
                );
        }
    }
}
