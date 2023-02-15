using System.Numerics;


namespace CommonTools.Math
{
    public static class TransformExtensions
    {
        public static void ChangePosition(this ref Transform transform, Vector3 newPosition)
        {
            var newTransform = new Transform();
            newTransform.Position = newPosition;
            newTransform.Rotation = transform.Rotation;
            newTransform.Scale = transform.Scale;
            transform = newTransform;
        }


        public static void Towards(this ref Transform transform, Transform target, float moveSpeed, float rotationSpeed, float scaleSpeed)
        {
            transform.MoveTowards(target, moveSpeed);
            transform.RotateTowards(target, rotationSpeed);
            transform.ScaleTowards(target, scaleSpeed);
        }


        public static void MoveTowards(this ref Transform transform, Transform target, float moveSpeed)
        {
            transform.Position = GeometryUtils.ChangeTowards(transform.Position, target.Position, moveSpeed);
        }


        public static void RotateTowards(this ref Transform transform, Transform target, float rotationSpeed)
        {
            transform.Rotation = GeometryUtils.ChangeTowards(transform.Rotation, target.Rotation, rotationSpeed);
        }


        public static void ScaleTowards(this ref Transform transform, Transform target, float scaleSpeed)
        {
            transform.Scale = GeometryUtils.ChangeTowards(transform.Scale, target.Scale, scaleSpeed);
        }


        public static Matrix4x4 CalcTRS(this Transform transform)
        {
            return
                GetTranslationMatrix(transform.Position) *
                GetRotationMatrix(transform.Rotation) *
                GetScaleMatrix(transform.Scale);
        }


        public static Matrix4x4 GetTranslationMatrix(Vector3 position)
        {
            return new Matrix4x4(
                1, 0, 0, position.X,
                0, 1, 0, position.Y,
                0, 0, 1, position.Z,
                0, 0, 0, 1);
        }

        public static Matrix4x4 GetRotationMatrix(Quaternion q)
        {
            var _2xx = 2 * q.X * q.X;
            var _2yy = 2 * q.Y * q.Y;
            var _2zz = 2 * q.Z * q.Z;
            var _2xy = 2 * q.X * q.Y;
            var _2wx = 2 * q.W * q.X;
            var _2wy = 2 * q.W * q.Y;
            var _2wz = 2 * q.W * q.Z;
            var _2xz = 2 * q.X * q.Z;
            var _2yz = 2 * q.Y * q.Z;


            return new Matrix4x4(
                1 - _2yy - _2zz, _2xy - _2wz, _2xz + _2wy, 0,
                _2xy + _2wz, 1 - _2xx - _2zz, _2yz - _2wx, 0,
                _2xz - _2wy, _2yz + _2wx, 1 - _2xx - _2yy, 0,
                0, 0, 0, 1);
        }


        public static Matrix4x4 GetScaleMatrix(Vector3 scale)
        {
            return new Matrix4x4(
                scale.X, 0, 0, 0,
                0, scale.Y, 0, 0,
                0, 0, scale.Z, 0,
                0, 0, 0, 1);
        }


        public static Vector3 ExtractPosition(this Matrix4x4 m)
        {
            return new Vector3(m.M14, m.M24, m.M34);
        }


        public static Quaternion ExtractRotation(this Matrix4x4 m)
        {
            return GeometryUtils.InitialRotation(
                new Vector3(m.M13, m.M23, m.M33),
                new Vector3(m.M12, m.M22, m.M32));
        }


        public static Vector3 ExtractScale(this Matrix4x4 m)
        {
            return new Vector3(
                new Vector3(m.M11, m.M21, m.M31).Length(),
                new Vector3(m.M12, m.M22, m.M32).Length(),
                new Vector3(m.M13, m.M23, m.M33).Length());
        }



    }
}
