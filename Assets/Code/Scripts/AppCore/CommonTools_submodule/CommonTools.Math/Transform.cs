using System.Numerics;
using System.Runtime.CompilerServices;

namespace CommonTools.Math
{
    public struct Transform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public Vector3 Scale { get; set; }

        public static Transform Default => new Transform(Vector3.Zero, Quaternion.Identity, new Vector3(1, 1, 1));

        public Transform(Vector3 position)
        {
            Position = position;
            Rotation = Quaternion.Identity;
            Scale = new Vector3(1, 1, 1);
        }

        public Transform(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
            Scale = new Vector3(1, 1, 1);
        }

        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }


        public static Transform Lerp(Transform from, Transform to, float t)
        {
            var newTransform = new Transform();
            newTransform.Position = Vector3.Lerp(from.Position, to.Position, t);
            newTransform.Rotation = Quaternion.Lerp(from.Rotation, to.Rotation, t);
            newTransform.Scale = Vector3.Lerp(from.Scale, to.Scale, t);
            return newTransform;
        }

        public override string ToString()
        {
            return $"Transform: position '{Position}',   rotation '{Rotation}',   scale '{Scale}'";
        }

    }

}
