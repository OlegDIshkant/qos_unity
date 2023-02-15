using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using static CommonTools.Math.NDCPointToPlaneSolver;


namespace CommonTools.Math
{
    public static class GeometryUtils
    {
        public static Plane FromPointAndNormal(Vector3 norm, Vector3 point)
        {
            return new Plane(
                norm,
                -norm.X * point.X - norm.Y * point.Y - norm.Z * point.Z);
        }


        public static Plane PlaneX() => new Plane(Vector3.UnitX, 0);


        public static Plane PlaneY() => new Plane(Vector3.UnitY, 0);


        public static Plane PlaneZ() => new Plane(Vector3.UnitZ, 0);


        public static Quaternion InitialRotation(Vector3 lookDir, Vector3? up = null)
        {
            if (up == null)
            {
                up = Vector3.UnitY;
            }

            Vector3 vector = Vector3.Normalize(lookDir);
            Vector3 vector2 = Vector3.Normalize(Vector3.Cross(up.Value, vector));
            Vector3 vector3 = Vector3.Cross(vector, vector2);
            var m00 = vector2.X;
            var m01 = vector2.Y;
            var m02 = vector2.Z;
            var m10 = vector3.X;
            var m11 = vector3.Y;
            var m12 = vector3.Z;
            var m20 = vector.X;
            var m21 = vector.Y;
            var m22 = vector.Z;


            float num8 = (m00 + m11) + m22;
            var quaternion = new Quaternion();
            if (num8 > 0f)
            {
                var num = (float)System.Math.Sqrt(num8 + 1f);
                quaternion.W = num * 0.5f;
                num = 0.5f / num;
                quaternion.X = (m12 - m21) * num;
                quaternion.Y = (m20 - m02) * num;
                quaternion.Z = (m01 - m10) * num;
                return quaternion;
            }
            if ((m00 >= m11) && (m00 >= m22))
            {
                var num7 = (float)System.Math.Sqrt(((1f + m00) - m11) - m22);
                var num4 = 0.5f / num7;
                quaternion.X = 0.5f * num7;
                quaternion.Y = (m01 + m10) * num4;
                quaternion.Z = (m02 + m20) * num4;
                quaternion.W = (m12 - m21) * num4;
                return quaternion;
            }
            if (m11 > m22)
            {
                var num6 = (float)System.Math.Sqrt(((1f + m11) - m00) - m22);
                var num3 = 0.5f / num6;
                quaternion.X = (m10 + m01) * num3;
                quaternion.Y = 0.5f * num6;
                quaternion.Z = (m21 + m12) * num3;
                quaternion.W = (m20 - m02) * num3;
                return quaternion;
            }
            var num5 = (float)System.Math.Sqrt(((1f + m22) - m00) - m11);
            var num2 = 0.5f / num5;
            quaternion.X = (m20 + m02) * num2;
            quaternion.Y = (m21 + m12) * num2;
            quaternion.Z = 0.5f * num5;
            quaternion.W = (m01 - m10) * num2;
            return quaternion;
        }


        public static Quaternion FromVectors(Vector3 fromDir, Vector3 toDir)
        {
            float k_cos_theta = Vector3.Dot(fromDir, toDir);
            float k = (float)System.Math.Sqrt(fromDir.LengthSquared() * toDir.LengthSquared());

            if (k_cos_theta / k == -1)
            {
                // 180 degree rotation around any orthogonal vector
                return new Quaternion(Vector3.Normalize(fromDir.SomeOrthogonal()), 0);
            }

            return Quaternion.Normalize(new Quaternion(Vector3.Cross(fromDir, toDir), k_cos_theta + k));
        }


        public static Vector3 SomeOrthogonal(this Vector3 v)
        {
            float x = System.Math.Abs(v.X);
            float y = System.Math.Abs(v.Y);
            float z = System.Math.Abs(v.Z);

            Vector3 other = x < y ? 
                (x < z ? Vector3.UnitX : Vector3.UnitZ) : 
                (y < z ? Vector3.UnitY : Vector3.UnitZ);
            return Vector3.Cross(v, other);
        }


        public static Vector3 ToVector3(this Vector4 original)
        {
            return new Vector3(original.X, original.Y, original.Z);
        }


        public static Vector2 ToVector2(this Vector3 original)
        {
            return new Vector2(original.X, original.Y);
        }


        public static Vector3 FindPointBetween(this IEnumerable<Vector3> points)
        {
            return
                points.Aggregate((argregatedP, p) => argregatedP + p)
                / 
                points.Count();
        }


        public static void ChangeTowards(this ref Vector3 vector, Vector3 target, float changeSpeed)
        {
            vector = ChangeTowards(vector, target, changeSpeed);
        }


        public static Vector3 ChangeTowards(Vector3 vector, Vector3 target, float changeSpeed)
        {
            if (vector != target)
            {
                var dir = target - vector;
                var delta = Vector3.Normalize(dir) * changeSpeed;

                return
                    delta.Length() > dir.Length() ?
                    target :
                    vector + delta;
            }
            return target;
        }


        public static Quaternion ChangeTowards(Quaternion quaternion, Quaternion target, float changeSpeed)
        {
            if (quaternion != target)
            {
                var dir = target - quaternion;
                var delta = Quaternion.Normalize(dir) * changeSpeed;

                return 
                    delta.Length() > dir.Length() ?
                    target :
                    quaternion + delta;
            }
            return target;
        }


        public static void ChangeTowards(this ref Quaternion quaternion, Quaternion target, float changeSpeed)
        {
            quaternion = ChangeTowards(quaternion, target, changeSpeed);
        }


        public static Quaternion InvertAngle(this Quaternion quaternion)
        {
            return new Quaternion(
                quaternion.X,
                quaternion.Y,
                quaternion.Z,
                -quaternion.W);
        }


        public static Vector4 ApplyTo(this Matrix4x4 matrix, Vector4 vector)
        {
            return Vector4.Transform(
                vector,
                Matrix4x4.Transpose(matrix));
        }


        public static Vector3 WorldToNDC(this Vector3 point, CameraParams camParams)
        {
            var camPoint = camParams.ViewMatrix().ApplyTo(new Vector4(point, 1));
            var projPoint = camParams.ProjMatrix().ApplyTo(camPoint);
            return new Vector3(
                projPoint.X / projPoint.W,
                projPoint.Y / projPoint.W,
                projPoint.Z / projPoint.W);
        }
    }
}
