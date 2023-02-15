using System.Numerics;


namespace CommonTools.Math.Geometry3D
{
    public struct Line
    {
        public Vector3 Center { get; set; }

        private Vector3 _normDirection;
        public Vector3 NormDirection
        {
            get => _normDirection;
            set => _normDirection = Vector3.Normalize(value);
        }


        public Line(Vector3 center, Vector3 dir)
        {
            Center = center;
            _normDirection =  Vector3.Normalize(dir);
        }
    }
}
