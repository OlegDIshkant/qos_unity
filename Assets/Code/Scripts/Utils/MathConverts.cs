using CommonTools.Math;
using System.Numerics;


namespace Utils
{
    internal static class MathConverts
    {
        public static Vector3 ToVec3(this UnityEngine.Vector3 original)
        {
            return new Vector3(original.x, original.y, original.z);
        }


        public static Vector2 ToVec2(this UnityEngine.Vector2 original)
        {
            return new Vector2(original.x, original.y);
        }


        public static UnityEngine.Vector3 ToUnityVec3(this Vector3 original)
        {
            return new UnityEngine.Vector3(original.X, original.Y, original.Z);
        }



        public static UnityEngine.Vector2 ToUnityVec2(this Vector2 original)
        {
            return new UnityEngine.Vector2(original.X, original.Y);
        }


        public static Quaternion ToQuaternion(this UnityEngine.Quaternion original)
        {
            return new Quaternion(original.x, original.y, original.z, original.w);
        }


        public static UnityEngine.Quaternion ToUnityQuaternion(this Quaternion original)
        {
            return new UnityEngine.Quaternion(original.X, original.Y, original.Z, original.W);
        }




        public static UnityEngine.Matrix4x4 ToUnity(this Matrix4x4 original)
        {
            var result = new UnityEngine.Matrix4x4();
            result.SetRow(0, new UnityEngine.Vector4(original.M11, original.M12, original.M13, original.M14));
            result.SetRow(1, new UnityEngine.Vector4(original.M21, original.M22, original.M23, original.M24));
            result.SetRow(2, new UnityEngine.Vector4(original.M31, original.M32, original.M33, original.M34));
            result.SetRow(3, new UnityEngine.Vector4(original.M41, original.M42, original.M43, original.M44));
            return result;
        }


        public static Matrix4x4 FromUnity(this UnityEngine.Matrix4x4 original)
        {
            var result = new Matrix4x4(
                original.m00, original.m01, original.m02, original.m03,
                original.m10, original.m11, original.m12, original.m13,
                original.m20, original.m21, original.m22, original.m23,
                original.m30, original.m31, original.m32, original.m33);
            return result;
        }


        public static Transform FromUnity(this UnityEngine.Transform original)
        {
            var position = original.position.ToVec3();
            var rotation = original.rotation.ToQuaternion();
            var scale = original.localScale.ToVec3();

            return new Transform()
            {
                Position = position,
                Rotation = rotation,
                Scale = scale
            };
        }


        public static void ToUnity(this UnityEngine.Transform unityTransf, Transform transf)
        {
            unityTransf.position = transf.Position.ToUnityVec3();
            unityTransf.rotation = transf.Rotation.ToUnityQuaternion();
            unityTransf.localScale = transf.Scale.ToUnityVec3();
        }
    }
}
